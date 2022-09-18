namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using NScript.Utils;
    using System.Collections.Generic;

    public class InterpolatedStringExpression: Expression
    {
        public InterpolatedStringExpression(
            ClrContext context,
            Location location,
            List<Expression> parts)
            : base(context, location)
        {
            Parts = parts;
        }

        public override TypeReference ResultType => KnownReferences.String;

        public List<Expression> Parts
        { get; }
    }
}
