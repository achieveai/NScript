namespace Sunlight.Framework.UI.Test
{
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Issue #102: bound Razor attribute expressions other than a single-property
    /// ternary used to read unsuffixed auto-property backing fields (undefined).
    /// Row data is chosen so the correct answer differs from the "always falsy/undefined" bug answer.
    /// Cases: A control; B ==; C and; D or; E concat; G nested ternary; F/F2 root;
    /// H Model.X inside foreach; I bare loop variable; J negation; K string equality against a
    /// literal that is also a CSS class in this assembly; L Control.X.
    /// </summary>
    [TestFixture]
    public class RazorRawGetterTests
    {
        private static RazorModeRow Row(string name, bool editable, bool hasDescription)
        {
            var row = new RazorModeRow();
            row.Name = name;
            row.IsEditable = editable;
            row.HasDescription = hasDescription;
            return row;
        }

        private static string CaseClass(Element scope, string caseId)
        {
            var span = scope.QuerySelector("[data-case=" + caseId + "]");
            return span == null ? "<missing>" : span.ClassName;
        }

        private static RazorModeVM NewVm(bool flag, bool other, string name)
        {
            var vm = new RazorModeVM();
            var rows = new ObservableCollection<RazorModeRow>();
            rows.Add(Row("r1", true, true));
            rows.Add(Row("r2", true, false));
            rows.Add(Row("r3", false, true));
            rows.Add(Row("r4", false, false));
            rows.Add(Row("styled-content", true, true));
            vm.Rows = rows;
            var names = new ObservableCollection<string>();
            names.Add("alpha");
            names.Add("beta");
            vm.Names = names;
            vm.Flag = flag;
            vm.Other = other;
            vm.Name = name;
            // #104 restored-expression cases, set so every render evaluates them safely:
            // Count for / and %, Lead for the deep path Model.Lead.Name, Nick left null so the
            // ?? case (Model.Nick ?? "none") takes the null branch by default.
            vm.Count = 10;
            vm.Lead = Row("lead", true, true);
            return vm;
        }

        private static Element Render(RazorModeVM vm)
        {
            var host = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(host);
            control.DataContext = vm;
            control.Skin = RazorRawGetterTemplates.RazorRawGetterAutoProps;
            control.Activate();
            return host;
        }

        [Test]
        public static void TestForeachAutoPropertyExpressionsReadResolvedFields(Assert assert)
        {
            // Flag=true so H (Model.Flag && row.IsEditable) differs from the undefined answer.
            var host = Render(NewVm(true, false, "parent"));
            var rows = host.QuerySelectorAll("[data-row]");
            assert.Equal(5, rows.Length, "five rows rendered");

            // (IsEditable, HasDescription, Name): r1 (T,T) r2 (T,F) r3 (F,T) r4 (F,F) r5 (T,T,"styled-content")
            var expectA = new string[] { "on", "off", "on", "off", "on" };
            var expectB = new string[] { "on", "on", "off", "off", "on" };
            var expectC = new string[] { "on", "off", "off", "off", "on" };
            var expectD = new string[] { "on", "on", "on", "off", "on" };
            var expectE = new string[] { "x-r1", "x-r2", "x-r3", "x-r4", "x-styled-content" };
            var expectG = new string[] { "both", "edit", "none", "none", "both" };
            var expectH = new string[] { "on", "on", "off", "off", "on" };
            var expectH2 = "p-parent";
            var expectJ = new string[] { "off", "off", "on", "on", "off" };
            var expectK = new string[] { "nomatch", "nomatch", "nomatch", "nomatch", "match" };
            for (var i = 0; i < rows.Length; i++)
            {
                var r = rows[i];
                assert.Equal(expectA[i], CaseClass(r, "A"), "A (control: row.HasDescription ? ...) row " + i);
                assert.Equal(expectB[i], CaseClass(r, "B"), "B (row.IsEditable == true) row " + i);
                assert.Equal(expectC[i], CaseClass(r, "C"), "C (a && b) row " + i);
                assert.Equal(expectD[i], CaseClass(r, "D"), "D (a || b) row " + i);
                assert.Equal(expectE[i], CaseClass(r, "E"), "E (\"x-\" + row.Name) row " + i);
                assert.Equal(expectG[i], CaseClass(r, "G"), "G (nested ternary) row " + i);
                assert.Equal(expectH[i], CaseClass(r, "H"), "H (Model.Flag && row.IsEditable, parent path) row " + i);
                assert.Equal(expectH2, CaseClass(r, "H2"), "H2 (\"p-\" + Model.Name inside foreach) row " + i);
                assert.Equal(expectJ[i], CaseClass(r, "J"), "J (!row.IsEditable) row " + i);
                assert.Equal(expectK[i], CaseClass(r, "K"),
                    "K (row.Name == \"styled-content\", literal is also a CSS class) row " + i);
            }
        }

        [Test]
        public static void TestForeachParentModelReadUpdatesAfterActivation(Assert assert)
        {
            // F-003: H2 is "p-" + Model.Name — a parent-Model read inside a foreach. Initial
            // rendering alone passes even if parent-read recovery were wrongly restricted, so this
            // mutates Model.Name after activation and requires every live row's H2 to update
            // without the row collection being rebuilt.
            var vm = NewVm(true, false, "parent");
            var host = Render(vm);
            var rows = host.QuerySelectorAll("[data-row]");
            assert.Equal(5, rows.Length, "five rows rendered");
            for (var i = 0; i < rows.Length; i++)
                assert.Equal("p-parent", CaseClass(rows[i], "H2"), "H2 initial, row " + i);

            vm.Name = "changed";

            var rowsAfter = host.QuerySelectorAll("[data-row]");
            assert.Equal(5, rowsAfter.Length, "same five rows after parent mutation (collection not replaced)");
            for (var i = 0; i < rowsAfter.Length; i++)
                assert.Equal("p-changed", CaseClass(rowsAfter[i], "H2"),
                    "H2 updates after Model.Name change, row " + i);
        }

        [Test]
        public static void TestRestoredExpressionFormsRenderResolvedValues(Assert assert)
        {
            // #104 "restore full expression support": division and remainder, a deep instance path,
            // an instance method invocation, and null-coalescing all compile to resolved getters
            // and render the correct values at the root level.
            var host = Render(NewVm(true, false, "parent"));
            assert.Equal("lead-lead", CaseClass(host, "M1"), "M1 deep path (\"lead-\" + Model.Lead.Name)");
            assert.Equal("d-parent", CaseClass(host, "M2"), "M2 invocation (Model.Decorate(Model.Name))");
            assert.Equal("yes", CaseClass(host, "M3"), "M3 (Model.Count / 2 % 4 == 1), Count=10");
            assert.Equal("none", CaseClass(host, "M5"), "M5 (Model.Nick ?? \"none\"), Nick null");

            var vm = NewVm(true, false, "x");
            vm.Nick = "nick";
            var host2 = Render(vm);
            assert.Equal("nick", CaseClass(host2, "M5"), "M5 (Model.Nick ?? \"none\"), Nick set");
        }

        [Test]
        public static void TestBareLoopVariableRendersItemValue(Assert assert)
        {
            var host = Render(NewVm(false, false, "p"));
            var spans = host.QuerySelectorAll("[data-name]");
            assert.Equal(2, spans.Length, "two string items rendered");
            if (spans.Length != 2) return;
            assert.Equal("alpha", spans[0].TextContent, "I: bare @s text, item 0");
            assert.Equal("beta", spans[1].TextContent, "I: bare @s text, item 1");
            assert.Equal("alpha", spans[0].GetAttribute("title"), "I: bare @s attribute, item 0");
            assert.Equal("beta", spans[1].GetAttribute("title"), "I: bare @s attribute, item 1");
        }

        [Test]
        public static void TestRootAutoPropertyExpressionsReadResolvedFields(Assert assert)
        {
            var host = Render(NewVm(true, false, "styled-content"));
            assert.Equal("on", CaseClass(host, "F"), "F (Model.Flag == true), Flag=true");
            assert.Equal("off", CaseClass(host, "F2"), "F2 (Flag && Other), Flag=true Other=false");
            assert.Equal("off", CaseClass(host, "J0"), "J0 (!Model.Flag), Flag=true");
            assert.Equal("match", CaseClass(host, "K0"),
                "K0 (Model.Name == \"styled-content\", literal is also a CSS class)");

            host = Render(NewVm(true, true, "other"));
            assert.Equal("on", CaseClass(host, "F2"), "F2 (Flag && Other), both true");
            assert.Equal("nomatch", CaseClass(host, "K0"), "K0 with a different name");

            host = Render(NewVm(false, false, "other"));
            assert.Equal("off", CaseClass(host, "F"), "F, Flag=false");
            assert.Equal("on", CaseClass(host, "J0"), "J0 (!Model.Flag), Flag=false");
        }

        [Test]
        public static void TestControlPropertyExpressionsReadResolvedFields(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new RazorRawGetterControl(element);
            control.DataContext = NewVm(true, false, "p");
            control.Skin = RazorRawGetterControl.DefaultSkin;
            control.Armed = true;
            control.Label = "lbl";
            control.Activate();

            assert.Equal("on", CaseClass(element, "L1"), "L1 (Control.Armed == true), Armed=true");
            assert.Equal("c-lbl", CaseClass(element, "L2"), "L2 (\"c-\" + Control.Label)");
            assert.Equal("on", CaseClass(element, "L3"), "L3 (Control.Armed && Model.Flag), both true");
            assert.Equal("off", CaseClass(element, "L4"), "L4 (!Control.Armed), Armed=true");

            control.Armed = false;
            control.Label = "next";
            assert.Equal("off", CaseClass(element, "L1"), "L1 after Armed=false");
            assert.Equal("c-next", CaseClass(element, "L2"), "L2 after Label change");
            assert.Equal("off", CaseClass(element, "L3"), "L3 after Armed=false");
            assert.Equal("on", CaseClass(element, "L4"), "L4 after Armed=false");
        }
    }
}
