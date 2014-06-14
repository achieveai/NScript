namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class SR : System.Object {
        internal const System.String InvalidArgumentValue = "InvalidArgumentValue";
        internal const System.String ComObjectExpected = "ComObjectExpected";
        internal const System.String CannotCall = "CannotCall";
        internal const System.String COMObjectDoesNotSupportEvents = "COMObjectDoesNotSupportEvents";
        internal const System.String COMObjectDoesNotSupportSourceInterface = "COMObjectDoesNotSupportSourceInterface";
        internal const System.String SetComObjectDataFailed = "SetComObjectDataFailed";
        internal const System.String MethodShouldNotBeCalled = "MethodShouldNotBeCalled";
        internal const System.String UnexpectedVarEnum = "UnexpectedVarEnum";
        internal const System.String DispBadParamCount = "DispBadParamCount";
        internal const System.String DispMemberNotFound = "DispMemberNotFound";
        internal const System.String DispNoNamedArgs = "DispNoNamedArgs";
        internal const System.String DispOverflow = "DispOverflow";
        internal const System.String DispTypeMismatch = "DispTypeMismatch";
        internal const System.String DispParamNotOptional = "DispParamNotOptional";
        internal const System.String CannotRetrieveTypeInformation = "CannotRetrieveTypeInformation";
        internal const System.String GetIDsOfNamesInvalid = "GetIDsOfNamesInvalid";
        internal const System.String UnsupportedEnumType = "UnsupportedEnumType";
        internal const System.String UnsupportedHandlerType = "UnsupportedHandlerType";
        internal const System.String CouldNotGetDispId = "CouldNotGetDispId";
        internal const System.String AmbiguousConversion = "AmbiguousConversion";
        internal const System.String VariantGetAccessorNYI = "VariantGetAccessorNYI";
        extern internal SR();
        extern public static System.String GetString(System.String name, System.Object[] args);
        extern public static System.String GetString(System.String name);
        extern public static System.String GetString(System.String name, out System.Boolean usedFallback);
        extern public static System.Object GetObject(System.String name);
    }
}
