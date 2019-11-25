//-----------------------------------------------------------------------
// <copyright file="StructClassMethodsTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

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
        [TestCase(TestClassNameStr, "Simple0ArgObjectAccessMethod", "Simple0ArgObjectAccessMethod.js", TestType.All)]
        [TestCase(TestClassNameStr, "Foo", "InterfaceImplMethod.js", TestType.All)]
        [TestCase(TestClassNameStr, "ToString", "OverridenMethod.js", TestType.All)]
        [TestCase(TestClassNameStr, "GetFunc", "StructDelegate.js", TestType.All)]
        [TestCase(TestClassNameStr, "CallStructOverride", "CallStructOverride.js", TestType.All)]
        [TestCase(TestClassNameStr2, "GetFooValue", "StructPropertyInvokeMcs.js", TestType.Debug)]
        [TestCase(TestClassNameStr, ".ctor", "Constructor.js", TestType.All)]
        [TestCase(TestClassNameStr2, "GetScriptedInt", "ScriptedMethod.js", TestType.All)]
        [TestCase(TestClassNameStr2, ".ctor", "ScriptedConstructor.js", TestType.All)]
        [TestCase("EnumUsingClass", "GetStr", "EnumToString.js", TestType.All)]
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
