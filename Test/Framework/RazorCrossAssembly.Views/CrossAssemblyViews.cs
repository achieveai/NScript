namespace RazorCrossAssembly.Views
{
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;

    public class CrossAssemblyViews
    {
        [Skin("RazorCrossAssembly.Views.RazorTemplates.CrossAssemblyParent.skin.cshtml")]
        public static Skin Parent
        {
            get { return null; }
        }

        [Skin("RazorCrossAssembly.Views.RazorTemplates.SharedName.skin.cshtml")]
        public static Skin SharedSkin
        {
            get { return null; }
        }
    }
}
