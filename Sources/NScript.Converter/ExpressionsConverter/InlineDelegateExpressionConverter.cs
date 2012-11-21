//-----------------------------------------------------------------------
// <copyright file="InlineDelegateExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for InlineDelegateExpressionConverter
    /// </summary>
    public static class InlineDelegateExpressionConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Delegate</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            InlineDelegateExpression expression)
        {
            return DelegateMethodConverter.Convert(
                converter,
                expression.Delegate);
        }
    }
}
