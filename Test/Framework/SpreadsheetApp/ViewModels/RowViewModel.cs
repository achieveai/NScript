namespace SpreadsheetApp.ViewModels
{
    using Sunlight.Framework.Observables;

    /// <summary>One grid row: its 1-based label and its cells, left to right.</summary>
    public class RowViewModel : ObservableObject
    {
        private string label;
        private ObservableCollection<CellViewModel> cells;

        public RowViewModel(int row)
        {
            this.label = (row + 1).ToString();
            this.cells = new ObservableCollection<CellViewModel>();
        }

        public string Label
        {
            get { return this.label; }
        }

        public string CssClass
        {
            get { return SheetCss.SheetRow; }
        }

        public ObservableCollection<CellViewModel> Cells
        {
            get { return this.cells; }
        }

        public void Unbind()
        {
            for (int i = 0; i < this.cells.Count; i++) this.cells[i].Unsubscribe();
        }
    }
}
