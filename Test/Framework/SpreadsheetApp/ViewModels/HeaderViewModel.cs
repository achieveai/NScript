namespace SpreadsheetApp.ViewModels
{
    using Sunlight.Framework.Observables;

    /// <summary>A column header: the letter plus the column's format name.</summary>
    public class HeaderViewModel : ObservableObject
    {
        private string label;
        private string formatName;
        private string cssClass;

        public HeaderViewModel(string label)
        {
            this.label = label;
            this.formatName = "";
            this.cssClass = SheetCss.Head;
        }

        public string Label
        {
            get { return this.label; }
        }

        public string FormatName
        {
            get { return this.formatName; }
            set
            {
                if (this.formatName == value) return;
                this.formatName = value;
                base.FirePropertyChanged("FormatName");
            }
        }

        public string CssClass
        {
            get { return this.cssClass; }
        }

        public void SetSelected(bool selected)
        {
            string next = selected ? SheetCss.Head + " " + SheetCss.HeadSelected : SheetCss.Head;
            if (next == this.cssClass) return;
            this.cssClass = next;
            base.FirePropertyChanged("CssClass");
        }
    }
}
