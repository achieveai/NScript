namespace NScript.JST.Visitors
{
    using System;

    public interface IJstVisitor
    {
        void VisitArrayLiteralExpression(ArrayLiteralExpression expr)
            => JstVisitorExtension.VisitArrayLiteralExpressionExt(this, expr);

        void VisitAwaitExpression(AwaitExpression expr)
            => JstVisitorExtension.VisitAwaitExpressionExt(this, expr);

        void VisitBinaryExpression(BinaryExpression expr)
            => JstVisitorExtension.VisitBinaryExpressionExt(this, expr);

        void VisitBooleanLiteralExpression(BooleanLiteralExpression expr)
            => JstVisitorExtension.VisitBooleanLiteralExpressionExt(this, expr);

        void VisitConditionalOperatorExpression(ConditionalOperatorExpression expr)
            => JstVisitorExtension.VisitConditionalOperatorExpressionExt(this, expr);

        void VisitDoubleLiteralExpression(DoubleLiteralExpression expr)
            => JstVisitorExtension.VisitDoubleLiteralExpressionExt(this, expr);

        void VisitExpressionList(ExpressionsList expr)
            => JstVisitorExtension.VisitExpressionListExt(this, expr);

        void VisitFunctionExpression(FunctionExpression expr)
            => JstVisitorExtension.VisitFunctionExpressionExt(this, expr);

        void VisitIdentifierExpression(IdentifierExpression expr)
            => JstVisitorExtension.VisitIdentifierExpressionExt(this, expr);

        void VisitIdentifierStringExpression(IdentifierStringExpression expr)
            => JstVisitorExtension.VisitIdentifierStringExpressionExt(this, expr);

        void VisitIndexExpression(IndexExpression expr)
            => JstVisitorExtension.VisitIndexExpressionExt(this, expr);

        void VisitInlineNewArrayInitializationExpression(InlineNewArrayInitialization expr)
            => JstVisitorExtension.VisitInlineNewArrayInitializationExpressionExt(this, expr);

        void VisitInlineObjectInitializerExpression(InlineObjectInitializer expr)
            => JstVisitorExtension.VisitInlineObjectInitializerExpressionExt(this, expr);

        void VisitMethodCallExpression(MethodCallExpression expr)
            => JstVisitorExtension.VisitMethodCallExpressionExt(this, expr);

        void VisitNewArrayExpression(NewArrayExpression expr)
            => JstVisitorExtension.VisitNewArrayExpressionExt(this, expr);

        void VisitNewObjectExpression(NewObjectExpression expr)
            => JstVisitorExtension.VisitNewObjectExpressionExt(this, expr);

        void VisitNullLiteralExpression(NullLiteralExpression expr)
            => JstVisitorExtension.VisitNullLiteralExpressionExt(this, expr);

        void VisitNumberLiteralExpression(NumberLiteralExpression expr)
            => JstVisitorExtension.VisitNumberLiteralExpressionExt(this, expr);

        void VisitScriptLiteralExpression(ScriptLiteralExpression expr)
            => JstVisitorExtension.VisitScriptLiteralExpressionExt(this, expr);

        void VisitStringLiteralExpression(StringLiteralExpression expr)
            => JstVisitorExtension.VisitStringLiteralExpressionExt(this, expr);

        void VisitThisExpression(ThisExpression expr)
            => JstVisitorExtension.VisitThisExpressionExt(this, expr);

        void VisitThrowExpression(ThrowExpression throwExpression)
            => JstVisitorExtension.VisitThrowExpressionExt(this, throwExpression);

        void VisitTupleExpression(TupleExpression expr)
            => JstVisitorExtension.VisitTupleExpressionExt(this, expr);

        void VisitUnaryExpression(UnaryExpression expr)
            => JstVisitorExtension.VisitUnaryExpressionExt(this, expr);

        void VisitYieldExpression(YieldExpression expr)
            => JstVisitorExtension.VisitYieldExpressionExt(this, expr);

        void DispatchExpression(Expression expr)
            => JstVisitorExtension.DispatchExpressionExt(this, expr);

        void VisitBreakStatement(BreakStatement breakStatement)
            => JstVisitorExtension.VisitBreakStatementExt(this, breakStatement);

        void VisitContinueStatement(ContinueStatement continueStatement)
            => JstVisitorExtension.VisitContinueStatementExt(this, continueStatement);

        void VisitCatchHandler(CatchHandler catchHandler)
            => JstVisitorExtension.VisitCatchHandlerExt(this, catchHandler);

        void VisitDoWhileLoop(DoWhileLoop doWhileLoop)
            => JstVisitorExtension.VisitDoWhileLoopExt(this, doWhileLoop);

        void VisitExpressionStatement(ExpressionStatement expressionStatement)
            => JstVisitorExtension.VisitExpressionStatementExt(this, expressionStatement);

        void VisitForInLoop(ForInLoop forLoop)
            => JstVisitorExtension.VisitForInLoopExt(this, forLoop);

        void VisitForLoop(ForLoop forLoop)
            => JstVisitorExtension.VisitForLoopExt(this, forLoop);

        void VisitIfBlockStatement(IfBlockStatement ifBlockStatement)
            => JstVisitorExtension.VisitIfBlockStatementExt(this, ifBlockStatement);

        void VisitInitializerStatement(InitializerStatement initializerStatement)
            => JstVisitorExtension.VisitInitializerStatementExt(this, initializerStatement);

        void VisitReturnStatement(ReturnStatement returnStatement)
            => JstVisitorExtension.VisitReturnStatementExt(this, returnStatement);

        void VisitScopeBlock(ScopeBlock scopeBlock)
            => JstVisitorExtension.VisitScopeBlockExt(this, scopeBlock);

        void VisitSwitchBlockStatement(SwitchBlockStatement switchBlockStatement)
            => JstVisitorExtension.VisitSwitchBlockStatementExt(this, switchBlockStatement);

        void VisitTryCatchFinallyBlock(TryCatchFinalyBlock tryCatchFinalyBlock)
            => JstVisitorExtension.VisitTryCatchFinallyBlockExt(this, tryCatchFinalyBlock);

        void VisitVarInitializerStatement(VarInitializerStatement varInitializerStatement)
            => JstVisitorExtension.VisitVarInitializerStatementExt(this, varInitializerStatement);

        void VisitWhileLoop(WhileLoop whileLoop)
            => JstVisitorExtension.VisitWhileLoopExt(this, whileLoop);

        void DispatchStatement(Statement statement)
            => JstVisitorExtension.DispatchStatementExt(this, statement);
    }
}
