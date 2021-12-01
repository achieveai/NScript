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
            var getAwaiterCall = awaitExpression.GetAwaiterMethodExpression;
            return new JST.AwaitExpression(
                awaitable.Location,
                methodConverter.Scope,
                GetJSTAwaitableExpr(awaitable, (MethodCallExpression)getAwaiterCall, methodConverter));
        }

        private static JST.Expression GetJSTAwaitableExpr(Expression awaitable, MethodCallExpression getAwaiterMethodCall,IMethodScopeConverter methodScopeConverter)
        {
            var tyDef = awaitable.ResultType.Resolve();
            var clrKnownReferences = methodScopeConverter.ClrKnownReferences;
            var knownReferences = methodScopeConverter.KnownReferences;

            if (tyDef == clrKnownReferences.PromiseType || tyDef == clrKnownReferences.PromiseGenericTypeReference)
            {
                return ExpressionConverterBase.Convert(methodScopeConverter, awaitable);
            }
            else
            {
                var methodRef = ((MethodReferenceExpression)getAwaiterMethodCall.MethodReference).MethodReference;
                // return ExpressionConverterBase.Convert(methodScopeConverter, getAwaiterMethodCall);
                // TODO(Vijay): Non-Generic case
                var getAwaiterMethodRef = clrKnownReferences.GetAwaiterMethodReference(tyDef);
                var methodIdentifier = methodScopeConverter.ResolveStaticMember(methodRef);
                var methodExpression = new JST.IdentifierExpression(methodIdentifier[0], methodScopeConverter.Scope);

                var awaitableJST = ExpressionConverterBase.Convert(methodScopeConverter, awaitable);

                return new JST.MethodCallExpression(
                    awaitable.Location,
                    methodScopeConverter.Scope,
                    methodExpression,
                    new List<JST.Expression>() { awaitableJST });
            }
        }
    }
}
