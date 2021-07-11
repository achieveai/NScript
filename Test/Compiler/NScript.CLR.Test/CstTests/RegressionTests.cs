//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for RegressionTests
    /// </summary>
    [TestClass]
    public class RegressionTests
    {
        private const string TestClassNameStr = "FuncRegressions";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.Regressions.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(true, "IfElseInForBlock")]
        public void TestIfElseInForBlock(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".csast",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "IfElseInForBlock",
                    isDebug).RootBlock);
        }

        [DataTestMethod]
        [DataRow(false, "NestedWhileLoops")]
        public void TestNestedWhileLoops(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".csast",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "NestedWhileLoops",
                    isDebug).RootBlock);
        }

        [DataTestMethod]
        [DataRow(false, "NestedWhileLoops2")]
        public void TestNestedWhileLoops2(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".csast",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "NestedWhileLoops2",
                    isDebug).RootBlock);
        }

        [DataTestMethod]
        [DataRow(false, "TwoConsicutivePreIncrements")]
        // [Timeout(8)]
        public void TestPrePostIncrement(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".csast",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "TwoConsicutivePreIncrements",
                    isDebug).RootBlock);
        }
    }
}
