//-----------------------------------------------------------------------
// <copyright file="InlineInitializationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
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
        [DataRow(TestClassNameStr, "CallMethodWithInlineSettersElement", "CallMethodWithInlineSettersElement.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "CallMethodWithInlineSettersElement", "CallMethodWithInlineSettersElement.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ProcessItems", "ProcessItems.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ProcessItems", "ProcessItems.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ReturnInlineConstIntArray", "ReturnInlineConstIntArray.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ReturnInlineConstIntArray", "ReturnInlineConstIntArray.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ReturnInlineObjectArray", "ReturnInlineObjectArray.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ReturnInlineObjectArray", "ReturnInlineObjectArray.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ReturnInlineObjectArrayWithPropInit", "ReturnInlineObjectArrayWithPropInit.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ReturnInlineObjectArrayWithPropInit", "ReturnInlineObjectArrayWithPropInit.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ReturnInlineSettersElement", "ReturnInlineSettersElement.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ReturnInlineSettersElement", "ReturnInlineSettersElement.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ReturnStrDict", "ReturnStrDict.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ReturnStrDict", "ReturnStrDict.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ReturnStrDict2", "ReturnStrDict2.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ReturnStrDict2", "ReturnStrDict2.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestVarArgs", "VarArgs.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestVarArgs", "VarArgs.static.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "TestVarArgs2", "VarArgs2.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "TestVarArgs2", "VarArgs2.static.js", TestType.All, true)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType, bool isInstanceStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                TestClassNameStr,
                methodName,
                testType,
                true,
                false,
                isInstanceStatic);
        }
    }
}
