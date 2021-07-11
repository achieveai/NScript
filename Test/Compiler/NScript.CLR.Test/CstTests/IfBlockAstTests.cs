using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
namespace NScript.CLR.Test.CstTests
{
    [TestClass]
    public class IfBlockAstTests
    {
        private const string TestClassNameStr = "IfBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.IfBlocks";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(IfBlockAstTests.TestClassNameStr, "IfBlock", true,   ".IfBlock.csast")]
        [DataRow(IfBlockAstTests.TestClassNameStr, "IfElseBlock", true,   ".IfElseBlock.csast")]
        [DataRow(IfBlockAstTests.TestClassNameStr, "NestedIfElseBlock", true,   ".NestedIfElseBlock.csast")]
        [DataRow(IfBlockAstTests.TestClassNameStr, "IfNestedElseBlock", true,   ".IfNestedElseBlock.csast")]
        [DataRow(IfBlockAstTests.TestClassNameStr, "IfReturnBlock", true,   ".IfReturnBlock.csast")]
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
