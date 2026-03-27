using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class RazorParsingTests
    {
        [TestMethod]
        public void ParsesSimpleTemplateToGeneratedCSharp()
        {
            var template = "@model TestModel\n\n<div>\n    <span>@Model.Name</span>\n</div>";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().NotBeNullOrEmpty();
            result.GeneratedCSharp.Should().Contain("WriteLiteral");
            result.GeneratedCSharp.Should().Contain("Model.Name");
        }

        [TestMethod]
        public void ProducesRazorSyntaxTree()
        {
            var template = "@model TestModel\n\n<div>@Model.Name</div>";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.SyntaxTree.Should().NotBeNull();
        }

        [TestMethod]
        public void HandlesIfBlocks()
        {
            var template = "@model TestModel\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().Contain("Model.IsActive");
        }

        [TestMethod]
        public void HandlesForeachBlocks()
        {
            var template = "@model TestModel\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().Contain("Model.Items");
            result.GeneratedCSharp.Should().Contain("item.Name");
        }

        [TestMethod]
        public void HandlesFunctionsBlock()
        {
            var template = "@model TestModel\n\n@functions {\n    string Format(int x) => x.ToString();\n}\n\n<div>@Format(Model.Count)</div>";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().Contain("Format");
        }
    }
}
