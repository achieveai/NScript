namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class ComRuntimeHelpers : System.Object {
        extern public static void  CheckThrowException(System.Int32 hresult, ref System.Dynamic.ExcepInfo excepInfo, System.UInt32 argErr, System.String message);
        extern internal static void  GetInfoFromType(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo, out System.String name, out System.String documentation);
        extern internal static System.String GetNameOfMethod(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo, System.Int32 memid);
        extern internal static System.String GetNameOfLib(System.Runtime.InteropServices.ComTypes.ITypeLib typeLib);
        extern internal static System.String GetNameOfType(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo);
        extern internal static System.Runtime.InteropServices.ComTypes.ITypeInfo GetITypeInfoFromIDispatch(System.Dynamic.IDispatch dispatch, System.Boolean throwIfMissingExpectedTypeInfo);
        extern internal static System.Runtime.InteropServices.ComTypes.TYPEATTR GetTypeAttrForTypeInfo(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo);
        extern internal static System.Runtime.InteropServices.ComTypes.TYPELIBATTR GetTypeAttrForTypeLib(System.Runtime.InteropServices.ComTypes.ITypeLib typeLib);
        extern public static System.Dynamic.BoundDispEvent CreateComEvent(System.Object rcw, System.Guid sourceIid, System.Int32 dispid);
        extern public static System.Dynamic.DispCallable CreateDispCallable(System.Dynamic.IDispatchComObject dispatch, System.Dynamic.ComMethodDesc method);
    }
}
