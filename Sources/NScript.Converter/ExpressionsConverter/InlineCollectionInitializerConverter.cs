//-----------------------------------------------------------------------
// <copyright file="InlineCollectionInitializerConverter.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using Mono.Cecil;
    using NScript.CLR.AST;
    using NScript.Converter.TypeSystemConverter;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition for InlineCollectionInitializerConverter
    /// </summary>
    public static class InlineCollectionInitializerConverter
    {
        public static JST.Expression Convert(
            IMethodScopeConverter converter,
            InlineCollectionInitializationExpression expression)
        {
            if (IsStringDictionary(
                converter,
                expression.ResultType))
            {
                var nativeDictionaryConstructor = GetNativeDictionaryConstructor(
                    converter,
                    expression.ResultType);

                if (nativeDictionaryConstructor != null)
                {
                    var rv = ConverterUsingNativeDictionary(
                        converter,
                        nativeDictionaryConstructor,
                        expression.Setters,
                        expression.Location);

                    if (rv != null)
                    { return rv; }
                }
            }
            else
            {
                var nativeArrayConstructor = GetNativeArrayConstructor(
                    converter,
                    expression.ResultType);

                if (nativeArrayConstructor != null)
                {
                    return ConverterUsingNativeArray(
                        converter,
                        nativeArrayConstructor,
                        expression);
                }
            }

            throw new NotImplementedException();
        }

        public static bool IsStringDictionary(
            IMethodScopeConverter converter,
            TypeReference typeRef)
        {
            var typeDef = typeRef.Resolve();
            return typeDef.Module == converter.KnownReferences.ListGeneric.Module
                && typeDef.Name == "StringDictionary`1";
        }

        private static bool IsDictionary(
            IMethodScopeConverter converter,
            TypeReference typeRef)
        => typeRef.Resolve()
           .Interfaces
           .Select(iface => iface.InterfaceType.Resolve())
           .Any(iface =>
               iface.Module == converter.KnownReferences.ListGeneric.Module
               && iface.FullName == "System.Collections.Generic.IDictionary`2");

        public static JST.Expression ConverterUsingNativeDictionary(
            IMethodScopeConverter converter,
            MethodReference constructor,
            IList<MethodCallExpression> setters,
            Utils.Location location)
        {
            if (setters
                .Any(setter => !(setter.Parameters[0] is StringLiteral)))
            { return null; }

            var objExpression = new JST.InlineObjectInitializer(
                location,
                converter.Scope);

            foreach (var setter in setters)
            {
                objExpression.AddInitializer(
                    (setter.Parameters[0] as StringLiteral).String,
                    ExpressionConverterBase.Convert(converter, setter.Parameters[1]));
            }

            return new JST.MethodCallExpression(
                location,
                converter.Scope,
                JST.IdentifierExpression.Create(
                    location,
                    converter.Scope,
                    converter.ResolveFactory(constructor)),
                objExpression);
        }

        private static JST.Expression ConverterUsingNativeArray(
            IMethodScopeConverter converter,
            MethodReference constructor,
            InlineCollectionInitializationExpression expression)
        {
            var arrayExpression = new JST.InlineNewArrayInitialization(
                expression.Location,
                converter.Scope,
                expression.Setters
                    .Select(m => ExpressionConverterBase.Convert(converter, m.Parameters[0]))
                    .ToList());

            return new JST.MethodCallExpression(
                expression.Location,
                converter.Scope,
                JST.IdentifierExpression.Create(
                    expression.Location,
                    converter.Scope,
                    converter.ResolveFactory(constructor)),
                arrayExpression);
        }

        private static MethodReference GetNativeArrayConstructor(
            IMethodScopeConverter converter,
            TypeReference typeReference)
        {
            var typeDef = typeReference.Resolve();
            var rvConstructorDef = typeDef
                .Methods
                .Where(m => m.IsPublic && m.IsConstructor && !m.IsStatic)
                .Where(m => m.Parameters.Count == 1 && m.Parameters[0].ParameterType.Resolve() == converter.KnownReferences.NativeArrayGeneric)
                .FirstOrDefault();

            var nativeArrayType = rvConstructorDef
                .Parameters[0]
                .ParameterType as GenericInstanceType;

            // var nativeGenericTypeParam = nativeArrayType.GenericArguments[0] as GenericParameter;
            // if (nativeGenericTypeParam != null)
            // {
            //     var genericTypeRef = typeReference as GenericInstanceType;
            //     if (genericTypeRef != null)
            //     {
            //         var itemType = genericTypeRef
            //             .GenericArguments[nativeGenericTypeParam.Position];

            //         nativeArrayType = new GenericInstanceType(nativeArrayType.ElementType);
            //         nativeArrayType.GenericArguments.Add(itemType);
            //     }
            //     else
            //     { throw new InvalidProgramException(); }
            // }

            var rv = converter.KnownReferences.GetMethodReference(
                ".ctor",
                converter.ClrKnownReferences.Void,
                typeReference,
                nativeArrayType);

            return rv;
        }

        public static MethodReference GetNativeDictionaryConstructor(
            IMethodScopeConverter converter,
            TypeReference typeReference)
        {
            var typeDef = typeReference.Resolve();
            var rvConstructorDef = typeDef
                .Methods
                .Where(m => m.IsPublic && m.IsConstructor && !m.IsStatic)
                .Where(m => m.Parameters.Count == 1
                    && (m.Parameters[0].ParameterType.Resolve() == converter.ClrKnownReferences.Object
                       || m.Parameters[0].ParameterType.Resolve().FullName == "System.Collections.Dictionary"))
                .FirstOrDefault();

            var rv = converter.KnownReferences.GetMethodReference(
                ".ctor",
                converter.ClrKnownReferences.Void,
                typeReference,
                rvConstructorDef.Parameters[0].ParameterType);

            return rv;
        }
    }
}
