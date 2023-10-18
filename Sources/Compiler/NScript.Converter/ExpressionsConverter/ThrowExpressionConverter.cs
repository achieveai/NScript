//-----------------------------------------------------------------------
// <copyright file="ThrowStatementConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.Converter.TypeSystemConverter;
    using NScript.CLR.AST;
    using System.Collections.Generic;
    using NScript.JST;

    public static class ThrowExpressionConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            ThrowExpression statement)
        {
            var shell = new JST.FunctionExpression(
                statement.Location,
                converter.Scope,
                new IdentifierScope(converter.Scope),
                new List<IIdentifier>(),
                null);

            shell.AddStatement(
                new JST.ThrowStatement(
                    statement.Location,
                    converter.Scope,
                    ExpressionConverterBase.Convert(
                        converter,
                        statement.Expression),
                    writeOnNewLine: false));

            return new JST.MethodCallExpression(statement.Location, converter.Scope, shell);
        }

        public static JST.Statement ConvertStatement(
            IMethodScopeConverter methodScopeConverter,
            CLR.AST.ThrowStatement throwStatement)
        {
            return new JST.ThrowStatement(
                throwStatement.Location,
                methodScopeConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodScopeConverter,
                    throwStatement.Expression),
                writeOnNewLine: true);
        }
    }
}
