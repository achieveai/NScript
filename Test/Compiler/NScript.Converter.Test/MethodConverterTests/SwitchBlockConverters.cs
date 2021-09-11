//-----------------------------------------------------------------------
// <copyright file="SwitchBlockConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for SwitchBlockConverters
    /// </summary>
    [TestClass]
    public class SwitchBlockConverters
    {
        private const string TestClassNameStr = "SwitchTest";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.SwitchBlockConverters.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "SimpleIntSwitch", "SimpleIntSwitch.js", TestType.All)]
        [DataRow(TestClassNameStr, "SwitchPatternMatching", "SwitchPatternMatching.js", TestType.All)]
        [DataRow(TestClassNameStr, "SwitchScopeTest", "SwitchScopeTest.js", TestType.All)]
        [DataRow(TestClassNameStr, "SwitchOnlyFunction", "SwitchOnlyFunction.js", TestType.Debug)]
        [DataRow(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturn.js", TestType.Debug)]
        [DataRow(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturnRetail.js", TestType.Retail)]
        [DataRow(TestClassNameStr, "SwitchWithReturnsOnly", "SwitchWithReturnOnlyRetail.js", TestType.Retail)]
        [DataRow(TestClassNameStr, "RegressSwitchWithFor", "RegressSwitchWithForDebug.js", TestType.Debug)]
        [DataRow(TestClassNameStr, "RegressionContinousSwitchValues", "RegressionContinousSwitchValues.js", TestType.Debug)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true);
        }
    }
}
