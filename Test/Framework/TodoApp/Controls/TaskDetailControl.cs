namespace TodoApp.Controls
{
    using System.Web.Html;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    public class TaskDetailControl : UISkinableElement
    {
        public TaskDetailControl(Element element) : base(element)
        {
        }

        [Skin("TodoApp.RazorTemplates.TaskDetailControl.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }

        [AutoFire]
        public TitleEditor Editor { get; private set; }

        protected override void ApplySkinInternal(SkinInstance skin)
        {
            base.ApplySkinInternal(skin);
            this.Editor = (TitleEditor)skin.GetChildById("titleEditor");
        }
    }
}
