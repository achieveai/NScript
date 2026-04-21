namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Iterator over records in an object store or index.
    /// </summary>
    /// <typeparam name="K">Type of the primary key.</typeparam>
    /// <typeparam name="T">Type of stored records.</typeparam>
    [ImportedType, IgnoreNamespace, IgnoreGenericArguments]
    public class IDBCursor<K, T> where K : class where T : class
    {
        internal extern IDBCursor();

        /// <summary>Direction string ("next", "nextunique", "prev", or "prevunique").</summary>
        public extern string Direction
        { get; }

        [ScriptName("key")]
        public extern IDBKeyRange KeyRange
        { get; }

        public extern object Source
        { get; }

        public extern K PrimaryKey
        { get; }

        public extern T Value
        { get; }

        public extern void Advance(int count);

        public extern void Continue();

        public extern void Continue(IDBKeyRange key);

        public extern IDBRequest Delete();

        public extern IDBRequest Update(T value);

        [ScriptName("NEXT")]
        public static extern string Next
        { get; }

        [ScriptName("NEXT_NO_DUPLICATE")]
        public static extern string NextNoDuplicate
        { get; }

        [ScriptName("PREV")]
        public static extern string Previous
        { get; }

        [ScriptName("PREV_NO_DUPLICATE")]
        public static extern string PreviousNoDuplicate
        { get; }
    }
}
