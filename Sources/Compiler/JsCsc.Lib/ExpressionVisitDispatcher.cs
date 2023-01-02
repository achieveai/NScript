//-----------------------------------------------------------------------
// <copyright file="ExpressionVisitDispatcher.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

/*
namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using Mono.CSharp;
    using Mono.CSharp.Linq;
    using Mono.CSharp.Nullable;

    /// <summary>
    /// Definition for ExpressionVisitDispatcher
    /// </summary>
    public class ExpressionVisitDispatcher<T> where T : class
    {
        private static Dictionary<Type, Func<IMonoAstVisitor<T>, Expression, T>> dispatcherMap =
            ExpressionVisitDispatcher<T>.BuildBasicMap();

        private static Dictionary<Type, Func<IMonoAstVisitor<T>, Statement, T>> statementDispatcherMap =
            ExpressionVisitDispatcher<T>.BuildBasicStatementMap();

        private IMonoAstVisitor<T> visitor;

        public ExpressionVisitDispatcher(IMonoAstVisitor<T> visitor)
        {
            this.visitor = visitor;
        }

        public T Dispatch(Expression expression)
        {
            return ExpressionVisitDispatcher<T>.dispatcherMap[expression.GetType()](this.visitor, expression);
        }

        public T Dispatch(Statement statement)
        {
            return ExpressionVisitDispatcher<T>.statementDispatcherMap[statement.GetType()](this.visitor, statement);
        }

        private static Dictionary<Type, Func<IMonoAstVisitor<T>, Expression, T>> BuildBasicMap()
        {
            Dictionary<Type, Func<IMonoAstVisitor<T>, Expression, T>> map = new Dictionary<Type, Func<IMonoAstVisitor<T>, Expression, T>>();

            map.Add(typeof(Expression), (visitor, expr) => visitor.Visit((Expression)expr));
            map.Add(typeof(MemberExpr), (visitor, expr) => visitor.Visit((MemberExpr)expr));
            map.Add(typeof(MethodGroupExpr), (visitor, expr) => visitor.Visit((MethodGroupExpr)expr));
            map.Add(typeof(FieldExpr), (visitor, expr) => visitor.Visit((FieldExpr)expr));
            map.Add(typeof(PropertyExpr), (visitor, expr) => visitor.Visit((PropertyExpr)expr));
            map.Add(typeof(IndexerExpr), (visitor, expr) => visitor.Visit((IndexerExpr)expr));
            map.Add(typeof(ConstantExpr), (visitor, expr) => visitor.Visit((ConstantExpr)expr));
            map.Add(typeof(EventExpr), (visitor, expr) => visitor.Visit((EventExpr)expr));
            map.Add(typeof(ExpressionStatement), (visitor, expr) => visitor.Visit((ExpressionStatement)expr));
            map.Add(typeof(AnonymousExpression), (visitor, expr) => visitor.Visit((AnonymousExpression)expr));
            map.Add(typeof(StateMachineInitializer), (visitor, expr) => visitor.Visit((StateMachineInitializer)expr));
            map.Add(typeof(Iterator), (visitor, expr) => visitor.Visit((Iterator)expr));
            map.Add(typeof(AsyncInitializer), (visitor, expr) => visitor.Visit((AsyncInitializer)expr));
            map.Add(typeof(AnonymousMethodBody), (visitor, expr) => visitor.Visit((AnonymousMethodBody)expr));
            map.Add(typeof(ConstructorInitializer), (visitor, expr) => visitor.Visit((ConstructorInitializer)expr));
            map.Add(typeof(ConstructorBaseInitializer), (visitor, expr) => visitor.Visit((ConstructorBaseInitializer)expr));
            map.Add(typeof(ConstructorThisInitializer), (visitor, expr) => visitor.Visit((ConstructorThisInitializer)expr));
            map.Add(typeof(UnaryMutator), (visitor, expr) => visitor.Visit((UnaryMutator)expr));
            map.Add(typeof(Invocation), (visitor, expr) => visitor.Visit((Invocation)expr));
            map.Add(typeof(New), (visitor, expr) => visitor.Visit((New)expr));
            map.Add(typeof(NewInitialize), (visitor, expr) => visitor.Visit((NewInitialize)expr));
            map.Add(typeof(NewAnonymousType), (visitor, expr) => visitor.Visit((NewAnonymousType)expr));
            map.Add(typeof(EmptyExpressionStatement), (visitor, expr) => visitor.Visit((EmptyExpressionStatement)expr));
            map.Add(typeof(Assign), (visitor, expr) => visitor.Visit((Assign)expr));
            map.Add(typeof(ElementInitializer), (visitor, expr) => visitor.Visit((ElementInitializer)expr));
            map.Add(typeof(SimpleAssign), (visitor, expr) => visitor.Visit((SimpleAssign)expr));
            map.Add(typeof(RuntimeExplicitAssign), (visitor, expr) => visitor.Visit((RuntimeExplicitAssign)expr));
            map.Add(typeof(FieldInitializer), (visitor, expr) => visitor.Visit((FieldInitializer)expr));
            map.Add(typeof(CompoundAssign), (visitor, expr) => visitor.Visit((CompoundAssign)expr));
            map.Add(typeof(CompoundAssign.TargetExpression), (visitor, expr) => visitor.Visit((CompoundAssign.TargetExpression)expr));
            map.Add(typeof(CollectionOrObjectInitializers), (visitor, expr) => visitor.Visit((CollectionOrObjectInitializers)expr));
            map.Add(typeof(CompletingExpression), (visitor, expr) => visitor.Visit((CompletingExpression)expr));
            map.Add(typeof(CompletionSimpleName), (visitor, expr) => visitor.Visit((CompletionSimpleName)expr));
            map.Add(typeof(CompletionMemberAccess), (visitor, expr) => visitor.Visit((CompletionMemberAccess)expr));
            map.Add(typeof(CompletionElementInitializer), (visitor, expr) => visitor.Visit((CompletionElementInitializer)expr));
            map.Add(typeof(Await), (visitor, expr) => visitor.Visit((Await)expr));
            map.Add(typeof(CompositeExpression), (visitor, expr) => visitor.Visit((CompositeExpression)expr));
            map.Add(typeof(DefaultParameterValueExpression), (visitor, expr) => visitor.Visit((DefaultParameterValueExpression)expr));
            map.Add(typeof(UnwrapCall), (visitor, expr) => visitor.Visit((UnwrapCall)expr));
            map.Add(typeof(Constant), (visitor, expr) => visitor.Visit((Constant)expr));
            map.Add(typeof(NullConstant), (visitor, expr) => visitor.Visit((NullConstant)expr));
            map.Add(typeof(NullLiteral), (visitor, expr) => visitor.Visit((NullLiteral)expr));
            map.Add(typeof(LiftedNull), (visitor, expr) => visitor.Visit((LiftedNull)expr));
            map.Add(typeof(BoolConstant), (visitor, expr) => visitor.Visit((BoolConstant)expr));
            map.Add(typeof(BoolLiteral), (visitor, expr) => visitor.Visit((BoolLiteral)expr));
            map.Add(typeof(CharConstant), (visitor, expr) => visitor.Visit((CharConstant)expr));
            map.Add(typeof(CharLiteral), (visitor, expr) => visitor.Visit((CharLiteral)expr));
            map.Add(typeof(IntegralConstant), (visitor, expr) => visitor.Visit((IntegralConstant)expr));
            map.Add(typeof(IntConstant), (visitor, expr) => visitor.Visit((IntConstant)expr));
            map.Add(typeof(IntLiteral), (visitor, expr) => visitor.Visit((IntLiteral)expr));
            map.Add(typeof(UIntConstant), (visitor, expr) => visitor.Visit((UIntConstant)expr));
            map.Add(typeof(UIntLiteral), (visitor, expr) => visitor.Visit((UIntLiteral)expr));
            map.Add(typeof(LongConstant), (visitor, expr) => visitor.Visit((LongConstant)expr));
            map.Add(typeof(LongLiteral), (visitor, expr) => visitor.Visit((LongLiteral)expr));
            map.Add(typeof(ULongConstant), (visitor, expr) => visitor.Visit((ULongConstant)expr));
            map.Add(typeof(ULongLiteral), (visitor, expr) => visitor.Visit((ULongLiteral)expr));
            map.Add(typeof(ByteConstant), (visitor, expr) => visitor.Visit((ByteConstant)expr));
            map.Add(typeof(SByteConstant), (visitor, expr) => visitor.Visit((SByteConstant)expr));
            map.Add(typeof(ShortConstant), (visitor, expr) => visitor.Visit((ShortConstant)expr));
            map.Add(typeof(UShortConstant), (visitor, expr) => visitor.Visit((UShortConstant)expr));
            map.Add(typeof(FloatConstant), (visitor, expr) => visitor.Visit((FloatConstant)expr));
            map.Add(typeof(FloatLiteral), (visitor, expr) => visitor.Visit((FloatLiteral)expr));
            map.Add(typeof(DoubleConstant), (visitor, expr) => visitor.Visit((DoubleConstant)expr));
            map.Add(typeof(DoubleLiteral), (visitor, expr) => visitor.Visit((DoubleLiteral)expr));
            map.Add(typeof(DecimalConstant), (visitor, expr) => visitor.Visit((DecimalConstant)expr));
            map.Add(typeof(DecimalLiteral), (visitor, expr) => visitor.Visit((DecimalLiteral)expr));
            map.Add(typeof(StringConstant), (visitor, expr) => visitor.Visit((StringConstant)expr));
            map.Add(typeof(StringLiteral), (visitor, expr) => visitor.Visit((StringLiteral)expr));
            map.Add(typeof(EmptyConstantCast), (visitor, expr) => visitor.Visit((EmptyConstantCast)expr));
            map.Add(typeof(EnumConstant), (visitor, expr) => visitor.Visit((EnumConstant)expr));
            map.Add(typeof(SideEffectConstant), (visitor, expr) => visitor.Visit((SideEffectConstant)expr));
            map.Add(typeof(UserOperatorCall), (visitor, expr) => visitor.Visit((UserOperatorCall)expr));
            map.Add(typeof(ConditionalLogicalOperator), (visitor, expr) => visitor.Visit((ConditionalLogicalOperator)expr));
            map.Add(typeof(ShimExpression), (visitor, expr) => visitor.Visit((ShimExpression)expr));
            map.Add(typeof(ParenthesizedExpression), (visitor, expr) => visitor.Visit((ParenthesizedExpression)expr));
            map.Add(typeof(Cast), (visitor, expr) => visitor.Visit((Cast)expr));
            map.Add(typeof(ImplicitCast), (visitor, expr) => visitor.Visit((ImplicitCast)expr));
            map.Add(typeof(BooleanExpression), (visitor, expr) => visitor.Visit((BooleanExpression)expr));
            map.Add(typeof(RefValueExpr), (visitor, expr) => visitor.Visit((RefValueExpr)expr));
            map.Add(typeof(RefTypeExpr), (visitor, expr) => visitor.Visit((RefTypeExpr)expr));
            map.Add(typeof(MakeRefExpr), (visitor, expr) => visitor.Visit((MakeRefExpr)expr));
            map.Add(typeof(AnonymousTypeParameter), (visitor, expr) => visitor.Visit((AnonymousTypeParameter)expr));
            map.Add(typeof(ConstInitializer), (visitor, expr) => visitor.Visit((ConstInitializer)expr));
            map.Add(typeof(AQueryClause), (visitor, expr) => visitor.Visit((AQueryClause)expr));
            map.Add(typeof(QueryExpression), (visitor, expr) => visitor.Visit((QueryExpression)expr));
            map.Add(typeof(ARangeVariableQueryClause), (visitor, expr) => visitor.Visit((ARangeVariableQueryClause)expr));
            map.Add(typeof(QueryStartClause), (visitor, expr) => visitor.Visit((QueryStartClause)expr));
            map.Add(typeof(SelectMany), (visitor, expr) => visitor.Visit((SelectMany)expr));
            map.Add(typeof(Join), (visitor, expr) => visitor.Visit((Join)expr));
            map.Add(typeof(GroupJoin), (visitor, expr) => visitor.Visit((GroupJoin)expr));
            map.Add(typeof(Let), (visitor, expr) => visitor.Visit((Let)expr));
            map.Add(typeof(GroupBy), (visitor, expr) => visitor.Visit((GroupBy)expr));
            map.Add(typeof(Select), (visitor, expr) => visitor.Visit((Select)expr));
            map.Add(typeof(Where), (visitor, expr) => visitor.Visit((Where)expr));
            map.Add(typeof(OrderByAscending), (visitor, expr) => visitor.Visit((OrderByAscending)expr));
            map.Add(typeof(ThenByAscending), (visitor, expr) => visitor.Visit((ThenByAscending)expr));
            map.Add(typeof(OrderByDescending), (visitor, expr) => visitor.Visit((OrderByDescending)expr));
            map.Add(typeof(ThenByDescending), (visitor, expr) => visitor.Visit((ThenByDescending)expr));
            map.Add(typeof(DynamicResultCast), (visitor, expr) => visitor.Visit((DynamicResultCast)expr));
            map.Add(typeof(DynamicMemberAssignable), (visitor, expr) => visitor.Visit((DynamicMemberAssignable)expr));
            map.Add(typeof(DynamicEventCompoundAssign), (visitor, expr) => visitor.Visit((DynamicEventCompoundAssign)expr));
            map.Add(typeof(DynamicIndexBinder), (visitor, expr) => visitor.Visit((DynamicIndexBinder)expr));
            map.Add(typeof(DynamicMemberBinder), (visitor, expr) => visitor.Visit((DynamicMemberBinder)expr));
            map.Add(typeof(DynamicInvocation), (visitor, expr) => visitor.Visit((DynamicInvocation)expr));
            map.Add(typeof(DynamicUnaryConversion), (visitor, expr) => visitor.Visit((DynamicUnaryConversion)expr));
            map.Add(typeof(DynamicConversion), (visitor, expr) => visitor.Visit((DynamicConversion)expr));
            map.Add(typeof(DynamicExpressionStatement), (visitor, expr) => visitor.Visit((DynamicExpressionStatement)expr));
            map.Add(typeof(Unary), (visitor, expr) => visitor.Visit((Unary)expr));
            map.Add(typeof(BooleanExpressionFalse), (visitor, expr) => visitor.Visit((BooleanExpressionFalse)expr));
            map.Add(typeof(LiftedUnaryOperator), (visitor, expr) => visitor.Visit((LiftedUnaryOperator)expr));
            map.Add(typeof(Indirection), (visitor, expr) => visitor.Visit((Indirection)expr));
            map.Add(typeof(Probe), (visitor, expr) => visitor.Visit((Probe)expr));
            map.Add(typeof(Is), (visitor, expr) => visitor.Visit((Is)expr));
            map.Add(typeof(As), (visitor, expr) => visitor.Visit((As)expr));
            map.Add(typeof(DefaultValueExpression), (visitor, expr) => visitor.Visit((DefaultValueExpression)expr));
            map.Add(typeof(Binary), (visitor, expr) => visitor.Visit((Binary)expr));
            map.Add(typeof(LiftedBinaryOperator), (visitor, expr) => visitor.Visit((LiftedBinaryOperator)expr));
            map.Add(typeof(StringConcat), (visitor, expr) => visitor.Visit((StringConcat)expr));
            map.Add(typeof(PointerArithmetic), (visitor, expr) => visitor.Visit((PointerArithmetic)expr));
            map.Add(typeof(Conditional), (visitor, expr) => visitor.Visit((Conditional)expr));
            map.Add(typeof(VariableReference), (visitor, expr) => visitor.Visit((VariableReference)expr));
            map.Add(typeof(LocalVariableReference), (visitor, expr) => visitor.Visit((LocalVariableReference)expr));
            map.Add(typeof(ParameterReference), (visitor, expr) => visitor.Visit((ParameterReference)expr));
            map.Add(typeof(This), (visitor, expr) => visitor.Visit((This)expr));
            map.Add(typeof(BaseThis), (visitor, expr) => visitor.Visit((BaseThis)expr));
            map.Add(typeof(TemporaryVariableReference), (visitor, expr) => visitor.Visit((TemporaryVariableReference)expr));
            map.Add(typeof(ArrayInitializer), (visitor, expr) => visitor.Visit((ArrayInitializer)expr));
            map.Add(typeof(ArrayCreation), (visitor, expr) => visitor.Visit((ArrayCreation)expr));
            map.Add(typeof(ArglistAccess), (visitor, expr) => visitor.Visit((ArglistAccess)expr));
            map.Add(typeof(Arglist), (visitor, expr) => visitor.Visit((Arglist)expr));
            map.Add(typeof(TypeOf), (visitor, expr) => visitor.Visit((TypeOf)expr));
            map.Add(typeof(SizeOf), (visitor, expr) => visitor.Visit((SizeOf)expr));
            map.Add(typeof(FullNamedExpression), (visitor, expr) => visitor.Visit((FullNamedExpression)expr));
            map.Add(typeof(ATypeNameExpression), (visitor, expr) => visitor.Visit((ATypeNameExpression)expr));
            map.Add(typeof(MemberAccess), (visitor, expr) => visitor.Visit((MemberAccess)expr));
            map.Add(typeof(QualifiedAliasMember), (visitor, expr) => visitor.Visit((QualifiedAliasMember)expr));
            map.Add(typeof(SimpleName), (visitor, expr) => visitor.Visit((SimpleName)expr));
            map.Add(typeof(TypeExpr), (visitor, expr) => visitor.Visit((TypeExpr)expr));
            map.Add(typeof(ComposedCast), (visitor, expr) => visitor.Visit((ComposedCast)expr));
            map.Add(typeof(TypeExpression), (visitor, expr) => visitor.Visit((TypeExpression)expr));
            map.Add(typeof(TypeParameterExpr), (visitor, expr) => visitor.Visit((TypeParameterExpr)expr));
            map.Add(typeof(NullableType), (visitor, expr) => visitor.Visit((NullableType)expr));
            map.Add(typeof(SpecialContraintExpr), (visitor, expr) => visitor.Visit((SpecialContraintExpr)expr));
            map.Add(typeof(Namespace), (visitor, expr) => visitor.Visit((Namespace)expr));
            map.Add(typeof(RootNamespace), (visitor, expr) => visitor.Visit((RootNamespace)expr));
            map.Add(typeof(GlobalRootNamespace), (visitor, expr) => visitor.Visit((GlobalRootNamespace)expr));
            map.Add(typeof(CheckedExpr), (visitor, expr) => visitor.Visit((CheckedExpr)expr));
            map.Add(typeof(UnCheckedExpr), (visitor, expr) => visitor.Visit((UnCheckedExpr)expr));
            map.Add(typeof(ElementAccess), (visitor, expr) => visitor.Visit((ElementAccess)expr));
            map.Add(typeof(ArrayAccess), (visitor, expr) => visitor.Visit((ArrayAccess)expr));
            map.Add(typeof(EmptyExpression), (visitor, expr) => visitor.Visit((EmptyExpression)expr));
            map.Add(typeof(ErrorExpression), (visitor, expr) => visitor.Visit((ErrorExpression)expr));
            map.Add(typeof(UserCast), (visitor, expr) => visitor.Visit((UserCast)expr));
            map.Add(typeof(TypeCast), (visitor, expr) => visitor.Visit((TypeCast)expr));
            map.Add(typeof(ArrayIndexCast), (visitor, expr) => visitor.Visit((ArrayIndexCast)expr));
            map.Add(typeof(EmptyCast), (visitor, expr) => visitor.Visit((EmptyCast)expr));
            map.Add(typeof(OperatorCast), (visitor, expr) => visitor.Visit((OperatorCast)expr));
            map.Add(typeof(BoxedCast), (visitor, expr) => visitor.Visit((BoxedCast)expr));
            map.Add(typeof(UnboxCast), (visitor, expr) => visitor.Visit((UnboxCast)expr));
            map.Add(typeof(ConvCast), (visitor, expr) => visitor.Visit((ConvCast)expr));
            map.Add(typeof(ClassCast), (visitor, expr) => visitor.Visit((ClassCast)expr));
            map.Add(typeof(Wrap), (visitor, expr) => visitor.Visit((Wrap)expr));
            map.Add(typeof(StackAlloc), (visitor, expr) => visitor.Visit((StackAlloc)expr));
            map.Add(typeof(ReducedExpression), (visitor, expr) => visitor.Visit((ReducedExpression)expr));
            map.Add(typeof(LocalTemporary), (visitor, expr) => visitor.Visit((LocalTemporary)expr));
            map.Add(typeof(AnonymousMethodExpression), (visitor, expr) => visitor.Visit((AnonymousMethodExpression)expr));
            map.Add(typeof(LambdaExpression), (visitor, expr) => visitor.Visit((LambdaExpression)expr));
            map.Add(typeof(RuntimeValueExpression), (visitor, expr) => visitor.Visit((RuntimeValueExpression)expr));
            map.Add(typeof(DelegateCreation), (visitor, expr) => visitor.Visit((DelegateCreation)expr));
            map.Add(typeof(ImplicitDelegateCreation), (visitor, expr) => visitor.Visit((ImplicitDelegateCreation)expr));
            map.Add(typeof(NewDelegate), (visitor, expr) => visitor.Visit((NewDelegate)expr));
            map.Add(typeof(Unwrap), (visitor, expr) => visitor.Visit((Unwrap)expr));
            map.Add(typeof(Lifted), (visitor, expr) => visitor.Visit((Lifted)expr));
            map.Add(typeof(NullCoalescingOperator), (visitor, expr) => visitor.Visit((NullCoalescingOperator)expr));
            map.Add(typeof(DelegateInvocation), (visitor, expr) => visitor.Visit((DelegateInvocation)expr));

            ExpressionVisitDispatcher<T>.BuildLeftoutMap(map);

            return map;
        }

        private static Dictionary<Type, Func<IMonoAstVisitor<T>, Statement, T>> BuildBasicStatementMap()
        {
            Dictionary<Type, Func<IMonoAstVisitor<T>, Statement, T>> map = new Dictionary<Type, Func<IMonoAstVisitor<T>, Statement, T>>();
            map.Add(typeof(Statement), (visitor, expr) => visitor.Visit((Statement)expr));
            map.Add(typeof(ResumableStatement), (visitor, expr) => visitor.Visit((ResumableStatement)expr));
            map.Add(typeof(Yield), (visitor, expr) => visitor.Visit((Yield)expr));
            map.Add(typeof(YieldStatement<AsyncInitializer>), (visitor, expr) => visitor.Visit((YieldStatement<AsyncInitializer>)expr));
            map.Add(typeof(ExceptionStatement), (visitor, expr) => visitor.Visit((ExceptionStatement)expr));
            map.Add(typeof(TryFinallyBlock), (visitor, expr) => visitor.Visit((TryFinallyBlock)expr));
            map.Add(typeof(Lock), (visitor, expr) => visitor.Visit((Lock)expr));
            map.Add(typeof(TryFinally), (visitor, expr) => visitor.Visit((TryFinally)expr));
            map.Add(typeof(Using), (visitor, expr) => visitor.Visit((Using)expr));
            map.Add(typeof(Using.VariableDeclaration), (visitor, expr) => visitor.Visit((Using.VariableDeclaration)expr));
            map.Add(typeof(TryCatch), (visitor, expr) => visitor.Visit((TryCatch)expr));
            map.Add(typeof(ExitStatement), (visitor, expr) => visitor.Visit((ExitStatement)expr));
            map.Add(typeof(YieldBreak), (visitor, expr) => visitor.Visit((YieldBreak)expr));
            map.Add(typeof(Return), (visitor, expr) => visitor.Visit((Return)expr));
            map.Add(typeof(ContextualReturn), (visitor, expr) => visitor.Visit((ContextualReturn)expr));
            map.Add(typeof(Block), (visitor, expr) => visitor.Visit((Block)expr));
            map.Add(typeof(ExplicitBlock), (visitor, expr) => visitor.Visit((ExplicitBlock)expr));
            map.Add(typeof(ParametersBlock), (visitor, expr) => visitor.Visit((ParametersBlock)expr));
            map.Add(typeof(QueryBlock), (visitor, expr) => visitor.Visit((QueryBlock)expr));
            map.Add(typeof(ToplevelBlock), (visitor, expr) => visitor.Visit((ToplevelBlock)expr));
            map.Add(typeof(EmptyStatement), (visitor, expr) => visitor.Visit((EmptyStatement)expr));
            map.Add(typeof(If), (visitor, expr) => visitor.Visit((If)expr));
            map.Add(typeof(Do), (visitor, expr) => visitor.Visit((Do)expr));
            map.Add(typeof(While), (visitor, expr) => visitor.Visit((While)expr));
            map.Add(typeof(For), (visitor, expr) => visitor.Visit((For)expr));
            map.Add(typeof(StatementExpression), (visitor, expr) => visitor.Visit((StatementExpression)expr));
            map.Add(typeof(StatementErrorExpression), (visitor, expr) => visitor.Visit((StatementErrorExpression)expr));
            map.Add(typeof(StatementList), (visitor, expr) => visitor.Visit((StatementList)expr));
            map.Add(typeof(Goto), (visitor, expr) => visitor.Visit((Goto)expr));
            map.Add(typeof(LabeledStatement), (visitor, expr) => visitor.Visit((LabeledStatement)expr));
            map.Add(typeof(GotoDefault), (visitor, expr) => visitor.Visit((GotoDefault)expr));
            map.Add(typeof(GotoCase), (visitor, expr) => visitor.Visit((GotoCase)expr));
            map.Add(typeof(Throw), (visitor, expr) => visitor.Visit((Throw)expr));
            map.Add(typeof(Break), (visitor, expr) => visitor.Visit((Break)expr));
            map.Add(typeof(Continue), (visitor, expr) => visitor.Visit((Continue)expr));
            map.Add(typeof(BlockVariableDeclaration), (visitor, expr) => visitor.Visit((BlockVariableDeclaration)expr));
            map.Add(typeof(BlockConstantDeclaration), (visitor, expr) => visitor.Visit((BlockConstantDeclaration)expr));
            map.Add(typeof(Switch), (visitor, expr) => visitor.Visit((Switch)expr));
            map.Add(typeof(Unchecked), (visitor, expr) => visitor.Visit((Unchecked)expr));
            map.Add(typeof(Checked), (visitor, expr) => visitor.Visit((Checked)expr));
            map.Add(typeof(Unsafe), (visitor, expr) => visitor.Visit((Unsafe)expr));
            map.Add(typeof(Fixed), (visitor, expr) => visitor.Visit((Fixed)expr));
            map.Add(typeof(Catch), (visitor, expr) => visitor.Visit((Catch)expr));
            map.Add(typeof(Foreach), (visitor, expr) => visitor.Visit((Foreach)expr));
            map.Add(typeof(Foreach.ArrayForeach), (visitor, expr) => visitor.Visit((Foreach.ArrayForeach)expr));
            map.Add(typeof(Foreach.CollectionForeach), (visitor, expr) => visitor.Visit((Foreach.CollectionForeach)expr));

            ExpressionVisitDispatcher<T>.BuildLeftoutMap(map);

            return map;
        }

        private static void BuildLeftoutMap<U>(Dictionary<Type, Func<IMonoAstVisitor<T>, U, T>> delegateMap)
        {
            var assembly = typeof(U).Assembly;

            Dictionary<Type, IList<Type>> typeMap = new Dictionary<Type, IList<Type>>();

            List<Type> types = new List<Type>(assembly.GetTypes());
            while (types.Count > 0)
            {
                Type type = types[0];
                types.RemoveAt(0);

                if (!type.IsClass) continue;

                // var nestedTypes = type.GetNestedTypes(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                // types.AddRange(nestedTypes);

                IList<Type> typeList;
                if (!typeMap.TryGetValue(type.BaseType, out typeList))
                {
                    typeList = new List<Type>();
                    typeMap[type.BaseType] = typeList;
                }

                typeList.Add(type);
            }

            ExpressionVisitDispatcher<T>.InsertNonPublicTypes(
                delegateMap,
                typeMap,
                typeof(U));
        }

        private static void InsertNonPublicTypes<U>(
            Dictionary<Type, Func<IMonoAstVisitor<T>, U, T>> delegateMap,
            Dictionary<Type, IList<Type>> baseTypeToDerivedMap,
            Type type)
        {
            IList<Type> derivedTypes;
            if (baseTypeToDerivedMap.TryGetValue(type, out derivedTypes))
            {
                foreach (var derivedType in baseTypeToDerivedMap[type])
                {
                    if (derivedType.IsNotPublic || (derivedType.IsNested && !derivedType.IsNestedPublic))
                    {
                        delegateMap.Add(derivedType, delegateMap[type]);
                    }

                    ExpressionVisitDispatcher<T>.InsertNonPublicTypes(
                        delegateMap,
                        baseTypeToDerivedMap,
                        derivedType);
                }
            }
        }
    }
}
*/