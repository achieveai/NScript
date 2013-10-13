//-----------------------------------------------------------------------
// <copyright file="IResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;

    /// <summary>
    /// Definition for IResolver
    /// </summary>
    public interface IClrResolver
    {
        /// <summary>
        /// Gets a type reference.
        /// </summary>
        /// <param name="fullName"> Name of the full. </param>
        /// <returns>
        /// The type reference.
        /// </returns>
        TypeReference GetTypeReference(string fullName);

        /// <summary>
        /// Gets a type reference.
        /// </summary>
        /// <param name="typeName"> Name of the type. </param>
        /// <returns>
        /// The type reference.
        /// </returns>
        TypeReference GetTypeReference(Tuple<string, string> typeName);

        /// <summary>
        /// Gets a property reference.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="propertyName">  Name of the property. </param>
        /// <returns>
        /// The property reference.
        /// </returns>
        PropertyReference GetPropertyReference(TypeReference typeReference, string propertyName);

        /// <summary>
        /// Gets a method reference.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="methodName">    Name of the method. </param>
        /// <returns>
        /// The method reference.
        /// </returns>
        List<MethodReference> GetMethodReference(TypeReference typeReference, string methodName);

        /// <summary>
        /// Gets an event references.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="eventName">     Name of the event. </param>
        /// <returns>
        /// The event references.
        /// </returns>
        EventReference GetEventReferences(TypeReference typeReference, string eventName);

        /// <summary>
        /// Type implements.
        /// </summary>
        /// <param name="typeReference">        The type reference. </param>
        /// <param name="interfaceReference">   The interface reference. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        bool TypeImplements(TypeReference typeReference, TypeReference interfaceReference);

        /// <summary>
        /// Type inherits.
        /// </summary>
        /// <param name="typeReference">    The type reference. </param>
        /// <param name="parentType">       Type of the parent. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        bool TypeInherits(TypeReference typeReference, TypeReference parentType);
    }
}