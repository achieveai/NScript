namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class UnsafeMethods : System.Object {
        internal readonly static System.IntPtr NullInterfaceId;
        extern internal static System.IntPtr ConvertSByteByrefToPtr(ref System.SByte value);
        extern internal static System.IntPtr ConvertInt16ByrefToPtr(ref System.Int16 value);
        extern public static System.IntPtr ConvertInt32ByrefToPtr(ref System.Int32 value);
        extern internal static System.IntPtr ConvertInt64ByrefToPtr(ref System.Int64 value);
        extern internal static System.IntPtr ConvertByteByrefToPtr(ref System.Byte value);
        extern internal static System.IntPtr ConvertUInt16ByrefToPtr(ref System.UInt16 value);
        extern internal static System.IntPtr ConvertUInt32ByrefToPtr(ref System.UInt32 value);
        extern internal static System.IntPtr ConvertUInt64ByrefToPtr(ref System.UInt64 value);
        extern internal static System.IntPtr ConvertIntPtrByrefToPtr(ref System.IntPtr value);
        extern internal static System.IntPtr ConvertUIntPtrByrefToPtr(ref System.UIntPtr value);
        extern internal static System.IntPtr ConvertSingleByrefToPtr(ref System.Single value);
        extern internal static System.IntPtr ConvertDoubleByrefToPtr(ref System.Double value);
        extern internal static System.IntPtr ConvertDecimalByrefToPtr(ref System.Decimal value);
        extern public static System.IntPtr ConvertVariantByrefToPtr(ref System.Dynamic.Variant value);
        extern internal static System.Dynamic.Variant GetVariantForObject(System.Object obj);
        extern internal static void  InitVariantForObject(System.Object obj, ref System.Dynamic.Variant variant);
        extern public static System.Object GetObjectForVariant(System.Dynamic.Variant variant);
        extern public static System.Int32 IUnknownRelease(System.IntPtr interfacePointer);
        extern public static void  IUnknownReleaseNotZero(System.IntPtr interfacePointer);
        extern public static System.Int32 IDispatchInvoke(System.IntPtr dispatchPointer, System.Int32 memberDispId, System.Runtime.InteropServices.ComTypes.INVOKEKIND flags, ref System.Runtime.InteropServices.ComTypes.DISPPARAMS dispParams, out System.Dynamic.Variant result, out System.Dynamic.ExcepInfo excepInfo, out System.UInt32 argErr);
        extern public static System.IntPtr GetIdsOfNamedParameters(System.Dynamic.IDispatch dispatch, System.String[] names, System.Int32 methodDispId, out System.Runtime.InteropServices.GCHandle pinningHandle);
    }
}
