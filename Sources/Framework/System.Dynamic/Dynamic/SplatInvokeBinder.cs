namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class SplatInvokeBinder : System.Runtime.CompilerServices.CallSiteBinder {
        internal readonly static System.Dynamic.SplatInvokeBinder Instance;
        extern public override System.Linq.Expressions.Expression Bind(System.Object[] args, System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression> parameters, System.Linq.Expressions.LabelTarget returnLabel);
        extern public SplatInvokeBinder();
    }
}
