using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.Converter;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Content projection (issue #137): caller content inside a control tag and the
    /// @RenderBody() outlet in the control's skin.
    /// </summary>
    [TestClass]
    public class RazorSlotCompilerTests
    {
        private const string Outlet = "<template data-ns-slot=\"\"></template><template data-ns-slot=\"\"></template>";

        [TestMethod]
        public void ContentHost_EmitsContentInsideHostAndPathsThroughIt()
        {
            var ir = BuildIR("@model TestVM\n<Frame><p>@Model.Name</p></Frame>");
            NumberSubControls(ir.Children);

            var events = RazorSkinCodeGenerator.CollectEventsPublic(ir.Children);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(ir.Children, events, paths);

            html.Should().Contain("<div data-ns-content data-ns-subctl=\"0\"><p><span data-ns-ph></span></p></div>");
            paths.Should().ContainEquivalentOf(new List<int> { 0, 0, 0 }, "the caller's text marker is resolved through the host");
        }

        [TestMethod]
        public void SelfClosingAndWhitespaceOnlyHosts_HaveNoContent()
        {
            var ir = BuildIR("@model TestVM\n<div><Frame /><Frame>  \n </Frame></div>");

            var subs = ir.Children.OfType<SubControlNode>().ToList();
            subs.Should().HaveCount(2);
            subs.Should().OnlyContain(sub => !sub.HasContent);
            ir.Children.OfType<SubControlEndNode>().Should().BeEmpty();
        }

        [TestMethod]
        public void RenderBody_EmitsAnchorPair()
        {
            var ir = BuildIR("@model TestVM\n<section>@RenderBody()</section>");

            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(
                ir.Children, RazorSkinCodeGenerator.CollectEventsPublic(ir.Children), new List<List<int>>());

            html.Should().Contain("<section>" + Outlet + "</section>");
            ir.Children.OfType<ExpressionBindingNode>().Should().BeEmpty("the outlet is markup, not a binding");
        }

        [TestMethod]
        public void RenderBody_InIfBranch_IsInTheBranchTemplate()
        {
            var ir = BuildIR("@model TestVM\n<div>@if (Model.Flag) { <section>@RenderBody()</section> }</div>");

            var gate = ir.Children.OfType<ConditionalNode>().Single();
            RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(gate.TrueBranch)
                .Should().Contain("<section>" + Outlet + "</section>");
        }

        [TestMethod]
        public void RenderBody_OncePerIfAndElseBranch_IsAllowed()
        {
            Action build = () => BuildIR(
                "@model TestVM\n@if (Model.Flag) { <section>@RenderBody()</section> } else { <aside>@RenderBody()</aside> }");

            build.Should().NotThrow();
        }

        [TestMethod]
        public void RenderBody_InNestedIf_IsAllowed()
        {
            Action build = () => BuildIR(
                "@model TestVM\n@if (Model.Flag) { <div>@if (Model.Other) { <section>@RenderBody()</section> }</div> }");

            build.Should().NotThrow();
        }

        [TestMethod]
        public void RenderBody_InForeach_IsACompileError()
        {
            Action build = () => BuildIR(
                "@model TestVM\n@foreach (var i in Model.Items) { <li>@RenderBody()</li> }");

            build.Should().Throw<ConverterLocationException>().WithMessage("*@RenderBody()*@foreach*");
        }

        [TestMethod]
        public void RenderBody_InIfInsideForeach_IsACompileError()
        {
            Action build = () => BuildIR(
                "@model TestVM\n@foreach (var i in Model.Items) { <li>@if (i.Flag) { <b>@RenderBody()</b> }</li> }");

            build.Should().Throw<ConverterLocationException>().WithMessage("*@RenderBody()*@foreach*");
        }

        [TestMethod]
        public void RenderBody_TwiceOnOnePath_IsACompileError()
        {
            Action build = () => BuildIR(
                "@model TestVM\n<header>@RenderBody()</header>@if (Model.Flag) { <section>@RenderBody()</section> }");

            build.Should().Throw<ConverterLocationException>().WithMessage("*@RenderBody()*2 times*");
        }

        [TestMethod]
        public void RenderBody_InAttribute_IsACompileError()
        {
            Action build = () => BuildIR("@model TestVM\n<div title=\"@RenderBody()\"></div>");

            build.Should().Throw<ConverterLocationException>().WithMessage("*outlet*alone*");
        }

        [TestMethod]
        public void ContentHost_ClosedInsideAnIf_FailsTheBuild()
        {
            Action build = () => BuildIR("@model TestVM\n<Frame>@if (Model.Flag) { </Frame> }");

            build.Should().Throw<ConverterLocationException>().WithMessage("*same block*");
        }

        [TestMethod]
        public void NamedSlot_CompilesToBoundaryMarkersNotAControl()
        {
            var ir = BuildIR("@model TestVM\n<Frame><Slot Name=\"head\"><h2>Hi</h2></Slot><p>body</p></Frame>");
            NumberSubControls(ir.Children);

            ir.Children.OfType<SubControlNode>().Should().ContainSingle(sub => sub.TypeName == "Frame");
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(
                ir.Children, RazorSkinCodeGenerator.CollectEventsPublic(ir.Children), new List<List<int>>());
            html.Should().Contain("<template data-ns-slot-for=\"head\"></template><h2>Hi</h2><template data-ns-slot-end></template><p>body</p>");
        }

        [TestMethod]
        public void RenderSlot_EmitsNamedAnchorPair()
        {
            var ir = BuildIR("@model TestVM\n<footer>@RenderSlot(\"actions\")</footer>");

            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(
                ir.Children, RazorSkinCodeGenerator.CollectEventsPublic(ir.Children), new List<List<int>>());
            html.Should().Contain("<footer><template data-ns-slot=\"actions\"></template><template data-ns-slot=\"actions\"></template></footer>");
        }

        [TestMethod]
        public void NamedSlot_WithoutLiteralName_IsACompileError()
        {
            Action build = () => BuildIR("@model TestVM\n<Frame><Slot><i>x</i></Slot></Frame>");

            build.Should().Throw<ConverterLocationException>().WithMessage("*<Slot> needs a literal Name*");
        }

        [TestMethod]
        public void NamedSlot_OutsideAControl_IsACompileError()
        {
            Action build = () => BuildIR("@model TestVM\n<div><Slot Name=\"head\"><i>x</i></Slot></div>");

            build.Should().Throw<ConverterLocationException>().WithMessage("*<Slot>*directly inside the control's tag*");
        }

        // PR 148 S6: caught at compile time, at the Slot tag's own line, not when the control mounts.
        [TestMethod]
        public void NamedSlot_TwiceInOneControl_IsACompileErrorAtTheSecondSlot()
        {
            Action build = () => BuildIR("@model TestVM\n<Frame>\n<Slot Name=\"head\"><i>a</i></Slot>\n"
                + "<Slot Name=\"head\"><i>b</i></Slot>\n</Frame>");

            var location = build.Should().Throw<ConverterLocationException>()
                .WithMessage("*<Frame> has two <Slot Name=\"head\">*").Which.Location;
            location.StartLine.Should().Be(4);
            location.StartColumn.Should().Be(0, "columns are 0-based");
        }

        [TestMethod]
        public void NamedSlot_InsideAnotherSlot_IsACompileErrorAtTheInnerSlot()
        {
            Action build = () => BuildIR("@model TestVM\n<Frame>\n<Slot Name=\"head\">\n"
                + "    <Slot Name=\"tail\"><i>x</i></Slot>\n</Slot>\n</Frame>");

            var location = build.Should().Throw<ConverterLocationException>()
                .WithMessage("*<Slot Name=\"tail\"> is inside <Slot Name=\"head\">*").Which.Location;
            location.StartLine.Should().Be(4);
            location.StartColumn.Should().Be(4);
        }

        // PR 148 review round 2: a stray </Slot> is already a compile error (the preprocessor's
        // control tag check), at its own line, also inside an @if.
        [TestMethod]
        public void NamedSlot_StrayClosingTag_IsACompileErrorAtTheClosingTag()
        {
            Action stray = () => BuildIR("@model TestVM\n@if (Model.Flag) {\n<Frame>\n<i>x</i>\n  </Slot>\n</Frame>\n}");
            Action extra = () => BuildIR(
                "@model TestVM\n<Frame><Slot Name=\"head\"><i>a</i></Slot></Slot></Frame>");

            var location = stray.Should().Throw<RazorSkinPreprocessorException>()
                .WithMessage("*</Slot>*expected </Frame>*").Which.Location;
            location.StartLine.Should().Be(5);
            location.StartColumn.Should().Be(2);
            extra.Should().Throw<RazorSkinPreprocessorException>().WithMessage("*</Slot>*expected </Frame>*");
        }

        [TestMethod]
        public void NamedSlot_SameNameInSiblingOrInnerControls_IsAllowed()
        {
            Action siblings = () => BuildIR("@model TestVM\n<Frame><Slot Name=\"head\"><i>a</i></Slot></Frame>"
                + "<Frame><Slot Name=\"head\"><i>b</i></Slot></Frame>");
            Action inner = () => BuildIR("@model TestVM\n<Frame><Slot Name=\"head\">"
                + "<Frame><Slot Name=\"head\"><i>x</i></Slot></Frame></Slot></Frame>");
            Action reopened = () => BuildIR("@model TestVM\n<Frame><Slot Name=\"head\"><i>a</i></Slot>"
                + "<Slot Name=\"tail\"><i>b</i></Slot></Frame>");

            siblings.Should().NotThrow();
            inner.Should().NotThrow();
            reopened.Should().NotThrow();
        }

        private static void NumberSubControls(List<IRNode> nodes)
        {
            int next = 0;
            foreach (var sub in nodes.OfType<SubControlNode>())
                sub.RuntimeMarkerIdx = next++;
        }

        private static SkinTemplateNode BuildIR(string template)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);
            return TemplateIRBuilder.Build("TestSkin", preprocessed, parsed);
        }
    }
}
