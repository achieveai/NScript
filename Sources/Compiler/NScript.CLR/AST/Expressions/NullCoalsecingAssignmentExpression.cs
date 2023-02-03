using Mono.Cecil;
using NScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NScript.CLR.AST
{
    public class NullCoalsecingAssignmentExpression : Expression
    {
        public NullCoalsecingAssignmentExpression(ClrContext context, Location location, Expression left, Expression right) : base(context, location)
        {
            Left = left;
            Right = right;
        }

        public Expression Left { get; private set; }

        public Expression Right { get; private set; }

        public override TypeReference ResultType => Right.ResultType;
    }
}
