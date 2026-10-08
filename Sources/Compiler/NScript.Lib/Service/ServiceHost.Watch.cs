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
        public const string WatchNote = "while watching, build only with -p:NScriptWatch=true, or run nscript service --stop first; Sdk.props/Sdk.targets edits need a dotnet build";

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
        private long firstEventMs = -1;
        private long lastEventMs;
        private long batchCounter;
        private readonly RetryBudget bundleRetries = new RetryBudget(MaxBundleRetries);
        private string lastBatch = "none";

        // The result word of lastBatch: ok, failed, blocked or kept.
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
            this.registry = new WatchRegistry(ProbeFile, ReadStamp);
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

            bool watching;
            lock (this.watchGate)
            {
                watching = !this.registry.IsEmpty;
            }

            if (watching || ex != null)
            {
                CompilerLog.ForComponent("Watch").Information(ex, "WatchStop Reason={Reason}", reason);
                this.WatchLog("watch stopped: {0}{1}", reason, ex == null ? string.Empty : " " + ex);
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
                var response = this.ExecuteLocked(replay, "client", queueWaitMs, out var inputs, out _);
                if (inputs == null || response.InternalError)
                {
                    log.Warning("WatchRegister Kind={Kind} Skipped=true Reason={Reason}", request.Kind, response.InternalError ? response.Message : "arguments did not parse");
                    return response;
                }

                var existing = EnumerateSourceFiles(request.Cwd);
                var buildFiles = HashBuildFiles(request.Cwd, request.WatchSdkDir);
                ProjectRecord record;
                lock (this.watchGate)
                {
                    record = this.registry.RegisterCompile(request.Cwd, replayArgs, inputs, response.ExitCode, request.WatchSdkDir, existing, buildFiles);
                }

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
                    foreach (var input in this.registry.AllInputs())
                    {
                        this.pendingPaths.Add(input);
                    }

                    this.MarkEvent();
                }
            }
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
                        if (!this.registry.IsIgnored(candidate))
                        {
                            this.pendingPaths.Add(candidate);
                            any = true;
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

        private void OnWatcherError(string root, Exception ex)
        {
            try
            {
                CompilerLog.ForComponent("Watch").Warning(ex, "WatchOverflow Root={Root}", root);
                this.WatchLog("watcher error on {0} ({1}); rescanning", root, ex.Message);
                lock (this.watchGate)
                {
                    foreach (var input in this.registry.AllInputs())
                    {
                        this.pendingPaths.Add(input);
                    }

                    this.MarkEvent();
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
                    long events, debounceMs;
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
                        debounceMs = this.firstEventMs < 0 ? 0 : this.uptime.ElapsedMilliseconds - this.firstEventMs;
                        this.pendingPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        this.pendingEvents = 0;
                        this.firstEventMs = -1;
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

                        this.RunBatch(paths, events, debounceMs);
                    }
                    finally
                    {
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

        private void RunBatch(HashSet<string> paths, long events, long debounceMs)
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
                foreach (var (projectKey, reason) in changes.NeedsBuild)
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

                log.Information(
                    "WatchBatchStart BatchId={BatchId} Compiles={Compiles} Bundles={Bundles} CopyRetries={CopyRetries}",
                    batchId,
                    plan.Compiles.Select(p => p.Name).ToList(),
                    plan.Bundles.Select(b => b.Name).ToList(),
                    retryCopies.Select(c => c.Target).ToList());
                this.Echo("batch {0}: compile [{1}] emit [{2}]", batchId, string.Join(", ", plan.Compiles.Select(p => p.Name)), string.Join(", ", plan.Bundles.Select(b => b.Name)));

                string result = "ok";
                long firstBundleMs = -1;
                foreach (var edge in retryCopies)
                {
                    if (!this.RunCopy(batchId, edge))
                    {
                        result = "failed";
                    }
                }

                foreach (var project in plan.Compiles)
                {
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

                foreach (var bundle in plan.Bundles)
                {
                    string? kept;
                    lock (this.watchGate)
                    {
                        kept = this.registry.BundleBlockReason(bundle);
                    }

                    if (kept != null)
                    {
                        log.Information("WatchStep BatchId={BatchId} Step={Step} Key={Key} Result={Result} Reason={Reason}", batchId, "emitJs", bundle.Key, "kept", kept);
                        this.WatchLog("KEPT    {0} (last good; {1})", bundle.Key, kept);
                        if (result == "ok")
                        {
                            result = "kept";
                        }

                        continue;
                    }

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
                    this.WatchLog("emit    {0} {1} {2} ms", bundle.Key, ok ? "ok" : "FAILED", step.ElapsedMilliseconds);
                    if (ok)
                    {
                        lock (this.watchGate)
                        {
                            this.bundleRetries.Succeeded(bundle.Key);
                        }
                    }
                    else
                    {
                        result = "failed";
                        this.WatchLogDiagnostics(response);
                        this.RetryIfOutputInUse(bundle, response);
                    }
                }

                long totalMs = batchClock.ElapsedMilliseconds + debounceMs;
                log.Information(
                    "WatchBatchEnd BatchId={BatchId} Result={Result} ElapsedMs={ElapsedMs} FirstBundleMs={FirstBundleMs} DebounceMs={DebounceMs}",
                    batchId, result, totalMs, firstBundleMs, debounceMs);
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
