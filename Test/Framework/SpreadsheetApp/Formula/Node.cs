namespace SpreadsheetApp.Formula
{
    using System.Collections.Generic;

    /// <summary>
    /// Formula syntax tree. One class with a Kind tag keeps the generated
    /// JavaScript small and avoids virtual dispatch in the evaluator.
    /// </summary>
    public class Node
    {
        public const int Number = 1;
        public const int Text = 2;
        public const int Bool = 3;
        public const int Ref = 4;
        public const int Range = 5;
        public const int Unary = 6;    // Op is "-" or "+", Left is the operand
        public const int Binary = 7;   // Op is + - * / ^ & = <> < > <= >=
        public const int Percent = 8;  // Left / 100
        public const int Call = 9;     // Name(Args)
        public const int Error = 10;   // a literal error such as #REF!

        public int Kind;
        public double NumberValue;
        public string TextValue;     // text literal, error code, or function name
        public bool BoolValue;
        public CellRef RefValue;     // Ref, or the range start
        public CellRef RefEnd;       // range end
        public string Op;
        public Node Left;
        public Node Right;
        public List<Node> Args;

        public Node(int kind)
        {
            this.Kind = kind;
        }

        public static Node MakeNumber(double v) { var n = new Node(Number); n.NumberValue = v; return n; }
        public static Node MakeText(string s) { var n = new Node(Text); n.TextValue = s; return n; }
        public static Node MakeBool(bool b) { var n = new Node(Bool); n.BoolValue = b; return n; }
        public static Node MakeRef(CellRef r) { var n = new Node(Ref); n.RefValue = r; return n; }
        public static Node MakeRange(CellRef a, CellRef b) { var n = new Node(Range); n.RefValue = a; n.RefEnd = b; return n; }
        public static Node MakeUnary(string op, Node operand) { var n = new Node(Unary); n.Op = op; n.Left = operand; return n; }
        public static Node MakeBinary(string op, Node l, Node r) { var n = new Node(Binary); n.Op = op; n.Left = l; n.Right = r; return n; }
        public static Node MakePercent(Node operand) { var n = new Node(Percent); n.Left = operand; return n; }
        public static Node MakeCall(string name, List<Node> args) { var n = new Node(Call); n.TextValue = name; n.Args = args; return n; }
        public static Node MakeError(string code) { var n = new Node(Error); n.TextValue = code; return n; }
    }
}
