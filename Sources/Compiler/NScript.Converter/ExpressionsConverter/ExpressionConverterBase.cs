//-----------------------------------------------------------------------
// <copyright file="ExpressionConverterBase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    // using JsCsc.Lib.Serialization;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using System;
    using System.Collections.Generic;

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

            var rv = methodConverter.GetReplacementExpression(
                clrExpression);

            if (rv != null)
            {
                return rv;
            }

            var expressionType = clrExpression.GetType();

            if (!ExpressionConverterBase.typeMappings.TryGetValue(expressionType, out var processor))
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
            var returnValue =
                new Dictionary<Type, Func<IMethodScopeConverter, Expression, JST.Expression>>
                {
                    {
                        typeof(AnonymousMethodBodyExpression),
                        ExpressionConverterBase.SimplifyConverter<AnonymousMethodBodyExpression>(
                            AnonymousMethodBodyExpressionConverter.Convert)
                    },

                    {
                        typeof(IntLiteral),
                        ExpressionConverterBase.SimplifyConverter<IntLiteral>(
                            LiteralExpressionConverter.ConvertIntLiteral)
                    },

                    {
                        typeof(UIntLiteral),
                        ExpressionConverterBase.SimplifyConverter<UIntLiteral>(
                            LiteralExpressionConverter.ConvertUIntLiteral)
                    },

                    {
                        typeof(DoubleLiteral),
                        ExpressionConverterBase.SimplifyConverter<DoubleLiteral>(
                            LiteralExpressionConverter.ConvertDoubleLiteral)
                    },

                    {
                        typeof(StringLiteral),
                        ExpressionConverterBase.SimplifyConverter<StringLiteral>(
                            LiteralExpressionConverter.ConvertStringLiteral)
                    },

                    {
                        typeof(NullLiteral),
                        ExpressionConverterBase.SimplifyConverter<NullLiteral>(
                            LiteralExpressionConverter.ConvertNullLiteral)
                    },

                    {
                        typeof(BooleanLiteral),
                        ExpressionConverterBase.SimplifyConverter<BooleanLiteral>(
                            LiteralExpressionConverter.ConvertBooleanLiteral)
                    },

                    {
                        typeof(CharLiteral),
                        ExpressionConverterBase.SimplifyConverter<CharLiteral>(
                            LiteralExpressionConverter.ConvertCharLiteral)
                    },

                    {
                        typeof(BinaryExpression),
                        ExpressionConverterBase.SimplifyConverter<BinaryExpression>(
                            BinaryExpressionConverter.Convert)
                    },

                    {
                        typeof(BaseVariableReference),
                        ExpressionConverterBase.SimplifyConverter<BaseVariableReference>(
                            VariableReferenceConverter.Convert)
                    },

                    {
                        typeof(VariableReference),
                        ExpressionConverterBase.SimplifyConverter<VariableReference>(
                            VariableReferenceConverter.Convert)
                    },

                    {
                        typeof(MethodReferenceExpression),
                        ExpressionConverterBase.SimplifyConverter<MethodReferenceExpression>(
                            MemberReferenceConverter.Convert)
                    },

                    {
                        typeof(FieldReferenceExpression),
                        ExpressionConverterBase.SimplifyConverter<FieldReferenceExpression>(
                            MemberReferenceConverter.Convert)
                    },

                    {
                        typeof(VirtualMethodReferenceExpression),
                        ExpressionConverterBase.SimplifyConverter<VirtualMethodReferenceExpression>(
                            MemberReferenceConverter.Convert)
                    },

                    {
                        typeof(MethodCallExpression),
                        ExpressionConverterBase.SimplifyConverter<MethodCallExpression>(
                            MethodCallExpressionConverter.Convert)
                    },

                    {
                        typeof(NewObjectExpression),
                        ExpressionConverterBase.SimplifyConverter<NewObjectExpression>(
                            NewObjectConverter.Convert)
                    },

                    {
                        typeof(NewArrayExpression),
                        ExpressionConverterBase.SimplifyConverter<NewArrayExpression>(
                            NewArrayConverter.Convert)
                    },

                    {
                        typeof(ArrayElementExpression),
                        ExpressionConverterBase.SimplifyConverter<ArrayElementExpression>(
                            NewArrayConverter.ConvertArrayElementLoad)
                    },

                    {
                        typeof(TypeCheckExpression),
                        ExpressionConverterBase.SimplifyConverter<TypeCheckExpression>(
                            TypeCheckConverter.Convert)
                    },

                    {
                        typeof(UnaryExpression),
                        ExpressionConverterBase.SimplifyConverter<UnaryExpression>(
                            UnaryExpressionConverter.Convert)
                    },

                    {
                        typeof(ConditionalOperatorExpression),
                        ExpressionConverterBase.SimplifyConverter<ConditionalOperatorExpression>(
                            ConditionalOperatorConverter.Convert)
                    },

                    {
                        typeof(ConditionalAccessExpression),
                        ExpressionConverterBase.SimplifyConverter<ConditionalAccessExpression>(
                            ConditionalAccessConverter.Convert)
                    },

                    {
                        typeof(ConditionalAccessExpression.ConditionalReceiver),
                        ExpressionConverterBase.SimplifyConverter<ConditionalAccessExpression.ConditionalReceiver>(
                            ConditionalAccessConverter.Convert)
                    },

                    {
                        typeof(BoxExpression),
                        ExpressionConverterBase.SimplifyConverter<BoxExpression>(
                            BoxExpressionConvereter.Convert)
                    },

                    {
                        typeof(UnboxExpression),
                        ExpressionConverterBase.SimplifyConverter<UnboxExpression>(
                            BoxExpressionConvereter.Convert)
                    },

                    {
                        typeof(DefaultValueExpression),
                        ExpressionConverterBase.SimplifyConverter<DefaultValueExpression>(
                            DefaultValueConverter.Convert)
                    },

                    {
                        typeof(InlineArrayInitialization),
                        ExpressionConverterBase.SimplifyConverter<InlineArrayInitialization>(
                            InlineNewObjectArrayConverter.Convert)
                    },

                    {
                        typeof(InlineCollectionInitializationExpression),
                        ExpressionConverterBase.SimplifyConverter<InlineCollectionInitializationExpression>(
                            InlineCollectionInitializerConverter.Convert)
                    },

                    {
                        typeof(InlinePropertyInitilizationExpression),
                        ExpressionConverterBase.SimplifyConverter<InlinePropertyInitilizationExpression>(
                            InlinePropertyInitializerConverter.Convert)
                    },

                    {
                        typeof(VariableAddressReference),
                        ExpressionConverterBase.SimplifyConverter<VariableAddressReference>(
                            VariableAddressReferenceConverter.Convert)
                    },

                    {
                        typeof(PropertyReferenceExpression),
                        ExpressionConverterBase.SimplifyConverter<PropertyReferenceExpression>(
                            PropertyReferenceConverter.Convert)
                    },

                    {
                        typeof(EventReferenceExpression),
                        ExpressionConverterBase.SimplifyConverter<EventReferenceExpression>(
                            EventReferenceConverter.Convert)
                    },

                    {
                        typeof(LoadAddressExpression),
                        ExpressionConverterBase.SimplifyConverter<LoadAddressExpression>(
                            LoadAddressExpressionConverter.Convert)
                    },

                    {
                        typeof(DelegateMethodExpression),
                        ExpressionConverterBase.SimplifyConverter<DelegateMethodExpression>(
                            DelegateMethodConverter.Convert)
                    },

                    {
                        typeof(NullConditional),
                        ExpressionConverterBase.SimplifyConverter<NullConditional>(
                            NullConditionalConverter.Convert)
                    },

                    {
                        typeof(NullCoalsecingAssignmentExpression),
                        SimplifyConverter<NullCoalsecingAssignmentExpression>(NullConditionalConverter.Convert)
                    },

                    {
                        typeof(InlineDelegateExpression),
                        ExpressionConverterBase.SimplifyConverter<InlineDelegateExpression>(
                            InlineDelegateExpressionConverter.Convert)
                    },

                    {
                        typeof(TypeofExpression),
                        ExpressionConverterBase.SimplifyConverter<TypeofExpression>(
                            TypeofExpressionConverter.Convert)
                    },
                    {
                        typeof(ThrowExpression),
                        ExpressionConverterBase.SimplifyConverter<ThrowExpression>(
                            ThrowExpressionConverter.Convert)
                    },
                    {
                        typeof(ToNullable),
                        ExpressionConverterBase.SimplifyConverter<ToNullable>(
                            NullableConverter.ConvertTo)
                    },

                    {
                        typeof(FromNullable),
                        ExpressionConverterBase.SimplifyConverter<FromNullable>(
                            NullableConverter.ConvertFrom)
                    },

                    {
                        typeof(AnonymousNewExpression),
                        ExpressionConverterBase.SimplifyConverter<AnonymousNewExpression>(
                            AnonymousNewExpressionConverter.Convert)
                    },

                    {
                        typeof(DynamicMemberAccessor),
                        ExpressionConverterBase.SimplifyConverter<DynamicMemberAccessor>(
                            DynamicMemberAcessorConverter.Convert)
                    },

                    {
                        typeof(DynamicIndexAccessor),
                        ExpressionConverterBase.SimplifyConverter<DynamicIndexAccessor>(
                            DynamicMemberAcessorConverter.Convert)
                    },

                    {
                        typeof(DynamicCallInvocationExpression),
                        ExpressionConverterBase.SimplifyConverter<DynamicCallInvocationExpression>(
                            DynamicMemberAcessorConverter.Convert)
                    },

                    {
                        typeof(LocalFunctionReference),
                        ExpressionConverterBase.SimplifyConverter<LocalFunctionReference>(
                            LocalFunctionReferenceConverter.Convert)
                    },

                    {
                        typeof(TupleLiteral),
                        ExpressionConverterBase.SimplifyConverter<TupleLiteral>(
                            TupleLiteralConverter.Convert)
                    },

                    {
                        typeof(TupleDeconstructExpression),
                        ExpressionConverterBase.SimplifyConverter<TupleDeconstructExpression>(
                            TupleDeconstructConverter.Convert)
                    },

                    {
                        typeof(AwaitExpression),
                        ExpressionConverterBase.SimplifyConverter<AwaitExpression>(
                            AwaitExpressionConverter.Convert)
                    },

                    {
                        typeof(IsPatternExpression),
                        ExpressionConverterBase.SimplifyConverter<IsPatternExpression>(
                            IsPatternConverter.Convert)
                    },

                    {
                        typeof(SwitchExpression),
                        SimplifyConverter<SwitchExpression>(SwitchExpressionConverter.Convert)
                    },
                };

            return returnValue;
        }

        /// <summary>
        /// Simplifies the converter.
        /// </summary>
        /// <typeparam name="T">Expression sub class.</typeparam>
        /// <param name="converter">The converter.</param>
        /// <returns>Function that will convert Expression to JST.Expression.</returns>
        private static Func<IMethodScopeConverter, Expression, JST.Expression> SimplifyConverter<T>(
            Func<IMethodScopeConverter, T, JST.Expression> converter)
            where T : Expression => delegate (IMethodScopeConverter mc, Expression statement)
        {
            try
            {
                return converter(mc, (T)statement);
            }
            catch (ApplicationException ex)
            {
                if (ex is ConverterLocationException)
                { throw; }

                if (statement.Location != null)
                {
                    throw new ConverterLocationException(
                        statement.Location,
                        ex.Message);
                }
                else
                { throw; }
            }
        };
    }
}