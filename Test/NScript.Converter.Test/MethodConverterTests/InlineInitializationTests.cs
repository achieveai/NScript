//-----------------------------------------------------------------------
// <copyright file="InlineInitializationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for InlineInitializationTests
    /// </summary>
    [TestFixture]
    public class InlineInitializationTests
    {
        private const string TestClassNameStr = "InlineComplexStatements";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.InlineInitializations.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "ReturnInlineObjectArray", "ReturnInlineObjectArray.js", TestType.All)]
        [Row(TestClassNameStr, "TestVarArgs", "VarArgs.js", TestType.All)]
        [Row(TestClassNameStr, "ReturnInlineSettersElement", "ReturnInlineSettersElement.js", TestType.All)]
        [Row(TestClassNameStr, "CallMethodWithInlineSettersElement", "CallMethodWithInlineSettersElement.js", TestType.All)]
        [Row(TestClassNameStr, "ReturnInlineConstIntArray", "ReturnInlineConstIntArray.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                TestClassNameStr,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "ReturnInlineObjectArray", "ReturnInlineObjectArray.js", TestType.All)]
        [Row(TestClassNameStr, "TestVarArgs", "VarArgs.js", TestType.All)]
        [Row(TestClassNameStr, "ReturnInlineSettersElement", "ReturnInlineSettersElement.js", TestType.All)]
        [Row(TestClassNameStr, "CallMethodWithInlineSettersElement", "CallMethodWithInlineSettersElement.js", TestType.All)]
        [Row(TestClassNameStr, "ReturnInlineConstIntArray", "ReturnInlineConstIntArray.js", TestType.All)]
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
