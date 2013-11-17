//-----------------------------------------------------------------------
// <copyright file="JsExecutionHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.JsExecutionTests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using NScript.CLR.Test;
    using NScript.Converter.ExpressionsConverter;
    using NScript.Converter.TypeSystemConverter;
    using NUnit.Framework;
    using Mono.Cecil;
    using Noesis.Javascript;

    /// <summary>
    /// Definition for JsExecutionHelper
    /// </summary>
    public static class JsExecutionHelper
    {
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <param name="classNames">The class names.</param>
        /// <returns>Script for given classes.</returns>
        public static string GetScript(
            bool isDebug,
            bool isMcs,
            Tuple<string, string> entryPoint)
        {
            RuntimeScopeManager runtimeScopeManager = new RuntimeScopeManager(
                new ConverterContext(isMcs ? TestAssemblyLoader.McsContext : TestAssemblyLoader.Context));
            MethodDefinition methodDefinition = TestAssemblyLoader.GetMethodDefinition(
                entryPoint.Item1,
                entryPoint.Item2,
                isDebug,
                isMcs);

            List<JST.Statement> statements =
                runtimeScopeManager.Convert(new MethodDefinition[] { methodDefinition });

            MethodCallContext callContext = new MethodCallContext(
                methodDefinition,
                null,
                runtimeScopeManager.Scope);

            statements.Add(
                new JST.ExpressionStatement(
                    null,
                    runtimeScopeManager.Scope,
                    MethodCallExpressionConverter.CreateMethodCallExpression(
                        callContext,
                        new JST.Expression[0],
                        runtimeScopeManager.GetTypeConverter(methodDefinition.DeclaringType.Resolve()),
                        runtimeScopeManager)));

            return ConverterTestHelpers.GetScriptString(statements);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="isDebug">if set to <c>true</c> [is debug].</param>
        /// <param name="entryPoint">The class names.</param>
        public static void ExecuteScript(
            string output,
            bool isDebug,
            Tuple<string, string> entryPoint,
            bool isMcs = false)
        {
            JsConsole jsConsole = new JsConsole();
            // Jint.JintEngine engine = new Jint.JintEngine(Jint.Options.Ecmascript3);
            using (JavascriptContext engine = new JavascriptContext())
            {
                string script = JsExecutionHelper.GetScript(
                    isDebug,
                    isMcs,
                    entryPoint);
                System.Diagnostics.Debug.WriteLine("==================== Script generated ====================");
                System.Diagnostics.Debug.WriteLine(script);
                // engine.SetDebugMode(true);
                engine.SetParameter("console", jsConsole);
                engine.Run(script);
            }

            string outputContent = "NScript.Converter.Test.JsExecutionTests.Results." + output + ".txt";
            outputContent = ConverterTestHelpers.GetResourceString(outputContent);

            if (!string.IsNullOrEmpty(outputContent))
            {
                output = outputContent;
            }

            string consoleString = JsExecutionHelper.GetConsoleString(jsConsole);
            System.Diagnostics.Debug.WriteLine("==================== Script Console ====================");
            System.Diagnostics.Debug.WriteLine(consoleString);

            Assert.AreEqual(
                output,
                consoleString);
        }

        /// <summary>
        /// Gets the test file.
        /// </summary>
        /// <param name="testFile">The test file.</param>
        /// <returns>Text from the testFile.</returns>
        public static string GetTestFile(string testFile)
        {
            string script = null;
            using (var stream = typeof(ConverterTestHelpers)
                .Assembly.GetManifestResourceStream(testFile))
            {
                if (stream != null)
                {
                    System.IO.TextReader reader = new System.IO.StreamReader(stream);
                    script = reader.ReadToEnd().Trim();
                }
            }

            return script;
        }

        /// <summary>
        /// Gets the console string.
        /// </summary>
        /// <param name="jsConsole">The js console.</param>
        /// <returns>JsConsole string.</returns>
        private static string GetConsoleString(JsConsole jsConsole)
        {
            StringBuilder sb = new StringBuilder();
            jsConsole.LogLines.ForEach(str => sb.AppendLine(str));
            return sb.ToString().Trim();
        }
    }
}