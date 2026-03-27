using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class ExpressionJsEmitterTests
    {
        [TestMethod]
        public void SimpleProperty_ConvertsToGetterCall()
        {
            var result = ExpressionJsEmitter.ToJsGetter("Model.Name");
            result.Should().Be("dc.get_name()");
        }

        [TestMethod]
        public void ControlProperty_ConvertsToTemplateParentGetter()
        {
            var result = ExpressionJsEmitter.ToJsGetter("Control.CssClass");
            result.Should().Be("tp.get_cssClass()");
        }

        [TestMethod]
        public void MultipleProperties_AllConverted()
        {
            var result = ExpressionJsEmitter.ToJsGetter("Model.Price * Model.Quantity");
            result.Should().Contain("dc.get_price()");
            result.Should().Contain("dc.get_quantity()");
        }

        [TestMethod]
        public void CustomParamNames_Used()
        {
            var result = ExpressionJsEmitter.ToJsGetter("Model.Name", "src", "ctrl");
            result.Should().Be("src.get_name()");
        }

        [TestMethod]
        public void PropertyToGetterName_LowercasesFirst()
        {
            ExpressionJsEmitter.PropertyToGetterName("Name").Should().Be("get_name");
            ExpressionJsEmitter.PropertyToGetterName("CssClass").Should().Be("get_cssClass");
            ExpressionJsEmitter.PropertyToGetterName("IsActive").Should().Be("get_isActive");
        }

        [TestMethod]
        public void PropertyToSetterName_LowercasesFirst()
        {
            ExpressionJsEmitter.PropertyToSetterName("Name").Should().Be("set_name");
            ExpressionJsEmitter.PropertyToSetterName("Query").Should().Be("set_query");
        }

        [TestMethod]
        public void EmptyOrNull_HandledGracefully()
        {
            ExpressionJsEmitter.PropertyToGetterName("").Should().Be("");
            ExpressionJsEmitter.PropertyToGetterName(null).Should().BeNull();
            ExpressionJsEmitter.PropertyToSetterName("").Should().Be("");
            ExpressionJsEmitter.PropertyToSetterName(null).Should().BeNull();
        }

        [TestMethod]
        public void ChainedPropertyAccess_ConvertsNestedGetters()
        {
            var result = ExpressionJsEmitter.ToJsGetter("Model.Customer.Name");
            result.Should().Contain("get_customer()");
            result.Should().Contain("get_name()");
        }
    }
}
