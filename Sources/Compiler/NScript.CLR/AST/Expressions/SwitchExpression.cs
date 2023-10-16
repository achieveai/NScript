using Mono.Cecil;
using NScript.Utils;
using System.Collections.Generic;

namespace NScript.CLR.AST
{
    public class SwitchExpression : Expression
    {
        public SwitchExpression(
            ClrContext context,
            Location location,
            Expression switchValue,
            List<Pattern> patterns,
            List<Expression> expressions,
            TypeReference resultType) : base(context, location)
        {
            SwitchValue = switchValue;
            Patterns = patterns;
            Expressions = expressions;
            ResultType = resultType;
        }

        public override TypeReference ResultType { get; }

        public Expression SwitchValue { get; }

        public List<Pattern> Patterns { get; }

        public List<Expression> Expressions { get; }
    }
}
