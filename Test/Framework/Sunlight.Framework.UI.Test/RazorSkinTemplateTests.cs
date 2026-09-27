namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework.Observables;
    using RazorCrossAssembly.Models;
    using RazorCrossAssembly.Controls;
    using RazorCrossAssembly.Views;

    /// <summary>
    /// Browser-based tests for Razor skin templates.
    /// These tests verify that .skin.cshtml templates compiled through the full
    /// NScript pipeline produce correct runtime behavior in the browser.
    /// </summary>
    [TestFixture]
    public class RazorSkinTemplateTests
    {
        [Test]
        public static void TestShortSkinNameSelectsCompatibleTemplate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new RazorLiteralProbeControl(element);
            control.DataContext = new object();
            control.Skin = RazorLiteralProbeControl.ShortSkin;
            control.Activate();

            assert.NotEqual(null, element.QuerySelector(".literal-probe"),
                "A short [Skin] name should find its compatible template");
        }

        [Test]
        public static void TestAmbiguousShortSkinNameThrowsAtRuntime(Assert assert)
        {
            var threw = false;
            try
            {
                var skin = RazorLiteralProbeControl.AmbiguousSkin;
            }
            catch
            {
                threw = true;
            }

            assert.IsTrue(threw, "Two compatible short-name templates must throw");
        }

        [Test]
        public static void TestSubControlStaticAttributesUseTargetTypes(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = new RazorTestVM();
            control.Skin = RazorSkinTemplatesClass.RazorSubControlLiterals;
            control.Activate();

            var child = RazorLiteralProbeControl.LastCreated;
            assert.NotEqual(null, child, "The literal probe should be constructed");
            if (child == null) return;
            assert.Equal("plain text", child.Text, "String attributes remain text");
            assert.Equal(false, child.IsOn, "Boolean attributes use C# values");
            assert.Equal(5, child.Count, "Numeric attributes use C# values");
            assert.Equal(RazorProbeMode.Fast, child.Mode, "Enum attributes use C# values");
        }

        [Test]
        public static void TestTopLevelSubControlBindsPropertyAndRendersSkin(Assert assert)
        {
            RazorProbeControl.LastTextAtActivate = null;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            vm.Title = "First";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
            control.Activate();

            var child = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, child, "Top-level child host should be present");
            if (child == null) return;
            assert.Equal("First", child.GetAttribute("data-bound-text"),
                "The child receives the parent property before activation");
            assert.Equal("First", RazorProbeControl.LastTextAtActivate,
                "The child must receive bound properties before it activates");
            assert.Equal(1, child.QuerySelectorAll(".probe-skin").Length,
                "The child's own skin should render on its host");

            vm.Title = "Second";
            assert.Equal("Second", child.GetAttribute("data-bound-text"),
                "The child property should update with its parent ViewModel");
        }

        [Test]
        public static void TestShortSkinNameOnNonControlHolderRenders(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM { Title = "Short name" };
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevelShort;
            control.Activate();

            var child = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, child, "A short skin name on the template catalog should resolve");
            if (child != null)
                assert.Equal("Short name", child.GetAttribute("data-bound-text"),
                    "The resolved skin should bind its child");
        }

        [Test]
        public static void TestUnrelatedParentChangePreservesChildOwnedProperty(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            vm.Title = "From parent";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
            control.Activate();

            var child = RazorProbeControl.LastCreated;
            assert.NotEqual(null, child, "The bound child should exist");
            if (child == null) return;
            child.Text = "Changed by child";
            vm.Count = 1;

            assert.Equal("Changed by child", child.Text,
                "An unrelated parent change must not reapply an unchanged child binding");
            vm.Title = "New parent value";
            assert.Equal("New parent value", child.Text,
                "A changed source must still update the child");
        }

        [Test]
        public static void TestChildModelWriteDuringParentFlushIsAppliedImmediately(Assert assert)
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
            assert.Equal("Second", vm.Title, "The child should change the parent model");
            assert.Equal("Second", child.Text,
                "The parent must apply the child's model change before the flush returns");
            assert.Equal("Second", child.Element.GetAttribute("data-bound-text"),
                "The child host should show the final value");
        }

        [Test]
        public static void TestChildModelWriteDuringInitialActivationIsApplied(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM { Title = "First" };
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
            RazorProbeControl.TextChangedForTest = delegate(RazorProbeControl child)
            {
                if (child.Text == "First")
                    vm.Title = "Second";
            };

            try
            {
                control.Activate();
            }
            finally
            {
                RazorProbeControl.TextChangedForTest = null;
            }

            var created = RazorProbeControl.LastCreated;
            assert.NotEqual(null, created, "Initial activation should create the child");
            if (created == null) return;
            assert.Equal("Second", vm.Title, "The child should change the parent model");
            assert.Equal("Second", created.Text,
                "Initial activation should reconcile a model change made by the child");
        }

        [Test]
        public static void TestSubControlDataContextOverrideRebinds(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var first = new RazorItemVM();
            first.Name = "First child";
            vm.Child = first;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlDataContext;
            control.Activate();

            var name = element.QuerySelector(".probe-context-skin");
            assert.NotEqual(null, name, "The child skin should render");
            if (name == null) return;
            assert.Equal("First child", name.TextContent,
                "The child skin should use the explicit DataContext");

            var second = new RazorItemVM();
            second.Name = "Second child";
            vm.Child = second;
            name = element.QuerySelector(".probe-context-skin");
            assert.NotEqual(null, name, "The child skin should remain after replacing its ViewModel");
            if (name == null) return;
            assert.Equal("Second child", name.TextContent,
                "Replacing the child ViewModel should rebind the child skin");

            first.Name = "Old child changed";
            assert.Equal("Second child", name.TextContent,
                "The old child ViewModel should no longer affect the skin");
        }

        [Test]
        public static void TestTopLevelSubControlInheritsDataContext(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorItemVM();
            vm.Name = "Parent model";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlDefaultContext;
            control.Activate();

            var name = element.QuerySelector(".probe-context-skin");
            assert.NotEqual(null, name, "Default child skin should render");
            if (name == null) return;
            assert.Equal("Parent model", name.TextContent,
                "An unbound top-level child should inherit its parent's DataContext");

            vm.Name = "Parent updated";
            assert.Equal("Parent updated", name.TextContent,
                "The inherited DataContext should remain reactive");
        }

        [Test]
        public static void TestTopLevelSubControlsFollowParentLifecycle(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DeactivatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = new RazorTestVM();
            control.Skin = RazorSkinTemplatesClass.RazorSubControlLifecycle;
            control.Activate();
            assert.Equal(3, RazorProbeControl.CreatedCount,
                "The parent should create all three children");

            control.Deactivate();
            assert.Equal(3, RazorProbeControl.DeactivatedCount,
                "Parent deactivation should deactivate each child once");

            control.Dispose();
            assert.Equal(3, RazorProbeControl.DisposedCount,
                "Parent disposal should dispose every child");
        }

        [Test]
        public static void TestForeachSubControlBindsItemAndDisposesRemovedChild(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var first = new RazorItemVM();
            first.Name = "First";
            var second = new RazorItemVM();
            second.Name = "Second";
            items.Add(first);
            items.Add(second);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlForeach;
            control.Activate();

            var children = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(2, children.Length, "The loop should create one control per item");
            if (children.Length != 2) return;
            assert.Equal("First", children[0].GetAttribute("data-bound-text"),
                "The first child property should read the first item");
            assert.Equal("Second", children[1].GetAttribute("data-bound-text"),
                "The second child property should read the second item");

            second.Name = "Second updated";
            assert.Equal("Second updated", children[1].GetAttribute("data-bound-text"),
                "A loop item change should update its child property");

            items.RemoveAt(0);
            assert.Equal(1, RazorProbeControl.DisposedCount,
                "Removing an item should dispose that item's child");
            children = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(1, children.Length, "Only the remaining child should stay in the DOM");
            if (children.Length != 1) return;
            assert.Equal("Second updated", children[0].GetAttribute("data-bound-text"),
                "The remaining child should keep its item binding");
        }

        [Test]
        public static void TestForeachSubControlReactivationDoesNotDuplicateHosts(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "First" });
            items.Add(new RazorItemVM { Name = "Second" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlForeach;
            control.Activate();
            assert.Equal(2, element.QuerySelectorAll("[data-ns-subctl]").Length,
                "Initial activation should mount one child per item");

            var originalHosts = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(2, RazorProbeControl.CreatedCount,
                "Initial activation should construct two children");

            control.Deactivate();
            assert.Equal(0, RazorProbeControl.DisposedCount,
                "Deactivation should preserve loop children");
            items[0].Name = "First updated while inactive";
            control.Activate();
            var reactivatedHosts = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(2, reactivatedHosts.Length,
                "Reactivation must leave exactly one child host per item");
            assert.Equal(2, RazorProbeControl.CreatedCount,
                "Unchanged items should retain their control instances");
            assert.Equal(0, RazorProbeControl.DisposedCount,
                "Reactivation should not dispose unchanged children");
            if (reactivatedHosts.Length == 2)
                assert.Equal("First updated while inactive",
                    reactivatedHosts[0].GetAttribute("data-bound-text"),
                    "The preserved child should show item changes made while inactive");
            if (reactivatedHosts.Length == 2)
            {
                assert.IsTrue(originalHosts[0] == reactivatedHosts[0],
                    "The first host should survive reactivation");
                assert.IsTrue(originalHosts[1] == reactivatedHosts[1],
                    "The second host should survive reactivation");
            }

            items[1].Name = "Second updated after reactivation";
            if (reactivatedHosts.Length == 2)
                assert.Equal("Second updated after reactivation",
                    reactivatedHosts[1].GetAttribute("data-bound-text"),
                    "An existing item should keep its live binding after reactivation");

            items.Add(new RazorItemVM { Name = "Third" });
            assert.Equal(3, element.QuerySelectorAll("[data-ns-subctl]").Length,
                "A collection Add after reactivation should create one host");
            assert.Equal(3, RazorProbeControl.CreatedCount,
                "A collection Add after reactivation should create one control");

            control.Deactivate();
            var replacement = new ObservableCollection<RazorItemVM>();
            replacement.Add(new RazorItemVM { Name = "Replacement" });
            vm.Items = replacement;
            control.Activate();
            assert.Equal(3, RazorProbeControl.DisposedCount,
                "A changed collection should dispose the old children on reactivation");
            assert.Equal(4, RazorProbeControl.CreatedCount,
                "A changed collection should create its new child");
            assert.Equal(1, element.QuerySelectorAll("[data-ns-subctl]").Length,
                "Only the replacement item should remain");

            control.Dispose();
            assert.Equal(RazorProbeControl.CreatedCount, RazorProbeControl.DisposedCount,
                "All children created across activations should be disposed");
        }

        [Test]
        public static void TestForeachBindingAfterSubControlTargetsCorrectSpan(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var first = new RazorItemVM { Name = "First", Status = "Ready" };
            items.Add(first);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlForeach;
            control.Activate();

            var status = element.QuerySelector(".item-status");
            assert.NotEqual(null, status, "The span after the control should render");
            if (status == null) return;
            assert.Equal("Ready", status.TextContent,
                "The initial binding after a child control should target its span");
            first.Status = "Working";
            assert.Equal("Working", status.TextContent,
                "A changed item should update the span after its child control");

            var second = new RazorItemVM { Name = "Second", Status = "Queued" };
            items.Add(second);
            var statuses = element.QuerySelectorAll(".item-status");
            assert.Equal(2, statuses.Length, "Adding an item should render two status spans");
            if (statuses.Length != 2) return;
            second.Status = "Running";
            assert.Equal("Running", statuses[1].TextContent,
                "The new item's binding should update its own span");
            assert.Equal("Working", statuses[0].TextContent,
                "The first item's span should remain unchanged");
        }

        [Test]
        public static void TestReplacingWholeCollectionDisposesOldSubControls(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var oldItems = new ObservableCollection<RazorItemVM>();
            var oldItem = new RazorItemVM();
            oldItem.Name = "Old";
            oldItems.Add(oldItem);
            vm.Items = oldItems;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlForeach;
            control.Activate();

            var oldHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, oldHost, "The first collection should mount a child");
            if (oldHost == null) return;

            var newItems = new ObservableCollection<RazorItemVM>();
            var newItem = new RazorItemVM();
            newItem.Name = "New";
            newItems.Add(newItem);
            vm.Items = newItems;

            var newHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, newHost, "The replacement collection should mount a child");
            if (newHost == null) return;
            assert.NotEqual(oldHost, newHost, "The old child host should be replaced");
            assert.Equal(1, RazorProbeControl.DisposedCount,
                "Replacing the collection should dispose its old child");
            assert.Equal("New", newHost.GetAttribute("data-bound-text"),
                "The new child should bind the new item");

            oldItem.Name = "Stale";
            assert.Equal("Old", oldHost.GetAttribute("data-bound-text"),
                "The old child must not respond to stale item updates");
            newItem.Name = "Current";
            assert.Equal("Current", newHost.GetAttribute("data-bound-text"),
                "The new child should remain reactive");
            oldItems.Add(new RazorItemVM());
            assert.Equal(1, element.QuerySelectorAll("[data-ns-subctl]").Length,
                "The old collection listener should be detached");
        }

        [Test]
        public static void TestForeachBareSubControlUsesItemAsDefaultDataContext(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var first = new RazorItemVM();
            first.Name = "First";
            var second = new RazorItemVM();
            second.Name = "Second";
            items.Add(first);
            items.Add(second);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlForeachDefaultContext;
            control.Activate();

            var names = element.QuerySelectorAll(".probe-context-skin");
            assert.Equal(2, names.Length, "The loop should render both child skins");
            if (names.Length != 2) return;
            assert.Equal("First", names[0].TextContent,
                "The first bare child should read the first item as DataContext");
            assert.Equal("Second", names[1].TextContent,
                "The second bare child should read the second item as DataContext");

            second.Name = "Second updated";
            assert.Equal("Second updated", names[1].TextContent,
                "A bare child's inherited item DataContext should remain reactive");
        }

        [Test]
        public static void TestConditionalSubControlRecreatesWithoutOldBinding(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            vm.ShowDetails = true;
            vm.Title = "Visible";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlConditional;
            control.Activate();

            var firstHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, firstHost, "The true branch should create a child");
            if (firstHost == null) return;
            assert.Equal("Visible", firstHost.GetAttribute("data-bound-text"),
                "The child should receive its initial property");

            vm.ShowDetails = false;
            assert.Equal(0, element.QuerySelectorAll("[data-ns-subctl]").Length,
                "The false branch should remove the child host");
            assert.Equal(1, RazorProbeControl.DisposedCount,
                "The hidden child should be disposed");

            vm.Title = "Reopened";
            assert.Equal("Visible", firstHost.GetAttribute("data-bound-text"),
                "The removed child must stop receiving parent changes");
            firstHost.Click();
            assert.Equal(0, vm.ClickCount,
                "The removed child must not retain its click handler");
            vm.ShowDetails = true;
            var secondHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, secondHost, "Showing the branch again should create a child");
            if (secondHost == null) return;
            assert.Equal(2, RazorProbeControl.CreatedCount,
                "Showing the branch again should create a new instance");
            assert.Equal("Reopened", secondHost.GetAttribute("data-bound-text"),
                "The new child should receive the latest property");
            secondHost.Click();
            assert.Equal(1, vm.ClickCount,
                "The replacement child should handle clicks exactly once");
        }

        [Test]
        public static void TestIfBindingAfterSubControlTargetsCorrectSpanAcrossToggle(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            vm.ShowDetails = true;
            vm.Title = "Child";
            vm.Count = 1;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlConditional;
            control.Activate();

            var count = element.QuerySelector(".branch-count");
            assert.NotEqual(null, count, "The span after the conditional child should render");
            if (count == null) return;
            assert.Equal("1", count.TextContent, "The initial count should target its span");
            vm.Count = 2;
            assert.Equal("2", count.TextContent, "The open branch should update its span");

            vm.ShowDetails = false;
            assert.Equal(0, element.QuerySelectorAll(".branch-count").Length,
                "Closing the branch should remove its span");
            vm.Count = 3;
            vm.ShowDetails = true;
            count = element.QuerySelector(".branch-count");
            assert.NotEqual(null, count, "Reopening should recreate the count span");
            if (count == null) return;
            assert.Equal("3", count.TextContent, "The reopened span should read the latest value");
            vm.Count = 4;
            assert.Equal("4", count.TextContent, "The reopened span should stay reactive");
        }

        [Test]
        public static void TestSubControlDelegatePropertyHandlesClick(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlDelegate;
            control.Activate();

            var host = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, host, "The button control should render");
            if (host == null) return;
            host.Click();
            assert.Equal(1, vm.ClickCount,
                "A delegate property on the child tag should call the parent method");
        }

        [Test]
        public static void TestSubControlDelegateCapturesForeachItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var first = new RazorItemVM();
            first.Name = "First";
            var second = new RazorItemVM();
            second.Name = "Second";
            items.Add(first);
            items.Add(second);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlDelegateForeach;
            control.Activate();

            var hosts = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(2, hosts.Length, "The loop should render both control hosts");
            if (hosts.Length != 2) return;
            hosts[1].Click();
            assert.Equal("Second", vm.PickedName,
                "The second child delegate should capture the second item");
            hosts[0].Click();
            assert.Equal("First", vm.PickedName,
                "The first child delegate should capture the first item");
        }

        [Test]
        public static void TestSubControlDelegateCallsForeachItemMethod(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var first = new RazorItemVM { Name = "First" };
            var second = new RazorItemVM { Name = "Second" };
            items.Add(first);
            items.Add(second);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlItemDelegate;
            control.Activate();

            var hosts = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(2, hosts.Length, "The loop should render both controls");
            if (hosts.Length != 2) return;
            hosts[1].Click();
            assert.Equal(0, first.SelectCount, "The other item must stay unchanged");
            assert.Equal(1, second.SelectCount, "The item delegate method must be emitted");
        }

        [Test]
        public static void TestInheritedControlHandlerUsesResolvedMethod(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new RazorInheritedHandlerControl(element);
            RazorHandlerBase.ClickCount = 0;
            control.DataContext = new object();
            control.Skin = RazorInheritedHandlerControl.DefaultSkin;
            control.Activate();

            var button = element.QuerySelector(".inherited-handler");
            assert.NotEqual(null, button, "The inherited handler template should render");
            if (button == null) return;
            button.Click();
            assert.Equal(1, RazorHandlerBase.ClickCount,
                "A handler inherited from the control base class should run once");
        }

        [Test]
        public static void TestSubControlLowercaseDomEventHandlesClick(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlDomEvent;
            control.Activate();

            var host = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, host, "The DOM event control host should render");
            if (host == null) return;
            host.Click();
            assert.IsTrue(vm.ClickFired,
                "A lowercase onclick binding should attach to the child host");
        }

        [Test]
        public static void TestSubControlDomEventDoesNotMultiplyOnReactivation(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlDomEvent;
            control.Activate();

            var host = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, host, "The event host should exist");
            if (host == null) return;
            host.Click();
            assert.Equal(1, vm.ClickCount, "The first activation should handle one click once");

            control.Deactivate();
            control.Activate();
            host.Click();
            assert.Equal(2, vm.ClickCount, "Reactivation must not attach a second click handler");

            control.Deactivate();
            control.Activate();
            host.Click();
            assert.Equal(3, vm.ClickCount, "Repeated reactivation must still handle each click once");
        }

        [Test]
        public static void TestSubControlExplicitSkinAndDataContextCompose(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var child = new RazorItemVM();
            child.Name = "Composed child";
            vm.Child = child;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlComposition;
            control.Activate();

            var alternate = element.QuerySelector(".probe-alternate");
            assert.NotEqual(null, alternate, "The explicit Skin should replace the default child skin");
            if (alternate == null) return;
            assert.Equal("Composed child", alternate.TextContent,
                "The explicit Skin should bind against the explicit DataContext");
            assert.Equal(0, element.QuerySelectorAll(".probe-skin").Length,
                "The default child skin should not render alongside the explicit Skin");
        }

        [Test]
        public static void TestSubControlIdResolvesInApplySkinInternal(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new RazorPartProbeParent(element);
            control.DataContext = new RazorTestVM();
            control.Skin = RazorSkinTemplatesClass.RazorSubControlPart;
            control.Activate();

            assert.NotEqual(null, control.ProbePart,
                "ApplySkinInternal should find a named Razor sub-control by id");
            if (control.ProbePart == null) return;
            assert.Equal(element.QuerySelector("[data-ns-subctl]"), control.ProbePart.Element,
                "The named part should be the child bound to its host element");
        }

        [Test]
        public static void TestCrossAssemblyControlSkinAndModel(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new CrossAssemblyParentModel();
            var child = new CrossAssemblyChildModel();
            vm.Title = "From parent";
            child.Name = "From child";
            vm.Child = child;
            control.DataContext = vm;
            control.Skin = CrossAssemblyViews.Parent;
            assert.NotEqual(null, control.Skin,
                "The parent skin from another library should compile");
            control.Activate();

            var host = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, host,
                "The child control class from a third library should instantiate");
            if (host == null) return;
            assert.Equal("From parent", host.GetAttribute("data-cross-text"),
                "The parent skin should bind a property on the external control");
            var label = host.QuerySelector(".cross-assembly-label");
            assert.NotEqual(null, label,
                "The external control's embedded skin should render");
            if (label == null) return;
            assert.Equal("From child", label.TextContent,
                "The child skin should use the model from the model library");

            vm.Title = "Updated parent";
            child.Name = "Updated child";
            assert.Equal("Updated parent", host.GetAttribute("data-cross-text"),
                "The external control property should update reactively");
            assert.Equal("Updated child", label.TextContent,
                "The external model should update the child skin reactively");
        }

        [Test]
        public static void TestSameNamedSkinsInDifferentAssembliesStayDistinct(Assert assert)
        {
            var controlsElement = Window.Instance.Document.CreateElement("div");
            var controlsHost = new UISkinableElement(controlsElement);
            controlsHost.DataContext = new object();
            controlsHost.Skin = CrossAssemblyLabel.SharedSkin;
            controlsHost.Activate();

            var viewsElement = Window.Instance.Document.CreateElement("div");
            var viewsHost = new UISkinableElement(viewsElement);
            viewsHost.DataContext = new object();
            viewsHost.Skin = CrossAssemblyViews.SharedSkin;
            viewsHost.Activate();

            var controlsSkin = controlsElement.QuerySelector(".shared-skin-controls");
            var viewsSkin = viewsElement.QuerySelector(".shared-skin-views");
            assert.NotEqual(null, controlsSkin,
                "Controls assembly SharedName skin should render its own content");
            assert.NotEqual(null, viewsSkin,
                "Views assembly SharedName skin should render its own content");
            assert.Equal(null, controlsElement.QuerySelector(".shared-skin-views"),
                "Controls assembly must not receive Views assembly content");
            assert.Equal(null, viewsElement.QuerySelector(".shared-skin-controls"),
                "Views assembly must not receive Controls assembly content");
        }

        [Test]
        public static void TestSubControlPropertyTwoWayBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new RazorPartProbeParent(element);
            var vm = new RazorTestVM();
            vm.Draft = "Initial draft";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTwoWay;
            control.Activate();

            var child = control.ProbePart as RazorValueProbeControl;
            assert.NotEqual(null, child,
                "The named child should be a value control");
            if (child == null) return;
            assert.Equal("Initial draft", child.Value,
                "The parent ViewModel should set the child property initially");

            vm.Draft = "Parent changed";
            assert.Equal("Parent changed", child.Value,
                "The parent ViewModel should continue to update the child property");

            child.Value = "Child changed";
            assert.Equal("Child changed", vm.Draft,
                "The child property change should update the parent ViewModel");
        }

        [Test]
        public static void TestTwoWaySubControlWriteBackWithNullDataContext(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new RazorPartProbeParent(element);
            control.InactiveIfNullContext = false;
            var vm = new RazorTestVM();
            vm.Draft = "Initial";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTwoWay;
            control.Activate();

            var child = control.ProbePart as RazorValueProbeControl;
            assert.NotEqual(null, child, "The two-way child should exist");
            if (child == null) return;
            control.DataContext = null;
            assert.IsTrue(control.IsActive,
                "The parent must remain active to exercise null-source write-back");
            child.Value = "Local after unbind";
            assert.Equal("Initial", vm.Draft,
                "Write-back must ignore a missing current source and leave the old model untouched");
            assert.Equal("Local after unbind", child.Value,
                "A child edit should remain usable while the parent DataContext is null");
        }

        [Test]
        public static void TestSubControlParentDataContextSwapDetachesOldModel(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var oldVm = new RazorTestVM();
            oldVm.Title = "Old parent";
            control.DataContext = oldVm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
            control.Activate();

            var child = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, child, "The bound child should render");
            if (child == null) return;
            assert.Equal("Old parent", child.GetAttribute("data-bound-text"),
                "The initial parent should bind the child");

            var newVm = new RazorTestVM();
            newVm.Title = "New parent";
            control.DataContext = newVm;
            assert.Equal("New parent", child.GetAttribute("data-bound-text"),
                "The child should bind the new parent DataContext");

            oldVm.Title = "Stale update";
            assert.Equal("New parent", child.GetAttribute("data-bound-text"),
                "The old parent must no longer update the child");

            newVm.Title = "Current update";
            assert.Equal("Current update", child.GetAttribute("data-bound-text"),
                "The new parent should remain reactive");
        }

        [Test]
        public static void TestDeactivatedParentDataContextAndQueuedUpdateKeepChildInactive(Assert assert)
        {
            var timer = new TestWindowTimer(true);
            TaskScheduler.Instance = new TaskScheduler(timer, 10, 10);
            var element = Window.Instance.Document.CreateElement("div");
            var control = new RazorPartProbeParent(element);
            var initialVm = new RazorTestVM();
            initialVm.Title = "Before";
            control.DataContext = initialVm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlDeactivation;
            control.Activate();

            var child = control.ProbePart as RazorProbeControl;
            assert.NotEqual(null, child, "The named bound child should render");
            if (child == null)
            {
                TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
                return;
            }
            assert.IsTrue(child.IsActive, "The child should start active");

            control.Deactivate();
            assert.IsTrue(!child.IsActive, "Parent deactivation should deactivate the child");

            var newVm = new RazorTestVM();
            newVm.Title = "New context";
            control.DataContext = newVm;
            assert.IsTrue(!child.IsActive,
                "Changing DataContext directly must not reactivate a deactivated child");

            TaskScheduler.Instance.EnqueueOnAnimationFrame(
                delegate { newVm.Title = "Queued update"; },
                "deactivated-subcontrol-update");
            timer.FlushAnimationFrames();
            assert.Equal("Queued update", newVm.Title,
                "The deferred ViewModel update should run before reactivation");
            assert.IsTrue(!child.IsActive,
                "Flushing queued work must not reactivate a deactivated child");

            control.Activate();
            assert.IsTrue(child.IsActive, "The child should reactivate with its parent");
            assert.Equal("Queued update", child.Text,
                "Reactivation should apply the latest parent value");
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestIfInForeachReplacementDisposesOldSubControl(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var oldItem = new RazorItemVM();
            oldItem.Name = "Old item";
            oldItem.IsComplete = true;
            items.Add(oldItem);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlIfInForeach;
            control.Activate();

            var oldHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, oldHost, "The old conditional child should render");
            if (oldHost == null) return;
            assert.Equal("Old item", oldHost.GetAttribute("data-bound-text"),
                "The old child should bind its item");

            var newItem = new RazorItemVM();
            newItem.Name = "New item";
            newItem.IsComplete = true;
            items[0] = newItem;

            var newHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, newHost, "The replacement item should mount a child");
            if (newHost == null) return;
            assert.NotEqual(oldHost, newHost,
                "The replacement item should have a new child host");
            assert.Equal(1, RazorProbeControl.DisposedCount,
                "Replacing the item should dispose the old conditional child");
            assert.Equal("New item", newHost.GetAttribute("data-bound-text"),
                "The new child should bind the replacement item");

            oldItem.Name = "Stale old item";
            assert.Equal("Old item", oldHost.GetAttribute("data-bound-text"),
                "The disposed child must not receive stale item updates");
            newItem.Name = "Updated new item";
            assert.Equal("Updated new item", newHost.GetAttribute("data-bound-text"),
                "The replacement child should remain reactive");
        }

        [Test]
        public static void TestOpenConditionalChildReactivationDoesNotDuplicateHost(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            RazorProbeControl.LastCreated = null;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            vm.ShowDetails = true;
            vm.Title = "Open branch";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlConditional;
            control.Activate();

            var firstChild = RazorProbeControl.LastCreated;
            assert.NotEqual(null, firstChild, "The open branch should create a child");
            if (firstChild == null) return;
            assert.Equal(1, element.QuerySelectorAll("[data-ns-subctl]").Length,
                "The open branch should have one child host");

            control.Deactivate();
            assert.IsTrue(!firstChild.IsActive,
                "Parent deactivation should deactivate the branch child");
            control.Activate();

            var hosts = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(1, hosts.Length,
                "Reactivation must not duplicate the open branch host");
            if (hosts.Length != 1) return;
            var activeChild = RazorProbeControl.LastCreated;
            assert.NotEqual(null, activeChild,
                "The branch should have a child after reactivation");
            if (activeChild == null) return;
            assert.IsTrue(activeChild.IsActive,
                "The retained or replacement child should be active");
            assert.Equal(hosts[0], activeChild.Element,
                "The active child should own the only branch host");
            assert.Equal(RazorProbeControl.CreatedCount - 1, RazorProbeControl.DisposedCount,
                "Any replaced child should be disposed, while a reused child stays live");
        }

        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(
                new TestWindowTimer(),
                10,
                10);
        }

        // ------------------------------------------------------------------
        // Phase 1: Basic text binding (toolchain smoke test)
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorSimpleTextBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Hello Razor";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;

            assert.NotEqual(null, control.Skin, "Razor skin should be compiled and available");

            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "Skin should render a span element inside the data-test div");
            assert.Equal("Hello Razor", span.TextContent,
                "Span text content should match the bound PropStr1 value");
        }

        // ------------------------------------------------------------------
        // Phase 2: OneWay binding reactivity
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorOneWayReactivity(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Initial";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("Initial", span.TextContent, "Initial value should be rendered");

            vm.PropStr1 = "Updated";
            assert.Equal("Updated", span.TextContent,
                "Span should update reactively when observable property changes");
        }

        [Test]
        public static void TestRazorMultiplePropertyChanges(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "V1";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("V1", span.TextContent, "Initial value");

            vm.PropStr1 = "V2";
            assert.Equal("V2", span.TextContent, "After first update");

            vm.PropStr1 = "V3";
            assert.Equal("V3", span.TextContent, "After second update");

            vm.PropStr1 = "";
            assert.Equal("", span.TextContent, "After clearing to empty string");
        }

        // ------------------------------------------------------------------
        // Phase 3: OneTime binding (non-observable)
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorOneTimeBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorPlainVM();
            vm.AppVersion = "1.0.0";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorOneTimeText;

            assert.NotEqual(null, control.Skin, "OneTime skin should be compiled");

            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "Should render span element");
            assert.Equal("1.0.0", span.TextContent, "Should show initial value");

            // OneTime bindings should NOT update when property changes
            vm.AppVersion = "2.0.0";
            assert.Equal("1.0.0", span.TextContent,
                "OneTime binding should NOT update after property change");
        }

        // ------------------------------------------------------------------
        // Phase 3: Multiple independent bindings
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorMultiBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Alice";
            vm.Count = 42;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
            control.Activate();

            var nameSpan = element.QuerySelector("[data-test] .name span");
            var countSpan = element.QuerySelector("[data-test] .count span");
            assert.NotEqual(null, nameSpan, "Name span should exist");
            assert.NotEqual(null, countSpan, "Count span should exist");
            assert.Equal("Alice", nameSpan.TextContent, "Name should show initial value");
            assert.Equal("42", countSpan.TextContent, "Count should show initial value");

            vm.Name = "Bob";
            assert.Equal("Bob", nameSpan.TextContent, "Name should update reactively");
            assert.Equal("42", countSpan.TextContent,
                "Count should remain unchanged when only Name changes");

            vm.Count = 99;
            assert.Equal("Bob", nameSpan.TextContent,
                "Name should remain unchanged when only Count changes");
            assert.Equal("99", countSpan.TextContent, "Count should update reactively");
        }

        // ------------------------------------------------------------------
        // Phase 3: Lifecycle tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorActivateRendersInitialValues(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Before Activate";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;

            // Before activation, skin should not be rendered
            var span = element.QuerySelector("[data-test] span");
            assert.Equal(null, span, "Before Activate, no skin content should be in DOM");

            control.Activate();

            span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "After Activate, skin content should be in DOM");
            assert.Equal("Before Activate", span.TextContent, "Should show value set before Activate");
        }

        [Test]
        public static void TestRazorDataContextBeforeActivate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Set Before";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("Set Before", span.TextContent,
                "DataContext set before Activate should render correctly");
        }

        [Test]
        public static void TestRazorChangeDataContextAfterActivate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm1 = new TestViewModelA();
            vm1.PropStr1 = "VM1";
            control.DataContext = vm1;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("VM1", span.TextContent, "Should show first VM value");

            var vm2 = new TestViewModelA();
            vm2.PropStr1 = "VM2";
            control.DataContext = vm2;

            span = element.QuerySelector("[data-test] span");
            assert.Equal("VM2", span.TextContent,
                "Should show second VM value after DataContext change");

            // Changes to old VM should NOT affect the control
            vm1.PropStr1 = "VM1 Updated";
            span = element.QuerySelector("[data-test] span");
            assert.Equal("VM2", span.TextContent,
                "Old VM changes should not affect control after DataContext swap");
        }

        // ------------------------------------------------------------------
        // Graph mode tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestGraphSimpleTextBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Hello Graph";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.GraphSimpleText;

            assert.NotEqual(null, control.Skin, "Graph skin should be compiled and available");

            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "Skin should render a span element");
            assert.Equal("Hello Graph", span.TextContent,
                "Span text should match bound PropStr1 value");
        }

        [Test]
        public static void TestGraphOneWayReactivity(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Initial";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.GraphSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("Initial", span.TextContent, "Initial value should be rendered");

            vm.PropStr1 = "Updated";
            assert.Equal("Updated", span.TextContent,
                "Graph binding should update reactively when property changes");
        }

        [Test]
        public static void TestGraphDataContextChange(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm1 = new TestViewModelA();
            vm1.PropStr1 = "VM1";
            control.DataContext = vm1;
            control.Skin = RazorSkinTemplatesClass.GraphSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("VM1", span.TextContent, "Should show first VM value");

            var vm2 = new TestViewModelA();
            vm2.PropStr1 = "VM2";
            control.DataContext = vm2;

            span = element.QuerySelector("[data-test] span");
            assert.Equal("VM2", span.TextContent,
                "Should show second VM value after DataContext change");

            vm1.PropStr1 = "VM1 Updated";
            span = element.QuerySelector("[data-test] span");
            assert.Equal("VM2", span.TextContent,
                "Old VM changes should not affect control after DataContext swap");
        }

        [Test]
        public static void TestGraphMultiBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Alice";
            vm.Count = 42;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.GraphMultiBinding;
            control.Activate();

            var nameSpan = element.QuerySelector("[data-test] .name span");
            var countSpan = element.QuerySelector("[data-test] .count span");
            assert.NotEqual(null, nameSpan, "Name span should exist");
            assert.NotEqual(null, countSpan, "Count span should exist");
            assert.Equal("Alice", nameSpan.TextContent, "Name should show initial value");
            assert.Equal("42", countSpan.TextContent, "Count should show initial value");

            vm.Name = "Bob";
            assert.Equal("Bob", nameSpan.TextContent, "Name should update reactively");
            assert.Equal("42", countSpan.TextContent,
                "Count should remain unchanged when only Name changes");

            vm.Count = 99;
            assert.Equal("Bob", nameSpan.TextContent,
                "Name should remain unchanged when only Count changes");
            assert.Equal("99", countSpan.TextContent, "Count should update reactively");
        }

        // ------------------------------------------------------------------
        // Attribute / Style / Class Binding Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorClassBindingInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.CssClass = "highlight";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorClassBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            assert.Equal("highlight", div.ClassName, "Class should reflect initial CssClass value");
        }

        [Test]
        public static void TestRazorClassBindingUpdate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.CssClass = "highlight";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorClassBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.Equal("highlight", div.ClassName, "Initial class");

            vm.CssClass = "selected";
            assert.Equal("selected", div.ClassName, "Class should update when CssClass changes");
        }

        [Test]
        public static void TestRazorStyleBindingInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.DisplayStyle = "block";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStyleBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            assert.Equal("display:block", div.GetAttribute("style"),
                "Style should contain initial DisplayStyle value");
        }

        [Test]
        public static void TestRazorStyleBindingUpdate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.DisplayStyle = "block";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStyleBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");

            vm.DisplayStyle = "none";
            assert.Equal("display:none", div.GetAttribute("style"),
                "Style should update when DisplayStyle changes");
        }

        [Test]
        public static void TestRazorAttrBindingInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Title = "My Title";
            vm.Count = 5;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorAttrBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            assert.Equal("My Title", div.GetAttribute("title"),
                "title attribute should reflect initial Title value");
            assert.Equal("5", div.GetAttribute("data-count"),
                "data-count attribute should reflect initial Count value");
        }

        [Test]
        public static void TestRazorAttrBindingUpdate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Title = "Original";
            vm.Count = 1;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorAttrBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");

            vm.Title = "Updated Title";
            assert.Equal("Updated Title", div.GetAttribute("title"),
                "title attribute should update when Title changes");
        }

        [Test]
        public static void TestRazorMultiAttrBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.CssClass = "active";
            vm.Title = "Tooltip";
            vm.Count = 10;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorMultiAttr;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            assert.Equal("active", div.ClassName, "class should bind");
            assert.Equal("Tooltip", div.GetAttribute("title"), "title should bind");
            assert.Equal("10", div.GetAttribute("data-count"), "data-count should bind");

            vm.CssClass = "inactive";
            vm.Title = "New Tip";
            assert.Equal("inactive", div.ClassName, "class should update");
            assert.Equal("New Tip", div.GetAttribute("title"), "title should update");
        }

        // ------------------------------------------------------------------
        // Computed Expression Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorComputedInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Price = 10;
            vm.Quantity = 3;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorComputed;
            control.Activate();

            var span = element.QuerySelector("[data-test] .total");
            assert.NotEqual(null, span, "Computed template should render");
            assert.Equal("30", span.TextContent, "Should show Price * Quantity = 30");
        }

        [Test]
        public static void TestRazorComputedPriceChange(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Price = 10;
            vm.Quantity = 3;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorComputed;
            control.Activate();

            var span = element.QuerySelector("[data-test] .total");
            assert.Equal("30", span.TextContent, "Initial computed value");

            vm.Price = 20;
            assert.Equal("60", span.TextContent,
                "Changing Price should trigger recompute: 20 * 3 = 60");
        }

        [Test]
        public static void TestRazorComputedQuantityChange(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Price = 10;
            vm.Quantity = 3;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorComputed;
            control.Activate();

            var span = element.QuerySelector("[data-test] .total");
            assert.Equal("30", span.TextContent, "Initial computed value");

            vm.Quantity = 5;
            assert.Equal("50", span.TextContent,
                "Changing Quantity should trigger recompute: 10 * 5 = 50");
        }

        // ------------------------------------------------------------------
        // Conditional (@if / @else) Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorIfOnlyTrue(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfOnly;
            control.Activate();

            var content = element.QuerySelector("[data-test] .active-content");
            assert.NotEqual(null, content, "@if(true) should render content");
            assert.Equal("Active", content.TextContent, "Content should be 'Active'");
        }

        [Test]
        public static void TestRazorIfOnlyFalse(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfOnly;
            control.Activate();

            var content = element.QuerySelector("[data-test] .active-content");
            assert.Equal(null, content, "@if(false) should NOT render content");
        }

        [Test]
        public static void TestRazorIfElseShowsTrue(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElse;
            control.Activate();

            var ifBranch = element.QuerySelector("[data-test] .if-branch");
            var elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.NotEqual(null, ifBranch, "If branch should be visible when IsActive=true");
            assert.Equal(null, elseBranch, "Else branch should NOT be visible when IsActive=true");
        }

        [Test]
        public static void TestRazorIfElseShowsFalse(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElse;
            control.Activate();

            var ifBranch = element.QuerySelector("[data-test] .if-branch");
            var elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.Equal(null, ifBranch, "If branch should NOT be visible when IsActive=false");
            assert.NotEqual(null, elseBranch, "Else branch should be visible when IsActive=false");
        }

        [Test]
        public static void TestRazorIfElseToggle(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElse;
            control.Activate();

            var ifBranch = element.QuerySelector("[data-test] .if-branch");
            assert.NotEqual(null, ifBranch, "If branch visible initially");

            vm.IsActive = false;
            var elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.NotEqual(null, elseBranch, "Else branch should appear after toggle to false");
            ifBranch = element.QuerySelector("[data-test] .if-branch");
            assert.Equal(null, ifBranch, "If branch should disappear after toggle to false");

            vm.IsActive = true;
            ifBranch = element.QuerySelector("[data-test] .if-branch");
            assert.NotEqual(null, ifBranch, "If branch should reappear after toggle back to true");
            elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.Equal(null, elseBranch, "Else branch should disappear after toggle back to true");
        }

        [Test]
        public static void TestRazorIfElseIfFirstBranch(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            vm.ShowDetails = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElseIf;
            control.Activate();

            var active = element.QuerySelector("[data-test] .branch-active");
            var details = element.QuerySelector("[data-test] .branch-details");
            var def = element.QuerySelector("[data-test] .branch-default");
            assert.NotEqual(null, active, "First branch should show when IsActive=true");
            assert.Equal(null, details, "Second branch should not show");
            assert.Equal(null, def, "Default branch should not show");
        }

        [Test]
        public static void TestRazorIfElseIfSecondBranch(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            vm.ShowDetails = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElseIf;
            control.Activate();

            var active = element.QuerySelector("[data-test] .branch-active");
            var details = element.QuerySelector("[data-test] .branch-details");
            var def = element.QuerySelector("[data-test] .branch-default");
            assert.Equal(null, active, "First branch should not show");
            assert.NotEqual(null, details, "Second branch should show when IsActive=false, ShowDetails=true");
            assert.Equal(null, def, "Default branch should not show");
        }

        [Test]
        public static void TestRazorIfElseIfDefaultBranch(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            vm.ShowDetails = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElseIf;
            control.Activate();

            var active = element.QuerySelector("[data-test] .branch-active");
            var details = element.QuerySelector("[data-test] .branch-details");
            var def = element.QuerySelector("[data-test] .branch-default");
            assert.Equal(null, active, "First branch should not show");
            assert.Equal(null, details, "Second branch should not show");
            assert.NotEqual(null, def, "Default branch should show when both are false");
        }

        [Test]
        public static void TestRazorIfBindingsActive(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            vm.Name = "Alice";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfBindings;
            control.Activate();

            var nameSpan = element.QuerySelector("[data-test] .active-name");
            assert.NotEqual(null, nameSpan, "Active branch with binding should render");
            assert.Equal("Alice", nameSpan.TextContent, "Binding inside @if should show initial value");

            vm.Name = "Bob";
            assert.Equal("Bob", nameSpan.TextContent,
                "Binding inside @if should update reactively");
        }

        [Test]
        public static void TestRazorNestedIfBothTrue(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            vm.ShowDetails = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorNestedIf;
            control.Activate();

            var withDetails = element.QuerySelector("[data-test] .active-with-details");
            assert.NotEqual(null, withDetails,
                "Nested @if should show inner content when both conditions true");
        }

        [Test]
        public static void TestRazorStaticIf(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorPlainVM();
            vm.IsStatic = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStaticIf;
            control.Activate();

            var content = element.QuerySelector("[data-test] .static-content");
            assert.NotEqual(null, content, "Static @if(true) should render content");
            assert.Equal("Static", content.TextContent, "Content should be 'Static'");
        }

        // ------------------------------------------------------------------
        // @foreach / Collection Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorForeachInitialRender(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Apple";
            items.Add(item1);
            var item2 = new RazorItemVM();
            item2.Name = "Banana";
            items.Add(item2);
            var item3 = new RazorItemVM();
            item3.Name = "Cherry";
            items.Add(item3);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(3, lis.Length, "Should render 3 li elements");
            assert.Equal("Apple", lis[0].TextContent, "First item");
            assert.Equal("Banana", lis[1].TextContent, "Second item");
            assert.Equal("Cherry", lis[2].TextContent, "Third item");
        }

        [Test]
        public static void TestRazorForeachAddItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Apple";
            items.Add(item1);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(1, lis.Length, "Should start with 1 item");

            var item2 = new RazorItemVM();
            item2.Name = "Banana";
            items.Add(item2);
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "Should have 2 items after Add");
            assert.Equal("Banana", lis[1].TextContent, "New item should appear at end");
        }

        [Test]
        public static void TestRazorForeachRemoveItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Apple";
            items.Add(item1);
            var item2 = new RazorItemVM();
            item2.Name = "Banana";
            items.Add(item2);
            var item3 = new RazorItemVM();
            item3.Name = "Cherry";
            items.Add(item3);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            items.RemoveAt(1);
            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "Should have 2 items after RemoveAt(1)");
            assert.Equal("Apple", lis[0].TextContent, "First item unchanged");
            assert.Equal("Cherry", lis[1].TextContent, "Cherry should move up");
        }

        [Test]
        public static void TestRazorForeachClear(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Apple";
            items.Add(item1);
            var item2 = new RazorItemVM();
            item2.Name = "Banana";
            items.Add(item2);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "Should start with 2 items");

            items.Clear();
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(0, lis.Length, "Clear should remove all items from DOM");
        }

        [Test]
        public static void TestRazorForeachMultipleOps(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var itemA = new RazorItemVM();
            itemA.Name = "A";
            items.Add(itemA);
            var itemB = new RazorItemVM();
            itemB.Name = "B";
            items.Add(itemB);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var itemC = new RazorItemVM();
            itemC.Name = "C";
            items.Add(itemC);
            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(3, lis.Length, "After add: 3 items");

            items.RemoveAt(0);
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "After remove: 2 items");
            assert.Equal("B", lis[0].TextContent, "B should be first after removing A");

            var itemD = new RazorItemVM();
            itemD.Name = "D";
            items.Add(itemD);
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(3, lis.Length, "After second add: 3 items");
            assert.Equal("D", lis[2].TextContent, "D should be last");
        }

        [Test]
        public static void TestRazorForeachItemBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Task 1";
            item1.IsComplete = false;
            var item2 = new RazorItemVM();
            item2.Name = "Task 2";
            item2.IsComplete = true;
            items.Add(item1);
            items.Add(item2);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachBindings;
            control.Activate();

            var names = element.QuerySelectorAll("[data-test] .item-name");
            assert.Equal(2, names.Length, "Should render 2 items");
            assert.Equal("Task 1", names[0].TextContent, "First item name");
            assert.Equal("Task 2", names[1].TextContent, "Second item name");

            item1.Name = "Updated Task 1";
            names = element.QuerySelectorAll("[data-test] .item-name");
            assert.Equal("Updated Task 1", names[0].TextContent,
                "Changing item property should update only that items DOM");
            assert.Equal("Task 2", names[1].TextContent,
                "Other items should remain unchanged");
        }

        // ------------------------------------------------------------------
        // Nested Control Flow Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorIfInForeach(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Done Task";
            item1.IsComplete = true;
            items.Add(item1);
            var item2 = new RazorItemVM();
            item2.Name = "Pending Task";
            item2.IsComplete = false;
            items.Add(item2);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfInForeach;
            control.Activate();

            var doneItems = element.QuerySelectorAll("[data-test] .done");
            var pendingItems = element.QuerySelectorAll("[data-test] .pending");
            assert.Equal(1, doneItems.Length, "Should have 1 done item");
            assert.Equal(1, pendingItems.Length, "Should have 1 pending item");
            assert.Equal("Done Task", doneItems[0].TextContent, "Done item text");
            assert.Equal("Pending Task", pendingItems[0].TextContent, "Pending item text");
        }

        [Test]
        public static void TestRazorForeachInIfActive(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Item 1";
            items.Add(item1);
            var item2 = new RazorItemVM();
            item2.Name = "Item 2";
            items.Add(item2);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachInIf;
            control.Activate();

            var list = element.QuerySelector("[data-test] .active-list");
            assert.NotEqual(null, list, "List should render when IsActive=true");
            var lis = element.QuerySelectorAll("[data-test] .active-list li");
            assert.Equal(2, lis.Length, "Should show 2 list items");
            var disabled = element.QuerySelector("[data-test] .disabled-msg");
            assert.Equal(null, disabled, "Disabled message should not show");
        }

        [Test]
        public static void TestRazorForeachInIfToggle(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Item 1";
            items.Add(item1);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachInIf;
            control.Activate();

            var list = element.QuerySelector("[data-test] .active-list");
            assert.NotEqual(null, list, "List visible when active");

            vm.IsActive = false;
            list = element.QuerySelector("[data-test] .active-list");
            assert.Equal(null, list, "List should disappear when IsActive toggled to false");
            var disabled = element.QuerySelector("[data-test] .disabled-msg");
            assert.NotEqual(null, disabled, "Disabled message should appear");
        }

        // ------------------------------------------------------------------
        // Event Binding Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorEventMethodRef(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.ClickCount = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorEventClick;
            control.Activate();

            assert.Equal(0, vm.ClickCount, "ClickCount should start at 0");

            var btn = element.QuerySelector("[data-test] .btn-click");
            assert.NotEqual(null, btn, "Button should render");
            btn.Click();

            assert.Equal(1, vm.ClickCount,
                "Method ref click should fire IncrementClick, ClickCount = 1");
        }

        [Test]
        public static void TestRazorEventLambda(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.ClickCount = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorEventLambda;
            control.Activate();

            var btn = element.QuerySelector("[data-test] .btn-lambda");
            assert.NotEqual(null, btn, "Lambda button should render");
            btn.Click();

            assert.Equal(1, vm.ClickCount,
                "Lambda click should fire IncrementClick, ClickCount = 1");
        }

        [Test]
        public static void TestRazorEventUpdatesBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.ClickCount = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorEventClick;
            control.Activate();

            var countSpan = element.QuerySelector("[data-test] .click-count");
            assert.Equal("0", countSpan.TextContent, "Count should show 0 initially");

            var btn = element.QuerySelector("[data-test] .btn-click");
            btn.Click();

            assert.Equal("1", countSpan.TextContent,
                "Click should update ClickCount, which should reactively update the span");

            btn.Click();
            assert.Equal("2", countSpan.TextContent,
                "Second click should show 2");
        }

        // ------------------------------------------------------------------
        // Extended Lifecycle Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorDeactivateStopsUpdates(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Before";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("Before", span.TextContent, "Initial value");

            control.Deactivate();
            vm.PropStr1 = "After Deactivate";
            assert.Equal("Before", span.TextContent,
                "After Deactivate, VM changes should NOT update DOM");
        }

        [Test]
        public static void TestRazorReactivateResumes(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "V1";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("V1", span.TextContent, "Initial value");

            control.Deactivate();
            vm.PropStr1 = "V2";

            control.Activate();
            span = element.QuerySelector("[data-test] span");
            assert.Equal("V2", span.TextContent,
                "After reactivation, should show latest VM value");
        }

        [Test]
        public static void TestRazorDisposeCleanup(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Alice";
            vm.Count = 1;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
            control.Activate();

            var nameSpan = element.QuerySelector("[data-test] .name span");
            assert.Equal("Alice", nameSpan.TextContent, "Initial value before dispose");

            control.Dispose();
            vm.Name = "Bob";
            assert.IsTrue(true, "Dispose should not throw when VM changes afterward");
        }

        [Test]
        public static void TestRazorNullDataContext(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "HasValue";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("HasValue", span.TextContent, "Initial value");

            control.DataContext = null;
            assert.IsTrue(true, "Setting DataContext to null should not throw");
        }

        [Test]
        public static void TestRazorEmptyStringBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "Span should still render with empty string");
            assert.Equal("", span.TextContent, "Empty string should render as empty text");
        }

        // ------------------------------------------------------------------
        // Real-Life Scenario Tests (Todo App)
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorTodoInitialRender(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 2;
            var items = new ObservableCollection<RazorItemVM>();
            var todo1 = new RazorItemVM();
            todo1.Name = "Buy groceries";
            todo1.IsComplete = false;
            items.Add(todo1);
            var todo2 = new RazorItemVM();
            todo2.Name = "Write tests";
            todo2.IsComplete = true;
            items.Add(todo2);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var countSpan = element.QuerySelector("[data-test] .todo-count");
            assert.Equal("2", countSpan.TextContent, "Count should show 2");

            var pending = element.QuerySelectorAll("[data-test] .todo-pending");
            var done = element.QuerySelectorAll("[data-test] .todo-done");
            assert.Equal(1, pending.Length, "Should have 1 pending item");
            assert.Equal(1, done.Length, "Should have 1 done item");

            var pendingName = pending[0].QuerySelector(".todo-name");
            assert.Equal("Buy groceries", pendingName.TextContent, "Pending item name");
            var doneName = done[0].QuerySelector(".todo-name");
            assert.Equal("Write tests", doneName.TextContent, "Done item name");
        }

        [Test]
        public static void TestRazorTodoAddItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 1;
            var items = new ObservableCollection<RazorItemVM>();
            var task1 = new RazorItemVM();
            task1.Name = "Task 1";
            task1.IsComplete = false;
            items.Add(task1);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(1, allItems.Length, "Should start with 1 item");

            var task2 = new RazorItemVM();
            task2.Name = "Task 2";
            task2.IsComplete = false;
            items.Add(task2);
            vm.Count = 2;
            allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(2, allItems.Length, "Should have 2 items after Add");

            var countSpan = element.QuerySelector("[data-test] .todo-count");
            assert.Equal("2", countSpan.TextContent, "Count should update to 2");
        }

        [Test]
        public static void TestRazorTodoToggleComplete(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 1;
            var items = new ObservableCollection<RazorItemVM>();
            var task = new RazorItemVM();
            task.Name = "My Task";
            task.IsComplete = false;
            items.Add(task);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var pending = element.QuerySelectorAll("[data-test] .todo-pending");
            assert.Equal(1, pending.Length, "Should start as pending");

            task.IsComplete = true;
            var done = element.QuerySelectorAll("[data-test] .todo-done");
            pending = element.QuerySelectorAll("[data-test] .todo-pending");
            assert.Equal(1, done.Length, "Should show as done after toggle");
            assert.Equal(0, pending.Length, "Should not show as pending after toggle");
        }

        [Test]
        public static void TestRazorTodoRemoveItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 2;
            var items = new ObservableCollection<RazorItemVM>();
            var keep = new RazorItemVM();
            keep.Name = "Keep";
            keep.IsComplete = false;
            items.Add(keep);
            var remove = new RazorItemVM();
            remove.Name = "Remove";
            remove.IsComplete = true;
            items.Add(remove);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(2, allItems.Length, "Should start with 2 items");

            items.RemoveAt(1);
            vm.Count = 1;
            allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(1, allItems.Length, "Should have 1 item after remove");

            var countSpan = element.QuerySelector("[data-test] .todo-count");
            assert.Equal("1", countSpan.TextContent, "Count should update to 1");
        }

        // ------------------------------------------------------------------
        // Additional Lifecycle / Edge-Case Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorSubControlSkinSwapDisposesOldChild(Assert assert)
        {
            RazorProbeControl.CreatedCount = 0;
            RazorProbeControl.DisposedCount = 0;
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM();
            vm.Title = "Before";
            vm.ShowDetails = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSubControlTopLevel;
            control.Activate();

            var oldHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, oldHost, "The first skin should mount a child");
            if (oldHost == null) return;

            control.Skin = RazorSkinTemplatesClass.RazorSubControlConditional;
            var newHost = element.QuerySelector("[data-ns-subctl]");
            assert.NotEqual(null, newHost, "The new skin should mount a child");
            if (newHost == null) return;
            assert.NotEqual(oldHost, newHost, "The new skin should own a new child host");
            assert.Equal(1, RazorProbeControl.DisposedCount,
                "Swapping skins should dispose the first skin's child");

            vm.Title = "After";
            assert.Equal("Before", oldHost.GetAttribute("data-bound-text"),
                "The old skin child must not receive updates");
            assert.Equal("After", newHost.GetAttribute("data-bound-text"),
                "The new skin child should stay reactive");
        }

        [Test]
        public static void TestRazorSkinSwap(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "First Skin";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("First Skin", span.TextContent, "First skin should render");

            // Swap to a different skin
            control.Skin = RazorSkinTemplatesClass.GraphSimpleText;

            span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "New skin should render after swap");
            assert.Equal("First Skin", span.TextContent,
                "New skin should show same DataContext value");
        }

        [Test]
        public static void TestRazorSkinSwapUpdates(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Initial";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            // Swap to graph skin
            control.Skin = RazorSkinTemplatesClass.GraphSimpleText;

            // Updates should work on new skin
            vm.PropStr1 = "After Swap";
            var span = element.QuerySelector("[data-test] span");
            assert.Equal("After Swap", span.TextContent,
                "Reactive updates should work after skin swap");
        }

        [Test]
        public static void TestRazorForeachEmptyStart(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(0, lis.Length, "Should start with 0 items");

            // Add items dynamically to empty collection
            var item1 = new RazorItemVM();
            item1.Name = "First";
            items.Add(item1);
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(1, lis.Length, "Should have 1 item after first Add");
            assert.Equal("First", lis[0].TextContent, "First item text");

            var item2 = new RazorItemVM();
            item2.Name = "Second";
            items.Add(item2);
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "Should have 2 items after second Add");
        }

        [Test]
        public static void TestRazorComputedBothChange(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Price = 5;
            vm.Quantity = 2;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorComputed;
            control.Activate();

            var span = element.QuerySelector("[data-test] .total");
            assert.Equal("10", span.TextContent, "Initial: 5 * 2 = 10");

            // Change both properties
            vm.Price = 7;
            vm.Quantity = 4;
            assert.Equal("28", span.TextContent,
                "After changing both: 7 * 4 = 28");
        }

        [Test]
        public static void TestRazorEventMultipleClicks(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.ClickCount = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorEventClick;
            control.Activate();

            var btn = element.QuerySelector("[data-test] .btn-click");
            var countSpan = element.QuerySelector("[data-test] .click-count");

            btn.Click();
            btn.Click();
            btn.Click();

            assert.Equal(3, vm.ClickCount, "ClickCount should be 3 after 3 clicks");
            assert.Equal("3", countSpan.TextContent,
                "DOM should reactively show 3 after 3 clicks");
        }

        [Test]
        public static void TestRazorIfConditionWithBinding(Assert assert)
        {
            // Test that bindings inside @if blocks update correctly after gate toggles
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            vm.Name = "Alice";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfBindings;
            control.Activate();

            var nameSpan = element.QuerySelector("[data-test] .active-name");
            assert.Equal(null, nameSpan, "When IsActive=false, binding content should not render");

            // Toggle gate open
            vm.IsActive = true;
            nameSpan = element.QuerySelector("[data-test] .active-name");
            assert.NotEqual(null, nameSpan, "When IsActive toggled to true, content should appear");
            assert.Equal("Alice", nameSpan.TextContent,
                "Content should show current Name when gate opens");

            // Change name while gate is open
            vm.Name = "Bob";
            nameSpan = element.QuerySelector("[data-test] .active-name");
            assert.Equal("Bob", nameSpan.TextContent,
                "Binding should update while gate is open");
        }

        [Test]
        public static void TestRazorDeactivatePreservesElements(Assert assert)
        {
            // Verify that deactivate/reactivate cycle doesn't corrupt the DOM.
            // After reactivation, updates to the VM should still propagate.
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Before";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("Before", span.TextContent, "Initial value");

            control.Deactivate();
            control.Activate();

            // After reactivation, DOM should still be functional
            span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "Span should exist after reactivation");

            vm.PropStr1 = "After Reactivate";
            span = element.QuerySelector("[data-test] span");
            assert.Equal("After Reactivate", span.TextContent,
                "Reactive updates should work after deactivate/reactivate cycle");
        }

        /// <summary>
        /// Verifies that property changes flush synchronously to the DOM.
        /// This is the key behavioral contract of CreatePropertyCallback: each
        /// property mutation must update the DOM before the next line of
        /// application code executes. If the flush were async (via MarkDirty +
        /// TaskScheduler), these intermediate assertions would fail because
        /// the DOM wouldn't update until the next microtask boundary.
        /// </summary>
        [Test]
        public static void TestRazorSynchronousFlushGuarantee(Assert assert)
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
            assert.Equal("Step0", nameSpan.TextContent, "Initial Name");
            assert.Equal("0", countSpan.TextContent, "Initial Count");

            // Rapid sequential mutations — each must flush to DOM synchronously
            // before the next assertion executes. If flush were async, these
            // intermediate checks would still see stale values.
            vm.Name = "Step1";
            assert.Equal("Step1", nameSpan.TextContent,
                "Name must be 'Step1' synchronously after first mutation");

            vm.Count = 10;
            assert.Equal("10", countSpan.TextContent,
                "Count must be '10' synchronously after second mutation");

            vm.Name = "Step2";
            assert.Equal("Step2", nameSpan.TextContent,
                "Name must be 'Step2' synchronously after third mutation");

            vm.Count = 20;
            assert.Equal("20", countSpan.TextContent,
                "Count must be '20' synchronously after fourth mutation");
        }

        // ------------------------------------------------------------------
        // Mixed Marker Tests (span + non-span elements in foreach)
        // ------------------------------------------------------------------

        /// <summary>
        /// Verifies that CollectSpanElements correctly collects both span markers
        /// and non-span self-binding elements (e.g. input[data-ns-bind]) inside
        /// a @foreach template. Without the fix, the input element is missed by
        /// getElementsByTagName("span") and all subsequent element indices are
        /// off by one, causing the input's value to bind to the wrong DOM node.
        /// </summary>
        [Test]
        public static void TestForeachMixedMarkersInitialBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Task A";
            item1.Status = "active";
            items.Add(item1);
            var item2 = new RazorItemVM();
            item2.Name = "Task B";
            item2.Status = "done";
            items.Add(item2);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachMixedMarkers;
            control.Activate();

            // Verify span text binding works
            var labels = element.QuerySelectorAll("[data-test] .item-label");
            assert.Equal(2, labels.Length, "Should render 2 label spans");
            assert.Equal("Task A", labels[0].TextContent, "First label text");
            assert.Equal("Task B", labels[1].TextContent, "Second label text");

            // Verify input value binding works (the bug that was fixed)
            var inputs = element.QuerySelectorAll("[data-test] .item-input");
            assert.Equal(2, inputs.Length, "Should render 2 input elements");
            assert.Equal("active", ((InputElement)inputs[0]).Value,
                "First input value must be 'active' — fails if CollectSpanElements misses non-span markers");
            assert.Equal("done", ((InputElement)inputs[1]).Value,
                "Second input value must be 'done'");
        }

        /// <summary>
        /// Verifies that reactive updates propagate correctly to both span and
        /// input elements within the same foreach item template.
        /// </summary>
        [Test]
        public static void TestForeachMixedMarkersReactiveUpdate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM();
            item1.Name = "Original";
            item1.Status = "pending";
            items.Add(item1);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachMixedMarkers;
            control.Activate();

            // Mutate the span-bound property
            item1.Name = "Updated";
            var label = element.QuerySelector("[data-test] .item-label");
            assert.Equal("Updated", label.TextContent,
                "Span binding should react to Name change");

            // Mutate the input-bound property
            item1.Status = "completed";
            var input = element.QuerySelector("[data-test] .item-input");
            assert.Equal("completed", ((InputElement)input).Value,
                "Input value binding should react to Status change");
        }

        /// <summary>
        /// Verifies that dynamically adding items to the collection correctly
        /// binds both span and input elements for the new item.
        /// </summary>
        [Test]
        public static void TestForeachMixedMarkersDynamicAdd(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachMixedMarkers;
            control.Activate();

            // Start empty — add an item dynamically
            var newItem = new RazorItemVM();
            newItem.Name = "Dynamic Task";
            newItem.Status = "new";
            items.Add(newItem);

            var labels = element.QuerySelectorAll("[data-test] .item-label");
            var inputs = element.QuerySelectorAll("[data-test] .item-input");
            assert.Equal(1, labels.Length, "Should render 1 item after add");
            assert.Equal("Dynamic Task", labels[0].TextContent, "Label text for dynamically added item");
            assert.Equal("new", ((InputElement)inputs[0]).Value,
                "Input value for dynamically added item — exercises CollectSpanElements on cloned template");

            // Verify reactivity on the dynamically added item
            newItem.Status = "in-progress";
            inputs = element.QuerySelectorAll("[data-test] .item-input");
            assert.Equal("in-progress", ((InputElement)inputs[0]).Value,
                "Dynamic item input binding should react to Status change");
        }

        // ------------------------------------------------------------------
        // Phase 8: @styles directive — CSS pipeline integration
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorStyledTemplate_Renders(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "CSS Test";
            vm.Count = 42;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStyledTemplate;

            assert.NotEqual(null, control.Skin, "Styled skin should compile with @styles directive");
            control.Activate();

            var header = element.QuerySelectorAll("[data-test] h1");
            assert.Equal(1, header.Length, "Template should render an h1 element");
            assert.Equal("CSS Test", header[0].TextContent, "h1 should display model Name");
        }

        [Test]
        public static void TestRazorStyledTemplate_HasCssClass(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Styled";
            vm.Count = 1;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStyledTemplate;
            control.Activate();

            var header = element.QuerySelectorAll("[data-test] h1");
            var cssClass = header[0].GetAttribute("class");
            assert.NotEqual(null, cssClass, "h1 should have a class attribute from @styles");
        }

        // Note: TestRazorStyledTemplate_CssInDom omitted — verifying <style> injection
        // requires Document-level QuerySelectorAll which is not supported by the NScript
        // compiler. The CSS IIFE injection is verified by the E2E TodoApp tests instead.

        [Test]
        public static void TestRazorMultiStyled_Renders(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Multi CSS";
            vm.Count = 2;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorMultiStyled;

            assert.NotEqual(null, control.Skin, "Multi-styled skin should compile with two @styles directives");
            control.Activate();

            var header = element.QuerySelectorAll("[data-test] h1");
            assert.Equal(1, header.Length, "Multi-styled template should render");
            assert.Equal("Multi CSS", header[0].TextContent, "h1 should display model Name");
        }

        [Test]
        public static void TestRazorMultiStyled_BothSheetsApplied(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Both Sheets";
            vm.Count = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorMultiStyled;
            control.Activate();

            var sidebar = element.QuerySelectorAll("[data-test] aside");
            assert.Equal(1, sidebar.Length, "aside element should render for styled-sidebar class");
            var sidebarClass = sidebar[0].GetAttribute("class");
            assert.NotEqual(null, sidebarClass, "aside should have class from second styles sheet");
        }

        [Test]
        public static void TestRazorStyledTemplate_ReactiveWithCss(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Before";
            vm.Count = 10;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStyledTemplate;
            control.Activate();

            var header = element.QuerySelectorAll("[data-test] h1");
            assert.Equal("Before", header[0].TextContent, "Initial Name binding");

            vm.Name = "After";

            header = element.QuerySelectorAll("[data-test] h1");
            assert.Equal("After", header[0].TextContent,
                "Name binding should update reactively even with styles CSS classes");
        }
    }
}
