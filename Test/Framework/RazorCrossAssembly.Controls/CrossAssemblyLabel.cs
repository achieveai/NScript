namespace RazorCrossAssembly.Controls
{
    using System.Web.Html;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;

    public class CrossAssemblyLabel : UISkinableElement
    {
        private string text;

        public CrossAssemblyLabel(Element element)
            : base(element)
        {
        }

        [Skin("RazorCrossAssembly.Controls.RazorTemplates.CrossAssemblyLabel.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }

        [Skin("RazorCrossAssembly.Controls.RazorTemplates.SharedName.skin.cshtml")]
        public static Skin SharedSkin
        {
            get { return null; }
        }

        public string Text
        {
            get { return this.text; }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.Element.SetAttribute("data-cross-text", value);
                    this.FirePropertyChanged("Text");
                }
            }
        }
    }
}
