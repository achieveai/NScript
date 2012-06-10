//-----------------------------------------------------------------------
// <copyright file="JumpStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for JumpStatementConverter
    /// </summary>
    public class JumpStatementConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>IfBlockStatement</returns>
        public static JST.Statement Convert(
            MethodConverter converter,
            BreakStatement statement)
        {
            return new JST.BreakStatement(
                statement.Location,
                converter.Scope);
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>IfBlockStatement</returns>
        public static JST.Statement Convert(
            MethodConverter converter,
            ContinueStatement statement)
        {
            return new JST.ContinueStatement(
                statement.Location,
                converter.Scope);
        }
    }
}
