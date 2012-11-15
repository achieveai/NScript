//-----------------------------------------------------------------------
// <copyright file="UnaryExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for UnaryExpressionConverter
    /// </summary>
    public static class UnaryExpressionConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>JST Expression.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            UnaryExpression expression)
        {
            if (expression.IsAssignment
                && BinaryExpressionConverter.IsResolvingToFunctionCall(expression.Expression))
            {
                return UnaryExpressionConverter.ConvertFunctionAssignmentExpression(
                    converter,
                    expression);
            }

            return UnaryExpressionConverter.Convert(
                converter,
                expression.Location,
                expression.Expression,
                expression.Operator,
                expression.IsLifted);
        }

        /// <summary>
        /// Converts the function assignment expression.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Converted expression</returns>
        private static JST.Expression ConvertFunctionAssignmentExpression(
            IMethodScopeConverter converter,
            UnaryExpression expression)
        {
            List<JST.Expression> arguments = new List<JST.Expression>();
            Location location = expression.Location;
            JST.IdentifierScope scope = converter.Scope;

            if (BinaryExpressionConverter.IsIntrinsicExpression(
                converter,
                expression.Expression))
            {
                return UnaryExpressionConverter.Convert(
                    converter,
                    location,
                    expression.Expression,
                    expression.Operator,
                    expression.IsLifted);
            }

            bool isPost = false;
            bool isIncrement = true;
            switch (expression.Operator)
            {
                case UnaryOperator.PostDecrement:
                    isPost = true;
                    isIncrement = false;
                    break;
                case UnaryOperator.PostIncrement:
                    isPost = true;
                    break;
                case UnaryOperator.PreDecrement:
                    isIncrement = false;
                    break;
                case UnaryOperator.PreIncrement:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            JST.Expression expr = null;
            if (!expression.IsLifted)
            {
                expr = ExpressionConverterBase.Convert(
                    converter,
                    expression.Expression);
            }
            else
            {
                expr = ExpressionConverterBase.Convert(
                    converter,
                    ((FromNullable)expression.Expression).InnerExpression);
            }

            JST.IdentifierExpression identExpr = expr as JST.IdentifierExpression;
            JST.Expression valueExpression = expr;

            // Only for below two cases do we really need indentifierExpression
            // to keep the value of this expression.
            if (identExpr == null
                && (expression.IsLifted || isPost))
            {
                identExpr = new JST.IdentifierExpression(
                    converter.GetTempVariable(),
                    scope);

                valueExpression = new JST.BinaryExpression(
                    location,
                    scope,
                    JST.BinaryOperator.Assignment,
                    identExpr,
                    expr);
            }

            if (!expression.IsLifted)
            {
                valueExpression = new JST.BinaryExpression(
                    location,
                    scope,
                    isIncrement ? JST.BinaryOperator.Plus : JST.BinaryOperator.Minus,
                    valueExpression,
                    new JST.NumberLiteralExpression(
                        scope,
                        1));
            }
            else
            {
                valueExpression = new JST.ConditionalOperatorExpression(
                    location,
                    scope,
                    new JST.BinaryExpression(
                        location,
                        scope,
                        JST.BinaryOperator.StrictEquals,
                        valueExpression,
                        new JST.NullLiteralExpression(scope)),
                    new JST.NullLiteralExpression(scope),
                    identExpr);
            }

            var writerExpression = BinaryExpressionConverter.GetWriteFunction(
                converter,
                expression.Expression,
                valueExpression);

            if (isPost)
            {
                return new JST.ExpressionsList(
                    location,
                    scope,
                    writerExpression,
                    identExpr);
            }
            else
            {
                return writerExpression;
            }
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="location">The location.</param>
        /// <param name="expression">The left expression.</param>
        /// <param name="clrOp">The CLR op.</param>
        /// <returns></returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            Location location,
            Expression expression,
            UnaryOperator clrOp,
            bool isLifted)
        {
            JST.UnaryOperator op;
            bool isAssignment = false;
            bool isAdd = false;
            bool isPost = false;

            switch (clrOp)
            {
                case UnaryOperator.BitwiseNot:
                    op = JST.UnaryOperator.BitwiseNot;
                    break;
                case UnaryOperator.LogicalNot:
                    op = JST.UnaryOperator.LogicalNot;
                    break;
                case UnaryOperator.PostDecrement:
                    op = JST.UnaryOperator.PostDecrement;
                    isAssignment = true;
                    isPost = true;
                    break;
                case UnaryOperator.PostIncrement:
                    op = JST.UnaryOperator.PostIncrement;
                    isAssignment = true;
                    isAdd = true;
                    isPost = true;
                    break;
                case UnaryOperator.PreDecrement:
                    op = JST.UnaryOperator.PreDecrement;
                    isAssignment = true;
                    break;
                case UnaryOperator.PreIncrement:
                    op = JST.UnaryOperator.PreIncrement;
                    isAdd = true;
                    isAssignment = true;
                    break;
                case UnaryOperator.UnaryMinus:
                    op = JST.UnaryOperator.UnaryMinus;
                    break;
                case UnaryOperator.UnaryPlus:
                    op = JST.UnaryOperator.UnaryPlus;
                    break;
                case UnaryOperator.TypeOf:
                case UnaryOperator.Void:
                default:
                    throw new NotImplementedException();
            }

            JST.Expression expr = null;

            // If not lifted expression, then nothing special to do.
            if (!isLifted)
            {
                return new JST.UnaryExpression(
                    location,
                    converter.Scope,
                    op,
                    ExpressionConverterBase.Convert(
                        converter,
                        expression));
            }

            // In case of lifted expression, we need to check for null
            // and if the value is null then return null else do the operation.
            expr = ExpressionConverterBase.Convert(
                converter,
                ((FromNullable)expression).InnerExpression);

            JST.Expression conditionExpr = null;

            JST.IdentifierExpression identExpr = expr as JST.IdentifierExpression;
            if (identExpr == null)
            {
                identExpr = new JST.IdentifierExpression(
                    converter.GetTempVariable(), converter.Scope);

                conditionExpr = new JST.BinaryExpression(
                    location,
                    converter.Scope,
                    JST.BinaryOperator.StrictEquals,
                    new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.Assignment,
                        identExpr,
                        expr),
                    new JST.NullLiteralExpression(converter.Scope));
            }
            else
            {
                conditionExpr = new JST.BinaryExpression(
                    location,
                    converter.Scope,
                    JST.BinaryOperator.StrictEquals,
                    identExpr,
                    new JST.NullLiteralExpression(converter.Scope));
            }

            // If the operation is not assignment, then just create the
            // conditional operator and return.
            if (!isAssignment)
            {
                return new JST.ConditionalOperatorExpression(
                    location,
                    converter.Scope,
                    conditionExpr,
                    new JST.NullLiteralExpression(converter.Scope),
                    new JST.UnaryExpression(
                        location,
                        converter.Scope,
                        op,
                        identExpr));
            }

            JST.Expression valueExpr =
                new JST.ConditionalOperatorExpression(
                    location,
                    converter.Scope,
                    conditionExpr,
                    new JST.NullLiteralExpression(converter.Scope),
                    new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        isAdd ? JST.BinaryOperator.Plus : JST.BinaryOperator.Minus,
                        identExpr,
                        new JST.NumberLiteralExpression(
                            converter.Scope,
                            1)));

            valueExpr = new JST.BinaryExpression(
                location,
                converter.Scope,
                JST.BinaryOperator.Assignment,
                expr,
                valueExpr);

            // In case this is post expression, then the real value
            // of this expression is our first expression, so
            // let's create Expressions list and last expression in
            // the list is the first expression.
            // (inst.bar = (tmp = inst.bar) === null ? null : tmp + 1, tmp)
            if (isPost)
            {
                valueExpr = new JST.ExpressionsList(
                    location,
                    converter.Scope,
                    valueExpr,
                    identExpr);
            }

            return valueExpr;
        }
    }
}