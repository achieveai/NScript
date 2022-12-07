//-----------------------------------------------------------------------
// <copyright file="ConverterTestHelpers.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    // using NScript.CLR.Test;
    using NScript.Csc.Lib.Test;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using FluentAssertions;

    [Flags]
    public enum TestType
    {
        Debug = 1,
        Retail = 2,
        All = 3
    }

    /// <summary>
    /// Definition for ConverterTestHelpers
    /// </summary>
    public static class ConverterTestHelpers
    {
        private static ConverterContext dasmContext;
        private static ConverterContext mcsConverterContext;

        /// <summary>
        /// Gets the dasm context.
        /// </summary>
        public static ConverterContext DasmContext
        {
            get
            {
                if (ConverterTestHelpers.dasmContext == null)
                {
                    TestAssemblyLoader.LoadAssemblies();
                    ConverterTestHelpers.dasmContext = new ConverterContext(TestAssemblyLoader.Context);
                }

                return ConverterTestHelpers.dasmContext;
            }
        }

        /// <summary>
        /// Gets the MCS context.
        /// </summary>
        public static ConverterContext McsContext
        {
            get
            {
                if (ConverterTestHelpers.mcsConverterContext == null)
                {
                    TestAssemblyLoader.LoadAssemblies();
                    ConverterTestHelpers.mcsConverterContext = new ConverterContext(TestAssemblyLoader.McsContext);
                }

                return ConverterTestHelpers.mcsConverterContext;
            }
        }

        /// <summary>
        /// Gets the resource string.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>string from resource, if exists, else null.</returns>
        public static string GetResourceString(
            string resourceName)
        {
            using (var stream = typeof(ConverterTestHelpers)
                .Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    System.IO.TextReader reader = new System.IO.StreamReader(stream);
                    return reader.ReadToEnd().Trim();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the script string.
        /// </summary>
        /// <param name="statements">The statements.</param>
        /// <returns>Returns script string.</returns>
        public static string GetScriptString(
            IList<JST.Statement> statements)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriter writer = new StringWriter(strBuilder);
            JST.JSWriter jsWriter = new JST.JSWriter(
                    true,
                    false);

            foreach (var statement in statements)
            {
                statement.Write(jsWriter);
            }

            jsWriter.Write(writer);
            return strBuilder.ToString().Trim();
        }

        /// <summary>
        /// Gets the script string.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns script string.</returns>
        public static string GetScriptString(
            Expression expression)
        {
            StringBuilder strBuilder = new StringBuilder();
            StringWriter writer = new StringWriter(strBuilder);
            JST.JSWriter jsWriter = new JST.JSWriter(
                    true,
                    false);

            expression.Write(jsWriter);

            jsWriter.Write(writer);
            return strBuilder.ToString().Trim();
        }

        /// <summary>
        /// Checks the script values.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="actualValue">The actual value.</param>
        public static void CheckScriptValues(
            string expectedValue,
            string actualValue)
        {
            expectedValue = expectedValue.Trim();
            actualValue = actualValue.Trim();
            if (string.IsNullOrWhiteSpace(expectedValue))
            {
                Console.WriteLine(actualValue);
                Assert.IsNotNull(expectedValue);
            }
            else
            {
                if (expectedValue != actualValue)
                {
                    Console.WriteLine("====== Expected ================================> ");
                    Console.WriteLine(expectedValue);
                    Console.WriteLine("====== Actual ==================================> ");
                    Console.WriteLine(actualValue);
                }
            }

            Assert.AreEqual(
                expectedValue,
                actualValue);
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="testType">Type of the test.</param>
        /// <param name="script">The script.</param>
        public static void RunTest(
            string className,
            string methodName,
            TestType testType,
            string script,
            bool isMcs)
        {
            if ((testType & TestType.Debug) != 0)
            {
                ConverterTestHelpers.RunTest(
                    className,
                    methodName,
                    true,
                    script,
                    isMcs);
            }

            if ((testType & TestType.Retail) != 0)
            {
                ConverterTestHelpers.RunTest(
                    className,
                    methodName,
                    false,
                    script,
                    isMcs);
            }
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="testType">Type of the test.</param>
        /// <param name="script">The script.</param>
        public static void RunTest(
            string testjsFile,
            string className,
            string methodName,
            TestType testType,
            bool isMcs = false)
        {
            string script = ConverterTestHelpers.GetResourceString(testjsFile);

            if ((testType & TestType.Debug) != 0)
            {
                ConverterTestHelpers.RunTest(
                    className,
                    methodName,
                    true,
                    script,
                    isMcs);
            }

            if ((testType & TestType.Retail) != 0)
            {
                ConverterTestHelpers.RunTest(
                    className,
                    methodName,
                    false,
                    script,
                    isMcs);
            }
        }

        /// <summary>
        /// Runs the test.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <param name="script">The script.</param>
        private static void RunTest(
            string className,
            string methodName,
            bool isDebug,
            string script,
            bool isMcs = false)
        {
            MethodDefinition methodDefinition = TestAssemblyLoader.GetMethodDefinition(
                className,
                methodName,
                isDebug,
                isMcs);

            RuntimeScopeManager runtimeScopeManager = new RuntimeScopeManager(
                isMcs ? ConverterTestHelpers.McsContext : ConverterTestHelpers.DasmContext);

            TypeConverter typeConverter = TypeConverter.Create(
                runtimeScopeManager,
                methodDefinition.DeclaringType);

            MethodConverter methodConverter = new MethodConverter(
                typeConverter,
                methodDefinition);

            var functionExpression = methodConverter.MethodFunctionExpression;
            string functionStr = ConverterTestHelpers.GetScriptString(functionExpression);

            ConverterTestHelpers.CheckScriptValues(
                script,
                functionStr);
        }
    }
}