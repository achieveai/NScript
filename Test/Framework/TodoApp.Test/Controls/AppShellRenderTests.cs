namespace TodoApp.Test.Controls
{
    using Sunlight.Framework;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Helpers;
    using SunlightUnit;
    using System.Web.Html;
    using TodoApp.Controls;
    using TodoApp.Skins;
    using TodoApp.ViewModels;

    [TestFixture]
    public class AppShellRenderTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestListHostsRenderAndSelectTheirOwnTodo(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var shell = new UISkinableElement(element);
            var vm = new AppViewModel();
            vm.InitializeWithData();
            shell.DataContext = vm;
            shell.Skin = TodoAppSkins.AppShell;
            shell.Activate();

            var hosts = element.QuerySelectorAll("[data-ns-subctl]");
            assert.Equal(3, hosts.Length, "The app should render two active and one completed todo");
            hosts[0].Click();
            assert.Equal(vm.CurrentTodos[0], vm.SelectedTodo,
                "Clicking a control host should select its own loop item");
        }

        [Test]
        public static void TestCompletedTodoUsesCompactSkinWithoutStar(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var shell = new UISkinableElement(element);
            var vm = new AppViewModel();
            vm.InitializeWithData();
            shell.DataContext = vm;
            shell.Skin = TodoAppSkins.AppShell;
            shell.Activate();

            var hosts = element.QuerySelectorAll("[data-ns-subctl]");
            assert.NotEqual(null, hosts[0].QuerySelector("[class*='star']"),
                "An active todo should show its star");
            assert.NotEqual(null, hosts[2].QuerySelector("[data-compact]"),
                "A completed todo should render the compact skin");
            assert.Equal(null, hosts[2].QuerySelector("[class*='star']"),
                "The completed row should hide its star");
        }

        [Test]
        public static void TestGatedStarClickUpdatesTodoWithoutSelectingIt(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var shell = new UISkinableElement(element);
            var vm = new AppViewModel();
            vm.InitializeWithData();
            shell.DataContext = vm;
            shell.Skin = TodoAppSkins.AppShell;
            shell.Activate();

            var star = element.QuerySelector("[data-ns-subctl] [class*='star']");
            assert.NotEqual(null, star, "The active todo should have a star");
            if (star == null) return;
            star.Click();
            assert.Equal(false, vm.CurrentTodos[0].IsImportant,
                "The star click should toggle importance through the gated event");
            assert.Equal(null, vm.SelectedTodo,
                "The star click should not bubble into the host selection handler");
        }

        [Test]
        public static void TestShowStarPropertyUpdatesTheChildSkin(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var vm = new AppViewModel();
            vm.InitializeWithData();
            var item = new TodoItemControl(element);
            item.DataContext = vm.CurrentTodos[0];
            item.Skin = TodoItemControl.DefaultSkin;
            item.ShowStar = false;
            item.Activate();

            assert.Equal(null, element.QuerySelector("[class*='star']"),
                "A bound false value should close the star gate");
            item.ShowStar = true;
            assert.NotEqual(null, element.QuerySelector("[class*='star']"),
                "Changing the auto property should open the gate live");
            item.ShowStar = false;
            assert.Equal(null, element.QuerySelector("[class*='star']"),
                "Changing it back should remove the star");
        }

        [Test]
        public static void TestShowStarGateWorksWithoutDataContext(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var item = new TodoItemControl(element);
            item.InactiveIfNullContext = false;
            item.Skin = TodoItemControl.DefaultSkin;
            item.ShowStar = true;
            item.Activate();

            assert.NotEqual(null, element.QuerySelector("[data-star-gate]"),
                "The Control.ShowStar gate should open with no DataContext");
            item.ShowStar = false;
            assert.Equal(null, element.QuerySelector("[data-star-gate]"),
                "The gate should still react to the control auto property");
        }

        [Test]
        public static void TestDetailControlFollowsSelectionAndPaneLifecycle(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var shell = new UISkinableElement(element);
            var vm = new AppViewModel();
            vm.InitializeWithData();
            shell.DataContext = vm;
            shell.Skin = TodoAppSkins.AppShell;
            shell.Activate();

            assert.Equal(null, element.QuerySelector("[data-task-detail]"),
                "No detail control should exist before selection");
            var hosts = element.QuerySelectorAll("[data-ns-subctl]");
            hosts[0].Click();
            var detail = element.QuerySelector("[data-task-detail]");
            assert.NotEqual(null, detail, "Selecting a todo should create its detail control");
            if (detail == null) return;
            assert.Equal("Buy groceries", ((InputElement)detail.QuerySelector("input")).Value,
                "The detail control should receive the selected todo as DataContext");

            vm.ToggleRightPane();
            assert.Equal(null, element.QuerySelector("[data-task-detail]"),
                "Closing the pane should remove its detail control");
            vm.ToggleRightPane();
            detail = element.QuerySelector("[data-task-detail]");
            assert.NotEqual(null, detail, "Reopening should create the detail control again");

            hosts[1].Click();
            detail = element.QuerySelector("[data-task-detail]");
            assert.Equal("Read a book", ((InputElement)detail.QuerySelector("input")).Value,
                "Switching selection should update the child DataContext");
        }

        [Test]
        public static void TestTitleEditorBindsBothWaysAndExposesNamedPart(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var vm = new AppViewModel();
            vm.InitializeWithData();
            var todo = vm.CurrentTodos[0];
            var detail = new TaskDetailControl(element);
            detail.DataContext = todo;
            detail.Skin = TaskDetailControl.DefaultSkin;
            detail.Activate();

            assert.NotEqual(null, detail.Editor, "The named editor should be reachable from parent code");
            if (detail.Editor == null) return;
            assert.Equal(todo.Title, detail.Editor.Value, "The model should initialize the editor");
            assert.Equal("filled", element.QuerySelector("input").GetAttribute("data-editor-state"),
                "A Control property ternary should render in the child skin");

            detail.Editor.Value = "Updated through child";
            assert.Equal("Updated through child", todo.Title,
                "Changing the child auto property should write back to the model");

            todo.Title = "Updated through model";
            assert.Equal("Updated through model", detail.Editor.Value,
                "A model change should update the editor auto property");
            assert.Equal("Updated through model", ((InputElement)element.QuerySelector("input")).Value,
                "The editor skin should update its input");
        }

        [Test]
        public static void TestTitleEditorWorksWithoutDataContext(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var editor = new TitleEditor(element);
            editor.InactiveIfNullContext = false;
            editor.Skin = TitleEditor.DefaultSkin;
            editor.Value = "Standalone";
            editor.Activate();

            var input = (InputElement)element.QuerySelector("input");
            assert.NotEqual(null, input, "The editor should render with no DataContext");
            if (input == null) return;
            assert.Equal("Standalone", input.Value,
                "Control.Value should render even when the graph DataContext is null");
            assert.Equal("filled", input.GetAttribute("data-editor-state"),
                "The Control.Value ternary should also render without DataContext");

            editor.Value = "";
            assert.Equal("", input.Value, "Changing Control.Value should update the input");
            assert.Equal("empty", input.GetAttribute("data-editor-state"),
                "The ternary should update when the Control property changes");

            input.Value = "Typed locally";
            var change = Window.Instance.Document.CreateEvent("Event");
            change.InitEvent("change", true, true);
            input.DispatchEvent(change);
            assert.Equal("Typed locally", editor.Value,
                "The Control.Commit event should run without DataContext");
        }
    }
}
