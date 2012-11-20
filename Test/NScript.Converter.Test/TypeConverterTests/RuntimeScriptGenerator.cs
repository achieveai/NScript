//-----------------------------------------------------------------------
// <copyright file="RuntimeScriptGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System.Collections.Generic;
    using NScript.CLR.Test;
    using NScript.Converter.DependencyBuilder;
    using MbUnit.Framework;
    using Mono.Cecil;

    /// <summary>
    /// Definition for RuntimeScriptGenerator
    /// </summary>
    [TestFixture]
    public class RuntimeScriptGenerator
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "NScript.Converter.Test.TypeConverterTests.RuntimeScript.";

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
        [Row(true)]
        public void TestDependencies(bool isDebug)
        {
            TypeDefinition[] typeDefinitions =
                new TypeDefinition[]
                {
                    TestAssemblyLoader.GetTypeReference(
                        "SecondOrderInterfaceInherit",
                        isDebug).Resolve(),
                    TestAssemblyLoader.GetTypeReference(
                        "InheritInterface",
                        isDebug).Resolve(),
                    TestAssemblyLoader.GetTypeReference(
                        "SimpleInterface",
                        isDebug).Resolve(),
                };

            List<TypeDefinition> result =
                InheritanceDependencyBuilder.GetTypesByInheritanceOrder(
                    typeDefinitions);

            Assert.LessThan(
                result.IndexOf(typeDefinitions[1]),
                result.IndexOf(typeDefinitions[0]));

            Assert.LessThan(
                result.IndexOf(typeDefinitions[2]),
                result.IndexOf(typeDefinitions[1]));
        }

        [Test]
        [Row("SimpleFullScript.js",
             TestType.All,
             new[]{
                 "SecondOrderInterfaceInherit",
                 "InheritInterface",
                 "SimpleInterface"})]
        [Row("EnumUsingClassScript.js",
             TestType.All,
             new[]{
                 "SimpleEnumType",
                 "EnumUsingClass"})]
        [Row("FullSpectrumScript.js",
             TestType.All,
             new[]{
                 "InheritDerivedInterface",
                 "SimpleInheritedInterface",
                 "SecondOrderInterfaceInherit",
                 "InheritInterface",
                 "SimpleInterface",
                 "MultipleConstructorsType",
                 "SameNameInstanceAndStaticMethod",
                 "VirtualBase",
                 "VirtualOverride",
                 "SimpleInstanceType",
                 "SimpleStaticType",
                 "StaticConstructorType",
                 "SimpleEnumType",
                 "EnumUsingClass"})]
        [Row("GenericTypeConversionFull.js",
             TestType.Retail,
             new[]{
                 "List`1",
                 "GenericSamplesList",
                 "GenericSamples",
                 "TestGeneric"})]
        [Row("GenericTypeConversionFullDebug.js",
             TestType.Debug,
             new[]{
                 "List`1",
                 "GenericSamplesList",
                 "GenericSamples",
                 "TestGeneric"})]
        public void Test(string resourceName, TestType testType, params string[] classNames)
        {
            ScriptConverterHelper.RunTest(
                RuntimeScriptGenerator.TestFilesNSStr + resourceName,
                TestType.All,
                false,
                classNames);
        }

        [Test]
        [Row("SimpleFullScript.js",
             TestType.All,
             new[]{
                 "SecondOrderInterfaceInherit",
                 "InheritInterface",
                 "SimpleInterface"})]
        [Row("EnumUsingClassScript.js",
             TestType.All,
             new[]{
                 "SimpleEnumType",
                 "EnumUsingClass"})]
        [Row("FullSpectrumScript.js",
             TestType.All,
             new[]{
                 "InheritDerivedInterface",
                 "SimpleInheritedInterface",
                 "SecondOrderInterfaceInherit",
                 "InheritInterface",
                 "SimpleInterface",
                 "MultipleConstructorsType",
                 "SameNameInstanceAndStaticMethod",
                 "VirtualBase",
                 "VirtualOverride",
                 "SimpleInstanceType",
                 "SimpleStaticType",
                 "StaticConstructorType",
                 "SimpleEnumType",
                 "EnumUsingClass"})]
        [Row("GenericTypeConversionFullMcs.js",
             TestType.All,
             new[]{
                 "List`1",
                 "GenericSamplesList",
                 "GenericSamples",
                 "TestGeneric"})]
        public void TestMcs(string resourceName, TestType testType, params string[] classNames)
        {
            ScriptConverterHelper.RunTest(
                RuntimeScriptGenerator.TestFilesNSStr + resourceName,
                TestType.All,
                true,
                classNames);
        }
    }
}