//-----------------------------------------------------------------------
// <copyright file="TupleDeconstructConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition for TupleDeconstructConverter
    /// </summary>
    public static class TupleDeconstructConverter
    {
        internal static JST.Expression Convert(
            IMethodScopeConverter scopeConverter,
            TupleDeconstructExpression expr)
        {
            // Create assignment operators for each of left
            // expression and match it to corresponding property on tuple
            // E.g. idx: 1, should be matched with Item2 field on ValueTuple
            var lhs = expr.LeftExpressions;
            var jsRhs = ExpressionConverterBase.Convert(scopeConverter, expr.RightExpression);

            // C#: var temp = tuple;
            var tempIdentifier = new JST.IdentifierExpression(scopeConverter.GetTempVariable(), scopeConverter.Scope);
            var tupleTempAssignment = JST.ExpressionStatement.CreateAssignmentExpression(tempIdentifier, jsRhs);

            var inlineInitializer = new JST.InlineObjectInitializer(expr.Location, scopeConverter.Scope);
            var expressions = new List<JST.Expression>();

            // JS: (a = jsRhs.Item1, b = jsRhs.Item2, ..., jsRhs)
            foreach (var (assignee, index) in lhs.Select((elem, index) => (elem, index)))
            {
                var jsLhs = ExpressionConverterBase.Convert(scopeConverter, assignee);
                var accessProperty = new JST.StringLiteralExpression(null, $"Item{index + 1}");
                var accessExpression = new JST.IndexExpression(jsRhs.Location, scopeConverter.Scope, jsRhs, accessProperty);
                expressions.Add(
                    new JST.BinaryExpression(null, null,
                        JST.BinaryOperator.Assignment, jsLhs,
                        accessExpression)
                    );
            }
            expressions.Add(jsRhs);
            return new JST.ExpressionsList(expr.Location, scopeConverter.Scope, expressions);
        }
    }
}
