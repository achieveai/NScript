//-----------------------------------------------------------------------
// <copyright file="UnaryExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for UnaryExpression
    /// </summary>
    public class UnaryExpression : Expression
    {
        /// <summary>
        /// Backing field for Operator.
        /// </summary>
        private readonly UnaryOperator op;

        /// <summary>
        /// Backing field for Expression.
        /// </summary>
        private Expression innerExpression;

        /// <summary>
        /// Backing field for IsLifted.
        /// </summary>
        private bool isLifted;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="innerExpression">The inner expression.</param>
        /// <param name="op">The op.</param>
        /// <param name="isLifted">if set to <c>true</c> [is lifted].</param>
        public UnaryExpression(
            ClrContext context,
            Location location,
            Expression innerExpression,
            UnaryOperator op,
            bool isLifted = false)
            : base(context, location)
        {
            this.innerExpression = innerExpression;
            this.op = op;
            this.isLifted = isLifted;
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public Expression Expression
        {
            get
            {
                return this.innerExpression;
            }
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public UnaryOperator Operator
        {
            get
            {
                return this.op;
            }
        }

        public bool IsLifted
        {
            get
            {
                return this.isLifted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is assignment.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is assignment; otherwise, <c>false</c>.
        /// </value>
        public bool IsAssignment
        {
            get
            {
                switch (this.Operator)
                {
                    case UnaryOperator.BitwiseNot:
                    case UnaryOperator.LogicalNot:
                    case UnaryOperator.TypeOf:
                    case UnaryOperator.UnaryMinus:
                    case UnaryOperator.UnaryPlus:
                    case UnaryOperator.Void:
                        return false;
                    case UnaryOperator.PostDecrement:
                    case UnaryOperator.PostIncrement:
                    case UnaryOperator.PreDecrement:
                    case UnaryOperator.PreIncrement:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
                TypeReference rv = null;
                switch (this.op)
                {
                    case UnaryOperator.LogicalNot:
                        return this.KnownReferences.Boolean;
                    case UnaryOperator.BitwiseNot:
                    case UnaryOperator.PostDecrement:
                    case UnaryOperator.PreDecrement:
                    case UnaryOperator.PostIncrement:
                    case UnaryOperator.PreIncrement:
                    case UnaryOperator.UnaryMinus:
                    case UnaryOperator.UnaryPlus:
                        rv = this.Expression.ResultType;
                        break;
                    case UnaryOperator.Void:
                        return null;
                    default:
                        throw new InvalidOperationException();
                }

                if (isLifted)
                {
                    var tmp = new GenericInstanceType(this.Context.KnownReferences.NullableType);
                    tmp.GenericArguments.Add(rv);
                    rv = tmp;
                }

                return rv;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.innerExpression = (Expression)processor.Process(this.innerExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("operator", this.Operator);
            serializationInfo.AddValue("expression", this.Expression);
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
            UnaryExpression right = obj as UnaryExpression;

            return right != null
                && this.Operator == right.Operator
                && this.Expression.Equals(right.Expression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Expression.GetHashCode() + (int)this.Operator;
        }
    }
}