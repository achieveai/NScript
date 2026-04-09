namespace NScript.Converter.ExpressionsConverter
{
    using System.Collections.Generic;
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
            var innerExpr = GetJSTAwaitableExpr(awaitable, (MethodCallExpression)getAwaiterCall, methodConverter);

            // Wrap external/imported type awaitables in CallContext.WrapPromise
            // so that the ambient CallContext is preserved across the await boundary.
            // Internal NScript-compiled methods don't need wrapping because
            // TaskScheduler.ExecuteTask already saves/restores context.
            if (IsExternalAwaitable(awaitable, methodConverter))
            {
                innerExpr = WrapInCallContextWrapPromise(methodConverter, innerExpr, awaitable.Location);
            }

            return new JST.AwaitExpression(
                awaitable.Location,
                methodConverter.Scope,
                innerExpr);
        }

        private static JST.Expression GetJSTAwaitableExpr(Expression awaitable, MethodCallExpression getAwaiterMethodCall, IMethodScopeConverter methodScopeConverter)
        {
            var ty = awaitable.ResultType;

            return IsPromiseLike(ty, methodScopeConverter)
                ? ExpressionConverterBase.Convert(methodScopeConverter, awaitable)
                : ExpressionConverterBase.Convert(methodScopeConverter, getAwaiterMethodCall);
        }

        /// <summary>
        /// Returns true if the awaited expression is a direct method call on an
        /// external (imported) type — i.e., a JS library or browser API whose
        /// promises bypass TaskScheduler.
        ///
        /// Only method calls are checked because:
        /// - Delegate invocations (func()) run user code, not external code
        /// - Local function calls run in NScript context
        /// - Variable/property access (await someVar) — if the promise came from
        ///   an external source, WrapPromise was already applied at the original call site.
        ///   KNOWN LIMITATION: if a method returns an external promise without awaiting
        ///   it (e.g., Promise p = JsLib.Fetch("x"); ... return await p;), the variable
        ///   await won't be wrapped. The workaround is to await at the call site.
        /// - Custom awaiters (await nativeArray) are handled by TaskScheduler
        ///
        /// The result type is NOT checked because many NScript facade types
        /// (Promise, NativeArray, etc.) are classified as Imported by GetTypeKind
        /// due to having extern methods, which would cause false positive wrapping.
        /// </summary>
        private static bool IsExternalAwaitable(Expression awaitable, IMethodScopeConverter methodConverter)
        {
            if (awaitable is MethodCallExpression methodCall
                && methodCall.MethodReference is MethodReferenceExpression methodRefExpr)
            {
                var declaringType = methodRefExpr.MethodReference?.DeclaringType?.Resolve();
                if (declaringType != null)
                {
                    var context = methodConverter.RuntimeManager.Context;
                    return context.IsImportedType(declaringType);
                }
            }

            return false;
        }

        /// <summary>
        /// Wraps the awaitable JST expression in a call to CallContext.WrapPromise()
        /// which captures the current context and restores it when the promise resolves.
        /// Returns the original expression unchanged if WrapPromise is not available.
        /// </summary>
        private static JST.Expression WrapInCallContextWrapPromise(
            IMethodScopeConverter methodConverter,
            JST.Expression awaitableExpr,
            NScript.Utils.Location location)
        {
            var wrapPromiseRef = methodConverter.ClrKnownReferences.WrapPromiseMethod;
            if (wrapPromiseRef == null)
            {
                return awaitableExpr;
            }

            var resolvedIdentifiers = methodConverter.ResolveStaticMember(wrapPromiseRef);

            return new JST.MethodCallExpression(
                location,
                methodConverter.Scope,
                JST.IdentifierExpression.Create(location, methodConverter.Scope, resolvedIdentifiers),
                new List<JST.Expression> { awaitableExpr });
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
