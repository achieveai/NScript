//-----------------------------------------------------------------------
// <copyright file="TupleLiteralConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.Converter.TypeSystemConverter;
    using NScript.CLR.AST;
    using System.Linq;
    using System.Collections.Generic;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TupleLiteralConverter
    /// </summary>
    public static class TupleLiteralConverter
    {
        internal static JST.Expression Convert(
            IMethodScopeConverter scopeConverter,
            TupleLiteral tupleLiteral)
        {
            return ConstructTupleLiteral(
                scopeConverter,
                tupleLiteral
                    .TupleArgs
                    .Select(_ => ExpressionConverterBase.Convert(scopeConverter, _)),
                tupleLiteral.ResultType);
        }

        public static JST.Expression ConstructTupleLiteral(
            IMethodScopeConverter methodScopeConverter,
            IEnumerable<JST.Expression> expressions,
            TypeReference ty)
        {
            var initializer = new JST.InlineObjectInitializer(
                null,
                methodScopeConverter.Scope);

            foreach(var (expr, field) in expressions.Zip(ty.Resolve().Fields))
            {
                initializer.AddInitializer(methodScopeConverter.Resolve(field), expr);
            }

            return initializer;
        }

    }
}
