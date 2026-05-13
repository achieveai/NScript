//-----------------------------------------------------------------------
// <copyright file="ProtoBufSerialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib.Serialization
{
    using System;
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [Serializable]
    public class MethodSpecSer
    {
        public TypeSpecSer DeclaringType { get; set; }

        public TypeSpecSer ReturnType { get; set; }

        public string Name { get; set; }

        public bool IsStatic { get; set; }

        public int Arity { get; set; }

        public List<ParamSer> Parameters { get; set; }

        public List<TypeSpecSer> TypeArgs { get; set; }

        /// <summary>
        /// True when the method is a C# 9 <c>init</c> accessor — i.e. its
        /// return type carries a <c>modreq(System.Runtime.CompilerServices.IsExternalInit)</c>
        /// in metadata. Persisted here so the deserializer can rebuild the
        /// modreq on the round-tripped <see cref="Mono.Cecil.MethodReference"/>,
        /// which is required for Cecil's <c>MetadataResolver</c> to match the
        /// reference back to its <see cref="Mono.Cecil.MethodDefinition"/>.
        /// </summary>
        public bool IsInitOnly { get; set; }
    }
}
