namespace Sunlight.Framework.UI.Test
{
    using System.Web.Html;
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    public class RazorValueProbeControl : UISkinableElement
    {
        private string value;

        public static string ActivationValueForTest;
        public static RazorValueProbeControl LastCreated;

        public RazorValueProbeControl(Element element)
            : base(element)
        {
            LastCreated = this;
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorValueProbeControl.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }

        [DefaultDataBinding(Mode = DataBindingMode.TwoWay)]
        public string Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.Element.SetAttribute("data-probe-value", value);
                    this.FirePropertyChanged("Value");
                }
            }
        }

        protected override void OnActivate()
        {
            if (ActivationValueForTest != null)
                this.Value = ActivationValueForTest;
            base.OnActivate();
        }
    }
}
