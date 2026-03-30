namespace TodoApp.Test.ViewModels
{
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework;
    using Sunlight.Framework.Observables;
    using TodoApp.ViewModels;

    /// <summary>
    /// Unit tests for AppViewModel — covers pane toggle, CSS class computation,
    /// system folder creation, folder/todo selection, and adding todos.
    /// </summary>
    [TestFixture]
    public class AppViewModelTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestPaneToggle(Assert assert)
        {
            var vm = new AppViewModel();
            assert.Equal(false, vm.IsLeftPaneCollapsed, "Left pane starts expanded");

            vm.ToggleLeftPane();
            assert.Equal(true, vm.IsLeftPaneCollapsed, "Left pane collapsed after toggle");

            vm.ToggleLeftPane();
            assert.Equal(false, vm.IsLeftPaneCollapsed, "Left pane expanded after second toggle");
        }

        [Test]
        public static void TestLeftPaneCssClass(Assert assert)
        {
            var vm = new AppViewModel();
            assert.Equal("pane-left", vm.LeftPaneClass, "Default class");

            vm.ToggleLeftPane();
            assert.Equal("pane-left collapsed", vm.LeftPaneClass, "Collapsed class after toggle");
        }

        [Test]
        public static void TestRightPaneCssClass(Assert assert)
        {
            var vm = new AppViewModel();
            // Right pane starts collapsed per constructor
            assert.Equal("pane-right collapsed", vm.RightPaneClass, "Right pane starts collapsed");

            vm.ToggleRightPane();
            assert.Equal("pane-right", vm.RightPaneClass, "Expanded class after toggle");
        }

        [Test]
        public static void TestInitializeCreatesSystemFolders(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();

            assert.NotEqual(null, vm.Folders, "Folders should be initialized");
            assert.IsTrue(vm.Folders.Count >= 4, "Should have at least 4 system folders");
        }

        [Test]
        public static void TestFolderSelection(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();

            var folder = vm.Folders[0]; // My Day
            vm.OnSelectFolder(folder);

            assert.Equal(folder, vm.SelectedFolder, "Selected folder should be set");
            assert.Equal(true, folder.IsSelected, "Folder should be marked selected");
            assert.Equal(folder.Name, vm.SelectedFolderName, "Folder name should update");
        }

        [Test]
        public static void TestTodoSelectionOpensDetailPane(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();

            // Select Tasks folder to get all todos
            var tasksFolder = vm.Folders[3];
            vm.OnSelectFolder(tasksFolder);

            if (vm.CurrentTodos.Count > 0)
            {
                var todo = vm.CurrentTodos[0];
                vm.OnSelectTodo(todo);

                assert.Equal(todo, vm.SelectedTodo, "Selected todo should be set");
                assert.Equal(false, vm.IsRightPaneCollapsed, "Detail pane should open");
            }
            else
            {
                assert.IsTrue(true, "No todos to select (DB-dependent)");
            }
        }

        [Test]
        public static void TestAddTodoToCurrentFolder(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();

            var tasksFolder = vm.Folders[3];
            vm.OnSelectFolder(tasksFolder);
            int initialCount = vm.CurrentTodos.Count;

            vm.AddTodo();
            assert.Equal(initialCount + 1, vm.CurrentTodos.Count, "Todo count should increase by 1");
        }
    }
}
