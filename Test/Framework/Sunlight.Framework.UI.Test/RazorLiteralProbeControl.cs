namespace Competing
{
    public enum RazorProbeMode
    {
        Slow = 90,
        Fast = 99
    }
}

namespace Sunlight.Framework.UI.Test
{
    using System.Web.Html;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    public enum RazorProbeMode
    {
        Slow,
        Fast
    }

    public class RazorLiteralProbeControl : UISkinableElement
    {
        public static RazorLiteralProbeControl LastCreated;

        public RazorLiteralProbeControl(Element element) : base(element)
        {
            LastCreated = this;
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorLiteralProbeControl.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }

        [Skin("RazorLiteralProbeControl")]
        public static Skin ShortSkin
        {
            get { return null; }
        }

        [Skin("SharedName")]
        public static Skin AmbiguousSkin
        {
            get { return null; }
        }

        [AutoFire] public string Text { get; set; }
        [AutoFire] public bool IsOn { get; set; }
        [AutoFire] public int Count { get; set; }
        [AutoFire] public RazorProbeMode Mode { get; set; }
    }
}
