//-----------------------------------------------------------------------
// <copyright file="WhileLoopConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.StatementsConverter
{
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for WhileLoopConverter
    /// </summary>
    public static class WhileConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>WhileLoop block.</returns>
        public static JST.Statement Convert(
            MethodConverter converter,
            WhileLoop statement)
        {
            return new JST.WhileLoop(
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
