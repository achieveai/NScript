//-----------------------------------------------------------------------
// <copyright file="ExpressionStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for ExpressionStatement
    /// </summary>
    public class ExpressionStatement : Statement
    {
        /// <summary>
        /// Backing store for expression.
        /// </summary>
        private readonly Expression expression;

        public ExpressionStatement(
            Location location,
            IdentifierScope scope,
            Expression expression)
            : base(location, scope)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public Expression Expression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>
        /// Creates the assignment expression.
        /// </summary>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="rightExpression">The right expression.</param>
        /// <returns>Expression statment with assignment.</returns>
        public static ExpressionStatement CreateAssignmentExpression(
            Expression leftExpression,
            Expression rightExpression)
        {
            if (leftExpression is IdentifierExpression
                || leftExpression is IndexExpression)
            {
                return new ExpressionStatement(
                    leftExpression.Location,
                    leftExpression.Scope,
                    new BinaryExpression(
                        leftExpression.Location,
                        leftExpression.Scope,
                        BinaryOperator.Assignment,
                        leftExpression,
                        rightExpression));
            }
            else
            {
                throw new ArgumentException("leftExpression");
            }
        }

        /// <summary>
        /// Creates the method call expression.
        /// </summary>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public static ExpressionStatement CreateMethodCallExpression(
            Expression leftExpression,
            params Expression[] arguments)
        {
            return ExpressionStatement.CreateMethodCallExpression(
                leftExpression,
                (IList<Expression>) arguments);
        }

        /// <summary>
        /// Creates the method call expression.
        /// </summary>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public static ExpressionStatement CreateMethodCallExpression(
            Expression leftExpression,
            IList<Expression> arguments)
        {
            return new ExpressionStatement(
                leftExpression.Location,
                leftExpression.Scope,
                new MethodCallExpression(
                    leftExpression.Location,
                    leftExpression.Scope,
                    leftExpression,
                    arguments));
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("expression", this.Expression);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .EnterLocation(this.Location)
                .Write(this.Expression)
                .Write(Symbols.SemiColon)
                .LeaveLocation();
        }
    }
}
