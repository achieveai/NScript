//-----------------------------------------------------------------------
// <copyright file="AnonymousMethodBodyExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.StatementsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Cs2JsC.Utils;

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
