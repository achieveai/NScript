//-----------------------------------------------------------------------
// <copyright file="TypeManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cs2JsC.PELoader.Reflection;

    /// <summary>
    /// Definition for TypeManager
    /// </summary>
    public static class TypeManager
    {
        /// <summary>
        /// Gets the type reference id.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <returns></returns>
        public static string GetTypeReferenceId(TypeReference typeReference)
        {
            return string.Empty;
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="typeNames">The type names.</param>
        /// <returns></returns>
        public static TypeReference ResolveType(Tuple<string, string> typeNames)
        {
            return new TypeReference(typeNames);
        }

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static List<TypeReference> ResolveType(string typeName)
        {
            return null;
        }

        /// <summary>
        /// Gets the id string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string GetIdString(string str, int i)
        {
            return str + i.ToString();
        }

        /// <summary>
        /// Adds the type reference.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        public static void AddTypeReference(TypeReference typeReference)
        {
        }

        /// <summary>
        /// Resolves the field.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Field reference if one exists, else null.</returns>
        public static FieldReference ResolveField(TypeReference type, string fieldName)
        {
            FieldReference rv = type.ResolveField(fieldName);
            if (rv != null)
            {
                return rv;
            }

            if (type.Extends != null)
            {
                TypeManager.ResolveField(type.Extends, fieldName);
            }

            return null;
        }

        /// <summary>
        /// Resolves the property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>PropertyReference (if one exists)</returns>
        public static PropertyReference ResolveProperty(TypeReference type, string propertyName)
        {
            PropertyReference rv = type.ResolverProperty(propertyName);
            if (rv != null)
            {
                return rv;
            }

            if (type.Extends != null)
            {
                TypeManager.ResolveProperty(type.Extends, propertyName);
            }

            return null;
        }

        /// <summary>
        /// Resolves the property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="lookupChildren">if set to <c>true</c> [lookup children].</param>
        /// <returns>
        /// PropertyReference (if one exists)
        /// </returns>
        public static PropertyReference ResolveProperty(
            TypeReference type,
            string propertyName,
            bool lookupChildren)
        {
            PropertyReference rv = TypeManager.ResolveProperty(
                type,
                propertyName);

            if (rv != null && !lookupChildren)
            {
                return rv;
            }

            return TypeManager.ResolveChildProperty(
                type,
                propertyName);
        }

        /// <summary>
        /// Resolves the children property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Returns property on any of the child classes.</returns>
        private static PropertyReference ResolveChildProperty(
            TypeReference type,
            string propertyName)
        {
            return null;
        }
    }
}
