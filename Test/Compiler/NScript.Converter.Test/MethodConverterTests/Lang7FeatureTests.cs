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
        [DataRow(TestClassNameStr, "TestTupleReturn", "TestTupleReturn.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestTupleReturn", "TestTupleReturn.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestTupleUnfolding", "TestTupleUnfolding.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestTupleUnfolding", "TestTupleUnfolding.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestThrowExpression", "TestThrowExpression.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestThrowExpression", "TestThrowExpression.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "DefaultLiteralExpression", "DefaultLiteralExpression.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "DefaultLiteralExpression", "DefaultLiteralExpression.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "SumPositiveNumbers", "SumPositiveNumbers.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "SumPositiveNumbers", "SumPositiveNumbers.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestSwitchPatternMatching_1", "TestSwitchPatternMatching_1.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestSwitchPatternMatching_1", "TestSwitchPatternMatching_1.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestSwitchPatternMatching_2", "TestSwitchPatternMatching_2.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestSwitchPatternMatching_2", "TestSwitchPatternMatching_2.js", TestType.All, true)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool isInstanceStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                isInstanceStatic);
        }
    }
}
