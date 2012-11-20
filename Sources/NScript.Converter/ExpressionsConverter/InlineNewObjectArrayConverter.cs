//-----------------------------------------------------------------------
// <copyright file="InlineNewObjectArrayConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using NScript.Converter.StatementsConverter;
    using NScript.Converter.TypeSystemConverter;
    using System;

    /// <summary>
    /// Definition for InlineNewObjectArrayConverter
    /// </summary>
    public static class InlineNewObjectArrayConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            InlineArrayInitialization expression)
        {
            List<JST.Expression> expressions = new List<JST.Expression>();

            foreach (Expression elementInitValue in expression.ElementInitValues)
            {
                expressions.Add(
                    (JST.Expression) ExpressionConverterBase.Convert(
                        converter,
                        elementInitValue));
            }

            /*
            return new JST.InlineNewArrayInitialization(
                expression.Location.ConvertToJsLocation(),
                converter.Scope,
                expressions);
             */
            return new JST.MethodCallExpression(
                expression.Location,
                converter.Scope,
                JST.IdentifierExpression.Create(
                    expression.Location,
                    converter.Scope,
                    converter.ResolveStaticMember(
                        converter.KnownReferences.GetArrayNativeArrayArgCtor(
                            expression.ElementType))),
                new JST.Expression[]
                {
                    new JST.InlineNewArrayInitialization(
                        expression.Location,
                        converter.Scope,
                        expressions)
                });
        }
    }
}
