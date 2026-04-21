namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents an index on an <see cref="IDBObjectStore{K, T}"/>.
    /// </summary>
    /// <typeparam name="K">Type of the primary key on the owning object store.</typeparam>
    /// <typeparam name="T">Type of stored records.</typeparam>
    [IgnoreNamespace, IgnoreGenericArguments]
    public class IDBIndex<K, T>
            where K : class where T : class
    {
        public extern object KeyPath
        { get; }

        public extern bool MultiEntry
        { get; }

        public extern string Name
        { get; }

        public extern IDBObjectStore<K, T> ObjectStore
        { get; }

        [ScriptName("keyPath")]
        public extern string SingleKeyPath
        { get; set; }

        public extern bool Unique
        { get; }

        public extern IDBRequest Count(IDBKeyRange key);

        public extern IDBRequest Count(K key);

        public extern IDBRequest Get(IDBKeyRange key);

        public extern IDBRequest Get(K key);

        public extern IDBRequest GetKey(IDBKeyRange key);

        public extern IDBRequest GetKey(K key);

        public extern IDBRequest OpenCursor(IDBKeyRange key);

        public extern IDBRequest OpenCursor(K key);

        public extern IDBRequest OpenCursor(IDBKeyRange key, string direction);

        public extern IDBRequest OpenCursor(K key, string direction);

        public extern IDBRequest OpenKeyCursor(IDBKeyRange key);

        public extern IDBRequest OpenKeyCursor(K key);

        public extern IDBRequest OpenKeyCursor(IDBKeyRange key, string direction);

        public extern IDBRequest OpenKeyCursor(K key, string direction);
    }
}
