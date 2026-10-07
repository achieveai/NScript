namespace SpreadsheetApp.Formula
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Evaluates a parsed formula against a cell source. Errors become error
    /// values (never exceptions) so a bad cell shows "#DIV/0!" instead of
    /// breaking the sheet.
    /// </summary>
    public class Evaluator
    {
        private ICellSource source;

        public Evaluator(ICellSource source)
        {
            this.source = source;
        }

        public CellValue Evaluate(Node node)
        {
            try
            {
                return this.Eval(node);
            }
            catch (FormulaException ex)
            {
                return CellValue.Error(ex.Code);
            }
        }

        private CellValue Eval(Node node)
        {
            switch (node.Kind)
            {
                case Node.Number: return CellValue.Number(node.NumberValue);
                case Node.Text: return CellValue.Text(node.TextValue);
                case Node.Bool: return CellValue.Bool(node.BoolValue);
                case Node.Error: return CellValue.Error(node.TextValue);
                case Node.Ref: return this.ReadCell(node.RefValue);
                case Node.Range: throw new FormulaException("#VALUE!", "A range needs a function such as SUM");
                case Node.Unary: return this.EvalUnary(node);
                case Node.Percent:
                {
                    CellValue v = this.Eval(node.Left);
                    if (v.IsError) return v;
                    return CellValue.Number(v.AsNumber() / 100);
                }
                case Node.Binary: return this.EvalBinary(node);
                case Node.Call: return this.EvalCall(node);
            }
            throw new FormulaException("#NAME?", "Unknown node");
        }

        private CellValue ReadCell(CellRef r)
        {
            if (r.Col < 0 || r.Row < 0 || r.Col >= this.source.ColumnCount || r.Row >= this.source.RowCount)
                return CellValue.Error("#REF!");
            return this.source.GetValue(r.Col, r.Row);
        }

        private CellValue EvalUnary(Node node)
        {
            CellValue v = this.Eval(node.Left);
            if (v.IsError) return v;
            if (v.IsText) return CellValue.Error("#VALUE!");
            return CellValue.Number(node.Op == "-" ? -v.AsNumber() : v.AsNumber());
        }

        private CellValue EvalBinary(Node node)
        {
            CellValue l = this.Eval(node.Left);
            if (l.IsError) return l;
            CellValue r = this.Eval(node.Right);
            if (r.IsError) return r;
            string op = node.Op;

            if (op == "&") return CellValue.Text(l.AsText() + r.AsText());

            if (op == "=") return CellValue.Bool(CellValue.AreEqual(l, r));
            if (op == "<>") return CellValue.Bool(!CellValue.AreEqual(l, r));
            if (op == "<" || op == ">" || op == "<=" || op == ">=")
            {
                int cmp = Compare(l, r);
                if (op == "<") return CellValue.Bool(cmp < 0);
                if (op == ">") return CellValue.Bool(cmp > 0);
                if (op == "<=") return CellValue.Bool(cmp <= 0);
                return CellValue.Bool(cmp >= 0);
            }

            if (l.IsText || r.IsText) return CellValue.Error("#VALUE!");
            double a = l.AsNumber();
            double b = r.AsNumber();
            if (op == "+") return CellValue.Number(a + b);
            if (op == "-") return CellValue.Number(a - b);
            if (op == "*") return CellValue.Number(a * b);
            if (op == "/")
            {
                if (b == 0) return CellValue.Error("#DIV/0!");
                return CellValue.Number(a / b);
            }
            if (op == "^") return CellValue.Number(Math.Pow(a, b));
            throw new FormulaException("#NAME?", "Unknown operator " + op);
        }

        private static int Compare(CellValue l, CellValue r)
        {
            if (l.IsText && r.IsText)
            {
                string a = l.TextValue.ToLowerCase();
                string b = r.TextValue.ToLowerCase();
                int c = string.Compare(a, b);
                return c < 0 ? -1 : (c > 0 ? 1 : 0);
            }
            // Numbers sort before text, like a real sheet.
            if (l.IsText) return 1;
            if (r.IsText) return -1;
            double x = l.AsNumber();
            double y = r.AsNumber();
            return x == y ? 0 : (x < y ? -1 : 1);
        }

        // ─── Functions ───────────────────────────────────────────────────

        private CellValue EvalCall(Node node)
        {
            string name = node.TextValue;
            List<Node> args = node.Args;

            if (name == "SUM" || name == "AVG" || name == "AVERAGE" || name == "MIN" || name == "MAX" || name == "COUNT")
            {
                var numbers = new List<double>();
                CellValue err = this.CollectNumbers(args, numbers);
                if (err != null) return err;
                return Aggregate(name, numbers);
            }
            if (name == "IF")
            {
                if (args.Count < 2 || args.Count > 3) return CellValue.Error("#VALUE!");
                CellValue test = this.Eval(args[0]);
                if (test.IsError) return test;
                if (test.AsBool()) return this.Eval(args[1]);
                return args.Count == 3 ? this.Eval(args[2]) : CellValue.Bool(false);
            }
            if (name == "AND" || name == "OR")
            {
                var values = new List<CellValue>();
                CellValue err = this.CollectValues(args, values);
                if (err != null) return err;
                bool result = name == "AND";
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].IsEmpty) continue;
                    bool b = values[i].AsBool();
                    result = name == "AND" ? (result && b) : (result || b);
                }
                return CellValue.Bool(result);
            }
            if (name == "NOT")
            {
                if (args.Count != 1) return CellValue.Error("#VALUE!");
                CellValue v = this.Eval(args[0]);
                if (v.IsError) return v;
                return CellValue.Bool(!v.AsBool());
            }
            if (name == "ABS" || name == "ROUND" || name == "INT")
            {
                if (args.Count < 1) return CellValue.Error("#VALUE!");
                CellValue v = this.Eval(args[0]);
                if (v.IsError) return v;
                if (v.IsText) return CellValue.Error("#VALUE!");
                double x = v.AsNumber();
                if (name == "ABS") return CellValue.Number(Math.Abs(x));
                if (name == "INT") return CellValue.Number(Math.Floor(x));
                int digits = 0;
                if (args.Count > 1)
                {
                    CellValue d = this.Eval(args[1]);
                    if (d.IsError) return d;
                    digits = (int)d.AsNumber();
                }
                double scale = Math.Pow(10, digits);
                return CellValue.Number(Math.Round(x * scale) / scale);
            }
            if (name == "LEN")
            {
                if (args.Count != 1) return CellValue.Error("#VALUE!");
                CellValue v = this.Eval(args[0]);
                if (v.IsError) return v;
                return CellValue.Number(v.AsText().Length);
            }
            if (name == "TODAY" || name == "NOW")
            {
                DateTime now = new DateTime();
                if (name == "TODAY")
                    now = new DateTime(now.GetFullYear(), now.GetMonth(), now.GetDate());
                return CellValue.Number(now.GetTime());
            }
            if (name == "DATE")
            {
                if (args.Count != 3) return CellValue.Error("#VALUE!");
                var parts = new List<double>();
                for (int i = 0; i < 3; i++)
                {
                    CellValue v = this.Eval(args[i]);
                    if (v.IsError) return v;
                    parts.Add(v.AsNumber());
                }
                DateTime d = new DateTime((int)parts[0], (int)parts[1] - 1, (int)parts[2]);
                return CellValue.Number(d.GetTime());
            }
            return CellValue.Error("#NAME?");
        }

        private static CellValue Aggregate(string name, List<double> numbers)
        {
            if (name == "COUNT") return CellValue.Number(numbers.Count);
            if (numbers.Count == 0)
                return name == "SUM" ? CellValue.Number(0) : (name == "MIN" || name == "MAX" ? CellValue.Number(0) : CellValue.Error("#DIV/0!"));
            double acc = numbers[0];
            for (int i = 1; i < numbers.Count; i++)
            {
                double x = numbers[i];
                if (name == "MIN") { if (x < acc) acc = x; }
                else if (name == "MAX") { if (x > acc) acc = x; }
                else acc = acc + x;
            }
            if (name == "AVG" || name == "AVERAGE") return CellValue.Number(acc / numbers.Count);
            return CellValue.Number(acc);
        }

        /// <summary>
        /// Flattens arguments into numbers the way SUM does: ranges and references
        /// contribute numeric cells only; literal arguments are coerced. Returns an
        /// error value if any contributing cell is an error, else null.
        /// </summary>
        private CellValue CollectNumbers(List<Node> args, List<double> numbers)
        {
            for (int i = 0; i < args.Count; i++)
            {
                Node arg = args[i];
                if (arg.Kind == Node.Range)
                {
                    CellValue err = this.ForEachInRange(arg, numbers, null);
                    if (err != null) return err;
                }
                else if (arg.Kind == Node.Ref)
                {
                    CellValue v = this.ReadCell(arg.RefValue);
                    if (v.IsError) return v;
                    if (v.IsNumber) numbers.Add(v.NumberValue);
                }
                else
                {
                    CellValue v = this.Eval(arg);
                    if (v.IsError) return v;
                    if (v.IsText) return CellValue.Error("#VALUE!");
                    if (!v.IsEmpty) numbers.Add(v.AsNumber());
                }
            }
            return null;
        }

        private CellValue CollectValues(List<Node> args, List<CellValue> values)
        {
            for (int i = 0; i < args.Count; i++)
            {
                Node arg = args[i];
                if (arg.Kind == Node.Range)
                {
                    CellValue err = this.ForEachInRange(arg, null, values);
                    if (err != null) return err;
                }
                else
                {
                    CellValue v = this.Eval(arg);
                    if (v.IsError) return v;
                    values.Add(v);
                }
            }
            return null;
        }

        private CellValue ForEachInRange(Node range, List<double> numbers, List<CellValue> values)
        {
            int c0 = Math.Min(range.RefValue.Col, range.RefEnd.Col);
            int c1 = Math.Max(range.RefValue.Col, range.RefEnd.Col);
            int r0 = Math.Min(range.RefValue.Row, range.RefEnd.Row);
            int r1 = Math.Max(range.RefValue.Row, range.RefEnd.Row);
            if (c0 < 0 || r0 < 0 || c1 >= this.source.ColumnCount || r1 >= this.source.RowCount)
                return CellValue.Error("#REF!");
            for (int r = r0; r <= r1; r++)
            {
                for (int c = c0; c <= c1; c++)
                {
                    CellValue v = this.source.GetValue(c, r);
                    if (v.IsError) return v;
                    if (numbers != null && v.IsNumber) numbers.Add(v.NumberValue);
                    if (values != null) values.Add(v);
                }
            }
            return null;
        }
    }
}
