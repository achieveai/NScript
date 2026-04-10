namespace TodoApp.Models
{
    /// <summary>
    /// Plain data model for a subtask (step) within a todo item.
    /// </summary>
    public class SubTask
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
