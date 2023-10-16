using Mono.Cecil;
using NScript.Utils;

namespace NScript.CLR.AST
{
    public abstract class Pattern: Node
    {
        protected Pattern(ClrContext ctx, Location location)
            : base(ctx, location)
        {
        }
    }

    public class DeclarationPattern : Pattern
    {
        public DeclarationPattern(
            ClrContext ctx,
            Location location,
            VariableReference? localVariableOpt,
            TypeReference ty,
            Expression? whenExpressionOpt)
            : base(ctx, location)
        {
            VariableOpt = localVariableOpt;
            TypeReference = ty;
            WhenExpressionOpt = whenExpressionOpt;
        }

        public TypeReference TypeReference { get; }

        public VariableReference? VariableOpt { get; }

        public Expression? WhenExpressionOpt { get;  }

    }

    public class ConstantPattern : Pattern
    {
        public ConstantPattern(ClrContext ctx, Location location, Expression constantExpression)
            : base(ctx, location)
        {
            ConstantExpression = constantExpression;
        }

        public Expression ConstantExpression { get;  }
    }

    public class DiscardPattern : Pattern
    {
        public DiscardPattern(ClrContext ctx, Location location)
            : base(ctx, location)
        { }
    }
}
