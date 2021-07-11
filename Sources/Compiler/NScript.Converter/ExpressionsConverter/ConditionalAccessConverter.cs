//-----------------------------------------------------------------------
// <copyright file="ConditionalAccessConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;

    public static class ConditionalAccessConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            ConditionalAccessExpression expression)
        {
            var receiverExpression = ExpressionConverterBase.Convert(
                converter,
                expression.Receiver);

            var conditionalVariable = converter.GetConditionalAccessTempVariable();

            var tmpAssignement = new JST.BinaryExpression(
                null,
                converter.Scope,
                JST.BinaryOperator.Assignment,
                new JST.IdentifierExpression(
                    conditionalVariable,
                    converter.Scope,
                    null),
                receiverExpression);

            return new JST.ConditionalOperatorExpression(
                receiverExpression.Location,
                converter.Scope,
                tmpAssignement,
                ExpressionConverterBase.Convert(
                    converter,
                    expression.Access),
                new JST.NullLiteralExpression(converter.Scope));
        }

        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            ConditionalAccessExpression.ConditionalReceiver _)
            => new JST.IdentifierExpression(
                converter.GetConditionalAccessTempVariable(),
                converter.Scope,
                null);
    }
}
