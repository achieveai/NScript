namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a single object store within an <see cref="IDBDatabase"/>.
    /// Provides CRUD operations and index management.
    /// </summary>
    /// <typeparam name="K">Type of the primary key.</typeparam>
    /// <typeparam name="T">Type of stored records.</typeparam>
    [IgnoreNamespace, ImportedType, IgnoreGenericArguments]
    public class IDBObjectStore<K, T> where K : class where T : class
    {
        public extern NativeArray<string> IndexNames
        { get; }

        [ScriptName("keyPath")]
        public extern string SingleKeyPath
        { get; set; }

        public extern NativeArray<string> KeyPath
        { get; set; }

        public extern string Name
        { get; }

        public extern IDBTransaction Transaction
        { get; }

        public extern bool AutoIncrement
        { get; set; }

        [ScriptName("openKeyCursor")]
        public extern bool HasOpenKeyCursor
        { get; }

        public extern IDBRequest Add(T value);

        public extern IDBRequest Add(T value, K key);

        public extern IDBRequest Add(T value, IDBKeyRange key);

        public extern IDBRequest Count(K key);

        public extern IDBRequest Count(IDBKeyRange key);

        public extern IDBRequest CreateIndex(string name, string keyPath, IDBIndexParameters optionalParameters = null);

        public extern IDBRequest CreateIndex(string name, NativeArray<string> keyPath, IDBIndexParameters optionalParameters = null);

        public extern void DeleteIndex(string indexName);

        public extern IDBRequest Delete(IDBKeyRange key);

        public extern IDBRequest Delete(K key);

        public extern IDBRequest Get(K key);

        public extern IDBIndex<K, T> Index(string name);

        public extern IDBRequest OpenCursor(IDBKeyRange range, string direction = null);

        public extern IDBRequest OpenCursor(K range, string direction = null);

        public extern IDBRequest OpenKeyCursor(IDBKeyRange range, string direction = null);

        public extern IDBRequest OpenKeyCursor(K range, string direction = null);

        public extern IDBRequest Put(T value);

        public extern IDBRequest Put(T value, K key);

        public extern IDBRequest Put(T value, IDBKeyRange key);
    }
}
