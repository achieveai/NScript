using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class GraphCodeGenTests
    {
        [TestMethod]
        public void GenerateGraphMode_ProducesGraphDescriptorAndFactory()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "Sunlight.Framework.UI.UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div>" });
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
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
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "</div>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // Should contain graph descriptor variable
            js.Should().Contain("var TestSkin_graph =");
            js.Should().Contain("nodeTypes:");
            js.Should().Contain("getters:");
            js.Should().Contain("consumers:");
            js.Should().Contain("subscriptions:");

            // Should contain factory function
            js.Should().Contain("function TestSkin_factory(skinFactory, doc)");
            js.Should().Contain("createElement");
            js.Should().Contain("innerHTML");
            js.Should().Contain("cloneNode");

            // Should contain getter function
            js.Should().Contain("function TestSkin()");
            js.Should().Contain("TestSkin_var");

            // SkinInstance_factory call should reference the graph descriptor
            js.Should().Contain("TestSkin_graph");
            js.Should().Contain("SkinInstance_factory");
        }

        [TestMethod]
        public void GenerateGraphMode_EmitsElementPathResolution()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "PathTest",
                ModelTypeName = "TestVM",
                ControlTypeName = "Sunlight.Framework.UI.UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div>" });
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
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
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "</div>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // Should emit GetElementFromPath calls for element path resolution
            js.Should().Contain("GetElementFromPath");
            js.Should().Contain("objStorage[0]");
        }

        [TestMethod]
        public void GenerateGraphMode_StaticHtml_EmitsEmptyGraph()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "StaticSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "Sunlight.Framework.UI.UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div>Hello</div>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // Should still have graph descriptor (with just the Source node)
            js.Should().Contain("var StaticSkin_graph =");
            js.Should().Contain("nodeCount: 1");

            // Factory and getter should still be present
            js.Should().Contain("function StaticSkin_factory");
            js.Should().Contain("function StaticSkin()");

            // SkinInstance_factory should reference graph
            js.Should().Contain("StaticSkin_graph");
        }

        [TestMethod]
        public void GenerateGraphMode_ManglesTypeName()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "MangleTest",
                ModelTypeName = "Sunlight.Framework.UI.Test.TestViewModelA",
                ControlTypeName = "Sunlight.Framework.UI.UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div/>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // Model type should be mangled with double underscores
            js.Should().Contain("Sunlight__Framework__UI__Test__TestViewModelA");
            // Control type should also be mangled
            js.Should().Contain("Sunlight__Framework__UI__UISkinableElement");
        }

        [TestMethod]
        public void GenerateGraphMode_PassesGraphDescriptorToSkinInstanceFactory()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "FactoryTest",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<span>" });
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
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
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "</span>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // The 5th parameter to SkinInstance_factory should be the graph descriptor,
            // not a tmplStore binder array
            js.Should().Contain("FactoryTest_graph, null, 0, 0)");
            // It should NOT contain SkinBinderInfo references
            js.Should().NotContain("SkinBinderInfo");
        }

        [TestMethod]
        public void GenerateGraphMode_WithPartIds_EmitsPartMapping()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "PartTest",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div>" });
            ir.Children.Add(new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
                ElementId = "nameLabel",
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
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "</div>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // Should contain the part ID mapping in SkinInstance_factory call
            js.Should().Contain("\"nameLabel\"");
        }

        [TestMethod]
        public void GenerateGraphMode_WithEvents_EmitsEventListeners()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "EventTest",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<button>" });
            ir.Children.Add(new EventNode
            {
                DomEventName = "click",
                HandlerExpression = "Model.Cancel()",
                IsLambda = true
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "</button>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // Should emit event listener attachment
            js.Should().Contain("addEventListener('click'");
        }

        [TestMethod]
        public void GenerateGraphMode_WithFunctions_EmitsFunctionBlocks()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "FuncTest",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement",
                Functions = new List<FunctionNode>
                {
                    new FunctionNode
                    {
                        FunctionName = "FormatPrice",
                        CSharpSource = "FormatPrice(decimal price) => price.ToString()",
                        IsPure = true
                    }
                }
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div/>" });

            var js = RazorSkinCodeGenerator.GenerateGraphMode(ir);

            // Should emit the function before the graph descriptor
            js.Should().Contain("function FormatPrice");
        }
    }
}
