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
        [TestCase("SimpleStaticType.js", TestType.All, new []{"SimpleStaticType"})]
        [TestCase("SimpleInstanceType.js", TestType.All, new []{"SimpleInstanceType"})]
        [TestCase("SimpleEnumType.js", TestType.All, new []{"SimpleEnumType"})]
        [TestCase("MultipleConstructorsType.js", TestType.All, new []{"MultipleConstructorsType"})]
        [TestCase("SameNameInstanceAndStaticMethod.js", TestType.All, new []{"SameNameInstanceAndStaticMethod"})]
        [TestCase("VirtualOverride.js", TestType.All, new []{"VirtualOverride"})]
        [TestCase("VirtualBase.js", TestType.All, new []{"VirtualBase"})]
        [TestCase("SimpleFullVirtual.js", TestType.All, new []{"VirtualBase", "VirtualOverride"})]
        [TestCase("InheritInterface.js", TestType.All, new []{"InheritInterface"})]
        [TestCase("InheritDerivedInterface.js", TestType.All, new []{"InheritDerivedInterface"})]
        [TestCase("SecondOrderInterfaceInherit.js", TestType.All, new []{"SecondOrderInterfaceInherit"})]
        [TestCase("FullInheritDerivedInterface.js", TestType.All, new []{"SimpleInterface", "SimpleInheritedInterface", "InheritDerivedInterface"})]
        [TestCase("EnumUsingClass.js", TestType.All, new []{"EnumUsingClass"})]
        [TestCase("StructClassType.js", TestType.Retail, new []{"StructClass", "StructClass2"})]
        public void Test(string resourceName, TestType testType, params string[] classNames)
        {
            TypeConverterHelper.RunTest(
                TestFilesNSStr + resourceName,
                testType,
                false,
                classNames);
        }

        [Test]
        [TestCase("SimpleStaticType.js", TestType.All, new []{"SimpleStaticType"})]
        [TestCase("SimpleInstanceType.js", TestType.All, new []{"SimpleInstanceType"})]
        [TestCase("SimpleEnumType.js", TestType.All, new []{"SimpleEnumType"})]
        [TestCase("MultipleConstructorsType.js", TestType.All, new []{"MultipleConstructorsType"})]
        [TestCase("SameNameInstanceAndStaticMethod.js", TestType.All, new []{"SameNameInstanceAndStaticMethod"})]
        [TestCase("VirtualOverride.js", TestType.All, new []{"VirtualOverride"})]
        [TestCase("VirtualBase.js", TestType.All, new []{"VirtualBase"})]
        [TestCase("SimpleFullVirtual.js", TestType.All, new []{"VirtualBase", "VirtualOverride"})]
        [TestCase("InheritInterface.js", TestType.All, new []{"InheritInterface"})]
        [TestCase("InheritDerivedInterface.js", TestType.All, new []{"InheritDerivedInterface"})]
        [TestCase("SecondOrderInterfaceInherit.js", TestType.All, new []{"SecondOrderInterfaceInherit"})]
        [TestCase("FullInheritDerivedInterface.js", TestType.All, new []{"SimpleInterface", "SimpleInheritedInterface", "InheritDerivedInterface"})]
        [TestCase("EnumUsingClass.js", TestType.All, new []{"EnumUsingClass"})]
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
