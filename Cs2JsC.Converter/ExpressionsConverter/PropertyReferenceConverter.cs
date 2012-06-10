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
            MethodConverter converter,
            PropertyReferenceExpression expression)
        {
            List<JST.Expression> arguments = new List<JST.Expression>();
            bool isIntrinsic;
            JST.Expression returnValue =
                PropertyReferenceConverter.Convert(
                    converter,
                    expression,
                    arguments,
                    true,
                    out isIntrinsic);

            if (!isIntrinsic)
            {
                returnValue = new JST.MethodCallExpression(
                        expression.Location,
                        converter.Scope,
                        returnValue,
                        arguments);
            }

            return returnValue;
        }

        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="isReader">if set to <c>true</c> [is reader].</param>
        /// <param name="isIntrinsic">if set to <c>true</c> [is intrinsic].</param>
        /// <returns>Expression for property access.</returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            PropertyReferenceExpression expression,
            List<JST.Expression> arguments,
            bool isReader,
            out bool isIntrinsic)
        {
            isIntrinsic = converter.RuntimeManager.Context.IsIntrinsicProperty(expression.PropertyReference.Resolve());
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
                    PropertyDefinition propertyDefinition = expression.PropertyReference.Resolve();
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
                MethodReference accessorMethod =
                            isReader
                                ? expression.PropertyReference.Resolve().GetMethod.FixGenericArguments(expression.PropertyReference.DeclaringType)
                                : expression.PropertyReference.Resolve().SetMethod.FixGenericArguments(expression.PropertyReference.DeclaringType);

                MemberReferenceConverter.FixMethodReference<MethodReference>(converter.RuntimeManager.Context, ref accessorMethod);

                if (!accessorMethod.Resolve().IsVirtual
                    && accessorMethod.Resolve().DeclaringType.IsValueType)
                {
                    returnValue =
                        JST.IdentifierExpression.Create(
                            expression.Location,
                            converter.Scope,
                            converter.ResolveStaticMember(accessorMethod));

                    arguments.Add(
                        ExpressionConverterBase.Convert(
                            converter,
                            expression.LeftExpression));
                }
                else
                {
                    returnValue =
                            converter.ResolveVirtualMethod(
                                accessorMethod,
                                converter.Scope);

                    returnValue = new JST.IndexExpression(
                        expression.Location,
                        converter.Scope,
                        ExpressionConverterBase.Convert(
                            converter,
                            expression.LeftExpression),
                        returnValue);
                }
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
            else if (expression.Arguments.Count > 0)
            {
                arguments.AddRange(
                    expression.Arguments.Select(
                        t => ExpressionConverterBase.Convert(converter, t)));
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