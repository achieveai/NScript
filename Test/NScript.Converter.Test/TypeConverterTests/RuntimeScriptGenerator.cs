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
    using NUnit.Framework;
    using Mono.Cecil;
    using FluentAssertions;

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
        [TestCase(true)]
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

            result.IndexOf(typeDefinitions[1]).Should().BeLessThan(
                result.IndexOf(typeDefinitions[0]));
            result.IndexOf(typeDefinitions[2]).Should().BeLessThan(
                result.IndexOf(typeDefinitions[1]));
        }

        [Test]
        [TestCase("SimpleFullScript.js",
             TestType.Debug,
             new[]{
                 "SecondOrderInterfaceInherit",
                 "InheritInterface",
                 "SimpleInterface"})]
        [TestCase("EnumUsingClassScript.js",
             TestType.Debug,
             new[]{
                 "SimpleEnumType",
                 "EnumUsingClass"})]
        [TestCase("FullSpectrumScript.js",
             TestType.Debug,
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
        [TestCase("GenericTypeConversionFullMcs.js",
             TestType.Debug,
             new[]{
                 "List`1",
                 "GenericSamplesList",
                 "GenericSamples",
                 "TestGeneric"})]
        public void TestMcs(string resourceName, TestType testType, params string[] classNames)
        {
            ScriptConverterHelper.RunTest(
                RuntimeScriptGenerator.TestFilesNSStr + resourceName,
                testType,
                true,
                classNames);
        }
    }
}