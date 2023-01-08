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
        [DataRow(TestClassNameStr, "GetDictionary", "GetDictionary.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "ReturnIntrinsicIndexer", "ReturnIntrinsicIndexer.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "ReturnIntrinsicProperty", "ReturnIntrinsicProperty.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "SetIntrinsicIndexer", "SetIntrinsicIndexer.js", TestType.All, false, false)]
        [DataRow(TestClassNameStr, "DelegateCombine", "DelegateCombine.js", TestType.All, false, false)]
        public void TestMcs(
            string className,
            string methodName,
            string resourceName,
            TestType testType,
            bool testMinifiedNames,
            bool instanceAsStatic)
        {
            ConverterTestHelpers.RunTest(
                TemplateNamespace + resourceName,
                className,
                methodName,
                testType,
                true,
                testMinifiedNames,
                instanceAsStatic);
        }
    }
}
