namespace NScript.Utils.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Converter;
    using NScript.Csc.Lib.Service;
    using NScript.JST;
    using NScript.Lib.Service;

    /// <summary>
    /// The build service's live patch socket (<see cref="HmrServer"/>), in process: who gets
    /// in, what a page that connects late is sent, and what a page sends is ignored.
    /// </summary>
    [TestClass]
    public class HmrServerTests
    {
        private static readonly string JsFile = Path.Combine(Path.GetTempPath(), "nscript-hmr-test", "App.js");
        private static readonly string Id = HmrClient.BundleId(JsFile);

        private readonly List<string> watchLog = new List<string>();
        private HmrServer server;

        [TestInitialize]
        public void Setup() => this.server = HmrServer.Start(line => { lock (this.watchLog) this.watchLog.Add(line); });

        [TestCleanup]
        public void Cleanup() => this.server.Dispose();

        /// <summary>
        /// Only the socket path, with the daemon's token, through the 127.0.0.1 host name: a
        /// DNS-rebinding page names its own host and gets 403 even with the token (A1).
        /// </summary>
        [TestMethod]
        public void Upgrade_NeedsPathTokenAndLoopbackHost()
        {
            var port = new Uri(this.server.Url).Port;
            var token = this.server.Endpoint.Token;
            string Status(string path, string host) => RawUpgrade(port, path, host);
            string At(string t) => "/nscript-hmr?t=" + t + "&id=" + Id + "&file=App.js&at=g1:0";

            Assert.AreEqual("101", Status(At(token), "127.0.0.1:" + port), "the page's own request");
            Assert.AreEqual("404", Status("/other?t=" + token + "&id=" + Id + "&at=g1:0", "127.0.0.1:" + port));
            Assert.AreEqual("403", Status(At("wrong"), "127.0.0.1:" + port), "a wrong token");
            Assert.AreEqual("403", Status(At(token.Substring(1)), "127.0.0.1:" + port), "a short token");
            Assert.AreEqual("403", Status(At(token), "evil.example:" + port), "a rebinding host");
            Assert.AreEqual("403", Status(At(token), "localhost:" + port), "another name for loopback");
            lock (this.watchLog)
            {
                Assert.IsFalse(this.watchLog.Any(line => line.Contains(token)), "the token is never logged");
            }
        }

        /// <summary>
        /// Any site can load a dev bundle with a script tag and read its token, so a browser
        /// page must come from a loopback or local dev origin, or one NSCRIPT_HMR_ORIGINS lists.
        /// </summary>
        [TestMethod]
        public void Upgrade_NeedsALocalOrigin()
        {
            var port = new Uri(this.server.Url).Port;
            var at = "/nscript-hmr?t=" + this.server.Endpoint.Token + "&id=" + Id + "&file=App.js&at=g1:0";
            string Status(string origin) => RawUpgrade(port, at, "127.0.0.1:" + port, origin);

            foreach (var page in new[] { "http://localhost:5000", "http://127.0.0.1:8080", "http://[::1]:3000", "https://app.localhost", "https://shop.test:4443" })
            {
                Assert.AreEqual("101", Status(page), page);
            }

            foreach (var page in new[] { "https://evil.example", "null", "http://localhost.evil.example", "file://", "chrome-extension://abc" })
            {
                Assert.AreEqual("403", Status(page), page);
            }

            Assert.IsTrue(HmrServer.AllowedOrigin("https://dev.mcqdb.com", new[] { "https://dev.mcqdb.com" }), "a listed origin");
            Assert.IsFalse(HmrServer.AllowedOrigin("https://dev.mcqdb.com", Array.Empty<string>()));
        }

        /// <summary>
        /// A patch an open page cannot take live carries why it must reload, and no script: on
        /// the open socket and in a late page's hello.
        /// </summary>
        [TestMethod]
        public async Task Connect_AReloadRecordSaysWhy_LiveAndInALateHello()
        {
            this.server.Publish(JsFile, LivePatch.NewBuild("g1"));
            using var page = await this.Connect("g1:0");
            Assert.AreEqual("g1", (await Receive(page)).GetProperty("gen").GetString());

            this.server.Publish(JsFile, new LivePatch("g1", 1, null, 1, "load-time A.Main", Array.Empty<string>()));
            var live = (await Receive(page)).GetProperty("records")[0];
            Assert.AreEqual("load-time A.Main", live.GetProperty("reload").GetString());
            Assert.IsFalse(live.TryGetProperty("js", out _), "a reload record has no script");

            using var late = await this.Connect("g1:0");
            var hello = (await Receive(late)).GetProperty("records")[0];
            Assert.AreEqual(1, hello.GetProperty("seq").GetInt32());
            Assert.AreEqual("load-time A.Main", hello.GetProperty("reload").GetString());
        }

        /// <summary>
        /// A page that connects after patches gets, in one hello, only the records past the seq
        /// it holds; a page on another gen gets the gen alone and checks its script. Later
        /// patches arrive as they are published; frames from the page change nothing.
        /// </summary>
        [TestMethod]
        public async Task Connect_CatchesUpPastItsSeq_ThenGetsLivePatches_IgnoresPageFrames()
        {
            this.server.Publish(JsFile, LivePatch.NewBuild("g1"));
            this.server.Publish(JsFile, new LivePatch("g1", 1, "one", 1, null, Array.Empty<string>()));
            this.server.Publish(JsFile, new LivePatch("g1", 2, "two", 2, null, new[] { "closure A.B" }));

            using var page = await this.Connect("g1:1");
            var hello = await Receive(page);
            Assert.AreEqual("g1", hello.GetProperty("gen").GetString());
            var records = hello.GetProperty("records").EnumerateArray().ToList();
            Assert.AreEqual(1, records.Count, "only seq 2 is past the page's seq 1");
            Assert.AreEqual(2, records[0].GetProperty("seq").GetInt32());
            Assert.AreEqual("two", records[0].GetProperty("js").GetString());
            Assert.AreEqual("closure A.B", records[0].GetProperty("notes")[0].GetString());

            using var stale = await this.Connect("g0:7");
            var staleHello = await Receive(stale);
            Assert.AreEqual("g1", staleHello.GetProperty("gen").GetString());
            Assert.IsFalse(staleHello.TryGetProperty("records", out _), "another gen gets no records");

            // A page's frames are ignored; the socket stays open and keeps getting patches.
            await page.SendAsync(Encoding.UTF8.GetBytes("{\"gen\":\"g1\",\"records\":[{\"seq\":9,\"js\":\"evil\"}]}"), WebSocketMessageType.Text, true, CancellationToken.None);
            this.server.Publish(JsFile, new LivePatch("g1", 3, "three", 1, null, Array.Empty<string>()));
            var live = await Receive(page);
            Assert.AreEqual(3, live.GetProperty("records")[0].GetProperty("seq").GetInt32());

            using var late = await this.Connect("g1:0");
            var lateSeqs = (await Receive(late)).GetProperty("records").EnumerateArray().Select(r => r.GetProperty("seq").GetInt32());
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, lateSeqs.ToList(), "the page's frame added no record");

            // A full emit asks every open page to reload onto the new gen.
            this.server.Publish(JsFile, LivePatch.NewBuild("g2"));
            var reload = await Receive(page);
            Assert.AreEqual("g2", reload.GetProperty("gen").GetString());
            Assert.AreEqual("new build", reload.GetProperty("reload").GetString());
        }

        /// <summary>At most <see cref="HmrServer.MaxClients"/> pages: the next upgrade gets 503 before the socket opens.</summary>
        [TestMethod]
        public async Task Connect_PastTheCap_Gets503()
        {
            var pages = new List<ClientWebSocket>();
            try
            {
                for (int i = 0; i < HmrServer.MaxClients; i++)
                {
                    pages.Add(await this.Connect("g1:0"));
                }

                for (int i = 0; i < 100 && this.server.ClientCount < HmrServer.MaxClients; i++)
                {
                    await Task.Delay(10);
                }

                var port = new Uri(this.server.Url).Port;
                Assert.AreEqual(
                    "503",
                    RawUpgrade(port, "/nscript-hmr?t=" + this.server.Endpoint.Token + "&id=" + Id + "&at=g1:0", "127.0.0.1:" + port));
            }
            finally
            {
                pages.ForEach(page => page.Dispose());
            }
        }

        /// <summary>Pages that connect at the same moment still stop at the cap: each upgrade holds its slot before it opens.</summary>
        [TestMethod]
        public async Task Connect_AllAtOnce_StopsAtTheCap()
        {
            var attempts = Enumerable.Range(0, HmrServer.MaxClients * 2).Select(async _ =>
            {
                try
                {
                    return await this.Connect("g1:0");
                }
                catch (WebSocketException)
                {
                    return null;
                }
            }).ToList();
            var pages = (await Task.WhenAll(attempts)).Where(page => page != null).ToList();
            try
            {
                await Task.Delay(200);
                Assert.IsTrue(pages.Count <= HmrServer.MaxClients, "opened " + pages.Count);
                Assert.IsTrue(this.server.ClientCount <= HmrServer.MaxClients, "server holds " + this.server.ClientCount);
            }
            finally
            {
                pages.ForEach(page => page.Dispose());
            }
        }

        /// <summary>
        /// What a page names in its url reaches watch.log only as one line: a file name with a
        /// line break falls back to the bundle id, and a gen with one is refused.
        /// </summary>
        [TestMethod]
        public async Task Connect_PageValuesNeverBreakAWatchLogLine()
        {
            using var page = await this.Connect("g1:0", "a%0D%0Ahmr%20connect%20forged");
            await Receive(page);
            var port = new Uri(this.server.Url).Port;
            Assert.AreEqual(
                "404",
                RawUpgrade(port, "/nscript-hmr?t=" + this.server.Endpoint.Token + "&id=" + Id + "&file=App.js&at=g1%0Aforged:0", "127.0.0.1:" + port));
            lock (this.watchLog)
            {
                Assert.IsFalse(this.watchLog.Any(line => line.Contains('\n') || line.Contains('\r') || line.Contains("forged")), string.Join(" | ", this.watchLog));
                Assert.IsTrue(this.watchLog.Any(line => line.StartsWith("hmr connect " + Id + " at g1:0")), string.Join(" | ", this.watchLog));
            }
        }

        /// <summary>
        /// S6: a notice (a red build) reaches open pages as <c>{"note"}</c> and pages that
        /// connect later after their hello; the same notice is sent once. A good build ends it:
        /// a patch drops it, and a clear sends an empty note only while one stands.
        /// </summary>
        [TestMethod]
        public async Task Note_ReachesOpenAndLatePages_OnceEach_AGoodBuildEndsIt()
        {
            this.server.Publish(JsFile, LivePatch.NewBuild("g1"));
            using var page = await this.Connect("g1:0");
            await Receive(page);

            this.server.Note(JsFile, "build failed: A.cs(1,2): error CS1002: ; expected");
            this.server.Note(JsFile, "build failed: A.cs(1,2): error CS1002: ; expected");
            this.server.Note(JsFile, "build failed: A.cs(3,4): error CS0029: no");
            Assert.AreEqual("build failed: A.cs(1,2): error CS1002: ; expected", (await Receive(page)).GetProperty("note").GetString());
            var second = await Receive(page);
            Assert.AreEqual("build failed: A.cs(3,4): error CS0029: no", second.GetProperty("note").GetString(), "the repeat was not sent");
            Assert.IsFalse(second.TryGetProperty("gen", out _), "a notice frame names no gen: the page checks nothing");

            using var late = await this.Connect("g1:0");
            Assert.AreEqual("g1", (await Receive(late)).GetProperty("gen").GetString());
            Assert.AreEqual("build failed: A.cs(3,4): error CS0029: no", (await Receive(late)).GetProperty("note").GetString(), "a late page gets the standing notice");

            // The fix patches: the notice ends with the patch, so the clear after it sends nothing.
            this.server.Publish(JsFile, new LivePatch("g1", 1, "fix", 1, null, Array.Empty<string>()));
            this.server.Note(JsFile, null);
            this.server.Note(JsFile, "NEEDS BUILD App: new file B.cs; run dotnet build -p:NScriptWatch=true");
            Assert.AreEqual(1, (await Receive(page)).GetProperty("records")[0].GetProperty("seq").GetInt32());
            Assert.AreEqual("NEEDS BUILD App: new file B.cs; run dotnet build -p:NScriptWatch=true", (await Receive(page)).GetProperty("note").GetString(), "no empty note between");

            // A clear with no patch (the build came back to what the page has) ends it with "".
            this.server.Note(JsFile, null);
            Assert.AreEqual(string.Empty, (await Receive(page)).GetProperty("note").GetString());
            using var after = await this.Connect("g1:1");
            await Receive(after);
            this.server.Publish(JsFile, new LivePatch("g1", 2, "two", 1, null, Array.Empty<string>()));
            Assert.IsTrue((await Receive(after)).TryGetProperty("records", out _), "no notice is left for a page that connects after the clear");
        }

        /// <summary>
        /// S6 notice texts: a red project's first error, short; a project that needs a real
        /// build in the same words from the change and from the kept bundle (the page saw it
        /// twice, worded two ways); anything else kept, as is.
        /// </summary>
        [TestMethod]
        public void NoticeTexts_FirstErrorShort_NeedsBuildWordedOnce()
        {
            var response = new ServiceResponse
            {
                Stdout = @"B:\src\My App\Foo.cs(3,9): warning CS0168: unused [B:\src\My App\App.csproj]" + "\n"
                    + @"B:\src\My App\Controls\Bar.cs(52,30): error CS1002: ; expected [B:\src\My App\App.csproj]" + "\n",
                Stderr = string.Empty,
            };
            Assert.AreEqual("Bar.cs(52,30): error CS1002: ; expected", ServiceHost.FirstError(response));
            Assert.IsNull(ServiceHost.FirstError(new ServiceResponse { Stdout = "Build succeeded.", Stderr = string.Empty }));

            var errors = new Dictionary<string, string> { ["App.dll"] = "Bar.cs(52,30): error CS1002: ; expected" };
            Assert.AreEqual("build failed: Bar.cs(52,30): error CS1002: ; expected", ServiceHost.KeptNote("App.dll red", errors));
            Assert.AreEqual(
                ServiceHost.NeedsBuildNote(@"B:\src\My App\obj\Debug\App.dll", @"new file B:\src\My App\New.cs"),
                ServiceHost.KeptNote(@"App.dll needs dotnet build -p:NScriptWatch=true: new file B:\src\My App\New.cs", errors),
                "the kept bundle's notice is the change's notice, so the page shows it once");
            Assert.AreEqual(
                @"NEEDS BUILD App: new file B:\src\My App\New.cs; run dotnet build -p:NScriptWatch=true",
                ServiceHost.NeedsBuildNote(@"B:\src\My App\obj\Debug\App.dll", @"new file B:\src\My App\New.cs"));
            Assert.AreEqual("kept last good: copy pending B:\\bin\\App.dll", ServiceHost.KeptNote("copy pending B:\\bin\\App.dll", errors));
        }

        private async Task<ClientWebSocket> Connect(string at, string file = "App.js")
        {
            var socket = new ClientWebSocket();
            var url = this.server.Url + "?t=" + this.server.Endpoint.Token + "&id=" + Id + "&file=" + file + "&at=" + at;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await socket.ConnectAsync(new Uri(url), timeout.Token);
            return socket;
        }

        private static async Task<JsonElement> Receive(ClientWebSocket socket)
        {
            var buffer = new byte[64 * 1024];
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var text = new StringBuilder();
            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(buffer, timeout.Token);
                text.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            }
            while (!result.EndOfMessage);

            return JsonDocument.Parse(text.ToString()).RootElement;
        }

        /// <summary>
        /// The status code of a hand-written upgrade request: ClientWebSocket cannot set Host.
        /// </summary>
        private static string RawUpgrade(int port, string path, string host, string origin = null)
        {
            using var client = new TcpClient("127.0.0.1", port);
            client.ReceiveTimeout = 5000;
            var stream = client.GetStream();
            var request = "GET " + path + " HTTP/1.1\r\nHost: " + host + "\r\nUpgrade: websocket\r\nConnection: Upgrade\r\n"
                + (origin == null ? string.Empty : "Origin: " + origin + "\r\n")
                + "Sec-WebSocket-Key: dGhlIHNhbXBsZSBub25jZQ==\r\nSec-WebSocket-Version: 13\r\n\r\n";
            var bytes = Encoding.ASCII.GetBytes(request);
            stream.Write(bytes, 0, bytes.Length);
            var buffer = new byte[256];
            int read = stream.Read(buffer, 0, buffer.Length);
            var statusLine = Encoding.ASCII.GetString(buffer, 0, read).Split("\r\n")[0];
            return statusLine.Split(' ')[1];
        }
    }
}
