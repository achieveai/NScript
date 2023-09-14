namespace NScript.Lib
{
    using System;
    using System.Collections.Generic;
    using NScript.Converter;
    using NScript.Converter.Plugins;
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

            var plugins = new List<IConverterPlugin>()
            {
                new XwmlTemplatingPlugin(),
                new TestGenerator()
            };

            var builder = new Builder(
                parseOptions.JsFileName,
                parseOptions.JsParts,
                parseOptions.EntryAssembly,
                parseOptions.ReferenceDlls.ToArray(),
                plugins.ToArray(),
                (parseOptions.Minify, parseOptions.Uglify, parseOptions.Optimize));

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            _ = builder.Execute();

            stopWatch.Stop();
            System.Console.WriteLine("TimeTaken[cs2jsc]: {0}ms", stopWatch.ElapsedMilliseconds);
            return 0;
        }
    }
}