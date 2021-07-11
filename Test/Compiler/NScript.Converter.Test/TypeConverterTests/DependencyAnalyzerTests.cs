//-----------------------------------------------------------------------
// <copyright file="DependencyAnalyzerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.CLR.Test;
    using NScript.Converter.TypeSystemConverter;
    using Mono.Cecil;
    using FluentAssertions;

    /// <summary>
    /// Definition for DependencyAnalyzerTests
    /// </summary>
    [TestClass]
    public class DependencyAnalyzerTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [DataTestMethod]
        public void CheckDerivedDependency()
        {
            DependencyAnalyzer dependencyAnalyzer = new DependencyAnalyzer(new ConverterContext(TestAssemblyLoader.Context));
            TypeDefinition typeDefinition = TestAssemblyLoader.GetTypeReference(
                                "SimplImpl",
                                false).Resolve() as TypeDefinition;

            dependencyAnalyzer.AddTypeForAnalysis(typeDefinition);

            dependencyAnalyzer.TypeToTypeReferences[typeDefinition]
                .Should<TypeReference>().Contain(
                    TestAssemblyLoader.GetTypeReference(
                        "Foo",
                        false));

            dependencyAnalyzer.TypeToTypeReferences[typeDefinition]
                .Should<TypeReference>().Contain(
                    TestAssemblyLoader.GetTypeReference(
                        "BaseInterface",
                        false));
        }
    }
}
