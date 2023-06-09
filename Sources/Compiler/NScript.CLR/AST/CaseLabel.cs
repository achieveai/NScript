using Mono.Cecil;
using NScript.Utils;

namespace NScript.CLR.AST
{
    public abstract class CaseLabel: Node
    {
        protected CaseLabel(ClrContext ctx, Location location)
            : base(ctx, location)
        {
        }
    }

    public class DeclarationCaseLabel : CaseLabel
    {
        public DeclarationCaseLabel(
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

    public class ConstCaseLabel : CaseLabel
    {
        public ConstCaseLabel(ClrContext ctx, Location location, Expression constantExpression)
            : base(ctx, location)
        {
            ConstantExpression = constantExpression;
        }

        public Expression ConstantExpression { get;  }
    }

    public class DiscardCaseLabel : CaseLabel
    {
        public DiscardCaseLabel(ClrContext ctx, Location location)
            : base(ctx, location)
        { }
    }
}
