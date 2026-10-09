namespace NScript.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading;
    using System.Threading.Tasks;
    using NScript.Converter;
    using NScript.Csc.Lib;
    using NScript.Csc.Lib.Service;
    using NScript.Utils;
    using Serilog.Context;
    using Serilog.Events;

    /// <summary>
    /// Tunables and test seams of a <see cref="ServiceHost"/>. Defaults are the production values.
    /// </summary>
    public sealed class ServiceHostOptions
    {
        /// <summary>A request or watch step running longer than this makes the daemon exit (watchdog).</summary>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(600);

        /// <summary>Idle timeout while anything is registered for watch.</summary>
        public TimeSpan WatchIdleTimeout { get; set; } = TimeSpan.FromHours(8);

        /// <summary>Trailing quiet window after the last file event before a batch starts.</summary>
        public TimeSpan Debounce { get; set; } = TimeSpan.FromMilliseconds(50);

        /// <summary>Longest wait from the first event of a window, so a steady writer cannot starve watch.</summary>
        public TimeSpan DebounceCap { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// After a watch registration leaves work dirty (dependents of a project MSBuild just
        /// rebuilt, typically after NEEDS BUILD), a batch replays it once no request has
        /// arrived for this long, so it does not race the rest of the build.
        /// </summary>
        public TimeSpan RegistrationQuiet { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        /// A build session not used for this long is released (a bundle watched but not
        /// edited). <see cref="TimeSpan.Zero"/> turns the sweep off.
        /// </summary>
        public TimeSpan SessionIdleTimeout { get; set; } = ServiceHost.DefaultSessionIdleTimeout;

        /// <summary>A setting that could not be read and fell back to its default; logged once at start.</summary>
        public string? StartupWarning { get; set; }

        /// <summary>Recomputes the toolset hash before each batch. Null: <see cref="ServiceIdentity.ComputeToolsetHash"/>.</summary>
        public Func<string>? ToolsetHash { get; set; }

        /// <summary>
        /// Ends the process after the watchdog fired. Null: hard-kill fallback after 10 s, then
        /// <c>Environment.Exit(3)</c>. Tests record the reason instead.
        /// </summary>
        public Action<string>? ExitProcess { get; set; }

        /// <summary>Runs one stage invocation. Null: the real compile/emit. Tests fake slow or failing steps.</summary>
        public Func<ServiceRequest, ServiceResponse>? RunRequest { get; set; }

        /// <summary>
        /// With <see cref="RunRequest"/>: what a faked compile read, so tests can register and
        /// replay a watch project without Roslyn. Null: a faked compile reports no inputs.
        /// </summary>
        public Func<ServiceRequest, CompileInputs?>? RunRequestInputs { get; set; }

        /// <summary>
        /// With <see cref="RunRequest"/>: the options a faked emit parsed, so tests can register
        /// and replay a watch bundle. Null: a faked emit registers no bundle.
        /// </summary>
        public Func<ServiceRequest, ParseOptions?>? RunRequestEmitOptions { get; set; }
    }

    /// <summary>
    /// The warm build-service daemon (<c>nscript.exe service</c>). One process serves both
    /// stages for one toolset build. Requests are serialized by a global lock and each runs
    /// today's code path with fresh per-request objects; under the lock the daemon swaps the
    /// process-global state those paths rely on (cwd, Console.Out/Error, the Logger and the
    /// Cecil comparer cache). Nothing is incremental: the process is only warm. Requests sent
    /// with <c>Watch</c> are also recorded and replayed on file changes (ServiceHost.Watch.cs).
    /// </summary>
    public sealed partial class ServiceHost
    {
        /// <summary>Environment variable overriding the idle timeout in seconds.</summary>
        public const string IdleSecondsEnvVar = "NSCRIPT_SERVICE_IDLE_SECONDS";

        /// <summary>Environment variable overriding the daemon log level (default Information).</summary>
        public const string LogLevelEnvVar = "NSCRIPT_SERVICE_LOG_LEVEL";

        /// <summary>Environment variable overriding the per-request watchdog timeout in seconds (default 600).</summary>
        public const string RequestTimeoutEnvVar = "NSCRIPT_SERVICE_REQUEST_TIMEOUT";

        /// <summary>Environment variable overriding the idle timeout in seconds while watching (default 8 h).</summary>
        public const string WatchIdleSecondsEnvVar = "NSCRIPT_WATCH_IDLE_SECONDS";

        /// <summary>
        /// Environment variable overriding how long an unused build session is kept, in whole
        /// seconds (default 1800); 0 keeps sessions until the daemon exits.
        /// </summary>
        public const string SessionIdleSecondsEnvVar = "NSCRIPT_SESSION_IDLE_SECONDS";

        /// <summary>How long an unused build session is kept by default.</summary>
        public static readonly TimeSpan DefaultSessionIdleTimeout = TimeSpan.FromMinutes(30);

        /// <summary>Pid file in the run dir: line 1 pid, line 2 process start time as UTC ticks.</summary>
        public const string PidFileName = "daemon.pid";

        /// <summary>Exit code of a daemon ended by its watchdog.</summary>
        public const int WatchdogExitCode = 3;

        /// <summary>How long <c>--status</c> waits for a reply before saying the daemon is not answering.</summary>
        public static readonly TimeSpan StatusReplyTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// How long a plain <c>--stop</c> waits for a reply. A healthy daemon answers at once
        /// (it stops after in-flight requests, without holding the reply).
        /// </summary>
        public static readonly TimeSpan StopReplyTimeout = TimeSpan.FromSeconds(10);

        private const int DefaultIdleSeconds = 600;

        private const string RebuildHint = "if a build was running: dotnet build --no-incremental <project>";

        private readonly ServiceIdentity identity;
        private readonly string pipeName;
        private readonly TimeSpan idleTimeout;
        private readonly bool echoToConsole;
        private readonly ServiceHostOptions options;
        private readonly SemaphoreSlim requestLock = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource stopSource = new CancellationTokenSource();
        private readonly Stopwatch uptime = Stopwatch.StartNew();
        private readonly TextWriter consoleOut = Console.Out;

        private long requestCounter;
        private int inFlight;
        private long lastActivityMs;
        private string stopReason = "unknown";
        private CurrentRequest? current;

        /// <summary>
        /// Creates a host. Tests pass a private <paramref name="pipeName"/>.
        /// </summary>
        public ServiceHost(ServiceIdentity identity, string pipeName, TimeSpan idleTimeout, bool echoToConsole)
            : this(identity, pipeName, idleTimeout, echoToConsole, new ServiceHostOptions())
        {
        }

        /// <summary>
        /// Creates a host with explicit <paramref name="options"/>.
        /// </summary>
        public ServiceHost(ServiceIdentity identity, string pipeName, TimeSpan idleTimeout, bool echoToConsole, ServiceHostOptions options)
        {
            this.identity = identity;
            this.pipeName = pipeName;
            this.idleTimeout = idleTimeout;
            this.echoToConsole = echoToConsole;
            this.options = options;
            this.InitializeWatch();
        }

        /// <summary>
        /// Entry point for <c>nscript.exe service [--foreground] [--status] [--stop [--force]]
        /// [--toolset-dir dir --toolset-hash hash]</c>.
        /// Every connection resets the idle timer, including <c>--status</c>: polling status
        /// more often than the idle timeout keeps the daemon alive.
        /// </summary>
        public static int Run(string[] args)
        {
            bool foreground = false, status = false, stop = false, force = false;
            string? toolsetDir = null, toolsetHash = null;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--foreground": foreground = true; break;
                    case "--status": status = true; break;
                    case "--stop": stop = true; break;
                    case "--force": force = true; break;
                    case "--toolset-dir" when i + 1 < args.Length: toolsetDir = args[++i]; break;
                    case "--toolset-hash" when i + 1 < args.Length: toolsetHash = args[++i]; break;
                    default:
                        Console.Error.WriteLine("nscript service: unknown argument '{0}'", args[i]);
                        Console.Error.WriteLine("Usage: nscript service [--foreground] | --status | --stop [--force]");
                        return 1;
                }
            }

            if (force && !stop)
            {
                Console.Error.WriteLine("nscript service: --force only goes with --stop");
                return 1;
            }

            // A daemon launched from a shadow copy is told which toolset it serves; a daemon or
            // a --status/--stop started by hand serves the toolset it runs from.
            var identity = toolsetDir != null && toolsetHash != null
                ? ServiceIdentity.FromKnown(toolsetDir, toolsetHash)
                : ServiceIdentity.ForToolset(AppContext.BaseDirectory);

            if (force)
            {
                return ForceStop(identity, Console.Out, process => process.Kill(entireProcessTree: false));
            }

            if (status || stop)
            {
                return SendControl(identity, stop ? ServiceProtocol.KindStop : ServiceProtocol.KindStatus, Console.Out, stop ? StopReplyTimeout : StatusReplyTimeout);
            }

            var options = new ServiceHostOptions
            {
                RequestTimeout = ReadSeconds(RequestTimeoutEnvVar, TimeSpan.FromSeconds(600)),
                WatchIdleTimeout = ReadSeconds(WatchIdleSecondsEnvVar, TimeSpan.FromHours(8)),
                SessionIdleTimeout = ParseSessionIdleTimeout(Environment.GetEnvironmentVariable(SessionIdleSecondsEnvVar), out var sessionIdleWarning),
                StartupWarning = sessionIdleWarning,
            };
            return new ServiceHost(identity, identity.PipeName, ReadSeconds(IdleSecondsEnvVar, TimeSpan.FromSeconds(DefaultIdleSeconds)), foreground, options).Serve();
        }

        /// <summary>
        /// <c>--stop --force</c>: kills the daemon of <paramref name="identity"/> without using
        /// its pipe, so it works on a wedged daemon. Kills only when the run dir's lock file is
        /// held and the process with the recorded pid started at the recorded time; a stale pid
        /// file (dead daemon, pid reused by another process) never leads to a kill.
        /// </summary>
        /// <returns>0 when a daemon was killed, 1 when there was no live daemon.</returns>
        public static int ForceStop(ServiceIdentity identity, TextWriter output, Action<Process> kill)
        {
            var runDir = identity.RunDir;
            var pidFile = Path.Combine(runDir, PidFileName);
            int NoDaemon(string why)
            {
                output.WriteLine("nscript service: no live daemon for {0} ({1})", identity.ToolsetDir, why);
                return 1;
            }

            if (!File.Exists(pidFile))
            {
                return NoDaemon("no pid file at " + pidFile);
            }

            var lines = File.ReadAllLines(pidFile);
            if (lines.Length < 2
                || !int.TryParse(lines[0], NumberStyles.None, CultureInfo.InvariantCulture, out int pid)
                || !long.TryParse(lines[1], NumberStyles.None, CultureInfo.InvariantCulture, out long startTicks))
            {
                return NoDaemon("unreadable pid file " + pidFile);
            }

            if (!IsLockHeld(Path.Combine(runDir, ServiceLauncher.DaemonLockFile)))
            {
                return NoDaemon($"stale pid file: pid {pid} does not hold {ServiceLauncher.DaemonLockFile}");
            }

            Process process;
            try
            {
                process = Process.GetProcessById(pid);
            }
            catch (ArgumentException)
            {
                return NoDaemon($"pid {pid} is not running");
            }

            using (process)
            {
                long actualTicks;
                try
                {
                    actualTicks = process.StartTime.ToUniversalTime().Ticks;
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is System.ComponentModel.Win32Exception)
                {
                    return NoDaemon($"pid {pid}: start time unreadable ({ex.Message})");
                }

                if (actualTicks != startTicks)
                {
                    return NoDaemon($"pid {pid} belongs to another process (start time differs)");
                }

                kill(process);
                process.WaitForExit(10_000);
            }

            // The lifetime mutex is released when the process is gone; wait for it so the
            // next build can start a fresh daemon.
            using (var mutex = new Mutex(false, identity.MutexName))
            {
                try
                {
                    if (mutex.WaitOne(TimeSpan.FromSeconds(10)))
                    {
                        mutex.ReleaseMutex();
                    }
                }
                catch (AbandonedMutexException)
                {
                    mutex.ReleaseMutex();
                }
            }

            output.WriteLine("nscript service: killed daemon pid={0}; {1}", pid, RebuildHint);
            return 0;
        }

        /// <summary>
        /// Serves until idle, stopped, or another daemon already owns the pipe.
        /// </summary>
        /// <returns>The process exit code.</returns>
        public int Serve()
        {
            using var lifetime = new Mutex(initiallyOwned: true, @"Local\" + this.pipeName, out bool createdNew);
            if (!createdNew)
            {
                this.consoleOut.WriteLine("nscript service: a daemon for {0} is already running; exiting.", this.pipeName);
                return 0;
            }

            FileStream? shadowLock = null;
            FileStream? runLock = null;
            var pidFile = Path.Combine(this.identity.RunDir, PidFileName);
            UnhandledExceptionEventHandler onCrash = (_, e) => this.OnUnhandledException(e.ExceptionObject as Exception);
            try
            {
                // The daemon log is long-lived and shared by every build: Information by
                // default (Razor's Verbose events are ~330 KB per request); NSCRIPT_SERVICE_LOG_LEVEL
                // (a Serilog level name, e.g. Verbose) overrides it. Capped at 20 MB, one old file kept.
                CompilerLog.Initialize(this.identity.LogPath, "service", runId: null, ReadLogLevel(), fileSizeLimitBytes: 20L * 1024 * 1024);
                var log = CompilerLog.ForComponent("ServiceHost");

                // The run dir belongs to this daemon alone (one daemon per toolset hash, by the
                // lifetime mutex). Its lock file keeps stale-dir cleanup away while we live.
                Directory.CreateDirectory(this.identity.RunDir);
                runLock = new FileStream(
                    Path.Combine(this.identity.RunDir, ServiceLauncher.DaemonLockFile),
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None);
                using (var self = Process.GetCurrentProcess())
                {
                    File.WriteAllText(
                        pidFile,
                        Environment.ProcessId.ToString(CultureInfo.InvariantCulture) + "\n"
                            + self.StartTime.ToUniversalTime().Ticks.ToString(CultureInfo.InvariantCulture) + "\n");
                }

                AppDomain.CurrentDomain.UnhandledException += onCrash;

                // A daemon running from a shadow copy marks it in use, then removes copies of
                // older toolset builds that no daemon uses any more (for every toolset key).
                var runningFrom = AppContext.BaseDirectory;
                if (Path.GetFullPath(runningFrom).StartsWith(Path.GetFullPath(this.identity.ServiceRoot), StringComparison.OrdinalIgnoreCase))
                {
                    shadowLock = new FileStream(
                        Path.Combine(runningFrom, ServiceLauncher.DaemonLockFile),
                        FileMode.OpenOrCreate,
                        FileAccess.ReadWrite,
                        FileShare.None);
                    ServiceLauncher.DeleteStaleShadowCopies(this.identity, runningFrom);
                }

                this.StartWatch();
                log.Information(
                    "ServiceStart ToolsetDir={ToolsetDir} ShadowDir={ShadowDir} ToolsetHash={ToolsetHash} PipeName={PipeName} IdleTimeoutSec={IdleTimeoutSec} RequestTimeoutSec={RequestTimeoutSec} RunDir={RunDir}",
                    this.identity.ToolsetDir,
                    AppContext.BaseDirectory,
                    this.identity.ToolsetHash,
                    this.pipeName,
                    (int)this.idleTimeout.TotalSeconds,
                    (int)this.options.RequestTimeout.TotalSeconds,
                    this.identity.RunDir);
                this.consoleOut.WriteLine(
                    "nscript service: pid={0} pipe={1} log={2}",
                    Environment.ProcessId,
                    this.pipeName,
                    this.identity.LogPath);
                if (this.options.StartupWarning != null)
                {
                    log.Warning("ServiceConfigWarning Message={Message}", this.options.StartupWarning);
                    this.consoleOut.WriteLine("nscript service: warning: {0}", this.options.StartupWarning);
                }

                var watchdog = new Thread(this.WatchdogLoop) { IsBackground = true, Name = "nscript-watchdog" };
                watchdog.Start();
                if (this.options.SessionIdleTimeout > TimeSpan.Zero)
                {
                    new Thread(this.SweepLoop) { IsBackground = true, Name = "nscript-sweep" }.Start();
                }

                this.Touch();
                this.AcceptLoop();

                // Let in-flight requests and a running watch batch finish before the process goes away.
                var drain = Stopwatch.StartNew();
                while (Volatile.Read(ref this.inFlight) > 0 && drain.Elapsed < TimeSpan.FromMinutes(10))
                {
                    Thread.Sleep(50);
                }

                this.StopWatch(this.stopReason);
                log.Information(
                    "ServiceStop Reason={Reason} UptimeSec={UptimeSec} Requests={Requests}",
                    this.stopReason,
                    (long)this.uptime.Elapsed.TotalSeconds,
                    Interlocked.Read(ref this.requestCounter));
                this.consoleOut.WriteLine("nscript service: stopped ({0}).", this.stopReason);
                return 0;
            }
            finally
            {
                AppDomain.CurrentDomain.UnhandledException -= onCrash;
                this.DisposeWatch();
                shadowLock?.Dispose();
                if (runLock != null)
                {
                    TryDelete(pidFile);
                    runLock.Dispose();
                }

                CompilerLog.Shutdown();
                lifetime.ReleaseMutex();
            }
        }

        /// <summary>Asks the daemon to stop after in-flight work.</summary>
        public void RequestStop(string reason)
        {
            this.stopReason = reason;
            this.stopSource.Cancel();
        }

        private static bool IsLockHeld(string lockFile)
        {
            try
            {
                using (new FileStream(lockFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        private static void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        /// <summary>
        /// Sends <c>--status</c> or <c>--stop</c> and prints the reply. <paramref name="replyTimeout"/>
        /// bounds the wait for a daemon that accepts but does not answer (<c>--stop</c> waits for
        /// a running batch, so it passes an infinite timeout).
        /// </summary>
        public static int SendControl(ServiceIdentity identity, string kind, TextWriter output, TimeSpan replyTimeout)
        {
            using var pipe = ServiceClient.TryConnect(identity.PipeName, 1000);
            if (pipe == null && IsLockHeld(Path.Combine(identity.RunDir, ServiceLauncher.DaemonLockFile)))
            {
                // A live daemon that accepts no connection: hung, or its waiting pipe instance
                // was taken by a client that gave up (a --status that timed out).
                output.WriteLine(
                    "nscript service: a daemon holds {0} but is not answering on pipe {1}; try nscript service --stop --force",
                    Path.Combine(identity.RunDir, ServiceLauncher.DaemonLockFile),
                    identity.PipeName);
                output.WriteLine("Log: {0}", identity.LogPath);
                return 1;
            }

            if (pipe == null)
            {
                output.WriteLine("nscript service: no daemon running for {0} (pipe {1})", identity.ToolsetDir, identity.PipeName);
                output.WriteLine("Log: {0}", identity.LogPath);
                return 1;
            }

            var exchange = Task.Run(() =>
            {
                ServiceProtocol.WriteMessage(pipe, new ServiceRequest
                {
                    Kind = kind,
                    ClientPid = Environment.ProcessId,
                    Cwd = Directory.GetCurrentDirectory(),
                    Args = Array.Empty<string>(),
                });
                return ServiceProtocol.ReadMessage<ServiceResponse>(pipe);
            });
            ServiceResponse response;
            try
            {
                if (!exchange.Wait(replyTimeout))
                {
                    output.WriteLine(
                        "nscript service: daemon not answering after {0:0.#} s; try nscript service --stop --force",
                        replyTimeout.TotalSeconds);
                    return 1;
                }

                response = exchange.Result;
            }
            catch (AggregateException ex) when (ex.InnerException is IOException)
            {
                output.WriteLine("nscript service: connection to the daemon broke ({0})", ex.InnerException.Message);
                return 1;
            }

            if (response == null)
            {
                output.WriteLine("nscript service: the daemon closed the connection without replying");
                return 1;
            }

            if (response.Status != null)
            {
                foreach (var pair in response.Status)
                {
                    output.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }
            }

            if (!string.IsNullOrEmpty(response.Message))
            {
                output.WriteLine(response.Message);
            }

            return response.ExitCode;
        }

        private static LogEventLevel ReadLogLevel()
        {
            var value = Environment.GetEnvironmentVariable(LogLevelEnvVar);
            if (string.IsNullOrWhiteSpace(value))
            {
                return LogEventLevel.Information;
            }

            if (!Enum.TryParse(value, ignoreCase: true, out LogEventLevel level))
            {
                throw new ArgumentException($"{LogLevelEnvVar} must be a Serilog level (Verbose, Debug, Information, Warning, Error, Fatal), got '{value}'");
            }

            return level;
        }

        /// <summary>
        /// Reads <see cref="SessionIdleSecondsEnvVar"/>: unset gives the default, 0 turns the
        /// sweep off, a positive whole number of seconds sets it. Anything else gives the default
        /// and a <paramref name="warning"/> naming the variable, rather than a daemon that cannot start.
        /// </summary>
        public static TimeSpan ParseSessionIdleTimeout(string? value, out string? warning)
        {
            warning = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return DefaultSessionIdleTimeout;
            }

            if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            warning = $"{SessionIdleSecondsEnvVar}='{value}' is not a whole number of seconds (0 = off); using {(int)DefaultSessionIdleTimeout.TotalSeconds}";
            return DefaultSessionIdleTimeout;
        }

        private static TimeSpan ReadSeconds(string envVar, TimeSpan defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out int seconds) || seconds <= 0)
            {
                throw new ArgumentException($"{envVar} must be a positive integer, got '{value}'");
            }

            return TimeSpan.FromSeconds(seconds);
        }

        private void AcceptLoop()
        {
            var token = this.stopSource.Token;
            var idlePoll = this.idleTimeout < TimeSpan.FromSeconds(1) ? this.idleTimeout : TimeSpan.FromSeconds(1);
            while (!token.IsCancellationRequested)
            {
                var server = new NamedPipeServerStream(
                    this.pipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);

                var connect = server.WaitForConnectionAsync(token);
                while (!connect.IsCompleted)
                {
                    try
                    {
                        connect.Wait(idlePoll);
                    }
                    catch (AggregateException) when (connect.IsCanceled || connect.IsFaulted)
                    {
                        break;
                    }

                    if (!connect.IsCompleted && this.IsIdle())
                    {
                        this.RequestStop("idle");
                    }
                }

                if (connect.Status != TaskStatus.RanToCompletion)
                {
                    server.Dispose();
                    if (connect.IsFaulted && !token.IsCancellationRequested)
                    {
                        CompilerLog.ForComponent("ServiceHost").Error(
                            connect.Exception, "ServiceAcceptFailed");
                        this.RequestStop("fatal");
                    }

                    continue;
                }

                Interlocked.Increment(ref this.inFlight);
                this.Touch();
                _ = Task.Run(() => this.HandleConnection(server));
            }
        }

        private bool IsIdle()
        {
            var timeout = this.IsWatching ? this.options.WatchIdleTimeout : this.idleTimeout;
            return Volatile.Read(ref this.inFlight) == 0
                && this.uptime.ElapsedMilliseconds - Interlocked.Read(ref this.lastActivityMs) > timeout.TotalMilliseconds;
        }

        private void Touch() => Interlocked.Exchange(ref this.lastActivityMs, this.uptime.ElapsedMilliseconds);

        private void HandleConnection(NamedPipeServerStream server)
        {
            try
            {
                using (server)
                {
                    var request = ServiceProtocol.ReadMessage<ServiceRequest>(server);
                    if (request == null)
                    {
                        return;
                    }

                    ServiceResponse response;
                    switch (request.Kind)
                    {
                        case ServiceProtocol.KindStatus:
                            response = this.BuildStatus();
                            break;
                        case ServiceProtocol.KindStop:
                            response = new ServiceResponse
                            {
                                DaemonPid = Environment.ProcessId,
                                Message = $"nscript service: pid={Environment.ProcessId} stopping after in-flight requests",
                            };
                            this.RequestStop("shutdown");
                            break;
                        case ServiceProtocol.KindEmitJs:
                        case ServiceProtocol.KindCompile:
                            response = this.Execute(request);
                            break;
                        default:
                            response = new ServiceResponse
                            {
                                DaemonPid = Environment.ProcessId,
                                InternalError = true,
                                Message = $"unknown request kind '{request.Kind}'",
                            };
                            break;
                    }

                    ServiceProtocol.WriteMessage(server, response);
                    server.WaitForPipeDrain();
                }
            }
            catch (Exception ex)
            {
                // The client hung up or sent garbage; the daemon stays up.
                CompilerLog.ForComponent("ServiceHost").Warning(ex, "ServiceConnectionFailed");
            }
            finally
            {
                this.Touch();
                Interlocked.Decrement(ref this.inFlight);
            }
        }

        private ServiceResponse Execute(ServiceRequest request)
        {
            var queueWait = Stopwatch.StartNew();
            this.requestLock.Wait();
            try
            {
                queueWait.Stop();
                if (!request.Watch)
                {
                    return this.ExecuteLocked(request, "client", queueWait.ElapsedMilliseconds, out _, out _);
                }

                return this.ExecuteAndRegister(request, queueWait.ElapsedMilliseconds);
            }
            finally
            {
                this.requestLock.Release();
            }
        }

        /// <summary>
        /// Runs one request with logging and the <c>current</c> marker the watchdog and
        /// <c>--status</c> read. Must be called under <see cref="requestLock"/>. Compile
        /// requests report what they read (<paramref name="inputs"/>) for watch mode; emit
        /// requests report their parsed options.
        /// </summary>
        private ServiceResponse ExecuteLocked(ServiceRequest request, string origin, long queueWaitMs, out CompileInputs? inputs, out ParseOptions? emitOptions)
        {
            long requestId = Interlocked.Increment(ref this.requestCounter);
            var log = CompilerLog.ForComponent("ServiceHost");
            var elapsed = Stopwatch.StartNew();
            this.current = new CurrentRequest(requestId, request.Kind, elapsed);
            try
            {
                using (LogContext.PushProperty("RequestId", requestId))
                {
                    log.Information(
                        "RequestStart RequestId={RequestId} Kind={Kind} Origin={Origin} Watch={Watch} ClientPid={ClientPid} Cwd={Cwd} Args={Args} QueueWaitMs={QueueWaitMs}",
                        requestId,
                        request.Kind,
                        origin,
                        request.Watch,
                        request.ClientPid,
                        request.Cwd,
                        request.Args,
                        queueWaitMs);
                    this.Echo("request {0} {1} start (queued {2}ms)", requestId, request.Kind, queueWaitMs);

                    ServiceResponse response;
                    if (this.options.RunRequest != null)
                    {
                        response = this.options.RunRequest(request);
                        inputs = request.Kind == ServiceProtocol.KindCompile ? this.options.RunRequestInputs?.Invoke(request) : null;
                        emitOptions = request.Kind == ServiceProtocol.KindEmitJs ? this.options.RunRequestEmitOptions?.Invoke(request) : null;
                    }
                    else
                    {
                        response = this.RunIsolated(request, captureInputs: request.Kind == ServiceProtocol.KindCompile && (request.Watch || origin == "watch"), out inputs, out emitOptions);
                    }

                    response.DaemonPid = Environment.ProcessId;
                    response.RequestId = requestId;
                    response.ElapsedMs = elapsed.ElapsedMilliseconds;

                    using var process = Process.GetCurrentProcess();
                    log.Information(
                        "RequestEnd RequestId={RequestId} Kind={Kind} Origin={Origin} ExitCode={ExitCode} ElapsedMs={ElapsedMs} InternalError={InternalError} WorkingSetMb={WorkingSetMb} PeakWorkingSetMb={PeakWorkingSetMb} GcHeapMb={GcHeapMb} Gen2Count={Gen2Count}",
                        requestId,
                        request.Kind,
                        origin,
                        response.ExitCode,
                        response.ElapsedMs,
                        response.InternalError,
                        process.WorkingSet64 / (1024 * 1024),
                        process.PeakWorkingSet64 / (1024 * 1024),
                        GC.GetTotalMemory(false) / (1024 * 1024),
                        GC.CollectionCount(2));
                    this.Echo(
                        "request {0} {1} exit={2} {3}ms{4}",
                        requestId,
                        request.Kind,
                        response.ExitCode,
                        response.ElapsedMs,
                        response.InternalError ? " INTERNAL ERROR: " + response.Message : string.Empty);
                    return response;
                }
            }
            finally
            {
                this.current = null;
            }
        }

        /// <summary>
        /// Runs one stage invocation with the process-global state it relies on set for this
        /// request and restored afterwards. Must be called under <see cref="requestLock"/>.
        /// </summary>
        private ServiceResponse RunIsolated(ServiceRequest request, bool captureInputs, out CompileInputs? inputs, out ParseOptions? emitOptions)
        {
            var stdout = new StringWriter();
            var stderr = new StringWriter();
            var savedOut = Console.Out;
            var savedErr = Console.Error;
            var savedCwd = Directory.GetCurrentDirectory();
            var response = new ServiceResponse();
            inputs = null;
            emitOptions = null;
            try
            {
                Directory.SetCurrentDirectory(request.Cwd);
                Console.SetOut(stdout);
                Console.SetError(stderr);
                Builder.ResetProcessState();

                if (request.Kind == ServiceProtocol.KindEmitJs)
                {
                    response.ExitCode = EmitJs(request.Args, out emitOptions);
                }
                else
                {
                    response.ExitCode = CscCompiler.RunInProcess(request.Args, request.Cwd, stdout, captureInputs, out bool utf8Output, out inputs);
                    response.Utf8Output = utf8Output;
                }
            }
            catch (Exception ex)
            {
                response.InternalError = true;
                response.Message = ex.GetType().Name + ": " + ex.Message;
                CompilerLog.ForComponent("ServiceHost").Error(ex, "RequestInternalError");
            }
            finally
            {
                Console.SetOut(savedOut);
                Console.SetError(savedErr);
                Directory.SetCurrentDirectory(savedCwd);
            }

            response.Stdout = stdout.ToString();
            response.Stderr = stderr.ToString();
            return response;
        }

        private static int EmitJs(string[] args, out ParseOptions? options)
        {
            options = ParseOptions.ParseArgs(args);
            if (options == null)
            {
                // Batch prints the usage and exits 1 (ParseOptions.PrintUsage); the daemon
                // must not exit, so it returns the same output and code instead.
                Console.Out.WriteLine(ParseOptions.Usage);
                return 1;
            }

            return NScriptCompiler.Run(options);
        }

        private ServiceResponse BuildStatus()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            using var process = Process.GetCurrentProcess();
            var running = this.current;
            var status = new Dictionary<string, string>
            {
                ["Pid"] = Environment.ProcessId.ToString(),
                ["UptimeSec"] = ((long)this.uptime.Elapsed.TotalSeconds).ToString(),
                ["Requests"] = Interlocked.Read(ref this.requestCounter).ToString(),
                ["CurrentRequest"] = running == null
                    ? "none"
                    : $"{running.Id} {running.Kind} {running.Elapsed.ElapsedMilliseconds}ms",
                ["ToolsetDir"] = this.identity.ToolsetDir,
                ["ToolsetHash"] = this.identity.ToolsetHash,
                ["RunningFrom"] = AppContext.BaseDirectory,
                ["RunDir"] = this.identity.RunDir,
                ["PipeName"] = this.pipeName,
                ["LogPath"] = this.identity.LogPath,
                ["IdleTimeoutSec"] = ((int)this.idleTimeout.TotalSeconds).ToString(),
                ["WatchIdleTimeoutSec"] = ((int)this.options.WatchIdleTimeout.TotalSeconds).ToString(),
                ["SessionIdleTimeoutSec"] = this.options.SessionIdleTimeout > TimeSpan.Zero
                    ? ((int)this.options.SessionIdleTimeout.TotalSeconds).ToString()
                    : "off",
                ["Sessions"] = BuilderSessions.Count.ToString(),
                ["RequestTimeoutSec"] = ((int)this.options.RequestTimeout.TotalSeconds).ToString(),
                ["WorkingSetMb"] = (process.WorkingSet64 / (1024 * 1024)).ToString(),
                ["PeakWorkingSetMb"] = (process.PeakWorkingSet64 / (1024 * 1024)).ToString(),
                ["GcHeapMb"] = (GC.GetTotalMemory(false) / (1024 * 1024)).ToString(),
            };
            foreach (var pair in this.WatchStatusFields())
            {
                status[pair.Key] = pair.Value;
            }

            return new ServiceResponse
            {
                DaemonPid = Environment.ProcessId,
                Status = status,
            };
        }

        /// <summary>
        /// Exits the process when one request or watch step has run longer than
        /// <see cref="ServiceHostOptions.RequestTimeout"/>. A wedged daemon would otherwise
        /// block every flagged build of this toolset; after the exit clients see a broken
        /// pipe and compile locally (NSS001).
        /// </summary>
        private void WatchdogLoop()
        {
            var poll = TimeSpan.FromMilliseconds(Math.Min(1000, Math.Max(10, this.options.RequestTimeout.TotalMilliseconds / 4)));
            while (!this.stopSource.IsCancellationRequested)
            {
                Thread.Sleep(poll);
                var running = this.current;
                if (running == null || running.Elapsed.Elapsed <= this.options.RequestTimeout)
                {
                    continue;
                }

                CompilerLog.ForComponent("ServiceHost").Error(
                    "ServiceStop Reason={Reason} RequestId={RequestId} Kind={Kind} ElapsedMs={ElapsedMs} TimeoutSec={TimeoutSec}",
                    "watchdog",
                    running.Id,
                    running.Kind,
                    running.Elapsed.ElapsedMilliseconds,
                    (int)this.options.RequestTimeout.TotalSeconds);
                this.WatchLog(
                    "watchdog: request {0} ({1}) ran {2} ms, over the {3} s limit; daemon exiting; {4}",
                    running.Id,
                    running.Kind,
                    running.Elapsed.ElapsedMilliseconds,
                    (int)this.options.RequestTimeout.TotalSeconds,
                    RebuildHint);
                this.Echo("watchdog fired on request {0}; exiting", running.Id);
                (this.options.ExitProcess ?? DefaultExit)("watchdog");
                return;
            }
        }

        /// <summary>
        /// Releases build sessions unused for <see cref="ServiceHostOptions.SessionIdleTimeout"/>
        /// (a bundle watched all day but not edited). The sweep runs only when the request lock
        /// is free without waiting and no connection is open, so it never runs during a request
        /// or a watch batch, and a request never waits on more than one dictionary pass.
        /// </summary>
        private void SweepLoop()
        {
            var timeout = this.options.SessionIdleTimeout;
            var poll = TimeSpan.FromMilliseconds(Math.Clamp(timeout.TotalMilliseconds / 4, 10, 1000));
            while (!this.stopSource.IsCancellationRequested)
            {
                Thread.Sleep(poll);
                if (Volatile.Read(ref this.inFlight) > 0 || !this.requestLock.Wait(0))
                {
                    continue;
                }

                int dropped;
                try
                {
                    dropped = BuilderSessions.DropIdle(timeout);
                }
                finally
                {
                    this.requestLock.Release();
                }

                if (dropped > 0)
                {
                    CompilerLog.ForComponent("ServiceHost").Information(
                        "SessionsDropped Count={Count} IdleSec={IdleSec} Left={Left}",
                        dropped,
                        (int)timeout.TotalSeconds,
                        BuilderSessions.Count);
                    this.Echo("dropped {0} build session(s) unused for {1} s", dropped, (int)timeout.TotalSeconds);
                }
            }
        }

        private void DefaultExit(string reason)
        {
            // Environment.Exit runs exit handlers on this thread; if a wedged thread blocks
            // them (for example one holding a log sink), kill the process after 10 s.
            var killer = new Thread(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                using var self = Process.GetCurrentProcess();
                self.Kill();
            })
            { IsBackground = true, Name = "nscript-hard-kill" };
            killer.Start();
            this.DisposeWatchLog();
            CompilerLog.Shutdown();
            Environment.Exit(WatchdogExitCode);
        }

        private void OnUnhandledException(Exception? ex)
        {
            CompilerLog.ForComponent("ServiceHost").Fatal(ex, "ServiceStop Reason={Reason}", "crash");
            this.WatchStopped("internalError", ex);
            this.DisposeWatchLog();
            CompilerLog.Shutdown();
        }

        private void Echo(string format, params object[] args)
        {
            if (this.echoToConsole)
            {
                this.consoleOut.WriteLine("nscript service: " + format, args);
            }
        }

        private sealed record CurrentRequest(long Id, string Kind, Stopwatch Elapsed);
    }
}
