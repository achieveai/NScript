using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.Converter;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Tests for RazorTemplatingPlugin.
    ///
    /// The plugin's Initialize, GetOverwrite, and GetPostJavascript methods require
    /// ClrContext and RuntimeScopeManager infrastructure that is too heavy for unit tests.
    /// MapCecilTypeToSimpleName is private static and not accessible without InternalsVisibleTo.
    ///
    /// These tests cover:
    /// 1. CanHandle (public static) — file name detection
    /// 2. Plugin construction and interface compliance
    /// 3. ParseArgs (no-op but part of IConverterPlugin contract)
    /// </summary>
    [TestClass]
    public class RazorTemplatingPluginTests
    {
        [TestMethod]
        public void TemplateDiagnostics_ReportOneBasedColumns()
        {
            // Template locations keep 0-based columns (source maps); MSBuild and watch.log read
            // the printed column as 1-based, so a template error at offset 66 prints column 67.
            var reported = ConverterContext.ToReportedColumn(
                new NScript.Utils.Location("TitleEditor.skin.cshtml", 5, 66));
            reported.FileName.Should().Be("TitleEditor.skin.cshtml");
            reported.StartLine.Should().Be(5);
            reported.StartColumn.Should().Be(67);

            ConverterContext.ToReportedColumn(null).Should().BeNull();
            var noLine = new NScript.Utils.Location("App.css", 0, 0);
            ConverterContext.ToReportedColumn(noLine).Should().BeSameAs(noLine);
        }

        // --- CanHandle tests ---

        [TestMethod]
        public void CanHandle_SkinCshtmlFile_ReturnsTrue()
        {
            RazorTemplatingPlugin.CanHandle("MyTemplate.skin.cshtml").Should().BeTrue();
        }

        [TestMethod]
        public void CanHandle_UpperCaseExtension_ReturnsTrue()
        {
            RazorTemplatingPlugin.CanHandle("MyTemplate.SKIN.CSHTML").Should().BeTrue();
        }

        [TestMethod]
        public void CanHandle_MixedCaseExtension_ReturnsTrue()
        {
            RazorTemplatingPlugin.CanHandle("MyTemplate.Skin.Cshtml").Should().BeTrue();
        }

        [TestMethod]
        public void CanHandle_RegularCshtmlFile_ReturnsFalse()
        {
            RazorTemplatingPlugin.CanHandle("MyView.cshtml").Should().BeFalse();
        }

        [TestMethod]
        public void CanHandle_CsFile_ReturnsFalse()
        {
            RazorTemplatingPlugin.CanHandle("MyClass.cs").Should().BeFalse();
        }

        [TestMethod]
        public void CanHandle_HtmlFile_ReturnsFalse()
        {
            RazorTemplatingPlugin.CanHandle("Template.html").Should().BeFalse();
        }

        [TestMethod]
        public void CanHandle_EmptyString_ReturnsFalse()
        {
            RazorTemplatingPlugin.CanHandle("").Should().BeFalse();
        }

        [TestMethod]
        public void CanHandle_PathWithDirectories_ReturnsTrue()
        {
            RazorTemplatingPlugin.CanHandle("Views/Skins/TodoList.skin.cshtml").Should().BeTrue();
        }

        [TestMethod]
        public void CanHandle_DotInName_SkinCshtml_ReturnsTrue()
        {
            RazorTemplatingPlugin.CanHandle("My.App.TodoList.skin.cshtml").Should().BeTrue();
        }

        // --- Interface compliance tests ---

        [TestMethod]
        public void Plugin_ImplementsIMethodConverterPlugin()
        {
            var plugin = new RazorTemplatingPlugin();
            plugin.Should().BeAssignableTo<IMethodConverterPlugin>();
        }

        [TestMethod]
        public void Plugin_ImplementsIRuntimeConverterPlugin()
        {
            var plugin = new RazorTemplatingPlugin();
            plugin.Should().BeAssignableTo<IRuntimeConverterPlugin>();
        }

        [TestMethod]
        public void Plugin_ImplementsIConverterPlugin()
        {
            var plugin = new RazorTemplatingPlugin();
            plugin.Should().BeAssignableTo<IConverterPlugin>();
        }

        [TestMethod]
        public void Plugin_CanBeConstructed()
        {
            var plugin = new RazorTemplatingPlugin();
            plugin.Should().NotBeNull();
        }

        [TestMethod]
        public void ParseArgs_AcceptsNull()
        {
            var plugin = new RazorTemplatingPlugin();
            // ParseArgs is a no-op for Razor templates but should not throw
            plugin.ParseArgs(null);
        }

        [TestMethod]
        public void ParseArgs_AcceptsEmptyList()
        {
            var plugin = new RazorTemplatingPlugin();
            plugin.ParseArgs(new System.Collections.Generic.List<System.Tuple<string, string>>());
        }
    }
}
