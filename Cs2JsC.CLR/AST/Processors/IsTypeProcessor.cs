//-----------------------------------------------------------------------
// <copyright file="IsTypeProcessor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST.Processors
{
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for IsTypeProcessor
    /// </summary>
    public static class IsTypeProcessor
    {
        /// <summary>
        /// Processes the binary expression.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// TypeCheckExpression if this expression results to is operator.
        /// </returns>
        public static Node ProcessBinaryExpression(
            IAstProcessor processor,
            BinaryExpression expression)
        {
            if (expression.Operator != BinaryOperator.NotEquals)
            {
                return expression;
            }

            TypeCheckExpression typeCheckExpression = expression.Left as TypeCheckExpression;
            NullLiteral nullLiteral = null;

            if (typeCheckExpression == null)
            {
                typeCheckExpression = expression.Right as TypeCheckExpression;

                if (typeCheckExpression != null)
                {
                    nullLiteral = expression.Left as NullLiteral;
                }
            }
            else
            {
                nullLiteral = expression.Right as NullLiteral;
            }

            if (nullLiteral != null &&
                typeCheckExpression.CheckType == TypeCheckType.AsType)
            {
                return new TypeCheckExpression(
                    typeCheckExpression.Context,
                    typeCheckExpression.Location,
                    typeCheckExpression.Expression,
                    typeCheckExpression.Type,
                    TypeCheckType.IsType);
            }

            return expression;
        }
    }
}
