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

        public BinaryExpression AsBinaryExpression()
        {
            return new BinaryExpression(
                Context,
                Location,
                Left,
                new NullConditional(
                    Context,
                    Location,
                    Left,
                    Right,
                    ResultType),
                BinaryOperator.Assignment);
        }
    }
}
