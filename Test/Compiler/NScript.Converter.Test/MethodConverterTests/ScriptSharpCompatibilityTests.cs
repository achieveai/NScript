//-----------------------------------------------------------------------
// <copyright file="ScriptSharpCompatibilityTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    /// <summary>
    /// Definition for ScriptSharpCompatibilityTests
    /// </summary>
    [TestClass]
    public class ScriptSharpCompatibilityTests
    {
        private const string TestClassNameStr = "ScriptSharpCompat";
        private const string TemplateNamespace = "NScript.Converter.Test.MethodConverterTests.ScriptSharpCompatiblity.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        public void TestReturnIntrinsicProperty()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "ReturnIntrinsicProperty.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "ReturnIntrinsicProperty",
                TestType.All);
        }

        [DataTestMethod]
        public void TestReturnIntrinsicIndexer()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "ReturnIntrinsicIndexer.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "ReturnIntrinsicIndexer",
                TestType.All);
        }

        [DataTestMethod]
        public void TestSetIntrinsicIndexer()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "SetIntrinsicIndexer.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "SetIntrinsicIndexer",
                TestType.All);
        }

        [DataTestMethod]
        public void TestGetDictionary()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "GetDictionary.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "GetDictionary",
                TestType.All);
        }

        [DataTestMethod]
        public void TestDelegateCombine()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "DelegateCombine.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "DelegateCombine",
                TestType.All);
        }
    }
}
