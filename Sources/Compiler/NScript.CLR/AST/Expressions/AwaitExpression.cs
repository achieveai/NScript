namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class AwaitExpression : Expression
    {
        public AwaitExpression(
            ClrContext context,
            Location location,
            Expression expression,
            Expression getAwaiterMethodExpression)
            : base(context, location)
        {
            Expression = expression;
            GetAwaiterMethodExpression = getAwaiterMethodExpression;
        }

        public Expression Expression
        { get; private set; }

        public Expression GetAwaiterMethodExpression
        { get; private set; }

        public override TypeReference ResultType => Expression.ResultType;
    }
}
