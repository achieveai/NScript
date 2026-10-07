namespace SpreadsheetApp.Formula
{
    /// <summary>
    /// A cell's computed value: empty, number, text, boolean, or an error code
    /// such as "#DIV/0!". Dates are numbers (milliseconds since the epoch) whose
    /// display is chosen by the cell format, like serial dates in a real sheet.
    /// </summary>
    public class CellValue
    {
        public const int KindEmpty = 0;
        public const int KindNumber = 1;
        public const int KindText = 2;
        public const int KindBool = 3;
        public const int KindError = 4;

        public static readonly CellValue Empty = new CellValue(KindEmpty, 0, "", false);

        private int kind;
        private double number;
        private string text;
        private bool flag;

        private CellValue(int kind, double number, string text, bool flag)
        {
            this.kind = kind;
            this.number = number;
            this.text = text;
            this.flag = flag;
        }

        public static CellValue Number(double n) { return new CellValue(KindNumber, n, "", false); }
        public static CellValue Text(string s) { return new CellValue(KindText, 0, s, false); }
        public static CellValue Bool(bool b) { return new CellValue(KindBool, 0, "", b); }
        public static CellValue Error(string code) { return new CellValue(KindError, 0, code, false); }

        public int Kind { get { return this.kind; } }
        public bool IsEmpty { get { return this.kind == KindEmpty; } }
        public bool IsNumber { get { return this.kind == KindNumber; } }
        public bool IsText { get { return this.kind == KindText; } }
        public bool IsBool { get { return this.kind == KindBool; } }
        public bool IsError { get { return this.kind == KindError; } }

        public double NumberValue { get { return this.number; } }
        public string TextValue { get { return this.text; } }
        public bool BoolValue { get { return this.flag; } }
        public string ErrorCode { get { return this.text; } }

        /// <summary>Number view used by arithmetic: empty is 0, bool is 0/1, text is 0.</summary>
        public double AsNumber()
        {
            if (this.kind == KindNumber) return this.number;
            if (this.kind == KindBool) return this.flag ? 1 : 0;
            return 0;
        }

        public bool AsBool()
        {
            if (this.kind == KindBool) return this.flag;
            if (this.kind == KindNumber) return this.number != 0;
            if (this.kind == KindText) return this.text != "";
            return false;
        }

        public string AsText()
        {
            if (this.kind == KindText || this.kind == KindError) return this.text;
            if (this.kind == KindBool) return this.flag ? "TRUE" : "FALSE";
            if (this.kind == KindNumber) return this.number.ToString();
            return "";
        }

        public static bool AreEqual(CellValue a, CellValue b)
        {
            if (a.kind != b.kind)
            {
                bool numeric = (a.kind == KindEmpty || a.kind == KindNumber) && (b.kind == KindEmpty || b.kind == KindNumber);
                return numeric && a.AsNumber() == b.AsNumber();
            }
            if (a.kind == KindNumber) return a.number == b.number;
            if (a.kind == KindText) return a.text.ToLowerCase() == b.text.ToLowerCase();
            if (a.kind == KindError) return a.text == b.text;
            if (a.kind == KindBool) return a.flag == b.flag;
            return true;
        }
    }
}
