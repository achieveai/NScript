//-----------------------------------------------------------------------
// <copyright file="NotConditionSimplifier.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST.Processors
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for NotConditionSimplifier
    /// </summary>
    public class NotConditionSimplifier
    {
        /// <summary>
        /// Processes the not operator.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="unaryExpression">The unary expression.</param>
        /// <returns>
        /// Simplified expression instead of Not unary expression.
        /// </returns>
        public static Expression ProcessNotOperator(
            IAstProcessor processor,
            UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operator != UnaryOperator.LogicalNot)
            {
                return unaryExpression;
            }

            return NotConditionSimplifier.ConvertToNot(unaryExpression.Expression) ?? unaryExpression;
        }

        /// <summary>
        /// Processes the equal to false operator.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="binaryExpression">The binary expression.</param>
        /// <returns>
        /// UnaryOperator with Not if condition is meat.
        /// </returns>
        public static Expression ProcessEqualToFalseOperator(
            IAstProcessor processor,
            BinaryExpression binaryExpression)
        {
            BooleanLiteral boolLiteral = binaryExpression.Right as BooleanLiteral;
            IntLiteral intLiteral = binaryExpression.Right as IntLiteral;
            if (binaryExpression.Operator == BinaryOperator.Equals
                && binaryExpression.Left.ResultType.IsBoolean()
                && ((boolLiteral != null && !boolLiteral.Value)
                    || (intLiteral != null && intLiteral.Value == 0)))
            {
                return new UnaryExpression(
                    binaryExpression.Context,
                    binaryExpression.Location,
                    binaryExpression.Left,
                    UnaryOperator.LogicalNot);
            }

            return binaryExpression;
        }

        /// <summary>
        /// Converts to not.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Not of the given expression or null if simplified Not is not possible.</returns>
        private static Expression ConvertToNot(Expression expression)
        {
            UnaryExpression childUnaryExpression = expression as UnaryExpression;

            if (childUnaryExpression != null)
            {
                if (childUnaryExpression.Operator == UnaryOperator.LogicalNot)
                {
                    return childUnaryExpression.Expression;
                }

                return null;
            }

            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                BinaryOperator notOperator;
                switch (binaryExpression.Operator)
                {
                    case BinaryOperator.Equals:
                        notOperator = BinaryOperator.NotEquals;
                        break;
                    case BinaryOperator.GreaterThan:
                        notOperator = BinaryOperator.LessThanOrEqual;
                        break;
                    case BinaryOperator.GreaterThanOrEqual:
                        notOperator = BinaryOperator.LessThan;
                        break;
                    case BinaryOperator.LessThan:
                        notOperator = BinaryOperator.GreaterThanOrEqual;
                        break;
                    case BinaryOperator.LessThanOrEqual:
                        notOperator = BinaryOperator.GreaterThan;
                        break;
                    case BinaryOperator.LogicalAnd:
                        notOperator = BinaryOperator.LogicalOr;
                        break;
                    case BinaryOperator.LogicalOr:
                        notOperator = BinaryOperator.LogicalAnd;
                        break;
                    case BinaryOperator.NotEquals:
                        notOperator = BinaryOperator.Equals;
                        break;
                    default:
                        return null;
                }

                if (notOperator == BinaryOperator.LogicalOr ||
                    notOperator == BinaryOperator.LogicalAnd)
                {
                    Expression leftExpression = NotConditionSimplifier.ConvertToNot(binaryExpression.Left);
                    Expression rightExpression = NotConditionSimplifier.ConvertToNot(binaryExpression.Right);

                    if (leftExpression != null && rightExpression != null)
                    {
                        return new BinaryExpression(
                            binaryExpression.Context,
                            binaryExpression.Location,
                            leftExpression,
                            rightExpression,
                            notOperator);
                    }
                }
                else
                {
                    return new BinaryExpression(
                        binaryExpression.Context,
                        binaryExpression.Location,
                        binaryExpression.Left,
                        binaryExpression.Right,
                        notOperator);
                }
            }

            return null;
        }
    }
}
