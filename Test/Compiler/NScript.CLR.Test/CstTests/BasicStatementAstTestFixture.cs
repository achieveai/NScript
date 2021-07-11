namespace NScript.CLR.Test.CstTests
{
    using NScript.CLR.AST;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    [TestClass]
    public class BasicStatementAstTestFixture
    {
        private const string TestClassNameStr = "BasicStatements";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.BasicStatements";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "ReturnInt", true, ".ReturnConstInt.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "ReturnUInt", true, ".ReturnConstUInt.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLong", true, ".ReturnConstLong.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "ReturnLargeLong", true, ".ReturnConstLargeLong.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "ReturnIntArray", true, ".ReturnIntArray.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "ReturnObjectArray", true, ".ReturnObjectArray.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "ReturnArrayElement", true, ".ReturnArrayElement.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "SetArrayElement", true, ".SetArrayElement.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "CheckType", true, ".CheckType.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "CastType", true, ".CastType.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "AsType", true, ".AsType.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleCompare", true, ".TestSimpleCompare.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleAndComparision", true, ".TestSimpleAndComparision.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "TestFourPartCondition", true, ".TestFourPartCondition.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "TestSimpleConditional", true, ".SimpleConditional.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "SetVariableSimple", true, ".SetVariableSimple.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "PassVariableByRef", true, ".PassVariableByRef.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "PassObjectFieldByRef", true, ".PassObjectFieldByRef.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "PassArrayElementByRef", true, ".PassArrayElementByRef.csast")]
        [DataRow(BasicStatementAstTestFixture.TestClassNameStr, "AccessRefArgument", true, ".AccessRefArgument.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                BasicStatementAstTestFixture.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        // [DataTestMethod]
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