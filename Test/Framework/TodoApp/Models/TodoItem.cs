namespace TodoApp.Models
{
    /// <summary>
    /// Plain data model for a todo task item.
    /// </summary>
    public class TodoItem
    {
        public string Id { get; set; }
        public string FolderId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsImportant { get; set; }
        public string DueDate { get; set; }
        public string Notes { get; set; }
        public bool IsMyDay { get; set; }
        public string CreatedAt { get; set; }
    }
}
