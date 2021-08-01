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
    public class Lang7FeatureTests
    {
        private const string TestClassNameStr = @"Lang7Features";
        private const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.NewLanguageFeatures.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "TestTupleReturn", "TestTupleReturn.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestTupleUnfolding", "TestTupleUnfolding.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestThrowExpression", "TestThrowExpression.js", TestType.All)]
        [DataRow(TestClassNameStr, "DefaultLiteralExpression", "DefaultLiteralExpression.js", TestType.All)]
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
