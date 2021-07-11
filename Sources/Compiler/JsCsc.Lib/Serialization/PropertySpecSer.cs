//-----------------------------------------------------------------------
// <copyright file="ProtoBufSerialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JsCsc.Lib.Serialization
{
    using System;
    using ProtoBuf;

    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    [Serializable]
    public class PropertySpecSer
    {
        public int? Setter { get; set; }

        public int? Getter { get; set; }
    }
}
