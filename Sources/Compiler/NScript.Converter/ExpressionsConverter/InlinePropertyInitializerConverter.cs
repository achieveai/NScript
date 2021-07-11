//-----------------------------------------------------------------------
// <copyright file="InlinePropertyInitializerConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR.AST;
    using NScript.Converter.StatementsConverter;
    using NScript.Converter.TypeSystemConverter;
    using Mono.Cecil;
    using System.Collections.Generic;
    using System;
    using System.Linq;

    /// <summary>
    /// Definition for InlinePropertyInitializerConverter
    /// </summary>
    public static class InlinePropertyInitializerConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>JST expression</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            InlinePropertyInitilizationExpression expression)
        {
            // initialize a varible that will be used to hold object for initialization.
            var variable = converter.GetTempVariable();

            var expressions = new List<JST.Expression>();
            JST.InlineObjectInitializer inlineObjectInitializer = null;
            JST.Expression initExpression;

            if (CanUseNativeStringDictionary(converter, expression))
            {
                var rv = ConvertStringDictionary(converter, expression);
                if (rv != null)
                {
                    return rv;
                }
            }

            if (!converter.RuntimeManager.Context.IsJsonType(expression.Constructor.ResultType.Resolve()))
            {
                initExpression = ExpressionConverterBase.Convert(converter, expression.Constructor);
            }
            else
            {
                initExpression = inlineObjectInitializer = new JST.InlineObjectInitializer(expression.Location, converter.Scope);
            }

            // Add the constructor statement.
            expressions.Add(
                new JST.BinaryExpression(
                    initExpression.Location,
                    converter.Scope,
                    JST.BinaryOperator.Assignment,
                    new JST.IdentifierExpression(variable, converter.Scope),
                    initExpression));

            foreach (var setter in expression.Setters)
            {
                JST.Expression assignmentExpression = null;
                var thisVariable = new JST.IdentifierExpression(variable, converter.Scope);

                if (setter.Item1 is MethodReferenceExpression)
                {
                    var methodCallContext = new MethodCallContext(
                            thisVariable,
                            (MethodReference)setter.Item1.MemberReference,
                            false);

                    var args = new List<JST.Expression>();
                    foreach (var arg in setter.Item2)
                    {
                        args.Add(ExpressionConverterBase.Convert(converter, arg));
                    }

                    assignmentExpression = MethodCallExpressionConverter.CreateMethodCallExpression(
                        methodCallContext,
                        args,
                        converter,
                        converter.RuntimeManager);
                }
                else
                {
                    var valueExpression =
                            ExpressionConverterBase.Convert(converter, setter.Item2[0]);

                    if (setter.Item1 is FieldReferenceExpression)
                    {
                        if (inlineObjectInitializer != null)
                        {
                            inlineObjectInitializer.AddInitializer(
                                    converter.Resolve((FieldReference)setter.Item1.MemberReference),
                                    valueExpression);
                        }
                        else
                        {
                            var fieldAccessExpression =
                                new JST.IndexExpression(
                                    setter.Item2[0].Location,
                                    converter.Scope,
                                    thisVariable,
                                    new JST.IdentifierExpression(
                                        converter.Resolve(
                                            (FieldReference)setter.Item1.MemberReference), converter.Scope));

                            assignmentExpression = new JST.BinaryExpression(
                                setter.Item2[0].Location,
                                converter.Scope,
                                JST.BinaryOperator.Assignment,
                                fieldAccessExpression,
                                valueExpression);
                        }
                    }
                    else
                    {
                        var propertyReference = (PropertyReference)setter.Item1.MemberReference;

                        if (converter.RuntimeManager.Context.IsIntrinsicProperty(propertyReference.Resolve())
                            && (converter.RuntimeManager.Context.GetTypeKind(propertyReference.DeclaringType.Resolve())
                                & (ConverterContext.TypeKind.Imported | ConverterContext.TypeKind.JSONType)) != 0)
                        {
                            if (converter.RuntimeManager.Context.IsWrappedType(propertyReference.PropertyType))
                            {
                                var arrayConstruction = valueExpression as JST.MethodCallExpression;
                                if (propertyReference.PropertyType.IsArray
                                    && setter.Item2[0] is InlineArrayInitialization
                                    && arrayConstruction != null
                                    && arrayConstruction.Arguments.Count == 1
                                    && (arrayConstruction.Arguments[0] is JST.InlineNewArrayInitialization
                                        || arrayConstruction.Arguments[0] is JST.NewArrayExpression))
                                {
                                    valueExpression = arrayConstruction.Arguments[0];
                                }
                                else
                                {
                                    valueExpression = MethodConverter.GenerateExtrationExpression(
                                        propertyReference.PropertyType,
                                        valueExpression,
                                        converter,
                                        converter.Scope);
                                }
                            }

                            if (inlineObjectInitializer != null)
                            {
                                inlineObjectInitializer.AddInitializer(
                                        converter.Resolve(propertyReference),
                                        valueExpression);
                            }
                            else
                            {
                                var fieldAccessExpression =
                                    new JST.IndexExpression(
                                        setter.Item2[0].Location,
                                        converter.Scope,
                                        thisVariable,
                                        new JST.IdentifierExpression(
                                            converter.Resolve(propertyReference),
                                            converter.Scope));

                                assignmentExpression = new JST.BinaryExpression(
                                    setter.Item2[0].Location,
                                    converter.Scope,
                                    JST.BinaryOperator.Assignment,
                                    fieldAccessExpression,
                                    valueExpression);
                            }
                        }
                        else
                        {
                            var methodCallContext = new MethodCallContext(
                                thisVariable,
                                propertyReference.Resolve().SetMethod,
                                false);

                            var args = new List<JST.Expression>();
                            foreach (var arg in setter.Item2)
                            {
                                args.Add(ExpressionConverterBase.Convert(converter, arg));
                            }

                            assignmentExpression = MethodCallExpressionConverter.CreateMethodCallExpression(
                                methodCallContext,
                                args,
                                converter,
                                converter.RuntimeManager);
                        }
                    }
                }

                if (assignmentExpression != null)
                {
                    expressions.Add(assignmentExpression);
                }
            }

            if (expressions.Count > 1 || inlineObjectInitializer == null)
            {
                expressions.Add(new JST.IdentifierExpression(variable, converter.Scope));
                return new JST.ExpressionsList(expression.Location, converter.Scope, expressions);
            }
            else
            {
                return inlineObjectInitializer;
            }
        }

        private static bool CanUseNativeStringDictionary(
            IMethodScopeConverter converter,
            InlinePropertyInitilizationExpression expr)
        {
            return InlineCollectionInitializerConverter.IsStringDictionary(converter, expr.Constructor.ResultType);
        }

        private static JST.Expression ConvertStringDictionary(
            IMethodScopeConverter converter,
            InlinePropertyInitilizationExpression expression)
        {
            var nativeDictionaryConstructor = InlineCollectionInitializerConverter.GetNativeDictionaryConstructor(
                converter,
                expression.ResultType);

            if (nativeDictionaryConstructor != null)
            {
                return ConverterUsingNativeDictionary(
                    converter,
                    nativeDictionaryConstructor,
                    expression.Setters,
                    expression.Location);
            }

            return null;
        }

        private static JST.Expression ConverterUsingNativeDictionary(
            IMethodScopeConverter converter,
            MethodReference constructor,
            IList<Tuple<MemberReferenceExpression, Expression[]>> setters,
            Utils.Location location)
        {
            if (setters
                .Any(setter => !(setter.Item2[0] is StringLiteral)))
            { return null; }

            var objExpression = new JST.InlineObjectInitializer(
                location,
                converter.Scope);

            foreach (var setter in setters)
            {
                objExpression.AddInitializer(
                    (setter.Item2[0] as StringLiteral).String,
                    ExpressionConverterBase.Convert(converter, setter.Item2[1]));
            }

            return new JST.MethodCallExpression(
                location,
                converter.Scope,
                JST.IdentifierExpression.Create(
                    location,
                    converter.Scope,
                    converter.ResolveFactory(constructor)),
                objExpression);
        }
    }
}
