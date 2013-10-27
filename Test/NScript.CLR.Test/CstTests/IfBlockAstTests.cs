using NUnit.Framework;
using System.Diagnostics;
namespace NScript.CLR.Test.CstTests
{
    [TestFixture]
    public class IfBlockAstTests
    {
        private const string TestClassNameStr = "IfBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.IfBlocks";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfBlock", true,   ".IfBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfBlock", false,  ".IfBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfElseBlock", true,   ".IfElseBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfElseBlock", false,  ".IfElseBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", true,   ".NestedIfElseBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", false,  ".NestedIfElseBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", true,   ".IfNestedElseBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", false,  ".IfNestedElseBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", true,   ".IfReturnBlock.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", false,  ".IfReturnBlockRelease.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                IfBlockAstTests.ResourceFileNamepace + resourceName,
                TestHelpers.GetAST(testClassName, methodName, isDebug).RootBlock);
        }

        [Test]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfBlock", true,   ".IfBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfBlock", false,  ".IfBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfElseBlock", true,   ".IfElseBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfElseBlock", false,  ".IfElseBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", true,   ".NestedIfElseBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", false,  ".NestedIfElseBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", true,   ".IfNestedElseBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", false,  ".IfNestedElseBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", true,   ".IfReturnBlockMcs.xml")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", false,  ".IfReturnBlockMcs.xml")]
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

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
