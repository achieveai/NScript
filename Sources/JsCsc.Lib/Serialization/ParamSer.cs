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
    public class ParamSer
    {
        public TypeSpecSer ParamType { get; set; }

        public int ModFlags { get; set; }

        public string Name { get; set; }
    }
}
