namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;

    /// <summary>
    /// Razor skin template registrations for browser-based tests.
    /// Each [Skin] property points to a .skin.cshtml embedded resource.
    /// The RazorTemplatingPlugin compiles these at build time.
    /// </summary>
    public class RazorSkinTemplatesClass
    {
        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSimpleText.skin.cshtml")]
        public static Skin RazorSimpleText
        {
            get
            { return null; }
        }
    }
}
