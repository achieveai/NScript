using System;
using System.Collections.Generic;
using NScript.Converter;

namespace NScript
{
    internal class CommandLine
    {
        private static void Main(string[] args)
        {
            ParseOptions parseOptions = ParseOptions.ParseArgs(args);

            if (parseOptions == null)
            {
                ParseOptions.PrintUsage();
                Console.ReadKey();
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

            Converter.Builder builder = new Converter.Builder(
                parseOptions.JsFileName,
                parseOptions.EntryAssembly,
                parseOptions.ReferenceDlls.ToArray(),
                plugins.ToArray());

            if (builder.Execute())
            {
                System.Environment.ExitCode = 0;
            }
        }
    }
}