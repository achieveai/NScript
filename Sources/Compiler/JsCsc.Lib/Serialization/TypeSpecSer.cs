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
    [ProtoInclude(100, typeof(GenericParamSer))]
    [ProtoInclude(101, typeof(ArrayTypeSer))]
    [ProtoInclude(102, typeof(GenericInstanceTypeSer))]
    [ProtoInclude(209, typeof(PointerTypeSer))]
    [Serializable]
    public class TypeSpecSer
    {
        public string Name { get; set; }

        public ModuleSpecSer Module { get; set; }

        public TypeSpecSer NestedParent { get; set; }

        public string Namespace { get; set; }

        public int Arity { get; set; }
    }
}
