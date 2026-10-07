namespace SpreadsheetApp.Test.ViewModels
{
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework;
    using SpreadsheetApp.Formula;
    using SpreadsheetApp.ViewModels;

    /// <summary>
    /// Sheet behaviour without the DOM: edits propagate through precedents,
    /// cycles are refused, copy/paste shifts references, formats resolve
    /// cell &gt; row &gt; column, and the demo build wires its totals.
    /// </summary>
    [TestFixture]
    public class SheetViewModelTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        private static SheetViewModel NewSheet(int rows)
        {
            var vm = new SheetViewModel(Window.Instance.Document.CreateElement("div"));
            vm.Build(rows);
            return vm;
        }

        [Test]
        public static void TestEditPropagatesThroughPrecedents(Assert assert)
        {
            var vm = NewSheet(5);
            vm.SelectCell(vm.CellNamed("B3"));
            assert.Equal("B3", vm.SelectedName, "selection name");
            assert.Equal("3", vm.EditText, "bar shows the raw literal");

            vm.Commit("=A3*10");
            assert.Equal(30, vm.CellNamed("B3").Value.NumberValue, "B3 recomputed");
            assert.Equal("$90.00", vm.CellNamed("C3").Display, "C3 = A3*B3 with the currency column format");
            assert.Equal(1 + 2 + 30 + 4 + 5, vm.CellNamed("B6").Value.NumberValue, "totals row follows");
            assert.Equal(true, vm.CellNamed("E5").Value.NumberValue > 0, "running total chain stays live");

            vm.CellNamed("A3").SetNumber(0);
            assert.Equal(0, vm.CellNamed("B3").Value.NumberValue, "formula re-evaluates when its precedent changes");
        }

        /// <summary>
        /// Enter in the formula bar commits and moves down; the browser then
        /// fires the input's change event with the old text, which must not be
        /// committed into the newly selected cell.
        /// </summary>
        [Test]
        public static void TestEnterCommitDoesNotLeakIntoNextCell(Assert assert)
        {
            var vm = NewSheet(5);
            vm.SelectCell(vm.CellNamed("B3"));
            vm.CommitAndMoveDown("=A3*10");
            vm.CommitFromBar("=A3*10");
            assert.Equal("B4", vm.SelectedName, "selection moved down");
            assert.Equal("4", vm.CellNamed("B4").Raw, "B4 keeps its own literal");
            assert.Equal("=A3*10", vm.CellNamed("B3").Raw, "B3 got the formula");

            vm.CommitFromBar("9");
            assert.Equal("9", vm.CellNamed("B4").Raw, "a real change still commits");
        }

        [Test]
        public static void TestCycleIsRefused(Assert assert)
        {
            var vm = NewSheet(3);
            vm.SelectCell(vm.CellNamed("A1"));
            vm.Commit("=E1");
            assert.Equal("#CYCLE!", vm.CellNamed("A1").Value.ErrorCode, "A1 -> E1 -> D1 -> A1 is a cycle");
            vm.Commit("=K1+1");
            assert.Equal(1, vm.CellNamed("A1").Value.NumberValue, "a non-cyclic formula replaces the error");
            vm.SelectCell(vm.CellNamed("K1"));
            vm.Commit("=K1");
            assert.Equal("#CYCLE!", vm.CellNamed("K1").Value.ErrorCode, "self reference");
        }

        [Test]
        public static void TestCopyPasteShiftsReferences(Assert assert)
        {
            var vm = NewSheet(5);
            vm.SelectCell(vm.CellNamed("C1"));
            vm.CopySelected();
            vm.SelectCell(vm.CellNamed("K5"));
            vm.PasteToSelected();
            assert.Equal("=I5*J5", vm.CellNamed("K5").Raw, "relative refs move with the paste");

            vm.SelectCell(vm.CellNamed("J1"));
            vm.FillDown();
            assert.Equal("J2", vm.SelectedName, "fill down selects the cell below");
            assert.Equal("=ROUND(C2*$B$1%,2)", vm.CellNamed("J2").Raw, "$B$1 stays fixed");
        }

        [Test]
        public static void TestFormatPrecedence(Assert assert)
        {
            var vm = NewSheet(3);
            var a1 = vm.CellNamed("A1");
            vm.SelectCell(a1);
            assert.Equal("1", a1.Display, "general by default");

            vm.ScopeColumn();
            vm.FormatPercent();
            assert.Equal("100.0%", a1.Display, "column format");
            assert.Equal("Percent", vm.Headers[0].FormatName, "header shows the column format");

            vm.ScopeRow();
            vm.FormatCurrency();
            assert.Equal("$1.00", a1.Display, "row format beats column");
            assert.Equal("$1.00", vm.CellNamed("B1").Display, "whole row re-formatted");

            vm.ScopeCell();
            vm.FormatInteger();
            assert.Equal("1", a1.Display, "cell format beats row");
            vm.FormatGeneral();
            assert.Equal("$1.00", a1.Display, "clearing the cell format falls back to the row");

            vm.SelectCell(vm.CellNamed("L1"));
            vm.Commit("2026-10-06");
            assert.Equal("2026-10-06", vm.CellNamed("L1").Display, "typed ISO date keeps reading as a date");
            vm.Commit("=DATE(2026,10,7)");
            assert.Equal("2026-10-07", vm.CellNamed("L1").Display, "the auto date format survives a formula");
        }

        [Test]
        public static void TestErrorsAndLiterals(Assert assert)
        {
            var vm = NewSheet(2);
            vm.SelectCell(vm.CellNamed("B2"));
            vm.Commit("=1/0");
            assert.Equal("#DIV/0!", vm.CellNamed("B2").Display, "division error shows in the cell");
            assert.Equal("#DIV/0!", vm.CellNamed("C2").Display, "and propagates to dependents");
            assert.Equal(true, vm.CellNamed("B2").CssClass.IndexOf(SheetCss.Error) >= 0, "error class");

            vm.Commit("=SUM(A1");
            assert.Equal("#NAME?", vm.CellNamed("B2").Display, "parse error");
            vm.Commit("hello");
            assert.Equal("hello", vm.CellNamed("B2").Display, "text literal");
            assert.Equal("#VALUE!", vm.CellNamed("C2").Display, "text in arithmetic");
            vm.Commit("true");
            assert.Equal("TRUE", vm.CellNamed("B2").Display, "boolean literal");
            vm.Commit("");
            assert.Equal("", vm.CellNamed("B2").Display, "cleared");
            assert.Equal(0, vm.CellNamed("C2").Value.NumberValue, "empty counts as zero");
        }
    }
}
