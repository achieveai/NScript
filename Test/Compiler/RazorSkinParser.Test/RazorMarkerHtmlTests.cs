using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;
using System.Linq;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Regressions for the AgentWorkBench binder probes (#132, #133, #134, #135): the
    /// placeholder markup the code generator emits must survive the HTML parser, and the
    /// compile-time path scanner must find it.
    /// </summary>
    [TestClass]
    public class RazorMarkerHtmlTests
    {
        // #134: a span marker inside <textarea> becomes text (RCDATA). Owners now carry a count.
        [TestMethod]
        public void ItemTemplate_TextareaBindings_UseOwnerCountNotSpanMarkers()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "@foreach (var i in Model.Items) { <textarea placeholder=\"@i.Hint\" class=\"@i.Cls\"></textarea> }");

            var loop = ir.Children.OfType<LoopNode>().Single();
            var html = RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(loop.ItemTemplate);

            html.Should().Contain("<textarea data-ns-m=\"2\"></textarea>");
            html.Should().NotContain("<span", "no marker may sit inside RCDATA content");
        }

        [TestMethod]
        public void ItemTemplate_OwnerCount_GoesAtEndOfOpenTag()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "@foreach (var i in Model.Items) { <input class=\"@i.Cls\" data-test=\"x\" /><b>@i.Name</b> }");

            var loop = ir.Children.OfType<LoopNode>().Single();
            var html = RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(loop.ItemTemplate);

            html.Should().Contain("data-test=\"x\" data-ns-m=\"1\"/>");
            html.Should().Contain("<b><span data-ns-ph></span></b>");
        }

        // #133: a <span> anchor inside <tbody> is foster-parented out of the table; <template> is not.
        [TestMethod]
        public void LoopAnchor_IsTemplateElement()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "<table><tbody>@foreach (var i in Model.Items) { <tr><td>@i.Name</td></tr> }</tbody></table>");

            var html = RazorSkinCodeGenerator.CollectHtmlPublic(ir.Children);

            html.Should().Contain("<tbody><template data-ns-ph></template></tbody>");
        }

        // #132: a user-authored empty <span> and a void <hr> must not shift the anchor path.
        [TestMethod]
        public void FindNthEmptySpanPath_SkipsUserSpansAndVoidTags()
        {
            const string html =
                "<div><i class=\"icon\"></i><span></span><hr><div><template data-ns-ph></template></div>" +
                "<span data-ns-ph></span></div>";

            RazorSkinJSTGenerator.FindNthEmptySpanPath(html, 0).Should().Equal(0, 3, 0);
            RazorSkinJSTGenerator.FindNthEmptySpanPath(html, 1).Should().Equal(0, 4);
            RazorSkinJSTGenerator.FindNthEmptySpanPath(html, 2).Should().BeNull();
        }

        [TestMethod]
        public void IsVoidTag_RecognisesVoidElementsWithOrWithoutSlash()
        {
            RazorSkinCodeGenerator.IsVoidTag("hr").Should().BeTrue();
            RazorSkinCodeGenerator.IsVoidTag("input type=\"text\"").Should().BeTrue();
            RazorSkinCodeGenerator.IsVoidTag("br/").Should().BeTrue();
            RazorSkinCodeGenerator.IsVoidTag("span data-ns-ph").Should().BeFalse();
            RazorSkinCodeGenerator.IsVoidTag("template data-ns-ph").Should().BeFalse();
        }

        // #135: boolean attributes are toggled by presence, not by the "true"/"false" string.
        [TestMethod]
        public void IsBooleanAttribute_CoversDisabledReadonlyCheckedSelected()
        {
            GraphDescriptorJSTEmitter.IsBooleanAttribute("disabled").Should().BeTrue();
            GraphDescriptorJSTEmitter.IsBooleanAttribute("readonly").Should().BeTrue();
            GraphDescriptorJSTEmitter.IsBooleanAttribute("Checked").Should().BeTrue();
            GraphDescriptorJSTEmitter.IsBooleanAttribute("selected").Should().BeTrue();
            GraphDescriptorJSTEmitter.IsBooleanAttribute("value").Should().BeFalse();
            GraphDescriptorJSTEmitter.IsBooleanAttribute("aria-disabled").Should().BeFalse();
        }

        // Review F-001: the only content of <textarea>/<option> targets the element itself.
        [TestMethod]
        public void RawTextOnlyContent_TargetsOwnerElement_NoSpanPlaceholder()
        {
            var ir = BuildIR(
                "@model TestVM\n" +
                "@foreach (var i in Model.Items) { <div><textarea class=\"@i.Cls\">@i.Name</textarea><b class=\"@i.Css\">x</b></div> }");

            var loop = ir.Children.OfType<LoopNode>().Single();
            loop.ItemTemplate.OfType<ExpressionBindingNode>()
                .Single(b => b.Target == ExpressionTarget.TextContent).TargetsOwnerElement.Should().BeTrue();

            var html = RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(loop.ItemTemplate);
            html.Should().Contain("<textarea data-ns-m=\"2\"></textarea>", "class + content bindings on one owner");
            html.Should().NotContain("<span");
        }

        [TestMethod]
        public void RawTextOnlyContent_TopLevel_MarksOwnerWithBindIdx()
        {
            var ir = BuildIR("@model TestVM\n<div><select><option>@Model.Name</option></select><i>@Model.Other</i></div>");

            var events = RazorSkinCodeGenerator.CollectEventsPublic(ir.Children);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(ir.Children, events, paths);

            html.Should().Contain("<option></option>");
            paths.Should().HaveCount(2);
            paths[0].Should().Equal(0, 0, 0);
            paths[1].Should().Equal(0, 1, 0);
        }

        [TestMethod]
        public void RawTextMixedContent_FailsTheBuildNamingTheElement()
        {
            System.Action build = () => BuildIR(
                "@model TestVM\n@if (Model.Flag) { <textarea>Draft: @Model.Name</textarea> }");

            build.Should().Throw<NScript.Converter.ConverterLocationException>()
                .WithMessage("*'@Model.Name' is inside <textarea>*value=*");
        }

        // Review F-002: an element with its own oninput/onchange keeps value= one-way.
        [TestMethod]
        public void ValueBinding_WithOwnChangeHandler_StaysOneWay()
        {
            var withHandler = GraphTopologyBuilder.Build(TopologyTestHelpers.MakeTemplate(
                new HtmlNode { HtmlContent = "<input " },
                ValueBinding("Model.Title"),
                new HtmlNode { HtmlContent = " " },
                TopologyTestHelpers.MakeEvent("change", "Model.OnTitleChange"),
                new HtmlNode { HtmlContent = " />" }));
            var without = GraphTopologyBuilder.Build(TopologyTestHelpers.MakeTemplate(
                new HtmlNode { HtmlContent = "<input " },
                ValueBinding("Model.Title"),
                new HtmlNode { HtmlContent = " /><button " },
                TopologyTestHelpers.MakeEvent("change", "Model.OnTitleChange"),
                new HtmlNode { HtmlContent = "></button>" }));

            withHandler.DomTargets.Single().TwoWaySourceProperty.Should().BeNull("the author's onchange owns write-back");
            without.DomTargets.Single().TwoWaySourceProperty.Should().Be("Title", "the handler is on another element");
        }

        // #145: the loop variable of a T[] / List<T> source resolves to T.
        [TestMethod]
        public void CollectionItemTypeName_ResolvesArrayAndGenericElementTypes()
        {
            var item = new Mono.Cecil.TypeReference("Probe", "MkItem", null, null);
            var array = new Mono.Cecil.PropertyDefinition(
                "Arr", Mono.Cecil.PropertyAttributes.None, new Mono.Cecil.ArrayType(item));
            var listType = new Mono.Cecil.GenericInstanceType(
                new Mono.Cecil.TypeReference("System.Collections.Generic", "List`1", null, null));
            listType.GenericArguments.Add(item);
            var list = new Mono.Cecil.PropertyDefinition("List", Mono.Cecil.PropertyAttributes.None, listType);

            CecilTypeHelper.CollectionItemTypeName(array).Should().Be("Probe.MkItem");
            CecilTypeHelper.CollectionItemTypeName(list).Should().Be("Probe.MkItem");
        }

        // #144: a bound attribute defaults to null so a null value removes it.
        [TestMethod]
        public void AttributeTarget_DefaultsToNull_OtherTargetsToEmpty()
        {
            var title = TopologyTestHelpers.MakeBinding(
                "Model.Maybe", BindingMode.OneWay, ExpressionTarget.Attribute, null, "Maybe");
            title.AttributeName = "title";
            var text = TopologyTestHelpers.MakeBinding(
                "Model.Text", BindingMode.OneWay, ExpressionTarget.TextContent, null, "Text");

            var topology = GraphTopologyBuilder.Build(TopologyTestHelpers.MakeTemplate(
                new HtmlNode { HtmlContent = "<span " }, title, new HtmlNode { HtmlContent = ">" },
                text, new HtmlNode { HtmlContent = "</span>" }));

            var attr = topology.DomTargets.Single(d => d.Target == ExpressionTarget.Attribute);
            var txt = topology.DomTargets.Single(d => d.Target == ExpressionTarget.TextContent);
            topology.DefaultValues[attr.NodeIdx].Should().BeNull();
            topology.DefaultValues[txt.NodeIdx].Should().Be("");
        }

        private static ExpressionBindingNode ValueBinding(string expr)
        {
            var binding = TopologyTestHelpers.MakeBinding(
                expr, BindingMode.OneWay, ExpressionTarget.Attribute, null, expr.Substring("Model.".Length));
            binding.AttributeName = "value";
            return binding;
        }

        private static SkinTemplateNode BuildIR(string template)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);
            return TemplateIRBuilder.Build("TestSkin", preprocessed, parsed);
        }
    }
}
