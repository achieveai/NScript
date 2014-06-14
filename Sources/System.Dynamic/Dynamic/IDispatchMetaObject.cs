namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class IDispatchMetaObject : System.Dynamic.ComFallbackMetaObject {
        extern internal IDispatchMetaObject(System.Linq.Expressions.Expression expression, System.Dynamic.IDispatchComObject self);
        extern public override System.Dynamic.DynamicMetaObject BindInvokeMember(System.Dynamic.InvokeMemberBinder binder, System.Dynamic.DynamicMetaObject[] args);
        extern public override System.Dynamic.DynamicMetaObject BindInvoke(System.Dynamic.InvokeBinder binder, System.Dynamic.DynamicMetaObject[] args);
        extern public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder);
        extern public override System.Dynamic.DynamicMetaObject BindGetIndex(System.Dynamic.GetIndexBinder binder, System.Dynamic.DynamicMetaObject[] indexes);
        extern public override System.Dynamic.DynamicMetaObject BindSetIndex(System.Dynamic.SetIndexBinder binder, System.Dynamic.DynamicMetaObject[] indexes, System.Dynamic.DynamicMetaObject value);
        extern public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value);
        extern internal static System.Dynamic.BindingRestrictions IDispatchRestriction(System.Linq.Expressions.Expression expr, System.Dynamic.ComTypeDesc typeDesc);
        extern protected override System.Dynamic.ComUnwrappedMetaObject UnwrapSelf();
    }
}
