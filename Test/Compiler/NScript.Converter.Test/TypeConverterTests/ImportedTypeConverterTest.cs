//-----------------------------------------------------------------------
// <copyright file="ImportedTypeConverterTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for ImportedTypeConverterTest
    /// </summary>
    [TestClass]
    public class ImportedTypeConverterTest
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "NScript.Converter.Test.TypeConverterTests.SimpleTypeConverter.";

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        /// <summary>
        /// Tests the type of the simple static.
        /// </summary>
        [DataTestMethod]
        [DataRow("TestJsonType.js",
             TestType.All,
             new[]{"TestJsonType"})]
        [DataRow("TestImportedType.js",
             TestType.All,
             new[]{"TestImportedType"})]
        [DataRow("TestImportedGeneric.js",
             TestType.All,
             new[]{"ImportedGeneric`1"})]
        [DataRow("PsudoTypeSimple.js",
             TestType.All,
             new[]{"PsudoUsage"})]
        public void TestImportedTypes(string resourceName, TestType testType, params string[] classNames)
        {
            ScriptConverterHelper.RunTest(
                ImportedTypeConverterTest.TestFilesNSStr + resourceName,
                TestType.All,
                true,
                classNames);
        }
    }
}
