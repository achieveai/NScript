//-----------------------------------------------------------------------
// <copyright file="BoxExpressionConvereter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for BoxExpressionConvereter
    /// </summary>
    public static class BoxExpressionConvereter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="boxExpression">The box expression.</param>
        /// <returns>Currently S# implementation, so we return the inner expression.</returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            CLR.AST.BoxExpression boxExpression)
        {
            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    JST.IdentifierExpression.Create(
                        boxExpression.Location,
                        converter.Scope,
                        converter.Resolve(
                            boxExpression.BoxedExpression.ResultType)),
                        converter.KnownReferences.BoxMethod,
                        false),
                    new JST.Expression[]
                    {
                        ExpressionsConverter.ExpressionConverterBase.Convert(
                                converter,
                                boxExpression.BoxedExpression)
                    },
                    converter,
                    converter.RuntimeManager);
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="boxExpression">The box expression.</param>
        /// <returns>Currently S# implementation, so we return the inner expression.</returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            CLR.AST.UnboxExpression boxExpression)
        {
            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    JST.IdentifierExpression.Create(
                        boxExpression.Location,
                        converter.Scope,
                        converter.Resolve(
                            boxExpression.ResultType)),
                    converter.KnownReferences.UnboxMethod,
                    false),
                new JST.Expression[]
                {
                    ExpressionsConverter.ExpressionConverterBase.Convert(
                            converter,
                            boxExpression.InnerExpression)
                },
                converter,
                converter.RuntimeManager);
        }
    }
}