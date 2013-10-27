namespace NScript.CLR.Test.CstTests
{
    using NScript.CLR.AST;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class BasicStatementAstTestFixture
    {
        private const string TestClassNameStr = "BasicStatements";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.BasicStatements";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", false, ".ReturnConstInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", true, ".ReturnConstInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", false, ".ReturnConstUInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", true, ".ReturnConstUInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", false, ".ReturnConstLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", true, ".ReturnConstLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", false, ".ReturnConstLargeLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", true, ".ReturnConstLargeLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", false, ".ReturnIntArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", true, ".ReturnIntArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", false, ".ReturnObjectArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", true, ".ReturnObjectArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", false, ".ReturnArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", true, ".ReturnArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", false, ".SetArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", true, ".SetArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", false, ".CheckType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", true, ".CheckType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CastType", false, ".CastType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CastType", true, ".CastType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AsType", false, ".AsType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AsType", true, ".AsType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", false, ".TestSimpleCompare.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", true, ".TestSimpleCompare.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", false, ".TestSimpleAndComparision.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", true, ".TestSimpleAndComparision.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", false, ".TestFourPartCondition.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", true, ".TestFourPartCondition.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", false, ".SimpleConditional.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", true, ".SimpleConditional.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", false, ".SetVariableSimple.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", true, ".SetVariableSimple.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", false, ".PassVariableByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", true, ".PassVariableByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", false, ".PassObjectFieldByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", true, ".PassObjectFieldByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", false, ".PassArrayElementByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", true, ".PassArrayElementByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", false, ".AccessRefArgument.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", true, ".AccessRefArgument.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                BasicStatementAstTestFixture.ResourceFileNamepace + resourceName,
                TestHelpers.GetAST(testClassName, methodName, isDebug).RootBlock);
        }

        [Test]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", false, ".ReturnConstInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", true, ".ReturnConstInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", false, ".ReturnConstUInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", true, ".ReturnConstUInt.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", false, ".ReturnConstLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", true, ".ReturnConstLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", false, ".ReturnConstLargeLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", true, ".ReturnConstLargeLong.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", false, ".ReturnIntArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", true, ".ReturnIntArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", false, ".ReturnObjectArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", true, ".ReturnObjectArray.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", false, ".ReturnArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", true, ".ReturnArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", false, ".SetArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", true, ".SetArrayElement.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", false, ".CheckType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", true, ".CheckType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CastType", false, ".CastType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CastType", true, ".CastType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AsType", false, ".AsType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AsType", true, ".AsType.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", false, ".TestSimpleCompare.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", true, ".TestSimpleCompare.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", false, ".TestSimpleAndComparision.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", true, ".TestSimpleAndComparision.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", false, ".TestFourPartCondition.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", true, ".TestFourPartCondition.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", false, ".SimpleConditional.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", true, ".SimpleConditional.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", false, ".SetVariableSimple.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", true, ".SetVariableSimple.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", false, ".PassVariableByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", true, ".PassVariableByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", false, ".PassObjectFieldByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", true, ".PassObjectFieldByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", false, ".PassArrayElementByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", true, ".PassArrayElementByRef.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", false, ".AccessRefArgumentMcs.xml")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", true, ".AccessRefArgumentMcs.xml")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                BasicStatementAstTestFixture.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        [Test]
        public void TestReturnConstLargeUInt()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "ReturnLargeUInt"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<UIntLiteral>(returnExpression);

                UIntLiteral expression = (UIntLiteral)returnExpression;
                Assert.AreEqual(0xC089A310, (uint)expression.Value);
                Assert.AreEqual(IntSize.I32, expression.Size);
            }
        }

        [Test]
        public void TestReturnConstSingle()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "ReturnSingle"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<DoubleLiteral>(returnExpression);

                DoubleLiteral expression = (DoubleLiteral)returnExpression;
                Assert.AreEqual(10.0, expression.Value);
                Assert.AreEqual(true, expression.IsSingle);
            }
        }

        [Test]
        public void TestReturnConstDouble()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "ReturnDouble"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<DoubleLiteral>(returnExpression);

                DoubleLiteral expression = (DoubleLiteral)returnExpression;
                Assert.AreEqual(10.0, expression.Value);
                Assert.AreEqual(false, expression.IsSingle);
            }
        }

        [Test]
        public void TestAddIntArgs()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "AddIntArgs"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<BinaryExpression>(returnExpression);

                BinaryExpression expression = (BinaryExpression)returnExpression;
                Assert.AreEqual(BinaryOperator.Plus, expression.Operator);
                Assert.IsInstanceOf<VariableReference>(expression.Left);
                Assert.IsInstanceOf<VariableReference>(expression.Right);

                VariableReference varRef = (VariableReference)expression.Left;
                Assert.AreEqual("n1", varRef.Variable.Name);

                varRef = (VariableReference)expression.Right;
                Assert.AreEqual("n2", varRef.Variable.Name);
            }
        }

        [Test]
        public void TestAddIntArgToConst()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "AddIntArgToConst"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<BinaryExpression>(returnExpression);

                BinaryExpression expression = (BinaryExpression)returnExpression;
                Assert.AreEqual(BinaryOperator.Plus, expression.Operator);
                Assert.IsInstanceOf<VariableReference>(expression.Left);
                Assert.IsInstanceOf<IntLiteral>(expression.Right);

                VariableReference varRef = (VariableReference)expression.Left;
                Assert.AreEqual("n1", varRef.Variable.Name);
            }
        }

        [Test]
        public void TestReturnMethodCall()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "ReturnMethodCall"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<MethodCallExpression>(returnExpression);

                MethodCallExpression expression = (MethodCallExpression)returnExpression;
                Assert.AreEqual(0, expression.Parameters.Count);

                Assert.IsInstanceOf<MethodReferenceExpression>(expression.MethodReference);

                MethodReferenceExpression methodReferenceExpression = ((MethodReferenceExpression)expression.MethodReference);
                Assert.AreEqual("ReturnInt", methodReferenceExpression.MethodReference.Name);
                Assert.IsNull(methodReferenceExpression.LeftExpression);
            }
        }

        [Test]
        public void TestReturnMethodCallWith1Arg()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "ReturnMethodCallWith1Arg"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<MethodCallExpression>(returnExpression);

                MethodCallExpression expression = (MethodCallExpression)returnExpression;
                Assert.AreEqual(1, expression.Parameters.Count);

                Assert.IsInstanceOf<MethodReferenceExpression>(expression.MethodReference);

                MethodReferenceExpression methodReferenceExpression = ((MethodReferenceExpression)expression.MethodReference);
                Assert.AreEqual("AddIntArgToConst", methodReferenceExpression.MethodReference.Name);
                Assert.IsNull(methodReferenceExpression.LeftExpression);
            }
        }

        [Test]
        public void TestReturnNewObject()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "ReturnNewObject"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<NewObjectExpression>(returnExpression);

                NewObjectExpression expression = (NewObjectExpression)returnExpression;
                Assert.IsInstanceOf<ConstructorReferenceExpression>(expression.MethodReference);
            }
        }

        [Test]
        public void TestReturnNewObjectWithArg()
        {
            foreach (TopLevelBlock topLevelBlock in TestHelpers.GetAST("BasicStatements", "ReturnNewObjectWithArg"))
            {
                Assert.AreNotEqual(null, topLevelBlock.RootBlock);
                Assert.AreEqual(0, topLevelBlock.RootBlock.UsedVariableCount);
                Assert.AreEqual(1, topLevelBlock.RootBlock.Statements.Count);
                Assert.IsInstanceOf<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOf<NewObjectExpression>(returnExpression);

                NewObjectExpression expression = (NewObjectExpression)returnExpression;
                Assert.IsInstanceOf<ConstructorReferenceExpression>(expression.MethodReference);
                Assert.AreEqual(1, expression.Parameters.Count);
            }
        }

        // [Test]
        public void TestMaker()
        {
            var rootBlock = TestHelpers.GetAST(
                "ClassInitTest",
                "ToString",
                false).RootBlock;

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}