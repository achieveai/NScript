namespace SpreadsheetApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Html;
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Helpers.BindingGraph;
    using SpreadsheetApp.Formula;
    using FormulaNode = SpreadsheetApp.Formula.Node;
    using SpreadsheetApp.Services;

    /// <summary>
    /// The sheet: a fixed grid of A1-addressed cells, the selection and formula
    /// bar, per column/row/cell formats, copy/paste with reference shifting,
    /// and the before/after benchmark.
    /// </summary>
    public class SheetViewModel : ObservableObject, ICellSource
    {
        private const int Iterations = 5;   // timed passes; the minimum is reported (filters host interference)
        public const int Columns = 26;

        private const int ScopeCellMode = 0;
        private const int ScopeRowMode = 1;
        private const int ScopeColumnMode = 2;

        private Element root;
        private ObservableCollection<RowViewModel> rows;
        private ObservableCollection<HeaderViewModel> headers;
        private ObservableCollection<BenchResultViewModel> results;
        private List<CellFormat> columnFormats;
        private List<CellFormat> rowFormats;
        private int dataRows;

        private CellViewModel selected;
        private string editText;
        private string selectedName;
        private string barHint;
        private int scope;

        private CellViewModel clipboardCell;
        private string clipboardRaw;
        private int clipboardCol;
        private int clipboardRow;

        private string batchingLabel;
        private string batchingClass;
        private string rowsLabel;
        private string statusText;
        private int seed;

        private bool benchRunning;
        private List<Action> steps;
        private int stepIndex;

        private bool ignoreNextBarChange;
        private List<CellViewModel> dirtyHeap;
        private bool inWave;
        private int structureVersion;

        public SheetViewModel(Element root)
        {
            this.root = root;
            this.rows = new ObservableCollection<RowViewModel>();
            this.headers = new ObservableCollection<HeaderViewModel>();
            this.results = new ObservableCollection<BenchResultViewModel>();
            this.columnFormats = new List<CellFormat>();
            this.rowFormats = new List<CellFormat>();
            for (int c = 0; c < Columns; c++)
            {
                this.headers.Add(new HeaderViewModel(CellRef.ColumnName(c)));
                this.columnFormats.Add(null);
            }
            this.seed = 42;
            this.statusText = "";
            this.rowsLabel = "";
            this.editText = "";
            this.selectedName = "";
            this.barHint = "";
            this.scope = ScopeCellMode;
            this.dirtyHeap = new List<CellViewModel>();
            this.structureVersion = 1;
            this.UpdateBatchingLabel();
        }

        // ─── Recalc waves ────────────────────────────────────────────────

        /// <summary>Any formula was set or cleared: cached dependency depths are stale.</summary>
        public void StructureChanged()
        {
            this.structureVersion++;
        }

        /// <summary>
        /// Queues a dependent for recomputation. The first mark outside a wave
        /// starts one: dirty cells are recomputed in dependency-depth order so
        /// each cell evaluates once per change, however many precedents moved.
        /// </summary>
        public void MarkDirty(CellViewModel cell)
        {
            if (cell.IsDirty) return;
            cell.IsDirty = true;
            this.HeapPush(cell);
            if (this.inWave) return;

            this.inWave = true;
            try
            {
                while (this.dirtyHeap.Count > 0)
                {
                    CellViewModel next = this.HeapPop();
                    next.IsDirty = false;
                    next.Recompute();
                }
            }
            finally
            {
                this.inWave = false;
            }
        }

        private int DepthOf(CellViewModel cell)
        {
            return cell.DepthFor(this.structureVersion);
        }

        private void HeapPush(CellViewModel cell)
        {
            var heap = this.dirtyHeap;
            heap.Add(cell);
            int i = heap.Count - 1;
            int d = this.DepthOf(cell);
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (this.DepthOf(heap[parent]) <= d) break;
                heap[i] = heap[parent];
                i = parent;
            }
            heap[i] = cell;
        }

        private CellViewModel HeapPop()
        {
            var heap = this.dirtyHeap;
            CellViewModel top = heap[0];
            CellViewModel last = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            if (heap.Count == 0) return top;
            int i = 0;
            int d = this.DepthOf(last);
            int count = heap.Count;
            while (true)
            {
                int left = 2 * i + 1;
                if (left >= count) break;
                int right = left + 1;
                int child = right < count && this.DepthOf(heap[right]) < this.DepthOf(heap[left]) ? right : left;
                if (this.DepthOf(heap[child]) >= d) break;
                heap[i] = heap[child];
                i = child;
            }
            heap[i] = last;
            return top;
        }

        // ─── ICellSource ─────────────────────────────────────────────────

        public int ColumnCount { get { return Columns; } }
        public int RowCount { get { return this.rows.Count; } }

        public CellValue GetValue(int col, int row)
        {
            CellViewModel cell = this.CellAt(col, row);
            return cell == null ? CellValue.Empty : cell.Value;
        }

        public CellViewModel CellAt(int col, int row)
        {
            if (col < 0 || row < 0 || col >= Columns || row >= this.rows.Count) return null;
            return this.rows[row].Cells[col];
        }

        public CellViewModel CellNamed(string name)
        {
            var tokens = new Tokenizer(name).Tokenize();
            if (tokens.Count < 1 || tokens[0].Kind != Token.Ref) return null;
            return this.CellAt(tokens[0].RefValue.Col, tokens[0].RefValue.Row);
        }

        // ─── Bindable state ──────────────────────────────────────────────

        public ObservableCollection<RowViewModel> Rows { get { return this.rows; } }
        public ObservableCollection<HeaderViewModel> Headers { get { return this.headers; } }
        public ObservableCollection<BenchResultViewModel> Results { get { return this.results; } }
        public CellViewModel Selected { get { return this.selected; } }
        public string EditText { get { return this.editText; } }
        public string SelectedName { get { return this.selectedName; } }
        public string BarHint { get { return this.barHint; } }
        public string BatchingLabel { get { return this.batchingLabel; } }
        public string BatchingClass { get { return this.batchingClass; } }
        public string RowsLabel { get { return this.rowsLabel; } }
        public int DataRows { get { return this.dataRows; } }

        public string ScopeCellClass { get { return this.ScopeClass(ScopeCellMode); } }
        public string ScopeRowClass { get { return this.ScopeClass(ScopeRowMode); } }
        public string ScopeColumnClass { get { return this.ScopeClass(ScopeColumnMode); } }

        public string StatusText
        {
            get { return this.statusText; }
            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;
                    base.FirePropertyChanged("StatusText");
                }
            }
        }

        // ─── Sheet construction ──────────────────────────────────────────

        /// <summary>
        /// Rebuilds the demo sheet with n data rows plus a totals row. Columns:
        /// A, B inputs; C = A*B; D = SUM(A:C); E = running total of D;
        /// F = AVERAGE(A:E); G = IF on F; H = date from A; I = F as share of total F;
        /// J = ROUND(C * B1%). The totals row sums A..F.
        /// </summary>
        public void Build(int n)
        {
            this.selected = null;
            this.clipboardCell = null;
            for (int i = 0; i < this.rows.Count; i++) this.rows[i].Unbind();
            this.rows.Clear();
            this.rowFormats.Clear();
            this.dataRows = n;

            int total = n + 1;
            for (int r = 0; r < total; r++)
            {
                var row = new RowViewModel(r);
                for (int c = 0; c < Columns; c++) row.Cells.Add(new CellViewModel(this, c, r));
                this.rows.Add(row);
                this.rowFormats.Add(null);
            }

            this.SetColumnFormat(2, CellFormat.FromName("currency"));
            this.SetColumnFormat(5, CellFormat.FromName("number"));
            this.SetColumnFormat(7, CellFormat.FromName("date"));
            this.SetColumnFormat(8, CellFormat.FromName("percent"));

            for (int r = 0; r < n; r++)
            {
                string k = (r + 1).ToString();
                this.rows[r].Cells[0].SetNumber(r + 1);
                this.rows[r].Cells[1].SetNumber((r % 7) + 1);
                this.rows[r].Cells[2].SetRaw("=A" + k + "*B" + k);
                this.rows[r].Cells[3].SetRaw("=SUM(A" + k + ":C" + k + ")");
                this.rows[r].Cells[4].SetRaw(r == 0 ? "=D1" : "=E" + r.ToString() + "+D" + k);
                this.rows[r].Cells[5].SetRaw("=AVERAGE(A" + k + ":E" + k + ")");
                this.rows[r].Cells[6].SetRaw("=IF(F" + k + ">50,\"high\",\"low\")");
                this.rows[r].Cells[7].SetRaw("=DATE(2026,1,1)+A" + k + "*86400000");
                this.rows[r].Cells[8].SetRaw("=F" + k + "/$F$" + (n + 1).ToString());
                this.rows[r].Cells[9].SetRaw("=ROUND(C" + k + "*$B$1%,2)");
            }
            string last = n.ToString();
            for (int c = 0; c < 6; c++)
            {
                string col = CellRef.ColumnName(c);
                this.rows[n].Cells[c].SetRaw("=SUM(" + col + "1:" + col + last + ")");
            }

            this.rowsLabel = "Rows: " + n.ToString();
            base.FirePropertyChanged("RowsLabel");
            this.SelectCell(this.rows[0].Cells[0]);
        }

        public void CycleRows()
        {
            if (this.benchRunning) return;
            int next = this.dataRows >= 200 ? 50 : this.dataRows * 2;
            this.Build(next);
            this.StatusText = "Rebuilt with " + next.ToString() + " rows";
        }

        // ─── Selection and formula bar ───────────────────────────────────

        public void SelectCell(CellViewModel cell)
        {
            this.ignoreNextBarChange = false;
            if (cell == null) return;
            if (this.selected != null)
            {
                this.selected.SetSelected(false);
                this.headers[this.selected.Col].SetSelected(false);
            }
            this.selected = cell;
            cell.SetSelected(true);
            this.headers[cell.Col].SetSelected(true);
            this.SetEditText(cell.Raw);
            this.selectedName = cell.Name;
            base.FirePropertyChanged("SelectedName");
            this.UpdateHint();
        }

        private void SetEditText(string text)
        {
            this.editText = text;
            base.FirePropertyChanged("EditText");
        }

        private void UpdateHint()
        {
            CellViewModel cell = this.selected;
            string hint = "";
            if (cell != null)
            {
                CellValue v = cell.Value;
                if (v.IsError) hint = v.ErrorCode;
                else if (v.IsEmpty) hint = "empty";
                else if (v.IsNumber) hint = "number " + CellFormat.GeneralText(v.NumberValue);
                else if (v.IsBool) hint = "boolean";
                else hint = "text";
                if (cell.IsFormula) hint = hint + ", " + cell.PrecedentCount.ToString() + " precedents";
                hint = hint + ", format: " + this.FormatFor(cell).Name;
            }
            if (hint == this.barHint) return;
            this.barHint = hint;
            base.FirePropertyChanged("BarHint");
        }

        /// <summary>Commits typed text into the selected cell and refreshes dependents.</summary>
        public void Commit(string text)
        {
            if (this.selected == null) return;
            this.selected.SetRaw(text);
            this.SetEditText(this.selected.Raw);
            this.UpdateHint();
        }

        public void OnBarKeyDown(Element e, ElementEvent ev)
        {
            InputElement input = (InputElement)(ev != null && ev.Target != null ? ev.Target : e);
            if (ev.KeyCode == 13)
            {
                this.CommitAndMoveDown(input.Value);
                this.FocusGrid();
                ev.PreventDefault();
            }
            else if (ev.KeyCode == 27)
            {
                input.Value = this.selected == null ? "" : this.selected.Raw;
                this.FocusGrid();
            }
            else
            {
                this.ignoreNextBarChange = false;
            }
        }

        /// <summary>
        /// Enter in the formula bar: commit, select the cell below. The browser
        /// fires the input's change event right after, still carrying the text
        /// just committed; flag it so CommitFromBar does not write it into the
        /// new cell.
        /// </summary>
        public void CommitAndMoveDown(string text)
        {
            this.Commit(text);
            this.MoveSelection(0, 1);
            this.ignoreNextBarChange = true;
        }

        /// <summary>Change event from the formula bar (blur, or Enter's implicit change).</summary>
        public void CommitFromBar(string text)
        {
            bool skip = this.ignoreNextBarChange;
            this.ignoreNextBarChange = false;
            if (skip) return;
            if (this.selected != null && text != this.selected.Raw)
                this.Commit(text);
        }

        public void OnBarChange(Element e, ElementEvent ev)
        {
            InputElement input = (InputElement)(ev != null && ev.Target != null ? ev.Target : e);
            this.CommitFromBar(input.Value);
        }

        public void OnGridKeyDown(Element e, ElementEvent ev)
        {
            int key = ev.KeyCode;
            bool ctrl = ev.CtrlKey || ev.MetaKey;
            if (key == 37) { this.MoveSelection(-1, 0); ev.PreventDefault(); }
            else if (key == 38) { this.MoveSelection(0, -1); ev.PreventDefault(); }
            else if (key == 39) { this.MoveSelection(1, 0); ev.PreventDefault(); }
            else if (key == 40) { this.MoveSelection(0, 1); ev.PreventDefault(); }
            else if (key == 13 || key == 113) { this.FocusBar(); ev.PreventDefault(); }
            else if (key == 46 || key == 8) { this.Commit(""); ev.PreventDefault(); }
            else if (ctrl && key == 67) { this.CopySelected(); ev.PreventDefault(); }
            else if (ctrl && key == 86) { this.PasteToSelected(); ev.PreventDefault(); }
            else if (ctrl && key == 68) { this.FillDown(); ev.PreventDefault(); }
        }

        public void MoveSelection(int dCol, int dRow)
        {
            if (this.selected == null) return;
            CellViewModel next = this.CellAt(this.selected.Col + dCol, this.selected.Row + dRow);
            if (next != null) this.SelectCell(next);
        }

        private void FocusBar()
        {
            Element input = Window.Instance.Document.GetElementById("formula-input");
            if (input != null) input.Focus();
        }

        private void FocusGrid()
        {
            Element grid = Window.Instance.Document.GetElementById("sheet");
            if (grid != null) grid.Focus();
        }

        // ─── Copy / paste ────────────────────────────────────────────────

        public void CopySelected()
        {
            if (this.selected == null) return;
            this.clipboardCell = this.selected;
            this.clipboardRaw = this.selected.Raw;
            this.clipboardCol = this.selected.Col;
            this.clipboardRow = this.selected.Row;
            this.StatusText = "Copied " + this.selected.Name;
        }

        public void PasteToSelected()
        {
            if (this.selected == null || this.clipboardCell == null) return;
            this.PasteInto(this.selected);
            this.StatusText = "Pasted into " + this.selected.Name;
        }

        /// <summary>Copies the selected cell into the cell below, shifting references, and selects it.</summary>
        public void FillDown()
        {
            if (this.selected == null) return;
            CellViewModel below = this.CellAt(this.selected.Col, this.selected.Row + 1);
            if (below == null) return;
            CellViewModel source = this.selected;
            this.clipboardCell = source;
            this.clipboardRaw = source.Raw;
            this.clipboardCol = source.Col;
            this.clipboardRow = source.Row;
            this.PasteInto(below);
            this.SelectCell(below);
        }

        private void PasteInto(CellViewModel target)
        {
            string raw = this.clipboardCell.RawShifted(target.Col - this.clipboardCol, target.Row - this.clipboardRow);
            target.SetRaw(raw);
            if (target == this.selected)
            {
                this.SetEditText(target.Raw);
                this.UpdateHint();
            }
        }

        // ─── Formats ─────────────────────────────────────────────────────

        public CellFormat FormatFor(CellViewModel cell)
        {
            if (cell.Format != null) return cell.Format;
            CellFormat rowFormat = cell.Row < this.rowFormats.Count ? this.rowFormats[cell.Row] : null;
            if (rowFormat != null) return rowFormat;
            CellFormat colFormat = this.columnFormats[cell.Col];
            return colFormat != null ? colFormat : CellFormat.Default;
        }

        public void ScopeCell() { this.SetScope(ScopeCellMode); }
        public void ScopeRow() { this.SetScope(ScopeRowMode); }
        public void ScopeColumn() { this.SetScope(ScopeColumnMode); }

        private void SetScope(int mode)
        {
            this.scope = mode;
            base.FirePropertyChanged("ScopeCellClass");
            base.FirePropertyChanged("ScopeRowClass");
            base.FirePropertyChanged("ScopeColumnClass");
        }

        private string ScopeClass(int mode)
        {
            return this.scope == mode ? SheetCss.Btn + " " + SheetCss.BtnOn : SheetCss.Btn;
        }

        public void FormatGeneral() { this.ApplyFormat(null); }
        public void FormatNumber() { this.ApplyFormat(CellFormat.FromName("number")); }
        public void FormatInteger() { this.ApplyFormat(CellFormat.FromName("integer")); }
        public void FormatPercent() { this.ApplyFormat(CellFormat.FromName("percent")); }
        public void FormatCurrency() { this.ApplyFormat(CellFormat.FromName("currency")); }
        public void FormatDate() { this.ApplyFormat(CellFormat.FromName("date")); }
        public void FormatDateTime() { this.ApplyFormat(CellFormat.FromName("datetime")); }

        /// <summary>Applies a format to the selected cell, its row, or its column (null = General).</summary>
        public void ApplyFormat(CellFormat f)
        {
            if (this.selected == null) return;
            if (this.scope == ScopeCellMode)
            {
                this.selected.SetFormat(f);
            }
            else if (this.scope == ScopeRowMode)
            {
                this.SetRowFormat(this.selected.Row, f);
            }
            else
            {
                this.SetColumnFormat(this.selected.Col, f);
            }
            this.UpdateHint();
        }

        public void SetRowFormat(int row, CellFormat f)
        {
            this.rowFormats[row] = f;
            var cells = this.rows[row].Cells;
            for (int c = 0; c < cells.Count; c++) cells[c].Refresh();
        }

        public void SetColumnFormat(int col, CellFormat f)
        {
            this.columnFormats[col] = f;
            this.headers[col].FormatName = f == null ? "" : f.Name;
            for (int r = 0; r < this.rows.Count; r++) this.rows[r].Cells[col].Refresh();
        }

        // ─── Cycle detection ─────────────────────────────────────────────

        /// <summary>True if any cell the formula reads depends (transitively) on the cell itself.</summary>
        public bool WouldCycle(CellViewModel cell, FormulaNode formula)
        {
            var refs = new List<CellRef>();
            FormulaText.CollectPrecedents(formula, Columns, this.rows.Count, refs);
            var visited = new List<CellViewModel>();
            for (int i = 0; i < refs.Count; i++)
            {
                CellViewModel p = this.CellAt(refs[i].Col, refs[i].Row);
                if (p == null) continue;
                if (p == cell || this.ReachesTarget(p, cell, visited)) return true;
            }
            return false;
        }

        private bool ReachesTarget(CellViewModel from, CellViewModel target, List<CellViewModel> visited)
        {
            if (visited.IndexOf(from) >= 0) return false;
            visited.Add(from);
            List<CellViewModel> precedents = from.Precedents;
            if (precedents == null) return false;
            for (int i = 0; i < precedents.Count; i++)
            {
                if (precedents[i] == target) return true;
                if (this.ReachesTarget(precedents[i], target, visited)) return true;
            }
            return false;
        }

        // ─── Operations (toolbar buttons and benchmark ops) ──────────────

        /// <summary>A1 + 1: one input; fans out across row 1, every running total, and the totals row.</summary>
        public void EditA1()
        {
            if (this.dataRows == 0) return;
            CellViewModel a1 = this.rows[0].Cells[0];
            a1.SetNumber(a1.NumberOrZero() + 1);
            if (this.selected == a1) { this.SetEditText(a1.Raw); this.UpdateHint(); }
        }

        /// <summary>Every A + 1 in one task: each edit re-dirties the running totals below it.</summary>
        public void ShiftAll()
        {
            for (int i = 0; i < this.dataRows; i++)
            {
                CellViewModel a = this.rows[i].Cells[0];
                a.SetNumber(a.NumberOrZero() + 1);
            }
        }

        /// <summary>Random A and B for every row in one task (deterministic sequence).</summary>
        public void Randomize()
        {
            for (int i = 0; i < this.dataRows; i++)
            {
                this.rows[i].Cells[0].SetNumber(this.NextRandom());
                this.rows[i].Cells[1].SetNumber(this.NextRandom());
            }
        }

        public void RunOp(string name)
        {
            if (name == "a1") this.EditA1();
            else if (name == "shift") this.ShiftAll();
            else if (name == "random") this.Randomize();
        }

        private double NextRandom()
        {
            // Park-Miller; stays well under 2^53 so JS doubles are exact.
            this.seed = (this.seed * 48271) % 2147483647;
            return 1 + (this.seed % 100);
        }

        // ─── Batching toggle ─────────────────────────────────────────────

        public void ToggleBatching()
        {
            this.SetBatching(!GraphFlushCoordinator.BatchingEnabled);
        }

        public void SetBatching(bool on)
        {
            GraphFlushCoordinator.BatchingEnabled = on;
            this.UpdateBatchingLabel();
        }

        private void UpdateBatchingLabel()
        {
            bool on = GraphFlushCoordinator.BatchingEnabled;
            this.batchingLabel = on ? "Batched flush: ON" : "Batched flush: OFF";
            this.batchingClass = on ? SheetCss.Btn + " " + SheetCss.BtnOn : SheetCss.Btn;
            base.FirePropertyChanged("BatchingLabel");
            base.FirePropertyChanged("BatchingClass");
        }

        // ─── Benchmark ───────────────────────────────────────────────────

        public bool IsBenchmarkRunning()
        {
            return this.benchRunning;
        }

        /// <summary>
        /// Runs every op at 50/100/200 rows with batching off and on, three
        /// iterations each, and records the median ms and DOM writes.
        /// Steps are chained through setTimeout so each flush settles and the
        /// status line is painted before the next measurement starts.
        /// </summary>
        public void RunBenchmark()
        {
            if (this.benchRunning) return;
            this.benchRunning = true;
            this.results.Clear();
            this.steps = new List<Action>();
            this.stepIndex = 0;

            int[] sizes = new int[] { 50, 100, 200 };
            string[] ops = new string[] { "a1", "shift", "random" };
            for (int s = 0; s < sizes.Length; s++)
            {
                this.steps.Add(this.MakeSetRowsStep(sizes[s]));
                for (int o = 0; o < ops.Length; o++)
                {
                    this.steps.Add(this.MakeMeasureStep(ops[o], sizes[s], false));
                    this.steps.Add(this.MakeMeasureStep(ops[o], sizes[s], true));
                }
            }
            this.NextStep();
        }

        private void NextStep()
        {
            if (this.stepIndex >= this.steps.Count)
            {
                this.SetBatching(true);
                this.benchRunning = false;
                this.StatusText = "Benchmark done: " + this.results.Count.ToString() + " results";
                return;
            }
            Action step = this.steps[this.stepIndex];
            this.stepIndex = this.stepIndex + 1;
            Globals.SetTimeout(step, 15);
        }

        private Action MakeSetRowsStep(int size)
        {
            return delegate()
            {
                this.Build(size);
                this.StatusText = "Rows: " + size.ToString();
                this.NextStep();
            };
        }

        private Action MakeMeasureStep(string op, int size, bool batched)
        {
            return delegate()
            {
                this.SetBatching(batched);
                this.seed = 42;
                this.StatusText = "Measuring " + OpLabel(op) + " x " + size.ToString() + " rows, "
                    + (batched ? "batched" : "sync");
                // Let the status line flush before the measurement window opens.
                Globals.SetTimeout(delegate()
                {
                    this.RunIteration(op, size, batched, Iterations, new List<double>(), new List<int>());
                }, 15);
            };
        }

        /// <summary>
        /// Two passes per cell. Timed passes run with no MutationObserver so the
        /// number is property changes + flush + forced layout, unskewed by record
        /// allocation. One final observed pass only counts DOM writes.
        /// </summary>
        private void RunIteration(string op, int size, bool batched, int remaining,
            List<double> msList, List<int> writesList)
        {
            Element sheet = Window.Instance.Document.GetElementById("sheet");
            bool countOnly = remaining == 0;
            Perf.Measure(sheet, countOnly, delegate() { this.RunOp(op); }, delegate(double ms, int writes)
            {
                if (countOnly) writesList.Add(writes); else msList.Add(ms);
                if (remaining > 0)
                {
                    this.RunIteration(op, size, batched, remaining - 1, msList, writesList);
                    return;
                }
                this.results.Add(new BenchResultViewModel(OpLabel(op), size, batched ? "batched" : "sync",
                    MinD(msList), MedianI(writesList)));
                this.NextStep();
            });
        }

        private static string OpLabel(string op)
        {
            if (op == "a1") return "A1 + 1";
            if (op == "shift") return "All A + 1";
            return "Randomize A,B";
        }

        private static double MinD(List<double> xs)
        {
            double m = xs[0];
            for (int i = 1; i < xs.Count; i++) if (xs[i] < m) m = xs[i];
            return m;
        }

        private static int MedianI(List<int> xs)
        {
            var copy = new List<int>();
            for (int i = 0; i < xs.Count; i++) copy.Add(xs[i]);
            for (int i = 1; i < copy.Count; i++)
            {
                int v = copy[i]; int j = i - 1;
                while (j >= 0 && copy[j] > v) { copy[j + 1] = copy[j]; j = j - 1; }
                copy[j + 1] = v;
            }
            return copy[copy.Count / 2];
        }

        /// <summary>Results as JSON for the headless benchmark script.</summary>
        public string ResultsJson()
        {
            string s = "[";
            for (int i = 0; i < this.results.Count; i++)
            {
                var r = this.results[i];
                if (i > 0) s = s + ",";
                s = s + "{\"op\":\"" + r.Op + "\",\"rows\":" + r.Rows.ToString()
                    + ",\"mode\":\"" + r.Mode + "\",\"ms\":" + r.Ms.ToFixed(2)
                    + ",\"writes\":" + r.Writes.ToString() + "}";
            }
            return s + "]";
        }
    }
}
