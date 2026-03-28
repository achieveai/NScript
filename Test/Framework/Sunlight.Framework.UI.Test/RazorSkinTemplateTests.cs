namespace Sunlight.Framework.UI.Test
{
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>
    /// Browser-based tests for Razor skin templates.
    /// These tests verify that .skin.cshtml templates compiled through the full
    /// NScript pipeline produce correct runtime behavior in the browser.
    /// </summary>
    [TestFixture]
    public class RazorSkinTemplateTests
    {
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
    }
}
