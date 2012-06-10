//-----------------------------------------------------------------------
// <copyright file="IResolver.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System.Collections.Generic;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for IResolver
    /// </summary>
    public interface IResolver
    {
        JST.Expression ResolveVirtualMethod(
            MethodReference methodReference,
            IdentifierScope scope);

        Identifier Resolve(MethodReference memberReference);

        IList<Identifier> Resolve(TypeReference typeReference);

        IList<Identifier> ResolveStaticMember(MethodReference member);
    }
}