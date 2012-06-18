//-----------------------------------------------------------------------
// <copyright file="NewArrayConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.Converter.StatementsConverter;
    using Mono.Cecil;

    /// <summary>
    /// Definition for NewArrayConverter
    /// </summary>
    public static class NewArrayConverter
    {
        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="newObjectExpression">The new object expression.</param>
        /// <returns>Expression converter.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter methodConverter,
            NewArrayExpression newObjectExpression)
        {
            MethodReference ctorMethod = methodConverter.KnownReferences.GetArrayIntArgCtor(
                newObjectExpression.Type);

            return new JST.MethodCallExpression(
                newObjectExpression.Location,
                methodConverter.Scope,
                JST.IdentifierExpression.Create(
                    newObjectExpression.Location,
                    methodConverter.Scope,
                    methodConverter.ResolveStaticMember(ctorMethod)),
                new JST.Expression[]
                {
                    ExpressionConverterBase.Convert(
                        methodConverter,
                        newObjectExpression.Size)
                });
            /*
            return new JST.NewArrayExpression(
                newObjectExpression.Location.ConvertToJsLocation(),
                methodConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodConverter,
                    newObjectExpression.Size));
             */
        }

        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="newObjectExpression">The new object expression.</param>
        /// <returns>Expression converter.</returns>
        public static JST.Expression ConvertArrayElementLoad(
            IMethodScopeConverter methodConverter,
            ArrayElementExpression arrayElementLoadExpression)
        {
            return ExpressionConverterBase.Convert(
                methodConverter,
                PropertyReferenceConverter.ConvertToPropertyReference(
                    methodConverter.RuntimeManager.Context,
                    arrayElementLoadExpression));
            /*
            return new JST.IndexExpression(
                arrayElementLoadExpression.Location.ConvertToJsLocation(),
                methodConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodConverter,
                    arrayElementLoadExpression.Array),
                ExpressionConverterBase.Convert(
                    methodConverter,
                    arrayElementLoadExpression.Index),
                true);
             */
        }
    }
}
