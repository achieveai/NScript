using Mono.Cecil;
using NScript.Utils;

namespace NScript.CLR.AST
{
    public class IsPatternExpression : Expression
    {
        public IsPatternExpression(ClrContext context, Location location, Expression lhs, CaseLabel pattern) : base(context, location)
        {
            Lhs = lhs;
            Pattern = pattern;
        }

        public Expression Lhs { get; }

        public CaseLabel Pattern { get; }

        public override TypeReference ResultType => Context.KnownReferences.Boolean;
    }
}