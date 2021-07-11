namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ComFallbackMetaObject : System.Dynamic.DynamicMetaObject {
        extern internal ComFallbackMetaObject(System.Linq.Expressions.Expression expression, System.Dynamic.BindingRestrictions restrictions, System.Object arg);
        extern public override System.Dynamic.DynamicMetaObject BindGetIndex(System.Dynamic.GetIndexBinder binder, System.Dynamic.DynamicMetaObject[] indexes);
        extern public override System.Dynamic.DynamicMetaObject BindSetIndex(System.Dynamic.SetIndexBinder binder, System.Dynamic.DynamicMetaObject[] indexes, System.Dynamic.DynamicMetaObject value);
        extern public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder);
        extern public override System.Dynamic.DynamicMetaObject BindInvokeMember(System.Dynamic.InvokeMemberBinder binder, System.Dynamic.DynamicMetaObject[] args);
        extern public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value);
        extern protected virtual System.Dynamic.ComUnwrappedMetaObject UnwrapSelf();
    }
}
