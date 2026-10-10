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
    using NScript.Csc.Lib.Test;
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

        /// <summary>
        /// A generic instance must be initialized after the instances its type arguments name,
        /// even when the argument is another object for the same type: Cecil makes a new
        /// GenericInstanceType per reference, so KeyValuePair&lt;String, Action&lt;String&gt;&gt;
        /// rarely holds the very Action&lt;String&gt; the analyzer was given. Out of order, the
        /// bundle reads typeId of an undefined Action&lt;String&gt; while loading.
        /// </summary>
        [TestMethod]
        public void OrderedGenericTypes_ArgumentEqualButNotSameObject_ComesFirst()
        {
            var context = new ConverterContext(TestAssemblyLoader.Context);
            var corlib = context.ClrKnownReferences.String.Resolve().Module;
            var action = corlib.GetType("System.Action`1");
            var pair = corlib.GetType("System.Collections.Generic.KeyValuePair`2");
            Assert.IsNotNull(action, "fixture mscorlib has Action`1");
            Assert.IsNotNull(pair, "fixture mscorlib has KeyValuePair`2");

            GenericInstanceType Instance(TypeDefinition definition, params TypeReference[] arguments)
            {
                var instance = new GenericInstanceType(definition);
                foreach (var argument in arguments)
                {
                    instance.GenericArguments.Add(argument);
                }

                return instance;
            }

            var actionOfString = Instance(action, context.ClrKnownReferences.String);
            var pairOfAction = Instance(pair, context.ClrKnownReferences.String, Instance(action, context.ClrKnownReferences.String));

            var ordered = new DependencyAnalyzer(context).GetOrderedGenericTypeDependencies(
                new TypeReference[] { actionOfString, pairOfAction });

            CollectionAssert.AreEqual(new TypeReference[] { actionOfString, pairOfAction }, ordered);
        }
    }
}
