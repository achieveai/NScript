//-----------------------------------------------------------------------
// <copyright file="TypeofExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.StatementsConverter;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for TypeofExpressionConverter
    /// </summary>
    public static class TypeofExpressionConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Expression for given type.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            TypeofExpression expression)
        {
            return JST.IdentifierExpression.Create(
                expression.Location,
                converter.Scope,
                converter.Resolve(expression.InnerExpression.Type));
        }
    }
}
