namespace NScript
{
    using System;
    using System.Collections.Generic;
    using NScript.Converter;

    internal static class CommandLine
    {
        private static void Main(string[] args)
        {
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            ParseOptions parseOptions = ParseOptions.ParseArgs(args);

            if (parseOptions == null)
            {
                ParseOptions.PrintUsage();
                _ = Console.ReadKey();
            }

            List<IConverterPlugin> plugins = new List<IConverterPlugin>();

            var pluginsInfos = new List<PluginLoadInfo>();

            if (parseOptions.PluginConfigFileName != null)
            {
                pluginsInfos = PluginLoadInfo.LoadPluginInfos(
                    parseOptions.PluginConfigFileName,
                    parseOptions.PluginHintPaths);
            }

            foreach (var pluginInfo in pluginsInfos)
            {
                var plugin = pluginInfo.GetScriptPlugin();

                if (plugin != null)
                {
                    plugins.Add(plugin);
                }
            }

            var builder = new Builder(
                parseOptions.JsFileName,
                parseOptions.JsParts,
                parseOptions.EntryAssembly,
                parseOptions.ReferenceDlls.ToArray(),
                plugins.ToArray());

            _ = builder.Execute();

            stopWatch.Stop();
            System.Console.WriteLine("TimeTaken: {0}ms", stopWatch.ElapsedMilliseconds);
            System.Environment.Exit(0);
        }
    }
}