//-----------------------------------------------------------------------
// <copyright file="SwitchBlockConverters.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for SwitchBlockConverters
    /// </summary>
    [TestFixture]
    public class SwitchBlockConverters
    {
        private const string TestClassNameStr = "SwitchTest";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.SwitchBlockConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "SimpleIntSwitch", "SimpleIntSwitch.js", TestType.All)]
        [TestCase(TestClassNameStr, "SwitchOnlyFunction", "SwitchOnlyFunction.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "SwitchOnlyFunction", "SwitchOnlyFunctionRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturn.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturnRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "SwitchWithReturnsOnly", "SwitchWithReturnOnly.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "SwitchWithReturnsOnly", "SwitchWithReturnOnlyRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "RegressSwitchWithFor", "RegressSwitchWithForDebug.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "RegressSwitchWithFor", "RegressSwitchWithForRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "RegressionContinousSwitchValues", "RegressionContinousSwitchValues.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "RegressionContinousSwitchValues", "RegressionContinousSwitchValuesRetail.js", TestType.Retail)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [TestCase(TestClassNameStr, "SimpleIntSwitch", "SimpleIntSwitch.js", TestType.All)]
        [TestCase(TestClassNameStr, "SwitchOnlyFunction", "SwitchOnlyFunction.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturn.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "SwitchWithReturn", "SwitchWithReturnRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "SwitchWithReturnsOnly", "SwitchWithReturnOnlyRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "RegressSwitchWithFor", "RegressSwitchWithForDebug.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "RegressionContinousSwitchValues", "RegressionContinousSwitchValues.js", TestType.Debug)]
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
