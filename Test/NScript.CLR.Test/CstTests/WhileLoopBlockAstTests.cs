using NUnit.Framework;
using System.Diagnostics;
namespace NScript.CLR.Test.CstTests
{
    [TestFixture]
    public class WhileLoopBlockAstTests
    {
        private const string TestClassNameStr = "WhileLoopBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.WhileLoopBlocks.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", true,  "DoWhileLoop.csast")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", true,  "WhileLoop.csast")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", true,  "DoWhilePaddedLoop.csast")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", true,  "WhilePaddedLoop.csast")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", true,  "WhileLoopWithBreak.csast")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", true,  "WhileLoopWithContinue.csast")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", true,  "WhileInfiniteLoop.csast")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", true,  "WhileToForConfusionRegression2.csast")]
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
