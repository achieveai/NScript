namespace TodoApp.Services
{
    using System;
    using System.Collections.Generic;
    using Sunlight.Framework.Data.WebStore;

    /// <summary>
    /// Typed data access service over <see cref="WebStoreClient"/>. Replaces the
    /// previous manual-JSON IndexedDbService with a strongly-typed layer so
    /// callers never touch raw JSON or JS interop.
    /// </summary>
    public class TodoDataService
    {
        private WebStoreClient client;

        /// <summary>
        /// Opens the underlying database and caches the resulting client.
        /// Must complete before any other operation on this service is called.
        /// </summary>
        public Promise<bool> Initialize()
        {
            return new Promise<bool>(delegate(Action<bool> resolve, Action<object> reject)
            {
                var factory = new WebStoreFactory();
                factory.Create(TodoWebStoreSchema.Create()).Then<bool>(delegate(WebStoreClient opened)
                {
                    this.client = opened;
                    resolve(true);
                    return true;
                });
            });
        }

        /// <summary>
        /// Inserts-or-replaces a todo record. The record is stored as-is; no
        /// serialization metadata is included.
        /// </summary>
        public Promise<string> SaveTodo(TodoEntity todo)
        {
            return this.TodosTable().UpSert(todo);
        }

        /// <summary>
        /// Loads every todo record. Caller is expected to translate to view models.
        /// </summary>
        public Promise<List<TodoEntity>> GetAllTodos()
        {
            return this.TodosTable().Query(Query.All);
        }

        /// <summary>Deletes a single todo by id. Resolves true on success.</summary>
        public Promise<bool> DeleteTodo(string id)
        {
            return this.TodosTable().Delete(id);
        }

        /// <summary>
        /// Inserts-or-replaces a user-created folder. System folders are not
        /// persisted — they are recreated on startup.
        /// </summary>
        public Promise<string> SaveFolder(FolderEntity folder)
        {
            return this.FoldersTable().UpSert(folder);
        }

        /// <summary>Loads every user-created folder record.</summary>
        public Promise<List<FolderEntity>> GetAllFolders()
        {
            return this.FoldersTable().Query(Query.All);
        }

        /// <summary>Deletes a single folder by id. Resolves true on success.</summary>
        public Promise<bool> DeleteFolder(string id)
        {
            return this.FoldersTable().Delete(id);
        }

        private WebStoreTable<string, TodoEntity> TodosTable()
        {
            return this.client.Table<string, TodoEntity>(TodoWebStoreSchema.TodosTable);
        }

        private WebStoreTable<string, FolderEntity> FoldersTable()
        {
            return this.client.Table<string, FolderEntity>(TodoWebStoreSchema.FoldersTable);
        }
    }
}
