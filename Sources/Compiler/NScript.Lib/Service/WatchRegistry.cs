namespace NScript.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NScript.Csc.Lib.Service;

    /// <summary>What a probe of one path found after the debounce.</summary>
    public enum FileProbeState
    {
        /// <summary>The file does not exist.</summary>
        Missing,

        /// <summary>The file exists but could not be read after the retries.</summary>
        Locked,

        /// <summary>The file exists; <see cref="FileProbe.Hash"/> holds its content hash.</summary>
        Present,
    }

    /// <summary>Result of probing one path: existence and content hash.</summary>
    public readonly record struct FileProbe(FileProbeState State, string? Hash)
    {
        public static FileProbe Missing => new FileProbe(FileProbeState.Missing, null);

        public static FileProbe Locked => new FileProbe(FileProbeState.Locked, null);

        public static FileProbe Present(string hash) => new FileProbe(FileProbeState.Present, hash);
    }

    /// <summary>Identity and write time of an output assembly on disk.</summary>
    public readonly record struct AssemblyStamp(Guid Mvid, DateTime MtimeUtc);

    /// <summary>A stage-1 compile recorded for watch mode, keyed by its full output path.</summary>
    public sealed class ProjectRecord
    {
        internal ProjectRecord(string cwd, string[] args, CompileInputs inputs, string? sdkDir, long seq, string? propsHash)
        {
            this.PropsHash = propsHash;
            this.Cwd = Path.TrimEndingDirectorySeparator(Path.GetFullPath(cwd));
            this.Args = args;
            this.Inputs = inputs;
            this.SdkDir = string.IsNullOrWhiteSpace(sdkDir) ? null : Path.TrimEndingDirectorySeparator(Path.GetFullPath(sdkDir));
            this.Seq = seq;
            this.Hashes = new Dictionary<string, string>(inputs.Hashes, StringComparer.OrdinalIgnoreCase);
            this.InputSet = new HashSet<string>(inputs.Sources.Concat(inputs.Resources), StringComparer.OrdinalIgnoreCase);
            this.SourceSet = new HashSet<string>(inputs.Sources, StringComparer.OrdinalIgnoreCase);
        }

        public string Key => this.Inputs.Output;

        public string Name => Path.GetFileName(this.Inputs.Output);

        public string Cwd { get; }

        /// <summary>The args to replay: the recorded ones with every @rsp pointing at a daemon-owned snapshot.</summary>
        public string[] Args { get; }

        public CompileInputs Inputs { get; }

        public string? SdkDir { get; }

        public long Seq { get; }

        /// <summary>The compile properties hash of the watch build that registered this project (null: not sent).</summary>
        public string? PropsHash { get; }

        /// <summary>At registration: the <see cref="PropsHash"/> of each registered project this one reads, by key.</summary>
        public IReadOnlyDictionary<string, string?> ReadPropsHashes { get; internal set; } = new Dictionary<string, string?>();

        /// <summary>Content hashes the latest compile attempt saw (refreshed after every attempt).</summary>
        public Dictionary<string, string> Hashes { get; private set; }

        /// <summary>Source-kind files in the project tree that existed at registration but are not inputs.</summary>
        public HashSet<string> PreexistingNonInputs { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>csproj/props/targets that define this project, with their hashes at registration.</summary>
        public Dictionary<string, string> BuildFiles { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        internal HashSet<string> InputSet { get; }

        internal HashSet<string> SourceSet { get; }

        internal IEnumerable<string> Outputs
            => this.Inputs.RefOut == null ? new[] { this.Inputs.Output } : new[] { this.Inputs.Output, this.Inputs.RefOut };

        internal void ReplaceHashes(IReadOnlyDictionary<string, string> hashes)
            => this.Hashes = new Dictionary<string, string>(hashes, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>A stage-2 emit recorded for watch mode, keyed by its full -outJs path.</summary>
    public sealed class BundleRecord
    {
        internal BundleRecord(string key, string cwd, string[] args, string entry, IReadOnlyList<string> references, long seq)
        {
            this.Key = key;
            this.Cwd = cwd;
            this.Args = args;
            this.Entry = entry;
            this.References = references;
            this.Seq = seq;
        }

        public string Key { get; }

        public string Name => Path.GetFileName(this.Key);

        public string Cwd { get; }

        public string[] Args { get; }

        public string Entry { get; }

        public IReadOnlyList<string> References { get; }

        public long Seq { get; }

        internal IEnumerable<string> Reads => new[] { this.Entry }.Concat(this.References);
    }

    /// <summary>A copy MSBuild made in the recorded build (obj to bin/lib, refint to ref), refreshed by watch.</summary>
    public sealed record CopyEdge(string ProjectKey, string Source, string Target, string HolderKey)
    {
        /// <summary>True for the reference-assembly copy (refint to ref), which MSBuild only makes when the MVID changes.</summary>
        public bool IsRefAssembly { get; init; }
    }

    /// <summary>The outcome of classifying the paths of one debounce window.</summary>
    public sealed class WatchChanges
    {
        /// <summary>Paths whose content differs from what the owner's last compile saw.</summary>
        public List<string> Changed { get; } = new List<string>();

        /// <summary>Paths with no effect: same hash, ignored location, temp files that came and went.</summary>
        public List<string> Dropped { get; } = new List<string>();

        /// <summary>
        /// Source-kind files in a watched project that no compile reads (they existed at
        /// registration). Also in <see cref="Dropped"/>; listed so a stale edit is visible.
        /// </summary>
        public List<string> NotInputs { get; } = new List<string>();

        /// <summary>Paths still locked after the retries; they go to the next window.</summary>
        public List<string> Pending { get; } = new List<string>();

        /// <summary>
        /// Projects that need a <c>dotnet build</c> before watch can rebuild them, with the reason
        /// and, for a new source file, that file (the reason lapses when it is gone again).
        /// </summary>
        public List<(string ProjectKey, string Reason, string? AddedFile)> NeedsBuild { get; } = new List<(string, string, string?)>();

        internal HashSet<string> ChangedOwners { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        internal HashSet<string> CsOwners { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>The ordered work of one batch.</summary>
    /// <summary>
    /// What changed in a registering project after MSBuild evaluated it (D-F001): source-kind
    /// files in its folder created or written since, and its build files written since. The
    /// compile could not have read them, so the registration must not vouch for them.
    /// </summary>
    public sealed record ChangedSinceEvaluation(IReadOnlyCollection<string> SourceFiles, IReadOnlyCollection<string> BuildFiles);

    public sealed record WatchPlan(IReadOnlyList<ProjectRecord> Compiles, IReadOnlyList<BundleRecord> Bundles);

    /// <summary>
    /// One step of a batch: a compile (<see cref="Compile"/>) or an emit (<see cref="Emit"/>).
    /// A compile step with <see cref="Patch"/> set only had resources change: patch them into
    /// the DLL instead of running Roslyn.
    /// </summary>
    public sealed record WatchStep(ProjectRecord? Compile, BundleRecord? Emit, bool Patch = false);

    /// <summary>
    /// Watch-mode state: the compile and emit requests built with <c>NScriptWatch=true</c>,
    /// what each one read, and the planner that turns changed paths into ordered replays.
    /// Its only IO goes through the injected probe and assembly-stamp functions, so tests
    /// fake both. Not thread-safe: the host serializes access.
    /// </summary>
    public sealed class WatchRegistry
    {
        private static readonly string[] SourceKindExtensions = { ".cs", ".cshtml", ".css", ".html", ".xhtml" };

        private static readonly string[] BuildFileExtensions = { ".csproj", ".props", ".targets" };

        private readonly Func<string, FileProbe> probe;
        private readonly Func<string, AssemblyStamp?> readStamp;
        private readonly Dictionary<string, ProjectRecord> projects = new Dictionary<string, ProjectRecord>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, BundleRecord> bundles = new Dictionary<string, BundleRecord>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> dirty = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Dirty projects a resource patch cannot bring up to date: a C# input changed, or a
        // dependency's did. Cleared only by a compile attempt or a new registration.
        private readonly HashSet<string> compileDirty = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> dirtyBundles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, AssemblyStamp?> red = new Dictionary<string, AssemblyStamp?>(StringComparer.OrdinalIgnoreCase);
        // Every reason a project needs a dotnet build, latest last; AddedFile is set for "new file".
        private readonly Dictionary<string, List<(string Reason, string? AddedFile)>> needsBuild = new Dictionary<string, List<(string, string?)>>(StringComparer.OrdinalIgnoreCase);

        // Hash of the JS each bundle's last successful daemon emit left on disk (D-S3-1).
        private readonly Dictionary<string, string> emittedJs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Stamp of each obj DLL as the daemon last left it (registration, compile attempt or
        // patch); another stamp at sync means another build rewrote it (RefreshForeignOutputs).
        private readonly Dictionary<string, AssemblyStamp> written = new Dictionary<string, AssemblyStamp>(StringComparer.OrdinalIgnoreCase);
        private readonly List<CopyEdge> copyEdges = new List<CopyEdge>();
        private readonly HashSet<CopyEdge> copyPending = new HashSet<CopyEdge>();
        private HashSet<string> lastOwners = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private long seq;

        public WatchRegistry(Func<string, FileProbe> probe, Func<string, AssemblyStamp?> readStamp)
        {
            this.probe = probe;
            this.readStamp = readStamp;
        }

        public IReadOnlyCollection<ProjectRecord> Projects => this.projects.Values;

        public IReadOnlyCollection<BundleRecord> Bundles => this.bundles.Values;

        public bool IsEmpty => this.projects.Count == 0 && this.bundles.Count == 0;

        public IReadOnlyCollection<string> Dirty => this.dirty;

        public IReadOnlyCollection<string> DirtyBundles => this.dirtyBundles;

        /// <summary>Projects that need a <c>dotnet build</c>, each with its latest reason.</summary>
        public IReadOnlyDictionary<string, string> NeedsBuild
            => this.needsBuild.ToDictionary(p => p.Key, p => p.Value[^1].Reason, StringComparer.OrdinalIgnoreCase);

        public IReadOnlyCollection<string> Red => this.red.Keys;

        public IReadOnlyList<CopyEdge> CopyEdges => this.copyEdges;

        public IReadOnlyCollection<CopyEdge> CopyPending => this.copyPending;

        /// <summary>
        /// False forces every dirty project through a Roslyn compile, even when only its
        /// resources changed (<c>NSCRIPT_RESOURCE_PATCH=off</c>, read at daemon start).
        /// </summary>
        public bool ResourcePatchEnabled { get; set; } = true;

        /// <summary>
        /// True when <paramref name="extension"/> (with dot) is a kind a compile can read:
        /// C#, Razor skin, CSS or XWML.
        /// </summary>
        public static bool IsSourceKind(string path)
            => SourceKindExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

        public static bool IsBuildFile(string path)
            => BuildFileExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Records a compile request (MSBuild-driven, Watch=true), whatever its exit code, so a
        /// session that starts red still recovers. Replaces an earlier record for the same output.
        /// It clears the project's NEEDS BUILD reasons, then raises again whatever
        /// <paramref name="sinceEvaluation"/> shows the compile could not have seen: a new
        /// non-input file, an edited build file, a missing input. Null (an SDK that does not
        /// send the evaluation time): every reason clears.
        /// </summary>
        public ProjectRecord RegisterCompile(
            string cwd,
            string[] replayArgs,
            CompileInputs inputs,
            int exitCode,
            string? sdkDir,
            IEnumerable<string> existingSourceFiles,
            IReadOnlyDictionary<string, string> buildFiles,
            ChangedSinceEvaluation? sinceEvaluation = null,
            string? propsHash = null)
        {
            var record = new ProjectRecord(cwd, replayArgs, inputs, sdkDir, ++this.seq, propsHash);
            var late = new HashSet<string>(sinceEvaluation?.SourceFiles ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            late.ExceptWith(record.InputSet);
            foreach (var file in existingSourceFiles)
            {
                // A file added after evaluation is new, never "there at registration".
                if (!record.InputSet.Contains(file) && !late.Contains(file))
                {
                    record.PreexistingNonInputs.Add(file);
                }
            }

            foreach (var pair in buildFiles)
            {
                record.BuildFiles[pair.Key] = pair.Value;
            }

            this.projects[record.Key] = record;
            this.copyEdges.RemoveAll(e => Same(e.ProjectKey, record.Key) || Same(e.HolderKey, record.Key));
            this.copyPending.RemoveWhere(e => Same(e.ProjectKey, record.Key) || Same(e.HolderKey, record.Key));
            this.needsBuild.Remove(record.Key);
            if (sinceEvaluation != null)
            {
                var reasons = late.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).Select(f => ("new file " + f, (string?)f))
                    .Concat(sinceEvaluation.BuildFiles.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).Select(f => ("build file changed: " + f, (string?)null)))
                    .Concat(record.InputSet.Where(f => this.probe(f).State == FileProbeState.Missing).OrderBy(f => f, StringComparer.OrdinalIgnoreCase).Select(f => ("deleted " + f, (string?)null)))
                    .ToList();
                if (reasons.Count > 0)
                {
                    this.needsBuild[record.Key] = reasons;
                }
            }

            this.dirty.Remove(record.Key);
            this.compileDirty.Remove(record.Key);
            this.SetResult(record.Key, exitCode);
            this.RecordWritten(record.Key);
            this.RefreshCopyEdges(record.Key);
            record.ReadPropsHashes = this.DependenciesOf(record).ToDictionary(p => p.Key, p => p.PropsHash, StringComparer.OrdinalIgnoreCase);
            return record;
        }

        /// <summary>
        /// Why a build whose compile properties hash to <paramref name="propsHash"/> must not
        /// take the daemon's outputs for <paramref name="projectKey"/>, or null when it may (F-017).
        /// The daemon replays each watch build's command line, so the key must have registered
        /// with that hash, and every project it reads must still have the hash it had then: a
        /// watch build with other properties that stopped before the key registered them again.
        /// A missing hash on either side is a difference.
        /// </summary>
        public string? PropsBlocker(string projectKey, string? propsHash)
        {
            if (!this.projects.TryGetValue(projectKey, out var project))
            {
                return "not watched";
            }

            if (string.IsNullOrEmpty(propsHash) || project.PropsHash != propsHash)
            {
                return "build properties differ from the watch build";
            }

            foreach (var dependency in this.DependenciesOf(project).OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
            {
                if (dependency.PropsHash == null
                    || !project.ReadPropsHashes.TryGetValue(dependency.Key, out var seen)
                    || seen != dependency.PropsHash)
                {
                    return $"build properties differ from the watch build: {dependency.Name} was registered again with other properties";
                }
            }

            return null;
        }

        /// <summary>Records an emit request (Watch=true). Replaces an earlier record for the same -outJs.</summary>
        public BundleRecord RegisterBundle(string cwd, string[] args, string outJs, string entry, IEnumerable<string> references)
        {
            string Full(string path) => Path.GetFullPath(path, cwd);
            var key = Full(outJs);
            var record = new BundleRecord(key, cwd, args, Full(entry), references.Select(Full).ToList(), ++this.seq);
            this.bundles[key] = record;
            this.copyEdges.RemoveAll(e => Same(e.HolderKey, key));
            this.copyPending.RemoveWhere(e => Same(e.HolderKey, key));
            this.dirtyBundles.Remove(key);
            this.RefreshCopyEdges(key);
            return record;
        }

        /// <summary>
        /// Refresh rule: after every compile attempt of a registered project (watch replay,
        /// pass or fail), the recorded hashes become that attempt's pre-compile snapshot, the
        /// project leaves the dirty set, and its red mark follows the exit code.
        /// </summary>
        public void RecordCompileAttempt(string projectKey, CompileInputs? inputs, int exitCode, bool referenceUnreadable = false)
        {
            if (referenceUnreadable)
            {
                // A reference DLL was locked or half-written (a plain build rewriting it): the
                // sources are not at fault, so no red; the project stays dirty and is retried.
                return;
            }

            var record = this.projects[projectKey];
            if (inputs != null)
            {
                record.ReplaceHashes(inputs.Hashes);
            }

            this.dirty.Remove(projectKey);
            this.compileDirty.Remove(projectKey);
            this.SetResult(projectKey, exitCode);
            this.RecordWritten(projectKey);
        }

        /// <summary>
        /// True when <paramref name="project"/> is dirty only because resource files changed,
        /// so patching them into its DLL equals a compile: no C# change in it or in a
        /// dependency, not red (its sources failed; a compile must prove them again) and not
        /// waiting for a <c>dotnet build</c>.
        /// </summary>
        public bool IsPatchOnly(ProjectRecord project)
            => this.ResourcePatchEnabled
                && this.dirty.Contains(project.Key)
                && !this.compileDirty.Contains(project.Key)
                && !this.red.ContainsKey(project.Key)
                && !this.needsBuild.ContainsKey(project.Key);

        /// <summary>
        /// A resource patch made the DLL embed the files with <paramref name="hashes"/>: record
        /// them as seen, like a compile does for every input, and the project is clean.
        /// </summary>
        public void RecordPatch(string projectKey, IReadOnlyDictionary<string, string> hashes)
        {
            var record = this.projects[projectKey];
            foreach (var pair in hashes)
            {
                record.Hashes[pair.Key] = pair.Value;
            }

            this.dirty.Remove(projectKey);
            this.RecordWritten(projectKey);
        }

        /// <summary>
        /// A bundle ran. Only a successful emit makes its output reflect the current DLLs; a
        /// failed one stays dirty, so the next batch emits it again.
        /// </summary>
        public void RecordBundleRun(string bundleKey, bool succeeded)
        {
            if (succeeded)
            {
                this.dirtyBundles.Remove(bundleKey);
                var written = this.probe(bundleKey);
                if (written.State == FileProbeState.Present)
                {
                    this.emittedJs[bundleKey] = written.Hash!;
                }
                else
                {
                    this.emittedJs.Remove(bundleKey);
                }
            }
        }

        /// <summary>
        /// True when <paramref name="path"/> is under a registered project's obj or bin folder
        /// (MSBuild's and watch's own outputs).
        /// </summary>
        public bool IsIgnored(string path)
        {
            foreach (var project in this.projects.Values)
            {
                var rel = RelativeUnder(project.Cwd, path);
                if (rel == null)
                {
                    continue;
                }

                var first = rel.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];
                if (string.Equals(first, "obj", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(first, "bin", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Classifies the paths of one debounce window by what is on disk now, never by the
        /// event kind: an editor that renames the original away and writes a new file (VS,
        /// vim) ends up as "exists, hash differs". Only an input still missing is a deletion.
        /// </summary>
        public WatchChanges Classify(IEnumerable<string> paths)
        {
            var result = new WatchChanges();
            var expanded = new HashSet<string>(paths, StringComparer.OrdinalIgnoreCase);

            // A directory event (rename, delete) stands for every recorded input below it.
            foreach (var path in expanded.ToList())
            {
                var prefix = Path.TrimEndingDirectorySeparator(path) + Path.DirectorySeparatorChar;
                foreach (var input in this.projects.Values.SelectMany(p => p.InputSet))
                {
                    if (input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        expanded.Add(input);
                    }
                }
            }

            foreach (var path in expanded.OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                if (this.IsIgnored(path))
                {
                    result.Dropped.Add(path);
                    continue;
                }

                var owners = this.projects.Values.Where(p => p.InputSet.Contains(path)).ToList();
                if (owners.Count > 0)
                {
                    this.ClassifyInput(path, owners, result);
                    continue;
                }

                var name = Path.GetFileName(path);
                if (name.StartsWith(".", StringComparison.Ordinal))
                {
                    result.Dropped.Add(path);
                    continue;
                }

                if (IsBuildFile(path))
                {
                    this.ClassifyBuildFile(path, result);
                    continue;
                }

                if (IsSourceKind(path))
                {
                    this.ClassifyNewFile(path, result);
                    continue;
                }

                result.Dropped.Add(path);
            }

            return result;
        }

        /// <summary>
        /// Folds a classification into the dirty sets: changed owners, plus the transitive
        /// dependents of every owner with a C# change (a resource-only change recompiles only
        /// its owner), plus every bundle that includes a dirty project.
        /// </summary>
        public void Apply(WatchChanges changes)
        {
            foreach (var (projectKey, reason, addedFile) in changes.NeedsBuild)
            {
                if (!this.needsBuild.TryGetValue(projectKey, out var reasons))
                {
                    this.needsBuild[projectKey] = reasons = new List<(string, string?)>();
                }

                reasons.RemoveAll(r => r.Reason == reason);
                reasons.Add((reason, addedFile));
            }

            var lapsed = this.DropVanishedAddedFiles();

            // An owner stays "changed" while a bundle whose entry it built is still dirty: a
            // superseded batch carries its emits over, and the next window may change nothing.
            this.lastOwners.RemoveWhere(o => !this.projects.TryGetValue(o, out var owner)
                || !this.dirtyBundles.Any(b => this.Owned(owner).Contains(this.bundles[b].Entry)));
            this.lastOwners.UnionWith(changes.ChangedOwners);
            foreach (var owner in changes.ChangedOwners)
            {
                this.dirty.Add(owner);
            }

            this.dirty.UnionWith(lapsed);
            var recompile = changes.CsOwners.Concat(lapsed).ToList();
            var queue = new Queue<string>(recompile);
            var seen = new HashSet<string>(recompile, StringComparer.OrdinalIgnoreCase);
            this.compileDirty.UnionWith(recompile);
            while (queue.Count > 0)
            {
                var key = queue.Dequeue();
                foreach (var dependent in this.DependentsOf(key))
                {
                    if (seen.Add(dependent.Key))
                    {
                        this.dirty.Add(dependent.Key);
                        this.compileDirty.Add(dependent.Key);
                        queue.Enqueue(dependent.Key);
                    }
                }
            }

            foreach (var key in this.dirty.Concat(this.needsBuild.Keys))
            {
                foreach (var bundle in this.BundlesIncluding(key))
                {
                    this.dirtyBundles.Add(bundle.Key);
                }
            }
        }

        /// <summary>
        /// The batch's work: dirty compiles in dependency order (ties by registration order),
        /// then dirty bundles: those whose entry project changed (this window, or an earlier
        /// one whose bundle is still dirty, as after a supersede) first, in registration
        /// order; then the rest, most recently registered first (the app the user built last).
        /// </summary>
        public WatchPlan Plan()
        {
            var dirtyProjects = this.dirty.Select(k => this.projects[k]).ToList();
            var ordered = new List<ProjectRecord>();
            var remaining = new HashSet<string>(this.dirty, StringComparer.OrdinalIgnoreCase);
            while (remaining.Count > 0)
            {
                var ready = dirtyProjects
                    .Where(p => remaining.Contains(p.Key)
                        && !this.DependenciesOf(p).Any(d => remaining.Contains(d.Key)))
                    .OrderBy(p => p.Seq)
                    .FirstOrDefault()
                    // A cycle cannot come from MSBuild; if it ever does, fall back to registration order.
                    ?? dirtyProjects.Where(p => remaining.Contains(p.Key)).OrderBy(p => p.Seq).First();
                ordered.Add(ready);
                remaining.Remove(ready.Key);
            }

            bool OwnerChanged(BundleRecord b) => this.lastOwners.Any(o => this.Owned(this.projects[o]).Contains(b.Entry));
            var orderedBundles = this.dirtyBundles
                .Select(k => this.bundles[k])
                .OrderBy(b => OwnerChanged(b) ? 0 : 1)
                .ThenBy(b => OwnerChanged(b) ? b.Seq : -b.Seq)
                .ToList();
            return new WatchPlan(ordered, orderedBundles);
        }

        /// <summary>
        /// The plan as steps, each bundle emitted as soon as what it reads has compiled: for
        /// each bundle in plan order, its compiles not yet scheduled (with their dirty
        /// dependencies) in plan order, then its emit. Compiles no bundle reads come last.
        /// Plan order is topological, so every subsequence of it is too.
        /// </summary>
        public IReadOnlyList<WatchStep> Schedule(WatchPlan plan)
        {
            var planned = new HashSet<string>(plan.Compiles.Select(p => p.Key), StringComparer.OrdinalIgnoreCase);
            var scheduled = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var steps = new List<WatchStep>();
            foreach (var bundle in plan.Bundles)
            {
                var needed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var queue = new Queue<ProjectRecord>(plan.Compiles.Where(p => this.Includes(bundle, p)));
                while (queue.Count > 0)
                {
                    var project = queue.Dequeue();
                    if (needed.Add(project.Key))
                    {
                        foreach (var dependency in this.DependenciesOf(project).Where(d => planned.Contains(d.Key)))
                        {
                            queue.Enqueue(dependency);
                        }
                    }
                }

                foreach (var project in plan.Compiles.Where(p => needed.Contains(p.Key) && scheduled.Add(p.Key)))
                {
                    steps.Add(new WatchStep(project, null, this.IsPatchOnly(project)));
                }

                steps.Add(new WatchStep(null, bundle));
            }

            foreach (var project in plan.Compiles.Where(p => scheduled.Add(p.Key)))
            {
                steps.Add(new WatchStep(project, null, this.IsPatchOnly(project)));
            }

            return steps;
        }

        /// <summary>Why <paramref name="project"/> must not compile now, or null.</summary>
        public string? CompileBlockReason(ProjectRecord project)
        {
            if (this.NeedsBuildReason(project.Key) is string reason)
            {
                return "needs dotnet build -p:NScriptWatch=true: " + reason;
            }

            foreach (var dependency in this.DependenciesOf(project))
            {
                var blocked = this.ProjectBlocker(dependency, project.Inputs.References);
                if (blocked != null)
                {
                    return blocked;
                }
            }

            return null;
        }

        /// <summary>Why <paramref name="bundle"/> must keep its last good output, or null.</summary>
        public string? BundleBlockReason(BundleRecord bundle)
        {
            foreach (var project in this.projects.Values.Where(p => this.Includes(bundle, p)))
            {
                if (this.NeedsBuildReason(project.Key) is string reason)
                {
                    return project.Name + " needs dotnet build -p:NScriptWatch=true: " + reason;
                }

                var blocked = this.ProjectBlocker(project, bundle.Reads);
                if (blocked != null)
                {
                    return blocked;
                }
            }

            return null;
        }

        /// <summary>
        /// True when a dirty compile or bundle has nothing keeping it now, so a batch would run
        /// it: what a copy that just cleared (<see cref="RefreshCopyPending"/>) was keeping.
        /// </summary>
        public bool HasRunnableWork()
            => this.dirty.Any(k => this.CompileBlockReason(this.projects[k]) == null)
                || this.dirtyBundles.Any(b => this.BundleBlockReason(this.bundles[b]) == null);

        /// <summary>
        /// The project whose obj DLL is <paramref name="projectKey"/> and the registered projects
        /// it reads, or null when it is not registered. One level is the whole closure: an SDK
        /// project's csc references include every transitive project output.
        /// </summary>
        public IReadOnlyList<ProjectRecord>? SyncClosure(string projectKey)
            => this.projects.TryGetValue(projectKey, out var project)
                ? new[] { project }.Concat(this.DependenciesOf(project)).ToList()
                : null;

        /// <summary>
        /// Why <paramref name="projectKey"/> is not current for a build that skips its project
        /// references, or null when it is: it and the projects it reads are not dirty, red,
        /// waiting for a <c>dotnet build</c> or a copy, every bundle including it is emitted,
        /// and every copy of a registered output that they read matches it (<see cref="StaleCopy"/>).
        /// </summary>
        public string? SyncBlocker(string projectKey)
        {
            if (!this.projects.TryGetValue(projectKey, out var project))
            {
                return "not watched";
            }

            foreach (var dependency in this.DependenciesOf(project))
            {
                var blocked = this.ProjectBlocker(dependency, project.Inputs.References);
                if (blocked != null)
                {
                    return blocked;
                }
            }

            var own = this.ProjectBlocker(project, Array.Empty<string>());
            if (own != null)
            {
                return own;
            }

            foreach (var bundle in this.BundlesIncluding(project.Key).OrderBy(b => b.Seq))
            {
                var blocked = this.BundleBlockReason(bundle)
                    ?? (this.dirtyBundles.Contains(bundle.Key) ? Path.GetFileName(bundle.Key) + " not emitted" : null);
                if (blocked != null)
                {
                    return blocked;
                }
            }

            var reads = this.SyncClosure(projectKey)!.SelectMany(p => p.Inputs.References)
                .Concat(this.BundlesIncluding(project.Key).SelectMany(b => b.References));
            return this.StaleCopy(reads);
        }

        /// <summary>
        /// The JS files of the bundles including <paramref name="projectKey"/> that are not what
        /// the daemon's last successful emit left: rewritten since (another configuration or
        /// mode), deleted, or never emitted by it. A synced build keeps only the daemon's JS.
        /// </summary>
        public IReadOnlyList<string> ForeignJs(string projectKey)
            => this.projects.ContainsKey(projectKey)
                ? this.BundlesIncluding(projectKey)
                    .Where(b => !this.emittedJs.TryGetValue(b.Key, out var hash) || this.probe(b.Key) != FileProbe.Present(hash))
                    .Select(b => b.Key)
                    .ToList()
                : Array.Empty<string>();

        /// <summary>
        /// The csc references of <paramref name="projectKey"/> that are another registered
        /// project's output or a copy of one (same file name), each with that project's
        /// directory and intermediate directory (where its watch build recorded its target path);
        /// null when it is not registered or a name matches two projects. A synced
        /// <c>dotnet build</c> passes these instead of evaluating its project references:
        /// <see cref="SyncBlocker"/> has just vouched for each of them.
        /// </summary>
        public IReadOnlyList<(string ProjectDir, string Reference, string IntermediateDir)>? SyncReferences(string projectKey)
        {
            if (!this.projects.TryGetValue(projectKey, out var project))
            {
                return null;
            }

            var byName = this.projects.Values.Where(p => !Same(p.Key, projectKey)).ToLookup(p => p.Name, StringComparer.OrdinalIgnoreCase);
            var result = new List<(string, string, string)>();
            foreach (var reference in project.Inputs.References)
            {
                var owners = byName[Path.GetFileName(reference)].ToList();
                if (owners.Count > 1)
                {
                    return null;
                }

                if (owners.Count == 1)
                {
                    result.Add((owners[0].Cwd, reference, Path.GetDirectoryName(owners[0].Key)!));
                }
            }

            return result;
        }

        /// <summary>
        /// The first of <paramref name="reads"/> that is a copy of a registered output (same
        /// file name) and matches none: an assembly copy needs the obj DLL's MVID and write
        /// time (a resource patch keeps the MVID), a reference-assembly copy needs the
        /// <c>/refout</c>'s MVID (MSBuild skips that copy while the MVID is unchanged). An
        /// unreadable copy is stale. It reads the disk, not copy edges, so an edge that was
        /// never found cannot turn into a wrong yes.
        /// </summary>
        private string? StaleCopy(IEnumerable<string> reads)
        {
            var byName = this.projects.Values.ToLookup(p => p.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var path in reads.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var owners = byName[Path.GetFileName(path)].Where(p => !p.Outputs.Any(o => Same(o, path))).ToList();
                if (owners.Count == 0)
                {
                    continue;
                }

                var copy = this.readStamp(path);
                if (copy == null)
                {
                    return path + " unreadable (a copy of " + owners[0].Name + ")";
                }

                if (owners.Any(p => (this.readStamp(p.Key) is AssemblyStamp obj && CopyMatches(obj, copy.Value, mvidOnly: false))
                    || (p.Inputs.RefOut != null && this.readStamp(p.Inputs.RefOut) is AssemblyStamp refOut && CopyMatches(refOut, copy.Value, mvidOnly: true))))
                {
                    continue;
                }

                return path + " stale (" + owners[0].Name + " changed since it was copied)";
            }

            return null;
        }

        /// <summary>
        /// Adds copy edges that the files on disk prove now: a recorded reference with the
        /// same file name and the same MVID as a project's output is MSBuild's copy of it.
        /// A same-named reference that exists but cannot be read (a share-none lock) is taken
        /// as the copy too, with the copy pending, so its readers are kept, never skipped; it
        /// pairs with the reference assembly only when it sits in a <c>ref</c> folder.
        /// Edges are only added here; a stored edge lives until either record is replaced, so
        /// consecutive edits (new MVIDs on both sides) keep refreshing the copy.
        /// </summary>
        /// <param name="onlyKey">
        /// A registration passes its record's key and refreshes only that record's edges: a
        /// parallel build may be mid-copy on other projects' outputs. A batch refreshes all.
        /// </param>
        public void RefreshCopyEdges(string? onlyKey = null)
        {
            var holders = this.projects.Values.Select(p => (Key: p.Key, Paths: (IEnumerable<string>)p.Inputs.References))
                .Concat(this.bundles.Values.Select(b => (Key: b.Key, Paths: b.Reads)))
                .ToList();
            foreach (var project in this.projects.Values)
            {
                foreach (var output in project.Outputs)
                {
                    var name = Path.GetFileName(output);
                    AssemblyStamp? outputStamp = null;
                    foreach (var (holderKey, paths) in holders)
                    {
                        if (onlyKey != null && !Same(project.Key, onlyKey) && !Same(holderKey, onlyKey))
                        {
                            continue;
                        }

                        foreach (var path in paths)
                        {
                            if (Same(path, output)
                                || !string.Equals(Path.GetFileName(path), name, StringComparison.OrdinalIgnoreCase)
                                || this.copyEdges.Any(e => Same(e.Source, output) && Same(e.Target, path)))
                            {
                                continue;
                            }

                            outputStamp ??= this.readStamp(output);
                            if (outputStamp == null)
                            {
                                continue;
                            }

                            bool isRef = Same(output, project.Inputs.RefOut);
                            var targetStamp = this.readStamp(path);
                            bool unreadable = targetStamp == null
                                && this.probe(path).State != FileProbeState.Missing
                                && isRef == string.Equals(Path.GetFileName(Path.GetDirectoryName(path)), "ref", StringComparison.OrdinalIgnoreCase);
                            // Found by MVID alone: a copy is its source's copy while stale too.
                            if (unreadable || (targetStamp != null && CopyMatches(outputStamp.Value, targetStamp.Value, mvidOnly: true)))
                            {
                                var edge = new CopyEdge(project.Key, output, path, holderKey) { IsRefAssembly = isRef };
                                this.copyEdges.Add(edge);
                                if (unreadable)
                                {
                                    this.copyPending.Add(edge);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The one copy-freshness rule (F-006). <paramref name="mvidOnly"/>: a reference-assembly
        /// copy matches by MVID, as MSBuild copies it only when the MVID changes. Otherwise an
        /// assembly copy matches by MVID and write time: MSBuild's Copy keeps the write time,
        /// and a resource patch keeps the MVID but not the write time.
        /// </summary>
        public static bool CopyMatches(AssemblyStamp source, AssemblyStamp copy, bool mvidOnly)
            => mvidOnly ? source.Mvid == copy.Mvid : source == copy;

        /// <summary>The copies to refresh after <paramref name="projectKey"/> compiled.</summary>
        public IReadOnlyList<CopyEdge> CopiesOf(string projectKey)
            => this.copyEdges.Where(e => Same(e.ProjectKey, projectKey))
                .GroupBy(e => e.Target, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

        public void MarkCopyPending(CopyEdge edge) => this.copyPending.Add(edge);

        public void ClearCopyPending(CopyEdge edge) => this.copyPending.Remove(edge);

        /// <summary>
        /// Clears pending copies whose target now equals its source, as after the full build
        /// that copied it (S3 D-S3-3): the obj DLL by MVID and mtime (the rule StaleCopy uses),
        /// the reference assembly by MVID (MSBuild copies it only when the MVID changes). A
        /// target that is still stale or cannot be read stays pending; nothing is copied here.
        /// </summary>
        public void RefreshCopyPending()
        {
            this.copyPending.RemoveWhere(edge =>
            {
                var target = this.readStamp(edge.Target);
                var source = this.readStamp(edge.Source);
                return target != null && source != null && CopyMatches(source.Value, target.Value, edge.IsRefAssembly);
            });
        }

        /// <summary>
        /// Clears red marks whose obj DLL changed since the failure: somebody built it (for
        /// example a plain <c>dotnet build</c>). A DLL that cannot be read (deleted by a clean,
        /// locked mid-write) is not a rebuild and keeps red. Returns the cleared project keys.
        /// </summary>
        public IReadOnlyList<string> RefreshRed()
        {
            var cleared = new List<string>();
            foreach (var pair in this.red.ToList())
            {
                var now = this.readStamp(pair.Key);
                if (now != null && !Nullable.Equals(now, pair.Value))
                {
                    this.red.Remove(pair.Key);
                    cleared.Add(pair.Key);
                }
            }

            return cleared;
        }

        /// <summary>
        /// Marks for a compile the projects whose obj DLL another build rewrote or deleted
        /// since the daemon last left it (a <c>-p:DefineConstants=X</c> build compiles other
        /// code into it), with their dependents and bundles, as for a C# save. The batch that
        /// runs them answers the sync, never a standing no. A failed compile records the DLL
        /// it left too, so red never recompiles in a loop. A locked DLL waits for the next
        /// sync. Returns the marked project keys.
        /// </summary>
        public IReadOnlyList<string> RefreshForeignOutputs()
        {
            var changes = new WatchChanges();
            foreach (var pair in this.written)
            {
                var now = this.readStamp(pair.Key);
                if (now != null ? now.Value != pair.Value : this.probe(pair.Key).State == FileProbeState.Missing)
                {
                    changes.ChangedOwners.Add(pair.Key);
                    changes.CsOwners.Add(pair.Key);
                }
            }

            if (changes.CsOwners.Count > 0)
            {
                this.Apply(changes);
            }

            return changes.CsOwners.ToList();
        }

        /// <summary>Every input of every project, for a rescan after a watcher overflow.</summary>
        public IEnumerable<string> AllInputs() => this.projects.Values.SelectMany(p => p.InputSet).Distinct(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Directories to watch recursively: every project directory and every directory of a
        /// compiled file outside its project directory (linked files), as a minimal cover.
        /// </summary>
        public IReadOnlyList<string> RecursiveRoots()
        {
            var dirs = new List<string>();
            foreach (var project in this.projects.Values)
            {
                dirs.Add(project.Cwd);
                foreach (var input in project.InputSet)
                {
                    if (RelativeUnder(project.Cwd, input) == null)
                    {
                        dirs.Add(Path.GetDirectoryName(input)!);
                    }
                }
            }

            return MinimalCover(dirs);
        }

        /// <summary>Directories watched non-recursively for build files: those of recorded build files not under a recursive root.</summary>
        public IReadOnlyList<string> BuildFileRoots()
        {
            var recursive = this.RecursiveRoots();
            return this.projects.Values
                .SelectMany(p => p.BuildFiles.Keys)
                .Select(p => Path.GetDirectoryName(p)!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(d => !recursive.Any(r => RelativeUnder(r, d) != null || Same(r, d)))
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>The <c>--status</c> watch fields.</summary>
        public IEnumerable<KeyValuePair<string, string>> StatusFields()
        {
            yield return new KeyValuePair<string, string>("WatchProjects", this.projects.Count.ToString());
            yield return new KeyValuePair<string, string>("WatchBundles", this.bundles.Count.ToString());
            yield return new KeyValuePair<string, string>("WatchRed", string.Join(";", this.red.Keys.Select(Path.GetFileName)));
            yield return new KeyValuePair<string, string>(
                "WatchNeedsBuild",
                string.Join(";", this.needsBuild.Select(p => Path.GetFileName(p.Key) + ": " + p.Value[^1].Reason)));
            yield return new KeyValuePair<string, string>(
                "WatchCopyPending",
                string.Join(";", this.copyPending.Select(e => e.Target)));
            yield return new KeyValuePair<string, string>("WatchResourcePatch", this.ResourcePatchEnabled ? "on" : "off");
        }

        internal static IReadOnlyList<string> MinimalCover(IEnumerable<string> dirs)
        {
            var sorted = dirs.Select(d => Path.TrimEndingDirectorySeparator(Path.GetFullPath(d)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d.Length)
                .ToList();
            var cover = new List<string>();
            foreach (var dir in sorted)
            {
                if (!cover.Any(c => RelativeUnder(c, dir) != null))
                {
                    cover.Add(dir);
                }
            }

            return cover.OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static string? RelativeUnder(string dir, string path)
        {
            var prefix = Path.TrimEndingDirectorySeparator(dir) + Path.DirectorySeparatorChar;
            return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && path.Length > prefix.Length
                ? path.Substring(prefix.Length)
                : null;
        }

        private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        private void ClassifyInput(string path, List<ProjectRecord> owners, WatchChanges result)
        {
            var found = this.probe(path);
            switch (found.State)
            {
                case FileProbeState.Missing:
                    foreach (var owner in owners)
                    {
                        result.NeedsBuild.Add((owner.Key, "deleted " + path, null));
                    }

                    break;
                case FileProbeState.Locked:
                    result.Pending.Add(path);
                    break;
                default:
                    var changedOwners = owners
                        .Where(o => !o.Hashes.TryGetValue(path, out var recorded) || recorded != found.Hash)
                        .ToList();
                    if (changedOwners.Count == 0)
                    {
                        result.Dropped.Add(path);
                        break;
                    }

                    result.Changed.Add(path);
                    foreach (var owner in changedOwners)
                    {
                        result.ChangedOwners.Add(owner.Key);
                        if (owner.SourceSet.Contains(path))
                        {
                            result.CsOwners.Add(owner.Key);
                        }
                    }

                    break;
            }
        }

        private void ClassifyBuildFile(string path, WatchChanges result)
        {
            var dir = Path.GetDirectoryName(path)!;
            var affected = this.projects.Values
                .Where(p => p.BuildFiles.ContainsKey(path)
                    || Same(p.Cwd, dir)
                    || RelativeUnder(p.Cwd, path) != null
                    || (p.SdkDir != null && Same(p.SdkDir, dir)))
                .ToList();
            if (affected.Count == 0)
            {
                result.Dropped.Add(path);
                return;
            }

            var found = this.probe(path);
            if (found.State == FileProbeState.Locked)
            {
                result.Pending.Add(path);
                return;
            }

            var changed = affected
                .Where(p => !(found.State == FileProbeState.Present
                    && p.BuildFiles.TryGetValue(path, out var recorded)
                    && recorded == found.Hash)
                    && !(found.State == FileProbeState.Missing && !p.BuildFiles.ContainsKey(path)))
                .ToList();
            if (changed.Count == 0)
            {
                result.Dropped.Add(path);
                return;
            }

            foreach (var project in changed)
            {
                result.NeedsBuild.Add((project.Key, "build file changed: " + path, null));
            }
        }

        private void ClassifyNewFile(string path, WatchChanges result)
        {
            var containing = this.projects.Values.Where(p => RelativeUnder(p.Cwd, path) != null).ToList();
            if (containing.Count == 0)
            {
                result.Dropped.Add(path);
                return;
            }

            var found = this.probe(path);
            if (found.State == FileProbeState.Missing)
            {
                // An editor temp file that came and went within the window.
                result.Dropped.Add(path);
                return;
            }

            if (found.State == FileProbeState.Locked)
            {
                result.Pending.Add(path);
                return;
            }

            var added = containing.Where(p => !p.PreexistingNonInputs.Contains(path)).ToList();
            if (added.Count == 0)
            {
                result.Dropped.Add(path);
                result.NotInputs.Add(path);
                return;
            }

            foreach (var project in added)
            {
                result.NeedsBuild.Add((project.Key, "new file " + path, path));
            }
        }

        private string? NeedsBuildReason(string projectKey)
            => this.needsBuild.TryGetValue(projectKey, out var reasons) ? reasons[^1].Reason : null;

        /// <summary>
        /// A "new file" reason lapses once that file is gone again (added, then deleted before
        /// any build). Other reasons stay until a build. A plain build may have compiled the
        /// file meanwhile, so the projects whose reason lapsed are returned to recompile.
        /// </summary>
        private List<string> DropVanishedAddedFiles()
        {
            var lapsed = new List<string>();
            foreach (var pair in this.needsBuild.ToList())
            {
                if (pair.Value.RemoveAll(r => r.AddedFile != null && this.probe(r.AddedFile).State == FileProbeState.Missing) > 0)
                {
                    lapsed.Add(pair.Key);
                }

                if (pair.Value.Count == 0)
                {
                    this.needsBuild.Remove(pair.Key);
                }
            }

            return lapsed;
        }

        private void RecordWritten(string projectKey)
        {
            if (this.readStamp(projectKey) is AssemblyStamp stamp)
            {
                this.written[projectKey] = stamp;
            }
            else
            {
                this.written.Remove(projectKey);
            }
        }

        private void SetResult(string projectKey, int exitCode)
        {
            if (exitCode == 0)
            {
                this.red.Remove(projectKey);
            }
            else
            {
                this.red[projectKey] = this.readStamp(projectKey);
            }
        }

        /// <summary>Why a bundle or dependent reading <paramref name="reads"/> must not use <paramref name="project"/> now, or null.</summary>
        private string? ProjectBlocker(ProjectRecord project, IEnumerable<string> reads)
        {
            if (this.NeedsBuildReason(project.Key) is string reason)
            {
                return project.Name + " needs dotnet build -p:NScriptWatch=true: " + reason;
            }

            if (this.red.ContainsKey(project.Key))
            {
                return project.Name + " red";
            }

            if (this.dirty.Contains(project.Key))
            {
                return project.Name + " not rebuilt";
            }

            var readSet = new HashSet<string>(reads, StringComparer.OrdinalIgnoreCase);
            var pending = this.copyPending.FirstOrDefault(e => Same(e.ProjectKey, project.Key) && readSet.Contains(e.Target));
            return pending != null ? "copy pending " + pending.Target : null;
        }

        private HashSet<string> Owned(ProjectRecord project)
        {
            var owned = new HashSet<string>(project.Outputs, StringComparer.OrdinalIgnoreCase);
            foreach (var edge in this.copyEdges.Where(e => Same(e.ProjectKey, project.Key)))
            {
                owned.Add(edge.Target);
            }

            return owned;
        }

        private IEnumerable<ProjectRecord> DependenciesOf(ProjectRecord project)
            => this.projects.Values.Where(p => !Same(p.Key, project.Key) && this.Owned(p).Overlaps(project.Inputs.References));

        private IEnumerable<ProjectRecord> DependentsOf(string projectKey)
        {
            var owned = this.Owned(this.projects[projectKey]);
            return this.projects.Values.Where(p => !Same(p.Key, projectKey) && owned.Overlaps(p.Inputs.References));
        }

        private bool Includes(BundleRecord bundle, ProjectRecord project) => this.Owned(project).Overlaps(bundle.Reads);

        private IEnumerable<BundleRecord> BundlesIncluding(string projectKey)
        {
            var project = this.projects[projectKey];
            return this.bundles.Values.Where(b => this.Includes(b, project));
        }
    }
}
