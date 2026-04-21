namespace TodoApp.Services
{
    using System.Runtime.CompilerServices;
    using Sunlight.Framework.Data.WebStore;

    /// <summary>
    /// Persisted shape of a user-created folder. [JsonType] + extern properties
    /// with explicit [ScriptName] preserve exact field casing across minification
    /// so IndexedDB structured-clone round-trips stay stable.
    /// </summary>
    [JsonType]
    public class FolderEntity
    {
        [ScriptName("Id")]
        public extern string Id { get; set; }

        [ScriptName("Name")]
        public extern string Name { get; set; }

        [ScriptName("Icon")]
        public extern string Icon { get; set; }

        [ScriptName("SortOrder")]
        public extern int SortOrder { get; set; }
    }

    /// <summary>
    /// Persisted shape of a single subtask nested inside a <see cref="TodoEntity"/>.
    /// </summary>
    [JsonType]
    public class SubTaskEntity
    {
        [ScriptName("Id")]
        public extern string Id { get; set; }

        [ScriptName("Title")]
        public extern string Title { get; set; }

        [ScriptName("IsCompleted")]
        public extern bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Persisted shape of a todo record. Subtasks are stored inline as an array
    /// on the same record rather than a separate object store.
    /// </summary>
    [JsonType]
    public class TodoEntity
    {
        [ScriptName("Id")]
        public extern string Id { get; set; }

        [ScriptName("FolderId")]
        public extern string FolderId { get; set; }

        [ScriptName("Title")]
        public extern string Title { get; set; }

        [ScriptName("IsCompleted")]
        public extern bool IsCompleted { get; set; }

        [ScriptName("IsImportant")]
        public extern bool IsImportant { get; set; }

        [ScriptName("IsMyDay")]
        public extern bool IsMyDay { get; set; }

        [ScriptName("DueDate")]
        public extern string DueDate { get; set; }

        [ScriptName("Notes")]
        public extern string Notes { get; set; }

        [ScriptName("SubTasks")]
        public extern SubTaskEntity[] SubTasks { get; set; }
    }

    /// <summary>
    /// Schema declaration for the TodoApp IndexedDB database. Exposes the
    /// canonical table names + a factory that builds the <see cref="WebStoreSchema"/>
    /// consumed by <see cref="WebStoreFactory.Create"/>.
    /// </summary>
    public static class TodoWebStoreSchema
    {
        public const string DatabaseName = "TodoAppDb";
        public const int VersionId = 1;
        public const string FoldersTable = "folders";
        public const string TodosTable = "todos";
        public const string FolderIdIndex = "folderId";

        public static WebStoreSchema Create()
        {
            return new WebStoreSchema(
                DatabaseName,
                VersionId,
                new TableSchema(
                    FoldersTable,
                    new KeyInfo("Id", false)),
                new TableSchema(
                    TodosTable,
                    new KeyInfo("Id", false),
                    new SingleIndexInfo(FolderIdIndex, "FolderId", false, false)));
        }
    }
}
