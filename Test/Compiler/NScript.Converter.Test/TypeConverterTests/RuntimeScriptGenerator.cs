//-----------------------------------------------------------------------
// <copyright file="RuntimeScriptGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System.Collections.Generic;
    using NScript.Csc.Lib.Test;
    using NScript.Converter.DependencyBuilder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using FluentAssertions;

    /// <summary>
    /// Definition for RuntimeScriptGenerator
    /// </summary>
    [TestClass]
    public class RuntimeScriptGenerator
    {
        /// <summary>
        /// Namespace for test files.
        /// </summary>
        private const string TestFilesNSStr = "NScript.Converter.Test.TypeConverterTests.RuntimeScript.";

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        [DataRow(true)]
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

            _ = result.IndexOf(typeDefinitions[1]).Should().BeLessThan(
                result.IndexOf(typeDefinitions[0]));
            _ = result.IndexOf(typeDefinitions[2]).Should().BeLessThan(
                result.IndexOf(typeDefinitions[1]));
        }

        [DataTestMethod]
        [DataRow("SimpleFullScript.js",
             TestType.Debug,
             new string[]{
                 "SecondOrderInterfaceInherit",
                 "InheritInterface",
                 "SimpleInterface"})]
        [DataRow("EnumUsingClassScript.js",
             TestType.Debug,
             new string[]{
                 "SimpleEnumType",
                 "EnumUsingClass"})]
        [DataRow("FullSpectrumScript.js",
             TestType.Debug,
             new string[]{
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
        [DataRow("GenericTypeConversionFullMcs.js",
             TestType.Debug,
             new string[]{
                 "List`1",
                 "GenericSamplesList",
                 "GenericSamples",
                 "TestGeneric"})]
        public void TestMcs(string resourceName, TestType testType, string[] classNames)
        {
            ScriptConverterHelper.RunTest(
                RuntimeScriptGenerator.TestFilesNSStr + resourceName,
                testType,
                true,
                classNames);
        }
    }
}