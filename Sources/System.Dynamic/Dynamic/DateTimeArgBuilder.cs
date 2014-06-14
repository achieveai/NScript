namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class DateTimeArgBuilder : System.Dynamic.SimpleArgBuilder {
        extern internal DateTimeArgBuilder(System.Type parameterType);
        extern internal override System.Linq.Expressions.Expression MarshalToRef(System.Linq.Expressions.Expression parameter);
        extern internal override System.Linq.Expressions.Expression UnmarshalFromRef(System.Linq.Expressions.Expression value);
    }
}
