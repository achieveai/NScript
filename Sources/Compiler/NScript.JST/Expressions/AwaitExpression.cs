using NScript.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace NScript.JST
{
    public class AwaitExpression : Expression
    {
        public AwaitExpression(Location location, IdentifierScope scope, Expression expression) : base(location, scope)
        {
            Expression = expression;
        }

        public Expression Expression
        { get; private set; }

        public override Precedence Precedence
            => Precedence.Comma;

        public override OperatorPlacement OperatorPlacement => OperatorPlacement.Prefix;

        public override void Write(JSWriter writer)
        {
            writer.Write(Keyword.Await);
            writer.Write(Expression);
        }
    }
}
