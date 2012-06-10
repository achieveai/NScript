//-----------------------------------------------------------------------
// <copyright file="IResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JSParser
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.JST;

    /// <summary>
    /// Definition for IResolver
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Resolves the identifier.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>Resolved identifier.</returns>
        Expression ResolveIdentifier(
            IdentifierScope scope,
            string identifier);

        /// <summary>
        /// Resolves the type.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// Expression for resolved type.
        /// </returns>
        Expression ResolveType(
            IdentifierScope scope,
            Tuple<string, string> typeName);

        /// <summary>
        /// Resolves the field.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="isInstance">if set to <c>true</c> [is instance].</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>
        /// Expression for resolved field.
        /// </returns>
        Expression ResolveField(
            IdentifierScope scope,
            Tuple<string, string> typeName,
            bool isInstance,
            string fieldName);

        /// <summary>
        /// Resolves the method.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="isInstance">if set to <c>true</c> [is instance].</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="methodArguments">The method arguments.</param>
        /// <returns>
        /// Expression for referncing given method.
        /// </returns>
        Expression ResolveMethod(
            IdentifierScope scope,
            Tuple<string, string> typeName,
            bool isInstance,
            string methodName,
            params Tuple<string, string>[] methodArguments);
    }
}
