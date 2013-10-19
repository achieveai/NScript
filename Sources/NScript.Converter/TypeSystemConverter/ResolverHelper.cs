//-----------------------------------------------------------------------
// <copyright file="ResolverHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using Mono.Cecil;
    using NScript.JST;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NScript.CLR;

    public static class ResolverHelper
    {
        public static IList<IIdentifier> ResolveStaticMember(
            RuntimeScopeManager runtimeManager,
            MethodReference method,
            Func<TypeReference, IList<IIdentifier>> resolver,
            bool isFactory)
        {
            bool isFixedName, isAlias;

            string name = runtimeManager.Context.GetMemberName(
                method.GetDefinition(),
                false,
                out isFixedName,
                out isAlias);

            if (isAlias)
            {
                return runtimeManager.ResolveScriptAlias(name);
            }

            List<IIdentifier> returnValue = new List<IIdentifier>();
            TypeDefinition memberTypeDef = method.DeclaringType.Resolve();
            if ((!memberTypeDef.IsGeneric()
                || runtimeManager.Context.IsPsudoType(memberTypeDef)
                || runtimeManager.Context.IsExtended(memberTypeDef))
                && runtimeManager.Context.IsImplemented(method.Resolve()))
            {
                if (!isFactory)
                {
                    returnValue.Add(runtimeManager.ResolveStatic(method.Resolve()));
                }
                else
                {
                    returnValue.Add(runtimeManager.ResolveFactory(method.Resolve()));
                }

                return returnValue;
            }

            IList<IIdentifier> parentPath = resolver(method.DeclaringType);
            if (parentPath != null)
            {
                returnValue.AddRange(parentPath);
            }

            returnValue.Add(runtimeManager.Resolve(method, true));
            return returnValue;
        }

        /// <summary>
        /// Resolves the specified member reference.
        /// </summary>
        /// <param name="fieldReference">The field reference.</param>
        /// <returns>
        /// Identifier identifying the member.
        /// </returns>
        public static IIdentifier Resolve(
            RuntimeScopeManager runtimeManager,
            IResolver resolver,
            FieldReference fieldReference)
        {
            if (runtimeManager.Context.IsPsudoType(fieldReference.DeclaringType.Resolve()))
            {
                return new CompoundIdentifier(
                    resolver.Resolve(
                        runtimeManager.Context.KnownReferences.ImportedExtensionField),
                    runtimeManager.Resolve(fieldReference));
            }

            return runtimeManager.Resolve(fieldReference);
        }

        /// <summary>
        /// Resolves the static member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="resolver">The resolver.</param>
        /// <returns>Idnentifiers for accessing static member.</returns>
        public static IList<IIdentifier> ResolveStaticMember(
            RuntimeScopeManager runtimeManager,
            IResolver thisResolver,
            FieldReference member,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            bool isFixedName, isAlias;

            string name = runtimeManager.Context.GetMemberName(
                member.GetDefinition(),
                false,
                out isFixedName,
                out isAlias);

            if (isAlias)
            {
                return runtimeManager.ResolveScriptAlias(name);
            }

            List<IIdentifier> returnValue = new List<IIdentifier>();
            TypeDefinition memberTypeDef = member.DeclaringType.Resolve();
            if (!memberTypeDef.IsGeneric()
                && runtimeManager.Context.IsImplemented(member.Resolve()))
            {
                returnValue.Add(runtimeManager.ResolveStatic(member.Resolve()));
                return returnValue;
            }

            IList<IIdentifier> parentPath = resolver(member.DeclaringType);
            if (parentPath != null)
            {
                returnValue.AddRange(parentPath);
            }

            returnValue.Add(thisResolver.Resolve(member));
            return returnValue;
        }

        /// <summary>
        /// Resolve static member.
        /// </summary>
        /// <param name="propertyDefinition"> The property definition. </param>
        /// <param name="resolver">           The resolver. </param>
        /// <returns>
        /// Identifier for static member.
        /// </returns>
        public static IIdentifier ResolveStaticMember(
            RuntimeScopeManager runtimeManager,
            PropertyDefinition propertyDefinition,
            Func<TypeReference, IList<IIdentifier>> resolver)
        {
            bool isFixedName, isAlias;

            string name = runtimeManager.Context.GetMemberName(
                propertyDefinition,
                false,
                out isFixedName,
                out isAlias);

            if (isAlias)
            {
                return new CompoundIdentifier(runtimeManager.ResolveScriptAlias(name));
            }

            List<IIdentifier> returnValue = new List<IIdentifier>();
            TypeDefinition memberTypeDef = propertyDefinition.DeclaringType.Resolve();
            if (!memberTypeDef.IsGeneric()
                && runtimeManager.Context.IsImplemented(propertyDefinition))
            {
                returnValue.Add(runtimeManager.ResolveStatic(propertyDefinition));
                return new CompoundIdentifier(returnValue);
            }

            IList<IIdentifier> parentPath = resolver(propertyDefinition.DeclaringType);
            if (parentPath != null)
            {
                returnValue.AddRange(parentPath);
            }

            returnValue.Add(runtimeManager.Resolve(propertyDefinition));
            return new CompoundIdentifier(returnValue);
        }

        /// <summary>
        /// Resolves the specified type reference base.
        /// </summary>
        /// <param name="paramDef">The type reference base.</param>
        /// <returns>Identifier for givenType.</returns>
        public static IList<IIdentifier> Resolve(
            RuntimeScopeManager runtimeManager,
            Func<TypeReference, IIdentifier> localTypeResolver,
            TypeReference typeReference)
        {
            GenericParameterType? typeScope = typeReference.GetGenericTypeScope();
            TypeDefinition typeDefinition = typeReference.Resolve();

            if ((typeDefinition == null || !runtimeManager.Context.IsPsudoType(typeDefinition))
                && typeScope.HasValue
                && typeScope.Value == GenericParameterType.Type)
            {
                IIdentifier rv;
                if ((rv = localTypeResolver(typeReference)) == null)
                {
                    throw new InvalidOperationException();
                }

                return new IIdentifier[] { rv };
            }

            return runtimeManager.ResolveType((TypeReference)typeReference);
        }
    }
}
