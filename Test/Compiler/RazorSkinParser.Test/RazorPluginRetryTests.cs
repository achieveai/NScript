using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.CLR;
using NScript.Converter;
using NScript.Converter.TypeSystemConverter;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// A retried build (Builder.Execute) initializes the same plugin again, with a new
    /// RuntimeScopeManager. The retry must give what a new plugin gives. Uses the Razor skins
    /// of Sunlight.Framework.UI.Test, so it needs the Debug framework build.
    /// </summary>
    [TestClass]
    public class RazorPluginRetryTests
    {
#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        private static ClrContext clrContext;

        [ClassInitialize]
        public static void LoadAssemblies(TestContext _)
        {
            var repoRoot = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(typeof(RazorPluginRetryTests).Assembly.Location), "..", "..", "..", ".."));
            var libPath = Path.Combine(repoRoot, "NScriptToolSet", "lib", Configuration);
            var testDll = Directory.GetFiles(
                Path.Combine(repoRoot, "Test", "Framework", "Sunlight.Framework.UI.Test", "bin", Configuration),
                "Sunlight.Framework.UI.Test.dll",
                SearchOption.AllDirectories).First();

            clrContext = new ClrContext();
            foreach (var lib in new[] { "MsCorLib.dll", "System.Web.dll", "System.Web.Html.dll",
                "Sunlight.Framework.dll", "SunlightUnit.dll", "Sunlight.Framework.UI.dll" })
            {
                clrContext.LoadAssembly(Path.Combine(libPath, lib), false);
            }

            clrContext.LoadAssembly(testDll, false);
        }

        private static string Build(RazorTemplatingPlugin plugin, ConverterContext context)
        {
            plugin.Initialize(clrContext, new RuntimeScopeManager(context));
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

        [TestMethod]
        public void RetriedInitialize_StartsFromNothing_EqualsANewPlugin()
        {
            var fresh = Build(new RazorTemplatingPlugin(), new ConverterContext(clrContext, null, null));
            fresh.Should().NotBeEmpty("the skins must produce code, or this test proves nothing");

            var context = new ConverterContext(clrContext, null, null);
            var plugin = new RazorTemplatingPlugin();
            Build(plugin, context);
            context.BeginBuild(null, null);
            var retried = Build(plugin, context);

            retried.Should().Be(fresh);
            context.Errors.Should().BeEmpty();
        }
    }
}
