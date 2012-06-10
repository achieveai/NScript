//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Gallio.Framework;
    using MbUnit.Framework;
    using Cs2JsC.CLR.Test;

    /// <summary>
    /// Definition for ExceptionHandlerTests
    /// </summary>
    [TestFixture]
    public class ExceptionHandlerTests
    {
        private const string TestClassNameStr = @"ExceptionHandlerSamples";
        private const string TestFilesNSStr = @"Cs2JsC.Converter.Test.MethodConverterTests.ExceptionHandling.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "TryFinallySimple", "TryFinallySimple.js", TestType.All)]
        [Row(TestClassNameStr, "TryFinallyWithReturn", "TryFinallyWithReturn.js", TestType.Debug)]
        [Row(TestClassNameStr, "TryFinallyWithReturn", "TryFinallyWithReturnRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "TryCatchSimple", "TryCatchSimple.js", TestType.All)]
        [Row(TestClassNameStr, "TryCatchWithReturn", "TryCatchWithReturn.js", TestType.Debug)]
        [Row(TestClassNameStr, "TryCatchWithReturn", "TryCatchWithReturnRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "TryCatchFinallySimple", "TryCatchFinallySimple.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "TryFinallySimple", "TryFinallySimple.js", TestType.All)]
        [Row(TestClassNameStr, "TryFinallyWithReturn", "TryFinallyWithReturnMcs.js", TestType.All)]
        [Row(TestClassNameStr, "TryCatchSimple", "TryCatchSimple.js", TestType.All)]
        [Row(TestClassNameStr, "TryCatchWithReturn", "TryCatchWithReturnMcs.js", TestType.All)]
        [Row(TestClassNameStr, "TryCatchFinallySimple", "TryCatchFinallySimple.js", TestType.All)]
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
