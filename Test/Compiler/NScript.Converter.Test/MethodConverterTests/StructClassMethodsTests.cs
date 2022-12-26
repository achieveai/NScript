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
        [DataRow("EnumUsingClass", "GetStr", "EnumToString.js", TestType.All, false)]
        [DataRow(TestClassNameStr, ".ctor", "Constructor.js", TestType.All, false)]
        [DataRow(TestClassNameStr, ".ctor", "Constructor.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "CallStructOverride", "CallStructOverride.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "CallStructOverride", "CallStructOverride.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "Foo", "InterfaceImplMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "Foo", "InterfaceImplMethod.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "GetFunc", "StructDelegate.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "GetFunc", "StructDelegate.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "Simple0ArgObjectAccessMethod", "Simple0ArgObjectAccessMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "Simple0ArgObjectAccessMethod", "Simple0ArgObjectAccessMethod.js", TestType.All, true)]
        [DataRow(TestClassNameStr, "ToString", "OverridenMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr, "ToString", "OverridenMethod.js", TestType.All, true)]
        [DataRow(TestClassNameStr2, ".ctor", "ScriptedConstructor.js", TestType.All, false)]
        [DataRow(TestClassNameStr2, ".ctor", "ScriptedConstructor.js", TestType.All, true)]
        [DataRow(TestClassNameStr2, "GetFooValue", "StructPropertyInvokeMcs.js", TestType.Debug, false)]
        [DataRow(TestClassNameStr2, "GetFooValue", "StructPropertyInvokeMcs.js", TestType.Debug, true)]
        [DataRow(TestClassNameStr2, "GetScriptedInt", "ScriptedMethod.js", TestType.All, false)]
        [DataRow(TestClassNameStr2, "GetScriptedInt", "ScriptedMethod.js", TestType.All, true)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType, bool instanceAsStatic)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false,
                instanceAsStatic);
        }
    }
}
