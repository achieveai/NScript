//-----------------------------------------------------------------------
// <copyright file="NullConditionalConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.JST;
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;

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
            IMethodScopeConverter converter,
            NullConditional expression)
        {
            JST.Expression firstExpression = ExpressionConverterBase.Convert(
                converter,
                expression.FirstChoice);

            JST.Expression alternateExpression = ExpressionConverterBase.Convert(
                converter,
                expression.Alternate);

            JST.IIdentifier tmpIdentifier = converter.GetTempVariable();
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
                        JST.BinaryOperator.StrictNotEquals,
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
