//-----------------------------------------------------------------------
// <copyright file="MonoAstVisitor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using Mono.CSharp;
    using Mono.CSharp.Linq;
    using Mono.CSharp.Nullable;

    /// <summary>
    /// Definition for MonoAstVisitor
    /// </summary>
    public interface IMonoAstVisitor<T> where T : class
    {
        T Visit(Expression expression);

        #region Literals

        T Visit(NullLiteral expression);

        T Visit(BoolLiteral expression);

        T Visit(CharLiteral expression);

        T Visit(IntLiteral expression);

        T Visit(UIntLiteral expression);

        T Visit(LongLiteral expression);

        T Visit(ULongLiteral expression);

        T Visit(FloatLiteral expression);

        T Visit(DoubleLiteral expression);

        T Visit(DecimalLiteral expression);

        T Visit(StringLiteral expression);

        #endregion Literals

        #region Assignments

        T Visit(LocalTemporary expression);

        T Visit(Assign expression);

        T Visit(SimpleAssign expression);

        T Visit(RuntimeExplicitAssign expression);

        T Visit(FieldInitializer expression);

        T Visit(CompoundAssign expression);

        T Visit(CompoundAssign.TargetExpression expression);

        #endregion Assignments

        #region TypeCast

        T Visit(EmptyCast expression);

        T Visit(OperatorCast expression);

        T Visit(BoxedCast expression);

        T Visit(UnboxCast expression);

        T Visit(ClassCast expression);

        T Visit(Cast expression);

        T Visit(ImplicitCast expression);

        #endregion TypeCast

        #region Constants

        T Visit(BoolConstant expression);

        T Visit(ByteConstant expression);

        T Visit(CharConstant expression);

        T Visit(SByteConstant expression);

        T Visit(ShortConstant expression);

        T Visit(UShortConstant expression);

        T Visit(IntConstant expression);

        T Visit(UIntConstant expression);

        T Visit(LongConstant expression);

        T Visit(ULongConstant expression);

        T Visit(FloatConstant expression);

        T Visit(DoubleConstant expression);

        T Visit(DecimalConstant expression);

        T Visit(StringConcat expression);

        T Visit(NullConstant expression);

        T Visit(EmptyConstantCast expression);

        T Visit(EnumConstant expression);

        T Visit(SideEffectConstant expression);

        #endregion Constants

        #region Linq

        T Visit(QueryExpression expression);

        T Visit(QueryStartClause expression);

        T Visit(GroupBy expression);

        T Visit(Join expression);

        T Visit(GroupJoin expression);

        T Visit(Let expresstion);

        T Visit(Select expression);

        T Visit(SelectMany expression);

        T Visit(Where expression);

        T Visit(OrderByAscending expression);

        T Visit(OrderByDescending expression);

        T Visit(ThenByAscending expression);

        T Visit(ThenByDescending expression);

        T Visit(QueryBlock expression);

        #endregion Linq

        #region Statements

        T Visit(EmptyStatement expression);

        T Visit(StatementExpression expression);

        T Visit(StatementErrorExpression expression);

        T Visit(StatementList expression);

        T Visit(Return expression);

        T Visit(Goto expression);

        T Visit(GotoDefault expression);

        T Visit(GotoCase expression);

        T Visit(LabeledStatement expression);

        T Visit(Throw expression);

        T Visit(Break expression);

        T Visit(Continue expression);

        T Visit(BlockVariableDeclaration expression);

        T Visit(BlockConstantDeclaration expression);

        T Visit(LocalVariable exprssion);

        #region Logicals

        T Visit(If expression);

        T Visit(Do expression);

        T Visit(While expression);

        T Visit(For expression);

        T Visit(Foreach expression);

        T Visit(Foreach.ArrayForeach expression);

        T Visit(Foreach.CollectionForeach expression);

        T Visit(Switch expression);

        #endregion Logicals

        #region Blocks

        T Visit(Block expression);

        T Visit(ExplicitBlock expression);

        T Visit(ParametersBlock expression);

        T Visit(ToplevelBlock expression);

        T Visit(Lock expression);

        T Visit(Unchecked expression);

        T Visit(Checked expression);

        T Visit(Unsafe expression);

        T Visit(Fixed expression);

        T Visit(Catch expression);

        T Visit(TryFinally expression);

        T Visit(TryCatch expression);

        T Visit(Using expression);

        T Visit(Using.VariableDeclaration expression);

        #endregion Blocks

        #endregion Statements

        #region nullable

        T Visit(NullableType expression);

        T Visit(Unwrap expression);

        T Visit(UnwrapCall expression);

        T Visit(Wrap expression);

        T Visit(LiftedNull expression);

        T Visit(Lifted expression);

        T Visit(LiftedUnaryOperator expression);

        T Visit(LiftedBinaryOperator expression);

        T Visit(NullCoalescingOperator expression);

        #endregion nullable

        #region Iterators

        T Visit(Yield expression);

        T Visit(YieldBreak expression);

        T Visit(StateMachine expression);

        T Visit(StateMachineInitializer expression);

        T Visit(Iterator expression);

        #endregion Iterators

        #region Anonymous

        T Visit(AnonymousExpression expression);

        T Visit(AnonymousMethodBody expression);

        #endregion Anonymous

        T Visit(UserOperatorCall expression);

        T Visit(ParenthesizedExpression expression);

        T Visit(Unary expression);

        T Visit(Indirection expression);

        T Visit(UnaryMutator expression);

        T Visit(Binary expression);

        T Visit(Is expression);

        T Visit(As expression);

        T Visit(ConditionalLogicalOperator expression);

        T Visit(BooleanExpression expression);

        T Visit(BooleanExpressionFalse expression);

        T Visit(Conditional expression);

        T Visit(LocalVariableReference expression);

        T Visit(ParameterReference expression);

        T Visit(Invocation expression);

        T Visit(New expression);

        T Visit(ArrayInitializer expression);

        T Visit(ArrayCreation expression);

        T Visit(This expression);

        T Visit(ArglistAccess expression);

        T Visit(RefValueExpr expression);

        T Visit(RefTypeExpr expression);

        T Visit(MakeRefExpr expression);

        T Visit(TypeOf expression);

        T Visit(SizeOf expression);

        T Visit(MemberAccess expression);

        T Visit(CheckedExpr expression);

        T Visit(UnCheckedExpr experssion);

        T Visit(ElementAccess expression);

        T Visit(ArrayAccess expression);

        T Visit(BaseThis expression);

        T Visit(EmptyExpression expression);

        T Visit(ErrorExpression expression);

        T Visit(ComposedTypeSpecifier expression);

        T Visit(ComposedCast expression);

        T Visit(ArrayIndexCast expression);

        T Visit(StackAlloc expression);

        T Visit(ElementInitializer expression);

        T Visit(CollectionOrObjectInitializers expression);

        T Visit(NewInitialize expression);

        T Visit(NewAnonymousType expression);

        T Visit(AnonymousTypeParameter expression);

        T Visit(DynamicResultCast expression);

        T Visit(CompositeExpression expression);

        T Visit(ConstantExpr expression);

        T Visit(MemberExpr expression);

        T Visit(MethodGroupExpr expression);

        T Visit(FieldExpr expression);

        T Visit(PropertyExpr expression);

        T Visit(IndexerExpr expression);

        T Visit(EventExpr expression);

        T Visit(ExpressionStatement expression);

        T Visit(AsyncInitializer expression);

        T Visit(ConstructorInitializer expression);

        T Visit(ConstructorThisInitializer expression);

        T Visit(EmptyExpressionStatement expression);

        T Visit(CompletingExpression expression);

        T Visit(CompletionSimpleName expression);

        T Visit(CompletionElementInitializer expression);

        T Visit(Await expression);

        T Visit(DefaultParameterValueExpression expression);

        T Visit(Constant expression);

        T Visit(StringConstant expression);

        T Visit(ShimExpression expression);

        T Visit(AQueryClause expression);

        T Visit(ARangeVariableQueryClause expression);

        T Visit(DefaultValueExpression expression);

        T Visit(PointerArithmetic expression);

        T Visit(VariableReference expression);

        T Visit(TemporaryVariableReference expression);

        T Visit(Arglist expression);

        T Visit(ATypeNameExpression expression);

        T Visit(QualifiedAliasMember expression);

        T Visit(TypeExpression expression);

        T Visit(TypeParameterExpr expression);

        T Visit(SpecialContraintExpr expression);

        T Visit(Namespace expression);

        T Visit(RootNamespace expression);

        T Visit(GlobalRootNamespace expression);

        T Visit(UserCast expression);

        T Visit(TypeCast expression);

        T Visit(ConvCast expression);

        T Visit(ReducedExpression expression);

        T Visit(AnonymousMethodExpression expression);

        T Visit(LambdaExpression expression);

        T Visit(RuntimeValueExpression expression);

        T Visit(DelegateCreation expression);

        T Visit(ImplicitDelegateCreation expression);

        T Visit(NewDelegate expression);

        T Visit(Statement statement);

        T Visit(ResumableStatement statement);

        T Visit(YieldStatement<AsyncInitializer> statement);

        T Visit(ExceptionStatement statement);

        T Visit(TryFinallyBlock statement);

        T Visit(ExitStatement statement);

        T Visit(ContextualReturn statement);

        T Visit(DelegateInvocation statement);
    }
}