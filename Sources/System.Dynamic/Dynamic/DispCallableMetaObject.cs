namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class DispCallableMetaObject : System.Dynamic.DynamicMetaObject {
        extern internal DispCallableMetaObject();
        extern public override System.Dynamic.DynamicMetaObject BindGetIndex(System.Dynamic.GetIndexBinder binder, System.Dynamic.DynamicMetaObject[] indexes);
        extern public override System.Dynamic.DynamicMetaObject BindInvoke(System.Dynamic.InvokeBinder binder, System.Dynamic.DynamicMetaObject[] args);
        extern public override System.Dynamic.DynamicMetaObject BindSetIndex(System.Dynamic.SetIndexBinder binder, System.Dynamic.DynamicMetaObject[] indexes, System.Dynamic.DynamicMetaObject value);
    }
}
