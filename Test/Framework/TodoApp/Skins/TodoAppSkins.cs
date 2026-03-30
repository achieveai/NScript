namespace TodoApp.Skins
{
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;

    /// <summary>
    /// Razor skin template registrations for the TodoApp.
    /// Each [Skin] property points to a .skin.cshtml embedded resource.
    /// The RazorTemplatingPlugin compiles these at build time.
    /// </summary>
    public class TodoAppSkins
    {
        [Skin("TodoApp.RazorTemplates.AppShell.skin.cshtml")]
        public static Skin AppShell
        {
            get { return null; }
        }
    }
}
