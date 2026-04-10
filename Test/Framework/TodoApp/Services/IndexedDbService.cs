namespace TodoApp.Services
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Low-level IndexedDB wrapper. Each method is a raw JavaScript body via [Script].
    /// The @{...} resolved identifier syntax ensures minified field names are used correctly.
    /// The @: prefix is used to reference browser globals (Promise, indexedDB, JSON)
    /// that are not in the NScript known-identifier list.
    /// </summary>
    public class IndexedDbService
    {
        /// <summary>
        /// The underlying IDBDatabase instance, assigned on successful Open().
        /// </summary>
        private object db;

        /// <summary>
        /// Opens (or creates) the TodoAppDb database with folders and todos object stores.
        /// Returns a Promise that resolves to true on success.
        /// </summary>
        [Script(@"
            var self = this;
            return new @:Promise(function(resolve, reject) {
                var request = @:indexedDB.open('TodoAppDb', 1);
                request.onupgradeneeded = function(e) {
                    var db = e.target.result;
                    if (!db.objectStoreNames.contains('folders')) {
                        db.createObjectStore('folders', { keyPath: 'Id' });
                    }
                    if (!db.objectStoreNames.contains('todos')) {
                        var store = db.createObjectStore('todos', { keyPath: 'Id' });
                        store.createIndex('folderId', 'FolderId', { unique: false });
                    }
                };
                request.onsuccess = function(e) {
                    self.@{[TodoApp]TodoApp.Services.IndexedDbService::db} = e.target.result;
                    resolve(true);
                };
                request.onerror = function(e) { reject(e.target.error || new @:Error('IndexedDB open failed')); };
            });
        ")]
        public extern Promise<bool> Open();

        /// <summary>
        /// Inserts or updates a record in the given store from a JSON string.
        /// </summary>
        [Script(@"
            var self = this;
            return new @:Promise(function(resolve, reject) {
                var tx = self.@{[TodoApp]TodoApp.Services.IndexedDbService::db}.transaction(storeName, 'readwrite');
                var store = tx.objectStore(storeName);
                var request = store.put(@:JSON.parse(json));
                request.onsuccess = function() { resolve(true); };
                request.onerror = function(e) { reject(e.target.error || new @:Error('IndexedDB put failed')); };
            });
        ")]
        public extern Promise<bool> PutRaw(string storeName, string json);

        /// <summary>
        /// Returns all records in the given store as a JSON array string.
        /// </summary>
        [Script(@"
            var self = this;
            return new @:Promise(function(resolve, reject) {
                var tx = self.@{[TodoApp]TodoApp.Services.IndexedDbService::db}.transaction(storeName, 'readonly');
                var store = tx.objectStore(storeName);
                var request = store.getAll();
                request.onsuccess = function() { resolve(@:JSON.stringify(request.result || [])); };
                request.onerror = function(e) { reject(e.target.error || new @:Error('IndexedDB getAll failed')); };
            });
        ")]
        public extern Promise<string> GetAllRaw(string storeName);

        /// <summary>
        /// Deletes the record with the given key from the given store.
        /// </summary>
        [Script(@"
            var self = this;
            return new @:Promise(function(resolve, reject) {
                var tx = self.@{[TodoApp]TodoApp.Services.IndexedDbService::db}.transaction(storeName, 'readwrite');
                var store = tx.objectStore(storeName);
                var request = store['delete'](key);
                request.onsuccess = function() { resolve(true); };
                request.onerror = function(e) { reject(e.target.error || new @:Error('IndexedDB delete failed')); };
            });
        ")]
        public extern Promise<bool> DeleteRaw(string storeName, string key);
    }
}
