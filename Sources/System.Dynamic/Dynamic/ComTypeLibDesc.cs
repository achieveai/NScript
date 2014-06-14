namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComTypeLibDesc : System.Object {
        extern public override System.String ToString();
        extern internal static System.Dynamic.ComTypeLibDesc GetFromTypeLib(System.Runtime.InteropServices.ComTypes.ITypeLib typeLib);
        extern internal System.Dynamic.ComTypeClassDesc GetCoClassForInterface(System.String itfName);
    }
}
