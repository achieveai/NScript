//-----------------------------------------------------------------------
// <copyright file="ThrowStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using NScript.Converter.TypeSystemConverter;
    using NScript.CLR.AST;

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
            return new JST.ThrowExpression(
                statement.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(
                    converter,
                    statement.Expression));
        }
    }
}
