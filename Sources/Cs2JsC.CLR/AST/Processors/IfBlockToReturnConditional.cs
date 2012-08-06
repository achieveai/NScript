//-----------------------------------------------------------------------
// <copyright file="IfBlockToReturnConditional.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST.Processors
{
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for IfBlockToReturnConditional
    /// </summary>
    public class IfBlockToReturnConditional
    {
        /// <summary>
        /// Processes the specified context.
        /// Simplifies "if (cond) return foo; else return bar;" to "return cond ? foo : bar;"
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="statement">The statement.</param>
        /// <returns>Return Statement</returns>
        public static Node Process(
            IAstProcessor processor,
            IfBlockStatement statement)
        {
            if (statement.TrueCaseStatements.Statements.Count == 1 &&
                statement.FalseCaseStatements != null &&
                statement.TrueCaseStatements.LocalVariables.Count == 0 &&
                statement.FalseCaseStatements.LocalVariables.Count == 0 &&
                statement.TrueCaseStatements.Statements[0] is ReturnStatement &&
                statement.FalseCaseStatements.Statements[0] is ReturnStatement)
            {
                ReturnStatement trueReturn = statement.TrueCaseStatements.Statements[0] as ReturnStatement;
                ReturnStatement falseReturn = statement.FalseCaseStatements.Statements[0] as ReturnStatement;

                if (trueReturn.ReturnExpression != null)
                {
                    Expression conditional = new ConditionalOperatorExpression(
                        statement.Context,
                        statement.Location,
                        statement.Condition,
                        trueReturn.ReturnExpression,
                        falseReturn.ReturnExpression);

                    // Make sure that we have processed conditional as well.
                    conditional = (Expression)processor.Process(conditional);

                    return new ReturnStatement(
                        statement.Context,
                        statement.Location,
                        conditional);
                }
            }

            return statement;
        }
    }
}
