//-----------------------------------------------------------------------
// <copyright file="TupleDeconstructConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using Mono.Cecil;
    using NScript.CLR;
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
            return InternalConvert(scopeConverter, expr, true);
        }

        private static JST.Expression InternalConvert(
            IMethodScopeConverter scopeConverter,
            TupleDeconstructExpression expr,
            bool addTailExpr = false)
        {
            return expr.RightExpression switch
            {
                TupleLiteral literal => InternalConvert(
                    scopeConverter,
                    expr.LeftExpressions,
                    literal,
                    expr.ResultType,
                    addTailExpr),

                _ => InternalConvert(
                    scopeConverter,
                    expr.LeftExpressions,
                    expr.RightExpression,
                    expr.ResultType,
                    addTailExpr)
            };
        }

        /// <summary>
        /// Call when rhs is a tuple literal.
        /// The lhs expressions are assigned to the corresponding tuple args of rhs.
        /// Ex: (a, (b, c)) = (f(), z())
        /// </summary>
        private static JST.Expression InternalConvert(
            IMethodScopeConverter scopeConverter,
            Expression[] lhs,
            TupleLiteral rhs,
            TypeReference resultType,
            bool addTailExpr = false)
        {
            var expressions = new List<JST.Expression>(lhs.Count());
            var tupleArgs = new List<JST.Expression>(lhs.Count());

            foreach (var (expr, tupleArg) in lhs.Zip(rhs.TupleArgs))
            {
                if (expr is TupleLiteral lit)
                {
                    // TODO: Check
                    var decons = new TupleDeconstructExpression(expr.Context, expr.Location, lit.TupleArgs, tupleArg, false);
                    var tmpVar = new JST.IdentifierExpression(
                        scopeConverter.GetTempVariable(),
                        scopeConverter.Scope);

                    expressions.Add(
                        new JST.BinaryExpression(
                            expr.Location,
                            scopeConverter.Scope,
                            JST.BinaryOperator.Assignment,
                            tmpVar,
                            InternalConvert(scopeConverter, decons, true)));

                    tupleArgs.Add(tmpVar);
                    continue;
                }

                var jsLhs = expr switch
                {
                    DiscardExpression => new JST.IdentifierExpression(
                        scopeConverter.GetTempVariable(),
                        scopeConverter.Scope),
                    _ => ExpressionConverterBase.Convert(scopeConverter, expr)
                };

                var jsRhs = ExpressionConverterBase.Convert(scopeConverter, tupleArg);

                if (!(expr is VariableReference) && !(expr is DiscardExpression))
                {
                    // When lhs is property we need to make sure we dont 'get' it multiple times,
                    // And only set it once

                    var tmpVar = new JST.IdentifierExpression(
                        scopeConverter.GetTempVariable(),
                        scopeConverter.Scope);

                    var assignment = new JST.BinaryExpression(
                        rhs.Location,
                        scopeConverter.Scope,
                        JST.BinaryOperator.Assignment,
                        tmpVar,
                        jsRhs);

                    expressions.Add(assignment);

                    expressions.Add(
                        new JST.BinaryExpression(
                            rhs.Location,
                            scopeConverter.Scope,
                            JST.BinaryOperator.Assignment,
                            jsLhs,
                            tmpVar));

                    tupleArgs.Add(tmpVar);
                }
                else
                {
                    expressions.Add(
                        new JST.BinaryExpression(
                            rhs.Location,
                            scopeConverter.Scope,
                            JST.BinaryOperator.Assignment,
                            jsLhs,
                            jsRhs));
                    tupleArgs.Add(jsLhs);
                }
            }

            // Finally add the tuple literal, since the tuple deconstruct expression evaluates
            // to the tupleLiteral
            if (addTailExpr)
            {
                expressions.Add(TupleLiteralConverter.ConstructTupleLiteral(scopeConverter, tupleArgs, resultType));
                // expressions.Add(ExpressionConverterBase.Convert(scopeConverter, rhs));
            }

            return new JST.ExpressionsList(
                rhs.Location,
                scopeConverter.Scope,
                expressions);
        }

        /// <summary>
        /// Used when rhs is TupleDeconstructExpression is not a TupleLiteral
        /// Ex: (a, b) = f() | x
        /// The rhs is evaluated once and stored into a temp variable (if required)
        /// and conversion is carried out.
        /// </summary>
        private static JST.Expression InternalConvert(
            IMethodScopeConverter scopeConverter,
            Expression[] lhs,
            Expression rhs,
            TypeReference resultType,
            bool addTailExpr = false)
        {
            // Cache function result in a tmp variable
            var tmpIdentifier = scopeConverter.GetTempVariable();
            try
            {
                var jsRhs = ExpressionConverterBase.Convert(scopeConverter, rhs);
                JST.Expression tempVarInitializer = null;

                // Create tempVar only when rhs is not variable, else use the variable as it is.
                if (!(rhs is VariableReference))
                {
                    var tmpVarExpression = new JST.IdentifierExpression(tmpIdentifier, scopeConverter.Scope, rhs.Location);
                    tempVarInitializer = new JST.BinaryExpression(
                        rhs.Location,
                        scopeConverter.Scope,
                        JST.BinaryOperator.Assignment,
                        tmpVarExpression,
                        jsRhs);

                    jsRhs = tmpVarExpression;
                }

                var expressionList = InternalConvert(
                    scopeConverter,
                    lhs,
                    jsRhs,
                    resultType,
                    addTailExpr);

                if (tempVarInitializer != null)
                {
                    expressionList.Expressions.Insert(0, tempVarInitializer);
                }

                return expressionList;
            }
            finally
            {
                scopeConverter.ReleaseTempVariable(tmpIdentifier);
            }
        }

        /// <summary>
        /// Used when rhs is cached into a JS tmp variable, and
        /// each lhs arg needs to be assigned to corresponding field of the tuple
        /// </summary>
        private static JST.ExpressionsList InternalConvert(
            IMethodScopeConverter scopeConverter,
            Expression[] lhs,
            JST.Expression rhs,
            TypeReference rhsTy,
            bool addTrailingExpr = false)
        {
            var expressions = new List<JST.Expression>();

            foreach (var (expr, (field, ty)) in lhs.Zip(rhsTy.Resolve().Fields.Zip(rhsTy.GetGenericArguments())))
            {
                if (expr is DiscardExpression)
                {
                    continue;
                }

                var itemAccess = CreateItemAccessExpression(scopeConverter, rhs, field);

                if (expr is TupleLiteral literal)
                {
                    expressions.Add(InternalConvert(
                        scopeConverter,
                        literal.TupleArgs,
                        itemAccess,
                        ty));
                    continue;
                }

                expressions.Add(
                    new JST.BinaryExpression(
                        expr.Location,
                        scopeConverter.Scope,
                        JST.BinaryOperator.Assignment,
                        ExpressionConverterBase.Convert(scopeConverter, expr),
                        itemAccess));
            }

            if (addTrailingExpr)
            {
                expressions.Add(rhs);
            }

            return new JST.ExpressionsList(null, scopeConverter.Scope, expressions);
        }


        private static JST.Expression CreateItemAccessExpression(
            IMethodScopeConverter scopeConverter,
            JST.Expression rhs,
            FieldDefinition fieldDef)
        {
            var fieldIdentifier = scopeConverter.Resolve(fieldDef);
            var itemAccessExpr = new JST.IndexExpression(
                rhs.Location,
                scopeConverter.Scope,
                rhs,
                new JST.IdentifierExpression(fieldIdentifier, scopeConverter.Scope));

            return itemAccessExpr;
        }
    }
}