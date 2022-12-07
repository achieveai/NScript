//-----------------------------------------------------------------------
// <copyright file="NumberOperationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Csc.Lib.Test;

    /// <summary>
    /// Definition for NumberOperationTests
    /// </summary>
    public class NumberOperationTests
    {
        private const string TestClassNameStr = "NumberOperations";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.NumberOperations.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "Div", "IntDivide.js", TestType.All)]
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
