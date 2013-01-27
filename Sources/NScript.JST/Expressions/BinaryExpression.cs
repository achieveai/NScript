//-----------------------------------------------------------------------
// <copyright file="BinaryExpression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using NScript.Utils;

    /// <summary>
    /// Binary expression that works on two set of expressions.
    /// </summary>
    public class BinaryExpression : Expression
    {
        /// <summary>
        /// Binary operator
        /// </summary>
        private BinaryOperator binaryOp;

        /// <summary>
        /// Backing field for left expression..
        /// </summary>
        private Expression leftExpression;

        /// <summary>
        /// Backing field for rightExpression.
        /// </summary>
        private Expression rightExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExpression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="op">The binary operator.</param>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="rightExpression">The right expression.</param>
        public BinaryExpression(
            Location location,
            IdentifierScope scope,
            BinaryOperator op,
            Expression leftExpression,
            Expression rightExpression)
            : base(location, scope)
        {
            this.binaryOp = op;
            this.leftExpression = leftExpression;
            this.rightExpression = rightExpression;
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public BinaryOperator Operator
        {
            get
            {
                return this.binaryOp;
            }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left expression.</value>
        public Expression Left
        {
            get
            {
                return this.leftExpression;
            }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right expression.</value>
        public Expression Right
        {
            get
            {
                return this.rightExpression;
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
                switch (this.Operator)
                {
                    case BinaryOperator.Assignment:
                    case BinaryOperator.BitwiseAndAssignment:
                    case BinaryOperator.BitwiseOrAssignment:
                    case BinaryOperator.BitwiseXorAssignment:
                    case BinaryOperator.DivAssignment:
                    case BinaryOperator.LeftShiftAssignment:
                    case BinaryOperator.MinusAssignment:
                    case BinaryOperator.ModAssignment:
                    case BinaryOperator.PlusAssignment:
                    case BinaryOperator.RightShiftAssignment:
                    case BinaryOperator.MulAssignment:
                    case BinaryOperator.UnsignedRightShiftAssignment:
                        return Precedence.Assignment;
                    case BinaryOperator.BitwiseAnd:
                        return Precedence.BitwiseAnd;
                    case BinaryOperator.BitwiseOr:
                        return Precedence.BitwiseOr;
                    case BinaryOperator.BitwiseXor:
                        return Precedence.BitwiseXor;
                    case BinaryOperator.Comma:
                        return Precedence.Expression;
                    case BinaryOperator.Div:
                    case BinaryOperator.Mod:
                    case BinaryOperator.Mul:
                        return Precedence.Multiplicative;
                    case BinaryOperator.Equals:
                    case BinaryOperator.NotEquals:
                    case BinaryOperator.StrictEquals:
                    case BinaryOperator.StrictNotEquals:
                        return Precedence.Equality;
                    case BinaryOperator.GreaterThan:
                    case BinaryOperator.GreaterThanOrEqual:
                    case BinaryOperator.In:
                    case BinaryOperator.InstanceOf:
                    case BinaryOperator.LessThan:
                    case BinaryOperator.LessThanOrEqual:
                        return Precedence.Relational;
                    case BinaryOperator.LogicalAnd:
                        return Precedence.LogicalAnd;
                    case BinaryOperator.LogicalOr:
                        return Precedence.LogicalOr;
                    case BinaryOperator.Minus:
                    case BinaryOperator.Plus:
                        return Precedence.Additive;
                    case BinaryOperator.LeftShift:
                    case BinaryOperator.RightShift:
                    case BinaryOperator.UnsignedRightShift:
                        return Precedence.Shift;
                    default:
                        throw new InvalidOperationException();
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
                switch (this.Operator)
                {
                    case BinaryOperator.Assignment:
                    case BinaryOperator.BitwiseAndAssignment:
                    case BinaryOperator.BitwiseOrAssignment:
                    case BinaryOperator.BitwiseXorAssignment:
                    case BinaryOperator.DivAssignment:
                    case BinaryOperator.LeftShiftAssignment:
                    case BinaryOperator.MinusAssignment:
                    case BinaryOperator.ModAssignment:
                    case BinaryOperator.PlusAssignment:
                    case BinaryOperator.RightShiftAssignment:
                    case BinaryOperator.MulAssignment:
                    case BinaryOperator.UnsignedRightShiftAssignment:
                        return false;
                    case BinaryOperator.Comma:
                    case BinaryOperator.BitwiseAnd:
                    case BinaryOperator.BitwiseOr:
                    case BinaryOperator.BitwiseXor:
                    case BinaryOperator.Div:
                    case BinaryOperator.Mod:
                    case BinaryOperator.Mul:
                    case BinaryOperator.Equals:
                    case BinaryOperator.NotEquals:
                    case BinaryOperator.StrictEquals:
                    case BinaryOperator.StrictNotEquals:
                    case BinaryOperator.GreaterThan:
                    case BinaryOperator.GreaterThanOrEqual:
                    case BinaryOperator.In:
                    case BinaryOperator.InstanceOf:
                    case BinaryOperator.LessThan:
                    case BinaryOperator.LessThanOrEqual:
                    case BinaryOperator.LogicalAnd:
                    case BinaryOperator.LogicalOr:
                    case BinaryOperator.Minus:
                    case BinaryOperator.Plus:
                    case BinaryOperator.LeftShift:
                    case BinaryOperator.RightShift:
                    case BinaryOperator.UnsignedRightShift:
                        return true;
                    default:
                        throw new InvalidOperationException();
                }
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
            serializer.AddValue("operator", this.Operator);
            serializer.AddValue("left", this.Left);
            serializer.AddValue("right", this.Right);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            Symbols? binaryOpSym = null;
            Keyword? keyword = null;

            switch (this.binaryOp)
            {
                case BinaryOperator.Assignment:
                    binaryOpSym = Symbols.Assign;
                    break;
                case BinaryOperator.BitwiseAnd:
                    binaryOpSym = Symbols.And;
                    break;
                case BinaryOperator.BitwiseAndAssignment:
                    binaryOpSym = Symbols.AndEquals;
                    break;
                case BinaryOperator.BitwiseOr:
                    binaryOpSym = Symbols.Or;
                    break;
                case BinaryOperator.BitwiseOrAssignment:
                    binaryOpSym = Symbols.OrEquals;
                    break;
                case BinaryOperator.BitwiseXor:
                    binaryOpSym = Symbols.Xor;
                    break;
                case BinaryOperator.BitwiseXorAssignment:
                    binaryOpSym = Symbols.XorEquals;
                    break;
                case BinaryOperator.Comma:
                    binaryOpSym = Symbols.Comma;
                    break;
                case BinaryOperator.Div:
                    binaryOpSym = Symbols.Divide;
                    break;
                case BinaryOperator.DivAssignment:
                    binaryOpSym = Symbols.DivideEquals;
                    break;
                case BinaryOperator.Equals:
                    binaryOpSym = Symbols.Equals;
                    break;
                case BinaryOperator.GreaterThan:
                    binaryOpSym = Symbols.GreaterThan;
                    break;
                case BinaryOperator.GreaterThanOrEqual:
                    binaryOpSym = Symbols.GreaterThanEquals;
                    break;
                case BinaryOperator.In:
                    keyword = Keyword.In;
                    break;
                case BinaryOperator.InstanceOf:
                    keyword = Keyword.InstanceOf;
                    break;
                case BinaryOperator.LeftShift:
                    binaryOpSym = Symbols.ShiftLeft;
                    break;
                case BinaryOperator.LeftShiftAssignment:
                    binaryOpSym = Symbols.ShiftLeftEquals;
                    break;
                case BinaryOperator.LessThan:
                    binaryOpSym = Symbols.LessThan;
                    break;
                case BinaryOperator.LessThanOrEqual:
                    binaryOpSym = Symbols.LessThanEquals;
                    break;
                case BinaryOperator.LogicalAnd:
                    binaryOpSym = Symbols.LogicalAnd;
                    break;
                case BinaryOperator.LogicalOr:
                    binaryOpSym = Symbols.LogicalOr;
                    break;
                case BinaryOperator.Minus:
                    binaryOpSym = Symbols.Minus;
                    break;
                case BinaryOperator.MinusAssignment:
                    binaryOpSym = Symbols.MinusEquals;
                    break;
                case BinaryOperator.Mod:
                    binaryOpSym = Symbols.Modulus;
                    break;
                case BinaryOperator.ModAssignment:
                    binaryOpSym = Symbols.ModulusEquals;
                    break;
                case BinaryOperator.NotEquals:
                    binaryOpSym = Symbols.NotEquals;
                    break;
                case BinaryOperator.Plus:
                    binaryOpSym = Symbols.Plus;
                    break;
                case BinaryOperator.PlusAssignment:
                    binaryOpSym = Symbols.PlusEquals;
                    break;
                case BinaryOperator.RightShift:
                    binaryOpSym = Symbols.ShiftRight;
                    break;
                case BinaryOperator.RightShiftAssignment:
                    binaryOpSym = Symbols.ShiftRightEquals;
                    break;
                case BinaryOperator.StrictEquals:
                    binaryOpSym = Symbols.EqualsReally;
                    break;
                case BinaryOperator.StrictNotEquals:
                    binaryOpSym = Symbols.NotEqualsReally;
                    break;
                case BinaryOperator.Mul:
                    binaryOpSym = Symbols.Multiply;
                    break;
                case BinaryOperator.MulAssignment:
                    binaryOpSym = Symbols.MultiplyEquals;
                    break;
                case BinaryOperator.UnsignedRightShift:
                    binaryOpSym = Symbols.UnsignedShiftRight;
                    break;
                case BinaryOperator.UnsignedRightShiftAssignment:
                    binaryOpSym = Symbols.UnsignedShiftRightEquals;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            writer.Write(
                    this.Left,
                    this.Left.OperatorPlacement != JST.OperatorPlacement.Postfix
                        && this.Left.Precedence < this.Precedence);

            if (binaryOpSym.HasValue)
            {
                writer.Write(binaryOpSym.Value);
            }
            else
            {
                writer.Write(keyword.Value);
            }

            writer.Write(
                this.rightExpression,
                this.Right.OperatorPlacement != JST.OperatorPlacement.Prefix
                    && this.Right.Precedence < this.Precedence);
        }
    }
}
