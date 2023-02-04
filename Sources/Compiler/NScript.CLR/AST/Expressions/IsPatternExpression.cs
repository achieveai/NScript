using Mono.Cecil;
using NScript.Utils;

namespace NScript.CLR.AST
{
    public class IsPatternExpression : Expression
    {
        public IsPatternExpression(ClrContext context, Location location, Expression lhs, Pattern pattern) : base(context, location)
        {
            Lhs = lhs;
            Pattern = pattern;
        }

        public Expression Lhs { get; set; }

        public Pattern Pattern { get; set; }

        public override TypeReference ResultType => Context.KnownReferences.Boolean;
    }

    public abstract class Pattern : Node
    {
        public Pattern(ClrContext clrContext, Location location) : base(clrContext, location)
        {
        }
    }

    public class DeclarationPattern : Pattern
    {
        public DeclarationPattern(ClrContext clrContext, Location location, Expression variableAccess, TypeReference type) : base(clrContext, location)
        {
            Type = type;
            VariableAccess = variableAccess;
        }

        public Expression VariableAccess { get; set; }

        public TypeReference Type { get; set; }
    }

    public class ConstantPattern : Pattern
    {
        public ConstantPattern(ClrContext clrContext, Location location, Expression constantExpression) : base(clrContext, location)
        {
            ConstantExpression = constantExpression;
        }

        public Expression ConstantExpression { get; set; }
    }
}
