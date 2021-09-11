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
        public TypeReference TypeReference { get; }
        public VariableReference? VariableOpt { get; }
        public Expression? WhenExpressionOpt { get;  }
        public DeclarationCaseLabel(ClrContext ctx, Location location, VariableReference? localVariableOpt, TypeReference ty, Expression? whenExpressionOpt)
            : base(ctx, location)
        {
            this.VariableOpt = localVariableOpt;
            this.TypeReference = ty;
            this.WhenExpressionOpt = whenExpressionOpt;
        }
    }

    public class ConstCaseLabel : CaseLabel
    {
        public Expression ConstantExpression { get;  }
        public ConstCaseLabel(ClrContext ctx, Location location, Expression constantExpression)
            : base(ctx, location)
        {
            this.ConstantExpression = constantExpression;
        }
    }
}
