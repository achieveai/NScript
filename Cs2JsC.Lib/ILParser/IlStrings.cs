using System;

namespace Cs2JsC.Lib.ILParser
{
    public static class IlStrings
    {
        public static class ScopeNames
        {
            public const string ClassType = ".class";
            public const string MethodType = ".method";
            public const string AssemblyType = ".assembly";
            public const string FieldType = ".field";
            public const string PropertyType = ".property";
            public const string CustomType = ".custom";
        }

        public static class ClassAttributes
        {
            public const string Public = "public";
            public const string Private = "private";
            public const string Nested = "nested";
            public const string Family = "family";
            public const string Assembly = "assembly";
            public const string Famandassem = "famandassem";
            public const string Famorassem = "famorassem";
            public const string Value = "value";
            public const string Enum = "enum";
            public const string Interface = "interface";
            public const string Sealed = "sealed";
            public const string Abstract = "abstract";
            public const string Auto = "auto";
            public const string Sequential = "sequential";
            public const string Explicit = "explicit";
            public const string Ansi = "ansi";
            public const string Unicode = "unicode";
            public const string Autochar = "autochar";
            public const string Import = "import";
            public const string Serializable = "serializable";
            public const string BeforeFieldInit = "beforefieldinit";
            public const string SpecialName = "specialname";
            public const string RtSpecialName = "rtspecialname";

            public static readonly string[] AllAttributes = 
            {
                ClassAttributes.Public,
                ClassAttributes.Private,
                ClassAttributes.Nested,
                ClassAttributes.Family,
                ClassAttributes.Assembly,
                ClassAttributes.Famandassem,
                ClassAttributes.Famorassem,
                ClassAttributes.Value,
                ClassAttributes.Enum,
                ClassAttributes.Interface,
                ClassAttributes.Sealed,
                ClassAttributes.Abstract,
                ClassAttributes.Auto,
                ClassAttributes.Sequential,
                ClassAttributes.Explicit,
                ClassAttributes.Ansi,
                ClassAttributes.Unicode,
                ClassAttributes.Autochar,
                ClassAttributes.Import,
                ClassAttributes.Serializable,
                ClassAttributes.BeforeFieldInit,
                ClassAttributes.SpecialName,
                ClassAttributes.RtSpecialName,
            };
        }

        public static class FieldAttributes
        {
            public const string Public = ClassAttributes.Public;
            public const string Private = ClassAttributes.Private;
            public const string Family = ClassAttributes.Family;
            public const string Assembly = ClassAttributes.Assembly;
            public const string Famandassem = ClassAttributes.Famandassem;
            public const string Famorassem = ClassAttributes.Famorassem;
            public const string PrivateScope = "privatescope";
            public const string Static = "static";
            public const string Initonly = "initonly";
            public const string RtSpecialName = ClassAttributes.RtSpecialName;
            public const string SpecialName = ClassAttributes.SpecialName;
            public const string Literal = "literal";
            public const string NotSerialized = "notserialized";

            public static string[] AllAttributes =
    		{
    			FieldAttributes.Public,
    			FieldAttributes.Private,
    			FieldAttributes.Family,
    			FieldAttributes.Assembly,
    			FieldAttributes.Famandassem,
    			FieldAttributes.Famorassem,
    			FieldAttributes.PrivateScope,
    			FieldAttributes.Static,
    			FieldAttributes.Initonly,
    			FieldAttributes.RtSpecialName,
    			FieldAttributes.SpecialName,
    			FieldAttributes.Literal,
    			FieldAttributes.NotSerialized,
    		};
        }

        public static class MethodAttributes
        {
            public const string Public = ClassAttributes.Public;
            public const string Private = ClassAttributes.Private;
            public const string Family = ClassAttributes.Family;
            public const string Assembly = ClassAttributes.Assembly;
            public const string Famandassem = ClassAttributes.Famandassem;
            public const string Famorassem = ClassAttributes.Famorassem;
            public const string PrivateScope = FieldAttributes.PrivateScope;
            public const string Static = FieldAttributes.Static;
            public const string RtSpecialName = ClassAttributes.RtSpecialName;
            public const string SpecialName = ClassAttributes.SpecialName;
            public const string Final = "final";
            public const string Virtual = "virtual";
            public const string Abstract = ClassAttributes.Abstract;
            public const string HideBySig = "hidebysig";
            public const string NewSlot = "newslot";
            public const string ReqSecObj = "reqsecobj";

