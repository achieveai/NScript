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
            Expression getAwaiterCallExpression)
            : base(context, location)
        {
            Expression = expression;
            GetAwaiterCallExpression = getAwaiterCallExpression;
        }

        public Expression Expression
        { get; private set; }

        public Expression GetAwaiterCallExpression
        { get; private set; }

        public override TypeReference ResultType => Expression.ResultType;
    }
}
