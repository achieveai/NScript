namespace Sunlight.Framework.UI.Test
{
    using System.Web.Html;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    /// <summary>Control-typed fixture for issue #102: Control.X inside computed expressions.</summary>
    public class RazorRawGetterControl : UISkinableElement
    {
        public RazorRawGetterControl(Element element) : base(element)
        {
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorRawGetterControl.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }

        [AutoFire] public bool Armed { get; set; }
        [AutoFire] public string Label { get; set; }
    }
}
