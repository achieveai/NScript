namespace NScript.JST.Visitors
{
    public interface ITransformerVisitor
    {
        Expression VisitArrayLiteralExpression(ArrayLiteralExpression expr)
            => TransformerVisitorExtension.VisitArrayLiteralExpressionExt(this, expr);

        Expression VisitAwaitExpression(AwaitExpression expr)
            => TransformerVisitorExtension.VisitAwaitExpressionExt(this, expr);

        Expression VisitBinaryExpression(BinaryExpression expr)
            => TransformerVisitorExtension.VisitBinaryExpressionExt(this, expr);

        Expression VisitBooleanLiteralExpression(BooleanLiteralExpression expr)
            => TransformerVisitorExtension.VisitBooleanLiteralExpressionExt(this, expr);

        Expression VisitConditionalOperatorExpression(ConditionalOperatorExpression expr)
            => TransformerVisitorExtension.VisitConditionalOperatorExpressionExt(this, expr);

        Expression VisitDoubleLiteralExpression(DoubleLiteralExpression expr)
            => TransformerVisitorExtension.VisitDoubleLiteralExpressionExt(this, expr);

        Expression VisitExpressionList(ExpressionsList expr)
            => TransformerVisitorExtension.VisitExpressionListExt(this, expr);

        Expression VisitFunctionExpression(FunctionExpression expr)
            => TransformerVisitorExtension.VisitFunctionExpressionExt(this, expr);

        Expression VisitIdentifierExpression(IdentifierExpression expr)
            => TransformerVisitorExtension.VisitIdentifierExpressionExt(this, expr);

        Expression VisitIdentifierStringExpression(IdentifierStringExpression expr)
            => TransformerVisitorExtension.VisitIdentifierStringExpressionExt(this, expr);

        Expression VisitIndexExpression(IndexExpression expr)
            => TransformerVisitorExtension.VisitIndexExpressionExt(this, expr);

        Expression VisitInlineNewArrayInitializationExpression(InlineNewArrayInitialization expr)
            => TransformerVisitorExtension.VisitInlineNewArrayInitializationExpressionExt(this, expr);

        Expression VisitInlineObjectInitializerExpression(InlineObjectInitializer expr)
            => TransformerVisitorExtension.VisitInlineObjectInitializerExpressionExt(this, expr);

        Expression VisitMethodCallExpression(MethodCallExpression expr)
            => TransformerVisitorExtension.VisitMethodCallExpressionExt(this, expr);

        Expression VisitNewArrayExpression(NewArrayExpression expr)
            => TransformerVisitorExtension.VisitNewArrayExpressionExt(this, expr);

        Expression VisitNewObjectExpression(NewObjectExpression expr)
            => TransformerVisitorExtension.VisitNewObjectExpressionExt(this, expr);

        Expression VisitNullLiteralExpression(NullLiteralExpression expr)
            => TransformerVisitorExtension.VisitNullLiteralExpressionExt(this, expr);

        Expression VisitNumberLiteralExpression(NumberLiteralExpression expr)
            => TransformerVisitorExtension.VisitNumberLiteralExpressionExt(this, expr);

        Expression VisitScriptLiteralExpression(ScriptLiteralExpression expr)
            => TransformerVisitorExtension.VisitScriptLiteralExpressionExt(this, expr);

        Expression VisitStringLiteralExpression(StringLiteralExpression expr)
            => TransformerVisitorExtension.VisitStringLiteralExpressionExt(this, expr);

        Expression VisitThisExpression(ThisExpression expr)
            => TransformerVisitorExtension.VisitThisExpressionExt(this, expr);

        Expression VisitThrowExpression(ThrowExpression expr)
            => TransformerVisitorExtension.VisitThrowExpressionExt(this, expr);

        Expression VisitTupleExpression(TupleExpression expr)
            => TransformerVisitorExtension.VisitTupleExpressionExt(this, expr);

        Expression VisitUnaryExpression(UnaryExpression expr)
            => TransformerVisitorExtension.VisitUnaryExpressionExt(this, expr);

        Expression VisitYieldExpression(YieldExpression expr)
            => TransformerVisitorExtension.VisitYieldExpressionExt(this, expr);

        Expression DispatchExpression(Expression expr)
            => TransformerVisitorExtension.DispatchExpressionExt(this, expr);

        Statement VisitBreakStatement(BreakStatement breakStatement)
            => TransformerVisitorExtension.VisitBreakStatementExt(this, breakStatement);

        Statement VisitContinueStatement(ContinueStatement continueStatement)
            => TransformerVisitorExtension.VisitContinueStatementExt(this, continueStatement);

        Statement VisitCatchHandler(CatchHandler catchHandler)
            => TransformerVisitorExtension.VisitCatchHandlerExt(this, catchHandler);

        Statement VisitDoWhileLoop(DoWhileLoop doWhileLoop)
            => TransformerVisitorExtension.VisitDoWhileLoopExt(this, doWhileLoop);

        Statement VisitExpressionStatement(ExpressionStatement expressionStatement)
            => TransformerVisitorExtension.VisitExpressionStatementExt(this, expressionStatement);

        Statement VisitForInLoop(ForInLoop forLoop)
            => TransformerVisitorExtension.VisitForInLoopExt(this, forLoop);

        Statement VisitForLoop(ForLoop forLoop)
            => TransformerVisitorExtension.VisitForLoopExt(this, forLoop);

        Statement VisitIfBlockStatement(IfBlockStatement ifBlockStatement)
            => TransformerVisitorExtension.VisitIfBlockStatementExt(this, ifBlockStatement);

        Statement VisitInitializerStatement(InitializerStatement initializerStatement)
            => TransformerVisitorExtension.VisitInitializerStatementExt(this, initializerStatement);

        Statement VisitReturnStatement(ReturnStatement returnStatement)
            => TransformerVisitorExtension.VisitReturnStatementExt(this, returnStatement);

        Statement VisitScopeBlock(ScopeBlock scopeBlock)
            => TransformerVisitorExtension.VisitScopeBlockExt(this, scopeBlock);

        Statement VisitSwitchBlockStatement(SwitchBlockStatement switchBlockStatement)
            => TransformerVisitorExtension.VisitSwitchBlockStatementExt(this, switchBlockStatement);

        Statement VisitTryCatchFinallyBlock(TryCatchFinalyBlock tryCatchFinalyBlock)
            => TransformerVisitorExtension.VisitTryCatchFinallyBlockExt(this, tryCatchFinalyBlock);

        Statement VisitVarInitializerStatement(VarInitializerStatement varInitializerStatement)
            => TransformerVisitorExtension.VisitVarInitializerStatementExt(this, varInitializerStatement);

        Statement VisitWhileLoop(WhileLoop whileLoop)
            => TransformerVisitorExtension.VisitWhileLoopExt(this, whileLoop);

        Statement DispatchStatement(Statement statement)
            => TransformerVisitorExtension.DispatchStatementExt(this, statement);
    }
}
