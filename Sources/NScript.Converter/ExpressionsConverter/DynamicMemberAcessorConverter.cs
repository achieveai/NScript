//-----------------------------------------------------------------------
// <copyright file="DynamicMemberAcessorConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using System.Linq;

    /// <summary>
    /// Definition for DynamicMemberAcessorConverter
    /// </summary>
    public static class DynamicMemberAcessorConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            DynamicMemberAccessor expression)
        {
            return new JST.IndexExpression(
                expression.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(converter, expression.InstanceExpression),
                new JST.IdentifierExpression(
                    converter.RuntimeManager
                        .JSBaseObjectScopeManager
                        .ObjecTypeScopeManager
                        .GetIdentifier(
                            expression.MemberName,
                            true,
                            true),
                converter.Scope));
        }

        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            DynamicIndexAccessor expression)
        {
            return new JST.IndexExpression(
                expression.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(converter, expression.InstanceExpression),
                ExpressionConverterBase.Convert(
                    converter,
                    expression.IndexExpression));
        }

        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            DynamicCallInvocationExpression expression)
        {
            return new JST.MethodCallExpression(
                expression.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(
                    converter,
                    expression.InstanceExpression),
                expression.Parameters
                    .Select(
                        exp => ExpressionConverterBase.Convert(
                            converter,
                            exp))
                    .ToArray());
        }
    }
}
