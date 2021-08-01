//-----------------------------------------------------------------------
// <copyright file="LiteralExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using NScript.Converter.StatementsConverter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.CLR.AST;

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
        public static JST.Expression ConvertIntLiteral(IMethodScopeConverter methodConverter, IntLiteral intLiteral)
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
        public static JST.Expression ConvertUIntLiteral(IMethodScopeConverter methodConverter, UIntLiteral intLiteral)
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
        public static JST.Expression ConvertDoubleLiteral(IMethodScopeConverter methodConverter, DoubleLiteral literal)
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
        public static JST.Expression ConvertBooleanLiteral(IMethodScopeConverter methodConverter, BooleanLiteral literal)
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
        public static JST.Expression ConvertStringLiteral(IMethodScopeConverter methodConverter, StringLiteral literal)
        {
            if (literal.String == null)
            { return new JST.NullLiteralExpression(methodConverter.Scope); }

            return new JST.StringLiteralExpression(
                methodConverter.Scope,
                literal.String);
        }

        /// <summary>
        /// Converts the char literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="literal">The literal.</param>
        /// <returns></returns>
        public static JST.Expression ConvertCharLiteral(IMethodScopeConverter methodConverter, CharLiteral literal)
        {
            return new JST.StringLiteralExpression(
                methodConverter.Scope,
                literal.Value.ToString());
        }

        /// <summary>
        /// Converts the null literal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="literal">The literal.</param>
        /// <returns>JS Expression node</returns>
        public static JST.Expression ConvertNullLiteral(IMethodScopeConverter methodConverter, NullLiteral literal)
        {
            return new JST.NullLiteralExpression(
                methodConverter.Scope);
        }
    }
}
