//-----------------------------------------------------------------------
// <copyright file="ScriptBodyStatementTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// [Script] bodies accept try/catch/finally, 'for (var ...)' and regex literals (#136),
    /// and an unsupported form names itself and its line.
    /// </summary>
    [TestClass]
    public class ScriptBodyStatementTests
    {
        [TestMethod]
        public void Parse_TryCatch()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "try { b = a; } catch (e) { b = e; }",
                "var e;\r\ntry {\r\n  b = a;\r\n} catch (e) {\r\n  b = e;\r\n}",
                "b", "a");
        }

        [TestMethod]
        public void Parse_TryFinally()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "try { b = a; } finally { a = b; }",
                "try {\r\n  b = a;\r\n} finally {\r\n  a = b;\r\n}",
                "b", "a");
        }

        [TestMethod]
        public void Parse_TwoCatchesWithTheSameName()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "try { b = a; } catch (e) { b = e; } try { a = b; } catch (e) { a = e; }",
                "var e, ea;\r\ntry {\r\n  b = a;\r\n} catch (e) {\r\n  b = e;\r\n}\r\ntry {\r\n  a = b;\r\n} catch (ea) {\r\n  a = ea;\r\n}",
                "b", "a");
        }

        [TestMethod]
        public void Parse_VarDeclaredInCatch_IsVisibleAfterIt()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "try { b = a; } catch (e) { var x = e; } b = x;",
                "var x, e;\r\ntry {\r\n  b = a;\r\n} catch (e) {\r\n  x = e;\r\n}\r\nb = x;",
                "b", "a");
        }

        [TestMethod]
        public void Parse_ForWithVar()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var n = 0; for (var i = 0; i < a; i = i + 1) { n = n + i; } b = n;",
                "var n, i;\r\nn = 0;\r\nfor (i = 0; i < a; i = i + 1)\r\n  n = n + i;\r\nb = n;",
                "b", "a");
        }

        [DataTestMethod]
        [DataRow("b = /a+/.test(a);", "b = /a+/.test(a);")]
        [DataRow("b = /[/]\\//g;", "b = /[/]\\//g;")]
        [DataRow("b = a.replace(/^\\s+/, '');", "b = a.replace(/^\\s+/, \"\");")]
        [DataRow("b = a / 2 / a;", "b = a / 2 / a;")]
        [DataRow("b = (a) / 2;", "b = a / 2;")]
        [DataRow("b = a[0] / 2;", "b = a[0] / 2;")]
        [DataRow("b = a++ / 2;", "b = a++ / 2;")]
        [DataRow("b = 'x' / 2;", "b = \"x\" / 2;")]
        [DataRow("b = this / 2;", "b = this / 2;")]
        [DataRow("b = @{[System.Web]System.Int::fooBar} / 2;", "b = \"System.Web__System.Int#fooBar\" / 2;")]
        [DataRow("b = !/a/.test(a);", "b = !/a/.test(a);")]
        [DataRow("b = a && /a/.test(a);", "b = a && /a/.test(a);")]
        [DataRow("b = a ? /a/ : /b/;", "b = a ? /a/ : /b/;")]
        [DataRow("/a/.test(a);", "/a/.test(a);")]
        public void Parse_RegexLiteralAndDivision(string body, string expected)
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(body, expected, "b", "a");
        }

        [TestMethod]
        public void Parse_ObjectLiteral_KeepsEveryProperty()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b = { key: a, code: a, ctrlKey: true };",
                "b = {\r\n  \"key\": a,\r\n  \"code\": a,\r\n  \"ctrlKey\": true\r\n};",
                "b", "a");
        }

        [TestMethod]
        public void Parse_UnsupportedStatement_NamesItAndTheLine()
        {
            var ex = Assert.ThrowsException<NotSupportedException>(
                () => JSParserAndGeneratorHelper.ParseAndGenerateTest("b = a;\n  do { b = a; } while (a);", string.Empty, "b", "a"));
            StringAssert.Contains(ex.Message, "'do' is not supported in [Script] bodies (line 2:");
        }

        [TestMethod]
        public void Parse_SyntaxError_NamesTheToken()
        {
            var ex = Assert.ThrowsException<ApplicationException>(
                () => JSParserAndGeneratorHelper.ParseAndGenerateTest("b = = ;", string.Empty, "b"));
            StringAssert.Contains(ex.Message, "syntax error near '");
            StringAssert.Contains(ex.Message, "at line 1:");
        }
    }
}
