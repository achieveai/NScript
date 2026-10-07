namespace SpreadsheetApp.Controls
{
    using System.Web.Html;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Attributes;

    /// <summary>
    /// One sheet row. Its skin is a nested graph (depth parent + 1) whose
    /// cell loop sits one level deeper again.
    /// </summary>
    public class RowControl : UISkinableElement
    {
        public RowControl(Element element)
            : base(element)
        {
        }

        [Skin("SpreadsheetApp.RazorTemplates.RowControl.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }
    }
}
