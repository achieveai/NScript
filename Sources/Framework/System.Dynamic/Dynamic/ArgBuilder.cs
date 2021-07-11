namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal abstract class ArgBuilder : System.Object {
        internal abstract System.Linq.Expressions.Expression Marshal(System.Linq.Expressions.Expression parameter);
        extern internal virtual System.Linq.Expressions.Expression MarshalToRef(System.Linq.Expressions.Expression parameter);
        extern internal virtual System.Linq.Expressions.Expression UnmarshalFromRef(System.Linq.Expressions.Expression newValue);
        extern protected ArgBuilder();
    }
}
