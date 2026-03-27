using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class PreprocessorTests
    {
        [TestMethod]
        public void ExtractsModelDirective()
        {
            var input = "@model MyApp.ViewModels.OrderVM\n<div>Hello</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.ModelTypeName.Should().Be("MyApp.ViewModels.OrderVM");
            result.CleanedTemplate.Should().Contain("@model MyApp.ViewModels.OrderVM");
        }

        [TestMethod]
        public void ExtractsControlDirective()
        {
            var input = "@model MyVM\n@control Sunlight.Framework.UI.UISkinableElement\n<div>Hello</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.ControlTypeName.Should().Be("Sunlight.Framework.UI.UISkinableElement");
            result.CleanedTemplate.Should().NotContain("@control");
        }

        [TestMethod]
        public void DefaultsControlToUISkinableElement()
        {
            var input = "@model MyVM\n<div>Hello</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.ControlTypeName.Should().Be("Sunlight.Framework.UI.UISkinableElement");
        }

        [TestMethod]
        public void ExtractsUsingDirectives()
        {
            var input = "@model MyVM\n@using Sunlight.Framework.UI\n@using MyApp.Controls\n<div/>";
            var result = RazorSkinPreprocessor.Process(input);

            result.UsingNamespaces.Should().BeEquivalentTo(
                new[] { "Sunlight.Framework.UI", "MyApp.Controls" });
        }

        [TestMethod]
        public void PreservesTemplateBodyAfterDirectiveRemoval()
        {
            var input = "@model MyVM\n@control MyCtrl\n@using NS1\n\n<div class=\"test\">\n    <span>@Model.Name</span>\n</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.CleanedTemplate.Should().Contain("<div class=\"test\">");
            result.CleanedTemplate.Should().Contain("@Model.Name");
            result.CleanedTemplate.Should().NotContain("@control");
        }
    }
}
