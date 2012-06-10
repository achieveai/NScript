//-----------------------------------------------------------------------
// <copyright file="ExpressionStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using System.Collections.Generic;
    using Cs2JsC.Converter.ExpressionsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for ExpressionStatement
    /// </summary>
    public static class ExpressionStatementConverter
    {
        /// <summary>
        /// Converts the specified expression statement.
        /// </summary>
        /// <param name="expressionStatement">The expression statement.</param>
        /// <param name="methodConverter">The method converter.</param>
        /// <returns>Converted JST expression statement.</returns>
        public static JST.Statement Convert(
            MethodConverter methodConverter,
            CLR.AST.ExpressionStatement expressionStatement)
        {
            JST.Expression innerExpression = ExpressionConverterBase.Convert(
                methodConverter,
                expressionStatement.Expression);

            // Some expressions may become NoOps so ignore them.
            if (innerExpression != null)
            {
                return new JST.ExpressionStatement(
                    expressionStatement.Location,
                    methodConverter.Scope,
                    innerExpression);
            }

            return null;
        }

        /// <summary>
        /// Converts the specified InitializerStatement.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="initializerStatement">The initializer statement.</param>
        /// <returns></returns>
        public static JST.Statement Convert(
            MethodConverter methodConverter,
            CLR.AST.InitializerStatement initializerStatement)
        {
            List<JST.Expression> expressions = new List<JST.Expression>();
            foreach (var expr in initializerStatement.Initializers)
            {
                expressions.Add(ExpressionConverterBase.Convert(methodConverter, expr));
            }

            return new JST.InitializerStatement(
                expressions[0].Location,
                methodConverter.Scope,
                expressions);
        }
    }
}