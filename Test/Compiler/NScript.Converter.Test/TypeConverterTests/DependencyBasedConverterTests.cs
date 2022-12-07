//-----------------------------------------------------------------------
// <copyright file="DependencyBasedConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Csc.Lib.Test;

    /// <summary>
    /// Definition for DependencyBasedConverterTests
    /// </summary>
    [TestClass]
    public class DependencyBasedConverterTests
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "NScript.Converter.Test.TypeConverterTests.DependencyBasedConverter.";

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
        public void TestSimpleStaticDependencyType()
        {
            TypeConverterHelper.RunTest(
                DependencyBasedConverterTests.TestFilesNSStr + "SimpleStaticDependencyType.js",
                TestType.All,
                Tuple.Create("SimpleStaticType", "GetField"));
        }

        /// <summary>
        /// Tests the type of the simple static.
        /// </summary>
        [DataTestMethod]
        public void TestSimpleInstanceDependencyType()
        {
            TypeConverterHelper.RunTest(
                DependencyBasedConverterTests.TestFilesNSStr + "SimpleInstanceDependencyType.js",
                TestType.All,
                Tuple.Create("SimpleInstanceType", "GetField"));
        }
    }
}
