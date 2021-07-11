//-----------------------------------------------------------------------
// <copyright file="ProtoBufSerialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib.Serialization
{
    using ProtoBuf;
    using System;

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [Serializable]
    public class ArrayTypeSer
        : TypeSpecSer
    {
        public TypeSpecSer ElementType { get; set; }
    }
}
