//-----------------------------------------------------------------------
// <copyright file="TupleLiteralConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.Converter.TypeSystemConverter;
    using NScript.CLR.AST;
    using NScript.JST;
    using System.Linq;
    using System;

    /// <summary>
    /// Definition for TupleLiteralConverter
    /// </summary>
    public static class TupleLiteralConverter
    {
        internal static JST.Expression Convert(
            IMethodScopeConverter scopeConverter,
            TupleLiteral tupleLiteral)
        {
            // Create new Object, where each property/field is 
            // using field identifier of ValueTuple type's respective field
            // to generate the field name. Initialize the value to the expression
            // from tupleLiteral.
            var inlineInitializer = new JST.InlineObjectInitializer(tupleLiteral.Location, scopeConverter.Scope);
            var elements = tupleLiteral.TupleArgs;
            var typeDefinition = tupleLiteral.ResultType.Resolve();

            foreach(var (field, expr) in typeDefinition.Fields.Zip(elements))
            {
                inlineInitializer.AddInitializer(scopeConverter.Resolve(field), ExpressionConverterBase.Convert(scopeConverter, expr));
            }

            return inlineInitializer;
        }
    }
}
