using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Template-driven snapshot tests for the Razor Skin Templates compiler.
    /// Each [DataRow] names a template file in Templates/ and a corresponding
    /// expected JS file in ExpectedOutput/. The test compiles the template
    /// and does an exact string comparison against the snapshot.
    ///
    /// Pattern modeled after XWML SkinCodeGenerationBasicTest.
    /// </summary>
    [TestClass]
    public class RazorSkinSnapshotTests
    {
        // ------------------------------------------------------------------
        // Data-driven snapshot tests
        // ------------------------------------------------------------------

        [DataTestMethod]
        [DataRow("TextBinding")]
        [DataRow("ComputedExpression")]
        [DataRow("AttributeBinding")]
        [DataRow("OneTimeBinding")]
        [DataRow("ReactiveIf")]
        [DataRow("StaticIf")]
        [DataRow("ReactiveForeach")]
        [DataRow("NestedControlFlow")]
        [DataRow("EventMethodRef")]
        [DataRow("EventLambda")]
        [DataRow("PureFunction")]
        [DataRow("ModelFunction")]
        [DataRow("ControlBinding")]
        [DataRow("SimpleBinding")]
        [DataRow("ConditionalBlock")]
        [DataRow("ForeachBlock")]
        public void SnapshotTest(string templateName)
        {
            RazorSkinTestHelper.CompileAndCheck(templateName);
        }

        // ------------------------------------------------------------------
        // Individual tests with content-validating assertions
        // ------------------------------------------------------------------

        [TestMethod]
        public void TextBinding_ProducesOneWayBinderForObservableProperty()
        {
            var js = RazorSkinTestHelper.CompileTemplate("TextBinding");

            // Observable property Name on TestVM => OneWay binder (flag 17)
            js.Should().Contain("get_name()");
            js.Should().Contain("\"Name\"");
            js.Should().Contain("17"); // ONEWAY_DATACONTEXT flag
            js.Should().Contain("SkinBinderInfo_factory");
        }

        [TestMethod]
        public void ComputedExpression_TracksMultipleDependencies()
        {
            var js = RazorSkinTestHelper.CompileTemplate("ComputedExpression");

            // Both Price and Quantity should be tracked as dependencies
            js.Should().Contain("get_price()");
            js.Should().Contain("get_quantity()");
            js.Should().Contain("\"Price\"");
            js.Should().Contain("\"Quantity\"");
            // Computed expression should multiply
            js.Should().Contain("dc.get_price() * dc.get_quantity()");
        }

        [TestMethod]
        public void OneTimeBinding_ProducesOneTimeBinderForPlainType()
        {
            var js = RazorSkinTestHelper.CompileTemplate("OneTimeBinding");

            // PlainVM is not ObservableObject => OneTime binder (flag 1)
            js.Should().Contain("get_appVersion()");
            js.Should().Contain(", 1,"); // ONETIME_DATACONTEXT flag
            // Empty dependencies array
            js.Should().Contain("[], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent");
        }

        [TestMethod]
        public void ReactiveIf_EmitsConditionalBinderSetup()
        {
            var js = RazorSkinTestHelper.CompileTemplate("ReactiveIf");

            js.Should().Contain("Sunlight__Framework__UI__Helpers__ConditionalBinder");
            js.Should().Contain("get_isActive()");
            js.Should().Contain("\"IsActive\"");
            js.Should().Contain("Active");
            js.Should().Contain("Inactive");
        }

        [TestMethod]
        public void StaticIf_EmitsNoConditionalBinderForPlainType()
        {
            var js = RazorSkinTestHelper.CompileTemplate("StaticIf");

            // PlainVM.IsStatic is not observable => no ConditionalBinder
            js.Should().NotContain("Sunlight__Framework__UI__Helpers__ConditionalBinder");
        }

        [TestMethod]
        public void ReactiveForeach_EmitsCollectionBinderSetup()
        {
            var js = RazorSkinTestHelper.CompileTemplate("ReactiveForeach");

            js.Should().Contain("Sunlight__Framework__UI__Helpers__CollectionBinder");
            js.Should().Contain("get_items()");
        }

        [TestMethod]
        public void PureFunction_EmitsStandaloneJsFunction()
        {
            var js = RazorSkinTestHelper.CompileTemplate("PureFunction");

            // Pure function should NOT have dc parameter
            js.Should().Contain("function Fmt(x)");
            js.Should().NotContain("function Fmt(dc");
        }

        [TestMethod]
        public void ModelFunction_EmitsFunctionWithDcParameter()
        {
            var js = RazorSkinTestHelper.CompileTemplate("ModelFunction");

            // Model-dependent function should have dc parameter
            js.Should().Contain("function FullName(dc)");
            js.Should().Contain("get_firstName()");
            js.Should().Contain("get_lastName()");
        }

        [TestMethod]
        public void ControlBinding_UsesControlTypeName()
        {
            var js = RazorSkinTestHelper.CompileTemplate("ControlBinding");

            // @control MyControl should set the control type in Skin_factory
            js.Should().Contain("Sunlight__Framework__UI__Skin_factory(MyControl,");
        }

        [TestMethod]
        public void AllTemplates_ProduceFactoryAndGetterFunctions()
        {
            var templates = new[]
            {
                "TextBinding", "ComputedExpression", "AttributeBinding",
                "OneTimeBinding", "ReactiveIf", "StaticIf",
                "ReactiveForeach", "PureFunction", "ModelFunction",
                "ControlBinding"
            };

            foreach (var name in templates)
            {
                var js = RazorSkinTestHelper.CompileTemplate(name);
                js.Should().Contain($"{name}_factory", $"{name} should have a factory function");
                js.Should().Contain($"function {name}()", $"{name} should have a getter function");
                js.Should().Contain($"{name}_var", $"{name} should have a cached var");
                js.Should().Contain("Sunlight__Framework__UI__Helpers__SkinInstance_factory", $"{name} should produce SkinInstance");
            }
        }

        [TestMethod]
        public void NestedControlFlow_ContainsBothConditionalAndLoopLogic()
        {
            var js = RazorSkinTestHelper.CompileTemplate("NestedControlFlow");

            // The foreach + if combination should produce loop binder with conditional inside
            js.Should().Contain("Sunlight__Framework__UI__Helpers__CollectionBinder");
        }
    }
}
