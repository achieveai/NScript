namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class ContractUtils : System.Object {
        extern internal static void  Requires(System.Boolean precondition, System.String paramName);
        extern internal static void  Requires(System.Boolean precondition, System.String paramName, System.String message);
        extern internal static void  RequiresNotNull(System.Object value, System.String paramName);
    }
}
