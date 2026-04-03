using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;
using System.Linq;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class GraphTopologyBuilderTests
    {
        [TestMethod]
        public void SimpleTextBinding_ProducesSourcePropertyDomTarget()
        {
            // IR: @Model.Name as text content
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new ExpressionBindingNode
                    {
                        Target = ExpressionTarget.TextContent,
                        ElementId = "e0",
                        Classification = new BindingClassification
                        {
                            CSharpExpression = "Model.Name",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "Name", "Name")
                            }
                        }
                    }
                }
            };

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
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    MakeBinding("Model.Name", BindingMode.OneWay, BindingSourceKind.DataContext,
                        ExpressionTarget.TextContent, "e0", "Name"),
                    MakeBinding("Model.Name", BindingMode.OneWay, BindingSourceKind.DataContext,
                        ExpressionTarget.TextContent, "e1", "Name")
                }
            };

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
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    MakeBinding("Model.Name", BindingMode.OneWay, BindingSourceKind.DataContext,
                        ExpressionTarget.TextContent, "e0", "Name"),
                    MakeBinding("Model.Count", BindingMode.OneWay, BindingSourceKind.DataContext,
                        ExpressionTarget.TextContent, "e1", "Count")
                }
            };

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
            topology.Subscriptions.Select(s => s.PropertyName).Should().Contain("Name");
            topology.Subscriptions.Select(s => s.PropertyName).Should().Contain("Count");
        }

        [TestMethod]
        public void OneTimeBinding_HasNoSubscription()
        {
            // IR: @Model.AppVersion with Mode=OneTime
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new ExpressionBindingNode
                    {
                        Target = ExpressionTarget.TextContent,
                        ElementId = "e0",
                        Classification = new BindingClassification
                        {
                            CSharpExpression = "Model.AppVersion",
                            Mode = BindingMode.OneTime,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>()
                        }
                    }
                }
            };

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
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
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
                    }
                }
            };

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
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new EventNode
                    {
                        DomEventName = "click",
                        HandlerExpression = "Model.OnSubmit",
                        IsLambda = false
                    }
                }
            };

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
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new ConditionalNode
                    {
                        IsReactive = true,
                        Condition = new BindingClassification
                        {
                            CSharpExpression = "Model.IsVisible",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "IsVisible", "IsVisible")
                            }
                        },
                        TrueBranch = new List<IRNode>
                        {
                            MakeBinding("Model.Name", BindingMode.OneWay, BindingSourceKind.DataContext,
                                ExpressionTarget.TextContent, "e0", "Name")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property_IsVisible(1) -> Gate(2) -> Property_Name(3) -> DomTarget(4)
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.Gate);

            // Gate node's gateIndex is itself
            topology.GateIndices[2].Should().Be(2);

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
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    MakeBinding("Model.Name", BindingMode.OneWay, BindingSourceKind.DataContext,
                        ExpressionTarget.TextContent, "e0", "Name")
                }
            };

            var topology = GraphTopologyBuilder.Build(template);

            // Source default is null, Property default is null, DomTarget default is ""
            topology.DefaultValues[0].Should().BeNull();   // Source
            topology.DefaultValues[1].Should().BeNull();   // Property
            topology.DefaultValues[2].Should().Be("");     // DomTarget (TextContent)
        }

        [TestMethod]
        public void SourceNode_AlwaysAtIndexZero()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel"
            };

            var topology = GraphTopologyBuilder.Build(template);

            topology.NodeCount.Should().BeGreaterOrEqualTo(1);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.RootSourceSlot.Should().Be(0);
        }

        /// <summary>
        /// Verifies that an attribute binding with AttributeName="value" creates a
        /// DomTargetTopology with the correct target and attribute name.
        /// The emitter uses this data to generate e.value (DOM property) instead of
        /// setAttribute("value"...) — ensuring input values update correctly after
        /// user interaction (H2 review finding).
        /// </summary>
        [TestMethod]
        public void ValueAttributeBinding_ProducesDomTargetWithValueAttributeName()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new ExpressionBindingNode
                    {
                        Target = ExpressionTarget.Attribute,
                        AttributeName = "value",
                        ElementId = "e0",
                        Classification = new BindingClassification
                        {
                            CSharpExpression = "Model.SomeProperty",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "SomeProperty", "SomeProperty")
                            }
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);

            // Should have Source(0) -> Property(1) -> DomTarget(2)
            topology.NodeCount.Should().Be(3);
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.DomTarget);

            // DomTarget should carry "value" attribute name so the emitter
            // generates e.value (DOM property) rather than setAttribute("value"...)
            topology.DomTargets.Should().HaveCount(1);
            topology.DomTargets[0].AttributeName.Should().Be("value",
                "value attribute must use DOM property setter, not setAttribute");
            topology.DomTargets[0].Target.Should().Be(ExpressionTarget.Attribute);
        }

        // --- Helper ---

        /// <summary>
        /// Verifies that a negated gate condition (@if (!Model.IsCollapsed)) produces
        /// a Property node with "!" prefix getter expression (M3 review finding).
        /// </summary>
        [TestMethod]
        public void NegatedGateCondition_ProducesNegatedPropertyGetter()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new ConditionalNode
                    {
                        IsReactive = true,
                        Condition = new BindingClassification
                        {
                            CSharpExpression = "!Model.IsCollapsed",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "IsCollapsed", "IsCollapsed")
                            }
                        },
                        TrueBranch = new List<IRNode>
                        {
                            MakeBinding("Model.Content", BindingMode.OneWay, BindingSourceKind.DataContext,
                                ExpressionTarget.TextContent, "e0", "Content")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);

            // Source(0) -> Property_IsCollapsed(1) -> NegatedProperty_!IsCollapsed(2) -> Gate(3)
            topology.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            topology.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            topology.NodeTypes[2].Should().Be(GraphNodeTypeConstants.Property,
                "negated condition should create a second Property node");

            // The negated property getter should have "!" prefix
            topology.GetterExpressions[2].Should().StartWith("!",
                "negated gate condition should produce getter with ! prefix");
            topology.GetterExpressions[2].Should().Be("!IsCollapsed");

            // Gate node follows
            topology.NodeTypes[3].Should().Be(GraphNodeTypeConstants.Gate);
        }

        private static ExpressionBindingNode MakeBinding(
            string csharpExpr,
            BindingMode mode,
            BindingSourceKind sourceKind,
            ExpressionTarget target,
            string elementId,
            string propertyName)
        {
            var deps = mode == BindingMode.OneTime
                ? new List<ObservableDependency>()
                : new List<ObservableDependency>
                {
                    new ObservableDependency(sourceKind, propertyName, propertyName)
                };

            return new ExpressionBindingNode
            {
                Target = target,
                ElementId = elementId,
                Classification = new BindingClassification
                {
                    CSharpExpression = csharpExpr,
                    Mode = mode,
                    SourceKind = sourceKind,
                    Dependencies = deps
                }
            };
        }
    }
}
