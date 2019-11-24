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
    }
}
