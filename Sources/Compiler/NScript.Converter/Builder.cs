//-----------------------------------------------------------------------
// <copyright file="Builder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter
{
    using System.Collections.Generic;
    using System.IO;
    using NScript.CLR;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using NScript.Utils;
    using Mono.Cecil;
    using System.Linq;
    using NScript.JST.Visitors;

    /// <summary>
    /// Definition for Builder.
    /// </summary>
    public class Builder : System.IDisposable
    {
        /// <summary>
        /// The main assembly.
        /// </summary>
        private readonly string mainAssembly;

        /// <summary>
        /// The js script.
        /// </summary>
        private readonly string jsScript;

        /// <summary>
        /// The references.
        /// </summary>
        private readonly string[] references;

        /// <summary>
        /// The plugins given to the constructor; <see cref="Execute()"/> builds with them.
        /// </summary>
        private readonly IConverterPlugin[] ctorPlugins;

        /// <summary>
        /// The plugins of the running build.
        /// </summary>
        private IRuntimeConverterPlugin[] plugins;

        /// <summary>
        /// The method converter plugins of the running build.
        /// </summary>
        private IMethodConverterPlugin[] methodConverterPlugins;

        /// <summary>
        /// The type converter plugins of the running build.
        /// </summary>
        private ITypeConverterPlugin[] typeConverterPlugins;

        /// <summary>
        /// Build session (slice 2, Inc 3; dev mode only): the loaded modules and the converter
        /// context kept from the last successful build, reused while no input file changed.
        /// </summary>
        private ClrContext sessionClr;

        private ConverterContext sessionContext;

        /// <summary>
        /// The session's converted methods (dev chunks only); dropped with the session, since a
        /// cold build loads new modules.
        /// </summary>
        private MethodCache methodCache;

        /// <summary>
        /// Dev patch emit (<c>NSCRIPT_PATCH_EMIT=1</c>): what the last full emit left for
        /// patching its bundle; dropped with the session and by any miss.
        /// </summary>
        private DevPatchSession patchSession;

        /// <summary>
        /// A method cache hit failed its check, or a plugin asked for a retry
        /// (<see cref="ConverterContext.RetryReason"/>): the build runs once more.
        /// </summary>
        private bool retryBuild;

        /// <summary>
        /// (full path, SHA-256 of the content) of every input the session was built from.
        /// Content, not length and mtime: a rewrite can keep both (cp -p, restored packages,
        /// two writes in one timestamp tick).
        /// </summary>
        private List<(string path, string sha256)> sessionStamps;

        private readonly int jsParts;

        private readonly (bool minify, bool uglify, bool optimize) scriptGenerateSettings;

        /// <summary>
        /// Optional <c>sourceRoot</c> to write into the generated source map.
        /// Empty/null means fall back to the legacy <c>SrcMapper.ashx?js=...&amp;fname=</c> handler path.
        /// </summary>
        private readonly string sourceMapRoot;

        /// <summary>
        /// Optional absolute path to the Git repo root. When set together with
        /// <see cref="sourceMapRoot"/>, <c>sources[]</c> entries are rebased to repo-relative,
        /// forward-slash paths so they combine correctly with a remote-repo sourceRoot URL.
        /// </summary>
        private readonly string repoRoot;

        /// <summary>
        /// Optional raw-file base URL for a second repository (e.g. a deployed framework or
        /// shared library) whose sources should resolve from a different remote than
        /// <see cref="sourceMapRoot"/>. Files under <see cref="secondaryRepoRoot"/> are
        /// emitted into <c>sources[]</c> as absolute <c>https://</c> URLs prefixed by this
        /// value — V3 spec says DevTools uses such absolute entries directly, bypassing the
        /// primary <see cref="sourceMapRoot"/>.
        /// </summary>
        private readonly string secondarySourceRoot;

        /// <summary>
        /// Optional absolute path to the secondary repository's worktree. Paired with
        /// <see cref="secondarySourceRoot"/> to enable the multi-repo source map emission.
        /// </summary>
        private readonly string secondaryRepoRoot;

        /// <summary>
        /// Dev mode: identity-derived stable names and hashed type ids (slice 2, Inc 1).
        /// </summary>
        private readonly bool devMode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsScript">               The js script. </param>
        /// <param name="mainAssembly">           The main assembly. </param>
        /// <param name="references">             The references. </param>
        /// <param name="plugins">                The plugins. </param>
        /// <param name="typeConverterPlugins">   The type converter plugins. </param>
        /// <param name="methodConverterPlugins"> The method converter plugins. </param>
        /// <param name="sourceMapRoot">          Optional <c>sourceRoot</c> URL to embed in the
        ///     generated source map. When null or empty, the compiler emits the legacy
        ///     <c>SrcMapper.ashx</c> handler path and drops the handler sidecar alongside the map. </param>
        /// <param name="repoRoot">                Optional absolute path to the Git repo root.
        ///     When supplied alongside <paramref name="sourceMapRoot"/>, <c>sources[i]</c> entries
        ///     are emitted as forward-slash, repo-relative paths so they combine with a remote
        ///     repo URL. Files outside the repo root keep the legacy absolutized form. </param>
        /// <param name="secondarySourceRoot">     Optional raw-file base URL for a second
        ///     repository. Sources living under <paramref name="secondaryRepoRoot"/> are emitted
        ///     as absolute <c>https://</c> URLs prefixed by this value so DevTools bypasses the
        ///     primary <paramref name="sourceMapRoot"/> for them. </param>
        /// <param name="secondaryRepoRoot">       Optional absolute path to the worktree of a
        ///     second repository whose files should be served from the secondary remote. </param>
        public Builder(
            string jsScript,
            int jsParts,
            string mainAssembly,
            string[] references,
            IConverterPlugin[] plugins,
            (bool minify, bool uglify, bool optimize) scriptGenerateSettings,
            string sourceMapRoot = null,
            string repoRoot = null,
            string secondarySourceRoot = null,
            string secondaryRepoRoot = null,
            bool devMode = false)
        {
            this.mainAssembly = mainAssembly;
            this.jsScript = jsScript;
            this.references = references;
            this.ctorPlugins = plugins;
            this.jsParts = jsParts;
            this.scriptGenerateSettings = scriptGenerateSettings;
            this.sourceMapRoot = sourceMapRoot;
            this.repoRoot = repoRoot;
            this.secondarySourceRoot = secondarySourceRoot;
            this.secondaryRepoRoot = secondaryRepoRoot;
            this.devMode = devMode;
        }

        /// <summary>
        /// Resets process-wide state that one build leaves behind, so a long-lived process
        /// (the build service) starts every build as a fresh process would: the Cecil
        /// comparer's reference-keyed hash cache and the sticky error flag of the Logger.
        /// </summary>
        public static void ResetProcessState()
        {
            MemberReferenceComparer.Instance.ClearCache();
            Logger.Instance = new Logger();
        }

        /// <summary>
        /// "cold" when the last <see cref="Execute()"/> loaded its inputs, "warm" when it reused
        /// the session; null before the first build.
        /// </summary>
        public string LastBuildKind { get; private set; }

        /// <summary>
        /// Why the last build was warm or cold (the Session.Build reason), with the refresh's miss
        /// reason when one was tried and missed; null before the first build.
        /// </summary>
        public string LastBuildReason { get; private set; }

        /// <summary>Why the last dev patch emit missed, or null when it patched.</summary>
        public string LastPatchMiss { get; private set; }

        /// <summary>
        /// Dev patch emit: the build service's live patch socket, asked at each full emit that
        /// bakes the client. Null, or null from it, bakes a client that only checks its script.
        /// </summary>
        public System.Func<HmrEndpoint> HmrEndpointSource { get; set; }

        /// <summary>
        /// What the last <see cref="Execute()"/> wrote means for open pages: a full emit with
        /// the client, or a patch. Null when it wrote no bundle with a client.
        /// </summary>
        public LivePatch LastLivePatch { get; private set; }

        /// <summary>The last full emit's generation, which its patches carry.</summary>
        private string hmrGen;

        /// <summary>
        /// Whether builds keep a session: dev mode, unless <c>NSCRIPT_SESSION=off</c>.
        /// </summary>
        private bool UseSession =>
            this.devMode && System.Environment.GetEnvironmentVariable("NSCRIPT_SESSION") != "off";

        /// <summary>
        /// Releases the session's modules (and the files the resolver read).
        /// </summary>
        public void Dispose() => this.DropSession();

        private void DropSession()
        {
            this.sessionClr?.Dispose();
            this.sessionClr = null;
            this.sessionContext = null;
            this.sessionStamps = null;
            this.methodCache = null;
            this.patchSession = null;
        }

        private void SetPlugins(IConverterPlugin[] buildPlugins)
        {
            this.plugins = (from p in buildPlugins where p is IRuntimeConverterPlugin select p as IRuntimeConverterPlugin)
                .ToArray<IRuntimeConverterPlugin>();
            this.methodConverterPlugins = (from p in buildPlugins where p is IMethodConverterPlugin select p as IMethodConverterPlugin)
                .ToArray<IMethodConverterPlugin>();
            this.typeConverterPlugins = (from p in buildPlugins where p is IRuntimeConverterPlugin select p as ITypeConverterPlugin)
                .ToArray<ITypeConverterPlugin>();
        }

        /// <summary>
        /// Hashes every input, in parallel. <paramref name="changedImages"/> keeps the bytes of
        /// each input whose hash differs from the session stamp at the same index, so a refresh
        /// takes exactly the bytes its stamp describes without reading the file again.
        /// </summary>
        private List<(string path, string sha256)> ReadInputStamps(out byte[][] changedImages)
        {
            var paths = this.references.Append(this.mainAssembly).Select(Path.GetFullPath).ToArray();
            var hashes = new string[paths.Length];
            var images = new byte[paths.Length][];
            var session = this.sessionStamps;
            try
            {
                System.Threading.Tasks.Parallel.For(0, paths.Length, index =>
                {
                    var image = File.ReadAllBytes(paths[index]);
                    hashes[index] = System.Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(image));
                    if (session != null && index < session.Count && session[index].path == paths[index] && session[index].sha256 != hashes[index])
                    {
                        images[index] = image;
                    }
                });
            }
            catch (System.AggregateException ex)
            {
                // Callers see the read's own exception, as they did when this ran in sequence.
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerExceptions[0]).Throw();
                throw;
            }

            changedImages = images;
            return paths.Select((path, index) => (path, hashes[index])).ToList();
        }

        /// <summary>
        /// Keeps the session when the changed inputs differ only in resources (a watch-mode
        /// resource patch) or, for a recompiled module, only in method bodies of the types its
        /// changed source files declare (<see cref="ModuleRefresh"/>): the kept modules take
        /// the change in place, the converter context reads their ASTs again, the method cache
        /// drops the changed types' methods, and the stamps move on. The same inputs must be in
        /// the same order, and each changed file must still hash to its new stamp, so the change
        /// taken is the bytes the stamps describe. The refresh is an optimisation: any failure
        /// is a logged miss (a cold build, which drops the session), never a new way to fail.
        /// </summary>
        /// <param name="kind">"resources-refreshed" or "bodies-refreshed" on success.</param>
        /// <param name="changed">The changed types (kept definitions), empty unless bodies were refreshed.</param>
        /// <param name="withResources">Whether a recompiled module's resources changed too.</param>
        private bool TryRefreshSession(List<(string path, string sha256)> stamps, byte[][] changedImages, Serilog.ILogger log, out string kind, out string miss, out IReadOnlyList<TypeDefinition> changed, out bool withResources)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string reason = null;
            int replaced = 0, recompiled = 0, bodies = 0, invalidated = 0;
            string changedTypes = null;
            kind = null;
            changed = System.Array.Empty<TypeDefinition>();
            withResources = false;
            var images = new List<byte[]>();
            try
            {
                if (stamps.Count != this.sessionStamps.Count
                    || stamps.Where((stamp, index) => stamp.path != this.sessionStamps[index].path).Any())
                {
                    reason = "input-set";
                }
                else
                {
                    for (int index = 0; index < stamps.Count && reason == null; index++)
                    {
                        if (stamps[index].sha256 == this.sessionStamps[index].sha256)
                        {
                            continue;
                        }

                        var image = changedImages[index] ?? File.ReadAllBytes(stamps[index].path);
                        if (changedImages[index] == null && System.Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(image)) != stamps[index].sha256)
                        {
                            reason = "rewritten " + Path.GetFileName(stamps[index].path);
                        }

                        images.Add(image);
                    }
                }

                using var refresh = reason == null ? this.sessionClr.TryPlanRefresh(images, out reason) : null;
                if (refresh != null)
                {
                    replaced = refresh.ResourcesReplaced;
                    recompiled = refresh.Recompiled.Count;
                    changedTypes = string.Join(",", refresh.ChangedTypes.Take(10).Select(type => type.FullName));
                    if (recompiled == 0)
                    {
                        refresh.Commit();
                        kind = "resources-refreshed";
                    }
                    else
                    {
                        var kinds = this.sessionContext.BeginRefresh(refresh);
                        invalidated = this.methodCache?.Invalidate(method => refresh.IsAffected(method.DeclaringType)) ?? 0;
                        refresh.Commit();
                        bodies = refresh.BodiesReplaced;
                        if (this.sessionContext.Refresh(refresh, kinds, out reason))
                        {
                            kind = "bodies-refreshed";
                            changed = refresh.ChangedTypes;
                            withResources = refresh.RecompiledResourcesChanged;
                        }
                    }

                    if (reason == null)
                    {
                        this.sessionStamps = stamps;
                    }
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException))
            {
                reason = "error " + ex.GetType().Name + ": " + ex.Message;
            }

            log.Information(
                "Session.Refresh Refreshed={Refreshed} Kind={Kind} Images={Images} Resources={Resources} Recompiled={Recompiled} ChangedTypes={ChangedTypes} Bodies={Bodies} CacheInvalidated={CacheInvalidated} Miss={Miss} ElapsedMs={ElapsedMs}",
                reason == null,
                kind,
                images.Count,
                replaced,
                recompiled,
                changedTypes,
                bodies,
                invalidated,
                reason,
                sw.ElapsedMilliseconds);
            miss = reason;
            return reason == null;
        }

        /// <summary>
        /// Patches the last full emit's bundle for a body-only change of
        /// <paramref name="changedTypes"/>, with the plugins' sections when
        /// <paramref name="withResources"/>, or for a resource-only change when it is null
        /// (<see cref="DevPatchSession"/>). False when there is no patch session or it misses;
        /// the build then emits in full.
        /// </summary>
        private bool TryPatchEmit(IReadOnlyList<TypeDefinition> changedTypes, bool withResources, Serilog.ILogger log)
        {
            var session = this.patchSession;
            if (session == null)
            {
                return false;
            }

            // A miss can leave the session's scope manager half changed: it is never used again.
            this.patchSession = null;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            bool patched;
            string miss;
            int methods = 0;
            try
            {
                patched = changedTypes != null
                    ? session.TryPatch(changedTypes, withResources, this.sessionContext, out miss, out methods)
                    : session.TryPatchResources(this.sessionContext, out miss, out methods);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException))
            {
                patched = false;
                miss = "error " + ex.GetType().Name + ": " + ex.Message;
            }

            this.LastPatchMiss = patched ? null : miss;

            log.Information(
                "Patch.Emit Patched={Patched} Methods={Methods} Patches={Patches} ScriptChars={ScriptChars} Miss={Miss} ElapsedMs={ElapsedMs} FragmentMs={FragmentMs} WriteMs={WriteMs}",
                patched,
                methods,
                session.Patches,
                session.ScriptLength,
                miss,
                sw.ElapsedMilliseconds,
                patched ? session.ProbeFragmentMs : 0,
                patched ? session.ProbeWriteMs : 0);
            if (patched)
            {
                this.patchSession = session;
                this.LastLivePatch = session.LastLive;
                this.LastBuildReason += changedTypes != null
                    ? "; patched " + methods + " methods"
                    : "; patched resources" + (methods > 0 ? " and " + methods + " methods" : string.Empty);
            }

            return patched;
        }

        /// <summary>Writes the context's warnings from index <paramref name="from"/> on, as MSBuild lines.</summary>
        private static void WriteWarnings(ConverterContext converterContext, int from)
        {
            for (int i = from; i < converterContext.Warnings.Count; i++)
            {
                var warning = converterContext.Warnings[i];
                if (warning.Item1 != null)
                {
                    System.Console.Out.WriteLine(
                        string.Format("{0}({1},{2}): warning WRN0123: {3}",
                            warning.Item1.FileName,
                            warning.Item1.StartLine,
                            warning.Item1.StartColumn,
                            warning.Item2));
                }
            }
        }

        /// <summary>
        /// Logs chunk counts and render time; with <c>NSCRIPT_DEV_CHUNK_INDEX=1</c> also writes
        /// <c>&lt;out&gt;.chunks.tsv</c> (name, 1-based start line, line count).
        /// </summary>
        private static void LogDevChunks(JSWriter writer, string jsScript, Serilog.ILogger log)
        {
            log.Information(
                "DevChunks Chunks={Chunks} Fallbacks={Fallbacks} ChunkRenderMs={ChunkRenderMs}",
                writer.Chunks.Count,
                writer.ChunkFallbacks,
                System.Math.Round(writer.ChunkRenderTicks * 1000.0 / System.Diagnostics.Stopwatch.Frequency));

            if (System.Environment.GetEnvironmentVariable("NSCRIPT_DEV_CHUNK_INDEX") == "1")
            {
                var index = new System.Text.StringBuilder("name\tstartLine\tlines\n");
                foreach (var chunk in writer.Chunks)
                {
                    index.Append(chunk.Name).Append('\t')
                        .Append(chunk.StartLine + 1).Append('\t')
                        .Append(chunk.LineCount).Append('\n');
                }

                File.WriteAllText(jsScript + ".chunks.tsv", index.ToString());
            }
        }

        /// <summary>
        /// Dev-mode naming (slice 2, Inc 1): stable names for the global and member trees.
        /// NSDEV errors become converter errors, so no bundle is written.
        /// </summary>
        private static void NameForDevMode(
            RuntimeScopeManager runtimeManager,
            ConverterContext converterContext,
            Serilog.ILogger log)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var rootNamer = IdentifierScope.DevStableNamer.NameExecutionTree(runtimeManager.Scope);
            var memberNamer = IdentifierScope.DevStableNamer.NameTypeTree(
                runtimeManager.JSBaseObjectScopeManager.InstanceScope);
            foreach (var error in rootNamer.Errors.Concat(memberNamer.Errors))
            {
                converterContext.AddError(null, error, false);
                log.Error("DevNaming.Collision {Message}", error);
            }

            var fallbacks = rootNamer.Fallbacks.Concat(memberNamer.Fallbacks).ToList();
            log.Information(
                "DevNaming RootNamed={RootNamed} MemberNamed={MemberNamed} Errors={Errors} Fallbacks={Fallbacks} FallbackNames={FallbackNames} ElapsedMs={ElapsedMs}",
                rootNamer.NamedCount,
                memberNamer.NamedCount,
                rootNamer.Errors.Count + memberNamer.Errors.Count,
                fallbacks.Count,
                string.Join(",", fallbacks.Distinct().Take(40)),
                sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Builds with the plugins given to the constructor.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool Execute() => this.Execute(this.ctorPlugins);

        /// <summary>
        /// Builds with this build's plugin instances. In dev mode the instance keeps a session
        /// after a successful build: the next call reuses the loaded modules and the converter
        /// context while no input file changed (<see cref="LastBuildKind"/> "warm"). Any
        /// failure drops the session, so the next build is cold.
        /// </summary>
        public bool Execute(IConverterPlugin[] buildPlugins)
        {
            this.SetPlugins(buildPlugins);
            this.retryBuild = false;
            this.LastLivePatch = null;
            bool succeeded = false;
            try
            {
                succeeded = this.ExecuteCore();
                for (int retry = 0; !succeeded && this.retryBuild && retry < 2; retry++)
                {
                    // The cache entries that failed are gone, and a plugin that asked does not
                    // reuse that work again; the rest replay again. Two, as each cause can fire once.
                    this.retryBuild = false;
                    succeeded = this.ExecuteCore();
                }

                if (!succeeded && this.retryBuild)
                {
                    // Out of retries: say so, as the attempt stopped before printing errors.
                    CompilerLog.ForComponent("Builder").Error("Builder.RetriesExhausted");
                    System.Console.Out.WriteLine("(0,0): error ERR0124: the build still asked for a retry after 2 retries.");
                }

                return succeeded;
            }
            finally
            {
                this.SetPlugins(System.Array.Empty<IConverterPlugin>());
                if (succeeded && this.UseSession)
                {
                    this.sessionContext.EndBuild();
                }
                else
                {
                    this.DropSession();
                }
            }
        }

        private bool ExecuteCore()
        {
            var log = CompilerLog.ForComponent("Builder");
            TypeConverter.ProbeMethodConvertTicks = TypeConverter.ProbeMaxMethodConvertTicks = TypeConverter.ProbeParseTicks = 0;
            TypeConverter.ProbeSlowMethods.Clear();
            TypeConverter.ProbeMethodsConverted = TypeConverter.ProbeNestedConverts = 0;
            var totalSw = System.Diagnostics.Stopwatch.StartNew();
            log.Information("Builder.Start {MainAssembly} {ReferenceCount}", this.mainAssembly, this.references?.Length ?? 0);

            if (!this.VerifyPaths())
            {
                log.Warning("Builder.VerifyPaths failed");
                return false;
            }

            var loadSw = System.Diagnostics.Stopwatch.StartNew();
            var stamps = this.ReadInputStamps(out var changedImages);
            bool warm = this.UseSession
                && this.sessionContext != null
                && stamps.SequenceEqual(this.sessionStamps);
            string sessionReason = !this.UseSession ? "off"
                : warm ? "unchanged"
                : this.sessionContext == null ? "new"
                : "inputs-changed";
            string refreshMiss = null;
            IReadOnlyList<TypeDefinition> changedTypes = null;
            bool withResources = false;
            if (!warm && this.UseSession && this.sessionContext != null && this.TryRefreshSession(stamps, changedImages, log, out var refreshKind, out refreshMiss, out changedTypes, out withResources))
            {
                warm = true;
                sessionReason = refreshKind;
            }

            // Set before loading, so a build that fails to load reports this build, not the last.
            this.LastBuildKind = warm ? "warm" : "cold";
            this.LastBuildReason = refreshMiss == null ? sessionReason : sessionReason + "; refresh miss: " + refreshMiss;

            ClrContext clrContext;
            if (warm)
            {
                clrContext = this.sessionClr;
            }
            else
            {
                // Execute(plugins) releases it after the build unless the session keeps it.
                this.DropSession();
                clrContext = new ClrContext();
                this.sessionClr = clrContext;
                foreach (var reference in references)
                {
                    clrContext.LoadAssembly(reference);
                }

                clrContext.LoadAssembly(this.mainAssembly);
            }

            loadSw.Stop();
            log.Information("LoadAssemblies completed in {ElapsedMs}ms", loadSw.ElapsedMilliseconds);
            log.Information(
                "Session.Build Kind={Kind} Reason={Reason}",
                this.LastBuildKind,
                sessionReason);

            if ((sessionReason == "bodies-refreshed" || sessionReason == "resources-refreshed")
                && this.TryPatchEmit(sessionReason == "bodies-refreshed" ? changedTypes : null, withResources, log))
            {
                // A patch misses on any error. Its BeginBuild cleared the last build's warnings,
                // so the list holds only the warnings its conversions added.
                WriteWarnings(this.sessionContext, 0);
                log.Information(
                    "Builder.End {ElapsedMs}ms Warnings={WarningCount} Errors={ErrorCount}",
                    totalSw.ElapsedMilliseconds,
                    this.sessionContext.Warnings.Count,
                    0);
                return true;
            }

            // A full emit makes a new patch session when it writes the bundle.
            this.patchSession = null;

            RuntimeScopeManager runtimeManager;
            ConverterContext converterContext;
            List<MethodDefinition> methodDefinitionsToEmit;
            MethodDefinition entryPoint;
            List<MethodDefinition> moduleInitializers;

            var contextSw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (warm)
                {
                    converterContext = this.sessionContext;
                    converterContext.BeginBuild(this.methodConverterPlugins, this.typeConverterPlugins);
                }
                else
                {
                    converterContext = new ConverterContext(
                        clrContext,
                        this.methodConverterPlugins,
                        this.typeConverterPlugins);
                    this.sessionContext = converterContext;
                    this.sessionStamps = stamps;
                }

                converterContext.DevMode = this.devMode;
                converterContext.LivePatchHook = DevPatchSession.IsEnabled && this.devMode && !this.scriptGenerateSettings.optimize;
                converterContext.DevChunks = this.devMode
                    && System.Environment.GetEnvironmentVariable("NSCRIPT_DEV_CHUNKS") != "off";
                if (converterContext.DevChunks
                    && this.UseSession
                    && !this.scriptGenerateSettings.optimize
                    && MethodCache.IsEnabled)
                {
                    this.methodCache ??= new MethodCache();
                    this.methodCache.BeginBuild();
                    converterContext.MethodCache = this.methodCache;
                }
                else
                {
                    converterContext.MethodCache = null;
                }

                runtimeManager = new RuntimeScopeManager(
                    converterContext,
                    instanceAsStatic: this.scriptGenerateSettings.optimize);

                methodDefinitionsToEmit = new List<MethodDefinition>();
                entryPoint = this.GetEntryPoint(converterContext, Path.GetFileName(mainAssembly));
                moduleInitializers = this.GetModuleInitializers(converterContext, Path.GetFileName(mainAssembly));
                log.Information("ConverterContext completed in {ElapsedMs}ms", contextSw.ElapsedMilliseconds);
            }
            catch(System.Exception ex)
            {
                System.Console.Out.WriteLine(
                    string.Format("{0}({1},{2}): error ERR0123: {3}",
                        string.Empty,
                        0,
                        0,
                        ex.Message));

                System.Console.Out.WriteLine(ex.StackTrace);

                return false;
            }

            bool emitFailed = false;

            try
            {
                if (entryPoint != null)
                {
                    methodDefinitionsToEmit.Add(entryPoint);
                }

                foreach (var initializer in moduleInitializers)
                {
                    methodDefinitionsToEmit.Add(initializer);
                }

                // Let's go through first pass and collect all the method references
                // to emit.
                var pluginInitSw = System.Diagnostics.Stopwatch.StartNew();
                if (this.plugins != null)
                {
                    foreach (var plugin in this.plugins)
                    {
                        plugin.Initialize(clrContext, runtimeManager);

                        var methodsToEmit = plugin.GetMethodsToEmitPass1();

                        // Let's resolve references for all the methods that we may be emitting. This will
                        // cause runtimeManager to traverse these methods as well during analysis.
                        if (methodsToEmit != null)
                        {
                            for (int methodIndex = 0; methodIndex < methodsToEmit.Count; methodIndex++)
                            {
                                runtimeManager.Resolve(methodsToEmit[methodIndex]);
                                methodDefinitionsToEmit.Add(methodsToEmit[methodIndex].Resolve());
                            }
                        }
                    }
                }

                // Let's convert all the code to JS.
                var pluginInitMs = pluginInitSw.ElapsedMilliseconds;
                var convertSw = System.Diagnostics.Stopwatch.StartNew();
                var statements = runtimeManager.Convert(methodDefinitionsToEmit, plugins);
                log.Information("Convert completed in {ElapsedMs}ms", convertSw.ElapsedMilliseconds);
                var ticksPerMs = System.Diagnostics.Stopwatch.Frequency / 1000.0;
                log.Information(
                    "Probe.Convert ConvertMs={ConvertMs} MethodConvertMs={MethodConvertMs} PluginInitMs={PluginInitMs} MethodsConverted={MethodsConverted} MaxMethodConvertMs={MaxMethodConvertMs} NestedConverts={NestedConverts} ParseMs={ParseMs} SlowMethodCount={SlowMethodCount} SlowMethodMs={SlowMethodMs} SlowMethods={SlowMethods}",
                    convertSw.ElapsedMilliseconds,
                    System.Math.Round(TypeConverter.ProbeMethodConvertTicks / ticksPerMs),
                    pluginInitMs,
                    TypeConverter.ProbeMethodsConverted,
                    System.Math.Round(TypeConverter.ProbeMaxMethodConvertTicks / ticksPerMs, 1),
                    TypeConverter.ProbeNestedConverts,
                    System.Math.Round(TypeConverter.ProbeParseTicks / ticksPerMs),
                    TypeConverter.ProbeSlowMethods.Count,
                    System.Math.Round(TypeConverter.ProbeSlowMethods.Sum(m => m.Ticks) / ticksPerMs),
                    TypeConverter.ProbeSlowMethods
                        .OrderByDescending(m => m.Ticks)
                        .Take(10)
                        .Select(m => m.Method + "=" + System.Math.Round(m.Ticks / ticksPerMs, 1))
                        .ToList());

                if (this.plugins != null)
                {
                    foreach (var plugin in this.plugins)
                    {
                        var pluginJsStatements = plugin.GetPreJavascript();
                        if (pluginJsStatements != null)
                        { statements.InsertRange(0, pluginJsStatements); }

                        pluginJsStatements = plugin.GetPostJavascript();
                        if (pluginJsStatements != null)
                        { statements.AddRange(pluginJsStatements); }

                        if (this.devMode && plugin.PatchSectionName != null)
                        {
                            // Patch emit puts a resource change's statements before it.
                            statements.Add(new JST.MarkerStatement(runtimeManager.Scope, JST.MarkerStatement.ForSection(plugin.PatchSectionName)));
                        }
                    }
                }

                if (converterContext.RetryReason != null)
                {
                    log.Warning("Builder.Retry Reason={Reason}", converterContext.RetryReason);
                    this.retryBuild = true;
                    return false;
                }

                // Per the CLR spec, module initializers run before any user code
                // in the module — emit their invocations directly before the
                // entry-point call.
                foreach (var initializer in moduleInitializers)
                {
                    statements.Add(
                        JST.ExpressionStatement.CreateMethodCallExpression(
                            new JST.IdentifierExpression(runtimeManager.ResolveFunctionName(initializer), runtimeManager.Scope)));
                }

                if (entryPoint != null)
                {
                    // Not at the end, let's insert call to entryPoint.
                    statements.Add(
                        JST.ExpressionStatement.CreateMethodCallExpression(
                            new JST.IdentifierExpression(runtimeManager.ResolveFunctionName(entryPoint), runtimeManager.Scope)));
                }

                if (scriptGenerateSettings.optimize)
                {
                    var identCounter = new IdentifierCounterVisitor();
                    var unusedMethodRemover = new UnusedMethodRemover();
                    var inlinableVisitor = new InlineableVisitor();
                    var methodNameRemover = new MethodNameRemover();

                    statements.ForEach(((IJstVisitor)inlinableVisitor).DispatchStatement);
                    var proxyFixer = new ProxyFixer(inlinableVisitor.Functions);
                    statements = statements
                        .ConvertAll(((ITransformerVisitor)proxyFixer).DispatchStatement);

                    runtimeManager.Scope.ResetUsageCounter();
                    runtimeManager.JSBaseObjectScopeManager.InstanceScope.ResetUsageCounter();
                    statements.ForEach(((IJstVisitor)identCounter).DispatchStatement);
                    statements = statements
                        .ConvertAll(((ITransformerVisitor)methodNameRemover).DispatchStatement)
                        .ConvertAll(((ITransformerVisitor)unusedMethodRemover).DispatchStatement);
                }

                var stopWatch = new System.Diagnostics.Stopwatch();

                stopWatch.Start();
                if (this.devMode)
                {
                    NameForDevMode(runtimeManager, converterContext, log);
                    if (converterContext.MethodCache?.ValidateHits() == false)
                    {
                        log.Warning(
                            "MethodCache.Retry Invalidated={Invalidated} Hits={Hits}",
                            converterContext.MethodCache.Invalidated,
                            converterContext.MethodCache.Hits);
                        this.retryBuild = true;
                        return false;
                    }
                }
                else
                {
                    IdentifierScope.IdentifierMinifiedNamer.MinifyNames(
                        runtimeManager.Scope,
                        scriptGenerateSettings.minify);
                    stopWatch.Stop();
                    System.Console.WriteLine("Root scope naming time taken: {0}", stopWatch.ElapsedMilliseconds);
                    log.Information("RootScopeNaming completed in {ElapsedMs}ms", stopWatch.ElapsedMilliseconds);
                    stopWatch.Restart();
                    IdentifierScope.IdentifierMinifiedNamer.MinifyNames(
                        runtimeManager.JSBaseObjectScopeManager.InstanceScope,
                        scriptGenerateSettings.minify);
                    System.Console.WriteLine("Instance scope naming time taken: {0}", stopWatch.ElapsedMilliseconds);
                    log.Information("InstanceScopeNaming completed in {ElapsedMs}ms", stopWatch.ElapsedMilliseconds);
                }

                var writerSw = System.Diagnostics.Stopwatch.StartNew();
                var writer = new JSWriter(true, scriptGenerateSettings.uglify);
                writer.LivePatchHook = converterContext.LivePatchHook;
                string hmrPrefix = null;
                if (writer.LivePatchHook)
                {
                    this.hmrGen = HmrClient.NewGen();
                    hmrPrefix = HmrClient.BootPrefix(
                        Path.GetFileName(this.jsScript),
                        HmrClient.BundleId(this.jsScript),
                        this.HmrEndpointSource?.Invoke());
                    writer.HmrBoot = hmrPrefix + this.hmrGen + ":0";
                }

                var initializerStatement = runtimeManager.GetVariableDeclarations();
                if (initializerStatement != null)
                {
                    writer.Write(initializerStatement);
                }

                foreach (var statement in statements)
                {
                    if (statement != null)
                    {
                        writer.Write(statement);
                    }
                }

                if (converterContext.MethodCache != null)
                {
                    var cache = converterContext.MethodCache;
                    cache.Harvest();
                    log.Information(
                        "Probe.MethodCache Hits={Hits} Misses={Misses} Stored={Stored} Entries={Entries} Uncacheable={Uncacheable}",
                        cache.Hits,
                        cache.Misses,
                        cache.Stored,
                        cache.Count,
                        string.Join(",", cache.Uncacheable.OrderByDescending(u => u.Value).Select(u => u.Key + "=" + u.Value)));
                }

                // Use the explicit sourceRoot when provided (e.g. an ASP.NET Core handler path
                // or an ADO/GitHub repo URL). Otherwise fall back to the legacy SrcMapper.ashx
                // handler so existing IIS-based deployments continue to work unchanged.
                // The legacy path also keeps the sidecar .ashx drop enabled; any explicit override
                // suppresses it.
                bool isLegacySourceRoot = string.IsNullOrEmpty(this.sourceMapRoot);
                string effectiveSourceRoot = isLegacySourceRoot
                    ? string.Format(
                        "SrcMapper.ashx?js={0}&fname=",
                        Path.GetFileName(this.jsScript))
                    : this.sourceMapRoot;

                if (converterContext.Errors.Count > 0)
                {
                    // Conversion produced errors (e.g. an unresolved Razor handler or an
                    // unsupported binding expression that was turned into a diagnostic). Do not
                    // publish a partial bundle that silently omits the failed output: the errors
                    // are printed below and Execute returns failure so callers exit non-zero.
                    log.Warning(
                        "Builder: {ErrorCount} converter error(s); skipping JavaScript output {JsScript}",
                        converterContext.Errors.Count, this.jsScript);
                }
                else
                {
                    if (DevPatchSession.IsEnabled && this.devMode && this.UseSession && !this.scriptGenerateSettings.optimize)
                    {
                        var (script, map) = writer.WriteAndKeep(
                            this.jsScript,
                            effectiveSourceRoot,
                            emitLegacyAshxHandler: isLegacySourceRoot,
                            repoRoot: this.repoRoot,
                            secondaryRepoRoot: this.secondaryRepoRoot,
                            secondarySourceRoot: this.secondarySourceRoot);
                        this.patchSession = new DevPatchSession(
                            runtimeManager,
                            this.methodConverterPlugins,
                            this.typeConverterPlugins,
                            this.plugins,
                            converterContext.DevChunks,
                            this.jsScript,
                            script,
                            map,
                            hmrPrefix,
                            this.hmrGen,
                            new HashSet<MethodDefinition>(moduleInitializers.Append(entryPoint).Where(method => method != null)));
                    }
                    else
                    {
                        writer.Write(
                            this.jsScript,
                            effectiveSourceRoot,
                            emitLegacyAshxHandler: isLegacySourceRoot,
                            repoRoot: this.repoRoot,
                            secondaryRepoRoot: this.secondaryRepoRoot,
                            secondarySourceRoot: this.secondarySourceRoot);
                    }

                    if (hmrPrefix != null)
                    {
                        this.LastLivePatch = LivePatch.NewBuild(this.hmrGen);
                    }

                    log.Information("JSWriter.End {JsScript} {ElapsedMs}ms", this.jsScript, writerSw.ElapsedMilliseconds);
                    if (converterContext.DevChunks)
                    {
                        LogDevChunks(writer, this.jsScript, log);
                    }
                }
            }
            catch(ConverterLocationException ex)
            {
                emitFailed = true;
                System.Console.Out.WriteLine(
                    string.Format("{0}({1},{2}): error ERR0123: {3}",
                        ex.Location.FileName,
                        ex.Location.StartLine,
                        ex.Location.StartColumn,
                        ex.Message));
            }
            catch(System.Exception ex)
            {
                emitFailed = true;
                System.Console.Out.WriteLine(
                    "NScript.Exe(0,0): error UNK0001: {0}",
                    ex.Message);
                System.Console.Out.WriteLine(ex.StackTrace);

                while(ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    System.Console.WriteLine("-------------------------");
                    System.Console.Out.WriteLine(ex.Message);
                    System.Console.Out.WriteLine(ex.StackTrace);
                }
            }

            WriteWarnings(converterContext, 0);

            foreach (var warning in converterContext.Errors)
            {
                if (warning.Item1 != null)
                {
                    System.Console.Out.WriteLine(
                        string.Format("{0}({1},{2}): error ERR0123: {3}",
                            warning.Item1.FileName,
                            warning.Item1.StartLine,
                            warning.Item1.StartColumn,
                            warning.Item2));
                }
                else
                {
                    System.Console.Out.WriteLine(
                        string.Format("{0}({1},{2}): error ERR0123: {3}",
                            string.Empty,
                            0,
                            0,
                            warning.Item2));
                }
            }

            totalSw.Stop();
            log.Information(
                "Builder.End {ElapsedMs}ms Warnings={WarningCount} Errors={ErrorCount}",
                totalSw.ElapsedMilliseconds,
                converterContext.Warnings.Count,
                converterContext.Errors.Count);

            return !emitFailed && converterContext.Errors.Count == 0;
        }

        /// <summary>
        /// Determines if we can verify paths.
        /// </summary>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        private bool VerifyPaths()
        {
            bool returnValue = true;
            if (!File.Exists(mainAssembly))
            {
                returnValue = false;
                Logger.Instance.LogError(
                    string.Format("main assembly: ({0}) not found", mainAssembly));
            }

            foreach (var reference in this.references)
            {
                if (!File.Exists(reference))
                {
                    returnValue = false;
                    Logger.Instance.LogError(
                        string.Format("reference: ({0}) not found", reference));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Gets entry point.
        /// </summary>
        /// <param name="context">      The context. </param>
        /// <param name="mainAssembly"> The main assembly. </param>
        /// <returns>
        /// The entry point.
        /// </returns>
        private MethodDefinition GetEntryPoint(ConverterContext context, string mainAssembly)
        {
            ModuleDefinition module;
            context.ClrContext.TryGetModuleDefinition(mainAssembly, out module);

            foreach (var item in module.Types)
            {
                if (item.IsInterface
                    || item.IsValueType
                    || item.HasGenericParameters)
                {
                    continue;
                }

                foreach (var method in item.Methods)
                {
                    if (method.HasGenericParameters
                        || method.HasAssociatedMember()
                        || !method.IsStatic)
                    {
                        continue;
                    }

                    if (method.IsPublic
                        && method.ReturnType.FullName == context.ClrKnownReferences.Void.FullName
                        && !method.HasParameters
                        && method.Parameters.Count == 0
                        && method.CustomAttributes.SelectAttribute(context.KnownReferences.EntryPointAttribute) != null)
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Collects every static, parameterless, void method in the main assembly that
        /// carries <c>[ModuleInitializer]</c>. Returns them in stable declaration order
        /// (type metadata-token order, then method declaration order) — close-enough
        /// proxy for CLR's "metadata token order" guarantee in a single-bundle world.
        /// </summary>
        private List<MethodDefinition> GetModuleInitializers(ConverterContext context, string mainAssembly)
        {
            var result = new List<MethodDefinition>();

            ModuleDefinition module;
            if (!context.ClrContext.TryGetModuleDefinition(mainAssembly, out module))
            {
                return result;
            }

            foreach (var item in module.Types)
            {
                if (item.IsInterface
                    || item.HasGenericParameters)
                {
                    continue;
                }

                foreach (var method in item.Methods)
                {
                    if (method.HasGenericParameters
                        || method.HasAssociatedMember()
                        || !method.IsStatic)
                    {
                        continue;
                    }

                    if (method.ReturnType.FullName == context.ClrKnownReferences.Void.FullName
                        && !method.HasParameters
                        && method.Parameters.Count == 0
                        && method.CustomAttributes.SelectAttribute(context.KnownReferences.ModuleInitializerAttribute) != null)
                    {
                        result.Add(method);
                    }
                }
            }

            return result;
        }
    }
}