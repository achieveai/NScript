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
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForEachLoop", true,  "ForEachLoop.csast")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", true,  "ForLoopPadded.csast")]
        [TestCase(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", true,  "ForLoopBasic.csast")]
        public void TestMcs(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                ForLoopAstTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                true);
        }
    }
}
