using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.AsmDeasm.Ast
{
    public enum AstType
    {
        UnitWork,
        Assignment,
        SimpleExpression,
        BinaryExpression,
        Return,
        Method,
        UnaryExpression,
        Block,
        Constructor,
    }
}
