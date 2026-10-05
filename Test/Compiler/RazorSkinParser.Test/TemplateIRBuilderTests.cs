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
        public void FullyQualifiedSubControlTagKeepsItsTypeName()
        {
            var ir = BuildIR("@model TestVM\n\n<Controls.Shared.ListView id=\"myList\" />");

            ir.Children.OfType<SubControlNode>().Should().ContainSingle()
                .Which.TypeName.Should().Be("Controls.Shared.ListView");
        }

        [TestMethod]
        public void SubControlPropertyBindingsExtracted()
        {
            var ir = BuildIR("@model TestVM\n\n<div><SearchBox Query=\"Model.Query\" /></div>");

            var sub = ir.Children.OfType<SubControlNode>().FirstOrDefault();
            sub.Should().NotBeNull();
            sub.PropertyBindings.Should().Contain(p => p.PropertyName == "Query" && p.IsLiteral && p.Classification.CSharpExpression == "Model.Query");
        }

        [TestMethod]
        public void SubControlDynamicAttributesRemainOnTheControl()
        {
            var ir = BuildIR("@model TestVM\n<div class=\"parent\"><SearchBox Query=\"@Model.Query\" ItemSkin=\"@Some.Type.StaticSkin\" Placeholder=\"literal\" /></div>");

            var sub = ir.Children.OfType<SubControlNode>().Single();
            sub.PropertyBindings.Should().Contain(p => p.PropertyName == "Query" && !p.IsLiteral && p.Classification.CSharpExpression == "Model.Query");
            sub.PropertyBindings.Should().Contain(p => p.PropertyName == "ItemSkin" && !p.IsLiteral && p.Classification.CSharpExpression == "Some.Type.StaticSkin");
            sub.PropertyBindings.Should().Contain(p => p.PropertyName == "Placeholder" && p.IsLiteral && p.Classification.CSharpExpression == "literal");
            ir.Children.OfType<ExpressionBindingNode>().Should().BeEmpty();
            NScript.RazorSkin.CodeGen.GraphTopologyBuilder.Build(ir);
            NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children)
                .Should().Contain("<div class=\"parent\">").And.Contain("</div>");
        }

        [TestMethod]
        public void OnPrefixedControlPropertyIsNotDiscardedAsDomEvent()
        {
            var ir = BuildIR("@model TestVM\n<TodoItemControl OnSelected=\"@Model.Select\" onclick=\"@Model.Click\" />");

            var sub = ir.Children.OfType<SubControlNode>().Single();
            sub.PropertyBindings.Should().ContainSingle()
                .Which.PropertyName.Should().Be("OnSelected");
            sub.EventBindings.Should().ContainSingle()
                .Which.DomEventName.Should().Be("click");
        }

        [TestMethod]
        public void SubControlExplicitExpressionIsOneBinding()
        {
            var ir = BuildIR("@model TestVM\n<SearchBox Query=\"@(Model.First + Model.Last)\" />");

            var sub = ir.Children.OfType<SubControlNode>().Single();
            sub.PropertyBindings.Should().ContainSingle()
                .Which.Classification.CSharpExpression.Should().Be("Model.First + Model.Last");
            sub.PropertyBindings.Single().IsLiteral.Should().BeFalse();
            ir.Children.OfType<ExpressionBindingNode>().Should().BeEmpty();
        }

        [TestMethod]
        public void SubControlExpressionCanContainComparison()
        {
            var ir = BuildIR("@model TestVM\n<SearchBox Visible=\"@(Model.Count > 0)\" />");

            ir.Children.OfType<SubControlNode>().Single().PropertyBindings
                .Should().ContainSingle().Which.Classification.CSharpExpression
                .Should().Be("Model.Count > 0");
        }

        [TestMethod]
        public void ForeachSubControlKeepsItemBinding()
        {
            var ir = BuildIR("@model TestVM\n@foreach (var item in Model.Items) { <MenuItem Label=\"@item.Name\" /> }");

            var loop = ir.Children.OfType<LoopNode>().Single();
            loop.ItemTemplate.OfType<SubControlNode>().Single().PropertyBindings
                .Should().ContainSingle().Which.Classification.CSharpExpression
                .Should().Be("item.Name");
        }

        [TestMethod]
        public void ConditionalSubControlIsExtracted()
        {
            var ir = BuildIR("@model TestVM\n@if (Model.Show) { <SearchBox Query=\"@Model.Query\" /> }");

            var conditional = ir.Children.OfType<ConditionalNode>().Single();
            conditional.TrueBranch.OfType<SubControlNode>().Single().PropertyBindings
                .Should().ContainSingle().Which.Classification.CSharpExpression
                .Should().Be("Model.Query");
        }

        [TestMethod]
        public void SubControlKeepsItsPositionAmongParentMarkup()
        {
            var ir = BuildIR("@model TestVM\n<div>before <SearchBox /> after</div>");

            ir.Children.Select(node => node is SubControlNode ? "control" :
                    node is HtmlNode html ? html.HtmlContent : "other")
                .Should().Equal("<div>before ", "control", " after</div>");
            NScript.RazorSkin.CodeGen.GraphTopologyBuilder.Build(ir);
            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.IndexOf("before ").Should().BeLessThan(html.IndexOf("data-ns-subctl"));
            html.IndexOf("data-ns-subctl").Should().BeLessThan(html.IndexOf(" after"));
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

        // --- Attribute stripping whitespace preservation ---

        [TestMethod]
        public void BindingAttributeStrip_PreservesSpaceBeforeStaticAttributes()
        {
            // When class="@Model.X" is stripped, the space before the next attribute must remain.
            // Regression: Razor eats inter-attribute whitespace for structured attributes;
            // EnsureTrailingSpaceOnPrecedingHtml() restores it.
            var ir = BuildIR(
                "@model TestVM\n\n<div class=\"@Model.CssClass\" draggable=\"true\">Hello</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain(" draggable=\"true\"",
                "space between tag name and static attribute must be preserved when binding attribute is stripped");
            html.Should().NotContain("<divdraggable",
                "binding attribute removal must not eat the whitespace separator");
        }

        [TestMethod]
        public void EventAttributeStrip_PreservesSpaceBeforeStaticAttributes()
        {
            // When onclick="@Model.X" is stripped, the space before the next attribute must remain.
            var ir = BuildIR(
                "@model TestVM\n\n<div onclick=\"@Model.Click\" title=\"hello\">Hello</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("title=\"hello\"",
                "static attribute after stripped event should remain intact");
        }

        [TestMethod]
        public void MultipleBindingStrips_PreserveAllSpaces()
        {
            // Both class and onclick are bindings; draggable is static and must survive.
            var ir = BuildIR(
                "@model TestVM\n\n<div class=\"@Model.Css\" draggable=\"true\" onclick=\"@Model.Click\">Hello</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("draggable=\"true\"",
                "static attribute between two bindings must be preserved");
        }

        [TestMethod]
        public void ForeachItemTemplate_BindingStrip_PreservesSpaces()
        {
            var ir = BuildIR(
                "@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <div class=\"@item.Css\" draggable=\"true\">@item.Name</div>\n}");

            var loop = ir.Children.OfType<LoopNode>().FirstOrDefault();
            loop.Should().NotBeNull();

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(loop.ItemTemplate);
            html.Should().Contain("<div draggable=\"true\"",
                "foreach item template must preserve space when binding attribute is stripped");
            html.Should().NotContain("<divdraggable",
                "binding attribute removal must not eat the whitespace separator in item templates");
        }

        // --- Bound attribute must not delete earlier static HTML ---
        // Regression for the nscript-bound-attr-trim-bug: a bound attribute such as
        // class="@Model.X" used LastIndexOf(attrName + "=") over the whole preceding HTML
        // block. For the structured (HtmlAttributeIntermediateNode) representation the bound
        // attribute is NOT in that HTML, so the search found an EARLIER element's attribute
        // and cut from there, silently deleting static markup. The fix cuts only the
        // still-open attribute at the tail of the open tag.

        [TestMethod]
        public void BoundClassAttribute_DoesNotDeleteEarlierStaticElement()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div class=\"wrap\">\n" +
                "    <p>@Model.Text</p>\n" +
                "    <span class=\"static-one\" data-test=\"repro-static\">STATIC_TEXT</span>\n" +
                "    <span class=\"@Model.CssClass\" data-test=\"repro-bound\">BOUND_TEXT</span>\n" +
                "</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("static-one", "the earlier static element's class must survive");
            html.Should().Contain("STATIC_TEXT", "the earlier static element's text must survive");
            html.Should().Contain("repro-static");
            html.Should().Contain("repro-bound", "the bound element must still be emitted");
            ir.Children.OfType<ExpressionBindingNode>()
                .Should().Contain(b => b.Target == ExpressionTarget.CssClass
                    && b.Classification.CSharpExpression.Contains("Model.CssClass"),
                    "the bound class becomes a binding, not static class text");
        }

        [TestMethod]
        public void BoundTitleAttribute_DoesNotDeleteEarlierStaticElementWithTitle()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div>\n" +
                "    <span title=\"static-title\" data-test=\"s\">AAA</span>\n" +
                "    <span title=\"@Model.Tip\" data-test=\"b\">BBB</span>\n" +
                "</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("static-title", "earlier static title must survive");
            html.Should().Contain("AAA");
            html.Should().Contain("BBB");
            ir.Children.OfType<ExpressionBindingNode>()
                .Should().Contain(b => b.AttributeName == "title"
                    && b.Classification.CSharpExpression.Contains("Model.Tip"));
        }

        [TestMethod]
        public void BoundClassAttribute_WhenBoundElementHasEarlierStaticAttribute_KeepsBothElements()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div>\n" +
                "    <span class=\"earlier\" data-test=\"e\">E</span>\n" +
                "    <span data-test=\"x\" class=\"@Model.Y\">Y</span>\n" +
                "</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("earlier", "earlier static element must survive");
            html.Should().Contain("data-test=\"x\"", "the bound element's own static attribute must stay on it");
            html.Should().Contain("data-test=\"e\"");
        }

        [TestMethod]
        public void BoundClassAttribute_DoesNotFalseMatchDataClassOrQuotedClassText()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div>\n" +
                "    <span data-class=\"keep1\" title=\"class=keep2\" data-test=\"e\">E</span>\n" +
                "    <span class=\"@Model.Y\" data-test=\"b\">B</span>\n" +
                "</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("data-class=\"keep1\"", "data-class is not a class attribute");
            html.Should().Contain("class=keep2", "a class= inside a quoted value is not an attribute");
            html.Should().Contain("E");
        }

        [TestMethod]
        public void BoundClassAttribute_WithLiteralPrefix_KeepsEarlierElementAndPrefix()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div>\n" +
                "    <span class=\"earlier\" data-test=\"e\">E</span>\n" +
                "    <span class=\"base @Model.Y\" data-test=\"b\">B</span>\n" +
                "</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("earlier", "earlier static element must survive");

            var binding = ir.Children.OfType<ExpressionBindingNode>()
                .FirstOrDefault(b => b.Target == ExpressionTarget.CssClass);
            binding.Should().NotBeNull();
            binding.AttributePrefix.Should().Contain("base", "the literal class prefix must be preserved");
            binding.Classification.CSharpExpression.Should().Contain("Model.Y");
        }

        [TestMethod]
        public void BoundClassAttribute_InForeach_DoesNotDeleteEarlierStaticElement()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "@foreach (var item in Model.Items)\n{\n" +
                "    <span class=\"static-one\" data-test=\"s\">S</span>\n" +
                "    <span class=\"@item.Css\" data-test=\"b\">B</span>\n" +
                "}");

            var loop = ir.Children.OfType<LoopNode>().Single();
            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(loop.ItemTemplate);
            html.Should().Contain("static-one", "earlier static element in the loop body must survive");
            html.Should().Contain("data-test=\"s\"");
        }

        [TestMethod]
        public void BoundClassAttribute_InIfElseBranches_DoesNotDeleteEarlierStaticElements()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "@if (Model.Flag)\n{\n" +
                "    <span class=\"static-one\" data-test=\"s1\">S1</span>\n" +
                "    <span class=\"@Model.Css\" data-test=\"b1\">B1</span>\n" +
                "}\nelse\n{\n" +
                "    <span class=\"static-two\" data-test=\"s2\">S2</span>\n" +
                "    <span class=\"@Model.Css2\" data-test=\"b2\">B2</span>\n" +
                "}");

            var cond = ir.Children.OfType<ConditionalNode>().Single();
            var trueHtml = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(cond.TrueBranch);
            var falseHtml = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(cond.FalseBranch);
            trueHtml.Should().Contain("static-one", "earlier static element in the if-branch must survive");
            falseHtml.Should().Contain("static-two", "earlier static element in the else-branch must survive");
        }

        [TestMethod]
        public void BoundClassAttribute_AsFirstChild_EmitsElementAndBinding()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div class=\"@Model.Css\" data-test=\"only\">X</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("data-test=\"only\"");
            html.Should().Contain("</div>");
            html.Should().NotContain("<divdata-test", "tag name and attribute must not fuse");
            ir.Children.OfType<ExpressionBindingNode>()
                .Should().Contain(b => b.Target == ExpressionTarget.CssClass);
        }

        [TestMethod]
        public void ConsecutiveBoundAttributes_OnSameTag_AllBecomeBindingsWithoutError()
        {
            // The inline path strips each binding's closing quote, so a later bound attribute
            // begins the next HTML node (e.g. data-count=") with no leading whitespace. The trim
            // helper must recognise it there and must not raise a false "refusing to trim" error.
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div data-test=\"1\" title=\"@Model.Title\" data-count=\"@Model.Count\">Attributed</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("data-test=\"1\"", "the static attribute must remain");
            html.Should().Contain(">Attributed<");
            var attrs = ir.Children.OfType<ExpressionBindingNode>().Select(b => b.AttributeName).ToList();
            attrs.Should().Contain("title").And.Contain("data-count");
        }

        [TestMethod]
        public void MultipleBoundAttributes_ClassTitleData_AllBecomeBindings()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<div data-test=\"1\" class=\"@Model.CssClass\" title=\"@Model.Title\" data-count=\"@Model.Count\">Multi</div>");

            var html = NScript.RazorSkin.CodeGen.RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);
            html.Should().Contain("data-test=\"1\"");
            html.Should().Contain(">Multi<");
            var attrs = ir.Children.OfType<ExpressionBindingNode>().Select(b => b.AttributeName).ToList();
            attrs.Should().Contain("class").And.Contain("title").And.Contain("data-count");
        }
    }
}
