using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
namespace NScript.CLR.Test.CstTests
{
    [TestClass]
    public class WhileLoopBlockAstTests
    {
        private const string TestClassNameStr = "WhileLoopBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.WhileLoopBlocks.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", true,  "DoWhileLoop.csast")]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", true,  "WhileLoop.csast")]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", true,  "DoWhilePaddedLoop.csast")]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", true,  "WhilePaddedLoop.csast")]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", true,  "WhileLoopWithBreak.csast")]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", true,  "WhileLoopWithContinue.csast")]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", true,  "WhileInfiniteLoop.csast")]
        [DataRow(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", true,  "WhileToForConfusionRegression2.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                WhileLoopBlockAstTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }
    }
}