            public static readonly string[] AllAttributes =
            {
                MethodAttributes.Public,
                MethodAttributes.Private,
                MethodAttributes.Family,
                MethodAttributes.Assembly,
                MethodAttributes.Famandassem,
                MethodAttributes.Famorassem,
                MethodAttributes.PrivateScope,
                MethodAttributes.Static,
                MethodAttributes.RtSpecialName,
                MethodAttributes.SpecialName,
                MethodAttributes.Final,
                MethodAttributes.Virtual,
                MethodAttributes.Abstract,
                MethodAttributes.HideBySig,
                MethodAttributes.NewSlot,
                MethodAttributes.ReqSecObj,
            };
        }

        public static class MethodImplementationAttributes
        {
            public const string None = "none";
            public const string Native = "native";
            public const string Cil = "cil";
            public const string OptIl = "optil";
            public const string Managed = "managed";
            public const string Unmanaged = "unmanaged";
            public const string Forwardref = "forwardref";
            public const string PreserveSig = "preservesig";
            public const string Runtime = "runtime";
            public const string InternalCall = "internalcall";
            public const string Synchronized = "synchronized";
            public const string Noninlining = "noninlining";
        }

        public static class MethodCallingConvention
        {
            public const string VarArg = "vararg";
            public const string Instance = "instance";
        }

        public static class MethodReturnClassTypes
        {
            public const string Void = "void";
            public const string Class = "class";
            public const string Struct = "valuetype";
        }

        public static class Instructions
        {
            public const string Call = "call";
            public const string Tail = "tail";
            public const string CallVirutal = "callvirt";
            public const string CallIndirect = "calli";
            public const string TailCall = "tail.call";
            public const string TailCallVirtual = "tail.callvirt";
            public const string TailCallIndirect = "tail.calli";

            public const string LoadConstant = "ldc";
            public const string LoadArgument = "ldarg";
            public const string LoadArgumentAddress = "ldarga";
            public const string LoadLocal = "ldlocal";
            public const string LoadLocalAddress = "ldlocala";
            public const string LoadIndirect = "ldind";
            public const string LoadNull = "ldnull";
            public const string LoadArrayElement = "ldelem";
            public const string LoadArrayElementAddress = "ldelema";
            public const string LoadField = "ldfld";
            public const string LoadFieldAddress = "ldflda";
            public const string LoadStaticField = "ldsfld";
            public const string LoadStaticFieldAddress = "ldsflda";
            public const string LoadArrayLength = "ldlen";
            public const string LoadString = "ldstr";

            public const string StoreArgument = "starg";
            public const string StoreLocal = "stloc";
            public const string StoreIndirect = "stind";
            public const string StoreArrayElement = "stelem";
            public const string StoreField = "stfld";
            public const string StoreStaticField = "stsfld";

            public const string NewObject = "newobj";
            public const string NewArray = "newarray";

            public const string Branch = "br";
            public const string BranchIfLessThan = "blt";
            public const string BranchIfLessThanOrEqualTo = "ble";
            public const string BranchIfTrue = "brtrue";
            public const string BranchIfFalse = "brfalse";
            public const string BranchIfGreaterThan = "bgt";
            public const string BranchIfGreaterThanOrEqualTo = "bge";
            public const string BranchIfEqualTo = "beq";
            public const string BranchIfNotEqualTo = "bne";
            public const string Return = "ret";

            public const string Add = "add";
            public const string Subtract = "sub";
            public const string Divide = "div";
            public const string Multiply = "mul";
            public const string Negate = "neg";
            public const string Remainder = "rem";

            public const string BitwiseAnd = "and";
            public const string BitwiseOr = "or";
            public const string Xor = "xor";
            public const string ShiftLeft = "shl";
            public const string ShiftRight = "shr";

            public const string CompareEqual = "ceq";
            public const string CompareGreaterThan = "cgt";
            public const string CompareLessThan = "clt";
            public const string Not = "not";

            public const string Pop = "pop";
            public const string Nop = "nop";

            public const string CastToClass = "castclass";
            public const string IsClassInstance = "isinst";
        }
    }
}
