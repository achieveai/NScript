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
    using NScript.Csc.Lib.Test;

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
        [DataRow("EnumUsingClass.js", TestType.Debug, new []{"EnumUsingClass"})]
        [DataRow("FullInheritDerivedInterface.js", TestType.Debug, new []{"SimpleInterface", "SimpleInheritedInterface", "InheritDerivedInterface"})]
        [DataRow("InheritDerivedInterface.js", TestType.Debug, new []{"InheritDerivedInterface"})]
        [DataRow("InheritInterface.js", TestType.Debug, new []{"InheritInterface"})]
        [DataRow("MultipleConstructorsType.js", TestType.Debug, new []{"MultipleConstructorsType"})]
        [DataRow("SameNameInstanceAndStaticMethod.js", TestType.Debug, new []{"SameNameInstanceAndStaticMethod"})]
        [DataRow("SameNameInstanceAndStaticMethod.static.js", TestType.Debug, new []{"SameNameInstanceAndStaticMethod"}, true)]
        [DataRow("SecondOrderInterfaceInherit.js", TestType.Debug, new []{"SecondOrderInterfaceInherit"})]
        [DataRow("SecondOrderInterfaceInherit.static.js", TestType.Debug, new []{"SecondOrderInterfaceInherit"}, true)]
        [DataRow("SimpleEnumType.js", TestType.Debug, new []{"SimpleEnumType"})]
        [DataRow("SimpleEnumType.js", TestType.Debug, new []{"SimpleEnumType"}, true)]
        [DataRow("SimpleFullVirtual.js", TestType.Debug, new []{"VirtualBase", "VirtualOverride"})]
        [DataRow("SimpleFullVirtual.static.js", TestType.Debug, new []{"VirtualBase", "VirtualOverride"}, true)]
        [DataRow("SimpleInstanceType.js", TestType.Debug, new []{"SimpleInstanceType"})]
        [DataRow("SimpleInstanceType.static.js", TestType.Debug, new []{"SimpleInstanceType"}, true)]
        [DataRow("SimpleStaticType.js", TestType.Debug, new []{"SimpleStaticType"})]
        [DataRow("SimpleStaticType.js", TestType.Debug, new []{"SimpleStaticType"}, true)]
        [DataRow("StructClassTypeMcs.js", TestType.Retail, new []{"StructClass", "StructClass2"})]
        [DataRow("StructClassTypeMcs.js", TestType.Retail, new []{"StructClass", "StructClass2"}, true)]
        [DataRow("VirtualBase.js", TestType.Debug, new []{"VirtualBase"})]
        [DataRow("VirtualBase.static.js", TestType.Debug, new []{"VirtualBase"}, true)]
        [DataRow("VirtualOverride.js", TestType.Debug, new []{"VirtualOverride"})]
        [DataRow("VirtualOverride.static.js", TestType.Debug, new []{"VirtualOverride"}, true)]
        public void TestMcs(
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
