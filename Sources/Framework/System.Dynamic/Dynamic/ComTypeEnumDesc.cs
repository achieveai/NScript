namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComTypeEnumDesc : System.Dynamic.ComTypeDesc {
        extern public override System.String ToString();
        extern internal ComTypeEnumDesc(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo);
    }
}
