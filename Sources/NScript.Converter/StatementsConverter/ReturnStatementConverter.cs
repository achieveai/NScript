//-----------------------------------------------------------------------
// <copyright file="ReturnStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using NScript.CLR;
    using NScript.Converter.ExpressionsConverter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.Utils;

    /// <summary>
    /// Definition for ReturnStatementConverter
    /// </summary>
    public static class ReturnStatementConverter
    {
        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodScopeConverter">The method converter.</param>
        /// <param name="clrStatement">The CLR statement.</param>
        /// <returns>JST.ReturnStatement  for CLR.AST.ReturnStatement</returns>
        public static JST.Statement Convert(
            IMethodScopeConverter methodScopeConverter,
            CLR.AST.ReturnStatement clrStatement)
        {
            Location location = clrStatement.Location;
            MethodConverter methodConverter = methodScopeConverter as MethodConverter;

            if (methodConverter != null
                && methodConverter.IsConstructor
                && methodConverter.MethodDefinition.DeclaringType.IsValueOrEnum())
            {
                return new JST.ReturnStatement(
                    location,
                    methodScopeConverter.Scope,
                    methodConverter.ResolveThis(methodScopeConverter.Scope, location));
            }

            return new JST.ReturnStatement(
                location,
                methodScopeConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodScopeConverter,
                    clrStatement.ReturnExpression));
        }
    }
}
