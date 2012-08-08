//-----------------------------------------------------------------------
// <copyright file="NullableConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for NullableConverter
    /// </summary>
    public static class NullableConverter
    {
        public static JST.Expression ConvertTo(
            IMethodScopeConverter converter,
            ToNullable nullable)
        {
            return ExpressionConverterBase.Convert(converter, nullable.InnerExpression);
        }

        public static JST.Expression ConvertFrom(
            IMethodScopeConverter converter,
            FromNullable nullable)
        {
            return ExpressionConverterBase.Convert(converter, nullable.InnerExpression);
            // return ExpressionConverterBase.Convert(
            //     converter,
            //     new MethodCallExpression(
            //         nullable.Context,
            //         nullable.Location,
            //         new MethodReferenceExpression(
            //             nullable.Context,
            //             nullable.Location,
            //             converter.KnownReferences.NullableValuePropertyGetter(
            //                 nullable.ResultType),
            //             nullable.InnerExpression)));
        }
    }
}