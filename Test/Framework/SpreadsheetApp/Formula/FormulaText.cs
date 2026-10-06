namespace SpreadsheetApp.Formula
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Tree utilities: serialize back to text, shift references for copy/paste,
    /// and list the cells a formula reads (its precedents).
    /// </summary>
    public static class FormulaText
    {
        /// <summary>Renders the tree as formula text without the leading '='.</summary>
        public static string ToText(Node node)
        {
            switch (node.Kind)
            {
                case Node.Number: return node.NumberValue.ToString();
                case Node.Text: return "\"" + node.TextValue.Replace("\"", "\"\"") + "\"";
                case Node.Bool: return node.BoolValue ? "TRUE" : "FALSE";
                case Node.Error: return node.TextValue;
                case Node.Ref: return node.RefValue.Text();
                case Node.Range: return node.RefValue.Text() + ":" + node.RefEnd.Text();
                case Node.Unary: return node.Op + Wrap(node.Left, node);
                case Node.Percent: return Wrap(node.Left, node) + "%";
                case Node.Binary: return Wrap(node.Left, node) + node.Op + Wrap(node.Right, node);
                case Node.Call:
                {
                    string s = node.TextValue + "(";
                    for (int i = 0; i < node.Args.Count; i++)
                    {
                        if (i > 0) s = s + ",";
                        s = s + ToText(node.Args[i]);
                    }
                    return s + ")";
                }
            }
            return "";
        }

        private static string Wrap(Node child, Node parent)
        {
            string text = ToText(child);
            return Precedence(child) < Precedence(parent) ? "(" + text + ")" : text;
        }

        private static int Precedence(Node node)
        {
            if (node.Kind == Node.Binary)
            {
                string op = node.Op;
                if (op == "=" || op == "<>" || op == "<" || op == ">" || op == "<=" || op == ">=") return 1;
                if (op == "&") return 2;
                if (op == "+" || op == "-") return 3;
                if (op == "*" || op == "/") return 4;
                return 5; // ^
            }
            if (node.Kind == Node.Unary) return 6;
            if (node.Kind == Node.Percent) return 7;
            return 8;
        }

        /// <summary>
        /// Copy of the tree with every relative reference moved by (dCol, dRow).
        /// A reference pushed off the sheet becomes #REF!.
        /// </summary>
        public static Node Shift(Node node, int dCol, int dRow)
        {
            switch (node.Kind)
            {
                case Node.Ref:
                {
                    CellRef r = node.RefValue.Shifted(dCol, dRow);
                    return r.Col < 0 || r.Row < 0 ? Node.MakeError("#REF!") : Node.MakeRef(r);
                }
                case Node.Range:
                {
                    CellRef a = node.RefValue.Shifted(dCol, dRow);
                    CellRef b = node.RefEnd.Shifted(dCol, dRow);
                    if (a.Col < 0 || a.Row < 0 || b.Col < 0 || b.Row < 0) return Node.MakeError("#REF!");
                    return Node.MakeRange(a, b);
                }
                case Node.Unary: return Node.MakeUnary(node.Op, Shift(node.Left, dCol, dRow));
                case Node.Percent: return Node.MakePercent(Shift(node.Left, dCol, dRow));
                case Node.Binary: return Node.MakeBinary(node.Op, Shift(node.Left, dCol, dRow), Shift(node.Right, dCol, dRow));
                case Node.Call:
                {
                    var args = new List<Node>();
                    for (int i = 0; i < node.Args.Count; i++) args.Add(Shift(node.Args[i], dCol, dRow));
                    return Node.MakeCall(node.TextValue, args);
                }
            }
            return node;
        }

        /// <summary>
        /// Appends every cell the formula reads, ranges expanded and clipped to
        /// the sheet bounds. Order follows the formula text.
        /// </summary>
        public static void CollectPrecedents(Node node, int columnCount, int rowCount, List<CellRef> cells)
        {
            switch (node.Kind)
            {
                case Node.Ref:
                    if (InBounds(node.RefValue, columnCount, rowCount)) cells.Add(node.RefValue);
                    return;
                case Node.Range:
                {
                    int c0 = Math.Max(0, Math.Min(node.RefValue.Col, node.RefEnd.Col));
                    int c1 = Math.Min(columnCount - 1, Math.Max(node.RefValue.Col, node.RefEnd.Col));
                    int r0 = Math.Max(0, Math.Min(node.RefValue.Row, node.RefEnd.Row));
                    int r1 = Math.Min(rowCount - 1, Math.Max(node.RefValue.Row, node.RefEnd.Row));
                    for (int r = r0; r <= r1; r++)
                        for (int c = c0; c <= c1; c++)
                            cells.Add(new CellRef(c, r, false, false));
                    return;
                }
                case Node.Unary:
                case Node.Percent:
                    CollectPrecedents(node.Left, columnCount, rowCount, cells);
                    return;
                case Node.Binary:
                    CollectPrecedents(node.Left, columnCount, rowCount, cells);
                    CollectPrecedents(node.Right, columnCount, rowCount, cells);
                    return;
                case Node.Call:
                    for (int i = 0; i < node.Args.Count; i++)
                        CollectPrecedents(node.Args[i], columnCount, rowCount, cells);
                    return;
            }
        }

        private static bool InBounds(CellRef r, int columnCount, int rowCount)
        {
            return r.Col >= 0 && r.Row >= 0 && r.Col < columnCount && r.Row < rowCount;
        }
    }
}
