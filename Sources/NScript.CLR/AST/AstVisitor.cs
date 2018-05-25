namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class AstVisitor<A,R>
    {
        public R Visit(Node node, A arg)
        {
            switch(node)
            {
                case AnonymousMethodBodyExpression n:
                    return this.VisitExpression(n, arg);
                case AnonymousNewExpression n:
                    return this.VisitExpression(n, arg);
                case ArrayElementExpression n:
                    return this.VisitExpression(n, arg);
                case BaseVariableReference n:
                    return this.VisitExpression(n, arg);
                case BinaryExpression n:
                    return this.VisitExpression(n, arg);
                case BooleanLiteral n:
                    return this.VisitExpression(n, arg);
                case BoxExpression n:
                    return this.VisitExpression(n, arg);
                case CharLiteral n:
                    return this.VisitExpression(n, arg);
                case ConditionalOperatorExpression n:
                    return this.VisitExpression(n, arg);
                case ConstructorReferenceExpression n:
                    return this.VisitExpression(n, arg);
                case ConvertTypeExpression n:
                    return this.VisitExpression(n, arg);
                case DefaultValueExpression n:
                    return this.VisitExpression(n, arg);
                case DoubleLiteral n:
                    return this.VisitExpression(n, arg);
                case DynamicIndexAccessor n:
                    return this.VisitExpression(n, arg);
                case DynamicMemberAccessor n:
                    return this.VisitExpression(n, arg);
                case FieldReferenceExpression n:
                    return this.VisitExpression(n, arg);
                case InitObjectWithDefaultValue n:
                    return this.VisitExpression(n, arg);
                case InlineIteratorExpression n:
                    return this.VisitExpression(n, arg);
                case InlinePropertyInitilizationExpression n:
                    return this.VisitExpression(n, arg);
                case IntLiteral n:
                    return this.VisitExpression(n, arg);
                case VirtualMethodReferenceExpression n:
                    return this.VisitExpression(n, arg);
                case MethodReferenceExpression n:
                    return this.VisitExpression(n, arg);
                case NewArrayExpression n:
                    return this.VisitExpression(n, arg);
                case NewObjectExpression n:
                    return this.VisitExpression(n, arg);
                case NullConditional n:
                    return this.VisitExpression(n, arg);
                case NullLiteral n:
                    return this.VisitExpression(n, arg);
                case PropertyReferenceExpression n:
                    return this.VisitExpression(n, arg);
                case StringLiteral n:
                    return this.VisitExpression(n, arg);
                case ToNullable n:
                    return this.VisitExpression(n, arg);
                case TypeCheckExpression n:
                    return this.VisitExpression(n, arg);
                case TypeofExpression n:
                    return this.VisitExpression(n, arg);
                case TypeReferenceExpression n:
                    return this.VisitExpression(n, arg);
                case UIntLiteral n:
                    return this.VisitExpression(n, arg);
                case UnaryExpression n:
                    return this.VisitExpression(n, arg);
                case UnboxExpression n:
                    return this.VisitExpression(n, arg);
                case VariableAddressReference n:
                    return this.VisitExpression(n, arg);
                case VariableReference n:
                    return this.VisitExpression(n, arg);
                case LoadAddressExpression n:
                    return this.VisitExpression(n, arg);
                case MethodCallExpression n:
                    return this.VisitExpression(n, arg);
                case MemberReferenceExpression n:
                    return this.VisitExpression(n, arg);
                case BreakStatement n:
                    return this.VisitStatement(n, arg);
                case ContinueStatement n:
                    return this.VisitStatement(n, arg);
                case DebuggerBreakStatement n:
                    return this.VisitStatement(n, arg);
                case DoWhileLoop n:
                    return this.VisitStatement(n, arg);
                case ExpressionStatement n:
                    return this.VisitStatement(n, arg);
                case ForEachLoop n:
                    return this.VisitStatement(n, arg);
                case ForLoop n:
                    return this.VisitStatement(n, arg);
                case HandlerBlock n:
                    return this.VisitStatement(n, arg);
                case IfBlockStatement n:
                    return this.VisitStatement(n, arg);
                case InitializerStatement n:
                    return this.VisitStatement(n, arg);
                case ParameterBlock n:
                    return this.VisitStatement(n, arg);
                case ReturnStatement n:
                    return this.VisitStatement(n, arg);
                case ScopeBlock n:
                    return this.VisitStatement(n, arg);
                case SwitchStatement n:
                    return this.VisitStatement(n, arg);
                case ThrowStatement n:
                    return this.VisitStatement(n, arg);
                case TryCatchFinally n:
                    return this.VisitStatement(n, arg);
                case WhileLoop n:
                    return this.VisitStatement(n, arg);
                case YieldStatement n:
                    return this.VisitStatement(n, arg);
                case ExplicitBlock n:
                    return this.VisitStatement(n, arg);
            }

            throw new NotImplementedException();
        }

        public abstract R VisitExpression(AnonymousMethodBodyExpression node, A arg);
        public abstract R VisitExpression(AnonymousNewExpression node, A arg);
        public abstract R VisitExpression(ArrayElementExpression node, A arg);
        public abstract R VisitExpression(BaseVariableReference node, A arg);
        public abstract R VisitExpression(BinaryExpression node, A arg);
        public abstract R VisitExpression(BooleanLiteral node, A arg);
        public abstract R VisitExpression(BoxExpression node, A arg);
        public abstract R VisitExpression(CharLiteral node, A arg);
        public abstract R VisitExpression(ConditionalOperatorExpression node, A arg);
        public abstract R VisitExpression(ConstructorReferenceExpression node, A arg);
        public abstract R VisitExpression(ConvertTypeExpression node, A arg);
        public abstract R VisitExpression(DefaultValueExpression node, A arg);
        public abstract R VisitExpression(DoubleLiteral node, A arg);
        public abstract R VisitExpression(DynamicIndexAccessor node, A arg);
        public abstract R VisitExpression(DynamicMemberAccessor node, A arg);
        public abstract R VisitExpression(FieldReferenceExpression node, A arg);
        public abstract R VisitExpression(InitObjectWithDefaultValue node, A arg);
        public abstract R VisitExpression(InlineIteratorExpression node, A arg);
        public abstract R VisitExpression(InlinePropertyInitilizationExpression node, A arg);
        public abstract R VisitExpression(IntLiteral node, A arg);
        public abstract R VisitExpression(LoadAddressExpression node, A arg);
        public abstract R VisitExpression(MemberReferenceExpression node, A arg);
        public abstract R VisitExpression(MethodCallExpression node, A arg);
        public abstract R VisitExpression(MethodReferenceExpression node, A arg);
        public abstract R VisitExpression(NewArrayExpression node, A arg);
        public abstract R VisitExpression(NewObjectExpression node, A arg);
        public abstract R VisitExpression(NullConditional node, A arg);
        public abstract R VisitExpression(NullLiteral node, A arg);
        public abstract R VisitExpression(PropertyReferenceExpression node, A arg);
        public abstract R VisitExpression(StringLiteral node, A arg);
        public abstract R VisitExpression(ToNullable node, A arg);
        public abstract R VisitExpression(TypeCheckExpression node, A arg);
        public abstract R VisitExpression(TypeofExpression node, A arg);
        public abstract R VisitExpression(TypeReferenceExpression node, A arg);
        public abstract R VisitExpression(UIntLiteral node, A arg);
        public abstract R VisitExpression(UnaryExpression node, A arg);
        public abstract R VisitExpression(UnboxExpression node, A arg);
        public abstract R VisitExpression(VariableAddressReference node, A arg);
        public abstract R VisitExpression(VariableReference node, A arg);
        public abstract R VisitExpression(VirtualMethodReferenceExpression node, A arg);
        public abstract R VisitStatement(BreakStatement node, A arg);
        public abstract R VisitStatement(ContinueStatement node, A arg);
        public abstract R VisitStatement(DebuggerBreakStatement node, A arg);
        public abstract R VisitStatement(DoWhileLoop node, A arg);
        public abstract R VisitStatement(ExplicitBlock node, A arg);
        public abstract R VisitStatement(ExpressionStatement node, A arg);
        public abstract R VisitStatement(ForEachLoop node, A arg);
        public abstract R VisitStatement(ForLoop node, A arg);
        public abstract R VisitStatement(HandlerBlock node, A arg);
        public abstract R VisitStatement(IfBlockStatement node, A arg);
        public abstract R VisitStatement(InitializerStatement node, A arg);
        public abstract R VisitStatement(ParameterBlock node, A arg);
        public abstract R VisitStatement(ReturnStatement node, A arg);
        public abstract R VisitStatement(ScopeBlock node, A arg);
        public abstract R VisitStatement(SwitchStatement node, A arg);
        public abstract R VisitStatement(ThrowStatement node, A arg);
        public abstract R VisitStatement(TryCatchFinally node, A arg);
        public abstract R VisitStatement(WhileLoop node, A arg);
        public abstract R VisitStatement(YieldStatement node, A arg);
    }
}
