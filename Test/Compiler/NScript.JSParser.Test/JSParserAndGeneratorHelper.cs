//-----------------------------------------------------------------------
// <copyright file="JSParserAndGeneratorHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.JST;
    using System.IO;

    /// <summary>
    /// Definition for JSParserAndGeneratorHelper
    /// </summary>
    public static class JSParserAndGeneratorHelper
    {
        public static void ParseAndGenerateTest(
            string jsIn,
            string jsOut,
            params string[] argsNames)
        {
            ParseAndGenerateTest(
                jsIn,
                jsOut,
                false,
                argsNames);
        }

        public static void ParseAndGenerateTest(
            string jsIn,
            string jsOut,
            bool isOptimized,
            params string[] argsNames)
        {
            IdentifierScope globalScope = new IdentifierScope(true);
            IdentifierScope scope = new IdentifierScope(
                globalScope,
                argsNames,
                false);

            ScopeBlock block = Parser.Parse(
                jsIn,
                scope,
                new TestTypeResolver(scope));

            JSWriter writer = new JSWriter(true, isOptimized);

            if (scope.UsedLocalIdentifiers.Count > 0)
            {
                int realIdentifiers = 0;
                for (int identifierIndex = 0; identifierIndex < scope.UsedLocalIdentifiers.Count; identifierIndex++)
                {
                    if (!scope.UsedLocalIdentifiers[identifierIndex].IsFunctionName)
                    { realIdentifiers++; }
                }

                if (realIdentifiers > 0)
                {
                    writer.Write(Keyword.Var);

                    for (int identifierIndex = 0, writtenIdentifiers = 0; identifierIndex < scope.UsedLocalIdentifiers.Count; identifierIndex++)
                    {
                        if (scope.UsedLocalIdentifiers[identifierIndex].IsFunctionName)
                        { continue; }

                        if (writtenIdentifiers++ > 0)
                        {
                            writer.Write(Symbols.Comma);
                        }

                        writer.Write(scope.UsedLocalIdentifiers[identifierIndex]);
                    }

                    writer.Write(Symbols.SemiColon);
                }
            }

            foreach (var statement in block.Statements)
            {
                statement.Write(writer);
            }

            StringWriter strWriter = new StringWriter();
            writer.Write(strWriter);

            string js = strWriter.ToString().Trim();

            Assert.AreEqual(jsOut, js);
        }

        public static void CompareJstTokens(
            ScopeBlock scopeBlock,
            string resourceName)
        {
            var otherBlock = ReadJstFromResourceScript(resourceName);
            var actualScript = GetJsFromScopeBlock(scopeBlock);
            var expectedScript = GetJsFromScopeBlock(otherBlock);

            if (actualScript != expectedScript)
            {
                Console.WriteLine("====== ExpectedScript ================================> ");
                Console.WriteLine(expectedScript);
                Console.WriteLine("====== ActualScript ==================================> ");
                Console.WriteLine(actualScript);
            }

            Assert.AreEqual(
                expectedScript,
                actualScript);
        }

        public static ScopeBlock ReadJstFromResourceScript(string resourceName)
        {
            var jsScript = GetResourceString(resourceName);
            IdentifierScope globalScope = new IdentifierScope(true);

            return Parser.Parse(
                jsScript,
                globalScope,
                new TestTypeResolver(globalScope));
        }

        public static string GetJsFromScopeBlock(ScopeBlock scopeBlock)
        {
            var leftWriter = new JSWriter(true, false);
            leftWriter.Write(scopeBlock);

            var stringStreamLeft = new StringWriter();
            leftWriter.Write(stringStreamLeft);

            return stringStreamLeft.ToString();
        }

        /// <summary>
        /// Gets the resource string.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>string from resource, if exists, else null.</returns>
        public static string GetResourceString(
            string resourceName)
        {
            using (var stream = typeof(JSParserAndGeneratorHelper)
                .Assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    TextReader reader = new StreamReader(stream);
                    return reader.ReadToEnd().Trim();
                }
            }

            return null;
        }
    }
}
