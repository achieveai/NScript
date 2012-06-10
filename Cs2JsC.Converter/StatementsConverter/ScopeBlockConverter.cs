//-----------------------------------------------------------------------
// <copyright file="ScopeBlockConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for ScopeBlockConverter
    /// </summary>
    public static class ScopeBlockConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="scopeBlock">The scope block.</param>
        /// <returns>Statement block for given scope block.</returns>
        public static JST.Statement Convert(
            MethodConverter converter,
            ScopeBlock scopeBlock)
        {
            converter.PushScopeBlock(scopeBlock);
            try
            {
                return ScopeBlockConverter.Convert(converter, (ExplicitBlock)scopeBlock);
            }
            finally
            {
                converter.PopScopeBlock();
            }
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="explicitBlock">The scope block.</param>
        /// <returns>Statement block for given scope block.</returns>
        public static JST.Statement Convert(
            MethodConverter converter,
            ExplicitBlock explicitBlock)
        {
            if (explicitBlock.Statements.Count > 1)
            {
                List<JST.Statement> statements = new List<JST.Statement>();

                foreach (var statement in explicitBlock.Statements)
                {
                    statements.Add(
                        StatementConverterBase.Convert(
                            converter,
                            statement));
                }

                return new JST.ScopeBlock(
                    explicitBlock.Location,
                    converter.Scope,
                    statements);
            }

            if (explicitBlock.Statements.Count == 1)
            {
                return StatementConverterBase.Convert(converter, (Statement)explicitBlock.Statements[0]);
            }

            return null;
        }
    }
}
