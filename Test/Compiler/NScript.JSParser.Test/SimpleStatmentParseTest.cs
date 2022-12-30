//-----------------------------------------------------------------------
// <copyright file="SimpleStatmentParseTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for SimpleStatmentParseTest
    /// </summary>
    [TestClass]
    public class SimpleStatmentParseTest
    {
        [TestMethod]
        public void TestAssignment()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b=a;",
                "b = a;",
                "b", "a");
        }

        [TestMethod]
        public void TestAssignAndAdd()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b=a + 10;",
                "b = a + 10;",
                "b", "a");
        }

        [TestMethod]
        public void TestConditional()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b=a ? 10 : 11;",
                "b = a ? 10 : 11;",
                "b", "a");
        }

        [TestMethod]
        public void TestReturn()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return a+b;",
                "return a + b;",
                "b", "a");
        }

        [TestMethod]
        public void EmptyReturn()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return ;",
                "return;",
                "b", "a");
        }

        [TestMethod]
        public void IfStatement()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "if (a) return b; return b+10;",
                "if (a)\r\n  return b;\r\nreturn b + 10;",
                "b", "a");
        }

        [TestMethod]
        public void IfElseStatement()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "if (a) return b; else return b+10;",
                "if (a)\r\n  return b;\r\nelse\r\n  return b + 10;",
                "b", "a");
        }

        [TestMethod]
        public void IfEmptyStatement()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "if (a); return b+10;",
                "if (a);\r\nreturn b + 10;",
                "b", "a");
        }

        [TestMethod]
        public void TestIndexer()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "a.b=a[b];",
                "a.b = a[b];",
                "b", "a");
        }

        [TestMethod]
        public void TestLogicalNot()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b=!a;",
                "b = !a;",
                "b", "a");
        }

        [TestMethod]
        public void TestNewObject()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b= new a();",
                "b = new a();",
                "b", "a");
        }

        [TestMethod]
        public void TestIndexerWithExpr()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "a.b=a[b+a];",
                "a.b = a[b + a];",
                "b", "a");
        }

        [TestMethod]
        public void TestMethodCallWithArgs()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "a.b(a,b);",
                "a.b(a, b);",
                "b", "a");
        }

        [TestMethod]
        public void TestMethodCallWithoutArgs()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b();",
                "b();",
                "b", "a");
        }

        [TestMethod]
        public void TestVars()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var c;c=b;",
                "var c;\r\nc = b;",
                "b", "a");
        }

        [TestMethod]
        public void TestMultipleVars()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var c,d;c=b;d=b+a;",
                "var c, d;\r\nc = b;\r\nd = b + a;",
                "b", "a");
        }

        [TestMethod]
        public void TestVarWithInit()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var c=b,d=a+b;b=d;",
                "var c, d;\r\nc = b, d = a + b;\r\nb = d;",
                "b", "a");
        }

        [TestMethod]
        public void TestImportTypeReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return @{[System.Web]System.Int};",
                "return \"System.Web__System.Int\";",
                "b", "a");
        }

        [TestMethod]
        public void TestImportVariableReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return @{[System.Web]System.Int::fooBar};",
                "return \"System.Web__System.Int#fooBar\";",
                "b", "a");
        }

        [TestMethod]
        public void TestImportMethod0ArgReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return @{[System.Web]System.Int::fooBar()};",
                 "return \"System.Web__System.Int#fooBar(args:0)\";",
                "b", "a");
        }

        [TestMethod]
        public void TestImportMethod1ArgReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return @{[System.Web]System.Int::fooBar([mscorlib]System.String)};",
                 "return \"System.Web__System.Int#fooBar(args:1)\";",
                "b", "a");
        }

        [TestMethod]
        public void TestImportMethodMoreArgsReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return @{[System.Web]System.Int::fooBar([mscorlib]System.Int, [mscorlib]System.String)};",
                 "return \"System.Web__System.Int#fooBar(args:2)\";",
                "b", "a");
        }

        [TestMethod]
        public void TestImportInstanceVariableReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return a.@{[System.Web]System.Int::fooBar};",
                "return a[\"instance System.Web__System.Int#fooBar\"];",
                "b", "a");
        }

        [TestMethod]
        public void TestImportInstanceMethod0ArgReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return a.@{[System.Web]System.Int::fooBar()};",
                 "return a[\"instance System.Web__System.Int#fooBar(args:0)\"];",
                "b", "a");
        }

        [TestMethod]
        public void TestImportInstanceMethod1ArgReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return a.@{[System.Web]System.Int::fooBar([mscorlib]System.String)};",
                 "return a[\"instance System.Web__System.Int#fooBar(args:1)\"];",
                "b", "a");
        }

        [TestMethod]
        public void TestImportInstanceMethodMoreArgsReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "return a.@{[System.Web]System.Int::fooBar([mscorlib]System.Int, [mscorlib]System.String)};",
                 "return a[\"instance System.Web__System.Int#fooBar(args:2)\"];",
                "b", "a");
        }

        [TestMethod]
        public void TestImportGenericTypeReference()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b.@{[mscorlib]System.InternalArrayImpl`1::innerArray}.apply(a,b);",
                "b[\"instance mscorlib__System.InternalArrayImpl`1#innerArray\"].apply(a, b);",
                "b", "a");
        }

        [TestMethod]
        public void TestEmptyInlinInitializer()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var rv = {};",
                "var rv;\r\nrv = {\r\n};",
                "b", "a");
        }

        [TestMethod]
        public void TestInlineObjInitializer()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var rv = {t: 'test', v: 'value'};",
                "var rv;\r\nrv = {\r\n  \"t\": \"test\",\r\n  \"v\": \"value\"\r\n};",
                "b", "a");
        }

        [TestMethod]
        public void TestIndexAssignment()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "b[key] = a[key];",
                "b[key] = a[key];",
                "b", "a", "key");
        }

        [TestMethod]
        public void TestForIn()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "for(var key in a) { b[key] = a[key]; }",
                "var key;\r\nfor (key in a)\r\n  b[key] = a[key];",
                "b", "a");
        }

        [TestMethod]
        public void TestPostFix()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "a = b++;",
                "a = b++;",
                "b", "a");
        }

        [TestMethod]
        public void TestThrowStatement()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "throw new a(a,b);",
                "throw new a(a, b);",
                "b", "a");
        }


        [TestMethod]
        public void TestForLoop()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var key; for(key = 0; key < 10; key++) { b[key] = a[key]; }",
                "var key;\r\nfor (key = 0; key < 10; key++)\r\n  b[key] = a[key];",
                "b", "a");
        }

        [TestMethod]
        public void TestFunction()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var key = function (a1, a2) { return a1 + b*a2;}; a = key();",
                "var key;\r\nkey = function(a1, a2) {\r\n    return a1 + b * a2;\r\n};\r\na = key();",
                "b", "a");
        }

        [TestMethod]
        public void TestRegressionShrPrecidence()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var key = (b << 15) + (a << 8) + c;",
                "var key;\r\nkey = (b << 15) + (a << 8) + c;",
                "b", "a", "c");
        }

        [TestMethod]
        public void TestFooBar()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                @"
            this.@{[mscorlib]System.Type::IsClass} = true;
            this.@{[mscorlib]System.Type::FullName} = typeName;
            this.@{[mscorlib]System.@Type::BaseType} = parentType;
            @{[mscorlib]System.Type::FixBaseTypes([mscorlib]System.Type)}(this);
                ",
                "var rv, baseType, types, key;\r\nrv = {\r\n};\r\nbaseType = type[\"instance mscorlib__System.Type#BaseType\"];\r\nif (baseType != null) {\r\n  types = baseType[\"instance mscorlib__System.Type#baseTypes\"];\r\n  key[baseType] = baseType;\r\n  for (key in types)\r\n    rv[key] = types[key];\r\n}\r\nif (interfaces != null) {\r\n  for (key = 0; key < interfaces.length; key++)\r\n    rv[interfaces[key]] = interfaces[key];\r\n  type[\"instance mscorlib__System.Type#BaseType\"] = rv;\r\n}",
                "baseType",
                "typeName", "parentType");
        }

        [TestMethod]
        public void TestReleaseWriter()
        {
            JSParserAndGeneratorHelper.ParseAndGenerateTest(
                "var a = 10, b = 11;",
                "var a,b;a=10,b=11;",
                true);
        }
    }
}
