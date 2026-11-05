using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Compiler follow-ups to the AWB Razor work: the reserved <c>Slot</c> tag, host attributes
    /// on a control tag, and a bound <c>Skin</c>. Behaviour is covered by the UI QUnit suite
    /// (RazorCompilerFollowupTests); these pin the compile-time contracts.
    /// </summary>
    [TestClass]
    public class RazorCompilerFollowupTests
    {
        [TestMethod]
        public void SlotTag_WithAUserControlNamedSlot_IsALocatedCompileError()
        {
            var ir = BuildIR("@model TestVM\n<Frame>\n<Slot Name=\"head\"><i>x</i></Slot></Frame>");

            Action check = () => RazorSkinJSTGenerator.ValidateReservedSlotTag(ir.Children, CreateTypes(withSlotControl: true));

            var error = check.Should().Throw<RazorSubControlDiagnosticException>()
                .WithMessage("*Slot*reserved for content projection*Tests.Slot*").Which;
            error.Location.Should().NotBeNull();
            error.Location.FileName.Should().Be("TestSkin");
        }

        [TestMethod]
        public void SlotTag_WithoutAControlNamedSlot_IsAccepted()
        {
            var ir = BuildIR("@model TestVM\n<Frame><Slot Name=\"head\"><i>x</i></Slot></Frame>");

            Action check = () => RazorSkinJSTGenerator.ValidateReservedSlotTag(ir.Children, CreateTypes(withSlotControl: false));

            check.Should().NotThrow();
        }

        [TestMethod]
        public void ControlNamedSlot_NotUsedAsATag_IsAccepted()
        {
            var ir = BuildIR("@model TestVM\n<Frame><i>x</i></Frame>");

            Action check = () => RazorSkinJSTGenerator.ValidateReservedSlotTag(ir.Children, CreateTypes(withSlotControl: true));

            check.Should().NotThrow();
        }

        [DataTestMethod]
        [DataRow("@if (Model.Open) { <Frame><Slot Name=\"head\"><i>x</i></Slot></Frame> }", DisplayName = "inside @if")]
        [DataRow("@if (Model.Open) { <b>-</b> } else { <Frame><Slot Name=\"head\"><i>x</i></Slot></Frame> }", DisplayName = "inside @else")]
        [DataRow("@foreach (var row in Model.Rows) { <Frame><Slot Name=\"head\"><i>x</i></Slot></Frame> }", DisplayName = "inside @foreach")]
        public void SlotTag_NestedInIfOrForeach_WithAUserControlNamedSlot_IsALocatedCompileError(string body)
        {
            // The caller's control tag, and so its <Slot>, sits in an @if, @else or @foreach body.
            var ir = BuildIR("@model TestVM\n<div>" + body + "</div>");

            Action check = () => RazorSkinJSTGenerator.ValidateReservedSlotTag(ir.Children, CreateTypes(withSlotControl: true));

            check.Should().Throw<RazorSubControlDiagnosticException>()
                .WithMessage("*Slot*reserved for content projection*Tests.Slot*")
                .Which.Location.Should().NotBeNull();
        }

        [DataTestMethod]
        [DataRow("Id")]
        [DataRow("ID")]
        public void IdNotLowercase_OnAControlTag_IsACSharpProperty(string name)
        {
            var ir = BuildIR("@model TestVM\n<div><Probe " + name + "=\"@Model.Name\" /></div>");
            var sub = ir.Children.OfType<SubControlNode>().Single();

            sub.ElementId.Should().BeNull();
            sub.HostAttributes.Should().BeEmpty("only lowercase id is the host element's id");
            sub.PropertyBindings.Select(binding => (binding.PropertyName, binding.IsHostAttribute))
                .Should().Equal((name, false));
        }

        [TestMethod]
        public void HostAttributes_OnAControlTag_AreNotProperties()
        {
            var ir = BuildIR("@model TestVM\n<div><Probe data-id=\"d\" id=\"p1\" aria-label=\"L\" data-x=\"@Model.Name\" Text=\"t\" /></div>");
            var sub = ir.Children.OfType<SubControlNode>().Single();

            sub.ElementId.Should().Be("p1", "data-id is not the part id");
            sub.HostAttributes.Should().Equal(
                new KeyValuePair<string, string>("data-id", "d"),
                new KeyValuePair<string, string>("id", "p1"),
                new KeyValuePair<string, string>("aria-label", "L"));
            sub.PropertyBindings.Select(binding => (binding.PropertyName, binding.IsHostAttribute))
                .Should().Equal(("data-x", true), ("Text", false));

            sub.RuntimeMarkerIdx = 0;
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(
                ir.Children, RazorSkinCodeGenerator.CollectEventsPublic(ir.Children), new List<List<int>>());
            html.Should().Contain("<div data-id=\"d\" id=\"p1\" aria-label=\"L\" data-ns-subctl=\"0\"></div>");
        }

        [TestMethod]
        public void BoundId_OnAControlTag_IsAHostAttributeNotAPartId()
        {
            var ir = BuildIR("@model TestVM\n<div><Probe id=\"@Model.Name\" /></div>");
            var sub = ir.Children.OfType<SubControlNode>().Single();

            sub.ElementId.Should().BeNull();
            sub.PropertyBindings.Should().ContainSingle(binding => binding.PropertyName == "id" && binding.IsHostAttribute);
        }

        // Skin is declared in Sunlight.Framework.UI: a property typed Skin there and one typed Skin
        // in another assembly reach Cecil with different scopes (module vs assembly reference).
        [TestMethod]
        public void SkinBinding_AcrossAssemblies_IsAssignable()
        {
            var ui = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("Sunlight.Framework.UI", new Version(1, 0, 0, 0)),
                "Sunlight.Framework.UI.dll", ModuleKind.Dll).MainModule;
            var skin = new TypeDefinition("Sunlight.Framework.UI", "Skin", TypeAttributes.Public);
            ui.Types.Add(skin);
            var model = new TypeDefinition("Tests", "SkinModel", TypeAttributes.Public);
            ui.Types.Add(model);
            AddProperty(model, "FrameSkin", skin);

            var app = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("App", new Version(1, 0, 0, 0)), "App.dll", ModuleKind.Dll).MainModule;
            var uiElement = new TypeDefinition("Sunlight.Framework.UI", "UIElement", TypeAttributes.Public);
            app.Types.Add(uiElement);
            var element = new TypeDefinition("System.Web.Html", "Element", TypeAttributes.Public);
            app.Types.Add(element);
            var control = new TypeDefinition("Tests", "Frame", TypeAttributes.Public, uiElement);
            app.Types.Add(control);
            var ctor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName, app.TypeSystem.Void);
            ctor.Parameters.Add(new ParameterDefinition(element));
            control.Methods.Add(ctor);
            AddProperty(control, "Skin", new TypeReference("Sunlight.Framework.UI", "Skin", app,
                new AssemblyNameReference("Sunlight.Framework.UI", new Version(1, 0, 0, 0))));

            var sub = new SubControlNode { TypeName = "Frame" };
            sub.PropertyBindings.Add(new SubControlPropertyBinding
            {
                PropertyName = "Skin",
                Classification = new BindingClassification { CSharpExpression = "Model.FrameSkin" }
            });

            Action validate = () => RazorSkinJSTGenerator.ValidateSubControlTagInfo(sub, control,
                (type, name) => type.Properties.FirstOrDefault(p => p.Name == name), model);

            validate.Should().NotThrow();
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "gated consumer first")]
        [DataRow(false, DisplayName = "ungated consumer first")]
        public void SharedPropertyNode_WithAConsumerOutsideTheIf_IsUngated(bool gatedFirst)
        {
            // @if (Model.Flag) { <u>@Model.Name</u> } <b>@Model.Name</b>
            var (gated, ungated) = GateAndOutsideReaders("Model", gatedFirst);

            AssertSharedNodeUngated(GraphTopologyBuilder.Build(TopologyTestHelpers.MakeTemplate(gated, ungated)));
        }

        [DataTestMethod]
        [DataRow(true, DisplayName = "gated consumer first")]
        [DataRow(false, DisplayName = "ungated consumer first")]
        public void SharedPropertyNode_InALoopRow_WithAConsumerOutsideTheIf_IsUngated(bool gatedFirst)
        {
            // @foreach (var i in Model.Items) { @if (i.Flag) { <u>@i.Name</u> } <b>@i.Name</b> }
            var (gated, ungated) = GateAndOutsideReaders("i", gatedFirst);
            var loop = TopologyTestHelpers.MakeLoop("Model.Items", "i", new List<IRNode> { gated, ungated });

            AssertSharedNodeUngated(GraphTopologyBuilder.Build(TopologyTestHelpers.MakeTemplate(loop))
                .Collections.Single().ItemTopology);
        }

        [TestMethod]
        public void SharedPropertyNode_WithEveryConsumerInsideTheIf_StaysGated()
        {
            var cond = TopologyTestHelpers.MakeConditional("Model.Flag", "Flag", new List<IRNode>
            {
                TopologyTestHelpers.MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"),
                TopologyTestHelpers.MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.CssClass, "e1", "Name")
            });

            var topology = GraphTopologyBuilder.Build(TopologyTestHelpers.MakeTemplate(cond));

            int gate = Array.IndexOf(topology.NodeTypes, GraphNodeTypeConstants.Gate);
            int shared = SharedValueNode(topology);
            topology.GateIndices[shared].Should().Be(gate);
        }

        [DataTestMethod]
        [DataRow("@if (Model.Editing) { <Probe Value=\"@Model.Name\" /> } else { <span>@Model.Name</span> }",
            DisplayName = "control in @if, text in @else")]
        [DataRow("<Probe Value=\"@Model.Name\" />@if (Model.Editing) { <span>@Model.Name</span> }",
            DisplayName = "control outside, text in @if")]
        [DataRow("@if (Model.Editing) { <span>@Model.Name</span> }<Probe Skin=\"@Model.Name\" />",
            DisplayName = "bound Skin outside, text in @if")]
        public void SharedPropertyNode_ReadByASubControl_CountsThatReadersGate(string body)
        {
            // A sub-control binding reads the Property node directly (no consumer edge).
            var topology = GraphTopologyBuilder.Build(BuildIR("@model TestVM\n<div>" + body + "</div>"));

            int controlRead = topology.SubControls.Single().PropertyBindings.Single().NodeIdx;
            int textRead = topology.ParentIndices[topology.DomTargets.Single().NodeIdx].Single();
            textRead.Should().Be(controlRead, "both read one Model.Name Property node");
            topology.GateIndices[controlRead].Should().Be(-1, "the control must get Name in whichever branch it sits");
        }

        /// <summary>Returns the two template pieces; the gated one is wrapped in its @if.</summary>
        private static (IRNode First, IRNode Second) GateAndOutsideReaders(string root, bool gatedFirst)
        {
            IRNode gated = TopologyTestHelpers.MakeConditional(root + ".Flag", "Flag", new List<IRNode>
            {
                TopologyTestHelpers.MakeBinding(root + ".Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name")
            });
            IRNode outside = TopologyTestHelpers.MakeBinding(root + ".Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e1", "Name");
            return gatedFirst ? (gated, outside) : (outside, gated);
        }

        private static int SharedValueNode(GraphTopology topology)
        {
            topology.DomTargets.Should().HaveCount(2);
            var producers = topology.DomTargets
                .Select(target => topology.ParentIndices[target.NodeIdx].Single()).Distinct().ToList();
            producers.Should().ContainSingle("both reads of Name share one Property node");
            return producers[0];
        }

        private static void AssertSharedNodeUngated(GraphTopology topology)
        {
            int gate = Array.IndexOf(topology.NodeTypes, GraphNodeTypeConstants.Gate);
            gate.Should().BeGreaterThan(0);
            int shared = SharedValueNode(topology);

            topology.GateIndices[shared].Should().Be(-1, "the <b> outside the @if must get Name while the gate is closed");
            topology.DomTargets.Select(target => topology.GateIndices[target.NodeIdx])
                .Should().BeEquivalentTo(new[] { gate, -1 }, "each DOM target keeps its own gate");
        }

        private static void AddProperty(TypeDefinition type, string name, TypeReference propertyType)
        {
            var getter = new MethodDefinition("get_" + name,
                MethodAttributes.Public | MethodAttributes.SpecialName, propertyType);
            var setter = new MethodDefinition("set_" + name,
                MethodAttributes.Public | MethodAttributes.SpecialName, type.Module.TypeSystem.Void);
            setter.Parameters.Add(new ParameterDefinition(propertyType));
            type.Methods.Add(getter);
            type.Methods.Add(setter);
            type.Properties.Add(new PropertyDefinition(name, PropertyAttributes.None, propertyType)
            {
                GetMethod = getter,
                SetMethod = setter
            });
        }

        private static TypeDefinition[] CreateTypes(bool withSlotControl)
        {
            var module = ModuleDefinition.CreateModule("ReservedSlot", ModuleKind.Dll);
            var ui = new TypeDefinition("Sunlight.Framework.UI", "UIElement", TypeAttributes.Public);
            module.Types.Add(ui);
            // A non-control type named Slot (e.g. a calendar slot) is not a conflict.
            var data = new TypeDefinition("Tests.Data", "Slot", TypeAttributes.Public, module.TypeSystem.Object);
            module.Types.Add(data);
            if (!withSlotControl)
                return new[] { ui, data };

            var control = new TypeDefinition("Tests", "Slot", TypeAttributes.Public, ui);
            module.Types.Add(control);
            return new[] { ui, data, control };
        }

        private static SkinTemplateNode BuildIR(string template)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);
            return TemplateIRBuilder.Build("TestSkin", preprocessed, parsed);
        }
    }
}
