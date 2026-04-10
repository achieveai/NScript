using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Validates that compiler-side GraphNodeTypeConstants stay in sync with
    /// the framework-side GraphNodeType constants (Sunlight.Framework.UI.Helpers.BindingGraph.GraphNodeType).
    /// These values are used as array indices in the static graph descriptor and must match exactly.
    /// </summary>
    [TestClass]
    public class GraphNodeTypeValidationTests
    {
        [TestMethod]
        public void GraphNodeTypeConstants_MatchFrameworkValues()
        {
            // These must stay in sync with GraphNodeType.cs in
            // Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphNodeType.cs
            Assert.AreEqual(0, GraphNodeTypeConstants.Source);
            Assert.AreEqual(1, GraphNodeTypeConstants.Property);
            Assert.AreEqual(2, GraphNodeTypeConstants.Computed);
            Assert.AreEqual(3, GraphNodeTypeConstants.DomTarget);
            Assert.AreEqual(4, GraphNodeTypeConstants.EventBinding);
            Assert.AreEqual(5, GraphNodeTypeConstants.Gate);
            Assert.AreEqual(6, GraphNodeTypeConstants.CollectionManager);
            Assert.AreEqual(7, GraphNodeTypeConstants.TypeGuard);
        }

        [TestMethod]
        public void Source_IsAlwaysZero()
        {
            // Source must be index 0 — GraphTopologyBuilder.Build relies on this
            GraphNodeTypeConstants.Source.Should().Be(0);
        }

        [TestMethod]
        public void AllConstants_AreDistinct()
        {
            var values = new[]
            {
                GraphNodeTypeConstants.Source,
                GraphNodeTypeConstants.Property,
                GraphNodeTypeConstants.Computed,
                GraphNodeTypeConstants.DomTarget,
                GraphNodeTypeConstants.EventBinding,
                GraphNodeTypeConstants.Gate,
                GraphNodeTypeConstants.CollectionManager,
                GraphNodeTypeConstants.TypeGuard
            };

            values.Should().OnlyHaveUniqueItems();
        }

        [TestMethod]
        public void AllConstants_AreContiguousFromZero()
        {
            // The constants serve as array indices, so they must be 0..N-1
            var values = new[]
            {
                GraphNodeTypeConstants.Source,
                GraphNodeTypeConstants.Property,
                GraphNodeTypeConstants.Computed,
                GraphNodeTypeConstants.DomTarget,
                GraphNodeTypeConstants.EventBinding,
                GraphNodeTypeConstants.Gate,
                GraphNodeTypeConstants.CollectionManager,
                GraphNodeTypeConstants.TypeGuard
            };

            values.Should().BeInAscendingOrder();
            values[0].Should().Be(0);
            values[values.Length - 1].Should().Be(values.Length - 1);
        }
    }
}
