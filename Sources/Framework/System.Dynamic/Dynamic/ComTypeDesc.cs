namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ComTypeDesc : System.Object {
        extern internal System.Collections.Hashtable Funcs{ get; set; }
        extern internal System.Collections.Generic.Dictionary<System.String, System.Dynamic.ComEventDesc> Events{ get; set; }
        extern internal System.Guid Guid{ get; set; }
        extern internal ComTypeDesc(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo);
        extern internal static System.Dynamic.ComTypeDesc FromITypeInfo(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo, System.Runtime.InteropServices.ComTypes.TYPEATTR typeAttr);
        extern internal static System.Dynamic.ComTypeDesc CreateEmptyTypeDesc();
        extern internal System.Boolean TryGetFunc(System.String name, out System.Dynamic.ComMethodDesc method);
        extern internal void  AddFunc(System.String name, System.Dynamic.ComMethodDesc method);
        extern internal System.Boolean TryGetPut(System.String name, out System.Dynamic.ComMethodDesc method);
        extern internal void  AddPut(System.String name, System.Dynamic.ComMethodDesc method);
        extern internal System.Boolean TryGetPutRef(System.String name, out System.Dynamic.ComMethodDesc method);
        extern internal void  AddPutRef(System.String name, System.Dynamic.ComMethodDesc method);
        extern internal System.Boolean TryGetEvent(System.String name, out System.Dynamic.ComEventDesc @event);
        extern internal System.String[] GetMemberNames(System.Boolean dataOnly);
        extern internal void  EnsureGetItem(System.Dynamic.ComMethodDesc candidate);
        extern internal void  EnsureSetItem(System.Dynamic.ComMethodDesc candidate);
    }
}
