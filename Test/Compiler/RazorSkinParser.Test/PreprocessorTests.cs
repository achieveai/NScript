using System;
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

        // --- Error / diagnostic path tests ---

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DuplicateModelDirective_ThrowsInvalidOperationException()
        {
            var source = "@model MyViewModel\n@model AnotherViewModel\n<div>test</div>";
            RazorSkinPreprocessor.Process(source);
        }

        [TestMethod]
        public void EmptyTemplateBody_ProducesEmptyCleanedTemplate()
        {
            // Template with only a @model directive and no HTML body
            var source = "@model MyVM";
            var result = RazorSkinPreprocessor.Process(source);

            result.ModelTypeName.Should().Be("MyVM");
            result.CleanedTemplate.Should().Contain("@model MyVM");
        }

        [TestMethod]
        public void MissingModelDirective_ModelTypeNameIsNull()
        {
            // Template with no @model directive
            var source = "<div>Hello World</div>";
            var result = RazorSkinPreprocessor.Process(source);

            result.ModelTypeName.Should().BeNull();
            result.CleanedTemplate.Should().Contain("<div>Hello World</div>");
        }

        [TestMethod]
        public void MissingModelDirective_StillDefaultsControlType()
        {
            var source = "<div>Hello</div>";
            var result = RazorSkinPreprocessor.Process(source);

            result.ControlTypeName.Should().Be("Sunlight.Framework.UI.UISkinableElement");
        }

        [TestMethod]
        public void WhitespaceOnlyBody_HandledGracefully()
        {
            var source = "@model MyVM\n\n   \n\n";
            var result = RazorSkinPreprocessor.Process(source);

            result.ModelTypeName.Should().Be("MyVM");
        }
    }
}
