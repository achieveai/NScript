//-----------------------------------------------------------------------
// <copyright file="NewObjectConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using System.Linq;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using Mono.Cecil;

    /// <summary>
    /// Definition for NewObjectConverter
    /// </summary>
    public static class NewObjectConverter
    {
        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="newObjectExpression">The new object expression.</param>
        /// <returns>JST.MethodCallExpression for method constructor.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter methodConverter,
            NewObjectExpression newObjectExpression)
        {
            MethodReference methodReference = ((ConstructorReferenceExpression)newObjectExpression.MethodReference).Constructor;

            var converter = NewObjectConverter.ConverterSpecialMethod(methodReference);
            if (converter != null)
            {
                return converter(methodConverter, newObjectExpression);
            }

            TypeDefinition typeDef = methodReference.DeclaringType.Resolve();
            if (!methodConverter.RuntimeManager.Context.IsExtended(typeDef)
                && !methodConverter.RuntimeManager.Context.IsPsudoType(typeDef))
            {
                if (methodReference.Parameters.Count == 0
                    && typeDef.IsValueType)
                {
                    // Let's use default constructor when there are 0 constructor parameters.
                    return DefaultValueConverter.GetDefaultValue(
                        methodConverter,
                        methodConverter.RuntimeManager,
                        methodConverter.Scope,
                        methodReference.DeclaringType,
                        newObjectExpression.Location);
                }

                var (args, toPreInject) = MethodCallExpressionConverter.ReorderArgs(
                    methodConverter,
                    newObjectExpression.Parameters,
                    newObjectExpression.ArgumentOrderOpt);

                // Call into factory methods that will create and return the type.
                var methodCallExpression = new JST.MethodCallExpression(
                    newObjectExpression.Location,
                    methodConverter.Scope,
                    JST.IdentifierExpression.Create(
                        newObjectExpression.Location,
                        methodConverter.Scope,
                        methodConverter.ResolveFactory(methodReference)),
                    args.ToArray());

                if (toPreInject == null)
                {
                    return methodCallExpression;
                }

                toPreInject.Add(methodCallExpression);
                return new JST.ExpressionsList(
                    newObjectExpression.Location,
                    methodConverter.Scope,
                    toPreInject);
            }
            else
            {
                var typeIdentifier = methodConverter.Resolve(typeDef);
                JST.SimpleIdentifier simpleIdentifier = typeIdentifier.Count == 1 ? typeIdentifier[0] as JST.SimpleIdentifier : null;
                if (simpleIdentifier != null
                    && !methodReference.HasParameters
                    && simpleIdentifier.SuggestedName == "Object"
                    && simpleIdentifier.ShouldEnforceSuggestion)
                {
                    return new JST.InlineObjectInitializer(newObjectExpression.Location, methodConverter.Scope);
                }

                var (args, toPreInject) = MethodCallExpressionConverter.ReorderArgs(
                    methodConverter,
                    newObjectExpression.Parameters,
                    newObjectExpression.ArgumentOrderOpt);

                var expr = new JST.NewObjectExpression(
                    newObjectExpression.Location,
                    methodConverter.Scope,
                    JST.IdentifierExpression.Create(
                        newObjectExpression.Location,
                        methodConverter.Scope,
                        methodConverter.Resolve(methodReference.DeclaringType)),
                    args.ToArray());

                if (toPreInject == null)
                {
                    return expr;
                }

                toPreInject.Add(expr);
                return new JST.ExpressionsList(
                    newObjectExpression.Location,
                    methodConverter.Scope,
                    toPreInject);
            }
        }

        /// <summary>
        /// Converters the special method.
        /// </summary>
        /// <param name="methodReference">The method reference.</param>
        /// <returns>Convert special method.</returns>
        internal static Func<IMethodScopeConverter, NewObjectExpression, JST.Expression> ConverterSpecialMethod(
            MethodReference methodReference)
        {
            if (methodReference.DeclaringType.FullName == "System.Collections.Dictionary")
            {
                if (methodReference.Name == ".ctor"
                    && methodReference.Parameters.Count == 1
                    && methodReference.Parameters[0].ParameterType.IsArray)
                {
                    return NewObjectConverter.CreateDictionary;
                }
            }

            return null;
        }

        private static JST.Expression CreateDictionary(
            IMethodScopeConverter converter,
            NewObjectExpression methodCall)
        {
            var newInlineObject =
                new JST.InlineObjectInitializer(
                    methodCall.Location,
                    converter.Scope);

            var inlineArrayInitialization =
                methodCall.Parameters[0] as InlineArrayInitialization;

            if (inlineArrayInitialization == null)
            {
                throw new InvalidProgramException(
                    string.Format(
                        "Wrong use of dict constructor at {0}, need to pass params argument on the function call.",
                        methodCall.Location));
            }

            for (int arrayElementIndex = 0;
                arrayElementIndex < inlineArrayInitialization.ElementInitValues.Count;
                arrayElementIndex += 2)
            {
                if (!(inlineArrayInitialization.ElementInitValues[arrayElementIndex] is StringLiteral))
                {
                    throw new InvalidProgramException(
                        string.Format(
                            "Wrong use of dict constructor at {0}",
                            methodCall.Location));
                }

                newInlineObject.AddInitializer(
                    ((StringLiteral)inlineArrayInitialization.ElementInitValues[arrayElementIndex]).String,
                    ExpressionConverterBase.Convert(
                        converter,
                        inlineArrayInitialization.ElementInitValues[arrayElementIndex + 1]));
            }

            return newInlineObject;
        }
    }
}