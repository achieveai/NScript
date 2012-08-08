using Gallio.Framework;
using MbUnit.Framework;

namespace Cs2JsC.CLR.Test.CstTests
{
    [TestFixture]
    public class WhileLoopBlockAstTests
    {
        private const string TestClassNameStr = "WhileLoopBlocks";
        private const string ResourceFileNamepace = @"Cs2JsC.CLR.Test.CstTests.WhileLoopBlocks.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", true,  "DoWhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", false, "DoWhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", true,  "WhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", false, "WhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", true,  "DoWhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", false, "DoWhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", true,  "WhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", false, "WhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", true,  "WhileLoopWithBreak.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", false, "WhileLoopWithBreak.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", true,  "WhileLoopWithContinue.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", false, "WhileLoopWithContinueRelease.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", true,  "WhileInfiniteLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", false, "WhileInfiniteLoopRelease.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", true,  "WhileToForConfusionRegression2.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", false, "WhileToForConfusionRegression2.xml")]
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
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", true,  "DoWhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhileLoop", false, "DoWhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", true,  "WhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoop", false, "WhileLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", true,  "DoWhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "DoWhilePaddedLoop", false, "DoWhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", true,  "WhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhilePaddedLoop", false, "WhilePaddedLoop.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", true,  "WhileLoopWithBreak.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithBreak", false, "WhileLoopWithBreak.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", true,  "WhileLoopWithContinueMcs.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileLoopWithContinue", false, "WhileLoopWithContinueMcs.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", true,  "WhileInfiniteLoopRelease.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "WhileInfiniteLoop", false, "WhileInfiniteLoopRelease.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", true,  "WhileToForConfusionRegression2Mcs.xml")]
        [Row(WhileLoopBlockAstTests.TestClassNameStr, "TestWhileToForConfusionRegression2", false, "WhileToForConfusionRegression2Mcs.xml")]
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

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
