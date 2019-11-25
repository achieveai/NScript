using JsCsc.Lib.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScript.Csc.Lib.Test
{
    [TestFixture]
    public class TestClass
    {
        const string code = @"
using System;
public static class TestClass {
    public static void TestWriteLine(string str, int i) {
        Console.WriteLine(str, i);
    }
}";

        private Dictionary<IMethodSymbol, MethodBody> compilationResults;

        [SetUp]
        public void Setup()
        {
            var tree = CSharpSyntaxTree.ParseText(
                code,
                path: "testCode.cs");

            var mscorlib = MetadataReference.CreateFromFile(
                typeof(object).Assembly.Location);

            var compilation = CSharpCompilation.Create(
                "TestCompilation",
                syntaxTrees: new[] { tree },
                references: new[] { mscorlib });

            var model = compilation.GetSemanticModel(tree, true);

            compilationResults = SerializationHelper.ExpressionVisitMap(
                compilation,
                System.IO.Path.GetTempPath(),
                "testcode");
        }

        [Test]
        public void TestWriteLine()
        {
            var body = this.GetVisitMap("TestWriteLine");

            Assert.AreEqual(
                1,
                body.Body.Statements.Count);

            Assert.IsAssignableFrom<StatementExpressionSer>(
                body.Body.Statements[0]);
        }

        private MethodBody GetVisitMap(
            string methodName,
            int parameterCount = -1)
        {
            return compilationResults
                .Where(_ => _.Key.Name == methodName)
                .Where(_ => parameterCount == -1
                || _.Key.Parameters.Length == parameterCount)
                .Select(_ => _.Value)
                .First();
        }
    }
}
