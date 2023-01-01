namespace NScript.JST.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal interface ITransformerVisitor
    {
        Expression VisitArrayLiteralExpression(ArrayLiteralExpression expr)
            => new ArrayLiteralExpression(
                expr.Location,
                expr.Scope,
                expr.Elements
                    .Select(ex => DispatchExpression(ex))
                    .ToList());

        Expression VisitAwaitExpression(AwaitExpression expr)
            => new AwaitExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.Expression));

        Expression VisitBinaryExpression(BinaryExpression expr)
            => new BinaryExpression(
                expr.Location,
                expr.Scope,
                expr.Operator,
                DispatchExpression(expr.Left),
                DispatchExpression(expr.Right));

        Expression VisitBooleanLiteralExpression(BooleanLiteralExpression expr)
            => new BooleanLiteralExpression(
                expr.Scope,
                expr.Value);

        Expression VisitConditionalOperatorExpression(ConditionalOperatorExpression expr)
            => new ConditionalOperatorExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.Condition),
                DispatchExpression(expr.TrueExpression),
                DispatchExpression(expr.FalseExpression));

        Expression VisitDoubleLiteralExpression(DoubleLiteralExpression expr)
            => new DoubleLiteralExpression(
                expr.Scope,
                expr.Double);

        Expression VisitExpressionList(ExpressionsList expr)
            => new ExpressionsList(
                expr.Location,
                expr.Scope,
                expr.Expressions
                    .Select(ex => DispatchExpression(ex))
                    .ToArray());

        Expression VisitFunctionExpression(FunctionExpression expr)
        {
            var rv = new FunctionExpression(
                expr.Location,
                expr.Scope,
                expr.InnerScope,
                expr.Parameters,
                expr.Name,
                expr.IsAsync,
                expr.IsGenerator);

            rv.AddStatements(
                expr.Statements
                    .Select(statement => DispatchStatement(statement))
                    .ToList());

            return rv;
        }

        Expression VisitIdentifierExpression(IdentifierExpression expr)
            => new IdentifierExpression(
                expr.Identifier,
                expr.Scope,
                expr.Location);

        Expression VisitIdentifierStringExpression(IdentifierStringExpression expr)
        {
            var rv = new IdentifierStringExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.Expression[0]),
                expr.PrependString,
                expr.AppendString);

            foreach (var exp in expr.Expression.Skip(1))
            {
                switch (DispatchExpression(exp))
                {
                    case IdentifierExpression identifierExpression:
                        rv.Append(identifierExpression);
                        break;
                    case StringLiteralExpression stringLiteralExpression:
                        rv.Append(stringLiteralExpression);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return rv;
        }

        Expression VisitIndexExpression(IndexExpression expr)
            => new IndexExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.LeftExpression),
                DispatchExpression(expr.RightExpression));

        Expression VisitInlineNewArrayInitializationExpression(InlineNewArrayInitialization expr)
            => new InlineNewArrayInitialization(
                expr.Location,
                expr.Scope,
                expr.Values
                    .Select(exp => DispatchExpression(exp))
                    .ToList());

        Expression VisitInlineObjectInitializerExpression(InlineObjectInitializer expr)
        {
            var rv = new InlineObjectInitializer(
                expr.Location,
                expr.Scope);

            foreach (var item in expr.Initializers)
            {
                rv.AddInitializer(
                    (item.Item1 as IdentifierExpression).Identifier,
                    DispatchExpression(item.Item2));
            }

            return rv;
        }

        Expression VisitMethodCallExpression(MethodCallExpression expr)
            => new MethodCallExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.MethodExpression),
                expr.Arguments
                    .Select(exp => DispatchExpression(exp))
                    .ToArray());

        Expression VisitNewArrayExpression(NewArrayExpression expr)
            => new NewArrayExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.Size));

        Expression VisitNewObjectExpression(NewObjectExpression expr)
            => new NewObjectExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.ObjectAccessExpression),
                expr.Arguments
                    .Select(exp => DispatchExpression(exp))
                    .ToArray());

        Expression VisitNullLiteralExpression(NullLiteralExpression expr)
            => new NullLiteralExpression(expr.Scope);

        Expression VisitNumberLiteralExpression(NumberLiteralExpression expr)
            => new NumberLiteralExpression(expr.Scope, expr.Number);

        Expression VisitScriptLiteralExpression(ScriptLiteralExpression expr)
            => new ScriptLiteralExpression(
                expr.Location,
                expr.Scope,
                expr.Literal,
                expr.LiteralArguments
                    .Select(exp => DispatchExpression(exp))
                    .ToArray());

        Expression VisitStringLiteralExpression(StringLiteralExpression expr)
            => new StringLiteralExpression(
                expr.Scope,
                expr.StringLiteral);

        Expression VisitThisExpression(ThisExpression expr)
            => new ThisExpression(
                expr.Location,
                expr.Scope);

        Expression VisitThrowExpression(ThrowExpression expr)
            => new ThrowExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.Expression));

        Expression VisitTupleExpression(TupleExpression expr)
            => new TupleExpression(
                expr.Location,
                expr.Scope,
                expr.ParameterNames.ToList(),
                expr.Arguments
                    .Select(exp => DispatchExpression(exp))
                    .ToArray());

        Expression VisitUnaryExpression(UnaryExpression expr)
            => new UnaryExpression(
                expr.Location,
                expr.Scope,
                expr.Operator,
                DispatchExpression(expr.NestedExpression));

        Expression VisitYieldExpression(YieldExpression expr)
            => new YieldExpression(
                expr.Location,
                expr.Scope,
                DispatchExpression(expr.YieldValueOpt));

        Expression DispatchExpression(Expression expr)
        {
            switch(expr)
            {
                case ArrayLiteralExpression arrayLiteralExpression:
                    return VisitArrayLiteralExpression(arrayLiteralExpression);
                case AwaitExpression awaitExpression:
                    return VisitAwaitExpression(awaitExpression);
                case BinaryExpression binaryExpression:
                    return VisitBinaryExpression(binaryExpression);
                case BooleanLiteralExpression booleanLiteralExpression:
                    return VisitBooleanLiteralExpression(booleanLiteralExpression);
                case ConditionalOperatorExpression conditionalOperatorExpression:
                    return VisitConditionalOperatorExpression(conditionalOperatorExpression);
                case DoubleLiteralExpression doubleLiteralExpression:
                    return VisitDoubleLiteralExpression(doubleLiteralExpression);
                case ExpressionsList expressionsList:
                    return VisitExpressionList(expressionsList);
                case FunctionExpression functionExpression:
                    return VisitFunctionExpression(functionExpression);
                case IdentifierExpression identifierExpression:
                    return VisitIdentifierExpression(identifierExpression);
                case IdentifierStringExpression identifierStringExpression:
                    return VisitIdentifierStringExpression(identifierStringExpression);
                case IndexExpression indexExpression:
                    return VisitIndexExpression(indexExpression);
                case InlineNewArrayInitialization inlineNewArrayInitialization:
                    return VisitInlineNewArrayInitializationExpression(inlineNewArrayInitialization);
                case InlineObjectInitializer inlineObjectInitializer:
                    return VisitInlineObjectInitializerExpression(inlineObjectInitializer);
                case MethodCallExpression methodCallExpression:
                    return VisitMethodCallExpression(methodCallExpression);
                case NewArrayExpression newArrayExpression:
                    return VisitNewArrayExpression(newArrayExpression);
                case NewObjectExpression newObjectExpression:
                    return VisitNewObjectExpression(newObjectExpression);
                case NullLiteralExpression nullLiteralExpression:
                    return VisitNullLiteralExpression(nullLiteralExpression);
                case NumberLiteralExpression numberLiteralExpression:
                    return VisitNumberLiteralExpression(numberLiteralExpression);
                case ScriptLiteralExpression scriptLiteralExpression:
                    return VisitScriptLiteralExpression(scriptLiteralExpression);
                case ThisExpression thisExpression:
                    return VisitThisExpression(thisExpression);
                case ThrowExpression throwExpression:
                    return VisitThrowExpression(throwExpression);
                case TupleExpression tupleExpression:
                    return VisitTupleExpression(tupleExpression);
                case UnaryExpression unaryExpression:
                    return VisitUnaryExpression(unaryExpression);
                case YieldExpression yieldExpression:
                    return VisitYieldExpression(yieldExpression);
                default: throw new NotImplementedException();
            }
        }

        Statement VisitBreakStatement(BreakStatement breakStatement)
            => new BreakStatement(
                breakStatement.Location,
                breakStatement.Scope);

        Statement VisitContinueStatement(ContinueStatement continueStatement)
            => new ContinueStatement(
                continueStatement.Location,
                continueStatement.Scope);

        Statement VisitCatchHandler(CatchHandler catchHandler)
            => new CatchHandler(
                catchHandler.Scope,
                (IdentifierExpression)DispatchExpression(catchHandler.Identifier),
                (ScopeBlock)DispatchStatement(catchHandler.CatchBlock));

        Statement VisitDoWhileLoop(DoWhileLoop doWhileLoop)
            => new DoWhileLoop(
                doWhileLoop.Location,
                doWhileLoop.Scope,
                DispatchExpression(doWhileLoop.Condition),
                DispatchStatement(doWhileLoop.Loop));

        Statement VisitExpressionStatement(ExpressionStatement expressionStatement)
            => new ExpressionStatement(
                expressionStatement.Location,
                expressionStatement.Scope,
                DispatchExpression(expressionStatement.Expression));

        Statement VisitForInLoop(ForInLoop forLoop)
            => new ForInLoop(
                forLoop.Location,
                forLoop.Scope,
                DispatchExpression(forLoop.Collection),
                (IdentifierExpression)DispatchExpression(forLoop.Key),
                DispatchStatement(forLoop.Loop));

        Statement VisitForLoop(ForLoop forLoop)
            => new ForLoop(
                forLoop.Location,
                forLoop.Scope,
                DispatchExpression(forLoop.Condition),
                DispatchStatement(forLoop.InitializationBlock),
                DispatchStatement(forLoop.IncrementBlock),
                DispatchStatement(forLoop.Loop));

        Statement VisitIfBlockStatement(IfBlockStatement ifBlockStatement)
            => new IfBlockStatement(
                ifBlockStatement.Location,
                ifBlockStatement.Scope,
                DispatchExpression(ifBlockStatement.Condition),
                DispatchStatement(ifBlockStatement.TrueBlock),
                ifBlockStatement.FalseBlock != null
                    ? DispatchStatement(ifBlockStatement.FalseBlock)
                    : null);

        Statement VisitInitializerStatement(InitializerStatement initializerStatement)
            => new InitializerStatement(
                initializerStatement.Location,
                initializerStatement.Scope,
                initializerStatement.Initializers
                    .Select(exp => DispatchExpression(exp))
                    .ToList());

        Statement VisitReturnStatement(ReturnStatement returnStatement)
            => new ReturnStatement(
                returnStatement.Location,
                returnStatement.Scope,
                returnStatement.ReturnExpression != null
                    ? DispatchExpression(returnStatement.ReturnExpression)
                    : null);

        Statement VisitScopeBlock(ScopeBlock scopeBlock)
            => new ScopeBlock(
                scopeBlock.Location,
                scopeBlock.Scope,
                scopeBlock.Statements
                    .Select(st => DispatchStatement(st))
                    .ToList());

        Statement VisitSwitchBlockStatement(SwitchBlockStatement switchBlockStatement)
            => new SwitchBlockStatement(
                switchBlockStatement.Location,
                switchBlockStatement.Scope,
                DispatchExpression(switchBlockStatement.Key),
                switchBlockStatement.CaseBlocks
                    .Select(caseBlock =>
                        KeyValuePair.Create(
                            caseBlock.Key
                                .Select(exp => DispatchExpression(exp))
                                .ToList(),
                            DispatchStatement(caseBlock.Value)))
                    .ToList());

        Statement VisitTryCatchFinallyBlock(TryCatchFinalyBlock tryCatchFinalyBlock)
            => new TryCatchFinalyBlock(
                tryCatchFinalyBlock.Scope,
                DispatchStatement(tryCatchFinalyBlock.Try) switch
                {
                    ScopeBlock sb => sb,
                    Statement stmt => new ScopeBlock(stmt.Location, stmt.Scope, new {stmt})
                },
                tryCatchFinalyBlock.Catch != null
                    ? (CatchHandler)VisitCatchHandler(tryCatchFinalyBlock.Catch)
                    : null,
                tryCatchFinalyBlock.Finally != null
                    ? DispatchStatement(tryCatchFinalyBlock.Finally)
                    : null);

        Statement VisitVarInitializerStatement(VarInitializerStatement varInitializerStatement)
            => new VarInitializerStatement(
                varInitializerStatement.Location,
                varInitializerStatement.Scope,
                varInitializerStatement.Initializers
                    .Select(stmt => DispatchStatement(stmt))
                    .ToList());

        Statement VisitWhileLoop(WhileLoop whileLoop)
            => new WhileLoop(
                whileLoop.Location,
                whileLoop.Scope,
                DispatchExpression(whileLoop.Condition),
                DispatchStatement(whileLoop.Loop));

        Statement DispatchStatement(Statement statement)
        {
            switch(statement)
            {
                case BreakStatement breakStatement:
                    return VisitBreakStatement(breakStatement);
                case CatchHandler catchHandler:
                    return VisitCatchHandler(catchHandler);
                case ContinueStatement continueStatement:
                    return VisitContinueStatement(continueStatement);
                case DoWhileLoop doWhileLoop:
                    return VisitDoWhileLoop(doWhileLoop);
                case ExpressionStatement expressionStatement:
                    return VisitExpressionStatement(expressionStatement);
                case ForInLoop forInLoop:
                    return VisitForInLoop(forInLoop);
                case ForLoop forLoop:
                    return VisitForLoop(forLoop);
                case IfBlockStatement ifBlockStatement:
                    return VisitIfBlockStatement(ifBlockStatement);
                case VarInitializerStatement varInitializerStatement:
                    return VisitVarInitializerStatement(varInitializerStatement);
                case InitializerStatement initializerStatement:
                    return VisitInitializerStatement(initializerStatement);
                case ReturnStatement returnStatement:
                    return VisitReturnStatement(returnStatement);
                case ScopeBlock scopeBlock:
                    return VisitScopeBlock(scopeBlock);
                case SwitchBlockStatement switchBlockStatement:
                    return VisitSwitchBlockStatement(switchBlockStatement);
                case TryCatchFinalyBlock tryCatchFinalyBlock:
                    return VisitTryCatchFinallyBlock(tryCatchFinalyBlock);
                case WhileLoop whileLoop:
                    return VisitWhileLoop(whileLoop);
                default: throw new NotImplementedException();
            }
        }
    }
}
