//-----------------------------------------------------------------------
// <copyright file="GenericTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for GenericTests
    /// </summary>
    [TestClass]
    public class GenericTests
    {
        private const string TestClassNameStr = @"GenericSamples";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.GenericTests.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow("GenericRegressions", "AddEvent", "IgnoreGenericParamsRegression.static.js", TestType.All, true)]
        [DataRow("GenericRegressions", "GenericStructBoxing", "GenericStructBoxing.js", TestType.All, false)]
        [DataRow("GenericRegressions", "GenericStructBoxing", "GenericStructBoxing.js", TestType.All, true)]
        [DataRow("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCallMcs.js", TestType.All, false)]
        [DataRow("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCallMcs.js", TestType.All, true)]
        [DataRow("GenericRegressions", "TestGenericMethodCalls", "GenericMethodCalls.js", TestType.All, false)]
        [DataRow("GenericRegressions", "TestGenericMethodCalls", "GenericMethodCalls.js", TestType.All, true)]
        [DataRow("TestGenerics", "TestRecursiveTypes", "GenericRecursiveCalls.js", TestType.All, false)]
        [DataRow("TestGenerics", "TestRecursiveTypes", "GenericRecursiveCalls.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "GenericMethodCall", "GenericMethodCall.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "GenericMethodCall", "GenericMethodCall.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "GenericMethodCall2", "GenericMethodCall2.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "GenericMethodCall2", "GenericMethodCall2.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "NewGenericObject", "NewGenericObject.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "NewGenericObject", "NewGenericObject.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "NewGenericObject2", "NewGenericObject2.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "NewGenericObject2", "NewGenericObject2.js", TestType.All, true)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType, bool isInstanceStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                isInstanceStatic);
        }
    }
}
