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

        [TestMethod]
        public void SubControlDetectedFromPascalCaseTag()
        {
            var ir = BuildIR("@model TestVM\n\n<div><ListView id=\"myList\" ItemCssClassName=\"item\" /></div>");

            ir.Children.OfType<SubControlNode>().Should().NotBeEmpty();
            var sub = ir.Children.OfType<SubControlNode>().First();
            sub.TypeName.Should().Be("ListView");
            sub.ElementId.Should().Be("myList");
        }

        [TestMethod]
        public void SubControlPropertyBindingsExtracted()
        {
            var ir = BuildIR("@model TestVM\n\n<div><SearchBox Query=\"Model.Query\" /></div>");

            var sub = ir.Children.OfType<SubControlNode>().FirstOrDefault();
            sub.Should().NotBeNull();
            sub.PropertyBindings.Should().Contain(p => p.PropertyName == "Query");
        }

        // --- Content-validating assertions ---

        [TestMethod]
        public void HtmlNodeContent_PreservesStaticHtml()
        {
            var ir = BuildIR("@model TestVM\n\n<div class=\"container\">Hello World</div>");

            var html = ir.Children.OfType<HtmlNode>().FirstOrDefault();
            html.Should().NotBeNull();
            html.HtmlContent.Should().Contain("container");
            html.HtmlContent.Should().Contain("Hello World");
        }

        [TestMethod]
        public void ExpressionBindingNode_CapturesCSharpExpression()
        {
            var ir = BuildIR("@model TestVM\n\n<span>@Model.Name</span>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Classification.CSharpExpression.Should().Be("Model.Name");
            binding.Classification.SourceKind.Should().Be(BindingSourceKind.DataContext);
        }

        [TestMethod]
        public void ComputedExpression_CapturesFullExpression()
        {
            var ir = BuildIR("@model TestVM\n\n<span>@(Model.Price * Model.Quantity)</span>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Classification.CSharpExpression.Should().Contain("Model.Price");
            binding.Classification.CSharpExpression.Should().Contain("Model.Quantity");
        }

        [TestMethod]
        public void ConditionalNode_CapturesConditionExpression()
        {
            var ir = BuildIR("@model TestVM\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}");

            var cond = ir.Children.OfType<ConditionalNode>().First();
            cond.Condition.CSharpExpression.Should().Be("Model.IsActive");
            cond.Condition.SourceKind.Should().Be(BindingSourceKind.DataContext);
        }

        [TestMethod]
        public void ConditionalNode_CapturesBothBranches()
        {
            var ir = BuildIR("@model TestVM\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}\nelse\n{\n    <div>Inactive</div>\n}");

            var cond = ir.Children.OfType<ConditionalNode>().First();
            cond.TrueBranch.Should().NotBeEmpty();
            cond.FalseBranch.Should().NotBeEmpty();

            var trueHtml = cond.TrueBranch.OfType<HtmlNode>().First();
            trueHtml.HtmlContent.Should().Contain("Active");

            var falseHtml = cond.FalseBranch.OfType<HtmlNode>().First();
            falseHtml.HtmlContent.Should().Contain("Inactive");
        }

        [TestMethod]
        public void LoopNode_CapturesCollectionExpression()
        {
            var ir = BuildIR("@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}");

            var loop = ir.Children.OfType<LoopNode>().First();
            loop.CollectionExpression.Should().Be("Model.Items");
            loop.ItemVariableName.Should().Be("item");
            loop.CollectionSourceKind.Should().Be(BindingSourceKind.DataContext);
        }

        [TestMethod]
        public void LoopNode_ItemTemplateContainsBindings()
        {
            var ir = BuildIR("@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}");

            var loop = ir.Children.OfType<LoopNode>().First();
            loop.ItemTemplate.Should().NotBeEmpty();
            loop.ItemTemplate.OfType<ExpressionBindingNode>().Should().NotBeEmpty();
        }

        [TestMethod]
        public void FunctionNode_CapturesSourceAndPurity()
        {
            var ir = BuildIR("@model TestVM\n\n@functions {\n    string Fmt(int x) => x.ToString();\n    string FullName() => Model.FirstName + \" \" + Model.LastName;\n}\n\n<div>test</div>");

            var pureFn = ir.Functions.FirstOrDefault(f => f.FunctionName == "Fmt");
            pureFn.Should().NotBeNull();
            pureFn.IsPure.Should().BeTrue();
            pureFn.CSharpSource.Should().Contain("x.ToString()");

            var modelFn = ir.Functions.FirstOrDefault(f => f.FunctionName == "FullName");
            modelFn.Should().NotBeNull();
            modelFn.IsPure.Should().BeFalse();
            modelFn.CSharpSource.Should().Contain("Model.FirstName");
        }

        [TestMethod]
        public void ControlBinding_SetsSourceKindToTemplateParent()
        {
            var ir = BuildIR("@model TestVM\n\n<div>@Control.CssClass</div>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Classification.SourceKind.Should().Be(BindingSourceKind.TemplateParent);
            binding.Classification.CSharpExpression.Should().Contain("Control.");
        }

        // --- Attribute binding IR classification tests ---

        [TestMethod]
        public void AttributeBinding_CssClass_ProducesCssClassTarget()
        {
            var ir = BuildIR("@model TestVM\n\n<div class=\"@Model.CssClass\">Hello</div>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().FirstOrDefault();
            binding.Should().NotBeNull("class attribute binding should produce an ExpressionBindingNode");
            binding.Target.Should().Be(ExpressionTarget.CssClass);
            binding.Classification.CSharpExpression.Should().Contain("Model.CssClass");
            binding.Classification.SourceKind.Should().Be(BindingSourceKind.DataContext);
        }

        [TestMethod]
        public void AttributeBinding_Style_ProducesStyleTarget()
        {
            var ir = BuildIR("@model TestVM\n\n<div style=\"display: @Model.DisplayStyle\">Content</div>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().FirstOrDefault();
            binding.Should().NotBeNull("style attribute binding should produce an ExpressionBindingNode");
            binding.Target.Should().Be(ExpressionTarget.Style);
            binding.AttributePrefix.Should().Contain("display");
            binding.Classification.CSharpExpression.Should().Contain("Model.DisplayStyle");
        }

        [TestMethod]
        public void EventBinding_OnClick_ProducesEventNode()
        {
            var ir = BuildIR("@model TestVM\n\n<button onclick=\"@Model.HandleClick\">Click</button>");

            var eventNode = ir.Children.OfType<EventNode>().FirstOrDefault();
            eventNode.Should().NotBeNull("onclick attribute should produce an EventNode");
            eventNode.DomEventName.Should().Be("click");
            eventNode.HandlerExpression.Should().Contain("Model.HandleClick");
        }

        [TestMethod]
        public void AttributeBinding_DataAttribute_ProducesGenericAttributeBinding()
        {
            var ir = BuildIR("@model TestVM\n\n<div data-id=\"@Model.Id\">Content</div>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().FirstOrDefault();
            binding.Should().NotBeNull("data-* attribute binding should produce an ExpressionBindingNode");
            binding.Target.Should().Be(ExpressionTarget.Attribute);
            binding.AttributeName.Should().Be("data-id");
            binding.Classification.CSharpExpression.Should().Contain("Model.Id");
        }

        [TestMethod]
        public void AttributeBinding_Title_ProducesAttributeBinding()
        {
            var ir = BuildIR("@model TestVM\n\n<span title=\"@Model.Tooltip\">Hover me</span>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().FirstOrDefault();
            binding.Should().NotBeNull("title attribute binding should produce an ExpressionBindingNode");
            binding.Target.Should().Be(ExpressionTarget.Attribute);
            binding.AttributeName.Should().Be("title");
            binding.Classification.CSharpExpression.Should().Contain("Model.Tooltip");
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
