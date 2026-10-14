namespace NScript.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Net.WebSockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using NScript.Converter;
    using NScript.JST;
    using NScript.Utils;

    /// <summary>
    /// The build service's live patch socket: <c>ws://127.0.0.1:port/nscript-hmr</c>. A page's
    /// baked client (<see cref="HmrClient"/>) connects with the daemon's token, its bundle id
    /// and the <c>gen:seq</c> it holds; the server sends the patches since then, and each new
    /// one as its build writes it. Read-only: frames from pages are ignored. Records live in
    /// memory per bundle and generation; a full emit starts a new generation and asks open
    /// pages to reload.
    /// </summary>
    public sealed class HmrServer : IDisposable
    {
        /// <summary>Pages connected at once, over all bundles.</summary>
        public const int MaxClients = 16;

        /// <summary>Messages waiting for one page; a page that falls further behind is dropped and checks its script.</summary>
        private const int QueueLimit = 64;

        private const string PathName = "/nscript-hmr";

        /// <summary>A page's id, gen and file name reach watch.log, so they hold no line break.</summary>
        private static readonly Regex Plain = new Regex(@"^[\w.\-]*$");

        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>Origins a page may also connect from, besides loopback ones (<see cref="AllowedOrigin"/>).</summary>
        private static readonly string[] ExtraOrigins = (Environment.GetEnvironmentVariable("NSCRIPT_HMR_ORIGINS") ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        private readonly HttpListener listener;
        private readonly byte[] token;
        private readonly string host;
        private readonly Action<string> watchLog;
        private readonly Serilog.ILogger log = CompilerLog.ForComponent("Hmr");
        private readonly object gate = new object();
        private readonly Dictionary<string, Bundle> bundles = new Dictionary<string, Bundle>();

        /// <summary>The notice each bundle's pages show now, by bundle id (<see cref="Note"/>).</summary>
        private readonly Dictionary<string, string> notes = new Dictionary<string, string>();
        private readonly List<Client> clients = new List<Client>();
        private int opening;
        private readonly CancellationTokenSource stopping = new CancellationTokenSource();

        private HmrServer(HttpListener listener, int port, string token, Action<string> watchLog)
        {
            this.listener = listener;
            this.token = Encoding.ASCII.GetBytes(token);
            this.watchLog = watchLog;
            this.host = "127.0.0.1:" + port;
            this.Endpoint = new HmrEndpoint("ws://" + this.host + PathName, token);
            _ = Task.Run(this.AcceptLoop);
        }

        /// <summary>The url and token dev bundles bake.</summary>
        public HmrEndpoint Endpoint { get; }

        /// <summary>The url for <c>--status</c>, without the token.</summary>
        public string Url => this.Endpoint.Url;

        /// <summary>Pages connected now.</summary>
        public int ClientCount
        {
            get
            {
                lock (this.gate)
                {
                    return this.clients.Count;
                }
            }
        }

        /// <summary>
        /// Listens on a free loopback port with a new 128-bit token. HttpListener takes no port
        /// 0, so a free port is picked first; a port taken in between is tried again, 3 times.
        /// </summary>
        /// <param name="watchLog">Writes one line to watch.log.</param>
        public static HmrServer Start(Action<string> watchLog)
        {
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
            for (int attempt = 1; ; attempt++)
            {
                var probe = new TcpListener(IPAddress.Loopback, 0);
                probe.Start();
                int port = ((IPEndPoint)probe.LocalEndpoint).Port;
                probe.Stop();

                // 127.0.0.1, never localhost. The listener takes any Host header on this
                // address, so Serve checks it (a DNS-rebinding page names its own host).
                var listener = new HttpListener();
                listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
                try
                {
                    listener.Start();
                }
                catch (HttpListenerException) when (attempt < 3)
                {
                    listener.Close();
                    continue;
                }

                var server = new HmrServer(listener, port, token, watchLog);
                server.log.Information("HmrListening Url={Url}", server.Url);
                watchLog("hmr listening " + server.Url);
                return server;
            }
        }

        /// <summary>
        /// Sends <paramref name="live"/> to the pages with <paramref name="jsFileName"/>'s
        /// bundle open and keeps it for pages that connect later. Only enqueues: the build
        /// holds the sessions' lock while it runs.
        /// </summary>
        public void Publish(string jsFileName, LivePatch live)
        {
            var id = HmrClient.BundleId(jsFileName);
            var file = Path.GetFileName(jsFileName);
            lock (this.gate)
            {
                // A good build ends the notice: the page drops it when it takes the patch.
                this.notes.Remove(id);
                if (live.Gen == null || live.Seq == 0)
                {
                    if (live.Gen != null)
                    {
                        this.bundles[id] = new Bundle(live.Gen);
                    }

                    int sent = this.Broadcast(id, Serialize(new Message { Gen = live.Gen ?? string.Empty, Reload = live.Reload }));
                    this.log.Information("HmrReload File={File} Gen={Gen} Reason={Reason} Clients={Clients}", file, live.Gen, live.Reload, sent);
                    this.watchLog("hmr reload " + file + " " + live.Reload + " clients=" + sent);
                    return;
                }

                if (!this.bundles.TryGetValue(id, out var bundle) || bundle.Gen != live.Gen)
                {
                    this.bundles[id] = bundle = new Bundle(live.Gen);
                }

                var record = new Record
                {
                    Seq = live.Seq,
                    Js = live.Js,
                    Methods = live.Methods,
                    Reload = live.Reload,
                    Notes = live.Notes.Count > 0 ? live.Notes : null,
                };
                bundle.Records.Add(record);
                int clients = this.Broadcast(id, Serialize(new Message { Gen = live.Gen, Records = new[] { record } }));
                this.log.Information(
                    "HmrSend File={File} Gen={Gen} From={From} To={To} Reload={Reload} Clients={Clients}",
                    file,
                    live.Gen,
                    live.Seq,
                    live.Seq,
                    live.Reload,
                    clients);
                this.watchLog("hmr send " + file + " " + live.Gen + ":" + live.Seq + ".." + live.Seq + (live.Reload != null ? " reload " + live.Reload : string.Empty) + " clients=" + clients);
            }
        }

        /// <summary>
        /// Tells the pages with <paramref name="jsFileName"/>'s bundle open why their code is
        /// not current (a red build, a project that needs <c>dotnet build</c>), and keeps it
        /// for pages that connect later. Null ends the notice; a page that showed one says so.
        /// Sends only on a change.
        /// </summary>
        public void Note(string jsFileName, string? note)
        {
            var id = HmrClient.BundleId(jsFileName);
            var file = Path.GetFileName(jsFileName);
            lock (this.gate)
            {
                this.notes.TryGetValue(id, out var before);
                if (before == note)
                {
                    return;
                }

                if (note == null)
                {
                    this.notes.Remove(id);
                }
                else
                {
                    this.notes[id] = note;
                }

                int sent = this.Broadcast(id, Serialize(new Notice { Note = note ?? string.Empty }));
                this.log.Information("HmrNote File={File} Note={Note} Clients={Clients}", file, note, sent);
                this.watchLog("hmr note " + file + " " + (note ?? "cleared") + " clients=" + sent);
            }
        }

        /// <summary>Stops listening and drops every page without a close handshake: pages then check their script.</summary>
        public void Dispose()
        {
            this.stopping.Cancel();
            List<Client> open;
            lock (this.gate)
            {
                open = this.clients.ToList();
                this.clients.Clear();
            }

            foreach (var client in open)
            {
                client.Drop();
            }

            try
            {
                this.listener.Close();
            }
            catch (ObjectDisposedException)
            {
            }

            this.log.Information("HmrStopped Clients={Clients}", open.Count);
        }

        private static string Serialize(Message message) => JsonSerializer.Serialize(message, Json);

        private static string Serialize(Notice notice) => JsonSerializer.Serialize(notice, Json);

        /// <summary>Queues <paramref name="json"/> for each page of the bundle. Under <see cref="gate"/>.</summary>
        private int Broadcast(string id, string json)
        {
            int sent = 0;
            foreach (var client in this.clients.Where(client => client.BundleId == id).ToList())
            {
                if (client.Queue.Writer.TryWrite(json))
                {
                    sent++;
                }
                else
                {
                    this.clients.Remove(client);
                    client.Drop();
                    this.log.Warning("HmrDropped File={File} Reason={Reason}", client.File, "queue full");
                }
            }

            return sent;
        }

        private async Task AcceptLoop()
        {
            while (!this.stopping.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await this.listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is HttpListenerException || ex is ObjectDisposedException || ex is InvalidOperationException)
                {
                    return;
                }

                _ = Task.Run(() => this.Serve(context));
            }
        }

        /// <summary>
        /// Whether a page at <paramref name="origin"/> may connect: no Origin (not a browser), an
        /// http(s) page on a loopback host or a <c>*.localhost</c> or <c>*.test</c> name, or one
        /// of <paramref name="extra"/> (<c>NSCRIPT_HMR_ORIGINS</c>). <c>null</c> (a sandboxed or
        /// file page) is refused unless listed.
        /// </summary>
        internal static bool AllowedOrigin(string origin, IEnumerable<string> extra)
        {
            if (origin == null || extra.Contains(origin, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            return Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                && (uri.IsLoopback
                    || uri.Host.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase)
                    || uri.Host.EndsWith(".test", StringComparison.OrdinalIgnoreCase));
        }

        private async Task Serve(HttpListenerContext context)
        {
            var request = context.Request;
            var query = request.QueryString;
            string id = query["id"] ?? string.Empty;
            string at = query["at"] ?? string.Empty;
            int colon = at.LastIndexOf(':');
            if (request.Url?.AbsolutePath != PathName || !request.IsWebSocketRequest || colon < 0 || !int.TryParse(at.AsSpan(colon + 1), out int seq)
                || !Plain.IsMatch(id) || !Plain.IsMatch(at.Substring(0, colon)))
            {
                Refuse(context, 404);
                return;
            }

            // The baked url names 127.0.0.1:port; any other Host came through another name.
            if (!string.Equals(request.UserHostName, this.host, StringComparison.OrdinalIgnoreCase))
            {
                this.log.Warning("HmrRefused Status={Status} Host={Host} Origin={Origin}", 403, request.UserHostName, request.Headers["Origin"]);
                Refuse(context, 403);
                return;
            }

            // Any site can load the bundle with a script tag and read its token; the browser
            // names that site in Origin.
            if (!AllowedOrigin(request.Headers["Origin"], ExtraOrigins))
            {
                this.log.Warning("HmrRefused Status={Status} Origin={Origin}", 403, request.Headers["Origin"]);
                Refuse(context, 403);
                return;
            }

            // Fixed time: a page cannot learn the token a byte at a time. The query is never logged.
            var given = Encoding.ASCII.GetBytes(query["t"] ?? string.Empty);
            if (given.Length != this.token.Length || !CryptographicOperations.FixedTimeEquals(given, this.token))
            {
                this.log.Warning("HmrRefused Status={Status} Origin={Origin}", 403, request.Headers["Origin"]);
                Refuse(context, 403);
                return;
            }

            // An upgrade holds its slot while it opens, so pages that connect at once stop at the cap.
            lock (this.gate)
            {
                if (this.clients.Count + this.opening >= MaxClients)
                {
                    this.log.Warning("HmrRefused Status={Status} Origin={Origin}", 503, request.Headers["Origin"]);
                    Refuse(context, 503);
                    return;
                }

                this.opening++;
            }

            WebSocket socket;
            try
            {
                socket = (await context.AcceptWebSocketAsync(null).ConfigureAwait(false)).WebSocket;
            }
            catch (Exception ex) when (ex is WebSocketException || ex is HttpListenerException || ex is ObjectDisposedException)
            {
                lock (this.gate)
                {
                    this.opening--;
                }

                return;
            }

            string gen = at.Substring(0, colon);
            string file = query["file"] ?? string.Empty;
            var client = new Client(socket, id, file.Length > 0 && Plain.IsMatch(file) ? file : id);
            int count;
            lock (this.gate)
            {
                this.opening--;
                if (this.stopping.IsCancellationRequested)
                {
                    client.Drop();
                    return;
                }

                // The records the page lacks, or the generation it must reload onto.
                this.bundles.TryGetValue(id, out var bundle);
                var hello = new Message { Gen = bundle?.Gen ?? string.Empty };
                if (bundle != null && bundle.Gen == gen)
                {
                    hello.Records = bundle.Records.Where(record => record.Seq > seq).ToArray();
                }

                client.Queue.Writer.TryWrite(Serialize(hello));
                if (this.notes.TryGetValue(id, out var note))
                {
                    client.Queue.Writer.TryWrite(Serialize(new Notice { Note = note }));
                }

                this.clients.Add(client);
                count = this.clients.Count;
                this.log.Information(
                    "HmrConnect Bundle={Bundle} At={At} Records={Records} Clients={Clients} Origin={Origin}",
                    id,
                    at,
                    hello.Records?.Length ?? 0,
                    count,
                    request.Headers["Origin"]);
                this.watchLog("hmr connect " + client.File + " at " + at + " clients=" + count);
                if (hello.Records?.Length > 0)
                {
                    this.watchLog("hmr send " + client.File + " " + gen + ":" + hello.Records[0].Seq + ".." + hello.Records[^1].Seq + " clients=1");
                }
            }

            var sending = this.SendLoop(client);
            await this.ReceiveLoop(client).ConfigureAwait(false);
            lock (this.gate)
            {
                this.clients.Remove(client);
                count = this.clients.Count;
            }

            client.Drop();
            await sending.ConfigureAwait(false);
            this.log.Information("HmrClose Bundle={Bundle} Clients={Clients}", id, count);
            this.watchLog("hmr close " + client.File + " clients=" + count);
        }

        private static void Refuse(HttpListenerContext context, int status)
        {
            try
            {
                context.Response.StatusCode = status;
                context.Response.Close();
            }
            catch (Exception ex) when (ex is HttpListenerException || ex is ObjectDisposedException || ex is InvalidOperationException)
            {
            }
        }

        /// <summary>Reads until the page closes; what it sends is ignored.</summary>
        private async Task ReceiveLoop(Client client)
        {
            var buffer = new byte[1024];
            try
            {
                while (client.Socket.State == WebSocketState.Open)
                {
                    var result = await client.Socket.ReceiveAsync(buffer, this.stopping.Token).ConfigureAwait(false);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex) when (ex is WebSocketException || ex is OperationCanceledException || ex is ObjectDisposedException)
            {
            }
        }

        private async Task SendLoop(Client client)
        {
            try
            {
                await foreach (var json in client.Queue.Reader.ReadAllAsync(this.stopping.Token).ConfigureAwait(false))
                {
                    await client.Socket.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, this.stopping.Token).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is WebSocketException || ex is OperationCanceledException || ex is ObjectDisposedException)
            {
                client.Drop();
            }
        }

        private sealed class Bundle
        {
            public Bundle(string gen)
            {
                this.Gen = gen;
            }

            public string Gen { get; }

            /// <summary>Every patch of this generation, in seq order; the next full emit drops them.</summary>
            public List<Record> Records { get; } = new List<Record>();
        }

        private sealed class Client
        {
            public Client(WebSocket socket, string bundleId, string file)
            {
                this.Socket = socket;
                this.BundleId = bundleId;
                this.File = file;
            }

            public WebSocket Socket { get; }

            public string BundleId { get; }

            public string File { get; }

            public Channel<string> Queue { get; } = Channel.CreateBounded<string>(new BoundedChannelOptions(QueueLimit) { SingleReader = true });

            /// <summary>Ends the page's socket at once, with no close handshake.</summary>
            public void Drop()
            {
                this.Queue.Writer.TryComplete();
                this.Socket.Abort();
            }
        }

        /// <summary>One frame to a page: <c>{"gen","records"}</c> or <c>{"gen","reload"}</c>.</summary>
        private sealed class Message
        {
            public string Gen { get; set; } = string.Empty;

            public Record[]? Records { get; set; }

            public string? Reload { get; set; }
        }

        /// <summary>A notice frame, <c>{"note"}</c>: the page logs it; an empty note ends it.</summary>
        private sealed class Notice
        {
            public string Note { get; set; } = string.Empty;
        }

        private sealed class Record
        {
            public int Seq { get; set; }

            public string? Js { get; set; }

            public int Methods { get; set; }

            public string? Reload { get; set; }

            public IReadOnlyList<string>? Notes { get; set; }
        }
    }
}
