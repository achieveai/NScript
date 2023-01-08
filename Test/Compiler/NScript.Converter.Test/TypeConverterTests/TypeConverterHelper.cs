//-----------------------------------------------------------------------
// <copyright file="TypeConverterHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System.IO;
    using System.Text;
    using NScript.CLR.Test;
    using NScript.Converter.TypeSystemConverter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TypeConverterHelper
    /// </summary>
    public static class TypeConverterHelper
    {
        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="testJsFile">The test js file.</param>
        /// <param name="testType">Type of the test.</param>
        /// <param name="classNames">Name of the class.</param>
        public static void RunTest(
            string testJsFile,
            TestType testType,
            params string[] classNames)
        {
            RunTest(
                testJsFile,
                testType,
                false,
                false,
                classNames);
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="testJsFile">The test js file.</param>
        /// <param name="testType">Type of the test.</param>
        /// <param name="classNames">Name of the class.</param>
        public static void RunTest(
            string testJsFile,
            TestType testType,
            bool isMcs,
            bool instanceAsStatic,
            params string[] classNames)
        {
            string script = ConverterTestHelpers.GetResourceString(testJsFile);

            if ((testType & TestType.Debug) != 0)
            {
                TypeConverterHelper.RunTest(
                    script,
                    true,
                    classNames,
                    isMcs,
                    instanceAsStatic);
            }

            if ((testType & TestType.Retail) != 0)
            {
                TypeConverterHelper.RunTest(
                    script,
                    false,
                    classNames,
                    isMcs,
                    instanceAsStatic);
            }
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="testJsFile">The test js file.</param>
        /// <param name="testType">Type of the test.</param>
        /// <param name="methodTuples">The method tuples.</param>
        public static void RunTest(
            string testJsFile,
            TestType testType,
            bool instanceAsStatic,
            params Tuple<string, string>[] methodTuples)
        {
            string script = ConverterTestHelpers.GetResourceString(testJsFile);

            if ((testType & TestType.Debug) != 0)
            {
                TypeConverterHelper.RunTest(
                    script,
                    true,
                    methodTuples,
                    instanceAsStatic);
            }

            if ((testType & TestType.Retail) != 0)
            {
                TypeConverterHelper.RunTest(
                    script,
                    false,
                    methodTuples,
                    instanceAsStatic);
            }
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        private static void RunTest(
            string script,
            bool isDebug,
            string[] classNames,
            bool isMcs = false,
            bool instaceAsStatic = false)
        {
            List<JST.Statement> statements = new List<JST.Statement>();
            RuntimeScopeManager runtimeScopeManager = new RuntimeScopeManager(
                isMcs
                ? ConverterTestHelpers.McsContext
                : ConverterTestHelpers.DasmContext,
                instaceAsStatic);

            foreach (var className in classNames)
            {
                TypeReference typeReference =
                    TestAssemblyLoader.GetTypeReference(
                        className,
                        isDebug);

                TypeConverter typeConverter = TypeConverter.Create(
                    runtimeScopeManager,
                    typeReference.Resolve());

                statements.AddRange(
                    typeConverter.Convert(
                        (a, b) => { return; }));
            }

            string functionStr = ConverterTestHelpers.GetScriptString(statements);
            ConverterTestHelpers.CheckScriptValues(
                script,
                functionStr);
        }

        private static void RunTest(
            string script,
            bool isDebug,
            Tuple<string, string>[] methodTuples,
            bool instanceAsStatic)
        {
            MethodDefinition[] methodDefinitions = new MethodDefinition[methodTuples.Length];
            for (int tupleIndex = 0; tupleIndex < methodTuples.Length; tupleIndex++)
            {
                methodDefinitions[tupleIndex] = TestAssemblyLoader.GetMethodDefinition(
                    methodTuples[tupleIndex].Item1,
                    methodTuples[tupleIndex].Item2,
                    isDebug);
            }

            RuntimeScopeManager scopeManager = new RuntimeScopeManager(
                new ConverterContext(TestAssemblyLoader.Context),
                instanceAsStatic);
            var statements = scopeManager.Convert(methodDefinitions);

            ConverterTestHelpers.CheckScriptValues(
                script,
                ConverterTestHelpers.GetScriptString(
                    statements));
        }
    }
}
