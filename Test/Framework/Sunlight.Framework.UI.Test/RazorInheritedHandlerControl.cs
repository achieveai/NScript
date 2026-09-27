namespace Sunlight.Framework.UI.Test
{
    using System.Web.Html;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    public class RazorHandlerBase : UISkinableElement
    {
        public static int ClickCount;

        public RazorHandlerBase(Element element) : base(element) { }

        public void HandleClick(Element element, ElementEvent evt)
        {
            ClickCount++;
        }
    }

    public class RazorInheritedHandlerControl : RazorHandlerBase
    {
        public RazorInheritedHandlerControl(Element element) : base(element) { }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorInheritedHandlerControl.skin.cshtml")]
        public static Skin DefaultSkin => null;
    }
}
