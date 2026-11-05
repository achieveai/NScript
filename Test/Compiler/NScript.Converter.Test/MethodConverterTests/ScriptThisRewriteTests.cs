namespace NScript.Converter.Test.MethodConverterTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// When an instance [Script] method becomes static (optimized builds), its body's <c>this</c>
    /// is rewritten to the <c>this_</c> parameter. Regex literals are text, not code.
    /// </summary>
    [TestClass]
    public class ScriptThisRewriteTests
    {
        [DataTestMethod]
        [DataRow("return /this/.test(a);", "return /this/.test(a);")]
        [DataRow("return /this/.test(this.a);", "return /this/.test(this_.a);")]
        [DataRow("b = a.replace(/this|[/]this/g, this);", "b = a.replace(/this|[/]this/g, this_);")]
        [DataRow("b = (/\\/this/).test(a);", "b = (/\\/this/).test(a);")]
        [DataRow("b = this.a / this.b / 2;", "b = this_.a / this_.b / 2;")]
        [DataRow("b = (this) / 2;", "b = (this_) / 2;")]
        [DataRow("b = 'this';", "b = 'this';")]
        public void FixThisVariable_KeepsRegexLiteralsAndRewritesReceiver(string script, string expected)
        {
            Assert.AreEqual(expected, MethodConverter.FixThisVariable(script));
        }

        [DataTestMethod]
        [DataRow("a = 1; // this\r\nb = this;", "a = 1; // this\r\nb = this_;")]
        [DataRow("a = 1; // this\nb = this;", "a = 1; // this\nb = this_;")]
        [DataRow("b = /* this */ this.a;", "b = /* this */ this_.a;")]
        [DataRow("b = /* x */this.a;", "b = /* x */this_.a;")]
        [DataRow("b = this; // last", "b = this_; // last")]
        [DataRow("if (a) /this/.test(b);", "if (a) /this/.test(b);")]
        [DataRow("while (this.a) /this/.test(b);", "while (this_.a) /this/.test(b);")]
        [DataRow("if (f(a)) /this/.test(this);", "if (f(a)) /this/.test(this_);")]
        [DataRow("b = f(a) / this.c;", "b = f(a) / this_.c;")]
        public void FixThisVariable_HandlesCommentsAndControlHeaders(string script, string expected)
        {
            Assert.AreEqual(expected, MethodConverter.FixThisVariable(script));
        }

        // Same regex-vs-division rule as JavaScriptLexer.RegexAllowed: '++', '--' and the '}'
        // closing an @{...} import are values, so the '/' after them divides.
        [DataTestMethod]
        [DataRow("x = this.i++ / this.n / 2;", "x = this_.i++ / this_.n / 2;")]
        [DataRow("x = this.i-- / this.n / 2;", "x = this_.i-- / this_.n / 2;")]
        [DataRow("x = @{[System.Web]System.Int::fooBar} / this.n / 2;", "x = @{[System.Web]System.Int::fooBar} / this_.n / 2;")]
        [DataRow("x = @{[mscorlib]System.Math::f([mscorlib]System.String)} / this.n / 2;", "x = @{[mscorlib]System.Math::f([mscorlib]System.String)} / this_.n / 2;")]
        [DataRow("if (a) { b = 1; } /this/.test(c);", "if (a) { b = 1; } /this/.test(c);")]
        [DataRow("x = a + +/this/.test(b);", "x = a + +/this/.test(b);")]
        public void FixThisVariable_DivisionAfterIncrementAndImport(string script, string expected)
        {
            Assert.AreEqual(expected, MethodConverter.FixThisVariable(script));
        }
    }
}
