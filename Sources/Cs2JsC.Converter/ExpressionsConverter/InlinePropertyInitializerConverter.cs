//-----------------------------------------------------------------------
// <copyright file="InlinePropertyInitializerConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.StatementsConverter;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.JST;
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
            // we are creating inline function for this initialization.
            var newScope =
                new IdentifierScope(converter.Scope);

            var function = new JST.FunctionExpression(
                expression.Location,
                converter.Scope,
                newScope, 
                new Identifier[0],
                null);

            // initialize a varible that will be used to hold object for initialization.
            var variable = converter.GetTempVariable();

            List<JST.Expression> expressions = new List<JST.Expression>();
            JST.Expression initExpression = ExpressionConverterBase.Convert(converter, expression.Constructor);

            // Add the constructor statement.
            expressions.Add(
                    new JST.BinaryExpression(
                        initExpression.Location,
                        function.Scope,
                        JST.BinaryOperator.Assignment,
                        new JST.IdentifierExpression(variable),
                        initExpression));

            foreach (var setter in expression.Setters)
            {
                JST.Expression assignmentExpression;
                var thisVariable = new JST.IdentifierExpression(variable);

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
                        var fieldAccessExpression =
                            new JST.IndexExpression(
                                setter.Item2[0].Location,
                                function.Scope,
                                thisVariable,
                                new JST.IdentifierExpression(
                                    converter.Resolve(
                                        (FieldReference)setter.Item1.MemberReference)));

                        assignmentExpression = new JST.BinaryExpression(
                            setter.Item2[0].Location,
                            function.Scope,
                            JST.BinaryOperator.Assignment,
                            fieldAccessExpression,
                            valueExpression);
                    }
                    else
                    {
                        PropertyReference propertyReference = (PropertyReference)setter.Item1.MemberReference;

                        if (converter.RuntimeManager.Context.IsIntrinsicProperty(propertyReference.Resolve()))
                        {
                            var fieldAccessExpression =
                                new JST.IndexExpression(
                                    setter.Item2[0].Location,
                                    function.Scope,
                                    thisVariable,
                                    new JST.IdentifierExpression(converter.Resolve(propertyReference)));

                            assignmentExpression = new JST.BinaryExpression(
                                setter.Item2[0].Location,
                                function.Scope,
                                JST.BinaryOperator.Assignment,
                                fieldAccessExpression,
                                valueExpression);
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

                expressions.Add(assignmentExpression);
            }

            expressions.Add(new JST.IdentifierExpression(variable));
            JST.ExpressionsList expressionList = new ExpressionsList(expression.Location, converter.Scope, expressions);

            return expressionList;
        }
    }
}
