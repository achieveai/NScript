namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class CollectionExtensions : System.Object {
        extern internal static T[] RemoveFirst<T>(T[] array);
        extern internal static T[] AddFirst<T>(System.Collections.Generic.IList<T> list, T item);
        extern internal static T[] ToArray<T>(System.Collections.Generic.IList<T> list);
        extern internal static T[] AddLast<T>(System.Collections.Generic.IList<T> list, T item);
    }
}
