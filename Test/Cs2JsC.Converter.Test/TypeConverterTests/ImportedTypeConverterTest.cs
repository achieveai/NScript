//-----------------------------------------------------------------------
// <copyright file="ImportedTypeConverterTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.Test;
    using MbUnit.Framework;

    /// <summary>
    /// Definition for ImportedTypeConverterTest
    /// </summary>
    [TestFixture]
    public class ImportedTypeConverterTest
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "Cs2JsC.Converter.Test.TypeConverterTests.SimpleTypeConverter.";

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
        public void TestGenericType()
        {
            TypeConverterHelper.RunTest(
                ImportedTypeConverterTest.TestFilesNSStr + "PsudoTypeSimple.js",
                TestType.All,
                "TestPsudoType");
        }
    }
}
