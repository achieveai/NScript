namespace Sunlight.Framework.Data.WebStore
{
    using System.Web.Html.Data.IndexedDB;

    /// <summary>
    /// Thin wrapper around an <see cref="IDBTransaction"/> returned by
    /// <see cref="WebStoreClient.StartTransaction"/>. Exposed so callers can
    /// pass the same transaction across multiple <see cref="WebStoreTable{TKey,TValue}"/>
    /// calls to batch work atomically.
    /// </summary>
    public class WebStoreTransaction
    {
        internal WebStoreTransaction(IDBTransaction transaction)
        { Transaction = transaction; }

        internal IDBTransaction Transaction { get; }

        /// <summary>Aborts this transaction, reverting any writes.</summary>
        public void Abort()
        { Transaction.Abort(); }
    }
}
