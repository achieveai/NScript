//-----------------------------------------------------------------------
// <copyright file="TypeDependencyInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.DependencyBuilder
{
    using System;
    using System.Collections.Generic;
    using NScript.PELoader.Reflection;
    using Mono.Cecil;

    /// <summary>
    /// Definition for TypeDependencyInfo
    /// </summary>
    public class TypeDependencyInfo
    {
        /// <summary>
        /// Backing field for typeDependencies for the type.
        /// </summary>
        private readonly List<TypeReference> typeDependencies = new List<TypeReference>();

        /// <summary>
        /// Backing field for all typeDependencies from all the members.
        /// </summary>
        private readonly Dictionary<MemberDefinition, HashSet<TypeReference>> memberTypeDependencies =
            new Dictionary<MemberDefinition, HashSet<TypeReference>>();

        /// <summary>
        /// Backing field for all the memberDependencies from all the methods.
        /// </summary>
        private readonly Dictionary<MethodDefinition, HashSet<MemberReference>> methodToMemberDependencies =
            new Dictionary<MethodDefinition, HashSet<MemberReference>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDependencyInfo"/> class.
        /// </summary>
        /// <param name="typeDefinition">The type definition.</param>
        public TypeDependencyInfo(
            TypeDefinition typeDefinition)
        {
        }
    }
}
