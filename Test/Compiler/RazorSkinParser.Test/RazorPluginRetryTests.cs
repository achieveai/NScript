using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.CLR;
using NScript.Converter;
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
        private static ClrContext clrContext;

        [ClassInitialize]
        public static void LoadAssemblies(TestContext _)
        {
            clrContext = UiTestAssemblies.Load();
        }

        private static string Build(RazorTemplatingPlugin plugin, ConverterContext context)
            => UiTestAssemblies.Build(plugin, clrContext, context);

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
