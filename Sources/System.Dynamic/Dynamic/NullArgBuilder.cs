namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class NullArgBuilder : System.Dynamic.ArgBuilder {
        extern internal NullArgBuilder();
        extern internal override System.Linq.Expressions.Expression Marshal(System.Linq.Expressions.Expression parameter);
    }
}
