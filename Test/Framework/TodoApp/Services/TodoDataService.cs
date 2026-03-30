namespace TodoApp.Services
{
    using System;

    /// <summary>
    /// Typed data access service. Wraps IndexedDbService with C#-friendly methods
    /// that build JSON strings manually to ensure clean storage (no framework metadata).
    /// </summary>
    public class TodoDataService
    {
        private IndexedDbService dbService;

        public TodoDataService(IndexedDbService dbService)
        {
            this.dbService = dbService;
        }

        /// <summary>
        /// Opens the IndexedDB database. Must be called before any read/write operations.
        /// </summary>
        public Promise<bool> Initialize()
        {
            return this.dbService.Open();
        }

        /// <summary>
        /// Persists a todo item to the 'todos' object store.
        /// All fields are serialized as a plain JSON object with well-known keys.
        /// </summary>
        public Promise<bool> SaveTodo(string id, string folderId, string title, bool isCompleted, bool isImportant, bool isMyDay, string dueDate, string notes)
        {
            string json = "{";
            json = json + "\"Id\":\"" + id + "\"";
            json = json + ",\"FolderId\":\"" + folderId + "\"";
            json = json + ",\"Title\":\"" + EscapeJson(title) + "\"";
            json = json + ",\"IsCompleted\":" + (isCompleted ? "true" : "false");
            json = json + ",\"IsImportant\":" + (isImportant ? "true" : "false");
            json = json + ",\"IsMyDay\":" + (isMyDay ? "true" : "false");
            json = json + ",\"DueDate\":\"" + (dueDate != null ? EscapeJson(dueDate) : "") + "\"";
            json = json + ",\"Notes\":\"" + (notes != null ? EscapeJson(notes) : "") + "\"";
            json = json + "}";
            return this.dbService.PutRaw("todos", json);
        }

        /// <summary>
        /// Loads all todos from the 'todos' object store as a JSON array string.
        /// </summary>
        public Promise<string> GetAllTodos()
        {
            return this.dbService.GetAllRaw("todos");
        }

        /// <summary>
        /// Deletes a todo by ID from the 'todos' object store.
        /// </summary>
        public Promise<bool> DeleteTodo(string id)
        {
            return this.dbService.DeleteRaw("todos", id);
        }

        /// <summary>
        /// Persists a user-created folder to the 'folders' object store.
        /// System folders are not persisted — they are always recreated on startup.
        /// </summary>
        public Promise<bool> SaveFolder(string id, string name, string icon, int sortOrder)
        {
            string json = "{";
            json = json + "\"Id\":\"" + id + "\"";
            json = json + ",\"Name\":\"" + EscapeJson(name) + "\"";
            json = json + ",\"Icon\":\"" + EscapeJson(icon) + "\"";
            json = json + ",\"SortOrder\":" + sortOrder;
            json = json + "}";
            return this.dbService.PutRaw("folders", json);
        }

        /// <summary>
        /// Loads all user-created folders from the 'folders' object store as a JSON array string.
        /// </summary>
        public Promise<string> GetAllFolders()
        {
            return this.dbService.GetAllRaw("folders");
        }

        /// <summary>
        /// Deletes a folder by ID from the 'folders' object store.
        /// </summary>
        public Promise<bool> DeleteFolder(string id)
        {
            return this.dbService.DeleteRaw("folders", id);
        }

        /// <summary>
        /// Escapes a string for safe embedding in a JSON value.
        /// Handles backslashes and double-quotes.
        /// </summary>
        private static string EscapeJson(string input)
        {
            if (input == null)
            {
                return "";
            }

            string result = input;
            result = result.Replace("\\", "\\\\");
            result = result.Replace("\"", "\\\"");
            return result;
        }
    }
}
