//-----------------------------------------------------------------------
// <copyright file="AstToJObject.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

/*
namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Mono.CSharp;
    using Mono.CSharp.Linq;
    using Mono.CSharp.Nullable;
    using Newtonsoft.Json.Linq;
    using System.Linq;
    using Ast = NScript.CLR.AST;

    /// <summary>
    /// Definition for SerializeAst
    /// </summary>
    public class AstToJObject : IMonoAstVisitor<JObject>
    {
        private ExpressionVisitDispatcher<JObject> dispatcher;

        private int id = 0;

        private LinkedList<Tuple<int, ExplicitBlock>> scopeBlockStack = new LinkedList<Tuple<int, ExplicitBlock>>();

        private LinkedList<Tuple<int, Ast.ParameterBlock>> parameterBlockStack = new LinkedList<Tuple<int, Ast.ParameterBlock>>();

        public AstToJObject()
        {
            this.dispatcher = new ExpressionVisitDispatcher<JObject>(this);
        }

        public static Ast.BinaryOperator? GetOperator(Binary.Operator binaryOperator)
        {
            switch (binaryOperator)
            {
                case Binary.Operator.Division:
                    return Ast.BinaryOperator.Div;
                case Binary.Operator.Multiply:
                    return Ast.BinaryOperator.Mul;
                case Binary.Operator.Addition:
                    return Ast.BinaryOperator.Plus;
                case Binary.Operator.Subtraction:
                    return Ast.BinaryOperator.Minus;
                case Binary.Operator.LeftShift:
                    return Ast.BinaryOperator.LeftShift;
                case Binary.Operator.RightShift:
                    return Ast.BinaryOperator.RightShift;
                case Binary.Operator.BitwiseAnd:
                    return Ast.BinaryOperator.BitwiseAnd;
                case Binary.Operator.BitwiseOr:
                    return Ast.BinaryOperator.BitwiseOr;
                case Binary.Operator.ExclusiveOr:
                    return Ast.BinaryOperator.BitwiseXor;
                case Binary.Operator.Equality:
                    return Ast.BinaryOperator.Equals;
                case Binary.Operator.GreaterThan:
                    return Ast.BinaryOperator.GreaterThan;
                case Binary.Operator.GreaterThanOrEqual:
                    return Ast.BinaryOperator.GreaterThanOrEqual;
                case Binary.Operator.Inequality:
                    return Ast.BinaryOperator.NotEquals;
                case Binary.Operator.LessThan:
                    return Ast.BinaryOperator.LessThan;
                case Binary.Operator.LessThanOrEqual:
                    return Ast.BinaryOperator.LessThanOrEqual;
                case Binary.Operator.LogicalAnd:
                    return Ast.BinaryOperator.LogicalAnd;
                case Binary.Operator.LogicalOr:
                    return Ast.BinaryOperator.LogicalOr;
                case Binary.Operator.Modulus:
                    return Ast.BinaryOperator.Mod;
            }

            return null;
        }

        public static Ast.UnaryOperator? GetOperator(Unary.Operator unaryOperator)
        {
            switch (unaryOperator)
            {
                case Unary.Operator.AddressOf:
                    return null;
                case Unary.Operator.LogicalNot:
                    return Ast.UnaryOperator.LogicalNot;
                case Unary.Operator.OnesComplement:
                    return Ast.UnaryOperator.BitwiseNot;
                case Unary.Operator.TOP:
                    return null;
                case Unary.Operator.UnaryNegation:
                    return Ast.UnaryOperator.UnaryMinus;
                case Unary.Operator.UnaryPlus:
                    return Ast.UnaryOperator.UnaryPlus;
                default:
                    return null; ;
            }
        }

        public JObject SerializeMethodBody(
            Constructor constructor,
            IEnumerable<FieldInitializer> fields,
            ToplevelBlock rootBlock)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.MethodBody;
            rv[NameTokens.Method] = MemberReferenceSerializer.Serialize(constructor.Spec);
            if (rootBlock != null)
            {
                rv[NameTokens.FileName] = rootBlock.Location.NameFullPath;
            }
            else if (fields.FirstOrDefault() != null)
            {
                rv[NameTokens.FileName] = fields.First().Location.NameFullPath;
            }

            rv[NameTokens.Block] = this.Visit(
                rootBlock,
                constructor.Initializer,
                fields);
            return rv;
        }

        public JObject SerializeMethodBody(
            AbstractPropertyEventMethod propertyMethod,
            ToplevelBlock rootBlock)
        {
            JObject rv = new JObject();

            rv[NameTokens.TypeName] = TypeTokens.MethodBody;
            rv[NameTokens.Method] = MemberReferenceSerializer.Serialize(propertyMethod.Spec);
            rv[NameTokens.FileName] = rootBlock != null
                    ? rootBlock.Location.NameFullPath
                    : null;
            rv[NameTokens.Block] =
                rootBlock != null
                    ? this.Visit(rootBlock)
                    : null;

            return rv;
        }

        public JObject SerializeMethodBody(
            Method method,
            ToplevelBlock rootBlock)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.MethodBody;
            rv[NameTokens.Method] = MemberReferenceSerializer.Serialize(method);
            rv[NameTokens.FileName] = rootBlock.Location.NameFullPath;
            rv[NameTokens.Block] = this.Dispatch(rootBlock);
            return rv;
        }

        public JObject Visit(Expression expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(NullLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.NullLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            return rv;
        }

        public JObject Visit(BoolLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.BoolLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(CharLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.CharLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(IntLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.IntLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(UIntLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.UIntLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(LongLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.LongLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(ULongLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ULongLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(FloatLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.FloatLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(DoubleLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DoubleLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(DecimalLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DecimalLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(StringLiteral expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.StringLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(LocalTemporary expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Assign expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.Assignment;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.LeftExpr] = this.Dispatch(expression.Target);
            rv[NameTokens.RightExpr] = this.Dispatch(expression.Source);
            return rv;
        }

        public JObject Visit(SimpleAssign expression)
        { return this.Visit((Assign)expression); }

        public JObject Visit(RuntimeExplicitAssign expression)
        { throw new NotImplementedException(); }

        public JObject Visit(FieldInitializer expression)
        { throw new NotImplementedException(); }

        public JObject Visit(CompoundAssign expression)
        {
            JObject rv = new JObject();
            if (expression.Target is EventExpr)
            {
                EventExpr evtExpr = expression.Target as EventExpr;

                rv[NameTokens.TypeName] = TypeTokens.MethodCall;
                rv[NameTokens.Loc] = expression.GetStrLoc();
                rv[NameTokens.Method] = MemberReferenceSerializer.Serialize(evtExpr.Operator);
                rv[NameTokens.Instance] = this.Dispatch(evtExpr.InstanceExpression);
                rv[NameTokens.Arguments] = this.Dispatch(new Expression[] { expression.Source });

                return rv;
            }

            Ast.BinaryOperator? op = AstToJObject.GetOperator(expression.Operator);
            if (!op.HasValue)
            {
                throw new InvalidOperationException();
            }

            switch (op.Value)
            {
                case NScript.CLR.AST.BinaryOperator.Assignment:
                    break;
                case NScript.CLR.AST.BinaryOperator.BitwiseAnd:
                    op = NScript.CLR.AST.BinaryOperator.BitwiseAndAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.BitwiseOr:
                    op = NScript.CLR.AST.BinaryOperator.BitwiseOrAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.BitwiseXor:
                    op = NScript.CLR.AST.BinaryOperator.BitwiseXorAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.Div:
                    op = NScript.CLR.AST.BinaryOperator.DivAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.LeftShift:
                    op = NScript.CLR.AST.BinaryOperator.LeftShiftAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.Minus:
                    op = NScript.CLR.AST.BinaryOperator.MinusAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.Mod:
                    op = NScript.CLR.AST.BinaryOperator.ModAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.Plus:
                    op = NScript.CLR.AST.BinaryOperator.PlusAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.RightShift:
                    op = NScript.CLR.AST.BinaryOperator.RightShiftAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.Mul:
                    op = NScript.CLR.AST.BinaryOperator.MulAssignment;
                    break;
                case NScript.CLR.AST.BinaryOperator.UnsignedRightShift:
                    op = NScript.CLR.AST.BinaryOperator.UnsignedRightShiftAssignment;
                    break;
                default:
                    throw new NotImplementedException();
            }

            rv[NameTokens.TypeName] = TypeTokens.BinaryExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Operator] = op.Value.ToString();
            rv[NameTokens.LeftExpr] = this.Dispatch(expression.Left);
            rv[NameTokens.RightExpr] = this.Dispatch(expression.Right);

            return rv;
        }

        public JObject Visit(CompoundAssign.TargetExpression expression)
        { return this.Dispatch(expression.child); }

        public JObject Visit(EmptyCast expression)
        {
            return this.Visit((TypeCast)expression);
        }

        public JObject Visit(OperatorCast expression)
        { throw new NotImplementedException(); }

        public JObject Visit(BoxedCast expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.BoxExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.Child);

            return rv;
        }

        public JObject Visit(UnboxCast expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.UnBoxExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);
            rv[NameTokens.Expr] = this.Dispatch(expression.Child);

            return rv;
        }

        public JObject Visit(ClassCast expression)
        {
            return this.Visit((TypeCast)expression);
        }

        public JObject Visit(Cast expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TypeCast;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = this.Dispatch(expression.TargetType);
            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);

            return rv;
        }

        public JObject Visit(ImplicitCast expression)
        {
            throw new InvalidOperationException();
        }

        public JObject Visit(BoolConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.BoolLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(ByteConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ByteLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = (int)expression.Value;
            return rv;
        }

        public JObject Visit(CharConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.CharLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(SByteConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.SByteLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(ShortConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ShortLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(UShortConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.UShortConstant;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(IntConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.IntLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(UIntConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.UIntLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(LongConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.LongLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(ULongConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ULongLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(FloatConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.FloatLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(DoubleConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DoubleLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(DecimalConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DecimalLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(StringConcat expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.StrCatExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Method] = MemberReferenceSerializer.Serialize(expression.Method);
            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments);

            return rv;
        }

        public JObject Visit(NullConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.NullLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            return rv;
        }

        public JObject Visit(EmptyConstantCast expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TypeCast;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.child);
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);

            return rv;
        }

        public JObject Visit(EnumConstant expression)
        {
            return this.Dispatch(expression.Child);
        }

        public JObject Visit(SideEffectConstant expression)
        {
            return this.Dispatch(expression.value);
        }

        public JObject Visit(QueryExpression expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(QueryStartClause expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(GroupBy expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(Join expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(GroupJoin expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(Let expresstion)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(Select expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(SelectMany expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(Where expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(OrderByAscending expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(OrderByDescending expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ThenByAscending expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ThenByDescending expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(QueryBlock expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(EmptyStatement expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.EmptyStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            return rv;
        }

        public JObject Visit(StatementExpression expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.StatementExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);
            return rv;
        }

        public JObject Visit(StatementErrorExpression expression)
        {
            JObject rv = new JObject();
            return rv;
        }

        public JObject Visit(StatementList expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.StatementList;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = this.Dispatch(expression.Statements);
            return rv;
        }

        public JObject Visit(Return expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ReturnStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = this.Dispatch(expression.Expr);
            return rv;
        }

        public JObject Visit(Goto expression)
        {
            JObject rv = new JObject();
            return rv;
        }

        public JObject Visit(GotoDefault expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(GotoCase expression)
        {
            JObject rv = new JObject();
            return rv;
        }

        public JObject Visit(LabeledStatement expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(Throw expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ThrowStatment;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = this.Dispatch(expression.Expr);
            return rv;
        }

        public JObject Visit(Break expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.BreakExpression;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            return rv;
        }

        public JObject Visit(Continue expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ContinueExpression;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            return rv;
        }

        public JObject Visit(BlockVariableDeclaration expression)
        {
            JObject rv = new JObject();
            if (expression.Initializer != null)
            {
                List<Expression> expressions = new List<Expression>();
                expressions.Add(expression.Initializer);
                if (expression.Declarators != null)
                {
                    foreach (var decl in expression.Declarators)
                    {
                        if (decl.Initializer != null)
                        {
                            expressions.Add(decl.Initializer);
                        }
                    }
                }

                rv[NameTokens.TypeName] = TypeTokens.VariableInitializer;
                rv[NameTokens.Loc] = expression.GetStrLoc();
                rv[NameTokens.Value] = this.Dispatch(expressions);
            }
            else
            {
                rv[NameTokens.TypeName] = TypeTokens.EmptyStatement;
                rv[NameTokens.Loc] = expression.GetStrLoc();
            }

            return rv;
        }

        public JObject Visit(BlockConstantDeclaration expression)
        {
            return this.Visit((BlockVariableDeclaration)expression);
        }

        public JObject Visit(LocalVariable exprssion)
        { throw new NotImplementedException(); }

        public JObject Visit(If expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.IfStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Condition] = this.Dispatch(expression.Expr);
            rv[NameTokens.TrueStatement] = this.Dispatch(expression.TrueStatement);
            rv[NameTokens.FalseCondition] = this.Dispatch(expression.FalseStatement);

            return rv;
        }

        public JObject Visit(Do expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DoWhileStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Condition] = this.Dispatch(expression.expr);

            rv[NameTokens.Loop] = this.Dispatch(expression.EmbeddedStatement);
            return rv;
        }

        public JObject Visit(While expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.WhileStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Condition] = this.Dispatch(expression.expr);

            rv[NameTokens.Loop] = this.Dispatch(expression.Statement);
            return rv;
        }

        public JObject Visit(For expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ForStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Initializer] = this.Dispatch(expression.Initializer);
            rv[NameTokens.Condition] = this.Dispatch(expression.Condition);
            rv[NameTokens.Iterator] = this.Dispatch(expression.Iterator);

            rv[NameTokens.Loop] = this.Dispatch(expression.Statement);
            return rv;
        }

        public JObject Visit(Foreach expression)
        {
            return this.Dispatch(expression.Statement);
        }

        public JObject Visit(Foreach.ArrayForeach expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ForEachStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Iterator] = this.Dispatch(expression.ForEach.Expr);
            rv[NameTokens.LocalVariable] = expression.Variable.local_info.Name;
            rv[NameTokens.Loop] = this.Dispatch(expression.ForEach.Body);
            return rv;
        }

        public JObject Visit(Foreach.CollectionForeach expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ForEachStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Iterator] = this.Dispatch(expression.Expr);
            rv[NameTokens.LocalVariable] = expression.variable.Name;
            // rv[NameTokens.Loop] = this.Dispatch(((While)((Using)expression.Statement).Statement).Statement);
            rv[NameTokens.Loop] = this.Dispatch(expression.ForEach.Body);

            return rv;
        }

        public JObject Visit(Switch expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.SwitchStatement;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);
            JArray jarray = new JArray();

            foreach (var section in expression.Sections)
            {
                JObject sectObj = new JObject();
                sectObj[NameTokens.TypeName] = TypeTokens.SwitchSection;

                JArray labelArr = new JArray();
                foreach (var label in section.Labels)
                {
                    JObject labelObj = new JObject();
                    if (label.IsDefault)
                    {
                        labelObj[NameTokens.TypeName] = TypeTokens.Default;
                    }
                    else
                    {
                        labelObj[NameTokens.TypeName] = TypeTokens.LabelLiteral;
                        labelObj[NameTokens.Value] = this.Dispatch(label.Converted ?? label.Label);
                    }

                    labelArr.Add(labelObj);
                }

                sectObj[NameTokens.Labels] = labelArr;
                sectObj[NameTokens.Block] = this.Dispatch(section.Block);
                jarray.Add(sectObj);
            }

            rv[NameTokens.Blocks] = jarray;
            return rv;
        }

        public JObject Visit(Block expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.StatementList;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = this.Dispatch(expression.Statements);
            return rv;
        }

        public JObject Visit(ExplicitBlock expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ScopeBlock;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Id] = ++this.id;
            this.scopeBlockStack.AddFirst(Tuple.Create(this.id, expression));
            rv[NameTokens.Value] = this.Dispatch(expression.Statements);
            this.scopeBlockStack.RemoveFirst();

            return rv;
        }

        public JObject Visit(
            ParametersBlock expression,
            ConstructorInitializer initializer,
            IEnumerable<FieldInitializer> fields = null)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ParameterBlock;
            JArray jArray = null;
            rv[NameTokens.Id] = ++this.id;

            // Expression can be null for static field initializer constructors.
            if (expression != null)
            {
                rv[NameTokens.IsMethodOwned] = expression is ToplevelBlock;
                rv[NameTokens.Loc] = expression.GetStrLoc();
                rv[NameTokens.Parameters] = this.GetParameters(expression.Parameters);

                this.scopeBlockStack.AddFirst(Tuple.Create(this.id, (ExplicitBlock)expression));
                jArray = this.Dispatch(expression.Statements);
            }
            else
            {
                jArray = new JArray();
                rv[NameTokens.Loc] = fields.FirstOrDefault().GetStrLoc();
                rv[NameTokens.Parameters] = new JArray();
            }

            int insertOffset = 0;
            if (initializer != null
                && initializer.BaseConstructor != null)
            {
                jArray.Insert(insertOffset++, this.Visit(initializer));
            }

            if (initializer != null
                && (initializer is ConstructorBaseInitializer)
                && fields != null)
            {
                foreach (var fieldInitializer in fields)
                {
                    if (fieldInitializer != null
                        && !fieldInitializer.IsDefaultInitializer)
                    {
                        JObject fieldInit = this.Visit((Assign)fieldInitializer);
                        jArray.Insert(insertOffset++, fieldInit);
                    }
                }
            }

            rv[NameTokens.Value] = jArray;
            if (expression != null)
            {
                this.scopeBlockStack.RemoveFirst();
            }

            return rv;
        }

        public JObject Visit(ParametersBlock expression)
        {
            return this.Visit(expression, null);
        }

        public JObject Visit(ToplevelBlock expression)
        {
            return this.Visit((ParametersBlock)expression);
        }

        public JObject Visit(Lock expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Unchecked expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Checked expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Unsafe expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Fixed expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Catch expression)
        { throw new NotImplementedException(); }

        public JObject Visit(TryFinally expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TryFinally;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.TryBlock] = this.Dispatch(expression.Statement);
            rv[NameTokens.FinallyBlock] = this.Dispatch(expression.Finallyblock);

            return rv;
        }

        public JObject Visit(TryCatch expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TryCatch;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.TryBlock] = this.Dispatch(expression.Block);

            JArray jarray = new JArray();
            foreach (var catchBlock in expression.Clauses)
            {
                JObject rv1 = new JObject();
                rv1[NameTokens.TypeName] = TypeTokens.CatchBlock;
                rv1[NameTokens.Loc] = catchBlock.GetStrLoc();
                rv1[NameTokens.Type] = MemberReferenceSerializer.Serialize(catchBlock.CatchType);
                rv1[NameTokens.Block] = this.Dispatch(catchBlock.Block);

                this.scopeBlockStack.AddFirst(
                    Tuple.Create(
                        rv1.Value<JObject>(NameTokens.Block).Value<int>(NameTokens.Id),
                        (ExplicitBlock)catchBlock.Block));
                rv1[NameTokens.LocalVariable] = this.Dispatch(catchBlock.Variable);

                this.scopeBlockStack.RemoveFirst();
                jarray.Add(rv1);
            }

            rv[NameTokens.CatchBlocks] = jarray;
            return rv;
        }

        public JObject Visit(Using expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Using.VariableDeclaration expression)
        { throw new NotImplementedException(); }

        public JObject Visit(NullableType expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Unwrap expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.UnwrapFromNullable;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.Original);

            return rv;
        }

        public JObject Visit(UnwrapCall expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Wrap expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.WrapToNullable;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.Child);

            return rv;
        }

        public JObject Visit(LiftedNull expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DefaultValue;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);

            return rv;
        }

        public JObject Visit(Lifted expression)
        {
            return this.Dispatch(expression.InnerExpression);
        }

        public JObject Visit(LiftedUnaryOperator expression)
        {
            JObject rv = this.Visit((Unary)expression);
            rv[NameTokens.IsLifted] = true;

            return rv;
        }

        public JObject Visit(LiftedBinaryOperator expression)
        {
            JObject rv = this.Visit((Binary)expression);
            rv[NameTokens.IsLifted] = true;

            return rv;
        }

        public JObject Visit(NullCoalescingOperator expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.NullCoalascing;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.LeftExpr] = this.Dispatch(expression.LeftExpression);
            rv[NameTokens.RightExpr] = this.Dispatch(expression.RightExpression);
            return rv;
        }

        public JObject Visit(Yield expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.Yield;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);
            return rv;
        }

        public JObject Visit(YieldBreak expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.Yield;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = null;
            return rv;
        }

        public JObject Visit(StateMachine expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(StateMachineInitializer expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(Iterator expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.Iterator;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);
            rv[NameTokens.Block] = this.Dispatch(expression.Block);

            return rv;
        }

        public JObject Visit(AnonymousExpression expression)
        { throw new NotImplementedException(); }

        public JObject Visit(AnonymousMethodBody expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.AnonymousMethod;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);
            rv[NameTokens.Block] = this.Dispatch(expression.Block);

            return rv;
        }

        public JObject Visit(UserOperatorCall expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.MethodCall;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Method] = MemberReferenceSerializer.Serialize(expression.oper);
            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.arguments);
            return rv;
        }

        public JObject Visit(ParenthesizedExpression expression)
        { return this.Dispatch(expression.Expr); }

        public JObject Visit(Unary expression)
        {
            Ast.UnaryOperator? oper = AstToJObject.GetOperator(expression.Oper);
            if (!oper.HasValue)
            {
                throw new InvalidOperationException();
            }

            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.UnaryExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Operator] = oper.Value.ToString();
            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);

            return rv;
        }

        public JObject Visit(Indirection expression)
        { throw new NotImplementedException(); }

        public JObject Visit(UnaryMutator expression)
        {
            Ast.UnaryOperator oper = Ast.UnaryOperator.Void;
            switch (expression.UnaryMutatorMode)
            {
                case UnaryMutator.Mode.PostDecrement:
                    oper = Ast.UnaryOperator.PostDecrement;
                    break;
                case UnaryMutator.Mode.PostIncrement:
                    oper = Ast.UnaryOperator.PostIncrement;
                    break;
                case UnaryMutator.Mode.PreDecrement:
                    oper = Ast.UnaryOperator.PreDecrement;
                    break;
                case UnaryMutator.Mode.PreIncrement:
                    oper = Ast.UnaryOperator.PreIncrement;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.UnaryExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Operator] = oper.ToString();
            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);

            return rv;
        }

        public JObject Visit(Binary expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.BinaryExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Operator] = AstToJObject.GetOperator(expression.Oper).Value.ToString();
            rv[NameTokens.LeftExpr] = this.Dispatch(expression.Left);

            rv[NameTokens.RightExpr] = this.Dispatch(expression.Right);

            return rv;
        }

        public JObject Visit(Is expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.IsExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.ProbeType.Type);

            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);

            return rv;
        }

        public JObject Visit(As expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.AsExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.ProbeType.Type);

            rv[NameTokens.Expr] = this.Dispatch(expression.Expr);

            return rv;
        }

        public JObject Visit(ConditionalLogicalOperator expression)
        { throw new NotImplementedException(); }

        public JObject Visit(BooleanExpression expression)
        { return this.Dispatch(expression.Expr); }

        public JObject Visit(BooleanExpressionFalse expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Conditional expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.Conditional;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Condition] = this.Dispatch(expression.Expr);
            rv[NameTokens.TrueStatement] = this.Dispatch(expression.TrueExpr);
            rv[NameTokens.FalseCondition] = this.Dispatch(expression.FalseExpr);

            return rv;
        }

        public JObject Visit(LocalVariableReference expression)
        {
            JObject rv = new JObject();
            if (expression.IsRef)
            {
                rv[NameTokens.TypeName] = TypeTokens.VariableAddressReference;
            }
            else
            {
                rv[NameTokens.TypeName] = TypeTokens.VariableReference;
            }

            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = this.Dispatch(expression.local_info);
            return rv;
        }

        public JObject Visit(Mono.CSharp.ParameterReference expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ParameterReference;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = this.Dispatch(expression.Parameter);
            return rv;
        }

        public JObject Visit(Invocation expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.Invocation;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Method] = this.Dispatch(expression.MethodGroup);
            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments);
            return rv;
        }

        public JObject Visit(New expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.New;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = MemberReferenceSerializer.Serialize(expression.Constructor);
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);
            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments);

            return rv;
        }

        public JObject Visit(ArrayInitializer expression)
        { throw new NotImplementedException(); }

        public JObject Visit(ArrayCreation expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ArrayCreation;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);
            rv[NameTokens.ElementType] = MemberReferenceSerializer.Serialize(expression.ArrayElementType);
            rv[NameTokens.Initializer] = this.Dispatch(expression.ResolvedInitializers);
            rv[NameTokens.Arguments] = this.Dispatch(expression.Arguments);

            return rv;
        }

        public JObject Visit(This expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ThisExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            return rv;
        }

        public JObject Visit(ArglistAccess expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(RefValueExpr expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(RefTypeExpr expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(MakeRefExpr expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(TypeOf expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TypeOf;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.TypeArgument);

            return rv;
        }

        public JObject Visit(SizeOf expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(MemberAccess expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(CheckedExpr expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(UnCheckedExpr experssion)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ElementAccess expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ArrayElementAccess;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.LeftExpr] = this.Dispatch(expression.Expr);
            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments);

            return rv;
        }

        public JObject Visit(ArrayAccess expression)
        {
            return this.Visit(expression.ElementAccess);
        }

        public JObject Visit(BaseThis expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.BaseExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);

            return rv;
        }

        public JObject Visit(EmptyExpression expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ErrorExpression expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ComposedTypeSpecifier expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ComposedCast expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ArrayIndexCast expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(StackAlloc expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ElementInitializer expression)
        {
            return this.Visit((Assign)expression);
        }

        public JObject Visit(CollectionOrObjectInitializers expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(NewInitialize expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.NewInitializer;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = MemberReferenceSerializer.Serialize(expression.Constructor);
            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments);

            JArray initializerArray = null;
            if (expression.Initializers != null)
            {
                initializerArray = new JArray();

                foreach (var init in expression.Initializers.Initializers)
                {
                    JObject initializerObj = new JObject();

                    ElementInitializer initializer = init as ElementInitializer;
                    if (initializer != null)
                    {
                        PropertyExpr propertyExpr = initializer.Target as PropertyExpr;
                        if (propertyExpr != null)
                        {
                            initializerObj[NameTokens.ElementType] = TypeTokens.PropertySpec;
                            initializerObj[NameTokens.Setter] = MemberReferenceSerializer.Serialize(
                                propertyExpr.Setter);
                        }
                        else
                        {
                            FieldExpr fieldExpr = (FieldExpr)initializer.Target;
                            initializerObj[NameTokens.ElementType] = TypeTokens.FieldSpec;
                            initializerObj[NameTokens.Field] = MemberReferenceSerializer.Serialize(
                                fieldExpr.Spec);
                        }

                        initializerObj[NameTokens.Loc] = init.GetStrLoc();
                        initializerObj[NameTokens.Value] =
                            this.Dispatch(initializer.Source);
                    }
                    else
                    {
                        Invocation invocation = (Invocation)init;
                        initializerObj[NameTokens.ElementType] = TypeTokens.MethodSpec;
                        JArray args = new JArray();
                        initializerObj[NameTokens.Arguments] = this.EnumerateArgs(invocation.Arguments);
                        initializerObj[NameTokens.Method] = MemberReferenceSerializer.Serialize(invocation.MethodGroup.BestCandidate);
                    }

                    initializerArray.Add(initializerObj);
                }

                rv[NameTokens.Initializer] = initializerArray;
            }

            return rv;
        }

        public JObject Visit(NewAnonymousType expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.NewAnonymousType;
            rv[NameTokens.Loc] = expression.GetStrLoc();

            JArray jArray = new JArray();
            foreach (var parameter in expression.Parameters)
            {
                JObject jo = new JObject();
                jo[NameTokens.Name] = parameter.Name;
                jo[NameTokens.Value] = this.Dispatch(parameter.Expr);

                jArray.Add(jo);
            }

            rv[NameTokens.Setter] = jArray;
            return rv;
        }

        public JObject Visit(AnonymousTypeParameter expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(DynamicResultCast expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(CompositeExpression expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(ConstantExpr expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(MemberExpr expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(MethodGroupExpr expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.MethodExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = MemberReferenceSerializer.Serialize(expression.BestCandidate);
            rv[NameTokens.Instance] = this.Dispatch(expression.InstanceExpression);

            return rv;
        }

        public JObject Visit(FieldExpr expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.FieldExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Field] = MemberReferenceSerializer.Serialize(expression.Spec);
            rv[NameTokens.Instance] = this.Dispatch(expression.InstanceExpression);

            return rv;
        }

        public JObject Visit(PropertyExpr expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.PropertyExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Getter] = MemberReferenceSerializer.Serialize(expression.Getter);
            rv[NameTokens.Setter] = MemberReferenceSerializer.Serialize(expression.Setter);
            rv[NameTokens.Instance] = this.Dispatch(expression.InstanceExpression);

            return rv;
        }

        public JObject Visit(IndexerExpr expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.IndexerExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Getter] = MemberReferenceSerializer.Serialize(expression.Getter);
            rv[NameTokens.Setter] = MemberReferenceSerializer.Serialize(expression.Setter);
            rv[NameTokens.Instance] = this.Dispatch(expression.InstanceExpression);
            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments, expression.Setter != null ? 1 : 0);

            return rv;
        }

        public JObject Visit(EventExpr expression)
        { throw new NotImplementedException(); }

        public JObject Visit(ExpressionStatement expression)
        { throw new NotImplementedException(); }

        public JObject Visit(DynamicExpressionStatement expression)
        {
            return this.Dispatch(expression.Binder);
        }

        public JObject Visit(DynamicEventCompoundAssign expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(DynamicConversion expression)
        {
            return this.Dispatch(expression.BinderExpression);
        }

        public JObject Visit(DynamicConstructorBinder expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(DynamicIndexBinder expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DynamicIndexBinder;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Instance] = this.Dispatch(expression.Arguments[0].Expr);
            rv[NameTokens.Index] = this.Dispatch(expression.Arguments[1].Expr);

            return rv;
        }

        public JObject Visit(DynamicInvocation expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(DynamicMemberBinder expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DynamicMemberBinder;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Name] = expression.Name;
            rv[NameTokens.Instance] = this.Dispatch(expression.Arguments[0].Expr);
            rv[NameTokens.Value] = this.Dispatch(expression.Setter);

            return rv;
        }

        public JObject Visit(DynamicUnaryConversion expression)
        {
            throw new NotImplementedException();
        }

        public JObject Visit(AsyncInitializer expression)
        { throw new NotImplementedException(); }

        public JObject Visit(DelegateInvocation expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DelegateInvocation;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.InstanceExpr);

            rv[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments);

            return rv;
        }

        public JObject Visit(ConstructorInitializer expression)
        {
            JObject thisObject = new JObject();
            thisObject[NameTokens.TypeName] = TypeTokens.ThisExpr;
            thisObject[NameTokens.Loc] = expression.GetStrLoc();

            JObject ctorCall = new JObject();
            ctorCall[NameTokens.TypeName] = TypeTokens.MethodCall;
            ctorCall[NameTokens.Loc] = expression.GetStrLoc();
            ctorCall[NameTokens.Method] = MemberReferenceSerializer.Serialize(
                expression.BaseConstructor);
            ctorCall[NameTokens.Instance] = thisObject;
            ctorCall[NameTokens.Arguments] = this.EnumerateArgs(expression.Arguments);

            JObject rv = new JObject();

            rv[NameTokens.TypeName] = TypeTokens.StatementExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = ctorCall;

            return rv;
        }

        public JObject Visit(ConstructorThisInitializer expression)
        { throw new NotImplementedException(); }

        public JObject Visit(EmptyExpressionStatement expression)
        { throw new NotImplementedException(); }

        public JObject Visit(CompletingExpression expression)
        { throw new NotImplementedException(); }

        public JObject Visit(CompletionSimpleName expression)
        { throw new NotImplementedException(); }

        public JObject Visit(CompletionElementInitializer expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Await expression)
        { throw new NotImplementedException(); }

        public JObject Visit(DefaultParameterValueExpression expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Constant expression)
        { throw new NotImplementedException(); }

        public JObject Visit(StringConstant expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.StringLiteral;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = expression.Value;
            return rv;
        }

        public JObject Visit(ShimExpression expression)
        { throw new NotImplementedException(); }

        public JObject Visit(AQueryClause expression)
        { throw new NotImplementedException(); }

        public JObject Visit(ARangeVariableQueryClause expression)
        { throw new NotImplementedException(); }

        public JObject Visit(DefaultValueExpression expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DefaultValue;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);
            return rv;
        }

        public JObject Visit(PointerArithmetic expression)
        { throw new NotImplementedException(); }

        public JObject Visit(VariableReference expression)
        { throw new NotImplementedException(); }

        public JObject Visit(TemporaryVariableReference expression)
        {
            JObject rv = new JObject();
            if (expression.IsRef)
            {
                rv[NameTokens.TypeName] = TypeTokens.TempVariableAddressReference;
            }
            else
            {
                rv[NameTokens.TypeName] = TypeTokens.TempVariableReference;
            }

            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Value] = this.Dispatch(expression.LocalInfo);
            return rv;
        }

        public JObject Visit(Arglist expression)
        { throw new NotImplementedException(); }

        public JObject Visit(ATypeNameExpression expression)
        { throw new NotImplementedException(); }

        public JObject Visit(QualifiedAliasMember expression)
        { throw new NotImplementedException(); }

        public JObject Visit(TypeExpression expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TypeExpr;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);

            return rv;
        }

        public JObject Visit(TypeParameterExpr expression)
        { throw new NotImplementedException(); }

        public JObject Visit(SpecialContraintExpr expression)
        { throw new NotImplementedException(); }

        public JObject Visit(Namespace expression)
        { throw new NotImplementedException(); }

        public JObject Visit(RootNamespace expression)
        { throw new NotImplementedException(); }

        public JObject Visit(GlobalRootNamespace expression)
        { throw new NotImplementedException(); }

        public JObject Visit(UserCast expression)
        {
            // throw new NotImplementedException();
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.MethodCall;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Method] = MemberReferenceSerializer.Serialize(expression.Method);
            JArray jArray = new JArray();
            jArray.Add(this.Dispatch(expression.Source));
            rv[NameTokens.Arguments] = jArray;

            return rv;
        }

        public JObject Visit(TypeCast expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.TypeCast;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Expr] = this.Dispatch(expression.Child);

            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);

            return rv;
        }

        public JObject Visit(ConvCast expression)
        { return this.Visit((TypeCast)expression); }

        public JObject Visit(ReducedExpression expression)
        { return this.Dispatch(expression.Expression); }

        public JObject Visit(AnonymousMethodExpression expression)
        {
            // This is pretty much like AnonymousMethodBody
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.AnonymousMethod;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Type] = null;
            rv[NameTokens.Block] = this.Dispatch(expression.Block);

            return rv;
        }

        public JObject Visit(LambdaExpression expression)
        { return this.Visit((AnonymousMethodExpression)expression); }

        public JObject Visit(RuntimeValueExpression expression)
        { throw new NotImplementedException(); }

        public JObject Visit(DelegateCreation expression)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.DelegateCreation;
            rv[NameTokens.Loc] = expression.GetStrLoc();
            rv[NameTokens.Method] = this.Dispatch(expression.MethodGroup);

            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(expression.Type);

            return rv;
        }

        public JObject Visit(ImplicitDelegateCreation expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public JObject Visit(NewDelegate expression)
        {
            return this.Visit((DelegateCreation)expression);
        }

        public JObject Visit(Statement statement)
        { throw new NotImplementedException(); }

        public JObject Visit(ResumableStatement statement)
        { throw new NotImplementedException(); }

        public JObject Visit(YieldStatement<AsyncInitializer> statement)
        { throw new NotImplementedException(); }

        public JObject Visit(ExceptionStatement statement)
        { throw new NotImplementedException(); }

        public JObject Visit(TryFinallyBlock statement)
        { throw new NotImplementedException(); }

        public JObject Visit(ExitStatement statement)
        { throw new NotImplementedException(); }

        public JObject Visit(ContextualReturn statement)
        {
            return this.Visit((Return)statement);
        }

        private JArray EnumerateArgs(Arguments arguments, int skipLast = 0)
        {
            JArray rv = new JArray();
            for (int i = 0; arguments != null && i < arguments.Count - skipLast; i++)
            {
                rv.Add(this.Dispatch(arguments[i]));
            }

            return rv;
        }

        private JArray GetParameters(ParametersCompiled parametersCompiled)
        {
            JArray rv = new JArray();
            for (int iParam = 0; iParam < parametersCompiled.Count; iParam++)
            {
                rv.Add(this.Dispatch(parametersCompiled[iParam]));
            }

            return rv;
        }

        private JObject Dispatch(Parameter parameter)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.ParameterType;

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

            rv[NameTokens.Name] = parameter.Name;
            rv[NameTokens.ModFlags] = attributes.ToString();
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(parameter.Type);

            return rv;
        }

        private JObject Dispatch(LocalVariable local)
        {
            if (local == null)
            {
                return null;
            }

            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.LocalVariable;
            rv[NameTokens.Name] = local.Name;
            rv[NameTokens.Type] = MemberReferenceSerializer.Serialize(local.Type);

            int blockId = -1;

            foreach (var node in this.scopeBlockStack)
            {
                if (node.Item2 == local.Block)
                {
                    blockId = node.Item1;
                    break;
                }
            }

            rv[NameTokens.Block] = blockId;

            if (blockId == -1)
            {
                throw new InvalidOperationException();
            }

            return rv;
        }

        private JObject Dispatch(Argument argument)
        {
            JObject rv = new JObject();
            rv[NameTokens.TypeName] = TypeTokens.Argument;
            rv[NameTokens.IsByRef] = argument.IsByRef;
            rv[NameTokens.Value] = this.Dispatch(argument.Expr);
            return rv;
        }

        private JArray Dispatch(IEnumerable<Statement> coll)
        {
            JArray rv = new JArray();
            foreach (var stmt in coll)
            { rv.Add(this.Dispatch(stmt)); }

            return rv;
        }

        private JArray Dispatch(IEnumerable<Expression> coll)
        {
            if (coll == null)
            {
                return null;
            }

            JArray rv = new JArray();
            foreach (var expr in coll)
            {
                rv.Add(this.Dispatch(expr));
            }

            return rv;
        }

        private JObject Dispatch(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            return this.dispatcher.Dispatch(expression);
        }

        private JObject Dispatch(Statement statement)
        {
            if (statement == null)
            {
                return null;
            }

            return this.dispatcher.Dispatch(statement);
        }
    }
}
*/