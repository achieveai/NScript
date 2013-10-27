//-----------------------------------------------------------------------
// <copyright file="ForLoopAstTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using NUnit.Framework;
    using System.Diagnostics;

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
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForEachLoop", true,  "ForEachLoop.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForEachLoop", false, "ForEachLoop.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", true,  "ForLoopPadded.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", false, "ForLoopPadded.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", true,  "ForLoopBasic.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", false, "ForLoopBasic.xml")]
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
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForEachLoop", true,  "ForEachLoop.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForEachLoop", false, "ForEachLoop.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", true,  "ForLoopPadded.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", false, "ForLoopPadded.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", true,  "ForLoopBasic.xml")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", false, "ForLoopBasic.xml")]
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

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
