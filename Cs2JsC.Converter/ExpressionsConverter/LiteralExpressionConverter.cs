//-----------------------------------------------------------------------
// <copyright file="LiteralExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Converter.StatementsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for LiteralExpressionConverter
    /// </summary>
    public static class LiteralExpressionConverter
    {
        /// <summary>
        /// Converts the int literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="intLiteral">The int literal.</param>
        /// <returns>JS Expression node</returns>
        public static JST.Expression ConvertIntLiteral(MethodConverter methodConverter, IntLiteral intLiteral)
        {
            return new JST.NumberLiteralExpression(
                methodConverter.Scope,
                intLiteral.Value);
        }

        /// <summary>
        /// Converts the U int literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="intLiteral">The int literal.</param>
        /// <returns>JS Expression node</returns>
        public static JST.Expression ConvertUIntLiteral(MethodConverter methodConverter, UIntLiteral intLiteral)
        {
            return new JST.NumberLiteralExpression(
                methodConverter.Scope,
                (long)intLiteral.Value);
        }

        /// <summary>
        /// Converts the double literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="literal">The literal.</param>
        /// <returns>JS Expression node</returns>
        public static JST.Expression ConvertDoubleLiteral(MethodConverter methodConverter, DoubleLiteral literal)
        {
            return new JST.DoubleLiteralExpression(
                methodConverter.Scope,
                literal.Value);
        }

        /// <summary>
        /// Converts the boolean literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="literal">The literal.</param>
        /// <returns>JS Expression node</returns>
        public static JST.Expression ConvertBooleanLiteral(MethodConverter methodConverter, BooleanLiteral literal)
        {
            return new JST.BooleanLiteralExpression(
                methodConverter.Scope,
                literal.Value);
        }

        /// <summary>
        /// Converts the string literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="literal">The literal.</param>
        /// <returns>JS Expression node</returns>
        public static JST.Expression ConvertStringLiteral(MethodConverter methodConverter, StringLiteral literal)
        {
            return new JST.StringLiteralExpression(
                methodConverter.Scope,
                literal.String);
        }

        /// <summary>
        /// Converts the string literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="literal">The literal.</param>
        /// <returns></returns>
        public static JST.Expression ConvertCharLiteral(MethodConverter methodConverter, CharLiteral literal)
        {
            return new JST.StringLiteralExpression(
                methodConverter.Scope,
                string.Format("{0}", literal.Value));
        }

        /// <summary>
        /// Converts the boolean literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="literal">The literal.</param>
        /// <returns>JS Expression node</returns>
        public static JST.Expression ConvertNullLiteral(MethodConverter methodConverter, NullLiteral literal)
        {
            return new JST.NullLiteralExpression(
                methodConverter.Scope);
        }
    }
}
