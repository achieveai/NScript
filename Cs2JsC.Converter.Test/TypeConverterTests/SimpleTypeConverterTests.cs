//-----------------------------------------------------------------------
// <copyright file="SimpleTypeConverterTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using MbUnit.Framework;
    using Cs2JsC.CLR.Test;

    /// <summary>
    /// Definition for SimpleTypeConverterTests
    /// </summary>
    [TestFixture]
    public class SimpleTypeConverterTests
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

        [Test]
        [Row("SimpleStaticType.js", TestType.All, new []{"SimpleStaticType"})]
        [Row("SimpleInstanceType.js", TestType.All, new []{"SimpleInstanceType"})]
        [Row("SimpleEnumType.js", TestType.All, new []{"SimpleEnumType"})]
        [Row("MultipleConstructorsType.js", TestType.All, new []{"MultipleConstructorsType"})]
        [Row("SameNameInstanceAndStaticMethod.js", TestType.All, new []{"SameNameInstanceAndStaticMethod"})]
        [Row("VirtualOverride.js", TestType.All, new []{"VirtualOverride"})]
        [Row("VirtualBase.js", TestType.All, new []{"VirtualBase"})]
        [Row("SimpleFullVirtual.js", TestType.All, new []{"VirtualBase", "VirtualOverride"})]
        [Row("InheritInterface.js", TestType.All, new []{"InheritInterface"})]
        [Row("InheritDerivedInterface.js", TestType.All, new []{"InheritDerivedInterface"})]
        [Row("SecondOrderInterfaceInherit.js", TestType.All, new []{"SecondOrderInterfaceInherit"})]
        [Row("FullInheritDerivedInterface.js", TestType.All, new []{"SimpleInterface", "SimpleInheritedInterface", "InheritDerivedInterface"})]
        [Row("EnumUsingClass.js", TestType.All, new []{"EnumUsingClass"})]
        [Row("StructClassType.js", TestType.Retail, new []{"StructClass", "StructClass2"})]
        public void Test(string resourceName, TestType testType, params string[] classNames)
        {
            TypeConverterHelper.RunTest(
                TestFilesNSStr + resourceName,
                testType,
                false,
                classNames);
        }

        [Test]
        [Row("SimpleStaticType.js", TestType.All, new []{"SimpleStaticType"})]
        [Row("SimpleInstanceType.js", TestType.All, new []{"SimpleInstanceType"})]
        [Row("SimpleEnumType.js", TestType.All, new []{"SimpleEnumType"})]
        [Row("MultipleConstructorsType.js", TestType.All, new []{"MultipleConstructorsType"})]
        [Row("SameNameInstanceAndStaticMethod.js", TestType.All, new []{"SameNameInstanceAndStaticMethod"})]
        [Row("VirtualOverride.js", TestType.All, new []{"VirtualOverride"})]
        [Row("VirtualBase.js", TestType.All, new []{"VirtualBase"})]
        [Row("SimpleFullVirtual.js", TestType.All, new []{"VirtualBase", "VirtualOverride"})]
        [Row("InheritInterface.js", TestType.All, new []{"InheritInterface"})]
        [Row("InheritDerivedInterface.js", TestType.All, new []{"InheritDerivedInterface"})]
        [Row("SecondOrderInterfaceInherit.js", TestType.All, new []{"SecondOrderInterfaceInherit"})]
        [Row("FullInheritDerivedInterface.js", TestType.All, new []{"SimpleInterface", "SimpleInheritedInterface", "InheritDerivedInterface"})]
        [Row("EnumUsingClass.js", TestType.All, new []{"EnumUsingClass"})]
        [Row("StructClassTypeMcs.js", TestType.Retail, new []{"StructClass", "StructClass2"})]
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
