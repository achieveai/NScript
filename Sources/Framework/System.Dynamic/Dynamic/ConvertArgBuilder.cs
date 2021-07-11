namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ConvertArgBuilder : System.Dynamic.SimpleArgBuilder {
        extern internal ConvertArgBuilder(System.Type parameterType, System.Type marshalType);
        extern internal override System.Linq.Expressions.Expression Marshal(System.Linq.Expressions.Expression parameter);
        extern internal override System.Linq.Expressions.Expression UnmarshalFromRef(System.Linq.Expressions.Expression newValue);
    }
}
