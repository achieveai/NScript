//-----------------------------------------------------------------------
// <copyright file="DynamicConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for DynamicConverterTests
    /// </summary>
    [TestFixture]
    public class DynamicConverterTests
    {
        public const string TestClassNameStr = @"DynamicTest";
        public const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.DynamicConverters.";

        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [TestCase(TestClassNameStr, "CheckIndex", "CheckIndex.js", TestType.All)]
        [TestCase(TestClassNameStr, "CheckMethod", "CheckMethod.js", TestType.All)]
        [TestCase(TestClassNameStr, "GetArray", "GetArray.js", TestType.All)]
        [TestCase(TestClassNameStr, "GetDictionary", "GetDictionary.js", TestType.All)]
        [TestCase(TestClassNameStr, "GetFoo", "GetFoo.js", TestType.All)]
        [TestCase(TestClassNameStr, "GetList", "GetList.js", TestType.All)]
        [TestCase(TestClassNameStr, "GetSomething", "GetSomething.js", TestType.All)]
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
