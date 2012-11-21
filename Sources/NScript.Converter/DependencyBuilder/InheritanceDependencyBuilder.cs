//-----------------------------------------------------------------------
// <copyright file="InheritanceDependencyBuilder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.DependencyBuilder
{
    using System.Collections.Generic;
    using Mono.Cecil;

    /// <summary>
    /// Definition for InheritanceDependencyBuilder
    /// </summary>
    public static class InheritanceDependencyBuilder
    {
        /// <summary>
        /// Gets the types by inheritance order.
        /// </summary>
        /// <param name="typeDefinitions">The type definitions.</param>
        /// <returns>
        /// TypeDefinitions in order of base class first.
        /// </returns>
        public static List<TypeDefinition> GetTypesByInheritanceOrder(
            IList<TypeDefinition> typeDefinitions)
        {
            List<TypeDefinition> returnValue = new List<TypeDefinition>(typeDefinitions.Count);
            HashSet<TypeDefinition> visited = new HashSet<TypeDefinition>();

            foreach (var typeDefinition in typeDefinitions)
            {
                InheritanceDependencyBuilder.OrderTypes(
                    returnValue,
                    visited,
                    typeDefinition);
            }

            return returnValue;
        }

        /// <summary>
        /// Orders the types.
        /// </summary>
        /// <param name="typeDefinitions">The type definitions.</param>
        /// <param name="visited">The visited.</param>
        /// <param name="typeDefinition">The type definition.</param>
        private static void OrderTypes(
            List<TypeDefinition> typeDefinitions,
            HashSet<TypeDefinition> visited,
            TypeDefinition typeDefinition)
        {
            if (typeDefinition == null
                || visited.Contains(typeDefinition))
            {
                return;
            }

            if (typeDefinition.BaseType != null)
            {
                InheritanceDependencyBuilder.OrderTypes(
                    typeDefinitions,
                    visited,
                    (TypeDefinition)typeDefinition.BaseType.Resolve());
            }

            foreach (var inface in typeDefinition.Interfaces)
            {
                InheritanceDependencyBuilder.OrderTypes(
                    typeDefinitions,
                    visited,
                    (TypeDefinition)inface.Resolve());
            }

            typeDefinitions.Add(typeDefinition);
            visited.Add(typeDefinition);
        }
    }
}
