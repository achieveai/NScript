//-----------------------------------------------------------------------
// <copyright file="SwitchStatementTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Test.CstTests
{
    using Gallio.Framework;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for SwitchStatementTests
    /// </summary>
    [TestFixture]
    public class SwitchStatementTests
    {
        private const string TestClassNameStr = "SwitchTest";
        private const string ResourceFileNamepace = @"Cs2JsC.CLR.Test.CstTests.SwitchStatement.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        private void TestCore(bool isDebug, string methodName, string fileName = null)
        {
            TestHelpers.Test(
                string.Format("{0}{1}.xml",
                    SwitchStatementTests.ResourceFileNamepace,
                    fileName ?? methodName),
                SwitchStatementTests.TestClassNameStr,
                methodName,
                isDebug,
                false);
        }

        [Test]
        [Row(SwitchStatementTests.TestClassNameStr, "SimpleIntSwitch", true,  "SimpleIntSwitch.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "SimpleIntSwitch", false, "SimpleIntSwitchRetail.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "SwitchOnlyFunction", true,  "SwitchOnlyFunction.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "SwitchOnlyFunction", false, "SwitchOnlyFunctionRetail.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "SwitchWithReturn", true,  "SwitchWithReturn.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "SwitchWithReturn", false, "SwitchWithReturnRetail.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "SwitchWithReturnsOnly", true,  "SwitchWithReturnsOnly.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "SwitchWithReturnsOnly", false, "SwitchWithReturnsOnlyRetail.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "RegressSwitchWithFor", true,  "RegressSwitchWithFor.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "RegressSwitchWithFor", false, "RegressSwitchWithForRetail.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "RegressionContinousSwitch2", true,  "RegressionContinousSwitch2.xml")]
        [Row(SwitchStatementTests.TestClassNameStr, "RegressionContinousSwitch2", false, "RegressionContinousSwitch2.xml")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                SwitchStatementTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }

        // [Test]
        [Timeout(6000)]
        public void TestMaker()
        {
            const string methodName = "RegressSwitchWithFor";

            AST.Node rootBlock;

            rootBlock = TestHelpers.GetAST(
                TestClassNameStr,
                methodName,
                true).RootBlock;

            TestLog.Write(TestHelpers.Serialize(rootBlock));
            TestLog.Write("\r\n");

            rootBlock = TestHelpers.GetAST(
                TestClassNameStr,
                methodName,
                false).RootBlock;

            TestLog.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
