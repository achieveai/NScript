namespace SpreadsheetApp.ViewModels
{
    using Sunlight.Framework.Observables;

    /// <summary>One line of the benchmark table.</summary>
    public class BenchResultViewModel : ObservableObject
    {
        private string op;
        private int rows;
        private string mode;
        private double ms;
        private int writes;

        public BenchResultViewModel(string op, int rows, string mode, double ms, int writes)
        {
            this.op = op;
            this.rows = rows;
            this.mode = mode;
            this.ms = ms;
            this.writes = writes;
        }

        public string Op { get { return this.op; } }
        public int Rows { get { return this.rows; } }
        public string Mode { get { return this.mode; } }
        public double Ms { get { return this.ms; } }
        public int Writes { get { return this.writes; } }

        public string RowsText { get { return this.rows.ToString(); } }
        public string MsText { get { return this.ms.ToFixed(1); } }
        public string WritesText { get { return this.writes.ToString(); } }
    }
}
