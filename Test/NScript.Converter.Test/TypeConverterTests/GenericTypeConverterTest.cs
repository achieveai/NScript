//-----------------------------------------------------------------------
// <copyright file="GenericTypeConverterTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using NScript.CLR.Test;
    using NUnit.Framework;

    /// <summary>
    /// Definition for GenericTypeConverterTest
    /// </summary>
    [TestFixture]
    public class GenericTypeConverterTest
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "NScript.Converter.Test.TypeConverterTests.SimpleTypeConverter.";

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        /// <summary>
        /// Tests the type of the simple static.
        /// </summary>
        [Test]
        [TestCase(TestType.Debug)]
        // [TestCase(TestType.Retail)]
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