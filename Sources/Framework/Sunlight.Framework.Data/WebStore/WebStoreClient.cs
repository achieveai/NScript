namespace Sunlight.Framework.Data.WebStore
{
    using System.Collections.Generic;
    using System.Web.Html.Data.IndexedDB;

    /// <summary>
    /// Typed client on top of an opened <see cref="IDBDatabase"/>. Issued by
    /// <see cref="WebStoreFactory.Create(WebStoreSchema)"/>. Callers go through
    /// <see cref="Table{TKey,TValue}(string)"/> to perform CRUD, or start a
    /// longer-lived transaction via <see cref="StartTransaction(TransactionKind, string[])"/>
    /// and thread it through multiple table calls.
    /// </summary>
    public class WebStoreClient
    {
        public const string ReadWrite = "readwrite";
        public const string ReadOnly = "readonly";
        public const string VersionChange = "versionchange";

        private readonly NamedLogger _logger;
        private readonly List<System.Action> _pendingWorkQueue = new List<System.Action>();
        private readonly WebStoreSchema _webStoreSchema;

        internal WebStoreClient(
            WebStoreSchema webStoreSchema,
            IDBDatabase database)
        {
            _webStoreSchema = webStoreSchema;
            Database = database;
            _logger = Logger.ForCategory("webStoreClient");
        }

        /// <summary>Name of the underlying database.</summary>
        public string Name => Database.Name;

        internal IDBDatabase Database { get; }

        /// <summary>
        /// Closes the database connection and releases the open-client lock
        /// maintained by <see cref="WebStoreFactory"/>. After calling this the
        /// client is unusable.
        /// </summary>
        public void Close()
        {
            WebStoreFactory.OpenClientTracker[_webStoreSchema.DatabaseName] = false;
            Database.Close();
        }

        /// <summary>
        /// Begins a transaction spanning <paramref name="tableNames"/> with the
        /// requested <paramref name="kind"/>. The returned transaction can be
        /// passed to any number of <see cref="WebStoreTable{TKey,TValue}"/>
        /// operations to batch them atomically.
        /// </summary>
        public WebStoreTransaction StartTransaction(
            TransactionKind kind,
            string[] tableNames)
        {
            string transactionMode;
            switch (kind)
            {
                case TransactionKind.Read:
                    transactionMode = ReadOnly;
                    break;
                case TransactionKind.ReadWrite:
                    transactionMode = ReadWrite;
                    break;
                case TransactionKind.VersionChange:
                    transactionMode = VersionChange;
                    break;
                default:
                    transactionMode = ReadOnly;
                    break;
            }

            return new WebStoreTransaction(
                    Database.Transaction(
                        tableNames,
                        transactionMode));
        }

        /// <summary>
        /// Obtain a typed handle to a single table. The schema entry for
        /// <paramref name="tableName"/> must have been declared on the schema
        /// passed to <see cref="WebStoreFactory.Create(WebStoreSchema)"/>.
        /// </summary>
        public WebStoreTable<TKey, TValue> Table<TKey, TValue>(string tableName)
                    where TValue : class
            where TKey : class
        {
            var tableSchema = _webStoreSchema.GetTableSchema(tableName);
            return new WebStoreTable<TKey, TValue>(
                tableSchema,
                this);
        }
    }
}
