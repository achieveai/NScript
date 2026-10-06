namespace SpreadsheetApp.Test.Engine
{
    using System.Collections.Generic;
    using SunlightUnit;
    using SpreadsheetApp.Formula;

    /// <summary>In-memory cell source: a small grid filled by the test.</summary>
    public class FakeSheet : ICellSource
    {
        private int cols;
        private int rows;
        private StringDictionary<CellValue> cells = new StringDictionary<CellValue>();

        public FakeSheet(int cols, int rows)
        {
            this.cols = cols;
            this.rows = rows;
        }

        public int ColumnCount { get { return this.cols; } }
        public int RowCount { get { return this.rows; } }

        public FakeSheet Set(string name, CellValue value)
        {
            this.cells[name] = value;
            return this;
        }

        public FakeSheet Num(string name, double value)
        {
            return this.Set(name, CellValue.Number(value));
        }

        public CellValue GetValue(int col, int row)
        {
            string key = CellRef.CellName(col, row);
            return this.cells.ContainsKey(key) ? this.cells[key] : CellValue.Empty;
        }
    }

    /// <summary>
    /// Pure engine tests: tokenizer, parser, evaluator, reference shifting,
    /// precedents and formatting. No DOM, no view models.
    /// </summary>
    [TestFixture]
    public class FormulaEngineTests
    {
        private static CellValue Eval(string formula, ICellSource sheet)
        {
            return new Evaluator(sheet).Evaluate(Parser.Parse(formula));
        }

        private static double Num(string formula, ICellSource sheet)
        {
            return Eval(formula, sheet).NumberValue;
        }

        private static FakeSheet Sheet()
        {
            return new FakeSheet(26, 100).Num("A1", 3).Num("B1", 7).Num("A2", 10).Num("B2", 20)
                .Num("A3", 5).Set("C1", CellValue.Text("hi")).Num("C2", 0);
        }

        [Test]
        public static void TestArithmeticPrecedence(Assert assert)
        {
            var s = Sheet();
            assert.Equal(14, Num("2+3*4", s), "* binds tighter than +");
            assert.Equal(20, Num("(2+3)*4", s), "parentheses");
            assert.Equal(512, Num("2^3^2", s), "^ is right-associative");
            assert.Equal(9, Num("-3^2", s), "unary minus binds tighter than ^, as in Excel");
            assert.Equal(0.5, Num("50%", s), "percent postfix");
            assert.Equal(1.5, Num("1.5e0", s), "exponent literal");
        }

        [Test]
        public static void TestReferencesAndRanges(Assert assert)
        {
            var s = Sheet();
            assert.Equal(10, Num("A1+B1", s), "relative refs");
            assert.Equal(10, Num("$A$1+a1+B$1-$a1", s), "absolute and lower-case refs read the same cells");
            assert.Equal(45, Num("SUM(A1:B2,A3)", s), "range plus extra argument");
            assert.Equal(6, Num("COUNT(A1:C3)", s), "COUNT ignores text and empty but counts zero");
            assert.Equal(10, Num("AVG(A1:B1,A2,B2)", s), "AVG over mixed args");
            assert.Equal(20, Num("MAX(A1:B2)", s), "MAX");
            assert.Equal(3, Num("MIN(B2:A1)", s), "reversed range corners normalize");
        }

        [Test]
        public static void TestTextBoolAndComparison(Assert assert)
        {
            var s = Sheet();
            assert.Equal("hi!", Eval("C1&\"!\"", s).TextValue, "concatenation");
            assert.Equal("say \"x\"", Eval("\"say \"\"x\"\"\"", s).TextValue, "doubled quotes escape");
            assert.Equal(true, Eval("A1<B1", s).BoolValue, "numeric compare");
            assert.Equal(true, Eval("C1=\"HI\"", s).BoolValue, "text compare ignores case");
            assert.Equal(false, Eval("A1<>3", s).BoolValue, "<>");
            assert.Equal(7, Num("IF(A1>2,B1,0)", s), "IF true branch");
            assert.Equal(0, Num("IF(A1>20,B1,0)", s), "IF false branch");
            assert.Equal(true, Eval("AND(TRUE,A1=3)", s).BoolValue, "AND");
            assert.Equal(true, Eval("OR(FALSE,NOT(A1=4))", s).BoolValue, "OR/NOT");
            assert.Equal(2, Num("LEN(C1)", s), "LEN");
            assert.Equal(3.14, Num("ROUND(3.14159,2)", s), "ROUND");
            assert.Equal(3, Num("ABS(-3)", s), "ABS");
        }

        [Test]
        public static void TestErrors(Assert assert)
        {
            var s = Sheet();
            assert.Equal("#DIV/0!", Eval("A1/C2", s).ErrorCode, "division by zero");
            assert.Equal("#DIV/0!", Eval("A1/0+1", s).ErrorCode, "errors propagate");
            assert.Equal("#VALUE!", Eval("C1*2", s).ErrorCode, "text in arithmetic");
            assert.Equal("#NAME?", Eval("FOO(1)", s).ErrorCode, "unknown function");
            assert.Equal("#REF!", Eval("ZZ1000", s).ErrorCode, "out of bounds ref");
            assert.Equal("#DIV/0!", Eval("AVG(D1:D3)", s).ErrorCode, "average of nothing");

            string code = "";
            try { Parser.Parse("1+"); } catch (FormulaException ex) { code = ex.Code; }
            assert.Equal("#NAME?", code, "parse error surfaces as #NAME?");
            code = "";
            try { Parser.Parse("SUM(1,2"); } catch (FormulaException ex) { code = ex.Code; }
            assert.Equal("#NAME?", code, "missing paren");
        }

        [Test]
        public static void TestRoundTripAndShift(Assert assert)
        {
            Node n = Parser.Parse("SUM(A1:B2)*$C$1+(A1+B1)*2-a2%");
            assert.Equal("SUM(A1:B2)*$C$1+(A1+B1)*2-A2%", FormulaText.ToText(n), "serializes with needed parens");

            Node shifted = FormulaText.Shift(n, 1, 2);
            assert.Equal("SUM(B3:C4)*$C$1+(B3+C3)*2-B4%", FormulaText.ToText(shifted), "relative refs move, absolute stay");

            Node mixed = Parser.Parse("$A1+A$1");
            assert.Equal("$A3+C$1", FormulaText.ToText(FormulaText.Shift(mixed, 2, 2)), "$ fixes one axis");

            Node off = FormulaText.Shift(Parser.Parse("A1+B5"), 0, -2);
            assert.Equal("#REF!+B3", FormulaText.ToText(off), "shifted off the sheet becomes #REF!");
            assert.Equal("#REF!", Eval(FormulaText.ToText(off), Sheet()).ErrorCode, "#REF! literal evaluates to the error");
        }

        [Test]
        public static void TestPrecedentsAndNames(Assert assert)
        {
            var cells = new List<CellRef>();
            FormulaText.CollectPrecedents(Parser.Parse("A1+SUM(B1:C2)+$D$9"), 26, 100, cells);
            string names = "";
            for (int i = 0; i < cells.Count; i++) names = names + (i > 0 ? "," : "") + cells[i].Text();
            assert.Equal("A1,B1,C1,B2,C2,$D$9", names, "ranges expand row-major");

            assert.Equal("Z", CellRef.ColumnName(25), "Z");
            assert.Equal("AA", CellRef.ColumnName(26), "AA");
            assert.Equal(26, Tokenizer.ParseColumn("AA"), "AA parses back");
        }

        [Test]
        public static void TestFormatting(Assert assert)
        {
            assert.Equal("1234.5", CellFormat.Default.Format(CellValue.Number(1234.5)), "general trims zeros");
            assert.Equal("1,234.50", CellFormat.FromName("number").Format(CellValue.Number(1234.5)), "number with thousands");
            assert.Equal("-1,234,567", CellFormat.FromName("integer").Format(CellValue.Number(-1234567)), "negative grouping");
            assert.Equal("12.5%", CellFormat.FromName("percent").Format(CellValue.Number(0.125)), "percent");
            assert.Equal("-$1,000.00", CellFormat.FromName("currency").Format(CellValue.Number(-1000)), "currency");
            assert.Equal("#DIV/0!", CellFormat.FromName("percent").Format(CellValue.Error("#DIV/0!")), "errors pass through");
            assert.Equal("", CellFormat.Default.Format(CellValue.Empty), "empty is blank");

            double millis = new System.DateTime(2026, 9, 6, 14, 5).GetTime();
            assert.Equal("2026-10-06", CellFormat.FromName("date").Format(CellValue.Number(millis)), "date");
            assert.Equal("14:05", CellFormat.FromName("time").Format(CellValue.Number(millis)), "time");
            assert.Equal("2026-10-06 14:05", CellFormat.FromName("datetime").Format(CellValue.Number(millis)), "date time");
            var sheet = new FakeSheet(5, 5);
            assert.Equal(millis, Num("DATE(2026,10,6)+14*3600000+5*60000", sheet), "DATE builds the same instant");
        }
    }
}
