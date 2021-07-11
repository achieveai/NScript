namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class Error : System.Object {
        extern internal static System.Exception COMObjectDoesNotSupportEvents();
        extern internal static System.Exception COMObjectDoesNotSupportSourceInterface();
        extern internal static System.Exception SetComObjectDataFailed();
        extern internal static System.Exception MethodShouldNotBeCalled();
        extern internal static System.Exception UnexpectedVarEnum(System.Object p0);
        extern internal static System.Exception DispBadParamCount(System.Object p0);
        extern internal static System.Exception DispMemberNotFound(System.Object p0);
        extern internal static System.Exception DispNoNamedArgs(System.Object p0);
        extern internal static System.Exception DispOverflow(System.Object p0);
        extern internal static System.Exception DispTypeMismatch(System.Object p0, System.Object p1);
        extern internal static System.Exception DispParamNotOptional(System.Object p0);
        extern internal static System.Exception CannotRetrieveTypeInformation();
        extern internal static System.Exception GetIDsOfNamesInvalid(System.Object p0);
        extern internal static System.Exception UnsupportedEnumType();
        extern internal static System.Exception UnsupportedHandlerType();
        extern internal static System.Exception CouldNotGetDispId(System.Object p0, System.Object p1);
        extern internal static System.Exception AmbiguousConversion(System.Object p0, System.Object p1);
        extern internal static System.Exception VariantGetAccessorNYI(System.Object p0);
        extern internal static System.Exception ArgumentNull(System.String paramName);
        extern internal static System.Exception ArgumentOutOfRange(System.String paramName);
        extern internal static System.Exception NotImplemented();
        extern internal static System.Exception NotSupported();
    }
}
