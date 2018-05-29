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
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", true, ".ReturnConstInt.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", true, ".ReturnConstUInt.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", true, ".ReturnConstLong.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", true, ".ReturnConstLargeLong.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", true, ".ReturnIntArray.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", true, ".ReturnObjectArray.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", true, ".ReturnArrayElement.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", true, ".SetArrayElement.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", true, ".CheckType.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "CastType", true, ".CastType.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AsType", true, ".AsType.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", true, ".TestSimpleCompare.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", true, ".TestSimpleAndComparision.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", true, ".TestFourPartCondition.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", true, ".SimpleConditional.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", true, ".SetVariableSimple.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", true, ".PassVariableByRef.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", true, ".PassObjectFieldByRef.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", true, ".PassArrayElementByRef.csast")]
        [TestCase(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", true, ".AccessRefArgument.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                BasicStatementAstTestFixture.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
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