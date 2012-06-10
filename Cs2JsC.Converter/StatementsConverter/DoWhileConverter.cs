//-----------------------------------------------------------------------
// <copyright file="DoWhileConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for DoWhileConverter
    /// </summary>
    public static class DoWhileConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>DoWhileLoop block.</returns>
        public static JST.Statement Convert(
            MethodConverter converter,
            DoWhileLoop statement)
        {
            return new JST.DoWhileLoop(
                statement.Location,
                converter.Scope,
                ExpressionsConverter.ExpressionConverterBase.Convert(
                    converter,
                    statement.Condition),
                StatementConverterBase.Convert(
                    converter,
                    statement.LoopBlock));
        }
    }
}
