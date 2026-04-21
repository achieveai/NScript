namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a logical transaction scope within an IndexedDB database.
    /// Obtain via <see cref="IDBDatabase.Transaction(string, string)"/>.
    /// </summary>
    [IgnoreNamespace, ImportedType]
    public class IDBTransaction : EventTarget
    {
        public extern IDBDatabase Db
        { get; }

        /// <summary>Transaction mode string ("readonly", "readwrite", or "versionchange").</summary>
        public extern string Mode
        { get; }

        /// <summary>Error string if the transaction failed.</summary>
        public extern string Error
        { get; }

        public event Action<Event> OnAbort;
        public event Action<Event> OnComplete;
        public event Action<Event> OnError;

        /// <summary>Retrieves an object store scoped to this transaction.</summary>
        [IgnoreGenericArguments]
        public extern IDBObjectStore<TKey, TValue> ObjectStore<TKey, TValue>(string storeName)
            where TKey : class
            where TValue : class;

        /// <summary>Aborts this transaction, reverting any writes.</summary>
        public extern void Abort();
    }
}
