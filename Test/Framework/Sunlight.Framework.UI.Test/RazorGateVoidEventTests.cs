namespace Sunlight.Framework.UI.Test
{
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// A gate branch whose root is a void element (input, br, img) with an
    /// event binding compiles to two top-level nodes: the element carrying
    /// data-ns-bind and a trailing data-ns-evt marker span. The runtime must
    /// keep both, or the event is silently never bound.
    /// </summary>
    [TestFixture]
    public class RazorGateVoidEventTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestGatedVoidElementBindsEventAndValue(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var vm = new RazorTestVM { IsActive = true, Name = "Go" };
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorGateVoidEvent;
            control.Activate();

            var button = element.QuerySelector(".gated-button");
            assert.NotEqual(null, button, "The gated input should render");
            if (button == null) return;
            assert.Equal("Go", ((InputElement)button).Value, "The value binding should reach the input");

            button.Click();
            assert.Equal(1, vm.ClickCount, "The onclick handler on a gated void element must fire");

            // Close the gate: the input goes away and no stale handler stays on the host.
            vm.IsActive = false;
            assert.Equal(null, element.QuerySelector(".gated-button"), "Closed gate removes the input");
            var host = element.QuerySelector("[data-test='gate-void-event']");
            host.Click();
            assert.Equal(1, vm.ClickCount, "Clicking the host after the gate closed must not fire the handler");

            // Reopen: the new input is wired exactly once.
            vm.IsActive = true;
            var reopened = element.QuerySelector(".gated-button");
            assert.NotEqual(null, reopened, "Reopened gate renders the input again");
            if (reopened == null) return;
            reopened.Click();
            assert.Equal(2, vm.ClickCount, "The reopened input fires its handler once per click");
        }

        /// <summary>
        /// An item handler named only inside an @if branch of a @foreach must still be
        /// emitted by the demand-driven converter. Before the fix the method was
        /// dead-code-eliminated and the click threw "onGateOnlyClick is not a function".
        /// </summary>
        [Test]
        public static void TestItemHandlerReferencedOnlyInsideGateIsEmitted(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var item = new RazorItemVM { Name = "one", IsComplete = true };
            var vm = new RazorTestVM { Items = new ObservableCollection<RazorItemVM>() };
            vm.Items.Add(item);
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorGateOnlyHandler;
            control.Activate();

            var button = element.QuerySelector(".gate-only-button");
            assert.NotEqual(null, button, "The gated button should render for a complete item");
            if (button == null) return;

            button.Click();
            assert.Equal(10, item.SelectCount, "The gate-only item handler must exist and run");
        }

        /// <summary>
        /// Two void inputs in one row (one of them gated) each carry a trailing event
        /// marker. Resolving both markers to the shared parent made every handler fire
        /// for every input's change. Each handler must fire only for its own input.
        /// </summary>
        [Test]
        public static void TestSiblingVoidInputsKeepSeparateHandlers(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            var item = new RazorItemVM { Name = "a", Status = "b", IsComplete = true };
            var vm = new RazorTestVM { Items = new ObservableCollection<RazorItemVM>() };
            vm.Items.Add(item);
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSiblingVoidEvents;
            control.Activate();

            var first = element.QuerySelector(".first-input");
            var second = element.QuerySelector(".second-input");
            assert.NotEqual(null, first, "The first input should render");
            assert.NotEqual(null, second, "The gated second input should render");
            if (first == null || second == null) return;

            DispatchChange(second);
            assert.Equal(0, item.FirstChanges, "Changing the second input must not run the first handler");
            assert.Equal(1, item.SecondChanges, "Changing the second input runs its own handler once");

            DispatchChange(first);
            assert.Equal(1, item.FirstChanges, "Changing the first input runs its own handler once");
            assert.Equal(1, item.SecondChanges, "Changing the first input must not run the second handler");
        }

        private static void DispatchChange(Element input)
        {
            var change = Window.Instance.Document.CreateEvent("Event");
            change.InitEvent("change", true, true);
            input.DispatchEvent(change);
        }
    }
}
