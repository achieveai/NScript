namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents an open IndexedDB database. Obtain via <see cref="IDBOpenDBRequest"/>
    /// success event. Use <see cref="Transaction(string, string)"/> to begin a
    /// read or write operation scope.
    /// </summary>
    [IgnoreNamespace, ImportedType]
    public class IDBDatabase : EventTarget
    {
        /// <summary>Name of this database.</summary>
        public extern string Name
        { get; }

        /// <summary>Names of all object stores in this database.</summary>
        public extern NativeArray<string> ObjectStoreNames
        { get; }

        /// <summary>Current version of this database.</summary>
        public extern int Version
        { get; set; }

        public extern event Action<Event> OnAbort;

        public extern event Action<Event> OnError;

        public extern event Action<IDBVersionChangeEvent> OnVersionChange;

        /// <summary>Closes this database connection.</summary>
        public extern void Close();

        /// <summary>Creates a new object store with a string key.</summary>
        [IgnoreGenericArguments]
        public extern IDBObjectStore<K, T> CreateObjectStore<K, T>(string name)
            where K : class where T : class;

        /// <summary>Creates a new object store with the given parameters (key path, auto-increment).</summary>
        [IgnoreGenericArguments]
        public extern IDBObjectStore<K, T> CreateObjectStore<K, T>(
            string name,
            IDBObjectStoreParameters parameters)
            where K : class where T : class;

        /// <summary>Deletes an object store from this database. Only valid during a versionchange transaction.</summary>
        public extern void DeleteObjectStore(string name);

        /// <summary>Begins a readonly transaction over a single store.</summary>
        public extern IDBTransaction Transaction(string storeName);

        /// <summary>Begins a readonly transaction over several stores.</summary>
        public extern IDBTransaction Transaction(NativeArray<string> storeNames);

        /// <summary>Begins a transaction with the specified mode ("readonly" or "readwrite").</summary>
        public extern IDBTransaction Transaction(string storeName, string mode);

        /// <summary>Begins a transaction over several stores with the specified mode.</summary>
        public extern IDBTransaction Transaction(NativeArray<string> storeNames, string mode);

        public void Bind(string eventName, Action<IDBDatabase, Event> handler, bool capture = false)
        {
            EventBinder.AddEvent(this, eventName, handler, capture);
        }

        public void UnBind(string eventName, Action<IDBDatabase, Event> handler, bool capture = false)
        {
            EventBinder.RemoveEvent(this, eventName, handler, capture);
        }
    }
}
