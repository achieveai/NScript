//-----------------------------------------------------------------------
// <copyright file="PropertyReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using Mono.Cecil;

    /// <summary>
    /// Definition for PropertyReferenceConverter
    /// </summary>
    public static class EventReferenceConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter methodScopeConverter,
            EventReferenceExpression expression)
        {
            return ExpressionConverterBase.Convert(
                methodScopeConverter,
                EventReferenceConverter.GetUnderlyingFieldReferenceExpression(expression));
        }

        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            EventReferenceExpression expression,
            Expression valueExpression,
            BinaryOperator binaryOperator)
        {
            bool? addOn = null;
            switch (binaryOperator)
            {
                case BinaryOperator.PlusAssignment:
                    addOn = true;
                    break;
                case BinaryOperator.MinusAssignment:
                    addOn = false;
                    break;
                case BinaryOperator.Assignment:
                    return BinaryExpressionConverter.Convert(
                        converter,
                        new BinaryExpression(
                            expression.Context,
                            expression.Location,
                            EventReferenceConverter.GetUnderlyingFieldReferenceExpression(expression),
                            valueExpression,
                            BinaryOperator.Assignment));
                default:
                    throw new InvalidOperationException("Event Expresison has " + binaryOperator + "expression");
            }

            return Convert(
                converter,
                expression,
                ExpressionConverterBase.Convert(converter, valueExpression),
                addOn.Value);
        }

        private static FieldReferenceExpression GetUnderlyingFieldReferenceExpression(EventReferenceExpression expression)
        {
            var eventReference = expression.EventReference;
            var fieldReference = new FieldReference(
                eventReference.Name,
                eventReference.EventType,
                eventReference.DeclaringType);

            return expression.LeftExpression != null
                ? new FieldReferenceExpression(
                    expression.Context,
                    expression.Location,
                    fieldReference,
                    expression.LeftExpression)
                : new FieldReferenceExpression(
                    expression.Context,
                    expression.Location,
                    fieldReference);
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="converter">  The converter. </param>
        /// <param name="expression"> The expression. </param>
        /// <param name="value">      The arguments. </param>
        /// <returns>
        /// Expression for property access.
        /// </returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            EventReferenceExpression expression,
            JST.Expression value,
            bool addOn)
        {
            var propertyDefinition = expression.EventReference.Resolve();
            MemberReferenceConverter.FixMethodReference(converter.RuntimeManager.Context, ref propertyDefinition);

            var arguments = new List<JST.Expression> { value };

            MethodCallContext callContext = null;
            var methodDefinition = (addOn ? propertyDefinition.AddMethod : propertyDefinition.RemoveMethod);
            var methodReference = methodDefinition.FixGenericTypeArguments(expression.EventReference.DeclaringType);
            MemberReferenceConverter.FixMethodReference(converter.RuntimeManager.Context, ref methodReference);

            if (expression.LeftExpression == null)
            {
                callContext = new MethodCallContext(
                    methodReference,
                    expression.Location,
                    converter.Scope);
            }
            else
            {
                callContext = new MethodCallContext(
                     ExpressionConverterBase.Convert(
                        converter,
                        expression.LeftExpression),
                    methodReference,
                    methodDefinition.IsVirtual
                    && !methodDefinition.IsFinal
                    && !(expression.LeftExpression is BaseVariableReference));
            }

            return MethodCallExpressionConverter.CreateMethodCallExpression(
                callContext,
                arguments,
                converter,
                converter.RuntimeManager);
        }
    }
}