//-----------------------------------------------------------------------
// <copyright file="SwitchBlockConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for SwitchBlockConverters
    /// </summary>
    [TestFixture]
    public class SwitchBlockConverters
    {
        private const string TestClassNameStr = "SwitchTest";
        private const string TestFilesNSStr = "Cs2JsC.Converter.Test.MethodConverterTests.SwitchBlockConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "SimpleIntSwitch", "SimpleIntSwitch.js", TestType.All)]
        [Row(TestClassNameStr, "SwitchOnlyFunction", "SwitchOnlyFunction.js", TestType.Debug)]
        [Row(TestClassNameStr, "SwitchOnlyFunction", "SwitchOnlyFunctionRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturn.js", TestType.Debug)]
        [Row(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturnRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "SwitchWithReturnsOnly", "SwitchWithReturnOnly.js", TestType.Debug)]
        [Row(TestClassNameStr, "SwitchWithReturnsOnly", "SwitchWithReturnOnlyRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "RegressSwitchWithFor", "RegressSwitchWithForDebug.js", TestType.Debug)]
        [Row(TestClassNameStr, "RegressSwitchWithFor", "RegressSwitchWithForRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "RegressionContinousSwitchValues", "RegressionContinousSwitchValues.js", TestType.Debug)]
        [Row(TestClassNameStr, "RegressionContinousSwitchValues", "RegressionContinousSwitchValuesRetail.js", TestType.Retail)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "SimpleIntSwitch", "SimpleIntSwitch.js", TestType.All)]
        [Row(TestClassNameStr, "SwitchOnlyFunction", "SwitchOnlyFunction.js", TestType.Debug)]
        [Row(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturn.js", TestType.Debug)]
        [Row(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturnRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "SwitchWithReturnsOnly", "SwitchWithReturnOnlyRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "RegressSwitchWithFor", "RegressSwitchWithForDebug.js", TestType.Debug)]
        [Row(TestClassNameStr, "RegressionContinousSwitchValues", "RegressionContinousSwitchValues.js", TestType.Debug)]
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
