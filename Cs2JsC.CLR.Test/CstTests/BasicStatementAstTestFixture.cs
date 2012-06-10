namespace Cs2JsC.CLR.Test.CstTests
{
    using Cs2JsC.CLR.AST;
    using Gallio.Framework;
    using MbUnit.Framework;

    [TestFixture]
    public class BasicStatementAstTestFixture
    {
        private const string TestClassNameStr = "BasicStatements";
        private const string ResourceFileNamepace = @"Cs2JsC.CLR.Test.CstTests.BasicStatements";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", false, ".ReturnConstInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", true, ".ReturnConstInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", false, ".ReturnConstUInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", true, ".ReturnConstUInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", false, ".ReturnConstLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", true, ".ReturnConstLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", false, ".ReturnConstLargeLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", true, ".ReturnConstLargeLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", false, ".ReturnIntArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", true, ".ReturnIntArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", false, ".ReturnObjectArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", true, ".ReturnObjectArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", false, ".ReturnArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", true, ".ReturnArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", false, ".SetArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", true, ".SetArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", false, ".CheckType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", true, ".CheckType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CastType", false, ".CastType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CastType", true, ".CastType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AsType", false, ".AsType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AsType", true, ".AsType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", false, ".TestSimpleCompare.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", true, ".TestSimpleCompare.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", false, ".TestSimpleAndComparision.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", true, ".TestSimpleAndComparision.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", false, ".TestFourPartCondition.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", true, ".TestFourPartCondition.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", false, ".SimpleConditional.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", true, ".SimpleConditional.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", false, ".SetVariableSimple.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", true, ".SetVariableSimple.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", false, ".PassVariableByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", true, ".PassVariableByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", false, ".PassObjectFieldByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", true, ".PassObjectFieldByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", false, ".PassArrayElementByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", true, ".PassArrayElementByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", false, ".AccessRefArgument.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", true, ".AccessRefArgument.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                BasicStatementAstTestFixture.ResourceFileNamepace + resourceName,
                TestHelpers.GetAST(testClassName, methodName, isDebug).RootBlock);
        }

        [Test]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", false, ".ReturnConstInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", true, ".ReturnConstInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", false, ".ReturnConstUInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", true, ".ReturnConstUInt.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", false, ".ReturnConstLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", true, ".ReturnConstLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", false, ".ReturnConstLargeLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", true, ".ReturnConstLargeLong.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", false, ".ReturnIntArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", true, ".ReturnIntArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", false, ".ReturnObjectArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", true, ".ReturnObjectArray.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", false, ".ReturnArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", true, ".ReturnArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", false, ".SetArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", true, ".SetArrayElement.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", false, ".CheckType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", true, ".CheckType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CastType", false, ".CastType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "CastType", true, ".CastType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AsType", false, ".AsType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AsType", true, ".AsType.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", false, ".TestSimpleCompare.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", true, ".TestSimpleCompare.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", false, ".TestSimpleAndComparision.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", true, ".TestSimpleAndComparision.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", false, ".TestFourPartCondition.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", true, ".TestFourPartCondition.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", false, ".SimpleConditional.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", true, ".SimpleConditional.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", false, ".SetVariableSimple.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", true, ".SetVariableSimple.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", false, ".PassVariableByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", true, ".PassVariableByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", false, ".PassObjectFieldByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", true, ".PassObjectFieldByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", false, ".PassArrayElementByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", true, ".PassArrayElementByRef.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", false, ".AccessRefArgumentMcs.xml")]
        [Row(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", true, ".AccessRefArgumentMcs.xml")]
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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<UIntLiteral>(returnExpression);

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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<DoubleLiteral>(returnExpression);

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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<DoubleLiteral>(returnExpression);

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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<BinaryExpression>(returnExpression);

                BinaryExpression expression = (BinaryExpression)returnExpression;
                Assert.AreEqual(BinaryOperator.Plus, expression.Operator);
                Assert.IsInstanceOfType<VariableReference>(expression.Left);
                Assert.IsInstanceOfType<VariableReference>(expression.Right);

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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<BinaryExpression>(returnExpression);

                BinaryExpression expression = (BinaryExpression)returnExpression;
                Assert.AreEqual(BinaryOperator.Plus, expression.Operator);
                Assert.IsInstanceOfType<VariableReference>(expression.Left);
                Assert.IsInstanceOfType<IntLiteral>(expression.Right);

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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<MethodCallExpression>(returnExpression);

                MethodCallExpression expression = (MethodCallExpression)returnExpression;
                Assert.AreEqual(0, expression.Parameters.Count);

                Assert.IsInstanceOfType<MethodReferenceExpression>(expression.MethodReference);

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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<MethodCallExpression>(returnExpression);

                MethodCallExpression expression = (MethodCallExpression)returnExpression;
                Assert.AreEqual(1, expression.Parameters.Count);

                Assert.IsInstanceOfType<MethodReferenceExpression>(expression.MethodReference);

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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<NewObjectExpression>(returnExpression);

                NewObjectExpression expression = (NewObjectExpression)returnExpression;
                Assert.IsInstanceOfType<ConstructorReferenceExpression>(expression.MethodReference);
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
                Assert.IsInstanceOfType<ReturnStatement>(topLevelBlock.RootBlock.Statements[0]);

                Expression returnExpression = ((ReturnStatement)topLevelBlock.RootBlock.Statements[0]).ReturnExpression;

                Assert.AreNotEqual(null, returnExpression);
                Assert.IsInstanceOfType<NewObjectExpression>(returnExpression);

                NewObjectExpression expression = (NewObjectExpression)returnExpression;
                Assert.IsInstanceOfType<ConstructorReferenceExpression>(expression.MethodReference);
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

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}