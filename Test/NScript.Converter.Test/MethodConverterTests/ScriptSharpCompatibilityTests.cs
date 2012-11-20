//-----------------------------------------------------------------------
// <copyright file="ScriptSharpCompatibilityTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using MbUnit.Framework;


    /// <summary>
    /// Definition for ScriptSharpCompatibilityTests
    /// </summary>
    [TestFixture]
    public class ScriptSharpCompatibilityTests
    {
        private const string TestClassNameStr = "ScriptSharpCompat";
        private const string TemplateNamespace = "NScript.Converter.Test.MethodConverterTests.ScriptSharpCompatiblity.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        public void TestReturnIntrinsicProperty()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "ReturnIntrinsicProperty.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "ReturnIntrinsicProperty",
                TestType.All);
        }

        [Test]
        public void TestReturnIntrinsicIndexer()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "ReturnIntrinsicIndexer.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "ReturnIntrinsicIndexer",
                TestType.All);
        }

        [Test]
        public void TestSetIntrinsicIndexer()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "SetIntrinsicIndexer.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "SetIntrinsicIndexer",
                TestType.All);
        }

        [Test]
        public void TestGetDictionary()
        {
            ConverterTestHelpers.RunTest(
                ScriptSharpCompatibilityTests.TemplateNamespace + "GetDictionary.js",
                ScriptSharpCompatibilityTests.TestClassNameStr,
                "GetDictionary",
                TestType.All);
        }

        [Test]
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
