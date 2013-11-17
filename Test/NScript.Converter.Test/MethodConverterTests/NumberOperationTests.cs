//-----------------------------------------------------------------------
// <copyright file="NumberOperationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NScript.CLR.Test;

    /// <summary>
    /// Definition for NumberOperationTests
    /// </summary>
    public class NumberOperationTests
    {
        private const string TestClassNameStr = "NumberOperations";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.NumberOperations.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "Div", "IntDivide.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [TestCase(TestClassNameStr, "Div", "IntDivide.js", TestType.All)]
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
