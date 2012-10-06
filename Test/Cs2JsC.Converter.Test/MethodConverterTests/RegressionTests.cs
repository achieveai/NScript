//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for RegressionTests
    /// </summary>
    public class RegressionTests
    {
        private const string TestClassNameStr = "FuncRegressions";
        private const string TestFilesNSStr = "Cs2JsC.Converter.Test.MethodConverterTests.Regressions.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.Debug)]
        [Row(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlockRetail.js", TestType.Retail)]
        [Row(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoops.js", TestType.Retail)]
        [Row(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.All)]
        [Row(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoopsMcs.js", TestType.All)]
        [Row(TestClassNameStr, "TestPassByRefAssignment", "TestPassByRefAssignment.js", TestType.All)]
        [Row(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All)]
        [Row("JsScriptImport", "GetAndSetIndexerProperty", "GetAndSetIndexerPropertyMcs.js", TestType.All)]
        [Row(TestClassNameStr, "CollapsingForInIfRegression", "CollapsingForInIfRegression.js", TestType.All)]
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
