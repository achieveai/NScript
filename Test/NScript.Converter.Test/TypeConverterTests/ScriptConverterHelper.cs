//-----------------------------------------------------------------------
// <copyright file="ScriptConverterHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.TypeConverterTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using NScript.CLR.Test;
    using NScript.Converter.TypeSystemConverter;
    using NUnit.Framework;
    using System;
    using Mono.Cecil;

    /// <summary>
    /// Definition for ScriptConverterHelper
    /// </summary>
    public class ScriptConverterHelper
    {
        public static void RunTest(
            string testJsFile,
            TestType testType,
            bool isMcs,
            params string[] classNames)
        {
            string script = ConverterTestHelpers.GetResourceString(testJsFile);

            if ((testType & TestType.Debug) != 0)
            {
                ScriptConverterHelper.RunTest(
                    script,
                    true,
                    classNames,
                    isMcs);
            }

            if ((testType & TestType.Retail) != 0)
            {
                ScriptConverterHelper.RunTest(
                    script,
                    false,
                    classNames,
                    isMcs);
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
            bool isMcs)
        {
            RuntimeScopeManager runtimeScopeManager = new RuntimeScopeManager(
                new ConverterContext(
                    isMcs
                        ? TestAssemblyLoader.McsContext
                        : TestAssemblyLoader.Context));
            List<TypeDefinition> typeDefinitions = new List<TypeDefinition>();

            foreach (var className in classNames)
            {
                TypeReference typeReference =
                    TestAssemblyLoader.GetTypeReference(
                        className,
                        isDebug);

                typeDefinitions.Add(typeReference.Resolve());
            }

            List<JST.Statement> statements =
                runtimeScopeManager.ConvertForTests(typeDefinitions);
            string functionStr = ConverterTestHelpers.GetScriptString(
                statements);

            ConverterTestHelpers.CheckScriptValues(
                script,
                functionStr);
        }
    }
}