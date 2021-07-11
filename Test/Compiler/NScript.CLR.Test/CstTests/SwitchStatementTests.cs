//-----------------------------------------------------------------------
// <copyright file="SwitchStatementTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test.CstTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    /// <summary>
    /// Definition for SwitchStatementTests
    /// </summary>
    [TestClass]
    public class SwitchStatementTests
    {
        private const string TestClassNameStr = "SwitchTest";
        private const string ResourceFileNamepace = @"NScript.CLR.Test.CstTests.SwitchStatement.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(SwitchStatementTests.TestClassNameStr, "SimpleIntSwitch", true,  "SimpleIntSwitch.csast")]
        [DataRow(SwitchStatementTests.TestClassNameStr, "SwitchOnlyFunction", true,  "SwitchOnlyFunction.csast")]
        [DataRow(SwitchStatementTests.TestClassNameStr, "SwitchWithReturn", true,  "SwitchWithReturn.csast")]
        [DataRow(SwitchStatementTests.TestClassNameStr, "SwitchWithReturnsOnly", true,  "SwitchWithReturnsOnly.csast")]
        [DataRow(SwitchStatementTests.TestClassNameStr, "RegressSwitchWithFor", true,  "RegressSwitchWithFor.csast")]
        [DataRow(SwitchStatementTests.TestClassNameStr, "RegressionContinousSwitch2", true,  "RegressionContinousSwitch2.csast")]
        public void Test(string testClassName, string methodName, bool isDebug, string resourceName)
        {
            TestHelpers.Test(
                SwitchStatementTests.ResourceFileNamepace + resourceName,
                testClassName,
                methodName,
                isDebug,
                false);
        }
    }
}
