//-----------------------------------------------------------------------
// <copyright file="AnonymousNewExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// Definition for AnonymousNewExpressionConverter
    /// </summary>
    public static class AnonymousNewExpressionConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            AnonymousNewExpression expression)
        {
            var rv = new JST.InlineObjectInitializer(
                expression.Location,
                converter.Scope);

            foreach (var item in expression.Values)
            {
                rv.AddInitializer(
                    item.Item1,
                    ExpressionConverterBase.Convert(converter, item.Item2));
            }

            return rv;
        }
    }
}
