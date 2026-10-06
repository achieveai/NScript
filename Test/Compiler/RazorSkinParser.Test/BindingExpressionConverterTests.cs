using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.RazorSkin.CodeGen;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Contract tests for the Razor binding-expression converter (issue #102, restored in #104):
    /// supported syntax — operators (incl. <c>/ % ??</c>), dotted paths of any depth, and instance
    /// method invocations — becomes resolved JST with correct precedence; anything else fails with
    /// a diagnostic naming the expression. Path/invocation resolution is stubbed here; the real
    /// Cecil-backed resolver is covered by the emitter and framework tests.
    /// </summary>
    [TestClass]
    public class BindingExpressionConverterTests
    {
        private IdentifierScope _scope;

        [TestInitialize]
        public void Setup()
        {
            _scope = new IdentifierScope(false);
        }

        // Stand-in for the emitter's resolver: every path becomes one identifier named
        // after its segments; paths rooted at "Unknown" are unresolvable.
        private Expression ResolvePath(IReadOnlyList<string> segments)
        {
            if (segments[0] == "Unknown") return null;
            return new IdentifierExpression(
                SimpleIdentifier.CreateScopeIdentifier(_scope, string.Join("_", segments), true),
                _scope);
        }

        // Stand-in for the emitter's invocation resolver: "recv.Method(args)" becomes a call to one
        // identifier named after the receiver segments and method; receivers rooted at "Unknown"
        // are unresolvable.
        private Expression ResolveInvocation(
            IReadOnlyList<string> receiverSegments, string methodName, IReadOnlyList<Expression> arguments)
        {
            if (receiverSegments[0] == "Unknown") return null;
            var target = new IdentifierExpression(
                SimpleIdentifier.CreateScopeIdentifier(
                    _scope, string.Join("_", receiverSegments) + "_" + methodName, true),
                _scope);
            return new MethodCallExpression(null, _scope, target, arguments.ToArray());
        }

        private string ToJs(string csharp)
        {
            var expression = BindingExpressionConverter.Convert(
                csharp, _scope, ResolvePath, ResolveInvocation, null);
            var builder = new StringBuilder();
            var jsWriter = new JSWriter(true, false);
            expression.Write(jsWriter);
            jsWriter.Write(new StringWriter(builder));
            return builder.ToString().Trim();
        }

        private void ShouldReject(string csharp, string reasonFragment)
        {
            Action act = () => BindingExpressionConverter.Convert(
                csharp, _scope, ResolvePath, ResolveInvocation, null);
            act.Should().Throw<RazorSubControlDiagnosticException>()
                .Which.Message.Should().Contain("'" + csharp + "'").And.Contain(reasonFragment);
        }

        [TestMethod]
        public void SupportedOperators_MapToJavaScript_WithLooseEquality()
        {
            ToJs("Model.A == Model.B").Should().Be("Model_A == Model_B");
            ToJs("Model.A != null").Should().Be("Model_A != null");
            ToJs("Model.A && !Control.B || Model.C").Should().Be("Model_A && !Control_B || Model_C");
            ToJs("Model.A < 1 && Model.B >= 2.5").Should().Be("Model_A < 1 && Model_B >= 2.5");
            ToJs("Model.A <= 1 || Model.B > 2 == true").Should().Be("Model_A <= 1 || Model_B > 2 == true");
            ToJs("Model.A ? \"on\" : \"off\"").Should().Be("Model_A ? \"on\" : \"off\"");
        }

        [TestMethod]
        public void ArithmeticOperators_IncludingDivideAndRemainder_AreSupported()
        {
            ToJs("Model.A / Model.B").Should().Be("Model_A / Model_B");
            ToJs("Model.A % 2").Should().Be("Model_A % 2");
            ToJs("Model.A / 2 % 4 == 1").Should().Be("Model_A / 2 % 4 == 1");
        }

        [TestMethod]
        public void NullCoalescing_DesugarsToSingleEvalConditional()
        {
            // `a ?? b` evaluates `a` once via a temp the enclosing getter declares as `var`.
            ToJs("Model.A ?? \"x\"").Should().Be("(nc0 = Model_A) != null ? nc0 : \"x\"");
            // Nested `??` is right-associative; each gets its own temp.
            ToJs("Model.A ?? Model.B ?? Model.C")
                .Should().Be("(nc0 = Model_A) != null ? nc0 : (nc1 = Model_B) != null ? nc1 : Model_C");
        }

        [TestMethod]
        public void InstanceInvocations_AreResolvedThroughTheResolver()
        {
            ToJs("Model.Format(Model.A)").Should().Be("Model_Format(Model_A)");
            ToJs("Model.Refresh()").Should().Be("Model_Refresh()");
            ToJs("Control.Join(Model.A, \"-\")").Should().Be("Control_Join(Model_A, \"-\")");
            ToJs("Model.Sub.Compute(1)").Should().Be("Model_Sub_Compute(1)");
        }

        [TestMethod]
        public void DeepInstancePaths_AreHandedToTheResolverWhole()
        {
            ToJs("Model.A.B.C").Should().Be("Model_A_B_C");
            ToJs("\"p-\" + Control.Inner.Label").Should().Be("\"p-\" + Control_Inner_Label");
        }

        [TestMethod]
        public void Precedence_IsPreservedAcrossParentheses()
        {
            ToJs("Model.A + Model.B * Model.C").Should().Be("Model_A + Model_B * Model_C");
            ToJs("(Model.A + Model.B) * Model.C").Should().Be("(Model_A + Model_B) * Model_C");
            ToJs("Model.A - -Model.B").Should().Be("Model_A - -Model_B");
        }

        [TestMethod]
        public void BareRootsAndStringEscapes_AreEmitted()
        {
            ToJs("item").Should().Be("item");
            ToJs("\"a\\\"b\" + item.Name").Should().Be("\"a\\\"b\" + item_Name");
        }

        [TestMethod]
        public void ConditionalsInConditionAndLeftOfOrPositions_AreParenthesised()
        {
            // PR #105 review F-001: the JST writer gave `?:` and `||` equal precedence and never
            // parenthesised a conditional as the condition of another conditional or as the left
            // operand of `||`. The emitter synthesises exactly that shape for null-safe mid-path
            // reads (`(h = recv) == null ? null : h.Prop`), so the writer must group it.
            ToJs("(Model.A ? Model.B : Model.C) ? \"x\" : \"y\"")
                .Should().Be("(Model_A ? Model_B : Model_C) ? \"x\" : \"y\"");
            ToJs("(Model.A ? Model.B : Model.C) || Model.D")
                .Should().Be("(Model_A ? Model_B : Model_C) || Model_D");
            // `??` desugars to the same assignment-in-conditional shape as a null-safe hop.
            ToJs("(Model.A ?? Model.B) ? \"x\" : \"y\"")
                .Should().Be("((nc0 = Model_A) != null ? nc0 : Model_B) ? \"x\" : \"y\"");
            ToJs("(Model.A ?? Model.B) || Model.C")
                .Should().Be("((nc0 = Model_A) != null ? nc0 : Model_B) || Model_C");
            // The right operand of `||` and both branches of `?:` were already grouped correctly;
            // `||` in condition position must stay ungrouped.
            ToJs("Model.A || (Model.B ? Model.C : Model.D)")
                .Should().Be("Model_A || (Model_B ? Model_C : Model_D)");
            ToJs("Model.A || Model.B ? \"x\" : \"y\"").Should().Be("Model_A || Model_B ? \"x\" : \"y\"");
            ToJs("Model.A ? Model.B ? \"x\" : \"y\" : \"z\"").Should().Be("Model_A ? Model_B ? \"x\" : \"y\" : \"z\"");
        }

        [TestMethod]
        public void UnsupportedSyntax_FailsWithLocatedDiagnostic()
        {
            ShouldReject("Model.A?.B", "is not supported");
            ShouldReject("$\"x{Model.A}\"", "is not supported");
            ShouldReject("'c'", "is not supported");
            ShouldReject("Model.A +", "not a valid C# expression");
            ShouldReject("Unknown.Thing", "cannot be resolved");
            ShouldReject("Unknown.Thing(1)", "cannot be resolved");
            ShouldReject("Format(Model.A)", "is not supported"); // receiver-less call target
        }

        [TestMethod]
        public void CollectMemberPaths_ReturnsOnlyMaximalChains()
        {
            BindingExpressionConverter.CollectMemberPaths("\"c-\" + Control.Label == Model.A.B ? x : \"y\"")
                .Select(p => string.Join(".", p))
                .Should().BeEquivalentTo("Control.Label", "Model.A.B", "x");
            BindingExpressionConverter.CollectMemberPaths("Model.A +").Should().BeEmpty();
        }
    }
}
