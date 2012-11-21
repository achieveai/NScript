//-----------------------------------------------------------------------
// <copyright file="DependencyAnalyzerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System;
    using System.Collections.Generic;
    using MbUnit.Framework;
    using NScript.CLR.Test;
    using NScript.Converter.TypeSystemConverter;
    using Mono.Cecil;

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

            Assert.Contains(
                dependencyAnalyzer.TypeToTypeReferences[typeDefinition],
                TestAssemblyLoader.GetTypeReference(
                    "Foo",
                    false));

            Assert.Contains(
                dependencyAnalyzer.TypeToTypeReferences[typeDefinition],
                TestAssemblyLoader.GetTypeReference(
                    "BaseInterface",
                    false));
        }
    }
}
