namespace SpreadsheetApp.Formula
{
    /// <summary>
    /// A1-style reference: zero-based column and row with an absolute flag on
    /// each axis ($A$1). Converts to and from text.
    /// </summary>
    public class CellRef
    {
        public int Col;
        public int Row;
        public bool ColAbsolute;
        public bool RowAbsolute;

        public CellRef(int col, int row, bool colAbsolute, bool rowAbsolute)
        {
            this.Col = col;
            this.Row = row;
            this.ColAbsolute = colAbsolute;
            this.RowAbsolute = rowAbsolute;
        }

        /// <summary>0 -> A, 25 -> Z, 26 -> AA.</summary>
        public static string ColumnName(int col)
        {
            string name = "";
            int c = col;
            while (true)
            {
                name = string.FromCharCode((char)(65 + (c % 26))) + name;
                c = (c / 26) - 1;
                if (c < 0) break;
            }
            return name;
        }

        public static string CellName(int col, int row)
        {
            return ColumnName(col) + (row + 1).ToString();
        }

        public string Text()
        {
            return (this.ColAbsolute ? "$" : "") + ColumnName(this.Col)
                + (this.RowAbsolute ? "$" : "") + (this.Row + 1).ToString();
        }

        /// <summary>A copy shifted by (dCol, dRow) on the relative axes only.</summary>
        public CellRef Shifted(int dCol, int dRow)
        {
            return new CellRef(
                this.ColAbsolute ? this.Col : this.Col + dCol,
                this.RowAbsolute ? this.Row : this.Row + dRow,
                this.ColAbsolute,
                this.RowAbsolute);
        }
    }
}
