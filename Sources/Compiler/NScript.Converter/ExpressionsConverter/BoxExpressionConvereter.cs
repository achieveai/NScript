//-----------------------------------------------------------------------
// <copyright file="BoxExpressionConvereter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.Converter.TypeSystemConverter;

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
            IMethodScopeConverter converter,
            CLR.AST.BoxExpression boxExpression)
        {
            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    converter.KnownReferences.BoxMethod,
                    boxExpression.Location,
                    converter.Scope),
                    new JST.Expression[]
                    {
                        JST.IdentifierExpression.Create(
                            boxExpression.Location,
                            converter.Scope,
                            converter.Resolve(
                                boxExpression.BoxedExpression.ResultType)),
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
            IMethodScopeConverter converter,
            CLR.AST.UnboxExpression boxExpression)
        {
            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    converter.KnownReferences.UnboxMethod,
                    boxExpression.Location,
                    converter.Scope),
                new JST.Expression[]
                {
                    JST.IdentifierExpression.Create(
                        boxExpression.Location,
                        converter.Scope,
                        converter.Resolve(
                            boxExpression.ResultType)),
                    ExpressionsConverter.ExpressionConverterBase.Convert(
                            converter,
                            boxExpression.InnerExpression)
                },
                converter,
                converter.RuntimeManager);
        }
    }
}