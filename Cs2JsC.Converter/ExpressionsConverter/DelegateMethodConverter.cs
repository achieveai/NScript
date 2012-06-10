//-----------------------------------------------------------------------
// <copyright file="DelegateMethodConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Mono.Cecil;

    /// <summary>
    /// Definition for DelegateMethodConverter
    /// </summary>
    public static class DelegateMethodConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Expression for creating delegate.</returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            DelegateMethodExpression expression)
        {
            List<JST.Expression> genericArguments = new List<JST.Expression>();
            var location = expression.Location;

            if (expression.Method.MethodReference.IsGenericInstance)
            {
                GenericInstanceMethod genericMethodInstance = (GenericInstanceMethod)expression.Method.MethodReference;
                foreach (var genericType in genericMethodInstance.GenericArguments)
                {
                    genericArguments.Add(
                        JST.IdentifierExpression.Create(
                            location,
                            converter.Scope,
                            converter.Resolve(genericType)));
                }
            }

            if (!expression.Method.MethodReference.HasThis)
            {
                if (expression.Method.MethodReference.IsGenericInstance)
                {
                    List<JST.Expression> args = new List<JST.Expression>();
                    var methodIdentifiers =
                            converter.ResolveStaticMember(expression.Method.MethodReference);

                    // Method name.
                    args.Add(
                        new JST.IdentifierStringExpression(
                            location,
                            converter.Scope,
                            new JST.IdentifierExpression(
                                methodIdentifiers[methodIdentifiers.Count - 1],
                                converter.Scope)));

                    // Type reference.
                    args.Add(
                        JST.IdentifierExpression.Create(
                            location,
                            converter.Scope,
                            converter.Resolve(expression.Method.MethodReference.DeclaringType)));

                    // method pointer.
                    args.Add(
                        JST.IdentifierExpression.Create(
                            location,
                            converter.Scope,
                            methodIdentifiers));

                    // generic arguments array.
                    args.Add(
                        new JST.InlineNewArrayInitialization(
                            location,
                            converter.Scope,
                            genericArguments));

                    return MethodCallExpressionConverter.CreateMethodCallExpression(
                        new MethodCallContext(
                            converter.KnownReferences.CreateGenericDelegate,
                            null,
                            converter.Scope),
                        args,
                        converter,
                        converter.RuntimeManager);
                }

                return JST.IdentifierExpression.Create(
                    expression.Location,
                    converter.Scope,
                    converter.ResolveStaticMember(expression.Method.MethodReference));
            }

            bool isVirtualCall = expression.Method is VirtualMethodReferenceExpression;

            JST.Expression objectExpression;
            List<JST.Expression> delegateConverterArgs = new List<JST.Expression>();
            MethodReference delegateCreateReference =
                genericArguments.Count == 0
                    ? converter.KnownReferences.CreateDelegate
                    : converter.KnownReferences.CreateGenericDelegate;

            delegateConverterArgs.Add(
                converter.ResolveMethodSlotName(
                    expression.Method.MethodReference,
                    isVirtualCall,
                    converter.Scope));

            if (!isVirtualCall
                && expression.Method.MethodReference.DeclaringType.Resolve().IsValueType)
            {
                objectExpression = ExpressionConverterBase.Convert(
                    converter,
                    ((BoxExpression)expression.Method.LeftExpression).BoxedExpression);

                delegateConverterArgs.Add(objectExpression);
                delegateConverterArgs.Add(
                    JST.IdentifierExpression.Create(
                        expression.Location,
                        converter.Scope,
                        converter.ResolveStaticMember(
                            expression.Method.MethodReference)));

                delegateCreateReference = converter.KnownReferences.StaticInstanceCreateDelegate;
            }
            else
            {
                objectExpression = ExpressionConverterBase.Convert(
                    converter,
                    expression.Method.LeftExpression);

                delegateConverterArgs.Add(objectExpression);
            }

            if (genericArguments.Count > 0)
            {
                // method pointer.
                delegateConverterArgs.Add(
                    new JST.IndexExpression(
                        location,
                        converter.Scope,
                        objectExpression,
                        new JST.IdentifierExpression(
                            converter.Resolve(expression.Method.MethodReference),
                            converter.Scope)));

                delegateConverterArgs.Add(
                        new JST.InlineNewArrayInitialization(
                            location,
                            converter.Scope,
                            genericArguments));
            }

            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(delegateCreateReference, null, converter.Scope),
                delegateConverterArgs,
                converter,
                converter.RuntimeManager);
        }
    }
}