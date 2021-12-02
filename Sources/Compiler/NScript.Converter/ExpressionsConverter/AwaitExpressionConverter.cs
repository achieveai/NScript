namespace NScript.Converter.ExpressionsConverter
{
    using CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using System.Collections.Generic;

    public class AwaitExpressionConverter
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
            var tyDef = awaitable.ResultType.Resolve();
            var clrKnownReferences = methodScopeConverter.ClrKnownReferences;

            if (tyDef == clrKnownReferences.PromiseType || tyDef == clrKnownReferences.PromiseGenericTypeReference)
            {
                return ExpressionConverterBase.Convert(methodScopeConverter, awaitable);
            }
            else
            {
                return ExpressionConverterBase.Convert(methodScopeConverter, getAwaiterMethodCall);
            }
        }
    }
}
