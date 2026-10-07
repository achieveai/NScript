namespace SpreadsheetApp.ViewModels
{
    using Sunlight.Framework.UI.Attributes;

    /// <summary>CSS classes the view models compose at runtime (template-only classes need no constant).</summary>
    public static class SheetCss
    {
        private const string Res = "SpreadsheetApp.RazorTemplates.Sheet.css";

        [CssClass(Res + ":sheet-row")]
        public const string SheetRow = "sheet-row";

        [CssClass(Res + ":cell")]
        public const string Cell = "cell";

        [CssClass(Res + ":formula")]
        public const string Formula = "formula";

        [CssClass(Res + ":text-cell")]
        public const string TextCell = "text-cell";

        [CssClass(Res + ":error")]
        public const string Error = "error";

        [CssClass(Res + ":selected")]
        public const string Selected = "selected";

        [CssClass(Res + ":head")]
        public const string Head = "head";

        [CssClass(Res + ":head-selected")]
        public const string HeadSelected = "head-selected";

        [CssClass(Res + ":btn")]
        public const string Btn = "btn";

        [CssClass(Res + ":btn-on")]
        public const string BtnOn = "btn-on";
    }
}
