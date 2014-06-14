namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class Assert : System.Object {
        extern internal static void  NotNull(System.Object var);
        extern internal static void  NotNull(System.Object var1, System.Object var2);
        extern internal static void  NotNull(System.Object var1, System.Object var2, System.Object var3);
        extern internal static void  NotNull(System.Object var1, System.Object var2, System.Object var3, System.Object var4);
        extern internal static void  NotEmpty(System.String str);
        extern internal static void  NotEmpty<T>(System.Collections.Generic.ICollection<T> array);
        extern internal static void  NotNullItems<T>(System.Collections.Generic.IEnumerable<T> items);
        extern internal static void  IsTrue(System.Func<System.Boolean> predicate);
    }
}
