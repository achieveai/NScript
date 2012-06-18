//-----------------------------------------------------------------------
// <copyright file="MemberReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.ExpressionsConverter
{
    using Cs2JsC.CLR;
    using Cs2JsC.CLR.AST;
    using Cs2JsC.Converter.TypeSystemConverter;
    using Mono.Cecil;

    /// <summary>
    /// Definition for MemberReferenceConverter
    /// </summary>
    public static class MemberReferenceConverter
    {
        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="memberReference">The member reference.</param>
        /// <returns>Expression for the member references.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter methodConverter,
            MemberReferenceExpression memberReference)
        {
            MemberReference memberRef = memberReference.MemberReference;
            MemberReferenceConverter.FixMethodReference(
                methodConverter.RuntimeManager.Context,
                ref memberRef);

            if (memberReference.LeftExpression == null)
            {
                // This expression conversion may change later on to single Static method identifier.
                // for now let's keep it this way.
                if (memberRef is FieldReference)
                {
                    return JST.IdentifierExpression.Create(
                        memberReference.Location,
                        methodConverter.Scope,
                        methodConverter.ResolveStaticMember(
                            (FieldReference)memberRef));
                }

                return JST.IdentifierExpression.Create(
                    memberReference.Location,
                    methodConverter.Scope,
                    methodConverter.ResolveStaticMember(
                        (MethodReference)memberRef));
            }

            JST.Identifier methodIdentifier =
                memberRef is FieldReference
                    ? methodConverter.Resolve((FieldReference)memberRef)
                    : methodConverter.Resolve((MethodReference)memberRef);

            return new JST.IndexExpression(
                memberReference.Location,
                methodConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodConverter,
                    memberReference.LeftExpression),
                new JST.IdentifierExpression(
                    methodIdentifier));
        }

        /// <summary>
        /// Converts the specified method converter.
        /// </summary>
        /// <param name="methodConverter">The method converter.</param>
        /// <param name="virtualMethodReference">The virtual method reference.</param>
        /// <returns>Expression for given virtual method.</returns>
        public static JST.Expression Convert(
            IMethodScopeConverter methodConverter,
            VirtualMethodReferenceExpression virtualMethodReference)
        {
            MemberReference methodReference = virtualMethodReference.MethodReference;
            MemberReferenceConverter.FixMethodReference(
                methodConverter.RuntimeManager.Context,
                ref methodReference);

            return new JST.IndexExpression(
                virtualMethodReference.Location,
                methodConverter.Scope,
                ExpressionConverterBase.Convert(
                    methodConverter,
                    virtualMethodReference.LeftExpression),
                methodConverter.ResolveVirtualMethod(
                    (MethodReference)methodReference,
                    methodConverter.Scope));
        }

        public static void FixMethodReference<T>(
            ConverterContext context,
            ref T memberReference) where T : MemberReference
        {
            ConverterKnownReferences converterKnownReferences = context.KnownReferences;
            MethodReference methodReference = memberReference as MethodReference;
            if (methodReference == null)
            {
                return;
            }

            if (methodReference.DeclaringType.IsSame(context.ClrContext.KnownReferences.SystemArray))
            {
                MethodReference replacementMethod = null;
                if (methodReference.IsSame(converterKnownReferences.ArrayCloneMethod))
                {
                    replacementMethod = converterKnownReferences.ArrayImplCloneMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayContainsMethod))
                {
                    replacementMethod = converterKnownReferences.ArrayImplContainsMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayReverseMethod))
                {
                    replacementMethod = converterKnownReferences.ArrayImplReverseMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayIndexOf1Method))
                {
                    replacementMethod = converterKnownReferences.ArrayImplIndexOf1Method;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayIndexOf2Method))
                {
                    replacementMethod = converterKnownReferences.ArrayImplIndexOf2Method;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayGetEnumeratorMethod))
                {
                    replacementMethod = converterKnownReferences.ArrayImplGetEnumeratorMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayLengthGetter))
                {
                    replacementMethod = converterKnownReferences.ArrayImplLengthGetter;
                }

                memberReference = (T)(object)replacementMethod;
            }
        }
    }
}