namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComInvokeAction : System.Dynamic.InvokeBinder {
        extern internal ComInvokeAction(System.Dynamic.CallInfo callInfo);
        extern public override System.Int32 GetHashCode();
        extern public override System.Boolean Equals(System.Object obj);
        extern public override System.Dynamic.DynamicMetaObject FallbackInvoke(System.Dynamic.DynamicMetaObject target, System.Dynamic.DynamicMetaObject[] args, System.Dynamic.DynamicMetaObject errorSuggestion);
    }
}
