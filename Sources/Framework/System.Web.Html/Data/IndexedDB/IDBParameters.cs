namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Options bag for <see cref="IDBDatabase.CreateObjectStore{K,T}(string, IDBObjectStoreParameters)"/>.
    /// </summary>
    [JsonType]
    public class IDBObjectStoreParameters
    {
        public extern bool? AutoIncrement
        { get; set; }

        public extern string KeyPath
        { get; set; }
    }

    /// <summary>
    /// Options bag for <see cref="IDBObjectStore{K,T}.CreateIndex(string, string, IDBIndexParameters)"/>.
    /// </summary>
    [JsonType]
    public class IDBIndexParameters
    {
        public extern bool? MultiEntry
        { get; set; }

        public extern bool? Unique
        { get; set; }
    }
}
