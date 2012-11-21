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
    using NScript.JST;
    using Mono.Cecil;
    using System.Collections.Generic;

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

            List<JST.Expression> expressions = new List<JST.Expression>();
            JST.InlineObjectInitializer inlineObjectInitializer = null;
            JST.Expression initExpression = null;

            if (!converter.RuntimeManager.Context.IsJsonType(expression.Constructor.ResultType.Resolve()))
            {
                initExpression = ExpressionConverterBase.Convert(converter, expression.Constructor);
            }
            else
            {
                initExpression = inlineObjectInitializer = new InlineObjectInitializer(expression.Location, converter.Scope);
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
                    MethodCallContext methodCallContext = new MethodCallContext(
                            thisVariable,
                            (MethodReference)setter.Item1.MemberReference,
                            false);

                    List<JST.Expression> args = new List<JST.Expression>();
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
                    JST.Expression valueExpression =
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
                        PropertyReference propertyReference = (PropertyReference)setter.Item1.MemberReference;

                        if (converter.RuntimeManager.Context.IsIntrinsicProperty(propertyReference.Resolve())
                            || converter.RuntimeManager.Context.IsWrappedType(propertyReference.PropertyType))
                        {
                            if (converter.RuntimeManager.Context.IsWrappedType(propertyReference.PropertyType))
                            {
                                valueExpression = MethodConverter.GenerateExtrationExpression(
                                    propertyReference.PropertyType,
                                    valueExpression,
                                    converter,
                                    converter.Scope);
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
                            MethodCallContext methodCallContext = new MethodCallContext(
                                thisVariable,
                                propertyReference.Resolve().SetMethod,
                                false);

                            List<JST.Expression> args = new List<JST.Expression>();
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
                return new ExpressionsList(expression.Location, converter.Scope, expressions);
            }
            else
            {
                return inlineObjectInitializer;
            }
        }
    }
}
