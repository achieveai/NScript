//-----------------------------------------------------------------------
// <copyright file="LocalFunctionStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.StatementsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for LocalFunctionStatementConverter
    /// </summary>
    public static class LocalFunctionStatementConverter
    {
        public static JST.Statement Convert(
            IMethodScopeConverter converter,
            LocalMethodStatement statement)
        {
            var rv = converter.ProcessParameterBlock(
                statement.MethodBlock,
                converter.ResolveLocalFunction(statement.LocalFunctionVariable.Name));

            return new JST.ExpressionStatement(
                rv.Location,
                rv.Scope,
                rv);
        }
    }
}
