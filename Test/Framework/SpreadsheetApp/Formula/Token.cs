namespace SpreadsheetApp.Formula
{
    public class Token
    {
        public const int Number = 1;
        public const int TextKind = 2;
        public const int Ref = 3;        // A1, $A$1
        public const int Name = 4;       // SUM, TRUE
        public const int Op = 5;         // + - * / ^ & = <> < > <= >= %
        public const int LParen = 6;
        public const int RParen = 7;
        public const int Comma = 8;
        public const int Colon = 9;
        public const int End = 10;
        public const int ErrorKind = 11;  // #REF!, #DIV/0!

        public int Kind;
        public string Text;
        public double NumberValue;
        public CellRef RefValue;
        public int Position;

        public Token(int kind, string text, int position)
        {
            this.Kind = kind;
            this.Text = text;
            this.Position = position;
        }
    }
}
