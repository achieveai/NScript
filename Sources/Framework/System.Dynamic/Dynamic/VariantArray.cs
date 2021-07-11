namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class VariantArray : System.Object {
        extern internal static System.Linq.Expressions.MemberExpression GetStructField(System.Linq.Expressions.ParameterExpression variantArray, System.Int32 field);
        extern internal static System.Type GetStructType(System.Int32 args);
    }
}
