namespace SpreadsheetApp.Skins
{
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;

    public class SpreadsheetSkins
    {
        [Skin("SpreadsheetApp.RazorTemplates.Sheet.skin.cshtml")]
        public static Skin Sheet
        {
            get { return null; }
        }
    }
}
