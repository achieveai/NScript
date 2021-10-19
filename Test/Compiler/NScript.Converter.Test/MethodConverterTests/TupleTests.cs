//-----------------------------------------------------------------------
// <copyright file="Lang7FeatureTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    /// <summary>
    /// Definition for Lang7FeatureTests
    /// </summary>
    [TestClass]
    public class TupleTests
    {
        private const string TestClassNameStr = @"TupleTests";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.TupleTests.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "TestTupleDecons", "TestTupleDecons.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestTupleReturn", "TestTupleReturn.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestTempVarCreation", "TestTempVarCreation.js", TestType.All)]
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