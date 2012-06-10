//-----------------------------------------------------------------------
// <copyright file="SerializeAst.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Utils;
    using Mono.Cecil;
    using Mono.CSharp;
    using Mono.CSharp.Linq;
    using Mono.CSharp.Nullable;
    using Ast = Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for SerializeAst
    /// </summary>
    public class SerializeAst : IMonoAstVisitor<System.Object>
    {
        ICustomSerializer serializer;

        private ExpressionVisitDispatcher<object> dispatcher;

        private int id = 0;

        private LinkedList<Tuple<int, ExplicitBlock>> scopeBlockStack = new LinkedList<Tuple<int, ExplicitBlock>>();

        private LinkedList<Tuple<int, Ast.ParameterBlock>> parameterBlockStack = new LinkedList<Tuple<int, Ast.ParameterBlock>>();

        public SerializeAst(ICustomSerializer serializer)
        {
            this.dispatcher = new ExpressionVisitDispatcher<object>(this);
            this.serializer = serializer;
        }

        public void SerializeMethodBody(
            Method method,
            ToplevelBlock rootBlock)
        {
            serializer.AddValue(NameTokens.TypeName, TypeTokens.MethodBody);
            serializer.AddValue(
                NameTokens.Method,
                method,
                MemberReferenceSerializer.Serialize);
            serializer.AddValue(
                NameTokens.FileName,
                rootBlock.loc.NameFullPath);
            serializer.AddValue(
                NameTokens.Block,
                rootBlock,
                this.Dispatch);
        }

        public object Visit(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(NullLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.NullLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            return null;
        }

        public object Visit(BoolLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.BoolLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(CharLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.CharLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(IntLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.IntLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(UIntLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.UIntLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(LongLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.LongLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(ULongLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ULongLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(FloatLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.FloatLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(DoubleLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DoubleLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(DecimalLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DecimalLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(StringLiteral expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.StringLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(LocalTemporary expression)
        { throw new NotImplementedException(); }

        public object Visit(Assign expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.Assignment);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.LeftExpr,
                expression.Target,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.RightExpr,
                expression.Source,
                this.Dispatch);
            return null;
        }

        public object Visit(SimpleAssign expression)
        {
            return this.Visit((Assign)expression);
        }

        public object Visit(RuntimeExplicitAssign expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(FieldInitializer expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(CompoundAssign expression)
        {
            if (expression.Target is EventExpr)
            {
                EventExpr evtExpr = expression.Target as EventExpr;

                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.MethodCall);
                this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
                this.serializer.AddValue(
                    NameTokens.Method,
                    evtExpr.Operator,
                    MemberReferenceSerializer.Serialize);
                this.serializer.AddValue(
                    NameTokens.Instance,
                    evtExpr.InstanceExpression,
                    this.Dispatch);
                this.serializer.AddValue(
                    NameTokens.Arguments,
                    new Expression[] { expression.Source },
                    this.Dispatch);

                return null;
            }

            Ast.BinaryOperator? op = AstToJObject.GetOperator(expression.Operator);
            if (!op.HasValue)
            {
                throw new InvalidOperationException();
            }

            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.BinaryExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Operator, op.Value);
            this.serializer.AddValue(
                NameTokens.LeftExpr,
                expression.Target,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.RightExpr,
                expression.Source,
                this.Dispatch);
            return null;
        }

        public object Visit(CompoundAssign.TargetExpression expression)
        {
            return this.Dispatch(expression.child);
        }

        public object Visit(EmptyCast expression)
        {
            return this.Dispatch(expression.Child);
        }

        public object Visit(OperatorCast expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(BoxedCast expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(UnboxCast expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ClassCast expression)
        {
            return this.Dispatch(expression.Child);
        }

        public object Visit(Cast expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TypeCast);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.TargetType,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.Expr,
                expression.Expr,
                this.Dispatch);

            return null;
        }

        public object Visit(ImplicitCast expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(BoolConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.BoolLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(ByteConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ByteLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(CharConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.CharLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(SByteConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.SByteLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(ShortConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ShortLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(UShortConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.UShortConstant);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(IntConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.IntLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(UIntConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.UIntLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(LongConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.LongLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(ULongConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ULongLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(FloatConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.FloatLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(DoubleConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DoubleLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(DecimalConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DecimalLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(StringConcat expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.MethodCall);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Method,
                expression.Method,
                MemberReferenceSerializer.Serialize);

            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.Arguments),
                this.Dispatch);

            return null;
        }

        public object Visit(NullConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.NullLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            return null;
        }

        public object Visit(EmptyConstantCast expression)
        {
            return this.Dispatch(expression.child);
        }

        public object Visit(EnumConstant expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(SideEffectConstant expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(QueryExpression expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(QueryStartClause expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(GroupBy expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(Join expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(GroupJoin expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(Let expresstion)
        {
            throw new NotImplementedException();
        }

        public object Visit(Select expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(SelectMany expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(Where expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(OrderByAscending expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(OrderByDescending expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ThenByAscending expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ThenByDescending expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(QueryBlock expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(EmptyStatement expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.EmptyStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            return null;
        }

        public object Visit(StatementExpression expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.StatementExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(NameTokens.Expr, expression.Expr, this.Dispatch);
            return null;
        }

        public object Visit(StatementErrorExpression expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(StatementList expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.StatementList);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Statements, this.Dispatch);
            return null;
        }

        public object Visit(Return expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ReturnStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Value,
                expression.Expr,
                this.Dispatch);
            return null;
        }

        public object Visit(Goto expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(GotoDefault expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(GotoCase expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(LabeledStatement expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(Throw expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ThrowStatment);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Expr, this.Dispatch);
            return null;
        }

        public object Visit(Break expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.BreakExpression);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            return null;
        }

        public object Visit(Continue expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ContinueExpression);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            return null;
        }

        public object Visit(BlockVariableDeclaration expression)
        {
            if (expression.Initializer != null)
            {
                this.Dispatch(expression.Initializer);
            }
            else
            {
                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.EmptyStatement);
                this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            }

            return null;
        }

        public object Visit(BlockConstantDeclaration expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(LocalVariable exprssion)
        {
            throw new NotImplementedException();
        }

        public object Visit(If expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.IfStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Condition,
                expression.Expr,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.TrueStatement,
                expression.TrueStatement,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.FalseCondition,
                expression.FalseStatement,
                this.Dispatch);

            return null;
        }

        public object Visit(Do expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DoWhileStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Condition,
                expression.expr,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.Loop,
                expression.EmbeddedStatement,
                this.Dispatch);
            return null;
        }

        public object Visit(While expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.WhileStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Condition,
                expression.expr,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.Loop,
                expression.Statement,
                this.Dispatch);
            return null;
        }

        public object Visit(For expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ForStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Initializer,
                expression.Initializer,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Condition,
                expression.Condition,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Iterator,
                expression.Iterator,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.Loop,
                expression.Statement,
                this.Dispatch);
            return null;
        }

        public object Visit(Foreach expression)
        {
            return this.Dispatch(expression.Statement);
        }

        public object Visit(Foreach.ArrayForeach expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ForEachStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Iterator,
                expression.ForEach.Expr,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.LocalVariable,
                expression.Variable.local_info,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Loop,
                expression.ForEach.Statement,
                this.Dispatch);
            return null;
        }

        public object Visit(Foreach.CollectionForeach expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ForEachStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Iterator,
                expression.Expr,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.LocalVariable,
                expression.variable,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Loop,
                ((While)((Using)expression.Statement).Statement).Statement,
                this.Dispatch);

            return null;
        }

        public object Visit(Switch expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.SwitchStatement);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Blocks,
                expression.Sections,
                delegate(ICustomSerializer s, SwitchSection section)
                {
                    this.serializer.AddValue(NameTokens.TypeName, TypeTokens.SwitchSection);
                    this.serializer.AddValue(
                        NameTokens.Labels,
                        section.Labels,
                        delegate(ICustomSerializer s2, SwitchLabel label)
                        {
                            if (label.IsDefault)
                            {
                                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.Default);
                            }
                            else
                            {
                                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.LabelLiteral);
                                this.serializer.AddValue(
                                    NameTokens.Value,
                                    label.Label,
                                    this.Dispatch);
                            }
                        });
                    this.serializer.AddValue(
                        NameTokens.Block,
                        section.Block,
                        this.Dispatch);
                });

            return null;
        }

        public object Visit(Block expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.StatementList);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Value,
                expression.Statements,
                this.Dispatch);
            return null;
        }

        public object Visit(ExplicitBlock expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ScopeBlock);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(NameTokens.Id, ++this.id);
            this.scopeBlockStack.AddFirst(Tuple.Create(this.id, expression));
            this.serializer.AddValue(
                NameTokens.Value,
                expression.Statements,
                this.Dispatch);
            this.scopeBlockStack.RemoveFirst();

            return null;
        }

        public object Visit(ParametersBlock expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ParameterBlock);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Parameters,
                this.GetParameters(expression.Parameters),
                this.Dispatch);

            this.serializer.AddValue(NameTokens.Id, ++this.id);
            this.scopeBlockStack.AddFirst(Tuple.Create(this.id, (ExplicitBlock)expression));
            this.serializer.AddValue(
                NameTokens.Value,
                expression.Statements,
                this.Dispatch);
            this.scopeBlockStack.RemoveFirst();

            return null;
        }

        public object Visit(ToplevelBlock expression)
        {
            return this.Visit((ParametersBlock)expression);
        }

        public object Visit(Lock expression)
        { throw new NotImplementedException(); }

        public object Visit(Unchecked expression)
        { throw new NotImplementedException(); }

        public object Visit(Checked expression)
        { throw new NotImplementedException(); }

        public object Visit(Unsafe expression)
        { throw new NotImplementedException(); }

        public object Visit(Fixed expression)
        { throw new NotImplementedException(); }

        public object Visit(Catch expression)
        { throw new NotImplementedException(); }

        public object Visit(TryFinally expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TryFinally);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.TryBlock,
                expression.Statement,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.FinallyBlock,
                expression.Finallyblock,
                this.Dispatch);

            return null;
        }

        public object Visit(TryCatch expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TryCatch);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.TryBlock,
                expression.Block,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.CatchBlocks,
                expression.Clauses,
                delegate(ICustomSerializer serializer, Catch catchBlock)
                {
                    this.serializer.AddValue(NameTokens.TypeName, TypeTokens.CatchBlock);
                    this.serializer.AddValue(NameTokens.Loc, catchBlock.loc.GetStrLoc());
                    this.serializer.AddValue(
                        NameTokens.Type,
                        catchBlock.CatchType,
                        MemberReferenceSerializer.Serialize);
                    this.serializer.AddValue(
                        NameTokens.LocalVariable,
                        catchBlock.Variable,
                        this.Dispatch);
                    this.serializer.AddValue(
                        NameTokens.Block,
                        catchBlock.Block,
                        this.Dispatch);
                });

            return null;
        }

        public object Visit(Using expression)
        { throw new NotImplementedException(); }

        public object Visit(Using.VariableDeclaration expression)
        { throw new NotImplementedException(); }

        public object Visit(NullableType expression)
        { throw new NotImplementedException(); }

        public object Visit(Unwrap expression)
        { throw new NotImplementedException(); }

        public object Visit(UnwrapCall expression)
        { throw new NotImplementedException(); }

        public object Visit(Wrap expression)
        { throw new NotImplementedException(); }

        public object Visit(LiftedNull expression)
        { throw new NotImplementedException(); }

        public object Visit(Lifted expression)
        { throw new NotImplementedException(); }

        public object Visit(LiftedUnaryOperator expression)
        { throw new NotImplementedException(); }

        public object Visit(LiftedBinaryOperator expression)
        { throw new NotImplementedException(); }

        public object Visit(NullCoalescingOperator expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.NullCoalascing);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.LeftExpr,
                expression.LeftExpression,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.RightExpr,
                expression.RightExpression,
                this.Dispatch);
            return null;
        }

        public object Visit(Yield expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.Yield);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Expr,
                expression.Expr,
                this.Dispatch);
            return null;
        }

        public object Visit(YieldBreak expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.Yield);
            this.serializer.AddValue(NameTokens.Loc, expression.loc.GetStrLoc());
            return null;
        }

        public object Visit(StateMachine expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(StateMachineInitializer expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(Iterator expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.Iterator);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);

            this.serializer.AddValue(
                NameTokens.Block,
                expression.Block,
                this.Dispatch);

            return null;
        }

        public object Visit(AnonymousExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(AnonymousMethodBody expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.LongLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);

            this.serializer.AddValue(
                NameTokens.Block,
                expression.Block,
                this.Dispatch);

            return null;
        }

        public object Visit(UserOperatorCall expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.MethodCall);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Method,
                expression.oper,
                MemberReferenceSerializer.Serialize);

            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.arguments),
                this.Dispatch);
            return null;
        }

        public object Visit(ParenthesizedExpression expression)
        { return this.Dispatch(expression.Expr); }

        public object Visit(Unary expression)
        { return this.Dispatch(expression.Expr); }

        public object Visit(Indirection expression)
        { throw new NotImplementedException(); }

        public object Visit(UnaryMutator expression)
        { return this.Dispatch(expression.Expr); }

        public object Visit(Binary expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.BinaryExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Operator, AstToJObject.GetOperator(expression.Oper).Value);
            this.serializer.AddValue(
                NameTokens.LeftExpr,
                expression.Left,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.RightExpr,
                expression.Right,
                this.Dispatch);

            return null;
        }

        public object Visit(Is expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.IsExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.ProbeType.Type,
                MemberReferenceSerializer.Serialize);

            this.serializer.AddValue(
                NameTokens.Expr,
                expression.Expr,
                this.Dispatch);

            return null;
        }

        public object Visit(As expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.AsExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.ProbeType.Type,
                MemberReferenceSerializer.Serialize);

            this.serializer.AddValue(
                NameTokens.Expr,
                expression.Expr,
                this.Dispatch);

            return null;
        }

        public object Visit(ConditionalLogicalOperator expression)
        { throw new NotImplementedException(); }

        public object Visit(BooleanExpression expression)
        { return this.Dispatch(expression.Expr); }

        public object Visit(BooleanExpressionFalse expression)
        { throw new NotImplementedException(); }

        public object Visit(Conditional expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.Conditional);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Condition,
                expression.Expr,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.TrueStatement,
                expression.TrueExpr,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.FalseCondition,
                expression.FalseExpr,
                this.Dispatch);

            return null;
        }

        public object Visit(LocalVariableReference expression)
        {
            if (expression.IsRef)
            {
                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.VariableAddressReference);
            }
            else
            {
                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.VariableReference);
            }

            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Value,
                expression.local_info,
                this.Dispatch);
            return null;
        }

        public object Visit(Mono.CSharp.ParameterReference expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ParameterReference);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Value,
                expression.Parameter,
                this.Dispatch);
            return null;
        }

        public object Visit(Invocation expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.Invocation);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Method,
                expression.MethodGroup,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.Arguments),
                this.Dispatch);
            return null;
        }

        public object Visit(New expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.New);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Value,
                expression.Constructor,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.Arguments),
                this.Dispatch);

            return null;
        }

        public object Visit(ArrayInitializer expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ArrayCreation expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ArrayCreation);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.ElementType,
                expression.ArrayElementType,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Initializer,
                expression.ResolvedInitializers,
                this.Dispatch);

            return null;
        }

        public object Visit(This expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ThisExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            return null;
        }

        public object Visit(ArglistAccess expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(RefValueExpr expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(RefTypeExpr expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(MakeRefExpr expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(TypeOf expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TypeOf);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.TypeArgument,
                MemberReferenceSerializer.Serialize);

            return null;
        }

        public object Visit(SizeOf expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(MemberAccess expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(CheckedExpr expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(UnCheckedExpr experssion)
        {
            throw new NotImplementedException();
        }

        public object Visit(ElementAccess expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ArrayElementAccess);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.LeftExpr,
                expression.Expr,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.Arguments),
                this.Dispatch);

            return null;
        }

        public object Visit(ArrayAccess expression)
        {
            return this.Visit(expression.ElementAccess);
        }

        public object Visit(BaseThis expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.BaseExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);

            return null;
        }

        public object Visit(EmptyExpression expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ErrorExpression expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ComposedTypeSpecifier expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ComposedCast expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ArrayIndexCast expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(StackAlloc expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ElementInitializer expression)
        {
            return this.Visit((Assign)expression);
        }

        public object Visit(CollectionOrObjectInitializers expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(NewInitialize expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.NewInitializer);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Value,
                expression.Constructor,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.Arguments),
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Initializer,
                expression.Initializers.Initializers,
                this.Dispatch);
            return null;
        }

        public object Visit(NewAnonymousType expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(AnonymousTypeParameter expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(DynamicResultCast expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(CompositeExpression expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(ConstantExpr expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(MemberExpr expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(MethodGroupExpr expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.MethodExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Value,
                expression.BestCandidate,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Instance,
                expression.InstanceExpression,
                this.Dispatch);

            return null;
        }

        public object Visit(FieldExpr expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.FieldExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Field,
                expression.Spec,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Instance,
                expression.InstanceExpression,
                this.Dispatch);

            return null;
        }

        public object Visit(PropertyExpr expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.PropertyExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Getter,
                expression.Getter,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Setter,
                expression.Setter,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Instance,
                expression.InstanceExpression,
                this.Dispatch);

            return null;
        }

        public object Visit(IndexerExpr expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.IndexerExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Getter,
                expression.Getter,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Setter,
                expression.Setter,
                MemberReferenceSerializer.Serialize);
            this.serializer.AddValue(
                NameTokens.Instance,
                expression.InstanceExpression,
                this.Dispatch);
            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.Arguments),
                this.Dispatch);

            return null;
        }

        public object Visit(EventExpr expression)
        { throw new NotImplementedException(); }

        public object Visit(ExpressionStatement expression)
        { throw new NotImplementedException(); }

        public object Visit(AsyncInitializer expression)
        { throw new NotImplementedException(); }

        public object Visit(DelegateInvocation expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DelegateInvocation);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Expr,
                expression.InstanceExpr,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.Arguments,
                this.EnumerateArgs(expression.Arguments),
                this.Dispatch);

            return null;
        }

        public object Visit(ConstructorInitializer expression)
        { throw new NotImplementedException(); }

        public object Visit(ConstructorThisInitializer expression)
        { throw new NotImplementedException(); }

        public object Visit(EmptyExpressionStatement expression)
        { throw new NotImplementedException(); }

        public object Visit(CompletingExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(CompletionSimpleName expression)
        { throw new NotImplementedException(); }

        public object Visit(CompletionElementInitializer expression)
        { throw new NotImplementedException(); }

        public object Visit(Await expression)
        { throw new NotImplementedException(); }

        public object Visit(DefaultParameterValueExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(Constant expression)
        { throw new NotImplementedException(); }

        public object Visit(StringConstant expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.StringLiteral);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.Value);
            return null;
        }

        public object Visit(ShimExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(AQueryClause expression)
        { throw new NotImplementedException(); }

        public object Visit(ARangeVariableQueryClause expression)
        { throw new NotImplementedException(); }

        public object Visit(DefaultValueExpression expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DefaultValue);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);
            return null;
        }

        public object Visit(PointerArithmetic expression)
        { throw new NotImplementedException(); }

        public object Visit(VariableReference expression)
        { throw new NotImplementedException(); }

        public object Visit(TemporaryVariableReference expression)
        {
            if (expression.IsRef)
            {
                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TempVariableAddressReference);
            }
            else
            {
                this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TempVariableReference);
            }

            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(NameTokens.Value, expression.LocalInfo, this.Dispatch);

            return null;
        }

        public object Visit(Arglist expression)
        { throw new NotImplementedException(); }

        public object Visit(ATypeNameExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(QualifiedAliasMember expression)
        { throw new NotImplementedException(); }

        public object Visit(TypeExpression expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TypeExpr);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);

            return null;
        }

        public object Visit(TypeParameterExpr expression)
        { throw new NotImplementedException(); }

        public object Visit(SpecialContraintExpr expression)
        { throw new NotImplementedException(); }

        public object Visit(Namespace expression)
        { throw new NotImplementedException(); }

        public object Visit(RootNamespace expression)
        { throw new NotImplementedException(); }

        public object Visit(GlobalRootNamespace expression)
        { throw new NotImplementedException(); }

        public object Visit(UserCast expression)
        { throw new NotImplementedException(); }

        public object Visit(TypeCast expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.TypeCast);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Expr,
                expression.Child,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);

            return null;
        }

        public object Visit(ConvCast expression)
        { return this.Visit((TypeCast)expression); }

        public object Visit(ReducedExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(AnonymousMethodExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(LambdaExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(RuntimeValueExpression expression)
        { throw new NotImplementedException(); }

        public object Visit(DelegateCreation expression)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.DelegateCreation);
            this.serializer.AddValue(NameTokens.Loc, expression.Location.GetStrLoc());
            this.serializer.AddValue(
                NameTokens.Method,
                expression.MethodGroup,
                this.Dispatch);

            this.serializer.AddValue(
                NameTokens.Type,
                expression.Type,
                MemberReferenceSerializer.Serialize);

            return null;
        }

        public object Visit(ImplicitDelegateCreation expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public object Visit(NewDelegate expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public object Visit(Statement statement)
        { throw new NotImplementedException(); }

        public object Visit(ResumableStatement statement)
        { throw new NotImplementedException(); }

        public object Visit(YieldStatement<AsyncInitializer> statement)
        { throw new NotImplementedException(); }

        public object Visit(ExceptionStatement statement)
        { throw new NotImplementedException(); }

        public object Visit(TryFinallyBlock statement)
        { throw new NotImplementedException(); }

        public object Visit(ExitStatement statement)
        { throw new NotImplementedException(); }

        public object Visit(ContextualReturn statement)
        { throw new NotImplementedException(); }

        private IEnumerable<Argument> EnumerateArgs(Arguments arguments)
        {
            for (int i = 0; i < arguments.Count; i++)
            {
                yield return arguments[i];
            }
        }

        private IEnumerable<Parameter> GetParameters(ParametersCompiled parametersCompiled)
        {
            for (int iParam = 0; iParam < parametersCompiled.Count; iParam++)
            {
                yield return parametersCompiled[iParam];
            }
        }

        private void Dispatch(ICustomSerializer serializer, Parameter parameter)
        {
            this.serializer.AddValue(NameTokens.TypeName, TypeTokens.ParameterType);
            ParameterAttributes attributes = ParameterAttributes.None;
            var modFlags = parameter.ModFlags;
            string paramName = parameter.Name;

            switch (parameter.ModFlags)
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

            this.serializer.AddValue(NameTokens.Name, parameter.Name);
            this.serializer.AddValue(NameTokens.ModFlags, attributes);
            this.serializer.AddValue(
                NameTokens.Type,
                parameter.Type,
                MemberReferenceSerializer.Serialize);
        }

        private void Dispatch(ICustomSerializer serializer, LocalVariable local)
        {
            serializer.AddValue(NameTokens.TypeName, TypeTokens.LocalVariable);
            serializer.AddValue(
                NameTokens.Name,
                local.Name);
            serializer.AddValue(
                NameTokens.Type,
                local.Type,
                MemberReferenceSerializer.Serialize);

            int blockId = -1;

            foreach (var node in this.scopeBlockStack)
            {
                if (node.Item2 == local.Block)
                {
                    blockId = node.Item1;
                    break;
                }
            }

            serializer.AddValue(
                NameTokens.Block,
                blockId);
        }

        private void Dispatch(ICustomSerializer serializer, Argument argument)
        {
            serializer.AddValue(NameTokens.TypeName, TypeTokens.Argument);
            serializer.AddValue(
                NameTokens.Value,
                argument.Expr,
                this.Dispatch);
        }

        private void Dispatch(ICustomSerializer serializer, Expression expression)
        {
            this.Dispatch(expression);
        }

        private void Dispatch(ICustomSerializer serializer, Statement expression)
        {
            this.Dispatch(expression);
        }

        private object Dispatch(Expression expression)
        {
            return this.dispatcher.Dispatch(expression);
        }

        public object Dispatch(Statement statement)
        {
            return this.dispatcher.Dispatch(statement);
        }
    }
}