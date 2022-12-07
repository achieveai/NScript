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
        [DataRow(TestClassNameStr, "NewGenericObject", "NewGenericObject.js", TestType.All)]
        [DataRow(TestClassNameStr, "NewGenericObject2", "NewGenericObject2.js", TestType.All)]
        [DataRow(TestClassNameStr, "GenericMethodCall", "GenericMethodCall.js", TestType.All)]
        [DataRow(TestClassNameStr, "GenericMethodCall2", "GenericMethodCall2.js", TestType.All)]
        [DataRow("GenericRegressions", "GenericStructBoxing", "GenericStructBoxing.js", TestType.All)]
        [DataRow("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCallMcs.js", TestType.All)]
        [DataRow("GenericRegressions", "TestGenericMethodCalls", "GenericMethodCalls.js", TestType.All)]
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
