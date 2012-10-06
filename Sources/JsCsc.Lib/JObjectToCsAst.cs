//-----------------------------------------------------------------------
// <copyright file="JObjectToCsAst.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Utils;
    using Mono.Cecil;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Definition for JObjectToCsAst
    /// </summary>
    public class JObjectToCsAst
    {
        private ClrContext _clrContext;
        private Dictionary<string, Func<JObject, Node>> _parserMap;
        private MethodDefinition _currentMethod;
        private string _currentMethodFileName;
        private MemberReferenceDeserializer _memberReferenceDeserializer;
        private LinkedList<Tuple<int, ScopeBlock>> scopeBlockStack = new LinkedList<Tuple<int, ScopeBlock>>();

        public JObjectToCsAst(ClrContext context)
        {
            this._parserMap = this.BuildParserMap();
            this._memberReferenceDeserializer = new MemberReferenceDeserializer(this, context);
            this._clrContext = context;
        }

        public MethodDefinition CurrentMethod
        {
            get { return this._currentMethod; }
        }

        public Tuple<MethodDefinition, TopLevelBlock> ParseMethodBody(
            JObject jObject)
        {
            MethodDefinition method = this.DeserializeMethod(
                jObject.Value<JObject>(NameTokens.Method)).Resolve();

            string fileName = jObject.Value<string>(NameTokens.FileName);

            this._currentMethod = method;
            this._currentMethodFileName = fileName;
            this._memberReferenceDeserializer.SetMethodContext(this._currentMethod);
            try
            {

                JObject methodBlockObject = jObject.Value<JObject>(NameTokens.Block);
                if (methodBlockObject != null)
                {
                    TopLevelBlock rv = new TopLevelBlock(method);
                    rv.RootBlock = (ParameterBlock)this.ParseNode(methodBlockObject);
                    this._currentMethod = null;
                    this._currentMethodFileName = null;

                    return Tuple.Create(method, rv);
                }
                else
                {
                    return Tuple.Create(method, (TopLevelBlock)null);
                }
            }
            finally
            {
                this._memberReferenceDeserializer.SetMethodContext(null);
            }
        }

        public Node ParseNode(
            JObject jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            string typeName = jObject.Value<string>(NameTokens.TypeName);

            return this._parserMap[typeName](jObject);
        }

        private Node ParseNullLiteral(JObject jObject)
        {
            return new NullLiteral(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseBoolLiteral(JObject jObject)
        {
            return new BooleanLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<bool>(NameTokens.Value));
        }

        private Node ParseCharLiteral(JObject jObject)
        {
            return new CharLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<char>(NameTokens.Value));
        }

        private Node ParseLongLiteral(JObject jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<long>(NameTokens.Value));
        }

        private Node ParseIntLiteral(JObject jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<int>(NameTokens.Value));
        }

        private Node ParseUIntLiteral(JObject jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<uint>(NameTokens.Value));
        }

        private Node ParseStringLiteral(JObject jObject)
        {
            return new StringLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<string>(NameTokens.Value));
        }

        private Node ParseDecimalLiteral(JObject jObject)
        {
            throw new NotImplementedException();
        }

        private Node ParseDoubleLiteral(JObject jObject)
        {
            return new DoubleLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<double>(NameTokens.Value));
        }

        private Node ParseFloatLiteral(JObject jObject)
        {
            return new DoubleLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<float>(NameTokens.Value));
        }

        private Node ParseAssignment(JObject jObject)
        {
            return new BinaryExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.LeftExpr)),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.RightExpr)),
                BinaryOperator.Assignment);
        }

        private Node ParseMethodCall(JObject jObject)
        {
            Expression instance = this.ParseExpression(jObject.Value<JObject>(NameTokens.Instance));
            MethodReference methodReference = this.DeserializeMethod(
                jObject.Value<JObject>(NameTokens.Method));

            // TODO: move methodReferenceExpression to a different JObject node.
            return new MethodCallExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.GetMethodReferenceExpression(
                    instance,
                    methodReference),
                this.ParseArguments(jObject.Value<JArray>(NameTokens.Arguments)));
        }

        private Node ParseBinaryExpr(JObject jObject)
        {
            BinaryOperator op;

            if (!Enum.TryParse<BinaryOperator>(
                jObject.Value<string>(NameTokens.Operator),
                true,
                out op))
            {
                throw new InvalidOperationException();
            }

            return new BinaryExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.LeftExpr)),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.RightExpr)),
                op,
                jObject.Value<bool>(NameTokens.IsLifted));
        }

        private Node ParseUnaryExpr(JObject jObject)
        {
            UnaryOperator op;

            if (!Enum.TryParse<UnaryOperator>(
                jObject.Value<string>(NameTokens.Operator),
                true,
                out op))
            {
                throw new InvalidOperationException();
            }

            return new UnaryExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)),
                op,
                jObject.Value<bool>(NameTokens.IsLifted));
        }

        private Node ParseTypeCast(JObject jObject)
        {
            var innerExpression =
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr));
            var type =
                this.DeserializeType(jObject.Value<JObject>(NameTokens.Type));

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

        private Node ParseToNullable(JObject jObject)
        {
            return new ToNullable(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)));
        }

        private Node ParseFromNullable(JObject jObject)
        {
            return new FromNullable(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)));
        }

        private Node ParseByteLiteral(JObject jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                (byte)jObject.Value<int>(NameTokens.Value));
        }

        private Node ParseSByteLiteral(JObject jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                (sbyte)jObject.Value<int>(NameTokens.Value));
        }

        private Node ParseShortLiteral(JObject jObject)
        {
            return new IntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                (short)jObject.Value<int>(NameTokens.Value));
        }

        private Node ParseUShortConstant(JObject jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                (ushort)jObject.Value<uint>(NameTokens.Value));
        }

        private Node ParseEmptyStatement(JObject jObject)
        {
            return new ExplicitBlock(
                this._clrContext,
                null);
        }

        private Node ParseStatementExpr(JObject jObject)
        {
            return new ExpressionStatement(
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)));
        }

        private Node ParseStatementList(JObject jObject)
        {
            return new ExplicitBlock(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseStatements(jObject.Value<JArray>(NameTokens.Value)));
        }

        private Node ParseReturnStatement(JObject jObject)
        {
            return new ReturnStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Value)));
        }

        private Node ParseThrowStatment(JObject jObject)
        {
            return new ThrowStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Value)));
        }

        private Node ParseBreakExpression(JObject jObject)
        {
            return new BreakStatement(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseContinueExpression(JObject jObject)
        {
            return new ContinueStatement(
                this._clrContext,
                this.LocFromJObject(jObject));
        }

        private Node ParseVariableInitializers(JObject jObject)
        {
            var expressions = this.ParseExpressions(jObject.Value<JArray>(NameTokens.Value));
            if (expressions.Length == 1)
            {
                return new ExpressionStatement(expressions[0]);
            }

            return new InitializerStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                expressions);
        }

        private Node ParseIfStatement(JObject jObject)
        {
            var trueStatement = this.GetScopeBlock(jObject.Value<JObject>(NameTokens.TrueStatement));
            var falseStatement = this.GetScopeBlock(jObject.Value<JObject>(NameTokens.FalseCondition));

            return new IfBlockStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Condition)),
                trueStatement,
                falseStatement);
        }

        private Node ParseDoWhileStatement(JObject jObject)
        {
            return new DoWhileLoop(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Condition)),
                this.GetScopeBlock(jObject.Value<JObject>(NameTokens.Loop)));
        }

        private Node ParseWhileStatement(JObject jObject)
        {
            return new WhileLoop(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Condition)),
                this.GetScopeBlock(jObject.Value<JObject>(NameTokens.Loop)));
        }

        private Node ParseForStatement(JObject jObject)
        {
            return new ForLoop(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Condition)),
                this.ParseStatement(jObject.Value<JObject>(NameTokens.Initializer)),
                this.ParseStatement(jObject.Value<JObject>(NameTokens.Iterator)),
                this.GetScopeBlock(jObject.Value<JObject>(NameTokens.Loop)));
        }

        private Node ParseULongLiteral(JObject jObject)
        {
            return new UIntLiteral(
                this._clrContext,
                this.LocFromJObject(jObject),
                jObject.Value<ulong>(NameTokens.Value));
        }

        private Node ParseForEachStatement(JObject jObject)
        {
            string localVariableName = jObject.Val<string>(NameTokens.LocalVariable);
            var iterator = this.ParseExpression(jObject.Value<JObject>(NameTokens.Iterator));
            var body = this.GetScopeBlock(jObject.Value<JObject>(NameTokens.Loop));

            LocalVariable localVariable = null;
            foreach (var tmpVar in body.LocalVariables)
            {
                if (tmpVar.Name == localVariableName)
                {
                    localVariable = tmpVar;
                    break;
                }
            }

            var loopBlock = body.Statements[0] as ScopeBlock;
            if (loopBlock == null)
            {
                loopBlock = new ScopeBlock(
                    this._clrContext,
                    body.Statements[0].Location);

                loopBlock.AddStatement(
                    body.Statements[0]);
            }

            var rv = new ForEachLoop(
                this._clrContext,
                this.LocFromJObject(jObject),
                localVariable,
                iterator,
                loopBlock);

            if (body != null)
            {
                rv.MoveVariablesFrom(body);
            }

            return rv;
        }

        private Node ParseSwitchStatement(JObject jObject)
        {
            List<KeyValuePair<List<LiteralExpression>, Statement>> caseBlocks =
                new List<KeyValuePair<List<LiteralExpression>, Statement>>();

            JArray sectionArray = jObject.Value<JArray>(NameTokens.Blocks);
            for (int iSection = 0; iSection < sectionArray.Count; iSection++)
            {
                JObject sectionObj = sectionArray.Value<JObject>(iSection);
                JArray labelJArray = sectionObj.Value<JArray>(NameTokens.Labels);

                List<LiteralExpression> labels =
                    new List<LiteralExpression>(labelJArray.Count);

                for (int iLabel = 0; iLabel < labelJArray.Count; iLabel++)
                {
                    JObject labelObj = labelJArray.Value<JObject>(iLabel);
                    if (labelObj.Value<string>(NameTokens.TypeName) == TypeTokens.Default)
                    {
                        labels.Add(null);
                    }
                    else
                    {
                        labels.Add((LiteralExpression)
                            this.ParseExpression(labelObj.Value<JObject>(NameTokens.Value)));
                    }
                }

                caseBlocks.Add(
                    new KeyValuePair<List<LiteralExpression>, Statement>(
                        labels,
                        this.ParseStatement(sectionObj.Value<JObject>(NameTokens.Block))));
            }

            return new SwitchStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)),
                caseBlocks);
        }

        private Node ParseScopeBlock(JObject jObject)
        {
            ScopeBlock rv = new ScopeBlock(
                this._clrContext,
                this.LocFromJObject(jObject));

            this.scopeBlockStack.AddFirst(
                Tuple.Create(
                    jObject.Value<int>(NameTokens.Id),
                    rv));

            foreach (var statement in this.ParseStatements(jObject.Value<JArray>(NameTokens.Value)))
            {
                rv.AddStatement(statement);
            }

            this.scopeBlockStack.RemoveFirst();

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
        }

        private Node ParseParameterBlock(JObject jObject)
        {
            ParameterBlock rv = new ParameterBlock(
                this._clrContext,
                this.LocFromJObject(jObject));

            JArray parameterArray = jObject.Value<JArray>(NameTokens.Parameters);

            for (int iParam = 0; iParam < parameterArray.Count; iParam++)
            {
                JObject paramObj = parameterArray.Value<JObject>(iParam);
                ParameterAttributes attr = (ParameterAttributes)
                    Enum.Parse(
                        typeof(ParameterAttributes),
                        paramObj.Value<string>(NameTokens.ModFlags),
                        true);

                var parameterType =
                        this.DeserializeType(
                            paramObj.Value<JObject>(NameTokens.Type));

                if ((attr & ParameterAttributes.Out) != 0)
                {
                    parameterType = new ByReferenceType(parameterType);
                }

                rv.AddParameter(
                    new ParameterDefinition(
                        paramObj.Value<string>(NameTokens.Name),
                        attr,
                        parameterType));
            }

            if (jObject.Value<bool>(NameTokens.IsMethodOwned)
                && this._currentMethod.HasThis)
            {
                rv.AddThisParameter(
                    new ParameterDefinition(
                        "this",
                        ParameterAttributes.None,
                        this._currentMethod.DeclaringType));
            }

            this.scopeBlockStack.AddFirst(
                Tuple.Create<int, ScopeBlock>(
                    jObject.Value<int>(NameTokens.Id),
                    rv));

            foreach (var statement in this.ParseStatements(jObject.Value<JArray>(NameTokens.Value)))
            {
                rv.AddStatement(statement);
            }

            this.scopeBlockStack.RemoveFirst();

            return rv;
        }

        private Node ParseTryFinally(JObject jObject)
        {
            Statement tryBlock = this.ParseStatement(jObject.Value<JObject>(NameTokens.TryBlock));
            TryCatchFinally tryCatchFinallyBlock = tryBlock as TryCatchFinally;
            var finallyScope = this.GetScopeBlock(jObject.Value<JObject>(NameTokens.FinallyBlock));

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
                    tryBlock.Location);
                tryScopeBlock.AddStatement(tryBlock);
            }

            return new TryCatchFinally(
                this._clrContext,
                this.LocFromJObject(jObject),
                tryScopeBlock,
                finallyBlock);
        }

        private Node ParseTryCatch(JObject jObject)
        {
            var tryBlock = this.GetScopeBlock(jObject.Value<JObject>(NameTokens.TryBlock));
            var handlerList = new List<HandlerBlock>();

            TryCatchFinally rv = null;
            JArray handlersArray = jObject.Value<JArray>(NameTokens.CatchBlocks);
            for (int iHandler = 0; iHandler < handlersArray.Count; iHandler++)
            {
                JObject handlerObj = handlersArray.Value<JObject>(iHandler);

                ScopeBlock handlerScopeBlock = this.GetScopeBlock(
                    handlerObj.Value<JObject>(NameTokens.Block));

                // Push the scope on scopeStack so that our variable gets correct
                // scopeBlock assigned.
                this.scopeBlockStack.AddFirst(
                    Tuple.Create(
                        handlerObj.Value<JObject>(NameTokens.Block).Value<int>(NameTokens.Id),
                        handlerScopeBlock));

                LocalVariable exceptionVariable =
                    this.ParseLocalVariable(
                        handlerObj.Value<JObject>(NameTokens.LocalVariable));
                this.scopeBlockStack.RemoveFirst();

                HandlerBlock handlerBlock = new HandlerBlock(
                    this._clrContext,
                    this.LocFromJObject(handlerObj),
                    this.DeserializeType(
                        handlerObj.Value<JObject>(NameTokens.Type)),
                    exceptionVariable != null
                        ? new VariableReference(
                                this._clrContext,
                                null,
                                exceptionVariable)
                        : null,
                    handlerScopeBlock);

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

        private Node ParseNullCoalascing(JObject jObject)
        {
            return new NullConditional(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.LeftExpr)),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.RightExpr)));
        }

        private Node ParseYield(JObject jObject)
        {
            return new YieldStatement(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)));
        }

        private Node ParseIterator(JObject jObject)
        {
            return new InlineIteratorExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                (ParameterBlock)this.GetScopeBlock(
                    jObject.Value<JObject>(NameTokens.Block)),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.Type)));
        }

        private Node ParseBoxExpr(JObject jObject)
        {
            return new BoxExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)));
        }

        private Node ParseUnBoxExpr(JObject jObject)
        {
            return new UnboxExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)),
                this.DeserializeType(jObject.Value<JObject>(NameTokens.Type)));
        }

        private Node ParseTypeCheck(JObject jObject)
        {
            throw new NotImplementedException();
        }

        private Node ParseIsExpr(JObject jObject)
        {
            return new TypeCheckExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.Type)),
                TypeCheckType.IsType);
        }

        private Node ParseAsExpr(JObject jObject)
        {
            return new TypeCheckExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.Type)),
                TypeCheckType.AsType);
        }

        private Node ParseConditional(JObject jObject)
        {
            return new ConditionalOperatorExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Condition)),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.TrueStatement)),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.FalseCondition)));
        }

        private Node ParseVariableAddressReference(JObject jObject)
        {
            return new LoadAddressExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                new VariableReference(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.ParseLocalVariable(jObject.Value<JObject>(NameTokens.Value))));
        }

        private Node ParseVariableReference(JObject jObject)
        {
            return new VariableReference(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseLocalVariable(jObject.Value<JObject>(NameTokens.Value)));
        }

        private Node ParseParameterReference(JObject jObject)
        {
            // TODO: figure out how to deal with Ref varaibles.
            var parameter = this.ParseArgumentVariable(jObject.Value<JObject>(NameTokens.Value));

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
                this.ParseArgumentVariable(jObject.Value<JObject>(NameTokens.Value)));
        }

        private Node ParseInvocation(JObject jObject)
        {
            return new MethodCallExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Method)),
                this.ParseArguments(jObject.Value<JArray>(NameTokens.Arguments)));
        }

        private Node ParseNew(JObject jObject)
        {
            JObject methodObj = jObject.Value<JObject>(NameTokens.Value);
            if (methodObj == null)
            {
                // This is struct and should be initialized using default value constructor.
                return this.ParseDefaultValue(
                    jObject);
            }

            return new NewObjectExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeMethod(methodObj),
                this.ParseArguments(jObject.Value<JArray>(NameTokens.Arguments)));
        }

        private Node ParseArrayCreation(JObject jObject)
        {
            JArray initializers = jObject.Value<JArray>(NameTokens.Initializer);
            JArray arguments = jObject.Value<JArray>(NameTokens.Arguments);
            Expression argExpression = null;

            if (arguments != null)
            {
                if (arguments.Count != 1)
                {
                    throw new InvalidOperationException();
                }

                argExpression = this.ParseExpression(arguments.Value<JObject>(0));
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
                    this.DeserializeType(
                        jObject.Value<JObject>(NameTokens.ElementType)),
                    argExpression);
            }

            IList<Expression> initializerExpressions = this.ParseExpressions(initializers);

            return new InlineArrayInitialization(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.ElementType)),
                initializerExpressions);
        }

        private Node ParseThisExpr(JObject jObject)
        {
            var thisVar = ((ParameterBlock)this.scopeBlockStack.Last.Value.Item2).GetThisParameter();
            var node = this.scopeBlockStack.Last.Previous;
            while (node != null)
            {
                ParameterBlock paramBlock = node.Value.Item2 as ParameterBlock;
                if (paramBlock != null)
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

        private Node ParseTypeOf(JObject jObject)
        {
            return new TypeofExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                new TypeReferenceExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.DeserializeType(
                        jObject.Value<JObject>(NameTokens.Type))));
        }

        private Node ParseArrayElementAccess(JObject jObject)
        {
            return new ArrayElementExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.LeftExpr)),
                this.ParseArguments(jObject.Value<JArray>(NameTokens.Arguments))[0]);
        }

        private Node ParseBaseExpr(JObject jObject)
        {
            var thisVariable = ((ParameterBlock)this.scopeBlockStack.Last.Value.Item2).GetThisParameter();
            var node = this.scopeBlockStack.Last.Previous;
            while (node != null)
            {
                ParameterBlock paramBlock = node.Value.Item2 as ParameterBlock;
                if (paramBlock != null)
                {
                    paramBlock.AddEscapingVariable(thisVariable);
                }

                node = node.Previous;
            }

            return new BaseVariableReference(
                this._clrContext,
                this.LocFromJObject(jObject),
                thisVariable);
        }

        private Node ParseNewInitializer(JObject jObject)
        {
            var initializerArray = jObject.Value<JArray>(NameTokens.Initializer);

            var args = this.ParseArguments(
                jObject.Value<JArray>(NameTokens.Arguments));

            List<Tuple<MemberReferenceExpression, Expression>> setters =
                new List<Tuple<MemberReferenceExpression, Expression>>();

            if (initializerArray != null)
            {
                for (int iInit = 0; iInit < initializerArray.Count; iInit++)
                {
                    JObject initObj = initializerArray.Value<JObject>(iInit);
                    MemberReferenceExpression memberReferenceExpression = null;

                    if (initObj.Value<string>(NameTokens.ElementType) == TypeTokens.PropertySpec)
                    {
                        PropertyReference propertyReference = new InternalPropertyReference(
                            null,
                            this.DeserializeMethod(initObj.Value<JObject>(NameTokens.Setter)));

                        memberReferenceExpression = new PropertyReferenceExpression(
                            this._clrContext,
                            this.LocFromJObject(initObj),
                            propertyReference,
                            null);
                    }
                    else
                    {
                        memberReferenceExpression = new FieldReferenceExpression(
                            this._clrContext,
                            this.LocFromJObject(initObj),
                            this.DeserializeField(
                                initObj.Value<JObject>(NameTokens.Field)),
                            null);
                    }

                    setters.Add(
                        Tuple.Create(
                            memberReferenceExpression,
                            this.ParseExpression(initObj.Value<JObject>(NameTokens.Value))));
                }
            }

            return new InlinePropertyInitilizationExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                new NewObjectExpression(
                    this._clrContext,
                    this.LocFromJObject(jObject),
                    this.DeserializeMethod(
                        jObject.Value<JObject>(NameTokens.Value)),
                    args),
                setters); ;
        }

        private Node ParseMethodExpr(JObject jObject)
        {
            var methodReference = this.DeserializeMethod(
                jObject.Value<JObject>(NameTokens.Value));

            var instance = this.ParseExpression(
                jObject.Value<JObject>(NameTokens.Instance));

            var location = this.LocFromJObject(jObject);

            if (instance == null)
            {
                return new MethodReferenceExpression(
                    this._clrContext,
                    location,
                    methodReference);
            }

            bool isVirtual = methodReference.Resolve().IsVirtual
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

        private Node ParseFieldExpr(JObject jObject)
        {
            FieldReference fieldReference = this.DeserializeField(
                jObject.Value<JObject>(NameTokens.Field));

            var instance = this.ParseExpression(
                jObject.Value<JObject>(NameTokens.Instance));

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

        private Node ParsePropertyExpr(JObject jObject)
        {
            return this.ParseIndexerExpr(jObject);
        }

        private Node ParseIndexerExpr(JObject jObject)
        {
            MethodReference getter = jObject[NameTokens.Getter].HasValues
                ? this.DeserializeMethod(
                        jObject.Value<JObject>(NameTokens.Getter))
                : null;

            MethodReference setter = jObject[NameTokens.Setter].HasValues
                ? this.DeserializeMethod(
                        jObject.Value<JObject>(NameTokens.Setter))
                : null;

            PropertyReference propertyReference = new InternalPropertyReference(
                getter,
                setter);

            var instance = this.ParseExpression(
                jObject.Value<JObject>(NameTokens.Instance));

            var location = this.LocFromJObject(jObject);

            var args = this.ParseArguments(jObject.Value<JArray>(NameTokens.Arguments));

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

        private Node ParseDelegateInvocation(JObject jObject)
        {
            return new MethodCallExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.ParseExpression(jObject.Value<JObject>(NameTokens.Expr)),
                this.ParseArguments(jObject.Value<JArray>(NameTokens.Arguments)));
        }

        private Node ParseDefaultValue(JObject jObject)
        {
            return new DefaultValueExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.Type)));
        }

        private Node ParseTempVariableAddressReference(JObject jObject)
        {
            return this.ParseVariableAddressReference(jObject);
        }

        private Node ParseTempVariableReference(JObject jObject)
        {
            return this.ParseVariableReference(jObject);
        }

        private Node ParseTypeExpr(JObject jObject)
        {
            return new TypeReferenceExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.Type)));
        }

        private Node ParseDelegateCreation(JObject jObject)
        {
            return new DelegateMethodExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                (MethodReferenceExpression)this.ParseExpression(
                    jObject.Value<JObject>(NameTokens.Method)),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.Type)));
        }

        private Node ParseAnonymousMethod(JObject jObject)
        {
            return new AnonymousMethodBodyExpression(
                this._clrContext,
                this.LocFromJObject(jObject),
                (ParameterBlock)this.ParseParameterBlock(
                    jObject.Value<JObject>(NameTokens.Block)),
                this.DeserializeType(
                    jObject.Value<JObject>(NameTokens.Type)));
        }

        private ParameterVariable ParseArgumentVariable(JObject jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            List<ParameterBlock> parameterBlocks = null;

            foreach (var node in this.scopeBlockStack)
            {
                ParameterBlock parameterBlock =
                    node.Item2 as ParameterBlock;

                if (parameterBlock != null)
                {
                    ParameterVariable rv;
                    if (parameterBlock.GetParameterVariable(
                            jObject.Value<string>(NameTokens.Name),
                            out rv))
                    {
                        if (parameterBlocks != null)
                        {
                            for (int iParamBlock = 0; iParamBlock < parameterBlocks.Count; iParamBlock++)
                            {
                                parameterBlocks[iParamBlock].AddEscapingVariable(rv);
                            }
                        }

                        return rv;
                    }
                    else
                    {
                        if (parameterBlocks == null)
                        {
                            parameterBlocks = new List<ParameterBlock>();
                        }

                        parameterBlocks.Add(parameterBlock);
                    }
                }
            }

            throw new InvalidOperationException();
        }

        private LocalVariable ParseLocalVariable(JObject jObject)
        {
            if (jObject == null)
            {
                return null;
            }

            var type = this.DeserializeType(jObject.Value<JObject>(NameTokens.Type));
            var name = jObject.Value<string>(NameTokens.Name);
            var blockId = jObject.Value<int>(NameTokens.Block);

            List<ParameterBlock> parameterBlocks = null;

            foreach (var node in this.scopeBlockStack)
            {
                if (node.Item1 == blockId)
                {
                    LocalVariable rv = node.Item2.ResolveVariable(
                        name);
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
                else if (node.Item2 is ParameterBlock)
                {
                    if (parameterBlocks == null)
                    {
                        parameterBlocks = new List<ParameterBlock>();
                    }

                    parameterBlocks.Add((ParameterBlock)node.Item2);
                }
            }

            throw new InvalidOperationException();
        }

        private Node ParseArgument(JObject jObject)
        {
            throw new NotImplementedException();
        }

        private Expression ParseExpression(JObject jObject)
        {
            return (Expression)this.ParseNode(jObject);
        }

        private Statement ParseStatement(JObject jObject)
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

        private ScopeBlock GetScopeBlock(JObject jObject)
        {
            Statement statement = this.ParseStatement(jObject);
            if (statement == null)
            {
                return null;
            }

            if (statement is ScopeBlock)
            {
                return (ScopeBlock)statement;
            }

            var rv = new ScopeBlock(
                this._clrContext,
                statement.Location);

            rv.AddStatement(statement);

            return rv;
        }

        private Statement[] ParseStatements(JArray jArray)
        {
            Statement[] rv = new Statement[jArray.Count];

            for (int iStatement = 0; iStatement < jArray.Count; iStatement++)
            {
                rv[iStatement] = this.ParseStatement(
                    jArray.Value<JObject>(iStatement));
            }

            return rv;
        }

        private Expression[] ParseExpressions(JArray expressions)
        {
            if (expressions == null)
            {
                return null;
            }

            Expression[] rv = new Expression[expressions.Count];

            for (int iArg = 0; iArg < expressions.Count; iArg++)
            {
                rv[iArg] = (Expression)this.ParseNode(expressions.Value<JObject>(iArg));
            }

            return rv;
        }

        private Expression[] ParseArguments(JArray arguments)
        {
            if (arguments == null)
            {
                return null;
            }

            Expression[] rv = new Expression[arguments.Count];
            for (int iArg = 0; iArg < arguments.Count; iArg++)
            {
                JObject argObject = arguments.Value<JObject>(iArg);
                JObject argValue = argObject.Value<string>(NameTokens.TypeName) == "argument"
                    ? argObject.Value<JObject>(NameTokens.Value)
                    : null;

                rv[iArg] = this.ParseExpression(argValue ?? argObject);

                if (argObject.Value<bool>(NameTokens.IsByRef))
                {
                    rv[iArg] = new LoadAddressExpression(
                        this._clrContext,
                        rv[iArg].Location,
                        rv[iArg]);
                }
            }

            return rv;
        }

        private Location LocFromJObject(JObject jObject)
        {
            return this.StrToLoc(jObject.Value<string>(NameTokens.Loc));
        }

        private Location StrToLoc(string str)
        {
            if (str == null)
            {
                return null;
            }

            string[] parts = str.Split(':');

            return new Location(
                this._currentMethodFileName,
                int.Parse(parts[0]),
                int.Parse(parts[1]));
        }

        private TypeReference DeserializeType(JObject jObject)
        {
            return this._memberReferenceDeserializer.DeserializeType(jObject);
        }

        private MethodReference DeserializeMethod(JObject jObject)
        {
            return this._memberReferenceDeserializer.DeserializeMethod(jObject);
        }

        private FieldReference DeserializeField(JObject jObject)
        {
            return this._memberReferenceDeserializer.DeserializeField(jObject);
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
                || !methodDef.IsVirtual)
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
                    instanceExpression);
            }
        }

        private Dictionary<string, Func<JObject, Node>> BuildParserMap()
        {
            Dictionary<string, Func<JObject, Node>> parserMap = new Dictionary<string, Func<JObject, Node>>();
            parserMap.Add("nullLiteral", this.ParseNullLiteral);
            parserMap.Add("boolLiteral", this.ParseBoolLiteral);
            parserMap.Add("charLiteral", this.ParseCharLiteral);
            parserMap.Add("longLiteral", this.ParseLongLiteral);
            parserMap.Add("intLiteral", this.ParseIntLiteral);
            parserMap.Add("uintLiteral", this.ParseUIntLiteral);
            parserMap.Add("strLiteral", this.ParseStringLiteral);
            parserMap.Add("decimalLiteral", this.ParseDecimalLiteral);
            parserMap.Add("doubleLiteral", this.ParseDoubleLiteral);
            parserMap.Add("floatLiteral", this.ParseFloatLiteral);
            parserMap.Add("assignment", this.ParseAssignment);
            parserMap.Add("methodCall", this.ParseMethodCall);
            parserMap.Add("binaryExpr", this.ParseBinaryExpr);
            parserMap.Add("unaryExpr", this.ParseUnaryExpr);
            parserMap.Add("typeCast", this.ParseTypeCast);
            parserMap.Add("byteLiteral", this.ParseByteLiteral);
            parserMap.Add("sbyteLiteral", this.ParseSByteLiteral);
            parserMap.Add("shortLiteral", this.ParseShortLiteral);
            parserMap.Add("ushortConstant", this.ParseUShortConstant);
            parserMap.Add("emptyStatement", this.ParseEmptyStatement);
            parserMap.Add("statementExpr", this.ParseStatementExpr);
            parserMap.Add("statementList", this.ParseStatementList);
            parserMap.Add("returnStatement", this.ParseReturnStatement);
            parserMap.Add("throwStatment", this.ParseThrowStatment);
            parserMap.Add("breakExpression", this.ParseBreakExpression);
            parserMap.Add("continueExpression", this.ParseContinueExpression);
            parserMap.Add("ifStatement", this.ParseIfStatement);
            parserMap.Add("doWhileStatement", this.ParseDoWhileStatement);
            parserMap.Add("whileStatement", this.ParseWhileStatement);
            parserMap.Add("forStatement", this.ParseForStatement);
            parserMap.Add("ulongLiteral", this.ParseULongLiteral);
            parserMap.Add("forEachStatement", this.ParseForEachStatement);
            parserMap.Add("switchStatement", this.ParseSwitchStatement);
            parserMap.Add("scopeBlock", this.ParseScopeBlock);
            parserMap.Add("parameterBlock", this.ParseParameterBlock);
            parserMap.Add("tryFinally", this.ParseTryFinally);
            parserMap.Add("tryCatch", this.ParseTryCatch);
            parserMap.Add("nullCoalascing", this.ParseNullCoalascing);
            parserMap.Add("yield", this.ParseYield);
            parserMap.Add("iterator", this.ParseIterator);
            parserMap.Add("typeCheck", this.ParseTypeCheck);
            parserMap.Add("isExpr", this.ParseIsExpr);
            parserMap.Add("asExpr", this.ParseAsExpr);
            parserMap.Add("conditional", this.ParseConditional);
            parserMap.Add("varaibleAddressReference", this.ParseVariableAddressReference);
            parserMap.Add("variableReference", this.ParseVariableReference);
            parserMap.Add("parameterReference", this.ParseParameterReference);
            parserMap.Add("invocation", this.ParseInvocation);
            parserMap.Add("new", this.ParseNew);
            parserMap.Add("arrayCreation", this.ParseArrayCreation);
            parserMap.Add("thisExpr", this.ParseThisExpr);
            parserMap.Add("typeOf", this.ParseTypeOf);
            parserMap.Add("arrayElementAccess", this.ParseArrayElementAccess);
            parserMap.Add("baseExpr", this.ParseBaseExpr);
            parserMap.Add("newInitializer", this.ParseNewInitializer);
            parserMap.Add("methodExpr", this.ParseMethodExpr);
            parserMap.Add("fieldExpr", this.ParseFieldExpr);
            parserMap.Add("propertyExpr", this.ParsePropertyExpr);
            parserMap.Add("indexerExpr", this.ParseIndexerExpr);
            parserMap.Add("delegateInvocation", this.ParseDelegateInvocation);
            parserMap.Add("defaultValue", this.ParseDefaultValue);
            parserMap.Add("tempVariableAddressReference", this.ParseTempVariableAddressReference);
            parserMap.Add("tempVariableReference", this.ParseTempVariableReference);
            parserMap.Add("typeExpr", this.ParseTypeExpr);
            parserMap.Add("delegateCreation", this.ParseDelegateCreation);
            parserMap.Add("argument", this.ParseArgument);
            parserMap.Add("boxExpr", this.ParseBoxExpr);
            parserMap.Add("unBoxExpr", this.ParseUnBoxExpr);
            parserMap.Add("varInitializer", this.ParseVariableInitializers);
            parserMap.Add("anonymousMethod", this.ParseAnonymousMethod);
            parserMap.Add(TypeTokens.WrapToNullable, this.ParseToNullable);
            parserMap.Add(TypeTokens.UnwrapFromNullable, this.ParseFromNullable);

            return parserMap;
        }
    }
}