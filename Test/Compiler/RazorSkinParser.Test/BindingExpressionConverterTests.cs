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
    /// Contract tests for the Razor binding-expression whitelist (issue #102): supported syntax
    /// becomes resolved JST with correct precedence; anything else fails with a diagnostic
    /// naming the expression.
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

        private string ToJs(string csharp)
        {
            var expression = BindingExpressionConverter.Convert(csharp, _scope, ResolvePath, null);
            var builder = new StringBuilder();
            var jsWriter = new JSWriter(true, false);
            expression.Write(jsWriter);
            jsWriter.Write(new StringWriter(builder));
            return builder.ToString().Trim();
        }

        private void ShouldReject(string csharp, string reasonFragment)
        {
            Action act = () => BindingExpressionConverter.Convert(csharp, _scope, ResolvePath, null);
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
        public void ConditionalShapesTheJsWriterCannotParenthesise_AreRejected()
        {
            ShouldReject("(Model.A ? Model.B : Model.C) ? \"x\" : \"y\"", "condition of another conditional");
            ShouldReject("(Model.A ? Model.B : Model.C) || Model.D", "left operand of '||'");
        }

        [TestMethod]
        public void UnsupportedSyntax_FailsWithLocatedDiagnostic()
        {
            ShouldReject("Model.Format(Model.A)", "is not supported");
            ShouldReject("Model.A / 2", "operator '/'");
            ShouldReject("Model.A % 2", "operator '%'");
            ShouldReject("Model.A ?? \"x\"", "operator '??'");
            ShouldReject("Model.A?.B", "is not supported");
            ShouldReject("$\"x{Model.A}\"", "is not supported");
            ShouldReject("'c'", "is not supported");
            ShouldReject("Model.A +", "not a valid C# expression");
            ShouldReject("Unknown.Thing", "cannot be resolved");
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
