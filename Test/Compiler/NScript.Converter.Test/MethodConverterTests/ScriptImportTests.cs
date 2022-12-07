//-----------------------------------------------------------------------
// <copyright file="ScriptImportTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.Csc.Lib.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for ScriptImportTests
    /// </summary>
    [TestClass]
    public class ScriptImportTests
    {
        private const string TestClassNameStr = "JsScriptImport";
        private const string TemplateNamespace = "NScript.Converter.Test.MethodConverterTests.ScriptImports.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        public void TestScriptImportWith0Args()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "Simple0ArgScript.js",
                ScriptImportTests.TestClassNameStr,
                "Simple0ArgScript",
                TestType.All);
        }

        [DataTestMethod]
        public void TestScriptImportWith1Args()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "Simple1ArgScript.js",
                ScriptImportTests.TestClassNameStr,
                "Simple1ArgScript",
                TestType.All);
        }

        [DataTestMethod]
        public void TestSimpleStatic1ArgCsMethodCall()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "SimpleStatic1ArgCsMethodCall.js",
                ScriptImportTests.TestClassNameStr,
                "SimpleStatic1ArgCsMethodCall",
                TestType.All);
        }

        [DataTestMethod]
        public void TestSimpleCsMethodCall()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "SimpleCsMethodCall.js",
                ScriptImportTests.TestClassNameStr,
                "SimpleCsMethodCall",
                TestType.All);
        }

        [DataTestMethod]
        public void AccessStringElementCall()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "AccessStringElement.js",
                ScriptImportTests.TestClassNameStr,
                "AccessStringElement",
                TestType.All,
                true);
        }

        [DataTestMethod]
        public void CheckFooBarTest()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "CheckFooBar.js",
                ScriptImportTests.TestClassNameStr,
                "CheckFooBar",
                TestType.All,
                true);
        }

        [DataTestMethod]
        public void SelfMethodCallTest()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "SelfMethodCallTest.js",
                ScriptImportTests.TestClassNameStr,
                "GetSomething",
                TestType.All,
                true);
        }

        [DataTestMethod]
        public void ExternGenericCallTest()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "ExternGenericCallTest.js",
                ScriptImportTests.TestClassNameStr,
                "TestExternGenericCall",
                TestType.All,
                true);
        }

        [DataTestMethod]
        public void CallStaticScriptMethodOnImportedClass()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "CallStaticScriptMethodOnImportedClass.js",
                "System.Collections.Generic.List`1",
                "Add",
                TestType.All,
                true);
        }
    }
}
