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
                JST.Expression valueExpression = 
                        ExpressionConverterBase.Convert(converter, setter.Item2);

                if (setter.Item1 is FieldReferenceExpression)
                {
                    var fieldAccessExpression =
                        new JST.IndexExpression(
                            setter.Item2.Location,
                            function.Scope,
                            new JST.IdentifierExpression(variable),
                            new JST.IdentifierExpression(
                                converter.Resolve(
                                    (FieldReference)setter.Item1.MemberReference)));

                    assignmentExpression = new JST.BinaryExpression(
                        setter.Item2.Location,
                        function.Scope,
                        JST.BinaryOperator.Assignment,
                        fieldAccessExpression,
                        valueExpression);
                }
                else
                {
                    PropertyReference propertyReference = (PropertyReference)setter.Item1.MemberReference;

                    var propertySetterAccessExpression =
                        new JST.IndexExpression(
                            setter.Item2.Location,
                            function.Scope,
                            new JST.IdentifierExpression(variable),
                            new JST.IdentifierExpression(
                                converter.Resolve(propertyReference.Resolve().SetMethod)));

                    assignmentExpression = new JST.MethodCallExpression(
                        setter.Item2.Location,
                        function.Scope,
                        propertySetterAccessExpression,
                        new JST.Expression[] {valueExpression});
                }

                expressions.Add(assignmentExpression);
            }

            expressions.Add(new JST.IdentifierExpression(variable));
            JST.ExpressionsList expressionList = new ExpressionsList(expression.Location, converter.Scope, expressions);

            return expressionList;
        }
    }
}
