//-----------------------------------------------------------------------
// <copyright file="BondToAst.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition for BondToAst
    /// </summary>
    public class BondToAst
    {
        private ClrContext _clrContext;
        private readonly Dictionary<Type, Func<Serialization.AstBase, Node>> _parserMap;
        private MethodDefinition _currentMethod;
        private string _currentMethodFileName;
        private MemberReferenceDeserializer _memberReferenceDeserializer;
        private LinkedList<(int id, VariableCollector collector)> scopeBlockStack = new LinkedList<(int, VariableCollector)>();
        private Serialization.TypeInfoSer _typeInfo;

        private LinkedList<ConditionalAccessExpression.ConditionalReceiver> conditionalReceiverStack
            = new LinkedList<ConditionalAccessExpression.ConditionalReceiver>();

        public BondToAst(
            Serialization.TypeInfoSer typeInfo,
            ClrContext context)
        {
            _parserMap = BuildParserMap();
            _typeInfo = typeInfo;
            _memberReferenceDeserializer
                = new MemberReferenceDeserializer(context);
            _clrContext = context;
        }

        public MethodDefinition CurrentMethod => _currentMethod;

        public Tuple<MethodDefinition, Func<TopLevelBlock>> ParseMethodBody(
            Serialization.MethodBody jObject)
        {
            var method = DeserializeMethod(jObject.MethodId).Resolve();

            if (method.Name == "GenericMethodCall3")
            { }

            return new Tuple<MethodDefinition, Func<TopLevelBlock>>(
                method,
                () =>
                {
                    var fileName = jObject.FileName;

                    _currentMethod = method;
                    _currentMethodFileName = fileName;
                    _memberReferenceDeserializer.SetMethodContext(_currentMethod);
                    try
                    {
                        var methodBlockObject = jObject.Body;
                        if (methodBlockObject != null)
                        {
                            var rv = new TopLevelBlock(method)
                            {
                                RootBlock = (ParameterBlock)ParseNode(methodBlockObject)
                            };
                            _currentMethod = null;
                            _currentMethodFileName = null;

                            return rv;
                        }
                        else
                        { return null; }
                    }
                    finally
                    {
                        _memberReferenceDeserializer.SetMethodContext(null);
                    }
                });
        }

        public Node ParseNode(
            Serialization.AstBase jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            return _parserMap[jObject.GetType()](jObject);
        }

        private Node ParseNullLiteral(Serialization.NullExpression jObject) => new NullLiteral(
                _clrContext,
                LocFromJObject(jObject));

        private Node ParseBoolLiteral(Serialization.BoolLiteralExpression jObject) => new BooleanLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseCharLiteral(Serialization.CharLiteralExpression jObject) => new CharLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseLongLiteral(Serialization.LongLiteralExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseULongLiteral(Serialization.ULongLiteralExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                (long)jObject.Value);

        private Node ParseIntLiteral(Serialization.IntLiteralExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseUIntLiteral(Serialization.UIntLiteralExpression jObject) => new UIntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseStringLiteral(Serialization.StringLiteralExpression jObject) => new StringLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseDecimalLiteral(Serialization.DecimalLiteralExpression jObject) => throw new NotImplementedException();

        private Node ParseDoubleLiteral(Serialization.DoubleLiteralExpression jObject) => new DoubleLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseFloatLiteral(Serialization.FloatLiteralExpression jObject) => new DoubleLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseByteLiteral(Serialization.ByteLiteralExpression jObject) => new UIntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseSByteLiteral(Serialization.SByteLiteralExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseShortLiteral(Serialization.ShortLiteralExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseLiteral(Serialization.UShortLiteralExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseAssignment(Serialization.AssignExpression jObject) => new BinaryExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Left),
                ParseExpression(jObject.Right),
                BinaryOperator.Assignment);

        private Node ParseUserDefinedOperators(Serialization.UserDefinedBinaryOrUnaryOpExpression jObject) => ParseMethodCall(jObject);

        private Node ParseMethodCall(Serialization.LocalMethodCallExpression jObject)
        {
            LocalFunctionVariable lfv = null;
            foreach (var block in scopeBlockStack)
            {
                lfv = block.collector.ResolveLocalFunctionVariable(jObject.MethodName);
                if (lfv != null)
                { break; }
            }

            if (lfv == null)
            { throw new InvalidOperationException(); }

            // TODO: move methodReferenceExpression to a different JObject node.
            return new MethodCallExpression(
                _clrContext,
                LocFromJObject(jObject),
                new LocalFunctionReference(
                    _clrContext,
                    null,
                    lfv,
                    DeserializeType(jObject.ReturnType)),
                ParseArguments(jObject.Arguments));
        }

        private Node ParseMethodCall(Serialization.MethodCallExpression jObject)
        {
            var instance = ParseExpression(jObject.Instance);
            var methodReference = DeserializeMethod(jObject.Method);

            // TODO: move methodReferenceExpression to a different JObject node.
            return new MethodCallExpression(
                _clrContext,
                LocFromJObject(jObject),
                GetMethodReferenceExpression(
                    instance,
                    methodReference),
                ParseArguments(jObject.Arguments));
        }

        private Node ParseBinaryExpr(Serialization.BinaryExpression jObject) => new BinaryExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Left),
                ParseExpression(jObject.Right),
                (BinaryOperator)jObject.Operator,
                jObject.IsLifted);

        private Node ParseUnaryExpr(Serialization.UnaryExpression jObject) => new UnaryExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression),
                (UnaryOperator)jObject.Operator,
                jObject.IsLifted);

        private Node ParseTypeCast(Serialization.TypeCastExpression jObject)
        {
            if (jObject.IsUnbox)
            {
                return new UnboxExpression(
                    _clrContext,
                    LocFromJObject(jObject),
                    ParseExpression(jObject.Expression),
                    DeserializeType(jObject.Type));
            }

            var innerExpression =
                ParseExpression(jObject.Expression);
            var type =
                DeserializeType(jObject.Type);

            // Avoid un necessary cast expressions.
            if (MemberReferenceComparer.Instance.Equals(
                innerExpression.ResultType,
                type))
            {
                return innerExpression;
            }
            else if (innerExpression is FromNullable)
            {
                var fromNullable = (FromNullable)innerExpression;
                if (MemberReferenceComparer.Instance.Equals(type, fromNullable.InnerExpression.ResultType))
                {
                    return fromNullable.InnerExpression;
                }
            }

            return new TypeCheckExpression(
                _clrContext,
                LocFromJObject(jObject),
                innerExpression,
                type,
                TypeCheckType.CastType);
        }

        private Node ParseNullableToNormal(Serialization.NullableToNormal jObject)
            => new FromNullable(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression));

        private Node ParseToNullable(Serialization.WrapExpression jObject)
            => new ToNullable(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression));

        private Node ParseFromNullable(Serialization.UnwrapExpression jObject)
            => new FromNullable(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression));

        private Node ParseConstant(Serialization.ByteConstantExpression jObject) => new UIntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.SbyteConstantExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.ShortConstantExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.UshortConstantExpression jObject) => new UIntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.IntConstantExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.UintConstantExpression jObject) => new UIntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.LongConstantExpression jObject) => new IntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.UlongConstantExpression jObject) => new UIntLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.FloatConstantExpression jObject) => new DoubleLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.DoubleConstantExpression jObject) => new DoubleLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.DecimalConstantExpression jObject) => new DoubleLiteral(
                _clrContext,
                LocFromJObject(jObject),
                (double)jObject.Value);

        private Node ParseConstant(Serialization.StringConstantExpression jObject) => new StringLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.CharConstantExpression jObject) => new CharLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.BoolConstantExpression jObject) => new BooleanLiteral(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Value);

        private Node ParseConstant(Serialization.NullConstantExpression jObject) => new NullLiteral(
                _clrContext,
                LocFromJObject(jObject));

        private Node ParseEmptyStatement(Serialization.EmptyStatementSer jObject) => new ExplicitBlock(
                _clrContext,
                LocFromJObject(jObject));

        private Node ParseStatementExpr(Serialization.StatementExpressionSer jObject) => new ExpressionStatement(ParseExpression(jObject.Expression));

        private Node ParseStatementList(Serialization.StatementListSer jObject) => new ExplicitBlock(
                _clrContext,
                LocFromJObject(jObject),
                ParseStatements(jObject.Statements));

        private Node ParseReturnStatement(Serialization.ReturnStatement jObject) => new ReturnStatement(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression));

        private Node ParseThrowStatment(Serialization.ThrowExpression jObject) => new ThrowExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression));

        private Node ParseBreakExpression(Serialization.BreakStatement jObject) => new BreakStatement(
                _clrContext,
                LocFromJObject(jObject));

        private Node ParseContinueExpression(Serialization.ContinueStatement jObject) => new ContinueStatement(
                _clrContext,
                LocFromJObject(jObject));

        private Node ParseVariableInitializers(Serialization.VariableBlockDeclaration jObject)
        {
            var expressions = ParseExpressions(jObject.Initializers);
            if (expressions != null && expressions.Length == 1)
            { return new ExpressionStatement(expressions[0]); }
            else if (expressions == null)
            { expressions = new Expression[0]; }

            return new InitializerStatement(
                _clrContext,
                LocFromJObject(jObject),
                expressions);
        }

        private Node ParseIfStatement(Serialization.IfStatement jObject)
        {
            var trueStatement = GetScopeBlock(jObject.TrueStatement);
            var falseStatement = GetScopeBlock(jObject.FalseStatement);

            if (trueStatement == null && falseStatement == null)
            {
                return new ExpressionStatement(
                    ParseExpression(jObject.Condition));
            }
            else
            {
                return new IfBlockStatement(
                    _clrContext,
                    LocFromJObject(jObject),
                    ParseExpression(jObject.Condition),
                    trueStatement,
                    falseStatement);
            }
        }

        private Node ParseDoWhileStatement(Serialization.DoStatement jObject) => new DoWhileLoop(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Condition),
                GetScopeBlock(jObject.Loop));

        private Node ParseWhileStatement(Serialization.WhileStatement jObject) => new WhileLoop(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Condition),
                GetScopeBlock(jObject.Loop));

        private Node ParseForStatement(Serialization.ForStatement jObject)
        {
            var variableCollector = new VariableCollector(jObject.BlockId);
            _ = scopeBlockStack.AddFirst((jObject.BlockId, variableCollector));
            try
            {
                return new ForLoop(
                    _clrContext,
                    LocFromJObject(jObject),
                    ParseExpression(jObject.Condition),
                    ParseStatement(jObject.Initializer),
                    ParseStatement(jObject.Iterator),
                    GetScopeBlock(jObject.Loop),
                    variableCollector.GetCapturedVariables(),
                    variableCollector.GetLocalFunctionVariables());
            }
            finally
            { scopeBlockStack.RemoveFirst(); }
        }

        private Node ParseForEachStatement(Serialization.ForEachStatement jObject) => WrapVariableCollection(
                (vc) =>
                {
                    var iterator = ParseExpression(jObject.Collection);
                    var body = GetScopeBlock(jObject.Loop);
                    var variables = vc.GetCapturedVariables();
                    var localVariable = variables
                        .Where(_ => _.variable.Name == jObject.LocalVariableName)
                        .Select(_ => _.variable)
                        .FirstOrDefault();

                    return new ForEachLoop(
                        _clrContext,
                        LocFromJObject(jObject),
                        localVariable,
                        iterator,
                        body,
                        vc.GetCapturedVariables(),
                        vc.GetLocalFunctionVariables());
                },
                jObject.BlockId,
                false);

        private Node ParseSwitchStatement(Serialization.SwitchStatement jObject)
        {
            var variableCollector = new VariableCollector(jObject.BlockId);
            _ = scopeBlockStack.AddFirst((jObject.BlockId, variableCollector));

            var caseBlocks =
                new List<KeyValuePair<List<CaseLabel>, Statement>>();

            var sectionArray = jObject.Blocks;
            List<(LocalVariable localVariable, bool isUsed)> sectionVariables = new List<(LocalVariable localVariable, bool isUsed)>();
            List<LocalFunctionVariable> sectionLocalFunctionNames = new List<LocalFunctionVariable>();

            for (var iSection = 0; iSection < sectionArray.Count; iSection++)
            {
                var section = sectionArray[iSection];
                var sectionObj = sectionArray[iSection].Block;
                var labelJArray = sectionArray[iSection].Labels;

                var labels =
                    new List<CaseLabel>(labelJArray.Count);

                var sectionVariableCollector = new VariableCollector(section.BlockId);
                _ = scopeBlockStack.AddFirst((section.BlockId, sectionVariableCollector));

                for (var iLabel = 0; iLabel < labelJArray.Count; iLabel++)
                {
                    var @case = labelJArray[iLabel];
                    switch (labelJArray[iLabel])
                    {
                        case Serialization.SwitchConstCaseLabel sccl:
                            labels.Add(
                                new ConstCaseLabel(_clrContext, LocFromJObject(@case), ParseExpression(sccl.LabelValue)));
                            break;

                        case Serialization.SwitchDiscardCaseLabel:
                        case null:
                            labels.Add(null);
                            break;

                        case Serialization.SwitchDeclarationCaseLabel sdcl:
                            var localVariableOpt = sdcl.LocalVariableOpt != null
                                ? ParseLocalVariable(sdcl.LocalVariableOpt)
                                : null;

                            labels.Add(
                                new DeclarationCaseLabel(_clrContext,
                                LocFromJObject(@case),
                                localVariableOpt != null
                                    ? new VariableReference(
                                        _clrContext,
                                        null,
                                        localVariableOpt)
                                    : null,
                                DeserializeType(sdcl.DeclaredTypeOpt.Value),
                                ParseExpression(sdcl.When)));

                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }

                caseBlocks.Add(
                    new KeyValuePair<List<CaseLabel>, Statement>(
                        labels,
                        ParseStatement(sectionObj)));

                sectionVariables.AddRange(sectionVariableCollector.GetCapturedVariables());
                sectionLocalFunctionNames.AddRange(sectionVariableCollector.GetLocalFunctionVariables());

                scopeBlockStack.RemoveFirst();
            }

            scopeBlockStack.RemoveFirst();
            var expr = ParseExpression(jObject.SwitchExpression);
            sectionVariables.AddRange(variableCollector.GetCapturedVariables());
            sectionLocalFunctionNames.AddRange(variableCollector.GetLocalFunctionVariables());

            return new SwitchStatement(
                _clrContext,
                LocFromJObject(jObject),
                expr,
                caseBlocks,
                sectionVariables,
                sectionLocalFunctionNames);
        }

        private Node ParseScopeBlock(Serialization.ExplicitBlockSer jObject) => WrapVariableCollection(
                (vc) =>
                {
                    if (jObject.LocalFunctions != null)
                    {
                        foreach (var localFunction in jObject.LocalFunctions)
                        { _ = vc.CreateFunctionVariable(localFunction); }
                    }

                    var statements = new List<Statement>();
                    if (jObject.Statements != null)
                    {
                        foreach (var statement in ParseStatements(jObject.Statements))
                        { statements.Add(statement); }
                    }
                    var rv = new ScopeBlock(
                        _clrContext,
                        LocFromJObject(jObject),
                        vc.GetCapturedVariables(),
                        vc.GetLocalFunctionVariables());

                    statements.ForEach(_ => rv.AddStatement(_));

                    if (rv.Statements.Count == 1)
                    {
                        var singleStatement = rv.Statements[0];
                        if (singleStatement is ForEachLoop feLoop)
                        {
                            feLoop.MoveVariablesFrom(rv);
                            return feLoop;
                        }

                        if (singleStatement is ForLoop forLoop)
                        {
                            forLoop.MoveVariablesFrom(rv);
                            return forLoop;
                        }
                    }

                    return rv;
                },
                jObject.Id,
                false);

        private Node ParseBlock(Serialization.BlockSer jObject) => new ExplicitBlock(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Statements.Select(_ => ParseStatement(_))
                    .ToArray());

        private Node ParseParameterBlock(Serialization.ParameterBlock jObject)
            => ParseParameterBlock(jObject, false);

        private Node ParseParameterBlock(Serialization.ParameterBlock jObject, bool isDelegate)
        {
            if (_currentMethod.Name == "InstanceReferencingDelegate")
            { }

            return WrapVariableCollection(
                (vc) =>
                {
                    if (jObject.LocalFunctions != null)
                    {
                        foreach (var localFunction in jObject.LocalFunctions)
                        { _ = vc.CreateFunctionVariable(localFunction); }
                    }

                    var statements = new List<Statement>();
                    if (jObject.Statements != null)
                    {
                        foreach (var statement in ParseStatements(jObject.Statements))
                        { statements.Add(statement); }
                    }

                    var rv = new ParameterBlock(
                        _clrContext,
                        LocFromJObject(jObject),
                        vc.GetCapturedVariables(),
                        vc.GetParamBlockVariables(),
                        vc.GetLocalFunctionVariables());

                    statements.ForEach(_ => rv.AddStatement(_));

                    return rv;
                },
                jObject.Id,
                true,
                jObject.IsMethodOwned
                        && !isDelegate
                        && _currentMethod.HasThis
                        ? new ParameterDefinition(
                            "this",
                            ParameterAttributes.None,
                            _currentMethod.DeclaringType)
                        : null,
                jObject.Parameters != null
                    ? jObject.Parameters
                        .Select(paramObj =>
                        {
                            var attr = (ParameterAttributes)paramObj.Modifier;

                            var parameterType =
                                    DeserializeType(paramObj.Type);

                            if ((attr & ParameterAttributes.Out) != 0)
                            { parameterType = new ByReferenceType(parameterType); }

                            return new ParameterDefinition(
                                paramObj.Name,
                                attr,
                                parameterType);
                        })
                        .ToList()
                    : null
                );
        }

        private Node ParseTryFinally(Serialization.TryFinallyBlockSer jObject)
        {
            var tryBlock = ParseStatement(jObject.TryBlock);
            var tryCatchFinallyBlock = tryBlock as TryCatchFinally;
            var finallyScope = GetScopeBlock(jObject.FinallyBlock);

            var finallyBlock =
                new HandlerBlock(
                    _clrContext,
                    finallyScope.Location,
                    null,
                    null,
                    finallyScope);

            if (tryCatchFinallyBlock != null)
            {
                tryCatchFinallyBlock.AddHandler(finallyBlock);
                return tryCatchFinallyBlock;
            }

            var tryScopeBlock = tryBlock as ScopeBlock;
            if (tryScopeBlock == null)
            {
                tryScopeBlock = new ScopeBlock(
                    _clrContext,
                    tryBlock.Location,
                    null,
                    null);
                tryScopeBlock.AddStatement(tryBlock);
            }

            return new TryCatchFinally(
                _clrContext,
                LocFromJObject(jObject),
                tryScopeBlock,
                finallyBlock);
        }

        private Node ParseTryCatch(Serialization.TryCatchBlock jObject)
        {
            var tryBlock = GetScopeBlock(jObject.TryBlock);
            var handlerList = new List<HandlerBlock>();

            TryCatchFinally rv = null;
            var handlersArray = jObject.CatchBlocks;
            for (var iHandler = 0; iHandler < handlersArray.Count; iHandler++)
            {
                var handlerObj = handlersArray[iHandler];

                var handlerBlock = WrapVariableCollection(
                    (vc) =>
                    {
                        var handlerScopeBlock = GetScopeBlock(handlerObj.Block);
                        var exceptionVariable =
                            ParseLocalVariable(handlerObj.LocalVariable);

                        if (exceptionVariable != null)
                        {
                            var tmpBlock = new ScopeBlock(
                                _clrContext,
                                null,
                                new List<(LocalVariable localVariable, bool isUsed)> { (exceptionVariable, true) },
                                vc.GetLocalFunctionVariables());

                            handlerScopeBlock.MoveVariablesFrom(tmpBlock);
                        }

                        return new HandlerBlock(
                            _clrContext,
                            LocFromJObject(handlerObj),
                            DeserializeType(handlerObj.CatchType ?? 0)
                                ?? _clrContext.KnownReferences.Exception,
                            exceptionVariable != null
                                ? new VariableReference(
                                        _clrContext,
                                        null,
                                        exceptionVariable)
                                : null,
                            handlerScopeBlock);
                    },
                    handlerObj.Block.Id,
                    false);

                if (iHandler == 0)
                {
                    rv = new TryCatchFinally(
                        _clrContext,
                        LocFromJObject(jObject),
                        tryBlock,
                        handlerBlock);
                }
                else
                {
                    rv.AddHandler(handlerBlock);
                }
            }

            return rv;
        }

        private Node ParseNullCoalascing(Serialization.NullCoalescingOperatorSer jObject) => new NullConditional(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Left),
                ParseExpression(jObject.Right));

        private Node ParseYield(Serialization.YieldStatement jObject) => new YieldStatement(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression));

        private Node ParseYield(Serialization.YieldBreakStatement jObject) => new YieldStatement(
            _clrContext,
            LocFromJObject(jObject),
            null);

        private Node ParseIterator(Serialization.IteratorBlock jObject) => new InlineIteratorExpression(
                _clrContext,
                LocFromJObject(jObject),
                (ParameterBlock)GetScopeBlock(jObject.Block),
                DeserializeType(jObject.Type));

        private Node ParseBoxExpr(Serialization.BoxCastExpression jObject) => new BoxExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression));

        private Node ParseIsExpr(Serialization.IsExpression jObject) => new TypeCheckExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression),
                DeserializeType(jObject.Type),
                TypeCheckType.IsType);

        private Node ParseAsExpr(Serialization.AsExpression jObject) => new TypeCheckExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Expression),
                DeserializeType(jObject.Type),
                TypeCheckType.AsType);

        private Node ParseConditional(Serialization.ConditionalExpression jObject) => new ConditionalOperatorExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Condition),
                ParseExpression(jObject.TrueExpression),
                ParseExpression(jObject.FalseExpression));

        private Node ParseConditionalAccess(Serialization.ConditionalAccess jObject)
        {
            var receiver = ParseExpression(jObject.Receiver);
            var conditionalReceiver = new ConditionalAccessExpression.ConditionalReceiver(
                _clrContext,
                receiver.Location,
                receiver);

            _ = conditionalReceiverStack.AddLast(conditionalReceiver);
            var rv = new ConditionalAccessExpression(
                _clrContext,
                LocFromJObject(jObject),
                receiver,
                ParseExpression(jObject.AccessExpression));
            conditionalReceiverStack.RemoveLast();

            return rv;
        }

        private Node ParseConditionalReceiver(Serialization.ConditionalReceiver jObject)
            => conditionalReceiverStack.Last.Value;

        private Node ParseVariableReference(Serialization.LocalVariableRefExpression jObject)
        {
            if (jObject.IsAddressReference)
            {
                return new LoadAddressExpression(
                    _clrContext,
                    LocFromJObject(jObject),
                    new VariableReference(
                        _clrContext,
                        LocFromJObject(jObject),
                        ParseLocalVariable(jObject.LocalVariable)));
            }
            else
            {
                return new VariableReference(
                    _clrContext,
                    LocFromJObject(jObject),
                    ParseLocalVariable(jObject.LocalVariable));
            }
        }

        private Node ParseParameterReference(Serialization.ParameterReferenceExpression jObject)
        {
            // TODO: figure out how to deal with Ref varaibles.
            var parameter = ParseArgumentVariable(jObject.Parameter);

            if (((ParameterDefinition)parameter.ParameterReference).ParameterType.IsByReference)
            {
                return new VariableAddressReference(
                    _clrContext,
                    LocFromJObject(jObject),
                    parameter);
            }

            return new VariableReference(
                _clrContext,
                LocFromJObject(jObject),
                parameter);
        }

        private Node ParseNew(Serialization.NewExpression jObject)
        {
            var methodObj = jObject.Method;
            if (methodObj == 0)
            {
                // This is struct and should be initialized using default value constructor.
                return new DefaultValueExpression(
                    _clrContext,
                    LocFromJObject(jObject),
                    DeserializeType(jObject.Type));
            }

            return new NewObjectExpression(
                _clrContext,
                LocFromJObject(jObject),
                DeserializeMethod(methodObj),
                ParseArguments(jObject.Arguments));
        }

        private Node ParseArrayCreation(Serialization.ArrayCreationExpression jObject)
        {
            var initializers = jObject.Initializers;
            var arguments = jObject.Arguments;
            Expression argExpression = null;

            if (arguments != null)
            {
                if (arguments.Count != 1)
                { throw new InvalidOperationException(); }

                argExpression = ParseExpression(arguments[0]);
            }

            if (initializers == null
                || initializers.Count == 0)
            {
                if (initializers != null
                    && initializers.Count == 0)
                {
                    argExpression = new IntLiteral(_clrContext, LocFromJObject(jObject), 0);
                }
                else if (argExpression == null)
                {
                    throw new InvalidOperationException();
                }

                return new NewArrayExpression(
                    _clrContext,
                    LocFromJObject(jObject),
                    DeserializeType(jObject.ElementType),
                    argExpression);
            }

            IList<Expression> initializerExpressions = ParseExpressions(initializers);

            return new InlineArrayInitialization(
                _clrContext,
                LocFromJObject(jObject),
                DeserializeType(jObject.ElementType),
                initializerExpressions);
        }

        private Node ParseThisExpr(Serialization.ThisExpression jObject)
        {
            var thisVar = scopeBlockStack.Last.Value.collector.ThisVariable;
            var node = scopeBlockStack.Last.Previous;

            while (node != null)
            {
                var paramBlock = node.Value.collector;
                if (paramBlock.IsParamBlock)
                {
                    paramBlock.AddEscapingVariable(thisVar);
                }

                node = node.Previous;
            }

            return new VariableReference(
                _clrContext,
                LocFromJObject(jObject),
                thisVar);
        }

        private Node ParseTypeOf(Serialization.TypeOfExpression jObject) => new TypeofExpression(
                _clrContext,
                LocFromJObject(jObject),
                new TypeReferenceExpression(
                    _clrContext,
                    LocFromJObject(jObject),
                    DeserializeType(jObject.Type)));

        private Node ParseArrayElementAccess(Serialization.ElementAccessExpression jObject) => new ArrayElementExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Left),
                ParseArguments(jObject.Arguments)[0]);

        private Node ParseBaseExpr(Serialization.BaseThisExpression jObject)
        {
            var thisVariable = scopeBlockStack.Last.Value.Item2.ThisVariable;
            var node = scopeBlockStack.Last.Previous;
            while (node != null)
            {
                var collector = node.Value.Item2;
                if (collector.IsParamBlock)
                { collector.AddEscapingVariable(thisVariable); }

                node = node.Previous;
            }

            return new BaseVariableReference(
                _clrContext,
                LocFromJObject(jObject),
                thisVariable);
        }

        private Node ParseNewCollectionInitializer(Serialization.NewCollectionInitializerExpression jObject)
        {
            var initializerArray = jObject.ItemInitializers;

            var args = ParseArguments(jObject.Arguments);

            var objectExpression =
                new NewObjectExpression(
                    _clrContext,
                    LocFromJObject(jObject),
                    DeserializeMethod(jObject.Method));

            var setters = initializerArray
                .Select(
                    mc =>
                    {
                        var methodReferenceExpression = GetMethodReferenceExpression(
                            objectExpression,
                            DeserializeMethod(mc.Method),
                            LocFromJObject(mc));
                        return new MethodCallExpression(
                            _clrContext,
                            LocFromJObject(mc),
                            methodReferenceExpression,
                            ParseArguments(mc.Arguments));
                    })
                .ToList();

            return new InlineCollectionInitializationExpression(
                _clrContext,
                LocFromJObject(jObject),
                objectExpression,
                setters);
        }

        private Node ParseNewInitializer(Serialization.NewInitializerExpression jObject)
        {
            var initializerArray = jObject.Initializers;

            var args = ParseArguments(jObject.Arguments);

            var setters =
                new List<Tuple<MemberReferenceExpression, Expression[]>>();

            if (initializerArray != null)
            {
                for (var iInit = 0; iInit < initializerArray.Count; iInit++)
                {
                    var initObj = initializerArray[iInit];
                    MemberReferenceExpression memberReferenceExpression = null;

                    if (initObj.Setter != null)
                    {
                        PropertyReference propertyReference = new InternalPropertyReference(
                            null,
                            DeserializeMethod(initObj.Setter.Value));

                        memberReferenceExpression = new PropertyReferenceExpression(
                            _clrContext,
                            LocFromJObject(initObj),
                            propertyReference,
                            null);

                        var setterArgs = initObj.PropertyArgs != null
                            ? initObj.PropertyArgs
                                .Select(arg => ParseExpression(arg.Value))
                                .Concat(new Expression[] { ParseExpression(initObj.Value) })
                                .ToArray()
                            : new Expression[] { ParseExpression(initObj.Value) };

                        setters.Add(
                            Tuple.Create(
                                memberReferenceExpression,
                                setterArgs));
                    }
                    else if (initObj.Field != null)
                    {
                        memberReferenceExpression = new FieldReferenceExpression(
                            _clrContext,
                            LocFromJObject(initObj),
                            DeserializeField(initObj.Field.Value),
                            null);

                        setters.Add(
                            Tuple.Create(
                                memberReferenceExpression,
                                new Expression[] { ParseExpression(initObj.Value) }));
                    }
                    else
                    {
                        memberReferenceExpression = new MethodReferenceExpression(
                            _clrContext,
                            LocFromJObject(initObj),
                            DeserializeMethod(initObj.MethodCall.Method),
                            null);

                        var arguments = ParseArguments(initObj.MethodCall.Arguments);

                        setters.Add(
                            Tuple.Create(
                                memberReferenceExpression,
                                arguments));
                    }
                }
            }

            return new InlinePropertyInitilizationExpression(
                _clrContext,
                LocFromJObject(jObject),
                jObject.Method == 0
                    ? (Expression)new DefaultValueExpression(
                        _clrContext,
                        LocFromJObject(jObject),
                        DeserializeType(jObject.Type))
                    : new NewObjectExpression(
                        _clrContext,
                        LocFromJObject(jObject),
                        DeserializeMethod(jObject.Method),
                        args),
                setters); ;
        }

        private Node ParseLocalMethodExpr(Serialization.LocalMethodExpression jObject)
        {
            LocalFunctionVariable lfv = null;
            foreach (var block in scopeBlockStack)
            {
                lfv = block.collector.ResolveLocalFunctionVariable(jObject.MethodName);
                if (lfv != null)
                { break; }
            }

            if (lfv == null)
            { throw new InvalidOperationException(); }

            // TODO: move methodReferenceExpression to a different JObject node.
            return new LocalFunctionReference(
                    _clrContext,
                    null,
                    lfv,
                    DeserializeType(jObject.ReturnType));
        }

        private Node ParseMethodExpr(Serialization.MethodExpression jObject)
        {
            var methodReference = DeserializeMethod(jObject.Method);

            if (jObject.GenericParameters != null && jObject.GenericParameters.Count > 0)
            {
                var typeReferences = jObject.GenericParameters
                    .Select(_ => DeserializeType(_))
                    .ToList();

                var genericMethodReference = new GenericInstanceMethod(methodReference);
                foreach (var typeReference in typeReferences)
                { genericMethodReference.GenericArguments.Add(typeReference); }

                methodReference = genericMethodReference;
            }

            var instance = ParseExpression(jObject.Instance);

            var location = LocFromJObject(jObject);

            if (instance == null)
            {
                return new MethodReferenceExpression(
                    _clrContext,
                    location,
                    methodReference);
            }

            var methodDef = methodReference.Resolve();
            var isVirtual = methodDef.IsVirtual
                && !(instance is BaseVariableReference);

            if (isVirtual)
            {
                return new VirtualMethodReferenceExpression(
                    _clrContext,
                    location,
                    methodReference,
                    instance.ResultType.IsValueType ?
                        new BoxExpression(
                            _clrContext,
                            instance.Location,
                            instance)
                        : instance);
            }

            return new MethodReferenceExpression(
                _clrContext,
                location,
                methodReference,
                instance);
        }

        private Node ParseFieldExpr(Serialization.FieldExpression jObject)
        {
            var fieldReference = DeserializeField(jObject.Field);

            var instance = ParseExpression(jObject.Instance);

            var location = LocFromJObject(jObject);

            if (instance == null)
            {
                return new FieldReferenceExpression(
                    _clrContext,
                    location,
                    fieldReference);
            }

            return new FieldReferenceExpression(
                _clrContext,
                location,
                fieldReference,
                instance);
        }

        private Node ParsePropertyExpr(Serialization.PropertyExpression jObject)
        {
            var propertyReference = DeserializeProperty(jObject.Property);

            var instance = ParseExpression(jObject.Instance);

            var location = LocFromJObject(jObject);

            var args = jObject is Serialization.IndexExpression
                ? ParseArguments(((Serialization.IndexExpression)jObject).Arguments)
                : null;

            if (instance == null)
            {
                return new PropertyReferenceExpression(
                    _clrContext,
                    location,
                    propertyReference);
            }

            return new PropertyReferenceExpression(
                _clrContext,
                location,
                propertyReference,
                instance,
                args);
        }

        private Node ParseEventExpr(Serialization.EventExpression jObject)
        {
            var eventReference = DeserializeEvent(jObject.Event);

            var instance = ParseExpression(jObject.Instance);

            var location = LocFromJObject(jObject);

            if (instance == null)
            {
                return new EventReferenceExpression(
                    _clrContext,
                    location,
                    eventReference);
            }

            return new EventReferenceExpression(
                _clrContext,
                location,
                eventReference,
                instance);
        }

        private Node ParseDynamicIndexBinder(Serialization.DynamicIndexBinderExpression jObject) => new DynamicIndexAccessor(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Instance),
                ParseExpression(jObject.Index));

        private Node ParseDynamicMemberExpression(Serialization.DynamicMemberExpression jObject) => new DynamicMemberAccessor(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Instance),
                jObject.MemberName);

        private Node ParseDynamicMemberBinder(Serialization.DynamicMethodBinderExpression jObject) => new DynamicMemberAccessor(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Instance),
                jObject.Name);

        private Node ParseDynamicMethodInvocation(Serialization.DynamicMethodInvocationExpression jObject)
        {
            var methodInstance = ParseExpression(jObject.Method);

            return new DynamicCallInvocationExpression(
                _clrContext,
                LocFromJObject(jObject),
                methodInstance,
                ParseArguments(jObject.Arguments));
        }

        private Node ParseNewAnonymousType(Serialization.NewAnonymoustype jObject)
        {
            var rv = new AnonymousNewExpression(
                _clrContext,
                LocFromJObject(jObject));

            var setters = jObject.Initializers;

            foreach (var setter in setters)
            {
                rv.AddValue(
                    setter.Key,
                    ParseExpression(setter.Value));
            }

            return rv;
        }

        private Node ParseStrCat(Serialization.StrCatExpression jObject)
        {
            if (jObject.Method != 0)
            { return ParseMethodCall(jObject); }

            var args = ParseArguments(jObject.Arguments);
            var instance = ParseExpression(jObject.Instance);
            MethodReference methodReference = null;

            foreach (var method in _clrContext.KnownReferences.String.Resolve().Methods)
            {
                if (method.Name != "Concat")
                { continue; }

                if (method.Parameters.Count == args.Length
                    && method.Parameters[0].ParameterType.IsSame(_clrContext.KnownReferences.Object))
                {
                    methodReference = method;
                    break;
                }
                else if (method.Parameters.Count == 1
                    && method.Parameters[0].ParameterType.IsArray
                    && ((ArrayType)method.Parameters[0].ParameterType).ElementType.IsSame(
                            _clrContext.KnownReferences.Object))
                {
                    methodReference = method;
                }
            }

            // TODO: move methodReferenceExpression to a different JObject node.
            return new MethodCallExpression(
                _clrContext,
                LocFromJObject(jObject),
                GetMethodReferenceExpression(
                    instance,
                    methodReference),
                args);
        }

        private Node ParseDelegateInvocation(Serialization.DelegateInvocationExpression jObject)
            => new MethodCallExpression(
                _clrContext,
                LocFromJObject(jObject),
                ParseExpression(jObject.Instance),
                ParseArguments(jObject.Arguments));

        private Node ParseDefaultValue(Serialization.DefaultValueExpr jObject) => new DefaultValueExpression(
                _clrContext,
                LocFromJObject(jObject),
                DeserializeType(jObject.Type));

        private Node ParseTempVariableAddressReference(Serialization.TempVariableRefExpression jObject) => ParseVariableReference(jObject);

        private Node ParseTempVariableReference(Serialization.TempVariableRefExpression jObject) => ParseVariableReference(jObject);

        private Node ParseTypeExpr(Serialization.TypeExpressionSer jObject)
            => new TypeReferenceExpression(
                _clrContext,
                LocFromJObject(jObject),
                DeserializeType(jObject.Type));

        private Node ParseDelegateCreation(Serialization.DelegateCreationExpression jObject)
        {
            var methodReference = ParseExpression(jObject.Method);
            if (methodReference is LocalFunctionReference lfr)
            {
                return lfr;
            }
            else
            {
                return new DelegateMethodExpression(
                    _clrContext,
                    LocFromJObject(jObject),
                    (MethodReferenceExpression)ParseExpression(jObject.Method),
                    DeserializeType(jObject.Type));
            }
        }

        private Node ParseAnonymousMethod(Serialization.AnonymousMethodBodyExpr jObject)
            => new AnonymousMethodBodyExpression(
                _clrContext,
                LocFromJObject(jObject),
                (ParameterBlock)ParseParameterBlock(jObject.Block, true),
                jObject.Type != 0
                    ? DeserializeType(jObject.Type)
                    : _clrContext.KnownReferences.MulticastDelegate);

        private Node ParseLocalMethodStatement(Serialization.LocalMethodStatement jObject)
        {
            LocalFunctionVariable lfv = null;
            foreach (var scopeBlock in scopeBlockStack)
            {
                if (scopeBlock.Item1 == jObject.ScopeBlockId)
                {
                    lfv = scopeBlock.Item2.ResolveLocalFunctionVariable(jObject.MethodId.MethodName);
                    break;
                }
            }

            if (lfv == null)
            { throw new InvalidOperationException(); }

            return new LocalMethodStatement(
                _clrContext,
                LocFromJObject(jObject),
                lfv,
                (ParameterBlock)ParseParameterBlock(jObject.Block, true));
        }

        private ParameterVariable ParseArgumentVariable(Serialization.ParameterSer jObject)
        {
            if (jObject == null)
            { return null; }

            List<VariableCollector> parameterBlocks = null;

            foreach (var node in scopeBlockStack)
            {
                if (node.collector.IsParamBlock)
                {
                    if (node.collector.GetParameterVariable(jObject.Name, out var rv))
                    {
                        if (parameterBlocks != null)
                        {
                            for (var iParamBlock = 0; iParamBlock < parameterBlocks.Count; iParamBlock++)
                            { parameterBlocks[iParamBlock].AddEscapingVariable(rv); }
                        }

                        return rv;
                    }
                    else
                    {
                        if (parameterBlocks == null)
                        {
                            parameterBlocks = new List<VariableCollector>();
                        }

                        parameterBlocks.Add(node.collector);
                    }
                }
            }

            throw new InvalidOperationException();
        }

        private LocalVariable ParseLocalVariable(Serialization.LocalVariableSer jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            var type = DeserializeType(jObject.Type);
            var name = jObject.Name;
            var blockId = jObject.BlockId;

            List<VariableCollector> parameterBlocks = null;

            foreach (var node in scopeBlockStack)
            {
                if (node.Item1 == blockId)
                {
                    var rv = node.Item2.ResolveVariable(name);

                    if (rv == null)
                    {
                        rv = node.Item2.CreateVariable(
                            name,
                            type);
                    }

                    if (parameterBlocks != null)
                    {
                        for (var iParamBlock = 0; iParamBlock < parameterBlocks.Count; iParamBlock++)
                        {
                            parameterBlocks[iParamBlock].AddEscapingVariable(rv);
                        }
                    }

                    return rv;
                }
                else if (node.Item2.IsParamBlock)
                {
                    if (parameterBlocks == null)
                    {
                        parameterBlocks = new List<VariableCollector>();
                    }

                    parameterBlocks.Add(node.Item2);
                }
            }

            throw new InvalidOperationException();
        }

        private Expression ParseExpression(Serialization.AstBase jObject) => (Expression)ParseNode(jObject);

        private Statement ParseStatement(Serialization.AstBase jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            var rv = ParseNode(jObject);
            if (rv is Expression)
            {
                return new ExpressionStatement(
                    (Expression)rv);
            }

            return (Statement)rv;
        }

        private ScopeBlock GetScopeBlock(Serialization.StatementSer jObject)
        {
            var statement = ParseStatement(jObject);
            if (statement == null)
            {
                return null;
            }

            if (statement is ScopeBlock)
            { return (ScopeBlock)statement; }

            var rv = new ScopeBlock(
                _clrContext,
                statement.Location,
                null,
                null);

            rv.AddStatement(statement);

            return rv;
        }

        private Statement[] ParseStatements(List<Serialization.StatementSer> jArray)
        {
            var rv = new Statement[jArray.Count];

            for (var iStatement = 0; iStatement < jArray.Count; iStatement++)
            { rv[iStatement] = ParseStatement(jArray[iStatement]); }

            return rv;
        }

        private Expression[] ParseExpressions(List<Serialization.ExpressionSer> expressions)
        {
            if (expressions == null)
            { return null; }

            var rv = new Expression[expressions.Count];

            for (var iArg = 0; iArg < expressions.Count; iArg++)
            {
                rv[iArg] = (Expression)ParseNode(expressions[iArg]);
                if (rv[iArg] == null)
                { }
            }

            return rv;
        }

        private Expression[] ParseArguments(List<Serialization.MethodCallArg> arguments)
        {
            if (arguments == null)
            { return new Expression[0]; }

            var rv = new Expression[arguments.Count];
            for (var iArg = 0; iArg < arguments.Count; iArg++)
            {
                var argObject = arguments[iArg];
                var argValue = argObject.Value;

                rv[iArg] = ParseExpression(argValue);

                if (argObject.IsByRef)
                {
                    rv[iArg] = new LoadAddressExpression(
                        _clrContext,
                        rv[iArg].Location,
                        rv[iArg]);
                }
            }

            return rv;
        }

        private Location LocFromJObject(Serialization.AstBase jObject)
        {
            if (jObject.Location == null)
            { return null; }

            return new Location(
                _currentMethodFileName,
                jObject.Location.StartLine,
                jObject.Location.StartColumn,
                jObject.Location.EndLine,
                jObject.Location.EndColumn);
        }

        private TypeReference DeserializeType(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = _typeInfo.Types[jObject];
            return tmp == null
                ? null
                : _memberReferenceDeserializer.DeserializeType(tmp);
        }

        private MethodReference DeserializeMethod(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = _typeInfo.Methods[jObject];
            return tmp == null
                ? null
                : _memberReferenceDeserializer.DeserializeMethod(tmp);
        }

        private FieldReference DeserializeField(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = _typeInfo.Fields[jObject];
            return tmp == null
                ? null
                : _memberReferenceDeserializer.DeserializeField(tmp);
        }

        private PropertyReference DeserializeProperty(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = _typeInfo.Properties[jObject];

            var getter = tmp.Getter != null
                ? DeserializeMethod(tmp.Getter.Value)
                : null;

            var setter = tmp.Setter != null
                ? DeserializeMethod(tmp.Setter.Value)
                : null;

            return new NScript.CLR.AST.InternalPropertyReference(
                getter,
                setter);
        }

        private EventReference DeserializeEvent(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = _typeInfo.Events[jObject];

            var addOnMethod = tmp.AddOn != null
                ? DeserializeMethod(tmp.AddOn.Value)
                : null;

            var removeOnMethod = tmp.RemoveOn != null
                ? DeserializeMethod(tmp.RemoveOn.Value)
                : null;

            return new NScript.CLR.AST.InternalEventReference(
                addOnMethod,
                removeOnMethod);
        }

        private MethodReferenceExpression GetMethodReferenceExpression(
            Expression instanceExpression,
            MethodReference methodRef,
            Location loc = null)
        {
            var methodDef = methodRef.Resolve();
            if (methodDef.IsStatic)
            {
                return new MethodReferenceExpression(
                    _clrContext,
                    loc,
                    methodRef);
            }

            if (methodDef.IsConstructor
                || !methodDef.IsVirtual
                || instanceExpression is BaseVariableReference
                || methodDef.DeclaringType.IsValueType)
            {
                return new MethodReferenceExpression(
                    _clrContext,
                    loc,
                    methodRef,
                    instanceExpression);
            }
            else
            {
                return new VirtualMethodReferenceExpression(
                    _clrContext,
                    loc,
                    methodRef,
                    instanceExpression.ResultType.IsValueType ?
                        new BoxExpression(
                            _clrContext,
                            instanceExpression.Location,
                            instanceExpression)
                        : instanceExpression);
            }
        }

        private Dictionary<Type, Func<Serialization.AstBase, Node>> BuildParserMap()
        {
            var parserMap = new Dictionary<Type, Func<Serialization.AstBase, Node>>
            {
                {
                    typeof(Serialization.NullExpression),
                    (a) => ParseNullLiteral((Serialization.NullExpression)a)
                },
                {
                    typeof(Serialization.BoolLiteralExpression),
                    (a) => ParseBoolLiteral((Serialization.BoolLiteralExpression)a)
                },
                {
                    typeof(Serialization.CharLiteralExpression),
                    (a) => ParseCharLiteral((Serialization.CharLiteralExpression)a)
                },
                {
                    typeof(Serialization.ByteLiteralExpression),
                    (a) => ParseByteLiteral((Serialization.ByteLiteralExpression)a)
                },
                {
                    typeof(Serialization.SByteLiteralExpression),
                    (a) => ParseSByteLiteral((Serialization.SByteLiteralExpression)a)
                },
                {
                    typeof(Serialization.ShortLiteralExpression),
                    (a) => ParseShortLiteral((Serialization.ShortLiteralExpression)a)
                },
                {
                    typeof(Serialization.UShortLiteralExpression),
                    (a) => ParseLiteral((Serialization.UShortLiteralExpression)a)
                },
                {
                    typeof(Serialization.IntLiteralExpression),
                    (a) => ParseIntLiteral((Serialization.IntLiteralExpression)a)
                },
                {
                    typeof(Serialization.UIntLiteralExpression),
                    (a) => ParseUIntLiteral((Serialization.UIntLiteralExpression)a)
                },
                {
                    typeof(Serialization.LongLiteralExpression),
                    (a) => ParseLongLiteral((Serialization.LongLiteralExpression)a)
                },
                {
                    typeof(Serialization.ULongLiteralExpression),
                    (a) => ParseULongLiteral((Serialization.ULongLiteralExpression)a)
                },
                {
                    typeof(Serialization.FloatLiteralExpression),
                    (a) => ParseFloatLiteral((Serialization.FloatLiteralExpression)a)
                },
                {
                    typeof(Serialization.DoubleLiteralExpression),
                    (a) => ParseDoubleLiteral((Serialization.DoubleLiteralExpression)a)
                },
                {
                    typeof(Serialization.DecimalLiteralExpression),
                    (a) => ParseDecimalLiteral((Serialization.DecimalLiteralExpression)a)
                },
                {
                    typeof(Serialization.StringLiteralExpression),
                    (a) => ParseStringLiteral((Serialization.StringLiteralExpression)a)
                },
                {
                    typeof(Serialization.AssignExpression),
                    (a) => ParseAssignment((Serialization.AssignExpression)a)
                },
                {
                    typeof(Serialization.UserDefinedBinaryOrUnaryOpExpression),
                    (a) => ParseUserDefinedOperators((Serialization.UserDefinedBinaryOrUnaryOpExpression)a)
                },
                {
                    typeof(Serialization.MethodCallExpression),
                    (a) => ParseMethodCall((Serialization.MethodCallExpression)a)
                },
                {
                    typeof(Serialization.BinaryExpression),
                    (a) => ParseBinaryExpr((Serialization.BinaryExpression)a)
                },
                {
                    typeof(Serialization.UnaryExpression),
                    (a) => ParseUnaryExpr((Serialization.UnaryExpression)a)
                },
                {
                    typeof(Serialization.TypeCastExpression),
                    (a) => ParseTypeCast((Serialization.TypeCastExpression)a)
                },
                {
                    typeof(Serialization.ByteConstantExpression),
                    (a) => ParseConstant((Serialization.ByteConstantExpression)a)
                },
                {
                    typeof(Serialization.SbyteConstantExpression),
                    (a) => ParseConstant((Serialization.SbyteConstantExpression)a)
                },
                {
                    typeof(Serialization.ShortConstantExpression),
                    (a) => ParseConstant((Serialization.ShortConstantExpression)a)
                },
                {
                    typeof(Serialization.UshortConstantExpression),
                    (a) => ParseConstant((Serialization.UshortConstantExpression)a)
                },
                {
                    typeof(Serialization.IntConstantExpression),
                    (a) => ParseConstant((Serialization.IntConstantExpression)a)
                },
                {
                    typeof(Serialization.UintConstantExpression),
                    (a) => ParseConstant((Serialization.UintConstantExpression)a)
                },
                {
                    typeof(Serialization.LongConstantExpression),
                    (a) => ParseConstant((Serialization.LongConstantExpression)a)
                },
                {
                    typeof(Serialization.UlongConstantExpression),
                    (a) => ParseConstant((Serialization.UlongConstantExpression)a)
                },
                {
                    typeof(Serialization.FloatConstantExpression),
                    (a) => ParseConstant((Serialization.FloatConstantExpression)a)
                },
                {
                    typeof(Serialization.DoubleConstantExpression),
                    (a) => ParseConstant((Serialization.DoubleConstantExpression)a)
                },
                {
                    typeof(Serialization.StringConstantExpression),
                    (a) => ParseConstant((Serialization.StringConstantExpression)a)
                },
                {
                    typeof(Serialization.CharConstantExpression),
                    (a) => ParseConstant((Serialization.CharConstantExpression)a)
                },
                {
                    typeof(Serialization.NullConstantExpression),
                    (a) => ParseConstant((Serialization.NullConstantExpression)a)
                },
                {
                    typeof(Serialization.BoolConstantExpression),
                    (a) => ParseConstant((Serialization.BoolConstantExpression)a)
                },
                {
                    typeof(Serialization.EmptyStatementSer),
                    (a) => ParseEmptyStatement((Serialization.EmptyStatementSer)a)
                },
                {
                    typeof(Serialization.StatementExpressionSer),
                    (a) => ParseStatementExpr((Serialization.StatementExpressionSer)a)
                },
                {
                    typeof(Serialization.StatementListSer),
                    (a) => ParseStatementList((Serialization.StatementListSer)a)
                },
                {
                    typeof(Serialization.ReturnStatement),
                    (a) => ParseReturnStatement((Serialization.ReturnStatement)a)
                },
                {
                    typeof(Serialization.ThrowExpression),
                    (a) => ParseThrowStatment((Serialization.ThrowExpression)a)
                },
                {
                    typeof(Serialization.BreakStatement),
                    (a) => ParseBreakExpression((Serialization.BreakStatement)a)
                },
                {
                    typeof(Serialization.ContinueStatement),
                    (a) => ParseContinueExpression((Serialization.ContinueStatement)a)
                },
                {
                    typeof(Serialization.IfStatement),
                    (a) => ParseIfStatement((Serialization.IfStatement)a)
                },
                {
                    typeof(Serialization.DoStatement),
                    (a) => ParseDoWhileStatement((Serialization.DoStatement)a)
                },
                {
                    typeof(Serialization.WhileStatement),
                    (a) => ParseWhileStatement((Serialization.WhileStatement)a)
                },
                {
                    typeof(Serialization.ForStatement),
                    (a) => ParseForStatement((Serialization.ForStatement)a)
                },
                {
                    typeof(Serialization.ForEachStatement),
                    (a) => ParseForEachStatement((Serialization.ForEachStatement)a)
                },
                {
                    typeof(Serialization.SwitchStatement),
                    (a) => ParseSwitchStatement((Serialization.SwitchStatement)a)
                },
                {
                    typeof(Serialization.ExplicitBlockSer),
                    (a) => ParseScopeBlock((Serialization.ExplicitBlockSer)a)
                },
                {
                    typeof(Serialization.BlockSer),
                    (a) => ParseBlock((Serialization.BlockSer)a)
                },
                {
                    typeof(Serialization.ParameterBlock),
                    (a) => ParseParameterBlock((Serialization.ParameterBlock)a)
                },
                {
                    typeof(Serialization.TryFinallyBlockSer),
                    (a) => ParseTryFinally((Serialization.TryFinallyBlockSer)a)
                },
                {
                    typeof(Serialization.TryCatchBlock),
                    (a) => ParseTryCatch((Serialization.TryCatchBlock)a)
                },
                {
                    typeof(Serialization.NullCoalescingOperatorSer),
                    (a) => ParseNullCoalascing((Serialization.NullCoalescingOperatorSer)a)
                },
                {
                    typeof(Serialization.YieldStatement),
                    (a) => ParseYield((Serialization.YieldStatement)a)
                },
                {
                    typeof(Serialization.YieldBreakStatement),
                    (a) => ParseYield((Serialization.YieldBreakStatement)a)
                },
                {
                    typeof(Serialization.IteratorBlock),
                    (a) => ParseIterator((Serialization.IteratorBlock)a)
                },
                {
                    typeof(Serialization.IsExpression),
                    (a) => ParseIsExpr((Serialization.IsExpression)a)
                },
                {
                    typeof(Serialization.AsExpression),
                    (a) => ParseAsExpr((Serialization.AsExpression)a)
                },
                {
                    typeof(Serialization.ConditionalExpression),
                    (a) => ParseConditional((Serialization.ConditionalExpression)a)
                },
                {
                    typeof(Serialization.ConditionalAccess),
                    (a) => ParseConditionalAccess((Serialization.ConditionalAccess)a)
                },
                {
                    typeof(Serialization.ConditionalReceiver),
                    (a) => ParseConditionalReceiver((Serialization.ConditionalReceiver)a)
                },
                {
                    typeof(Serialization.LocalVariableRefExpression),
                    (a) => ParseVariableReference((Serialization.LocalVariableRefExpression)a)
                },
                {
                    typeof(Serialization.ParameterReferenceExpression),
                    (a) => ParseParameterReference((Serialization.ParameterReferenceExpression)a)
                },
                {
                    typeof(Serialization.NewExpression),
                    (a) => ParseNew((Serialization.NewExpression)a)
                },
                {
                    typeof(Serialization.ArrayCreationExpression),
                    (a) => ParseArrayCreation((Serialization.ArrayCreationExpression)a)
                },
                {
                    typeof(Serialization.ThisExpression),
                    (a) => ParseThisExpr((Serialization.ThisExpression)a)
                },
                {
                    typeof(Serialization.TypeOfExpression),
                    (a) => ParseTypeOf((Serialization.TypeOfExpression)a)
                },
                {
                    typeof(Serialization.ElementAccessExpression),
                    (a) => ParseArrayElementAccess((Serialization.ElementAccessExpression)a)
                },
                {
                    typeof(Serialization.BaseThisExpression),
                    (a) => ParseBaseExpr((Serialization.BaseThisExpression)a)
                },
                {
                    typeof(Serialization.NewInitializerExpression),
                    (a) => ParseNewInitializer((Serialization.NewInitializerExpression)a)
                },
                {
                    typeof(Serialization.NewCollectionInitializerExpression),
                    (a) => ParseNewCollectionInitializer((Serialization.NewCollectionInitializerExpression)a)
                },
                {
                    typeof(Serialization.MethodExpression),
                    (a) => ParseMethodExpr((Serialization.MethodExpression)a)
                },
                {
                    typeof(Serialization.LocalMethodExpression),
                    (a) => ParseLocalMethodExpr((Serialization.LocalMethodExpression)a)
                },
                {
                    typeof(Serialization.FieldExpression),
                    (a) => ParseFieldExpr((Serialization.FieldExpression)a)
                },
                {
                    typeof(Serialization.PropertyExpression),
                    (a) => ParsePropertyExpr((Serialization.PropertyExpression)a)
                },
                {
                    typeof(Serialization.EventExpression),
                    (a) => ParseEventExpr((Serialization.EventExpression)a)
                },
                {
                    typeof(Serialization.IndexExpression),
                    (a) => ParsePropertyExpr((Serialization.IndexExpression)a)
                },
                {
                    typeof(Serialization.DelegateInvocationExpression),
                    (a) => ParseDelegateInvocation((Serialization.DelegateInvocationExpression)a)
                },
                {
                    typeof(Serialization.DefaultValueExpr),
                    (a) => ParseDefaultValue((Serialization.DefaultValueExpr)a)
                },
                {
                    typeof(Serialization.TempVariableRefExpression),
                    (a) => ParseTempVariableReference((Serialization.TempVariableRefExpression)a)
                },
                {
                    typeof(Serialization.TypeExpressionSer),
                    (a) => ParseTypeExpr((Serialization.TypeExpressionSer)a)
                },
                {
                    typeof(Serialization.DelegateCreationExpression),
                    (a) => ParseDelegateCreation((Serialization.DelegateCreationExpression)a)
                },
                {
                    typeof(Serialization.BoxCastExpression),
                    (a) => ParseBoxExpr((Serialization.BoxCastExpression)a)
                },
                {
                    typeof(Serialization.AnonymousMethodBodyExpr),
                    (a) => ParseAnonymousMethod((Serialization.AnonymousMethodBodyExpr)a)
                },
                {
                    typeof(Serialization.LocalMethodStatement),
                    (a) => ParseLocalMethodStatement((Serialization.LocalMethodStatement)a)
                },
                {
                    typeof(Serialization.LocalMethodCallExpression),
                    (a) => ParseMethodCall((Serialization.LocalMethodCallExpression)a)
                },
                {
                    typeof(Serialization.WrapExpression),
                    (a) => ParseToNullable((Serialization.WrapExpression)a)
                },
                {
                    typeof(Serialization.NullableToNormal),
                    (a) => ParseNullableToNormal((Serialization.NullableToNormal)a)
                },
                {
                    typeof(Serialization.UnwrapExpression),
                    (a) => ParseFromNullable((Serialization.UnwrapExpression)a)
                },
                {
                    typeof(Serialization.DynamicIndexBinderExpression),
                    (a) => ParseDynamicIndexBinder((Serialization.DynamicIndexBinderExpression)a)
                },
                {
                    typeof(Serialization.DynamicMemberExpression),
                    (a) => ParseDynamicMemberExpression((Serialization.DynamicMemberExpression)a)
                },
                {
                    typeof(Serialization.DynamicMethodBinderExpression),
                    (a) => ParseDynamicMemberBinder((Serialization.DynamicMethodBinderExpression)a)
                },
                {
                    typeof(Serialization.DynamicMethodInvocationExpression),
                    (a) => ParseDynamicMethodInvocation((Serialization.DynamicMethodInvocationExpression)a)
                },
                {
                    typeof(Serialization.NewAnonymoustype),
                    (a) => ParseNewAnonymousType((Serialization.NewAnonymoustype)a)
                },
                {
                    typeof(Serialization.StrCatExpression),
                    (a) => ParseStrCat((Serialization.StrCatExpression)a)
                },
                {
                    typeof(Serialization.VariableBlockDeclaration),
                    (a) => ParseVariableInitializers((Serialization.VariableBlockDeclaration)a)
                },
                {
                    typeof(Serialization.TupleLiteral),
                    (a) => ParseTupleLiteral((Serialization.TupleLiteral)a)
                },
                {
                    typeof(Serialization.DeconstructTupleAssignment),
                    (a) => ParseTupleDeconstruct((Serialization.DeconstructTupleAssignment)a)
                },
                {
                    typeof(Serialization.TupleCreationExpression),
                    (a) => ParseTupleCreation((Serialization.TupleCreationExpression)a)
                }
            };

            return parserMap;
        }

        private Node ParseTupleCreation(Serialization.TupleCreationExpression a)
        {
            throw new NotImplementedException();
        }

        private Node ParseTupleDeconstruct(Serialization.DeconstructTupleAssignment tupleDeconstruct)
        {
            return new TupleDeconstructExpression(
                _clrContext,
                LocFromJObject(tupleDeconstruct),
                this.ParseExpressions(tupleDeconstruct.LHSArgs),
                this.ParseExpression(tupleDeconstruct.RightTuple));
        }

        private Node ParseTupleLiteral(Serialization.TupleLiteral tupleLiteral)
        {
            return new TupleLiteral(
                _clrContext,
                LocFromJObject(tupleLiteral),
                DeserializeType(tupleLiteral.TupleType),
                this.ParseExpressions(tupleLiteral.TupleArgs));
        }

        private T WrapVariableCollection<T>(
            Func<VariableCollector, T> func,
            int blockId,
            bool isParamBlock,
            ParameterDefinition thisParameter = null,
            List<ParameterDefinition> paramDefinitions = null)
        {
            // Let's skip creating nested collector with same Id.
            if (scopeBlockStack.Count > 0 && scopeBlockStack.First.Value.id == blockId)
            { return func(scopeBlockStack.First.Value.collector); }

            var vc = new VariableCollector(
                blockId,
                isParamBlock,
                thisParameter,
                paramDefinitions);

            _ = scopeBlockStack.AddFirst((blockId, vc));

            try
            { return func(vc); }
            finally
            { scopeBlockStack.RemoveFirst(); }
        }
    }
}