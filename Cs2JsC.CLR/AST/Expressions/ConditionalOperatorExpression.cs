//-----------------------------------------------------------------------
// <copyright file="ConditionalOperatorExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for ConditionalOperatorExpression
    /// </summary>
    public class ConditionalOperatorExpression : Expression
    {
        /// <summary>
        /// Backing field for Condition Expression.
        /// </summary>
        private Expression conditionExpression;

        /// <summary>
        /// Backing field for TrueCaseExpression.
        /// </summary>
        private Expression trueCaseExpression;

        /// <summary>
        /// Backing field for FalseCaseExpression.
        /// </summary>
        private Expression falseCaseExpression;

        /// <summary>
        /// Field to keep track if this has already been inverted or not
        /// </summary>
        private bool inverted;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalOperatorExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="conditionExpression">The condition expression.</param>
        /// <param name="trueCaseExpression">The true case expression.</param>
        /// <param name="falseCaseExpression">The false case expression.</param>
        public ConditionalOperatorExpression(
            ClrContext context,
            Location location,
            Expression conditionExpression,
            Expression trueCaseExpression,
            Expression falseCaseExpression)
            : base(context, location)
        {
            this.conditionExpression = conditionExpression;
            this.trueCaseExpression = trueCaseExpression;
            this.falseCaseExpression = falseCaseExpression;
        }

        /// <summary>
        /// Gets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public Expression Condition
        {
            get
            {
                return this.conditionExpression;
            }
        }

        /// <summary>
        /// Gets the true case.
        /// </summary>
        /// <value>The true case.</value>
        public Expression TrueCase
        {
            get
            {
                return this.trueCaseExpression;
            }
        }

        /// <summary>
        /// Gets the false case.
        /// </summary>
        /// <value>The false case.</value>
        public Expression FalseCase
        {
            get
            {
                return this.falseCaseExpression;
            }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.trueCaseExpression.ResultType ?? this.falseCaseExpression.ResultType;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.conditionExpression = (Expression)processor.Process(this.conditionExpression);
            this.trueCaseExpression = (Expression)processor.Process(this.trueCaseExpression);
            this.falseCaseExpression = (Expression)processor.Process(this.falseCaseExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("condition", this.Condition);
            serializationInfo.AddValue("trueCase", this.TrueCase);
            serializationInfo.AddValue("falseCase", this.FalseCase);
        }

        /// <summary>
        /// Inverts this instance.
        /// This inversion is really used because when decompiling, conditional always comes up as inverted
        /// </summary>
        public void Invert()
        {
            if (!this.inverted)
            {
                this.inverted = true;

                this.conditionExpression = new UnaryExpression(
                    this.Context,
                    this.conditionExpression.Location,
                    this.conditionExpression,
                    UnaryOperator.LogicalNot);

                var tmp = this.falseCaseExpression;
                this.falseCaseExpression = this.trueCaseExpression;
                this.trueCaseExpression = tmp;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            ConditionalOperatorExpression right = obj as ConditionalOperatorExpression;

            return right != null
                && this.Condition.Equals(right.Condition)
                && this.TrueCase.Equals(right.TrueCase)
                && this.FalseCase.Equals(right.FalseCase);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(ConditionalOperatorExpression).GetHashCode()
                ^ this.Condition.GetHashCode()
                ^ this.TrueCase.GetHashCode()
                ^ this.FalseCase.GetHashCode();
        }
    }
}
