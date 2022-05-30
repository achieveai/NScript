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
        [DataRow(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoopsMcs.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestPassByRefAssignment", "TestPassByRefAssignment.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All, false)]
        [DataRow("JsScriptImport", "GetAndSetIndexerProperty", "GetAndSetIndexerPropertyMcs.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "CollapsingForInIfRegression", "CollapsingForInIfRegression.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "RegressionWithLastIndexOfString", "RegressionWithLastIndexOfString.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "OddNativeArrayPushBehavior", "OddNativeArrayPushBehavior.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "DeclarationWithOut", "DeclarationWithOut.js", TestType.All, false)]
        [DataRow("BasicStatements", "RegressionFlagsOr", "RegressionFlagsOr.js", TestType.All, false)]
        [DataRow("BasicStatements", "RegressionFlagsOrWithNullable", "RegressionFlagsOrNullable.js", TestType.All, false)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign", "RegressionFlagsOrAssign.js", TestType.All, false)]
        [DataRow("BasicStatements", "RegressionFlagsOr2", "RegressionFlagsOr2.js", TestType.All, false)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign2", "RegressionFlagsOrAssign2.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ShrPrecedenceRegression", "ShrPrecedenceRegression.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ArrayIndexOfName", "ArrayIndexOfName.js", TestType.Debug, true)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool testMinifiedNames)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                testMinifiedNames);
        }
    }
}
