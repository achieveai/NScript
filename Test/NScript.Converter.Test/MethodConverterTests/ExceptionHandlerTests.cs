//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NUnit.Framework;
    using NScript.CLR.Test;

    /// <summary>
    /// Definition for ExceptionHandlerTests
    /// </summary>
    [TestFixture]
    public class ExceptionHandlerTests
    {
        private const string TestClassNameStr = @"ExceptionHandlerSamples";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.ExceptionHandling.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "TryFinallySimple", "TryFinallySimple.js", TestType.All)]
        [TestCase(TestClassNameStr, "TryFinallyWithReturn", "TryFinallyWithReturn.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "TryFinallyWithReturn", "TryFinallyWithReturnRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "TryCatchSimple", "TryCatchSimple.js", TestType.All)]
        [TestCase(TestClassNameStr, "TryCatchWithReturn", "TryCatchWithReturn.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "TryCatchWithReturn", "TryCatchWithReturnRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "TryCatchFinallySimple", "TryCatchFinallySimple.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [TestCase(TestClassNameStr, "TryFinallySimple", "TryFinallySimple.js", TestType.All)]
        [TestCase(TestClassNameStr, "TryFinallyWithReturn", "TryFinallyWithReturnMcs.js", TestType.All)]
        [TestCase(TestClassNameStr, "TryCatchSimple", "TryCatchSimple.js", TestType.All)]
        [TestCase(TestClassNameStr, "TryCatchWithReturn", "TryCatchWithReturnMcs.js", TestType.All)]
        [TestCase(TestClassNameStr, "TryCatchFinallySimple", "TryCatchFinallySimple.js", TestType.All)]
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
