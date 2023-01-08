//-----------------------------------------------------------------------
// <copyright file="GenericTypeConverterTest.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using NScript.Csc.Lib.Test;
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
        [DataRow("MultipleArgumentGeneric.js", TestType.Debug, new []{"MultiArgGeneric`2", "MultiArgGeneric`3"}, false)]
        [DataRow("MultipleArgumentGeneric.static.js", TestType.Debug, new []{"MultiArgGeneric`2", "MultiArgGeneric`3"}, true)]
        [DataRow("GenericListType.js", TestType.Debug, new []{"List`1"}, false)]
        [DataRow("GenericListType.static.js", TestType.Debug, new []{"List`1"}, true)]
        [DataRow("SystemGenericListType.js", TestType.Debug, new []{"System.Collections.Generic.List`1"}, false)]
        [DataRow("SystemGenericListType.static.js", TestType.Debug, new []{"System.Collections.Generic.List`1"}, true)]
        [DataRow("GenericInterfaceInheritance.js", TestType.Debug, new []{"TestGenericB"}, false)]
        [DataRow("GenericInterfaceInheritance.static.js", TestType.Debug, new []{"TestGenericB"}, true)]
        public void TestGenericTypeMcs(
            string resourceName,
            TestType testType,
            string[] classNames,
            bool instanceAsStatic = false)
        {
            TypeConverterHelper.RunTest(
                TestFilesNSStr + resourceName,
                testType,
                true,
                instanceAsStatic,
                classNames);
        }
    }
}