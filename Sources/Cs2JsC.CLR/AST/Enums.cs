namespace Cs2JsC.CLR.AST
{
    /// <summary>
    /// Binary operators for expressions.
    /// </summary>
    public enum BinaryOperator
    {
        /// <summary>
        /// Assignment operator.
        /// </summary>
        Assignment,

        /// <summary>
        /// Bitwise and operator.
        /// </summary>
        BitwiseAnd,

        /// <summary>
        /// &amp;= operator.
        /// </summary>
        BitwiseAndAssignment,

        /// <summary>
        /// | operator.
        /// </summary>
        BitwiseOr,

        /// <summary>
        /// |= operator.
        /// </summary>
        BitwiseOrAssignment,

        /// <summary>
        /// ^ operator.
        /// </summary>
        BitwiseXor,

        /// <summary>
        /// ^= operator.
        /// </summary>
        BitwiseXorAssignment,

        /// <summary>
        /// , operator.
        /// </summary>
        Comma,

        /// <summary>
        /// / operator.
        /// </summary>
        Div,

        /// <summary>
        /// /= operator.
        /// </summary>
        DivAssignment,

        /// <summary>
        /// == operator.
        /// </summary>
        Equals,

        /// <summary>
        /// &gt; operator.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// &gt;= operator.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// is operator.
        /// </summary>
        IsTypeOf,

        /// <summary>
        /// *lt;&lt; operator.
        /// </summary>
        LeftShift,

        /// <summary>
        /// *lt;&lt;= operator.
        /// </summary>
        LeftShiftAssignment,

        /// <summary>
        /// &lt; operator.
        /// </summary>
        LessThan,

        /// <summary>
        /// &lt;= operator.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// &amp;&amp; operator.
        /// </summary>
        LogicalAnd,

        /// <summary>
        /// || operator.
        /// </summary>
        LogicalOr,

        /// <summary>
        /// - operator.
        /// </summary>
        Minus,

        /// <summary>
        /// -= operator.
        /// </summary>
        MinusAssignment,

        /// <summary>
        /// % operator.
        /// </summary>
        Mod,

        /// <summary>
        /// %= operator.
        /// </summary>
        ModAssignment,

        /// <summary>
        /// != operator.
        /// </summary>
        NotEquals,

        /// <summary>
        /// + operator.
        /// </summary>
        Plus,

        /// <summary>
        /// += operator.
        /// </summary>
        PlusAssignment,

        /// <summary>
        /// &gt;&gt; operator.
        /// </summary>
        RightShift,

        /// <summary>
        /// &gt;&gt;= operator.
        /// </summary>
        RightShiftAssignment,

        /// <summary>
        /// * operator.
        /// </summary>
        Mul,

        /// <summary>
        /// *= operator.
        /// </summary>
        MulAssignment,

        /// <summary>
        /// &gt;&gt;&gt; operator.
        /// </summary>
        UnsignedRightShift,

        /// <summary>
        /// &gt;&gt;&gt;= operator.
        /// </summary>
        UnsignedRightShiftAssignment
    }

    /// <summary>
    /// Unary operator for expressions.
    /// </summary>
    public enum UnaryOperator
    {
        /// <summary>
        /// Bitwise Not operator.
        /// </summary>
        BitwiseNot,

        /// <summary>
        /// Logical Not operator.
        /// </summary>
        LogicalNot,

        /// <summary>
        /// Post Decrement operator.
        /// </summary>
        PostDecrement,

        /// <summary>
        /// Post increment operator.
        /// </summary>
        PostIncrement,

        /// <summary>
        /// Pre increment operator.
        /// </summary>
        PreDecrement,

        /// <summary>
        /// Pre increment oprerator.
        /// </summary>
        PreIncrement,

        /// <summary>
        /// Typeof operator.
        /// </summary>
        TypeOf,

        /// <summary>
        /// UnaryMinus operator.
        /// </summary>
        UnaryMinus,

        /// <summary>
        /// Unary Plus.
        /// </summary>
        UnaryPlus,

        /// <summary>
        /// Void operator.
        /// </summary>
        Void
    }

    /// <summary>
    /// Size of integer.
    /// </summary>
    public enum IntSize
    {
        /// <summary>
        /// 8 bit integer.
        /// </summary>
        I8,

        /// <summary>
        /// 16 bit integer.
        /// </summary>
        I16,

        /// <summary>
        /// 32 bit integer.
        /// </summary>
        I32,

        /// <summary>
        /// 64 bit integer.
        /// </summary>
        I64,

        /// <summary>
        /// Pointer size.
        /// </summary>
        Ptr,
    }

    /// <summary>
    /// Type of type check.
    /// </summary>
    public enum TypeCheckType
    {
        /// <summary>
        /// This one is used for 'is' operator.
        /// </summary>
        IsType,

        /// <summary>
        /// This one is used for cast operator.
        /// </summary>
        CastType,

        /// <summary>
        /// This is used for 'as' operator.
        /// </summary>
        AsType,
    }
}
