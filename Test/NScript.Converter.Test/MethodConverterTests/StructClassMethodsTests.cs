//-----------------------------------------------------------------------
// <copyright file="StructClassMethodsTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for StructClassMethodsTests
    /// </summary>
    [TestFixture]
    public class StructClassMethodsTests
    {
        private const string TestClassNameStr = "StructClass";
        private const string TestClassNameStr2 = "StructClass2";
        private const string TestFilesNSStr = "NScript.Converter.Test.MethodConverterTests.StructClassMethods.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(TestClassNameStr, "Simple0ArgObjectAccessMethod", "Simple0ArgObjectAccessMethod.js", TestType.All)]
        [Row(TestClassNameStr, "Foo", "InterfaceImplMethod.js", TestType.All)]
        [Row(TestClassNameStr, "ToString", "OverridenMethod.js", TestType.All)]
        [Row(TestClassNameStr, "GetFunc", "StructDelegate.js", TestType.All)]
        [Row(TestClassNameStr, "CallStructOverride", "CallStructOverride.js", TestType.All)]
        [Row(TestClassNameStr2, "GetFooValue", "StructPropertyInvoke.js", TestType.Debug)]
        [Row(TestClassNameStr, ".ctor", "Constructor.js", TestType.All)]
        [Row(TestClassNameStr2, "GetScriptedInt", "ScriptedMethod.js", TestType.All)]
        [Row(TestClassNameStr2, ".ctor", "ScriptedConstructor.js", TestType.All)]
        [Row("EnumUsingClass", "GetStr", "EnumToString.js", TestType.All)]
        public void Test(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType);
        }

        [Test]
        [Row(TestClassNameStr, "Simple0ArgObjectAccessMethod", "Simple0ArgObjectAccessMethod.js", TestType.All)]
        [Row(TestClassNameStr, "Foo", "InterfaceImplMethod.js", TestType.All)]
        [Row(TestClassNameStr, "ToString", "OverridenMethod.js", TestType.All)]
        [Row(TestClassNameStr, "GetFunc", "StructDelegate.js", TestType.All)]
        [Row(TestClassNameStr, "CallStructOverride", "CallStructOverride.js", TestType.All)]
        [Row(TestClassNameStr2, "GetFooValue", "StructPropertyInvokeMcs.js", TestType.Debug)]
        [Row(TestClassNameStr, ".ctor", "Constructor.js", TestType.All)]
        [Row(TestClassNameStr2, "GetScriptedInt", "ScriptedMethod.js", TestType.All)]
        [Row(TestClassNameStr2, ".ctor", "ScriptedConstructor.js", TestType.All)]
        [Row("EnumUsingClass", "GetStr", "EnumToString.js", TestType.All)]
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
