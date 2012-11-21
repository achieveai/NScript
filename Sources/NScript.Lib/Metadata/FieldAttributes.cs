using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Lib.MetaData
{
    [Flags]
    public enum FieldAttributes
    {
        None = 0,
        Public = 0x1,
        Private = 0x2,
        Family = 0x4,
        Assembly = 0x8,
        Famandassem = 0x10,
        Famorassem = 0x20,
        PrivateScope = 0x40,
        Static = 0x80,
        Initonly = 0x100,
        RtSpecialName = 0x200,
        SpecialName = 0x400,
        Literal = 0x800,
        NotSerialized = 0x1000,
    }
}
