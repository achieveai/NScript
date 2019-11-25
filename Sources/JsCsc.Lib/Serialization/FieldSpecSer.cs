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
    public class FieldSpecSer
    {
        public TypeSpecSer DeclaringType { get; set; }

        public TypeSpecSer MemberType { get; set; }

        public string Name { get; set; }
    }
}
