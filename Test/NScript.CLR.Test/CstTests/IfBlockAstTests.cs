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
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfBlock", true,   ".IfBlock.csast")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfElseBlock", true,   ".IfElseBlock.csast")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", true,   ".NestedIfElseBlock.csast")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", true,   ".IfNestedElseBlock.csast")]
        [TestCase(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", true,   ".IfReturnBlock.csast")]
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
