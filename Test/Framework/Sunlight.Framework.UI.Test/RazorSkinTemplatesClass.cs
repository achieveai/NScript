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

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorOneTimeText.skin.cshtml")]
        public static Skin RazorOneTimeText
        {
            get
            { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorMultiBinding.skin.cshtml")]
        public static Skin RazorMultiBinding
        {
            get
            { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.GraphSimpleText.skin.cshtml")]
        public static Skin GraphSimpleText
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.GraphMultiBinding.skin.cshtml")]
        public static Skin GraphMultiBinding
        {
            get { return null; }
        }
    }
}
