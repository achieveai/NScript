//-----------------------------------------------------------------------
// <copyright file="GenericTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for GenericTests
    /// </summary>
    [TestFixture]
    public class GenericTests
    {
        private const string TestClassNameStr = @"GenericSamples";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.GenericTests.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        // [TestCase(TestClassNameStr, "NewGenericObject", "NewGenericObject.js", TestType.All)]
        // [TestCase(TestClassNameStr, "NewGenericObject2", "NewGenericObject2.js", TestType.All)]
        [TestCase(TestClassNameStr, "GenericMethodCall", "GenericMethodCall.js", TestType.All)]
        // [TestCase(TestClassNameStr, "GenericMethodCall2", "GenericMethodCall2.js", TestType.All)]
        // [TestCase(TestClassNameStr, "GenericMethodCall3", "GenericMethodCall3.js", TestType.All)]
        // [TestCase("GenericRegressions", "GenericStructBoxing", "GenericStructBoxing.js", TestType.All)]
        // [TestCase("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCall.js", TestType.Debug)]
        // [TestCase("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCallRetail.js", TestType.Retail)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        // [TestCase(TestClassNameStr, "NewGenericObject", "NewGenericObject.js", TestType.All)]
        // [TestCase(TestClassNameStr, "NewGenericObject2", "NewGenericObject2.js", TestType.All)]
        // [TestCase(TestClassNameStr, "GenericMethodCall", "GenericMethodCall.js", TestType.All)]
        // [TestCase(TestClassNameStr, "GenericMethodCall2", "GenericMethodCall2.js", TestType.All)]
        // [TestCase("GenericRegressions", "GenericStructBoxing", "GenericStructBoxing.js", TestType.All)]
        // [TestCase("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCallMcs.js", TestType.All)]
        [TestCase("GenericRegressions", "TestGenericMethodCalls", "GenericMethodCalls.js", TestType.All)]
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
