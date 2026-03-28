using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using System.Collections.Generic;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class BinderEmitterTests
    {
        [TestMethod]
        public void OneWayDataContext_EmitsCorrectFlags()
        {
            var binding = CreateBinding(
                "Model.Name",
                BindingMode.OneWay,
                BindingSourceKind.DataContext,
                ExpressionTarget.TextContent,
                new[] { new ObservableDependency(BindingSourceKind.DataContext, "Name", "Name") });

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 0, 0);

            // 17 = ONEWAY_DATACONTEXT (0x10 | 0x01)
            result.Should().Contain(", 17,");
            result.Should().Contain("SkinBinderInfo_factory");
            result.Should().Contain("get_name()");
            result.Should().Contain("\"Name\"");
            result.Should().Contain("SetTextContent");
        }

        [TestMethod]
        public void OneTimeDataContext_EmitsCorrectFlags()
        {
            var binding = CreateBinding(
                "Model.AppVersion",
                BindingMode.OneTime,
                BindingSourceKind.DataContext,
                ExpressionTarget.TextContent,
                new ObservableDependency[0]);

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 0, 0);

            // 17 = PropertyBinder | DataContext (both OneTime and OneWay always use PropertyBinder)
            result.Should().Contain(", 17,");
            result.Should().Contain("get_appVersion()");
        }

        [TestMethod]
        public void OneWayTemplateParent_EmitsCorrectFlags()
        {
            var binding = CreateBinding(
                "Control.CssClass",
                BindingMode.OneWay,
                BindingSourceKind.TemplateParent,
                ExpressionTarget.TextContent,
                new[] { new ObservableDependency(BindingSourceKind.TemplateParent, "CssClass", "CssClass") });

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 0, 0);

            // 19 = ONEWAY_TEMPLATEPARENT (0x10 | 0x03)
            result.Should().Contain(", 19,");
            result.Should().Contain("tp.get_cssClass()");
        }

        [TestMethod]
        public void OneTimeTemplateParent_EmitsCorrectFlags()
        {
            var binding = CreateBinding(
                "Control.Title",
                BindingMode.OneTime,
                BindingSourceKind.TemplateParent,
                ExpressionTarget.TextContent,
                new ObservableDependency[0]);

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 0, 0);

            // 19 = PropertyBinder | TemplateParent (both OneTime and OneWay always use PropertyBinder)
            result.Should().Contain(", 19,");
        }

        [TestMethod]
        public void AttributeTarget_EmitsSetAttribute()
        {
            var binding = CreateBinding(
                "Model.CssClass",
                BindingMode.OneWay,
                BindingSourceKind.DataContext,
                ExpressionTarget.Attribute,
                new[] { new ObservableDependency(BindingSourceKind.DataContext, "CssClass", "CssClass") });

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 0, 0);

            result.Should().Contain("SetAttribute");
        }

        [TestMethod]
        public void CssClassTarget_EmitsSetClassName()
        {
            var binding = CreateBinding(
                "Model.CssClass",
                BindingMode.OneWay,
                BindingSourceKind.DataContext,
                ExpressionTarget.CssClass,
                new[] { new ObservableDependency(BindingSourceKind.DataContext, "CssClass", "CssClass") });

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 0, 0);

            result.Should().Contain("SetClassName");
        }

        [TestMethod]
        public void MultipleDependencies_EmitsAllPropertyNames()
        {
            var binding = CreateBinding(
                "Model.Price * Model.Quantity",
                BindingMode.OneWay,
                BindingSourceKind.DataContext,
                ExpressionTarget.TextContent,
                new[]
                {
                    new ObservableDependency(BindingSourceKind.DataContext, "Price", "Price"),
                    new ObservableDependency(BindingSourceKind.DataContext, "Quantity", "Quantity")
                });

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 0, 0);

            result.Should().Contain("\"Price\"");
            result.Should().Contain("\"Quantity\"");
        }

        [TestMethod]
        public void ObjectAndBinderIndices_IncludedInOutput()
        {
            var binding = CreateBinding(
                "Model.Name",
                BindingMode.OneWay,
                BindingSourceKind.DataContext,
                ExpressionTarget.TextContent,
                new[] { new ObservableDependency(BindingSourceKind.DataContext, "Name", "Name") });

            var result = BinderEmitter.EmitSkinBinderInfo(binding, 3, 5);

            result.Should().Contain(", 3, 5,");
        }

        private static ExpressionBindingNode CreateBinding(
            string csharpExpr,
            BindingMode mode,
            BindingSourceKind sourceKind,
            ExpressionTarget target,
            ObservableDependency[] deps)
        {
            return new ExpressionBindingNode
            {
                Target = target,
                Classification = new BindingClassification
                {
                    CSharpExpression = csharpExpr,
                    Mode = mode,
                    SourceKind = sourceKind,
                    Dependencies = new List<ObservableDependency>(deps)
                }
            };
        }
    }
}
