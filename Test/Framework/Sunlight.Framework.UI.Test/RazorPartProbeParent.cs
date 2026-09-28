namespace Sunlight.Framework.UI.Test
{
    using System.Web.Html;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    public class RazorPartProbeParent : UISkinableElement
    {
        public RazorPartProbeParent(Element element)
            : base(element)
        {
        }

        [AutoFire] public UIElement ProbePart { get; private set; }

        protected override void ApplySkinInternal(SkinInstance skin)
        {
            base.ApplySkinInternal(skin);
            this.ProbePart = (UIElement)skin.GetChildById("probe");
        }
    }
}
