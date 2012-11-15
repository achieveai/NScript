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

        IIdentifier Resolve(MethodReference memberReference);

        IList<IIdentifier> Resolve(TypeReference typeReference);

        IList<IIdentifier> ResolveStaticMember(MethodReference member);

        IList<JST.IIdentifier> ResolveFactory(Mono.Cecil.MethodReference methodReference);

        IList<JST.IIdentifier> ResolveStaticMember(Mono.Cecil.FieldReference fieldReference);

        JST.IIdentifier Resolve(Mono.Cecil.FieldReference fieldReference);

        JST.Expression ResolveMethodSlotName(Mono.Cecil.MethodReference methodReference, bool isVirtualCall, JST.IdentifierScope identifierScope);
    }
}