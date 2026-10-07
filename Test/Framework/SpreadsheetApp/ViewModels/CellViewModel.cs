namespace SpreadsheetApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Html;
    using Sunlight.Framework.Observables;
    using SpreadsheetApp.Formula;
    using FormulaNode = SpreadsheetApp.Formula.Node;

    /// <summary>
    /// One grid cell. Holds what the user typed (Raw), the parsed formula, the
    /// computed Value, and the formatted Display. A formula cell subscribes to
    /// the Value of every precedent and recomputes synchronously; the DOM then
    /// updates per the flush mode.
    /// </summary>
    public class CellViewModel : ObservableObject
    {
        private SheetViewModel sheet;
        private int col;
        private int row;
        private string raw;
        private FormulaNode formula;
        private CellValue value;
        private string display;
        private string cssClass;
        private bool isSelected;
        private CellFormat format;           // per-cell override, null = inherit
        private List<CellViewModel> precedents;
        private Action<INotifyPropertyChanged, string> precedentListener;
        private int depth;
        private int depthVersion;

        /// <summary>Set by the sheet while the cell waits in the current recalc wave.</summary>
        public bool IsDirty;

        public CellViewModel(SheetViewModel sheet, int col, int row)
        {
            this.sheet = sheet;
            this.col = col;
            this.row = row;
            this.raw = "";
            this.value = CellValue.Empty;
            this.display = "";
            this.cssClass = SheetCss.Cell;
            this.precedentListener = this.OnPrecedentChanged;
        }

        public int Col { get { return this.col; } }
        public int Row { get { return this.row; } }
        public string Name { get { return CellRef.CellName(this.col, this.row); } }

        /// <summary>Text as typed: "=A1+B1", "42", "hello". Shown in the formula bar.</summary>
        public string Raw { get { return this.raw; } }
        public bool IsFormula { get { return this.formula != null; } }
        public CellValue Value { get { return this.value; } }
        public string Display { get { return this.display; } }
        public string CssClass { get { return this.cssClass; } }
        public bool IsSelected { get { return this.isSelected; } }
        public CellFormat Format { get { return this.format; } }
        public int PrecedentCount { get { return this.precedents == null ? 0 : this.precedents.Count; } }

        public bool DependsOn(CellViewModel other)
        {
            return this.precedents != null && this.precedents.IndexOf(other) >= 0;
        }

        /// <summary>Click handler from the row template.</summary>
        public void OnClick(Element e, ElementEvent ev)
        {
            this.sheet.SelectCell(this);
        }

        public void SetSelected(bool selected)
        {
            if (this.isSelected == selected) return;
            this.isSelected = selected;
            this.UpdateCssClass();
        }

        /// <summary>Per-cell format override; null clears it.</summary>
        public void SetFormat(CellFormat f)
        {
            this.format = f;
            this.Refresh();
        }

        /// <summary>Re-formats the display (used when a row or column format changes).</summary>
        public void Refresh()
        {
            string next = this.sheet.FormatFor(this).Format(this.value);
            if (next == this.display) return;
            this.display = next;
            base.FirePropertyChanged(nameof(Display));
        }

        /// <summary>
        /// Sets the cell from typed text. "=..." is a formula; otherwise a number,
        /// TRUE/FALSE, an ISO date, or text. Returns false if a formula failed to
        /// parse (the cell then shows the error).
        /// </summary>
        public bool SetRaw(string text)
        {
            string t = text == null ? "" : text;
            this.raw = t;
            this.Unsubscribe();
            this.formula = null;
            this.sheet.StructureChanged();

            if (t.StartsWith("="))
            {
                try
                {
                    this.formula = Parser.Parse(t.Substring(1));
                }
                catch (FormulaException ex)
                {
                    this.SetValue(CellValue.Error(ex.Code));
                    return false;
                }
                if (this.sheet.WouldCycle(this, this.formula))
                {
                    this.SetValue(CellValue.Error("#CYCLE!"));
                    return true;
                }
                this.Subscribe();
                this.Recompute();
                return true;
            }

            // A typed ISO date is stored as a number; give the cell a date format
            // unless the user already chose one, so it still reads as a date.
            string trimmed = t.Trim();
            if (this.format == null && IsIsoDate(trimmed))
                this.format = CellFormat.FromName(trimmed.Length == 16 ? "datetime" : "date");
            this.SetValue(ParseLiteral(t));
            return true;
        }

        /// <summary>Sets a literal number directly (benchmark ops).</summary>
        public void SetNumber(double n)
        {
            if (this.formula != null) { this.Unsubscribe(); this.formula = null; this.sheet.StructureChanged(); }
            this.raw = CellFormat.GeneralText(n);
            this.SetValue(CellValue.Number(n));
        }

        public double NumberOrZero()
        {
            return this.value.AsNumber();
        }

        public void Recompute()
        {
            if (this.formula == null) return;
            this.SetValue(new Evaluator(this.sheet).Evaluate(this.formula));
        }

        /// <summary>The formula shifted for a paste at (dCol, dRow); literals are copied as-is.</summary>
        public string RawShifted(int dCol, int dRow)
        {
            if (this.formula == null) return this.raw;
            return "=" + FormulaText.ToText(FormulaText.Shift(this.formula, dCol, dRow));
        }

        public void Clear()
        {
            this.SetRaw("");
        }

        private static CellValue ParseLiteral(string t)
        {
            string s = t.Trim();
            if (s == "") return CellValue.Empty;
            string upper = s.ToUpperCase();
            if (upper == "TRUE") return CellValue.Bool(true);
            if (upper == "FALSE") return CellValue.Bool(false);
            if (IsNumeric(s)) return CellValue.Number(double.Parse(s));
            if (IsIsoDate(s)) return CellValue.Number(ParseDateMillis(s));
            return CellValue.Text(t);
        }

        [Script("return s !== '' && !isNaN(Number(s));")]
        private static extern bool IsNumeric(string s);

        /// <summary>yyyy-MM-dd or yyyy-MM-dd HH:mm, digits checked by code point.</summary>
        private static bool IsIsoDate(string s)
        {
            if (s.Length != 10 && s.Length != 16) return false;
            for (int i = 0; i < s.Length; i++)
            {
                int c = (int)s.CharCodeAt(i);
                bool digit = c >= 48 && c <= 57;
                if (i == 4 || i == 7) { if (c != 45) return false; }
                else if (i == 10) { if (c != 32 && c != 84) return false; }
                else if (i == 13) { if (c != 58) return false; }
                else if (!digit) return false;
            }
            return true;
        }

        private static double ParseDateMillis(string s)
        {
            int year = (int)double.Parse(s.Substring(0, 4));
            int month = (int)double.Parse(s.Substring(5, 2));
            int day = (int)double.Parse(s.Substring(8, 2));
            int hours = s.Length == 16 ? (int)double.Parse(s.Substring(11, 2)) : 0;
            int minutes = s.Length == 16 ? (int)double.Parse(s.Substring(14, 2)) : 0;
            return new DateTime(year, month - 1, day, hours, minutes).GetTime();
        }

        private void Subscribe()
        {
            var refs = new List<CellRef>();
            FormulaText.CollectPrecedents(this.formula, this.sheet.ColumnCount, this.sheet.RowCount, refs);
            this.precedents = new List<CellViewModel>();
            for (int i = 0; i < refs.Count; i++)
            {
                CellViewModel p = this.sheet.CellAt(refs[i].Col, refs[i].Row);
                if (p == null || p == this || this.precedents.IndexOf(p) >= 0) continue;
                this.precedents.Add(p);
                p.AddPropertyChangedListener(nameof(Value), this.precedentListener);
            }
        }

        public void Unsubscribe()
        {
            if (this.precedents == null) return;
            for (int i = 0; i < this.precedents.Count; i++)
                this.precedents[i].RemovePropertyChangedListener(nameof(Value), this.precedentListener);
            this.precedents = null;
        }

        public List<CellViewModel> Precedents
        {
            get { return this.precedents; }
        }

        private void OnPrecedentChanged(INotifyPropertyChanged sender, string propName)
        {
            this.sheet.MarkDirty(this);
        }

        /// <summary>
        /// Dependency depth: literals are 0, a formula is 1 + its deepest precedent.
        /// Memoized until the sheet's structure (any formula) changes.
        /// </summary>
        public int DepthFor(int structureVersion)
        {
            if (this.depthVersion == structureVersion) return this.depth;
            int d = 0;
            if (this.precedents != null)
            {
                for (int i = 0; i < this.precedents.Count; i++)
                {
                    int pd = this.precedents[i].DepthFor(structureVersion) + 1;
                    if (pd > d) d = pd;
                }
            }
            this.depth = d;
            this.depthVersion = structureVersion;
            return d;
        }

        private void SetValue(CellValue v)
        {
            bool changed = !CellValue.AreEqual(this.value, v) || this.value.Kind != v.Kind;
            this.value = v;
            this.UpdateCssClass();
            this.Refresh();
            if (changed) base.FirePropertyChanged(nameof(Value));
        }

        private void UpdateCssClass()
        {
            string next = SheetCss.Cell;
            if (this.formula != null) next = next + " " + SheetCss.Formula;
            if (this.value.IsText) next = next + " " + SheetCss.TextCell;
            if (this.value.IsError) next = next + " " + SheetCss.Error;
            if (this.isSelected) next = next + " " + SheetCss.Selected;
            if (next == this.cssClass) return;
            this.cssClass = next;
            base.FirePropertyChanged(nameof(CssClass));
        }
    }
}
