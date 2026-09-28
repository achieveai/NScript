namespace TodoApp.Controls
{
    using System;
    using System.Web.Html;
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;

    public class TitleEditor : UISkinableElement
    {
        public TitleEditor(Element element) : base(element)
        {
        }

        [Skin("TodoApp.RazorTemplates.TitleEditor.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }

        [AutoFire]
        [DefaultDataBinding(Mode = DataBindingMode.TwoWay)]
        public string Value { get; set; }

        [AutoFire]
        public Action OnCommitted { get; set; }

        public void Commit(Element element, ElementEvent ev)
        {
            this.Value = ((InputElement)element).Value;
            if (this.OnCommitted != null)
                this.OnCommitted();
        }
    }
}
