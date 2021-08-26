using NScript.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace NScript.CLR.AST.Expressions
{
    class TupleExpression : Expression
    {
        public readonly IList<Expression> elements;
        public TupleExpression(ClrContext context, Location location, IList<Expression> elements) : base(context, location)
        {
            this.elements = elements;
        }
    }
}
