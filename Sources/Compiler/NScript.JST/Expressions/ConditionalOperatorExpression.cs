//-----------------------------------------------------------------------
// <copyright file="ConditionalOperatorExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;

    /// <summary>
    /// Definition for ConditionalOperatorExpression
    /// </summary>
    public class ConditionalOperatorExpression : Expression
    {
        /// <summary>
        /// Backing field for Condition.
        /// </summary>
        private readonly Expression condition;

        /// <summary>
        /// Backing field for TrueExpression.
        /// </summary>
        private readonly Expression trueExpression;

        /// <summary>
        /// Backing field for FalseExpression.
        /// </summary>
        private readonly Expression falseExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalOperatorExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="trueExpression">The true expression.</param>
        /// <param name="falseExpression">The false expression.</param>
        public ConditionalOperatorExpression(
            Location location,
            IdentifierScope scope,
            Expression condition,
            Expression trueExpression,
            Expression falseExpression)
            : base(location, scope)
        {
            this.condition = condition;
            this.trueExpression = trueExpression;
            this.falseExpression = falseExpression;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public Expression Condition
        {
            get
            {
                return this.condition;
            }
        }

        /// <summary>
        /// Gets the true expression.
        /// </summary>
        /// <value>The true expression.</value>
        public Expression TrueExpression
        {
            get
            {
                return this.trueExpression;
            }
        }

        /// <summary>
        /// Gets the false expression.
        /// </summary>
        /// <value>The false expression.</value>
        public Expression FalseExpression
        {
            get
            {
                return this.falseExpression;
            }
        }

        /// <summary>
        /// Gets the precendence.
        /// </summary>
        /// <value>The precendence.</value>
        public override Precedence Precedence
        {
            get
            {
                return Precedence.Conditional;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is left to right.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is left to right; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLeftToRight
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the operator placement.
        /// </summary>
        public override OperatorPlacement OperatorPlacement
        {
            get
            {
                return OperatorPlacement.Infix;
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("condition", this.Condition);
            serializer.AddValue("trueCase", this.TrueExpression);
            serializer.AddValue("falseCase", this.FalseExpression);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.Write(this.condition, this.condition.Precedence < this.Precedence)
                .Write(Symbols.Conditional)
                .Write(this.TrueExpression, this.TrueExpression.Precedence < this.Precedence)
                .Write(Symbols.ConditionalElse)
                .Write(this.FalseExpression, this.FalseExpression.Precedence < this.Precedence);
        }
    }
}
