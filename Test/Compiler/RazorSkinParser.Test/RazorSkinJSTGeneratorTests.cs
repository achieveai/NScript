using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;
using System.Linq;
using static RazorSkinParser.Test.TopologyTestHelpers;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Tests for RazorSkinJSTGenerator.
    ///
    /// The core Generate() method requires ClrContext, RuntimeScopeManager, and resolved
    /// identifiers which are too heavy for unit tests. The static helper methods
    /// FindNthEmptySpanPath and FindNthInteractiveElementPath are private.
    ///
    /// These tests validate the generator's behavior indirectly through the graph topology
    /// data structures that feed into Generate(), ensuring the input contract is met.
    ///
    /// TODO: If FindNthEmptySpanPath and FindNthInteractiveElementPath are made internal
    /// (with InternalsVisibleTo for the test assembly), add direct tests for:
    /// - Empty span path computation in nested HTML
    /// - Interactive element (button, a, input) path computation
    /// - Edge cases: no empty spans, deeply nested elements
    /// </summary>
    [TestClass]
    public class RazorSkinJSTGeneratorTests
    {
        // --- Graph topology validation tests ---
        // These test the data structures that feed into RazorSkinJSTGenerator.Generate()

        [TestMethod]
        public void SimpleTemplate_ProducesValidTopologyForGenerator()
        {
            // The generator consumes a GraphTopology built from IR.
            // Validate that a simple binding produces the expected topology shape.
            var template = MakeTemplate(
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"));

            var topology = GraphTopologyBuilder.Build(template);

            // Validate topology is well-formed for generator consumption
            topology.NodeCount.Should().BeGreaterOrEqualTo(3);
            topology.NodeTypes.Should().NotBeNull();
            topology.GetterExpressions.Should().NotBeNull();
            topology.Consumers.Should().NotBeNull();
            topology.GateIndices.Should().NotBeNull();
            topology.DefaultValues.Should().NotBeNull();
            topology.ParentIndices.Should().NotBeNull();
            topology.DomTargets.Should().HaveCount(1);
            topology.Subscriptions.Should().HaveCount(1);
        }

        [TestMethod]
        public void EventTopology_ProducesValidInputForGenerator()
        {
            var template = MakeTemplate(MakeEvent("click", "Model.OnSubmit"));

            var topology = GraphTopologyBuilder.Build(template);

            topology.Events.Should().HaveCount(1);
            topology.Events[0].EventName.Should().Be("click");
            topology.Events[0].HandlerExpression.Should().Be("Model.OnSubmit");
            topology.Events[0].NodeIdx.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void GateTopology_ProducesValidInputForGenerator()
        {
            var template = MakeTemplate(
                MakeConditional("Model.IsVisible", "IsVisible",
                    new List<IRNode> { new HtmlNode { HtmlContent = "<div>Visible</div>" } }));

            var topology = GraphTopologyBuilder.Build(template);

            topology.Gates.Should().HaveCount(1);
            topology.Gates[0].NodeIdx.Should().BeGreaterThan(0);
            topology.Gates[0].IrNode.Should().NotBeNull();
        }

        [TestMethod]
        public void CollectionTopology_ProducesItemTopologyForGenerator()
        {
            var template = MakeTemplate(
                MakeLoop("Model.Items", "item",
                    new List<IRNode>
                    {
                        MakeBinding("item.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name")
                    },
                    isObservable: false));

            var topology = GraphTopologyBuilder.Build(template);

            topology.Collections.Should().HaveCount(1);
            topology.Collections[0].ItemTopology.Should().NotBeNull();
            topology.Collections[0].ItemTopology.ItemVariablePrefix.Should().Be("item.");
            topology.Collections[0].IrNode.Should().NotBeNull();
        }

        [TestMethod]
        public void ParentIndices_AreInverseOfConsumers()
        {
            var template = MakeTemplate(
                MakeBinding("Model.Name", BindingMode.OneWay, ExpressionTarget.TextContent, "e0", "Name"));

            var topology = GraphTopologyBuilder.Build(template);

            // For every edge from->to in Consumers, ParentIndices[to] should contain from
            for (int from = 0; from < topology.NodeCount; from++)
            {
                foreach (int to in topology.Consumers[from])
                {
                    topology.ParentIndices[to].Should().Contain(from,
                        $"ParentIndices[{to}] should contain {from} since Consumers[{from}] contains {to}");
                }
            }
        }
    }
}
