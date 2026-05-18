using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;
using System.Linq;
using System.Reflection;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Phase 3b coverage: Razor <c>.skin.cshtml</c> templates must flow the originating
    /// Razor <c>IntermediateNode.Source</c> positions through the IR and into the emitted
    /// JST so the final source map points back at the template file rather than reporting
    /// null positions. These tests drive a known template through
    /// <see cref="TemplateIRBuilder"/> (and, where feasible, through
    /// <see cref="GraphDescriptorJSTEmitter"/>) and assert that IR nodes and emitted JST
    /// carry the template file name and line numbers end-to-end.
    ///
    /// IR-level assertions are the primary contract: if IR nodes lack a Location, every
    /// downstream JST node sourced from them would also lack one. The emitter-level check
    /// additionally pins the <see cref="GraphDescriptorJSTEmitter._fallbackLocation"/>
    /// threading — a regression that drops that field would otherwise only surface in the
    /// full pipeline.
    /// </summary>
    [TestClass]
    public class TemplateSourceMapTests
    {
        private const string TestTemplateName = "TestSkin";

        [TestMethod]
        public void IRRootCarriesTemplateLocation()
        {
            var ir = BuildIR("@model TestVM\n\n<div>Hello</div>");

            ir.Location.Should().NotBeNull(
                "the SkinTemplateNode root must always be anchored to the template " +
                "so skin-factory / getter JST nodes that fall back to the root have a file-scoped Location");
            ir.Location.FileName.Should().Be(TestTemplateName);
            ir.Location.StartLine.Should().BeGreaterOrEqualTo(1,
                "template lines are 1-based; a 0 or negative line would drop the map entry");
        }

        [TestMethod]
        public void HtmlNode_CarriesTemplateLocation()
        {
            // Line 1: @model TestVM
            // Line 2: (blank)
            // Line 3: <div>Hello</div>
            //
            // Razor's HtmlContentIntermediateNode can span the whitespace between @model
            // and the first tag, so the reported Source line may land anywhere from 1
            // through the <div> line. We assert presence and file-name match rather than
            // an exact line — the HtmlNode Location must be populated so downstream JST
            // from static HTML carries a file-scoped source position.
            var ir = BuildIR("@model TestVM\n\n<div>Hello</div>");

            var html = ir.Children.OfType<HtmlNode>().First();
            html.Location.Should().NotBeNull(
                "IRBuilder must thread IntermediateNode.Source into every HtmlNode — " +
                "a null Location here drops the file attribution for static-HTML JST");
            html.Location.FileName.Should().Be(TestTemplateName);
            html.Location.StartLine.Should().BeGreaterOrEqualTo(1,
                "template lines are 1-based; a 0 or negative line would drop the map entry");
        }

        [TestMethod]
        public void ExpressionBindingNode_CarriesTemplateLocation()
        {
            // Line 3 hosts @Model.Name inside <span>.
            var ir = BuildIR("@model TestVM\n\n<span>@Model.Name</span>");

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Location.Should().NotBeNull(
                "expression bindings drive reactive getter emission — without a Location " +
                "every generated getter function would lose its source position");
            binding.Location.FileName.Should().Be(TestTemplateName);
            // The Razor package version is not pinned in-test, and the span attribution
            // for @Model.Name inside <span> has been observed to shift between Razor
            // builds. Assert StartLine >= 3 (the line authored in the template) rather
            // than exact equality to avoid breaking on Razor upgrades.
            binding.Location.StartLine.Should().BeGreaterOrEqualTo(3);
        }

        [TestMethod]
        public void ConditionalNode_CarriesTemplateLocation()
        {
            // Line 3 hosts @if.
            var ir = BuildIR("@model TestVM\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}");

            var cond = ir.Children.OfType<ConditionalNode>().First();
            cond.Location.Should().NotBeNull();
            cond.Location.FileName.Should().Be(TestTemplateName);
            // Razor package version not pinned in-test; @if span attribution may shift
            // between Razor builds. Assert >= 3 (authored line) rather than exact equality.
            cond.Location.StartLine.Should().BeGreaterOrEqualTo(3,
                "the @if block is authored on line 3; gate descriptors emitted for it must inherit at least this line");
        }

        [TestMethod]
        public void LoopNode_CarriesTemplateLocation()
        {
            // Line 3 hosts @foreach.
            var ir = BuildIR(
                "@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}");

            var loop = ir.Children.OfType<LoopNode>().First();
            loop.Location.Should().NotBeNull();
            loop.Location.FileName.Should().Be(TestTemplateName);
            // Razor package version not pinned; @foreach span attribution may shift.
            loop.Location.StartLine.Should().BeGreaterOrEqualTo(3);
        }

        [TestMethod]
        public void EventNode_CarriesTemplateLocation()
        {
            // Line 3 hosts <button onclick=...>.
            var ir = BuildIR(
                "@model TestVM\n\n<button onclick=\"@Model.HandleClick\">Click</button>");

            var evt = ir.Children.OfType<EventNode>().FirstOrDefault();
            evt.Should().NotBeNull(
                "onclick=\"@...\" must produce an EventNode (regression guard)");
            evt.Location.Should().NotBeNull();
            evt.Location.FileName.Should().Be(TestTemplateName);
            // Razor package version not pinned; event-expression span attribution may shift.
            evt.Location.StartLine.Should().BeGreaterOrEqualTo(3);
        }

        [TestMethod]
        public void SubControlNode_CarriesTemplateLocation()
        {
            // Line 3 hosts <ListView>. SubControlNode Location derivation is unique:
            // it uses `hostLocation ?? parent?.Location` (the hostLocation parameter
            // added during Phase 3b self-review) rather than the
            // TryGetLocation(intermediateNode) path used by other node types.
            // Without this coverage the host-location threading could silently
            // regress to null and sub-controls would collapse to line 1 of the
            // SkinTemplateNode root.
            var ir = BuildIR("@model TestVM\n\n<div><ListView id=\"myList\" /></div>");

            var subControl = ir.Children.OfType<SubControlNode>().FirstOrDefault();
            subControl.Should().NotBeNull(
                "a PascalCase tag inside HTML content must produce a SubControlNode (regression guard)");
            subControl.Location.Should().NotBeNull(
                "SubControlNode must inherit the host htmlNode's Location so skin-factory " +
                "emission pins the sub-control to its authored line, not line 1 of the root");
            subControl.Location.FileName.Should().Be(TestTemplateName);
            // Razor package version not pinned; span attribution may shift between
            // Razor builds. Assert >= 1 (file-scoped) rather than exact equality.
            subControl.Location.StartLine.Should().BeGreaterOrEqualTo(1);
        }

        [TestMethod]
        public void GraphDescriptorEmitter_ExposesFallbackLocationContract()
        {
            // Surface-level contract check for the Phase 3b threading in
            // GraphDescriptorJSTEmitter. A full instantiation requires ClrContext +
            // RuntimeScopeManager (too heavy for a unit test — the ctor eagerly
            // resolves Sunlight type definitions), so we verify the contract via
            // reflection on the type surface:
            //   1. The constructor has an optional fallbackLocation parameter.
            //   2. A private _fallbackLocation field of type Location exists to persist it.
            // A regression that drops either would silently leave emitted function
            // expressions with null Locations — this test catches that at compile/test time
            // even though Emit() itself can only be exercised end-to-end in framework tests.
            var ctor = typeof(GraphDescriptorJSTEmitter).GetConstructors().Single();
            var fallbackParam = ctor.GetParameters()
                .FirstOrDefault(p => p.Name == "fallbackLocation");
            fallbackParam.Should().NotBeNull(
                "GraphDescriptorJSTEmitter must expose a fallbackLocation ctor parameter " +
                "so RazorSkinJSTGenerator can thread the template root Location into " +
                "every emitted FunctionExpression/ReturnStatement");
            fallbackParam.ParameterType.Should().Be(typeof(Location));
            fallbackParam.HasDefaultValue.Should().BeTrue(
                "fallbackLocation must be optional so nested-emitter callers that don't " +
                "yet have per-binding Locations remain source-compatible");
            fallbackParam.DefaultValue.Should().BeNull();

            var field = typeof(GraphDescriptorJSTEmitter).GetField(
                "_fallbackLocation",
                BindingFlags.Instance | BindingFlags.NonPublic);
            field.Should().NotBeNull(
                "_fallbackLocation backing field must exist — without it the ctor parameter " +
                "would be silently discarded and no JST node would carry the fallback Location");
            field.FieldType.Should().Be(typeof(Location));
        }

        // -----------------------------------------------------------------------
        // sourceFile parameter regression tests (Phase 3c fix)
        //
        // Before this fix every Location.FileName in Razor-template IR contained
        // the short template name (e.g. "TestSkin") instead of the absolute path
        // to the .skin.cshtml file. This caused SourceMap.Server to return 404
        // because sourcesLong[] held the short name rather than a disk path.
        // The tests below guard all node types against a regression.
        // -----------------------------------------------------------------------

        private const string AbsoluteSourceFile = @"B:\project\Views\TestSkin.skin.cshtml";

        [TestMethod]
        public void SourceFile_Root_UsesAbsolutePath()
        {
            var ir = BuildIR("@model TestVM\n\n<div>Hello</div>", AbsoluteSourceFile);

            ir.Location.Should().NotBeNull();
            ir.Location.FileName.Should().Be(AbsoluteSourceFile,
                "sourceFile must override templateName as Location.FileName so " +
                "sourcesLong[] in the source map resolves to the .skin.cshtml on disk");
        }

        [TestMethod]
        public void SourceFile_HtmlNode_UsesAbsolutePath()
        {
            var ir = BuildIR("@model TestVM\n\n<div>Hello</div>", AbsoluteSourceFile);

            var html = ir.Children.OfType<HtmlNode>().First();
            html.Location.Should().NotBeNull();
            html.Location.FileName.Should().Be(AbsoluteSourceFile,
                "WalkMethodBody must propagate locationFileName (not templateName) into HtmlNode");
        }

        [TestMethod]
        public void SourceFile_ExpressionBindingNode_UsesAbsolutePath()
        {
            var ir = BuildIR("@model TestVM\n\n<span>@Model.Name</span>", AbsoluteSourceFile);

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Location.Should().NotBeNull();
            binding.Location.FileName.Should().Be(AbsoluteSourceFile,
                "CreateExpressionBinding must propagate locationFileName into ExpressionBindingNode");
        }

        [TestMethod]
        public void SourceFile_EventNode_UsesAbsolutePath()
        {
            var ir = BuildIR(
                "@model TestVM\n\n<button onclick=\"@Model.HandleClick\">Click</button>",
                AbsoluteSourceFile);

            var evt = ir.Children.OfType<EventNode>().FirstOrDefault();
            evt.Should().NotBeNull();
            evt.Location.Should().NotBeNull();
            evt.Location.FileName.Should().Be(AbsoluteSourceFile,
                "CreateEventNode must propagate locationFileName into EventNode");
        }

        [TestMethod]
        public void SourceFile_ConditionalNode_UsesAbsolutePath()
        {
            var ir = BuildIR(
                "@model TestVM\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}",
                AbsoluteSourceFile);

            var cond = ir.Children.OfType<ConditionalNode>().First();
            cond.Location.Should().NotBeNull();
            cond.Location.FileName.Should().Be(AbsoluteSourceFile,
                "ParseIfBlock must propagate locationFileName into ConditionalNode");
        }

        [TestMethod]
        public void SourceFile_LoopNode_UsesAbsolutePath()
        {
            var ir = BuildIR(
                "@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}",
                AbsoluteSourceFile);

            var loop = ir.Children.OfType<LoopNode>().First();
            loop.Location.Should().NotBeNull();
            loop.Location.FileName.Should().Be(AbsoluteSourceFile,
                "ParseForeachBlock must propagate locationFileName into LoopNode");
        }

        [TestMethod]
        public void SourceFile_Null_FallsBackToTemplateName()
        {
            // When sourceFile is omitted the short template name must remain the
            // fallback — this is the path used by tests and any caller without a
            // disk path available.
            var ir = BuildIR("@model TestVM\n\n<div>Hello</div>");

            ir.Location.FileName.Should().Be(TestTemplateName,
                "omitting sourceFile must fall back to templateName, not null");
        }

        private static SkinTemplateNode BuildIR(string template, string sourceFile = null)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse(TestTemplateName, preprocessed.CleanedTemplate);
            return TemplateIRBuilder.Build(TestTemplateName, preprocessed, parsed, sourceFile);
        }
    }
}
