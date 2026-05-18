namespace NScript.Lib
{
    using System;
    using System.Collections.Generic;
    using NScript.Converter;
    using NScript.Converter.Plugins;
    using NScript.RazorSkin;
    using NScript.Utils;
    using XwmlParser;

    public static class NScriptCompiler
    {
        public static int Compile(string[] args)
        {
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
                var plugins = new List<IConverterPlugin>()
                {
                    // Razor MUST be before XWML: the first plugin returning Overwrite wins,
                    // and XWML would claim [Skin] attributes for .skin.cshtml templates
                    // then fail because it only handles .html templates.
                    new RazorTemplatingPlugin(),
                    new XwmlTemplatingPlugin(),
                    new TestGenerator()
                };

                var builder = new Builder(
                    parseOptions.JsFileName,
                    parseOptions.JsParts,
                    parseOptions.EntryAssembly,
                    parseOptions.ReferenceDlls.ToArray(),
                    plugins.ToArray(),
                    (parseOptions.Minify, parseOptions.Uglify, parseOptions.Optimize),
                    parseOptions.SourceMapRoot,
                    parseOptions.RepoRoot,
                    parseOptions.SecondarySourceRoot,
                    parseOptions.SecondaryRepoRoot);

                var stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                _ = builder.Execute();

                stopWatch.Stop();
                System.Console.WriteLine("TimeTaken[cs2jsc]: {0}ms", stopWatch.ElapsedMilliseconds);

                if (CompilerLog.IsEnabled)
                {
                    CompilerLog.ForComponent("NScriptCompiler").Information(
                        "cs2jsc total duration {ElapsedMs}ms",
                        stopWatch.ElapsedMilliseconds);
                }

                return 0;
            }
            finally
            {
                CompilerLog.Shutdown();
            }
        }
    }
}
