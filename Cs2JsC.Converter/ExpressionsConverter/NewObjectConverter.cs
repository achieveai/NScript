//-----------------------------------------------------------------------
// <copyright file="NewObjectConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using System.Linq;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
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

            if (!methodConverter.RuntimeManager.Context.IsImported(
                    methodReference.DeclaringType.Resolve()))
            {
                // Call into factory methods that will create and return the type.
                return new JST.MethodCallExpression(
                    newObjectExpression.Location,
                    methodConverter.Scope,
                    JST.IdentifierExpression.Create(
                        newObjectExpression.Location,
                        methodConverter.Scope,
                        methodConverter.ResolveFactory(methodReference)),
                    newObjectExpression.Parameters.Select(exp => ExpressionConverterBase.Convert(methodConverter, exp)).ToArray());
            }
            else
            {
                return new JST.NewObjectExpression(
                    newObjectExpression.Location,
                    methodConverter.Scope,
                    JST.IdentifierExpression.Create(
                        newObjectExpression.Location,
                        methodConverter.Scope,
                        methodConverter.Resolve(methodReference.DeclaringType)),
                    newObjectExpression.Parameters.Select(exp => ExpressionConverterBase.Convert(methodConverter, exp)).ToArray());
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
            JST.InlineObjectInitializer inlineArray =
                new JST.InlineObjectInitializer(
                    methodCall.Location,
                    converter.Scope);

            InlineArrayInitialization inlineArrayInitialization = methodCall.Parameters[0] as InlineArrayInitialization;

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

                inlineArray.AddInitializer(
                    ((StringLiteral)inlineArrayInitialization.ElementInitValues[arrayElementIndex]).String,
                    ExpressionConverterBase.Convert(
                        converter,
                        inlineArrayInitialization.ElementInitValues[arrayElementIndex + 1]));
            }

            return inlineArray;
        }
    }
}