//-----------------------------------------------------------------------
// <copyright file="AstToSerialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using JsCsc.Lib.Serialization;
    using Mono.Cecil;
    using Mono.CSharp;
    using Mono.CSharp.Linq;
    using Mono.CSharp.Nullable;
    using Newtonsoft.Json.Linq;
    using System.Linq;
    using Ast = NScript.CLR.AST;

    /// <summary>
    /// Definition for AstToSerialization
    /// </summary>
    public class AstToSerialization : IMonoAstVisitor<AstBase>
    {
        private ExpressionVisitDispatcher<AstBase> dispatcher;

        private int id = 0;

        private LinkedList<Tuple<int, ExplicitBlock>> scopeBlockStack = new LinkedList<Tuple<int, ExplicitBlock>>();

        private LinkedList<Tuple<int, Ast.ParameterBlock>> parameterBlockStack = new LinkedList<Tuple<int, Ast.ParameterBlock>>();

        private Dictionary<TypeSpec, int> typeDict = new Dictionary<TypeSpec, int>();
        private Dictionary<MethodSpec, int> methodDict = new Dictionary<MethodSpec, int>();
        private Dictionary<FieldSpec, int> fieldDict = new Dictionary<FieldSpec, int>();
        public TypeInfoSer TypeSerializationInfo;

        public AstToSerialization()
        {
            this.dispatcher = new ExpressionVisitDispatcher<AstBase>(this);
            this.TypeSerializationInfo = new TypeInfoSer()
            {
                Methods = new Dictionary<int,MethodSpecSer>(),
                Fields = new Dictionary<int,FieldSpecSer>(),
                Types = new Dictionary<int,TypeSpecSer>()
            };
        }

        public MethodBody SerializeMethodBody(
            Constructor constructor,
            IEnumerable<FieldInitializer> fields,
            ToplevelBlock rootBlock)
        {
            var methodBlock = new MethodBody
            {
                FileName = rootBlock != null
                    ? rootBlock.Location.NameFullPath
                    : fields.First().Location.NameFullPath,
                MethodId = this.GetMethodSpecId(constructor.Spec),
                Body = new Bond.Bonded<ParameterBlock>(
                    this.Visit(rootBlock, constructor.Initializer, fields))
            };

            return methodBlock;
        }

        public MethodBody SerializeMethodBody(
            AbstractPropertyEventMethod propertyMethod,
            ToplevelBlock rootBlock)
        {
            return new MethodBody
            {
                MethodId = this.GetMethodSpecId(propertyMethod.Spec),
                FileName =  rootBlock != null
                    ? rootBlock.Location.NameFullPath
                    : null,
                Body = rootBlock != null
                    ? new Bond.Bonded<ParameterBlock>(
                        this.Visit(rootBlock, null, null))
                    : null
            };
        }

        public MethodBody SerializeMethodBody(
            Method method,
            ToplevelBlock rootBlock)
        {
            return new MethodBody
            {
                MethodId = this.GetMethodSpecId(method.Spec),
                FileName =  rootBlock != null
                    ? rootBlock.Location.NameFullPath
                    : null,
                Body = rootBlock != null
                    ? new Bond.Bonded<ParameterBlock>(
                        this.Visit(rootBlock, null, null))
                    : null
            };
        }

        public AstBase Visit(Expression expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(NullLiteral expression)
        {
            return new NullExpression
                { Location = expression.GetSerLoc() };
        }

        public AstBase Visit(BoolLiteral expression)
        {
            return new LiteralExpression<bool>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(CharLiteral expression)
        {
            return new LiteralExpression<char>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(IntLiteral expression)
        {
            return new LiteralExpression<int>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(UIntLiteral expression)
        {
            return new LiteralExpression<uint>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(LongLiteral expression)
        {
            return new LiteralExpression<long>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(ULongLiteral expression)
        {
            return new LiteralExpression<ulong>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(FloatLiteral expression)
        {
            return new LiteralExpression<float>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(DoubleLiteral expression)
        {
            return new LiteralExpression<double>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(DecimalLiteral expression)
        {
            return new LiteralExpression<decimal>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(StringLiteral expression)
        {
            return new LiteralExpression<string>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(LocalTemporary expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Assign expression)
        {
            return new AssignExpression
            {
                Location = expression.GetSerLoc(),
                Left = this.Dispatch(expression.Target),
                Right = this.Dispatch(expression.Source)
            };
        }

        public AstBase Visit(SimpleAssign expression)
        { return this.Visit((Assign)expression); }

        public AstBase Visit(RuntimeExplicitAssign expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(FieldInitializer expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(CompoundAssign expression)
        {
            if (expression.Target is EventExpr)
            {
                var evtExpr = expression.Target as EventExpr;
                return new MethodCallExpression
                {
                    Location = expression.GetSerLoc(),
                    Method = this.GetMethodSpecId(evtExpr.Operator),
                    Instance = this.Dispatch(evtExpr.InstanceExpression),
                    Arguments = new List<MethodCallArg>
                    {
                        new MethodCallArg{
                            IsByRef = false,
                            Value = this.Dispatch(expression.Source)
                        }
                    }
                };
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

            return new CompoundAssignExpression
            {
                Location = expression.GetSerLoc(),
                Operator = (int)op.Value,
                Left = this.Dispatch(expression.Left),
                Right = this.Dispatch(expression.Right)
            };
        }

        public AstBase Visit(CompoundAssign.TargetExpression expression)
        { return this.Dispatch(expression.child); }

        public AstBase Visit(EmptyCast expression)
        { return this.Visit((TypeCast)expression); }

        public AstBase Visit(OperatorCast expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(BoxedCast expression)
        {
            return new BoxCastExpression
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.Child)
            };
        }

        public AstBase Visit(UnboxCast expression)
        {
            return new TypeCastExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type),
                Expression = this.Dispatch(expression.Child),
                IsUnbox = true
            };
        }

        public AstBase Visit(ClassCast expression)
        {
            return this.Visit((TypeCast)expression);
        }

        public AstBase Visit(Cast expression)
        {
            return new TypeCastExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.TargetType.Type),
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(ImplicitCast expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(BoolConstant expression)
        {
            return new ConstantExpression<bool>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(ByteConstant expression)
        {
            return new ConstantExpression<byte>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(CharConstant expression)
        {
            return new ConstantExpression<char>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(SByteConstant expression)
        {
            return new ConstantExpression<sbyte>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(ShortConstant expression)
        {
            return new ConstantExpression<short>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(UShortConstant expression)
        {
            return new ConstantExpression<ushort>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(IntConstant expression)
        {
            return new ConstantExpression<int>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(UIntConstant expression)
        {
            return new ConstantExpression<uint>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(LongConstant expression)
        {
            return new ConstantExpression<long>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(ULongConstant expression)
        {
            return new ConstantExpression<ulong>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(FloatConstant expression)
        {
            return new ConstantExpression<float>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(DoubleConstant expression)
        {
            return new ConstantExpression<double>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(DecimalConstant expression)
        {
            return new ConstantExpression<decimal>
            {
                Location = expression.GetSerLoc(),
                Value = expression.Value
            };
        }

        public AstBase Visit(StringConcat expression)
        {
            return new StrCatExpression
            {
                Location = expression.GetSerLoc(),
                Method = this.GetMethodSpecId(expression.Method),
                Arguments = this.EnumerateArgs(expression.Arguments)
            };
        }

        public AstBase Visit(NullConstant expression)
        {
            return new NullConstantExpression { Location = expression.GetSerLoc() };
        }

        public AstBase Visit(EmptyConstantCast expression)
        {
            return new TypeCastExpression
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.child),
                Type = this.GetTypeSpecId(expression.Type)
            };
        }

        public AstBase Visit(EnumConstant expression)
        { return this.Dispatch(expression.Child); }

        public AstBase Visit(SideEffectConstant expression)
        { return this.Visit(expression.value); }

        public AstBase Visit(QueryExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(QueryStartClause expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(GroupBy expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Join expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(GroupJoin expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(Let expresstion)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(Select expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(SelectMany expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(Where expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(OrderByAscending expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(OrderByDescending expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(ThenByAscending expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(ThenByDescending expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(QueryBlock expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(Mono.CSharp.EmptyStatement expression)
        { return new EmptyStatementSer { Location = expression.GetSerLoc() }; }

        public AstBase Visit(Mono.CSharp.StatementExpression expression)
        {
            return new StatementExpressionSer
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(StatementErrorExpression expression)
        { return null; }

        public AstBase Visit(Mono.CSharp.StatementList expression)
        {
            return new StatementListSer
            {
                Location = expression.GetSerLoc(),
                Statements = expression.Statements.Select(s => this.Dispatch(s)).ToList()
            };
        }

        public AstBase Visit(Return expression)
        {
            return new ReturnStatement
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(Goto expression)
        {
            return null;
        }

        public AstBase Visit(GotoDefault expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(GotoCase expression)
        {
            return null;
        }

        public AstBase Visit(LabeledStatement expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(Throw expression)
        {
            return new ThrowExpression
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(Break expression)
        { return new BreakStatement { Location = expression.GetSerLoc() }; }

        public AstBase Visit(Continue expression)
        { return new ContinueStatement { Location = expression.GetSerLoc() }; }

        public AstBase Visit(BlockVariableDeclaration expression)
        {
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

                return new VariableBlockDeclaration
                {
                    Location = expression.GetSerLoc(),
                    Initializers = expressions.Select(e => this.Dispatch(e)).ToList()
                };
            }
            else
            {
                return new VariableBlockDeclaration
                {
                    Location = expression.GetSerLoc(),
                };
            }
        }

        public AstBase Visit(BlockConstantDeclaration expression)
        {
            return this.Visit((BlockVariableDeclaration)expression);
        }

        public AstBase Visit(Mono.CSharp.LocalVariable exprssion)
        { throw new NotImplementedException(); }

        public AstBase Visit(If expression)
        {
            return new IfStatement
            {
                Location = expression.GetSerLoc(),
                Condition = this.Dispatch(expression.Expr),
                TrueStatement = this.Dispatch(expression.TrueStatement),
                FalseStatement = this.Dispatch(expression.FalseStatement)
            };
        }

        public AstBase Visit(Do expression)
        {
            return new DoStatement
            {
                Location = expression.GetSerLoc(),
                Condition = this.Dispatch(expression.expr),
                Loop = this.Dispatch(expression.EmbeddedStatement)
            };
        }

        public AstBase Visit(While expression)
        {
            return new WhileStatement
            {
                Location = expression.GetSerLoc(),
                Condition = this.Dispatch(expression.expr),
                Loop = this.Dispatch(expression.Statement)
            };
        }

        public AstBase Visit(For expression)
        {
            return new ForStatement
            {
                Location = expression.GetSerLoc(),
                Initializer = this.Dispatch(expression.Initializer),
                Condition = this.Dispatch(expression.Condition),
                Iterator = this.Dispatch(expression.Iterator),
                Loop = this.Dispatch(expression.Statement)
            };
        }

        public AstBase Visit(Foreach expression)
        { return this.Dispatch(expression.Statement); }

        public AstBase Visit(Foreach.ArrayForeach expression)
        {
            return new ForEachStatement
            {
                Location = expression.GetSerLoc(),
                LocalVariableName = expression.Variable.local_info.Name,
                Collection = this.Dispatch(expression.ForEach.Expr),
                Loop = this.Dispatch(expression.ForEach.Body),
            };
        }

        public AstBase Visit(Foreach.CollectionForeach expression)
        {
            return new ForEachStatement
            {
                Location = expression.GetSerLoc(),
                LocalVariableName = expression.variable.Name,
                Collection = this.Dispatch(expression.ForEach.Expr),
                Loop = this.Dispatch(expression.ForEach.Body),
            };
        }

        public AstBase Visit(Switch expression)
        {
            var switchSections = new List<SwitchSectionSer>();
            foreach (var section in expression.Sections)
            {
                JObject sectObj = new JObject();
                sectObj[NameTokens.TypeName] = TypeTokens.SwitchSection;

                var caseLabels = new List<SwitchCaseLabel>();
                foreach (var label in section.Labels)
                {
                    if (label.IsDefault)
                    { caseLabels.Add(new SwitchCaseLabel()); }
                    else
                    {
                        caseLabels.Add(
                            new SwitchCaseLabel
                            { LabelValue = this.Dispatch(label.Converted ?? label.Label) });
                    }
                }

                switchSections.Add(
                    new SwitchSectionSer
                    {
                        Labels = caseLabels,
                        Block = this.Dispatch(section.Block)
                    });
            }

            return new SwitchStatement
                {
                    Blocks = switchSections,
                    Location = expression.GetSerLoc(),
                    SwitchExpression = this.Dispatch(expression.Expr)
                };
        }

        public AstBase Visit(Mono.CSharp.Block expression)
        {
            return new BlockSer
            {
                Location = expression.GetSerLoc(),
                Statements = this.Dispatch(expression.Statements)
            };
        }

        public AstBase Visit(ExplicitBlock expression)
        {
            var rv = new ExplicitBlockSer
            {
                Id = ++this.id,
                Location = expression.GetSerLoc()
            };

            this.scopeBlockStack.AddFirst(Tuple.Create(this.id, expression));
            rv.Statements = this.Dispatch(expression.Statements);
            this.scopeBlockStack.RemoveFirst();

            return rv;
        }

        public ParameterBlock Visit(
            ParametersBlock expression,
            ConstructorInitializer initializer,
            IEnumerable<FieldInitializer> fields = null)
        {
            var rv = new ParameterBlock
            {
                Location = expression != null ? expression.GetSerLoc() : null,
                Id = ++this.id
            };

            // Expression can be null for static field initializer constructors.
            if (expression != null)
            {
                rv.IsMethodOwned = expression is ToplevelBlock;
                rv.Parameters = this.GetParameters(expression.Parameters);

                this.scopeBlockStack.AddFirst(Tuple.Create(this.id, (ExplicitBlock)expression));
                rv.Statements = this.Dispatch(expression.Statements);
            }
            else
            {
                rv.Location = fields.FirstOrDefault().GetSerLoc();
                rv.Parameters = new List<ParameterSer>();
                rv.Statements = new List<StatementSer>();
            }

            int insertOffset = 0;
            if (initializer != null
                && initializer.BaseConstructor != null)
            {
                rv.Statements.Insert(
                    insertOffset++,
                    new StatementExpressionSer
                    { Expression = (ExpressionSer)this.Visit(initializer) });
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
                        rv.Statements.Insert(
                            insertOffset++,
                            new StatementExpressionSer
                            { Expression = (ExpressionSer)this.Visit((Assign)fieldInitializer), });
                    }
                }
            }

            if (expression != null)
            { this.scopeBlockStack.RemoveFirst(); }

            return rv;
        }

        public AstBase Visit(ParametersBlock expression)
        { return this.Visit(expression, null); }

        public AstBase Visit(ToplevelBlock expression)
        { return this.Visit((ParametersBlock)expression); }

        public AstBase Visit(Lock expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Unchecked expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Checked expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Unsafe expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Fixed expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Catch expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(TryFinally expression)
        {
            return new TryFinallyBlockSer
            {
                Location = expression.GetSerLoc(),
                TryBlock = this.Dispatch(expression.Statement),
                FinallyBlock = this.Dispatch(expression.Finallyblock)
            };
        }

        public AstBase Visit(TryCatch expression)
        {
            var rv = new TryCatchBlock
            {
                Location = expression.GetSerLoc(),
                TryBlock = this.Dispatch(expression.Block),
                CatchBlocks = new List<CatchBlock>()
            };

            foreach (var catchBlock in expression.Clauses)
            {
                var catchBlockSer = new CatchBlock
                {
                    Location = catchBlock.GetSerLoc(),
                    Block = (ExplicitBlockSer)this.Dispatch(catchBlock.Block),
                };

                this.scopeBlockStack.AddFirst(
                    Tuple.Create(
                        catchBlockSer.Block.Id,
                        (ExplicitBlock)catchBlock.Block));
                catchBlockSer.LocalVariable = this.Dispatch(catchBlock.Variable);

                this.scopeBlockStack.RemoveFirst();
                rv.CatchBlocks.Add(catchBlockSer);
            }

            return rv;
        }

        public AstBase Visit(Using expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Using.VariableDeclaration expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(NullableType expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Unwrap expression)
        {
            return new UnwrapExpression
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.Original)
            };
        }

        public AstBase Visit(UnwrapCall expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Wrap expression)
        {
            return new WrapExpression
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.Child)
            };
        }

        public AstBase Visit(LiftedNull expression)
        {
            return new LiftedNullExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type)
            };
        }

        public AstBase Visit(Lifted expression)
        { return this.Dispatch(expression.InnerExpression); }

        public AstBase Visit(LiftedUnaryOperator expression)
        {
            var rv = (UnaryExpression)this.Visit((Unary)expression);
            rv.IsLifted = true;
            return rv;
        }

        public AstBase Visit(LiftedBinaryOperator expression)
        {
            var rv = (BinaryExpression)this.Visit((Binary)expression);
            rv.IsLifted = true;
            return rv;
        }

        public AstBase Visit(Mono.CSharp.Nullable.NullCoalescingOperator expression)
        {
            return new NullCoalescingOperatorSer
            {
                Location = expression.GetSerLoc(),
                Left = this.Dispatch(expression.LeftExpression),
                Right = this.Dispatch(expression.RightExpression)
            };
        }

        public AstBase Visit(Yield expression)
        {
            return new YieldStatement
            {
                Location = expression.GetSerLoc(),
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(YieldBreak expression)
        {
            return new YieldBreakStatement
            {
                Location = expression.GetSerLoc(),
            };
        }

        public AstBase Visit(StateMachine expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(StateMachineInitializer expression)
        {
            throw new NotImplementedException();
        }

        public AstBase Visit(Iterator expression)
        {
            return new IteratorBlock
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type),
                Block = (ParameterBlock)this.Visit(expression.Block)
            };
        }

        public AstBase Visit(AnonymousExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(AnonymousMethodBody expression)
        {
            return new AnonymousMethodBodyExpr
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type),
                Block = (ParameterBlock)this.Dispatch(expression.Block)
            };
        }

        public AstBase Visit(UserOperatorCall expression)
        {
            return new MethodCallExpression
            {
                Location = expression.GetSerLoc(),
                Method = this.GetMethodSpecId(expression.oper),
                Arguments = this.EnumerateArgs(expression.arguments)
            };
        }

        public AstBase Visit(ParenthesizedExpression expression)
        { return this.Dispatch(expression.Expr); }

        public AstBase Visit(Unary expression)
        {
            Ast.UnaryOperator? oper = AstToJObject.GetOperator(expression.Oper);
            if (!oper.HasValue)
            {
                throw new InvalidOperationException();
            }

            return new UnaryExpression
            {
                Location = expression.GetSerLoc(),
                Operator = (int)oper.Value,
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(Indirection expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(UnaryMutator expression)
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

            return new UnaryExpression
            {
                Location = expression.GetSerLoc(),
                Operator = (int)oper,
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(Binary expression)
        {
            return new BinaryExpression
            {
                Location = expression.GetSerLoc(),
                Operator = (int)AstToJObject.GetOperator(expression.Oper).Value,
                Left = this.Dispatch(expression.Left),
                Right = this.Dispatch(expression.Right)
            };
        }

        public AstBase Visit(Is expression)
        {
            return new IsExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.ProbeType.Type),
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(As expression)
        {
            return new AsExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.ProbeType.Type),
                Expression = this.Dispatch(expression.Expr)
            };
        }

        public AstBase Visit(ConditionalLogicalOperator expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(BooleanExpression expression)
        { return this.Dispatch(expression.Expr); }

        public AstBase Visit(BooleanExpressionFalse expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Conditional expression)
        {
            return new ConditionalExpression
            {
                Condition = this.Dispatch(expression.Expr),
                TrueExpression = this.Dispatch(expression.TrueExpr),
                FalseExpression = this.Dispatch(expression.FalseExpr),
                Location = expression.GetSerLoc()
            };
        }

        public AstBase Visit(LocalVariableReference expression)
        {
            return new LocalVariableRefExpression
            {
                IsAddressReference = expression.IsRef,
                LocalVariable = this.Dispatch(expression.local_info),
                Location = expression.GetSerLoc()
            };
        }

        public AstBase Visit(Mono.CSharp.ParameterReference expression)
        {
            return new ParameterReferenceExpression
            {
                Location = expression.GetSerLoc(),
                Parameter = (ParameterSer)this.Dispatch(expression.Parameter)
            };
        }

        public AstBase Visit(Invocation expression)
        {
            return new DelegateInvocationExpression
            {
                Location = expression.GetSerLoc(),
                Instance = this.Dispatch(expression.MethodGroup),
                Arguments = this.EnumerateArgs(expression.Arguments),
            };
        }

        public AstBase Visit(New expression)
        {
            return new NewExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type),
                Method = this.GetMethodSpecId(expression.Constructor),
                Arguments = this.EnumerateArgs(expression.Arguments)
            };
        }

        public AstBase Visit(ArrayInitializer expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ArrayCreation expression)
        {
            return new ArrayCreationExpression
            {
                Location = expression.GetSerLoc(),
                ArrayType = this.GetTypeSpecId(expression.Type),
                ElementType = this.GetTypeSpecId(expression.ArrayElementType),
                Initializers = this.Dispatch(expression.ResolvedInitializers),
                Arguments = this.Dispatch(expression.Arguments)
            };
        }

        public AstBase Visit(This expression)
        {
            return new ThisExpression
            {
                Location = expression.GetSerLoc(),
            };
        }

        public AstBase Visit(ArglistAccess expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(RefValueExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(RefTypeExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(MakeRefExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(TypeOf expression)
        {
            return new TypeOfExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.TypeArgument)
            };
        }

        public AstBase Visit(SizeOf expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(MemberAccess expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(CheckedExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(UnCheckedExpr experssion)
        { throw new NotImplementedException(); }

        public AstBase Visit(ElementAccess expression)
        {
            return new ElementAccessExpression
            {
                Location = expression.GetSerLoc(),
                Left = this.Dispatch(expression.Expr),
                Arguments = this.EnumerateArgs(expression.Arguments)
            };
        }

        public AstBase Visit(ArrayAccess expression)
        { return this.Visit(expression.ElementAccess); }

        public AstBase Visit(BaseThis expression)
        {
            return new BaseThisExpression
            { Location = expression.GetSerLoc() };
        }

        public AstBase Visit(EmptyExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ErrorExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ComposedTypeSpecifier expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ComposedCast expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ArrayIndexCast expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(StackAlloc expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ElementInitializer expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(CollectionOrObjectInitializers expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(NewInitialize expression)
        {
            var rv = new NewInitializerExpression
            {
                Location = expression.GetSerLoc(),
                Method = this.GetMethodSpecId(expression.Constructor),
                Arguments = this.EnumerateArgs(expression.Arguments),
                Initializers = new List<ObjectInitilaizer>(),
                Type = this.GetTypeSpecId(expression.Type)
            };

            if (expression.Initializers != null)
            {
                foreach (var init in expression.Initializers.Initializers)
                {
                    ElementInitializer initializer = init as ElementInitializer;
                    ObjectInitilaizer objInitializer = new ObjectInitilaizer
                    { Location = init.GetSerLoc(), };

                    if (initializer != null)
                    {
                        PropertyExpr propertyExpr = initializer.Target as PropertyExpr;
                        if (propertyExpr != null)
                        {
                            objInitializer.Setter = this.GetMethodSpecId(propertyExpr.Setter);
                        }
                        else
                        {
                            FieldExpr fieldExpr = (FieldExpr)initializer.Target;
                            objInitializer.Field = this.GetFieldSpecId(fieldExpr.Spec);
                        }

                        objInitializer.Value = this.Dispatch(initializer.Source);
                    }
                    else
                    {
                        Invocation invocation = (Invocation)init;
                        objInitializer.MethodCall = new MethodCallExpression
                        {
                            Location = expression.GetSerLoc(),
                            Method = this.GetMethodSpecId(invocation.MethodGroup.BestCandidate),
                            Arguments = this.EnumerateArgs(invocation.Arguments)
                        };
                    }

                    rv.Initializers.Add(objInitializer);
                }
            }

            return rv;
        }

        public AstBase Visit(NewAnonymousType expression)
        {
            Dictionary<string, ExpressionSer> initializers = new Dictionary<string, ExpressionSer>();
            foreach (var parameter in expression.Parameters)
            {
                initializers[parameter.Name] =
                    this.Dispatch(parameter.Expr);
            }

            return new NewAnonymoustype
            {
                Location = expression.GetSerLoc(),
                Initializers = initializers
            };
        }

        public AstBase Visit(AnonymousTypeParameter expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DynamicResultCast expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(CompositeExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ConstantExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(MemberExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(MethodGroupExpr expression)
        {
            return new MethodExpression
            {
                Location = expression.GetSerLoc(),
                Instance = this.Dispatch(expression.InstanceExpression),
                Method = this.GetMethodSpecId(expression.BestCandidate)
            };
        }

        public AstBase Visit(FieldExpr expression)
        {
            return new FieldExpression
            {
                Location = expression.GetSerLoc(),
                Instance = this.Dispatch(expression.InstanceExpression),
                Field = this.GetFieldSpecId(expression.Spec)
            };
        }

        public AstBase Visit(PropertyExpr expression)
        {
            return new PropertyExpression
            {
                Location = expression.GetSerLoc(),
                Getter = this.GetMethodSpecId(expression.Getter),
                Setter = this.GetMethodSpecId(expression.Setter),
                Instance = this.Dispatch(expression.InstanceExpression),
            };
        }

        public AstBase Visit(IndexerExpr expression)
        {
            return new IndexExpression
            {
                Getter = this.GetMethodSpecId(expression.Getter),
                Setter = this.GetMethodSpecId(expression.Setter),
                Instance = this.Dispatch(expression.InstanceExpression),
                Arguments = this.EnumerateArgs(
                    expression.Arguments,
                    expression.Setter != null ? 1 : 0)
            };
        }

        public AstBase Visit(EventExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ExpressionStatement expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DynamicExpressionStatement expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DynamicEventCompoundAssign expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DynamicConversion expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DynamicConstructorBinder expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DynamicIndexBinder expression)
        {
            return new DynamicIndexBinderExpression
            {
                Location = expression.GetSerLoc(),
                Instance = this.Dispatch(expression.Arguments[0].Expr),
                Index = this.Dispatch(expression.Arguments[1].Expr)
            };
        }

        public AstBase Visit(DynamicInvocation expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DynamicMemberBinder expression)
        {
            return new DynamicMethodBinderExpression
            {
                Location = expression.GetSerLoc(),
                Name = expression.Name,
                Instance = this.Dispatch(expression.Arguments[0].Expr),
                Value = this.Dispatch(expression.Setter)
            };
        }

        public AstBase Visit(DynamicUnaryConversion expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(AsyncInitializer expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DelegateInvocation expression)
        {
            return new DelegateInvocationExpression
            {
                Location = expression.GetSerLoc(),
                Instance = this.Dispatch(expression.InstanceExpr),
                Arguments = this.EnumerateArgs(expression.Arguments)
            };
        }

        public AstBase Visit(ConstructorInitializer expression)
        {
            return new ConstructorInitializerExpression
            {
                Location = expression.GetSerLoc(),
                Instance = new ThisExpression(),
                Method = this.GetMethodSpecId(expression.BaseConstructor),
                Arguments = this.EnumerateArgs(expression.Arguments)
            };
        }

        public AstBase Visit(ConstructorThisInitializer expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(EmptyExpressionStatement expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(CompletingExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(CompletionSimpleName expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(CompletionElementInitializer expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Await expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DefaultParameterValueExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Constant expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(StringConstant expression)
        {
            return new ConstantExpression<string>
            {
                Value = expression.Value,
                Location = expression.GetSerLoc(),
            };
        }

        public AstBase Visit(ShimExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(AQueryClause expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ARangeVariableQueryClause expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DefaultValueExpression expression)
        {
            return new DefaultValueExpr
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type)
            };
        }

        public AstBase Visit(PointerArithmetic expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(VariableReference expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(TemporaryVariableReference expression)
        {
            return new TempVariableRefExpression
            {
                Location = expression.GetSerLoc(),
                IsAddressReference = expression.IsRef,
                LocalVariable = this.Dispatch(expression.LocalInfo)
            };
        }

        public AstBase Visit(Arglist expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(ATypeNameExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(QualifiedAliasMember expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(TypeExpression expression)
        {
            return new TypeExpressionSer
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type)
            };
        }

        public AstBase Visit(TypeParameterExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(SpecialContraintExpr expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(Namespace expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(RootNamespace expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(GlobalRootNamespace expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(UserCast expression)
        {
            return new UserCastExpression
            {
                Location = expression.GetSerLoc(),
                Method = this.GetMethodSpecId(expression.Method),
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg { Value = this.Dispatch(expression.Source) }
                }
            };
        }

        public AstBase Visit(TypeCast expression)
        {
            return new TypeCastExpression
            {
                Location = expression.GetSerLoc(),
                Type = this.GetTypeSpecId(expression.Type),
                Expression = this.Dispatch(expression.Child)
            };
        }

        public AstBase Visit(ConvCast expression)
        { return this.Visit((TypeCast)expression); }

        public AstBase Visit(ReducedExpression expression)
        { return this.Dispatch(expression.Expression); }

        public AstBase Visit(AnonymousMethodExpression expression)
        {
            return new AnonymousMethodBodyExpr
            {
                Location = expression.GetSerLoc(),
                Block = (ParameterBlock)this.Dispatch(expression.Block),
                Type = 0
            };
        }

        public AstBase Visit(LambdaExpression expression)
        { return this.Visit((AnonymousMethodExpression)expression); }

        public AstBase Visit(RuntimeValueExpression expression)
        { throw new NotImplementedException(); }

        public AstBase Visit(DelegateCreation expression)
        {
            return new DelegateCreationExpression
            {
                Location = expression.GetSerLoc(),
                Method = this.Dispatch(expression.MethodGroup),
                Type = this.GetTypeSpecId(expression.Type)
            };
        }

        public AstBase Visit(ImplicitDelegateCreation expression)
        { return this.Visit((DelegateCreation)expression); }

        public AstBase Visit(NewDelegate expression)
        { return this.Visit((DelegateCreation)expression); }

        public AstBase Visit(Statement statement)
        { throw new NotImplementedException(); }

        public AstBase Visit(ResumableStatement statement)
        { throw new NotImplementedException(); }

        public AstBase Visit(YieldStatement<AsyncInitializer> statement)
        { throw new NotImplementedException(); }

        public AstBase Visit(ExceptionStatement statement)
        { throw new NotImplementedException(); }

        public AstBase Visit(Mono.CSharp.TryFinallyBlock statement)
        { throw new NotImplementedException(); }

        public AstBase Visit(ExitStatement statement)
        { throw new NotImplementedException(); }

        public AstBase Visit(ContextualReturn statement)
        { return this.Visit((Return)statement); }

        private ParameterSer Dispatch(Parameter parameter)
        {
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

            return new ParameterSer
                {
                    Location = ExtensionMethods.GetSerLoc(parameter.Location, parameter.EndLocation),
                    Modifier = (int)attributes,
                    Type = this.GetTypeSpecId(parameter.Type)
                };
        }

        private LocalVariableSer Dispatch(LocalVariable local)
        {
            if (local == null)
            {
                return null;
            }

            int blockId = -1;
            foreach (var node in this.scopeBlockStack)
            {
                if (node.Item2 == local.Block)
                {
                    blockId = node.Item1;
                    break;
                }
            }

            if (blockId == -1)
            {
                throw new InvalidOperationException();
            }

            return new LocalVariableSer
                {
                    Type = this.GetTypeSpecId(local.Type),
                    Name = local.Name,
                    BlockId = blockId
                };
        }

        private List<MethodCallArg> EnumerateArgs(Arguments arguments, int skipLast = 0)
        {
            var rv = new List<MethodCallArg>();
            for (int i = 0; arguments != null && i < arguments.Count - skipLast; i++)
            {
                rv.Add(
                    new MethodCallArg
                    {
                        IsByRef = arguments[i].IsByRef,
                        Value = this.Dispatch(arguments[i].Expr)
                    });
            }

            return rv;
        }

        private List<ParameterSer> GetParameters(ParametersCompiled parametersCompiled)
        {
            List<ParameterSer> rv = new List<ParameterSer>();
            for (int iParam = 0; iParam < parametersCompiled.Count; iParam++)
            {
                rv.Add(this.Dispatch(parametersCompiled[iParam]));
            }

            return rv;
        }

        private List<StatementSer> Dispatch(IEnumerable<Statement> coll)
        {
            List<StatementSer> rv = new List<StatementSer>();
            foreach (var stmt in coll)
            { rv.Add(this.Dispatch(stmt)); }

            return rv;
        }

        private List<ExpressionSer> Dispatch(IEnumerable<Expression> coll)
        {
            if (coll == null)
            { return null; }
            var rv = new List<ExpressionSer>();
            foreach (var expr in coll)
            {
                rv.Add(this.Dispatch(expr));
            }

            return rv;
        }

        private ExpressionSer Dispatch(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            return (ExpressionSer)this.dispatcher.Dispatch(expression);
        }

        private StatementSer Dispatch(Statement statement)
        {
            if (statement == null)
            {
                return null;
            }

            return (StatementSer)this.dispatcher.Dispatch(statement);
        }

        /// <summary>
        ///     Gets type specifier identifier.
        /// </summary>
        /// <param name="typeSpec"> Information describing the type. </param>
        /// <returns>
        ///     The type specifier identifier.
        /// </returns>
        public int GetTypeSpecId(TypeSpec typeSpec)
        {
            int token = 0;
            if (typeSpec != null)
            {
                if (!this.typeDict.TryGetValue(typeSpec, out token))
                { token = this.methodDict.Count + 1; }
            }

            // var token = typeSpec != null
            //     ? typeSpec.GetMetaInfo().MetadataToken
            //     : int.MinValue;
            if (this.TypeSerializationInfo.Types.ContainsKey(token))
            { return token; }

            this.TypeSerializationInfo.Types[token]
                = typeSpec != null
                    ? MemberReferenceSerializer.SerializeN(typeSpec)
                    : null;

            if (typeSpec != null)
            { this.typeDict.Add(typeSpec, token); }

            return token;
        }

        /// <summary>
        ///     Gets method specifier identifier.
        /// </summary>
        /// <param name="method"> The method. </param>
        /// <returns>
        ///     The method specifier identifier.
        /// </returns>
        public int GetMethodSpecId(Method method)
        {  return GetMethodSpecId((MethodSpec)method.Spec); }

        /// <summary>
        ///     Gets method specifier identifier.
        /// </summary>
        /// <param name="typeSpec"> Information describing the type. </param>
        /// <returns>
        ///     The method specifier identifier.
        /// </returns>
        public int GetMethodSpecId(MethodSpec typeSpec)
        {
            int token = 0;
            if (typeSpec != null)
            {
                if (!this.methodDict.TryGetValue(typeSpec, out token))
                { token = this.methodDict.Count + 1; }
            }

            // var token = typeSpec != null
            //     ? typeSpec.GetMetaInfo().MetadataToken
            //     : int.MinValue;
            if (this.TypeSerializationInfo.Methods.ContainsKey(token))
            { return token; }

            this.TypeSerializationInfo.Methods[token]
                = typeSpec != null
                    ? MemberReferenceSerializer.SerializeN(typeSpec)
                    : null;

            if (typeSpec != null)
            { this.methodDict.Add(typeSpec, token); }

            return token;
        }

        /// <summary>
        ///     Gets field specifier identifier.
        /// </summary>
        /// <param name="typeSpec"> Information describing the type. </param>
        /// <returns>
        ///     The field specifier identifier.
        /// </returns>
        public int GetFieldSpecId(FieldSpec typeSpec)
        {
            int token = 0;
            if (typeSpec != null)
            {
                if (!this.fieldDict.TryGetValue(typeSpec, out token))
                { token = this.fieldDict.Count + 1; }
            }

            // var token = typeSpec != null
            //     ? typeSpec.GetMetaInfo().MetadataToken
            //     : int.MinValue;
            if (this.TypeSerializationInfo.Fields.ContainsKey(token))
            { return token; }

            this.TypeSerializationInfo.Fields[token]
                = typeSpec != null
                    ? MemberReferenceSerializer.SerializeN(typeSpec)
                    : null;

            if (typeSpec != null)
            { this.fieldDict.Add(typeSpec, token); }

            return token;
        }
    }
}
