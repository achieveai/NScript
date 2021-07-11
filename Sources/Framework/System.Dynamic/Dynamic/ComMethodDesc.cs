namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComMethodDesc : System.Object {
        internal readonly System.Runtime.InteropServices.ComTypes.INVOKEKIND InvokeKind;
        extern internal ComMethodDesc(System.String name, System.Int32 dispId);
        extern internal ComMethodDesc(System.String name, System.Int32 dispId, System.Runtime.InteropServices.ComTypes.INVOKEKIND invkind);
        extern internal ComMethodDesc(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo, System.Runtime.InteropServices.ComTypes.FUNCDESC funcDesc);
    }
}
