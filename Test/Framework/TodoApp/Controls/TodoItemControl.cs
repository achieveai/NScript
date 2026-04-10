namespace TodoApp.Controls
{
    using System.Web.Html;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;

    /// <summary>
    /// Reusable control for rendering a single todo list item.
    /// Displays checkbox, title, and star icon.
    /// Data context is TodoItemViewModel.
    /// </summary>
    public class TodoItemControl : UISkinableElement
    {
        public TodoItemControl(Element element)
            : base(element)
        {
        }

        [Skin("TodoApp.RazorTemplates.TodoItemControl.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }
    }
}
