//-----------------------------------------------------------------------
// <copyright file="ConditionalOperatorConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.StatementsConverter;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for ConditionalOperatorConverter
    /// </summary>
    public static class ConditionalOperatorConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            ConditionalOperatorExpression expression)
        {
            return new JST.ConditionalOperatorExpression(
                expression.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(converter, expression.Condition),
                ExpressionConverterBase.Convert(converter, expression.TrueCase),
                ExpressionConverterBase.Convert(converter, expression.FalseCase));
        }
    }
}
