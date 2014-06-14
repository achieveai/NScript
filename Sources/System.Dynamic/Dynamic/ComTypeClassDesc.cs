namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComTypeClassDesc : System.Dynamic.ComTypeDesc {
        extern internal ComTypeClassDesc(System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo);
        extern internal System.Boolean Implements(System.String itfName, System.Boolean isSourceItf);
    }
}
