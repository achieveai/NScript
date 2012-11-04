//-----------------------------------------------------------------------
// <copyright file="UnaryExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Cs2JsC.Utils;
namespace Cs2JsC.JST
{
    /// <summary>
    /// UnaryExpression for unary operators..
    /// </summary>
    public class UnaryExpression : Expression
    {
        /// <summary>
        /// Backing fiedl for Operator
        /// </summary>
        private readonly UnaryOperator unaryOp;

        /// <summary>
        /// Backing field for NestedExpression.
        /// </summary>
        private readonly Expression nestedExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="op">The operator.</param>
        /// <param name="nestedExpression">The nested expression.</param>
        public UnaryExpression(
            Location location,
            IdentifierScope scope,
            UnaryOperator op,
            Expression nestedExpression)
            : base(location, scope)
        {
            this.unaryOp = op;
            this.nestedExpression = nestedExpression;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public UnaryOperator Operator
        {
            get
            {
                return this.unaryOp;
            }
        }

        /// <summary>
        /// Gets the nested expression.
        /// </summary>
        /// <value>The nested expression.</value>
        public Expression NestedExpression
        {
            get
            {
                return this.nestedExpression;
            }
        }

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        /// <value>The precedence.</value>
        public override Precedence Precedence
        {
            get
            {
                if (this.Operator == UnaryOperator.PreDecrement
                    || this.Operator == UnaryOperator.PreIncrement
                    || this.Operator == UnaryOperator.PostDecrement
                    || this.Operator == UnaryOperator.PostIncrement)
                {
                    return Precedence.IncrementDecrement;
                }
                else
                {
                    return Precedence.Unary;
                }
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
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            switch (this.Operator)
            {
                case UnaryOperator.Delete:
                    writer.Write(Keyword.Delete);
                    break;
                case UnaryOperator.BitwiseNot:
                    writer.Write(Symbols.Inverse);
                    break;
                case UnaryOperator.LogicalNot:
                    writer.Write(Symbols.Not);
                    break;
                case UnaryOperator.PostDecrement:
                case UnaryOperator.PostIncrement:
                    break;
                case UnaryOperator.PreDecrement:
                    writer.Write(Symbols.PreDecrement);
                    break;
                case UnaryOperator.PreIncrement:
                    writer.Write(Symbols.PreIncrement);
                    break;
                case UnaryOperator.TypeOf:
                    writer.Write(Keyword.TypeOf);
                    break;
                case UnaryOperator.UnaryMinus:
                    writer.Write(Symbols.UnaryMinus);
                    break;
                case UnaryOperator.UnaryPlus:
                    writer.Write(Symbols.Plus);
                    break;
                case UnaryOperator.Void:
                    throw new System.NotImplementedException();
            }

            writer.Write(this.nestedExpression, this.Precedence > this.nestedExpression.Precedence);

            if (this.Operator == UnaryOperator.PostDecrement)
            {
                writer.Write(Symbols.PostDecrement);
            }
            else if (this.Operator == UnaryOperator.PostIncrement)
            {
                writer.Write(Symbols.PostIncrement);
            }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(Cs2JsC.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("operator", this.Operator);
            serializer.AddValue("expression", this.NestedExpression);
        }

        /// <summary>
        /// Determines whether the specified op is postfix.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <returns>
        /// <c>true</c> if the specified op is postfix; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsPostfix(UnaryOperator op)
        {
            switch (op)
            {
                case UnaryOperator.PostDecrement:
                case UnaryOperator.PostIncrement:
                    return true;
            }

            return false;
        }
    }
}
