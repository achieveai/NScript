//-----------------------------------------------------------------------
// <copyright file="MemberReferenceConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.CLR;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
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

            JST.IIdentifier methodIdentifier =
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
                    methodIdentifier, methodConverter.Scope));
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
            if (!memberReference.DeclaringType.IsSame(context.ClrContext.KnownReferences.SystemArray))
            {
                return;
            }

            MethodReference methodReference = memberReference as MethodReference;
            PropertyReference propertyReference = memberReference as PropertyReference;
            if (methodReference != null)
            {
                if (methodReference.IsSame(converterKnownReferences.ArrayCloneMethod))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplCloneMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayContainsMethod))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplContainsMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayReverseMethod))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplReverseMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayIndexOf1Method))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplIndexOf1Method;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayIndexOf2Method))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplIndexOf2Method;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayGetEnumeratorMethod))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplGetEnumeratorMethod;
                }
                else if (methodReference.IsSame(converterKnownReferences.ArrayLengthGetter))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplLengthGetter;
                }
            }
            else if (propertyReference != null)
            {
                if (propertyReference.IsSame(converterKnownReferences.ArrayLengthProperty))
                {
                    memberReference = (T)(object)converterKnownReferences.ArrayImplLengthProperty;
                }
            }
        }
    }
}