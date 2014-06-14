namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ConversionArgBuilder : System.Dynamic.ArgBuilder {
        extern internal ConversionArgBuilder(System.Type parameterType, System.Dynamic.SimpleArgBuilder innerBuilder);
        extern internal override System.Linq.Expressions.Expression Marshal(System.Linq.Expressions.Expression parameter);
        extern internal override System.Linq.Expressions.Expression MarshalToRef(System.Linq.Expressions.Expression parameter);
    }
}
