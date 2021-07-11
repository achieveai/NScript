//-----------------------------------------------------------------------
// <copyright file="MonoAstVisitor.cs" company="">
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
    using JAst = NScript.CLR.AST;

    /// <summary>
    /// Definition for MonoAstVisitor
    /// </summary>
    public class MonoAstVisitor : IMonoAstVisitor<JAst.Expression>
    {
        ExpressionVisitDispatcher<JAst.Expression> dispatcher;
        Stack<string> prefixString = new Stack<string>();

        public MonoAstVisitor()
        {
            dispatcher = new ExpressionVisitDispatcher<JAst.Expression>(this);
            prefixString.Push(string.Empty);
        }

        public ExpressionVisitDispatcher<JAst.Expression> Dispatcher
        { get { return this.dispatcher; } }

        public JAst.Expression Visit(Expression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(NullLiteral expression)
        {
            this.PrintInfo("null");
            return null;
        }

        public JAst.Expression Visit(BoolLiteral expression)
        {
            this.PrintInfo("{0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(CharLiteral expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(IntLiteral expression)
        {
            this.PrintInfo("IntLiteral: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(UIntLiteral expression)
        {
            this.PrintInfo("UInt: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(LongLiteral expression)
        {
            this.PrintInfo("Long: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(ULongLiteral expression)
        {
            this.PrintInfo("ULong: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(FloatLiteral expression)
        {
            this.PrintInfo("Float: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(DoubleLiteral expression)
        {
            this.PrintInfo("Double: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(DecimalLiteral expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(StringLiteral expression)
        {
            this.PrintInfo("StringLiteral: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(LocalTemporary expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Assign expression)
        {
            this.PrintInfo("Assignment");
            this.PushIndent();
            this.PrintInfo("LHS: ");
            this.PushIndent();
            this.dispatcher.Dispatch(expression.Target);
            this.PopIndent();
            this.PrintInfo("RHS:");
            this.PushIndent();
            this.dispatcher.Dispatch(expression.Source);
            this.PopIndent();
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(SimpleAssign expression)
        {
            this.PrintInfo("Simple Assignment");
            this.PushIndent();
            this.PrintInfo("LHS: ");
            this.PushIndent();
            this.dispatcher.Dispatch(expression.Target);
            this.PopIndent();
            this.PrintInfo("RHS:");
            this.PushIndent();
            this.dispatcher.Dispatch(expression.Source);
            this.PopIndent();
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(RuntimeExplicitAssign expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(FieldInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(CompoundAssign expression)
        {
            this.PrintInfo("CompoundAssign, Op: {0}", (Binary.Operator)(expression.Operator & Binary.Operator.ValuesOnlyMask));
            this.PushIndent();
            this.PrintInfo("LHS:");
            this.PrintInfo(expression.Target);
            this.PrintInfo("RHS:");
            this.PrintInfo(expression.Source);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(CompoundAssign.TargetExpression expression)
        {
            this.PrintInfo("CompoundAssign.TargetExpession");
            this.PrintInfo(expression.child);

            return null;
        }

        public JAst.Expression Visit(EmptyCast expression)
        {
            // This is used when casting to base Types, in which case
            // there is no cast required.
            return this.dispatcher.Dispatch(expression.Child);
        }

        public JAst.Expression Visit(OperatorCast expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(BoxedCast expression)
        {
            this.PrintInfo("Box");
            this.PrintInfo(expression.Child);

            return null;
        }

        public JAst.Expression Visit(UnboxCast expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ClassCast expression)
        {
            this.PrintInfo("Cast to: {0}", expression.Type);
            this.PrintInfo(expression.Child);

            return null;
        }

        public JAst.Expression Visit(Cast expression)
        {
            this.PrintInfo("Cast");
            this.PrintInfo(expression.Expr);
            this.PrintInfo(expression.TargetType);

            return null;
        }

        public JAst.Expression Visit(ImplicitCast expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(BoolConstant expression)
        {
            this.PrintInfo("Bool const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(ByteConstant expression)
        {
            this.PrintInfo("Byte const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(CharConstant expression)
        {
            this.PrintInfo("CharConst const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(SByteConstant expression)
        {
            this.PrintInfo("SByte const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(ShortConstant expression)
        {
            this.PrintInfo("Short const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(UShortConstant expression)
        {
            this.PrintInfo("UShort const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(IntConstant expression)
        {
            this.PrintInfo("Int const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(UIntConstant expression)
        {
            this.PrintInfo("UIntConst: {0}", expression.Value);

            return null;
        }

        public JAst.Expression Visit(LongConstant expression)
        {
            this.PrintInfo("Long const: {0}", expression.Value);

            return null;
        }

        public JAst.Expression Visit(ULongConstant expression)
        {
            this.PrintInfo("ULong const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(FloatConstant expression)
        {
            this.PrintInfo("Float const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(DoubleConstant expression)
        {
            this.PrintInfo("Double const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(DecimalConstant expression)
        {
            this.PrintInfo("Decimal const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(StringConcat expression)
        {
            this.PrintInfo("StringConcat");
            this.PushIndent();
            foreach (var arg in expression.Arguments)
            {
                this.PrintInfo(arg.Expr);
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(NullConstant expression)
        {
            this.PrintInfo("Null const");
            return null;
        }

        public JAst.Expression Visit(EmptyConstantCast expression)
        {
            return this.dispatcher.Dispatch(expression.child);
        }

        public JAst.Expression Visit(EnumConstant expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(SideEffectConstant expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(QueryExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(QueryStartClause expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(GroupBy expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Join expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(GroupJoin expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Let expresstion)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Select expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(SelectMany expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Where expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(OrderByAscending expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(OrderByDescending expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ThenByAscending expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ThenByDescending expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(QueryBlock expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(EmptyStatement expression)
        {
            this.PrintInfo("empty statement");
            return null;
        }

        public JAst.Expression Visit(StatementExpression expression)
        {
            this.PrintInfo("StatementExpression");
            this.PushIndent();
            this.dispatcher.Dispatch(expression.Expr);
            this.PopIndent();
            return null;
        }

        public JAst.Expression Visit(StatementErrorExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(StatementList expression)
        {
            this.PrintInfo("StatementList");
            foreach (var statement in expression.Statements)
            {
                this.PrintInfo(statement);
            }

            return null;
        }

        public JAst.Expression Visit(Return expression)
        {
            this.PrintInfo("Return statement");
            this.PushIndent();
            if (expression.Expr != null)
            {
                this.Dispatcher.Dispatch(expression.Expr);
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Goto expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(GotoDefault expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(GotoCase expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(LabeledStatement expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Throw expression)
        {
            this.PrintInfo("Throw");
            this.PrintInfo(expression.Expr);

            return null;
        }

        public JAst.Expression Visit(Break expression)
        {
            this.PrintInfo("break;");
            return null;
        }

        public JAst.Expression Visit(Continue expression)
        {
            this.PrintInfo("continue;");

            return null;
        }

        public JAst.Expression Visit(BlockVariableDeclaration expression)
        {
            this.PrintInfo("Variable decleration, var:{0}", expression.Variable.Name);
            this.PushIndent();
            if (expression.Initializer != null)
            {
                this.dispatcher.Dispatch(expression.Initializer);
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(BlockConstantDeclaration expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(LocalVariable exprssion)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(If expression)
        {
            this.PrintInfo("If block");
            this.PushIndent();
            this.PrintInfo("Condition");
            this.PrintInfo(expression.Expr);
            this.PrintInfo("TrueBlock");
            this.PrintInfo(expression.TrueStatement);
            if (expression.FalseStatement != null)
            {
                this.PrintInfo("FalseBlock");
                this.PrintInfo(expression.FalseStatement);
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Do expression)
        {
            this.PrintInfo("Do-While Block");
            this.PushIndent();
            this.PrintInfo("Condition");
            this.PrintInfo(expression.expr);
            this.PrintInfo("Block");
            this.PrintInfo(expression.EmbeddedStatement);
            this.PopIndent();
            return null;
        }

        public JAst.Expression Visit(While expression)
        {
            this.PrintInfo("While Block");
            this.PushIndent();
            this.PrintInfo("Condition");
            this.PrintInfo(expression.expr);
            this.PrintInfo("Block");
            this.PrintInfo(expression.Statement);
            this.PopIndent();
            return null;
        }

        public JAst.Expression Visit(For expression)
        {
            this.PrintInfo("For Block");
            this.PushIndent();
            this.PrintInfo("Initializer:");
            this.PrintInfo(expression.Initializer);
            this.PrintInfo("Condition:");
            this.PrintInfo(expression.Condition);
            this.PrintInfo("Iterator:");
            this.PrintInfo(expression.Iterator);
            this.PrintInfo("Block");
            this.PrintInfo(expression.Statement);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Foreach expression)
        {
            this.PrintInfo("ForEach:");
            this.PushIndent();
            this.PrintInfo("varName: {0}", expression.Variable.Name);
            this.PrintInfo("Enumerable: ");
            this.PrintInfo(expression.Expr);
            this.PrintInfo("Loop Block:");
            this.PrintInfo(expression.Statement);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Foreach.ArrayForeach expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Foreach.CollectionForeach expression)
        {
            this.PrintInfo("Collection forEach");
            this.PushIndent();
            this.PrintInfo("Expression");
            this.PrintInfo(expression.Expr);
            this.PrintInfo("Temporary Enumerator store");
            this.PrintInfo(expression.EnumeratorVariable);
            this.PrintInfo("Body");
            this.PrintInfo(expression.Statement);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Switch expression)
        {
            this.PrintInfo("Switch");
            this.PushIndent();
            this.PrintInfo("Expression");
            this.PrintInfo(expression.Expr);
            foreach (var section in expression.Sections)
            {
                this.PrintInfo("Labels");
                foreach (var label in section.Labels)
                {
                    if (label.IsDefault)
                    {
                        this.PrintInfo("  Default:");
                    }
                    else
                    {
                        this.PrintInfo(label.Label);
                    }
                }
                this.PrintInfo("Section");
                this.PrintInfo(section.Block);
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Block expression)
        {
            this.PrintInfo("Block");
            foreach (var statement in expression.Statements)
            {
                this.PrintInfo(statement);
            }

            return null;
        }

        public JAst.Expression Visit(ExplicitBlock expression)
        {
            this.PrintInfo("Explicit Block:");
            foreach (var statement in expression.Statements)
            {
                this.PrintInfo(statement);
            }

            return null;
        }

        public JAst.Expression Visit(ParametersBlock expression)
        {
            this.PrintInfo("delegate (paramCount: {0})", expression.Parameters.Count);
            this.PushIndent();
            foreach (var statement in expression.Statements)
            {
                this.dispatcher.Dispatch(statement);
            }
            this.PopIndent();
            return null;
        }

        public JAst.Expression Visit(ToplevelBlock expression)
        {
            this.PrintInfo("TopLevelBlock");
            this.PushIndent();
            foreach (var statement in expression.Statements)
            {
                this.dispatcher.Dispatch(statement);
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Lock expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Unchecked expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Checked expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Unsafe expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Fixed expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Catch expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(TryFinally expression)
        {
            this.PrintInfo("TryFinally");
            this.PushIndent();
            this.PrintInfo("Try:");
            this.PrintInfo(expression.Statement);
            this.PrintInfo("Finally:");
            this.PrintInfo(expression.Finallyblock);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(TryCatch expression)
        {
            this.PrintInfo("TryCatch");
            this.PushIndent();
            this.PrintInfo("Try:");
            this.PrintInfo(expression.Block);
            foreach (var clause in expression.Clauses)
            {
                this.PrintInfo("Catch: {0}", clause.CatchType != null ? (object)clause.CatchType : "all");
                this.PrintInfo(clause.Block);
            }
            this.PopIndent();
            return null;
        }

        public JAst.Expression Visit(Using expression)
        {
            this.PrintInfo("Using expression");
            this.PushIndent();
            this.PrintInfo("Decleration");
            this.PrintInfo(expression.Variables);
            this.PrintInfo("Body");
            this.PrintInfo(expression.Statement);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(Using.VariableDeclaration expression)
        {
            this.PrintInfo("Using.VariableDeclaration");
            this.PushIndent();
            this.PrintInfo("Declaration");
            this.PrintInfo(expression.Initializer);
            this.PrintInfo("DisposeCall");
            this.PrintInfo(expression.DisposeCall);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(NullableType expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Unwrap expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(UnwrapCall expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Wrap expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(LiftedNull expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Lifted expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(LiftedUnaryOperator expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(LiftedBinaryOperator expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(NullCoalescingOperator expression)
        {
            this.PrintInfo("?? operator");
            this.PrintInfo(expression.LeftExpression);
            this.PrintInfo(expression.RightExpression);

            return null;
        }

        public JAst.Expression Visit(Yield expression)
        {
            this.PrintInfo("Yield");
            this.PrintInfo(expression.Expr);

            return null;
        }

        public JAst.Expression Visit(YieldBreak expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(StateMachine expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(StateMachineInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Iterator expression)
        {
            this.PrintInfo("Iterator");
            this.PushIndent();
            this.PrintInfo("Iterator Type:{0}", expression.Type);
            this.PrintInfo(expression.Block);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(AnonymousExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(AnonymousMethodBody expression)
        {
            this.PrintInfo("Anonymous method");
            this.PushIndent();
            if (expression.Parameters.Count > 0)
            {
                string paramStr = string.Empty;
                foreach (Parameter arg in expression.Parameters.FixedParameters)
                {
                    paramStr = paramStr.Length == 0 ? paramStr : paramStr + ", ";
                    paramStr += string.Format("{0} {1}", arg.Type, arg.Name);
                }

                this.PrintInfo("Parameters: ({0})", paramStr);
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(UserOperatorCall expression)
        {
            this.PrintInfo("Overloaded operator call: {0}", expression.oper);
            foreach (var arg in expression.arguments)
            {
                this.PrintInfo(arg.Expr);
            }

            return null;
        }

        public JAst.Expression Visit(ParenthesizedExpression expression)
        {
            this.PrintInfo("Paranthesis");
            this.PrintInfo(expression.Expr);
            return null;
        }

        public JAst.Expression Visit(Unary expression)
        {
            this.PrintInfo("Unary, Op: {0}", expression.Oper);
            this.PrintInfo(expression.Expr);
            return null;
        }

        public JAst.Expression Visit(Indirection expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(UnaryMutator expression)
        {
            this.PrintInfo("Unaryop, op: {0}", expression.UnaryMutatorMode);
            this.PrintInfo(expression.Expr);
            return null;
        }

        public JAst.Expression Visit(Binary expression)
        {
            this.PrintInfo("Binary Expression, op: {0}", expression.Oper);
            this.PrintInfo(expression.Left);
            this.PrintInfo(expression.Right);

            return null;
        }

        public JAst.Expression Visit(Is expression)
        {
            this.PrintInfo("Is");
            this.PrintInfo(expression.Expr);
            this.PrintInfo(expression.ProbeType);

            return null;
        }

        public JAst.Expression Visit(As expression)
        {
            this.PrintInfo("Is");
            this.PrintInfo(expression.Expr);
            this.PrintInfo(expression.ProbeType);

            return null;
        }

        public JAst.Expression Visit(ConditionalLogicalOperator expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(BooleanExpression expression)
        {
            this.PrintInfo("Boolean expression");
            this.PrintInfo(expression.Expr);
            return null;
        }

        public JAst.Expression Visit(BooleanExpressionFalse expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Conditional expression)
        {
            this.PrintInfo("Conditional: ");
            this.PushIndent();
            this.PrintInfo("Condition:");
            this.PrintInfo(expression.Expr);
            this.PrintInfo("TrueExpr:");
            this.PrintInfo(expression.TrueExpr);
            this.PrintInfo("FalseExpr:");
            this.PrintInfo(expression.FalseExpr);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(LocalVariableReference expression)
        {
            this.PrintInfo("LocalVariable reference: {0}", expression.Name);
            return null;
        }

        public JAst.Expression Visit(ParameterReference expression)
        {
            this.PrintInfo("Parameter reference: {0}", expression.Name);
            return null;
        }

        public JAst.Expression Visit(Invocation expression)
        {
            this.PrintInfo("Invocation");
            this.PushIndent();

            this.PrintInfo("Args:");
            if (expression.Arguments != null)
            {
                foreach (var arg in expression.Arguments)
                {
                    this.PrintInfo(arg.Expr);
                }
            }

            this.PrintInfo("Inoke:");
            this.PrintInfo(expression.Exp);

            this.PopIndent();
            return null;
        }

        public JAst.Expression Visit(New expression)
        {
            this.PrintInfo("New");
            this.PushIndent();
            this.PrintInfo("Constructor: {0}", expression.Constructor);
            if (expression.Arguments != null)
            {
                this.PrintInfo("Arguments");
                foreach (var arg in expression.Arguments)
                {
                    this.PrintInfo(arg.Expr);
                }
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(ArrayInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ArrayCreation expression)
        {
            this.PrintInfo("ArrayCreation:");
            this.PushIndent();
            this.PrintInfo("ArrayType:");
            this.PrintInfo(expression.TypeExpression);
            if (expression.Initializers != null && expression.Initializers.Count > 0)
            {
                this.PrintInfo("elements");
                foreach (var elem in expression.Initializers.Elements)
                {
                    this.PrintInfo(elem);
                }
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(This expression)
        {
            this.PrintInfo("this");
            return null;
        }

        public JAst.Expression Visit(ArglistAccess expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(RefValueExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(RefTypeExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(MakeRefExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(TypeOf expression)
        {
            this.PrintInfo("TypeOf");
            this.PrintInfo(expression.TypeExpression);

            return null;
        }

        public JAst.Expression Visit(SizeOf expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(CheckedExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(UnCheckedExpr experssion)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ElementAccess expression)
        {
            this.PrintInfo("Array access:");
            this.PushIndent();
            this.PrintInfo("Array");
            this.PrintInfo(expression.Expr);
            this.PrintInfo("Index:");
            this.PrintInfo(expression.Arguments[0].Expr);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(ArrayAccess expression)
        {
            return this.Visit(expression.ElementAccess);
        }

        public JAst.Expression Visit(BaseThis expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(EmptyExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ErrorExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ComposedTypeSpecifier expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ComposedCast expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ArrayIndexCast expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(StackAlloc expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ElementInitializer expression)
        {
            this.PrintInfo("Element Initializer");
            this.PushIndent();
            this.PrintInfo("Property or Field: {0}", expression.Name);
            this.PrintInfo(expression.Source);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(CollectionOrObjectInitializers expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(NewInitialize expression)
        {
            this.PrintInfo("New Initialize");
            this.PushIndent();
            this.PrintInfo("Constructor: {0}", expression.Constructor);
            if (expression.Arguments != null)
            {
                this.PrintInfo("Arguments");
                foreach (var arg in expression.Arguments)
                {
                    this.PrintInfo(arg.Expr);
                }
            }
            if (expression.Initializers != null)
            {
                this.PrintInfo("Initializers");
                foreach (var init in expression.Initializers.Initializers)
                {
                    this.PrintInfo(init);
                }
            }
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(NewAnonymousType expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(AnonymousTypeParameter expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicResultCast expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(CompositeExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ConstantExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(MemberExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(MethodGroupExpr expression)
        {
            this.PrintInfo("MethodInfo: {0}", expression.BestCandidate);
            return null;
        }

        public JAst.Expression Visit(FieldExpr expression)
        {
            this.PrintInfo("Field accessor: {0}", expression.Spec);
            if (expression.IsInstance)
            {
                this.PrintInfo(expression.InstanceExpression);
            }

            return null;
        }

        public JAst.Expression Visit(PropertyExpr expression)
        {
            this.PrintInfo("Property Expression: {0}", expression.Getter != null ? expression.Getter : expression.Setter);
            if (expression.IsInstance)
            {
                this.PrintInfo(expression.InstanceExpression);
            }

            return null;
        }

        public JAst.Expression Visit(IndexerExpr expression)
        {
            this.PrintInfo("Indexer {0} access", expression.Getter != null ? "get" : "set");
            this.PushIndent();
            if (expression.IsInstance)
            {
                this.PrintInfo("Instance:");
                this.PrintInfo(expression.InstanceExpression);
            }

            this.PrintInfo("Accessor {0}", expression.Getter ?? expression.Setter);

            this.PrintInfo("Arguments");
            foreach (var arg in expression.Arguments)
            {
                this.PrintInfo(arg.Expr);
            }

            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(EventExpr expression)
        {
            this.PrintInfo("Event access: {0}", expression.Operator);
            if (expression.IsInstance)
            {
                this.PrintInfo(expression.InstanceExpression);
            }

            return null;
        }

        public JAst.Expression Visit(ExpressionStatement expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicExpressionStatement expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicEventCompoundAssign expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicConversion expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicConstructorBinder expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicIndexBinder expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicInvocation expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicMemberBinder expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DynamicUnaryConversion expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DelegateInvocation expression)
        {
            this.PrintInfo("Delegate invocation");
            this.PushIndent();
            if (expression.Arguments != null)
            {
                this.PrintInfo("Arguments");
                foreach (var arg in expression.Arguments)
                {
                    this.PrintInfo(arg.Expr);
                }
            }

            if (expression.InstanceExpr != null)
            {
                this.PrintInfo("InstanceInfo");
                this.PrintInfo(expression.InstanceExpr);
            }

            this.PrintInfo("MethodInfo: {0}", expression.Method);
            this.PopIndent();
            return null;
        }

        public JAst.Expression Visit(AsyncInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ConstructorInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ConstructorThisInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(EmptyExpressionStatement expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(CompletingExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(CompletionSimpleName expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(CompletionElementInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Await expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DefaultParameterValueExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Constant expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(StringConstant expression)
        {
            this.PrintInfo("String const: {0}", expression.Value);
            return null;
        }

        public JAst.Expression Visit(ShimExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(AQueryClause expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ARangeVariableQueryClause expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DefaultValueExpression expression)
        {
            this.PrintInfo("DefaultValue: ");
            this.PrintInfo(expression.Expr);

            return null;
        }

        public JAst.Expression Visit(PointerArithmetic expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(VariableReference expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(TemporaryVariableReference expression)
        {
            this.PrintInfo("TempVariableReference {0}", expression.LocalInfo.Name);
            return null;
        }

        public JAst.Expression Visit(Arglist expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ATypeNameExpression expression)
        {
            this.PrintInfo("Name: {0}", expression.Name);
            return null;
        }

        public JAst.Expression Visit(QualifiedAliasMember expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(TypeExpression expression)
        {
            this.PrintInfo("Type: {0}", expression.Type);
            return null;
        }

        public JAst.Expression Visit(TypeParameterExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(SpecialContraintExpr expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(Namespace expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(RootNamespace expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(GlobalRootNamespace expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(UserCast expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(TypeCast expression)
        {
            this.PrintInfo("Type Cast (Number conversion?): {0}", expression.Type);
            this.PrintInfo(expression.Child);

            return null;
        }

        public JAst.Expression Visit(ConvCast expression)
        {
            this.PrintInfo("Number conversion cast to: {0}", expression.Type);
            this.PrintInfo(expression.Child);

            return null;
        }

        public JAst.Expression Visit(ReducedExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(AnonymousMethodExpression expression)
        {
            this.PrintInfo("Anonymous method");
            this.PrintInfo(expression.Block);
            return null;
        }

        public JAst.Expression Visit(LambdaExpression expression)
        {
            this.PrintInfo("Lembda expressions");
            this.PushIndent();
            if (expression.Parameters != null && expression.Parameters.Count > 0)
            {
                string paramStr = string.Empty;
                foreach (Parameter arg in expression.Parameters.FixedParameters)
                {
                    paramStr = paramStr.Length == 0 ? paramStr : paramStr + ", ";
                    paramStr += arg.Name;
                }
                this.PrintInfo("Parameters: {0}", paramStr);
            }
            this.PrintInfo(expression.Block);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(RuntimeValueExpression expression)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(DelegateCreation expression)
        {
            this.PrintInfo("Delegate Creation, delegateCtor: {0}", expression.DelegateConstructor);
            this.PushIndent();
            if (expression.MethodGroup.IsInstance)
            {
                this.PrintInfo("Instance");
                this.PrintInfo(expression.MethodGroup.InstanceExpression);
            }

            this.PrintInfo("Method spec: {0}", expression.MethodGroup.BestCandidate);
            this.PopIndent();

            return null;
        }

        public JAst.Expression Visit(ImplicitDelegateCreation expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public JAst.Expression Visit(NewDelegate expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public JAst.Expression Visit(Statement statement)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ResumableStatement statement)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(YieldStatement<AsyncInitializer> statement)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ExceptionStatement statement)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(TryFinallyBlock statement)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ExitStatement statement)
        {
            throw new NotImplementedException();
        }

        public JAst.Expression Visit(ContextualReturn statement)
        {
            throw new NotImplementedException();
        }

        private void PushIndent()
        {
            this.prefixString.Push(this.prefixString.Peek() + "  ");
        }

        private void PopIndent()
        {
            this.prefixString.Pop();
        }

        private void PrintInfo(string format, params object[] args)
        {
            Console.Write(this.prefixString.Peek());
            Console.WriteLine(format, args);
        }

        private void PrintInfo(Expression expr)
        {
            this.PushIndent();
            this.dispatcher.Dispatch(expr);
            this.PopIndent();
        }

        private void PrintInfo(Statement expr)
        {
            this.PushIndent();
            this.dispatcher.Dispatch(expr);
            this.PopIndent();
        }

        #region IMonoAstVisitor<Expression> Members

        public JAst.Expression Visit(MemberAccess expression)
        {
            throw new NotImplementedException();
        }

        #endregion IMonoAstVisitor<Expression> Members

    }
}