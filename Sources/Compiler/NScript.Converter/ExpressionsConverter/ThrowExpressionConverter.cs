//-----------------------------------------------------------------------
// <copyright file="ThrowStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.Converter.TypeSystemConverter;
    using NScript.CLR.AST;
    using System.Collections.Generic;
    using NScript.JST;

    /// <summary>
    /// Definition for ThrowStatementConverter
    /// </summary>
    public static class ThrowExpressionConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>ThrowStatement.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            ThrowExpression statement)
        {
            var shell = new JST.FunctionExpression(
                statement.Location,
                converter.Scope,
                new IdentifierScope(converter.Scope),
                new List<IIdentifier>(),
                null);

            shell.AddStatement(
                new JST.ThrowStatement(
                    statement.Location,
                    converter.Scope,
                    ExpressionConverterBase.Convert(
                        converter,
                        statement.Expression)));

            return new JST.MethodCallExpression(statement.Location, converter.Scope, shell);
        }
    }
}
