namespace NScript.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using NScript.Csc.Lib.Service;
    using NScript.Utils;
    using Serilog;
    using Serilog.Context;

    /// <summary>
    /// Watch mode: requests sent with <c>Watch</c> are recorded in a <see cref="WatchRegistry"/>;
    /// file events on their inputs are debounced into batches that replay the recorded
    /// compile and emit requests in dependency order, under the same global lock as every
    /// MSBuild request.
    /// </summary>
    public sealed partial class ServiceHost
    {
        /// <summary>The rule printed in <c>--status</c> and at the top of watch.log.</summary>
        public const string WatchNote = "a plain dotnet build asks this daemon first and runs in full when it cannot vouch; nscript service --stop ends watching; added or deleted files and .csproj/.props/.targets edits need dotnet build -p:NScriptWatch=true";

        private const int MaxBundleRetries = 3;

        private const int MaxReferenceRetries = 5;

        private static readonly TimeSpan ReferenceRetryDelay = TimeSpan.FromSeconds(1);

        // "error CS0009:", "Program.cs(28,62): error CS0029:", "NScript.Exe(0,0): error UNK0001:".
        private static readonly Regex ErrorLine = new Regex(@"(^|[\s:])error [A-Z]+[0-9]+:", RegexOptions.CultureInvariant);

        // CS0006 metadata file not found, CS0009 metadata file could not be opened.
        private static readonly Regex ReferenceUnreadableLine = new Regex(@"(^|[\s:])error CS000[69]:", RegexOptions.CultureInvariant);

        private readonly RetryBudget referenceRetries = new RetryBudget(MaxReferenceRetries);

        private readonly RetryBudget copyRetries = new RetryBudget(MaxReferenceRetries);

        private readonly object watchGate = new object();
        private readonly AutoResetEvent watchSignal = new AutoResetEvent(false);
        private readonly Dictionary<string, FileSystemWatcher> watchers = new Dictionary<string, FileSystemWatcher>(StringComparer.OrdinalIgnoreCase);
        private WatchRegistry registry = null!;
        private HashSet<string> pendingPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private long pendingEvents;

        // Watcher events on source or build files; a batch planned before the count moved is superseded.
        private long sourceEvents;
        private long firstEventMs = -1;
        private long lastEventMs;
        private long batchCounter;

        // Batches the watch loop took and finished, both under watchGate; --sync waits until they match.
        private long batchesTaken;
        private long batchesDone;

        // How often a waiting --sync wakes without a finished batch (to see a stop).
        private static readonly TimeSpan SyncPoll = TimeSpan.FromMilliseconds(250);
        private readonly RetryBudget bundleRetries = new RetryBudget(MaxBundleRetries);
        private string lastBatch = "none";

        // The result word of lastBatch: ok, failed, blocked, kept or superseded.
        private string lastBatchResult = "ok";
        private Thread? watchThread;
        private ILogger? watchLog;
        private int watchStopped;

        // Uptime ms at which a registration-armed batch is due, or -1.
        private long registrationKickMs = -1;

        private bool IsWatching
        {
            get
            {
                lock (this.watchGate)
                {
                    return !this.registry.IsEmpty;
                }
            }
        }

        private string WatchLogPath => Path.Combine(this.identity.RunDir, "watch.log");

        private string RspDir => Path.Combine(this.identity.RunDir, "rsp");

        /// <summary>
        /// Probes a path after the debounce: missing, locked (still being written after
        /// 3 tries, 30 ms apart) or present with its content hash.
        /// </summary>
        internal static FileProbe ProbeFile(string path)
        {
            for (int attempt = 1; ; attempt++)
            {
                try
                {
                    var hash = CompileInputs.HashFile(path);
                    return hash == null ? FileProbe.Missing : FileProbe.Present(hash);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    if (attempt >= 3)
                    {
                        return FileProbe.Locked;
                    }

                    Thread.Sleep(30);
                }
            }
        }

        /// <summary>The MVID and write time of an assembly, or null when it is missing or unreadable.</summary>
        internal static AssemblyStamp? ReadStamp(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var pe = new PEReader(stream);
                var metadata = pe.GetMetadataReader();
                var mvid = metadata.GetGuid(metadata.GetModuleDefinition().Mvid);
                return new AssemblyStamp(mvid, File.GetLastWriteTimeUtc(path));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is BadImageFormatException || ex is InvalidOperationException)
            {
                return null;
            }
        }

        private void InitializeWatch()
        {
            this.registry = new WatchRegistry(ProbeFile, ReadStamp) { ResourcePatchEnabled = this.options.ResourcePatch };
        }

        private void StartWatch()
        {
            // Snapshots of an earlier daemon of the same toolset hash are not ours to trust.
            if (Directory.Exists(this.RspDir))
            {
                Directory.Delete(this.RspDir, recursive: true);
            }

            Directory.CreateDirectory(this.RspDir);
            this.watchLog = new LoggerConfiguration()
                .WriteTo.File(
                    this.WatchLogPath,
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} {Message:lj}{NewLine}",
                    fileSizeLimitBytes: 5L * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 2)
                .CreateLogger();
            this.WatchLog("daemon pid {0} started (toolset {1}); {2}", Environment.ProcessId, this.identity.ToolsetHash.Substring(0, 16), WatchNote);
            if (this.options.DropWatchEvents.Count > 0)
            {
                this.WatchLog("TEST HOOK {0}={1}: watcher events for these extensions are ignored", DropWatchEventsEnvVar, string.Join(";", this.options.DropWatchEvents));
            }
            this.watchThread = new Thread(this.WatchLoop) { IsBackground = true, Name = "nscript-watch" };
            this.watchThread.Start();
        }

        /// <summary>Ends watching after the accept loop and the drain: no batch runs any more.</summary>
        private void StopWatch(string reason)
        {
            this.watchSignal.Set();
            this.watchThread?.Join(TimeSpan.FromSeconds(30));
            this.WatchStopped(reason, null);
        }

        private void WatchStopped(string reason, Exception? ex)
        {
            if (Interlocked.Exchange(ref this.watchStopped, 1) != 0)
            {
                return;
            }

            this.DeleteWatchMarkers();

            bool watching;
            lock (this.watchGate)
            {
                watching = !this.registry.IsEmpty;
            }

            if (watching || ex != null)
            {
                CompilerLog.ForComponent("Watch").Information(ex, "WatchStop Reason={Reason}", reason);
                this.WatchLog("watch stopped: {0}{1}", reason, ex == null ? string.Empty : " " + ex);
                this.LogStale(reason);
            }
        }

        /// <summary>
        /// Names in watch.log every output a stop leaves behind its sources: bundles not
        /// emitted since their inputs changed (red, blocked, superseded, or waiting for a
        /// retry) and bin copies not refreshed since their project compiled.
        /// </summary>
        private void LogStale(string reason)
        {
            List<string> stale;
            lock (this.watchGate)
            {
                stale = this.registry.DirtyBundles
                    .Concat(this.registry.CopyPending.Select(e => e.Target))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            foreach (var path in stale)
            {
                CompilerLog.ForComponent("Watch").Information("WatchStale Reason={Reason} Path={Path}", reason, path);
                this.WatchLog("STALE   {0} (not updated before watch stopped; run dotnet build)", path);
            }
        }

        private void DisposeWatch()
        {
            lock (this.watchGate)
            {
                foreach (var watcher in this.watchers.Values)
                {
                    watcher.Dispose();
                }

                this.watchers.Clear();
            }

            this.DisposeWatchLog();
        }

        private void DisposeWatchLog()
        {
            (Interlocked.Exchange(ref this.watchLog, null) as IDisposable)?.Dispose();
        }

        private void WatchLog(string format, params object[] args)
        {
            this.watchLog?.Information("{Line:l}", string.Format(format, args));
        }

        /// <summary>
        /// MSBuild request with <c>Watch</c>: snapshot the @rsp files (MSBuild deletes them
        /// after the compile), run the request with the snapshot args, and record it.
        /// Must be called under <see cref="requestLock"/>.
        /// </summary>
        private ServiceResponse ExecuteAndRegister(ServiceRequest request, long queueWaitMs)
        {
            var log = CompilerLog.ForComponent("Watch");
            if (request.Kind == ServiceProtocol.KindCompile)
            {
                var replayArgs = this.SnapshotResponseFiles(request.Args, request.Cwd);
                var replay = new ServiceRequest
                {
                    Kind = request.Kind,
                    ClientPid = request.ClientPid,
                    Cwd = request.Cwd,
                    Args = replayArgs,
                    Watch = true,
                    WatchSdkDir = request.WatchSdkDir,
                };

                // The baseline is taken before the compile (F-001): a file added or a build file
                // edited while it runs is then a change, never part of what sync vouches for.
                var existing = EnumerateSourceFiles(request.Cwd);
                var buildFiles = HashBuildFiles(request.Cwd, request.WatchSdkDir);
                var response = this.ExecuteLocked(replay, "client", queueWaitMs, out var inputs, out _);
                if (inputs == null || response.InternalError)
                {
                    log.Warning("WatchRegister Kind={Kind} Skipped=true Reason={Reason}", request.Kind, response.InternalError ? response.Message : "arguments did not parse");
                    return response;
                }

                ProjectRecord record;
                lock (this.watchGate)
                {
                    record = this.registry.RegisterCompile(request.Cwd, replayArgs, inputs, response.ExitCode, request.WatchSdkDir, existing, buildFiles);
                }

                // Kept while watching, red included (--sync answers that); deleted when watch stops.
                this.WriteWatchMarker(record.Key);

                log.Information(
                    "WatchRegister Kind={Kind} Key={Key} Inputs={Inputs} Outputs={Outputs} References={References} BuildFiles={BuildFiles} ExitCode={ExitCode}",
                    request.Kind,
                    record.Key,
                    record.Inputs.Sources.Count + record.Inputs.Resources.Count,
                    new[] { inputs.Output, inputs.RefOut },
                    inputs.References.Count,
                    buildFiles.Keys,
                    response.ExitCode);
                this.WatchLog("register compile {0} ({1} inputs, exit {2})", record.Name, record.Inputs.Sources.Count + record.Inputs.Resources.Count, response.ExitCode);
                this.UpdateWatchers();
                this.ArmBatchIfStillDirty(record.Name);
                return response;
            }

            var emitResponse = this.ExecuteLocked(request, "client", queueWaitMs, out _, out var options);
            if (options == null || emitResponse.InternalError)
            {
                log.Warning("WatchRegister Kind={Kind} Skipped=true Reason={Reason}", request.Kind, emitResponse.InternalError ? emitResponse.Message : "arguments did not parse");
                return emitResponse;
            }

            BundleRecord bundle;
            lock (this.watchGate)
            {
                bundle = this.registry.RegisterBundle(request.Cwd, request.Args, options.JsFileName, options.EntryAssembly, options.ReferenceDlls);

                // The registering build's emit ran here: its JS is the daemon's (D-S3-1).
                this.registry.RecordBundleRun(bundle.Key, !emitResponse.InternalError && emitResponse.ExitCode == 0);
            }

            log.Information(
                "WatchRegister Kind={Kind} Key={Key} Entry={Entry} References={References} ExitCode={ExitCode}",
                request.Kind,
                bundle.Key,
                bundle.Entry,
                bundle.References.Count,
                emitResponse.ExitCode);
            this.WatchLog("register bundle {0}", bundle.Key);
            this.UpdateWatchers();
            this.ArmBatchIfStillDirty(bundle.Name);
            return emitResponse;
        }

        /// <summary>
        /// A registration can leave dirty work no file event will replay: the dependents of a
        /// project MSBuild just rebuilt (after NEEDS BUILD the user builds one project). Arms a
        /// batch for when requests have been quiet for <see cref="ServiceHostOptions.RegistrationQuiet"/>;
        /// each later registration that still sees dirty work pushes it back.
        /// </summary>
        private void ArmBatchIfStillDirty(string registered)
        {
            WatchPlan plan;
            lock (this.watchGate)
            {
                plan = this.registry.Plan();
                if (plan.Compiles.Count == 0 && plan.Bundles.Count == 0)
                {
                    return;
                }

                this.registrationKickMs = this.uptime.ElapsedMilliseconds + (long)this.options.RegistrationQuiet.TotalMilliseconds;
            }

            // Wakes the loop so it waits for the new due time, not its current 1 s timeout.
            this.watchSignal.Set();
            // watch.log says it once, when the batch fires: mid-build the list still names
            // projects the build is about to produce.
            var stale = plan.Compiles.Select(p => p.Name).Concat(plan.Bundles.Select(b => b.Name)).ToList();
            CompilerLog.ForComponent("Watch").Information("WatchRegisterStale Registered={Registered} Stale={Stale}", registered, stale);
        }

        /// <summary>
        /// Rewrites every <c>@file</c> argument to a byte-identical, content-addressed copy in
        /// this daemon's rsp folder, so the request can be replayed after MSBuild deleted it.
        /// </summary>
        private string[] SnapshotResponseFiles(string[] args, string cwd)
        {
            var result = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (!arg.StartsWith("@", StringComparison.Ordinal))
                {
                    result[i] = arg;
                    continue;
                }

                var source = Path.GetFullPath(arg.Substring(1).Trim().Trim('"'), cwd);
                var bytes = File.ReadAllBytes(source);
                var name = Convert.ToHexString(SHA256.HashData(bytes)).Substring(0, 16).ToLowerInvariant() + ".rsp";
                var target = Path.Combine(this.RspDir, name);
                if (!File.Exists(target))
                {
                    var temp = target + "." + Guid.NewGuid().ToString("N") + ".tmp";
                    File.WriteAllBytes(temp, bytes);
                    File.Move(temp, target, overwrite: true);
                }

                result[i] = "@" + target;
            }

            return result;
        }

        private static List<string> EnumerateSourceFiles(string projectDir)
        {
            var files = new List<string>();
            var root = Path.GetFullPath(projectDir);
            var pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count > 0)
            {
                var dir = pending.Pop();
                foreach (var sub in Directory.EnumerateDirectories(dir))
                {
                    var name = Path.GetFileName(sub);
                    if (dir == root && (string.Equals(name, "obj", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "bin", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    pending.Push(sub);
                }

                files.AddRange(Directory.EnumerateFiles(dir).Where(WatchRegistry.IsSourceKind));
            }

            return files;
        }

        /// <summary>
        /// Hashes the files that define a project: csproj/props/targets in the project folder,
        /// every *.props/*.targets in each ancestor folder that has a Directory.Build.props or
        /// .targets, and the NScript.Sdk folder's props/targets.
        /// </summary>
        private static Dictionary<string, string> HashBuildFiles(string projectDir, string? sdkDir)
        {
            var dirs = new List<string> { Path.GetFullPath(projectDir) };
            for (var dir = Directory.GetParent(Path.GetFullPath(projectDir)); dir != null; dir = dir.Parent)
            {
                if (dir.EnumerateFiles("Directory.Build.*").Any(f => WatchRegistry.IsBuildFile(f.FullName)))
                {
                    dirs.Add(dir.FullName);
                }
            }

            if (!string.IsNullOrWhiteSpace(sdkDir) && Directory.Exists(sdkDir))
            {
                dirs.Add(Path.GetFullPath(sdkDir));
            }

            var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var dir in dirs)
            {
                foreach (var file in Directory.EnumerateFiles(dir).Where(WatchRegistry.IsBuildFile))
                {
                    var hash = CompileInputs.HashFile(file);
                    if (hash != null)
                    {
                        hashes[file] = hash;
                    }
                }
            }

            return hashes;
        }

        /// <summary>Creates watchers for new roots and disposes the ones no longer needed.</summary>
        private void UpdateWatchers()
        {
            lock (this.watchGate)
            {
                var wanted = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                foreach (var root in this.registry.RecursiveRoots())
                {
                    wanted[root] = true;
                }

                foreach (var root in this.registry.BuildFileRoots())
                {
                    wanted.TryAdd(root, false);
                }

                foreach (var stale in this.watchers.Keys.Where(k => !wanted.ContainsKey(k) || wanted[k] != this.watchers[k].IncludeSubdirectories).ToList())
                {
                    this.watchers[stale].Dispose();
                    this.watchers.Remove(stale);
                }

                bool added = false;
                foreach (var (root, recursive) in wanted)
                {
                    if (this.watchers.ContainsKey(root) || !Directory.Exists(root))
                    {
                        continue;
                    }

                    var watcher = new FileSystemWatcher(root)
                    {
                        IncludeSubdirectories = recursive,
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size,
                        InternalBufferSize = 64 * 1024,
                    };
                    if (!recursive)
                    {
                        watcher.Filters.Add("*.props");
                        watcher.Filters.Add("*.targets");
                    }

                    watcher.Changed += (_, e) => this.OnFileEvent(e.FullPath, null);
                    watcher.Created += (_, e) => this.OnFileEvent(e.FullPath, null);
                    watcher.Deleted += (_, e) => this.OnFileEvent(e.FullPath, null);
                    watcher.Renamed += (_, e) => this.OnFileEvent(e.FullPath, e.OldFullPath);
                    watcher.Error += (_, e) => this.OnWatcherError(root, e.GetException());
                    watcher.EnableRaisingEvents = true;
                    this.watchers[root] = watcher;
                    added = true;
                }

                if (added)
                {
                    CompilerLog.ForComponent("Watch").Information(
                        "WatchStart Projects={Projects} Bundles={Bundles} Roots={Roots}",
                        this.registry.Projects.Count,
                        this.registry.Bundles.Count,
                        this.watchers.Select(w => (w.Value.IncludeSubdirectories ? "r " : "f ") + w.Key).ToList());

                    // Edits between the registration hashes and the watcher start are found by a rescan.
                    this.QueueRescan();
                }
            }
        }

        /// <summary>
        /// Queues every input and recorded build file, so changes no event reported (before a
        /// watcher started, or lost in an overflow) are classified. Call under <see cref="watchGate"/>.
        /// </summary>
        private void QueueRescan()
        {
            foreach (var path in this.registry.AllInputs().Concat(this.registry.Projects.SelectMany(p => p.BuildFiles.Keys)))
            {
                this.pendingPaths.Add(path);
            }

            this.MarkEvent();
        }

        private void OnFileEvent(string path, string? oldPath)
        {
            try
            {
                lock (this.watchGate)
                {
                    bool any = false;
                    foreach (var candidate in oldPath == null ? new[] { path } : new[] { path, oldPath })
                    {
                        if (this.options.DropWatchEvents.Contains(Path.GetExtension(candidate), StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        if (!this.registry.IsIgnored(candidate))
                        {
                            this.pendingPaths.Add(candidate);
                            any = true;
                            if (WatchRegistry.IsSourceKind(candidate) || WatchRegistry.IsBuildFile(candidate))
                            {
                                this.sourceEvents++;
                            }
                        }
                    }

                    if (any)
                    {
                        this.MarkEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                this.WatchFatal(ex);
            }
        }

        /// <summary>The roots that have a file watcher now.</summary>
        internal IReadOnlyList<string> WatchRoots
        {
            get
            {
                lock (this.watchGate)
                {
                    return this.watchers.Keys.ToList();
                }
            }
        }

        /// <summary>
        /// An overflow loses events but keeps the watcher: every input is rescanned. Any other
        /// error stops that watcher for good (on Windows FileSystemWatcher turns
        /// EnableRaisingEvents off) and <see cref="UpdateWatchers"/> never replaces a root it
        /// has, so later edits there would go unseen and sync would vouch for stale output:
        /// the watch stops and its markers go, and dotnet build runs plain.
        /// </summary>
        internal void OnWatcherError(string root, Exception ex)
        {
            try
            {
                if (ex is not InternalBufferOverflowException)
                {
                    this.WatchLog("watcher lost {0} ({1}); watch stopped, run dotnet build -p:NScriptWatch=true", root, ex.Message);
                    this.WatchFatal(ex);
                    return;
                }

                CompilerLog.ForComponent("Watch").Warning(ex, "WatchOverflow Root={Root}", root);
                this.WatchLog("watcher overflow on {0} ({1}); rescanning", root, ex.Message);
                lock (this.watchGate)
                {
                    this.QueueRescan();
                }
            }
            catch (Exception fatal)
            {
                this.WatchFatal(fatal);
            }
        }

        /// <summary>Records one event for the debounce. Call under <see cref="watchGate"/>.</summary>
        private void MarkEvent()
        {
            var now = this.uptime.ElapsedMilliseconds;
            this.pendingEvents++;
            if (this.firstEventMs < 0)
            {
                this.firstEventMs = now;
            }

            this.lastEventMs = now;
            this.Touch();
            this.watchSignal.Set();
        }

        private void WatchFatal(Exception ex)
        {
            CompilerLog.ForComponent("Watch").Error(ex, "WatchStop Reason={Reason}", "internalError");
            this.WatchStopped("internalError", ex);
            this.RequestStop("fatal");
        }

        /// <summary>The watch worker: debounce, then one batch at a time.</summary>
        private void WatchLoop()
        {
            var token = this.stopSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    long wait = 1000;
                    lock (this.watchGate)
                    {
                        if (this.registrationKickMs >= 0)
                        {
                            wait = Math.Clamp(this.registrationKickMs - this.uptime.ElapsedMilliseconds, 10, 1000);
                        }
                    }

                    this.watchSignal.WaitOne(TimeSpan.FromMilliseconds(wait));
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    lock (this.watchGate)
                    {
                        if (this.registrationKickMs >= 0 && this.uptime.ElapsedMilliseconds >= this.registrationKickMs)
                        {
                            if (Volatile.Read(ref this.inFlight) == 0)
                            {
                                // Not an input, so it classifies as dropped; the batch replays what is dirty.
                                this.registrationKickMs = -1;
                                this.pendingPaths.Add(Path.Combine(this.identity.RunDir, "registered"));
                                this.WatchLog("build quiet after registering; replaying what is still dirty");
                            }
                            else
                            {
                                this.registrationKickMs = this.uptime.ElapsedMilliseconds + (long)this.options.RegistrationQuiet.TotalMilliseconds;
                            }
                        }
                    }

                    HashSet<string> paths;
                    long events, debounceMs, sourceEventsTaken;
                    while (true)
                    {
                        long now = this.uptime.ElapsedMilliseconds;
                        long first, last;
                        lock (this.watchGate)
                        {
                            first = this.firstEventMs;
                            last = this.lastEventMs;
                        }

                        if (first < 0
                            || now - last >= this.options.Debounce.TotalMilliseconds
                            || now - first >= this.options.DebounceCap.TotalMilliseconds)
                        {
                            break;
                        }

                        Thread.Sleep(10);
                    }

                    lock (this.watchGate)
                    {
                        paths = this.pendingPaths;
                        events = this.pendingEvents;
                        sourceEventsTaken = this.sourceEvents;
                        debounceMs = this.firstEventMs < 0 ? 0 : this.uptime.ElapsedMilliseconds - this.firstEventMs;
                        this.pendingPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        this.pendingEvents = 0;
                        this.firstEventMs = -1;
                        if (paths.Count > 0)
                        {
                            this.batchesTaken++;
                        }
                    }

                    if (paths.Count == 0)
                    {
                        continue;
                    }

                    // Counted like a request, so --stop and the idle check wait for the batch.
                    Interlocked.Increment(ref this.inFlight);
                    try
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        this.RunBatch(paths, events, debounceMs, sourceEventsTaken);
                    }
                    finally
                    {
                        lock (this.watchGate)
                        {
                            this.batchesDone++;
                            Monitor.PulseAll(this.watchGate);
                        }

                        this.Touch();
                        Interlocked.Decrement(ref this.inFlight);
                    }
                }
            }
            catch (Exception ex)
            {
                this.WatchFatal(ex);
            }
        }

        /// <summary>
        /// Runs one batch. <paramref name="sourceEventsTaken"/> is the source event count when
        /// <paramref name="paths"/> was taken: a later source event (a checkout still landing)
        /// supersedes the rest of the batch. Unrun steps stay dirty for the next batch, so each
        /// project compiles about once per storm, and every batch still runs at least one step.
        /// </summary>
        private void RunBatch(HashSet<string> paths, long events, long debounceMs, long sourceEventsTaken)
        {
            long batchId = Interlocked.Increment(ref this.batchCounter);
            var log = CompilerLog.ForComponent("Watch");
            using var batchProperty = LogContext.PushProperty("BatchId", batchId);
            var batchClock = Stopwatch.StartNew();

            // One writer per output: a daemon whose toolset was rebuilt never writes again.
            var toolsetHash = (this.options.ToolsetHash ?? (() => ServiceIdentity.ComputeToolsetHash(this.identity.ToolsetDir)))();
            if (!string.Equals(toolsetHash, this.identity.ToolsetHash, StringComparison.OrdinalIgnoreCase))
            {
                log.Information("WatchStop Reason={Reason} BatchId={BatchId} ToolsetHash={ToolsetHash}", "toolsetChanged", batchId, toolsetHash);
                this.WatchLog("compiler rebuilt; run dotnet build -p:NScriptWatch=true");
                this.Echo("compiler rebuilt; watch stopped, daemon exiting");
                Interlocked.Exchange(ref this.watchStopped, 1);

                // This stop skips WatchStopped. Fold in the save that found the new toolset, so
                // the bundles it leaves stale are named too; nothing runs on the registry after.
                lock (this.watchGate)
                {
                    this.registry.Apply(this.registry.Classify(paths));
                }

                this.LogStale("toolsetChanged");
                this.RequestStop("toolsetChanged");
                return;
            }

            foreach (var gone in AddFilesOfNewDirectories(paths, Directory.Exists))
            {
                log.Information("WatchDirectoryGone BatchId={BatchId} Path={Path}", batchId, gone);
            }

            this.requestLock.Wait();
            try
            {
                WatchChanges changes;
                WatchPlan plan;
                IReadOnlyList<WatchStep> steps;
                List<CopyEdge> retryCopies;
                lock (this.watchGate)
                {
                    changes = this.registry.Classify(paths);
                    foreach (var locked in changes.Pending)
                    {
                        this.pendingPaths.Add(locked);
                    }

                    if (changes.Pending.Count > 0)
                    {
                        this.MarkEvent();
                    }

                    foreach (var cleared in this.registry.RefreshRed())
                    {
                        log.Information("WatchRedCleared BatchId={BatchId} Key={Key} Reason={Reason}", batchId, cleared, "output rebuilt outside watch");
                    }

                    this.registry.RefreshCopyEdges();
                    this.registry.Apply(changes);
                    if (changes.Changed.Count > 0)
                    {
                        // A save is the "next save" a given-up retry waits for.
                        this.bundleRetries.Renew();
                        this.referenceRetries.Renew();
                        this.copyRetries.Renew();
                    }

                    plan = this.registry.Plan();
                    steps = this.registry.Schedule(plan);
                    retryCopies = this.registry.CopyPending.ToList();
                }

                log.Information(
                    "WatchChange BatchId={BatchId} Changed={Changed} Dropped={Dropped} NotInputs={NotInputs} Pending={Pending} NeedsBuild={NeedsBuild} EventCount={EventCount} DebounceMs={DebounceMs}",
                    batchId,
                    changes.Changed,
                    changes.Dropped.Count,
                    changes.NotInputs,
                    changes.Pending,
                    changes.NeedsBuild.Select(n => Path.GetFileName(n.ProjectKey) + ": " + n.Reason).ToList(),
                    events,
                    debounceMs);
                foreach (var (projectKey, reason, _) in changes.NeedsBuild)
                {
                    log.Warning("WatchNeedsBuild BatchId={BatchId} Key={Key} Reason={Reason}", batchId, projectKey, reason);
                    this.WatchLog("NEEDS BUILD {0}: {1}; run dotnet build -p:NScriptWatch=true", Path.GetFileName(projectKey), reason);
                }

                foreach (var path in changes.Changed)
                {
                    this.WatchLog("change  {0}", path);
                }

                foreach (var path in changes.NotInputs)
                {
                    this.WatchLog("ignored {0} (not a compile input)", path);
                }

                if (plan.Compiles.Count == 0 && plan.Bundles.Count == 0 && retryCopies.Count == 0)
                {
                    return;
                }

                var patchNames = steps.Where(s => s.Patch).Select(s => s.Compile!.Name).ToList();
                var compileNames = steps.Where(s => s.Compile != null && !s.Patch).Select(s => s.Compile!.Name).ToList();
                log.Information(
                    "WatchBatchStart BatchId={BatchId} Compiles={Compiles} Patches={Patches} Bundles={Bundles} CopyRetries={CopyRetries}",
                    batchId,
                    compileNames,
                    patchNames,
                    plan.Bundles.Select(b => b.Name).ToList(),
                    retryCopies.Select(c => c.Target).ToList());
                this.Echo("batch {0}: compile [{1}] patch [{2}] emit [{3}]", batchId, string.Join(", ", compileNames), string.Join(", ", patchNames), string.Join(", ", plan.Bundles.Select(b => b.Name)));

                string result = "ok";
                long firstBundleMs = -1;
                int compiles = 0, patches = 0, emits = 0;
                foreach (var edge in retryCopies)
                {
                    if (!this.RunCopy(batchId, edge))
                    {
                        result = "failed";
                    }
                }

                // Each bundle is emitted as soon as what it reads has compiled, not after every
                // compile of the batch: on a framework save the first bundle lands seconds sooner.
                for (int index = 0; index < steps.Count; index++)
                {
                    var work = steps[index];
                    long sourceEventsNow;
                    lock (this.watchGate)
                    {
                        sourceEventsNow = this.sourceEvents;
                    }

                    if (index > 0 && sourceEventsNow != sourceEventsTaken)
                    {
                        log.Information("WatchBatchSuperseded BatchId={BatchId} StepsRun={StepsRun} StepsLeft={StepsLeft}", batchId, index, steps.Count - index);
                        this.WatchLog("superseded batch {0}: new changes arrived; {1} step(s) left for the next batch", batchId, steps.Count - index);
                        result = result == "ok" ? "superseded" : result;
                        break;
                    }

                    if (work.Compile == null)
                    {
                        result = this.RunEmitStep(batchId, work.Emit!, result, batchClock, debounceMs, ref firstBundleMs, ref emits);
                        continue;
                    }

                    var project = work.Compile;
                    string? blocked;
                    lock (this.watchGate)
                    {
                        blocked = this.registry.CompileBlockReason(project);
                    }

                    if (blocked != null)
                    {
                        log.Information("WatchStep BatchId={BatchId} Step={Step} Key={Key} Result={Result} Reason={Reason}", batchId, "compile", project.Key, "blocked", blocked);
                        this.WatchLog("BLOCKED compile {0}: {1}", project.Name, blocked);
                        result = "blocked";
                        continue;
                    }

                    var patched = work.Patch ? this.RunPatch(batchId, project) : (ResourcePatchOutcome?)null;
                    if (patched == ResourcePatchOutcome.IoFailed)
                    {
                        // The DLL is unchanged and the project stays dirty: its bundles keep
                        // their last good output until the next save patches again.
                        result = "failed";
                        continue;
                    }

                    if (patched == ResourcePatchOutcome.Ok)
                    {
                        patches++;
                    }
                    else
                    {
                        // A compile step, or a patch that fell back: compile in the same step.
                        compiles++;
                        var step = Stopwatch.StartNew();
                        var response = this.ExecuteLocked(
                            new ServiceRequest { Kind = ServiceProtocol.KindCompile, Cwd = project.Cwd, Args = project.Args },
                            "watch",
                            0,
                            out var inputs,
                            out _);
                        int exitCode = response.InternalError ? -1 : response.ExitCode;
                        bool ok = exitCode == 0;
                        var diagnostics = ok ? new List<string>() : DiagnosticLines(response.Stdout + response.Stderr).Take(20).ToList();
                        bool referenceUnreadable = !ok && diagnostics.Any(d => ReferenceUnreadableLine.IsMatch(d));
                        lock (this.watchGate)
                        {
                            this.registry.RecordCompileAttempt(project.Key, inputs, exitCode, referenceUnreadable);
                        }

                        log.Information(
                            "WatchStep BatchId={BatchId} Step={Step} Key={Key} ExitCode={ExitCode} ElapsedMs={ElapsedMs} RequestId={RequestId} Result={Result} Message={Message} Diagnostics={Diagnostics}",
                            batchId, "compile", project.Key, exitCode, step.ElapsedMilliseconds, response.RequestId, ok ? "ok" : referenceUnreadable ? "referenceUnreadable" : "failed", response.Message, diagnostics);
                        this.WatchLog("compile {0} {1} {2} ms", project.Name, ok ? "ok" : "FAILED", step.ElapsedMilliseconds);
                        if (ok)
                        {
                            lock (this.watchGate)
                            {
                                this.referenceRetries.Succeeded(project.Key);
                            }
                        }
                        else
                        {
                            result = "failed";
                            this.WatchLogDiagnostics(response);
                            if (referenceUnreadable)
                            {
                                this.RetryUnreadableReference(project);
                            }

                            continue;
                        }
                    }

                    List<CopyEdge> copies;
                    lock (this.watchGate)
                    {
                        copies = this.registry.CopiesOf(project.Key).ToList();
                    }

                    foreach (var edge in copies)
                    {
                        if (!this.RunCopy(batchId, edge))
                        {
                            result = "failed";
                        }
                    }
                }

                long totalMs = batchClock.ElapsedMilliseconds + debounceMs;
                log.Information(
                    "WatchBatchEnd BatchId={BatchId} Result={Result} ElapsedMs={ElapsedMs} FirstBundleMs={FirstBundleMs} DebounceMs={DebounceMs} Compiles={Compiles} Patches={Patches} Emits={Emits} RoslynEmits={RoslynEmits}",
                    batchId, result, totalMs, firstBundleMs, debounceMs, compiles, patches, emits, Interlocked.Read(ref this.roslynEmits));
                this.WatchLog("done    batch {0} {1} {2} ms (from first event)", batchId, result, totalMs);
                this.Echo("batch {0} {1} {2}ms", batchId, result, totalMs);
                this.lastBatchResult = result;
                this.lastBatch = $"{batchId} {result} {totalMs}ms";
            }
            finally
            {
                this.requestLock.Release();
            }
        }

        /// <summary>
        /// Emits <paramref name="bundle"/>, or keeps its last good output when a project it reads
        /// is blocked, red or not rebuilt. Returns the batch result after this step. Must be
        /// called under <see cref="requestLock"/>.
        /// </summary>
        private string RunEmitStep(long batchId, BundleRecord bundle, string result, Stopwatch batchClock, long debounceMs, ref long firstBundleMs, ref int emits)
        {
            var log = CompilerLog.ForComponent("Watch");
            string? kept;
            lock (this.watchGate)
            {
                kept = this.registry.BundleBlockReason(bundle);
            }

            if (kept != null)
            {
                log.Information("WatchStep BatchId={BatchId} Step={Step} Key={Key} Result={Result} Reason={Reason}", batchId, "emitJs", bundle.Key, "kept", kept);
                this.WatchLog("KEPT    {0} (last good; {1})", bundle.Key, kept);
                return result == "ok" ? "kept" : result;
            }

            emits++;
            var step = Stopwatch.StartNew();
            var response = this.ExecuteLocked(
                new ServiceRequest { Kind = ServiceProtocol.KindEmitJs, Cwd = bundle.Cwd, Args = bundle.Args },
                "watch",
                0,
                out _,
                out _);
            bool ok = !response.InternalError && response.ExitCode == 0;
            lock (this.watchGate)
            {
                this.registry.RecordBundleRun(bundle.Key, ok);
            }

            if (firstBundleMs < 0)
            {
                firstBundleMs = batchClock.ElapsedMilliseconds + debounceMs;
            }

            log.Information(
                "WatchStep BatchId={BatchId} Step={Step} Key={Key} ExitCode={ExitCode} ElapsedMs={ElapsedMs} RequestId={RequestId} Result={Result} Message={Message}",
                batchId, "emitJs", bundle.Key, response.ExitCode, step.ElapsedMilliseconds, response.RequestId, ok ? "ok" : "failed", response.Message);
            this.WatchLog("emit    {0} {1} {2} ms session={3}", bundle.Key, ok ? "ok" : "FAILED", step.ElapsedMilliseconds, BuilderSessions.LastBuild(bundle.Key) ?? "none");
            if (ok)
            {
                lock (this.watchGate)
                {
                    this.bundleRetries.Succeeded(bundle.Key);
                }

                return result;
            }

            this.WatchLogDiagnostics(response);
            this.RetryIfOutputInUse(bundle, response);
            return "failed";
        }

        /// <summary>
        /// Patches the changed resources of <paramref name="project"/> into its DLL and records
        /// the embedded bytes as seen. <see cref="ResourcePatchOutcome.Fallback"/> means the
        /// caller compiles instead. Must be called under <see cref="requestLock"/>.
        /// </summary>
        private ResourcePatchOutcome RunPatch(long batchId, ProjectRecord project)
        {
            var log = CompilerLog.ForComponent("Watch");
            var clock = Stopwatch.StartNew();
            ResourcePatchResult patch;
            try
            {
                patch = ResourcePatcher.Patch(project.Inputs.Output, project.Inputs.Resources);
            }
            catch (Exception ex)
            {
                // A Cecil failure nobody foresaw: logged as an error; the compile path still
                // produces the right DLL.
                log.Error(ex, "ResourcePatchFailed BatchId={BatchId} Project={Project} Outcome={Outcome} Error={Error}", batchId, project.Key, ResourcePatchOutcome.Fallback, ex.GetType().Name + ": " + ex.Message);
                this.WatchLog("patch   {0} not possible ({1}); compiling", project.Name, ex.Message);
                return ResourcePatchOutcome.Fallback;
            }

            if (patch.Outcome != ResourcePatchOutcome.Ok)
            {
                log.Warning("ResourcePatchFailed BatchId={BatchId} Project={Project} Outcome={Outcome} Error={Error} ElapsedMs={ElapsedMs}", batchId, project.Key, patch.Outcome, patch.Reason, clock.ElapsedMilliseconds);
                this.WatchLog(
                    patch.Outcome == ResourcePatchOutcome.Fallback ? "patch   {0} not possible ({1}); compiling" : "patch   {0} FAILED: {1}",
                    project.Name,
                    patch.Reason!);
                return patch.Outcome;
            }

            lock (this.watchGate)
            {
                this.registry.RecordPatch(project.Key, patch.Hashes);
            }

            log.Information(
                "ResourcePatch BatchId={BatchId} Project={Project} Resource={Resource} Bytes={Bytes} ElapsedMs={ElapsedMs} MvidBefore={MvidBefore} MvidAfter={MvidAfter}",
                batchId,
                project.Key,
                patch.Replaced.Select(r => r.Name).ToList(),
                patch.Replaced.Sum(r => r.Bytes),
                clock.ElapsedMilliseconds,
                patch.MvidBefore,
                patch.MvidAfter);
            this.WatchLog("patch   {0} [{1}] {2} ms", project.Name, string.Join(", ", patch.Replaced.Select(r => r.Name)), clock.ElapsedMilliseconds);
            return ResourcePatchOutcome.Ok;
        }

        /// <summary>
        /// Refreshes one MSBuild copy (DLL and sibling pdb, preserving the write time as
        /// MSBuild's Copy does). A reference-assembly copy happens only when the MVID changed,
        /// like CopyRefAssembly. A copy still locked after one retry becomes copy-pending: it
        /// blocks every reader and is retried at the next batch.
        /// </summary>
        private bool RunCopy(long batchId, CopyEdge edge)
        {
            var log = CompilerLog.ForComponent("Watch");
            if (edge.IsRefAssembly)
            {
                var source = ReadStamp(edge.Source);
                var target = ReadStamp(edge.Target);
                if (source != null && target != null && source.Value.Mvid == target.Value.Mvid)
                {
                    lock (this.watchGate)
                    {
                        this.registry.ClearCopyPending(edge);
                    }

                    return true;
                }
            }

            for (int attempt = 1; ; attempt++)
            {
                try
                {
                    File.Copy(edge.Source, edge.Target, overwrite: true);
                    var sourcePdb = Path.ChangeExtension(edge.Source, ".pdb");
                    var targetPdb = Path.ChangeExtension(edge.Target, ".pdb");
                    if (File.Exists(sourcePdb) && File.Exists(targetPdb))
                    {
                        File.Copy(sourcePdb, targetPdb, overwrite: true);
                    }

                    lock (this.watchGate)
                    {
                        this.registry.ClearCopyPending(edge);
                        this.copyRetries.Succeeded(edge.Target);
                    }

                    log.Information("WatchStep BatchId={BatchId} Step={Step} Key={Key} Target={Target} Result={Result}", batchId, "copy", edge.Source, edge.Target, "ok");
                    return true;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    if (attempt < 2)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    lock (this.watchGate)
                    {
                        this.registry.MarkCopyPending(edge);
                    }

                    log.Warning("WatchStep BatchId={BatchId} Step={Step} Key={Key} Target={Target} Result={Result} Message={Message}", batchId, "copy", edge.Source, edge.Target, "pending", ex.Message);
                    // Same schedule as an unreadable reference: the holder is usually a build or
                    // a test host that lets go within seconds.
                    RetryDecision decision;
                    int retry;
                    lock (this.watchGate)
                    {
                        decision = this.copyRetries.OnFailure(edge.Target, out retry);
                    }

                    if (decision == RetryDecision.Schedule)
                    {
                        this.WatchLog("COPY PENDING {0} ({1}); retry {2}/{3} in {4} s", edge.Target, ex.Message, retry, MaxReferenceRetries, ReferenceRetryDelay.TotalSeconds);
                        this.ScheduleRetry(edge.Target, ReferenceRetryDelay, this.copyRetries);
                    }
                    else if (decision == RetryDecision.GiveUp)
                    {
                        this.WatchLog("COPY PENDING {0} ({1}); gave up after {2} retries; retried on the next save", edge.Target, ex.Message, retry);
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Adds the source-kind files under every path that is a directory: a folder renamed or
        /// copied in stands for the files inside it. Returns the directories that vanished or
        /// could not be read while being read (a checkout, a temp folder); the files they held
        /// are classified by their own events.
        /// </summary>
        public static List<string> AddFilesOfNewDirectories(HashSet<string> paths, Func<string, bool> isDirectory)
        {
            var gone = new List<string>();
            foreach (var dir in paths.Where(isDirectory).ToList())
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Where(WatchRegistry.IsSourceKind))
                    {
                        paths.Add(file);
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    gone.Add(dir);
                }
            }

            return gone;
        }

        /// <summary>The compiler output lines watch.log shows for a failed step.</summary>
        public static IEnumerable<string> DiagnosticLines(string output)
        {
            foreach (var line in (output ?? string.Empty).Split('\n'))
            {
                var trimmed = line.TrimEnd('\r');
                if (ErrorLine.IsMatch(trimmed) || trimmed.Contains(": warning NSS", StringComparison.Ordinal))
                {
                    yield return trimmed;
                }
            }
        }

        /// <summary>
        /// A reader held the bundle through the writer's retries: re-arm a batch for it, at
        /// most <see cref="MaxBundleRetries"/> times. It stays dirty either way, so the next
        /// save emits it again.
        /// </summary>
        private void RetryIfOutputInUse(BundleRecord bundle, ServiceResponse response)
        {
            var output = (response.Stdout ?? string.Empty) + (response.Stderr ?? string.Empty) + (response.Message ?? string.Empty);
            if (!output.Contains(OwaSourceMapper.AtomicFile.InUseMessagePrefix, StringComparison.Ordinal))
            {
                return;
            }

            RetryDecision decision;
            int retry;
            lock (this.watchGate)
            {
                decision = this.bundleRetries.OnFailure(bundle.Key, out retry);
            }

            if (decision == RetryDecision.Schedule)
            {
                this.WatchLog("emit    {0} output in use; retry {1}/{2}", bundle.Key, retry, MaxBundleRetries);
                this.ScheduleRetry(bundle.Key, TimeSpan.Zero, this.bundleRetries);
            }
            else if (decision == RetryDecision.GiveUp)
            {
                this.WatchLog("KEPT    {0} (stale: output in use after {1} retries; save again)", bundle.Key, retry);
            }
        }

        /// <summary>
        /// A reference DLL could not be read (CS0006/CS0009, typically a plain build rewriting
        /// it): retry the still-dirty project after <see cref="ReferenceRetryDelay"/>, at most
        /// <see cref="MaxReferenceRetries"/> times; after that it waits for the next save.
        /// </summary>
        private void RetryUnreadableReference(ProjectRecord project)
        {
            RetryDecision decision;
            int retry;
            lock (this.watchGate)
            {
                decision = this.referenceRetries.OnFailure(project.Key, out retry);
            }

            if (decision == RetryDecision.Schedule)
            {
                this.WatchLog("compile {0}: reference unreadable; retry {1}/{2} in {3} s", project.Name, retry, MaxReferenceRetries, ReferenceRetryDelay.TotalSeconds);
                this.ScheduleRetry(project.Key, ReferenceRetryDelay, this.referenceRetries);
            }
            else if (decision == RetryDecision.GiveUp)
            {
                this.WatchLog("waiting {0}: reference still unreadable after {1} retries; save again", project.Name, retry);
            }
        }

        /// <summary>
        /// Queues <paramref name="path"/> (an output, so it classifies as dropped) after
        /// <paramref name="delay"/>; the batch it starts replays whatever is still dirty.
        /// <paramref name="budget"/> learns that its one timer for the path has fired.
        /// </summary>
        private void ScheduleRetry(string path, TimeSpan delay, RetryBudget budget)
        {
            void Queue()
            {
                lock (this.watchGate)
                {
                    budget.TimerFired(path);
                    this.pendingPaths.Add(path);
                    this.MarkEvent();
                }
            }

            if (delay <= TimeSpan.Zero)
            {
                Queue();
                return;
            }

            System.Threading.Tasks.Task.Delay(delay).ContinueWith(
                _ =>
                {
                    try
                    {
                        Queue();
                    }
                    catch (Exception ex)
                    {
                        this.WatchFatal(ex);
                    }
                },
                System.Threading.Tasks.TaskScheduler.Default);
        }

        private void WatchLogDiagnostics(ServiceResponse response)
        {
            foreach (var line in DiagnosticLines(response.Stdout + response.Stderr))
            {
                this.WatchLog("{0}", line);
            }

            if (response.InternalError)
            {
                this.WatchLog("internal error: {0}", response.Message);
            }

            if (response.Stdout.Contains("BadImageFormat", StringComparison.Ordinal)
                || (response.Message ?? string.Empty).Contains("BadImageFormat", StringComparison.Ordinal))
            {
                this.WatchLog("an input assembly is damaged; {0}", RebuildHint);
            }
        }

        /// <summary>
        /// <c>--sync</c>: whether the project whose obj DLL is <c>Args[0]</c>, the projects it
        /// reads and the bundles that include it are current. Files in their project folders
        /// the watch has not seen (new source files, deleted inputs) are queued like watcher
        /// events, and the answer waits, at most <c>Args[1]</c> seconds, until the watch loop
        /// has run everything queued. Runs outside <see cref="requestLock"/>, so the watch loop
        /// stays the only batch runner. Exit code: <see cref="SyncExitYes"/>,
        /// <see cref="SyncExitNo"/> or <see cref="SyncExitBusy"/>.
        /// </summary>
        private ServiceResponse Sync(ServiceRequest request)
        {
            var clock = Stopwatch.StartNew();
            if (request.Args == null || request.Args.Length != 2
                || !int.TryParse(request.Args[1], out int waitSeconds) || waitSeconds <= 0)
            {
                return SyncResponse(SyncExitNo, "nscript service: sync needs <obj dll> <wait seconds>");
            }

            string key = Path.GetFullPath(request.Args[0], string.IsNullOrEmpty(request.Cwd) ? Directory.GetCurrentDirectory() : request.Cwd);
            List<ProjectRecord>? closure;
            string? sameName;
            lock (this.watchGate)
            {
                closure = this.registry.SyncClosure(key)?.ToList();
                sameName = this.registry.Projects
                    .FirstOrDefault(p => string.Equals(p.Name, Path.GetFileName(key), StringComparison.OrdinalIgnoreCase))?.Key;
            }

            int exitCode;
            string? reason;
            var unseen = new List<string>();
            if (closure == null)
            {
                // The key is the DLL csc writes (obj), not MSBuild's bin copy.
                (exitCode, reason) = (SyncExitNo, sameName == null
                    ? "not watched (pass the obj DLL csc writes)"
                    : "not watched; the watched key for that name is " + sameName);
            }
            else if (!TryFindUnseenFiles(closure, unseen, out var listFailure))
            {
                (exitCode, reason) = (SyncExitNo, listFailure);
            }
            else
            {
                (exitCode, reason) = this.WaitForSettled(key, unseen, TimeSpan.FromSeconds(waitSeconds), clock);
            }

            // On yes, the references Sdk.targets passes instead of evaluating project references.
            var references = new StringBuilder();
            if (exitCode == SyncExitYes)
            {
                IReadOnlyList<(string ProjectDir, string Reference, string IntermediateDir)>? vouched;
                IReadOnlyList<string> foreignJs;
                lock (this.watchGate)
                {
                    vouched = this.registry.SyncReferences(key);
                    foreignJs = this.registry.ForeignJs(key);
                }

                foreach (var js in foreignJs)
                {
                    references.Append('\n').Append(SyncJsForeignPrefix).Append(js);
                }

                foreach (var (projectDir, reference, intermediateDir) in vouched ?? Array.Empty<(string, string, string)>())
                {
                    references.Append('\n').Append(SyncRefLine(projectDir, reference, intermediateDir));
                }
            }

            string word = exitCode == SyncExitYes ? "yes" : exitCode == SyncExitBusy ? "busy" : "no";
            string name = Path.GetFileName(key);
            CompilerLog.ForComponent("Watch").Information(
                "WatchSync Key={Key} Answer={Answer} Reason={Reason} Unseen={Unseen} WaitMs={WaitMs}",
                key, word, reason, unseen.Count, clock.ElapsedMilliseconds);
            this.WatchLog("sync    {0} {1}{2} {3} ms", name, word, reason == null ? "" : " (" + reason + ")", clock.ElapsedMilliseconds);
            // The answer line is shown in every build log: file names only (the reason in
            // watch.log keeps paths), except the not-watched hint, whose path is the point.
            string shown = reason == null ? "" : ": " + (closure == null ? reason : ShortPaths(reason));
            return SyncResponse(exitCode, $"nscript service: sync {word} {name}{shown} ({clock.ElapsedMilliseconds} ms){references}");
        }

        /// <summary>
        /// Replaces every absolute path in <paramref name="text"/> with its file name. Folder
        /// names may hold spaces; a segment never crosses a colon, a parenthesis or a line end.
        /// </summary>
        internal static string ShortPaths(string text)
            => Regex.Replace(text, @"(?:\b[A-Za-z]:|\\\\[^\\\s]+)\\(?:[^\\:;|()\r\n]+\\)*", string.Empty);

        private static ServiceResponse SyncResponse(int exitCode, string message)
            => new ServiceResponse { DaemonPid = Environment.ProcessId, ExitCode = exitCode, Message = message };

        /// <summary>
        /// Adds to <paramref name="unseen"/> the files of <paramref name="closure"/> the watch
        /// has not seen: source files in a project folder that are neither inputs nor were there
        /// at registration (a watcher overflow can lose their events), and inputs that are gone.
        /// </summary>
        private static bool TryFindUnseenFiles(IReadOnlyList<ProjectRecord> closure, List<string> unseen, out string? failure)
        {
            foreach (var project in closure)
            {
                try
                {
                    unseen.AddRange(EnumerateSourceFiles(project.Cwd)
                        .Where(f => !project.InputSet.Contains(f) && !project.PreexistingNonInputs.Contains(f)));
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    failure = $"cannot list {project.Cwd}: {ex.Message}";
                    return false;
                }

                unseen.AddRange(project.InputSet.Where(f => !File.Exists(f)));
            }

            failure = null;
            return true;
        }

        /// <summary>
        /// Queues <paramref name="unseen"/> and waits until nothing is queued and no batch runs,
        /// then asks the registry. A registration's replay kick is not waited for: whatever it
        /// would replay is dirty, so the answer is no anyway. Outside that window, work that
        /// nothing keeps any more is run first (<see cref="WatchRegistry.HasRunnableWork"/>).
        /// </summary>
        private (int ExitCode, string? Reason) WaitForSettled(string key, List<string> unseen, TimeSpan wait, Stopwatch clock)
        {
            lock (this.watchGate)
            {
                foreach (var path in unseen)
                {
                    this.pendingPaths.Add(path);
                }

                if (unseen.Count > 0)
                {
                    this.MarkEvent();
                }

                bool kicked = false;
                while (true)
                {
                    if (this.stopSource.IsCancellationRequested)
                    {
                        return (SyncExitBusy, "daemon stopping");
                    }

                    if (this.pendingPaths.Count == 0 && this.batchesTaken == this.batchesDone)
                    {
                        this.registry.RefreshRed();
                        this.registry.RefreshCopyPending();

                        // A copy a full build refreshed clears only here, so what it kept is
                        // still dirty with nothing keeping it: run that batch and wait for it,
                        // once per sync, rather than answer no for work the daemon can do now.
                        if (!kicked && this.registrationKickMs < 0 && this.registry.HasRunnableWork())
                        {
                            kicked = true;
                            this.pendingPaths.Add(Path.Combine(this.identity.RunDir, "sync"));
                            this.MarkEvent();
                            this.WatchLog("sync    {0}: running what a cleared copy kept", Path.GetFileName(key));
                            continue;
                        }

                        var blocker = this.registry.SyncBlocker(key);
                        return blocker == null ? (SyncExitYes, null) : (SyncExitNo, blocker);
                    }

                    var left = wait - clock.Elapsed;
                    if (left <= TimeSpan.Zero)
                    {
                        return (SyncExitBusy, this.batchesTaken != this.batchesDone
                            ? $"a batch still running after {wait.TotalSeconds:0} s"
                            : $"{this.pendingPaths.Count} change(s) still queued after {wait.TotalSeconds:0} s");
                    }

                    Monitor.Wait(this.watchGate, left < SyncPoll ? left : SyncPoll);
                }
            }
        }

        /// <summary>
        /// The file Sdk.targets (<c>_NScriptWatchSync</c>) checks before asking <c>--sync</c>:
        /// next to the project's obj DLL, so MSBuild finds it as <c>$(IntermediateOutputPath)</c>.
        /// </summary>
        public static string WatchMarkerPath(string projectKey)
            => Path.Combine(Path.GetDirectoryName(projectKey)!, WatchMarkerFileName);

        private string WatchMarkerText
            => $"pipe={this.pipeName}\ntoolset={this.identity.ToolsetHash}\npid={Environment.ProcessId}\n";

        private void WriteWatchMarker(string projectKey)
        {
            var path = WatchMarkerPath(projectKey);
            try
            {
                File.WriteAllText(path, this.WatchMarkerText);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                CompilerLog.ForComponent("Watch").Warning(ex, "WatchMarkerWriteFailed Path={Path}", path);
                this.WatchLog("marker  {0} not written ({1}); dotnet build runs in full", path, ex.Message);
            }
        }

        /// <summary>Deletes the markers this daemon wrote; a marker another daemon rewrote since is left.</summary>
        private void DeleteWatchMarkers()
        {
            List<string> keys;
            lock (this.watchGate)
            {
                keys = this.registry.Projects.Select(p => p.Key).ToList();
            }

            foreach (var path in keys.Select(WatchMarkerPath))
            {
                try
                {
                    if (File.Exists(path) && File.ReadAllText(path) == this.WatchMarkerText)
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    CompilerLog.ForComponent("Watch").Warning(ex, "WatchMarkerDeleteFailed Path={Path}", path);
                    this.WatchLog("marker  {0} not deleted ({1}); dotnet build will ask a stopped daemon and run in full", path, ex.Message);
                }
            }
        }

        private IEnumerable<KeyValuePair<string, string>> WatchStatusFields()
        {
            List<KeyValuePair<string, string>> fields;
            bool settled;
            lock (this.watchGate)
            {
                this.registry.RefreshRed();
                fields = this.registry.StatusFields().ToList();
                settled = this.registry.NeedsBuild.Count == 0
                    && this.registry.Red.Count == 0
                    && this.registry.CopyPending.Count == 0
                    && this.registry.Dirty.Count == 0
                    && this.registry.DirtyBundles.Count == 0;
            }

            // A registration (the build the log asked for) can clear what the last batch hit.
            string lastBatch = this.lastBatch;
            if (this.lastBatchResult != "ok" && settled)
            {
                lastBatch += "; since resolved, nothing blocked or stale";
            }

            fields.Add(new KeyValuePair<string, string>("WatchLastBatch", lastBatch));
            fields.Add(new KeyValuePair<string, string>("WatchLog", this.WatchLogPath));
            fields.Add(new KeyValuePair<string, string>("WatchNote", WatchNote));
            return fields;
        }
    }
}
