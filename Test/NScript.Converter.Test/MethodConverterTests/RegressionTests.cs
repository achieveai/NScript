//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for RegressionTests
    /// </summary>
    [TestFixture]
    public class RegressionTests
    {
        private const string TestClassNameStr = "FuncRegressions";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.Regressions.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.Debug)]
        [TestCase(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlockRetail.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoops.js", TestType.Retail)]
        [TestCase(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        //[TestCase(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.All)]
        //[TestCase(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoopsMcs.js", TestType.All)]
        //[TestCase(TestClassNameStr, "TestPassByRefAssignment", "TestPassByRefAssignment.js", TestType.All)]
        //[TestCase(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All)]
        //[TestCase("JsScriptImport", "GetAndSetIndexerProperty", "GetAndSetIndexerPropertyMcs.js", TestType.All)]
        //[TestCase(TestClassNameStr, "CollapsingForInIfRegression", "CollapsingForInIfRegression.js", TestType.All)]
        //[TestCase(TestClassNameStr, "RegressionWithLastIndexOfString", "RegressionWithLastIndexOfString.js", TestType.All)]
        //[TestCase(TestClassNameStr, "OddNativeArrayPushBehavior", "OddNativeArrayPushBehavior.js", TestType.All)]
        [TestCase("BasicStatements", "RegressionFlagsOr", "RegressionFlagsOr.js", TestType.All)]
        //[TestCase("BasicStatements", "RegressionFlagsOrAssign", "RegressionFlagsOrAssign.js", TestType.All)]
        [TestCase("BasicStatements", "RegressionFlagsOr2", "RegressionFlagsOr2.js", TestType.All)]
        [TestCase("BasicStatements", "RegressionFlagsOrAssign2", "RegressionFlagsOrAssign2.js", TestType.All)]
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
