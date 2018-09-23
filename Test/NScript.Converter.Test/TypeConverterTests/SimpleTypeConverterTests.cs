//-----------------------------------------------------------------------
// <copyright file="SimpleTypeConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NScript.CLR.Test;

    /// <summary>
    /// Definition for SimpleTypeConverterTests
    /// </summary>
    [TestFixture]
    public class SimpleTypeConverterTests
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

        [Test]
        [TestCase("SimpleStaticType.js", TestType.Debug, new []{"SimpleStaticType"})]
        [TestCase("SimpleInstanceType.js", TestType.Debug, new []{"SimpleInstanceType"})]
        [TestCase("SimpleEnumType.js", TestType.Debug, new []{"SimpleEnumType"})]
        [TestCase("MultipleConstructorsType.js", TestType.Debug, new []{"MultipleConstructorsType"})]
        [TestCase("SameNameInstanceAndStaticMethod.js", TestType.Debug, new []{"SameNameInstanceAndStaticMethod"})]
        [TestCase("VirtualOverride.js", TestType.Debug, new []{"VirtualOverride"})]
        [TestCase("VirtualBase.js", TestType.Debug, new []{"VirtualBase"})]
        [TestCase("SimpleFullVirtual.js", TestType.Debug, new []{"VirtualBase", "VirtualOverride"})]
        [TestCase("InheritInterface.js", TestType.Debug, new []{"InheritInterface"})]
        [TestCase("InheritDerivedInterface.js", TestType.Debug, new []{"InheritDerivedInterface"})]
        [TestCase("SecondOrderInterfaceInherit.js", TestType.Debug, new []{"SecondOrderInterfaceInherit"})]
        [TestCase("FullInheritDerivedInterface.js", TestType.Debug, new []{"SimpleInterface", "SimpleInheritedInterface", "InheritDerivedInterface"})]
        [TestCase("EnumUsingClass.js", TestType.Debug, new []{"EnumUsingClass"})]
        [TestCase("StructClassTypeMcs.js", TestType.Retail, new []{"StructClass", "StructClass2"})]
        public void TestMcs(string resourceName, TestType testType, params string[] classNames)
        {
            TypeConverterHelper.RunTest(
                TestFilesNSStr + resourceName,
                testType,
                true,
                classNames);
        }
    }
}
