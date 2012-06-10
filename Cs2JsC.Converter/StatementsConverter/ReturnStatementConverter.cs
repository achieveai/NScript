//-----------------------------------------------------------------------
// <copyright file="ReturnStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using Cs2JsC.Converter.ExpressionsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for ReturnStatementConverter
    /// </summary>
    public static class ReturnStatementConverter
    {
        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="clrStatement">The CLR statement.</param>
        /// <returns>JST.ReturnStatement  for CLR.AST.ReturnStatement</returns>
        public static JST.Statement Convert(
            MethodConverter methodConverter,
            CLR.AST.ReturnStatement clrStatement)
        {
            Location location = clrStatement.Location;

            if (methodConverter.IsConstructor
                && methodConverter.MethodDefinition.DeclaringType.IsValueType)
            {
                return new JST.ReturnStatement(
                    location,
                    methodConverter.Scope,
                    methodConverter.ResolveThis(methodConverter.Scope));
            }

            return new JST.ReturnStatement(
                location,
                methodConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodConverter,
                    clrStatement.ReturnExpression));
        }
    }
}
