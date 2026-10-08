//-----------------------------------------------------------------------
// <copyright file="ScriptBodyLiteralTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// [Script] bodies are parsed into JST and written back out. A body that holds only a
    /// comment must parse, and a string literal must keep its value: the parser decodes its
    /// escapes once, the writer encodes them once.
    /// </summary>
    [TestClass]
    public class ScriptBodyLiteralTests
    {
        [DataTestMethod]
        [DataRow("/* only a comment */")]
        [DataRow("// only a comment")]
        [DataRow("  \r\n  ")]
        public void Parse_BodyWithNoStatements_WritesNothing(string body)
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(body, string.Empty);
        }

        [TestMethod]
        public void Parse_CommentsBetweenStatements_AreDropped()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b = a; /* block */ a = b; // line",
                "b = a;\r\na = b;",
                "b", "a");
        }

        [TestMethod]
        public void Parse_CommentInsideExpression_IsDropped()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "/* lead */ b = a /* mid */ + a;\r\n// tail",
                "b = a + a;",
                "b", "a");
        }

        [TestMethod]
        public void Parse_SyntaxError_StillThrows()
        {
            Assert.ThrowsException<System.ApplicationException>(
                () => JSParserAndGeneratorHelper.ParseAndGenerateTest("b = = ;", string.Empty, "b"));
        }

        [DataTestMethod]
        [DataRow(@"b = '\n';", @"b = ""\n"";")]
        [DataRow(@"b = 'a\r\nb';", @"b = ""a\r\nb"";")]
        [DataRow(@"b = '\\';", @"b = ""\\"";")]
        [DataRow(@"b = 'it\'s';", @"b = ""it's"";")]
        [DataRow(@"b = ""say \""hi\"""";", @"b = ""say \""hi\"""";")]
        [DataRow(@"b = '\" + "u0041" + @"\x42\C';", @"b = ""ABC"";")]
        [DataRow(@"b = 'plain';", @"b = ""plain"";")]
        public void Parse_StringLiteralEscapes_AreEncodedOnce(string body, string expected)
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(body, expected, "b");
        }
    }
}
