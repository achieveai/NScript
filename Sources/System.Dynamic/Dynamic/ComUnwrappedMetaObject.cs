namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComUnwrappedMetaObject : System.Dynamic.DynamicMetaObject {
        extern internal ComUnwrappedMetaObject(System.Linq.Expressions.Expression expression, System.Dynamic.BindingRestrictions restrictions, System.Object value);
    }
}
