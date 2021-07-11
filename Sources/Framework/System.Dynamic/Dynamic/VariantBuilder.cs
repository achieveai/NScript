namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class VariantBuilder : System.Object {
        extern internal VariantBuilder(System.Runtime.InteropServices.VarEnum targetComType, System.Dynamic.ArgBuilder builder);
        extern internal System.Linq.Expressions.Expression InitializeArgumentVariant(System.Linq.Expressions.MemberExpression variant, System.Linq.Expressions.Expression parameter);
        extern internal System.Linq.Expressions.Expression Clear();
        extern internal System.Linq.Expressions.Expression UpdateFromReturn(System.Linq.Expressions.Expression parameter);
    }
}
