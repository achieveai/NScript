//-----------------------------------------------------------------------
// <copyright file="BondToAst.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using NScript.CLR;
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using NScript.CLR.AST;
    using NScript.Utils;
    using System.Linq;

    /// <summary>
    /// Definition for BondToAst
    /// </summary>
    public class BondToAst
    {
        private ClrContext _clrContext;
        private Dictionary<Type, Func<Serialization.AstBase, Node>> _parserMap;
        private MethodDefinition _currentMethod;
        private string _currentMethodFileName;
        private MemberReferenceDeserializer _memberReferenceDeserializer;
        private LinkedList<(int id, VariableCollector collector)> scopeBlockStack = new LinkedList<(int, VariableCollector)>();
        private Serialization.TypeInfoSer _typeInfo;

        public BondToAst(
            Serialization.TypeInfoSer typeInfo,
            ClrContext context)
        {
            this._parserMap = this.BuildParserMap();
            this._typeInfo = typeInfo;
            this._memberReferenceDeserializer
                = new MemberReferenceDeserializer(context);
            this._clrContext = context;
        }

        public MethodDefinition CurrentMethod
        {
            get { return this._currentMethod; }
        }

        public Tuple<MethodDefinition, Func<TopLevelBlock>> ParseMethodBody(
            Serialization.MethodBody jObject)
        {
            MethodDefinition method = this.DeserializeMethod(jObject.MethodId).Resolve();

            if (method.Name == "GenericMethodCall3")
            { }

            return new Tuple<MethodDefinition, Func<TopLevelBlock>>(
                method,
                () =>
                {
                    string fileName = jObject.FileName;

                    this._currentMethod = method;
                    this._currentMethodFileName = fileName;
                    this._memberReferenceDeserializer.SetMethodContext(this._currentMethod);
                    try
                    {

                        var methodBlockObject = jObject.Body;
                        if (methodBlockObject != null)
                        {
                            TopLevelBlock rv = new TopLevelBlock(method);
                            rv.RootBlock = (ParameterBlock)this.ParseNode(methodBlockObject);
                            this._currentMethod = null;
                            this._currentMethodFileName = null;

                            return rv;
                        }
                        else
                        { return null; }
                    }
                    finally
                    {
                        this._memberReferenceDeserializer.SetMethodContext(null);
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

            return this._parserMap[jObject.GetType()](jObject);
        }

        private Node ParseNullLiteral(Serialization.NullExpression jObject)
        {
            return new NullLiteral(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseBoolLiteral(Serialization.BoolLiteralExpression jObject)
        {
            return new BooleanLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseCharLiteral(Serialization.CharLiteralExpression jObject)
        {
            return new CharLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseLongLiteral(Serialization.LongLiteralExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseULongLiteral(Serialization.ULongLiteralExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                (long)jObject.Value);
        }

        private Node ParseIntLiteral(Serialization.IntLiteralExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseUIntLiteral(Serialization.UIntLiteralExpression jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseStringLiteral(Serialization.StringLiteralExpression jObject)
        {
            return new StringLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseDecimalLiteral(Serialization.DecimalLiteralExpression jObject)
        {
            throw new NotImplementedException();
        }

        private Node ParseDoubleLiteral(Serialization.DoubleLiteralExpression jObject)
        {
            return new DoubleLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseFloatLiteral(Serialization.FloatLiteralExpression jObject)
        {
            return new DoubleLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseByteLiteral(Serialization.ByteLiteralExpression jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseSByteLiteral(Serialization.SByteLiteralExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseShortLiteral(Serialization.ShortLiteralExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseLiteral(Serialization.UShortLiteralExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseAssignment(Serialization.AssignExpression jObject)
        {
            return new BinaryExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Left),
                this.ParseExpression(jObject.Right),
                BinaryOperator.Assignment);
        }

        private Node ParseUserDefinedOperators(Serialization.UserDefinedBinaryOrUnaryOpExpression jObject)
        {
            return ParseMethodCall(jObject);
        }

        private Node ParseMethodCall(Serialization.MethodCallExpression jObject)
        {
            Expression instance = this.ParseExpression(jObject.Instance);
            MethodReference methodReference = this.DeserializeMethod(jObject.Method);

            // TODO: move methodReferenceExpression to a different JObject node.
            return new MethodCallExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.GetMethodReferenceExpression(
                    instance,
                    methodReference),
                this.ParseArguments(jObject.Arguments));
        }

        private Node ParseBinaryExpr(Serialization.BinaryExpression jObject)
        {
            return new BinaryExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Left),
                this.ParseExpression(jObject.Right),
                (BinaryOperator)jObject.Operator,
                jObject.IsLifted);
        }

        private Node ParseUnaryExpr(Serialization.UnaryExpression jObject)
        {
            return new UnaryExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression),
                (UnaryOperator)jObject.Operator,
                jObject.IsLifted);
        }

        private Node ParseTypeCast(Serialization.TypeCastExpression jObject)
        {
            if (jObject.IsUnbox)
            {
                return new UnboxExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.ParseExpression(jObject.Expression),
                    this.DeserializeType(jObject.Type));
            }

            var innerExpression =
                this.ParseExpression(jObject.Expression);
            var type =
                this.DeserializeType(jObject.Type);

            // Avoid un necessary cast expressions.
            if (MemberReferenceComparer.Instance.Equals(
                innerExpression.ResultType,
                type))
            {
                return innerExpression;
            }
            else if (innerExpression is FromNullable)
            {
                FromNullable fromNullable = (FromNullable)innerExpression;
                if (MemberReferenceComparer.Instance.Equals(type, fromNullable.InnerExpression.ResultType))
                {
                    return fromNullable.InnerExpression;
                }
            }

            return new TypeCheckExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                innerExpression,
                type,
                TypeCheckType.CastType);
        }

        private Node ParseNullableToNormal(Serialization.NullableToNormal jObject)
            => new FromNullable(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression));

        private Node ParseToNullable(Serialization.WrapExpression jObject)
            => new ToNullable(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression));

        private Node ParseFromNullable(Serialization.UnwrapExpression jObject)
            => new FromNullable(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression));


        private Node ParseConstant(Serialization.ByteConstantExpression jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.SbyteConstantExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.ShortConstantExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.UshortConstantExpression jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.IntConstantExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.UintConstantExpression jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.LongConstantExpression jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.UlongConstantExpression jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.FloatConstantExpression jObject)
        {
            return new DoubleLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.DoubleConstantExpression jObject)
        {
            return new DoubleLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.DecimalConstantExpression jObject)
        {
            return new DoubleLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                (double)jObject.Value);
        }

        private Node ParseConstant(Serialization.StringConstantExpression jObject)
        {
            return new StringLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.CharConstantExpression jObject)
        {
            return new CharLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.BoolConstantExpression jObject)
        {
            return new BooleanLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value);
        }

        private Node ParseConstant(Serialization.NullConstantExpression jObject)
        {
            return new NullLiteral(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseEmptyStatement(Serialization.EmptyStatementSer jObject)
        {
            return new ExplicitBlock(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseStatementExpr(Serialization.StatementExpressionSer jObject)
        { return new ExpressionStatement(this.ParseExpression(jObject.Expression)); }

        private Node ParseStatementList(Serialization.StatementListSer jObject)
        {
            return new ExplicitBlock(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseStatements(jObject.Statements));
        }

        private Node ParseReturnStatement(Serialization.ReturnStatement jObject)
        {
            return new ReturnStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression));
        }

        private Node ParseThrowStatment(Serialization.ThrowExpression jObject)
        {
            return new ThrowStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression));
        }

        private Node ParseBreakExpression(Serialization.BreakStatement jObject)
        {
            return new BreakStatement(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseContinueExpression(Serialization.ContinueStatement jObject)
        {
            return new ContinueStatement(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseVariableInitializers(Serialization.VariableBlockDeclaration jObject)
        {
            var expressions = this.ParseExpressions(jObject.Initializers);
            if (expressions != null && expressions.Length == 1)
            { return new ExpressionStatement(expressions[0]); }
            else if (expressions == null)
            { expressions = new Expression[0]; }

            return new InitializerStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                expressions);
        }

        private Node ParseIfStatement(Serialization.IfStatement jObject)
        {
            var trueStatement = this.GetScopeBlock(jObject.TrueStatement);
            var falseStatement = this.GetScopeBlock(jObject.FalseStatement);

            if (trueStatement == null && falseStatement == null)
            {
                return new ExpressionStatement(
                    this.ParseExpression(jObject.Condition));
            }
            else
            {
                return new IfBlockStatement(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.ParseExpression(jObject.Condition),
                    trueStatement,
                    falseStatement);
            }
        }

        private Node ParseDoWhileStatement(Serialization.DoStatement jObject)
        {
            return new DoWhileLoop(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Condition),
                this.GetScopeBlock(jObject.Loop));
        }

        private Node ParseWhileStatement(Serialization.WhileStatement jObject)
        {
            return new WhileLoop(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Condition),
                this.GetScopeBlock(jObject.Loop));
        }

        private Node ParseForStatement(Serialization.ForStatement jObject)
        {
            var variableCollector = new VariableCollector(jObject.BlockId);
            this.scopeBlockStack.AddFirst((jObject.BlockId, variableCollector));
            try
            {
                return new ForLoop(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.ParseExpression(jObject.Condition),
                    this.ParseStatement(jObject.Initializer),
                    this.ParseStatement(jObject.Iterator),
                    this.GetScopeBlock(jObject.Loop),
                    variableCollector.GetCapturedVariables());
            }finally
            { this.scopeBlockStack.RemoveFirst(); }
        }

        private Node ParseForEachStatement(Serialization.ForEachStatement jObject)
        {
            return WrapVariableCollection(
                (vc) =>
                {
                    var iterator = this.ParseExpression(jObject.Collection);
                    var body = this.GetScopeBlock(jObject.Loop);
                    var variables = vc.GetCapturedVariables();
                    var localVariable = variables
                        .Where(_ => _.variable.Name == jObject.LocalVariableName)
                        .Select(_ => _.variable)
                        .FirstOrDefault();

                    return new ForEachLoop(
                        this._clrContext,
                        this.LocFromJObject(jObject),
                        localVariable,
                        iterator,
                        body,
                        vc.GetCapturedVariables());
                },
                jObject.BlockId,
                false);
        }

        private Node ParseSwitchStatement(Serialization.SwitchStatement jObject)
        {
            List<KeyValuePair<List<LiteralExpression>, Statement>> caseBlocks =
                new List<KeyValuePair<List<LiteralExpression>, Statement>>();

            var sectionArray = jObject.Blocks;
            for (int iSection = 0; iSection < sectionArray.Count; iSection++)
            {
                var sectionObj = sectionArray[iSection].Block;
                var labelJArray = sectionArray[iSection].Labels;

                List<LiteralExpression> labels =
                    new List<LiteralExpression>(labelJArray.Count);

                for (int iLabel = 0; iLabel < labelJArray.Count; iLabel++)
                {
                    if (labelJArray[iLabel] == null)
                    { labels.Add(null); }
                    else
                    {
                        labels.Add((LiteralExpression)
                            this.ParseExpression(labelJArray[iLabel].LabelValue));
                    }
                }

                caseBlocks.Add(
                    new KeyValuePair<List<LiteralExpression>, Statement>(
                        labels,
                        this.ParseStatement(sectionObj)));
            }

            return new SwitchStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.SwitchExpression),
                caseBlocks);
        }

        private Node ParseScopeBlock(Serialization.ExplicitBlockSer jObject)
        {
            return WrapVariableCollection(
                (vc) =>
                {
                    var statements = new List<Statement>();
                    if (jObject.Statements != null)
                    {
                        foreach (var statement in this.ParseStatements(jObject.Statements))
                        { statements.Add(statement); }
                    }
                    var rv = new ScopeBlock(
                        this._clrContext,
                        this.LocFromJObject(jObject),
                        vc.GetCapturedVariables());

                    statements.ForEach(_ => rv.AddStatement(_));

                    if (rv.Statements.Count == 1)
                    {
                        Statement singleStatement = rv.Statements[0];
                        ForEachLoop feLoop = singleStatement as ForEachLoop;
                        if (feLoop != null)
                        {
                            feLoop.MoveVariablesFrom(rv);
                            return feLoop;
                        }

                        ForLoop forLoop = singleStatement as ForLoop;
                        if (forLoop != null)
                        {
                            forLoop.MoveVariablesFrom(rv);
                            return forLoop;
                        }
                    }

                    return rv;
                },
                jObject.Id,
                false);
        }

        private Node ParseBlock(Serialization.BlockSer jObject)
        {
            return new ExplicitBlock(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Statements.Select(_ => this.ParseStatement(_))
                    .ToArray());
        }

        private Node ParseParameterBlock(Serialization.ParameterBlock jObject)
            => this.ParseParameterBlock(jObject, false);

        private Node ParseParameterBlock(Serialization.ParameterBlock jObject, bool isDelegate)
        {
            if (this._currentMethod.Name == "InstanceReferencingDelegate")
            { }

            return WrapVariableCollection(
                (vc) =>
                {
                    var statements = new List<Statement>();
                    if (jObject.Statements != null)
                    {
                        foreach (var statement in this.ParseStatements(jObject.Statements))
                        { statements.Add(statement); }
                    }

                    ParameterBlock rv = new ParameterBlock(
                        this._clrContext,
                        this.LocFromJObject(jObject),
                        vc.GetCapturedVariables(),
                        vc.GetParamBlockVariables());

                    statements.ForEach(_ => rv.AddStatement(_));

                    return rv;
                },
                jObject.Id,
                true,
                jObject.IsMethodOwned
                        && !isDelegate
                        && this._currentMethod.HasThis
                        ?  new ParameterDefinition(
                            "this",
                            ParameterAttributes.None,
                            this._currentMethod.DeclaringType)
                        : null,
                jObject.Parameters != null
                    ? jObject.Parameters
                        .Select(paramObj =>
                        {
                            ParameterAttributes attr = (ParameterAttributes)paramObj.Modifier;

                            var parameterType =
                                    this.DeserializeType(paramObj.Type);

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
            Statement tryBlock = this.ParseStatement(jObject.TryBlock);
            TryCatchFinally tryCatchFinallyBlock = tryBlock as TryCatchFinally;
            var finallyScope = this.GetScopeBlock(jObject.FinallyBlock);

            HandlerBlock finallyBlock =
                new HandlerBlock(
                    this._clrContext,
                    finallyScope.Location,
                    null,
                    null,
                    finallyScope);

            if (tryCatchFinallyBlock != null)
            {
                tryCatchFinallyBlock.AddHandler(finallyBlock);
                return tryCatchFinallyBlock;
            }

            ScopeBlock tryScopeBlock = tryBlock as ScopeBlock;
            if (tryScopeBlock == null)
            {
                tryScopeBlock = new ScopeBlock(
                    this._clrContext,
                    tryBlock.Location,
                    null);
                tryScopeBlock.AddStatement(tryBlock);
            }

            return new TryCatchFinally(
                this._clrContext,
                this.LocFromJObject(jObject),
                tryScopeBlock,
                finallyBlock);
        }

        private Node ParseTryCatch(Serialization.TryCatchBlock jObject)
        {
            var tryBlock = this.GetScopeBlock(jObject.TryBlock);
            var handlerList = new List<HandlerBlock>();

            TryCatchFinally rv = null;
            var handlersArray = jObject.CatchBlocks;
            for (int iHandler = 0; iHandler < handlersArray.Count; iHandler++)
            {
                var handlerObj = handlersArray[iHandler];

                var handlerBlock = WrapVariableCollection(
                    (vc) =>
                    {
                        ScopeBlock handlerScopeBlock = this.GetScopeBlock(handlerObj.Block);
                        LocalVariable exceptionVariable =
                            this.ParseLocalVariable(handlerObj.LocalVariable);

                        if (exceptionVariable != null)
                        {
                            ScopeBlock tmpBlock = new ScopeBlock(
                                this._clrContext,
                                null,
                                new List<(LocalVariable localVariable, bool isUsed)> { (exceptionVariable, true) });

                            handlerScopeBlock.MoveVariablesFrom(tmpBlock);
                        }

                        return new HandlerBlock(
                            this._clrContext,
                            this.LocFromJObject(handlerObj),
                            this.DeserializeType(handlerObj.CatchType ?? 0)
                                ?? this._clrContext.KnownReferences.Exception,
                            exceptionVariable != null
                                ? new VariableReference(
                                        this._clrContext,
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
                        this._clrContext,
                        this.LocFromJObject(jObject),
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

        private Node ParseNullCoalascing(Serialization.NullCoalescingOperatorSer jObject)
        {
            return new NullConditional(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Left),
                this.ParseExpression(jObject.Right));
        }

        private Node ParseYield(Serialization.YieldStatement jObject)
        {
            return new YieldStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression));
        }

        private Node ParseIterator(Serialization.IteratorBlock jObject)
        {
            return new InlineIteratorExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                (ParameterBlock)this.GetScopeBlock(jObject.Block),
                this.DeserializeType(jObject.Type));
        }

        private Node ParseBoxExpr(Serialization.BoxCastExpression jObject)
        {
            return new BoxExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression));
        }

        private Node ParseIsExpr(Serialization.IsExpression jObject)
        {
            return new TypeCheckExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression),
                this.DeserializeType(jObject.Type),
                TypeCheckType.IsType);
        }

        private Node ParseAsExpr(Serialization.AsExpression jObject)
        {
            return new TypeCheckExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Expression),
                this.DeserializeType(jObject.Type),
                TypeCheckType.AsType);
        }

        private Node ParseConditional(Serialization.ConditionalExpression jObject)
        {
            return new ConditionalOperatorExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Condition),
                this.ParseExpression(jObject.TrueExpression),
                this.ParseExpression(jObject.FalseExpression));
        }

        private Node ParseVariableReference(Serialization.LocalVariableRefExpression jObject)
        {
            if (jObject.IsAddressReference)
            {
                return new LoadAddressExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    new VariableReference(
                        this._clrContext,
                        this.LocFromJObject(jObject),
                        this.ParseLocalVariable(jObject.LocalVariable)));
            }
            else
            {
                return new VariableReference(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.ParseLocalVariable(jObject.LocalVariable));
            }
        }

        private Node ParseParameterReference(Serialization.ParameterReferenceExpression jObject)
        {
            // TODO: figure out how to deal with Ref varaibles.
            var parameter = this.ParseArgumentVariable(jObject.Parameter);

            if (((ParameterDefinition)parameter.ParameterReference).ParameterType.IsByReference)
            {
                return new VariableAddressReference(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    parameter);
            }

            return new VariableReference(
                this._clrContext,
                this.LocFromJObject(jObject),
                parameter);
        }

        private Node ParseNew(Serialization.NewExpression jObject)
        {
            var methodObj = jObject.Method;
            if (methodObj == 0)
            {
                // This is struct and should be initialized using default value constructor.
                return new DefaultValueExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.DeserializeType(jObject.Type));
            }

            return new NewObjectExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeMethod(methodObj),
                this.ParseArguments(jObject.Arguments));
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

                argExpression = this.ParseExpression(arguments[0]);
            }

            if (initializers == null
                || initializers.Count == 0)
            {
                if (initializers != null
                    && initializers.Count == 0)
                {
                    argExpression = new IntLiteral(this._clrContext, this.LocFromJObject(jObject), (int)0);
                }
                else if (argExpression == null)
                {
                    throw new InvalidOperationException();
                }

                return new NewArrayExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.DeserializeType(jObject.ElementType),
                    argExpression);
            }

            IList<Expression> initializerExpressions = this.ParseExpressions(initializers);

            return new InlineArrayInitialization(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeType(jObject.ElementType),
                initializerExpressions);
        }

        private Node ParseThisExpr(Serialization.ThisExpression jObject)
        {
            var thisVar = this.scopeBlockStack.Last.Value.collector.ThisVariable;
            var node = this.scopeBlockStack.Last.Previous;

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
                this._clrContext,
                this.LocFromJObject(jObject),
                thisVar);
        }

        private Node ParseTypeOf(Serialization.TypeOfExpression jObject)
        {
            return new TypeofExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                new TypeReferenceExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.DeserializeType(jObject.Type)));
        }

        private Node ParseArrayElementAccess(Serialization.ElementAccessExpression jObject)
        {
            return new ArrayElementExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Left),
                this.ParseArguments(jObject.Arguments)[0]);
        }

        private Node ParseBaseExpr(Serialization.BaseThisExpression jObject)
        {
            var thisVariable = this.scopeBlockStack.Last.Value.Item2.ThisVariable;
            var node = this.scopeBlockStack.Last.Previous;
            while (node != null)
            {
                var collector = node.Value.Item2;
                if (collector.IsParamBlock)
                { collector.AddEscapingVariable(thisVariable); }

                node = node.Previous;
            }

            return new BaseVariableReference(
                this._clrContext,
                this.LocFromJObject(jObject),
                thisVariable);
        }

        private Node ParseNewInitializer(Serialization.NewInitializerExpression jObject)
        {
            var initializerArray = jObject.Initializers;

            var args = this.ParseArguments(jObject.Arguments);

            List<Tuple<MemberReferenceExpression, Expression[]>> setters =
                new List<Tuple<MemberReferenceExpression, Expression[]>>();

            if (initializerArray != null)
            {
                for (int iInit = 0; iInit < initializerArray.Count; iInit++)
                {
                    var initObj = initializerArray[iInit];
                    MemberReferenceExpression memberReferenceExpression = null;

                    if (initObj.Setter != null)
                    {
                        PropertyReference propertyReference = new InternalPropertyReference(
                            null,
                            this.DeserializeMethod(initObj.Setter.Value));

                        memberReferenceExpression = new PropertyReferenceExpression(
                            this._clrContext,
                            this.LocFromJObject(initObj),
                            propertyReference,
                            null);

                        setters.Add(
                            Tuple.Create(
                                memberReferenceExpression,
                                new Expression[] { this.ParseExpression(initObj.Value) }));
                    }
                    else if (initObj.Field != null)
                    {
                        memberReferenceExpression = new FieldReferenceExpression(
                            this._clrContext,
                            this.LocFromJObject(initObj),
                            this.DeserializeField(initObj.Field.Value),
                            null);

                        setters.Add(
                            Tuple.Create(
                                memberReferenceExpression,
                                new Expression[] { this.ParseExpression(initObj.Value) }));
                    }
                    else
                    {
                        memberReferenceExpression = new MethodReferenceExpression(
                            this._clrContext,
                            this.LocFromJObject(initObj),
                            this.DeserializeMethod(initObj.MethodCall.Method),
                            null);

                        var arguments = this.ParseArguments(initObj.MethodCall.Arguments);

                        setters.Add(
                            Tuple.Create(
                                memberReferenceExpression,
                                arguments));
                    }
                }
            }

            return new InlinePropertyInitilizationExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                new NewObjectExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.DeserializeMethod(jObject.Method),
                    args),
                setters); ;
        }

        private Node ParseMethodExpr(Serialization.MethodExpression jObject)
        {
            var methodReference = this.DeserializeMethod(jObject.Method);

            if (jObject.GenericParameters != null && jObject.GenericParameters.Count > 0)
            {
                var typeReferences = jObject.GenericParameters
                    .Select(_ => this.DeserializeType(_))
                    .ToList();

                var genericMethodReference = new GenericInstanceMethod(methodReference);
                foreach (var typeReference in typeReferences)
                { genericMethodReference.GenericArguments.Add(typeReference); }

                methodReference = genericMethodReference;
            }

            var instance = this.ParseExpression(jObject.Instance);

            var location = this.LocFromJObject(jObject);

            if (instance == null)
            {
                return new MethodReferenceExpression(
                    this._clrContext,
                    location,
                    methodReference);
            }

            var methodDef = methodReference.Resolve();
            bool isVirtual = methodDef.IsVirtual
                && !(instance is BaseVariableReference);

            if (isVirtual)
            {
                return new VirtualMethodReferenceExpression(
                    this._clrContext,
                    location,
                    methodReference,
                    instance.ResultType.IsValueType ?
                        new BoxExpression(
                            this._clrContext,
                            instance.Location,
                            instance)
                        : instance);
            }

            return new MethodReferenceExpression(
                this._clrContext,
                location,
                methodReference,
                instance);
        }

        private Node ParseFieldExpr(Serialization.FieldExpression jObject)
        {
            FieldReference fieldReference = this.DeserializeField(jObject.Field);

            var instance = this.ParseExpression(jObject.Instance);

            var location = this.LocFromJObject(jObject);

            if (instance == null)
            {
                return new FieldReferenceExpression(
                    this._clrContext,
                    location,
                    fieldReference);
            }

            return new FieldReferenceExpression(
                this._clrContext,
                location,
                fieldReference,
                instance);
        }

        private Node ParsePropertyExpr(Serialization.PropertyExpression jObject)
        {
            PropertyReference propertyReference = this.DeserializeProperty(jObject.Property);

            var instance = this.ParseExpression(jObject.Instance);

            var location = this.LocFromJObject(jObject);

            var args = jObject is Serialization.IndexExpression
                ? this.ParseArguments(((Serialization.IndexExpression)jObject).Arguments)
                : null;

            if (instance == null)
            {
                return new PropertyReferenceExpression(
                    this._clrContext,
                    location,
                    propertyReference);
            }

            return new PropertyReferenceExpression(
                this._clrContext,
                location,
                propertyReference,
                instance,
                args);
        }

        private Node ParseEventExpr(Serialization.EventExpression jObject)
        {
            EventReference eventReference = this.DeserializeEvent(jObject.Event);

            var instance = this.ParseExpression(jObject.Instance);

            var location = this.LocFromJObject(jObject);

            if (instance == null)
            {
                return new EventReferenceExpression(
                    this._clrContext,
                    location,
                    eventReference);
            }

            return new EventReferenceExpression(
                this._clrContext,
                location,
                eventReference,
                instance);
        }

        private Node ParseDynamicIndexBinder(Serialization.DynamicIndexBinderExpression jObject)
        {
            return new DynamicIndexAccessor(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Instance),
                this.ParseExpression(jObject.Index));
        }

        private Node ParseDynamicMemberExpression(Serialization.DynamicMemberExpression jObject)
        {
            return new DynamicMemberAccessor(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Instance),
                jObject.MemberName);
        }

        private Node ParseDynamicMemberBinder(Serialization.DynamicMethodBinderExpression jObject)
        {
            return new DynamicMemberAccessor(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Instance),
                jObject.Name);
        }

        private Node ParseDynamicMethodInvocation(Serialization.DynamicMethodInvocationExpression jObject)
        {
            Expression methodInstance = this.ParseExpression(jObject.Method);

            return new DynamicCallInvocationExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                methodInstance,
                this.ParseArguments(jObject.Arguments));
        }

        private Node ParseNewAnonymousType(Serialization.NewAnonymoustype jObject)
        {
            AnonymousNewExpression rv = new AnonymousNewExpression(
                this._clrContext,
                this.LocFromJObject(jObject));

            var setters = jObject.Initializers;

            foreach (var setter in setters)
            {
                rv.AddValue(
                    setter.Key,
                    this.ParseExpression(setter.Value));
            }

            return rv;
        }

        private Node ParseStrCat(Serialization.StrCatExpression jObject)
        {
            if (jObject.Method != 0)
            { return this.ParseMethodCall(jObject); }

            var args = this.ParseArguments(jObject.Arguments);
            Expression instance = this.ParseExpression(jObject.Instance);
            MethodReference methodReference = null;

            foreach (var method in this._clrContext.KnownReferences.String.Resolve().Methods)
            {
                if (method.Name != "Concat")
                {continue;}

                if (method.Parameters.Count == args.Length
                    && method.Parameters[0].ParameterType.IsSame(this._clrContext.KnownReferences.Object))
                {
                    methodReference = method;
                    break;
                }
                else if (method.Parameters.Count == 1
                    && method.Parameters[0].ParameterType.IsArray
                    && ((ArrayType)method.Parameters[0].ParameterType).ElementType.IsSame(
                            this._clrContext.KnownReferences.Object))
                {
                    methodReference = method;
                }
            }

            // TODO: move methodReferenceExpression to a different JObject node.
            return new MethodCallExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.GetMethodReferenceExpression(
                    instance,
                    methodReference),
                args);
        }

        private Node ParseDelegateInvocation(Serialization.DelegateInvocationExpression jObject)
        {
            return new MethodCallExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Instance),
                this.ParseArguments(jObject.Arguments));
        }

        private Node ParseDefaultValue(Serialization.DefaultValueExpr jObject)
        {
            return new DefaultValueExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeType(jObject.Type));
        }

        private Node ParseTempVariableAddressReference(Serialization.TempVariableRefExpression jObject)
        {
            return this.ParseVariableReference(jObject);
        }

        private Node ParseTempVariableReference(Serialization.TempVariableRefExpression jObject)
        {
            return this.ParseVariableReference(jObject);
        }

        private Node ParseTypeExpr(Serialization.TypeExpressionSer jObject)
        {
            return new TypeReferenceExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeType(jObject.Type));
        }

        private Node ParseDelegateCreation(Serialization.DelegateCreationExpression jObject)
        {
            return new DelegateMethodExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                (MethodReferenceExpression)this.ParseExpression(jObject.Method),
                this.DeserializeType(jObject.Type));
        }

        private Node ParseAnonymousMethod(Serialization.AnonymousMethodBodyExpr jObject)
        {
            return new AnonymousMethodBodyExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                (ParameterBlock)this.ParseParameterBlock(jObject.Block, true),
                jObject.Type != 0
                    ? this.DeserializeType(jObject.Type)
                    : this._clrContext.KnownReferences.MulticastDelegate);
        }

        private ParameterVariable ParseArgumentVariable(Serialization.ParameterSer jObject)
        {
            if (jObject == null)
            { return null; }

            List<VariableCollector> parameterBlocks = null;

            foreach (var node in this.scopeBlockStack)
            {
                if (node.collector.IsParamBlock)
                {
                    ParameterVariable rv;
                    if (node.collector.GetParameterVariable(jObject.Name, out rv))
                    {
                        if (parameterBlocks != null)
                        {
                            for (int iParamBlock = 0; iParamBlock < parameterBlocks.Count; iParamBlock++)
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

            var type = this.DeserializeType(jObject.Type);
            var name = jObject.Name;
            var blockId = jObject.BlockId;

            List<VariableCollector> parameterBlocks = null;

            foreach (var node in this.scopeBlockStack)
            {
                if (node.Item1 == blockId)
                {
                    LocalVariable rv = node.Item2.ResolveVariable(name);

                    if (rv == null)
                    {
                        rv = node.Item2.CreateVariable(
                            name,
                            type);
                    }

                    if (parameterBlocks != null)
                    {
                        for (int iParamBlock = 0; iParamBlock < parameterBlocks.Count; iParamBlock++)
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

        private Expression ParseExpression(Serialization.AstBase jObject)
        {
            return (Expression)this.ParseNode(jObject);
        }

        private Statement ParseStatement(Serialization.AstBase jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            Node rv = this.ParseNode(jObject);
            if (rv is Expression)
            {
                return new ExpressionStatement(
                    (Expression)rv);
            }

            return (Statement)rv;
        }

        private ScopeBlock GetScopeBlock(Serialization.StatementSer jObject)
        {
            Statement statement = this.ParseStatement(jObject);
            if (statement == null)
            {
                return null;
            }

            if (statement is ScopeBlock)
            { return (ScopeBlock)statement; }

            var rv = new ScopeBlock(
                this._clrContext,
                statement.Location,
                null);

            rv.AddStatement(statement);

            return rv;
        }

        private Statement[] ParseStatements(List<Serialization.StatementSer> jArray)
        {
            Statement[] rv = new Statement[jArray.Count];

            for (int iStatement = 0; iStatement < jArray.Count; iStatement++)
            { rv[iStatement] = this.ParseStatement(jArray[iStatement]); }

            return rv;
        }

        private Expression[] ParseExpressions(List<Serialization.ExpressionSer> expressions)
        {
            if (expressions == null)
            { return null; }

            Expression[] rv = new Expression[expressions.Count];

            for (int iArg = 0; iArg < expressions.Count; iArg++)
            { rv[iArg] = (Expression)this.ParseNode(expressions[iArg]); }

            return rv;
        }

        private Expression[] ParseArguments(List<Serialization.MethodCallArg> arguments)
        {
            if (arguments == null)
            { return new Expression[0]; }

            Expression[] rv = new Expression[arguments.Count];
            for (int iArg = 0; iArg < arguments.Count; iArg++)
            {
                var argObject = arguments[iArg];
                var argValue = argObject.Value;

                rv[iArg] = this.ParseExpression(argValue);

                if (argObject.IsByRef)
                {
                    rv[iArg] = new LoadAddressExpression(
                        this._clrContext,
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
                this._currentMethodFileName,
                jObject.Location.StartLine,
                jObject.Location.StartColumn,
                jObject.Location.EndLine,
                jObject.Location.EndColumn);
        }

        private TypeReference DeserializeType(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = this._typeInfo.Types[jObject];
            return tmp == null
                ? null
                : this._memberReferenceDeserializer.DeserializeType(tmp);
        }

        private MethodReference DeserializeMethod(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = this._typeInfo.Methods[jObject];
            return tmp == null
                ? null
                : this._memberReferenceDeserializer.DeserializeMethod(tmp);
        }

        private FieldReference DeserializeField(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = this._typeInfo.Fields[jObject];
            return tmp == null
                ? null
                : this._memberReferenceDeserializer.DeserializeField(tmp);
        }

        private PropertyReference DeserializeProperty(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = this._typeInfo.Properties[jObject];

            var getter = tmp.Getter != null
                ? this.DeserializeMethod(tmp.Getter.Value)
                : null;

            var setter = tmp.Setter != null
                ? this.DeserializeMethod(tmp.Setter.Value)
                : null;

            return new NScript.CLR.AST.InternalPropertyReference(
                getter,
                setter);
        }

        private EventReference DeserializeEvent(int jObject)
        {
            if (jObject == 0) { return null; }
            var tmp = this._typeInfo.Events[jObject];

            var addOnMethod = tmp.AddOn != null
                ? this.DeserializeMethod(tmp.AddOn.Value)
                : null;

            var removeOnMethod = tmp.RemoveOn != null
                ? this.DeserializeMethod(tmp.RemoveOn.Value)
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
            MethodDefinition methodDef = methodRef.Resolve();
            if (methodDef.IsStatic)
            {
                return new MethodReferenceExpression(
                    this._clrContext,
                    loc,
                    methodRef);
            }

            if (methodDef.IsConstructor
                || !methodDef.IsVirtual
                || instanceExpression is BaseVariableReference
                || methodDef.DeclaringType.IsValueType)
            {
                return new MethodReferenceExpression(
                    this._clrContext,
                    loc,
                    methodRef,
                    instanceExpression);
            }
            else
            {
                return new VirtualMethodReferenceExpression(
                    this._clrContext,
                    loc,
                    methodRef,
                    instanceExpression.ResultType.IsValueType ?
                        new BoxExpression(
                            this._clrContext,
                            instanceExpression.Location,
                            instanceExpression)
                        : instanceExpression);
            }
        }

        private Dictionary<Type, Func<Serialization.AstBase, Node>> BuildParserMap()
        {
            Dictionary<Type, Func<Serialization.AstBase, Node>> parserMap = new Dictionary<Type, Func<Serialization.AstBase, Node>>();
            parserMap.Add(
					typeof(Serialization.NullExpression),
					(a) => this.ParseNullLiteral((Serialization.NullExpression)a));
            parserMap.Add(
					typeof(Serialization.BoolLiteralExpression),
					(a) => this.ParseBoolLiteral((Serialization.BoolLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.CharLiteralExpression),
					(a) => this.ParseCharLiteral((Serialization.CharLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.ByteLiteralExpression),
					(a) => this.ParseByteLiteral((Serialization.ByteLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.SByteLiteralExpression),
					(a) => this.ParseSByteLiteral((Serialization.SByteLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.ShortLiteralExpression),
					(a) => this.ParseShortLiteral((Serialization.ShortLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.UShortLiteralExpression),
					(a) => this.ParseLiteral((Serialization.UShortLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.IntLiteralExpression),
					(a) => this.ParseIntLiteral((Serialization.IntLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.UIntLiteralExpression),
					(a) => this.ParseUIntLiteral((Serialization.UIntLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.LongLiteralExpression),
					(a) => this.ParseLongLiteral((Serialization.LongLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.ULongLiteralExpression),
					(a) => this.ParseULongLiteral((Serialization.ULongLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.FloatLiteralExpression),
					(a) => this.ParseFloatLiteral((Serialization.FloatLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.DoubleLiteralExpression),
					(a) => this.ParseDoubleLiteral((Serialization.DoubleLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.DecimalLiteralExpression),
					(a) => this.ParseDecimalLiteral((Serialization.DecimalLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.StringLiteralExpression),
					(a) => this.ParseStringLiteral((Serialization.StringLiteralExpression)a));
            parserMap.Add(
					typeof(Serialization.AssignExpression),
					(a) => this.ParseAssignment((Serialization.AssignExpression)a));
            parserMap.Add(
                    typeof(Serialization.UserDefinedBinaryOrUnaryOpExpression),
                    (a) => this.ParseUserDefinedOperators((Serialization.UserDefinedBinaryOrUnaryOpExpression)a));
            parserMap.Add(
					typeof(Serialization.MethodCallExpression),
					(a) => this.ParseMethodCall((Serialization.MethodCallExpression)a));
            parserMap.Add(
					typeof(Serialization.BinaryExpression),
					(a) => this.ParseBinaryExpr((Serialization.BinaryExpression)a));
            parserMap.Add(
					typeof(Serialization.UnaryExpression),
					(a) => this.ParseUnaryExpr((Serialization.UnaryExpression)a));
            parserMap.Add(
					typeof(Serialization.TypeCastExpression),
					(a) => this.ParseTypeCast((Serialization.TypeCastExpression)a));
            parserMap.Add(
					typeof(Serialization.ByteConstantExpression),
					(a) => this.ParseConstant((Serialization.ByteConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.SbyteConstantExpression),
					(a) => this.ParseConstant((Serialization.SbyteConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.ShortConstantExpression),
					(a) => this.ParseConstant((Serialization.ShortConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.UshortConstantExpression),
					(a) => this.ParseConstant((Serialization.UshortConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.IntConstantExpression),
					(a) => this.ParseConstant((Serialization.IntConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.UintConstantExpression),
					(a) => this.ParseConstant((Serialization.UintConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.LongConstantExpression),
					(a) => this.ParseConstant((Serialization.LongConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.UlongConstantExpression),
					(a) => this.ParseConstant((Serialization.UlongConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.FloatConstantExpression),
					(a) => this.ParseConstant((Serialization.FloatConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.DoubleConstantExpression),
					(a) => this.ParseConstant((Serialization.DoubleConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.StringConstantExpression),
					(a) => this.ParseConstant((Serialization.StringConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.CharConstantExpression),
					(a) => this.ParseConstant((Serialization.CharConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.NullConstantExpression),
					(a) => this.ParseConstant((Serialization.NullConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.BoolConstantExpression),
					(a) => this.ParseConstant((Serialization.BoolConstantExpression)a));
            parserMap.Add(
					typeof(Serialization.EmptyStatementSer),
					(a) => this.ParseEmptyStatement((Serialization.EmptyStatementSer)a));
            parserMap.Add(
					typeof(Serialization.StatementExpressionSer),
					(a) => this.ParseStatementExpr((Serialization.StatementExpressionSer)a));
            parserMap.Add(
					typeof(Serialization.StatementListSer),
					(a) => this.ParseStatementList((Serialization.StatementListSer)a));
            parserMap.Add(
					typeof(Serialization.ReturnStatement),
					(a) => this.ParseReturnStatement((Serialization.ReturnStatement)a));
            parserMap.Add(
					typeof(Serialization.ThrowExpression),
					(a) => this.ParseThrowStatment((Serialization.ThrowExpression)a));
            parserMap.Add(
					typeof(Serialization.BreakStatement),
					(a) => this.ParseBreakExpression((Serialization.BreakStatement)a));
            parserMap.Add(
					typeof(Serialization.ContinueStatement),
					(a) => this.ParseContinueExpression((Serialization.ContinueStatement)a));
            parserMap.Add(
					typeof(Serialization.IfStatement),
					(a) => this.ParseIfStatement((Serialization.IfStatement)a));
            parserMap.Add(
					typeof(Serialization.DoStatement),
					(a) => this.ParseDoWhileStatement((Serialization.DoStatement)a));
            parserMap.Add(
					typeof(Serialization.WhileStatement),
					(a) => this.ParseWhileStatement((Serialization.WhileStatement)a));
            parserMap.Add(
					typeof(Serialization.ForStatement),
					(a) => this.ParseForStatement((Serialization.ForStatement)a));
            parserMap.Add(
					typeof(Serialization.ForEachStatement),
					(a) => this.ParseForEachStatement((Serialization.ForEachStatement)a));
            parserMap.Add(
					typeof(Serialization.SwitchStatement),
					(a) => this.ParseSwitchStatement((Serialization.SwitchStatement)a));
            parserMap.Add(
					typeof(Serialization.ExplicitBlockSer),
					(a) => this.ParseScopeBlock((Serialization.ExplicitBlockSer)a));
            parserMap.Add(
					typeof(Serialization.BlockSer),
					(a) => this.ParseBlock((Serialization.BlockSer)a));
            parserMap.Add(
					typeof(Serialization.ParameterBlock),
					(a) => this.ParseParameterBlock((Serialization.ParameterBlock)a));
            parserMap.Add(
					typeof(Serialization.TryFinallyBlockSer),
					(a) => this.ParseTryFinally((Serialization.TryFinallyBlockSer)a));
            parserMap.Add(
					typeof(Serialization.TryCatchBlock),
					(a) => this.ParseTryCatch((Serialization.TryCatchBlock)a));
            parserMap.Add(
					typeof(Serialization.NullCoalescingOperatorSer),
					(a) => this.ParseNullCoalascing((Serialization.NullCoalescingOperatorSer)a));
            parserMap.Add(
					typeof(Serialization.YieldStatement),
					(a) => this.ParseYield((Serialization.YieldStatement)a));
            parserMap.Add(
					typeof(Serialization.IteratorBlock),
					(a) => this.ParseIterator((Serialization.IteratorBlock)a));
            parserMap.Add(
					typeof(Serialization.IsExpression),
					(a) => this.ParseIsExpr((Serialization.IsExpression)a));
            parserMap.Add(
					typeof(Serialization.AsExpression),
					(a) => this.ParseAsExpr((Serialization.AsExpression)a));
            parserMap.Add(
					typeof(Serialization.ConditionalExpression),
					(a) => this.ParseConditional((Serialization.ConditionalExpression)a));
            parserMap.Add(
					typeof(Serialization.LocalVariableRefExpression),
					(a) => this.ParseVariableReference((Serialization.LocalVariableRefExpression)a));
            parserMap.Add(
					typeof(Serialization.ParameterReferenceExpression),
					(a) => this.ParseParameterReference((Serialization.ParameterReferenceExpression)a));
            parserMap.Add(
					typeof(Serialization.NewExpression),
					(a) => this.ParseNew((Serialization.NewExpression)a));
            parserMap.Add(
					typeof(Serialization.ArrayCreationExpression),
					(a) => this.ParseArrayCreation((Serialization.ArrayCreationExpression)a));
            parserMap.Add(
					typeof(Serialization.ThisExpression),
					(a) => this.ParseThisExpr((Serialization.ThisExpression)a));
            parserMap.Add(
					typeof(Serialization.TypeOfExpression),
					(a) => this.ParseTypeOf((Serialization.TypeOfExpression)a));
            parserMap.Add(
					typeof(Serialization.ElementAccessExpression),
					(a) => this.ParseArrayElementAccess((Serialization.ElementAccessExpression)a));
            parserMap.Add(
					typeof(Serialization.BaseThisExpression),
					(a) => this.ParseBaseExpr((Serialization.BaseThisExpression)a));
            parserMap.Add(
					typeof(Serialization.NewInitializerExpression),
					(a) => this.ParseNewInitializer((Serialization.NewInitializerExpression)a));
            parserMap.Add(
					typeof(Serialization.MethodExpression),
					(a) => this.ParseMethodExpr((Serialization.MethodExpression)a));
            parserMap.Add(
					typeof(Serialization.FieldExpression),
					(a) => this.ParseFieldExpr((Serialization.FieldExpression)a));
            parserMap.Add(
					typeof(Serialization.PropertyExpression),
					(a) => this.ParsePropertyExpr((Serialization.PropertyExpression)a));
            parserMap.Add(
					typeof(Serialization.EventExpression),
					(a) => this.ParseEventExpr((Serialization.EventExpression)a));
            parserMap.Add(
					typeof(Serialization.IndexExpression),
					(a) => this.ParsePropertyExpr((Serialization.IndexExpression)a));
            parserMap.Add(
					typeof(Serialization.DelegateInvocationExpression),
					(a) => this.ParseDelegateInvocation((Serialization.DelegateInvocationExpression)a));
            parserMap.Add(
					typeof(Serialization.DefaultValueExpr),
					(a) => this.ParseDefaultValue((Serialization.DefaultValueExpr)a));
            parserMap.Add(
					typeof(Serialization.TempVariableRefExpression),
					(a) => this.ParseTempVariableReference((Serialization.TempVariableRefExpression)a));
            parserMap.Add(
					typeof(Serialization.TypeExpressionSer),
					(a) => this.ParseTypeExpr((Serialization.TypeExpressionSer)a));
            parserMap.Add(
					typeof(Serialization.DelegateCreationExpression),
					(a) => this.ParseDelegateCreation((Serialization.DelegateCreationExpression)a));
            parserMap.Add(
					typeof(Serialization.BoxCastExpression),
					(a) => this.ParseBoxExpr((Serialization.BoxCastExpression)a));
            parserMap.Add(
					typeof(Serialization.AnonymousMethodBodyExpr),
					(a) => this.ParseAnonymousMethod((Serialization.AnonymousMethodBodyExpr)a));
            parserMap.Add(
					typeof(Serialization.WrapExpression),
					(a) => this.ParseToNullable((Serialization.WrapExpression)a));
            parserMap.Add(
					typeof(Serialization.NullableToNormal),
					(a) => this.ParseNullableToNormal((Serialization.NullableToNormal)a));
            parserMap.Add(
					typeof(Serialization.UnwrapExpression),
					(a) => this.ParseFromNullable((Serialization.UnwrapExpression)a));
            parserMap.Add(
					typeof(Serialization.DynamicIndexBinderExpression),
					(a) => this.ParseDynamicIndexBinder((Serialization.DynamicIndexBinderExpression)a));
            parserMap.Add(
					typeof(Serialization.DynamicMemberExpression),
					(a) => this.ParseDynamicMemberExpression((Serialization.DynamicMemberExpression)a));
            parserMap.Add(
					typeof(Serialization.DynamicMethodBinderExpression),
					(a) => this.ParseDynamicMemberBinder((Serialization.DynamicMethodBinderExpression)a));
            parserMap.Add(
                    typeof(Serialization.DynamicMethodInvocationExpression),
                    (a) => this.ParseDynamicMethodInvocation((Serialization.DynamicMethodInvocationExpression)a));
            parserMap.Add(
					typeof(Serialization.NewAnonymoustype),
					(a) => this.ParseNewAnonymousType((Serialization.NewAnonymoustype)a));
            parserMap.Add(
					typeof(Serialization.StrCatExpression),
					(a) => this.ParseStrCat((Serialization.StrCatExpression)a));
            parserMap.Add(typeof(Serialization.VariableBlockDeclaration),
                    (a) => this.ParseVariableInitializers((Serialization.VariableBlockDeclaration)a));

            return parserMap;
        }

        private T WrapVariableCollection<T>(
            Func<VariableCollector, T> func,
            int blockId,
            bool isParamBlock,
            ParameterDefinition thisParameter = null,
            List<ParameterDefinition> paramDefinitions = null)
        {
            // Let's skip creating nested collector with same Id.
            if (this.scopeBlockStack.Count > 0 && this.scopeBlockStack.First.Value.id == blockId)
            { return func(this.scopeBlockStack.First.Value.collector); }

            var vc = new VariableCollector(
                blockId,
                isParamBlock,
                thisParameter,
                paramDefinitions);
            this.scopeBlockStack.AddFirst((blockId, vc));
            try
            { return func(vc); }
            finally
            { this.scopeBlockStack.RemoveFirst(); }
        }
    }
}
