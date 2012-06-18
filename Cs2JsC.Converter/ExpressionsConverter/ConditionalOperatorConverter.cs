//-----------------------------------------------------------------------
// <copyright file="ConditionalOperatorConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.StatementsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;

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
