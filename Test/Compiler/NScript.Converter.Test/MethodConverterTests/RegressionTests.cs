//-----------------------------------------------------------------------
// <copyright file="RegressionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
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
        [DataRow("BasicStatements", "TestRefs", "ReferenceWriteScopeClash.js", TestType.Debug, true, false)]
        [DataRow("BasicStatements", "TestRefs", "ReferenceWriteScopeClash.Retail.js", TestType.Retail, true, false)]
        [DataRow("BasicStatements", "RegressionFlagsOr", "RegressionFlagsOr.js", TestType.All, false, false)]
        [DataRow("BasicStatements", "RegressionFlagsOr", "RegressionFlagsOr.js", TestType.All, false, true)]
        [DataRow("BasicStatements", "RegressionFlagsOr2", "RegressionFlagsOr2.js", TestType.All, false, false)]
        [DataRow("BasicStatements", "RegressionFlagsOr2", "RegressionFlagsOr2.js", TestType.All, false, true)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign", "RegressionFlagsOrAssign.js", TestType.All, false, false)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign", "RegressionFlagsOrAssign.js", TestType.All, false, true)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign2", "RegressionFlagsOrAssign2.js", TestType.All, false, false)]
        [DataRow("BasicStatements", "RegressionFlagsOrAssign2", "RegressionFlagsOrAssign2.js", TestType.All, false, true)]
        [DataRow("BasicStatements", "RegressionFlagsOrWithNullable", "RegressionFlagsOrNullable.js", TestType.All, false, false)]
        [DataRow("BasicStatements", "RegressionFlagsOrWithNullable", "RegressionFlagsOrNullable.js", TestType.All, false, true)]
        [DataRow("JsScriptImport", "GetAndSetIndexerProperty", "GetAndSetIndexerPropertyMcs.js", TestType.All, false, false)]
        [DataRow("JsScriptImport", "GetAndSetIndexerProperty", "GetAndSetIndexerPropertyMcs.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "ArrayIndexOfName", "ArrayIndexOfName.js", TestType.Debug, true, false)]
        [DataRow(TestClassNameStr, "ArrayIndexOfName", "ArrayIndexOfName.static.js", TestType.Debug, true, true)]
        [DataRow(TestClassNameStr, "CollapsingForInIfRegression", "CollapsingForInIfRegression.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "CollapsingForInIfRegression", "CollapsingForInIfRegression.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "DeclarationWithOut", "DeclarationWithOut.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "DeclarationWithOut", "DeclarationWithOut.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "EscapesInString", "EscapesInString.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "IfElseInForBlock", "IfElseInForBlock.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoopsMcs.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "NestedWhileLoops", "NestedWhileLoopsMcs.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "OddNativeArrayPushBehavior", "OddNativeArrayPushBehavior.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "OddNativeArrayPushBehavior", "OddNativeArrayPushBehavior.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "RegressionWithLastIndexOfString", "RegressionWithLastIndexOfString.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "RegressionWithLastIndexOfString", "RegressionWithLastIndexOfString.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "ShrPrecedenceRegression", "ShrPrecedenceRegression.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "ShrPrecedenceRegression", "ShrPrecedenceRegression.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "TestPassByRefAssignment", "TestPassByRefAssignment.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "TestPassByRefAssignment", "TestPassByRefAssignment.js", TestType.All, false, true)]
        [DataRow(TestClassNameStr, "TestPassByRefAssignment", "TestPassByRefAssignment.js", TestType.All, false, true)]
        [DataRow("GeneratorWrapper", "MoveNext", "NativeGeneratorNext.js", TestType.All, false, true)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool testMinifiedNames,
            bool instanceAsStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                testMinifiedNames,
                instanceAsStatic);
        }
    }

}
