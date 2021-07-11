namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ConvertibleArgBuilder : System.Dynamic.ArgBuilder {
        extern internal ConvertibleArgBuilder();
        extern internal override System.Linq.Expressions.Expression Marshal(System.Linq.Expressions.Expression parameter);
        extern internal override System.Linq.Expressions.Expression MarshalToRef(System.Linq.Expressions.Expression parameter);
    }
}
