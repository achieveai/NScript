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
    public class IResolver
    {
        /// <summary>
        /// Gets a type reference.
        /// </summary>
        /// <param name="fullName"> Name of the full. </param>
        /// <returns>
        /// The type reference.
        /// </returns>
        private TypeReference GetTypeReference(string fullName);

        /// <summary>
        /// Gets a property reference.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="propertyName">  Name of the property. </param>
        /// <returns>
        /// The property reference.
        /// </returns>
        private List<PropertyReference> GetPropertyReference(TypeReference typeReference, string propertyName);

        /// <summary>
        /// Gets a method reference.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="methodName">    Name of the method. </param>
        /// <returns>
        /// The method reference.
        /// </returns>
        private List<MethodReference> GetMethodReference(TypeReference typeReference, string methodName);

        /// <summary>
        /// Gets an event references.
        /// </summary>
        /// <param name="typeReference"> The type reference. </param>
        /// <param name="eventName">     Name of the event. </param>
        /// <returns>
        /// The event references.
        /// </returns>
        private List<EventReference> GetEventReferences(TypeReference typeReference, string eventName);
    }
}