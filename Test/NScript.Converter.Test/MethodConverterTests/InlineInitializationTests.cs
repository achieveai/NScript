//-----------------------------------------------------------------------
// <copyright file="InlineInitializationTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

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
        [TestCase(TestClassNameStr, "ReturnStrDict2", "ReturnStrDict2.js", TestType.All)]
        [TestCase(TestClassNameStr, "ReturnStrDict", "ReturnStrDict.js", TestType.All)]
        // [TestCase(TestClassNameStr, "ReturnInlineObjectArray", "ReturnInlineObjectArray.js", TestType.All)]
        // [TestCase(TestClassNameStr, "TestVarArgs", "VarArgs.js", TestType.All)]
        // [TestCase(TestClassNameStr, "TestVarArgs2", "VarArgs2.js", TestType.All)]
        // [TestCase(TestClassNameStr, "ProcessItems", "ProcessItems.js", TestType.All)]
        // [TestCase(TestClassNameStr, "ReturnInlineSettersElement", "ReturnInlineSettersElement.js", TestType.All)]
        // [TestCase(TestClassNameStr, "CallMethodWithInlineSettersElement", "CallMethodWithInlineSettersElement.js", TestType.All)]
        // [TestCase(TestClassNameStr, "ReturnInlineConstIntArray", "ReturnInlineConstIntArray.js", TestType.All)]
        // [TestCase(TestClassNameStr, "ReturnInlineObjectArrayWithPropInit", "ReturnInlineObjectArrayWithPropInit.js", TestType.All)]
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
