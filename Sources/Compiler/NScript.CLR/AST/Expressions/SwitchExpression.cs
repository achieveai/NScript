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
            List<CaseLabel> caseLabels,
            List<Expression> expressions,
            TypeReference resultType) : base(context, location)
        {
            SwitchValue = switchValue;
            CaseLabels = caseLabels;
            Expressions = expressions;
            ResultType = resultType;
        }

        public override TypeReference ResultType { get; }

        public Expression SwitchValue { get; }

        public List<CaseLabel> CaseLabels { get; }

        public List<Expression> Expressions { get; }
    }
}
