//-----------------------------------------------------------------------
// <copyright file="StatementConverterBase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using System;
    using System.Collections.Generic;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for StatementConverterBase
    /// </summary>
    public static class StatementConverterBase
    {
        /// <summary>
        /// Mapping of all the convertible classes to converter functions.
        /// </summary>
        private static Dictionary<Type, Func<IMethodScopeConverter, CLR.AST.Statement, JST.Statement>> typeMappings =
            StatementConverterBase.CreateStatementConverterMapping();

        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="clrStatement">The CLR statement.</param>
        /// <returns></returns>
        public static JST.Statement Convert(
            IMethodScopeConverter methodConverter,
            CLR.AST.Statement clrStatement)
        {
            Func<IMethodScopeConverter, CLR.AST.Statement, JST.Statement> converter = null;

            if (clrStatement == null)
            {
                return null;
            }

            if (typeMappings.TryGetValue(clrStatement.GetType(), out converter))
            {
                return converter(methodConverter, clrStatement);
            }

            throw new NotSupportedException(string.Format("Conversion for {0} not supported", clrStatement.GetType()));
        }

        /// <summary>
        /// Creates the statement converter mapping.
        /// </summary>
        /// <returns>Mapping of type to converter function.</returns>
        private static Dictionary<Type, Func<IMethodScopeConverter, CLR.AST.Statement, JST.Statement>> CreateStatementConverterMapping()
        {
            Dictionary<Type, Func<IMethodScopeConverter, CLR.AST.Statement, JST.Statement>> returnValue =
                new Dictionary<Type, Func<IMethodScopeConverter, CLR.AST.Statement, JST.Statement>>();

            returnValue.Add(
                typeof(CLR.AST.InitializerStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.InitializerStatement>(
                    ExpressionStatementConverter.Convert));

            returnValue.Add(
                typeof(CLR.AST.ExpressionStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.ExpressionStatement>(
                    ExpressionStatementConverter.Convert));

            returnValue.Add(
                typeof(CLR.AST.ReturnStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.ReturnStatement>(
                    ReturnStatementConverter.Convert));

            returnValue.Add(
                typeof(CLR.AST.IfBlockStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.IfBlockStatement>(
                    IfBlockConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.DoWhileLoop),
                StatementConverterBase.SimplifyConverter<CLR.AST.DoWhileLoop>(
                    DoWhileConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.WhileLoop),
                StatementConverterBase.SimplifyConverter<CLR.AST.WhileLoop>(
                    WhileConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.ExplicitBlock),
                StatementConverterBase.SimplifyConverter<CLR.AST.ExplicitBlock>(
                    ScopeBlockConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.ScopeBlock),
                StatementConverterBase.SimplifyConverter<CLR.AST.ScopeBlock>(
                    ScopeBlockConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.ForLoop),
                StatementConverterBase.SimplifyConverter<CLR.AST.ForLoop>(
                    ForLoopConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.ForEachLoop),
                StatementConverterBase.SimplifyConverter<CLR.AST.ForEachLoop>(
                    ForLoopConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.BreakStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.BreakStatement>(
                    JumpStatementConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.ContinueStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.ContinueStatement>(
                    JumpStatementConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.SwitchStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.SwitchStatement>(
                    SwitchStatementConverter.Convert));

            returnValue.Add(
                typeof (CLR.AST.TryCatchFinally),
                StatementConverterBase.SimplifyConverter<CLR.AST.TryCatchFinally>(
                    TryCatchFinallyConverter.Convert));

            returnValue.Add(
                typeof(CLR.AST.LocalMethodStatement),
                StatementConverterBase.SimplifyConverter<CLR.AST.LocalMethodStatement>(
                    LocalFunctionStatementConverter.Convert));

            return returnValue;
        }

        /// <summary>
        /// Simplifies the converter.
        /// </summary>
        /// <typeparam name="T">Statement sub class.</typeparam>
        /// <param name="converter">The converter.</param>
        /// <returns>Function that will convert Statement to JST.Statement.</returns>
        private static Func<IMethodScopeConverter, CLR.AST.Statement, JST.Statement> SimplifyConverter<T>(
            Func<IMethodScopeConverter, T, JST.Statement> converter) where T: CLR.AST.Statement
        {
            return delegate(IMethodScopeConverter mc, CLR.AST.Statement statement)
            {
                return converter(mc, (T)statement);
            };
        }
    }
}
