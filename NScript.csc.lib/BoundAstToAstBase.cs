namespace NScript.Csc.Lib
{
    using JsCsc.Lib.Serialization;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Symbols;
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class BoundAstToAstBase
        : BoundAstToNotImplemented<SerializationContext, AstBase>
    {
        private int id = 0;
        private LinkedList<(int id, BoundNode nodeBlock)> scopeBlockStack = new LinkedList<(int, BoundNode)>();

        public MethodBody GetMethodBody(
            IMethodSymbol methodSymbol,
            BoundNode boundNode,
            BoundStatementList initializers,
            SerializationContext arg)
        {
            if (methodSymbol.Name == "ReturnInt" && methodSymbol.ContainingType.Name.StartsWith("BasicStatements"))
            { }

            var methodId = arg
                .SymbolSerializer
                .GetMethodSpecId(methodSymbol);

            var parameterBlock =
                boundNode == null
                ? null
                : this.Visit((BoundBlock)boundNode, arg, methodSymbol, initializers);

            // TODO: Make note of [Script] Attribute in case of empty body.
            var rv = new MethodBody
            {
                MethodId = arg
                    .SymbolSerializer
                    .GetMethodSpecId(methodSymbol),
                Body = parameterBlock,
                FileName = boundNode?.SyntaxTree.FilePath,
            };

            return rv;
        }

        public ParameterSer Visit(IParameterSymbol parameter, SerializationContext arg)
        {
            ParameterAttributes attributes = ParameterAttributes.None;
            string paramName = parameter.Name;

            switch (parameter.RefKind)
            {
                case RefKind.Out:
                    attributes = ParameterAttributes.Out;
                    break;
                case RefKind.Ref:
                    attributes = ParameterAttributes.Out | ParameterAttributes.In;
                    break;
                case RefKind.RefReadOnly:
                    paramName = "this";
                    break;
            }

            return new ParameterSer
            {
                Modifier = (int)attributes,
                Type = arg.SymbolSerializer.GetTypeSpecId(parameter.Type),
                Name = paramName
            };
        }

        public ParameterBlock Visit(
            BoundBlock node,
            SerializationContext arg,
            IMethodSymbol methodSymbol,
            BoundStatementList parentBlockWithInitializers)
        {
            var rv = new ParameterBlock
            {
                Location = node.Syntax.GetSerLoc(),
                Id = ++this.id
            };

            this.scopeBlockStack.AddFirst((rv.Id, node));

            // Expression can be null for static field initializer constructors.
            rv.IsMethodOwned = true;
            rv.Parameters = methodSymbol.Parameters.Select(_ => this.Visit(_, arg)).ToList();
            IEnumerable<BoundStatement> statements = null;

            if (parentBlockWithInitializers != null)
            {
                statements = parentBlockWithInitializers
                    .Statements
                    .Where(_ => _ != node);
            }

            if (node != null)
            {
                if (statements != null)
                { statements = statements.Concat(node.Statements); }
                else
                { statements = node.Statements; }
            }

            rv.Statements = statements == null
                ? null
                : statements
                    .Where(_ => !(_ is BoundSequencePointWithSpan))
                    .Select(_ => this.VisitToStatement(_, arg))
                    .ToList();

            this.scopeBlockStack.RemoveFirst();
            return rv;
        }

        public override AstBase DefaultVisit(BoundNode node, SerializationContext arg)
        { throw new NotImplementedException(); }
        public override AstBase VisitAddressOfOperator(BoundAddressOfOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitArgList(BoundArgList node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitArgListOperator(BoundArgListOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitArrayAccess(BoundArrayAccess node, SerializationContext arg)
            => new ElementAccessExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Left = (ExpressionSer)this.Visit(node.Expression, arg),
                Arguments = node
                    .Indices
                .Select(_ => new MethodCallArg
                    {
                        IsByRef = false,
                        Value = (ExpressionSer)this.Visit(_, arg)
                    })
                .ToList()
            };

        public override AstBase VisitArrayCreation(BoundArrayCreation node, SerializationContext arg)
            => new ArrayCreationExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                ArrayType = arg.SymbolSerializer.GetTypeSpecId(node.Type),
                ElementType = arg.SymbolSerializer.GetTypeSpecId(((IArrayTypeSymbol)node.Type).ElementType),
                Initializers = node.InitializerOpt?
                    .Initializers
                    .Select(_ => (ExpressionSer)this.Visit(_, arg))
                    .ToList(),
                Arguments = node.InitializerOpt != null
                    ? null
                    : node.Bounds
                        .Select(_ => (ExpressionSer)this.Visit(_, arg))
                        .ToList()
            };

        public override AstBase VisitArrayInitialization(BoundArrayInitialization node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitArrayLength(BoundArrayLength node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitAsOperator(BoundAsOperator node, SerializationContext arg)
            => new AsExpression
            {
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type),
                Expression = (ExpressionSer)this.Visit(node.Operand, arg)
            };

        public override AstBase VisitAssignmentOperator(BoundAssignmentOperator node, SerializationContext arg)
            => new BinaryExpression
            {
                Left = (ExpressionSer)this.Visit(node.Left, arg),
                Right = (ExpressionSer)this.Visit(node.Right, arg),
                Operator = (int)CLR.AST.BinaryOperator.Assignment,
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitAttribute(BoundAttribute node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitAwaitExpression(BoundAwaitExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitBadExpression(BoundBadExpression node, SerializationContext arg)
        {
            return null;
        }
        public override AstBase VisitBadStatement(BoundBadStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitBaseReference(BoundBaseReference node, SerializationContext arg)
            => new BaseThisExpression { Location = node.Syntax.Location.GetSerLoc() };

        public override AstBase VisitBinaryOperator(BoundBinaryOperator node, SerializationContext arg)
        {
            var op = node.OperatorKind;
            bool isLifted = IsLifted(op);
            bool isChecked = IsChecked(op);
            var typeMask = op & BinaryOperatorKind.TypeMask;
            if (typeMask == BinaryOperatorKind.UserDefined)
            {
                return UserDefinedBinaryOperator(
                    node,
                    arg,
                    node.MethodOpt,
                    node.OperatorKind & (BinaryOperatorKind.OpMask | BinaryOperatorKind.Logical),
                    IsLifted(node.OperatorKind));
            }

            var nscriptOp = GetNScriptOperator(op);
            if (typeMask <= BinaryOperatorKind.Bool
                || typeMask == BinaryOperatorKind.Enum
                || typeMask == BinaryOperatorKind.Delegate
                || nscriptOp == CLR.AST.BinaryOperator.Equals
                || nscriptOp == CLR.AST.BinaryOperator.NotEquals
                || (nscriptOp == CLR.AST.BinaryOperator.Plus
                    && (typeMask == BinaryOperatorKind.String
                        || typeMask == BinaryOperatorKind.StringAndObject
                        || typeMask == BinaryOperatorKind.ObjectAndString)))
            {
                return new BinaryExpression
                {
                    IsLifted = isLifted,
                    Left = (ExpressionSer)this.Visit(node.Left, arg),
                    Right = (ExpressionSer)this.Visit(node.Right, arg),
                    Operator = (int)nscriptOp,
                    Location = node.Syntax.Location.GetSerLoc()
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private AstBase UserDefinedBinaryOperator(
            BoundBinaryOperator node,
            SerializationContext arg,
            IMethodSymbol methodSymbol,
            BinaryOperatorKind binaryOperator,
            bool isLifted)
            => new UserDefinedBinaryOrUnaryOpExpression
               {
                   Method = arg.SymbolSerializer.GetMethodSpecId(methodSymbol),
                   Arguments = this.ToArgs(
                       methodSymbol,
                       new List<BoundExpression> { node.Left, node.Right },
                        arg),
                   Location = node.Syntax.Location.GetSerLoc(),
                   Instance = null,
                   IsLifted = isLifted,
                   Operator = (int)GetNScriptOperator(binaryOperator)
               };

        public override AstBase VisitBlock(BoundBlock node, SerializationContext arg)
        {
            this.scopeBlockStack.AddFirst((++this.id, node));
            try
            {
                return new ExplicitBlockSer
                {
                    Id = this.id,
                    Statements = node
                        .Statements
                        .Select(_ => this.VisitToStatement(_, arg))
                        .ToList()
                };
            }
            finally
            { this.scopeBlockStack.RemoveFirst(); }
        }

        public override AstBase VisitBreakStatement(BoundBreakStatement node, SerializationContext arg)
            => new BreakStatement { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitCall(BoundCall node, SerializationContext arg)
        // TODO: validate Method has full TypeInfo, e.g. generic arguments
        {
            bool isStatic = node.Method.ContainingSymbol.IsStatic
                || node.Method.IsStatic;

            return node.Method.MethodKind == MethodKind.DelegateInvoke
                ? (AstBase)new DelegateInvocationExpression
                    {
                        Arguments = this.ToArgs(node.Method, node.Arguments, arg),
                        Location = node.Syntax.Location.GetSerLoc(),
                        Instance = (ExpressionSer)this.Visit(node.ReceiverOpt, arg)
                    }
                : new MethodCallExpression
                    {
                        Method = arg.SymbolSerializer.GetMethodSpecId(
                            node.Method),
                        Arguments = this.ToArgs(node.Method, node.Arguments, arg),
                        Location = node.Syntax.Location.GetSerLoc(),
                        Instance = !isStatic
                            ? (ExpressionSer)this.Visit(node.ReceiverOpt, arg)
                            : null
                    };
        }

        public override AstBase VisitCatchBlock(BoundCatchBlock node, SerializationContext arg)
        {
            this.scopeBlockStack.AddFirst((++this.id, node));
            try
            {
                return new CatchBlock
                {
                    Block = new ExplicitBlockSer
                    {
                        Id = this.id,
                        Statements = node
                        .Body
                        .Statements
                        .Select(_ => this.VisitToStatement(_, arg))
                        .ToList()
                    },
                    CatchType = node.ExceptionTypeOpt != null
                        ? arg.SymbolSerializer.GetTypeSpecId(node.ExceptionTypeOpt)
                        : (int?)null,
                    LocalVariable = node.ExceptionSourceOpt != null
                        ? ((LocalVariableRefExpression)this.VisitLocal((BoundLocal)node.ExceptionSourceOpt, arg)).LocalVariable
                        : null
                };
            }
            finally
            {
                this.scopeBlockStack.RemoveFirst();
            }
        }

        public override AstBase VisitCollectionElementInitializer(BoundCollectionElementInitializer node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, SerializationContext arg)
        {
            var op = node.Operator.Kind;
            bool isLifted = IsLifted(op);
            bool isChecked = IsChecked(op);
            var typeMask = op & BinaryOperatorKind.TypeMask;
            if (typeMask == BinaryOperatorKind.UserDefined)
            { throw new NotImplementedException(); }

            var nscriptOp = GetNScriptOperator(op) + 1;
            if (typeMask <= BinaryOperatorKind.Bool
                || typeMask == BinaryOperatorKind.Enum
                || typeMask == BinaryOperatorKind.Delegate
                || nscriptOp == CLR.AST.BinaryOperator.Equals
                || nscriptOp == CLR.AST.BinaryOperator.NotEquals
                || (nscriptOp == CLR.AST.BinaryOperator.PlusAssignment
                    && (typeMask == BinaryOperatorKind.String
                        || typeMask == BinaryOperatorKind.StringAndObject
                        || typeMask == BinaryOperatorKind.ObjectAndString)))
            {
                return new BinaryExpression
                {
                    IsLifted = isLifted,
                    Left = (ExpressionSer)this.Visit(node.Left, arg),
                    Right = (ExpressionSer)this.Visit(node.Right, arg),
                    Operator = (int)nscriptOp,
                    Location = node.Syntax.Location.GetSerLoc()
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override AstBase VisitConditionalAccess(BoundConditionalAccess node, SerializationContext arg)
            => new ConditionalExpression
            {
                Condition = (ExpressionSer)this.Visit(node.Receiver, arg),
                TrueExpression = (ExpressionSer)this.Visit(node.AccessExpression, arg),
            };

        public override AstBase VisitConditionalGoto(BoundConditionalGoto node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitConditionalOperator(BoundConditionalOperator node, SerializationContext arg)
            => new ConditionalExpression
            {
                Condition = (ExpressionSer)this.Visit(node.Condition, arg),
                TrueExpression = (ExpressionSer)this.Visit(node.Consequence, arg),
                FalseExpression = (ExpressionSer)this.Visit(node.Alternative, arg),
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitConditionalReceiver(BoundConditionalReceiver node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitConstantPattern(BoundConstantPattern node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitConstructorMethodBody(BoundConstructorMethodBody node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitContinueStatement(BoundContinueStatement node, SerializationContext arg)
            => new ContinueStatement { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitConversion(BoundConversion node, SerializationContext arg)
        {
            switch (node.ConversionKind)
            {
                case ConversionKind.AnonymousFunction:
                    return this.VisitLambda((BoundLambda)node.Operand, arg);
                case ConversionKind.MethodGroup:
                    return new DelegateCreationExpression
                    {
                        Method = (ExpressionSer)this.VisitMethodGroup((BoundMethodGroup)node.Operand, arg),
                        Location = node.Syntax.GetSerLoc(),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
                    };
                case ConversionKind.Boxing:
                    return new BoxCastExpression
                    { Expression = (ExpressionSer)this.Visit(node.Operand, arg) };
                case ConversionKind.ImplicitReference:
                    return this.Visit(node.Operand, arg);

                case ConversionKind.ExplicitReference:
                    // return this.Visit(node.Operand, arg);
                case ConversionKind.ExplicitEnumeration: // Cast to enum from int/short, use cast b'cause we may be using string for enum
                case ConversionKind.ImplicitEnumeration: // Converts int/short to Enum, use cast b'cause we may be using string for enum
                case ConversionKind.ExplicitNumeric:
                    // Do we really have to cast, can't we use any other short cut?
                    return new TypeCastExpression
                    {
                        Expression = (ExpressionSer)this.Visit(node.Operand, arg),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
                    };
                case ConversionKind.ExplicitNullable:
                    return new NullableToNormal
                    {
                        Expression = (ExpressionSer)this.Visit(node.Operand, arg),
                    };
                case ConversionKind.Unboxing:
                    return new TypeCastExpression
                    {
                        Expression = (ExpressionSer)this.Visit(node.Operand, arg),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type),
                        IsUnbox = true
                    };
                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ExplicitUserDefined:
                    return new MethodCallExpression
                    {
                        Method = arg.SymbolSerializer.GetMethodSpecId(node.SymbolOpt),
                        Arguments = new List<MethodCallArg> { new MethodCallArg { Value = (ExpressionSer)this.Visit(node.Operand, arg) } },
                    };
                case ConversionKind.DefaultOrNullLiteral:
                    return new DefaultValueExpr
                    {
                        Location = node.Syntax.Location.GetSerLoc(),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
                    };
                case ConversionKind.ImplicitConstant:
                    return GetConstLiteral(node.ConstantValueOpt);
                case ConversionKind.Identity:
                    // This is cast to one of the base types
                    // Should we return this as typeCast Expression in case of interface?
                    return this.Visit(node.Operand, arg);
                case ConversionKind.NoConversion:
                case ConversionKind.ImplicitNullable:
                    return this.Visit(node.Operand, arg);
                case ConversionKind.ImplicitNumeric:
                    return new TypeCastExpression
                    {
                        Expression = (ExpressionSer)this.Visit(node.Operand, arg),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
                    };
                case ConversionKind.ImplicitThrow:
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ExplicitTupleLiteral:
                case ConversionKind.ExplicitTuple:
                case ConversionKind.PointerToVoid:
                case ConversionKind.NullToPointer:
                case ConversionKind.ImplicitDynamic:
                case ConversionKind.ExplicitDynamic:
                case ConversionKind.PointerToPointer:
                case ConversionKind.IntegerToPointer:
                case ConversionKind.PointerToInteger:
                case ConversionKind.IntPtr:
                case ConversionKind.InterpolatedString:
                case ConversionKind.Deconstruction:
                case ConversionKind.StackAllocToPointerType:
                case ConversionKind.StackAllocToSpanType:
                case ConversionKind.PinnedObjectToPointer:
                default:
                    throw new NotImplementedException();
            }
        }
        public override AstBase VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDeclarationPattern(BoundDeclarationPattern node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDefaultExpression(BoundDefaultExpression node, SerializationContext arg)
            => new DefaultValueExpr
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
            };

        public override AstBase VisitDelegateCreationExpression(BoundDelegateCreationExpression node, SerializationContext arg)
            => new DelegateCreationExpression
            {
                Location = node.Syntax.GetSerLoc(),
                Method = (ExpressionSer)this.Visit(node.Argument, arg),
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
            };

        public override AstBase VisitDiscardExpression(BoundDiscardExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDoStatement(BoundDoStatement node, SerializationContext arg)
            => new DoStatement
            {
                Loop = (StatementSer)this.VisitToStatement(node.Body, arg),
                Condition = (ExpressionSer)this.Visit(node.Condition, arg)
            };
        public override AstBase VisitDup(BoundDup node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDynamicInvocation(BoundDynamicInvocation node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDynamicMemberAccess(BoundDynamicMemberAccess node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitEventAccess(BoundEventAccess node, SerializationContext arg)
            => new EventExpression
            {
                Event = arg.SymbolSerializer.GetEventSpecId(node.EventSymbol),
                Instance = !node.EventSymbol.IsStatic
                    ? (ExpressionSer)this.Visit(node.ReceiverOpt, arg)
                    : null,
                Location = node.Syntax.GetSerLoc()
            };

        public override AstBase VisitEventAssignmentOperator(BoundEventAssignmentOperator node, SerializationContext arg)
            => new MethodCallExpression
            {
                // TODO: Track this as first class Binary Operator.
                Location = node.Syntax.GetSerLoc(),
                Method = arg.SymbolSerializer.GetMethodSpecId(
                    node.IsAddition
                    ? node.Event.AddMethod
                    : node.Event.RemoveMethod),
                Instance = !node.Event.IsStatic
                    ? (ExpressionSer)this.Visit(node.ReceiverOpt, arg)
                    : null,
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg
                    {
                        IsByRef = false,
                        Value = (ExpressionSer)this.Visit(node.Argument, arg)
                    }
                }
            };

        public override AstBase VisitExpressionStatement(BoundExpressionStatement node, SerializationContext arg)
            => new StatementExpressionSer { Expression = (ExpressionSer)this.Visit(node.Expression, arg) };

        public override AstBase VisitFieldAccess(BoundFieldAccess node, SerializationContext arg)
        // TODO: validate FieldSymbol has full TypeInfo, e.g. generic arguments
        {
            AstBase rv = null;
            if (node.FieldSymbol.IsConst)
            { rv = GetConstantValue(node.FieldSymbol.ConstantValue); }
            else
            {
                rv = new FieldExpression
                {
                    Field = arg.SymbolSerializer.GetFieldSpecId(node.FieldSymbol),
                    Instance = !node.FieldSymbol.IsStatic
                      ? (ExpressionSer)this.Visit(node.ReceiverOpt, arg)
                      : null
                };
            }

            rv.Location = node.Syntax.GetSerLoc();
            return rv;
        }

        public override AstBase VisitFieldEqualsValue(BoundFieldEqualsValue node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitFieldInfo(BoundFieldInfo node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitFixedStatement(BoundFixedStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitForEachDeconstructStep(BoundForEachDeconstructStep node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitForEachStatement(BoundForEachStatement node, SerializationContext arg)
            => this.WrapInBlock(
                node,
                id =>
                    new ForEachStatement {
                        BlockId = id,
                        LocalVariableName = node.IterationVariables[0].Name,
                        Collection = (ExpressionSer)this.Visit(node.Expression, arg),
                        Loop = this.VisitToStatement(node.Body, arg)
                    });

        public override AstBase VisitForStatement(BoundForStatement node, SerializationContext arg)
            => this.WrapInBlock(
                node,
                id =>
                    new ForStatement
                    {
                        BlockId = id,
                        Initializer = this.VisitToStatement(node.Initializer, arg),
                        Condition = (ExpressionSer)this.Visit(node.Condition, arg),
                        Iterator = this.VisitToStatement(node.Increment, arg),
                        Loop = (StatementSer)this.Visit(node.Body, arg)
                    });

        public override AstBase VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitGotoStatement(BoundGotoStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitHoistedFieldAccess(BoundHoistedFieldAccess node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitHostObjectMemberReference(BoundHostObjectMemberReference node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitIfStatement(BoundIfStatement node, SerializationContext arg)
            => new IfStatement
            {
                Condition = (ExpressionSer)this.Visit(node.Condition,arg),
                TrueStatement = this.VisitToStatement(node.Consequence, arg),
                FalseStatement = node.AlternativeOpt == null
                    ? null
                    : this.VisitToStatement(node.AlternativeOpt, arg)
            };

        public override AstBase VisitImplicitReceiver(BoundImplicitReceiver node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitIncrementOperator(BoundIncrementOperator node, SerializationContext arg)
            => this.ConvertUnaryOperator(
                node.Operand,
                node.MethodOpt,
                node.OperatorKind,
                arg);

        public override AstBase VisitIndexerAccess(BoundIndexerAccess node, SerializationContext arg)
            => new IndexExpression
            {
                Property = arg.SymbolSerializer.GetPropertySpecId(node.Indexer),
                Instance = node.ReceiverOpt != null ? (ExpressionSer)this.Visit(node.ReceiverOpt, arg) : null,
                Arguments = ToArgs(
                    node.Indexer.GetMethod,
                    node.Arguments,
                    arg)
            };

        public override AstBase VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitInterpolatedString(BoundInterpolatedString node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitIsOperator(BoundIsOperator node, SerializationContext arg)
            => new IsExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Expression = (ExpressionSer)this.Visit(node.Operand, arg),
                Type = arg.SymbolSerializer.GetTypeSpecId((ITypeSymbol)node.TargetType.ExpressionSymbol)
            };
        public override AstBase VisitIsPatternExpression(BoundIsPatternExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitLabel(BoundLabel node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitLabeledStatement(BoundLabeledStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitLabelStatement(BoundLabelStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitLambda(BoundLambda node, SerializationContext arg)
        {
            var parameterBlock = this.Visit(
                node.Body,
                arg,
                node.Symbol,
                null);

            return new AnonymousMethodBodyExpr
            {
                Block = parameterBlock,
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
            };
        }

        public override AstBase VisitLiteral(BoundLiteral node, SerializationContext arg)
        {
            var rv = GetConstLiteral(node.ConstantValue);
            rv.Location = node.Syntax.Location.GetSerLoc();
            return rv;
        }

        public override AstBase VisitLocal(BoundLocal node, SerializationContext arg)
            => new LocalVariableRefExpression
            {
                LocalVariable = this.GetLocalVariable(node.LocalSymbol, arg),
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitLocalDeclaration(BoundLocalDeclaration node, SerializationContext arg)
            => new BinaryExpression
            {
                Operator = (int)CLR.AST.BinaryOperator.Assignment,
                Left = new LocalVariableRefExpression
                {
                    Location = node.Syntax.Location.GetSerLoc(),
                    LocalVariable = this.GetLocalVariable(node.LocalSymbol, arg)
                },
                Right = (ExpressionSer)this.Visit(node.InitializerOpt, arg)
            };

        private LocalVariableSer GetLocalVariable(LocalSymbol localSymbol, SerializationContext arg)
        {
            int id = -1;
            foreach (var node in this.scopeBlockStack)
            {
                if (node.nodeBlock.Syntax == localSymbol.ScopeDesignatorOpt)
                {
                    id = node.id;
                    break;
                }
            }

            if (id == -1)
            { throw new InvalidOperationException(); }

            return new LocalVariableSer
            {
                Name = localSymbol.MetadataName,
                Type = arg.SymbolSerializer.GetTypeSpecId(localSymbol.Type.TypeSymbol),
                BlockId = id
            };
        }

        public override AstBase VisitLocalFunctionStatement(BoundLocalFunctionStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitLockStatement(BoundLockStatement node, SerializationContext arg)
            => this.Visit(node.Body, arg);

        public override AstBase VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitMakeRefOperator(BoundMakeRefOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitMethodDefIndex(BoundMethodDefIndex node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitMethodGroup(BoundMethodGroup node, SerializationContext arg)
            => new MethodExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Method = arg.SymbolSerializer.GetMethodSpecId(node.Methods[0]),
                GenericParameters = node.TypeArgumentsOpt != null
                    ? node.TypeArgumentsOpt
                        .Select(_ => arg.SymbolSerializer.GetTypeSpecId(_.TypeSymbol))
                        .ToList()
                    : null,
                Instance = !node.Methods[0].IsStatic
                    ? (ExpressionSer)this.Visit(node.ReceiverOpt, arg)
                    : null
            };
        public override AstBase VisitMethodInfo(BoundMethodInfo node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitModuleVersionId(BoundModuleVersionId node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitModuleVersionIdString(BoundModuleVersionIdString node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, SerializationContext arg)
        {
            return new VariableBlockDeclaration
            {
                Initializers = node
                    .LocalDeclarations
                    .Select(_ => (ExpressionSer)this.Visit(_, arg))
                    .ToList()
            };
        }

        public override AstBase VisitNameOfOperator(BoundNameOfOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitNamespaceExpression(BoundNamespaceExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitNewT(BoundNewT node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitNoOpStatement(BoundNoOpStatement node, SerializationContext arg)
            => new EmptyStatementSer { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitNullCoalescingOperator(BoundNullCoalescingOperator node, SerializationContext arg)
            => new NullCoalescingOperatorSer
            {
                Location = node.Syntax.GetSerLoc(),
                Left = (ExpressionSer)this.Visit(node.LeftOperand, arg),
                Right = (ExpressionSer)this.Visit(node.RightOperand, arg)
            };

        public override AstBase VisitObjectCreationExpression(BoundObjectCreationExpression node, SerializationContext arg)
        {
            var location = node.Syntax.Location.GetSerLoc();
            var type = arg.SymbolSerializer.GetTypeSpecId(node.Type);
            var method = arg.SymbolSerializer.GetMethodSpecId(node.Constructor);
            var arguments = ToArgs(node.Constructor, node.Arguments, arg);

            if (node.InitializerExpressionOpt == null)
            {
                return new NewExpression
                {
                    Location = location,
                    Type = type,
                    Method = method,
                    Arguments = arguments
                };
            }

            return new NewInitializerExpression
            {
                Location = location,
                Type = type,
                Method = method,
                Arguments = arguments,
                Initializers = ((BoundObjectInitializerExpression)node.InitializerExpressionOpt)
                    .Initializers
                    .Select(_ =>
                    {
                        if (_.Kind == BoundKind.AssignmentOperator)
                        {
                            var assignOp = (BoundAssignmentOperator)_;
                            var initializerMember = (BoundObjectInitializerMember)assignOp.Left;
                            var rv = new ObjectInitilaizer()
                            {
                                Location = _.Syntax.Location.GetSerLoc(),
                                Value = (ExpressionSer)this.Visit(assignOp.Right, arg)
                            };

                            if (initializerMember.MemberSymbol.Kind == SymbolKind.Field)
                            {
                                rv.Field = arg.SymbolSerializer.GetFieldSpecId(
                                    (FieldSymbol)initializerMember.MemberSymbol);
                            }
                            else 
                            {
                                var propertySymbol = (PropertySymbol)initializerMember.MemberSymbol;
                                rv.Proeprty = arg.SymbolSerializer.GetPropertySpecId(propertySymbol);
                                rv.Setter = arg.SymbolSerializer.GetMethodSpecId(propertySymbol.SetMethod);
                            }

                            return rv;
                        }
                        else
                        {
                            return null;
                        }
                    })
                    .ToList()
            };
        }

        public override AstBase VisitObjectInitializerExpression(BoundObjectInitializerExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitObjectInitializerMember(BoundObjectInitializerMember node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitOutVariablePendingInference(OutVariablePendingInference node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitParameter(BoundParameter node, SerializationContext arg)
        {
            var attrs = ParameterAttributes.None;
            var paramSymbol = node.ParameterSymbol;
            if ((paramSymbol.RefKind & RefKind.Ref) != 0)
            { attrs |= ParameterAttributes.Out | ParameterAttributes.In; }
            if ((paramSymbol.RefKind & RefKind.Out) != 0)
            { attrs |= ParameterAttributes.Out; }

            return
                new ParameterReferenceExpression
                {
                    Location = node.Syntax.Location.GetSerLoc(),
                    Parameter = new ParameterSer
                    {
                        Name = node.ParameterSymbol.MetadataName,
                        BlockId = 0,
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.ParameterSymbol.Type.TypeSymbol),
                        Modifier = (int)attrs
                    }
                };
        }

        public override AstBase VisitParameterEqualsValue(BoundParameterEqualsValue node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPassByCopy(BoundPassByCopy node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPatternSwitchLabel(BoundPatternSwitchLabel node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPatternSwitchSection(BoundPatternSwitchSection node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPatternSwitchStatement(BoundPatternSwitchStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPointerElementAccess(BoundPointerElementAccess node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPropertyAccess(BoundPropertyAccess node, SerializationContext arg)
            => new PropertyExpression
            {
                Property = arg.SymbolSerializer.GetPropertySpecId(node.PropertySymbol),
                Instance = !node.PropertySymbol.IsStatic ? (ExpressionSer)this.Visit(node.ReceiverOpt, arg) : null,
                IsNotVirtual = node.SuppressVirtualCalls,
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitPropertyEqualsValue(BoundPropertyEqualsValue node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPropertyGroup(BoundPropertyGroup node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitPseudoVariable(BoundPseudoVariable node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitQueryClause(BoundQueryClause node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitRangeVariable(BoundRangeVariable node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitRefTypeOperator(BoundRefTypeOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitRefValueOperator(BoundRefValueOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitReturnStatement(BoundReturnStatement node, SerializationContext arg)
            => new ReturnStatement {
                Location = node.Syntax.Location.GetSerLoc(),
                Expression = node.ExpressionOpt != null
                    ? (ExpressionSer)this.Visit(node.ExpressionOpt, arg)
                    : null
            };

        public override AstBase VisitScope(BoundScope node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitSequence(BoundSequence node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitSequencePoint(BoundSequencePoint node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitSequencePointExpression(BoundSequencePointExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitSequencePointWithSpan(BoundSequencePointWithSpan node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitSizeOfOperator(BoundSizeOfOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitSourceDocumentIndex(BoundSourceDocumentIndex node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitStateMachineScope(BoundStateMachineScope node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitStatementList(BoundStatementList node, SerializationContext arg)
            => new StatementListSer
            {
                Statements = node
                    .Statements
                    .Select(_ => this.VisitToStatement(_, arg))
                    .ToList()
            };

        public override AstBase VisitStringInsert(BoundStringInsert node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitSwitchLabel(BoundSwitchLabel node, SerializationContext arg)
            => node.ExpressionOpt != null
                ? (ExpressionSer)this.Visit(node.ExpressionOpt, arg)
                : (ExpressionSer)GetConstLiteral(node.ConstantValueOpt);

        public override AstBase VisitSwitchSection(BoundSwitchSection node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitSwitchStatement(BoundSwitchStatement node, SerializationContext arg)
        {
            var switchSections = new List<SwitchSectionSer>();
            foreach (var section in node.SwitchSections)
            {
                var caseLabels = new List<SwitchCaseLabel>();
                foreach (var label in section.SwitchLabels)
                {
                    if (label.ConstantValueOpt == null && label.ExpressionOpt == null)
                    { caseLabels.Add(new SwitchCaseLabel()); }
                    else
                    {
                        caseLabels.Add(
                            new SwitchCaseLabel
                            { LabelValue = (ExpressionSer)this.VisitSwitchLabel(label, arg) });
                    }
                }

                StatementSer blockSer = null;
                if (section.Statements != null)
                {
                    if (section.Statements.Length > 1)
                    {
                        blockSer = new BlockSer
                        {
                            Statements = section.Statements == null
                                        ? null
                                        : section
                                        .Statements
                                        .Select(_ => this.VisitToStatement(_, arg))
                                        .ToList()
                        };
                    }
                    else
                    { blockSer = this.VisitToStatement(section.Statements[0], arg); }
                }

                switchSections.Add(
                    new SwitchSectionSer
                    {
                        Labels = caseLabels,
                        Block = blockSer
                    });
            }

            return new SwitchStatement
                {
                    Blocks = switchSections,
                    SwitchExpression = (ExpressionSer)this.Visit(node.Expression, arg)
                };
        }

        public override AstBase VisitThisReference(BoundThisReference node, SerializationContext arg)
            => new ThisExpression { Location = node.Syntax.Location.GetSerLoc() };

        public override AstBase VisitThrowExpression(BoundThrowExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }

        public override AstBase VisitThrowStatement(BoundThrowStatement node, SerializationContext arg)
            => new ThrowExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Expression = node.ExpressionOpt != null
                    ? (ExpressionSer)this.Visit(node.ExpressionOpt, arg)
                    : null
            };
        public override AstBase VisitTryStatement(BoundTryStatement node, SerializationContext arg)
        {
            var tryCatchBlock = node.CatchBlocks.Length > 0
                ? new TryCatchBlock
                {
                    TryBlock = (StatementSer)this.Visit(node.TryBlock, arg),
                    CatchBlocks = node.CatchBlocks
                    .Select(_ => (CatchBlock)this.Visit(_, arg))
                    .ToList()
                }
                : null;

            if (node.FinallyBlockOpt != null)
            {
                return new TryFinallyBlockSer
                {
                    TryBlock = tryCatchBlock ?? (StatementSer)this.Visit(node.TryBlock, arg),
                    FinallyBlock = this.VisitToStatement(node.FinallyBlockOpt, arg)
                };
            }

            return tryCatchBlock;
        }

        public override AstBase VisitTupleBinaryOperator(BoundTupleBinaryOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitTupleLiteral(BoundTupleLiteral node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitTypeExpression(BoundTypeExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitTypeOfOperator(BoundTypeOfOperator node, SerializationContext arg)
            => new TypeOfExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Type = arg.SymbolSerializer.GetTypeSpecId((ITypeSymbol)node.SourceType.ExpressionSymbol)
            };
        public override AstBase VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitTypeOrValueExpression(BoundTypeOrValueExpression node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitUnaryOperator(BoundUnaryOperator node, SerializationContext arg)
            => this.ConvertUnaryOperator(
                node.Operand,
                node.MethodOpt,
                node.OperatorKind,
                arg);

        private AstBase ConvertUnaryOperator(
            BoundExpression node,
            IMethodSymbol methodOpt,
            UnaryOperatorKind op,
            SerializationContext arg)
        {
            bool isLifted = IsLifted(op);
            bool isChecked = IsChecked(op);
            var typeMask = op & UnaryOperatorKind.TypeMask;
            var nscriptOp = GetNScriptOperator(op);
            if (typeMask == UnaryOperatorKind.UserDefined)
            {
                return new UserDefinedBinaryOrUnaryOpExpression
                {
                    Method = arg.SymbolSerializer.GetMethodSpecId(methodOpt),
                    IsLifted = isLifted,
                    Operator = (int)nscriptOp,
                    Location = node.Syntax.Location.GetSerLoc(),
                    Arguments = ToArgs(
                        methodOpt,
                        new BoundExpression[] { node},
                        arg)
                };
            }

            if (typeMask <= UnaryOperatorKind.Bool)
            {
                return new UnaryExpression
                {
                    IsLifted = isLifted,
                    Expression = (ExpressionSer)this.Visit(node, arg),
                    Operator = (int)nscriptOp,
                    Location = node.Syntax.Location.GetSerLoc()
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override AstBase VisitUnboundLambda(UnboundLambda node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitUsingStatement(BoundUsingStatement node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitWhileStatement(BoundWhileStatement node, SerializationContext arg)
            => new WhileStatement
            {
                Loop = (StatementSer)this.VisitToStatement(node.Body, arg),
                Condition = (ExpressionSer)this.Visit(node.Condition, arg)
            };
        public override AstBase VisitWildcardPattern(BoundWildcardPattern node, SerializationContext arg)
            {throw new NotImplementedException(); }
        public override AstBase VisitYieldBreakStatement(BoundYieldBreakStatement node, SerializationContext arg)
            => new YieldBreakStatement { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitYieldReturnStatement(BoundYieldReturnStatement node, SerializationContext arg)
            => new YieldStatement
            {
                Location = node.Syntax.GetSerLoc(),
                Expression = (ExpressionSer)this.Visit(node.Expression, arg)
            };

        public static AstBase GetConstLiteral(ConstantValue constValue)
        {
            switch (constValue.Discriminator)
            {
                case ConstantValueTypeDiscriminator.Null:
                    return new NullExpression { };
                case ConstantValueTypeDiscriminator.Bad:
                    throw new NotImplementedException();
                case ConstantValueTypeDiscriminator.SByte:
                    return new SByteLiteralExpression
                    { Value = constValue.SByteValue };
                case ConstantValueTypeDiscriminator.Byte:
                    return new ByteLiteralExpression
                    { Value = constValue.ByteValue };
                case ConstantValueTypeDiscriminator.Int16:
                    return new ShortLiteralExpression
                    { Value = constValue.Int16Value } ;
                case ConstantValueTypeDiscriminator.UInt16:
                    return new UShortLiteralExpression
                    { Value = constValue.UInt16Value } ;
                case ConstantValueTypeDiscriminator.Int32:
                    return new IntLiteralExpression
                    { Value = constValue.Int32Value };
                case ConstantValueTypeDiscriminator.UInt32:
                    return new UIntLiteralExpression
                    { Value = constValue.UInt32Value };
                case ConstantValueTypeDiscriminator.Int64:
                    return new LongLiteralExpression
                    { Value = constValue.Int64Value };
                case ConstantValueTypeDiscriminator.UInt64:
                    return new ULongLiteralExpression
                    { Value = constValue.UInt64Value };
                case ConstantValueTypeDiscriminator.Char:
                    return new CharLiteralExpression
                    { Value = constValue.CharValue };
                case ConstantValueTypeDiscriminator.Boolean:
                    return new BoolLiteralExpression
                    { Value = constValue.BooleanValue };
                case ConstantValueTypeDiscriminator.Single:
                    return new FloatLiteralExpression
                    { Value = constValue.SingleValue };
                case ConstantValueTypeDiscriminator.Double:
                    return new DoubleLiteralExpression
                    { Value = constValue.DoubleValue };
                case ConstantValueTypeDiscriminator.String:
                    return new StringLiteralExpression
                    { Value = constValue.StringValue };
                case ConstantValueTypeDiscriminator.Decimal:
                    return new DecimalLiteralExpression
                    { Value = constValue.DecimalValue };
                case ConstantValueTypeDiscriminator.DateTime:
                default:
                    throw new NotImplementedException();
            }
        }

        public static AstBase GetConstantValue(object constValue)
        {
            if (constValue == null)
            { return new NullExpression { }; }

            switch (constValue)
            {
                case byte i:
                    return new ByteLiteralExpression
                    { Value = i };

                case sbyte i:
                    return new SByteLiteralExpression
                    { Value = i };

                case char i:
                    return new CharLiteralExpression
                    { Value = i };

                case short i:
                    return new ShortLiteralExpression
                    { Value = i };

                case ushort i:
                    return new UShortLiteralExpression
                    { Value = i };

                case int i:
                    return new IntLiteralExpression
                    { Value = i };

                case uint i:
                    return new UIntLiteralExpression
                    { Value = i };

                case long i:
                    return new LongLiteralExpression
                    { Value = i };

                case ulong i:
                    return new ULongLiteralExpression
                    { Value = i };

                case float i:
                    return new FloatLiteralExpression
                    { Value = i };

                case double i:
                    return new DoubleLiteralExpression
                    { Value = i };

                case string i:
                    return new StringLiteralExpression
                    { Value = i };

                default:
                    throw new NotImplementedException();
            }
        }

        public List<MethodCallArg> ToArgs(
            IMethodSymbol method,
            IList<BoundExpression> nodes,
            SerializationContext arg)
        {
            return Enumerable
                .Range(0, method.Parameters.Length)
                .Select(_ =>
                {
                    var parameter = method.Parameters[_];

                    if (parameter.IsParams)
                    {
                        if (nodes.Count <= _)
                        {
                            return new MethodCallArg
                            {
                                Value = new ArrayCreationExpression
                                {
                                    ArrayType = arg.SymbolSerializer.GetTypeSpecId(parameter.Type),
                                    ElementType = arg.SymbolSerializer.GetTypeSpecId(
                                        ((IArrayTypeSymbol)parameter.Type).ElementType),
                                    Initializers = null,
                                    Arguments =  new List<ExpressionSer>()
                                        { new IntLiteralExpression { Value = 0 } }
                                }
                            };
                        }

                        return new MethodCallArg
                        {
                            Value = new ArrayCreationExpression
                            {
                                ArrayType = arg.SymbolSerializer.GetTypeSpecId(parameter.Type),
                                ElementType = arg.SymbolSerializer.GetTypeSpecId(
                                    ((IArrayTypeSymbol)parameter.Type).ElementType),
                                Initializers = nodes.Skip(_)
                                    .Select(_a => (ExpressionSer)this.Visit(_a, arg))
                                    .ToList()
                            }
                        };
                    }
                    else if (nodes.Count > _)
                    {
                        return new MethodCallArg
                        {
                            IsByRef = (method.Parameters[_].RefKind & RefKind.Ref) != 0,
                            Value = (ExpressionSer)this.Visit(nodes[_], arg)
                        };
                    }
                    else
                    {
                        return new MethodCallArg
                        { Value = new NullExpression { } };
                    }
                })
                .ToList();
        }

        private static UnaryOperatorKind OperatorToTypeKind(UnaryOperatorKind op)
            => op & UnaryOperatorKind.TypeMask;

        private static bool IsLifted(UnaryOperatorKind op)
            => (op & UnaryOperatorKind.Lifted) != 0;

        public static bool IsChecked(UnaryOperatorKind op)
            => (op & UnaryOperatorKind.Checked) != 0;

        private static BinaryOperatorKind OperatorToTypeKind(BinaryOperatorKind op)
            => op & BinaryOperatorKind.TypeMask;

        private static bool IsLifted(BinaryOperatorKind op)
            => (op & BinaryOperatorKind.Lifted) != 0;

        public static bool IsChecked(BinaryOperatorKind op)
            => (op & BinaryOperatorKind.Checked) != 0;

        public static bool IsLogical(BinaryOperatorKind op)
            => (op & BinaryOperatorKind.Logical) != 0;

        public static CLR.AST.UnaryOperator GetNScriptOperator(UnaryOperatorKind op)
        {
            switch (op & UnaryOperatorKind.OpMask)
            {
                case UnaryOperatorKind.PostfixIncrement:
                    return CLR.AST.UnaryOperator.PostIncrement;
                case UnaryOperatorKind.PostfixDecrement:
                    return CLR.AST.UnaryOperator.PostDecrement;
                case UnaryOperatorKind.PrefixIncrement:
                    return CLR.AST.UnaryOperator.PreIncrement;
                case UnaryOperatorKind.PrefixDecrement:
                    return CLR.AST.UnaryOperator.PreDecrement;
                case UnaryOperatorKind.UnaryPlus:
                    return CLR.AST.UnaryOperator.UnaryPlus;
                case UnaryOperatorKind.UnaryMinus:
                    return CLR.AST.UnaryOperator.UnaryMinus;
                case UnaryOperatorKind.LogicalNegation:
                    return CLR.AST.UnaryOperator.LogicalNot;
                case UnaryOperatorKind.BitwiseComplement:
                    return CLR.AST.UnaryOperator.BitwiseNot;
                case UnaryOperatorKind.True:
                case UnaryOperatorKind.False:
                default:
                    return 0;
            }
        }

        public static CLR.AST.BinaryOperator GetNScriptOperator(BinaryOperatorKind op)
        {
            switch (op & (BinaryOperatorKind.OpMask | BinaryOperatorKind.Logical))
            {
                case BinaryOperatorKind.Multiplication:
                    return CLR.AST.BinaryOperator.Mul;
                case BinaryOperatorKind.Addition:
                    return CLR.AST.BinaryOperator.Plus;
                case BinaryOperatorKind.Subtraction:
                    return CLR.AST.BinaryOperator.Minus;
                case BinaryOperatorKind.Division:
                    return CLR.AST.BinaryOperator.Div;
                case BinaryOperatorKind.Remainder:
                    return CLR.AST.BinaryOperator.Mod;
                case BinaryOperatorKind.LeftShift:
                    return CLR.AST.BinaryOperator.LeftShift;
                case BinaryOperatorKind.RightShift:
                    return CLR.AST.BinaryOperator.RightShift;
                case BinaryOperatorKind.Equal:
                    return CLR.AST.BinaryOperator.Equals;
                case BinaryOperatorKind.NotEqual:
                    return CLR.AST.BinaryOperator.NotEquals;
                case BinaryOperatorKind.GreaterThan:
                    return CLR.AST.BinaryOperator.GreaterThan;
                case BinaryOperatorKind.LessThan:
                    return CLR.AST.BinaryOperator.LessThan;
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return CLR.AST.BinaryOperator.GreaterThanOrEqual;
                case BinaryOperatorKind.LessThanOrEqual:
                    return CLR.AST.BinaryOperator.LessThanOrEqual;
                case BinaryOperatorKind.And:
                    return CLR.AST.BinaryOperator.BitwiseAnd;
                case BinaryOperatorKind.Or:
                    return CLR.AST.BinaryOperator.BitwiseOr;
                case BinaryOperatorKind.Xor:
                    return CLR.AST.BinaryOperator.BitwiseXor;
                case BinaryOperatorKind.LogicalAnd:
                    return CLR.AST.BinaryOperator.LogicalAnd;
                case BinaryOperatorKind.LogicalOr:
                    return CLR.AST.BinaryOperator.LogicalOr;
                default:
                    return 0;
            }
        }

        private T WrapInBlock<T>(BoundNode node, Func<int, T> func) where T : StatementSer
        {
            this.scopeBlockStack.AddFirst((++this.id, node));
            try
            { return func(this.id); }
            finally
            { this.scopeBlockStack.RemoveFirst(); }
        }

        private StatementSer VisitToStatement(BoundNode node, SerializationContext arg)
        {
            var ser = this.Visit(node, arg);
            var expressionSer = ser as ExpressionSer;
            if (expressionSer != null)
            {
                return new StatementExpressionSer
                { Expression = expressionSer };
            }

            return (StatementSer)ser;
        }
    }
}
