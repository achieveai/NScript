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
    }
}
