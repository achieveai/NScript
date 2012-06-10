using System.Collections.Generic;
using Cs2JsC.Converter;

namespace Cs2JsC
{
    internal class CommandLine
    {
        private static void Main(string[] args)
        {
            ParseOptions parseOptions = ParseOptions.ParseArgs(args);

            if (parseOptions == null)
            {
                ParseOptions.PrintUsage();
            }

            List<IConverterPlugin> plugins = new List<IConverterPlugin>();
            if (parseOptions.PluginConfigFileName != null)
            {
                var pluginsInfos = PluginLoadInfo.LoadPluginInfos(
                    parseOptions.PluginConfigFileName,
                    parseOptions.PluginHintPaths);

                foreach (var pluginInfo in pluginsInfos)
                {
                    var plugin = pluginInfo.GetPlugin();

                    if (plugin != null)
                    {
                        plugins.Add(plugin);
                    }
                }
            }

            Converter.Builder builder = new Converter.Builder(
                parseOptions.JsFileName,
                parseOptions.EntryAssembly,
                parseOptions.ReferenceDlls.ToArray(),
                plugins.ToArray());

            builder.Execute();
        }
    }
}