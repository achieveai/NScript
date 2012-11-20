using System;

namespace NScript.Lib.MetaData
{
    [Flags]
    public enum TypeAttributes
    {
        None,
        Public = 0x1,
        Private = 0x2,
        NestedPublic = 0x4,
        NestedPrivate = 0x8,
        NestedFamily = 0x10,
        NestedAssembly = 0x20,
        NestedFamandassem = 0x40,
        NestedFamorassem = 0x80,
        Value = 0x100,
        Enum = 0x200,
        Interface = 0x400,
        Sealed = 0x800,
        Abstract = 0x1000,
        Auto = 0x2000,
        Sequential = 0x4000,
        Explicit = 0x8000,
        Ansi = 0x10000,
        Unicode = 0x20000,
        Autochar = 0x40000,
        Import = 0x80000,
        Serializable = 0x100000,
        BeforeFieldInit = 0x200000,
        SpecialName = 0x400000,
        RtSpecialName = 0x800000,
    }

}
