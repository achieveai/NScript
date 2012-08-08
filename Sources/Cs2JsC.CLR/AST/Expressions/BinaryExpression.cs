namespace Cs2JsC.CLR.AST
{
    using System;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Binary expression.
    /// </summary>
    public class BinaryExpression : Expression
    {
        /// <summary>
        /// Backing field for Operator.
        /// </summary>
        private readonly BinaryOperator op;

        /// <summary>
        /// Backing field for LeftExpression.
        /// </summary>
        private Expression leftExpression;

        /// <summary>
        /// Backing field for RightExpression.
        /// </summary>
        private Expression rightExpression;

        /// <summary>
        /// Backing field for IsLifted.
        /// </summary>
        private bool isLifted;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        /// <param name="op">The operator.</param>
        public BinaryExpression(
            ClrContext context,
            Location location,
            Expression left,
            Expression right,
            BinaryOperator op,
            bool isLifted = false)
            : base(context, location)
        {
            this.leftExpression = left;
            this.rightExpression = right;
            this.op = op;
            this.isLifted = isLifted;
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
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
        /// <value>The right.</value>
        public Expression Right
        {
            get
            {
                return this.rightExpression;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is lifted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is lifted; otherwise, <c>false</c>.
        /// </value>
        public bool IsLifted
        {
            get { return this.isLifted; }
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <value>The operator.</value>
        public BinaryOperator Operator
        {
            get
            {
                return this.op;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is assignment operator.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is assignment operator; otherwise, <c>false</c>.
        /// </value>
        public bool IsAssignmentOperator
        {
            get
            {
                switch (Operator)
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
                        return true;
                    case BinaryOperator.BitwiseAnd:
                    case BinaryOperator.BitwiseOr:
                    case BinaryOperator.BitwiseXor:
                    case BinaryOperator.Comma:
                    case BinaryOperator.Div:
                    case BinaryOperator.Equals:
                    case BinaryOperator.GreaterThan:
                    case BinaryOperator.GreaterThanOrEqual:
                    case BinaryOperator.IsTypeOf:
                    case BinaryOperator.LeftShift:
                    case BinaryOperator.LessThan:
                    case BinaryOperator.LessThanOrEqual:
                    case BinaryOperator.LogicalAnd:
                    case BinaryOperator.LogicalOr:
                    case BinaryOperator.Minus:
                    case BinaryOperator.Mod:
                    case BinaryOperator.NotEquals:
                    case BinaryOperator.Plus:
                    case BinaryOperator.RightShift:
                    case BinaryOperator.Mul:
                    case BinaryOperator.UnsignedRightShift:
                        return false;
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
                switch (this.Operator)
                {
                    case BinaryOperator.Assignment:
                        return this.Right.ResultType;
                    case BinaryOperator.BitwiseAnd:
                    case BinaryOperator.BitwiseAndAssignment:
                    case BinaryOperator.BitwiseOr:
                    case BinaryOperator.BitwiseOrAssignment:
                    case BinaryOperator.BitwiseXor:
                    case BinaryOperator.BitwiseXorAssignment:
                        rv = this.Left.ResultType;
                        break;
                    case BinaryOperator.Div:
                    case BinaryOperator.Minus:
                    case BinaryOperator.Plus:
                    case BinaryOperator.Mul:
                        if (BinaryExpression.CompareTypes(this.Left.ResultType, this.Right.ResultType) >= 0)
                        {
                            rv = this.Left.ResultType;
                        }
                        else
                        {
                            rv = this.Right.ResultType;
                        }

                        break;
                    case BinaryOperator.Equals:
                    case BinaryOperator.GreaterThan:
                    case BinaryOperator.GreaterThanOrEqual:
                    case BinaryOperator.IsTypeOf:
                    case BinaryOperator.NotEquals:
                        return this.KnownReferences.Boolean;
                    case BinaryOperator.LessThan:
                    case BinaryOperator.LessThanOrEqual:
                    case BinaryOperator.LogicalAnd:
                    case BinaryOperator.LogicalOr:
                        return this.KnownReferences.Boolean;
                    case BinaryOperator.DivAssignment:
                    case BinaryOperator.LeftShift:
                    case BinaryOperator.LeftShiftAssignment:
                    case BinaryOperator.MinusAssignment:
                    case BinaryOperator.Mod:
                    case BinaryOperator.ModAssignment:
                    case BinaryOperator.PlusAssignment:
                    case BinaryOperator.RightShift:
                    case BinaryOperator.RightShiftAssignment:
                    case BinaryOperator.MulAssignment:
                    case BinaryOperator.UnsignedRightShift:
                    case BinaryOperator.UnsignedRightShiftAssignment:
                        rv = this.Left.ResultType;
                        break;
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
        /// <param name="processorFunction">The processor function.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.leftExpression = (Expression)processor.Process(this.leftExpression);
            this.rightExpression = (Expression)processor.Process(this.rightExpression);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("operator", this.Operator);
            serializationInfo.AddValue("left", this.Left);
            serializationInfo.AddValue("right", this.Right);
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
            BinaryExpression right = obj as BinaryExpression;

            return right != null
                && this.Operator == right.Operator
                && this.Left.Equals(right.Left)
                && this.Right.Equals(right.Right);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(BooleanLiteral).GetHashCode()
                ^ (int)this.Operator
                ^ this.Left.GetHashCode()
                ^ ~this.Right.GetHashCode();
        }

        /// <summary>
        /// Compares the types.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>&gt; 0 if left is bigger than right &lt; 0 if less, 0 if equals</returns>
        public static int CompareTypes(TypeReference left, TypeReference right)
        {
            int? leftBigness = BinaryExpression.GetTypeBigness(left);
            if (!leftBigness.HasValue)
            { return 0; }

            int? rightBigness = BinaryExpression.GetTypeBigness(right);
            if (!rightBigness.HasValue)
            { return 0; }

            return leftBigness.Value - rightBigness.Value;
        }

        /// <summary>
        /// Gets the type bigness.
        /// </summary>
        /// <param name="typeReferenceBase">The type reference base.</param>
        /// <returns>How big the type storate is.</returns>
        private static int? GetTypeBigness(TypeReference typeReferenceBase)
        {
            TypeReference typeReference = typeReferenceBase as TypeReference;
            if (typeReference == null)
            { return null; }

            if (typeReference.GetTypeCode() == TypeCode.Boolean)
            { return 1; }

            if (typeReference.GetTypeCode() == TypeCode.Byte
                || typeReference.GetTypeCode() == TypeCode.SByte)
            { return 2; }

            if (typeReference.GetTypeCode() == TypeCode.Char
                || typeReference.GetTypeCode() == TypeCode.UInt16
                || typeReference.GetTypeCode() == TypeCode.Int16)
            { return 3; }

            if (typeReference.MetadataType == MetadataType.Int32
                || typeReference.MetadataType == MetadataType.UInt16
                || typeReference.MetadataType == MetadataType.IntPtr
                || typeReference.MetadataType == MetadataType.UIntPtr)
            { return 4; }

            if (typeReference.MetadataType == MetadataType.Int64
                || typeReference.MetadataType == MetadataType.UInt64)
            { return 5; }

            if (typeReference.MetadataType == MetadataType.Single)
            { return 6; }

            if (typeReference.MetadataType == MetadataType.Double)
            { return 7; }

            return null;
        }
    }
}