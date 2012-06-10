//-----------------------------------------------------------------------
// <copyright file="TypeCheckConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TypeCheckConverter
    /// </summary>
    public static class TypeCheckConverter
    {
        /// <summary>
        /// Converts the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>Converted JST expression.</returns>
        public static JST.Expression Convert(
            MethodConverter converter,
            TypeCheckExpression expression)
        {
            JST.Expression nestedExpression =
                ExpressionConverterBase.Convert(
                    converter,
                    expression.Expression);

            JST.Expression typeReferenceExpression = null;
            // There are cases when casting from Delegate to concrete delegate
            // we end up creating types for delegates. To avoid this let's check
            // if we are casting from delegate to concrete delegate or not.
            // if so use short cut.
            if (expression.Type.IsDelegate())
            {
                if (expression.Expression.ResultType.IsDelegate())
                {
                    switch (expression.CheckType)
                    {
                        case TypeCheckType.IsType:
                            return new JST.BooleanLiteralExpression(converter.Scope, true);
                        case TypeCheckType.CastType:
                        case TypeCheckType.AsType:
                            return nestedExpression;
                        default:
                            throw new InvalidProgramException();
                    }
                }

                typeReferenceExpression =
                    JST.IdentifierExpression.Create(
                        nestedExpression.Location,
                        converter.Scope,
                        converter.Resolve(converter.ClrKnownReferences.FunctionType));
            }
            else
            {
                typeReferenceExpression =
                    JST.IdentifierExpression.Create(
                        nestedExpression.Location,
                        converter.Scope,
                        converter.Resolve(expression.Type));
            }

            MethodReference methodReference;

            switch (expression.CheckType)
            {
                case TypeCheckType.IsType:
                    methodReference = converter.KnownReferences.IsInstanceOfMethod;
                    break;
                case TypeCheckType.CastType:
                    if (BinaryExpressionConverter.IsIntBased(expression.ResultType)
                        && BinaryExpressionConverter.IsFloatBased(expression.Expression.ResultType))
                    {
                        return new JST.BinaryExpression(
                            nestedExpression.Location,
                            nestedExpression.Scope,
                            JST.BinaryOperator.BitwiseOr,
                            nestedExpression,
                            new JST.NumberLiteralExpression(converter.Scope, 0));
                    }
                    else if (!TypeCheckConverter.RequireCast(
                            converter.RuntimeManager.Context,
                            expression.ResultType,
                            expression.Expression.ResultType))
                    {
                        return nestedExpression;
                    }

                    methodReference = converter.KnownReferences.CastTypeMethod;
                    break;
                case TypeCheckType.AsType:
                    methodReference = converter.KnownReferences.AsTypeMethod;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    typeReferenceExpression,
                    methodReference,
                    false),
                new JST.Expression[] { nestedExpression },
                converter,
                converter.RuntimeManager);
        }

        private static bool RequireCast(
            ConverterContext context,
            TypeReference t1,
            TypeReference t2)
        {
            if ((t1.IsDouble() || t1.IsIntegerOrEnum())
                && t2.IsIntegerOrEnum())
            {
                return false;
            }

            TypeDefinition tr1 = t1.Resolve();
            if (tr1 != null
                && tr1.BaseType != null
                && tr1.BaseType.IsSameDefinition(context.ClrKnownReferences.MulticastDelegate))
            {
                return false;
            }

            if (tr1.IsSameDefinition(context.ClrKnownReferences.Object))
            {
                return false;
            }

            return true;
        }
    }
}