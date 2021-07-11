//-----------------------------------------------------------------------
// <copyright file="StructClassMethodsTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for StructClassMethodsTests
    /// </summary>
    [TestClass]
    public class StructClassMethodsTests
    {
        private const string TestClassNameStr = "StructClass";
        private const string TestClassNameStr2 = "StructClass2";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.StructClassMethods.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "Simple0ArgObjectAccessMethod", "Simple0ArgObjectAccessMethod.js", TestType.All)]
        [DataRow(TestClassNameStr, "Foo", "InterfaceImplMethod.js", TestType.All)]
        [DataRow(TestClassNameStr, "ToString", "OverridenMethod.js", TestType.All)]
        [DataRow(TestClassNameStr, "GetFunc", "StructDelegate.js", TestType.All)]
        [DataRow(TestClassNameStr, "CallStructOverride", "CallStructOverride.js", TestType.All)]
        [DataRow(TestClassNameStr2, "GetFooValue", "StructPropertyInvokeMcs.js", TestType.Debug)]
        [DataRow(TestClassNameStr, ".ctor", "Constructor.js", TestType.All)]
        [DataRow(TestClassNameStr2, "GetScriptedInt", "ScriptedMethod.js", TestType.All)]
        [DataRow(TestClassNameStr2, ".ctor", "ScriptedConstructor.js", TestType.All)]
        [DataRow("EnumUsingClass", "GetStr", "EnumToString.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true);
        }
    }
}
