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
    public class GenericInstanceTypeSer
        : TypeSpecSer
    {
        public List<TypeSpecSer> TypeParams { get; set; }
   }
}
