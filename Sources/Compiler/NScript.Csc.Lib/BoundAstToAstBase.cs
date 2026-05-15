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
    using System.Threading.Tasks;

    internal class BoundAstToAstBase
        : BoundAstToNotImplemented<SerializationContext, AstBase>
    {
        private int id = 0;

        private LinkedList<(int id, BoundNode nodeBlock, List<string> localFunctions)> scopeBlockStack
            = new LinkedList<(int, BoundNode, List<string>)>();

        public MethodBody GetMethodBody(
            MethodSymbol methodSymbol,
            BoundNode boundNode,
            BoundStatementList initializers,
            SerializationContext arg)
        {
            if (methodSymbol.Name.StartsWith("PostRaw")
                || methodSymbol.Name == "TestNestedFunctionScoped")
            { }

            _ = arg
                .SymbolSerializer
                .GetMethodSpecId(methodSymbol);

            var parameterBlock =
                boundNode == null
                ? null
                : GetParameterBlock(methodSymbol, boundNode, initializers, arg);

            // TODO: Make note of [Script] Attribute in case of empty body.
            var rv = new MethodBody
            {
                MethodId = arg
                    .SymbolSerializer
                    .GetMethodSpecId(methodSymbol),
                Body = parameterBlock,
                FileName = boundNode?.SyntaxTree.FilePath,
                Location = boundNode?.Syntax.GetSerLoc(),
                BlockKind = GetBlockKind(methodSymbol)
            };

            return rv;
        }

        private ParameterBlock GetParameterBlock(
            MethodSymbol methodSymbol,
            BoundNode boundNode,
            BoundStatementList initializers,
            SerializationContext arg)
            => boundNode switch
            {
                BoundBlock block => Visit(block, arg, methodSymbol, initializers),
                BoundStatementList statements => Visit(statements, arg, methodSymbol),
                _ => throw new NotSupportedException($"Unsupported method body node: {boundNode.Kind}")
            };

        public ParameterSer Visit(ParameterSymbol parameter, SerializationContext arg)
        {
            var attributes = ParameterAttributes.None;
            var paramName = parameter.Name;

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
            MethodSymbol methodSymbol,
            BoundStatementList parentBlockWithInitializers)
        {
            var rv = new ParameterBlock
            {
                Location = node.Syntax.GetSerLoc(),
                Id = ++id,
                LocalFunctions = new List<string>(),
                BlockKind = GetBlockKind(methodSymbol)
            };

            _ = scopeBlockStack.AddFirst((rv.Id, node, rv.LocalFunctions));

            // Expression can be null for static field initializer constructors.
            rv.IsMethodOwned = true;
            rv.Parameters = methodSymbol
                .Parameters
                .Select(_ => Visit(_, arg))
                .ToList();

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
                    .Select(_ => VisitToStatement(_, arg))
                    .Where(_ => _ != null)
                    .ToList();

            scopeBlockStack.RemoveFirst();
            return rv;
        }

        public ParameterBlock Visit(
            BoundStatementList node,
            SerializationContext arg,
            MethodSymbol methodSymbol)
        {
            var rv = new ParameterBlock
            {
                Location = node.Syntax.GetSerLoc(),
                Id = ++id,
                LocalFunctions = new List<string>(),
                BlockKind = GetBlockKind(methodSymbol),
                IsMethodOwned = true,
                Parameters = methodSymbol
                    .Parameters
                    .Select(_ => Visit(_, arg))
                    .ToList(),
            };

            _ = scopeBlockStack.AddFirst((rv.Id, node, rv.LocalFunctions));
            rv.Statements = node.Statements
                .Where(_ => !(_ is BoundSequencePointWithSpan))
                .Select(_ => VisitToStatement(_, arg))
                .Where(_ => _ != null)
                .ToList();
            scopeBlockStack.RemoveFirst();
            return rv;
        }

        public override AstBase DefaultVisit(BoundNode node, SerializationContext arg)
            => throw new NotImplementedException($"{node.Kind} Visit not implemented");

        public override AstBase VisitAddressOfOperator(BoundAddressOfOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node, SerializationContext arg)
            => new NewAnonymoustype
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Initializers = Enumerable
                    .Range(0, node.Declarations.Length)
                    .Select(_ =>
                    {
                        var property = node.Declarations[_];
                        var expr = node.Arguments[0];
                        return new
                        {
                            key = property.Property.Name,
                            val = (ExpressionSer)Visit(expr, arg)
                        };
                    })
                    .Where(_ => _ != null)
                    .ToDictionary(_ => _.key, _ => _.val)
            };

        public override AstBase VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitArgList(BoundArgList node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitArgListOperator(BoundArgListOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitArrayAccess(BoundArrayAccess node, SerializationContext arg)
            => new ElementAccessExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Left = (ExpressionSer)Visit(node.Expression, arg),
                Arguments = node
                    .Indices
                .Select(_ => new MethodCallArg
                {
                    IsByRef = false,
                    Value = (ExpressionSer)Visit(_, arg)
                })
                .ToList()
            };

        public override AstBase VisitArrayCreation(BoundArrayCreation node, SerializationContext arg)
            => new ArrayCreationExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                ArrayType = arg.SymbolSerializer.GetTypeSpecId(node.Type),
                ElementType = arg.SymbolSerializer.GetTypeSpecId(((ArrayTypeSymbol)node.Type).ElementType),
                Initializers = node.InitializerOpt?
                    .Initializers
                    .Select(_ => (ExpressionSer)Visit(_, arg))
                    .ToList(),
                Arguments = node.InitializerOpt != null
                    ? null
                    : node.Bounds
                        .Select(_ => (ExpressionSer)Visit(_, arg))
                        .ToList()
            };

        public override AstBase VisitArrayInitialization(BoundArrayInitialization node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitArrayLength(BoundArrayLength node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitAsOperator(BoundAsOperator node, SerializationContext arg)
            => new AsExpression
            {
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type),
                Expression = (ExpressionSer)Visit(node.Operand, arg)
            };

        public override AstBase VisitAssignmentOperator(BoundAssignmentOperator node, SerializationContext arg)
            => node.Left is BoundDiscardExpression
            ? (ExpressionSer)Visit(node.Right, arg)
            : new BinaryExpression
            {
                Left = (ExpressionSer)Visit(node.Left, arg),
                Right = (ExpressionSer)Visit(node.Right, arg),
                Operator = (int)CLR.AST.BinaryOperator.Assignment,
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitAttribute(BoundAttribute node, SerializationContext arg) => throw new NotImplementedException();

        private AstBase AwaitableValue
        { get; set; }

        public override AstBase VisitAwaitExpression(BoundAwaitExpression node, SerializationContext arg)
        {
            var expr = (ExpressionSer)Visit(node.Expression, arg);
            this.AwaitableValue = expr;
            var methodCall = (MethodCallExpression)Visit(node.AwaitableInfo.GetAwaiter, arg);

            return new AwaitExpression
            {
                GetAwaiterMethodCall = methodCall,
                Expression = expr
            };
        }

        public override AstBase VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node, SerializationContext arg)
        {
            return AwaitableValue;
        }


        public override AstBase VisitBadExpression(BoundBadExpression node, SerializationContext arg) => null;

        public override AstBase VisitBadStatement(BoundBadStatement node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitBaseReference(BoundBaseReference node, SerializationContext arg)
            => new BaseThisExpression { Location = node.Syntax.Location.GetSerLoc() };

        public override AstBase VisitBinaryOperator(BoundBinaryOperator node, SerializationContext arg)
        {
            var op = node.OperatorKind;
            var isLifted = IsLifted(op);
            _ = IsChecked(op);
            var typeMask = op & BinaryOperatorKind.TypeMask;
            if (typeMask == BinaryOperatorKind.UserDefined)
            {
                return UserDefinedBinaryOperator(
                    node,
                    arg,
                    node.BinaryOperatorMethod!,
                    node.OperatorKind & (BinaryOperatorKind.OpMask | BinaryOperatorKind.Logical),
                    IsLifted(node.OperatorKind));
            }

            var nscriptOp = GetNScriptOperator(op);
            if (typeMask <= BinaryOperatorKind.Bool
                || typeMask == BinaryOperatorKind.Enum
                || typeMask == BinaryOperatorKind.Delegate
                || typeMask == BinaryOperatorKind.Dynamic
                || nscriptOp == CLR.AST.BinaryOperator.Equals
                || nscriptOp == CLR.AST.BinaryOperator.NotEquals
                || (nscriptOp == CLR.AST.BinaryOperator.Plus
                    && (typeMask == BinaryOperatorKind.String
                        || typeMask == BinaryOperatorKind.StringAndObject
                        || typeMask == BinaryOperatorKind.ObjectAndString
                        || typeMask == BinaryOperatorKind.EnumAndUnderlying)))
            {
                return new BinaryExpression
                {
                    IsLifted = isLifted,
                    Left = (ExpressionSer)Visit(node.Left, arg),
                    Right = (ExpressionSer)Visit(node.Right, arg),
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
            MethodSymbol methodSymbol,
            BinaryOperatorKind binaryOperator,
            bool isLifted)
            => new UserDefinedBinaryOrUnaryOpExpression
            {
                Method = arg.SymbolSerializer.GetMethodSpecId(methodSymbol),
                Arguments = ToArgs(
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
            _ = scopeBlockStack.AddFirst((++id, node, new List<string>()));
            try
            {
                return new ExplicitBlockSer
                {
                    Id = id,
                    LocalFunctions = scopeBlockStack.First.Value.localFunctions,
                    Statements = node
                        .Statements
                        .Select(_ => VisitToStatement(_, arg))
                        .Where(_ => _ != null)
                        .ToList()
                };
            }
            finally
            { scopeBlockStack.RemoveFirst(); }
        }

        public override AstBase VisitBreakStatement(BoundBreakStatement node, SerializationContext arg)
            => new BreakStatement { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitCall(BoundCall node, SerializationContext arg)
        {
            // TODO: validate Method has full TypeInfo, e.g. generic arguments
            var isStatic = node.Method.ContainingSymbol.IsStatic
                || node.Method.IsStatic;

            var argumentOrdering = (node.ArgsToParamsOpt != null
                && node.ArgsToParamsOpt.Length != 0
                && IsJumbled(node.ArgsToParamsOpt.ToList()))
                ? node.ArgsToParamsOpt.ToList()
                : null;

            if (node.Method.MethodKind == MethodKind.DelegateInvoke)
            {
                return new DelegateInvocationExpression
                {
                    Arguments = ToArgs(node.Method, node.Arguments, arg),
                    Location = node.Syntax.Location.GetSerLoc(),
                    Instance = (ExpressionSer)Visit(node.ReceiverOpt, arg),
                    ArgumentOrderOpt = argumentOrdering
                };
            }

            if (node.Method.MethodKind == MethodKind.LocalFunction)
            {
                return new LocalMethodCallExpression
                {
                    MethodName = node.Method.Name,
                    ReturnType = arg.SymbolSerializer.GetTypeSpecId(
                                node.Method.ReturnType),
                    Arguments = ToArgs(node.Method, node.Arguments, arg),
                    TypeParameters = new List<int>(),
                    Location = node.Syntax.Location.GetSerLoc(),
                    ArgumentOrderOpt = argumentOrdering
                };
            }

            return new MethodCallExpression
            {
                Method = arg.SymbolSerializer.GetMethodSpecId(
                        node.Method),
                Arguments = ToArgs(node.Method, node.Arguments, arg),
                Location = node.Syntax.Location.GetSerLoc(),
                Instance = !isStatic
                        ? (ExpressionSer)Visit(node.ReceiverOpt, arg)
                        : null,
                ArgumentOrderOpt = argumentOrdering
            };
        }

        public override AstBase VisitCatchBlock(BoundCatchBlock node, SerializationContext arg)
        {
            _ = scopeBlockStack.AddFirst((++id, node, new List<string>()));
            try
            {
                return new CatchBlock
                {
                    Block = new ExplicitBlockSer
                    {
                        Id = id,
                        LocalFunctions = scopeBlockStack.First.Value.localFunctions,
                        Statements = node
                            .Body
                            .Statements
                            .Select(_ => VisitToStatement(_, arg))
                            .Where(_ => _ != null)
                            .ToList()
                    },
                    CatchType = !TypeSymbol.Equals(node.ExceptionTypeOpt, null)
                        ? arg.SymbolSerializer.GetTypeSpecId(node.ExceptionTypeOpt)
                        : (int?)null,
                    LocalVariable = node.ExceptionSourceOpt != null
                        ? ((LocalVariableRefExpression)VisitLocal((BoundLocal)node.ExceptionSourceOpt, arg)).LocalVariable
                        : null
                };
            }
            finally
            {
                scopeBlockStack.RemoveFirst();
            }
        }

        public override AstBase VisitCollectionElementInitializer(BoundCollectionElementInitializer node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitCollectionExpression(BoundCollectionExpression node, SerializationContext arg)
        {
            // Phase F1 — array (T[]) target: spread sources include T[] and List<T>.
            // Phase F4 — List<T> target: literal-only and with spread sources
            // whose static type is List<T> or T[]. Lowers to
            // `new List<T>()` + Add/AddRange calls via the existing
            // NewCollectionInitializerExpression pipeline.
            // Phase F5 — list-shaped BCL interface targets
            // (IEnumerable<T> / IList<T> / ICollection<T> /
            // IReadOnlyList<T> / IReadOnlyCollection<T>) collapse to the
            // same `new List<T>()` + Add/AddRange lowering used by F4: every
            // one of these interfaces is implemented by List<T>, and the JS
            // runtime has no notion of an "interface type", so the static
            // type information is preserved at the C# call site only.
            // Phase F5 also adds IEnumerable<T> spread sources for both
            // T[] and List<T> targets via the existing AddRange(IEnumerable<T>)
            // overload (List<T> target) or a synthesised
            // `new List<T>(); AddRange(src); ToArray()` bridge (T[] target).
            // Phase F6 — index/range sub-spreads (`[..src[1..3]]`) ride
            // transparently on top of the C# 8 indices/ranges back-fill: the
            // sub-spread source is rewritten by the VisitImplicitIndexerAccess
            // visitor into a `RuntimeHelpers.GetSubArray<T>(...)` call which
            // then flows through the existing F1 array-source converter.
            //
            // Non-Goals (out of scope, will not be supported):
            //   - [CollectionBuilder]-attributed user types — depend on
            //     Span<T>/ReadOnlySpan<T>, which are Non-Goals for NScript
            //     (see docs/language/limitations.md).
            //   - Span<T> / ReadOnlySpan<T> targets and `params Span<T>`.
            if (node.Type is ArrayTypeSymbol arrayType)
            {
                return VisitArrayCollectionExpression(node, arrayType, arg);
            }

            if (node.Type is NamedTypeSymbol namedType)
            {
                if (IsSystemCollectionsGenericList(namedType))
                {
                    return VisitListCollectionExpression(node, namedType, arg);
                }

                var listFromInterface = TryConstructListFromListShapedInterface(namedType);
                if (listFromInterface is object)
                {
                    return VisitListCollectionExpression(node, listFromInterface, arg);
                }
            }

            throw new NotSupportedException(
                $"Collection expressions targeting '{node.Type}' are not yet supported. " +
                "Phase F4 supports T[] and List<T> targets; Phase F5 supports the five " +
                "list-shaped BCL interface targets (IEnumerable<T>, IList<T>, " +
                "ICollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>). " +
                "[CollectionBuilder] types and Span<T>/ReadOnlySpan<T> are Non-Goals; " +
                "see docs/language/limitations.md.");
        }

        private AstBase VisitArrayCollectionExpression(
            BoundCollectionExpression node,
            ArrayTypeSymbol arrayType,
            SerializationContext arg)
        {
            var location = node.Syntax.Location.GetSerLoc();
            var elementType = arrayType.ElementType;

            var elements = new List<CollectionExpressionElementSer>(node.Elements.Length);
            foreach (var element in node.Elements)
            {
                CollectionExpressionElementSer serElement;
                if (element is BoundCollectionExpressionSpreadElement spread)
                {
                    serElement = new SpreadElementSer
                    {
                        Location = element.Syntax.Location.GetSerLoc(),
                        Operand = BuildArrayTargetSpreadOperand(spread, arg),
                    };
                }
                else if (element is BoundExpression expr)
                {
                    serElement = new LiteralElementSer
                    {
                        Location = element.Syntax.Location.GetSerLoc(),
                        Operand = (ExpressionSer)Visit(expr, arg),
                    };
                }
                else
                {
                    throw new NotSupportedException(
                        $"Unsupported collection-expression element kind '{element.Kind}'. "
                        + "Supported shapes are literal expressions and spread sources whose static "
                        + "type is T[], List<T>, or one of the list-shaped BCL interfaces.");
                }

                elements.Add(serElement);
            }

            return new CollectionExpressionSer
            {
                Location = location,
                ElementType = arg.SymbolSerializer.GetTypeSpecId(elementType),
                Elements = elements,
            };
        }

        // Builds the serialised operand for a spread element inside a T[]-target
        // collection expression. Array sources pass through unchanged; List<T>
        // sources are normalised by routing through `List<T>.ToArray()` so the
        // existing F1 array-source converter (`ArrayWithSpreadsConverter`) handles
        // both shapes uniformly. IEnumerable<T> sources are bridged through a
        // synthesised `new List<T>(); AddRange(src); ToArray()` chain so they
        // collapse onto the same array-source converter.
        private ExpressionSer BuildArrayTargetSpreadOperand(
            BoundCollectionExpressionSpreadElement spread,
            SerializationContext arg)
        {
            var sourceType = spread.Expression.Type;
            var visitedExpression = (ExpressionSer)Visit(spread.Expression, arg);
            var spreadLocation = spread.Syntax.Location.GetSerLoc();

            if (sourceType is ArrayTypeSymbol)
            {
                return visitedExpression;
            }

            if (sourceType is NamedTypeSymbol namedSource
                && IsSystemCollectionsGenericList(namedSource))
            {
                return BuildToArrayCall(namedSource, visitedExpression, spreadLocation, arg);
            }

            // Phase F5 — IEnumerable<T> spread source. Synthesise
            // `new List<T>(); AddRange(src); ToArray()` so the existing
            // F1 array-source converter handles the result uniformly.
            // The element type is recovered from the IEnumerable<T> the
            // bound expression carries (or the IEnumerable<T> any other
            // list-shaped BCL interface is constructed over).
            if (sourceType is NamedTypeSymbol enumerableSource
                && TryGetListShapedInterfaceElementType(enumerableSource) is TypeSymbol elementType
                && TryFindSiblingListType(enumerableSource) is NamedTypeSymbol siblingList)
            {
                var listOfT = siblingList.Construct(elementType);
                return BuildEnumerableToArrayBridge(
                    listOfT,
                    enumerableSource,
                    visitedExpression,
                    spreadLocation,
                    arg);
            }

            throw new NotSupportedException(
                $"Spread element source type '{sourceType}' is not yet supported "
                + "inside a T[] target collection expression. Phase F5 supports "
                + "T[], List<T>, and the list-shaped BCL interfaces "
                + "(IEnumerable<T>, IList<T>, ICollection<T>, IReadOnlyList<T>, "
                + "IReadOnlyCollection<T>) as spread sources.");
        }

        // Emits a `<source>.ToArray()` MethodCallExpression for a constructed
        // List<T> source. Centralised so the same shape is reused by the
        // direct-List path and the IEnumerable bridge below.
        private static MethodCallExpression BuildToArrayCall(
            NamedTypeSymbol listType,
            ExpressionSer instance,
            JsCsc.Lib.Serialization.LocationSer location,
            SerializationContext arg)
        {
            var toArray = listType
                .GetMembers("ToArray")
                .OfType<MethodSymbol>()
                .FirstOrDefault(m => m.ParameterCount == 0
                    && m.MethodKind == MethodKind.Ordinary);

            if (toArray == null)
            {
                throw new InvalidOperationException(
                    $"Could not find parameterless 'ToArray' method on '{listType}'. "
                    + "Required to lower a List<T> spread source into a T[] target collection expression.");
            }

            return new MethodCallExpression
            {
                Location = location,
                Method = arg.SymbolSerializer.GetMethodSpecId(toArray),
                Instance = instance,
                Arguments = new List<MethodCallArg>(),
            };
        }

        // Bridges an IEnumerable<T>-shaped spread source into a T[] target by
        // synthesising `new List<T>() { AddRange(src) }.ToArray()`. The
        // intermediate List<T> is built via NewCollectionInitializerExpression
        // (the same shape used by VisitListCollectionExpression) so no new
        // ProtoBuf tag or Stage-2 converter is needed; ToArray() collapses the
        // result onto the existing F1 array-source converter path.
        private static ExpressionSer BuildEnumerableToArrayBridge(
            NamedTypeSymbol listType,
            NamedTypeSymbol enumerableSourceType,
            ExpressionSer source,
            JsCsc.Lib.Serialization.LocationSer location,
            SerializationContext arg)
        {
            var ctor = ResolveParameterlessCtor(
                listType,
                "bridge an IEnumerable<T> spread source into a T[] target");

            var addRange = ResolveAddRangeOverload(listType, enumerableSourceType);
            if (addRange == null)
            {
                throw new InvalidOperationException(
                    $"Could not find an 'AddRange' overload on '{listType}' accepting "
                    + $"'{enumerableSourceType}'. Required to bridge an IEnumerable<T> "
                    + "spread source into a T[] target collection expression.");
            }

            var listExpression = new NewCollectionInitializerExpression
            {
                Location = location,
                Type = arg.SymbolSerializer.GetTypeSpecId(listType),
                Method = arg.SymbolSerializer.GetMethodSpecId(ctor),
                Arguments = new List<MethodCallArg>(),
                ItemInitializers = new List<MethodCallExpression>
                {
                    new MethodCallExpression
                    {
                        Location = location,
                        Method = arg.SymbolSerializer.GetMethodSpecId(addRange),
                        Instance = null,
                        Arguments = new List<MethodCallArg>
                        {
                            new MethodCallArg { Value = source },
                        },
                    },
                },
            };

            return BuildToArrayCall(listType, listExpression, location, arg);
        }

        // Lowers a C# 12 collection expression whose target is List<T> into a
        // NewCollectionInitializerExpression: `new List<T>()` plus a sequence of
        // `Add(elem)` / `AddRange(spread)` calls. This reuses the existing
        // collection-initializer Stage-2 path (InlineCollectionInitializationExpression),
        // so no new ProtoBuf tag or Stage-2 converter is needed.
        // Also reused by the F5 BCL-interface dispatch
        // (TryConstructListFromListShapedInterface): when the bound node's
        // static type is IEnumerable<T>/IList<T>/IReadOnlyList<T>/
        // ICollection<T>/IReadOnlyCollection<T>, NScript constructs the
        // sibling List<T> in the same namespace and re-enters here.
        private AstBase VisitListCollectionExpression(
            BoundCollectionExpression node,
            NamedTypeSymbol listType,
            SerializationContext arg)
        {
            var location = node.Syntax.Location.GetSerLoc();

            var ctor = ResolveParameterlessCtor(
                listType,
                "lower a List<T>-target collection expression");

            // For List<T> collection-expression targets Roslyn does NOT
            // pre-resolve each literal element into a
            // BoundCollectionElementInitializer (that shape is reserved for
            // explicit collection-initializer syntax `new List<T>{ 1, 2 }`).
            // Instead, literal elements arrive as raw BoundExpression nodes and
            // we have to look up `Add(T)` on the constructed `List<T>` ourselves.
            // Spread elements arrive as BoundCollectionExpressionSpreadElement
            // and require picking the right `AddRange(...)` overload.
            var addMethod = ResolveAddMethod(listType);
            if (addMethod == null)
            {
                throw new InvalidOperationException(
                    $"Could not find a single-parameter 'Add' overload on '{listType}'. "
                    + "Required to lower a List<T>-target collection expression.");
            }

            var itemInitializers = new List<MethodCallExpression>(node.Elements.Length);
            foreach (var element in node.Elements)
            {
                if (element is BoundCollectionElementInitializer addCall)
                {
                    itemInitializers.Add(new MethodCallExpression
                    {
                        Location = element.Syntax.Location.GetSerLoc(),
                        Method = arg.SymbolSerializer.GetMethodSpecId(addCall.AddMethod),
                        Instance = null,
                        Arguments = ToArgs(addCall.AddMethod, addCall.Arguments, arg),
                    });
                }
                else if (element is BoundCollectionExpressionSpreadElement spread)
                {
                    var addRange = ResolveAddRangeOverload(listType, spread.Expression.Type);
                    if (addRange == null)
                    {
                        throw new InvalidOperationException(
                            $"No 'AddRange' overload on '{listType}' accepts spread source type "
                            + $"'{spread.Expression.Type}'. Required to lower a non-array spread "
                            + "source into a List<T> target.");
                    }

                    itemInitializers.Add(new MethodCallExpression
                    {
                        Location = spread.Syntax.Location.GetSerLoc(),
                        Method = arg.SymbolSerializer.GetMethodSpecId(addRange),
                        Instance = null,
                        Arguments = new List<MethodCallArg>
                        {
                            new MethodCallArg
                            {
                                Value = (ExpressionSer)Visit(spread.Expression, arg),
                            },
                        },
                    });
                }
                else if (element is BoundExpression expr)
                {
                    itemInitializers.Add(new MethodCallExpression
                    {
                        Location = element.Syntax.Location.GetSerLoc(),
                        Method = arg.SymbolSerializer.GetMethodSpecId(addMethod),
                        Instance = null,
                        Arguments = new List<MethodCallArg>
                        {
                            new MethodCallArg
                            {
                                Value = (ExpressionSer)Visit(expr, arg),
                            },
                        },
                    });
                }
                else
                {
                    throw new NotSupportedException(
                        $"Unsupported collection-expression element kind '{element.Kind}'. "
                        + "Phase F4 supports literal expressions and spread sources for List<T> targets.");
                }
            }

            return new NewCollectionInitializerExpression
            {
                Location = location,
                Type = arg.SymbolSerializer.GetTypeSpecId(listType),
                Method = arg.SymbolSerializer.GetMethodSpecId(ctor),
                Arguments = new List<MethodCallArg>(),
                ItemInitializers = itemInitializers,
            };
        }

        // Resolve the parameterless `.ctor()` on `listType`. Throws with a
        // contextual message that names the caller's lowering so failures point
        // at the right collection-expression dispatch path.
        private static MethodSymbol ResolveParameterlessCtor(
            NamedTypeSymbol listType,
            string callerContext)
        {
            var ctor = listType
                .GetMembers(WellKnownMemberNames.InstanceConstructorName)
                .OfType<MethodSymbol>()
                .FirstOrDefault(m => m.ParameterCount == 0);

            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"Could not find parameterless constructor on '{listType}'. "
                    + $"Required to {callerContext}.");
            }

            return ctor;
        }

        // Resolve `List<T>.Add(T)`. The constructed `listType` already has T
        // substituted in member signatures.
        private static MethodSymbol ResolveAddMethod(NamedTypeSymbol listType)
        {
            return listType
                .GetMembers("Add")
                .OfType<MethodSymbol>()
                .FirstOrDefault(m => m.ParameterCount == 1
                    && m.MethodKind == MethodKind.Ordinary
                    && !m.IsStatic);
        }

        // Picks the best `AddRange(...)` overload on `listType` that accepts a
        // single argument of `sourceType`. Mirrors C# overload resolution: prefer
        // an exact-match overload, then any overload whose parameter the source
        // is assignment-compatible with. Returns null if none qualifies.
        private static MethodSymbol ResolveAddRangeOverload(
            NamedTypeSymbol listType,
            TypeSymbol sourceType)
        {
            var candidates = listType
                .GetMembers("AddRange")
                .OfType<MethodSymbol>()
                .Where(m => m.ParameterCount == 1 && m.MethodKind == MethodKind.Ordinary)
                .ToList();

            var exact = candidates.FirstOrDefault(m => m.Parameters[0].Type
                .Equals(sourceType, TypeCompareKind.AllIgnoreOptions));
            if (exact != null)
            {
                return exact;
            }

            // Fall back to the IEnumerable<T> overload — every supported source
            // (T[], List<T>, IEnumerable<T>) implements IEnumerable<T>.
            return candidates.FirstOrDefault(m =>
                m.Parameters[0].Type is NamedTypeSymbol p
                && p.OriginalDefinition.SpecialType
                    == SpecialType.System_Collections_Generic_IEnumerable_T);
        }

        // True iff `type` is the constructed `System.Collections.Generic.List<T>`.
        private static bool IsSystemCollectionsGenericList(NamedTypeSymbol type)
            => IsSystemCollectionsGenericArity1(type, "List`1");

        // True iff `type` is one of the five list-shaped BCL interfaces in
        // System.Collections.Generic over a single type parameter.
        private static bool IsListShapedSystemCollectionsGenericInterface(NamedTypeSymbol type)
            => IsSystemCollectionsGenericArity1(type, "IEnumerable`1")
                || IsSystemCollectionsGenericArity1(type, "IList`1")
                || IsSystemCollectionsGenericArity1(type, "ICollection`1")
                || IsSystemCollectionsGenericArity1(type, "IReadOnlyList`1")
                || IsSystemCollectionsGenericArity1(type, "IReadOnlyCollection`1");

        // Anchors a name match against the global namespace
        // System.Collections.Generic. Naked metadata-name matching false-
        // positives on user types named e.g. `List<T>` in nested namespaces
        // (per the WI-47 learning recorded after the F4 review).
        private static bool IsSystemCollectionsGenericArity1(NamedTypeSymbol type, string metadataName)
        {
            if (type is null || type.Arity != 1)
            { return false; }

            var def = type.OriginalDefinition;
            if (def?.MetadataName != metadataName)
            { return false; }

            var ns = def.ContainingNamespace;
            return ns != null
                && ns.Name == "Generic"
                && ns.ContainingNamespace?.Name == "Collections"
                && ns.ContainingNamespace?.ContainingNamespace?.Name == "System"
                && ns.ContainingNamespace?.ContainingNamespace?.ContainingNamespace?.IsGlobalNamespace == true;
        }

        // For a target type that is one of the list-shaped BCL interfaces,
        // returns a constructed `System.Collections.Generic.List<T>` over the
        // same element type so the F4 list-target lowering can take over.
        // Returns null when `type` is not one of the five interfaces or when
        // a sibling `List<T>` cannot be located in the same containing
        // namespace (e.g. the test compilation references a stripped
        // mscorlib facade that does not declare `List<T>`).
        private static NamedTypeSymbol TryConstructListFromListShapedInterface(NamedTypeSymbol type)
        {
            if (!IsListShapedSystemCollectionsGenericInterface(type))
            { return null; }

            var elementType = type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.FirstOrDefault().Type;
            if (elementType is null)
            { return null; }

            var sibling = TryFindSiblingListType(type);
            if (sibling is null)
            { return null; }

            return sibling.Construct(elementType);
        }

        // Recovers the element type T from a list-shaped BCL interface
        // (IEnumerable<T>/IList<T>/etc.). Returns null for any other type.
        private static TypeSymbol TryGetListShapedInterfaceElementType(NamedTypeSymbol type)
        {
            if (!IsListShapedSystemCollectionsGenericInterface(type))
            { return null; }

            return type.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.FirstOrDefault().Type;
        }

        // Looks up the unconstructed `List`1` definition in the same
        // System.Collections.Generic namespace that contains `sibling`.
        // Centralised so the interface-target dispatch and the IEnumerable
        // spread bridge share lookup semantics.
        private static NamedTypeSymbol TryFindSiblingListType(NamedTypeSymbol sibling)
        {
            var ns = sibling?.OriginalDefinition?.ContainingNamespace;
            if (ns == null)
            { return null; }

            return ns
                .GetTypeMembers("List", 1)
                .OfType<NamedTypeSymbol>()
                .FirstOrDefault();
        }

        public override AstBase VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node, SerializationContext arg)
        {
            var op = node.Operator.Kind;
            var isLifted = IsLifted(op);
            _ = IsChecked(op);
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
                    Left = (ExpressionSer)Visit(node.Left, arg),
                    Right = (ExpressionSer)Visit(node.Right, arg),
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
            => new ConditionalAccess
            {
                Receiver = (ExpressionSer)Visit(node.Receiver, arg),
                AccessExpression = (ExpressionSer)Visit(node.AccessExpression, arg),
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitConditionalGoto(BoundConditionalGoto node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitConditionalOperator(BoundConditionalOperator node, SerializationContext arg)
            => new ConditionalExpression
            {
                Condition = (ExpressionSer)Visit(node.Condition, arg),
                TrueExpression = (ExpressionSer)Visit(node.Consequence, arg),
                FalseExpression = (ExpressionSer)Visit(node.Alternative, arg),
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitConditionalReceiver(
            BoundConditionalReceiver node,
            SerializationContext arg)
            => new ConditionalReceiver
            {
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
            };

        public override AstBase VisitConstantPattern(BoundConstantPattern node, SerializationContext arg)
            => new ConstantPattern
            {
                ConstantExpression = GetConstantValue(node.ConstantValue.Value) as ExpressionSer,
                Location = node.Syntax.GetSerLoc()
            };

        public override AstBase VisitConstructorMethodBody(BoundConstructorMethodBody node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitContinueStatement(BoundContinueStatement node, SerializationContext arg)
            => new ContinueStatement { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitConversion(BoundConversion node, SerializationContext arg)
        {
            switch (node.ConversionKind)
            {
                case ConversionKind.AnonymousFunction:
                    return VisitLambda((BoundLambda)node.Operand, arg);

                case ConversionKind.MethodGroup:
                    return new DelegateCreationExpression
                    {
                        Method = (ExpressionSer)VisitMethodGroup((BoundMethodGroup)node.Operand, arg),
                        Location = node.Syntax.GetSerLoc(),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
                    };

                case ConversionKind.Boxing:
                    return new BoxCastExpression
                    { Expression = (ExpressionSer)Visit(node.Operand, arg) };

                case ConversionKind.ImplicitReference:
                    return Visit(node.Operand, arg);

                case ConversionKind.ImplicitDynamic:
                case ConversionKind.ExplicitReference:

                // return this.Visit(node.Operand, arg);
                case ConversionKind.ExplicitEnumeration: // Cast to enum from int/short, use cast b'cause we may be using string for enum
                case ConversionKind.ImplicitEnumeration: // Converts int/short to Enum, use cast b'cause we may be using string for enum
                case ConversionKind.ExplicitNumeric:
                case ConversionKind.ExplicitDynamic:

                    // Do we really have to cast, can't we use any other short cut?
                    return new TypeCastExpression
                    {
                        Expression = (ExpressionSer)Visit(node.Operand, arg),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
                    };

                case ConversionKind.ExplicitNullable:
                    return new NullableToNormal
                    {
                        Expression = (ExpressionSer)Visit(node.Operand, arg),
                    };

                case ConversionKind.Unboxing:
                    return new TypeCastExpression
                    {
                        Expression = (ExpressionSer)Visit(node.Operand, arg),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type),
                        IsUnbox = true
                    };

                case ConversionKind.ImplicitUserDefined:
                case ConversionKind.ExplicitUserDefined:
                    return new MethodCallExpression
                    {
                        Method = arg.SymbolSerializer.GetMethodSpecId(node.SymbolOpt),
                        Arguments = new List<MethodCallArg> { new MethodCallArg { Value = (ExpressionSer)Visit(node.Operand, arg) } },
                    };

                case ConversionKind.DefaultLiteral:
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
                    return Visit(node.Operand, arg);

                case ConversionKind.NullLiteral:
                    return new NullExpression();

                case ConversionKind.NoConversion:
                case ConversionKind.ImplicitNullable:
                    return Visit(node.Operand, arg);

                case ConversionKind.ImplicitNumeric:
                    return new TypeCastExpression
                    {
                        Expression = (ExpressionSer)Visit(node.Operand, arg),
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
                    };

                case ConversionKind.ImplicitThrow:
                case ConversionKind.Deconstruction:
                case ConversionKind.SwitchExpression:
                case ConversionKind.CollectionExpression:
                case ConversionKind.ObjectCreation:
                    return Visit(node.Operand, arg);

                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ExplicitTupleLiteral:
                case ConversionKind.ExplicitTuple:
                case ConversionKind.IntPtr:
                case ConversionKind.InterpolatedString:
                case ConversionKind.StackAllocToPointerType:
                case ConversionKind.StackAllocToSpanType:
                case ConversionKind.PinnedObjectToPointer:
                default:
                    throw new NotImplementedException($"{node.ConversionKind} not supported.");
            }
        }

        public override AstBase VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, SerializationContext arg)
            => this.Visit(node.SourceTuple, arg);

        public override AstBase VisitDeclarationPattern(BoundDeclarationPattern node, SerializationContext arg)
            => new DeclarationPattern
            {
                LocalVariableOpt = node.VariableAccess != null
                    ? ((LocalVariableRefExpression)this.VisitLocal((BoundLocal)node.VariableAccess, arg)).LocalVariable
                    : null,
                DeclaredType = arg.SymbolSerializer.GetTypeSpecId(node.NarrowedType),
                Location = node.Syntax.GetSerLoc()
            };

        public override AstBase VisitDeconstructionAssignmentOperator(
            BoundDeconstructionAssignmentOperator node,
            SerializationContext arg)
        {
            var lhsArgs = node.Left.Arguments
                .Select(tupleArg => (ExpressionSer)this.Visit(tupleArg, arg))
                .ToList();
            var rightExpr = (ExpressionSer)Visit(node.Right, arg);

            // When rhs type has a `Deconstruct` method
            if (node.Right.SymbolOpt != null)
            {
                var isExtensionMethod = node.Right.SymbolOpt.IsExtensionMethod;
                var args = isExtensionMethod
                    ? new List<MethodCallArg>() { new MethodCallArg() { Value = rightExpr } }
                    : new List<MethodCallArg>();

                args.AddRange(lhsArgs.Select(_ =>
                    new MethodCallArg()
                    {
                        Value = _,
                        IsByRef = true
                    }));

                rightExpr = new MethodCallExpression()
                {
                    Instance = isExtensionMethod ? null : rightExpr,
                    Method = arg.SymbolSerializer.GetMethodSpecId(node.Right.SymbolOpt),
                    Arguments = args
                };
            }

            return new DeconstructTupleAssignment
            {
                Location = node.Syntax.GetSerLoc(),
                LHSArgs = lhsArgs,
                RightExpr = rightExpr,
                IsMethodCall = node.Right.SymbolOpt != null
            };
        }

        public override AstBase VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node, SerializationContext arg) => throw new NotImplementedException();

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
                Method = (ExpressionSer)Visit(node.Argument, arg),
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
            };

        public override AstBase VisitDiscardExpression(BoundDiscardExpression node, SerializationContext arg) =>
            new DiscardExpression
            {
                Location = node.Syntax.GetSerLoc(),
            };

        public override AstBase VisitDoStatement(BoundDoStatement node, SerializationContext arg)
            => new DoStatement
            {
                Loop = VisitToStatement(node.Body, arg),
                Condition = (ExpressionSer)Visit(node.Condition, arg)
            };

        public override AstBase VisitDup(BoundDup node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node, SerializationContext arg)
            => new DynamicIndexBinderExpression
            {
                Instance = (ExpressionSer)Visit(node.Receiver, arg),
                Index = (ExpressionSer)Visit(node.Arguments[0], arg),
                Location = node.Syntax.GetSerLoc()
            };

        public override AstBase VisitDynamicInvocation(BoundDynamicInvocation node, SerializationContext arg)
            => new DynamicMethodInvocationExpression
            {
                Location = node.Syntax.GetSerLoc(),
                Method = (ExpressionSer)Visit(node.Expression, arg),
                Arguments = node.Arguments
                    .Select(_ =>
                        new MethodCallArg
                        {
                            Location = _.Syntax.GetSerLoc(),
                            Value = (ExpressionSer)Visit(_, arg)
                        })
                    .ToList()
            };

        public override AstBase VisitDynamicMemberAccess(BoundDynamicMemberAccess node, SerializationContext arg)
            => new DynamicMemberExpression
            {
                Instance = (ExpressionSer)Visit(node.Receiver, arg),
                MemberName = node.Name,
                Location = node.Syntax.GetSerLoc()
            };

        public override AstBase VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node, SerializationContext arg)
            => new NewAnonymoustype
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Initializers = ((BoundObjectInitializerExpression)node.InitializerExpressionOpt)
                    .Initializers
                    .Select(_ =>
                    {
                        if (_.Kind == BoundKind.AssignmentOperator)
                        {
                            var assignOp = (BoundAssignmentOperator)_;
                            var initializerMember = (BoundObjectInitializerMember)assignOp.Left;
                            return new
                            {
                                key = initializerMember.Display,
                                val = (ExpressionSer)Visit(assignOp.Right, arg)
                            };
                        }
                        else
                        {
                            return null;
                        }
                    })
                    .Where(_ => _ != null)
                    .ToDictionary(_ => (string)_.key, _ => _.val)
            };

        public override AstBase VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitEventAccess(BoundEventAccess node, SerializationContext arg)
            => new EventExpression
            {
                Event = arg.SymbolSerializer.GetEventSpecId(node.EventSymbol),
                Instance = !node.EventSymbol.IsStatic
                    ? (ExpressionSer)Visit(node.ReceiverOpt, arg)
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
                    ? (ExpressionSer)Visit(node.ReceiverOpt, arg)
                    : null,
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg
                    {
                        IsByRef = false,
                        Value = (ExpressionSer)Visit(node.Argument, arg)
                    }
                }
            };

        public override AstBase VisitExpressionStatement(BoundExpressionStatement node, SerializationContext arg)
            => new StatementExpressionSer { Expression = (ExpressionSer)Visit(node.Expression, arg) };

        public override AstBase VisitFieldAccess(BoundFieldAccess node, SerializationContext arg)

        // TODO: validate FieldSymbol has full TypeInfo, e.g. generic arguments
        {
            AstBase rv;

            if (node.FieldSymbol.IsConst)
            { rv = GetConstantValue(node.FieldSymbol.ConstantValue); }
            else
            {
                rv = new FieldExpression
                {
                    Field = arg.SymbolSerializer.GetFieldSpecId(
                        node.FieldSymbol.CorrespondingTupleField
                        ?? node.FieldSymbol),
                    Instance = !node.FieldSymbol.IsStatic
                      ? (ExpressionSer)Visit(node.ReceiverOpt, arg)
                      : null
                };
            }

            rv.Location = node.Syntax.GetSerLoc();
            return rv;
        }

        public override AstBase VisitFieldEqualsValue(BoundFieldEqualsValue node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitFieldInfo(BoundFieldInfo node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitFixedStatement(BoundFixedStatement node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitForEachDeconstructStep(BoundForEachDeconstructStep node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitForEachStatement(BoundForEachStatement node, SerializationContext arg)
            => WrapInBlock(
                node,
                (id, _) =>
                    new ForEachStatement
                    {
                        BlockId = id,
                        LocalVariableName = node.IterationVariables[0].Name,
                        Collection = (ExpressionSer)Visit(node.Expression, arg),
                        Loop = VisitToStatement(node.Body, arg),
                        GetAwaiterMethodCallOpt = node.EnumeratorInfoOpt?.MoveNextAwaitableInfo?.GetAwaiter == null
                            ? null
                            : (MethodCallExpression)Visit(node.EnumeratorInfoOpt.MoveNextAwaitableInfo.GetAwaiter, arg)
                    });

        public override AstBase VisitForStatement(BoundForStatement node, SerializationContext arg)
            => WrapInBlock(
                node,
                (id, _) =>
                    new ForStatement
                    {
                        BlockId = id,
                        Initializer = VisitToStatement(node.Initializer, arg),
                        Condition = (ExpressionSer)Visit(node.Condition, arg),
                        Iterator = VisitToStatement(node.Increment, arg),
                        Loop = (StatementSer)Visit(node.Body, arg)
                    });

        public override AstBase VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitGotoStatement(BoundGotoStatement node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitHoistedFieldAccess(BoundHoistedFieldAccess node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitHostObjectMemberReference(BoundHostObjectMemberReference node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitIfStatement(BoundIfStatement node, SerializationContext arg)
            => new IfStatement
            {
                Condition = (ExpressionSer)Visit(node.Condition, arg),
                TrueStatement = VisitToStatement(node.Consequence, arg),
                FalseStatement = node.AlternativeOpt == null
                    ? null
                    : VisitToStatement(node.AlternativeOpt, arg)
            };

        public override AstBase VisitImplicitReceiver(BoundImplicitReceiver node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitIncrementOperator(BoundIncrementOperator node, SerializationContext arg)
            => ConvertUnaryOperator(
                node.Operand,
                node.MethodOpt,
                node.OperatorKind,
                arg);

        public override AstBase VisitIndexerAccess(BoundIndexerAccess node, SerializationContext arg)
            => new IndexExpression
            {
                Property = arg.SymbolSerializer.GetPropertySpecId(node.Indexer),
                Instance = node.ReceiverOpt != null ? (ExpressionSer)Visit(node.ReceiverOpt, arg) : null,
                Arguments = ToArgs(
                    node.Indexer.GetMethod,
                    node.Arguments,
                    arg)
            };

        // -----------------------------------------------------------------
        // C# 8 indices and ranges (Phase F6 back-fill).
        //
        // Roslyn binds `^x`, `x..y`, and `arr[index-or-range]` to dedicated
        // bound shapes (BoundFromEndIndexExpression / BoundRangeExpression /
        // BoundImplicitIndexerAccess) that have no overrides on the base
        // BoundAstToNotImplemented class — so they fell through to the
        // default `throw new NotImplementedException()` until this phase.
        //
        // The lowering shape is:
        //   - `^x`              → `new System.Index(x, fromEnd: true)`
        //   - `x..y`            → `new System.Range(x, y)`
        //   - `x..` / `..y` etc → `new System.Range(left ?? Index.Start,
        //                                            right ?? Index.End)`
        //   - `arr[index]`      → `arr[index.GetOffset(arr.Length)]`
        //   - `arr[range]`      → `RuntimeHelpers.GetSubArray<T>(arr, range)`
        //   - `list[index]`     → `list[index.GetOffset(list.Count)]`
        //   - `string[index]`   → `string[index.GetOffset(string.Length)]`
        //
        // Range-based slicing on List<T> / string is intentionally deferred
        // to a separate follow-up: List<T> has no Slice(int, int) member on
        // the NScript facade, and string Substring lowering needs more
        // wiring. Throw with an actionable message rather than emit broken JS.
        // -----------------------------------------------------------------

        public override AstBase VisitFromEndIndexExpression(BoundFromEndIndexExpression node, SerializationContext arg)
        {
            var location = node.Syntax.Location.GetSerLoc();
            if (!(node.Type is NamedTypeSymbol indexType))
            {
                throw new InvalidOperationException(
                    $"Expected NamedTypeSymbol for '^x' expression at {node.Syntax.Location}, "
                    + $"but was '{node.Type?.GetType().Name ?? "null"}'.");
            }
            var ctor = node.MethodOpt
                ?? indexType
                    .GetMembers(WellKnownMemberNames.InstanceConstructorName)
                    .OfType<MethodSymbol>()
                    .FirstOrDefault(m => m.ParameterCount == 2
                        && m.Parameters[0].Type.SpecialType == SpecialType.System_Int32
                        && m.Parameters[1].Type.SpecialType == SpecialType.System_Boolean);

            if (ctor is null)
            {
                throw new InvalidOperationException(
                    "System.Index..ctor(int, bool) is required to lower 'from-end' index expressions ('^x'). "
                    + "Add the constructor to the mscorlib facade.");
            }

            return new NewExpression
            {
                Location = location,
                Type = arg.SymbolSerializer.GetTypeSpecId(indexType),
                Method = arg.SymbolSerializer.GetMethodSpecId(ctor),
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg { Value = (ExpressionSer)Visit(node.Operand, arg) },
                    new MethodCallArg { Value = new BoolLiteralExpression { Value = true } },
                },
            };
        }

        public override AstBase VisitRangeExpression(BoundRangeExpression node, SerializationContext arg)
        {
            var location = node.Syntax.Location.GetSerLoc();
            if (!(node.Type is NamedTypeSymbol rangeType))
            {
                throw new InvalidOperationException(
                    $"Expected NamedTypeSymbol for range expression at {node.Syntax.Location}, "
                    + $"but was '{node.Type?.GetType().Name ?? "null"}'.");
            }

            // Always lower to `new Range(Index, Index)` — ignore node.MethodOpt
            // because Roslyn may bind it to a 0/1-parameter factory (Range.All,
            // Range.StartAt, Range.EndAt) for open-ended forms, while we want
            // a uniform 2-parameter ctor emission with synthesised boundaries.
            var ctor = rangeType
                .GetMembers(WellKnownMemberNames.InstanceConstructorName)
                .OfType<MethodSymbol>()
                .FirstOrDefault(m => m.ParameterCount == 2
                    && IsSystemIndex(m.Parameters[0].Type as NamedTypeSymbol)
                    && IsSystemIndex(m.Parameters[1].Type as NamedTypeSymbol));

            if (ctor is null)
            {
                throw new InvalidOperationException(
                    "System.Range..ctor(System.Index, System.Index) is required to lower range expressions ('..'). "
                    + "Add the constructor to the mscorlib facade.");
            }

            if (!(ctor.Parameters[0].Type is NamedTypeSymbol indexType))
            {
                throw new InvalidOperationException(
                    "System.Range..ctor first parameter type must be a NamedTypeSymbol (System.Index).");
            }

            var leftExpr = node.LeftOperandOpt != null
                ? (ExpressionSer)Visit(node.LeftOperandOpt, arg)
                : ResolveIndexBoundary(indexType, "Start", arg, location);
            var rightExpr = node.RightOperandOpt != null
                ? (ExpressionSer)Visit(node.RightOperandOpt, arg)
                : ResolveIndexBoundary(indexType, "End", arg, location);

            return new NewExpression
            {
                Location = location,
                Type = arg.SymbolSerializer.GetTypeSpecId(rangeType),
                Method = arg.SymbolSerializer.GetMethodSpecId(ctor),
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg { Value = leftExpr },
                    new MethodCallArg { Value = rightExpr },
                },
            };
        }

        public override AstBase VisitImplicitIndexerAccess(BoundImplicitIndexerAccess node, SerializationContext arg)
        {
            var location = node.Syntax.Location.GetSerLoc();
            var argType = node.Argument.Type as NamedTypeSymbol;

            if (argType is null)
            {
                throw new NotSupportedException(
                    "BoundImplicitIndexerAccess argument has no NamedTypeSymbol type; "
                    + "only System.Index and System.Range arguments are supported.");
            }

            if (IsSystemIndex(argType))
            {
                return LowerImplicitIndexAccess(node, argType, location, arg);
            }

            if (IsSystemRange(argType))
            {
                return LowerImplicitRangeAccess(node, location, arg);
            }

            throw new NotSupportedException(
                $"BoundImplicitIndexerAccess argument type '{argType}' is unsupported. "
                + "Only System.Index and System.Range arguments are recognised.");
        }

        private AstBase LowerImplicitIndexAccess(
            BoundImplicitIndexerAccess node,
            NamedTypeSymbol indexType,
            LocationSer location,
            SerializationContext arg)
        {
            var receiverType = node.Receiver.Type;
            var receiverExpr = (ExpressionSer)Visit(node.Receiver, arg);
            var indexExpr = (ExpressionSer)Visit(node.Argument, arg);

            var lengthOrCountExpr = BuildLengthOrCountAccess(receiverType, receiverExpr, location, arg);
            var getOffset = ResolveGetOffset(indexType);

            var offsetCall = new MethodCallExpression
            {
                Location = location,
                Method = arg.SymbolSerializer.GetMethodSpecId(getOffset),
                Instance = indexExpr,
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg { Value = lengthOrCountExpr },
                },
            };

            if (receiverType is ArrayTypeSymbol)
            {
                return new ElementAccessExpression
                {
                    Location = location,
                    Left = receiverExpr,
                    Arguments = new List<MethodCallArg>
                    {
                        new MethodCallArg { Value = offsetCall },
                    },
                };
            }

            var indexerProp = ResolveIntIndexer(receiverType);
            if (indexerProp is null)
            {
                throw new NotSupportedException(
                    $"Index-based indexer access on '{receiverType}' requires an 'int' indexer "
                    + "on the receiver type. Only T[], List<T>, IList<T>, IReadOnlyList<T>, "
                    + "and string are supported in this slice.");
            }

            return new IndexExpression
            {
                Property = arg.SymbolSerializer.GetPropertySpecId(indexerProp),
                Instance = receiverExpr,
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg { Value = offsetCall },
                },
            };
        }

        private AstBase LowerImplicitRangeAccess(
            BoundImplicitIndexerAccess node,
            LocationSer location,
            SerializationContext arg)
        {
            if (!(node.Receiver.Type is ArrayTypeSymbol arrayType))
            {
                throw new NotSupportedException(
                    $"Range-based indexer access on '{node.Receiver.Type}' is not supported. "
                    + "Only T[] receivers are supported in Phase F6; List<T> / string range "
                    + "slicing is deferred to a follow-up. Use 'src.ToArray()[range]' or "
                    + "'string.Substring(start, length)' as a workaround.");
            }

            var receiverExpr = (ExpressionSer)Visit(node.Receiver, arg);
            var rangeExpr = (ExpressionSer)Visit(node.Argument, arg);

            var corlib = arrayType.ElementType.ContainingAssembly
                ?? arrayType.ContainingAssembly;
            var runtimeHelpers = corlib?.GetTypeByMetadataName(
                "System.Runtime.CompilerServices.RuntimeHelpers");
            if (runtimeHelpers is null)
            {
                throw new InvalidOperationException(
                    "System.Runtime.CompilerServices.RuntimeHelpers is required to lower "
                    + "range-based indexer access on arrays. Add the type to the mscorlib facade.");
            }

            var getSubArrayDef = runtimeHelpers
                .GetMembers("GetSubArray")
                .OfType<MethodSymbol>()
                .FirstOrDefault(m => m.IsStatic
                    && m.IsGenericMethod
                    && m.Arity == 1
                    && m.ParameterCount == 2);
            if (getSubArrayDef is null)
            {
                throw new InvalidOperationException(
                    "System.Runtime.CompilerServices.RuntimeHelpers.GetSubArray<T>(T[], Range) "
                    + "is required to lower range-based indexer access on arrays. Add the method "
                    + "to the mscorlib facade.");
            }

            var getSubArray = getSubArrayDef.Construct(arrayType.ElementType);

            return new MethodCallExpression
            {
                Location = location,
                Method = arg.SymbolSerializer.GetMethodSpecId(getSubArray),
                Instance = null,
                Arguments = new List<MethodCallArg>
                {
                    new MethodCallArg { Value = receiverExpr },
                    new MethodCallArg { Value = rangeExpr },
                },
            };
        }

        private static ExpressionSer ResolveIndexBoundary(
            NamedTypeSymbol indexType,
            string propertyName,
            SerializationContext arg,
            LocationSer location)
        {
            var prop = indexType
                .GetMembers(propertyName)
                .OfType<PropertySymbol>()
                .FirstOrDefault(p => p.IsStatic && p.GetMethod != null);
            if (prop is null)
            {
                throw new InvalidOperationException(
                    $"System.Index.{propertyName} static property is required to lower "
                    + "open-ended range expressions ('x..' / '..y' / '..'). Add the property "
                    + "to the mscorlib facade.");
            }

            return new PropertyExpression
            {
                Property = arg.SymbolSerializer.GetPropertySpecId(prop),
                Instance = null,
                IsNotVirtual = false,
                Location = location,
            };
        }

        private static MethodSymbol ResolveGetOffset(NamedTypeSymbol indexType)
        {
            var method = indexType
                .GetMembers("GetOffset")
                .OfType<MethodSymbol>()
                .FirstOrDefault(m => !m.IsStatic
                    && m.ParameterCount == 1
                    && m.Parameters[0].Type.SpecialType == SpecialType.System_Int32);
            if (method is null)
            {
                throw new InvalidOperationException(
                    "System.Index.GetOffset(int) is required to lower index-based indexer "
                    + "access. Add the method to the mscorlib facade.");
            }

            return method;
        }

        private static PropertySymbol ResolveIntIndexer(TypeSymbol receiverType)
        {
            if (!(receiverType is NamedTypeSymbol named))
            {
                return null;
            }

            return named
                .GetMembers(WellKnownMemberNames.Indexer)
                .OfType<PropertySymbol>()
                .FirstOrDefault(p => !p.IsStatic
                    && p.IsIndexer
                    && p.ParameterCount == 1
                    && p.Parameters[0].Type.SpecialType == SpecialType.System_Int32);
        }

        private static ExpressionSer BuildLengthOrCountAccess(
            TypeSymbol receiverType,
            ExpressionSer receiverExpr,
            LocationSer location,
            SerializationContext arg)
        {
            PropertySymbol prop = null;

            if (receiverType is ArrayTypeSymbol arrayType)
            {
                prop = FindInstanceProperty(arrayType.BaseTypeNoUseSiteDiagnostics, "Length");
            }
            else if (receiverType is NamedTypeSymbol named)
            {
                if (named.SpecialType == SpecialType.System_String)
                {
                    prop = FindInstanceProperty(named, "Length");
                }
                else
                {
                    prop = ResolveCountProperty(named);
                }
            }

            if (prop is null)
            {
                throw new NotSupportedException(
                    $"Could not resolve a Length or Count property on receiver type "
                    + $"'{receiverType}'. Required to lower implicit indexer access with an "
                    + "Index or Range argument. Only T[], List<T>, IList<T>, IReadOnlyList<T>, "
                    + "and string are supported in this slice.");
            }

            return new PropertyExpression
            {
                Property = arg.SymbolSerializer.GetPropertySpecId(prop),
                Instance = !prop.IsStatic ? receiverExpr : null,
                IsNotVirtual = false,
                Location = location,
            };
        }

        private static PropertySymbol ResolveCountProperty(NamedTypeSymbol type)
        {
            var direct = FindInstanceProperty(type, "Count");
            if (direct != null)
            {
                return direct;
            }

            foreach (var iface in type.AllInterfacesNoUseSiteDiagnostics)
            {
                var fromIface = FindInstanceProperty(iface, "Count");
                if (fromIface != null)
                {
                    return fromIface;
                }
            }

            return null;
        }

        private static PropertySymbol FindInstanceProperty(TypeSymbol type, string name)
            => type?
                .GetMembers(name)
                .OfType<PropertySymbol>()
                .FirstOrDefault(p => !p.IsStatic && p.ParameterCount == 0);

        // True iff `type` is `System.Index`, anchored to the global namespace.
        private static bool IsSystemIndex(NamedTypeSymbol type)
            => IsSystemNamespaceType(type, "Index");

        // True iff `type` is `System.Range`, anchored to the global namespace.
        private static bool IsSystemRange(NamedTypeSymbol type)
            => IsSystemNamespaceType(type, "Range");

        private static bool IsSystemNamespaceType(NamedTypeSymbol type, string metadataName)
        {
            if (type is null || type.MetadataName != metadataName)
            {
                return false;
            }

            var ns = type.ContainingNamespace;
            return ns?.Name == "System"
                && ns.ContainingNamespace?.IsGlobalNamespace == true;
        }

        public override AstBase VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitInterpolatedString(BoundInterpolatedString node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitIsOperator(BoundIsOperator node, SerializationContext arg)
            => new IsExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Expression = (ExpressionSer)Visit(node.Operand, arg),
                Type = arg.SymbolSerializer.GetTypeSpecId((TypeSymbol)node.TargetType.ExpressionSymbol)
            };

        public override AstBase VisitIsPatternExpression(BoundIsPatternExpression node, SerializationContext arg)
            => new IsPatternExpression
            {
                Lhs = Visit(node.Expression, arg) as ExpressionSer,
                Pattern = Visit(node.Pattern, arg) as Pattern
            };

        public override AstBase VisitRelationalPattern(BoundRelationalPattern node, SerializationContext arg)
            => new RelationalPattern
            {
                Operator = (int)GetNScriptOperator(node.Relation),
                ConstantExpression = GetConstLiteral(node.ConstantValue) as ExpressionSer,
                Location = node.Syntax.GetSerLoc()
            };

        public override AstBase VisitBinaryPattern(BoundBinaryPattern node, SerializationContext arg)
            => new BinaryPattern
            {
                Disjunction = node.Disjunction,
                Left = (Pattern)Visit(node.Left, arg),
                Right = (Pattern)Visit(node.Right, arg),
                Location = node.Syntax.GetSerLoc()
            };

        public override AstBase VisitNegatedPattern(BoundNegatedPattern node, SerializationContext arg)
            => new NegatedPattern
            {
                Inner = (Pattern)Visit(node.Negated, arg),
                Location = node.Syntax.GetSerLoc()
            };


        public override AstBase VisitLabel(BoundLabel node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitLabeledStatement(BoundLabeledStatement node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitLabelStatement(BoundLabelStatement node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitLambda(BoundLambda node, SerializationContext arg)
        {
            var parameterBlock = Visit(
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

        public override AstBase VisitLocalFunctionStatement(
            BoundLocalFunctionStatement node,
            SerializationContext arg)
        {
            scopeBlockStack.First.Value.localFunctions.Add(node.Symbol.Name);

            var block =
                Visit(
                    node.Body,
                    arg,
                    node.Symbol,
                    null);

            return new LocalMethodStatement
            {
                Block = block,
                ScopeBlockId = scopeBlockStack.First.Value.id,
                MethodId = new LocalMethodIdentitySer
                {
                    MethodName = node.Symbol.Name,
                    ReturnType = arg.SymbolSerializer.GetTypeSpecId(node.Symbol.ReturnType),
                    GenericParameters = node.Symbol.Arity,
                    Parameters = block.Parameters
                }
            };
        }

        public override AstBase VisitLiteral(BoundLiteral node, SerializationContext arg)
        {
            var rv = GetConstLiteral(node.ConstantValueOpt!);
            rv.Location = node.Syntax.Location.GetSerLoc();
            return rv;
        }

        public override AstBase VisitLocal(BoundLocal node, SerializationContext arg)
            => new LocalVariableRefExpression
            {
                LocalVariable = GetLocalVariable(node.LocalSymbol, arg),
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitLocalDeclaration(BoundLocalDeclaration node, SerializationContext arg)
            => node.InitializerOpt == null
                ? null
                : new BinaryExpression
                {
                    Operator = (int)CLR.AST.BinaryOperator.Assignment,
                    Left = new LocalVariableRefExpression
                    {
                        Location = node.Syntax.Location.GetSerLoc(),
                        LocalVariable = GetLocalVariable(node.LocalSymbol, arg)
                    },
                    Right = (ExpressionSer)Visit(node.InitializerOpt, arg)
                };

        private LocalVariableSer GetLocalVariable(LocalSymbol localSymbol, SerializationContext arg)
        {
            var id = -1;
            foreach (var node in scopeBlockStack)
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
                Type = arg.SymbolSerializer.GetTypeSpecId(localSymbol.Type),
                BlockId = id
            };
        }

        public override AstBase VisitLockStatement(BoundLockStatement node, SerializationContext arg)
            => Visit(node.Body, arg);

        public override AstBase VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitMakeRefOperator(BoundMakeRefOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitMethodDefIndex(BoundMethodDefIndex node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitMethodGroup(BoundMethodGroup node, SerializationContext arg)
        {
            var method = node.Methods[0];
            if (method.MethodKind == MethodKind.LocalFunction)
            {
                return new LocalMethodExpression
                {
                    MethodName = method.Name,
                    ReturnType = arg.SymbolSerializer.GetTypeSpecId(method.ReturnType),
                    GenericParameters = node.TypeArgumentsOpt != null
                        ? node.TypeArgumentsOpt
                            .Select(t => arg.SymbolSerializer.GetTypeSpecId(t.Type))
                            .ToList()
                        : null,
                    Location = node.Syntax.Location.GetSerLoc(),
                };
            }
            else
            {
                return new MethodExpression
                {
                    Location = node.Syntax.Location.GetSerLoc(),
                    Method = arg.SymbolSerializer.GetMethodSpecId(method),
                    GenericParameters = node.TypeArgumentsOpt != null
                        ? node.TypeArgumentsOpt
                            .Select(t => arg.SymbolSerializer.GetTypeSpecId(t.Type))
                            .ToList()
                        : null,
                    Instance = !method.IsStatic
                        ? (ExpressionSer)Visit(node.ReceiverOpt, arg)
                        : null
                };
            }
        }

        public override AstBase VisitMethodInfo(BoundMethodInfo node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitModuleVersionId(BoundModuleVersionId node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitModuleVersionIdString(BoundModuleVersionIdString node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitMultipleLocalDeclarations(BoundMultipleLocalDeclarations node, SerializationContext arg)
        {
            var initializers = node
                    .LocalDeclarations
                    .Select(_ => (ExpressionSer)Visit(_, arg))
                    .Where(e => e != null)
                    .ToList();

            var location = node.Syntax.GetSerLoc();

            return initializers.Any()
                ? (StatementSer)new VariableBlockDeclaration
                {
                    Location = location,
                    Initializers = initializers
                }
                : new EmptyStatementSer { Location = location };
        }

        public override AstBase VisitNameOfOperator(BoundNameOfOperator node, SerializationContext arg)
            => (ExpressionSer)GetConstLiteral(node.ConstantValueOpt);

        public override AstBase VisitNamespaceExpression(BoundNamespaceExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitNewT(BoundNewT node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitNonConstructorMethodBody(BoundNonConstructorMethodBody node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitNoOpStatement(BoundNoOpStatement node, SerializationContext arg)
            => new EmptyStatementSer { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitNullCoalescingOperator(BoundNullCoalescingOperator node, SerializationContext arg)
            => new NullCoalescingOperatorSer
            {
                Location = node.Syntax.GetSerLoc(),
                Left = (ExpressionSer)Visit(node.LeftOperand, arg),
                Right = (ExpressionSer)Visit(node.RightOperand, arg),
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type)
            };

        public override AstBase VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node, SerializationContext arg)
            => new NullCoalescingAssignmentSer
            {
                Location = node.Syntax.GetSerLoc(),
                Left = (ExpressionSer)Visit(node.LeftOperand, arg),
                Right = (ExpressionSer)Visit(node.RightOperand, arg)
            };

        public override AstBase VisitObjectCreationExpression(BoundObjectCreationExpression node, SerializationContext arg)
        {
            var location = node.Syntax.Location.GetSerLoc();
            var type = arg.SymbolSerializer.GetTypeSpecId(node.Type);
            var method = node.Constructor.IsDefaultValueTypeConstructor()
                ? 0
                : arg.SymbolSerializer.GetMethodSpecId(node.Constructor);

            var arguments = ToArgs(node.Constructor, node.Arguments, arg);

            if (node.InitializerExpressionOpt == null)
            {
                var argumentOrdering = (node.ArgsToParamsOpt != null
                    && node.ArgsToParamsOpt.Length != 0
                    && IsJumbled(node.ArgsToParamsOpt.ToList()))
                    ? node.ArgsToParamsOpt.ToList()
                    : null;

                return new NewExpression
                {
                    Location = location,
                    Type = type,
                    Method = method,
                    Arguments = arguments,
                    ArgumentOrderOpt = argumentOrdering
                };
            }

            if (node.InitializerExpressionOpt is BoundObjectInitializerExpression)
            {
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
                                    Value = (ExpressionSer)Visit(assignOp.Right, arg)
                                };

                                if (initializerMember.MemberSymbol.Kind == SymbolKind.Field)
                                {
                                    rv.Field = arg.SymbolSerializer.GetFieldSpecId(
                                        (FieldSymbol)initializerMember.MemberSymbol);
                                }
                                else
                                {
                                    var propertySymbol = (PropertySymbol)initializerMember.MemberSymbol;
                                    rv.Property = arg.SymbolSerializer.GetPropertySpecId(propertySymbol);
                                    rv.Setter = arg.SymbolSerializer.GetMethodSpecId(propertySymbol.SetMethod);
                                    rv.Getter = propertySymbol.GetMethod != null
                                        ? arg.SymbolSerializer.GetMethodSpecId(propertySymbol.GetMethod)
                                        : 0;

                                    rv.PropertyArgs = ToArgs(
                                        propertySymbol.GetMethod,
                                        initializerMember.Arguments,
                                        arg);
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
            else
            {
                return new NewCollectionInitializerExpression
                {
                    Location = location,
                    Type = type,
                    Method = method,
                    Arguments = arguments,
                    ItemInitializers = ((BoundCollectionInitializerExpression)node.InitializerExpressionOpt)
                        .Initializers
                        .Select(item =>
                        {
                            if (item.Kind == BoundKind.CollectionElementInitializer)
                            {
                                var elementInitializer = (BoundCollectionElementInitializer)item;
                                return new MethodCallExpression
                                {
                                    Method = arg.SymbolSerializer.GetMethodSpecId(elementInitializer.AddMethod),
                                    Arguments = ToArgs(elementInitializer.AddMethod, elementInitializer.Arguments, arg),
                                    Location = node.Syntax.Location.GetSerLoc(),
                                    Instance = null
                                };
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }
                        })
                        .ToList()
                };
            }
        }

        public override AstBase VisitObjectInitializerExpression(BoundObjectInitializerExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitObjectInitializerMember(BoundObjectInitializerMember node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitOutVariablePendingInference(OutVariablePendingInference node, SerializationContext arg) => throw new NotImplementedException();

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
                        Type = arg.SymbolSerializer.GetTypeSpecId(node.ParameterSymbol.Type),
                        Modifier = (int)attrs
                    }
                };
        }

        public override AstBase VisitParameterEqualsValue(BoundParameterEqualsValue node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPassByCopy(BoundPassByCopy node, SerializationContext arg) => throw new NotImplementedException();

        /*
        public override AstBase VisitPatternSwitchLabel(BoundPatternSwitchLabel node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPatternSwitchSection(BoundPatternSwitchSection node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPatternSwitchStatement(BoundPatternSwitchStatement node, SerializationContext arg) => throw new NotImplementedException();
        */

        public override AstBase VisitPointerElementAccess(BoundPointerElementAccess node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPropertyAccess(BoundPropertyAccess node, SerializationContext arg)
            => new PropertyExpression
            {
                Property = arg.SymbolSerializer.GetPropertySpecId(node.PropertySymbol),
                Instance = !node.PropertySymbol.IsStatic ? (ExpressionSer)Visit(node.ReceiverOpt, arg) : null,
                IsNotVirtual = node.SuppressVirtualCalls,
                Location = node.Syntax.Location.GetSerLoc()
            };

        public override AstBase VisitPropertyEqualsValue(BoundPropertyEqualsValue node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPropertyGroup(BoundPropertyGroup node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitPseudoVariable(BoundPseudoVariable node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitQueryClause(BoundQueryClause node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitRangeVariable(BoundRangeVariable node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitRefTypeOperator(BoundRefTypeOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitRefValueOperator(BoundRefValueOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitReturnStatement(BoundReturnStatement node, SerializationContext arg)
            => new ReturnStatement
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Expression = node.ExpressionOpt != null
                    ? (ExpressionSer)Visit(node.ExpressionOpt, arg)
                    : null
            };

        public override AstBase VisitScope(BoundScope node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitSequence(BoundSequence node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitSequencePoint(BoundSequencePoint node, SerializationContext arg)
            => node.StatementOpt == null
                ? null
                : Visit(node.StatementOpt, arg);

        public override AstBase VisitSequencePointExpression(BoundSequencePointExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitSequencePointWithSpan(BoundSequencePointWithSpan node, SerializationContext arg)
            => node.StatementOpt == null
                ? null
                : Visit(node.StatementOpt, arg);

        public override AstBase VisitSizeOfOperator(BoundSizeOfOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitSourceDocumentIndex(BoundSourceDocumentIndex node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitStateMachineScope(BoundStateMachineScope node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitStatementList(BoundStatementList node, SerializationContext arg)
            => new StatementListSer
            {
                Statements = node
                    .Statements
                    .Select(_ => VisitToStatement(_, arg))
                    .Where(_ => _ != null)
                    .ToList()
            };

        public override AstBase VisitStringInsert(BoundStringInsert node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node, SerializationContext arg)
        {
            var arms = new List<Pattern>();
            var exprs = new List<ExpressionSer>();

            foreach (var switchArm in node.SwitchArms)
            {
                Pattern pattern = switchArm.Pattern switch
                {
                    // BoundRelationalPattern is a subclass of BoundConstantPattern; it must match first.
                    // Delegate to VisitRelationalPattern to avoid duplicating its Location wiring.
                    BoundRelationalPattern relPattern => (Pattern)Visit(relPattern, arg),

                    BoundConstantPattern constPattern =>
                        new ConstantPattern
                        {
                            ConstantExpression = GetConstLiteral(constPattern.ConstantValue) as ExpressionSer
                        },

                    BoundDeclarationPattern declarationPattern =>
                        new DeclarationPattern
                        {
                            LocalVariableOpt = declarationPattern.VariableAccess is BoundLocal local
                                ? ((LocalVariableRefExpression)VisitLocal(local, arg)).LocalVariable
                                : null,

                            DeclaredType = arg.SymbolSerializer.GetTypeSpecId(declarationPattern.DeclaredType.Type),

                            When = switchArm.WhenClause != null
                                ? (ExpressionSer)Visit(switchArm.WhenClause, arg)
                                : null,
                        },

                    BoundDiscardPattern _ => new DiscardPattern { },

                    // C# 9-11 patterns dispatch through their per-pattern visitors.
                    // Switch arms attach their `when` clause via WhenExpressionOpt on the dispatched
                    // pattern by wrapping in a BinaryPattern (and-style) below.
                    _ => (Pattern)Visit(switchArm.Pattern, arg)
                };

                if (switchArm.WhenClause != null
                    && pattern is not DeclarationPattern
                    && pattern is not ConstantPattern)
                {
                    // We only support 'when' on declaration/constant patterns today.
                    // For the new pattern shapes, surface a clear error early.
                    throw new NotImplementedException(
                        "'when' clauses are only supported on declaration and constant patterns. "
                        + "Track via docs/language/csharp9-13-status.md.");
                }

                var expr = Visit(switchArm.Value, arg) as ExpressionSer;

                exprs.Add(expr);
                arms.Add(pattern);
            }

            return new SwitchExpression
            {
                SwitchExpr = Visit(node.Expression, arg) as ExpressionSer,
                Patterns = arms,
                Expressions = exprs,
                Type = arg.SymbolSerializer.GetTypeSpecId(node.Type),
            };
        }

        public override AstBase VisitSwitchLabel(BoundSwitchLabel node, SerializationContext arg)
        {
            throw new NotImplementedException();
            // => node.WhenClause != null
            //     ? (ExpressionSer)Visit(node.WhenClause, arg)
            //     : (ExpressionSer)GetConstLiteral(node.ConstantValueOpt);
        }

        public override AstBase VisitSwitchSection(BoundSwitchSection node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitSwitchStatement(BoundSwitchStatement node, SerializationContext arg)
        {
            return WrapInBlock(node, (id, _) =>
            {
                bool isElseIfStatement = false;
                var switchSections = new List<SwitchSectionSer>();
                foreach (var section in node.SwitchSections)
                {
                    switchSections.Add(this.WrapInBlock(section, (id, _) =>
                    {
                        var caseLabels = new List<Pattern>();
                        foreach (var label in section.SwitchLabels)
                        {
                            // Also check BoundSwitchLabel visitor.

                            if (label.Pattern.Kind == BoundKind.ConstantPattern)
                            {
                                var constPattern = (BoundConstantPattern)label.Pattern;
                                caseLabels.Add(
                                    new ConstantPattern
                                    {
                                        ConstantExpression = (ExpressionSer)GetConstLiteral(constPattern.ConstantValue)
                                    });
                            }
                            else if (label.Pattern.Kind == BoundKind.DiscardPattern)
                            {
                                caseLabels.Add(new DiscardPattern());
                            }
                            else if (label.Pattern.Kind == BoundKind.DeclarationPattern)
                            {
                                var declaredTypeOpt = ((BoundDeclarationPattern)label.Pattern).DeclaredType.Type;
                                var ty = arg.SymbolSerializer.GetTypeSpecId(declaredTypeOpt);

                                var boundPattern = (BoundDeclarationPattern)label.Pattern;
                                BoundLocal? boundLocalOpt = (BoundLocal?)boundPattern.VariableAccess;

                                caseLabels.Add(new DeclarationPattern()
                                {
                                    LocalVariableOpt = boundLocalOpt != null
                                        ? ((LocalVariableRefExpression)this.VisitLocal(boundLocalOpt, arg)).LocalVariable
                                        : null,
                                    When = label.WhenClause != null
                                        ? (ExpressionSer)this.Visit(label.WhenClause, arg)
                                        : null,
                                    DeclaredType = ty,
                                });
                            }
                            else
                            {
                                throw new NotImplementedException();
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
                                                .Select(_ => VisitToStatement(_, arg))
                                                .Where(_ => _ != null)
                                                .ToList()
                                };
                            }
                            else
                            { blockSer = VisitToStatement(section.Statements[0], arg); }
                        }

                        return new SwitchSectionSer
                        {
                            Labels = caseLabels,
                            Block = blockSer,
                            BlockId = id,
                        };
                    }));
                }

                return new SwitchStatement
                {
                    IsIfElseStatement = isElseIfStatement,
                    Blocks = switchSections,
                    SwitchExpression = (ExpressionSer)Visit(node.Expression, arg),
                    BlockId = id,
                };
            });
        }

        public override AstBase VisitThisReference(BoundThisReference node, SerializationContext arg)
            => new ThisExpression { Location = node.Syntax.Location.GetSerLoc() };

        public override AstBase VisitThrowExpression(BoundThrowExpression node, SerializationContext arg)
            => new ThrowExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Expression = (ExpressionSer)Visit(node.Expression, arg)
            };

        public override AstBase VisitThrowStatement(BoundThrowStatement node, SerializationContext arg)
            => new ThrowStatement
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Expression = node.ExpressionOpt == null
                    ? throw new NotImplementedException("implicit rethrow is not supported")
                    : (ExpressionSer)Visit(node.ExpressionOpt, arg)
            };

        public override AstBase VisitTryStatement(BoundTryStatement node, SerializationContext arg)
        {
            var tryCatchBlock = node.CatchBlocks.Length > 0
                ? new TryCatchBlock
                {
                    TryBlock = (StatementSer)Visit(node.TryBlock, arg),
                    CatchBlocks = node.CatchBlocks
                    .Select(_ => (CatchBlock)Visit(_, arg))
                    .ToList()
                }
                : null;

            if (node.FinallyBlockOpt != null)
            {
                return new TryFinallyBlockSer
                {
                    TryBlock = tryCatchBlock ?? (StatementSer)Visit(node.TryBlock, arg),
                    FinallyBlock = VisitToStatement(node.FinallyBlockOpt, arg)
                };
            }

            return tryCatchBlock;
        }

        public override AstBase VisitTupleBinaryOperator(BoundTupleBinaryOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitTupleLiteral(BoundTupleLiteral node, SerializationContext arg)
            => new TupleLiteral
            {
                Location = node.Syntax.Location.GetSerLoc(),
                TupleType = arg.SymbolSerializer.GetTypeSpecId(node.Type),
                TupleArgs = node.Arguments
                    .Select(tupleArg => (ExpressionSer)this.Visit(tupleArg, arg))
                    .ToList(),
            };

        public override AstBase VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitTypeExpression(BoundTypeExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitTypeOfOperator(BoundTypeOfOperator node, SerializationContext arg)
            => new TypeOfExpression
            {
                Location = node.Syntax.Location.GetSerLoc(),
                Type = arg.SymbolSerializer.GetTypeSpecId((TypeSymbol)node.SourceType.ExpressionSymbol)
            };

        public override AstBase VisitTypeOrInstanceInitializers(BoundTypeOrInstanceInitializers node, SerializationContext arg)
            => VisitStatementList(node, arg);

        public override AstBase VisitTypeOrValueExpression(BoundTypeOrValueExpression node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitUnaryOperator(BoundUnaryOperator node, SerializationContext arg)
            => ConvertUnaryOperator(
                node.Operand,
                node.MethodOpt,
                node.OperatorKind,
                arg);

        private AstBase ConvertUnaryOperator(
            BoundExpression node,
            MethodSymbol methodOpt,
            UnaryOperatorKind op,
            SerializationContext arg)
        {
            var isLifted = IsLifted(op);
            _ = IsChecked(op);
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
                        new BoundExpression[] { node },
                        arg)
                };
            }

            if (typeMask <= UnaryOperatorKind.Bool
                || typeMask == UnaryOperatorKind.Enum)
            {
                return new UnaryExpression
                {
                    IsLifted = isLifted,
                    Expression = (ExpressionSer)Visit(node, arg),
                    Operator = (int)nscriptOp,
                    Location = node.Syntax.Location.GetSerLoc()
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override AstBase VisitUnboundLambda(UnboundLambda node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitUsingStatement(BoundUsingStatement node, SerializationContext arg) => throw new NotImplementedException();

        public override AstBase VisitWhileStatement(BoundWhileStatement node, SerializationContext arg)
            => new WhileStatement
            {
                Loop = VisitToStatement(node.Body, arg),
                Condition = (ExpressionSer)Visit(node.Condition, arg)
            };

        /*
        public override AstBase VisitWildcardPattern(BoundWildcardPattern node, SerializationContext arg) => throw new NotImplementedException();
        */

        public override AstBase VisitWithExpression(BoundWithExpression node, SerializationContext arg)
        {
            var location = node.Syntax.GetSerLoc();
            var receiver = (ExpressionSer)Visit(node.Receiver, arg);
            var cloneMethod = node.CloneMethod != null
                ? arg.SymbolSerializer.GetMethodSpecId(node.CloneMethod)
                : 0;
            var type = arg.SymbolSerializer.GetTypeSpecId(node.Type);

            var initializers = node.InitializerExpression
                .Initializers
                .Select(_ =>
                {
                    if (_.Kind != BoundKind.AssignmentOperator)
                    { return null; }

                    var assignOp = (BoundAssignmentOperator)_;
                    var initializerMember = (BoundObjectInitializerMember)assignOp.Left;
                    var rv = new ObjectInitilaizer
                    {
                        Location = _.Syntax.Location.GetSerLoc(),
                        Value = (ExpressionSer)Visit(assignOp.Right, arg)
                    };

                    if (initializerMember.MemberSymbol.Kind == SymbolKind.Field)
                    {
                        rv.Field = arg.SymbolSerializer.GetFieldSpecId(
                            (FieldSymbol)initializerMember.MemberSymbol);
                    }
                    else
                    {
                        var propertySymbol = (PropertySymbol)initializerMember.MemberSymbol;
                        rv.Property = arg.SymbolSerializer.GetPropertySpecId(propertySymbol);
                        rv.Setter = arg.SymbolSerializer.GetMethodSpecId(propertySymbol.SetMethod);
                        rv.Getter = propertySymbol.GetMethod != null
                            ? arg.SymbolSerializer.GetMethodSpecId(propertySymbol.GetMethod)
                            : 0;

                        rv.PropertyArgs = ToArgs(
                            propertySymbol.GetMethod,
                            initializerMember.Arguments,
                            arg);
                    }

                    return rv;
                })
                .ToList();

            return new WithExpressionSer
            {
                Location = location,
                Receiver = receiver,
                CloneMethod = cloneMethod,
                Type = type,
                Initializers = initializers
            };
        }

        public override AstBase VisitYieldBreakStatement(BoundYieldBreakStatement node, SerializationContext arg)
            => new YieldBreakStatement { Location = node.Syntax.GetSerLoc() };

        public override AstBase VisitYieldReturnStatement(BoundYieldReturnStatement node, SerializationContext arg)
            => new YieldStatement
            {
                Location = node.Syntax.GetSerLoc(),
                Expression = (ExpressionSer)Visit(node.Expression, arg)
            };

        public static AstBase GetConstLiteral(ConstantValue constValue)
        {
            switch (constValue.Discriminator)
            {
                case ConstantValueTypeDiscriminator.Null:
                    return new NullExpression();

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
                    { Value = constValue.Int16Value };

                case ConstantValueTypeDiscriminator.UInt16:
                    return new UShortLiteralExpression
                    { Value = constValue.UInt16Value };

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
                case bool i:
                    return new BoolLiteralExpression
                    { Value = i };

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
            MethodSymbol method,
            IList<BoundExpression> nodes,
            SerializationContext arg) => Enumerable
                .Range(0, method.Parameters.Length)
                .Select(paramIdx =>
                {
                    var parameter = method.Parameters[paramIdx];

                    if (parameter.IsParams)
                    {
                        if (nodes.Count <= paramIdx)
                        {
                            return new MethodCallArg
                            {
                                Value = new ArrayCreationExpression
                                {
                                    ArrayType = arg.SymbolSerializer.GetTypeSpecId(parameter.Type),
                                    ElementType = arg.SymbolSerializer.GetTypeSpecId(
                                        ((ArrayTypeSymbol)parameter.Type).ElementType),
                                    Initializers = null,
                                    Arguments = new List<ExpressionSer>()
                                        { new IntLiteralExpression { Value = 0 } }
                                }
                            };
                        }
                        else if (nodes.Count == paramIdx + 1
                            && nodes[nodes.Count - 1].Type.Equals(parameter.Type))
                        {
                            return new MethodCallArg
                            {
                                IsByRef = parameter.RefKind == RefKind.Ref
                                    || parameter.RefKind == RefKind.Out
                                    || parameter.RefKind == RefKind.RefReadOnly,
                                Value = (ExpressionSer)Visit(nodes[paramIdx], arg)
                            };
                        }

                        return new MethodCallArg
                        {
                            Value = new ArrayCreationExpression
                            {
                                ArrayType = arg.SymbolSerializer.GetTypeSpecId(parameter.Type),
                                ElementType = arg.SymbolSerializer.GetTypeSpecId(
                                    ((ArrayTypeSymbol)parameter.Type).ElementType),
                                Initializers = nodes.Skip(paramIdx)
                                    .Select(_a => (ExpressionSer)Visit(_a, arg))
                                    .ToList()
                            }
                        };
                    }
                    else if (nodes.Count > paramIdx)
                    {
                        return new MethodCallArg
                        {
                            IsByRef = parameter.RefKind == RefKind.Ref
                                || parameter.RefKind == RefKind.Out
                                || parameter.RefKind == RefKind.RefReadOnly,
                            Value = (ExpressionSer)Visit(nodes[paramIdx], arg)
                        };
                    }
                    else
                    {
                        return new MethodCallArg
                        {
                            Value = (ExpressionSer)GetConstantValue(parameter.ExplicitDefaultValue)
                        };
                    }
                })
                .ToList();

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

        private T WrapInBlock<T>(BoundNode node, Func<int, List<string>, T> func) where T : StatementSer
        {
            _ = scopeBlockStack.AddFirst((++id, node, new List<string>()));
            try
            { return func(id, scopeBlockStack.Last.Value.localFunctions); }
            finally
            { scopeBlockStack.RemoveFirst(); }
        }

        private StatementSer VisitToStatement(BoundNode node, SerializationContext arg)
        {
            var ser = Visit(node, arg);
            var expressionSer = ser as ExpressionSer;
            if (expressionSer != null)
            {
                return new StatementExpressionSer
                { Expression = expressionSer };
            }

            return (StatementSer)ser;
        }

        private BlockKind GetBlockKind(MethodSymbol symbol)
        {
            return (BlockKind)((symbol.IsAsync ? (int)BlockKind.Async : 0)
                | (symbol.IsIterator ? (int)BlockKind.Iterator : 0));
        }

        private static bool IsJumbled(IList<int> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (list[i - 1] > list[i])
                {
                    return true;
                }
            }

            return false;
        }

    }
}