namespace NScript.JST.Visitors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TransformerVisitorExtension
    {
        public static Expression VisitArrayLiteralExpressionExt(this ITransformerVisitor self, ArrayLiteralExpression expr)
            => new ArrayLiteralExpression(
                expr.Location,
                expr.Scope,
                expr.Elements
                       .Select(self.DispatchExpression)
                    .ToList());

        public static Expression VisitAwaitExpressionExt(this ITransformerVisitor self, AwaitExpression expr)
            => new AwaitExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.Expression));

        public static Expression VisitBinaryExpressionExt(this ITransformerVisitor self, BinaryExpression expr)
            => new BinaryExpression(
                expr.Location,
                expr.Scope,
                expr.Operator,
                self.DispatchExpression(expr.Left),
                self.DispatchExpression(expr.Right));

        public static Expression VisitBooleanLiteralExpressionExt(this ITransformerVisitor self, BooleanLiteralExpression expr)
            => new BooleanLiteralExpression(
                expr.Scope,
                expr.Value);

        public static Expression VisitConditionalOperatorExpressionExt(this ITransformerVisitor self, ConditionalOperatorExpression expr)
            => new ConditionalOperatorExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.Condition),
                self.DispatchExpression(expr.TrueExpression),
                self.DispatchExpression(expr.FalseExpression));

        public static Expression VisitDoubleLiteralExpressionExt(this ITransformerVisitor self, DoubleLiteralExpression expr)
            => new DoubleLiteralExpression(
                expr.Scope,
                expr.Double);

        public static Expression VisitExpressionListExt(this ITransformerVisitor self, ExpressionsList expr)
            => new ExpressionsList(
                expr.Location,
                expr.Scope,
                expr.Expressions
                    .Select(self.DispatchExpression)
                    .ToArray());

        public static Expression VisitFunctionExpressionExt(this ITransformerVisitor self, FunctionExpression expr)
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
                    .Select(self.DispatchStatement)
                    .ToList());

            return rv;
        }

        public static Expression VisitIdentifierExpressionExt(this ITransformerVisitor self, IdentifierExpression expr)
            => new IdentifierExpression(
                expr.Identifier,
                expr.Scope,
                expr.Location);

        public static Expression VisitIdentifierStringExpressionExt(this ITransformerVisitor self, IdentifierStringExpression expr)
        {
            var rv = new IdentifierStringExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.Expression[0]),
                expr.PrependString,
                expr.AppendString);

            foreach (var exp in expr.Expression.Skip(1))
            {
                switch (self.DispatchExpression(exp))
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

        public static Expression VisitIndexExpressionExt(this ITransformerVisitor self, IndexExpression expr)
            => new IndexExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.LeftExpression),
                self.DispatchExpression(expr.RightExpression));

        public static Expression VisitInlineNewArrayInitializationExpressionExt(this ITransformerVisitor self, InlineNewArrayInitialization expr)
            => new InlineNewArrayInitialization(
                expr.Location,
                expr.Scope,
                expr.Values
                    .Select(self.DispatchExpression)
                    .ToList());

        public static Expression VisitInlineObjectInitializerExpressionExt(this ITransformerVisitor self, InlineObjectInitializer expr)
        {
            var rv = new InlineObjectInitializer(
                expr.Location,
                expr.Scope);

            foreach (var item in expr.Initializers)
            {
                switch (item.Item1) {
                    case IdentifierExpression identExp:
                        rv.AddInitializer(
                            identExp.Identifier,
                            self.DispatchExpression(item.Item2));
                        break;
                    case StringLiteralExpression strExp:
                        rv.AddInitializer(
                            strExp.LiteralString,
                            self.DispatchExpression(item.Item2));
                        break;
                }
            }

            return rv;
        }

        public static Expression VisitMethodCallExpressionExt(this ITransformerVisitor self, MethodCallExpression expr)
            => new MethodCallExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.MethodExpression),
                expr.Arguments
                    .Select(self.DispatchExpression)
                    .ToArray());

        public static Expression VisitNewArrayExpressionExt(this ITransformerVisitor self, NewArrayExpression expr)
            => new NewArrayExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.Size));

        public static Expression VisitNewObjectExpressionExt(this ITransformerVisitor self, NewObjectExpression expr)
            => new NewObjectExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.ObjectAccessExpression),
                expr.Arguments
                    .Select(self.DispatchExpression)
                    .ToArray());

        public static Expression VisitNullLiteralExpressionExt(this ITransformerVisitor self, NullLiteralExpression expr)
            => new NullLiteralExpression(expr.Scope);

        public static Expression VisitNumberLiteralExpressionExt(this ITransformerVisitor self, NumberLiteralExpression expr)
            => new NumberLiteralExpression(expr.Scope, expr.Number);

        public static Expression VisitScriptLiteralExpressionExt(this ITransformerVisitor self, ScriptLiteralExpression expr)
            => new ScriptLiteralExpression(
                expr.Location,
                expr.Scope,
                expr.Literal,
                expr.LiteralArguments
                    .Select(self.DispatchExpression)
                    .ToArray());

        public static Expression VisitStringLiteralExpressionExt(this ITransformerVisitor self, StringLiteralExpression expr)
            => new StringLiteralExpression(
                expr.Scope,
                expr.StringLiteral);

        public static Expression VisitThisExpressionExt(this ITransformerVisitor self, ThisExpression expr)
            => new ThisExpression(
                expr.Location,
                expr.Scope);

        public static Expression VisitThrowExpressionExt(this ITransformerVisitor self, ThrowExpression expr)
            => new ThrowExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.Expression));

        public static Expression VisitTupleExpressionExt(this ITransformerVisitor self, TupleExpression expr)
            => new TupleExpression(
                expr.Location,
                expr.Scope,
                expr.ParameterNames.ToList(),
                expr.Arguments
                    .Select(self.DispatchExpression)
                    .ToArray());

        public static Expression VisitUnaryExpressionExt(this ITransformerVisitor self, UnaryExpression expr)
            => new UnaryExpression(
                expr.Location,
                expr.Scope,
                expr.Operator,
                self.DispatchExpression(expr.NestedExpression));

        public static Expression VisitYieldExpressionExt(this ITransformerVisitor self, YieldExpression expr)
            => new YieldExpression(
                expr.Location,
                expr.Scope,
                self.DispatchExpression(expr.YieldValueOpt));

        public static Expression DispatchExpressionExt(this ITransformerVisitor self, Expression expr)
        {
            switch(expr)
            {
                case ArrayLiteralExpression arrayLiteralExpression:
                    return self.VisitArrayLiteralExpression(arrayLiteralExpression);
                case AwaitExpression awaitExpression:
                    return self.VisitAwaitExpression(awaitExpression);
                case BinaryExpression binaryExpression:
                    return self.VisitBinaryExpression(binaryExpression);
                case BooleanLiteralExpression booleanLiteralExpression:
                    return self.VisitBooleanLiteralExpression(booleanLiteralExpression);
                case ConditionalOperatorExpression conditionalOperatorExpression:
                    return self.VisitConditionalOperatorExpression(conditionalOperatorExpression);
                case DoubleLiteralExpression doubleLiteralExpression:
                    return self.VisitDoubleLiteralExpression(doubleLiteralExpression);
                case ExpressionsList expressionsList:
                    return self.VisitExpressionList(expressionsList);
                case FunctionExpression functionExpression:
                    return self.VisitFunctionExpression(functionExpression);
                case IdentifierExpression identifierExpression:
                    return self.VisitIdentifierExpression(identifierExpression);
                case IdentifierStringExpression identifierStringExpression:
                    return self.VisitIdentifierStringExpression(identifierStringExpression);
                case IndexExpression indexExpression:
                    return self.VisitIndexExpression(indexExpression);
                case InlineNewArrayInitialization inlineNewArrayInitialization:
                    return self.VisitInlineNewArrayInitializationExpression(inlineNewArrayInitialization);
                case InlineObjectInitializer inlineObjectInitializer:
                    return self.VisitInlineObjectInitializerExpression(inlineObjectInitializer);
                case MethodCallExpression methodCallExpression:
                    return self.VisitMethodCallExpression(methodCallExpression);
                case NewArrayExpression newArrayExpression:
                    return self.VisitNewArrayExpression(newArrayExpression);
                case NewObjectExpression newObjectExpression:
                    return self.VisitNewObjectExpression(newObjectExpression);
                case NullLiteralExpression nullLiteralExpression:
                    return self.VisitNullLiteralExpression(nullLiteralExpression);
                case NumberLiteralExpression numberLiteralExpression:
                    return self.VisitNumberLiteralExpression(numberLiteralExpression);
                case ScriptLiteralExpression scriptLiteralExpression:
                    return self.VisitScriptLiteralExpression(scriptLiteralExpression);
                case StringLiteralExpression stringLiteralExpression:
                    return self.VisitStringLiteralExpression(stringLiteralExpression);
                case ThisExpression thisExpression:
                    return self.VisitThisExpression(thisExpression);
                case ThrowExpression throwExpression:
                    return self.VisitThrowExpression(throwExpression);
                case TupleExpression tupleExpression:
                    return self.VisitTupleExpression(tupleExpression);
                case UnaryExpression unaryExpression:
                    return self.VisitUnaryExpression(unaryExpression);
                case YieldExpression yieldExpression:
                    return self.VisitYieldExpression(yieldExpression);
                case null:
                    return null;
                default: throw new NotImplementedException();
            }
        }

        public static Statement VisitBreakStatementExt(this ITransformerVisitor self, BreakStatement breakStatement)
            => new BreakStatement(
                breakStatement.Location,
                breakStatement.Scope);

        public static Statement VisitContinueStatementExt(this ITransformerVisitor self, ContinueStatement continueStatement)
            => new ContinueStatement(
                continueStatement.Location,
                continueStatement.Scope);

        public static Statement VisitCatchHandlerExt(this ITransformerVisitor self, CatchHandler catchHandler)
            => new CatchHandler(
                catchHandler.Scope,
                (IdentifierExpression)self.DispatchExpression(catchHandler.Identifier),
                (ScopeBlock)self.DispatchStatement(catchHandler.CatchBlock));

        public static Statement VisitDoWhileLoopExt(this ITransformerVisitor self, DoWhileLoop doWhileLoop)
            => new DoWhileLoop(
                doWhileLoop.Location,
                doWhileLoop.Scope,
                self.DispatchExpression(doWhileLoop.Condition),
                self.DispatchStatement(doWhileLoop.Loop));

        public static Statement VisitEmptyStatementExt(this ITransformerVisitor self, EmptyStatement statement)
            => new EmptyStatement(statement.Location, statement.Scope);

        public static Statement VisitExpressionStatementExt(this ITransformerVisitor self, ExpressionStatement expressionStatement)
            => new ExpressionStatement(
                expressionStatement.Location,
                expressionStatement.Scope,
                self.DispatchExpression(expressionStatement.Expression));

        public static Statement VisitForInLoopExt(this ITransformerVisitor self, ForInLoop forLoop)
            => new ForInLoop(
                forLoop.Location,
                forLoop.Scope,
                self.DispatchExpression(forLoop.Collection),
                (IdentifierExpression)self.DispatchExpression(forLoop.Key),
                self.DispatchStatement(forLoop.Loop));

        public static Statement VisitForLoopExt(this ITransformerVisitor self, ForLoop forLoop)
            => new ForLoop(
                forLoop.Location,
                forLoop.Scope,
                self.DispatchExpression(forLoop.Condition),
                self.DispatchStatement(forLoop.InitializationBlock),
                self.DispatchStatement(forLoop.IncrementBlock),
                self.DispatchStatement(forLoop.Loop));

        public static Statement VisitIfBlockStatementExt(this ITransformerVisitor self, IfBlockStatement ifBlockStatement)
            => new IfBlockStatement(
                ifBlockStatement.Location,
                ifBlockStatement.Scope,
                self.DispatchExpression(ifBlockStatement.Condition),
                (ScopeBlock)self.DispatchStatement(ifBlockStatement.TrueBlock),
                ifBlockStatement.FalseBlock != null
                    ? (ScopeBlock)self.DispatchStatement(ifBlockStatement.FalseBlock)
                    : null);

        public static Statement VisitInitializerStatementExt(this ITransformerVisitor self, InitializerStatement initializerStatement)
            => new InitializerStatement(
                initializerStatement.Location,
                initializerStatement.Scope,
                initializerStatement.Initializers
                    .Select(self.DispatchExpression)
                    .ToList());

        public static Statement VisitReturnStatementExt(this ITransformerVisitor self, ReturnStatement returnStatement)
            => new ReturnStatement(
                returnStatement.Location,
                returnStatement.Scope,
                returnStatement.ReturnExpression != null
                    ? self.DispatchExpression(returnStatement.ReturnExpression)
                    : null);

        public static Statement VisitScopeBlockExt(this ITransformerVisitor self, ScopeBlock scopeBlock)
            => new ScopeBlock(
                scopeBlock.Location,
                scopeBlock.Scope,
                scopeBlock.Statements
                    .Select(self.DispatchStatement)
                    .Where(item => item != null && item is not EmptyStatement)
                    .ToList());

        public static Statement VisitSwitchBlockStatementExt(this ITransformerVisitor self, SwitchBlockStatement switchBlockStatement)
            => new SwitchBlockStatement(
                switchBlockStatement.Location,
                switchBlockStatement.Scope,
                self.DispatchExpression(switchBlockStatement.Key),
                switchBlockStatement.CaseBlocks
                    .Select(caseBlock =>
                        KeyValuePair.Create(
                            caseBlock.Key
                                .Select(self.DispatchExpression)
                                .Where(item => item != null)
                                .ToList(),
                            self.DispatchStatement(caseBlock.Value)))
                    .ToList());

        public static Statement VisitTryCatchFinallyBlockExt(this ITransformerVisitor self, TryCatchFinalyBlock tryCatchFinalyBlock)
            => new TryCatchFinalyBlock(
                tryCatchFinalyBlock.Scope,
                self.DispatchStatement(tryCatchFinalyBlock.Try) switch
                {
                    ScopeBlock sb => sb,
                    Statement stmt => new ScopeBlock(stmt.Location, stmt.Scope, new(){stmt})
                },
                tryCatchFinalyBlock.Catch != null
                    ? (CatchHandler)self.VisitCatchHandler(tryCatchFinalyBlock.Catch)
                    : null,
                tryCatchFinalyBlock.Finally != null
                    ? (ScopeBlock)self.DispatchStatement(tryCatchFinalyBlock.Finally)
                    : null);

        public static Statement VisitVarInitializerStatementExt(this ITransformerVisitor self, VarInitializerStatement varInitializerStatement)
            => new VarInitializerStatement(
                varInitializerStatement.Location,
                varInitializerStatement.Scope,
                varInitializerStatement.Initializers
                    .Select(self.DispatchExpression)
                    .Where(i => i != null)
                    .ToList());

        public static Statement VisitWhileLoopExt(this ITransformerVisitor self, WhileLoop whileLoop)
            => new WhileLoop(
                whileLoop.Location,
                whileLoop.Scope,
                self.DispatchExpression(whileLoop.Condition),
                self.DispatchStatement(whileLoop.Loop));

        public static Statement DispatchStatementExt(this ITransformerVisitor self, Statement statement)
        {
            switch(statement)
            {
                case BreakStatement breakStatement:
                    return self.VisitBreakStatement(breakStatement);
                case CatchHandler catchHandler:
                    return self.VisitCatchHandler(catchHandler);
                case ContinueStatement continueStatement:
                    return self.VisitContinueStatement(continueStatement);
                case DoWhileLoop doWhileLoop:
                    return self.VisitDoWhileLoop(doWhileLoop);
                case ExpressionStatement expressionStatement:
                    return self.VisitExpressionStatement(expressionStatement);
                case ForInLoop forInLoop:
                    return self.VisitForInLoop(forInLoop);
                case ForLoop forLoop:
                    return self.VisitForLoop(forLoop);
                case IfBlockStatement ifBlockStatement:
                    return self.VisitIfBlockStatement(ifBlockStatement);
                case VarInitializerStatement varInitializerStatement:
                    return self.VisitVarInitializerStatement(varInitializerStatement);
                case InitializerStatement initializerStatement:
                    return self.VisitInitializerStatement(initializerStatement);
                case ReturnStatement returnStatement:
                    return self.VisitReturnStatement(returnStatement);
                case ScopeBlock scopeBlock:
                    return self.VisitScopeBlock(scopeBlock);
                case SwitchBlockStatement switchBlockStatement:
                    return self.VisitSwitchBlockStatement(switchBlockStatement);
                case TryCatchFinalyBlock tryCatchFinalyBlock:
                    return self.VisitTryCatchFinallyBlock(tryCatchFinalyBlock);
                case WhileLoop whileLoop:
                    return self.VisitWhileLoop(whileLoop);
                case EmptyStatement blankStatement:
                    return self.VisitEmptyStatement(blankStatement);
                case null:
                    return null;
                default: throw new NotImplementedException();
            }
        }
    }
}
