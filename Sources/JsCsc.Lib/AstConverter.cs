//-----------------------------------------------------------------------
// <copyright file="AstConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR;
    using Mono.Cecil;
    using Mono.CSharp;
    using Mono.CSharp.Linq;
    using Mono.CSharp.Nullable;
    using Ast = NScript.CLR.AST;

    /// <summary>
    /// Definition for AstConverter
    /// </summary>
    public class AstConverter : IMonoAstVisitor<Ast.Node>
    {
        private ExpressionVisitDispatcher<Ast.Node> dispatcher;

        private MemberReferenceConverter _referenceConverter;

        private Mono.CSharp.Method _currentMethod;

        private Mono.Cecil.MethodDefinition _currentMethodDefinition;

        private Ast.TopLevelBlock _topLevelBlock;

        private LinkedList<Tuple<Ast.ScopeBlock, ExplicitBlock>> scopeBlockStack = new LinkedList<Tuple<Ast.ScopeBlock, ExplicitBlock>>();

        private LinkedList<Ast.ParameterBlock> parameterBlockStack = new LinkedList<Ast.ParameterBlock>();

        public AstConverter(MemberReferenceConverter referenceConverter)
        {
            this._referenceConverter = referenceConverter;
            this.dispatcher = new ExpressionVisitDispatcher<Ast.Node>(this);
        }

        public ClrContext Context
        { get { return this._referenceConverter.Context; } }

        public Ast.TopLevelBlock SetMethodToConvert(Mono.CSharp.Method methodImplementation, ToplevelBlock topLevelBlock)
        {
            this._currentMethod = methodImplementation;
            this._currentMethodDefinition = this._referenceConverter.GetMethodReference(methodImplementation).Resolve();
            this._topLevelBlock = new Ast.TopLevelBlock(this._currentMethodDefinition);
            this._topLevelBlock.RootBlock = (Ast.ParameterBlock)this.Visit(topLevelBlock);
            return this._topLevelBlock;
        }

        public Ast.Node Visit(Expression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(NullLiteral expression)
        {
            return new Ast.NullLiteral(
                this.Context,
                expression.Location.GetCsLocation());
        }

        public Ast.Node Visit(BoolLiteral expression)
        {
            return new Ast.BooleanLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(CharLiteral expression)
        {
            return new Ast.CharLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(IntLiteral expression)
        {
            return new Ast.IntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(UIntLiteral expression)
        {
            return new Ast.UIntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(LongLiteral expression)
        {
            return new Ast.IntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(ULongLiteral expression)
        {
            return new Ast.UIntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(FloatLiteral expression)
        {
            return new Ast.DoubleLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(DoubleLiteral expression)
        {
            return new Ast.DoubleLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(DecimalLiteral expression)
        { return null; }

        public Ast.Node Visit(StringLiteral expression)
        {
            return new Ast.StringLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(LocalTemporary expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Assign expression)
        {
            return new Ast.BinaryExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Target),
                (Ast.Expression)this.Dispatch(expression.Source),
                Ast.BinaryOperator.Assignment);
        }

        public Ast.Node Visit(SimpleAssign expression)
        {
            return this.Visit((Assign)expression);
        }

        public Ast.Node Visit(RuntimeExplicitAssign expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(FieldInitializer expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(CompoundAssign expression)
        {
            Ast.BinaryOperator? op = AstConverter.GetOperator(expression.Operator);
            if (!op.HasValue)
            {
                throw new InvalidOperationException();
            }

            if (expression.Target is EventExpr)
            {
                EventExpr evtExpr = expression.Target as EventExpr;
                return new Ast.MethodCallExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this.GetMethodReference(
                        evtExpr.Operator,
                        evtExpr.Location,
                        evtExpr.InstanceExpression),
                    (Ast.Expression)this.Dispatch(expression.Source));
            }

            return new Ast.BinaryExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Target),
                (Ast.Expression)this.Dispatch(expression.Source),
                op.Value);
        }

        public Ast.Node Visit(CompoundAssign.TargetExpression expression)
        { return this.Dispatch(expression.child); }

        public Ast.Node Visit(EmptyCast expression)
        { return this.Dispatch(expression.Child); }

        public Ast.Node Visit(OperatorCast expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(BoxedCast expression)
        { return this.Dispatch(expression.Child); }

        public Ast.Node Visit(UnboxCast expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ClassCast expression)
        { return this.Dispatch(expression.Child); }

        public Ast.Node Visit(Cast expression)
        {
            this.Dispatch(expression.TargetType);
            this.Dispatch(expression.Expr);

            return null;
        }

        public Ast.Node Visit(ImplicitCast expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(BoolConstant expression)
        {
            return new Ast.BooleanLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(ByteConstant expression)
        {
            return new Ast.UIntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(CharConstant expression)
        {
            return new Ast.CharLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(SByteConstant expression)
        {
            return new Ast.IntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(ShortConstant expression)
        {
            return new Ast.IntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(UShortConstant expression)
        {
            return new Ast.UIntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(IntConstant expression)
        {
            return new Ast.IntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(UIntConstant expression)
        {
            return new Ast.UIntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(LongConstant expression)
        {
            return new Ast.IntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(ULongConstant expression)
        {
            return new Ast.UIntLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(FloatConstant expression)
        {
            return new Ast.DoubleLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(DoubleConstant expression)
        {
            return new Ast.DoubleLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(DecimalConstant expression)
        { return null; }

        public Ast.Node Visit(StringConcat expression)
        {
            return new Ast.MethodCallExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                new Ast.MethodReferenceExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetMethodReference(expression.Method)),
                this.GetParameters(expression.Arguments));
        }

        public Ast.Node Visit(NullConstant expression)
        {
            return new Ast.NullLiteral(
                this.Context,
                expression.Location.GetCsLocation());
        }

        public Ast.Node Visit(EmptyConstantCast expression)
        { return this.Dispatch(expression.child); }

        public Ast.Node Visit(EnumConstant expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(SideEffectConstant expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(QueryExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(QueryStartClause expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(GroupBy expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Join expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(GroupJoin expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Let expresstion)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Select expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(SelectMany expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Where expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(OrderByAscending expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(OrderByDescending expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ThenByAscending expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ThenByDescending expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(QueryBlock expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(EmptyStatement expression)
        { return new Ast.ExplicitBlock(this.Context, expression.loc.GetCsLocation()); }

        public Ast.Node Visit(StatementExpression expression)
        { return this.Dispatch(expression.Expr); }

        public Ast.Node Visit(StatementErrorExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(StatementList expression)
        {
            var rv = new Ast.ExplicitBlock(
                this.Context,
                expression.loc.GetCsLocation());

            foreach (var statement in expression.Statements)
            {
                rv.AddStatement(this.GetStatement(this.Dispatch(statement)));
            }

            return rv;
        }

        public Ast.Node Visit(Return expression)
        {
            Ast.ReturnStatement rv = new Ast.ReturnStatement(
                this.Context,
                expression.loc.GetCsLocation(),
                expression.Expr != null
                    ? (Ast.Expression)this.Dispatch(expression.Expr)
                    : null);

            return rv;
        }

        public Ast.Node Visit(Goto expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(GotoDefault expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(GotoCase expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(LabeledStatement expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Throw expression)
        { return this.Dispatch(expression.Expr); }

        public Ast.Node Visit(Break expression)
        { return new Ast.BreakStatement(this.Context, expression.loc.GetCsLocation()); }

        public Ast.Node Visit(Continue expression)
        { return new Ast.ContinueStatement(this.Context, expression.loc.GetCsLocation()); }

        public Ast.Node Visit(BlockVariableDeclaration expression)
        {
            if (expression.Initializer != null)
            {
                return this.Dispatch(expression.Initializer);
            }
            else
            {
                return new Ast.NullLiteral(this.Context, expression.loc.GetCsLocation());
            }
        }

        public Ast.Node Visit(BlockConstantDeclaration expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(LocalVariable exprssion)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(If expression)
        {
            var condition = (Ast.Expression)this.Dispatch(expression.Expr);
            var trueBlock = this.GetScopeBlock(this.Dispatch(expression.TrueStatement));
            var falseBlock = expression.FalseStatement != null
                ? this.GetScopeBlock(this.Dispatch(expression.FalseStatement))
                : null;

            return new Ast.IfBlockStatement(
                this.Context,
                expression.loc.GetCsLocation(),
                condition,
                trueBlock,
                falseBlock);
        }

        public Ast.Node Visit(Do expression)
        {
            return new Ast.DoWhileLoop(
                this.Context,
                expression.loc.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.expr),
                this.GetScopeBlock(this.Dispatch(expression.EmbeddedStatement)));
        }

        public Ast.Node Visit(While expression)
        {
            return new Ast.WhileLoop(
                this.Context,
                expression.loc.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.expr),
                this.GetScopeBlock(this.Dispatch(expression.Statement)));
        }

        public Ast.Node Visit(For expression)
        {
            return new Ast.ForLoop(
                this.Context,
                expression.loc.GetCsLocation(),
                expression.Condition != null
                    ? (Ast.Expression)this.Dispatch(expression.Condition)
                    : null,
                expression.Initializer != null
                    ? this.GetStatement(this.Dispatch(expression.Initializer))
                    : null,
                expression.Iterator != null
                    ? this.GetStatement(this.Dispatch(expression.Iterator))
                    : null,
                this.GetScopeBlock(this.Dispatch(expression.Statement)));
        }

        public Ast.Node Visit(Foreach expression)
        {
            return (Ast.ForEachLoop)this.Dispatch(expression.Statement);
        }

        public Ast.Node Visit(Foreach.ArrayForeach expression)
        {
            return new Ast.ForEachLoop(
                this.Context,
                expression.loc.GetCsLocation(),
                this.GetLocalVariable(expression.Variable.local_info),
                (Ast.Expression)this.Dispatch(expression.ForEach.Expr),
                this.GetScopeBlock(this.Dispatch(expression.Statement)));
        }

        public Ast.Node Visit(Foreach.CollectionForeach expression)
        {
            return new Ast.ForEachLoop(
                this.Context,
                expression.loc.GetCsLocation(),
                this.GetLocalVariable(expression.variable),
                (Ast.Expression)this.Dispatch(expression.Expr),
                this.GetScopeBlock(this.Dispatch(((While)((Using)expression.Statement).Statement).Statement)));
        }

        public Ast.Node Visit(Foreach.CollectionForeach.Body expression)
        {
            return this.Dispatch(expression.Statement);
        }

        public Ast.Node Visit(Switch expression)
        {
            List<KeyValuePair<List<Ast.LiteralExpression>, Ast.Statement>> caseBlocks =
                new List<KeyValuePair<List<Ast.LiteralExpression>, Ast.Statement>>();
            this.Dispatch(expression.Expr);

            foreach (var section in expression.Sections)
            {
                List<Ast.LiteralExpression> labelList = new List<Ast.LiteralExpression>();
                foreach (var label in section.Labels)
                {
                    if (label.IsDefault)
                    {
                        labelList.Add(null);
                    }
                    else
                    {
                        labelList.Add((Ast.LiteralExpression)this.Dispatch(label.Label));
                    }
                }

                caseBlocks.Add(
                    new KeyValuePair<List<Ast.LiteralExpression>, Ast.Statement>(
                        labelList,
                        this.GetStatement(this.Dispatch(section.Block))));
            }

            return new Ast.SwitchStatement(
                this.Context,
                expression.loc.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Expr),
                caseBlocks);
        }

        public Ast.Node Visit(Block expression)
        {
            Ast.ExplicitBlock rv = new Ast.ExplicitBlock(
                this.Context,
                expression.loc.GetCsLocation());

            foreach (var statement in expression.Statements)
            {
                rv.AddStatement(
                    this.GetStatement(
                        this.Dispatch(statement)));
            }

            return rv;
        }

        public Ast.Node Visit(ExplicitBlock expression)
        {
            var rv = new Ast.ScopeBlock(this.Context, expression.loc.GetCsLocation());

            this.scopeBlockStack.AddFirst(Tuple.Create(rv, expression));
            foreach (var statement in expression.Statements)
            {
                rv.AddStatement(
                    this.GetStatement(
                        this.Dispatch(statement)));
            }

            this.scopeBlockStack.RemoveFirst();
            return rv;
        }

        public Ast.Node Visit(ParametersBlock expression)
        {
            Ast.ParameterBlock rv = new Ast.ParameterBlock(
                this.Context,
                expression.loc.GetCsLocation());

            if (expression is ToplevelBlock
                && this._currentMethodDefinition.HasThis)
            {
                rv.AddThisParameter(this._currentMethodDefinition.Body.ThisParameter);
            }

            foreach (var paramDef in this.GetParameters(expression.Parameters))
            {
                rv.AddParameter(paramDef);
            }

            this.scopeBlockStack.AddFirst(Tuple.Create<Ast.ScopeBlock, ExplicitBlock>(rv, expression));
            this.parameterBlockStack.AddFirst(rv);

            foreach (var statement in expression.Statements)
            {
                rv.AddStatement(
                    this.GetStatement(
                        this.Dispatch(statement)));
            }

            this.scopeBlockStack.RemoveFirst();
            this.parameterBlockStack.RemoveFirst();

            return rv;
        }

        public Ast.Node Visit(ToplevelBlock expression)
        {
            return this.Visit((ParametersBlock)expression);
        }

        public Ast.Node Visit(Lock expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Unchecked expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Checked expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Unsafe expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Fixed expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Catch expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(TryFinally expression)
        {
            var tryBlock = this.Dispatch(expression.Statement);
            var tryCatchFinallBlock = tryBlock as Ast.TryCatchFinally;
            var finallyBlock =
                    new Ast.HandlerBlock(
                        this.Context,
                        expression.Finallyblock.loc.GetCsLocation(),
                        null,
                        null,
                        this.GetScopeBlock(this.Dispatch(expression.Finallyblock)));
            if (tryCatchFinallBlock != null)
            {
                tryCatchFinallBlock.AddHandler(finallyBlock);
                return tryCatchFinallBlock;
            }
            else
            {
                return new Ast.TryCatchFinally(
                    this.Context,
                    expression.loc.GetCsLocation(),
                    this.GetScopeBlock(tryBlock),
                    finallyBlock);
            }
        }

        public Ast.Node Visit(TryCatch expression)
        {
            Ast.ScopeBlock tryBlock = this.GetScopeBlock(this.Dispatch(expression.Block));
            List<Ast.HandlerBlock> handlers = new List<Ast.HandlerBlock>();

            foreach (var clause in expression.Clauses)
            {
                handlers.Add(
                    new Ast.HandlerBlock(
                        this.Context,
                        clause.loc.GetCsLocation(),
                        clause.CatchType != null
                            ? this._referenceConverter.GetTypeReference(clause.CatchType)
                            : this.Context.KnownReferences.Exception,
                        null,
                        this.GetScopeBlock(this.Dispatch(clause.Block))));

                if (clause.Variable != null
                    && clause.Variable.Block == clause.Block)
                {
                    // Remember we don't have to create this variable,
                    // since we only need it if it is used, and in that case it will be used
                    handlers[handlers.Count - 1].CatchVariable =
                        new Ast.VariableReference(
                            this.Context,
                            expression.loc.GetCsLocation(),
                            handlers[handlers.Count - 1].Block.ResolveVariable(clause.Variable.Name));
                }
            }

            var rv = new Ast.TryCatchFinally(
                this.Context,
                expression.loc.GetCsLocation(),
                tryBlock,
                handlers[0]);

            for (int iHandler = 1; iHandler < handlers.Count; iHandler++)
            {
                rv.AddHandler(handlers[iHandler]);
            }

            return rv;
        }

        public Ast.Node Visit(Using expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Using.VariableDeclaration expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(NullableType expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Unwrap expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(UnwrapCall expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Wrap expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(LiftedNull expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Lifted expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(LiftedUnaryOperator expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(LiftedBinaryOperator expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(NullCoalescingOperator expression)
        {
            return new Ast.NullConditional(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.LeftExpression),
                (Ast.Expression)this.Dispatch(expression.Right));
        }

        public Ast.Node Visit(Yield expression)
        {
            return new Ast.YieldStatement(
                this.Context,
                expression.loc.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Expr));
        }

        public Ast.Node Visit(YieldBreak expression)
        {
            return new Ast.YieldStatement(
                this.Context,
                expression.loc.GetCsLocation(),
                null);
        }

        public Ast.Node Visit(StateMachine expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(StateMachineInitializer expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Iterator expression)
        {
            return new Ast.InlineIteratorExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.ParameterBlock)this.Dispatch(expression.Block),
                this._referenceConverter.GetTypeReference(expression.Type));
        }

        public Ast.Node Visit(AnonymousExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(AnonymousMethodBody expression)
        {
            return new Ast.AnonymousMethodBodyExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.ParameterBlock)this.Dispatch(expression.Block),
                this._referenceConverter.GetTypeReference(expression.Type));
        }

        public Ast.Node Visit(UserOperatorCall expression)
        {
            return new Ast.MethodCallExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                this.GetMethodReference(
                    expression.oper,
                    expression.Location,
                    null),
                this.GetParameters(expression.arguments));
        }

        public Ast.Node Visit(ParenthesizedExpression expression)
        {
            return this.Dispatch(expression.Expr);
        }

        public Ast.Node Visit(Unary expression)
        { return this.Dispatch(expression.Expr); }

        public Ast.Node Visit(Indirection expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(UnaryMutator expression)
        { return this.Dispatch(expression.Expr); }

        public Ast.Node Visit(Binary expression)
        {
            Ast.BinaryOperator? op = AstConverter.GetOperator(expression.Oper);

            if (!op.HasValue)
            {
                throw new InvalidOperationException();
            }

            return new Ast.BinaryExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Left),
                (Ast.Expression)this.Dispatch(expression.Right),
                op.Value);
        }

        public Ast.Node Visit(Is expression)
        {
            return new Ast.TypeCheckExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Expr),
                this._referenceConverter.GetTypeReference(expression.ProbeTypeSpec),
                Ast.TypeCheckType.IsType);
        }

        public Ast.Node Visit(As expression)
        {
            return new Ast.TypeCheckExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Expr),
                this._referenceConverter.GetTypeReference(expression.ProbeTypeSpec),
                Ast.TypeCheckType.AsType);
        }

        public Ast.Node Visit(ConditionalLogicalOperator expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(BooleanExpression expression)
        { return this.Dispatch(expression.Expr); }

        public Ast.Node Visit(BooleanExpressionFalse expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Conditional expression)
        {
            return new Ast.ConditionalOperatorExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Expr),
                (Ast.Expression)this.Dispatch(expression.TrueExpr),
                (Ast.Expression)this.Dispatch(expression.FalseExpr));
        }

        public Ast.Node Visit(LocalVariableReference expression)
        {
            if (expression.IsRef)
            {
                return new Ast.VariableAddressReference(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this.GetLocalVariable(expression.local_info));
            }
            else
            {
                return new Ast.VariableReference(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this.GetLocalVariable(expression.local_info));
            }
        }

        public Ast.Node Visit(Mono.CSharp.ParameterReference expression)
        {
            return new Ast.VariableReference(
                this.Context,
                expression.Location.GetCsLocation(),
                this.GetParameterVariable(expression.Name));
        }

        public Ast.Node Visit(Invocation expression)
        {
            Ast.Expression[] parameters = null;
            if (expression.Arguments != null)
            {
                parameters = new Ast.Expression[expression.Arguments.Count];
                for (int iArg = 0; iArg < expression.Arguments.Count; iArg++)
                {
                    parameters[iArg] = (Ast.Expression)this.Dispatch(expression.Arguments[iArg].Expr);
                }
            }
            else
            {
                parameters = new Ast.Expression[0];
            }

            return new Ast.MethodCallExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.MethodGroup),
                parameters);
        }

        public Ast.Node Visit(New expression)
        {
            if (expression.Constructor == null)
            {
                return new Ast.DefaultValueExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetTypeReference(expression.Type));
            }

            return new Ast.NewObjectExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                this._referenceConverter.GetMethodReference(expression.Constructor),
                this.GetParameters(expression.Arguments));
        }

        public Ast.Node Visit(ArrayInitializer expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ArrayCreation expression)
        {
            if (expression.ResolvedInitializers != null)
            {
                Ast.Expression[] initializers = new Ast.Expression[expression.ResolvedInitializers.Count];
                for (int iInit = 0; iInit < expression.ResolvedInitializers.Count; iInit++)
                {
                    initializers[iInit] = (Ast.Expression)this.Dispatch(expression.ResolvedInitializers[iInit]);
                }

                return new Ast.InlineArrayInitialization(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetTypeReference(expression.ArrayElementType),
                    initializers);
            }
            else
            {
                return new Ast.NewArrayExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetTypeReference(expression.ArrayElementType),
                    (Ast.Expression)this.Dispatch(expression.Arguments[0]));
            }
        }

        public Ast.Node Visit(This expression)
        {
            return new Ast.VariableReference(
                this.Context,
                expression.Location.GetCsLocation(),
                this.GetThisParameter());
        }

        public Ast.Node Visit(ArglistAccess expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(RefValueExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(RefTypeExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(MakeRefExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(TypeOf expression)
        {
            return new Ast.TypeofExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.TypeReferenceExpression)this.Dispatch(expression.TypeExpression));
        }

        public Ast.Node Visit(SizeOf expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(CheckedExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(UnCheckedExpr experssion)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ElementAccess expression)
        {
            return new Ast.ArrayElementExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Expr),
                (Ast.Expression)this.Dispatch(expression.Arguments[0].Expr));
        }

        public Ast.Node Visit(ArrayAccess expression)
        {
            return this.Visit(expression.ElementAccess);
        }

        public Ast.Node Visit(BaseThis expression)
        {
            return new Ast.VariableReference(
                this.Context,
                expression.Location.GetCsLocation(),
                this.GetThisParameter());
        }

        public Ast.Node Visit(EmptyExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ErrorExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ComposedTypeSpecifier expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ComposedCast expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ArrayIndexCast expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(StackAlloc expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ElementInitializer expression)
        {
            return this.Visit((Assign)expression);
        }

        public Ast.Node Visit(CollectionOrObjectInitializers expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(NewInitialize expression)
        {
            List<Tuple<Ast.MemberReferenceExpression, Ast.Expression>> list = new List<Tuple<Ast.MemberReferenceExpression, Ast.Expression>>();

            if (expression.Initializers != null)
            {
                foreach (var init in expression.Initializers.Initializers)
                {
                    ElementInitializer initializer = (ElementInitializer)init;
                    PropertyExpr propertyExpr = initializer.Target as PropertyExpr;
                    Ast.MemberReferenceExpression memberExpr = null;
                    if (propertyExpr != null)
                    {
                        memberExpr =
                            new Ast.PropertyReferenceExpression(
                                this.Context,
                                propertyExpr.Location.GetCsLocation(),
                                this._referenceConverter.GetPropertyReference(
                                    propertyExpr.Getter ?? propertyExpr.Setter),
                                null);
                    }
                    else
                    {
                        FieldExpr fieldExpr = (FieldExpr)initializer.Target;
                        memberExpr =
                            new Ast.FieldReferenceExpression(
                                this.Context,
                                fieldExpr.Location.GetCsLocation(),
                                this._referenceConverter.GetFieldReference(fieldExpr.Spec),
                                null);
                    }

                    list.Add(
                        new Tuple<Ast.MemberReferenceExpression, Ast.Expression>(
                            memberExpr,
                            (Ast.Expression)this.Dispatch(initializer.Source)));
                }
            }

            return new Ast.InlinePropertyInitilizationExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.NewObjectExpression)this.Visit((New)expression),
                list);
        }

        public Ast.Node Visit(NewAnonymousType expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(AnonymousTypeParameter expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(DynamicResultCast expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(CompositeExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ConstantExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(MemberExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(MethodGroupExpr expression)
        {
            return this.GetMethodReference(
                expression.BestCandidate,
                expression.Location,
                expression.InstanceExpression);
        }

        public Ast.Node Visit(FieldExpr expression)
        {
            if (expression.IsInstance)
            {
                return new Ast.FieldReferenceExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetFieldReference(expression.Spec),
                    (Ast.Expression)this.Dispatch(expression.InstanceExpression));
            }
            else
            {
                return new Ast.FieldReferenceExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetFieldReference(expression.Spec));
            }
        }

        public Ast.Node Visit(PropertyExpr expression)
        {
            if (expression.IsInstance)
            {
                return new Ast.PropertyReferenceExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetPropertyReference(expression.Getter ?? expression.Setter),
                    (Ast.Expression)this.Dispatch(expression.InstanceExpression));
            }

            return new Ast.PropertyReferenceExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                this._referenceConverter.GetPropertyReference(expression.Getter ?? expression.Setter));
        }

        public Ast.Node Visit(IndexerExpr expression)
        {
            var args = this.GetParameters(expression.Arguments);

            if (expression.IsInstance)
            {
                return new Ast.PropertyReferenceExpression(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this._referenceConverter.GetPropertyReference(expression.Getter ?? expression.Setter),
                    (Ast.Expression)this.Dispatch(expression.InstanceExpression),
                    args);
            }

            return new Ast.PropertyReferenceExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                this._referenceConverter.GetPropertyReference(expression.Getter ?? expression.Setter),
                null,
                args);
        }

        public Ast.Node Visit(EventExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ExpressionStatement expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(DelegateInvocation expression)
        {
            var arguments = this.GetParameters(expression.Arguments);

            return new Ast.MethodCallExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.InstanceExpr),
                arguments);
        }

        public Ast.Node Visit(AsyncInitializer expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ConstructorInitializer expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ConstructorThisInitializer expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(EmptyExpressionStatement expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(CompletingExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(CompletionSimpleName expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(CompletionElementInitializer expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Await expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(DefaultParameterValueExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Constant expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(StringConstant expression)
        {
            return new Ast.StringLiteral(
                this.Context,
                expression.Location.GetCsLocation(),
                expression.Value);
        }

        public Ast.Node Visit(ShimExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(AQueryClause expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ARangeVariableQueryClause expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(DefaultValueExpression expression)
        {
            return new Ast.DefaultValueExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                this._referenceConverter.GetTypeReference(expression.Type));
        }

        public Ast.Node Visit(PointerArithmetic expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(VariableReference expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(TemporaryVariableReference expression)
        {
            if (expression.IsRef)
            {
                return new Ast.VariableAddressReference(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this.GetLocalVariable(expression.LocalInfo));
            }
            else
            {
                return new Ast.VariableReference(
                    this.Context,
                    expression.Location.GetCsLocation(),
                    this.GetLocalVariable(expression.LocalInfo));
            }
        }

        public Ast.Node Visit(Arglist expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ATypeNameExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(QualifiedAliasMember expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(MemberAccess expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(TypeParameterName expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(TypeExpression expression)
        {
            return new Ast.TypeReferenceExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                this._referenceConverter.GetTypeReference(
                    expression.Type));
        }

        public Ast.Node Visit(TypeParameterExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(SpecialContraintExpr expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(Namespace expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(RootNamespace expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(GlobalRootNamespace expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(UserCast expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(TypeCast expression)
        {
            return new Ast.TypeCheckExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Child),
                this._referenceConverter.GetTypeReference(expression.Type),
                Ast.TypeCheckType.CastType);
        }

        public Ast.Node Visit(ConvCast expression)
        {
            return new Ast.TypeCheckExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.Expression)this.Dispatch(expression.Child),
                this._referenceConverter.GetTypeReference(expression.Type),
                Ast.TypeCheckType.CastType);
        }

        public Ast.Node Visit(ReducedExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(AnonymousMethodExpression expression)
        { return this.Dispatch(expression.Block); }

        public Ast.Node Visit(LambdaExpression expression)
        { return this.Dispatch(expression.Block); }

        public Ast.Node Visit(RuntimeValueExpression expression)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(DelegateCreation expression)
        {
            return new Ast.DelegateMethodExpression(
                this.Context,
                expression.Location.GetCsLocation(),
                (Ast.MethodReferenceExpression)this.Dispatch(expression.MethodGroup),
                this._referenceConverter.GetTypeReference(expression.Type));
        }

        public Ast.Node Visit(ImplicitDelegateCreation expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public Ast.Node Visit(NewDelegate expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public Ast.Node Visit(Statement statement)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ResumableStatement statement)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(YieldStatement<AsyncInitializer> statement)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ExceptionStatement statement)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(TryFinallyBlock statement)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ExitStatement statement)
        { throw new NotImplementedException(); }

        public Ast.Node Visit(ContextualReturn statement)
        { throw new NotImplementedException(); }

        private Ast.Statement GetStatement(Ast.Node node)
        {
            if (node is Ast.Statement)
            {
                return (Ast.Statement)node;
            }
            else
            {
                return new Ast.ExpressionStatement((Ast.Expression)node);
            }
        }

        private Ast.ScopeBlock GetScopeBlock(Ast.Node node)
        {
            Ast.Statement statement = this.GetStatement(node);
            if (statement is Ast.ScopeBlock)
            {
                return (Ast.ScopeBlock)statement;
            }

            var rv = new Ast.ScopeBlock(
                this.Context,
                statement.Location);

            rv.AddStatement(statement);

            return rv;
        }

        private Ast.Expression[] GetParameters(Arguments arguments)
        {
            Ast.Expression[] parameters = null;
            if (arguments != null)
            {
                parameters = new Ast.Expression[arguments.Count];
                for (int iArg = 0; iArg < arguments.Count; iArg++)
                {
                    parameters[iArg] = (Ast.Expression)this.Dispatch(arguments[iArg].Expr);
                }
            }
            else
            {
                parameters = new Ast.Expression[0];
            }

            return parameters;
        }

        private ParameterDefinition[] GetParameters(ParametersCompiled parameters)
        {
            List<ParameterDefinition> rv = new List<ParameterDefinition>();

            for (int iParam = 0; iParam < parameters.Count; iParam++)
            {
                ParameterAttributes attributes = ParameterAttributes.None;
                var modFlags = parameters[iParam].ModFlags;
                string paramName = parameters[iParam].Name;

                switch (parameters[iParam].ModFlags)
                {
                    case Parameter.Modifier.OUT:
                        attributes = ParameterAttributes.Out;
                        break;
                    case Parameter.Modifier.REF:
                        attributes = ParameterAttributes.Out | ParameterAttributes.In;
                        break;
                    case Parameter.Modifier.This:
                        paramName = "this";
                        break;
                }

                rv.Add(
                    new ParameterDefinition(
                        paramName,
                        attributes,
                        this._referenceConverter.GetTypeReference(
                            parameters[iParam].Type)));
            }

            return rv.ToArray();
        }

        private Ast.ParameterVariable GetParameterVariable(string parameterName)
        {
            foreach (var paramBlock in this.parameterBlockStack)
            {
                Ast.ParameterVariable rv;
                if (paramBlock.GetParameterVariable(parameterName, out rv))
                {
                    return rv;
                }
            }

            throw new InvalidOperationException();
        }

        private Ast.ThisVariable GetThisParameter()
        {
            foreach (var paramBlock in this.parameterBlockStack)
            {
                if (paramBlock.GetThisParameter() != null)
                {
                    return paramBlock.GetThisParameter();
                }
            }

            throw new InvalidOperationException();
        }

        private Ast.LocalVariable GetLocalVariable(LocalVariable variable)
        {
            foreach (var tuple in this.scopeBlockStack)
            {
                if (tuple.Item2 == variable.Block)
                {
                    Ast.LocalVariable rv = tuple.Item1.ResolveVariable(variable.Name);
                    if (rv == null)
                    {
                        rv = tuple.Item1.CreateVariable(
                            variable.Name,
                            this._referenceConverter.GetTypeReference(variable.Type));
                    }

                    return rv;
                }
            }

            throw new InvalidOperationException();
        }

        private Ast.MethodReferenceExpression GetMethodReference(
            MethodSpec method,
            Location loc,
            Expression instanceExpression)
        {
            if (instanceExpression != null)
            {
                bool isVirtual = !method.IsStatic;

                if (isVirtual)
                {
                    isVirtual = instanceExpression is BaseThis
                        ? false
                        : method.IsVirtual;
                }

                if (isVirtual)
                {
                    return new Ast.VirtualMethodReferenceExpression(
                        this.Context,
                        loc.GetCsLocation(),
                        this._referenceConverter.GetMethodReference(method),
                        (Ast.Expression)this.Dispatch(instanceExpression));
                }
                else
                {
                    return new Ast.MethodReferenceExpression(
                        this.Context,
                        loc.GetCsLocation(),
                        this._referenceConverter.GetMethodReference(method),
                        (Ast.Expression)this.Dispatch(instanceExpression));
                }
            }
            else
            {
                return new Ast.MethodReferenceExpression(
                    this.Context,
                    loc.GetCsLocation(),
                    this._referenceConverter.GetMethodReference(method));
            }
        }

        private Ast.Node Dispatch(Statement statement)
        {
            this.PushNode(statement);
            var rv = this.dispatcher.Dispatch(statement);
            this.PopNode();

            if (rv == null)
            {
                throw new NotImplementedException();
            }

            return rv;
        }

        private Ast.Node Dispatch(Expression expression)
        {
            this.PushNode(expression);
            var rv = this.dispatcher.Dispatch(expression);
            this.PopNode();

            if (rv == null)
            {
                throw new NotImplementedException();
            }

            return rv;
        }

        private void PushNode(Statement statement)
        {
        }

        private void PushNode(Expression expr)
        {
        }

        private void PopNode()
        {
        }
    }
}