namespace Sunlight.Framework.Data.WebStore
{
    using System;
    using System.Collections.Generic;
    using System.Web.Html.Data.IndexedDB;

    /// <summary>
    /// Static-like entry point for opening a typed
    /// <see cref="WebStoreClient"/> over IndexedDB. Builds/upgrades the schema
    /// on first open and tracks per-database open state so a second open on an
    /// already-open database surfaces a clear error instead of stalling.
    /// </summary>
    public class WebStoreFactory
    {
        private static IDictionary<string, bool> _openClientTracker;

        /// <summary>Tracker of currently-open clients keyed by database name.</summary>
        internal static IDictionary<string, bool> OpenClientTracker
        {
            get
            {
                _openClientTracker ??= new StringDictionary<bool>();
                return _openClientTracker;
            }
        }

        /// <summary>True when a <see cref="WebStoreClient"/> for <paramref name="databaseName"/> is currently open.</summary>
        public static bool CheckOpened(string databaseName)
        {
            _ = OpenClientTracker.TryGetValue(databaseName, out bool result);
            return result;
        }

        /// <summary>
        /// Deletes a database. Fails if the database is currently open —
        /// callers must <see cref="WebStoreClient.Close"/> first.
        /// </summary>
        public Promise<bool> DeleteDatabase(string databaseName)
        {
            return new Promise<bool>(
                (resolve, reject) =>
                {
                    if (CheckOpened(databaseName))
                    {
                        reject(new Exception("Db still open"));
                        return;
                    }

                    var idbRequest = IDBFactory.Instance.DeleteDatabase(databaseName);
                    idbRequest.OnSuccess += (req, evt) => resolve(true);
                    idbRequest.OnError += (req, evt) => reject(new EventBasedException(evt));
                });
        }

        /// <summary>
        /// Open a database matching <paramref name="webStoreSchema"/>, running
        /// the upgrade path if the on-disk version is behind the schema's
        /// <see cref="WebStoreSchema.VersionId"/>.
        /// </summary>
        public Promise<WebStoreClient> Create(
            WebStoreSchema webStoreSchema)
        {
            var initHelper = new WebStoreInitHelper(webStoreSchema);
            return initHelper.GetInitPromise();
        }
    }

    /// <summary>
    /// Encapsulates the async state machine for opening + upgrading a
    /// <see cref="WebStoreClient"/>. Internal — not part of the public surface.
    /// </summary>
    internal class WebStoreInitHelper
    {
        private readonly WebStoreSchema _schema;
        private IDBOpenDBRequest _idbOpenRequest;
        private readonly NamedLogger _logger;

        public WebStoreInitHelper(WebStoreSchema schema)
        {
            _schema = schema;
            _logger = Logger.ForCategory("webStoreInit");
        }

        public Promise<WebStoreClient> GetInitPromise()
        {
            _logger.Info("Initializing DB:" + _schema.DatabaseName + ", Version:" + _schema.VersionId);

            return new Promise<WebStoreClient>(this.HandlePromiseCallback);
        }

        private void HandlePromiseCallback(
            Action<WebStoreClient> resolve,
            Action<object> reject)
        {
            if (WebStoreFactory.CheckOpened(_schema.DatabaseName))
            {
                _logger.Error("DB:" + _schema.DatabaseName + " is already open, can't create new client");
                reject(new Exception("Database already opened"));
                return;
            }

            _idbOpenRequest = IDBFactory.Instance.Open(
                _schema.DatabaseName,
                _schema.VersionId);

            _idbOpenRequest.OnSuccess +=
                (req, evt) =>
                {
                    WebStoreFactory.OpenClientTracker[_schema.DatabaseName] = true;
                    resolve(
                        new WebStoreClient(
                            _schema,
                            _idbOpenRequest.Result as IDBDatabase));
                };

            _idbOpenRequest.OnError += (req, evt) =>
                reject(new EventBasedException(
                    req.Error != null
                        ? req.Error.Name
                        : null,
                    evt));

            _idbOpenRequest.OnBlocked += (req, evt) =>
                reject(new EventBasedException(
                    req.Error != null
                        ? req.Error.Name
                        : null,
                    evt));

            _idbOpenRequest.OnUpgradeNeeded += OnUpgradeNeeded;
        }

        private void OnUpgradeNeeded(IDBOpenDBRequest request, Event evt)
        {
            _logger.Warn("Upgrading DB:" + _schema.DatabaseName + ", VersionId:" + _schema.VersionId);

            var db = _idbOpenRequest.Result as IDBDatabase;
            var tableSchemas = _schema.Tables;

            var objectStoreNames = db.ObjectStoreNames;
            for (int iName = 0; iName < objectStoreNames.Length; iName++)
            {
                if (!_schema.HasTableSchema(objectStoreNames[iName]))
                { db.DeleteObjectStore(objectStoreNames[iName]); }
            }

            for (int iTable = tableSchemas.Length - 1; iTable >= 0; iTable--)
            {
                bool found = false;
                var tableName = tableSchemas[iTable].Name;

                // DOMStringList does not have indexOf methods, so do this manually.
                for (int i = objectStoreNames.Length - 1; i >= 0 && !found; i--)
                {
                    if (objectStoreNames[i] == tableName)
                    { found = true; }
                }

                if (!found)
                { tableSchemas[iTable].InitializeTable(db); }
            }

            _logger.Info("Upgrade Complete for DB:" + _schema.DatabaseName);
        }
    }
}
