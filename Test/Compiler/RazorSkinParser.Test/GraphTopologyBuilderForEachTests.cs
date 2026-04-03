using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;
using System.Linq;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class GraphTopologyBuilderForEachTests
    {
        /// <summary>
        /// Verifies that a foreach loop creates a CollectionManager node with an item topology.
        /// </summary>
        [TestMethod]
        public void ForEachLoop_CreatesCollectionManagerWithItemTopology()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            MakeBinding("todo.Title", BindingMode.OneWay,
                                BindingSourceKind.DataContext, ExpressionTarget.TextContent, "e0", "Title")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);

            // Root graph should have a CollectionManager node
            topology.NodeTypes.Should().Contain(GraphNodeTypeConstants.CollectionManager);

            // CollectionManager should have an item topology
            topology.Collections.Should().NotBeEmpty("foreach loop should produce collection target");
            var colTarget = topology.Collections.First();
            colTarget.ItemTopology.Should().NotBeNull("foreach loop should produce item topology");
        }

        /// <summary>
        /// Verifies that the item topology has the correct ItemVariablePrefix set.
        /// </summary>
        [TestMethod]
        public void ForEachLoop_ItemTopology_HasItemVariablePrefix()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            MakeBinding("todo.Title", BindingMode.OneWay,
                                BindingSourceKind.DataContext, ExpressionTarget.TextContent, "e0", "Title")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;

            itemTopo.ItemVariablePrefix.Should().Be("todo.", "item variable prefix should include trailing dot");
        }

        /// <summary>
        /// Verifies that an item template with a property binding creates
        /// Source -> Property -> DomTarget in the item topology.
        /// </summary>
        [TestMethod]
        public void ForEachLoop_ItemPropertyBinding_ProducesPropertyNode()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "item",
                        CollectionExpression = "Model.Items",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            MakeBinding("item.Name", BindingMode.OneWay,
                                BindingSourceKind.DataContext, ExpressionTarget.TextContent, "e0", "Name")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;

            // Item topology: Source(0), Property(1) for Name, DomTarget(2)
            itemTopo.NodeCount.Should().Be(3);
            itemTopo.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            itemTopo.NodeTypes[1].Should().Be(GraphNodeTypeConstants.Property);
            itemTopo.NodeTypes[2].Should().Be(GraphNodeTypeConstants.DomTarget);

            // Getter stores the property name (prefix stripped by builder)
            itemTopo.GetterExpressions[1].Should().Be("Name");
        }

        /// <summary>
        /// Verifies that an event handler inside a foreach item template creates
        /// an EventBinding node in the item topology.
        /// </summary>
        [TestMethod]
        public void ForEachLoop_ItemEventBinding_ProducesEventNode()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            new EventNode
                            {
                                DomEventName = "click",
                                HandlerExpression = "todo.ToggleComplete"
                            }
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;

            // Should have Source(0) and EventBinding node
            itemTopo.NodeTypes.Should().Contain(GraphNodeTypeConstants.EventBinding);
            var eventIdx = System.Array.IndexOf(itemTopo.NodeTypes, GraphNodeTypeConstants.EventBinding);
            itemTopo.GetterExpressions[eventIdx].Should().Be("todo.ToggleComplete");
        }

        /// <summary>
        /// Verifies that a parent-context event (Model.Method(item)) inside a foreach
        /// is stored as-is in the getter expression for the emitter to handle.
        /// </summary>
        [TestMethod]
        public void ForEachLoop_ParentMethodEvent_PreservesHandlerExpression()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            new EventNode
                            {
                                DomEventName = "click",
                                HandlerExpression = "Model.OnSelect(todo)"
                            }
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;

            var eventIdx = System.Array.IndexOf(itemTopo.NodeTypes, GraphNodeTypeConstants.EventBinding);
            itemTopo.GetterExpressions[eventIdx].Should().Be("Model.OnSelect(todo)");
        }

        /// <summary>
        /// Verifies that a ternary computed expression inside a foreach item template
        /// creates a Computed node in the item topology.
        /// </summary>
        [TestMethod]
        public void ForEachLoop_TernaryComputed_ProducesComputedNode()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            MakeBinding(
                                "todo.IsCompleted ? \"done\" : \"pending\"",
                                BindingMode.OneWay,
                                BindingSourceKind.DataContext,
                                ExpressionTarget.CssClass,
                                "e0",
                                "IsCompleted")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;

            // Should have Computed node for the ternary
            itemTopo.NodeTypes.Should().Contain(GraphNodeTypeConstants.Computed);
        }

        /// <summary>
        /// Verifies that mixed item bindings and parent events in a foreach
        /// produce the correct topology with both item Property nodes
        /// and parent EventBinding nodes.
        /// </summary>
        [TestMethod]
        public void ForEachLoop_MixedItemAndParentBindings_ProducesCorrectTopology()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            // Parent method event: @Model.OnSelect(todo)
                            new EventNode
                            {
                                DomEventName = "click",
                                HandlerExpression = "Model.OnSelect(todo)"
                            },
                            // Item property: @todo.Title
                            MakeBinding("todo.Title", BindingMode.OneWay,
                                BindingSourceKind.DataContext, ExpressionTarget.TextContent, "e0", "Title"),
                            // Item event: @todo.ToggleComplete
                            new EventNode
                            {
                                DomEventName = "click",
                                HandlerExpression = "todo.ToggleComplete"
                            }
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;

            // Should have Source + at least Property + 2 EventBindings
            itemTopo.NodeTypes[0].Should().Be(GraphNodeTypeConstants.Source);
            itemTopo.NodeTypes.Count(t => t == GraphNodeTypeConstants.EventBinding).Should().Be(2,
                "should have 2 event bindings: parent method + item method");
            itemTopo.NodeTypes.Should().Contain(GraphNodeTypeConstants.Property,
                "should have a Property node for todo.Title");

            // Verify getter expressions contain both patterns
            itemTopo.GetterExpressions.Should().Contain("Model.OnSelect(todo)");
            itemTopo.GetterExpressions.Should().Contain("todo.ToggleComplete");
            itemTopo.GetterExpressions.Should().Contain("todo.Title");
        }

        /// <summary>
        /// Verifies that a negated ternary expression inside a foreach item template
        /// creates a Computed node (M1 review finding).
        /// </summary>
        [TestMethod]
        public void ForEachLoop_NegatedTernary_ProducesComputedNode()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            MakeBinding(
                                "!todo.IsCompleted ? \"active\" : \"done\"",
                                BindingMode.OneWay,
                                BindingSourceKind.DataContext,
                                ExpressionTarget.CssClass,
                                "e0",
                                "IsCompleted")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;
            itemTopo.NodeTypes.Should().Contain(GraphNodeTypeConstants.Computed);
        }

        /// <summary>
        /// Verifies that a string concatenation expression inside a foreach
        /// produces a Computed node (M1 review finding).
        /// </summary>
        [TestMethod]
        public void ForEachLoop_StringConcatExpression_ProducesComputedNode()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestTemplate",
                ModelTypeName = "TestModel",
                Children =
                {
                    new LoopNode
                    {
                        ItemVariableName = "todo",
                        CollectionExpression = "Model.Todos",
                        IsObservableCollection = true,
                        CollectionSourceKind = BindingSourceKind.DataContext,
                        ItemTemplate = new List<IRNode>
                        {
                            MakeBinding(
                                "\"item-\" + todo.Status",
                                BindingMode.OneWay,
                                BindingSourceKind.DataContext,
                                ExpressionTarget.CssClass,
                                "e0",
                                "Status")
                        }
                    }
                }
            };

            var topology = GraphTopologyBuilder.Build(template);
            var itemTopo = topology.Collections.First().ItemTopology;
            itemTopo.NodeTypes.Should().Contain(GraphNodeTypeConstants.Computed);
        }

        private static ExpressionBindingNode MakeBinding(
            string csharpExpr, BindingMode mode, BindingSourceKind sourceKind,
            ExpressionTarget target, string elementId, string propertyName)
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
