//-----------------------------------------------------------------------
// <copyright file="GenericTypeConverterTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using NScript.CLR.Test;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Definition for GenericTypeConverterTest
    /// </summary>
    [TestClass]
    public class GenericTypeConverterTest
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
        [DataRow(TestType.Debug)]
        // [DataRow(TestType.Retail)]
        public void TestGenericTypeMcs(TestType testType)
        {
            TypeConverterHelper.RunTest(
                GenericTypeConverterTest.TestFilesNSStr + "GenericListType.js",
                testType,
                true,
                "List`1");
        }
    }
}