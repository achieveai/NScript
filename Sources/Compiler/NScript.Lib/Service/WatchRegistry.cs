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
        internal ProjectRecord(string cwd, string[] args, CompileInputs inputs, string? sdkDir, long seq)
        {
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

        /// <summary>Projects that need a <c>dotnet build</c> before watch can rebuild them, with the reason.</summary>
        public List<(string ProjectKey, string Reason)> NeedsBuild { get; } = new List<(string, string)>();

        internal HashSet<string> ChangedOwners { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        internal HashSet<string> CsOwners { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>The ordered work of one batch.</summary>
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
        private readonly Dictionary<string, string> needsBuild = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

        public IReadOnlyDictionary<string, string> NeedsBuild => this.needsBuild;

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
        /// </summary>
        public ProjectRecord RegisterCompile(
            string cwd,
            string[] replayArgs,
            CompileInputs inputs,
            int exitCode,
            string? sdkDir,
            IEnumerable<string> existingSourceFiles,
            IReadOnlyDictionary<string, string> buildFiles)
        {
            var record = new ProjectRecord(cwd, replayArgs, inputs, sdkDir, ++this.seq);
            foreach (var file in existingSourceFiles)
            {
                if (!record.InputSet.Contains(file))
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
            this.dirty.Remove(record.Key);
            this.compileDirty.Remove(record.Key);
            this.SetResult(record.Key, exitCode);
            return record;
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
            foreach (var (projectKey, reason) in changes.NeedsBuild)
            {
                this.needsBuild[projectKey] = reason;
            }

            this.lastOwners = new HashSet<string>(changes.ChangedOwners, StringComparer.OrdinalIgnoreCase);
            foreach (var owner in changes.ChangedOwners)
            {
                this.dirty.Add(owner);
            }

            var queue = new Queue<string>(changes.CsOwners);
            var seen = new HashSet<string>(changes.CsOwners, StringComparer.OrdinalIgnoreCase);
            this.compileDirty.UnionWith(changes.CsOwners);
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
        /// then dirty bundles: those whose entry project changed this window first, in
        /// registration order; then the rest, most recently registered first (the app the
        /// user built last).
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
            if (this.needsBuild.TryGetValue(project.Key, out var reason))
            {
                return "needs dotnet build: " + reason;
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
                if (this.needsBuild.TryGetValue(project.Key, out var reason))
                {
                    return project.Name + " needs dotnet build: " + reason;
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
        /// Adds copy edges that the files on disk prove now: a recorded reference with the
        /// same file name and the same MVID as a project's output is MSBuild's copy of it.
        /// Edges are only added here; a stored edge lives until either record is replaced, so
        /// consecutive edits (new MVIDs on both sides) keep refreshing the copy.
        /// </summary>
        public void RefreshCopyEdges()
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
                        foreach (var path in paths)
                        {
                            if (Same(path, output)
                                || !string.Equals(Path.GetFileName(path), name, StringComparison.OrdinalIgnoreCase)
                                || this.copyEdges.Any(e => Same(e.Source, output) && Same(e.Target, path)))
                            {
                                continue;
                            }

                            outputStamp ??= this.readStamp(output);
                            var targetStamp = this.readStamp(path);
                            if (outputStamp != null && targetStamp != null && outputStamp.Value.Mvid == targetStamp.Value.Mvid)
                            {
                                this.copyEdges.Add(new CopyEdge(project.Key, output, path, holderKey)
                                {
                                    IsRefAssembly = Same(output, project.Inputs.RefOut),
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>The copies to refresh after <paramref name="projectKey"/> compiled.</summary>
        public IReadOnlyList<CopyEdge> CopiesOf(string projectKey)
            => this.copyEdges.Where(e => Same(e.ProjectKey, projectKey))
                .GroupBy(e => e.Target, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

        public void MarkCopyPending(CopyEdge edge) => this.copyPending.Add(edge);

        public void ClearCopyPending(CopyEdge edge) => this.copyPending.Remove(edge);

        /// <summary>
        /// Clears red marks whose obj DLL changed since the failure: somebody built it (for
        /// example a plain <c>dotnet build</c>). Returns the cleared project keys.
        /// </summary>
        public IReadOnlyList<string> RefreshRed()
        {
            var cleared = new List<string>();
            foreach (var pair in this.red.ToList())
            {
                var now = this.readStamp(pair.Key);
                if (!Nullable.Equals(now, pair.Value))
                {
                    this.red.Remove(pair.Key);
                    cleared.Add(pair.Key);
                }
            }

            return cleared;
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
                string.Join(";", this.needsBuild.Select(p => Path.GetFileName(p.Key) + ": " + p.Value)));
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
                        result.NeedsBuild.Add((owner.Key, "deleted " + path));
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
                result.NeedsBuild.Add((project.Key, "build file changed: " + path));
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
                result.NeedsBuild.Add((project.Key, "new file " + path));
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
            if (this.needsBuild.ContainsKey(project.Key))
            {
                return project.Name + " needs dotnet build";
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
