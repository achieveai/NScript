//-----------------------------------------------------------------------
// <copyright file="DependencyAnalyzerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NScript.CLR.Test;
    using NScript.Converter.TypeSystemConverter;
    using Mono.Cecil;
    using FluentAssertions;

    /// <summary>
    /// Definition for DependencyAnalyzerTests
    /// </summary>
    [TestFixture]
    public class DependencyAnalyzerTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            TestAssemblyLoader.LoadAssemblies();
        }

        [Test]
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
