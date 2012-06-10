//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for RegressionTests
    /// </summary>
    [TestFixture]
    public class RegressionTests
    {
        private const string TestClassNameStr = "FuncRegressions";
        private const string ResourceFileNamepace = @"Cs2JsC.CLR.Test.CstTests.Regressions.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(true, "IfElseInForBlock")]
        [Row(false, "IfElseInForBlockRetail")]
        [Timeout(8)]
        public void TestIfElseInForBlock(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".xml",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "IfElseInForBlock",
                    isDebug).RootBlock);
        }

        [Test]
        [Row(false, "NestedWhileLoops")]
        [Timeout(8)]
        public void TestNestedWhileLoops(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".xml",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "NestedWhileLoops",
                    isDebug).RootBlock);
        }

        [Test]
        [Row(false, "NestedWhileLoops2")]
        [Timeout(8)]
        public void TestNestedWhileLoops2(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".xml",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "NestedWhileLoops2",
                    isDebug).RootBlock);
        }

        [Test]
        [Row(false, "TwoConsicutivePreIncrements")]
        // [Timeout(8)]
        public void TestPrePostIncrement(bool isDebug, string fileName)
        {
            TestHelpers.Test(
                RegressionTests.ResourceFileNamepace + fileName + ".xml",
                TestHelpers.GetAST(
                    RegressionTests.TestClassNameStr,
                    "TwoConsicutivePreIncrements",
                    isDebug).RootBlock);
        }
    }
}
