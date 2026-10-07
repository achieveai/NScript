namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Helpers.BindingGraph;

    /// <summary>
    /// Browser tests for GraphFlushCoordinator.BatchingEnabled: property
    /// changes only mark graph nodes dirty and one depth-ordered flush runs
    /// on the next microtask (or macrotask) boundary. Every test restores
    /// synchronous mode in finally so the rest of the suite is unaffected.
    /// </summary>
    [TestFixture]
    public class RazorBatchedFlushTests
    {
        private static TestWindowTimer EnableBatching(GraphFlushMode mode)
        {
            var timer = new TestWindowTimer(true);
            TaskScheduler.Instance = new TaskScheduler(timer, 10, 10);
            GraphFlushCoordinator.Reset();
            GraphFlushCoordinator.BatchingEnabled = true;
            GraphFlushCoordinator.Mode = mode;
            return timer;
        }

        private static void DisableBatching()
        {
            GraphFlushCoordinator.BatchingEnabled = false;
            GraphFlushCoordinator.Mode = GraphFlushMode.Microtask;
            GraphFlushCoordinator.Reset();
            RazorProbeControl.TextChangedForTest = null;
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestBatchedChangesCoalesceIntoOneFlushWithFinalValues(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var vm = new RazorTestVM();
                vm.Name = "Step0";
                vm.Count = 0;
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
                control.Activate();

                var nameSpan = element.QuerySelector("[data-test] .name span");
                var countSpan = element.QuerySelector("[data-test] .count span");
                assert.Equal("Step0", nameSpan.TextContent, "Initial activation stays synchronous");

                // x -> 0 -> y in one task: the DOM must not see the intermediate values.
                vm.Name = "Step1";
                vm.Name = "";
                vm.Name = "Step2";
                vm.Count = 10;
                vm.Count = 0;
                vm.Count = 20;

                assert.Equal("Step0", nameSpan.TextContent, "Name must not flush before the boundary");
                assert.Equal("0", countSpan.TextContent, "Count must not flush before the boundary");
                assert.Equal(1, timer.PendingMicrotaskCount, "Six changes schedule exactly one microtask");
                assert.Equal(1, GraphFlushCoordinator.PendingCount, "One graph is pending");

                timer.FlushMicrotasks();

                assert.Equal("Step2", nameSpan.TextContent, "Flush writes the final Name");
                assert.Equal("20", countSpan.TextContent, "Flush writes the final Count");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "Nothing pending after the flush");

                // The graph can be scheduled again after a flush.
                vm.Name = "Step3";
                assert.Equal(1, timer.PendingMicrotaskCount, "A later change schedules a new microtask");
                timer.FlushMicrotasks();
                assert.Equal("Step3", nameSpan.TextContent, "Second flush applies the next change");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestBatchedGateFlipFlopDoesNotSwapDom(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var vm = new RazorTestVM();
                vm.IsActive = true;
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorIfOnly;
                control.Activate();

                var before = element.QuerySelector("[data-test] .active-content");
                assert.NotEqual(null, before, "Open gate renders its branch");

                vm.IsActive = false;
                vm.IsActive = true;
                timer.FlushMicrotasks();

                var after = element.QuerySelector("[data-test] .active-content");
                assert.IsTrue(before == after, "true -> false -> true in one task must keep the same DOM node");

                vm.IsActive = false;
                timer.FlushMicrotasks();
                assert.Equal(null, element.QuerySelector("[data-test] .active-content"),
                    "A settled false still closes the gate");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestBatchedSubControlReceivesOneWriteWithFinalValue(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var vm = new RazorTestVM { Title = "Before" };
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
                control.Activate();

                var child = RazorProbeControl.LastCreated;
                assert.NotEqual(null, child, "The bound child should exist");
                if (child == null) return;

                int writes = 0;
                child.AddPropertyChangedListener("Text", delegate(INotifyPropertyChanged sender, string property)
                {
                    writes++;
                });

                vm.Title = "A";
                vm.Title = "B";
                vm.Title = "C";
                assert.Equal(0, writes, "The child must not be written before the boundary");
                assert.Equal("Before", child.Text, "Child keeps its value until the flush");

                timer.FlushMicrotasks();

                assert.Equal(1, writes, "Three parent changes produce one child write");
                assert.Equal("C", child.Text, "The child receives the final value");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestBatchedParentGraphFlushesBeforeNestedChildGraph(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var childA = new RazorItemVM { Name = "A1" };
                var childB = new RazorItemVM { Name = "B1" };
                var vm = new RazorTestVM { Title = "T", Child = childA };
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorBatchNested;
                control.Activate();

                var innerProbe = RazorProbeControl.LastCreated;
                assert.NotEqual(null, innerProbe, "The nested skin should create the inner probe");
                if (innerProbe == null) return;
                assert.Equal("A1", innerProbe.Text, "Initial nested value");
                assert.Equal(2, innerProbe.BindingDepth, "Inner probe sits two skins below the root");

                var writes = new NativeArray<string>(0);
                RazorProbeControl.TextChangedForTest = delegate(RazorProbeControl probe)
                {
                    writes.Push(probe.Text);
                };

                // Dirty the nested graph (depth 1) first, then the root (depth 0).
                childA.Name = "A2";
                vm.Child = childB;
                assert.Equal(2, GraphFlushCoordinator.PendingCount, "Both graphs are pending");

                timer.FlushMicrotasks();

                // Root flushed first and swapped the nested DataContext, so the
                // stale "A2" write never happened.
                assert.Equal(1, writes.Length, "Only one write reaches the inner probe");
                assert.Equal("B1", innerProbe.Text, "The inner probe shows the new child");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "Nothing left pending");

                // The nested graph is subscribed to the new child.
                childB.Name = "B2";
                timer.FlushMicrotasks();
                assert.Equal("B2", innerProbe.Text, "Nested graph follows the new child");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestBatchedChildWriteToParentDuringFlushSettlesInSameBoundary(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var vm = new RazorTestVM { Title = "Before" };
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
                control.Activate();

                var child = RazorProbeControl.LastCreated;
                assert.NotEqual(null, child, "The bound child should exist");
                if (child == null) return;

                child.AddPropertyChangedListener("Text", delegate(INotifyPropertyChanged sender, string property)
                {
                    if (child.Text == "First")
                        vm.Title = "Second";
                });

                vm.Title = "First";
                timer.FlushMicrotasks();

                assert.Equal("Second", vm.Title, "The child changed the parent model");
                assert.Equal("Second", child.Text, "The parent re-flushed inside the same boundary");
                assert.Equal("Second", child.Element.GetAttribute("data-bound-text"), "Host shows the final value");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "Nothing left pending");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestBatchedFlushSkipsDisposedAndDeactivatedGraphs(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var doc = Window.Instance.Document;

                var disposedElement = doc.CreateElement("div");
                var disposed = new UISkinableElement(disposedElement);
                var disposedVm = new RazorTestVM { Name = "D0" };
                disposed.DataContext = disposedVm;
                disposed.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
                disposed.Activate();

                var inactiveElement = doc.CreateElement("div");
                var inactive = new UISkinableElement(inactiveElement);
                var inactiveVm = new RazorTestVM { Name = "I0" };
                inactive.DataContext = inactiveVm;
                inactive.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
                inactive.Activate();
                var inactiveSpan = inactiveElement.QuerySelector("[data-test] .name span");

                disposedVm.Name = "D1";
                inactiveVm.Name = "I1";
                disposed.Dispose();
                inactive.Deactivate();

                timer.FlushMicrotasks();

                assert.Equal("I0", inactiveSpan.TextContent, "A deactivated graph must not write to the DOM");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "Pending entries were drained");

                inactive.Activate();
                inactiveSpan = inactiveElement.QuerySelector("[data-test] .name span");
                assert.Equal("I1", inactiveSpan.TextContent, "Reactivation pushes the latest value");
            }
            finally
            {
                DisableBatching();
            }
        }

        /// <summary>
        /// A collection item removed, replaced, or cleared while its graph has a
        /// pending batched flush must be retired: the drain may not write to its
        /// detached DOM or create sub-controls for it.
        /// </summary>
        [Test]
        public static void TestBatchedFlushSkipsRemovedCollectionItems(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var doc = Window.Instance.Document;
                var element = doc.CreateElement("div");
                var control = new UISkinableElement(element);
                var a = new RazorItemVM { Name = "A0", Status = "SA0" };
                var b = new RazorItemVM { Name = "B0", Status = "SB0" };
                var c = new RazorItemVM { Name = "C0", Status = "SC0" };
                var vm = new RazorTestVM { Items = new ObservableCollection<RazorItemVM>() };
                vm.Items.Add(a);
                vm.Items.Add(b);
                vm.Items.Add(c);
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorSubControlForeach;
                control.Activate();
                timer.FlushMicrotasks();

                var spans = element.QuerySelectorAll(".item-status");
                assert.Equal(3, spans.Length, "three items rendered");
                var spanA = spans[0];
                var spanB = spans[1];
                var spanC = spans[2];

                // Remove: dirty A, then take it out before the flush.
                a.Status = "SA1";
                a.Name = "A1";
                vm.Items.RemoveAt(0);
                int created = RazorProbeControl.CreatedCount;
                timer.FlushMicrotasks();
                assert.Equal("SA0", spanA.TextContent, "Removed item's detached span must not be written");
                assert.Equal(created, RazorProbeControl.CreatedCount, "Removed item must not create sub-controls");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "Drained after remove");

                // Replace: dirty B, then swap it for a new item before the flush.
                b.Status = "SB1";
                var b2 = new RazorItemVM { Name = "B2", Status = "SB2" };
                vm.Items[0] = b2;
                timer.FlushMicrotasks();
                assert.Equal("SB0", spanB.TextContent, "Replaced item's detached span must not be written");
                var replacement = element.QuerySelector(".item-status");
                assert.Equal("SB2", replacement.TextContent, "Replacement item renders");

                // Reset: dirty C, then clear the collection before the flush.
                c.Status = "SC1";
                vm.Items.Clear();
                created = RazorProbeControl.CreatedCount;
                timer.FlushMicrotasks();
                assert.Equal("SC0", spanC.TextContent, "Cleared item's detached span must not be written");
                assert.Equal(created, RazorProbeControl.CreatedCount, "Cleared items must not create sub-controls");
                assert.Equal(0, element.QuerySelectorAll(".item-status").Length, "Nothing rendered after clear");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "Drained after clear");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestBatchedFlushContinuesAfterOneGraphThrows(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var doc = Window.Instance.Document;

                var faultyElement = doc.CreateElement("div");
                var faulty = new UISkinableElement(faultyElement);
                var faultyVm = new RazorTestVM { Title = "F0" };
                faulty.DataContext = faultyVm;
                faulty.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
                faulty.Activate();

                var healthyElement = doc.CreateElement("div");
                var healthy = new UISkinableElement(healthyElement);
                var healthyVm = new RazorTestVM { Name = "H0" };
                healthy.DataContext = healthyVm;
                healthy.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
                healthy.Activate();
                var healthySpan = healthyElement.QuerySelector("[data-test] .name span");

                RazorProbeControl.TextChangedForTest = delegate(RazorProbeControl probe)
                {
                    throw new Exception("probe setter boom");
                };

                // Faulty graph is scheduled first at the same depth.
                faultyVm.Title = "F1";
                healthyVm.Name = "H1";

                bool threw = false;
                try
                {
                    timer.FlushMicrotasks();
                }
                catch
                {
                    threw = true;
                }

                assert.IsTrue(!threw, "A failing graph must not throw out of the flush");
                assert.Equal("H1", healthySpan.TextContent, "The healthy graph still flushed");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "The coordinator recovered");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestFlushNowAppliesPendingChangesSynchronously(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Microtask);
            try
            {
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var vm = new RazorTestVM { Name = "N0" };
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
                control.Activate();
                var nameSpan = element.QuerySelector("[data-test] .name span");

                vm.Name = "N1";
                GraphFlushCoordinator.FlushNow();
                assert.Equal("N1", nameSpan.TextContent, "FlushNow writes the DOM immediately");
                assert.Equal(0, GraphFlushCoordinator.PendingCount, "FlushNow drains the queue");

                // The already-scheduled microtask finds nothing to do.
                timer.FlushMicrotasks();
                assert.Equal("N1", nameSpan.TextContent, "The empty scheduled flush is harmless");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestMacrotaskModeSchedulesThroughTaskScheduler(Assert assert)
        {
            var timer = EnableBatching(GraphFlushMode.Macrotask);
            try
            {
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var vm = new RazorTestVM { Name = "M0" };
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
                control.Activate();
                var nameSpan = element.QuerySelector("[data-test] .name span");

                vm.Name = "M1";
                vm.Name = "M2";
                assert.Equal(0, timer.PendingMicrotaskCount, "Macrotask mode does not use the microtask queue");
                assert.Equal(1, timer.PendingImmediateCount, "Macrotask mode schedules one immediate");
                assert.Equal("M0", nameSpan.TextContent, "DOM is stale until the immediate runs");

                timer.FlushImmediates();
                assert.Equal("M2", nameSpan.TextContent, "The immediate flushes the final value");
            }
            finally
            {
                DisableBatching();
            }
        }

        [Test]
        public static void TestBatchingOffKeepsSynchronousFlush(Assert assert)
        {
            var timer = new TestWindowTimer(true);
            TaskScheduler.Instance = new TaskScheduler(timer, 10, 10);
            try
            {
                GraphFlushCoordinator.BatchingEnabled = false;
                var element = Window.Instance.Document.CreateElement("div");
                var control = new UISkinableElement(element);
                var vm = new RazorTestVM { Name = "S0" };
                control.DataContext = vm;
                control.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
                control.Activate();
                var nameSpan = element.QuerySelector("[data-test] .name span");

                vm.Name = "S1";
                assert.Equal("S1", nameSpan.TextContent, "Flag off: the DOM updates before the setter returns");
                assert.Equal(0, timer.PendingMicrotaskCount, "Flag off: nothing is scheduled");
            }
            finally
            {
                DisableBatching();
            }
        }
    }
}
