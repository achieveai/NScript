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
    public class TypeInfoSer
    {
        public Dictionary<int, TypeSpecSer> Types { get; set; }

        public Dictionary<int, MethodSpecSer> Methods { get; set; }

        public Dictionary<int, FieldSpecSer> Fields { get; set; }

        public Dictionary<int, PropertySpecSer> Properties { get; set; }

        public Dictionary<int, EventSpecSer> Events { get; set; }
    }
}
