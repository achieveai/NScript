namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal interface IDispatchEx {
        System.Int32 DeleteMemberByDispID(System.Int32 id);
        System.Int32 DeleteMemberByName(System.String bstrName, System.UInt32 grfdex);
        System.Int32 GetDispID(System.String bstrName, System.UInt32 grfdex, out System.Int32 pid);
        System.Int32 GetMemberName(System.Int32 id, out System.String pbstrName);
        void  GetMemberProperties(System.Int32 id, System.UInt32 grfdexFetch, out System.UInt32 pgrfdex);
        void  GetNameSpaceParent(out System.Object ppunk);
        void  GetNextDispID(System.UInt32 grfdex, System.Int32 id, out System.Int32 pid);
        void  InvokeEx(System.Int32 id, System.UInt32 lcid, System.UInt32 wFlags, ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pdp, out System.Object pVarRes, out System.Runtime.InteropServices.ComTypes.EXCEPINFO pei, System.IServiceProvider pspCaller);
    }
}
