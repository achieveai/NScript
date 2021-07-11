//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for RegressionTests
    /// </summary>
    [TestClass]
    public class RegressionTests
    {
        public const string TestClassNameStr = "FuncRegressions";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.Regressions.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.All)]
        [DataRow(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoopsMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "TestPassByRefAssignment", "TestPassByRefAssignment.js", TestType.All)]
        [DataRow(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All)]
        [DataRow("JsScriptImport", "GetAndSetIndexerProperty", "GetAndSetIndexerPropertyMcs.js", TestType.All)]
        [DataRow(TestClassNameStr, "CollapsingForInIfRegression", "CollapsingForInIfRegression.js", TestType.All)]
        [DataRow(TestClassNameStr, "RegressionWithLastIndexOfString", "RegressionWithLastIndexOfString.js", TestType.All)]
        [DataRow(TestClassNameStr, "OddNativeArrayPushBehavior", "OddNativeArrayPushBehavior.js", TestType.All)]
        [DataRow(TestClassNameStr, "DeclarationWithOut", "DeclarationWithOut.js", TestType.All)]
        [DataRow("BasicStatements", "RegressionFlagsOr", "RegressionFlagsOr.js", TestType.All)]
        [DataRow("BasicStatements", "RegressionFlagsOrWithNullable", "RegressionFlagsOrNullable.js", TestType.All)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign", "RegressionFlagsOrAssign.js", TestType.All)]
        [DataRow("BasicStatements", "RegressionFlagsOr2", "RegressionFlagsOr2.js", TestType.All)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign2", "RegressionFlagsOrAssign2.js", TestType.All)]
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
