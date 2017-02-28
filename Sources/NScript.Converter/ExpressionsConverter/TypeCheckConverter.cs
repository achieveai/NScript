//-----------------------------------------------------------------------
// <copyright file="TypeCheckConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using System;
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
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
            IMethodScopeConverter converter,
            TypeCheckExpression expression)
        {
            JST.Expression nestedExpression =
                ExpressionConverterBase.Convert(
                    converter,
                    expression.Expression);

            TypeReference innerUnderlyingType = TypeCheckConverter.GetUnderlyingType(
                converter,
                expression.Expression);

            if (expression.CheckType == TypeCheckType.CastType
                && innerUnderlyingType.Resolve().IsSameDefinition(converter.ClrKnownReferences.NullableType)
                && ((GenericInstanceType)innerUnderlyingType).GenericArguments[0].IsSame(expression.ResultType))
            {
                return MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(
                        nestedExpression,
                        converter.KnownReferences.NullableValuePropertyGetter(expression.ResultType),
                        false),
                    new JST.Expression[] { },
                    converter,
                    converter.RuntimeManager);
            }

            if (expression.CheckType == TypeCheckType.CastType
                && expression.ResultType.Resolve().IsSameDefinition(converter.ClrKnownReferences.NullableType))
            {
                if (TypeCheckConverter.ReferenceEquals(
                    ((GenericInstanceType)expression.ResultType).GenericArguments[0],
                    expression.Expression.ResultType))
                {
                    return nestedExpression;
                }
                else if (!RequireCast(
                    converter.RuntimeManager.Context,
                    expression.ResultType,
                    expression.Expression.ResultType))
                {
                    return nestedExpression;
                }
                else if (expression.Expression.ResultType.IsValueType)
                {
                    throw new NotImplementedException("Don't know how to handle this case");
                }
            }

            JST.Expression typeReferenceExpression = null;
            TypeDefinition expressionTypeDefinition = expression.Type.Resolve();

            // There is no point to check JSON Type or imported type for typeCheck. The reason being
            // type checks do not always work. The problem is that the type would only work for current type
            // and not base types.
            if (expressionTypeDefinition != null
                && (converter.RuntimeManager.Context.IsJsonType(expressionTypeDefinition)
                    || converter.RuntimeManager.Context.IsPsudoType(expressionTypeDefinition)
                    || expressionTypeDefinition.IsSameDefinition(converter.ClrKnownReferences.Object)))
            {
                switch (expression.CheckType)
                {
                    case TypeCheckType.AsType:
                    case TypeCheckType.CastType:
                        return nestedExpression;
                    case TypeCheckType.IsType:
                        return new JST.BooleanLiteralExpression(converter.Scope, true);
                }
            }

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
                    if (!TypeCheckConverter.RequireCast(
                            converter.RuntimeManager.Context,
                            expression.ResultType,
                            innerUnderlyingType))
                    {
                        return new JST.BooleanLiteralExpression(
                            converter.Scope,
                            true);
                    }
                    else
                    {
                        methodReference = converter.KnownReferences.IsInstanceOfMethod;
                    }

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
                            innerUnderlyingType))
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
            if (t1.IsSame(t2))
            {
                return false;
            }

            if ((t1.IsDouble() || t1.IsIntegerOrEnum())
                && t2.IsIntegerOrEnum())
            {
                return false;
            }

            if (t1.IsGenericInstance && t1.Resolve().IsSameDefinition(context.ClrKnownReferences.NullableType))
            {
                return RequireCast(
                    context,
                    ((GenericInstanceType)t1).GenericArguments[0],
                    t2);
            }

            if (t2.IsGenericInstance && t2.Resolve().IsSameDefinition(context.ClrKnownReferences.NullableType))
            {
                return RequireCast(
                    context,
                    t1,
                    ((GenericInstanceType)t2).GenericArguments[0]);
            }

            if (t1.IsArray && t2.Resolve().IsSameDefinition(context.KnownReferences.ArrayImplGeneric)
                || t2.IsArray && t1.Resolve().IsSameDefinition(context.KnownReferences.ArrayImplGeneric))
            {
                TypeReference ct1 = t1.IsArray ? t1.GetElementType() : t2.GetElementType();
                TypeReference ct2 = ((GenericInstanceType)(t1.IsArray ? t2 : t1)).GenericArguments[0];

                return TypeCheckConverter.RequireCast(
                    context,
                    ct1,
                    ct2);
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

        private static TypeReference GetUnderlyingType(
            IMethodScopeConverter converter,
            Expression expression)
        {
            TypeCheckExpression castExpression = expression as TypeCheckExpression;
            if (castExpression != null
                && castExpression.CheckType == TypeCheckType.CastType
                && castExpression.ResultType.IsSameDefinition(converter.ClrKnownReferences.Object))
            {
                return castExpression.Expression.ResultType;
            }

            return expression.ResultType;
        }
    }
}