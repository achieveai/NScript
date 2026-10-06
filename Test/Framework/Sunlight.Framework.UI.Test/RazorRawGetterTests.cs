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
            var child = new RazorModeChild();
            child.Leaf = "leaf1";
            child.Items = Leaves("w1", "w2");
            vm.Child = child;
            // Nested loop over a computed getter of the outer item: c1 has two tags, c2 one.
            var children = new ObservableCollection<RazorModeChild>();
            children.Add(Child("c1", "t1", "t2"));
            children.Add(Child("c2", "t3"));
            vm.Children = children;
            return vm;
        }

        private static ObservableCollection<RazorModeChild> Leaves(params string[] leaves)
        {
            var items = new ObservableCollection<RazorModeChild>();
            foreach (var leaf in leaves) items.Add(Child(leaf));
            return items;
        }

        private static RazorModeChild Child(string leaf, params string[] tags)
        {
            var child = new RazorModeChild();
            child.Leaf = leaf;
            child.Tags = new ObservableCollection<string>();
            foreach (var tag in tags) child.Tags.Add(tag);
            return child;
        }

        private static string[] Texts(Element scope, string selector)
        {
            var spans = scope.QuerySelectorAll(selector);
            var texts = new string[spans.Length];
            for (var i = 0; i < spans.Length; i++) texts[i] = spans[i].TextContent;
            return texts;
        }

        private static Element Render(RazorModeVM vm)
        {
            var host = Window.Instance.Document.CreateElement("div");
            Mount(vm, host);
            return host;
        }

        private static UISkinableElement Mount(RazorModeVM vm, Element host)
        {
            var control = new UISkinableElement(host);
            control.DataContext = vm;
            control.Skin = RazorRawGetterTemplates.RazorRawGetterAutoProps;
            control.Activate();
            return control;
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
        public static void TestTemplateOnlyComputedGetterRenders(Assert assert)
        {
            // Issue #82: a computed getter read only by the skin (@Model.ReproComputed) must be
            // retained and emitted under the name the getter calls. Before the fix the getter was
            // dead-code-eliminated and mounting threw "get_reproComputed is not a function".
            var host = Render(NewVm(true, false, "parent"));
            assert.Equal("rc-parent", CaseClass(host, "M6"),
                "M6 template-only computed getter (@Model.ReproComputed)");
        }

        [Test]
        public static void TestChainedBindingUpdatesOnLeafAndMidPathChange(Assert assert)
        {
            // A chained path binding (@Model.Child.Leaf) must listen on the leaf, not just the root.
            // It must update when the leaf changes, and move its listener when the whole child is
            // replaced so later leaf changes on the new child still propagate.
            var vm = NewVm(true, false, "parent");
            var host = Render(vm);
            assert.Equal("leaf1", CaseClass(host, "M7"), "M7 chained initial (Model.Child.Leaf)");

            vm.Child.Leaf = "leaf2";
            assert.Equal("leaf2", CaseClass(host, "M7"), "M7 updates when the leaf changes");

            var replacement = new RazorModeChild();
            replacement.Leaf = "leaf3";
            vm.Child = replacement;
            assert.Equal("leaf3", CaseClass(host, "M7"), "M7 updates when the mid-path child is replaced");

            vm.Child.Leaf = "leaf4";
            assert.Equal("leaf4", CaseClass(host, "M7"),
                "M7 updates when the new child's leaf changes (listener moved)");
        }

        [Test]
        public static void TestChainedBindingSurvivesNullMidPathAndForeignDataContext(Assert assert)
        {
            // Item 1 of the 1.1.12 gaps: the chained subscription (Model.Child.Leaf) used to call
            // every hop's owner getter even when the DataContext was null, so a foreign DataContext
            // (AsType → null) or a cleared mid-path object threw during re-wiring or flush.
            var vm = NewVm(true, false, "parent");
            var host = Window.Instance.Document.CreateElement("div");
            var control = Mount(vm, host);
            assert.Equal("leaf1", CaseClass(host, "M7"), "M7 initial");

            vm.Child = null;
            assert.Equal("", CaseClass(host, "M7"), "M7 falls back to the empty default when the child is null");

            var again = new RazorModeChild();
            again.Leaf = "leaf-again";
            vm.Child = again;
            assert.Equal("leaf-again", CaseClass(host, "M7"), "M7 recovers when the child is set again");
            again.Leaf = "leaf-again-2";
            assert.Equal("leaf-again-2", CaseClass(host, "M7"), "M7 follows the leaf of the re-set child");

            // A DataContext of another type: the skin resolves it to null and must not throw.
            control.DataContext = new RazorModeRow();
            assert.Equal("", CaseClass(host, "M7"), "M7 falls back when the DataContext is a foreign type");

            control.DataContext = vm;
            assert.Equal("leaf-again-2", CaseClass(host, "M7"), "M7 renders again once the view-model is restored");
            vm.Child.Leaf = "leaf-final";
            assert.Equal("leaf-final", CaseClass(host, "M7"), "M7 is re-wired to the restored view-model");
        }

        [Test]
        public static void TestNullSafeHopKeepsGroupingInOrAndConditionPositions(Assert assert)
        {
            // PR #105 review F-001: a null-safe mid-path read is emitted as
            // `(h = dc.child) == null ? null : h.flag`. As the left operand of `||` or as a ternary
            // condition it must stay grouped; ungrouped, a null Child made M8 and M9 evaluate to
            // null (class "") instead of taking the `||` fallback / the "no" branch.
            var vm = NewVm(true, true, "parent"); // Other = true, Child.Flag = false
            var host = Render(vm);
            assert.Equal("on", CaseClass(host, "M8"), "M8 (Model.Child.Flag || Model.Other): false || true");
            assert.Equal("no", CaseClass(host, "M9"), "M9 (Model.Child.Flag ? yes : no): false");

            vm.Child.Flag = true;
            assert.Equal("yes", CaseClass(host, "M9"), "M9 follows the chained flag");
            vm.Other = false;
            assert.Equal("on", CaseClass(host, "M8"), "M8: true || false");
            vm.Child.Flag = false;
            assert.Equal("off", CaseClass(host, "M8"), "M8: false || false");

            vm.Child = null;
            assert.Equal("off", CaseClass(host, "M8"), "M8 with a null Child: null || false");
            assert.Equal("no", CaseClass(host, "M9"), "M9 with a null Child selects the false branch");
            vm.Other = true;
            assert.Equal("on", CaseClass(host, "M8"), "M8 with a null Child and a true OR fallback");

            var again = Child("again");
            again.Flag = true;
            vm.Child = again;
            assert.Equal("yes", CaseClass(host, "M9"), "M9 recovers when the child is set again");
            vm.Other = false;
            assert.Equal("on", CaseClass(host, "M8"), "M8 reads the re-set child's flag");
        }

        [Test]
        public static void TestThreeHopChainedBindingClearsAndRecovers(Assert assert)
        {
            // PR #105 review F-002: @Model.Child.Inner.Leaf has two null-safe hops. The binding
            // must clear when either hop is null, recover when it is set, and re-wire when the
            // child is replaced.
            var vm = NewVm(true, false, "parent"); // Child.Inner = null
            var host = Render(vm);
            assert.Equal("", CaseClass(host, "M10"), "M10 falls back while Inner is null");

            vm.Child.Inner = Child("in1");
            assert.Equal("in1", CaseClass(host, "M10"), "M10 renders once Inner is set");
            vm.Child.Inner.Leaf = "in2";
            assert.Equal("in2", CaseClass(host, "M10"), "M10 follows the third hop's leaf");

            var replacement = Child("leaf-r");
            replacement.Inner = Child("in3");
            vm.Child = replacement;
            assert.Equal("in3", CaseClass(host, "M10"), "M10 re-renders from the replacement child's Inner");
            replacement.Inner.Leaf = "in4";
            assert.Equal("in4", CaseClass(host, "M10"), "M10 listener moved to the replacement's Inner");
            replacement.Inner = Child("in5");
            assert.Equal("in5", CaseClass(host, "M10"), "M10 re-renders when the middle hop is replaced");

            vm.Child.Inner = null;
            assert.Equal("", CaseClass(host, "M10"), "M10 clears when the middle hop is nulled");
            vm.Child = null;
            assert.Equal("", CaseClass(host, "M10"), "M10 clears when the first hop is nulled");

            vm.Child = replacement;
            replacement.Inner = Child("in6");
            assert.Equal("in6", CaseClass(host, "M10"), "M10 recovers after both hops are restored");
        }

        [Test]
        public static void TestLoopOverTemplateOnlyComputedGetterRenders(Assert assert)
        {
            // Item 2 of the 1.1.12 gaps: a getter read ONLY as a @foreach source (Model.RowsView)
            // was not retained, and mounting threw "get_rowsView is not a function".
            var vm = NewVm(true, false, "parent");
            var host = Render(vm);
            assert.DeepEqual(new string[] { "r1", "r2", "r3", "r4", "styled-content" },
                Texts(host, "[data-rowview]"), "rows rendered through the computed getter");

            vm.Rows.Add(Row("r6", false, false));
            assert.Equal(6, host.QuerySelectorAll("[data-rowview]").Length,
                "the computed getter returns the live collection, so an Add renders incrementally");
        }

        [Test]
        public static void TestNestedLoopOverLoopVariableComputedGetterRenders(Assert assert)
        {
            // The same retention inside an item template: @foreach (var t in c.TagsView) where
            // TagsView is a getter of the OUTER loop item.
            var vm = NewVm(true, false, "parent");
            var host = Render(vm);
            assert.Equal(2, host.QuerySelectorAll("[data-child]").Length, "two children rendered");
            assert.DeepEqual(new string[] { "t1", "t2", "t3" }, Texts(host, "[data-tag]"),
                "tags rendered through the item's computed getter");

            vm.Children[0].Tags.Add("t4");
            assert.DeepEqual(new string[] { "t1", "t2", "t4", "t3" }, Texts(host, "[data-tag]"),
                "an Add on the inner collection renders inside its own child");
        }

        [Test]
        public static void TestChainedLoopSourceFollowsEveryHop(Assert assert)
        {
            // Item 3 of the 1.1.12 gaps: @foreach (var w in Model.Child.Items) used to fail to
            // compile (ERR0123 'w.Name' cannot be resolved). It must render, add incrementally,
            // and re-render when the collection, the child, or a null child changes.
            var vm = NewVm(true, false, "parent");
            var host = Render(vm);
            assert.DeepEqual(new string[] { "w1", "w2" }, Texts(host, "[data-chained-item]"), "initial items");

            // The loop variable must be typed through the chain (Model → Child → Items → item),
            // otherwise item bindings are OneTime and never update.
            vm.Child.Items[0].Leaf = "w1-edited";
            assert.DeepEqual(new string[] { "w1-edited", "w2" }, Texts(host, "[data-chained-item]"),
                "an item property change updates the rendered item (loop variable typed through the chain)");

            vm.Child.Items.Add(Child("w3"));
            assert.DeepEqual(new string[] { "w1-edited", "w2", "w3" }, Texts(host, "[data-chained-item]"),
                "incremental Add on the chained collection");

            vm.Child.Items = Leaves("x1");
            assert.DeepEqual(new string[] { "x1" }, Texts(host, "[data-chained-item]"),
                "replacing the collection (last hop) re-renders");

            var replacement = new RazorModeChild();
            replacement.Leaf = "leaf-r";
            replacement.Items = Leaves("y1", "y2");
            vm.Child = replacement;
            assert.DeepEqual(new string[] { "y1", "y2" }, Texts(host, "[data-chained-item]"),
                "replacing the child (mid hop) re-renders from the new child's collection");
            replacement.Items.Add(Child("y3"));
            assert.Equal(3, host.QuerySelectorAll("[data-chained-item]").Length,
                "the collection listener moved to the new child's collection");

            vm.Child = null;
            assert.Equal(0, host.QuerySelectorAll("[data-chained-item]").Length,
                "a null child clears the loop instead of throwing");

            vm.Child = Child("leaf-z");
            vm.Child.Items = Leaves("z1");
            assert.DeepEqual(new string[] { "z1" }, Texts(host, "[data-chained-item]"),
                "setting the child again and then its collection renders");
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
