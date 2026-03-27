using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;
using System.Linq;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class TemplateIRBuilderTests
    {
        [TestMethod]
        public void StaticHtmlProducesHtmlNode()
        {
            var ir = BuildIR("@model TestVM\n\n<div>Hello World</div>");

            ir.Children.Should().ContainSingle()
                .Which.Should().BeOfType<HtmlNode>();
        }

        [TestMethod]
        public void SimpleExpressionProducesExpressionBindingNode()
        {
            var ir = BuildIR("@model TestVM\n\n<div>@Model.Name</div>");

            // At minimum, the IR should contain an expression binding
            ir.Children.OfType<ExpressionBindingNode>().Should().NotBeEmpty();
        }

        [TestMethod]
        public void IfBlockProducesConditionalNode()
        {
            var ir = BuildIR("@model TestVM\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}");

            ir.Children.OfType<ConditionalNode>().Should().NotBeEmpty();
        }

        [TestMethod]
        public void ForeachBlockProducesLoopNode()
        {
            var ir = BuildIR("@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}");

            ir.Children.OfType<LoopNode>().Should().NotBeEmpty();
        }

        [TestMethod]
        public void FunctionsBlockProducesFunctionNodes()
        {
            var ir = BuildIR("@model TestVM\n\n@functions {\n    string Fmt(int x) => x.ToString();\n}\n\n<div>@Fmt(42)</div>");

            ir.Functions.Should().NotBeEmpty();
            ir.Functions.First().FunctionName.Should().Be("Fmt");
        }

        private SkinTemplateNode BuildIR(string template)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            return TemplateIRBuilder.Build(
                "TestSkin",
                preprocessed,
                parsed);
        }
    }
}
