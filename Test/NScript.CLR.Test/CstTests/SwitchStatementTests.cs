//-----------------------------------------------------------------------
// <copyright file="SwitchStatementTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using NUnit.Framework;
    using System.Diagnostics;

    /// <summary>
    /// Definition for SwitchStatementTests
    /// </summary>
    [TestFixture]
    public class SwitchStatementTests
    {
        private const string TestClassNameStr = "SwitchTest";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.SwitchStatement.";

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
        [TestCase(SwitchStatementTests.TestClassNameStr, "SimpleIntSwitch", true,  "SimpleIntSwitch.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SimpleIntSwitch", false, "SimpleIntSwitchRetail.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchOnlyFunction", true,  "SwitchOnlyFunction.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchOnlyFunction", false, "SwitchOnlyFunctionRetail.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchWithReturn", true,  "SwitchWithReturn.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchWithReturn", false, "SwitchWithReturnRetail.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchWithReturnsOnly", true,  "SwitchWithReturnsOnly.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchWithReturnsOnly", false, "SwitchWithReturnsOnlyRetail.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "RegressSwitchWithFor", true,  "RegressSwitchWithFor.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "RegressSwitchWithFor", false, "RegressSwitchWithForRetail.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "RegressionContinousSwitch2", true,  "RegressionContinousSwitch2.xml")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "RegressionContinousSwitch2", false, "RegressionContinousSwitch2.xml")]
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

            Debug.Write(TestHelpers.Serialize(rootBlock));
            Debug.Write("\r\n");

            rootBlock = TestHelpers.GetAST(
                TestClassNameStr,
                methodName,
                false).RootBlock;

            Debug.Write(TestHelpers.Serialize(rootBlock));
        }
    }
}
