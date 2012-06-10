//-----------------------------------------------------------------------
// <copyright file="GenericTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for GenericTests
    /// </summary>
    [TestFixture]
    public class GenericTests
    {
        private const string TestClassNameStr = @"GenericSamples";
        private const string TestFilesNSStr = @"Cs2JsC.Converter.Test.MethodConverterTests.GenericTests.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "NewGenericObject", "NewGenericObject.js", TestType.All)]
        [Row(TestClassNameStr, "NewGenericObject2", "NewGenericObject2.js", TestType.All)]
        [Row(TestClassNameStr, "GenericMethodCall", "GenericMethodCall.js", TestType.All)]
        [Row(TestClassNameStr, "GenericMethodCall2", "GenericMethodCall2.js", TestType.All)]
        [Row(TestClassNameStr, "GenericMethodCall3", "GenericMethodCall3.js", TestType.All)]
        [Row("GenericRegressions", "GenericStructBoxing", "GenericStructBoxing.js", TestType.All)]
        [Row("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCall.js", TestType.Debug)]
        [Row("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCallRetail.js", TestType.Retail)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "NewGenericObject", "NewGenericObject.js", TestType.All)]
        [Row(TestClassNameStr, "NewGenericObject2", "NewGenericObject2.js", TestType.All)]
        [Row(TestClassNameStr, "GenericMethodCall", "GenericMethodCall.js", TestType.All)]
        [Row(TestClassNameStr, "GenericMethodCall2", "GenericMethodCall2.js", TestType.All)]
        [Row("GenericRegressions", "GenericStructBoxing", "GenericStructBoxing.js", TestType.All)]
        [Row("GenericRegressions", "TestGenericInterfacePropertyCall", "GenericInterfacePropertyCallMcs.js", TestType.All)]
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
