//-----------------------------------------------------------------------
// <copyright file="SimpleTypeConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.CLR.Test;

    /// <summary>
    /// Definition for SimpleTypeConverterTests
    /// </summary>
    [TestClass]
    public class SimpleTypeConverterTests
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

        [DataTestMethod]
        [DataRow("SimpleStaticType.js", TestType.Debug, new []{"SimpleStaticType"})]
        [DataRow("SimpleInstanceType.js", TestType.Debug, new []{"SimpleInstanceType"})]
        [DataRow("SimpleEnumType.js", TestType.Debug, new []{"SimpleEnumType"})]
        [DataRow("MultipleConstructorsType.js", TestType.Debug, new []{"MultipleConstructorsType"})]
        [DataRow("SameNameInstanceAndStaticMethod.js", TestType.Debug, new []{"SameNameInstanceAndStaticMethod"})]
        [DataRow("VirtualOverride.js", TestType.Debug, new []{"VirtualOverride"})]
        [DataRow("VirtualBase.js", TestType.Debug, new []{"VirtualBase"})]
        [DataRow("SimpleFullVirtual.js", TestType.Debug, new []{"VirtualBase", "VirtualOverride"})]
        [DataRow("InheritInterface.js", TestType.Debug, new []{"InheritInterface"})]
        [DataRow("InheritDerivedInterface.js", TestType.Debug, new []{"InheritDerivedInterface"})]
        [DataRow("SecondOrderInterfaceInherit.js", TestType.Debug, new []{"SecondOrderInterfaceInherit"})]
        [DataRow("FullInheritDerivedInterface.js", TestType.Debug, new []{"SimpleInterface", "SimpleInheritedInterface", "InheritDerivedInterface"})]
        [DataRow("EnumUsingClass.js", TestType.Debug, new []{"EnumUsingClass"})]
        [DataRow("StructClassTypeMcs.js", TestType.Retail, new []{"StructClass", "StructClass2"})]
        public void TestMcs(string resourceName, TestType testType, string[] classNames)
        {
            TypeConverterHelper.RunTest(
                TestFilesNSStr + resourceName,
                testType,
                true,
                classNames);
        }
    }
}
