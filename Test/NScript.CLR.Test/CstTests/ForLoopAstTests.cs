//-----------------------------------------------------------------------
// <copyright file="ForLoopAstTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for ForLoopAstTests
    /// </summary>
    [TestFixture]
    public class ForLoopAstTests
    {
        private const string TestClassNameStr = "ForLoopBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.ForLoopBlocks.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(ForLoopAstTests.TestClassNameStr, "ForEachLoop", true,  "ForEachLoop.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForEachLoop", false, "ForEachLoop.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", true,  "ForLoopPadded.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", false, "ForLoopPadded.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", true,  "ForLoopBasic.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", false, "ForLoopBasic.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                ForLoopAstTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }

        [Test]
        [Row(ForLoopAstTests.TestClassNameStr, "ForEachLoop", true,  "ForEachLoop.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForEachLoop", false, "ForEachLoop.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", true,  "ForLoopPadded.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", false, "ForLoopPadded.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", true,  "ForLoopBasic.xml")]
        [Row(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", false, "ForLoopBasic.xml")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                ForLoopAstTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }

        public void TestMaker()
        {
            var rootBlock = TestHelpers.GetAST(
                TestClassNameStr,
                "ForEachLoop",
                true).RootBlock;

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
