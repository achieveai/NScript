namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal interface IDispatch {
        System.Int32 TryGetTypeInfoCount(out System.UInt32 pctinfo);
        System.Int32 TryGetTypeInfo(System.UInt32 iTInfo, System.Int32 lcid, out System.IntPtr info);
        System.Int32 TryGetIDsOfNames(ref System.Guid iid, System.String[] names, System.UInt32 cNames, System.Int32 lcid, out System.Int32[] rgDispId);
        System.Int32 TryInvoke(System.Int32 dispIdMember, ref System.Guid riid, System.Int32 lcid, System.Runtime.InteropServices.ComTypes.INVOKEKIND wFlags, ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams, out System.Object VarResult, out System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo, out System.UInt32 puArgErr);
    }
}
