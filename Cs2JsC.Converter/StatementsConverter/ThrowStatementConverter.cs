//-----------------------------------------------------------------------
// <copyright file="ThrowStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for ThrowStatementConverter
    /// </summary>
    public static class ThrowStatementConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>ThrowStatement.</returns>
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            ThrowStatement statement)
        {
            return new JST.ThrowStatement(
                statement.Location,
                converter.Scope,
                ExpressionsConverter.ExpressionConverterBase.Convert(
                    converter,
                    statement.Expression));
        }
    }
}
