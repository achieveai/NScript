namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Entry point into the browser IndexedDB API. Obtain the singleton via
    /// <see cref="Instance"/> and call <see cref="Open(string, int)"/> to
    /// acquire a database.
    /// </summary>
    [IgnoreNamespace]
    public class IDBFactory
    {
        /// <summary>Compares two IndexedDB keys; returns -1, 0, or 1.</summary>
        public extern Number Cmp(object first, object second);

        /// <summary>Requests deletion of the named database.</summary>
        public extern IDBOpenDBRequest DeleteDatabase(string name);

        /// <summary>Opens the named database at its current version.</summary>
        public extern IDBOpenDBRequest Open(string name);

        /// <summary>Opens the named database at the given version, triggering upgrade if needed.</summary>
        public extern IDBOpenDBRequest Open(string name, int version);

        /// <summary>The window.indexedDB singleton.</summary>
        [ScriptAlias("indexedDB"), IntrinsicProperty]
        public static extern IDBFactory Instance
        { get; }
    }
}
