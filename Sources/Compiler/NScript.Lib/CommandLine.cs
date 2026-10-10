namespace NScript.Lib
{
    using System;
    using NScript.Converter;
    using NScript.Converter.Plugins;
    using NScript.Csc.Lib.Service;
    using NScript.RazorSkin;
    using NScript.Utils;
    using XwmlParser;

    public static class NScriptCompiler
    {
        public static int Compile(string[] args)
        {
            // -service routes the emit to the warm build service daemon (falls back to a
            // local compile with warning NSS001 when the daemon is unavailable). Shared by
            // nscript.exe and the Cs2Jsc dotnet tool, so both strip the flag.
            // -watch (Sdk.targets adds it only together with -service) asks the daemon to
            // record the request and replay it on file changes; batch never sees it.
            bool watch = ServiceArgs.TryStripWatchFlag(args, out var withoutWatch);
            args = withoutWatch;
            if (ServiceArgs.TryStripServiceFlag(args, out var strippedArgs))
            {
                args = strippedArgs;
                if (!ServiceArgs.HasReleaseFlags(args))
                {
                    args = ServiceArgs.EnsureDevMode(args);
                    int? serviceExitCode = RunThroughService(args, watch);
                    if (serviceExitCode.HasValue)
                    {
                        return serviceExitCode.Value;
                    }
                }
            }

            ParseOptions parseOptions = ParseOptions.ParseArgs(args);

            if (parseOptions == null)
            {
                ParseOptions.PrintUsage();
                _ = Console.ReadKey();
                return 1;
            }

            // Opt-in: only initialize structured logging when --log is supplied
            // (or the NSCRIPT_LOG_PATH env var is set, resolved by CompilerLog).
            CompilerLog.Initialize(parseOptions.LogPath, "cs2jsc", parseOptions.RunId);

            try
            {
                return Run(parseOptions);
            }
            finally
            {
                CompilerLog.Shutdown();
            }
        }

        /// <summary>
        /// Runs stage 2 for already parsed options. Never exits the process, never reads the
        /// console and never initializes or shuts down <see cref="CompilerLog"/>; the build
        /// service calls it once per request.
        /// </summary>
        public static int Run(ParseOptions parseOptions)
        {
            static IConverterPlugin[] CreatePlugins() => new IConverterPlugin[]
            {
                // Razor MUST be before XWML: the first plugin returning Overwrite wins,
                // and XWML would claim [Skin] attributes for .skin.cshtml templates
                // then fail because it only handles .html templates.
                new RazorTemplatingPlugin(),
                new XwmlTemplatingPlugin(),
                new TestGenerator()
            };

            Builder CreateBuilder(string jsFileName, IConverterPlugin[] builderPlugins) => new Builder(
                jsFileName,
                parseOptions.JsParts,
                parseOptions.EntryAssembly,
                parseOptions.ReferenceDlls.ToArray(),
                builderPlugins,
                (parseOptions.Minify, parseOptions.Uglify, parseOptions.Optimize),
                parseOptions.SourceMapRoot,
                parseOptions.RepoRoot,
                parseOptions.SecondarySourceRoot,
                parseOptions.SecondaryRepoRoot,
                parseOptions.DevMode);

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            // Execute returns false when conversion produced errors (and no output was
            // published). Surface that as a non-zero exit code so direct callers of the
            // compiler do not mistake an incomplete bundle for a successful build.
            // Dev mode builds through the output's session, which gets this build's plugins.
            bool succeeded = parseOptions.DevMode
                ? BuilderSessions.Execute(
                    parseOptions,
                    () => CreateBuilder(parseOptions.JsFileName, Array.Empty<IConverterPlugin>()),
                    CreatePlugins())
                : CreateBuilder(parseOptions.JsFileName, CreatePlugins()).Execute();

            stopWatch.Stop();
            System.Console.WriteLine("TimeTaken[cs2jsc]: {0}ms", stopWatch.ElapsedMilliseconds);

            if (CompilerLog.IsEnabled)
            {
                CompilerLog.ForComponent("NScriptCompiler").Information(
                    "cs2jsc total duration {ElapsedMs}ms",
                    stopWatch.ElapsedMilliseconds);
            }

            string lastBuild = parseOptions.DevMode ? BuilderSessions.LastBuild(parseOptions.JsFileName) : null;
            if (succeeded && IncrementalVerifier.IsEnabled && lastBuild != null && lastBuild.StartsWith("warm", StringComparison.Ordinal))
            {
                IncrementalVerifier.Verify(
                    parseOptions.JsFileName,
                    lastBuild,
                    jsFileName => CreateBuilder(jsFileName, Array.Empty<IConverterPlugin>()),
                    CreatePlugins);
            }

            return succeeded ? 0 : 1;
        }

        private static int? RunThroughService(string[] args, bool watch)
        {
            // The client logs only when NSCRIPT_LOG_PATH is set; the batch path below
            // re-initializes with its own --log value after this shuts down.
            CompilerLog.Initialize(null, "nscript-client");
            try
            {
                return ServiceClient.TryRun(ServiceProtocol.KindEmitJs, args, "nscript", watch);
            }
            finally
            {
                CompilerLog.Shutdown();
            }
        }
    }
}
