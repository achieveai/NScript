namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ErrorArgBuilder : System.Dynamic.SimpleArgBuilder {
        extern internal ErrorArgBuilder(System.Type parameterType);
        extern internal override System.Linq.Expressions.Expression Marshal(System.Linq.Expressions.Expression parameter);
        extern internal override System.Linq.Expressions.Expression UnmarshalFromRef(System.Linq.Expressions.Expression value);
    }
}
