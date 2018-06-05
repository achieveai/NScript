//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using NUnit.Framework;

    /// <summary>
    /// Definition for RegressionTests
    /// </summary>
    [TestFixture]
    public class RegressionTests
    {
        private const string TestClassNameStr = "FuncRegressions";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.Regressions.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(true, "IfElseInForBlock")]
        public void TestIfElseInForBlock(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".csast",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "IfElseInForBlock",
                    isDebug).RootBlock);
        }

        [Test]
        [TestCase(false, "NestedWhileLoops")]
        public void TestNestedWhileLoops(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".csast",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "NestedWhileLoops",
                    isDebug).RootBlock);
        }

        [Test]
        [TestCase(false, "NestedWhileLoops2")]
        public void TestNestedWhileLoops2(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".csast",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "NestedWhileLoops2",
                    isDebug).RootBlock);
        }

        [Test]
        [TestCase(false, "TwoConsicutivePreIncrements")]
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
