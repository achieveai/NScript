//-----------------------------------------------------------------------
// <copyright file="InlineInitializationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for InlineInitializationTests
    /// </summary>
    [TestClass]
    public class InlineInitializationTests
    {
        private const string TestClassNameStr = "InlineComplexStatements";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.InlineInitializations.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "ReturnStrDict2", "ReturnStrDict2.js", TestType.All)]
        [DataRow(TestClassNameStr, "ReturnStrDict", "ReturnStrDict.js", TestType.All)]
        // [DataRow(TestClassNameStr, "ReturnInlineObjectArray", "ReturnInlineObjectArray.js", TestType.All)]
        // [DataRow(TestClassNameStr, "TestVarArgs", "VarArgs.js", TestType.All)]
        // [DataRow(TestClassNameStr, "TestVarArgs2", "VarArgs2.js", TestType.All)]
        // [DataRow(TestClassNameStr, "ProcessItems", "ProcessItems.js", TestType.All)]
        // [DataRow(TestClassNameStr, "ReturnInlineSettersElement", "ReturnInlineSettersElement.js", TestType.All)]
        // [DataRow(TestClassNameStr, "CallMethodWithInlineSettersElement", "CallMethodWithInlineSettersElement.js", TestType.All)]
        // [DataRow(TestClassNameStr, "ReturnInlineConstIntArray", "ReturnInlineConstIntArray.js", TestType.All)]
        // [DataRow(TestClassNameStr, "ReturnInlineObjectArrayWithPropInit", "ReturnInlineObjectArrayWithPropInit.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                TestClassNameStr,
                methodName,
                testType,
                true);
        }
    }
}
