using Sunlight.Framework.UI.Attributes;

namespace TodoApp.ViewModels
{
    /// <summary>
    /// CSS class name constants for AppShell.css.
    /// Each const is registered with the NScript compiler via [CssClass]
    /// to enable CSS class name minification across CSS, HTML, and JS.
    /// </summary>
    public static class AppShellCss
    {
        private const string Res = "TodoApp.RazorTemplates.AppShell.css";

        // Layout panes
        [CssClass(Res + ":pane-left")]
        public const string PaneLeft = "pane-left";

        [CssClass(Res + ":pane-right")]
        public const string PaneRight = "pane-right";

        // State modifiers
        [CssClass(Res + ":collapsed")]
        public const string Collapsed = "collapsed";

        [CssClass(Res + ":hidden")]
        public const string Hidden = "hidden";

        [CssClass(Res + ":selected")]
        public const string Selected = "selected";

        [CssClass(Res + ":completed")]
        public const string Completed = "completed";

        [CssClass(Res + ":checked")]
        public const string Checked = "checked";

        // Item classes
        [CssClass(Res + ":folder-item")]
        public const string FolderItem = "folder-item";

        [CssClass(Res + ":todo-item")]
        public const string TodoItem = "todo-item";

        [CssClass(Res + ":subtask-item")]
        public const string SubtaskItem = "subtask-item";

        [CssClass(Res + ":btn-check")]
        public const string BtnCheck = "btn-check";

        // Section classes
        [CssClass(Res + ":completed-section")]
        public const string CompletedSection = "completed-section";
    }
}
