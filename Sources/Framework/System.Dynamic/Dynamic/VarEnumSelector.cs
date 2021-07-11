namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class VarEnumSelector : System.Object {
        extern internal VarEnumSelector(System.Type[] explicitArgTypes);
        extern internal static System.Type GetManagedMarshalType(System.Runtime.InteropServices.VarEnum varEnum);
    }
}
