//-----------------------------------------------------------------------
// <copyright file="ExceptionHandlerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Csc.Lib.Test;

    /// <summary>
    /// Definition for ExceptionHandlerTests
    /// </summary>
    [TestClass]
    public class ExceptionHandlerTests
    {
        private const string TestClassNameStr = @"ExceptionHandlerSamples";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.ExceptionHandling.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "TryFinallySimple", "TryFinallySimple.js", TestType.All)]
        [DataRow(TestClassNameStr, "TryFinallyWithReturn", "TryFinallyWithReturnMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "TryCatchSimple", "TryCatchSimple.js", TestType.All)]
        [DataRow(TestClassNameStr, "TryCatchWithReturn", "TryCatchWithReturnMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "TryCatchFinallySimple", "TryCatchFinallySimple.js", TestType.All)]
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
