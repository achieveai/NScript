//-----------------------------------------------------------------------
// <copyright file="JSParserAndGeneratorHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JSParser.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NScript.JST;
    using System.IO;
    using MbUnit.Framework;

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
            IdentifierScope globalScope = new IdentifierScope(true);
            IdentifierScope scope = new IdentifierScope(
                globalScope,
                argsNames,
                false);

            ScopeBlock block = Parser.Parse(
                jsIn,
                scope,
                new TestTypeResolver(scope));

            JSWriter writer = new JSWriter(true, false);

            if (scope.UsedLocalIdentifiers.Count > 0)
            {
                writer.WriteNewLine()
                    .Write(Keyword.Var);

                for (int identifierIndex = 0; identifierIndex < scope.UsedLocalIdentifiers.Count; identifierIndex++)
                {
                    if (identifierIndex > 0)
                    {
                        writer.Write(Symbols.Comma);
                    }

                    writer.Write(scope.UsedLocalIdentifiers[identifierIndex]);
                }

                writer.Write(Symbols.SemiColon);
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
    }
}
