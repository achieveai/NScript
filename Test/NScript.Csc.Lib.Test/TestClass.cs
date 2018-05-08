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
        [Test]
        public void TestWriteLine()
        {
            const string code = @"
class Test {
    public void TestMethod(System.Text.StringBuilder sb, int i)
    {
        sb.Append(i);
    }
}";
            var visitMap = GetVisitMap(code);
            // TODO: Add your test code here
            Assert.AreEqual(
@"StatementList: SyntaxKind:MethodDeclaration
  Block: SyntaxKind:Block
    SequencePoint: SyntaxKind:ReturnStatement
      ReturnStatement: SyntaxKind:ReturnStatement
        BinaryOperator: SyntaxKind:LeftShiftExpression Operator: IntLeftShift
          BinaryOperator: SyntaxKind:AddExpression Operator: IntAddition
            Parameter: SyntaxKind:IdentifierName, ParameterRef: int
            Literal: SyntaxKind:NumericLiteralExpression Val: 10
          Literal: SyntaxKind:NumericLiteralExpression Val: 1
StatementList: SyntaxKind:ClassDeclaration
  Block: SyntaxKind:ClassDeclaration
    ExpressionStatement: SyntaxKind:ClassDeclaration
      Call: SyntaxKind:ClassDeclaration
        object.Object(), IsVirt:False Start Args
    ReturnStatement: SyntaxKind:ClassDeclaration",
            visitMap);
        }

        [Test]
        public void TestSimpleBinaryExpression()
        {
            const string code = @"
class Test {
    public int TestMethod(int i)
    {
        return i + 10 << 1;
    }
}";
            var visitMap = GetVisitMap(code);
            // TODO: Add your test code here
            Assert.AreEqual(
@"Block: SyntaxKind:Block
  ReturnStatement: SyntaxKind:ReturnStatement
    BinaryOperator: SyntaxKind:LeftShiftExpression Operator: IntLeftShift
      BinaryOperator: SyntaxKind:AddExpression Operator: IntAddition
        Parameter: SyntaxKind:IdentifierName, ParameterRef: int
        Literal: SyntaxKind:NumericLiteralExpression Val: 10
      Literal: SyntaxKind:NumericLiteralExpression Val: 1
Block: SyntaxKind:ClassDeclaration
  ExpressionStatement: SyntaxKind:ClassDeclaration
    Call: SyntaxKind:ClassDeclaration
      object.Object(), IsVirt:False Start Args",
            visitMap);
        }

        [Test]
        public void TestStaticBinaryExpression()
        {
            const string code = @"
static class Test {
    public static int TestMethod(int i)
    {
        return i + 10 << 1;
    }
}";
            var result = @"
Block: SyntaxKind:Block
  ReturnStatement: SyntaxKind:ReturnStatement
    BinaryOperator: SyntaxKind:LeftShiftExpression Operator: IntLeftShift
      BinaryOperator: SyntaxKind:AddExpression Operator: IntAddition
        Parameter: SyntaxKind:IdentifierName, ParameterRef: int
        Literal: SyntaxKind:NumericLiteralExpression Val: 10
      Literal: SyntaxKind:NumericLiteralExpression Val: 1";
            var visitMap = GetVisitMap(code);
            // TODO: Add your test code here
            Assert.AreEqual(
                result,
                visitMap);
        }

        [Test]
        public void TestForloop()
        {
            const string code = @"
static class Test {
    public static int TestMethod(int i)
    {
        int sum = 0;
        for(int l = 0; l < i; ++i)
        {
            sum += l;
        }

        return sum;
    }
}";

            const string result = @"Block: SyntaxKind:Block
  LocalDeclaration: SyntaxKind:LocalDeclarationStatement Name: sum
    Literal: SyntaxKind:NumericLiteralExpression Val: 0
  ForStatement: SyntaxKind:ForStatement
    LocalDeclaration: SyntaxKind:VariableDeclarator Name: l
      Literal: SyntaxKind:NumericLiteralExpression Val: 0
    BinaryOperator: SyntaxKind:LessThanExpression Operator: IntLessThan
      Local: SyntaxKind:IdentifierName
      Parameter: SyntaxKind:IdentifierName, ParameterRef: int
    ExpressionStatement: SyntaxKind:PreIncrementExpression
      IncrementOperator: SyntaxKind:PreIncrementExpression
        Parameter: SyntaxKind:IdentifierName, ParameterRef: int
    Block: SyntaxKind:Block
      ExpressionStatement: SyntaxKind:ExpressionStatement
        CompoundAssignmentOperator: SyntaxKind:AddAssignmentExpression
          Local: SyntaxKind:IdentifierName
          Local: SyntaxKind:IdentifierName
  ReturnStatement: SyntaxKind:ReturnStatement
    Local: SyntaxKind:IdentifierName";

            var visitMap = GetVisitMap(code);

            Assert.AreEqual(
                result.Trim(),
                visitMap);
        }

        private static string GetVisitMap(string code)
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

            return SerializationHelper.ExpressionVisitMap(
                compilation,
                model).Trim();
        }
    }
}
