using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;
using System.Linq;
using static RazorSkinParser.Test.TopologyTestHelpers;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class GraphTopologyBuilderTests
    {
        [TestMethod]
        public void SimpleTextBinding_ProducesSourcePropertyDomTarget()
        {
            // IR: @Model.Name as text content
            var template = MakeTemplate(
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"));

            var topology = GraphTopologyBuilder.Build(template);

            // Expected: Source(0) -> Property(1) -> DomTarget(2)
            topology.NodeCount.Should().Be(3);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.DomTarget);

            // Property node getter
            topology.GetterExpressions[1].Should().Be("Name");

            // Adjacency: Source -> Property, Property -> DomTarget
            topology.Consumers[0].Should().Contain(1);
            topology.Consumers[1].Should().Contain(2);

            // 1 subscription for "Name"
            topology.Subscriptions.Should().HaveCount(1);
            topology.Subscriptions[0].PropertyName.Should().Be("Name");
            topology.Subscriptions[0].NodeIdx.Should().Be(1);

            // 1 DOM target
            topology.DomTargets.Should().HaveCount(1);
            topology.DomTargets[0].NodeIdx.Should().Be(2);
            topology.DomTargets[0].Target.Should().Be(ExpressionTarget.TextContent);

            topology.ModelTypeName.Should().Be("TestModel");
        }

        [TestMethod]
        public void TwoBindings_ShareSourceAndDeduplicateProperty()
        {
            // IR: @Model.Name appears twice
            var template = MakeTemplate(
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"),
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e1", "Name"));

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property(1) -> DomTarget(2), DomTarget(3)
            topology.NodeCount.Should().Be(4);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.DomTarget);
            topology.NodeTypes[3].Should().Be(GraphNodeTypeConstants.DomTarget);

            // Property node fans out to 2 DomTargets
            topology.Consumers[1].Should().HaveCount(2);
            topology.Consumers[1].Should().Contain(2);
            topology.Consumers[1].Should().Contain(3);

            // Still only 1 subscription (property is deduplicated)
            topology.Subscriptions.Should().HaveCount(1);

            // 2 DOM targets
            topology.DomTargets.Should().HaveCount(2);
        }

        [TestMethod]
        public void TwoDistinctProperties_CreateSeparatePropertyNodes()
        {
            // IR: @Model.Name and @Model.Count
            var template = MakeTemplate(
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"),
                MakeBinding("Model.Count", BindingMode.OneWay, ExpressionTarget.TextContent, "e1", "Count"));

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property_Name(1) -> DomTarget(2)
            //           -> Property_Count(3) -> DomTarget(4)
            topology.NodeCount.Should().Be(5);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.DomTarget);
            topology.NodeTypes[3].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[4].Should().Be(GraphNodeTypeConstants.DomTarget);

            // Source fans out to 2 property nodes
            topology.Consumers[0].Should().HaveCount(2);
            topology.Consumers[0].Should().Contain(1);
            topology.Consumers[0].Should().Contain(3);

            // 2 subscriptions
            topology.Subscriptions.Should().HaveCount(2);
            topology.Subscriptions.Select(s => s.PropertyName)
                .Should().BeEquivalentTo(new[] { "Name", "Count" });
        }

        [TestMethod]
        public void OneTimeBinding_HasNoSubscription()
        {
            // IR: @Model.AppVersion with Mode=OneTime
            var template = MakeTemplate(
                MakeBinding("Model.AppVersion", BindingMode.OneTime, ExpressionTarget.TextContent, "e0", "AppVersion"));

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property(1) -> DomTarget(2)
            topology.NodeCount.Should().Be(3);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);

            // 0 subscriptions (OneTime doesn't live-bind)
            topology.Subscriptions.Should().HaveCount(0);
        }

        [TestMethod]
        public void ComputedExpression_CreatesSeparateComputedNode()
        {
            // IR: @(Model.Price * Model.Quantity) with deps [Price, Quantity]
            var template = MakeTemplate(
                new ExpressionBindingNode
                {
                    Target = ExpressionTarget.TextContent,
                    ElementId = "e0",
                    Classification = new BindingClassification
                    {
                        CSharpExpression = "Model.Price * Model.Quantity",
                        Mode = BindingMode.OneWay,
                        SourceKind = BindingSourceKind.DataContext,
                        Dependencies = new List<ObservableDependency>
                        {
                            new ObservableDependency(BindingSourceKind.DataContext, "Price", "Price"),
                            new ObservableDependency(BindingSourceKind.DataContext, "Quantity", "Quantity")
                        }
                    }
                });

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property_Price(1)    -> Computed(3) -> DomTarget(4)
            //           -> Property_Quantity(2) -> Computed(3)
            topology.NodeCount.Should().Be(5);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[3].Should().Be(GraphNodeTypeConstants.Computed);
            topology.NodeTypes[4].Should().Be(GraphNodeTypeConstants.DomTarget);

            // Both property nodes feed into Computed
            topology.Consumers[1].Should().Contain(3);
            topology.Consumers[2].Should().Contain(3);

            // Computed feeds into DomTarget
            topology.Consumers[3].Should().Contain(4);

            // Computed node has the full expression
            topology.GetterExpressions[3].Should().Be("Model.Price * Model.Quantity");

            // 2 subscriptions for Price and Quantity
            topology.Subscriptions.Should().HaveCount(2);
        }

        [TestMethod]
        public void EventBinding_CreatesEventTopology()
        {
            var template = MakeTemplate(MakeEvent("click", "Model.OnSubmit"));

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> EventBinding(1)
            topology.NodeCount.Should().Be(2);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.EventBinding);

            topology.Events.Should().HaveCount(1);
            topology.Events[0].EventName.Should().Be("click");
            topology.Events[0].HandlerExpression.Should().Be("Model.OnSubmit");
        }

        [TestMethod]
        public void GateNode_ForConditionalBlock()
        {
            var template = MakeTemplate(
                MakeConditional("Model.IsVisible", "IsVisible",
                    new List<IRNode>
                    {
                        MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name")
                    }));

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property_IsVisible(1) -> Gate(2) -> Property_Name(3) -> DomTarget(4)
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.Gate);

            // Top-level gate is ungated (-1) — it always evaluates.
            // Only nested gates have their parent's gateIndex set.
            topology.GateIndices[2].Should().Be(-1);

            // Child nodes have gateIndices pointing to the Gate
            topology.GateIndices[3].Should().Be(2);
            topology.GateIndices[4].Should().Be(2);

            // Gate topology entry
            topology.Gates.Should().HaveCount(1);
            topology.Gates[0].NodeIdx.Should().Be(2);

            // Default value for gate is false
            topology.DefaultValues[2].Should().Be(false);
        }

        [TestMethod]
        public void DefaultValues_MatchTargetType()
        {
            var template = MakeTemplate(
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"));

            var topology = GraphTopologyBuilder.Build(template);

            // Source default is null, Property default is null, DomTarget default is ""
            topology.DefaultValues[0].Should().BeNull();   // Source
            topology.DefaultValues[1].Should().BeNull();   // Property
            topology.DefaultValues[2].Should().Be("");     // DomTarget (TextContent)
        }

        [TestMethod]
        public void SourceNode_AlwaysAtIndexZero()
        {
            var template = MakeTemplate();

            var topology = GraphTopologyBuilder.Build(template);

            topology.NodeCount.Should().BeGreaterOrEqualTo(1);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.RootSourceSlot.Should().Be(0);
        }

        // ------------------------------------------------------------------
        // SubControlNode handling
        // ------------------------------------------------------------------

        [TestMethod]
        public void SubControlNode_IsIgnoredInTopology()
        {
            // SubControlNode is present in IR but GraphTopologyBuilder skips it
            // (stub break; — no graph nodes created for sub-controls yet).
            var template = MakeTemplate(
                new SubControlNode
                {
                    TypeName = "ListView",
                    ResolvedTypeName = "Sunlight.Framework.UI.ListView",
                    ElementId = "myList"
                },
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"));

            var topology = GraphTopologyBuilder.Build(template);

            // SubControlNode should NOT add nodes — only the binding does
            // Source(0) -> Property(1) -> DomTarget(2)
            topology.NodeCount.Should().Be(3);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.DomTarget);
        }

        [TestMethod]
        public void SubControlNode_WithBindings_OnlyProcessesExpressionBindings()
        {
            // SubControlNode with property bindings — the sub-control itself is skipped,
            // but sibling expression bindings are still processed.
            var template = MakeTemplate(
                new SubControlNode
                {
                    TypeName = "SearchBox",
                    ResolvedTypeName = "App.SearchBox",
                    ElementId = "search",
                    PropertyBindings = new List<SubControlPropertyBinding>
                    {
                        new SubControlPropertyBinding
                        {
                            PropertyName = "Query",
                            Classification = new BindingClassification
                            {
                                CSharpExpression = "Model.SearchQuery",
                                Mode = BindingMode.OneWay,
                                SourceKind = BindingSourceKind.DataContext
                            }
                        }
                    }
                },
                MakeBinding("Model.Title", BindingMode.OneWay, ExpressionTarget.TextContent, "e1", "Title"));

            var topology = GraphTopologyBuilder.Build(template);

            // Only the expression binding is processed, not the sub-control
            topology.NodeCount.Should().Be(3);
            topology.Subscriptions.Should().HaveCount(1);
            topology.Subscriptions[0].PropertyName.Should().Be("Title");
        }

        // ------------------------------------------------------------------
        // Static-only template
        // ------------------------------------------------------------------

        [TestMethod]
        public void StaticHtmlOnly_ProducesMinimalTopology()
        {
            // Template with only static HTML (no bindings, no events)
            var template = MakeTemplate(new HtmlNode { HtmlContent = "<div>Hello World</div>" });

            var topology = GraphTopologyBuilder.Build(template);

            // Only Source node exists (index 0)
            topology.NodeCount.Should().Be(1);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.Subscriptions.Should().HaveCount(0);
            topology.DomTargets.Should().HaveCount(0);
            topology.Events.Should().HaveCount(0);
            topology.Gates.Should().HaveCount(0);
        }

        // ------------------------------------------------------------------
        // Nested conditional gates
        // ------------------------------------------------------------------

        [TestMethod]
        public void NestedConditionals_CreateChainedGates()
        {
            // @if(Model.IsVisible) { @if(Model.ShowDetails) { @Model.Name } }
            var template = MakeTemplate(
                MakeConditional("Model.IsVisible", "IsVisible",
                    new List<IRNode>
                    {
                        MakeConditional("Model.ShowDetails", "ShowDetails",
                            new List<IRNode>
                            {
                                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name")
                            })
                    }));

            var topology = GraphTopologyBuilder.Build(template);

            // Should have 2 gates
            topology.Gates.Should().HaveCount(2);

            // Find gate node indices
            var gateIndices = topology.NodeTypes
                .Select((t, i) => new { Type = t, Index = i })
                .Where(x => x.Type == GraphNodeTypeConstants.Gate)
                .Select(x => x.Index)
                .ToList();

            gateIndices.Should().HaveCount(2);

            int outerGate = gateIndices[0];
            int innerGate = gateIndices[1];

            // Outer gate is ungated (top-level, -1)
            topology.GateIndices[outerGate].Should().Be(-1);

            // Inner gate is gated by the outer gate
            topology.GateIndices[innerGate].Should().Be(outerGate);
        }

        // ------------------------------------------------------------------
        // Loop (CollectionManager) topology
        // ------------------------------------------------------------------

        [TestMethod]
        public void LoopNode_CreatesCollectionManagerTopology()
        {
            // @foreach(var item in Model.Items) { @item.Name }
            var template = MakeTemplate(
                MakeLoop("Model.Items", "item",
                    new List<IRNode>
                    {
                        MakeBinding("item.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name")
                    }));

            var topology = GraphTopologyBuilder.Build(template);

            // Should contain a CollectionManager node
            var collManagerIndices = topology.NodeTypes
                .Select((t, i) => new { Type = t, Index = i })
                .Where(x => x.Type == GraphNodeTypeConstants.CollectionManager)
                .Select(x => x.Index)
                .ToList();

            collManagerIndices.Should().HaveCount(1);

            // CollectionManager should be connected from Source
            int cmIdx = collManagerIndices[0];
            topology.Consumers[0].Should().Contain(cmIdx);
        }

        // ------------------------------------------------------------------
        // Attribute binding targets
        // ------------------------------------------------------------------

        [TestMethod]
        public void AttributeBinding_ProducesCorrectDomTarget()
        {
            var template = MakeTemplate(
                MakeBinding("Model.Title", BindingMode.OneWay, ExpressionTarget.Attribute, "e0", "Title"));

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property(1) -> DomTarget(2)
            topology.DomTargets.Should().HaveCount(1);
            topology.DomTargets[0].Target.Should().Be(ExpressionTarget.Attribute);
        }

        [TestMethod]
        public void CssClassBinding_ProducesCorrectDomTarget()
        {
            var template = MakeTemplate(
                MakeBinding("Model.CssClass", BindingMode.OneWay, ExpressionTarget.CssClass, "e0", "CssClass"));

            var topology = GraphTopologyBuilder.Build(template);

            topology.DomTargets.Should().HaveCount(1);
            topology.DomTargets[0].Target.Should().Be(ExpressionTarget.CssClass);
        }

        [TestMethod]
        public void StyleBinding_ProducesCorrectDomTarget()
        {
            var template = MakeTemplate(
                MakeBinding("Model.DisplayStyle", BindingMode.OneWay, ExpressionTarget.Style, "e0", "DisplayStyle"));

            var topology = GraphTopologyBuilder.Build(template);

            topology.DomTargets.Should().HaveCount(1);
            topology.DomTargets[0].Target.Should().Be(ExpressionTarget.Style);
        }

        // ------------------------------------------------------------------
        // Mixed template (binding + event + conditional)
        // ------------------------------------------------------------------

        [TestMethod]
        public void MixedTemplate_BindingEventConditional_AllProcessed()
        {
            var template = MakeTemplate(
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"),
                MakeEvent("click", "Model.OnClick"),
                MakeConditional("Model.IsActive", "IsActive",
                    new List<IRNode> { new HtmlNode { HtmlContent = "<span>Active</span>" } }));

            var topology = GraphTopologyBuilder.Build(template);

            // Should have all three: DomTarget, EventBinding, Gate
            topology.DomTargets.Should().HaveCount(1);
            topology.Events.Should().HaveCount(1);
            topology.Gates.Should().HaveCount(1);

            // Subscriptions: "Name" and "IsActive"
            topology.Subscriptions.Should().HaveCount(2);
            topology.Subscriptions.Select(s => s.PropertyName)
                .Should().BeEquivalentTo(new[] { "Name", "IsActive" });
        }

        [TestMethod]
        public void NegatedGateCondition_ProducesNegatedPropertyGetter()
        {
            // Arrange — template with @if (!Model.IsEnabled)
            var template = MakeTemplate(
                MakeConditional("!Model.IsEnabled", "IsEnabled",
                    new List<IRNode>
                    {
                        MakeBinding("Model.Label", BindingMode.OneWay,
                            ExpressionTarget.TextContent, "e0", "Label")
                    }));

            var topology = GraphTopologyBuilder.Build(template);

            // Assert — a negated gate should produce a Property node with "!" prefix getter
            topology.Gates.Should().HaveCount(1);
            // The negated condition creates a Property node with "!IsEnabled" getter
            topology.GetterExpressions.Should().Contain("!IsEnabled");
        }

        [TestMethod]
        public void ValueAttributeBinding_ProducesDomTargetWithValueAttributeName()
        {
            // Arrange — template with <input value="@Model.Name" />
            var binding = MakeBinding("Model.Name", BindingMode.OneWay,
                ExpressionTarget.Attribute, "e0", "Name");
            binding.AttributeName = "value";
            var template = MakeTemplate(binding);

            var topology = GraphTopologyBuilder.Build(template);

            // Assert — DomTarget should have AttributeName = "value" and Target = Attribute
            topology.DomTargets.Should().HaveCount(1);
            var domTarget = topology.DomTargets[0];
            domTarget.AttributeName.Should().Be("value");
            domTarget.Target.Should().Be(ExpressionTarget.Attribute);
        }
    }
}
