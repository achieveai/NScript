using System.IO;
using System.Linq;
using NScript.CLR;
using NScript.Converter;
using NScript.Converter.TypeSystemConverter;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// The Razor skins of Sunlight.Framework.UI.Test and the framework they build on, for
    /// tests that run the whole plugin. Needs the framework build of this configuration.
    /// </summary>
    internal static class UiTestAssemblies
    {
#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        /// <summary>A new context with the framework libraries and the UI test assembly.</summary>
        public static ClrContext Load()
        {
            var repoRoot = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(typeof(UiTestAssemblies).Assembly.Location), "..", "..", "..", ".."));
            var libPath = Path.Combine(repoRoot, "NScriptToolSet", "lib", Configuration);
            var testDll = Directory.GetFiles(
                Path.Combine(repoRoot, "Test", "Framework", "Sunlight.Framework.UI.Test", "bin", Configuration),
                "Sunlight.Framework.UI.Test.dll",
                SearchOption.AllDirectories).First();

            var clrContext = new ClrContext();
            foreach (var lib in new[] { "MsCorLib.dll", "System.Web.dll", "System.Web.Html.dll",
                "Sunlight.Framework.dll", "SunlightUnit.dll", "Sunlight.Framework.UI.dll" })
            {
                clrContext.LoadAssembly(Path.Combine(libPath, lib), false);
            }

            clrContext.LoadAssembly(testDll, false);
            return clrContext;
        }

        /// <summary>Runs <paramref name="plugin"/> as a build does and returns its post JavaScript.</summary>
        public static string Build(RazorTemplatingPlugin plugin, ClrContext clrContext, ConverterContext context)
            => Build(plugin, clrContext, new RuntimeScopeManager(context));

        /// <summary>As <see cref="Build(RazorTemplatingPlugin, ClrContext, ConverterContext)"/>, with the caller's scope manager.</summary>
        public static string Build(RazorTemplatingPlugin plugin, ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
        {
            plugin.Initialize(clrContext, runtimeScopeManager);
            plugin.GetMethodsToEmitPass1();
            var writer = new StringWriter();
            var serializer = new NScript.JST.JSWriter(true, false);
            foreach (var statement in plugin.GetPostJavascript())
            {
                serializer.Write(statement);
            }

            serializer.Write(writer);
            return writer.ToString();
        }
    }
}
