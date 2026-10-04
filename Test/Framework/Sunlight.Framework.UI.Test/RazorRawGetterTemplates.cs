namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    /// <summary>Skin registration for the issue #102 raw-getter fixture.</summary>
    public class RazorRawGetterTemplates
    {
        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorRawGetterAutoProps.skin.cshtml")]
        public static Skin RazorRawGetterAutoProps
        {
            get { return null; }
        }
    }
}
