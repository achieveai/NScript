//-----------------------------------------------------------------------
// <copyright file="PropertyReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Mono.Cecil;

    /// <summary>
    /// Definition for PropertyReferenceConverter
    /// </summary>
    public static class PropertyReferenceConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Expression for Property</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            PropertyReferenceExpression expression)
        {
            JST.Expression returnValue =
                PropertyReferenceConverter.Convert(
                    converter,
                    expression,
                    null);

            return returnValue;
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
            PropertyReferenceExpression expression,
            JST.Expression value)
        {
            bool isIntrinsic = converter.RuntimeManager.Context.IsIntrinsicProperty(expression.PropertyReference.Resolve());
            bool isIndexer = isIntrinsic && expression.Arguments.Count > 0;

            PropertyDefinition propertyDefinition = expression.PropertyReference.Resolve();
            List<JST.Expression> arguments = new List<JST.Expression>();

            if (!isIntrinsic)
            {
                arguments.AddRange(
                    expression.Arguments.Select(
                        t => ExpressionConverterBase.Convert(converter, t)));

                if (value != null)
                {
                    arguments.Add(value);
                }

                MethodCallContext callContext = null;
                var methodDefinition = (value == null ? propertyDefinition.GetMethod : propertyDefinition.SetMethod);
                var methodReference = methodDefinition.FixGenericArguments(expression.PropertyReference.DeclaringType);
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
                        methodDefinition.IsVirtual);
                }

                return MethodCallExpressionConverter.CreateMethodCallExpression(
                    callContext,
                    arguments,
                    converter,
                    converter.RuntimeManager);
            }
            else if (value == null)
            {
                return PropertyReferenceConverter.GetIntrinsicAccessor(
                    converter,
                    expression);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Gets an intrinsic accessor.
        /// </summary>
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
        /// <param name="converter">  The converter. </param>
        /// <param name="expression"> The expression. </param>
        /// <returns>
        /// The intrinsic accessor.
        /// </returns>
        public static JST.Expression GetIntrinsicAccessor(
            IMethodScopeConverter converter,
            PropertyReferenceExpression expression)
        {
            PropertyDefinition propertyDefinition = expression.PropertyReference.Resolve();
            bool isIntrinsic = converter.RuntimeManager.Context.IsIntrinsicProperty(propertyDefinition);
            if (!isIntrinsic)
            {
                return null;
            }

            bool isIndexer = isIntrinsic && expression.Arguments.Count > 0;

            JST.Expression returnValue;
            if (expression.LeftExpression == null)
            {
                if (isIndexer)
                {
                    returnValue =
                        JST.IdentifierExpression.Create(
                            expression.Location,
                            converter.Scope,
                            converter.Resolve(expression.PropertyReference.DeclaringType));
                }
                else
                {
                    MethodReference methodReference =
                        (propertyDefinition.SetMethod ?? propertyDefinition.GetMethod).FixGenericArguments(expression.PropertyReference.DeclaringType);

                    MemberReferenceConverter.FixMethodReference(converter.RuntimeManager.Context, ref methodReference);

                    // This is static method so we only have to return back setter for static method.
                    returnValue =
                        JST.IdentifierExpression.Create(
                            expression.Location,
                            converter.Scope,
                            converter.ResolveStaticMember(methodReference));
                }
            }
            else if (isIndexer)
            {
                // If the expression is indexer expression, we don't need member variable
                // name to do stuff.
                returnValue =
                    ExpressionConverterBase.Convert(
                        converter,
                        expression.LeftExpression);
            }
            else
            {
                returnValue = new JST.IdentifierExpression(
                    converter.Resolve(expression.PropertyReference),
                    converter.Scope);

                returnValue = new JST.IndexExpression(
                    expression.Location,
                    converter.Scope,
                    ExpressionConverterBase.Convert(
                        converter,
                        expression.LeftExpression),
                    returnValue);
            }

            if (isIndexer)
            {
                if (expression.Arguments.Count > 1)
                {
                    throw new NotSupportedException("Intrinsic properties can't have more than 1 property indexes");
                }

                returnValue = new JST.IndexExpression(
                    returnValue.Location,
                    returnValue.Scope,
                    returnValue,
                    ExpressionConverterBase.Convert(converter, expression.Arguments[0]),
                    true);
            }

            return returnValue;
        }

        /// <summary>
        /// Converts to property reference.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="arrayElement">The array element.</param>
        /// <returns>
        /// Property Reference expression point to array accessor property.
        /// </returns>
        public static PropertyReferenceExpression ConvertToPropertyReference(
            ConverterContext context,
            ArrayElementExpression arrayElement)
        {
            PropertyReference propertyReference = context.KnownReferences.ArrayAccessor;

            return new PropertyReferenceExpression(
                context.ClrContext,
                arrayElement.Location,
                propertyReference,
                arrayElement.Array,
                new Expression[] { arrayElement.Index });
        }
    }
}