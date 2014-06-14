namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class DispCallable : System.Object, System.Dynamic.IDynamicMetaObjectProvider {
        extern internal DispCallable(System.Dynamic.IDispatchComObject dispatch, System.String memberName, System.Int32 dispId);
        extern public override System.String ToString();
        extern public System.Dynamic.DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter);
        extern public override System.Boolean Equals(System.Object obj);
        extern public override System.Int32 GetHashCode();
    }
}
