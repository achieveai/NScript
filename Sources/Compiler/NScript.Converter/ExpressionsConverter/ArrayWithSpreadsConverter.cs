//-----------------------------------------------------------------------
// <copyright file="ArrayWithSpreadsConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System.Collections.Generic;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;

    /// <summary>
    /// Lowers <see cref="ArrayWithSpreadsInitialization"/> (C# 12 collection
    /// expression containing one or more spread elements, target-typed to
    /// <c>T[]</c>) into a JS expression of the form
    /// <code>new ArrayG&lt;T&gt;([].concat(arg, ..., arg))</code>
    /// where each <c>arg</c> is either an inline JS array bunching consecutive
    /// literal elements or the inner JS array extracted from a spread source.
    ///
    /// Phase F1 only accepts spread sources whose static type is <c>T[]</c>
    /// (validated at Stage 1); <c>List&lt;T&gt;</c> and <c>IEnumerable&lt;T&gt;</c>
    /// sources are tracked for Phase F4.
    /// </summary>
    public static class ArrayWithSpreadsConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            ArrayWithSpreadsInitialization expression)
        {
            var location = expression.Location;
            var scope = converter.Scope;
            var elementType = expression.ElementType;
            var knownReferences = converter.KnownReferences;

            var concatArgs = new List<JST.Expression>();
            List<JST.Expression> pendingLiterals = null;

            foreach (var item in expression.Items)
            {
                if (item.IsSpread)
                {
                    if (pendingLiterals != null)
                    {
                        concatArgs.Add(new JST.InlineNewArrayInitialization(
                            location,
                            scope,
                            pendingLiterals));
                        pendingLiterals = null;
                    }

                    var spreadSourceExpr = ExpressionConverterBase.Convert(
                        converter,
                        item.Operand);

                    concatArgs.Add(new JST.MethodCallExpression(
                        location,
                        scope,
                        JST.IdentifierExpression.Create(
                            location,
                            scope,
                            converter.ResolveStaticMember(
                                knownReferences.GetNativeArrayFromArrayMethod(elementType))),
                        spreadSourceExpr));
                }
                else
                {
                    if (pendingLiterals == null)
                    {
                        pendingLiterals = new List<JST.Expression>();
                    }

                    pendingLiterals.Add(ExpressionConverterBase.Convert(
                        converter,
                        item.Operand));
                }
            }

            if (pendingLiterals != null)
            {
                concatArgs.Add(new JST.InlineNewArrayInitialization(
                    location,
                    scope,
                    pendingLiterals));
            }

            // Always wrap through `[].concat(...)` even for a lone spread argument:
            // collection-expression semantics require a fresh array, so the spread
            // source must not be aliased into the result.
            var flatNativeArray = BuildConcatCall(location, scope, concatArgs);

            return new JST.MethodCallExpression(
                location,
                scope,
                JST.IdentifierExpression.Create(
                    location,
                    scope,
                    converter.ResolveStaticMember(
                        knownReferences.GetArrayNativeArrayArgCtor(elementType))),
                new JST.Expression[] { flatNativeArray });
        }

        // Public so the fresh-array invariant ([].concat(...) wrapper) can be
        // structurally asserted from a unit test without standing up a Roslyn
        // compilation. The lone-spread case especially must not collapse to the
        // bare spread source — that would alias and break collection-expression
        // semantics.
        public static JST.Expression BuildConcatCall(
            Utils.Location location,
            JST.IdentifierScope scope,
            IList<JST.Expression> concatArgs)
        {
            var receiver = new JST.InlineNewArrayInitialization(
                location,
                scope,
                new List<JST.Expression>());

            return new JST.MethodCallExpression(
                location,
                scope,
                new JST.IndexExpression(
                    location,
                    scope,
                    receiver,
                    new JST.StringLiteralExpression(scope, "concat", location)),
                concatArgs);
        }
    }
}
