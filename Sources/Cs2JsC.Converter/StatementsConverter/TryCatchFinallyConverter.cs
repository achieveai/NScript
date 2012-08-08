//-----------------------------------------------------------------------
// <copyright file="TryCatchFinallyConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.Converter.ExpressionsConverter;

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

            JST.Statement jstStatement = null;
            if (!statement.Handlers[statement.Handlers.Count - 1].IsCatchBlock)
            {
                jstStatement = 
                    ScopeBlockConverter.Convert(
                        converter,
                        statement.Handlers[statement.Handlers.Count - 1].Block);

                finallyBlock = jstStatement as JST.ScopeBlock;

                if (finallyBlock == null)
                {
                    var temp = new List<JST.Statement>();
                    temp.Add(jstStatement);
                    finallyBlock = new JST.ScopeBlock(
                        jstStatement.Location,
                        jstStatement.Scope,
                        temp);
                }
            }

            jstStatement = StatementConverterBase.Convert(
                    converter,
                    statement.TryBlock);

            JST.ScopeBlock tryBlock = jstStatement as JST.ScopeBlock;
            if (tryBlock == null)
            {
                var temp = new List<JST.Statement>();
                temp.Add(jstStatement);
                tryBlock = new JST.ScopeBlock(
                    jstStatement.Location,
                    jstStatement.Scope,
                    temp);
            }

            return new JST.TryCatchFinalyBlock(
                converter.Scope,
                tryBlock,
                catchHandler,
                finallyBlock);
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

                JST.Statement jstStatement =
                    StatementConverterBase.Convert(
                        converter,
                        handler.Block);

                JST.ScopeBlock catchBlock = jstStatement as JST.ScopeBlock;

                if (catchBlock == null)
                {
                    var temp = new List<JST.Statement>();
                    temp.Add(jstStatement);
                    catchBlock = new JST.ScopeBlock(
                        jstStatement.Location,
                        jstStatement.Scope,
                        temp);
                }

                return new JST.CatchHandler(
                    converter.Scope,
                    catchVariable,
                    catchBlock);
            }
            finally
            {
                converter.PopScopeBlock();
            }
        }
    }
}
