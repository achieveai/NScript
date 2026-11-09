//-----------------------------------------------------------------------
// <copyright file="TryCatchFinallyConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using NScript.Converter.ExpressionsConverter;

    /// <summary>
    /// Definition for TryCatchFinallyConverter
    /// </summary>
    public static class TryCatchFinallyConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>Try -catch-finally block.</returns>
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            TryCatchFinally statement)
        {
            if (statement.Handlers.Count > 2
                || (statement.Handlers.Count == 2
                    && statement.Handlers[1].IsCatchBlock))
            {
                throw new System.NotSupportedException("Currently we only support catching single object");
            }

            JST.CatchHandler catchHandler = null;
            JST.ScopeBlock finallyBlock = null;

            if (statement.Handlers[0].IsCatchBlock)
            {
                catchHandler = TryCatchFinallyConverter.ConvertCatchHandler(
                    converter,
                    statement.Handlers[0]);
            }

            if (!statement.Handlers[statement.Handlers.Count - 1].IsCatchBlock)
            {
                finallyBlock = TryCatchFinallyConverter.ToScopeBlock(
                    converter,
                    ScopeBlockConverter.Convert(
                        converter,
                        statement.Handlers[statement.Handlers.Count - 1].Block));
            }

            JST.ScopeBlock tryBlock = TryCatchFinallyConverter.ToScopeBlock(
                converter,
                StatementConverterBase.Convert(
                    converter,
                    statement.TryBlock));

            return new JST.TryCatchFinalyBlock(
                converter.Scope,
                tryBlock,
                catchHandler,
                finallyBlock,
                statement.Location);
        }

        /// <summary>
        /// Wraps a converted body in the block JavaScript requires. A body with nothing in it,
        /// such as a sole dropped call, converts to null and becomes an empty block.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The converted body, or null when it is empty.</param>
        /// <returns>The body as a scope block.</returns>
        private static JST.ScopeBlock ToScopeBlock(
            IMethodScopeConverter converter,
            JST.Statement statement)
        {
            if (statement is JST.ScopeBlock block)
            {
                return block;
            }

            var statements = new List<JST.Statement>();
            if (statement == null)
            {
                return new JST.ScopeBlock(null, converter.Scope, statements);
            }

            statements.Add(statement);
            return new JST.ScopeBlock(statement.Location, statement.Scope, statements);
        }

        /// <summary>
        /// Converts the catch handler.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>Catch handler</returns>
        private static JST.CatchHandler ConvertCatchHandler(
            IMethodScopeConverter converter,
            HandlerBlock handler)
        {
            JST.IdentifierExpression catchVariable = null;

            converter.PushScopeBlock(handler.Block);
            try
            {
                if (handler.CatchVariable != null)
                {
                    catchVariable = (JST.IdentifierExpression)ExpressionConverterBase.Convert(
                        converter,
                        handler.CatchVariable);
                }
                else
                {
                    catchVariable =
                        new JST.IdentifierExpression(
                            converter.GetTempVariable(),
                            converter.Scope);
                }

                JST.ScopeBlock catchBlock = TryCatchFinallyConverter.ToScopeBlock(
                    converter,
                    StatementConverterBase.Convert(
                        converter,
                        handler.Block));

                return new JST.CatchHandler(
                    converter.Scope,
                    catchVariable,
                    catchBlock,
                    handler.Location);
            }
            finally
            {
                converter.PopScopeBlock();
            }
        }
    }
}
