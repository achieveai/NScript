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
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", true,  "DoWhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", false, "DoWhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", true,  "WhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", false, "WhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", true,  "DoWhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", false, "DoWhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", true,  "WhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", false, "WhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", true,  "WhileLoopWithBreak.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", false, "WhileLoopWithBreak.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", true,  "WhileLoopWithContinue.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", false, "WhileLoopWithContinueRelease.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", true,  "WhileInfiniteLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", false, "WhileInfiniteLoopRelease.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", true,  "WhileToForConfusionRegression2.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", false, "WhileToForConfusionRegression2.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                WhileLoopBlockAstTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }

        [Test]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", true,  "DoWhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", false, "DoWhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", true,  "WhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", false, "WhileLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", true,  "DoWhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", false, "DoWhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", true,  "WhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", false, "WhilePaddedLoop.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", true,  "WhileLoopWithBreak.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", false, "WhileLoopWithBreak.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", true,  "WhileLoopWithContinueMcs.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", false, "WhileLoopWithContinueMcs.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", true,  "WhileInfiniteLoopRelease.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", false, "WhileInfiniteLoopRelease.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", true,  "WhileToForConfusionRegression2Mcs.xml")]
        [TestCase(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", false, "WhileToForConfusionRegression2Mcs.xml")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                WhileLoopBlockAstTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        [Test]
        [Timeout(5000)]
        public void TestMaker()
        {
            var rootBlock = TestHelpers.GetAST(
                    WhileLoopBlockAstTests.TestClassNameStr,
                    "WhileInfiniteLoop",
                    true).RootBlock;

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
