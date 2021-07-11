//-----------------------------------------------------------------------
// <copyright file="ForLoopAstTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    /// <summary>
    /// Definition for ForLoopAstTests
    /// </summary>
    [TestClass]
    public class ForLoopAstTests
    {
        private const string TestClassNameStr = "ForLoopBlocks";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.ForLoopBlocks.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(ForLoopAstTests.TestClassNameStr, "ForLoopBasic", true,  "ForLoopBasic.csast")]
        [DataRow(ForLoopAstTests.TestClassNameStr, "ForLoopWithContinue", true,  "ForLoopContinue.csast")]
        [DataRow(ForLoopAstTests.TestClassNameStr, "ForLoopWithBreak", true,  "ForLoopBreak.csast")]
        [DataRow(ForLoopAstTests.TestClassNameStr, "ForLoopPadded", true,  "ForLoopPadded.csast")]
        [DataRow(ForLoopAstTests.TestClassNameStr, "ForEachLoop", true,  "ForEachLoop.csast")]
        [DataRow(ForLoopAstTests.TestClassNameStr, "ForEachLoopArr", true,  "ForEachLoopArr.csast")]
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
