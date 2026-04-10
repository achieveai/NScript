namespace TodoApp.Services
{
    using System;
    using Sunlight.Framework.Observables;
    using TodoApp.ViewModels;

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
        public Promise<bool> SaveTodo(string id, string folderId, string title, bool isCompleted, bool isImportant, bool isMyDay, string dueDate, string notes, ObservableCollection<SubTaskViewModel> subTasks)
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
            json = json + ",\"SubTasks\":" + BuildSubTasksJson(subTasks);
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
        /// Escapes a string for safe embedding in a JSON value (RFC 8259).
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
            result = result.Replace("\n", "\\n");
            result = result.Replace("\r", "\\r");
            result = result.Replace("\t", "\\t");
            result = result.Replace("\b", "\\b");
            result = result.Replace("\f", "\\f");
            return result;
        }

        private static string BuildSubTasksJson(ObservableCollection<SubTaskViewModel> subTasks)
        {
            if (subTasks == null || subTasks.Count == 0)
            {
                return "[]";
            }

            string json = "[";
            for (int i = 0; i < subTasks.Count; i++)
            {
                if (i > 0)
                {
                    json = json + ",";
                }

                var subTask = subTasks[i];
                json = json + "{";
                json = json + "\"Id\":\"" + (subTask.Id != null ? EscapeJson(subTask.Id) : "") + "\"";
                json = json + ",\"Title\":\"" + (subTask.Title != null ? EscapeJson(subTask.Title) : "") + "\"";
                json = json + ",\"IsCompleted\":" + (subTask.IsCompleted ? "true" : "false");
                json = json + "}";
            }

            json = json + "]";
            return json;
        }
    }
}
