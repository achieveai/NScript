namespace Cs2JsC.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

    [TestFixture]
    public class IfBlockAstTests
    {
        private const string TestClassNameStr = "IfBlocks";
        private const string ResourceFileNamepace = @"Cs2JsC.CLR.Test.CstTests.IfBlocks";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(IfBlockAstTests.TestClassNameStr, "IfBlock", true,   ".IfBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfBlock", false,  ".IfBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfElseBlock", true,   ".IfElseBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfElseBlock", false,  ".IfElseBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", true,   ".NestedIfElseBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", false,  ".NestedIfElseBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", true,   ".IfNestedElseBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", false,  ".IfNestedElseBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", true,   ".IfReturnBlock.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", false,  ".IfReturnBlockRelease.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                IfBlockAstTests.ResourceFileNamepace + resourceName,
                TestHelpers.GetAST(testClassName, methodName, isDebug).RootBlock);
        }

        [Test]
        [Row(IfBlockAstTests.TestClassNameStr, "IfBlock", true,   ".IfBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfBlock", false,  ".IfBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfElseBlock", true,   ".IfElseBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfElseBlock", false,  ".IfElseBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", true,   ".NestedIfElseBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", false,  ".NestedIfElseBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", true,   ".IfNestedElseBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", false,  ".IfNestedElseBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", true,   ".IfReturnBlockMcs.xml")]
        [Row(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", false,  ".IfReturnBlockMcs.xml")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                IfBlockAstTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        [Timeout(2000)]
        public void TestMaker()
        {
            var rootBlock = TestHelpers.GetAST(
                IfBlockAstTests.TestClassNameStr,
                "NestedIfElseBlock",
                true).RootBlock;

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
