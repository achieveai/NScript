using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class GraphDescriptorEmitterTests
    {
        // --- Helpers ---

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

        private static GraphTopology BuildSimpleTextBinding()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "TestBinding",
                ModelTypeName = "TestVM",
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
            return GraphTopologyBuilder.Build(template);
        }

        // --- Tests ---

        [TestMethod]
        public void SimpleTextBinding_EmitsCorrectStructure()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // Should declare the graph variable
            js.Should().Contain("var TestBinding_graph =");

            // Should contain all required keys
            js.Should().Contain("nodeTypes:");
            js.Should().Contain("getters:");
            js.Should().Contain("consumers:");
            js.Should().Contain("gateIndices:");
            js.Should().Contain("defaultValues:");
            js.Should().Contain("targetInfos:");
            js.Should().Contain("subscriptions:");
            js.Should().Contain("sourceType:");
            js.Should().Contain("subscribeMode:");
            js.Should().Contain("nodeCount:");

            // nodeCount should be 3 (Source, Property, DomTarget)
            js.Should().Contain("nodeCount: 3");

            // sourceType should be the model type name
            js.Should().Contain("\"TestVM\"");

            // subscribeMode should be 0
            js.Should().Contain("subscribeMode: 0");

            // Should end with a semicolon
            js.TrimEnd().Should().EndWith(";");
        }

        [TestMethod]
        public void Getters_UseExpressionJsEmitter()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // Property node getter for "Name" should emit a getter function
            js.Should().Contain("function(dc)");
            js.Should().Contain("dc.get_name()");
        }

        [TestMethod]
        public void TargetInfos_IncludeSetterNames()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // DomTarget targetInfo should contain elem index and setter name
            js.Should().Contain("elem: 0");
            js.Should().Contain("set: SetTextContent");
        }

        [TestMethod]
        public void Subscriptions_OnlyForOneWayBindings()
        {
            // OneWay binding → has subscription
            var topology = BuildSimpleTextBinding();
            var jsOneWay = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);
            jsOneWay.Should().Contain("\"Name\"");

            // OneTime binding → no subscriptions
            var template = new SkinTemplateNode
            {
                TemplateName = "TestOneTime",
                ModelTypeName = "TestVM",
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
            var oneTimeTopology = GraphTopologyBuilder.Build(template);
            var jsOneTime = GraphDescriptorEmitter.EmitDescriptor("TestOneTime", oneTimeTopology, null);

            // subscriptions array should be empty
            jsOneTime.Should().Contain("subscriptions: []");
        }

        [TestMethod]
        public void NodeTypes_ContainCorrectIntValues()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // Source=0, Property=1, DomTarget=3
            js.Should().Contain("nodeTypes: [0, 1, 3]");
        }

        [TestMethod]
        public void GateIndices_AllMinusOneForUngatedNodes()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // All 3 nodes are ungated → [-1, -1, -1]
            js.Should().Contain("gateIndices: [-1, -1, -1]");
        }

        [TestMethod]
        public void DefaultValues_ContainStringDefaultForDomTarget()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // Source=null, Property=null, DomTarget=""
            js.Should().Contain("defaultValues: [null, null, \"\"]");
        }

        [TestMethod]
        public void Consumers_AdjacencyListEmitted()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // Source -> [1], Property -> [2], DomTarget -> []
            js.Should().Contain("consumers: [[1], [2], []]");
        }

        [TestMethod]
        public void AttributeTarget_EmitsSetAttribute()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "AttrTest",
                ModelTypeName = "TestVM",
                Children =
                {
                    new ExpressionBindingNode
                    {
                        Target = ExpressionTarget.Attribute,
                        ElementId = "e0",
                        Classification = new BindingClassification
                        {
                            CSharpExpression = "Model.Href",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "Href", "Href")
                            }
                        }
                    }
                }
            };
            var topology = GraphTopologyBuilder.Build(template);
            var js = GraphDescriptorEmitter.EmitDescriptor("AttrTest", topology, null);

            js.Should().Contain("set: SetAttribute");
        }

        [TestMethod]
        public void CssClassTarget_EmitsSetClassName()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "CssTest",
                ModelTypeName = "TestVM",
                Children =
                {
                    new ExpressionBindingNode
                    {
                        Target = ExpressionTarget.CssClass,
                        ElementId = "e0",
                        Classification = new BindingClassification
                        {
                            CSharpExpression = "Model.ActiveClass",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "ActiveClass", "ActiveClass")
                            }
                        }
                    }
                }
            };
            var topology = GraphTopologyBuilder.Build(template);
            var js = GraphDescriptorEmitter.EmitDescriptor("CssTest", topology, null);

            js.Should().Contain("set: SetClassName");
        }

        [TestMethod]
        public void StyleTarget_EmitsSetStyle()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "StyleTest",
                ModelTypeName = "TestVM",
                Children =
                {
                    new ExpressionBindingNode
                    {
                        Target = ExpressionTarget.Style,
                        ElementId = "e0",
                        Classification = new BindingClassification
                        {
                            CSharpExpression = "Model.Display",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "Display", "Display")
                            }
                        }
                    }
                }
            };
            var topology = GraphTopologyBuilder.Build(template);
            var js = GraphDescriptorEmitter.EmitDescriptor("StyleTest", topology, null);

            js.Should().Contain("set: SetStyle");
        }

        [TestMethod]
        public void ComputedNode_EmitsGetterWithFullExpression()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "ComputedTest",
                ModelTypeName = "TestVM",
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
            var js = GraphDescriptorEmitter.EmitDescriptor("ComputedTest", topology, null);

            // Computed getter should use the full expression with both property conversions
            js.Should().Contain("dc.get_price()");
            js.Should().Contain("dc.get_quantity()");
        }

        [TestMethod]
        public void KnownFunctionNames_PreservedInGetters()
        {
            var knownFunctions = new HashSet<string> { "FormatPrice" };
            var template = new SkinTemplateNode
            {
                TemplateName = "FuncTest",
                ModelTypeName = "TestVM",
                Children =
                {
                    new ExpressionBindingNode
                    {
                        Target = ExpressionTarget.TextContent,
                        ElementId = "e0",
                        Classification = new BindingClassification
                        {
                            CSharpExpression = "Model.FormatPrice",
                            Mode = BindingMode.OneWay,
                            SourceKind = BindingSourceKind.DataContext,
                            Dependencies = new List<ObservableDependency>
                            {
                                new ObservableDependency(BindingSourceKind.DataContext, "FormatPrice", "FormatPrice")
                            }
                        }
                    }
                }
            };
            var topology = GraphTopologyBuilder.Build(template);
            var js = GraphDescriptorEmitter.EmitDescriptor("FuncTest", topology, knownFunctions);

            // Known function name should NOT be converted to a getter call
            js.Should().Contain("dc.FormatPrice");
            js.Should().NotContain("get_formatPrice");
        }

        [TestMethod]
        public void SourceNode_HasNullGetter()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // getters array first element (Source node) should be null
            js.Should().Contain("getters: [null,");
        }

        [TestMethod]
        public void DomTargetNode_HasNullGetter()
        {
            var topology = BuildSimpleTextBinding();
            var js = GraphDescriptorEmitter.EmitDescriptor("TestBinding", topology, null);

            // DomTarget getter should be null (last element in the 3-element array)
            // Pattern: getters: [null, function(dc) {...}, null]
            js.Should().MatchRegex(@"getters:\s*\[null,.*null\]");
        }

        [TestMethod]
        public void BooleanDefault_EmittedWithoutQuotes()
        {
            var template = new SkinTemplateNode
            {
                TemplateName = "GateTest",
                ModelTypeName = "TestVM",
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
                        TrueBranch = new List<IRNode>()
                    }
                }
            };
            var topology = GraphTopologyBuilder.Build(template);
            var js = GraphDescriptorEmitter.EmitDescriptor("GateTest", topology, null);

            // Gate node has boolean default (false), emitted without quotes
            js.Should().Contain("false");
            js.Should().NotContain("\"false\"");
        }
    }
}
