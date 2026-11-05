using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.Converter;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Issue #141: a computed getter read only as a sub-control property value
    /// (<c>&lt;SubDcKidView DataContext="@i.Kid" /&gt;</c> in RazorSubDcGetter.skin.cshtml) must be
    /// handed to the converter, or dead-code elimination drops it. Needs the Debug framework build.
    /// </summary>
    [TestClass]
    public class RazorSubControlGetterRetentionTests
    {
        [TestMethod]
        public void SubControlDataContextGetter_ReadOnlyBySkin_IsEmitted()
        {
            var clrContext = UiTestAssemblies.Load();
            var plugin = new RazorTemplatingPlugin();
            UiTestAssemblies.Build(plugin, clrContext, new ConverterContext(clrContext, null, null));

            plugin.GetMethodsToEmitPassN().Select(method => method.FullName)
                .Should().Contain(name => name.Contains("Sunlight.Framework.UI.Test.SubDcItem::get_Kid()"));
        }
    }
}
