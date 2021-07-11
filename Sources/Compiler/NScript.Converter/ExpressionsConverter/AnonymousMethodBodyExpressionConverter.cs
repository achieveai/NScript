//-----------------------------------------------------------------------
// <copyright file="AnonymousMethodBodyExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for AnonymousMethodBodyExpressionConverter
    /// </summary>
    public static class AnonymousMethodBodyExpressionConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            AnonymousMethodBodyExpression expression)
        {
            return converter.ProcessParameterBlock(expression.ParameterBlock, null);
        }
    }
}
