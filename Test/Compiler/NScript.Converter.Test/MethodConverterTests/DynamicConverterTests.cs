//-----------------------------------------------------------------------
// <copyright file="DynamicConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.MethodConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for DynamicConverterTests
    /// </summary>
    [TestClass]
    public class DynamicConverterTests
    {
        public const string TestClassNameStr = @"DynamicTest";
        public const string TestFilesNSStr = @"NScript.Converter.Test.MethodConverterTests.DynamicConverters.";

        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(TestClassNameStr, "CheckIndex", "CheckIndex.js", TestType.All)]
        [DataRow(TestClassNameStr, "CheckMethod", "CheckMethod.js", TestType.All)]
        [DataRow(TestClassNameStr, "GetArray", "GetArray.js", TestType.All)]
        [DataRow(TestClassNameStr, "GetDictionary", "GetDictionary.js", TestType.All)]
        [DataRow(TestClassNameStr, "GetFoo", "GetFoo.js", TestType.All)]
        [DataRow(TestClassNameStr, "GetList", "GetList.js", TestType.All)]
        [DataRow(TestClassNameStr, "GetSomething", "GetSomething.js", TestType.All)]
        public void TestMcs(string className, string methodName, string resourceName, TestType testType)
        {
            ConverterTestHelpers.RunTest(
                TestFilesNSStr + resourceName,
                className,
                methodName,
                testType,
                true,
                false);
        }
    }
}
