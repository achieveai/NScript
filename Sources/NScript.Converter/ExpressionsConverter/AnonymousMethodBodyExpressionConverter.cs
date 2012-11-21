//-----------------------------------------------------------------------
// <copyright file="AnonymousMethodBodyExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Converter.StatementsConverter;
    using NScript.Converter.TypeSystemConverter;
    using System.Collections.Generic;
    using Mono.Cecil;
    using NScript.Utils;

    /// <summary>
    /// Definition for AnonymousMethodBodyExpressionConverter
    /// </summary>
    public class AnonymousMethodBodyExpressionConverter
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
            return converter.ProcessParameterBlock(expression.ParameterBlock);
        }
    }
}
