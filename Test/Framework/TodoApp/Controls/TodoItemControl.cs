namespace TodoApp.Controls
{
    using System;
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

        [Skin("TodoApp.RazorTemplates.TodoItemControlCompact.skin.cshtml")]
        public static Skin CompactSkin
        {
            get { return null; }
        }

        [AutoFire]
        public bool ShowStar { get; set; }

        [AutoFire]
        public Action OnSelected { get; set; }

        protected override void OnActivate()
        {
            this.OnClick += this.HandleClick;
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            this.OnClick -= this.HandleClick;
            base.OnDeactivate();
        }

        private void HandleClick(UIEvent ev)
        {
            if (this.OnSelected != null)
                this.OnSelected();
        }
    }
}
