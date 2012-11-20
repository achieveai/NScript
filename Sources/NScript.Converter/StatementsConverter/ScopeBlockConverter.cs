//-----------------------------------------------------------------------
// <copyright file="ScopeBlockConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;

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
            IMethodScopeConverter converter,
            ScopeBlock scopeBlock)
        {
            converter.PushScopeBlock(scopeBlock);
            try
            {
                if (scopeBlock is ScopeBlock
                    && scopeBlock.GetType() == typeof(ScopeBlock))
                {
                    return ScopeBlockConverter.Convert(converter, (ExplicitBlock)scopeBlock);
                }
                else
                {
                    return StatementConverterBase.Convert(converter, scopeBlock);
                }
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
            IMethodScopeConverter converter,
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
