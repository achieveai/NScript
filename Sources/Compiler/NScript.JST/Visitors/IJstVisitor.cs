namespace NScript.JST.Visitors
{
    internal interface IJstVisitor
    {
        void VisitArrayLiteralExpression(ArrayLiteralExpression expr)
        {
            foreach (var item in expr.Elements)
            {
                DispatchExpression(item);
            }
        }

        void VisitAwaitExpression(AwaitExpression expr)
        {
            DispatchExpression(expr.Expression);
        }

        void VisitBinaryExpression(BinaryExpression expr)
        {
            DispatchExpression(expr.Left);
            DispatchExpression(expr.Right);
        }

        void VisitBooleanLiteralExpression(BooleanLiteralExpression expr)
        {
        }

        void VisitConditionalOperatorExpression(ConditionalOperatorExpression expr)
        {
            DispatchExpression(expr.Condition);
            DispatchExpression(expr.TrueExpression);
            DispatchExpression(expr.FalseExpression);
        }

        void VisitDoubleLiteralExpression(DoubleLiteralExpression expr)
        {
        }

        void VisitExpressionList(ExpressionsList expr)
        {
            for(var item in expr.Expressions)
            {
                DispatchExpression(item);
            }
        }

        void VisitFunctionExpression(FunctionExpression expr)
        {
            for(var item in expr.Statements)
            {
                DispatchStatement(item);
            }
        }

        void VisitIdentifierExpression(IdentifierExpression expr)
        {
        }

        void VisitIdentifierStringExpression(IdentifierStringExpression expr)
        {
            DispatchExpression(expr.Expression);
        }

        void VisitIndexExpression(IndexExpression expr)
        {
            DispatchExpression(expr.LeftExpression);
            DispatchExpression(expr.RightExpression);
        }

        void VisitInlineNewArrayInitializationExpression(InlineNewArrayInitialization expr)
        {
            foreach (var item in expr.Values)
            {
                DispatchExpression(item);
            }
        }

        void VisitInlineObjectInitializerExpression(InlineObjectInitializer expr)
        {
            foreach (var item in expr.Initializers)
            {
                DispatchExpression(item.Item1);
                DispatchExpression(item.Item2);
            }
        }

        void VisitMethodCallExpression(MethodCallExpression expr)
        {
            DispatchExpression(expr.MethodExpression);
            foreach (var arg in expr.Arguments)
            {
                DispatchExpression(arg);
            }
        }

        void VisitNewArrayExpression(NewArrayExpression expr)
        {
            DispatchExpression(expr.Size);
        }

        void VisitNewObjectExpression(NewObjectExpression expr)
        {
            DispatchExpression(expr.ObjectAccessExpression);
            foreach (var arg in expr.Arguments)
            {
                DispatchExpression(arg);
            }
        }

        void VisitNullLiteralExpression(NullLiteralExpression expr)
        {
        }

        void VisitNumberLiteralExpression(NumberLiteralExpression expr)
        {
        }

        void VisitScriptLiteralExpression(ScriptLiteralExpression expr)
        {
            foreach (var arg in expr.LiteralArguments)
            {
                DispatchExpression(arg);
            }
        }

        void VisitStringLiteralExpression(StringLiteralExpression expr)
        {
        }

        void VisitThisExpression(ThisExpression expr)
        {
        }

        void VisitThrowExpression(ThrowExpression throwExpression)
        {
            DispatchExpression(throwExpression.Expression);
        }

        void VisitTupleExpression(TupleExpression expr)
        {
            foreach (var arg in expr.Arguments)
            {
                DispatchExpression(arg);
            }
        }

        void VisitUnaryExpression(UnaryExpression expr)
        {
            DispatchExpression(expr.NestedExpression);
        }

        void VisitYieldExpression(YieldExpression expr)
        {
            if (expr.YieldValueOpt != null)
            {
                DispatchExpression(expr.YieldValueOpt);
            }
        }

        void DispatchExpression(Expression expr)
        {
            switch(expr)
            {
                case ArrayLiteralExpression arrayLiteralExpression:
                    VisitArrayLiteralExpression(arrayLiteralExpression);
                    break;
                case AwaitExpression awaitExpression:
                    VisitAwaitExpression(awaitExpression);
                    break;
                case BinaryExpression binaryExpression:
                    VisitBinaryExpression(binaryExpression);
                    break;
                case BooleanLiteralExpression booleanLiteralExpression:
                    VisitBooleanLiteralExpression(booleanLiteralExpression);
                    break;
                case ConditionalOperatorExpression conditionalOperatorExpression:
                    VisitConditionalOperatorExpression(conditionalOperatorExpression);
                    break;
                case DoubleLiteralExpression doubleLiteralExpression:
                    VisitDoubleLiteralExpression(doubleLiteralExpression);
                    break;
                case ExpressionsList expressionsList:
                    VisitExpressionList(expressionsList);
                    break;
                case FunctionExpression functionExpression:
                    VisitFunctionExpression(functionExpression);
                    break;
                case IdentifierExpression identifierExpression:
                    VisitIdentifierExpression(identifierExpression);
                    break;
                case IdentifierStringExpression identifierStringExpression:
                    VisitIdentifierStringExpression(identifierStringExpression);
                    break;
                case IndexExpression indexExpression:
                    VisitIndexExpression(indexExpression);
                    break;
                case InlineNewArrayInitialization inlineNewArrayInitialization:
                    VisitInlineNewArrayInitializationExpression(inlineNewArrayInitialization);
                    break;
                case InlineObjectInitializer inlineObjectInitializer:
                    VisitInlineObjectInitializerExpression(inlineObjectInitializer);
                    break;
                case MethodCallExpression methodCallExpression:
                    VisitMethodCallExpression(methodCallExpression);
                    break;
                case NewArrayExpression newArrayExpression:
                    VisitNewArrayExpression(newArrayExpression);
                    break;
                case NewObjectExpression newObjectExpression:
                    VisitNewObjectExpression(newObjectExpression);
                    break;
                case NullLiteralExpression nullLiteralExpression:
                    VisitNullLiteralExpression(nullLiteralExpression);
                    break;
                case NumberLiteralExpression numberLiteralExpression:
                    VisitNumberLiteralExpression(numberLiteralExpression);
                    break;
                case ScriptLiteralExpression scriptLiteralExpression:
                    VisitScriptLiteralExpression(scriptLiteralExpression);
                    break;
                case ScriptLiteralExpression scriptLiteralExpression:
                    VisitScriptLiteralExpression(scriptLiteralExpression);
                    break;
                case ThisExpression thisExpression:
                    VisitThisExpression(thisExpression);
                    break;
                case ThrowExpression throwExpression:
                    VisitThrowExpression(throwExpression);
                    break;
                case TupleExpression tupleExpression:
                    VisitTupleExpression(tupleExpression);
                    break;
                case UnaryExpression unaryExpression:
                    VisitUnaryExpression(unaryExpression);
                    break;
                case YieldExpression yieldExpression:
                    VisitYieldExpression(yieldExpression);
                    break;
                default: throw new NotImplementedException();
            }
        }

        void VisitBreakStatement(BreakStatement breakStatement)
        { }

        void VisitContinueStatement(ContinueStatement continueStatement)
        {
        }

        void VisitCatchHandler(CatchHandler catchHandler)
        {
            if (catchHandler.Identifier != null)
            {
                DispatchExpression(catchHandler.Identifier);
            }

            DispatchStatement(catchHandler.CatchBlock);
        }

        void VisitDoWhileLoop(DoWhileLoop doWhileLoop)
        {
            DispatchStatement(doWhileLoop.Loop);
            DispatchExpression(doWhileLoop.Condition);
        }

        void VisitExpressionStatement(ExpressionStatement expressionStatement)
        {
            DispatchExpression(expressionStatement.Expression);
        }

        void VisitForInLoop(ForInLoop forLoop)
        {
            DispatchStatement(forLoop.Loop);
            DispatchExpression(forLoop.Collection);
            DispatchExpression(forLoop.Key);
        }

        void VisitForLoop(ForLoop forLoop)
        {
            DispatchStatement(forLoop.InitializationBlock);
            DispatchExpression(forLoop.Condition);
            DispatchStatement(forLoop.Loop);
            DispatchStatement(forLoop.IncrementBlock);
        }

        void VisitIfBlockStatement(IfBlockStatement ifBlockStatement)
        {
            DispatchExpression(ifBlockStatement.Condition);
            DispatchStatement(ifBlockStatement.TrueBlock);
            if (ifBlockStatement.FalseBlock != null)
            {
                DispatchStatement(ifBlockStatement.FalseBlock);
            }
        }

        void VisitInitializerStatement(InitializerStatement initializerStatement)
        {
            foreach (var init in initializerStatement.Initializers)
            {
                DispatchExpression(init);
            }
        }

        void VisitReturnStatement(ReturnStatement returnStatement)
        {
            if (returnStatement.ReturnExpression != null)
            {
                DispatchExpression(returnStatement.ReturnExpression);
            }
        }

        void VisitScopeBlock(ScopeBlock scopeBlock)
        {
            foreach (var statement in scopeBlock.Statements)
            {
                DispatchStatement(statement);
            }
        }

        void VisitSwitchBlockStatement(SwitchBlockStatement switchBlockStatement)
        {
            DispatchExpression(switchBlockStatement.Key);
            foreach (var caseBlock in switchBlockStatement.CaseBlocks)
            {
                foreach(var caseExpr in caseBlock.Key)
                {
                    DispatchExpression(caseExpr);
                }

                DispatchStatement(caseBlock.Value);
            }
        }

        void VisitTryCatchFinallyBlock(TryCatchFinalyBlock tryCatchFinalyBlock)
        {
            DispatchStatement(tryCatchFinalyBlock.Try);
            if (tryCatchFinalyBlock.Catch != null)
            {
                VisitCatchHandler(tryCatchFinalyBlock.Catch);
            }

            if (tryCatchFinalyBlock.Finally != null)
            {
                DispatchStatement(tryCatchFinalyBlock.Finally);
            }
        }

        void VisitVarInitializerStatement(VarInitializerStatement varInitializerStatement)
        {
            foreach (var item in varInitializerStatement.Initializers)
            {
                DispatchExpression(item);
            }
        }

        void VisitWhileLoop(WhileLoop whileLoop)
        {
            if (whileLoop.Condition!= null)
            {
                DispatchExpression(whileLoop.Condition);
            }

            DispatchStatement(whileLoop.Loop);
        }


        void DispatchStatement(Statement statement)
        {
            switch(statement)
            {
                case BreakStatement breakStatement:
                    VisitBinaryExpression(breakStatement);
                    break;
                case CatchHandler catchHandler:
                    VisitCatchHandler(catchHandler);
                    break;
                case ContinueStatement continueStatement:
                    VisitContinueStatement(continueStatement);
                    break;
                case DoWhileLoop doWhileLoop:
                    VisitDoWhileLoop(doWhileLoop);
                    break;
                case ExpressionStatement expressionStatement:
                    VisitExpressionStatement(expressionStatement);
                    break;
                case ForInLoop forInLoop:
                    VisitForInLoop(forInLoop);
                    break;
                case ForLoop forLoop:
                    VisitForLoop(forLoop);
                    break;
                case IfBlockStatement ifBlockStatement:
                    VisitIfBlockStatement(ifBlockStatement);
                    break;
                case InitializerStatement initializerStatement:
                    VisitInitializerStatement(initializerStatement);
                    break;
                case ReturnStatement returnStatement:
                    VisitReturnStatement(returnStatement);
                    break;
                case ScopeBlock scopeBlock:
                    VisitScopeBlock(scopeBlock);
                    break;
                case SwitchBlockStatement switchBlockStatement:
                    VisitSwitchBlockStatement(switchBlockStatement);
                    break;
                case TryCatchFinalyBlock tryCatchFinalyBlock:
                    VisitTryCatchFinallyBlock(tryCatchFinalyBlock);
                    break;
                case VarInitializerStatement varInitializerStatement:
                    VisitVarInitializerStatement(varInitializerStatement);
                    break;
                case WhileLoop whileLoop:
                    VisitWhileLoop(whileLoop);
                    break;
                default: throw new NotImplementedException();

            }
        }
    }
}
