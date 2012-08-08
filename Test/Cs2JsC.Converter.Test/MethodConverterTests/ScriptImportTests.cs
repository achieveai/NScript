//-----------------------------------------------------------------------
// <copyright file="ScriptImportTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.MethodConverterTests
{
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for ScriptImportTests
    /// </summary>
    [TestFixture]
    public class ScriptImportTests
    {
        private const string TestClassNameStr = "JsScriptImport";
        private const string TemplateNamespace = "Cs2JsC.Converter.Test.MethodConverterTests.ScriptImports.";

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
    }
}
