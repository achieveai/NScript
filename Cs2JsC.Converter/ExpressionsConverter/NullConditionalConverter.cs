//-----------------------------------------------------------------------
// <copyright file="NullConditionalConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Cs2JsC.JST;

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.StatementsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for NullConditionalConverter
    /// </summary>
    public static class NullConditionalConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            NullConditional expression)
        {
            JST.Expression firstExpression = ExpressionConverterBase.Convert(
                converter,
                expression.FirstChoice);

            JST.Expression alternateExpression = ExpressionConverterBase.Convert(
                converter,
                expression.Alternate);

            JST.Identifier tmpIdentifier = converter.GetTempVariable();
            JST.IdentifierExpression identifierExpression = new IdentifierExpression(
                tmpIdentifier,
                converter.Scope);

            firstExpression = new JST.BinaryExpression(
                firstExpression.Location,
                converter.Scope,
                JST.BinaryOperator.Assignment,
                identifierExpression,
                firstExpression);

            JST.Expression rv;

            if (converter.ClrKnownReferences.String.IsSameDefinition(expression.FirstChoice.ResultType)
                || converter.ClrKnownReferences.Object.IsSameDefinition(expression.ResultType))
            {
                // only for string do we check strict equals.
                rv = new JST.ConditionalOperatorExpression(
                    expression.Location,
                    converter.Scope,
                    new JST.BinaryExpression(
                        firstExpression.Location,
                        converter.Scope,
                        JST.BinaryOperator.StrictEquals,
                        firstExpression,
                        new JST.NullLiteralExpression(converter.Scope)),
                    new IdentifierExpression(tmpIdentifier, converter.Scope),
                    alternateExpression);
            }
            else
            {
                rv = new JST.ConditionalOperatorExpression(
                    expression.Location,
                    converter.Scope,
                    firstExpression,
                    new IdentifierExpression(tmpIdentifier, converter.Scope),
                    alternateExpression);
            }

            converter.ReleaseTempVariable(tmpIdentifier);

            return rv;
        }
    }
}
