//-----------------------------------------------------------------------
// <copyright file="MethodCallExpressionConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for MethodCallExpressionConverter
    /// </summary>
    public static class MethodCallExpressionConverter
    {
        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns>JST.MethodCallExpression</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter methodConverter,
            MethodCallExpression methodCallExpression)
        {
            MethodReferenceExpression methodReferenceExpression =
                methodCallExpression.MethodReference as MethodReferenceExpression;

            if (methodReferenceExpression != null)
            {
                var converterFunc =
                    MethodCallExpressionConverter.ConverterSpecialMethod(
                        methodConverter.RuntimeManager.Context,
                        methodReferenceExpression.MethodReference);

                if (converterFunc != null)
                {
                    return converterFunc(methodConverter, methodCallExpression);
                }
            }

            return MethodCallExpressionConverter.ConvertInternal(
                methodConverter,
                methodCallExpression);
        }

        /// <summary>
        /// Converts the internal.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns></returns>
        private static JST.Expression ConvertInternal(
            IMethodScopeConverter methodConverter,
            MethodCallExpression methodCallExpression)
        {
            MethodReferenceExpression methodReferenceExpression =
                methodCallExpression.MethodReference as MethodReferenceExpression;

            if (methodReferenceExpression != null)
            {
                if (methodConverter.RuntimeManager.Context.IsExtended(
                        methodReferenceExpression.MethodReference.DeclaringType.Resolve())
                    && methodReferenceExpression.MethodReference.Name == ".ctor")
                {
                    // There is no point of generating call to constructor for Object
                    // and also that constructor is not really implemented.
                    return null;
                }
            }

            // Here we are trying to re-route virtual calls to static method
            // for enums.
            VirtualMethodReferenceExpression virtualRefExpression =
                methodReferenceExpression as VirtualMethodReferenceExpression;

            BoxExpression boxedExpression =
                virtualRefExpression != null
                    ? virtualRefExpression.LeftExpression as BoxExpression
                    : null;

            if (boxedExpression != null)
            {
                TypeReference resultTypeReference =
                    (TypeReference)boxedExpression.BoxedExpression.ResultType;
                TypeDefinition resultTypeDefinition = resultTypeReference.Resolve();

                // Here we are trying to re-route virtual calls to static method
                // for enums.
                if (virtualRefExpression.MethodReference.Name == "ToString"
                    && resultTypeDefinition != null
                    && methodConverter.ClrKnownReferences.Enum.IsSame(
                        resultTypeDefinition.BaseType))
                {
                    JST.Expression enumExpression =
                        ExpressionConverterBase.Convert(
                            methodConverter,
                            boxedExpression.BoxedExpression);

                    var methodCallContext = new MethodCallContext(
                            methodConverter.KnownReferences.EnumToStringMethod,
                            methodCallExpression.Location,
                            methodConverter.Scope);

                    return MethodCallExpressionConverter.CreateMethodCallExpression(
                        methodCallContext,
                        new JST.Expression[] {
                            JST.IdentifierExpression.Create(
                                null,
                                methodConverter.Scope,
                                methodConverter.RuntimeManager.ResolveType(
                                    (TypeReference)resultTypeReference)),
                             enumExpression
                        },
                        methodConverter,
                        methodConverter.RuntimeManager);
                }

                if (!boxedExpression.BoxedExpression.ResultType.IsGenericParameter)
                {
                    // We have boxed operation as well as virtual method call.
                    // This means we can resolve the real method call and avoid boxing
                    // all together.
                    MethodReference overrideMethod = boxedExpression.BoxedExpression.ResultType.GetOverride(
                        virtualRefExpression.MethodReference);

                    // Note that if we couldn't find the override, then it may be that this type is depending on
                    // object's override method. So let's just use it.
                    methodReferenceExpression = new MethodReferenceExpression(
                        methodReferenceExpression.Context,
                        methodReferenceExpression.Location,
                        overrideMethod ?? virtualRefExpression.MethodReference,
                        boxedExpression.BoxedExpression);

                    virtualRefExpression = null;
                }
            }

            List<JST.Expression> argumentExpressions = null;

            if (methodReferenceExpression != null)
            {
                JST.Expression thisExpression = null;
                MethodCallContext methodCallContext;

                if (methodReferenceExpression.LeftExpression != null)
                {
                    // Let's generate static method for the method that we want to call.
                    // Value type methods are all implemented as static methods.
                    if (methodReferenceExpression.MethodReference.Resolve().IsVirtual
                        && methodReferenceExpression.MethodReference.DeclaringType.IsValueType
                        && methodReferenceExpression.LeftExpression is LoadAddressExpression)
                    {
                        thisExpression =
                            ExpressionConverterBase.Convert(
                                methodConverter,
                                ((LoadAddressExpression)methodReferenceExpression.LeftExpression)
                                    .NestedExpression);
                    }
                    else
                    {
                        thisExpression =
                            ExpressionConverterBase.Convert(
                                methodConverter,
                                methodReferenceExpression.LeftExpression);
                    }

                    methodCallContext =
                        new MethodCallContext(
                            thisExpression,
                            methodReferenceExpression.MethodReference,
                            methodReferenceExpression is VirtualMethodReferenceExpression);
                }
                else
                {
                    methodCallContext = new MethodCallContext(
                        methodReferenceExpression.MethodReference,
                        methodReferenceExpression.Location,
                        methodConverter.Scope);
                }

                return MethodCallExpressionConverter.CreateMethodCallExpression(
                    methodCallContext,
                    methodCallExpression.Parameters.Select(
                        exp => ExpressionConverterBase.Convert(methodConverter, exp)).ToArray(),
                    methodConverter,
                    methodConverter.RuntimeManager);
            }

            List<JST.Expression> genericArguments = null;
            GenericInstanceMethod genericMethod =
                methodReferenceExpression != null
                    ? methodReferenceExpression.MethodReference as GenericInstanceMethod
                    : null;

            if (genericMethod != null
                && null == genericMethod.ElementMethod.Resolve()
                    .CustomAttributes.SelectAttribute(
                        methodConverter.KnownReferences.IgnoreGenericArgumentsAttribute))
            {
                // If we are to ignore GenericArguments, let's skip creating
                // arguments for genericArguments.
                genericArguments =
                    genericMethod.GenericArguments.Select(
                        exp => JST.IdentifierExpression.Create(
                            null,
                            methodConverter.Scope,
                            methodConverter.Resolve(exp))).ToList();

                argumentExpressions = new List<JST.Expression>();
                argumentExpressions.AddRange(genericArguments);
            }
            else
            {
                argumentExpressions = new List<JST.Expression>();
            }

            argumentExpressions.AddRange(
                methodCallExpression.Parameters.Select(
                    exp => ExpressionConverterBase.Convert(methodConverter, exp)));

            return new JST.MethodCallExpression(
                methodCallExpression.Location,
                methodConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodConverter,
                    methodCallExpression.MethodReference),
                argumentExpressions);
        }

        /// <summary>
        /// Creates the method call expression.
        /// </summary>
        /// <param name="callContext">The call context.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="runtimeManager">The runtime manager.</param>
        /// <returns>Method call expression.</returns>
        public static JST.Expression CreateMethodCallExpression(
            MethodCallContext callContext,
            IList<JST.Expression> arguments,
            IResolver resolver,
            RuntimeScopeManager runtimeManager)
        {
            MethodReference methodReference = callContext.Method;
            if (methodReference.HasThis && callContext.ThisExpression == null)
            {
                throw new ArgumentNullException("thisExpression should not be null");
            }

            GenericInstanceMethod genericMethod = methodReference as GenericInstanceMethod;
            List<JST.Expression> genericArguments = new List<JST.Expression>();

            if (genericMethod != null
                && genericMethod.ElementMethod.Resolve()
                    .CustomAttributes.SelectAttribute(
                        runtimeManager.Context.KnownReferences.IgnoreGenericArgumentsAttribute) == null)
            {
                // If we are to ignore GenericArguments, let's skip creating
                // arguments for genericArguments.
                genericArguments =
                    genericMethod.GenericArguments.Select(
                        exp => JST.IdentifierExpression.Create(
                            null,
                            callContext.Scope,
                            resolver.Resolve(exp))).ToList();
            }

            MethodDefinition methodDefinition = methodReference.Resolve();
            TypeDefinition declaringTypeDefinition = methodDefinition != null ? methodDefinition.DeclaringType : null;
            bool isExtendedOrPsudo = runtimeManager.Context.IsExtended(declaringTypeDefinition)
                || runtimeManager.Context.IsPsudoType(declaringTypeDefinition);
            if (methodReference.HasThis
                && !(methodReference.DeclaringType.IsValueType
                    && (!isExtendedOrPsudo || runtimeManager.Context.IsImplemented(methodDefinition)))
                && (callContext.IsVirtual
                    || !isExtendedOrPsudo
                    || !runtimeManager.Context.IsImplemented(methodDefinition)
                    || methodReference.Resolve().CustomAttributes.SelectAttribute(
                            runtimeManager.Context.KnownReferences.KeepInstanceUsageAttribute) != null)
                && !runtimeManager.ImplementInstanceAsStatic)
            {
                genericArguments = genericArguments ?? new List<JST.Expression>();

                if (arguments != null)
                {
                    genericArguments.AddRange(arguments);
                }

                // Converter to instance method call.
                return new JST.MethodCallExpression(
                    callContext.Location,
                    callContext.Scope,
                    new JST.IndexExpression(
                        callContext.Location,
                        callContext.Scope,
                        callContext.ThisExpression,
                        callContext.IsVirtual
                            ? resolver.ResolveVirtualMethod(methodReference, callContext.Scope)
                            : new JST.IdentifierExpression(resolver.Resolve(methodReference), callContext.Scope)),
                    genericArguments);
            }
            else
            {
                List<JST.Expression> argList = new List<JST.Expression>();
                if (genericArguments != null)
                {
                    argList.AddRange(genericArguments);
                }

                if (callContext.ThisExpression != null)
                {
                    argList.Add(callContext.ThisExpression);
                }

                if (arguments != null)
                {
                    argList.AddRange(arguments);
                }

                return new JST.MethodCallExpression(
                    callContext.Location,
                    callContext.Scope,
                    JST.IdentifierExpression.Create(
                        callContext.Location,
                        callContext.Scope,
                        resolver.ResolveStaticMember(methodReference)),
                    argList);
            }
        }

        /// <summary>
        /// Converters the special method.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="methodReference">The method reference.</param>
        /// <returns></returns>
        internal static Func<IMethodScopeConverter, MethodCallExpression, JST.Expression> ConverterSpecialMethod(
            ConverterContext converter,
            MethodReference methodReference)
        {
            if (methodReference.DeclaringType.FullName == "System.String")
            {
                if (methodReference.Name == "op_Equality")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.StrictEquals);
                }
                else if (methodReference.Name == "op_Inequality")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.StrictNotEquals);
                }
                else if (methodReference.Name == "Concat")
                {
                    return MethodCallExpressionConverter.StringConcatConverter;
                }
            }
            else if (methodReference.DeclaringType.FullName == "System.DateTime")
            {
                if (methodReference.Name == "op_Equality")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.StrictEquals);
                }
                else if (methodReference.Name == "op_Inequality")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.StrictNotEquals);
                }
                else if (methodReference.Name == "op_GreaterThan")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.GreaterThan);
                }
                else if (methodReference.Name == "op_GreaterThanOrEqual")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.GreaterThanOrEqual);
                }
                else if (methodReference.Name == "op_LessThan")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.LessThan);
                }
                else if (methodReference.Name == "op_LessThanOrEqual")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.LessThanOrEqual);
                }
                else if (methodReference.Name == "op_Subtraction")
                {
                    return (c, e) =>
                        MethodCallExpressionConverter.FuncOperatorToNativeOperator(
                            c,
                            e,
                            JST.BinaryOperator.Minus);
                }
            }
            else if (methodReference.Name == "op_Explicit"
                || methodReference.Name == "op_Implicit")
            {
                string typeName = methodReference.DeclaringType.FullName;

                if (typeName == "System.Collections.ArrayList"
                    || typeName == "System.Number"
                    || typeName == "System.Int64"
                    || typeName == "System.Int32"
                    || typeName == "System.Int16"
                    || typeName == "System.SByte"
                    || typeName == "System.UInt64"
                    || typeName == "System.UInt32"
                    || typeName == "System.UInt16"
                    || typeName == "System.Byte"
                    || typeName == "System.Single")
                {
                    return delegate(
                        IMethodScopeConverter methodConverter,
                        MethodCallExpression methodCall)
                    {
                        return ExpressionConverterBase.Convert(
                            methodConverter,
                            methodCall.Parameters[0]);
                    };
                }
            }
            else if (methodReference.DeclaringType.FullName == "System.Number")
            {
                string methodName = methodReference.Name;
                if (methodName == "ParseInt"
                    || methodName == "ParseFloat"
                    || methodName == "IsNaN"
                    || methodName == "IsFinite")
                {
                    return MethodCallExpressionConverter.HardCodedAlias;
                }
            }

            return null;
        }

        /// <summary>
        /// Literals the converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns></returns>
        private static JST.Expression LiteralConverter(
            IMethodScopeConverter converter,
            MethodCallExpression methodCallExpression)
        {
            return new JST.ScriptLiteralExpression(
                methodCallExpression.Location,
                converter.Scope,
                ((StringLiteral)methodCallExpression.Parameters[0]).String);
        }

        /// <summary>
        /// Strings the concat converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns>BinaryExpression that will do concat.</returns>
        private static JST.Expression StringConcatConverter(
            IMethodScopeConverter converter,
            MethodCallExpression methodCallExpression)
        {
            IList<Expression> strParts;

            if (methodCallExpression.Parameters.Count == 1
                && methodCallExpression.Parameters[0] is InlineArrayInitialization)
            {
                strParts = ((InlineArrayInitialization)methodCallExpression.Parameters[0]).ElementInitValues;
            }
            else
            {
                strParts = methodCallExpression.Parameters;
            }

            Location location = methodCallExpression.Location;

            // Only emit plus expression if and only if all the elements are string.
            if (strParts.Count > 0
                && strParts.FirstOrDefault(exp => !exp.ResultType.IsSame(converter.ClrKnownReferences.String)) == null)
            {
                JST.Expression returnValue = ExpressionConverterBase.Convert(
                    converter,
                    strParts[0]);

                for (int iParam = 1; iParam < strParts.Count; iParam++)
                {
                    returnValue = new JST.BinaryExpression(
                        location,
                        converter.Scope,
                        JST.BinaryOperator.Plus,
                        returnValue,
                        ExpressionConverterBase.Convert(
                            converter,
                            strParts[iParam]));
                }

                return returnValue;
            }
            else
            {
                return MethodCallExpressionConverter.ConvertInternal(
                    converter,
                    methodCallExpression);
            }
        }

        /// <summary>
        /// Funcs the operator to native operator.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <param name="op">The op.</param>
        /// <returns>Change function operator to native operator.</returns>
        private static JST.Expression FuncOperatorToNativeOperator(
            IMethodScopeConverter converter,
            MethodCallExpression methodCallExpression,
            JST.BinaryOperator op)
        {
            return new JST.BinaryExpression(
                methodCallExpression.Location,
                converter.Scope,
                op,
                ExpressionConverterBase.Convert(
                    converter,
                    methodCallExpression.Parameters[0]),
                ExpressionConverterBase.Convert(
                    converter,
                    methodCallExpression.Parameters[1]));
        }

        /// <summary>
        /// Fields the getter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="methodCall">The method call.</param>
        /// <returns>Field getter expression.</returns>
        private static JST.Expression FieldGetter(
            IMethodScopeConverter converter,
            MethodCallExpression methodCall)
        {
            return new JST.IndexExpression(
                methodCall.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(
                    converter,
                    methodCall.Parameters[0]),
                ExpressionConverterBase.Convert(
                    converter,
                    methodCall.Parameters[1]));
        }

        /// <summary>
        /// Hards the coded alias.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="methodCall">The method call.</param>
        /// <returns>Return expression with hardCoded alias.</returns>
        private static JST.Expression HardCodedAlias(
            IMethodScopeConverter converter,
            MethodCallExpression methodCall)
        {
            if (methodCall == null || converter == null)
            {
                throw new InvalidProgramException("Check the implementation of compiler.");
            }

            List<JST.Expression> arguments = new List<JST.Expression>();

            for (int paramIndex = 0; paramIndex < methodCall.Parameters.Count; paramIndex++)
            {
                arguments.Add(
                    ExpressionConverterBase.Convert(
                        converter,
                        methodCall.Parameters[paramIndex]));
            }

            string methodName =
                ((MethodReferenceExpression)methodCall.MethodReference).MethodReference.Name;

            methodName = char.ToLower(methodName[0]) + methodName.Substring(1);

            return new JST.MethodCallExpression(
                methodCall.Location,
                converter.Scope,
                JST.IdentifierExpression.Create(
                    null,
                    converter.Scope,
                    converter.RuntimeManager.ResolveScriptAlias(
                        methodName)),
                arguments);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="methodCall">The method call.</param>
        /// <returns>Return new object expression.</returns>
        private static JST.Expression CreateInstance(
            IMethodScopeConverter converter,
            MethodCallExpression methodCall)
        {
            return new JST.NewObjectExpression(
                methodCall.Location,
                converter.Scope,
                ExpressionConverterBase.Convert(
                    converter,
                    methodCall.Parameters[0]));
        }
    }
}