namespace Sunlight.Framework.UI.Test
{
    using System.Web.Html;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    public class RazorContextProbeControl : UISkinableElement
    {
        public RazorContextProbeControl(Element element)
            : base(element)
        {
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorContextProbeControl.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }
    }
}
