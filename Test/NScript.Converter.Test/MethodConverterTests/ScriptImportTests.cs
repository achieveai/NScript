//-----------------------------------------------------------------------
// <copyright file="ScriptImportTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for ScriptImportTests
    /// </summary>
    [TestFixture]
    public class ScriptImportTests
    {
        private const string TestClassNameStr = "JsScriptImport";
        private const string TemplateNamespace = "NScript.Converter.Test.MethodConverterTests.ScriptImports.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        public void TestScriptImportWith0Args()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "Simple0ArgScript.js",
                ScriptImportTests.TestClassNameStr,
                "Simple0ArgScript",
                TestType.All);
        }

        [Test]
        public void TestScriptImportWith1Args()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "Simple1ArgScript.js",
                ScriptImportTests.TestClassNameStr,
                "Simple1ArgScript",
                TestType.All);
        }

        [Test]
        public void TestSimpleStatic1ArgCsMethodCall()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "SimpleStatic1ArgCsMethodCall.js",
                ScriptImportTests.TestClassNameStr,
                "SimpleStatic1ArgCsMethodCall",
                TestType.All);
        }

        [Test]
        public void TestSimpleCsMethodCall()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "SimpleCsMethodCall.js",
                ScriptImportTests.TestClassNameStr,
                "SimpleCsMethodCall",
                TestType.All);
        }

        [Test]
        public void AccessStringElementCall()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "AccessStringElement.js",
                ScriptImportTests.TestClassNameStr,
                "AccessStringElement",
                TestType.All,
                true);
        }

        [Test]
        public void CheckFooBarTest()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "CheckFooBar.js",
                ScriptImportTests.TestClassNameStr,
                "CheckFooBar",
                TestType.All,
                true);
        }

        [Test]
        public void SelfMethodCallTest()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "SelfMethodCallTest.js",
                ScriptImportTests.TestClassNameStr,
                "GetSomething",
                TestType.All,
                true);
        }

        [Test]
        public void ExternGenericCallTest()
        {
            ConverterTestHelpers.RunTest(
                ScriptImportTests.TemplateNamespace + "ExternGenericCallTest.js",
                ScriptImportTests.TestClassNameStr,
                "TestExternGenericCall",
                TestType.All,
                true);
        }

        [Test]
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
