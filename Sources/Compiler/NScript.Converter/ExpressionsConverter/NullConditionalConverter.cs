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
                || expression.ResultType.IsValueType
                || converter.ClrKnownReferences.Object.IsSameDefinition(expression.ResultType))
            {
                // In JS, '0' (Number) is implicitly false, so we cannot use the JS conditional directly.
                // NullOrUndefinedCheck does null check(===), Undefined check is usefuly in cases like JsonType.

                var methodRef = converter.ClrKnownReferences.NullOrUndefinedCheck;
                var methodIdentifier = converter.ResolveStaticMember(methodRef);
                var methodExpression = new JST.IdentifierExpression(methodIdentifier[0], converter.Scope);

                rv = new JST.ConditionalOperatorExpression(
                    expression.Location,
                    converter.Scope,
                    new JST.MethodCallExpression(expression.Location, converter.Scope, methodExpression, firstExpression),
                    alternateExpression,
                    new IdentifierExpression(tmpIdentifier, converter.Scope));
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

        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            NullCoalsecingAssignmentExpression expression)
        {
            return ExpressionConverterBase.Convert(
                converter,
                expression.AsBinaryExpression());
        }
    }
}
