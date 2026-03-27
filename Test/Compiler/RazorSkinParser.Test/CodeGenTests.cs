using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class CodeGenTests
    {
        [TestMethod]
        public void GeneratesFactoryFunctionForStaticHtml()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div>Hello</div>" });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("TestSkin_factory");
            js.Should().Contain("createElement");
            js.Should().Contain("innerHTML");
            js.Should().Contain("cloneNode");
            js.Should().Contain("SkinInstance");
        }

        [TestMethod]
        public void GeneratesGetterForOneWayBinding()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            var binding = new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
                Classification = new BindingClassification
                {
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    CSharpExpression = "Model.Name",
                    Dependencies = new System.Collections.Generic.List<ObservableDependency>
                    {
                        new ObservableDependency(BindingSourceKind.DataContext, "Name", "Name")
                    }
                }
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div></div>" });
            ir.Children.Add(binding);

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("get_name");
            js.Should().Contain("SkinBinderInfo");
            js.Should().Contain("\"Name\"");
        }

        [TestMethod]
        public void GeneratesSkinGetterFunction()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div/>" });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("TestSkin_var");
            js.Should().Contain("function TestSkin()");
            js.Should().Contain("Skin_factory");
        }

        [TestMethod]
        public void GeneratesEventBinderForMethodReference()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<button>Submit</button>" });
            ir.Children.Add(new EventNode
            {
                DomEventName = "click",
                HandlerExpression = "Model.OnSubmit",
                IsLambda = false
            });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("addEventListener");
            js.Should().Contain("'click'");
            js.Should().Contain("get_onSubmit");
        }

        [TestMethod]
        public void GeneratesEventBinderForLambda()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<button>Cancel</button>" });
            ir.Children.Add(new EventNode
            {
                DomEventName = "click",
                HandlerExpression = "(evt) => Model.Cancel()",
                IsLambda = true
            });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("addEventListener");
            js.Should().Contain("'click'");
            js.Should().Contain("function(e)");
        }

        [TestMethod]
        public void GeneratesPureFunctionFromFunctionsBlock()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Functions.Add(new FunctionNode
            {
                FunctionName = "FormatPrice",
                CSharpSource = "string FormatPrice(decimal price) => price.ToString(\"C\");",
                IsPure = true
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "<div/>" });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("function FormatPrice(price)");
        }

        [TestMethod]
        public void GeneratesModelDependentFunction()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Functions.Add(new FunctionNode
            {
                FunctionName = "FullName",
                CSharpSource = "string FullName() => Model.FirstName + \" \" + Model.LastName;",
                IsPure = false
            });
            ir.Children.Add(new HtmlNode { HtmlContent = "<div/>" });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("function FullName(dc)");
            js.Should().Contain("get_firstName");
            js.Should().Contain("get_lastName");
        }

        [TestMethod]
        public void GeneratesReactiveConditionalBinder()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            var cond = new ConditionalNode
            {
                IsReactive = true,
                Condition = new BindingClassification
                {
                    CSharpExpression = "Model.IsLoading",
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    Dependencies = new System.Collections.Generic.List<ObservableDependency>
                    {
                        new ObservableDependency(BindingSourceKind.DataContext, "IsLoading", "IsLoading")
                    }
                }
            };
            cond.TrueBranch.Add(new HtmlNode { HtmlContent = "<div>Loading...</div>" });
            cond.FalseBranch.Add(new HtmlNode { HtmlContent = "<div>Ready</div>" });
            ir.Children.Add(cond);

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("Sunlight__Framework__UI__Helpers__ConditionalBinder");
            js.Should().Contain("get_isLoading");
            js.Should().Contain("\"IsLoading\"");
            js.Should().Contain("Loading...");
        }

        [TestMethod]
        public void GeneratesReactiveLoopBinder()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            var loop = new LoopNode
            {
                ItemVariableName = "item",
                CollectionExpression = "Model.Items",
                IsObservableCollection = true,
                CollectionSourceKind = BindingSourceKind.DataContext
            };
            loop.ItemTemplate.Add(new HtmlNode { HtmlContent = "<li>Item</li>" });
            ir.Children.Add(loop);

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("Sunlight__Framework__UI__Helpers__CollectionBinder");
            js.Should().Contain("get_items");
        }

        [TestMethod]
        public void GeneratesSubControlFactoryCall()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            var sub = new SubControlNode
            {
                TypeName = "SearchBox",
                ResolvedTypeName = "SearchBox",
                ElementId = "searchBox"
            };
            sub.PropertyBindings.Add(new SubControlPropertyBinding
            {
                PropertyName = "Query",
                Classification = new BindingClassification
                {
                    CSharpExpression = "Model.SearchQuery",
                    Mode = BindingMode.OneTime,
                    SourceKind = BindingSourceKind.DataContext
                }
            });
            ir.Children.Add(sub);
            ir.Children.Add(new HtmlNode { HtmlContent = "<div/>" });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("SearchBox_factory");
            js.Should().Contain("set_query");
        }
    }
}
