//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
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
        /// for ... in .. operator.
        /// </summary>
        In,

        /// <summary>
        /// instanceof operator.
        /// </summary>
        InstanceOf,

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
        /// === operator.
        /// </summary>
        StrictEquals,

        /// <summary>
        /// !== operator.
        /// </summary>
        StrictNotEquals,

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
        /// Delete operator.
        /// </summary>
        Delete,

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
    /// Precedence of the expressession over other.
    /// </summary>
    public enum Precedence
    {
        /// <summary>
        /// Comma precedence (Weakest).
        /// </summary>
        Comma = 0,

        /// <summary>
        /// Expression precedence (Weakest).
        /// </summary>
        Expression = 0,

        /// <summary>
        /// Assignment precedence.
        /// </summary>
        Assignment = 1,

        /// <summary>
        /// ?: precedence.
        /// </summary>
        Conditional = 2,

        /// <summary>
        /// LogicalOr precedence.
        /// </summary>
        LogicalOr = 2,

        /// <summary>
        /// Logical And precedence.
        /// </summary>
        LogicalAnd = 3,

        /// <summary>
        /// BitwiseOr precedence.
        /// </summary>
        BitwiseOr = 4,

        /// <summary>
        /// BirwiseXor precedence.
        /// </summary>
        BitwiseXor = 5,

        /// <summary>
        /// BitwiseAnd precedence.
        /// </summary>
        BitwiseAnd = 6,

        /// <summary>
        /// Equality precedence.
        /// </summary>
        Equality = 7,

        /// <summary>
        /// Relational precedence.
        /// </summary>
        Relational = 8,

        /// <summary>
        /// Shift precedence.
        /// </summary>
        Shift = 9,

        /// <summary>
        /// Additive precedence.
        /// </summary>
        Additive = 10,

        /// <summary>
        /// Multiplicative precedence.
        /// </summary>
        Multiplicative = 11,

        /// <summary>
        /// Unary precedence.
        /// </summary>
        Unary = 12,

        /// <summary>
        /// PostFix precedence.
        /// </summary>
        IncrementDecrement = 13,

        /// <summary>
        /// LeftHandSide precedence.
        /// </summary>
        LHS = 14,

        /// <summary>
        /// Primary precedence.
        /// </summary>
        Primary = 15
    }

    /// <summary>
    /// Placement of operator relative to operands.
    /// </summary>
    public enum OperatorPlacement
    {
        /// <summary>
        /// The operator is placed before the operand.
        /// </summary>
        Prefix,

        /// <summary>
        /// The operator is placed between the operands.
        /// </summary>
        Infix,

        /// <summary>
        /// The operator is placed after the operand.
        /// </summary>
        Postfix
    }

    /// <summary>
    /// Definition for keyword enums
    /// </summary>
    public enum Keyword
    {
        /// <summary>
        /// async keyword
        /// </summary>
        Async,

        /// <summary>
        /// await keyword
        /// </summary>
        Await,

        /// <summary>
        /// break keyword
        /// </summary>
        Break,

        /// <summary>
        /// case keyword.
        /// </summary>
        Case,

        /// <summary>
        /// catch of try-catch keyword
        /// </summary>
        Catch,

        /// <summary>
        /// const keyword
        /// </summary>
        Const,

        /// <summary>
        /// continue keyword
        /// </summary>
        Continue,

        /// <summary>
        /// default keyword
        /// </summary>
        Default,

        /// <summary>
        /// delete keyword
        /// </summary>
        Delete,

        /// <summary>
        /// do of do-while keyword
        /// </summary>
        Do,

        /// <summary>
        /// else of if-else keyword
        /// </summary>
        Else,

        /// <summary>
        /// export keyword
        /// </summary>
        Export,

        /// <summary>
        /// false keyword.
        /// </summary>
        False,

        /// <summary>
        /// finally of try-finally keyword
        /// </summary>
        Finally,

        /// <summary>
        /// for keyword
        /// </summary>
        For,

        /// <summary>
        /// function keyword
        /// </summary>
        Function,

        /// <summary>
        /// if keyword
        /// </summary>
        If,

        /// <summary>
        /// import keyword
        /// </summary>
        Import,

        /// <summary>
        /// in keyword
        /// </summary>
        In,

        /// <summary>
        /// instranceOf keyword
        /// </summary>
        InstanceOf,

        /// <summary>
        /// label keyword
        /// </summary>
        Label,

        /// <summary>
        /// le
        /// let keyword
        /// </summary>
        Let,

        /// <summary>
        /// new keyword
        /// </summary>
        New,

        /// <summary>
        /// Null keyword.
        /// </summary>
        Null,

        /// <summary>
        /// return keyword
        /// </summary>
        Return,

        /// <summary>
        /// switch keyword
        /// </summary>
        Switch,

        /// <summary>
        /// this keyword
        /// </summary>
        This,

        /// <summary>
        /// throw keyword
        /// </summary>
        Throw,

        /// <summary>
        /// True keyword.
        /// </summary>
        True,

        /// <summary>
        /// try keyword
        /// </summary>
        Try,

        /// <summary>
        /// break keyword
        /// </summary>
        TypeOf,

        /// <summary>
        /// var keyword
        /// </summary>
        Var,

        /// <summary>
        /// void keyword
        /// </summary>
        Void,

        /// <summary>
        /// while keyword
        /// </summary>
        While,

        /// <summary>
        /// with keyword
        /// </summary>
        With,

        /// <summary>
        /// yield keyword
        /// </summary>
        Yield,
    }

    /// <summary>
    /// Definition for all the symbols in JS.
    /// </summary>
    public enum Symbols
    {
        /// <summary>
        /// = symbol.
        /// </summary>
        Assign,

        /// <summary>
        /// + operator
        /// </summary>
        And,

        /// <summary>
        /// += operator
        /// </summary>
        AndEquals,

        /// <summary>
        /// { symbol
        /// </summary>
        BrackedOpenCurly,

        /// <summary>
        /// } symbol
        /// </summary>
        BracketCloseCurly,

        /// <summary>
        /// ( symbol
        /// </summary>
        BracketOpenRound,

        /// <summary>
        /// ) symbol
        /// </summary>
        BracketCloseRound,

        /// <summary>
        /// [ symbol
        /// </summary>
        BrackedOpenSquare,

        /// <summary>
        /// ] symbol
        /// </summary>
        BracketCloseSquare,

        /// <summary>
        /// : symbol both for case labels.
        /// </summary>
        Colon,

        /// <summary>
        /// /* comment start symbol
        /// </summary>
        CommentStart,

        /// <summary>
        /// */ comment end symbol.
        /// </summary>
        CommentEnd,

        /// <summary>
        /// , symbol.
        /// </summary>
        Comma,

        /// <summary>
        /// ? from ?:
        /// </summary>
        Conditional,

        /// <summary>
        /// : from ?:
        /// </summary>
        ConditionalElse,

        /// <summary>
        /// / operator.
        /// </summary>
        Divide,

        /// <summary>
        /// /= operator
        /// .
        /// </summary>
        DivideEquals,

        /// <summary>
        /// . operator
        /// </summary>
        Dot,

        /// <summary>
        /// == operator.
        /// </summary>
        Equals,

        /// <summary>
        /// === operator
        /// </summary>
        EqualsReally,

        /// <summary>
        /// > operator
        /// </summary>
        GreaterThan,

        /// <summary>
        /// >= operator
        /// </summary>
        GreaterThanEquals,

        /// <summary>
        /// ~ operator.
        /// </summary>
        Inverse,

        /// <summary>
        /// < operator
        /// </summary>
        LessThan,

        /// <summary>
        /// <= operator
        /// </summary>
        LessThanEquals,

        /// <summary>
        /// && operator.
        /// </summary>
        LogicalAnd,

        /// <summary>
        /// || operator.
        /// </summary>
        LogicalOr,

        /// <summary>
        /// % operator
        /// </summary>
        Modulus,

        /// <summary>
        /// %= operator
        /// </summary>
        ModulusEquals,

        /// <summary>
        /// - operator.
        /// </summary>
        Minus,

        /// <summary>
        /// -= operator
        /// </summary>
        MinusEquals,

        /// <summary>
        /// * operator
        /// </summary>
        Multiply,

        /// <summary>
        /// *= operator
        /// </summary>
        MultiplyEquals,

        /// <summary>
        /// ! operator
        /// </summary>
        Not,

        /// <summary>
        /// != operator
        /// </summary>
        NotEquals,

        /// <summary>
        /// !=== operator
        /// </summary>
        NotEqualsReally,

        /// <summary>
        /// | operator
        /// </summary>
        Or,

        /// <summary>
        /// |= operator
        /// </summary>
        OrEquals,

        /// <summary>
        /// + operator
        /// </summary>
        Plus,

        /// <summary>
        /// += operator
        /// </summary>
        PlusEquals,

        /// <summary>
        /// ()-- operator
        /// </summary>
        PostDecrement,

        /// <summary>
        /// ()++ operator
        /// </summary>
        PostIncrement,

        /// <summary>
        /// --() operator
        /// </summary>
        PreDecrement,

        /// <summary>
        /// ++() operator
        /// </summary>
        PreIncrement,

        /// <summary>
        /// ; symbol
        /// </summary>
        SemiColon,

        /// <summary>
        /// >> operator
        /// </summary>
        ShiftRight,

        /// <summary>
        /// >>= operator
        /// </summary>
        ShiftRightEquals,

        /// <summary>
        /// << operator
        /// </summary>
        ShiftLeft,

        /// <summary>
        /// <<= operator
        /// </summary>
        ShiftLeftEquals,

        /// <summary>
        /// single line comment (//)
        /// </summary>
        SingleLineComment,

        /// <summary>
        /// -() operator
        /// </summary>
        UnaryMinus,

        /// <summary>
        /// >>> operator
        /// </summary>
        UnsignedShiftRight,

        /// <summary>
        /// >>>= operator
        /// </summary>
        UnsignedShiftRightEquals,

        /// <summary>
        /// ^ operator
        /// </summary>
        Xor,

        /// <summary>
        /// ^= operator
        /// </summary>
        XorEquals,
    }
}
