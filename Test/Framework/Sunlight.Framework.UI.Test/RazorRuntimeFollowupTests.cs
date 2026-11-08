namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;

    /// <summary>
    /// A row item whose chained path throws while the row wires its subscriptions. It counts
    /// its own PropertyChanged listeners, so a test can see what a removed row left attached.
    /// </summary>
    public class FuItem : ObservableObject, INotifyPropertyChanged
    {
        private FuItem child;
        public int Clicks;
        public int ListenerCount;

        [AutoFire] public string Name { get; set; }

        [AutoFire] public bool Flag { get; set; }

        [AutoFire] public bool Inner { get; set; }

        /// <summary>Throws for an item named "chain-boom".</summary>
        public FuItem Child
        {
            get
            {
                if (this.Name == "chain-boom") throw new Exception("chain boom");
                return this.child;
            }
            set
            {
                this.child = value;
                this.FirePropertyChanged("Child");
            }
        }

        public new void AddPropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback)
        {
            this.ListenerCount++;
            base.AddPropertyChangedListener(propertyName, callback);
        }

        public new void RemovePropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback)
        {
            this.ListenerCount--;
            base.RemovePropertyChangedListener(propertyName, callback);
        }

        public FuItem(string name)
        {
            this.Name = name;
        }

        public void Select(Element elem, ElementEvent evt)
        {
            this.Clicks++;
        }
    }

    public class FuVm : ObservableObject
    {
        [AutoFire] public ObservableCollection<FuItem> Items { get; set; }

        [AutoFire] public bool Flag { get; set; }

        public FuVm()
        {
            this.Items = new ObservableCollection<FuItem>();
        }
    }

    /// <summary>
    /// A root model for a loop inside a loop that counts every listener added and removed
    /// on it, so a test can see listener churn, not just the net count.
    /// </summary>
    public class FuNestVm : ObservableObject, INotifyPropertyChanged
    {
        public int AddCalls;
        public int RemoveCalls;

        [AutoFire] public string Title { get; set; }

        [AutoFire] public FuItem Sub { get; set; }

        public ObservableCollection<FuVm> Groups { get; private set; }

        public FuNestVm()
        {
            this.Title = "t1";
            this.Groups = new ObservableCollection<FuVm>();
        }

        public new void AddPropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback)
        {
            this.AddCalls++;
            base.AddPropertyChangedListener(propertyName, callback);
        }

        public new void RemovePropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback)
        {
            this.RemoveCalls++;
            base.RemovePropertyChangedListener(propertyName, callback);
        }
    }

    /// <summary>
    /// A collection that can raise Reset (ObservableCollection never does). It re-implements
    /// INotifyCollectionChanged, so the binding graph listens here, not on the base event.
    /// </summary>
    public class FuResetCollection : ObservableCollection<FuItem>, INotifyCollectionChanged
    {
        private Action<INotifyCollectionChanged, CollectionChangedEventArgs> handlers;

        event Action<INotifyCollectionChanged, CollectionChangedEventArgs> INotifyCollectionChanged.CollectionChanged
        {
            add { this.handlers += value; }
            remove { this.handlers -= value; }
        }

        /// <summary>Replaces the contents through the base (unheard), then raises one Reset.</summary>
        public void ResetTo(FuItem[] items)
        {
            this.Clear();
            for (int k = 0; k < items.Length; k++) this.Add(items[k]);
            if (this.handlers != null)
                this.handlers(this, new CollectionChangedEventArgs<FuItem>(CollectionChangedAction.Reset, -1, null, null));
        }
    }

    /// <summary>
    /// A row control whose Dispose throws when its Text starts with "dispose-boom" (the message
    /// names the Text), whose Deactivate throws when its Text starts with "deactivate-boom",
    /// and whose activation throws when its Text is "activate-boom".
    /// </summary>
    public class FuDisposeThrowControl : RazorProbeControl
    {
        /// <summary>How many more Dispose calls may throw; a test lowers it to throw once.</summary>
        public static int DisposeThrowsLeft = 1000000;

        public FuDisposeThrowControl(Element element)
            : base(element)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (this.Text == "activate-boom") throw new Exception("activate boom");
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            if (this.Text != null && this.Text.IndexOf("deactivate-boom") == 0)
                throw new Exception("deactivate boom: " + this.Text);
        }

        protected override void InternalDispose()
        {
            base.InternalDispose();
            if (this.Text != null && this.Text.IndexOf("dispose-boom") == 0 && DisposeThrowsLeft > 0)
            {
                DisposeThrowsLeft--;
                throw new Exception("dispose boom: " + this.Text);
            }
        }
    }

    /// <summary>
    /// Binding-graph runtime follow-ups to the AWB rounds: sub-controls in every root of a
    /// multi-root @if branch, rows kept in step with the collection when a row build throws,
    /// and a failed row's subscriptions retired with its row.
    /// </summary>
    [TestFixture]
    public class RazorRuntimeFollowupTests
    {
        private static Element Mount(object vm, Skin skin)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = skin;
            control.Activate();
            return element;
        }

        // #157: whitespace next to an inline binding is kept; layout whitespace is not.
        [Test]
        public static void TestFuStaticTextKeepsSpaceNextToInlineBinding(Assert assert)
        {
            var vm = new RazorTestVM();
            vm.Name = "Error";
            var element = Mount(vm, RazorSkinTemplatesClass.AwbInlineSpace);

            assert.Equal(SpanText(element, "before"), "Agent \u00b7 Error", "space before a binding");
            assert.Equal(SpanText(element, "after"), "Error units", "space after a binding");
            assert.Equal(SpanText(element, "both"), "Error Error", "space between two bindings");
            assert.Equal(SpanText(element, "tag"), "x Error", "space between a tag and a binding");
            assert.Equal(SpanText(element, "layout"), "Error", "a line break beside a tag is layout");

            vm.Name = "Ok";
            assert.Equal(SpanText(element, "before"), "Agent \u00b7 Ok", "the space survives an update");
        }

        private static string SpanText(Element root, string name)
        {
            return root.QuerySelector("[data-test='" + name + "']").TextContent;
        }

        private static int Count(Element root, string selector)
        {
            return root.QuerySelectorAll(selector).Length;
        }

        private static string BoundText(Element root, string selector)
        {
            var e = root.QuerySelector(selector);
            return e == null ? null : e.GetAttribute("data-bound-text");
        }

        private static string RowName(Element root, int index)
        {
            var rows = root.QuerySelectorAll("[data-test=fr-row]");
            return index < rows.Length ? rows[index].QuerySelector("b").TextContent : null;
        }

        private static string Catch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

        // Item 1: a sub-control that is not in the first top-level element of an @if branch.
        [Test]
        public static void TestFuSubControlInLaterRootOfIfBranch(Assert assert)
        {
            var vm = new ProbeVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuMultiRootIf);
            assert.Equal(Count(root, "[data-ns-subctl]"), 0, "both gates closed: no control hosts");

            for (int round = 0; round < 2; round++)
            {
                string r = "round " + round;
                int created = RazorProbeControl.CreatedCount;
                int disposed = RazorProbeControl.DisposedCount;
                vm.Flag = true;
                assert.Equal(RazorProbeControl.CreatedCount, created + 1, r + ": the second-root control is created once per open");
                var probe = RazorProbeControl.LastCreated;
                assert.Equal(probe.DataContext, vm.Other, r + ": DataContext binds");
                assert.Equal(BoundText(root, "[data-test=mr-a] ~ [data-ns-subctl]"), vm.Text, r + ": Text binds on mount");
                assert.Equal(Count(root, "[data-ns-subctl] span.probe-skin"), 1, r + ": the control is skinned (active)");
                vm.Text = "t-" + round;
                assert.Equal(BoundText(root, "[data-test=mr-a] ~ [data-ns-subctl]"), "t-" + round, r + ": Text binding updates");
                vm.Other = "o-" + round;
                assert.Equal(probe.DataContext, "o-" + round, r + ": DataContext binding updates");

                vm.Flag = false;
                assert.Equal(RazorProbeControl.DisposedCount, disposed + 1, r + ": closing disposes the control");
                assert.Equal(Count(root, "[data-ns-subctl]"), 0, r + ": closed: host gone");
                assert.Equal(RazorProbeControl.CreatedCount, created + 1, r + ": closing creates nothing");
            }

            // Two controls, each inside a different top-level element of one branch.
            for (int round = 0; round < 2; round++)
            {
                string r = "two roots, round " + round;
                int created = RazorProbeControl.CreatedCount;
                int disposed = RazorProbeControl.DisposedCount;
                vm.Busy = true;
                assert.Equal(RazorProbeControl.CreatedCount, created + 2, r + ": both controls created once");
                vm.Text = "pt-" + round;
                vm.Other = "so-" + round;
                assert.Equal(BoundText(root, "[data-test=mr-p] > [data-ns-subctl]"), "pt-" + round, r + ": first-root control binds");
                assert.Equal(BoundText(root, "[data-test=mr-s] > [data-ns-subctl]"), "so-" + round, r + ": second-root control binds");
                vm.Busy = false;
                assert.Equal(RazorProbeControl.DisposedCount, disposed + 2, r + ": closing disposes both");
                assert.Equal(Count(root, "[data-ns-subctl]"), 0, r + ": closed: hosts gone");
            }
        }

        // Item 2 + 3: a first render (RenderItemArray) that throws on one row keeps one tracked
        // row per item, and the failed row's subscriptions go with its row.
        [Test]
        public static void TestFuRenderThrowKeepsRowsInStep(Assert assert)
        {
            var vm = new ProbeVm();
            var boom = new ProbeItem("boom");
            vm.Items.Add(new ProbeItem("a"));
            vm.Items.Add(boom);
            vm.Items.Add(new ProbeItem("c"));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRowsThrow);

            string renderError = Catch(delegate { vm.Flag = true; });
            assert.NotEqual(renderError, null, "the failing row build is not swallowed");
            assert.Equal(Count(root, "[data-test=fr-row]"), 3, "after the throw: one row per item");
            assert.Equal(RowName(root, 0), "a", "the row before the failure is bound");

            var failedRow = root.QuerySelectorAll("[data-test=fr-row]")[1];
            string failedText = failedRow.QuerySelector("b").TextContent;
            assert.Equal(Catch(delegate { vm.Items.Remove(boom); }), null, "Remove of the failed row does not throw");
            assert.Equal(Count(root, "[data-test=fr-row]"), 2, "the failed row goes with its item");
            assert.Equal(RowName(root, 0), "a", "row 0 is still a");

            int clicks = boom.Clicks;
            boom.Name = "renamed";
            failedRow.Click();
            assert.Equal(failedRow.QuerySelector("b").TextContent, failedText, "a removed failed row gets no DOM write");
            assert.Equal(boom.Clicks, clicks, "a removed failed row calls no handler");

            assert.Equal(Catch(delegate { vm.Items[1] = new ProbeItem("c2"); }), null, "Replace of the never-built row does not throw");
            assert.Equal(Count(root, "[data-test=fr-row]"), 2, "Replace keeps the row count");
            assert.Equal(RowName(root, 1), "c2", "the replaced row is bound");
            vm.Items.Add(new ProbeItem("d"));
            assert.Equal(Count(root, "[data-test=fr-row]"), 3, "a later Add renders one row");
            assert.Equal(RowName(root, 2), "d", "the added row is bound");
        }

        // Item 2: a Replace whose new row throws leaves the rows in step.
        [Test]
        public static void TestFuReplaceThrowKeepsRowsInStep(Assert assert)
        {
            var vm = new ProbeVm();
            vm.Flag = true;
            vm.Items.Add(new ProbeItem("a"));
            vm.Items.Add(new ProbeItem("b"));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRowsThrow);
            var boom = new ProbeItem("boom");

            assert.NotEqual(Catch(delegate { vm.Items[1] = boom; }), null, "the failing row build is not swallowed");
            assert.Equal(Count(root, "[data-test=fr-row]"), 2, "after the throw: one row per item");

            var failedRow = root.QuerySelectorAll("[data-test=fr-row]")[1];
            string failedText = failedRow.QuerySelector("b").TextContent;
            assert.Equal(Catch(delegate { vm.Items[1] = new ProbeItem("c"); }), null, "Replace of the failed row does not throw");
            assert.Equal(Count(root, "[data-test=fr-row]"), 2, "Replace keeps the row count");
            assert.Equal(RowName(root, 1), "c", "the replacement row is bound");

            int clicks = boom.Clicks;
            boom.Name = "renamed";
            failedRow.Click();
            assert.Equal(failedRow.QuerySelector("b").TextContent, failedText, "a replaced failed row gets no DOM write");
            assert.Equal(boom.Clicks, clicks, "a replaced failed row calls no handler");

            assert.Equal(Catch(delegate { vm.Items.RemoveAt(0); }), null, "a later Remove does not throw");
            assert.Equal(Count(root, "[data-test=fr-row]"), 1, "Remove takes one row");
            assert.Equal(RowName(root, 0), "c", "the remaining row is c");
        }

        // Item 3: an Add whose row throws partway; removing that row retires what it attached.
        [Test]
        public static void TestFuFailedAddRowRetiresSubscriptions(Assert assert)
        {
            var vm = new ProbeVm();
            vm.Flag = true;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRowsThrow);
            var boom = new ProbeItem("boom");

            assert.NotEqual(Catch(delegate { vm.Items.Add(boom); }), null, "the failing row build is not swallowed");
            var failedRow = root.QuerySelector("[data-test=fr-row]");
            string failedText = failedRow.QuerySelector("b").TextContent;

            assert.Equal(Catch(delegate { vm.Items.Remove(boom); }), null, "Remove after a failed Add does not throw");
            assert.Equal(Count(root, "[data-test=fr-row]"), 0, "the failed row goes with its item");

            int clicks = boom.Clicks;
            boom.Name = "renamed";
            failedRow.Click();
            assert.Equal(failedRow.QuerySelector("b").TextContent, failedText, "a removed failed row gets no DOM write");
            assert.Equal(boom.Clicks, clicks, "a removed failed row calls no handler");
        }

        // The b text of every row with the given data-test, joined by "|".
        private static string Names(Element root, string test)
        {
            var rows = root.QuerySelectorAll("[data-test=" + test + "]");
            string names = "";
            for (int k = 0; k < rows.Length; k++)
                names = names + (k == 0 ? "" : "|") + rows[k].QuerySelector("b").TextContent;
            return names;
        }

        // Round 2 (a): a throw while a row wires its subscriptions (a chained path's getter)
        // must not leak the listeners the row attached before it.
        [Test]
        public static void TestFuWiringThrowRetiresAttachedListeners(Assert assert)
        {
            var vm = new FuVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuChainThrow);
            var boom = new FuItem("chain-boom");

            assert.NotEqual(Catch(delegate { vm.Items.Add(boom); }), null, "the failing wiring is not swallowed");
            assert.Equal(boom.ListenerCount, 0, "the row that threw while wiring is retired at once");
            var failedRow = root.QuerySelector("[data-test=fc-row]");
            string failedText = failedRow.QuerySelector("b").TextContent;
            string failedChildText = failedRow.QuerySelector("i").TextContent;

            assert.Equal(Catch(delegate { vm.Items.Remove(boom); }), null, "Remove after the failed wiring does not throw");
            assert.Equal(Count(root, "[data-test=fc-row]"), 0, "the failed row goes with its item");
            assert.Equal(boom.ListenerCount, 0, "removing the row detaches every listener it attached");

            boom.Name = "renamed";
            boom.Child = new FuItem("kid");
            failedRow.Click();
            assert.Equal(failedRow.QuerySelector("b").TextContent, failedText, "a removed failed row gets no DOM write (plain path)");
            assert.Equal(failedRow.QuerySelector("i").TextContent, failedChildText, "a removed failed row gets no DOM write (chained path)");
            assert.Equal(boom.Clicks, 0, "a removed failed row calls no handler");

            vm.Items.Add(new FuItem("ok"));
            assert.Equal(Names(root, "fc-row"), "ok", "a later Add renders a bound row");
        }

        // Round 2 (b): a row whose sub-control throws on Dispose still leaves the rows in step.
        [Test]
        public static void TestFuThrowingRowDisposeKeepsRowsInStep(Assert assert)
        {
            var vm = new FuVm();
            var bad = new FuItem("dispose-boom");
            vm.Items.Add(new FuItem("a"));
            vm.Items.Add(bad);
            vm.Items.Add(new FuItem("c"));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuDisposeThrow);
            assert.Equal(Names(root, "fd-row"), "a|dispose-boom|c", "mounted: 3 rows");

            assert.NotEqual(Catch(delegate { vm.Items.Remove(bad); }), null, "the throwing Dispose is not swallowed");
            assert.Equal(Names(root, "fd-row"), "a|c", "Remove: the row left even though its Dispose threw");

            vm.Items.Add(new FuItem("d"));
            assert.Equal(Names(root, "fd-row"), "a|c|d", "a later Add lands at the end");
            vm.Items.RemoveAt(0);
            assert.Equal(Names(root, "fd-row"), "c|d", "a later Remove takes the right row");

            vm.Items.Add(new FuItem("dispose-boom"));
            assert.NotEqual(Catch(delegate { vm.Items[2] = new FuItem("e"); }), null, "Replace: the throwing Dispose is not swallowed");
            assert.Equal(Names(root, "fd-row"), "c|d|e", "Replace: the new row is added even though the old row's Dispose threw");
            vm.Items.RemoveAt(1);
            assert.Equal(Names(root, "fd-row"), "c|e", "a Remove after the Replace takes the right row");
        }

        // Round 3: swapping the collection clears its rows (ClearCollectionItems); a middle
        // row whose Dispose throws still lets every row retire and leave.
        [Test]
        public static void TestFuThrowingRowDisposeOnClearRetiresEveryRow(Assert assert)
        {
            var vm = new FuVm();
            var c = new FuItem("c");
            vm.Items.Add(new FuItem("a"));
            vm.Items.Add(new FuItem("dispose-boom"));
            vm.Items.Add(c);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuDisposeThrow);
            assert.Equal(Names(root, "fd-row"), "a|dispose-boom|c", "mounted: 3 rows");
            assert.IsTrue(c.ListenerCount > 0, "the last row listens on its item: " + c.ListenerCount);
            int disposed = RazorProbeControl.DisposedCount;

            assert.NotEqual(Catch(delegate { vm.Items = new ObservableCollection<FuItem>(); }), null,
                "the throwing Dispose is not swallowed");
            assert.Equal(Names(root, "fd-row"), "", "swap: every old row left the DOM");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 3, "swap: every old row's control is disposed");
            assert.Equal(c.ListenerCount, 0, "swap: the row after the throwing one detached its listeners");

            var next = new ObservableCollection<FuItem>();
            next.Add(new FuItem("d"));
            assert.Equal(Catch(delegate { vm.Items = next; }), null, "a later swap does not throw");
            assert.Equal(Names(root, "fd-row"), "d", "a later swap renders its rows");
            next.Add(new FuItem("e"));
            next.RemoveAt(0);
            assert.Equal(Names(root, "fd-row"), "e", "the later collection's Add and Remove keep rows in step");
        }

        // Round 4 (1a): a collection swapped while inactive is cleared on Activate
        // (PushInitialValues); a throwing row Dispose still lets the new collection render.
        [Test]
        public static void TestFuThrowingRowDisposeOnReactivateSwapRendersNewRows(Assert assert)
        {
            var vm = new FuVm();
            var c = new FuItem("c");
            vm.Items.Add(new FuItem("a"));
            vm.Items.Add(new FuItem("dispose-boom"));
            vm.Items.Add(c);
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.AwbFuDisposeThrow;
            control.Activate();
            assert.Equal(Names(element, "fd-row"), "a|dispose-boom|c", "mounted: 3 rows");
            int disposed = RazorProbeControl.DisposedCount;

            control.Deactivate();
            var next = new ObservableCollection<FuItem>();
            next.Add(new FuItem("d"));
            vm.Items = next;
            assert.NotEqual(Catch(delegate { control.Activate(); }), null, "the throwing Dispose is not swallowed");
            assert.Equal(Names(element, "fd-row"), "d", "Activate: the new collection renders");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 3, "Activate: every old row's control is disposed");
            assert.Equal(c.ListenerCount, 0, "Activate: the row after the throwing one detached its listeners");
            next.Add(new FuItem("e"));
            assert.Equal(Names(element, "fd-row"), "d|e", "the new collection's Add renders a row");
        }

        // Round 4 (1b): a Reset whose middle row throws on Dispose still re-renders.
        [Test]
        public static void TestFuThrowingRowDisposeOnResetRendersNewRows(Assert assert)
        {
            var vm = new FuVm();
            var items = new FuResetCollection();
            items.Add(new FuItem("x"));
            vm.Items = items;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuDisposeThrow);
            assert.Equal(Names(root, "fd-row"), "x", "mounted: 1 row");

            var c = new FuItem("c");
            items.ResetTo(new FuItem[] { new FuItem("a"), new FuItem("dispose-boom"), c });
            assert.Equal(Names(root, "fd-row"), "a|dispose-boom|c", "a Reset re-renders the rows (test hook works)");
            int disposed = RazorProbeControl.DisposedCount;

            assert.NotEqual(Catch(delegate { items.ResetTo(new FuItem[] { new FuItem("d") }); }), null,
                "the throwing Dispose is not swallowed");
            assert.Equal(Names(root, "fd-row"), "d", "Reset: the new contents render");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 3, "Reset: every old row's control is disposed");
            assert.Equal(c.ListenerCount, 0, "Reset: the row after the throwing one detached its listeners");
            items.ResetTo(new FuItem[] { new FuItem("e") });
            assert.Equal(Names(root, "fd-row"), "e", "a later Reset keeps rows in step");
        }

        // Round 4 (3): a row whose first root is an @if: its gate content belongs to the row,
        // so an insert before that row lands before the gate content.
        [Test]
        public static void TestFuRowLeadingIfKeepsInsertOrder(Assert assert)
        {
            var vm = new FuVm();
            var a = new FuItem("a");
            var b = new FuItem("b");
            a.Flag = true;
            b.Flag = true;
            vm.Items.Add(a);
            vm.Items.Add(b);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRowLeadIf);
            var box = root.QuerySelector("[data-test=ml-box]");
            assert.Equal(RowNodes(box), "u:+|b:a|u:+|b:b", "mounted: each row is its gate content, then b");

            var m = new FuItem("m");
            m.Flag = true;
            vm.Items.InsertAt(1, m);
            assert.Equal(RowNodes(box), "u:+|b:a|u:+|b:m|u:+|b:b", "InsertAt(1) lands before row b's gate content");

            var z = new FuItem("z");
            vm.Items.InsertAt(0, z);
            assert.Equal(RowNodes(box), "b:z|u:+|b:a|u:+|b:m|u:+|b:b", "InsertAt(0) with a closed gate");
            z.Flag = true;
            assert.Equal(RowNodes(box), "u:+|b:z|u:+|b:a|u:+|b:m|u:+|b:b", "opening z's gate keeps it inside row z");

            var y = new FuItem("y");
            y.Flag = true;
            vm.Items.InsertAt(0, y);
            assert.Equal(RowNodes(box), "u:+|b:y|u:+|b:z|u:+|b:a|u:+|b:m|u:+|b:b", "InsertAt(0) before an opened gate");

            vm.Items.Remove(m);
            assert.Equal(RowNodes(box), "u:+|b:y|u:+|b:z|u:+|b:a|u:+|b:b", "Remove takes the row and its gate content");
            var r = new FuItem("r");
            r.Flag = true;
            vm.Items[3] = r;
            assert.Equal(RowNodes(box), "u:+|b:y|u:+|b:z|u:+|b:a|u:+|b:r", "Replace of the last row keeps the order");
            var s = new FuItem("s");
            s.Flag = true;
            vm.Items[1] = s;
            assert.Equal(RowNodes(box), "u:+|b:y|u:+|b:s|u:+|b:a|u:+|b:r", "Replace of a middle row lands before the next gate content");
        }

        // Round 3: every top-level node of a row, in order. Elements as tag:text, non-blank
        // text as its trimmed text; compiler markers (data-ns-ph) are skipped.
        private static string RowNodes(Element box)
        {
            string signature = "";
            var children = box.ChildNodes;
            for (int k = 0; k < children.Length; k++)
            {
                var node = children[k];
                string part;
                if (node.NodeType == ElementType.Element)
                {
                    var e = node.As<Element>();
                    if (e.HasAttribute("data-ns-ph")) continue;
                    part = e.TagName.ToLowerCase() + ":" + e.TextContent;
                }
                else
                {
                    part = node.TextContent.Trim();
                    if (part.Length == 0) continue;
                }

                signature = signature.Length == 0 ? part : signature + "|" + part;
            }

            return signature;
        }

        // Round 3: a two-root row (<b/><i onclick/>) inside an @if: add, insert, remove,
        // replace, rename, a click on the second root, clear and gate close keep the DOM exact.
        [Test]
        public static void TestFuTwoRootRowsKeepDomExact(Assert assert)
        {
            var vm = new FuVm();
            vm.Flag = true;
            var a = new FuItem("a");
            vm.Items.Add(a);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRow2Root);
            var box = root.QuerySelector("[data-test=m2-box]");
            assert.Equal(RowNodes(box), "b:a|i:a", "mount: both roots of the row render");

            var b = new FuItem("b");
            vm.Items.Add(b);
            assert.Equal(RowNodes(box), "b:a|i:a|b:b|i:b", "Add appends a whole row");
            vm.Items.InsertAt(0, new FuItem("z"));
            assert.Equal(RowNodes(box), "b:z|i:z|b:a|i:a|b:b|i:b", "InsertAt(0) goes before the first row's first root");

            b.Name = "b2";
            assert.Equal(RowNodes(box), "b:z|i:z|b:a|i:a|b:b2|i:b2", "a rename updates both roots");
            var italics = box.QuerySelectorAll("i");
            if (italics.Length > 2) italics[2].Click();
            assert.Equal(b.Clicks, 1, "a click on the second root reaches its own item");
            assert.Equal(a.Clicks, 0, "and no other item");

            vm.Items.RemoveAt(1);
            assert.Equal(RowNodes(box), "b:z|i:z|b:b2|i:b2", "Remove takes both roots of the row");
            vm.Items[0] = new FuItem("y");
            assert.Equal(RowNodes(box), "b:y|i:y|b:b2|i:b2", "Replace swaps both roots in place");

            vm.Items.Clear();
            assert.Equal(RowNodes(box), "", "Clear removes every root");
            vm.Items.Add(new FuItem("c"));
            assert.Equal(RowNodes(box), "b:c|i:c", "Add after Clear renders a whole row");

            vm.Flag = false;
            assert.Equal(root.QuerySelectorAll("b, i").Length, 0, "gate close removes every root");
            vm.Flag = true;
            box = root.QuerySelector("[data-test=m2-box]");
            assert.Equal(RowNodes(box), "b:c|i:c", "gate reopen renders the whole row again");
        }

        // Round 3: a three-root row whose event sits on the last root.
        [Test]
        public static void TestFuThreeRootRowsKeepDomExact(Assert assert)
        {
            var vm = new FuVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRow3Root);
            var box = root.QuerySelector("[data-test=m3-box]");
            assert.Equal(RowNodes(box), "", "empty collection: no row nodes");

            var a = new FuItem("a");
            var b = new FuItem("b");
            vm.Items.Add(a);
            vm.Items.Add(b);
            assert.Equal(RowNodes(box), "span:a|em:-|button:a|span:b|em:-|button:b", "Add renders all three roots");
            vm.Items.InsertAt(1, new FuItem("m"));
            assert.Equal(RowNodes(box), "span:a|em:-|button:a|span:m|em:-|button:m|span:b|em:-|button:b",
                "Insert lands between whole rows");

            var buttons = box.QuerySelectorAll("button");
            if (buttons.Length > 2) buttons[2].Click();
            assert.Equal(b.Clicks, 1, "a click on the third root reaches its own item");
            a.Name = "a2";
            assert.Equal(RowNodes(box), "span:a2|em:-|button:a2|span:m|em:-|button:m|span:b|em:-|button:b",
                "a rename updates the first and third roots");

            vm.Items.RemoveAt(1);
            assert.Equal(RowNodes(box), "span:a2|em:-|button:a2|span:b|em:-|button:b", "Remove takes all three roots");
            vm.Items[1] = new FuItem("c");
            assert.Equal(RowNodes(box), "span:a2|em:-|button:a2|span:c|em:-|button:c", "Replace swaps all three roots");
            vm.Items.Clear();
            assert.Equal(RowNodes(box), "", "Clear removes every root");
        }

        // The data-bound-text of every element the selector matches, joined by "|".
        private static string ControlTexts(Element root, string selector)
        {
            var hosts = root.QuerySelectorAll(selector);
            string texts = "";
            for (int k = 0; k < hosts.Length; k++)
                texts = texts + (k == 0 ? "" : "|") + hosts[k].GetAttribute("data-bound-text");
            return texts;
        }

        // Review F-005: multi-root @if branches already open at mount (PushInitialValues)
        // create the control in every root, not only the first.
        [Test]
        public static void TestFuSubControlInLaterRootOfIfBranchOpenAtMount(Assert assert)
        {
            var vm = new ProbeVm();
            vm.Flag = true;
            vm.Busy = true;
            vm.Text = "t0";
            vm.Other = "o0";
            int created = RazorProbeControl.CreatedCount;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuMultiRootIf);
            assert.Equal(RazorProbeControl.CreatedCount, created + 3, "mount: the three controls of both open branches are created");
            assert.Equal(BoundText(root, "[data-test=mr-a] ~ [data-ns-subctl]"), "t0", "Flag branch: the second-root control binds");
            assert.Equal(BoundText(root, "[data-test=mr-p] > [data-ns-subctl]"), "t0", "Busy branch: the first-root control binds");
            assert.Equal(BoundText(root, "[data-test=mr-s] > [data-ns-subctl]"), "o0", "Busy branch: the second-root control binds");
            assert.Equal(Count(root, "[data-ns-subctl] span.probe-skin"), 3, "all three controls are skinned (active)");

            vm.Text = "t1";
            vm.Other = "o1";
            assert.Equal(BoundText(root, "[data-test=mr-a] ~ [data-ns-subctl]"), "t1", "Flag branch: the second-root control updates");
            assert.Equal(BoundText(root, "[data-test=mr-s] > [data-ns-subctl]"), "o1", "Busy branch: the second-root control updates");

            int disposed = RazorProbeControl.DisposedCount;
            vm.Flag = false;
            vm.Busy = false;
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 3, "closing both branches disposes all three");
            assert.Equal(Count(root, "[data-ns-subctl]"), 0, "closed: hosts gone");
        }

        // Review F-006: a control in a row's second root is created per row, and add, insert,
        // rename, remove, replace and clear keep one bound control per row.
        [Test]
        public static void TestFuSubControlInLaterRowRoot(Assert assert)
        {
            string hosts = "[data-test=mc-c] > [data-ns-subctl]";
            var vm = new FuVm();
            var a = new FuItem("a");
            vm.Items.Add(a);
            int created = RazorProbeControl.CreatedCount;
            int disposed = RazorProbeControl.DisposedCount;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRowCtl);
            assert.Equal(RazorProbeControl.CreatedCount, created + 1, "mount: the row's second-root control is created");
            assert.Equal(ControlTexts(root, hosts), "a", "mount: it binds");

            vm.Items.Add(new FuItem("b"));
            assert.Equal(RazorProbeControl.CreatedCount, created + 2, "Add creates the new row's control");
            assert.Equal(ControlTexts(root, hosts), "a|b", "Add: bound");
            vm.Items.InsertAt(0, new FuItem("z"));
            assert.Equal(RazorProbeControl.CreatedCount, created + 3, "InsertAt creates the new row's control");
            assert.Equal(ControlTexts(root, hosts), "z|a|b", "InsertAt: bound, in order");
            a.Name = "a2";
            assert.Equal(ControlTexts(root, hosts), "z|a2|b", "a rename reaches the row's control");

            vm.Items.RemoveAt(1);
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 1, "Remove disposes the row's control");
            assert.Equal(ControlTexts(root, hosts), "z|b", "Remove: the right control left");
            vm.Items[1] = new FuItem("c");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 2, "Replace disposes the old row's control");
            assert.Equal(RazorProbeControl.CreatedCount, created + 4, "Replace creates the new row's control");
            assert.Equal(ControlTexts(root, hosts), "z|c", "Replace: bound");
            vm.Items.Clear();
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 4, "Clear disposes every row's control");
            assert.Equal(ControlTexts(root, hosts), "", "Clear: no controls left");
        }

        private static FuItem NestItem(string name, bool flag, bool inner)
        {
            var item = new FuItem(name);
            item.Flag = flag;
            item.Inner = inner;
            return item;
        }

        // Review F-007: a row led by an @if whose branch is led by a nested @if (or by an
        // @else): an insert before that row lands before its innermost leading content.
        [Test]
        public static void TestFuRowLeadingNestedIfKeepsInsertOrder(Assert assert)
        {
            var vm = new FuVm();
            vm.Items.Add(NestItem("a", true, true));
            vm.Items.Add(NestItem("b", true, true));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuRowNestIf);
            var box = root.QuerySelector("[data-test=mn-box]");
            assert.Equal(RowNodes(box), "s:+|u:+|b:a|s:+|u:+|b:b", "mounted: nested gate content, outer gate content, b");

            vm.Items.InsertAt(1, NestItem("m", true, false));
            assert.Equal(RowNodes(box), "s:+|u:+|b:a|u:+|b:m|s:+|u:+|b:b",
                "InsertAt(1) lands before row b's nested gate content");

            var e = NestItem("e", false, false);
            vm.Items.InsertAt(0, e);
            assert.Equal(RowNodes(box), "i:-|b:e|s:+|u:+|b:a|u:+|b:m|s:+|u:+|b:b", "InsertAt(0) before a nested-led row");
            vm.Items.InsertAt(0, NestItem("n", true, true));
            assert.Equal(RowNodes(box), "s:+|u:+|b:n|i:-|b:e|s:+|u:+|b:a|u:+|b:m|s:+|u:+|b:b",
                "InsertAt(0) before an @else-led row");

            e.Flag = true;
            e.Inner = true;
            assert.Equal(RowNodes(box), "s:+|u:+|b:n|s:+|u:+|b:e|s:+|u:+|b:a|u:+|b:m|s:+|u:+|b:b",
                "opening e's gates keeps their content inside row e");
            vm.Items.InsertAt(1, NestItem("p", false, false));
            assert.Equal(RowNodes(box), "s:+|u:+|b:n|i:-|b:p|s:+|u:+|b:e|s:+|u:+|b:a|u:+|b:m|s:+|u:+|b:b",
                "InsertAt(1) lands before gates that opened after mount");

            vm.Items.RemoveAt(4);
            assert.Equal(RowNodes(box), "s:+|u:+|b:n|i:-|b:p|s:+|u:+|b:e|s:+|u:+|b:a|s:+|u:+|b:b",
                "Remove takes the row and its gate content");
            vm.Items[2] = NestItem("r", true, true);
            assert.Equal(RowNodes(box), "s:+|u:+|b:n|i:-|b:p|s:+|u:+|b:r|s:+|u:+|b:a|s:+|u:+|b:b",
                "Replace of a middle row lands before the next row's nested gate content");
        }

        // Review F-002: when two steps of one change throw, the first error surfaces.
        [Test]
        public static void TestFuTwoFailuresSurfaceTheFirstError(Assert assert)
        {
            // Clear: rows tear down in order, so the first throwing row's error wins.
            var vm = new FuVm();
            vm.Items.Add(new FuItem("a"));
            vm.Items.Add(new FuItem("dispose-boom-1"));
            vm.Items.Add(new FuItem("dispose-boom-2"));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuDisposeThrow);
            assert.Equal(Catch(delegate { vm.Items = new ObservableCollection<FuItem>(); }), "dispose boom: dispose-boom-1",
                "clear: the first row's error surfaces");
            assert.Equal(Names(root, "fd-row"), "", "clear: every row left");

            // Replace: retiring the old row throws, then building the new row throws.
            vm.Items.Add(new FuItem("x"));
            vm.Items.Add(new FuItem("dispose-boom-r"));
            assert.Equal(Catch(delegate { vm.Items[1] = new FuItem("activate-boom"); }), "dispose boom: dispose-boom-r",
                "replace: the old row's error surfaces, not the new row's");
            assert.Equal(Names(root, "fd-row"), "x|activate-boom", "replace: the new row is in place");

            // Swap: clearing the old rows throws, then rendering the new collection throws.
            vm.Items.Add(new FuItem("dispose-boom-s"));
            var next = new ObservableCollection<FuItem>();
            next.Add(new FuItem("activate-boom"));
            assert.Equal(Catch(delegate { vm.Items = next; }), "dispose boom: dispose-boom-s",
                "swap: the clear's error surfaces, not the render's");
            assert.Equal(Names(root, "fd-row"), "activate-boom", "swap: the new collection rendered");

            // Reset: the same pair on a Reset.
            var items = new FuResetCollection();
            items.Add(new FuItem("dispose-boom-t"));
            assert.Equal(Catch(delegate { vm.Items = items; }), null, "a swap with no failing row does not throw");
            assert.Equal(Catch(delegate { items.ResetTo(new FuItem[] { new FuItem("activate-boom") }); }), "dispose boom: dispose-boom-t",
                "reset: the clear's error surfaces, not the render's");
            assert.Equal(Names(root, "fd-row"), "activate-boom", "reset: the new contents rendered");
        }

        // Review (pre-existing): closing an @if around two @foreach lists whose rows throw on
        // Dispose still resets every node of the branch; reopening renders both lists again.
        [Test]
        public static void TestFuGateCloseWithThrowingRowDisposeRetiresEveryList(Assert assert)
        {
            var vm = new FuVm();
            vm.Flag = true;
            var a = new FuItem("a");
            vm.Items.Add(a);
            vm.Items.Add(new FuItem("dispose-boom"));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuGateClose);
            assert.Equal(Names(root, "fg-row"), "a|dispose-boom|a|dispose-boom", "mounted: both lists render");
            int disposed = RazorProbeControl.DisposedCount;

            assert.Equal(Catch(delegate { vm.Flag = false; }), "dispose boom: dispose-boom", "the throwing Dispose is not swallowed");
            assert.Equal(Names(root, "fg-row"), "", "closed: no rows");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 4, "closed: every row control of both lists is disposed");
            assert.Equal(a.ListenerCount, 0, "closed: no row of either list listens on its item");
            assert.Equal(Catch(delegate { vm.Items.Add(new FuItem("late")); }), null, "an Add while closed does not throw");

            assert.Equal(Catch(delegate { vm.Flag = true; }), null, "reopening does not throw");
            assert.Equal(Names(root, "fg-row"), "a|dispose-boom|late|a|dispose-boom|late", "reopened: both lists render every item");
        }

        // PR 149 review P1: a row's first control throwing in Dispose must not keep the
        // row's later controls from being disposed.
        [Test]
        public static void TestFuThrowingDisposeStillDisposesTheRowsOtherControls(Assert assert)
        {
            var vm = new FuVm();
            var bad = new FuItem("dispose-boom");
            vm.Items.Add(new FuItem("a"));
            vm.Items.Add(bad);
            vm.Items.Add(new FuItem("c"));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuDispose2);
            assert.Equal(Names(root, "fd2-row"), "a|dispose-boom|c", "mounted: 3 rows");
            int disposed = RazorProbeControl.DisposedCount;
            int deactivated = RazorProbeControl.DeactivatedCount;

            // Throw once, so a second Dispose of the same control could not hide behind it.
            FuDisposeThrowControl.DisposeThrowsLeft = 1;
            try
            {
                assert.Equal(Catch(delegate { vm.Items.Remove(bad); }), "dispose boom: dispose-boom",
                    "Remove rethrows the first control's Dispose error");
                assert.Equal(RazorProbeControl.DisposedCount, disposed + 2, "both controls of the row are disposed");
                assert.Equal(RazorProbeControl.DeactivatedCount, deactivated + 2,
                    "both controls of the row are deactivated");
                assert.Equal(Names(root, "fd2-row"), "a|c", "the row left");
            }
            finally
            {
                FuDisposeThrowControl.DisposeThrowsLeft = 1000000;
            }

            vm.Items.Add(new FuItem("d"));
            vm.Items.RemoveAt(0);
            assert.Equal(Names(root, "fd2-row"), "c|d", "a later Add and Remove keep the rows in step");
        }

        // PR 149 review follow-up: a control whose Deactivate throws is still disposed, and the
        // row's other control is too.
        [Test]
        public static void TestFuThrowingDeactivateStillDisposesTheControl(Assert assert)
        {
            var vm = new FuVm();
            var bad = new FuItem("deactivate-boom");
            vm.Items.Add(new FuItem("a"));
            vm.Items.Add(bad);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuDispose2);
            assert.Equal(Names(root, "fd2-row"), "a|deactivate-boom", "mounted: 2 rows");
            int disposed = RazorProbeControl.DisposedCount;

            assert.Equal(Catch(delegate { vm.Items.Remove(bad); }), "deactivate boom: deactivate-boom",
                "Remove rethrows the Deactivate error");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 2,
                "both controls of the row are disposed, the one whose Deactivate threw too");
            assert.Equal(Names(root, "fd2-row"), "a", "the row left");
        }

        // PR 149 review follow-up: a nested list whose row throws while it is torn down does
        // not stop the row's later nested list, the row's own control, or (on host Dispose)
        // the host's own control from being disposed.
        [Test]
        public static void TestFuNestedTeardownErrorStillDisposesEveryOtherControl(Assert assert)
        {
            var vm = new FuNestVm();
            var g1 = new FuVm();
            g1.Items.Add(new FuItem("dispose-boom-1"));
            vm.Groups.Add(g1);
            var g2 = new FuVm();
            g2.Items.Add(new FuItem("dispose-boom-2"));
            vm.Groups.Add(g2);

            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.AwbFuNestedTeardown;
            control.Activate();
            assert.Equal(Count(element, "[data-test=nt-group]"), 2, "mounted: 2 groups");

            int disposed = RazorProbeControl.DisposedCount;
            assert.Equal(Catch(delegate { vm.Groups.Remove(g1); }), "dispose boom: dispose-boom-1",
                "Remove rethrows the nested row's Dispose error");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 3,
                "row removal: the row's own control and both nested lists' controls are disposed");
            assert.Equal(Count(element, "[data-test=nt-group]"), 1, "the group left");

            disposed = RazorProbeControl.DisposedCount;
            assert.Equal(Catch(delegate { control.Dispose(); }), "dispose boom: dispose-boom-2",
                "host Dispose rethrows the nested row's Dispose error");
            assert.Equal(RazorProbeControl.DisposedCount, disposed + 4,
                "host Dispose: the host's control, the row's control and both nested lists' controls");
        }

        // PR 149 review P2: an inner-loop row binding a root property listens once; a change
        // of that property marks the rows dirty without unhooking and re-hooking every row.
        [Test]
        public static void TestFuRootPropertyChangeDoesNotResubscribeInnerRows(Assert assert)
        {
            var vm = new FuNestVm();
            for (int g = 0; g < 2; g++)
            {
                var group = new FuVm();
                for (int t = 0; t < 3; t++) group.Items.Add(new FuItem("t" + t));
                vm.Groups.Add(group);
            }

            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuNestChurn);
            assert.Equal(Count(root, "[data-test=nc-row]"), 6, "mounted: 6 inner rows");

            int adds = vm.AddCalls;
            int removes = vm.RemoveCalls;
            vm.Title = "t2";
            assert.Equal(RowTexts(root, "b"), "t2|t2|t2|t2|t2|t2", "every row shows the new title");
            assert.Equal(vm.AddCalls - adds, 0, "a title change adds no listener on the model");
            assert.Equal(vm.RemoveCalls - removes, 0, "a title change removes no listener on the model");

            // A partial chain (Sub null at mount) still re-hooks when the middle object arrives.
            var sub = new FuItem("s1");
            vm.Sub = sub;
            assert.Equal(RowTexts(root, "i"), "s1|s1|s1|s1|s1|s1", "setting Sub shows its name in every row");
            adds = vm.AddCalls;
            removes = vm.RemoveCalls;
            int subListeners = sub.ListenerCount;
            sub.Name = "s2";
            assert.Equal(RowTexts(root, "i"), "s2|s2|s2|s2|s2|s2", "a Sub.Name change reaches every row");
            assert.Equal(vm.AddCalls - adds + vm.RemoveCalls - removes, 0, "a leaf change re-hooks nothing on the model");
            assert.Equal(sub.ListenerCount, subListeners, "a leaf change keeps the listeners on Sub");
        }

        // The text of the given child of every inner row, joined by "|".
        private static string RowTexts(Element root, string child)
        {
            var rows = root.QuerySelectorAll("[data-test=nc-row]");
            string texts = "";
            for (int k = 0; k < rows.Length; k++)
                texts = texts + (k == 0 ? "" : "|") + rows[k].QuerySelector(child).TextContent;
            return texts;
        }
    }
}
