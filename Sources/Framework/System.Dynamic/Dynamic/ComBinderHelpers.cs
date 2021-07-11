namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class ComBinderHelpers : System.Object {
        extern internal static System.Boolean PreferPut(System.Type type, System.Boolean holdsNull);
        extern internal static System.Boolean IsByRef(System.Dynamic.DynamicMetaObject mo);
        extern internal static System.Boolean IsStrongBoxArg(System.Dynamic.DynamicMetaObject o);
        extern internal static System.Boolean[] ProcessArgumentsForCom(ref System.Dynamic.DynamicMetaObject[] args);
        extern internal static System.Dynamic.BindingRestrictions GetTypeRestrictionForDynamicMetaObject(System.Dynamic.DynamicMetaObject obj);
    }
}
