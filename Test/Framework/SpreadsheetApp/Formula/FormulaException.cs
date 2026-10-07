namespace SpreadsheetApp.Formula
{
    using System;

    /// <summary>
    /// A formula that cannot be parsed or evaluated. Code is the sheet error
    /// shown in the cell, such as "#NAME?" or "#DIV/0!".
    /// </summary>
    public class FormulaException : Exception
    {
        private string code;

        public FormulaException(string code, string message)
            : base(message)
        {
            this.code = code;
        }

        public string Code
        {
            get { return this.code; }
        }
    }
}
