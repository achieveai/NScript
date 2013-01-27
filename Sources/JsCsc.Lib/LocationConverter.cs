//-----------------------------------------------------------------------
// <copyright file="LocationConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using Mono.CSharp;
    using Mono.CSharp.Linq;
    using Mono.CSharp.Nullable;
    using Location = NScript.Utils.Location;
    using MonoLocation = Mono.CSharp.Location;

    /// <summary>
    /// Definition for LocationConverter
    /// </summary>
    public class LocationConverter : IMonoAstVisitor<Location>
    {
        private class ExpressionOrStatement
        {
            public readonly Expression Expression;
            public readonly Statement Statement;

            public ExpressionOrStatement(Statement statement)
            {
                this.Statement = statement;
            }

            public ExpressionOrStatement(Expression expression)
            {
                this.Expression = expression;
            }

            public MonoLocation Location
            {
                get { return this.Statement != null ? this.Statement.Location : this.Expression.Location; }
            }

            public override bool Equals(object obj)
            {
                ExpressionOrStatement other = obj as ExpressionOrStatement;
                return other != null
                    && object.Equals(this.Statement, other.Statement)
                    && object.Equals(this.Expression, other.Expression);
            }

            public override int GetHashCode()
            {
                return this.Statement != null
                    ? this.Statement.GetHashCode()
                    : this.Expression.GetHashCode();
            }
        }

        private class MonoLocationComparer : IComparer<MonoLocation>
        {
            public static int Compare(MonoLocation x, MonoLocation y)
            {
                if (x.Row == y.Row)
                {
                    return x.Column.CompareTo(y.Column);
                }
                else
                {
                    return x.Row.CompareTo(y.Row);
                }
            }

            #region IComparer<MonoLocation> Members

            int IComparer<MonoLocation>.Compare(MonoLocation x, MonoLocation y)
            {
                return MonoLocationComparer.Compare(x, y);
            }

            #endregion IComparer<MonoLocation> Members
        }

        private Stack<Tuple<ExpressionOrStatement, List<ExpressionOrStatement>>> treeTraversalStack
            = new Stack<Tuple<ExpressionOrStatement, List<ExpressionOrStatement>>>();

        private Dictionary<ExpressionOrStatement, List<ExpressionOrStatement>> treeLinks =
            new Dictionary<ExpressionOrStatement, List<ExpressionOrStatement>>();

        private Dictionary<ExpressionOrStatement, ExpressionOrStatement> reverseTreeLinks =
            new Dictionary<ExpressionOrStatement, ExpressionOrStatement>();

        ExpressionOrStatement root;

        ExpressionVisitDispatcher<Location> dispatcher;

        Stack<string> prefixString = new Stack<string>();
        SortedDictionary<MonoLocation, List<ExpressionOrStatement>> locationToStatementMap =
            new SortedDictionary<MonoLocation, List<ExpressionOrStatement>>(new MonoLocationComparer());

        public LocationConverter()
        {
            dispatcher = new ExpressionVisitDispatcher<Location>(this);
            prefixString.Push(string.Empty);
        }

        public ExpressionVisitDispatcher<Location> Dispatcher
        { get { return this.dispatcher; } }

        public void ComputeLocations()
        {
        }

        public Location GetLocation(Expression expression)
        {
            return null;
        }

        public Location GetLocation(Statement statement)
        {
            return null;
        }

        public Location Visit(Expression expression)
        { throw new NotImplementedException(); }

        public Location Visit(NullLiteral expression)
        { return null; }

        public Location Visit(BoolLiteral expression)
        { return null; }

        public Location Visit(CharLiteral expression)
        { return null; }

        public Location Visit(IntLiteral expression)
        { return null; }

        public Location Visit(UIntLiteral expression)
        { return null; }

        public Location Visit(LongLiteral expression)
        { return null; }

        public Location Visit(ULongLiteral expression)
        { return null; }

        public Location Visit(FloatLiteral expression)
        { return null; }

        public Location Visit(DoubleLiteral expression)
        { return null; }

        public Location Visit(DecimalLiteral expression)
        { return null; }

        public Location Visit(StringLiteral expression)
        { return null; }

        public Location Visit(LocalTemporary expression)
        { throw new NotImplementedException(); }

        public Location Visit(Assign expression)
        {
            this.Dispatch(expression.Target);
            this.Dispatch(expression.Source);

            return null;
        }

        public Location Visit(SimpleAssign expression)
        {
            this.Dispatch(expression.Target);
            this.Dispatch(expression.Source);

            return null;
        }

        public Location Visit(RuntimeExplicitAssign expression)
        { throw new NotImplementedException(); }

        public Location Visit(FieldInitializer expression)
        { throw new NotImplementedException(); }

        public Location Visit(CompoundAssign expression)
        {
            this.Dispatch(expression.Target);
            this.Dispatch(expression.Source);

            return null;
        }

        public Location Visit(CompoundAssign.TargetExpression expression)
        { return this.Dispatch(expression.child); }

        public Location Visit(EmptyCast expression)
        { return this.Dispatch(expression.Child); }

        public Location Visit(OperatorCast expression)
        { throw new NotImplementedException(); }

        public Location Visit(BoxedCast expression)
        { return this.Dispatch(expression.Child); }

        public Location Visit(UnboxCast expression)
        { throw new NotImplementedException(); }

        public Location Visit(ClassCast expression)
        { return this.Dispatch(expression.Child); }

        public Location Visit(Cast expression)
        {
            this.Dispatch(expression.TargetType);
            this.Dispatch(expression.Expr);

            return null;
        }

        public Location Visit(ImplicitCast expression)
        { throw new NotImplementedException(); }

        public Location Visit(BoolConstant expression)
        { return null; }

        public Location Visit(ByteConstant expression)
        { return null; }

        public Location Visit(CharConstant expression)
        { return null; }

        public Location Visit(SByteConstant expression)
        { return null; }

        public Location Visit(ShortConstant expression)
        { return null; }

        public Location Visit(UShortConstant expression)
        { return null; }

        public Location Visit(IntConstant expression)
        { return null; }

        public Location Visit(UIntConstant expression)
        { return null; }

        public Location Visit(LongConstant expression)
        { return null; }

        public Location Visit(ULongConstant expression)
        { return null; }

        public Location Visit(FloatConstant expression)
        { return null; }

        public Location Visit(DoubleConstant expression)
        { return null; }

        public Location Visit(DecimalConstant expression)
        { return null; }

        public Location Visit(StringConcat expression)
        {
            foreach (var arg in expression.Arguments)
            {
                this.Dispatch(arg.Expr);
            }

            return null;
        }

        public Location Visit(NullConstant expression)
        { return null; }

        public Location Visit(EmptyConstantCast expression)
        { return this.Dispatch(expression.child); }

        public Location Visit(EnumConstant expression)
        { throw new NotImplementedException(); }

        public Location Visit(SideEffectConstant expression)
        { throw new NotImplementedException(); }

        public Location Visit(QueryExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(QueryStartClause expression)
        { throw new NotImplementedException(); }

        public Location Visit(GroupBy expression)
        { throw new NotImplementedException(); }

        public Location Visit(Join expression)
        { throw new NotImplementedException(); }

        public Location Visit(GroupJoin expression)
        { throw new NotImplementedException(); }

        public Location Visit(Let expresstion)
        { throw new NotImplementedException(); }

        public Location Visit(Select expression)
        { throw new NotImplementedException(); }

        public Location Visit(SelectMany expression)
        { throw new NotImplementedException(); }

        public Location Visit(Where expression)
        { throw new NotImplementedException(); }

        public Location Visit(OrderByAscending expression)
        { throw new NotImplementedException(); }

        public Location Visit(OrderByDescending expression)
        { throw new NotImplementedException(); }

        public Location Visit(ThenByAscending expression)
        { throw new NotImplementedException(); }

        public Location Visit(ThenByDescending expression)
        { throw new NotImplementedException(); }

        public Location Visit(QueryBlock expression)
        { throw new NotImplementedException(); }

        public Location Visit(EmptyStatement expression)
        { return null; }

        public Location Visit(StatementExpression expression)
        { return this.Dispatch(expression.Expr); }

        public Location Visit(StatementErrorExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(StatementList expression)
        {
            foreach (var statement in expression.Statements)
            {
                this.Dispatch(statement);
            }

            return null;
        }

        public Location Visit(Return expression)
        {
            if (expression.Expr != null)
            {
                this.Dispatcher.Dispatch(expression.Expr);
            }

            return null;
        }

        public Location Visit(Goto expression)
        { throw new NotImplementedException(); }

        public Location Visit(GotoDefault expression)
        { throw new NotImplementedException(); }

        public Location Visit(GotoCase expression)
        { throw new NotImplementedException(); }

        public Location Visit(LabeledStatement expression)
        { throw new NotImplementedException(); }

        public Location Visit(Throw expression)
        { return this.Dispatch(expression.Expr); }

        public Location Visit(Break expression)
        { return null; }

        public Location Visit(Continue expression)
        { return null; }

        public Location Visit(BlockVariableDeclaration expression)
        {
            if (expression.Initializer != null)
            {
                this.Dispatch(expression.Initializer);
            }

            return null;
        }

        public Location Visit(BlockConstantDeclaration expression)
        { throw new NotImplementedException(); }

        public Location Visit(LocalVariable exprssion)
        { throw new NotImplementedException(); }

        public Location Visit(If expression)
        {
            this.Dispatch(expression.Expr);
            this.Dispatch(expression.TrueStatement);
            if (expression.FalseStatement != null)
            {
                this.Dispatch(expression.FalseStatement);
            }

            return null;
        }

        public Location Visit(Do expression)
        {
            this.Dispatch(expression.EmbeddedStatement);
            this.Dispatch(expression.expr);
            return null;
        }

        public Location Visit(While expression)
        {
            this.Dispatch(expression.expr);
            this.Dispatch(expression.Statement);
            return null;
        }

        public Location Visit(For expression)
        {
            this.Dispatch(expression.Initializer);
            this.Dispatch(expression.Condition);
            this.Dispatch(expression.Iterator);
            this.Dispatch(expression.Statement);

            return null;
        }

        public Location Visit(Foreach expression)
        {
            this.Dispatch(expression.Expr);
            this.Dispatch(expression.Statement);

            return null;
        }

        public Location Visit(Foreach.ArrayForeach expression)
        { throw new NotImplementedException(); }

        public Location Visit(Foreach.CollectionForeach expression)
        {
            this.Dispatch(expression.Expr);
            this.Dispatch(expression.EnumeratorVariable);
            this.Dispatch(expression.Statement);

            return null;
        }

        public Location Visit(Switch expression)
        {
            this.Dispatch(expression.Expr);
            foreach (var section in expression.Sections)
            {
                foreach (var label in section.Labels)
                {
                    this.Dispatch(label.Label);
                }
                this.Dispatch(section.Block);
            }

            return null;
        }

        public Location Visit(Block expression)
        {
            foreach (var statement in expression.Statements)
            {
                this.Dispatch(statement);
            }

            return null;
        }

        public Location Visit(ExplicitBlock expression)
        {
            foreach (var statement in expression.Statements)
            {
                this.Dispatch(statement);
            }

            return null;
        }

        public Location Visit(ParametersBlock expression)
        {
            foreach (var statement in expression.Statements)
            {
                this.Dispatch(statement);
            }
            return null;
        }

        public Location Visit(ToplevelBlock expression)
        {
            foreach (var statement in expression.Statements)
            {
                this.Dispatch(statement);
            }

            return null;
        }

        public Location Visit(Lock expression)
        { throw new NotImplementedException(); }

        public Location Visit(Unchecked expression)
        { throw new NotImplementedException(); }

        public Location Visit(Checked expression)
        { throw new NotImplementedException(); }

        public Location Visit(Unsafe expression)
        { throw new NotImplementedException(); }

        public Location Visit(Fixed expression)
        { throw new NotImplementedException(); }

        public Location Visit(Catch expression)
        { throw new NotImplementedException(); }

        public Location Visit(TryFinally expression)
        {
            this.Dispatch(expression.Statement);
            this.Dispatch(expression.Finallyblock);

            return null;
        }

        public Location Visit(TryCatch expression)
        {
            this.Dispatch(expression.Block);
            foreach (var clause in expression.Clauses)
            {
                this.Dispatch(clause.Block);
            }
            return null;
        }

        public Location Visit(Using expression)
        {
            this.Dispatch(expression.Variables);
            this.Dispatch(expression.Statement);

            return null;
        }

        public Location Visit(Using.VariableDeclaration expression)
        {
            this.Dispatch(expression.Initializer);
            this.Dispatch(expression.DisposeCall);

            return null;
        }

        public Location Visit(NullableType expression)
        { throw new NotImplementedException(); }

        public Location Visit(Unwrap expression)
        { throw new NotImplementedException(); }

        public Location Visit(UnwrapCall expression)
        { throw new NotImplementedException(); }

        public Location Visit(Wrap expression)
        { throw new NotImplementedException(); }

        public Location Visit(LiftedNull expression)
        { throw new NotImplementedException(); }

        public Location Visit(Lifted expression)
        { throw new NotImplementedException(); }

        public Location Visit(LiftedUnaryOperator expression)
        { throw new NotImplementedException(); }

        public Location Visit(LiftedBinaryOperator expression)
        { throw new NotImplementedException(); }

        public Location Visit(NullCoalescingOperator expression)
        {
            this.Dispatch(expression.LeftExpression);
            this.Dispatch(expression.RightExpression);

            return null;
        }

        public Location Visit(Yield expression)
        { return this.Dispatch(expression.Expr); }

        public Location Visit(YieldBreak expression)
        { throw new NotImplementedException(); }

        public Location Visit(StateMachine expression)
        { throw new NotImplementedException(); }

        public Location Visit(StateMachineInitializer expression)
        { throw new NotImplementedException(); }

        public Location Visit(Iterator expression)
        { return this.Dispatch(expression.Block); }

        public Location Visit(AnonymousExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(AnonymousMethodBody expression)
        { return this.Dispatch(expression.Block); }

        public Location Visit(UserOperatorCall expression)
        {
            foreach (var arg in expression.arguments)
            {
                this.Dispatch(arg.Expr);
            }

            return null;
        }

        public Location Visit(ParenthesizedExpression expression)
        {
            return this.Dispatch(expression.Expr);
        }

        public Location Visit(Unary expression)
        { return this.Dispatch(expression.Expr); }

        public Location Visit(Indirection expression)
        { throw new NotImplementedException(); }

        public Location Visit(UnaryMutator expression)
        { return this.Dispatch(expression.Expr); }

        public Location Visit(Binary expression)
        {
            this.Dispatch(expression.Left);
            this.Dispatch(expression.Right);

            return null;
        }

        public Location Visit(Is expression)
        {
            this.Dispatch(expression.Expr);
            this.Dispatch(expression.ProbeType);

            return null;
        }

        public Location Visit(As expression)
        {
            this.Dispatch(expression.Expr);
            this.Dispatch(expression.ProbeType);

            return null;
        }

        public Location Visit(ConditionalLogicalOperator expression)
        { throw new NotImplementedException(); }

        public Location Visit(BooleanExpression expression)
        { return this.Dispatch(expression.Expr); }

        public Location Visit(BooleanExpressionFalse expression)
        { throw new NotImplementedException(); }

        public Location Visit(Conditional expression)
        {
            this.Dispatch(expression.Expr);
            this.Dispatch(expression.TrueExpr);
            this.Dispatch(expression.FalseExpr);

            return null;
        }

        public Location Visit(LocalVariableReference expression)
        {
            return null;
        }

        public Location Visit(ParameterReference expression)
        {
            return null;
        }

        public Location Visit(Invocation expression)
        {
            if (expression.Arguments != null)
            {
                foreach (var arg in expression.Arguments)
                {
                    this.Dispatch(arg.Expr);
                }
            }

            this.Dispatch(expression.Exp);
            return null;
        }

        public Location Visit(New expression)
        {
            if (expression.Arguments != null)
            {
                foreach (var arg in expression.Arguments)
                {
                    this.Dispatch(arg.Expr);
                }
            }

            return null;
        }

        public Location Visit(ArrayInitializer expression)
        { throw new NotImplementedException(); }

        public Location Visit(ArrayCreation expression)
        {
            this.Dispatch(expression.TypeExpression);
            if (expression.Initializers != null && expression.Initializers.Count > 0)
            {
                foreach (var elem in expression.Initializers.Elements)
                {
                    this.Dispatch(elem);
                }
            }

            return null;
        }

        public Location Visit(This expression)
        {
            return null;
        }

        public Location Visit(ArglistAccess expression)
        { throw new NotImplementedException(); }

        public Location Visit(RefValueExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(RefTypeExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(MakeRefExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(TypeOf expression)
        {
            this.Dispatch(expression.TypeExpression);

            return null;
        }

        public Location Visit(SizeOf expression)
        { throw new NotImplementedException(); }

        public Location Visit(CheckedExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(UnCheckedExpr experssion)
        { throw new NotImplementedException(); }

        public Location Visit(ElementAccess expression)
        {
            this.Dispatch(expression.Expr);
            this.Dispatch(expression.Arguments[0].Expr);

            return null;
        }

        public Location Visit(ArrayAccess expression)
        {
            return this.Visit(expression.ElementAccess);
        }

        public Location Visit(BaseThis expression)
        { throw new NotImplementedException(); }

        public Location Visit(EmptyExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(ErrorExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(ComposedTypeSpecifier expression)
        { throw new NotImplementedException(); }

        public Location Visit(ComposedCast expression)
        { throw new NotImplementedException(); }

        public Location Visit(ArrayIndexCast expression)
        { throw new NotImplementedException(); }

        public Location Visit(StackAlloc expression)
        { throw new NotImplementedException(); }

        public Location Visit(ElementInitializer expression)
        { return this.Dispatch(expression.Source); }

        public Location Visit(CollectionOrObjectInitializers expression)
        { throw new NotImplementedException(); }

        public Location Visit(NewInitialize expression)
        {
            if (expression.Arguments != null)
            {
                foreach (var arg in expression.Arguments)
                {
                    this.Dispatch(arg.Expr);
                }
            }
            if (expression.Initializers != null)
            {
                foreach (var init in expression.Initializers.Initializers)
                {
                    this.Dispatch(init);
                }
            }

            return null;
        }

        public Location Visit(NewAnonymousType expression)
        { throw new NotImplementedException(); }

        public Location Visit(AnonymousTypeParameter expression)
        { throw new NotImplementedException(); }

        public Location Visit(DynamicResultCast expression)
        { throw new NotImplementedException(); }

        public Location Visit(CompositeExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(ConstantExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(MemberExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(MethodGroupExpr expression)
        {
            return null;
        }

        public Location Visit(FieldExpr expression)
        {
            if (expression.IsInstance)
            {
                this.Dispatch(expression.InstanceExpression);
            }

            return null;
        }

        public Location Visit(PropertyExpr expression)
        {
            if (expression.IsInstance)
            {
                this.Dispatch(expression.InstanceExpression);
            }

            return null;
        }

        public Location Visit(IndexerExpr expression)
        {
            if (expression.IsInstance)
            {
                this.Dispatch(expression.InstanceExpression);
            }

            foreach (var arg in expression.Arguments)
            {
                this.Dispatch(arg.Expr);
            }

            return null;
        }

        public Location Visit(EventExpr expression)
        {
            if (expression.IsInstance)
            {
                this.Dispatch(expression.InstanceExpression);
            }

            return null;
        }

        public Location Visit(ExpressionStatement expression)
        { throw new NotImplementedException(); }

        public Location Visit(DelegateInvocation expression)
        {
            if (expression.Arguments != null)
            {
                foreach (var arg in expression.Arguments)
                {
                    this.Dispatch(arg.Expr);
                }
            }

            if (expression.InstanceExpr != null)
            {
                this.Dispatch(expression.InstanceExpr);
            }

            return null;
        }

        public Location Visit(AsyncInitializer expression)
        { throw new NotImplementedException(); }

        public Location Visit(ConstructorInitializer expression)
        { throw new NotImplementedException(); }

        public Location Visit(ConstructorThisInitializer expression)
        { throw new NotImplementedException(); }

        public Location Visit(EmptyExpressionStatement expression)
        { throw new NotImplementedException(); }

        public Location Visit(CompletingExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(CompletionSimpleName expression)
        { throw new NotImplementedException(); }

        public Location Visit(CompletionElementInitializer expression)
        { throw new NotImplementedException(); }

        public Location Visit(Await expression)
        { throw new NotImplementedException(); }

        public Location Visit(DefaultParameterValueExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(Constant expression)
        { throw new NotImplementedException(); }

        public Location Visit(StringConstant expression)
        {
            return null;
        }

        public Location Visit(ShimExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(AQueryClause expression)
        { throw new NotImplementedException(); }

        public Location Visit(ARangeVariableQueryClause expression)
        { throw new NotImplementedException(); }

        public Location Visit(DefaultValueExpression expression)
        {
            this.Dispatch(expression.Expr);

            return null;
        }

        public Location Visit(PointerArithmetic expression)
        { throw new NotImplementedException(); }

        public Location Visit(VariableReference expression)
        { throw new NotImplementedException(); }

        public Location Visit(TemporaryVariableReference expression)
        {
            return null;
        }

        public Location Visit(Arglist expression)
        { throw new NotImplementedException(); }

        public Location Visit(ATypeNameExpression expression)
        {
            return null;
        }

        public Location Visit(QualifiedAliasMember expression)
        { throw new NotImplementedException(); }

        public Location Visit(TypeExpression expression)
        {
            return null;
        }

        public Location Visit(TypeParameterExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(SpecialContraintExpr expression)
        { throw new NotImplementedException(); }

        public Location Visit(Namespace expression)
        { throw new NotImplementedException(); }

        public Location Visit(RootNamespace expression)
        { throw new NotImplementedException(); }

        public Location Visit(GlobalRootNamespace expression)
        { throw new NotImplementedException(); }

        public Location Visit(UserCast expression)
        { throw new NotImplementedException(); }

        public Location Visit(TypeCast expression)
        {
            this.Dispatch(expression.Child);

            return null;
        }

        public Location Visit(ConvCast expression)
        {
            this.Dispatch(expression.Child);

            return null;
        }

        public Location Visit(ReducedExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(AnonymousMethodExpression expression)
        { return this.Dispatch(expression.Block); }

        public Location Visit(LambdaExpression expression)
        {
            return this.Dispatch(expression.Block);
        }

        public Location Visit(RuntimeValueExpression expression)
        { throw new NotImplementedException(); }

        public Location Visit(DelegateCreation expression)
        {
            if (expression.MethodGroup.IsInstance)
            {
                this.Dispatch(expression.MethodGroup.InstanceExpression);
            }

            return null;
        }

        public Location Visit(ImplicitDelegateCreation expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public Location Visit(NewDelegate expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public Location Visit(Statement statement)
        { throw new NotImplementedException(); }

        public Location Visit(ResumableStatement statement)
        { throw new NotImplementedException(); }

        public Location Visit(YieldStatement<AsyncInitializer> statement)
        { throw new NotImplementedException(); }

        public Location Visit(ExceptionStatement statement)
        { throw new NotImplementedException(); }

        public Location Visit(TryFinallyBlock statement)
        { throw new NotImplementedException(); }

        public Location Visit(ExitStatement statement)
        { throw new NotImplementedException(); }

        public Location Visit(ContextualReturn statement)
        { throw new NotImplementedException(); }

        private Location Dispatch(Statement statement)
        {
            this.PushNode(statement);
            var rv = this.dispatcher.Dispatch(statement);
            this.PopNode();

            return rv;
        }

        private Location Dispatch(Expression expression)
        {
            this.PushNode(expression);
            var rv = this.dispatcher.Dispatch(expression);
            this.PopNode();

            return rv;
        }

        private void PushNode(Statement statement)
        {
            this.treeTraversalStack.Push(
                Tuple.Create(
                    new ExpressionOrStatement(statement),
                    new List<ExpressionOrStatement>()));
        }

        private void PushNode(Expression expr)
        {
            this.treeTraversalStack.Push(
                Tuple.Create(
                    new ExpressionOrStatement(expr),
                    new List<ExpressionOrStatement>()));
        }

        private void PopNode()
        {
            var top = this.treeTraversalStack.Pop();
            top.Item2.Sort((l, r) => MonoLocationComparer.Compare(l.Location, r.Location));

            if (this.treeTraversalStack.Count > 0)
            {
                this.treeTraversalStack.Peek().Item2.Add(top.Item1);
                this.reverseTreeLinks[top.Item1] = this.treeTraversalStack.Peek().Item1;
            }
            else
            {
                this.root = top.Item1;
            }
        }

        #region IMonoAstVisitor<Location> Members

        public Location Visit(MemberAccess expression)
        {
            throw new NotImplementedException();
        }

        #endregion IMonoAstVisitor<Location> Members
    }
}