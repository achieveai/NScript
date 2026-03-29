namespace Sunlight.Framework.UI.Test
{
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

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
    }
}
