namespace TodoApp.Models
{
    /// <summary>
    /// Plain data model for a todo folder/list.
    /// </summary>
    public class TodoFolder
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsSystem { get; set; }
        public string SystemType { get; set; }
        public int SortOrder { get; set; }
    }
}
