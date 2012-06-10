using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cs2JsC.Lib.MetaData
{
    [Flags]
    public enum MethodAttributes
    {
        None = 0,
        Static = 0x1,
        Public = 0x2,
        Private = 0x4,
        Family = 0x8,
        Assembly = 0x10,
        Famandassem = 0x20,
        Famorassem = 0x40,
        PrivateScope = 0x80,
        Final = 0x100,
        Virtual = 0x200,
        Abstract = 0x400,
        HideBySig = 0x800,
        NewSlot = 0x1000,
        ReqSecObj = 0x2000,
        SpecialName = 0x4000,
        RtSpecialName = 0x8000
    }

    [Flags]
    public enum MethodCallingConvention
    {
        None = 0,
        Instance = 1,
        Varargs = 2,
    }
}
