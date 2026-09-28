namespace Sunlight.Framework.UI.Test
{
    using System;
    using System.Web.Html;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    public class RazorProbeControl : UISkinableElement
    {
        private string text;

        public static int CreatedCount;
        public static int DeactivatedCount;
        public static int DisposedCount;
        public static string LastTextAtActivate;
        public static RazorProbeControl LastCreated;
        public static Action<RazorProbeControl> TextChangedForTest;

        public RazorProbeControl(Element element)
            : base(element)
        {
            CreatedCount++;
            LastCreated = this;
        }

        protected override void InternalDispose()
        {
            DisposedCount++;
            base.InternalDispose();
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorProbeControl.skin.cshtml")]
        public static Skin DefaultSkin
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
                    this.Element.SetAttribute("data-bound-text", value);
                    this.FirePropertyChanged("Text");
                    if (TextChangedForTest != null)
                        TextChangedForTest(this);
                }
            }
        }

        [AutoFire] public Action Click { get; set; }

        protected override void OnActivate()
        {
            LastTextAtActivate = this.Text;
            this.OnClick += this.HandleClick;
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            this.OnClick -= this.HandleClick;
            DeactivatedCount++;
            base.OnDeactivate();
        }

        private void HandleClick(UIEvent evt)
        {
            if (this.Click != null)
            {
                this.Click();
            }
        }
    }
}
