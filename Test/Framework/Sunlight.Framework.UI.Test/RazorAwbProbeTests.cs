namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>
    /// Ports of the AgentWorkBench binder probes (issues #131-#135, #140).
    /// Each test mounts the exact skin from its issue the way AWB does:
    /// DataContext, then Skin, then Activate, with synchronous flushing so a
    /// view-model change is in the DOM on the next line.
    /// </summary>
    [TestFixture]
    public class RazorAwbProbeTests
    {
        private static Element Mount(ProbeVm vm, Skin skin)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = skin;
            control.Activate();
            return element;
        }

        private static string Text(Element root, string test)
        {
            var e = root.QuerySelector("[data-test=" + test + "]");
            return e == null ? null : e.TextContent;
        }

        private static string Cls(Element root, string test)
        {
            var e = root.QuerySelector("[data-test=" + test + "]");
            return e == null ? null : e.ClassName;
        }

        private static int Rows(Element root, string test)
        {
            return root.QuerySelectorAll("[data-test=" + test + "]").Length;
        }

        // Review advisory 1: closing an @if must detach the @foreach's collection listener,
        // or a reopen attaches a second one and every Add renders twice.
        [Test]
        public static void TestR3GateCloseDetachesCollectionListener(Assert assert)
        {
            var vm = VmWithItems(2);
            vm.Flag = true;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbR3GateLoop);
            assert.Equal(Rows(root, "gl-row"), 2, "open: 2 rows");

            vm.Flag = false;
            assert.Equal(Rows(root, "gl-row"), 0, "closed: no rows");
            vm.Flag = true;
            assert.Equal(Rows(root, "gl-row"), 2, "reopened: 2 rows");

            vm.Items.Add(new ProbeItem("n2"));
            assert.Equal(Rows(root, "gl-row"), 3, "one Add after reopen renders one row");
            vm.Items.RemoveAt(0);
            assert.Equal(Rows(root, "gl-row"), 2, "one RemoveAt removes one row");
        }

        // Review advisory 2: an Add whose row build throws must leave the rendered rows in step
        // with the collection, so the next Remove does not hit "Invalid array length".
        [Test]
        public static void TestR3RemoveAfterThrowingAdd(Assert assert)
        {
            var vm = new ProbeVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbR3AddThrows);
            var boom = new ProbeItem("boom");

            string addError = null;
            try { vm.Items.Add(boom); }
            catch (Exception ex) { addError = ex.Message; }
            assert.NotEqual(addError, null, "the failing row build is not swallowed");

            string removeError = null;
            try { vm.Items.Remove(boom); }
            catch (Exception ex) { removeError = ex.Message; }
            assert.Equal(removeError, null, "Remove after a failed Add does not throw");
            assert.Equal(Rows(root, "at-row"), 0, "the failed row is removed with its item");

            vm.Items.Add(new ProbeItem("ok"));
            assert.Equal(Rows(root, "at-row"), 1, "later Adds still render");
            assert.Equal(Text(root, "at-row"), "ok", "later row is bound");
        }

        // #145 follow-up: one @foreach source walks array -> null -> observable -> array
        // inside an @if that closes and reopens midway.
        [Test]
        public static void TestR3SourceWalksArrayNullObservableArray(Assert assert)
        {
            var vm = new ProbeVm();
            vm.Flag = true;
            vm.Src = new ProbeItem[] { new ProbeItem("a0"), new ProbeItem("a1") };
            var root = Mount(vm, RazorSkinTemplatesClass.AwbR3SourceWalk);
            assert.Equal(Rows(root, "sw-row"), 2, "array: 2 rows");

            vm.Src = null;
            assert.Equal(Rows(root, "sw-row"), 0, "null: 0 rows");

            var obs = new Sunlight.Framework.Observables.ObservableCollection<ProbeItem>();
            obs.Add(new ProbeItem("o0"));
            obs.Add(new ProbeItem("o1"));
            vm.Src = obs;
            assert.Equal(Rows(root, "sw-row"), 2, "observable: 2 rows");
            obs.Add(new ProbeItem("o2"));
            assert.Equal(Rows(root, "sw-row"), 3, "observable Add: 3 rows");

            vm.Flag = false;
            assert.Equal(Rows(root, "sw-row"), 0, "@if closed: 0 rows");
            vm.Flag = true;
            assert.Equal(Rows(root, "sw-row"), 3, "@if reopened: 3 rows");

            obs.Add(new ProbeItem("o3"));
            assert.Equal(Rows(root, "sw-row"), 4, "Add after reopen: 4 rows");
            obs.RemoveAt(0);
            assert.Equal(Rows(root, "sw-row"), 3, "RemoveAt after reopen: 3 rows");

            vm.Src = new ProbeItem[] { new ProbeItem("b0") };
            assert.Equal(Rows(root, "sw-row"), 1, "back to an array: 1 row");
            obs.Add(new ProbeItem("o4"));
            assert.Equal(Rows(root, "sw-row"), 1, "the old collection no longer renders");
        }

        private static ProbeVm VmWithItems(int count)
        {
            var vm = new ProbeVm();
            for (int i = 0; i < count; i++)
            {
                vm.Items.Add(new ProbeItem("n" + i));
            }

            return vm;
        }

        [System.Runtime.CompilerServices.Script(@"return e.value;")]
        private static extern string GetValue(Element e);

        [System.Runtime.CompilerServices.Script(@"e.value = v;")]
        private static extern void SetValue(Element e, string v);

        private static void Dispatch(Element target, string type)
        {
            var evt = Window.Instance.Document.CreateEvent("Event");
            evt.InitEvent(type, true, true);
            target.DispatchEvent(evt);
        }

        // #131 A: bindings after a top-level @if.
        [Test]
        public static void TestB2BindingsAfterTopLevelIf(Assert assert)
        {
            var vm = new ProbeVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB2IfTop);

            assert.Equal(Text(root, "b2-before"), "o1", "before-span text on mount");
            assert.Equal(Text(root, "b2-after"), "o1", "after-span text on mount");
            assert.Equal(Cls(root, "b2-after"), "dyn-a", "after-span class on mount");
            assert.Equal(root.QuerySelector("[data-test=b2-if]"), null, "@if closed on mount");

            vm.Flag = true;
            vm.Other = "o2";
            vm.Cls = "dyn-b";

            assert.Equal(Text(root, "b2-if"), "t1", "@if branch text once open");
            assert.Equal(Text(root, "b2-before"), "o2", "before-span text after change");
            assert.Equal(Text(root, "b2-after"), "o2", "after-span text after change");
            assert.Equal(Cls(root, "b2-after"), "dyn-b", "after-span class after change");
        }

        // #131 B: bindings after an @if inside a @foreach row.
        [Test]
        public static void TestB2BindingsAfterIfInLoopRow(Assert assert)
        {
            var vm = VmWithItems(2);
            vm.Items[1].Flag = true;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB2IfLoop);

            var cls = root.QuerySelectorAll("[data-test=b2l-cls]");
            assert.Equal(cls.Length, 2, "two class spans");
            if (cls.Length != 2) return;
            assert.Equal(cls[0].ClassName, "cls-n0", "row 0 class on mount");
            assert.Equal(cls[1].ClassName, "cls-n1", "row 1 class on mount");
            var ifs = root.QuerySelectorAll("[data-test=b2l-if]");
            assert.Equal(ifs.Length, 1, "only row 1 @if open");
            if (ifs.Length == 1) assert.Equal(ifs[0].TextContent, "hint-n1", "row 1 @if text");

            vm.Items[0].Flag = true;
            vm.Items[0].Cls = "cls-z";

            cls = root.QuerySelectorAll("[data-test=b2l-cls]");
            assert.Equal(cls[0].ClassName, "cls-z", "row 0 class after toggle and change");
            assert.Equal(cls[1].ClassName, "cls-n1", "row 1 class unchanged");
            var names = root.QuerySelectorAll("[data-test=b2l-name]");
            assert.Equal(names[0].TextContent, "n0", "row 0 name");
            ifs = root.QuerySelectorAll("[data-test=b2l-if]");
            assert.Equal(ifs.Length, 2, "both @if open");
            if (ifs.Length == 2) assert.Equal(ifs[0].TextContent, "hint-n0", "row 0 @if text");
        }

        // #132: empty static elements before a loop container.
        [Test]
        public static void TestB4EmptyElementsBeforeLoop(Assert assert)
        {
            var vm = VmWithItems(2);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB4EmptyBeforeLoop);

            var list = root.QuerySelector("[data-test=b4-list]");
            assert.NotEqual(list, null, "list container rendered");
            if (list == null) return;
            assert.Equal(list.QuerySelectorAll("[data-test=b4-item]").Length, 2, "two items inside the list");
            assert.Equal(root.QuerySelectorAll("[data-test=b4-item]").Length, 2, "two items overall");
            assert.Equal(Text(root, "b4-after"), "t1", "after-span text");

            vm.Items.Add(new ProbeItem("n2"));
            assert.Equal(list.QuerySelectorAll("[data-test=b4-item]").Length, 3, "added item lands in the list");
        }

        // #132 variant: a void <hr> (no '/') before a text binding and a loop container.
        [Test]
        public static void TestB4VoidElementBeforeLoop(Assert assert)
        {
            var vm = VmWithItems(2);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB4bHrBeforeLoop);

            var list = root.QuerySelector("[data-test=b4b-list]");
            assert.NotEqual(list, null, "list container rendered");
            if (list == null) return;
            assert.Equal(Text(root, "b4b-before"), "o1", "before-span text");
            assert.Equal(list.QuerySelectorAll("[data-test=b4b-item]").Length, 2, "two items inside the list");
            assert.Equal(Text(root, "b4b-after"), "t1", "after-span text");

            vm.Items.Add(new ProbeItem("n2"));
            vm.Other = "o2";
            assert.Equal(list.QuerySelectorAll("[data-test=b4b-item]").Length, 3, "added item lands in the list");
            assert.Equal(Text(root, "b4b-before"), "o2", "before-span text updates");
        }

        // #133 variant: class binding and onclick on a <tr> row inside <tbody>.
        [Test]
        public static void TestB5RowClassAndClickInTbody(Assert assert)
        {
            var vm = VmWithItems(2);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB5bRowEvents);

            var body = root.QuerySelector("[data-test=b5b-body]");
            var rows = body.QuerySelectorAll("[data-test=b5b-row]");
            assert.Equal(rows.Length, 2, "two rows inside tbody");
            if (rows.Length != 2) return;
            assert.Equal(rows[0].ClassName, "cls-n0", "row 0 class");
            assert.Equal(rows[1].ClassName, "cls-n1", "row 1 class");
            assert.Equal(rows[1].TextContent, "n1", "row 1 text");

            rows[1].Click();
            assert.Equal(vm.Items[1].Clicks, 1, "row 1 click reaches its item");
            assert.Equal(vm.Items[0].Clicks, 0, "row 0 not clicked");

            vm.Items[0].Cls = "cls-z";
            assert.Equal(rows[0].ClassName, "cls-z", "row 0 class updates");
        }

        // Review F-001: a text binding that is the only content of <textarea>, in an @if and in rows.
        [Test]
        public static void TestF1TextareaContentBinding(Assert assert)
        {
            var vm = VmWithItems(2);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbF1RawText);

            string error = null;
            try
            {
                vm.Flag = true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            assert.Equal(error, null, "opening the @if must not throw");
            var ta = root.QuerySelector("[data-test=f1-ta]");
            assert.NotEqual(ta, null, "@if textarea rendered");
            if (ta != null) assert.Equal(((TextAreaElement)ta).Value, "t1", "@if textarea shows its binding");
            assert.Equal(Text(root, "f1-after"), "o1", "binding after the @if");

            var rowTas = root.QuerySelectorAll("[data-test=f1-rta]");
            var rowBs = root.QuerySelectorAll("[data-test=f1-rb]");
            assert.Equal(rowTas.Length, 2, "two row textareas");
            if (rowTas.Length != 2 || rowBs.Length != 2) return;
            assert.Equal(((TextAreaElement)rowTas[1]).Value, "n1", "row 1 textarea text");
            assert.Equal(rowBs[0].ClassName, "cls-n0", "row 0 class after the textarea (no shift)");
            assert.Equal(rowBs[1].ClassName, "cls-n1", "row 1 class after the textarea (no shift)");

            vm.Items[1].Name = "m1";
            vm.Items[1].Cls = "cls-z";
            assert.Equal(((TextAreaElement)rowTas[1]).Value, "m1", "row 1 textarea text updates");
            assert.Equal(rowBs[1].ClassName, "cls-z", "row 1 class updates");
        }

        // Review F-002: an element with its own onchange keeps value= one-way; one without writes back.
        [Test]
        public static void TestF2HandlerOwnsWriteBack(Assert assert)
        {
            var vm = new ProbeVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbF2HandlerOwned);

            var own = (InputElement)root.QuerySelector("[data-test=f2-own]");
            var free = (InputElement)root.QuerySelector("[data-test=f2-free]");
            assert.Equal(own.Value, "t1", "handler-owned input shows VM value");

            own.Value = "";
            Dispatch(own, "change");
            assert.Equal(vm.OwnChanges, 1, "author's onchange ran");
            assert.Equal(vm.Text, "t1", "no implicit write-back when the element has onchange");

            free.Value = "typed";
            Dispatch(free, "input");
            assert.Equal(vm.Other, "typed", "element without a handler writes back");

            vm.Text = "t2";
            assert.Equal(own.Value, "t2", "handler-owned input still follows the VM");
        }

        // Review F-002: two-way write-back in a @foreach row and in a re-opened @if branch.
        [Test]
        public static void TestF2TwoWayInRowsAndReopenedGate(Assert assert)
        {
            var vm = VmWithItems(2);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbF2RowAndGate);

            var rows = root.QuerySelectorAll("[data-test=f2-row]");
            assert.Equal(rows.Length, 2, "two row inputs");
            if (rows.Length != 2) return;
            ((InputElement)rows[1]).Value = "edited";
            Dispatch(rows[1], "input");
            assert.Equal(vm.Items[1].Name, "edited", "row 1 input writes its own item");
            assert.Equal(vm.Items[0].Name, "n0", "row 0 item untouched");

            vm.Flag = true;
            vm.Flag = false;
            vm.Flag = true;
            var gate = (InputElement)root.QuerySelector("[data-test=f2-gate]");
            assert.NotEqual(gate, null, "re-opened @if input rendered");
            if (gate == null) return;
            assert.Equal(gate.Value, "o1", "re-opened input shows VM value");
            gate.Value = "again";
            Dispatch(gate, "input");
            assert.Equal(vm.Other, "again", "re-opened @if input writes back");
        }

        // PR 146 review R1: a closed @if branch's input, still held by someone, must not write
        // the model; reopening wires the new input.
        [Test]
        public static void TestF2ClosedGateInputDoesNotWriteBack(Assert assert)
        {
            var vm = VmWithItems(0);
            vm.Flag = true;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbF2RowAndGate);
            var old = (InputElement)root.QuerySelector("[data-test=f2-gate]");
            assert.NotEqual(old, null, "open: input rendered");
            if (old == null) return;
            old.Value = "open-edit";
            Dispatch(old, "input");
            assert.Equal(vm.Other, "open-edit", "open: the input writes back");

            vm.Flag = false;
            assert.Equal(root.QuerySelector("[data-test=f2-gate]"), null, "closed: the input left the DOM");
            old.Value = "stale";
            Dispatch(old, "input");
            Dispatch(old, "change");
            assert.Equal(vm.Other, "open-edit", "closed: the retained input does not write the model");

            vm.Flag = true;
            var fresh = (InputElement)root.QuerySelector("[data-test=f2-gate]");
            assert.NotEqual(fresh, null, "reopened: input rendered");
            if (fresh == null) return;
            fresh.Value = "reopened";
            Dispatch(fresh, "input");
            assert.Equal(vm.Other, "reopened", "reopened: the new input writes back");
            old.Value = "stale-2";
            Dispatch(old, "input");
            assert.Equal(vm.Other, "reopened", "reopened: the old input still does not write the model");

            // Review P-2: a nested @if stays true while the outer @if closes; its input leaves too.
            var nvm = VmWithItems(0);
            nvm.Flag = true;
            nvm.Busy = true;
            var nroot = Mount(nvm, RazorSkinTemplatesClass.AwbF2NestedGate);
            var inner = (InputElement)nroot.QuerySelector("[data-test=f2-nested]");
            assert.NotEqual(inner, null, "nested: inner input rendered");
            if (inner == null) return;
            inner.Value = "n-edit";
            Dispatch(inner, "input");
            assert.Equal(nvm.Text, "n-edit", "nested: the inner input writes back");

            nvm.Flag = false;
            assert.Equal(nroot.QuerySelector("[data-test=f2-nested]"), null, "nested: outer closed, the inner input left the DOM");
            inner.Value = "n-stale";
            Dispatch(inner, "input");
            Dispatch(inner, "change");
            assert.Equal(nvm.Text, "n-edit", "nested: the retained inner input does not write the model");
        }

        // Review F-003: <select value=> with @foreach options.
        [Test]
        public static void TestF3SelectValueWithForeachOptions(Assert assert)
        {
            var vm = VmWithItems(3);
            vm.Choice = "n1";
            var root = Mount(vm, RazorSkinTemplatesClass.AwbF3Select);

            var sel = root.QuerySelector("[data-test=f3-sel]");
            assert.Equal(sel.QuerySelectorAll("option").Length, 3, "three options");
            assert.Equal(GetValue(sel), "n1", "select shows the VM value after mount");

            vm.Items.Add(new ProbeItem("n3"));
            assert.Equal(GetValue(sel), "n1", "value kept after an option is added");

            vm.Choice = "n3";
            assert.Equal(GetValue(sel), "n3", "VM change selects the option");

            vm.Choice = "n9";
            vm.Items.Add(new ProbeItem("n9"));
            assert.Equal(GetValue(sel), "n9", "value applied once its option arrives");

            SetValue(sel, "n0");
            Dispatch(sel, "change");
            assert.Equal(vm.Choice, "n0", "change writes back");
        }

        // Review F-009: a 0 value renders as "0", not "".
        [Test]
        public static void TestF9ZeroValueRenders(Assert assert)
        {
            var vm = new ProbeVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbF9Zero);

            var input = (InputElement)root.QuerySelector("[data-test=f9-in]");
            assert.Equal(input.Value, "0", "0 renders as 0");
            vm.Count = 7;
            assert.Equal(input.Value, "7", "7 renders");
            vm.Count = 0;
            assert.Equal(input.Value, "0", "back to 0");
        }

        // Review F-010: a top-level event after an @if that has an event binds its own element.
        [Test]
        public static void TestF10EventAfterIfBindsOwnElement(Assert assert)
        {
            var vm = new ProbeVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbF10EventAfterIf);

            root.QuerySelector("[data-test=f10-other]").Click();
            assert.Equal(vm.AfterClicks, 0, "a click elsewhere in the root does not fire the handler");

            root.QuerySelector("[data-test=f10-b]").Click();
            assert.Equal(vm.AfterClicks, 1, "the button's own click fires it");

            vm.Flag = true;
            root.QuerySelector("[data-test=f10-g]").Click();
            assert.Equal(vm.GateClicks, 1, "@if button click");
            assert.Equal(vm.AfterClicks, 1, "@if button click does not fire the after handler");
        }

        [System.Runtime.CompilerServices.Script(@"var n = 0; for (var c = e.firstElementChild; c; c = c.nextElementSibling) { if (c.ownerDocument.defaultView.getComputedStyle(c).display != 'none') n++; } return n;")]
        private static extern int LaidOutChildCount(Element e);

        // #143: the @foreach anchor takes no grid cell, with rows and with an empty list.
        [Test]
        public static void TestR143LoopAnchorTakesNoGridCell(Assert assert)
        {
            var vm = VmWithItems(2);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbR143Grid);
            Window.Instance.Document.Body.AppendChild(root);
            try
            {
                var grid = root.QuerySelector("[data-test=g-grid]");
                assert.Equal(LaidOutChildCount(grid), 2, "2 rows: 2 laid-out children");

                vm.Items.Add(new ProbeItem("n2"));
                assert.Equal(LaidOutChildCount(grid), 3, "3 rows: 3 laid-out children");

                vm.Items.Clear();
                assert.Equal(LaidOutChildCount(grid), 0, "empty list: no laid-out children");
            }
            finally
            {
                root.ParentNode.RemoveChild(root);
            }
        }

        // #144: a bound attribute whose value is null is removed; "" stays as attr="".
        [Test]
        public static void TestR144NullAttributeIsRemoved(Assert assert)
        {
            var vm = VmWithItems(1);
            vm.Items[0].Hint = null;
            vm.Text = "";
            var root = Mount(vm, RazorSkinTemplatesClass.AwbR144NullAttr);

            var plain = root.QuerySelector("[data-test=na-plain]");
            var tern = root.QuerySelector("[data-test=na-tern]");
            var title = root.QuerySelector("[data-test=na-title]");
            var empty = root.QuerySelector("[data-test=na-empty]");
            var row = root.QuerySelector("[data-test=na-row]");
            assert.IsTrue(!plain.HasAttribute("data-x"), "mount: null data-x absent");
            assert.IsTrue(!tern.HasAttribute("aria-sort"), "mount: ternary null aria-sort absent");
            assert.IsTrue(!title.HasAttribute("title"), "mount: null title absent");
            assert.IsTrue(!row.HasAttribute("data-x"), "mount: null data-x in a row absent");
            assert.Equal(empty.GetAttribute("data-y"), "", "\"\" stays as an empty attribute");

            vm.Maybe = "v";
            vm.Flag = true;
            vm.Items[0].Hint = "h";
            assert.Equal(plain.GetAttribute("data-x"), "v", "value set");
            assert.Equal(tern.GetAttribute("aria-sort"), "ascending", "ternary value set");
            assert.Equal(title.GetAttribute("title"), "v", "title set");
            assert.Equal(row.GetAttribute("data-x"), "h", "row value set");

            vm.Maybe = null;
            vm.Flag = false;
            vm.Items[0].Hint = null;
            assert.IsTrue(!plain.HasAttribute("data-x"), "back to null: data-x removed");
            assert.IsTrue(!tern.HasAttribute("aria-sort"), "back to null: aria-sort removed");
            assert.IsTrue(!title.HasAttribute("title"), "back to null: title removed");
            assert.IsTrue(!row.HasAttribute("data-x"), "back to null: row data-x removed");
        }

        // #145: @foreach over string[] and List<T> renders once; a new instance re-renders.
        [Test]
        public static void TestR145ArrayAndListSourcesRender(Assert assert)
        {
            var vm = new ProbeVm();
            vm.Names = new string[] { "a", "b" };
            vm.List = new System.Collections.Generic.List<ProbeItem>();
            vm.List.Add(new ProbeItem("l0"));
            vm.List.Add(new ProbeItem("l1"));
            vm.Arr = new ProbeItem[] { new ProbeItem("a0"), new ProbeItem("a1") };
            var root = Mount(vm, RazorSkinTemplatesClass.AwbR145Lists);

            var strs = root.QuerySelectorAll("[data-test=ar-s]");
            assert.Equal(strs.Length, 2, "string[]: 2 rows");
            if (strs.Length == 2) assert.Equal(strs[1].TextContent, "b", "string[] row text");
            var lists = root.QuerySelectorAll("[data-test=ar-l]");
            assert.Equal(lists.Length, 2, "List<T>: 2 rows");
            if (lists.Length == 2) assert.Equal(lists[0].TextContent, "l0", "List<T> row text");
            var arrs = root.QuerySelectorAll("[data-test=ar-a]");
            assert.Equal(arrs.Length, 2, "T[]: 2 rows");
            if (arrs.Length == 2) assert.Equal(arrs[1].TextContent, "a1", "T[] row text uses the element type");

            vm.Names = new string[] { "x", "y", "z" };
            vm.Arr = new ProbeItem[] { new ProbeItem("n0") };
            strs = root.QuerySelectorAll("[data-test=ar-s]");
            assert.Equal(strs.Length, 3, "new string[]: re-rendered");
            if (strs.Length == 3) assert.Equal(strs[0].TextContent, "x", "new array row text");
            assert.Equal(root.QuerySelector("[data-test=ar-str]").QuerySelectorAll("[data-test=ar-s]").Length, 3,
                "rows stay inside their container");
            assert.Equal(root.QuerySelectorAll("[data-test=ar-a]").Length, 1, "new T[]: re-rendered");
        }

        // #133: bound attributes on <table>/<tbody> with a @foreach of <tr>.
        [Test]
        public static void TestB5BindingsOnTable(Assert assert)
        {
            var vm = VmWithItems(2);
            Element root = null;
            string error = null;
            try
            {
                root = Mount(vm, RazorSkinTemplatesClass.AwbB5Table);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            assert.Equal(error, null, "Activate must not throw");
            if (root == null) return;
            assert.Equal(Cls(root, "b5-table"), "dyn-a", "table class");
            var body = root.QuerySelector("[data-test=b5-body]");
            assert.Equal(body.GetAttribute("data-x"), "t1", "tbody data-x");
            assert.Equal(body.QuerySelectorAll("[data-test=b5-row]").Length, 2, "two rows inside tbody");
            assert.Equal(root.QuerySelectorAll("[data-ns-bind]").Length, 0, "no data-ns-bind marker left");
            var rows = body.QuerySelectorAll("[data-test=b5-row]");
            if (rows.Length == 2) assert.Equal(rows[1].TextContent, "n1", "row 1 text");

            vm.Cls = "dyn-b";
            vm.Text = "t2";
            vm.Items.Add(new ProbeItem("n2"));
            vm.Items[0].Name = "m0";
            assert.Equal(Cls(root, "b5-table"), "dyn-b", "table class updates");
            assert.Equal(body.GetAttribute("data-x"), "t2", "tbody data-x updates");
            rows = body.QuerySelectorAll("[data-test=b5-row]");
            assert.Equal(rows.Length, 3, "added row lands inside tbody");
            assert.Equal(rows[0].TextContent, "m0", "row 0 text updates");
        }

        // #134: textarea and input in a @foreach row.
        [Test]
        public static void TestB7TwinControlsNoLeak(Assert assert)
        {
            var vm = VmWithItems(2);
            vm.Items[0].Flag = true;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB7TwinControls);

            var rootEl = root.QuerySelector("[data-test=b7-root]");
            assert.Equal(rootEl.TextContent.IndexOf("data-ns"), -1, "no marker markup as text");
            var tas = root.QuerySelectorAll("[data-test=b7-ta]");
            var ins = root.QuerySelectorAll("[data-test=b7-in]");
            assert.Equal(tas.Length, 2, "two textareas");
            assert.Equal(ins.Length, 2, "two inputs");
            if (tas.Length != 2 || ins.Length != 2) return;
            assert.Equal(tas[0].GetAttribute("placeholder"), "hint-n0", "textarea 0 placeholder");
            assert.Equal(ins[0].GetAttribute("placeholder"), "hint-n0", "input 0 placeholder");
            assert.Equal(tas[0].ClassName, "x", "textarea 0 class (Flag true)");
            assert.Equal(ins[0].ClassName, "awb-hidden", "input 0 class (Flag true)");
            assert.Equal(tas[1].ClassName, "awb-hidden", "textarea 1 class (Flag false)");
            assert.Equal(ins[1].ClassName, "x", "input 1 class (Flag false)");
            var rows = root.QuerySelectorAll("[data-test=b7-row]");
            assert.Equal(rows[0].ClassName, "cls-n0", "row 0 class");

            vm.Items[1].Hint = "h2";
            assert.Equal(tas[1].GetAttribute("placeholder"), "h2", "textarea 1 placeholder updates");
            assert.Equal(ins[1].GetAttribute("placeholder"), "h2", "input 1 placeholder updates");
        }

        // #135: boolean attributes bound to false are removed.
        [Test]
        public static void TestB8DisabledAndReadonlyRemoved(Assert assert)
        {
            var vm = new ProbeVm();
            vm.Busy = true;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB8Disabled);

            var btn = root.QuerySelector("[data-test=b8-btn]");
            var ro = root.QuerySelector("[data-test=b8-ro]");
            assert.IsTrue(btn.HasAttribute("disabled"), "disabled present while Busy");
            assert.IsTrue(ro.HasAttribute("readonly"), "readonly present while Busy");

            vm.Busy = false;
            assert.IsTrue(!btn.HasAttribute("disabled"), "disabled removed after Busy = false");
            assert.IsTrue(!ro.HasAttribute("readonly"), "readonly removed after Busy = false");

            vm.Busy = true;
            assert.Equal(btn.GetAttribute("disabled"), "", "disabled back as empty attribute");
            assert.Equal(ro.GetAttribute("readonly"), "", "readonly back as empty attribute");
        }

        // #140: value= on input/textarea writes back on input and change.
        [Test]
        public static void TestB9TwoWayValue(Assert assert)
        {
            var vm = new ProbeVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbB9TwoWay);

            var input = root.QuerySelector("[data-test=b9-in]");
            var ta = root.QuerySelector("[data-test=b9-ta]");
            assert.Equal(((InputElement)input).Value, "t1", "input shows VM value");
            assert.Equal(((TextAreaElement)ta).Value, "o1", "textarea shows VM value");

            ((InputElement)input).Value = "changed";
            Dispatch(input, "change");
            assert.Equal(vm.Text, "changed", "input change writes back");

            ((TextAreaElement)ta).Value = "changed2";
            Dispatch(ta, "change");
            assert.Equal(vm.Other, "changed2", "textarea change writes back");

            ((InputElement)input).Value = "typed";
            Dispatch(input, "input");
            assert.Equal(vm.Text, "typed", "input event writes back");

            ((TextAreaElement)ta).Value = "typed2";
            Dispatch(ta, "input");
            assert.Equal(vm.Other, "typed2", "textarea input event writes back");

            vm.Text = "";
            assert.Equal(((InputElement)input).Value, "", "VM -> DOM still works");
            vm.Other = "o3";
            assert.Equal(((TextAreaElement)ta).Value, "o3", "VM -> DOM textarea still works");
        }
    }
}
