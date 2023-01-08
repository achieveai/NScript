namespace NScript.Converter.ExpressionsConverter
{
    using CLR.AST;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter.TypeSystemConverter;

    public static class AwaitExpressionConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter methodConverter,
            AwaitExpression awaitExpression)
        {
            var awaitable = awaitExpression.Expression;
            var getAwaiterCall = awaitExpression.GetAwaiterCallExpression;
            return new JST.AwaitExpression(
                awaitable.Location,
                methodConverter.Scope,
                GetJSTAwaitableExpr(awaitable, (MethodCallExpression)getAwaiterCall, methodConverter));
        }

        private static JST.Expression GetJSTAwaitableExpr(Expression awaitable, MethodCallExpression getAwaiterMethodCall,IMethodScopeConverter methodScopeConverter)
        {
            var ty = awaitable.ResultType;

            return IsPromiseLike(ty, methodScopeConverter)
                ? ExpressionConverterBase.Convert(methodScopeConverter, awaitable)
                : ExpressionConverterBase.Convert(methodScopeConverter, getAwaiterMethodCall);
        }

        private static bool IsPromiseLike(TypeReference ty, IMethodScopeConverter methodScopeConverter)
        {
            var clrKnownReferences = methodScopeConverter.ClrKnownReferences;

            return TypeHelpers.IsSameDefinition(ty, clrKnownReferences.PromiseType)
                || TypeHelpers.IsSameDefinition(ty, clrKnownReferences.PromiseGenericTypeReference)
                || TypeHelpers.IsSameDefinition(ty, clrKnownReferences.TaskTypeReference)
                || TypeHelpers.IsSameDefinition(ty, clrKnownReferences.TaskGenericTypeReference);
        }
    }
}
