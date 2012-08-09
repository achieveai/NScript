using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    public enum BinaryOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulus,
        ShiftLeft,
        ShiftRight,
        IdentityEquality,
        IdentityInequality,
        ValueEquality,
        ValueInequality,
        BitwiseOr,
        BitwiseAnd,
        BitwiseExclusiveOr,
        BooleanOr,
        BooleanAnd,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    public enum UnaryOperator
    {
        Not,
        Negate,
        NoOp
    }
}
