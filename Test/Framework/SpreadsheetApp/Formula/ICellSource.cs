namespace SpreadsheetApp.Formula
{
    /// <summary>What the evaluator reads: cell values by zero-based column and row.</summary>
    public interface ICellSource
    {
        int ColumnCount { get; }
        int RowCount { get; }
        CellValue GetValue(int col, int row);
    }
}
