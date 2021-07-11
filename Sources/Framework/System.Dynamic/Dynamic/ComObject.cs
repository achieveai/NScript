namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ComObject : System.Object, System.Dynamic.IDynamicMetaObjectProvider {
        extern internal ComObject(System.Object rcw);
        extern public static System.Dynamic.ComObject ObjectToComObject(System.Object rcw);
        extern internal static System.Linq.Expressions.MemberExpression RcwFromComObject(System.Linq.Expressions.Expression comObject);
        extern internal static System.Linq.Expressions.MethodCallExpression RcwToComObject(System.Linq.Expressions.Expression rcw);
        extern internal virtual System.Collections.Generic.IList<System.String> GetMemberNames(System.Boolean dataOnly);
        extern internal virtual System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<System.String, System.Object>> GetMembers(System.Collections.Generic.IEnumerable<System.String> names);
        extern internal static System.Boolean IsComObject(System.Object obj);
        extern public DynamicMetaObject GetMetaObject(Linq.Expressions.Expression parameter);
    }
}
