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

        [Test]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SimpleIntSwitch", true,  "SimpleIntSwitch.csast")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchOnlyFunction", true,  "SwitchOnlyFunction.csast")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchWithReturn", true,  "SwitchWithReturn.csast")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "SwitchWithReturnsOnly", true,  "SwitchWithReturnsOnly.csast")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "RegressSwitchWithFor", true,  "RegressSwitchWithFor.csast")]
        [TestCase(SwitchStatementTests.TestClassNameStr, "RegressionContinousSwitch2", true,  "RegressionContinousSwitch2.csast")]
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
