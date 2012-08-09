//-----------------------------------------------------------------------
// <copyright file="ConditionalOperatorProcessor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST.Processors
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for ConditionalOperatorProcessor
    /// </summary>
    public class ConditionalOperatorProcessor
    {
        /// <summary>
        /// Processes the conditional operator expression.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>
        /// TypeCheckExpression if this expression results to is operator.
        /// </returns>
        public static Node Process(
            IAstProcessor processor,
            ConditionalOperatorExpression expression)
        {
            // Most of the time the above two expressions represent but in case of complex condition it may
            // may really look like conditional. e.g. i > 10 && i < 14 can be resolve to i > 10 ? i < 14 : false;
            // So let's detect this case.
            IntLiteral falseCase = expression.FalseCase as IntLiteral;
            BooleanLiteral falseBooleanCase = expression.FalseCase as BooleanLiteral;

            if (expression.TrueCase.ResultType == expression.Context.KnownReferences.Boolean &&
                ((falseCase != null && (falseCase.Value == 0 || falseCase.Value == 1))
                    || falseBooleanCase != null))
            {
                Expression returnValue = null;

                if ((falseBooleanCase != null && falseBooleanCase.Value)
                    || (falseCase != null && falseCase.Value == 1))
                {
                    // if falseCase is 1, this means that we have to not the condition and or it with trueCase.
                    returnValue = new BinaryExpression(
                        expression.Context,
                        expression.Condition.Location,
                        new UnaryExpression(
                            expression.Context,
                            expression.Condition.Location,
                            expression.Condition,
                            UnaryOperator.LogicalNot),
                        expression.TrueCase,
                        BinaryOperator.LogicalOr);

                    returnValue.ProcessThroughPipeline(processor);
                }
                else
                {
                    returnValue =
                        new BinaryExpression(
                            expression.Context,
                            expression.Location,
                            expression.Condition,
                            expression.TrueCase,
                            BinaryOperator.LogicalAnd);
                }

                return returnValue;
            }

            expression.Invert();
            expression.ProcessThroughPipeline(processor);
            return expression;
        }
    }
}
