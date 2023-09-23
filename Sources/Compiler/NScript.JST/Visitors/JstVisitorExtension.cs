namespace NScript.JST.Visitors
{
    using System;

    internal static class JstVisitorExtension
    {
        public static void VisitArrayLiteralExpressionExt(this IJstVisitor self, ArrayLiteralExpression expr)
        {
            foreach (var item in expr.Elements)
            {
                self.DispatchExpression(item);
            }
        }

        public static void VisitAwaitExpressionExt(this IJstVisitor self, AwaitExpression expr)
        {
            self.DispatchExpression(expr.Expression);
        }

        public static void VisitBinaryExpressionExt(this IJstVisitor self, BinaryExpression expr)
        {
            self.DispatchExpression(expr.Left);
            self.DispatchExpression(expr.Right);
        }

        public static void VisitBooleanLiteralExpressionExt(this IJstVisitor self, BooleanLiteralExpression expr)
        {
        }

        public static void VisitConditionalOperatorExpressionExt(this IJstVisitor self, ConditionalOperatorExpression expr)
        {
            self.DispatchExpression(expr.Condition);
            self.DispatchExpression(expr.TrueExpression);
            self.DispatchExpression(expr.FalseExpression);
        }

        public static void VisitDoubleLiteralExpressionExt(this IJstVisitor self, DoubleLiteralExpression expr)
        {
        }

        public static void VisitExpressionListExt(this IJstVisitor self, ExpressionsList expr)
        {
            foreach (var item in expr.Expressions)
            {
                self.DispatchExpression(item);
            }
        }

        public static void VisitFunctionExpressionExt(this IJstVisitor self, FunctionExpression expr)
        {
            foreach (var item in expr.Statements)
            {
                self.DispatchStatement(item);
            }
        }

        public static void VisitIdentifierExpressionExt(this IJstVisitor self, IdentifierExpression expr)
        {
        }

        public static void VisitIdentifierStringExpressionExt(this IJstVisitor self, IdentifierStringExpression expr)
        {
            foreach (var item in expr.Expression)
            {
                self.DispatchExpression(item);
            }
        }

        public static void VisitIndexExpressionExt(this IJstVisitor self, IndexExpression expr)
        {
            self.DispatchExpression(expr.LeftExpression);
            self.DispatchExpression(expr.RightExpression);
        }

        public static void VisitInlineNewArrayInitializationExpressionExt(this IJstVisitor self, InlineNewArrayInitialization expr)
        {
            foreach (var item in expr.Values)
            {
                self.DispatchExpression(item);
            }
        }

        public static void VisitInlineObjectInitializerExpressionExt(this IJstVisitor self, InlineObjectInitializer expr)
        {
            foreach (var item in expr.Initializers)
            {
                self.DispatchExpression(item.Item1);
                self.DispatchExpression(item.Item2);
            }
        }

        public static void VisitMethodCallExpressionExt(this IJstVisitor self, MethodCallExpression expr)
        {
            self.DispatchExpression(expr.MethodExpression);
            foreach (var arg in expr.Arguments)
            {
                self.DispatchExpression(arg);
            }
        }

        public static void VisitNewArrayExpressionExt(this IJstVisitor self, NewArrayExpression expr)
        {
            self.DispatchExpression(expr.Size);
        }

        public static void VisitNewObjectExpressionExt(this IJstVisitor self, NewObjectExpression expr)
        {
            self.DispatchExpression(expr.ObjectAccessExpression);
            foreach (var arg in expr.Arguments)
            {
                self.DispatchExpression(arg);
            }
        }

        public static void VisitNullLiteralExpressionExt(this IJstVisitor self, NullLiteralExpression expr)
        {
        }

        public static void VisitNumberLiteralExpressionExt(this IJstVisitor self, NumberLiteralExpression expr)
        {
        }

        public static void VisitScriptLiteralExpressionExt(this IJstVisitor self, ScriptLiteralExpression expr)
        {
            foreach (var arg in expr.LiteralArguments)
            {
                self.DispatchExpression(arg);
            }
        }

        public static void VisitStringLiteralExpressionExt(this IJstVisitor self, StringLiteralExpression expr)
        {
        }

        public static void VisitThisExpressionExt(this IJstVisitor self, ThisExpression expr)
        {
        }

        public static void VisitThrowExpressionExt(this IJstVisitor self, ThrowStatement throwExpression)
        {
            self.DispatchExpression(throwExpression.Expression);
        }

        public static void VisitTupleExpressionExt(this IJstVisitor self, TupleExpression expr)
        {
            foreach (var arg in expr.Arguments)
            {
                self.DispatchExpression(arg);
            }
        }

        public static void VisitUnaryExpressionExt(this IJstVisitor self, UnaryExpression expr)
        {
            self.DispatchExpression(expr.NestedExpression);
        }

        public static void VisitYieldExpressionExt(this IJstVisitor self, YieldExpression expr)
        {
            if (expr.YieldValueOpt != null)
            {
                self.DispatchExpression(expr.YieldValueOpt);
            }
        }

        public static void DispatchExpressionExt(this IJstVisitor self, Expression expr)
        {
            switch (expr)
            {
                case ArrayLiteralExpression arrayLiteralExpression:
                    self.VisitArrayLiteralExpression(arrayLiteralExpression);
                    break;
                case AwaitExpression awaitExpression:
                    self.VisitAwaitExpression(awaitExpression);
                    break;
                case BinaryExpression binaryExpression:
                    self.VisitBinaryExpression(binaryExpression);
                    break;
                case BooleanLiteralExpression booleanLiteralExpression:
                    self.VisitBooleanLiteralExpression(booleanLiteralExpression);
                    break;
                case ConditionalOperatorExpression conditionalOperatorExpression:
                    self.VisitConditionalOperatorExpression(conditionalOperatorExpression);
                    break;
                case DoubleLiteralExpression doubleLiteralExpression:
                    self.VisitDoubleLiteralExpression(doubleLiteralExpression);
                    break;
                case ExpressionsList expressionsList:
                    self.VisitExpressionList(expressionsList);
                    break;
                case FunctionExpression functionExpression:
                    self.VisitFunctionExpression(functionExpression);
                    break;
                case IdentifierExpression identifierExpression:
                    self.VisitIdentifierExpression(identifierExpression);
                    break;
                case IdentifierStringExpression identifierStringExpression:
                    self.VisitIdentifierStringExpression(identifierStringExpression);
                    break;
                case IndexExpression indexExpression:
                    self.VisitIndexExpression(indexExpression);
                    break;
                case InlineNewArrayInitialization inlineNewArrayInitialization:
                    self.VisitInlineNewArrayInitializationExpression(inlineNewArrayInitialization);
                    break;
                case InlineObjectInitializer inlineObjectInitializer:
                    self.VisitInlineObjectInitializerExpression(inlineObjectInitializer);
                    break;
                case MethodCallExpression methodCallExpression:
                    self.VisitMethodCallExpression(methodCallExpression);
                    break;
                case NewArrayExpression newArrayExpression:
                    self.VisitNewArrayExpression(newArrayExpression);
                    break;
                case NewObjectExpression newObjectExpression:
                    self.VisitNewObjectExpression(newObjectExpression);
                    break;
                case NullLiteralExpression nullLiteralExpression:
                    self.VisitNullLiteralExpression(nullLiteralExpression);
                    break;
                case NumberLiteralExpression numberLiteralExpression:
                    self.VisitNumberLiteralExpression(numberLiteralExpression);
                    break;
                case ScriptLiteralExpression scriptLiteralExpression:
                    self.VisitScriptLiteralExpression(scriptLiteralExpression);
                    break;
                case StringLiteralExpression stringLiteralExpression:
                    self.VisitStringLiteralExpression(stringLiteralExpression);
                    break;
                case ThisExpression thisExpression:
                    self.VisitThisExpression(thisExpression);
                    break;
                case TupleExpression tupleExpression:
                    self.VisitTupleExpression(tupleExpression);
                    break;
                case UnaryExpression unaryExpression:
                    self.VisitUnaryExpression(unaryExpression);
                    break;
                case YieldExpression yieldExpression:
                    self.VisitYieldExpression(yieldExpression);
                    break;
                case null:
                    break;
                default: throw new NotImplementedException();
            }
        }

        public static void VisitBreakStatementExt(this IJstVisitor self, BreakStatement breakStatement)
        { }

        public static void VisitContinueStatementExt(this IJstVisitor self, ContinueStatement continueStatement)
        {
        }

        public static void VisitCatchHandlerExt(this IJstVisitor self, CatchHandler catchHandler)
        {
            if (catchHandler.Identifier != null)
            {
                self.DispatchExpression(catchHandler.Identifier);
            }

            self.DispatchStatement(catchHandler.CatchBlock);
        }

        public static void VisitDoWhileLoopExt(this IJstVisitor self, DoWhileLoop doWhileLoop)
        {
            self.DispatchStatement(doWhileLoop.Loop);
            self.DispatchExpression(doWhileLoop.Condition);
        }

        public static void VisitEmptyStatementExt(this IJstVisitor self, EmptyStatement emptyStatement)
        { }

        public static void VisitExpressionStatementExt(this IJstVisitor self, ExpressionStatement expressionStatement)
        {
            self.DispatchExpression(expressionStatement.Expression);
        }

        public static void VisitForInLoopExt(this IJstVisitor self, ForInLoop forLoop)
        {
            self.DispatchStatement(forLoop.Loop);
            self.DispatchExpression(forLoop.Collection);
            self.DispatchExpression(forLoop.Key);
        }

        public static void VisitForLoopExt(this IJstVisitor self, ForLoop forLoop)
        {
            self.DispatchStatement(forLoop.InitializationBlock);
            self.DispatchExpression(forLoop.Condition);
            self.DispatchStatement(forLoop.Loop);
            self.DispatchStatement(forLoop.IncrementBlock);
        }

        public static void VisitIfBlockStatementExt(this IJstVisitor self, IfBlockStatement ifBlockStatement)
        {
            self.DispatchExpression(ifBlockStatement.Condition);
            self.DispatchStatement(ifBlockStatement.TrueBlock);
            if (ifBlockStatement.FalseBlock != null)
            {
                self.DispatchStatement(ifBlockStatement.FalseBlock);
            }
        }

        public static void VisitInitializerStatementExt(this IJstVisitor self, InitializerStatement initializerStatement)
        {
            foreach (var init in initializerStatement.Initializers)
            {
                self.DispatchExpression(init);
            }
        }

        public static void VisitReturnStatementExt(this IJstVisitor self, ReturnStatement returnStatement)
        {
            if (returnStatement.ReturnExpression != null)
            {
                self.DispatchExpression(returnStatement.ReturnExpression);
            }
        }

        public static void VisitScopeBlockExt(this IJstVisitor self, ScopeBlock scopeBlock)
        {
            foreach (var statement in scopeBlock.Statements)
            {
                self.DispatchStatement(statement);
            }
        }

        public static void VisitSwitchBlockStatementExt(this IJstVisitor self, SwitchBlockStatement switchBlockStatement)
        {
            self.DispatchExpression(switchBlockStatement.Key);
            foreach (var caseBlock in switchBlockStatement.CaseBlocks)
            {
                foreach (var caseExpr in caseBlock.Key)
                {
                    self.DispatchExpression(caseExpr);
                }

                self.DispatchStatement(caseBlock.Value);
            }
        }

        public static void VisitTryCatchFinallyBlockExt(this IJstVisitor self, TryCatchFinalyBlock tryCatchFinalyBlock)
        {
            self.DispatchStatement(tryCatchFinalyBlock.Try);
            if (tryCatchFinalyBlock.Catch != null)
            {
                self.VisitCatchHandler(tryCatchFinalyBlock.Catch);
            }

            if (tryCatchFinalyBlock.Finally != null)
            {
                self.DispatchStatement(tryCatchFinalyBlock.Finally);
            }
        }

        public static void VisitVarInitializerStatementExt(this IJstVisitor self, VarInitializerStatement varInitializerStatement)
        {
            foreach (var item in varInitializerStatement.Initializers)
            {
                self.DispatchExpression(item);
            }
        }

        public static void VisitWhileLoopExt(this IJstVisitor self, WhileLoop whileLoop)
        {
            if (whileLoop.Condition != null)
            {
                self.DispatchExpression(whileLoop.Condition);
            }

            self.DispatchStatement(whileLoop.Loop);
        }


        public static void DispatchStatementExt(this IJstVisitor self, Statement statement)
        {
            switch (statement)
            {
                case BreakStatement breakStatement:
                    self.VisitBreakStatement(breakStatement);
                    break;
                case CatchHandler catchHandler:
                    self.VisitCatchHandler(catchHandler);
                    break;
                case ContinueStatement continueStatement:
                    self.VisitContinueStatement(continueStatement);
                    break;
                case DoWhileLoop doWhileLoop:
                    self.VisitDoWhileLoop(doWhileLoop);
                    break;
                case EmptyStatement emptyStatement:
                    self.VisitEmptyStatement(emptyStatement);
                    break;
                case ExpressionStatement expressionStatement:
                    self.VisitExpressionStatement(expressionStatement);
                    break;
                case ForInLoop forInLoop:
                    self.VisitForInLoop(forInLoop);
                    break;
                case ForLoop forLoop:
                    self.VisitForLoop(forLoop);
                    break;
                case IfBlockStatement ifBlockStatement:
                    self.VisitIfBlockStatement(ifBlockStatement);
                    break;
                case VarInitializerStatement varInitializerStatement:
                    self.VisitVarInitializerStatement(varInitializerStatement);
                    break;
                case InitializerStatement initializerStatement:
                    self.VisitInitializerStatement(initializerStatement);
                    break;
                case ReturnStatement returnStatement:
                    self.VisitReturnStatement(returnStatement);
                    break;
                case ScopeBlock scopeBlock:
                    self.VisitScopeBlock(scopeBlock);
                    break;
                case SwitchBlockStatement switchBlockStatement:
                    self.VisitSwitchBlockStatement(switchBlockStatement);
                    break;
                case ThrowStatement throwStatement:
                    self.VisitThrowExpression(throwStatement);
                    break;
                case TryCatchFinalyBlock tryCatchFinalyBlock:
                    self.VisitTryCatchFinallyBlock(tryCatchFinalyBlock);
                    break;
                case WhileLoop whileLoop:
                    self.VisitWhileLoop(whileLoop);
                    break;
                case null:
                    break;
                default: throw new NotImplementedException();

            }
        }
    }
}
