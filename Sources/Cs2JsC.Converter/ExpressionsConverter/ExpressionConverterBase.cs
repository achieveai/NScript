//-----------------------------------------------------------------------
// <copyright file="ExpressionConverterBase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for ExpressionConverterBase
    /// </summary>
    public static class ExpressionConverterBase
    {
        /// <summary>
        /// Mapping of all the convertible classes to converter functions.
        /// </summary>
        private static Dictionary<Type, Func<IMethodScopeConverter, Expression, JST.Expression>> typeMappings =
            ExpressionConverterBase.CreateExpressionConverterMapping();

        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="clrExpression">The CLR statement.</param>
        /// <returns>JST.Experssion from provided clrExpression</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter methodConverter,
            Expression clrExpression)
        {
            if (clrExpression == null)
            {
                return null;
            }

            JST.Expression rv = methodConverter.GetReplacementExpression(
                clrExpression);

            if (rv != null)
            {
                return rv;
            }

            Type expressionType = clrExpression.GetType();
            Func<IMethodScopeConverter, Expression, JST.Expression> processor;

            if (!ExpressionConverterBase.typeMappings.TryGetValue(expressionType, out processor))
            {
                throw new NotImplementedException(expressionType.Name);
            }

            return processor(methodConverter, clrExpression);
        }

        /// <summary>
        /// Creates the statement converter mapping.
        /// </summary>
        /// <returns>Mapping of type to converter function.</returns>
        private static Dictionary<Type, Func<IMethodScopeConverter, Expression, JST.Expression>> CreateExpressionConverterMapping()
        {
            Dictionary<Type, Func<IMethodScopeConverter, Expression, JST.Expression>> returnValue =
                new Dictionary<Type, Func<IMethodScopeConverter, Expression, JST.Expression>>();

            returnValue.Add(
                typeof(AnonymousMethodBodyExpression),
                ExpressionConverterBase.SimplifyConverter<AnonymousMethodBodyExpression>(
                    AnonymousMethodBodyExpressionConverter.Convert));

            returnValue.Add(
                typeof(IntLiteral),
                ExpressionConverterBase.SimplifyConverter<IntLiteral>(
                    LiteralExpressionConverter.ConvertIntLiteral));

            returnValue.Add(
                typeof(UIntLiteral),
                ExpressionConverterBase.SimplifyConverter<UIntLiteral>(
                    LiteralExpressionConverter.ConvertUIntLiteral));

            returnValue.Add(
                typeof(DoubleLiteral),
                ExpressionConverterBase.SimplifyConverter<DoubleLiteral>(
                    LiteralExpressionConverter.ConvertDoubleLiteral));

            returnValue.Add(
                typeof(StringLiteral),
                ExpressionConverterBase.SimplifyConverter<StringLiteral>(
                    LiteralExpressionConverter.ConvertStringLiteral));

            returnValue.Add(
                typeof(NullLiteral),
                ExpressionConverterBase.SimplifyConverter<NullLiteral>(
                    LiteralExpressionConverter.ConvertNullLiteral));

            returnValue.Add(
                typeof(BooleanLiteral),
                ExpressionConverterBase.SimplifyConverter<BooleanLiteral>(
                    LiteralExpressionConverter.ConvertBooleanLiteral));

            returnValue.Add(
                typeof(CharLiteral),
                ExpressionConverterBase.SimplifyConverter<CharLiteral>(
                    LiteralExpressionConverter.ConvertCharLiteral));

            returnValue.Add(
                typeof(BinaryExpression),
                ExpressionConverterBase.SimplifyConverter<BinaryExpression>(
                    BinaryExpressionConverter.Convert));

            returnValue.Add(
                typeof(BaseVariableReference),
                ExpressionConverterBase.SimplifyConverter<BaseVariableReference>(
                    VariableReferenceConverter.Convert));

            returnValue.Add(
                typeof(VariableReference),
                ExpressionConverterBase.SimplifyConverter<VariableReference>(
                    VariableReferenceConverter.Convert));

            returnValue.Add(
                typeof(MethodReferenceExpression),
                ExpressionConverterBase.SimplifyConverter<MethodReferenceExpression>(
                    MemberReferenceConverter.Convert));

            returnValue.Add(
                typeof(FieldReferenceExpression),
                ExpressionConverterBase.SimplifyConverter<FieldReferenceExpression>(
                    MemberReferenceConverter.Convert));

            returnValue.Add(
                typeof(VirtualMethodReferenceExpression),
                ExpressionConverterBase.SimplifyConverter<VirtualMethodReferenceExpression>(
                    MemberReferenceConverter.Convert));

            returnValue.Add(
                typeof(MethodCallExpression),
                ExpressionConverterBase.SimplifyConverter<MethodCallExpression>(
                    MethodCallExpressionConverter.Convert));

            returnValue.Add(
                typeof(NewObjectExpression),
                ExpressionConverterBase.SimplifyConverter<NewObjectExpression>(
                    NewObjectConverter.Convert));

            returnValue.Add(
                typeof(NewArrayExpression),
                ExpressionConverterBase.SimplifyConverter<NewArrayExpression>(
                    NewArrayConverter.Convert));

            returnValue.Add(
                typeof(ArrayElementExpression),
                ExpressionConverterBase.SimplifyConverter<ArrayElementExpression>(
                    NewArrayConverter.ConvertArrayElementLoad));

            returnValue.Add(
                typeof(TypeCheckExpression),
                ExpressionConverterBase.SimplifyConverter<TypeCheckExpression>(
                    TypeCheckConverter.Convert));

            returnValue.Add(
                typeof(UnaryExpression),
                ExpressionConverterBase.SimplifyConverter<UnaryExpression>(
                    UnaryExpressionConverter.Convert));

            returnValue.Add(
                typeof(ConditionalOperatorExpression),
                ExpressionConverterBase.SimplifyConverter<ConditionalOperatorExpression>(
                    ConditionalOperatorConverter.Convert));

            returnValue.Add(
                typeof (BoxExpression),
                ExpressionConverterBase.SimplifyConverter<BoxExpression>(
                    BoxExpressionConvereter.Convert));

            returnValue.Add(
                typeof (UnboxExpression),
                ExpressionConverterBase.SimplifyConverter<UnboxExpression>(
                    BoxExpressionConvereter.Convert));

            returnValue.Add(
                typeof (DefaultValueExpression),
                ExpressionConverterBase.SimplifyConverter<DefaultValueExpression>(
                    DefaultValueConverter.Convert));

            returnValue.Add(
                typeof (InlineArrayInitialization),
                ExpressionConverterBase.SimplifyConverter<InlineArrayInitialization>(
                    InlineNewObjectArrayConverter.Convert));

            returnValue.Add(
                typeof (InlinePropertyInitilizationExpression),
                ExpressionConverterBase.SimplifyConverter<InlinePropertyInitilizationExpression>(
                    InlinePropertyInitializerConverter.Convert));

            returnValue.Add(
                typeof (VariableAddressReference),
                ExpressionConverterBase.SimplifyConverter<VariableAddressReference>(
                    VariableAddressReferenceConverter.Convert));

            returnValue.Add(
                typeof (PropertyReferenceExpression),
                ExpressionConverterBase.SimplifyConverter<PropertyReferenceExpression>(
                    PropertyReferenceConverter.Convert));

            returnValue.Add(
                typeof (LoadAddressExpression),
                ExpressionConverterBase.SimplifyConverter<LoadAddressExpression>(
                    LoadAddressExpressionConverter.Convert));

            returnValue.Add(
                typeof (DelegateMethodExpression),
                ExpressionConverterBase.SimplifyConverter<DelegateMethodExpression>(
                    DelegateMethodConverter.Convert));

            returnValue.Add(
                typeof (NullConditional),
                ExpressionConverterBase.SimplifyConverter<NullConditional>(
                    NullConditionalConverter.Convert));

            returnValue.Add(
                typeof (InlineDelegateExpression),
                ExpressionConverterBase.SimplifyConverter<InlineDelegateExpression>(
                    InlineDelegateExpressionConverter.Convert));

            returnValue.Add(
                typeof (TypeofExpression),
                ExpressionConverterBase.SimplifyConverter<TypeofExpression>(
                    TypeofExpressionConverter.Convert));

            returnValue.Add(
                typeof (ToNullable),
                ExpressionConverterBase.SimplifyConverter<ToNullable>(
                    NullableConverter.ConvertTo));

            returnValue.Add(
                typeof (FromNullable),
                ExpressionConverterBase.SimplifyConverter<FromNullable>(
                    NullableConverter.ConvertFrom));

            return returnValue;
        }

        /// <summary>
        /// Simplifies the converter.
        /// </summary>
        /// <typeparam name="T">Expression sub class.</typeparam>
        /// <param name="converter">The converter.</param>
        /// <returns>Function that will convert Expression to JST.Expression.</returns>
        private static Func<IMethodScopeConverter, Expression, JST.Expression> SimplifyConverter<T>(
            Func<IMethodScopeConverter, T, JST.Expression> converter) where T: Expression
        {
            return delegate(IMethodScopeConverter mc, Expression statement)
            {
                return converter(mc, (T)statement);
            };
        }
    }
}
