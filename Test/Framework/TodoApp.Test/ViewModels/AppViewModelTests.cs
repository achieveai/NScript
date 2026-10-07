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

            var tasksFolder = vm.Folders[3];
            vm.OnSelectFolder(tasksFolder);

            // Add a todo so the test is self-contained (no sample-data dependency)
            vm.AddTodo();
            var todo = vm.CurrentTodos[0];
            vm.OnSelectTodo(todo);

            assert.Equal(todo, vm.SelectedTodo, "Selected todo should be set");
            assert.Equal(false, vm.IsRightPaneCollapsed, "Detail pane should open");
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

        [Test]
        public static void TestAddTodoWithTitle(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();

            var tasksFolder = vm.Folders[3];
            vm.OnSelectFolder(tasksFolder);
            int initialCount = vm.CurrentTodos.Count;

            vm.AddTodoWithTitle("Buy groceries");

            assert.Equal(initialCount + 1, vm.CurrentTodos.Count, "Todo count should increase by 1");
            // The newly added todo is appended before RefreshCurrentTodos reorders,
            // so search the list for the title we just added.
            bool found = false;
            for (int i = 0; i < vm.CurrentTodos.Count; i++)
            {
                if (vm.CurrentTodos[i].Title == "Buy groceries")
                {
                    found = true;
                }
            }

            assert.IsTrue(found, "New todo with custom title should appear in CurrentTodos");
        }

        [Test]
        public static void TestDeleteSelectedTodo(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();

            var tasksFolder = vm.Folders[3];
            vm.OnSelectFolder(tasksFolder);

            // Add a dedicated todo so the test is self-contained
            vm.AddTodo();
            int countAfterAdd = vm.CurrentTodos.Count;

            // Select the last todo (the one we just added)
            var todo = vm.CurrentTodos[countAfterAdd - 1];
            vm.OnSelectTodo(todo);

            vm.DeleteSelectedTodo();

            assert.Equal(countAfterAdd - 1, vm.CurrentTodos.Count, "Todo count should decrease by 1");
            assert.Equal(null, vm.SelectedTodo, "Selected todo should be null after delete");
            assert.Equal(true, vm.IsRightPaneCollapsed, "Detail pane should close after delete");
        }
    

        [Test]
        public static void TestCompleteAllMovesEveryPendingTodo(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();
            vm.OnSelectFolder(vm.Folders[3]);

            int pendingBefore = vm.CurrentTodos.Count;
            int doneBefore = vm.CompletedCurrentTodos.Count;
            assert.IsTrue(pendingBefore > 0, "Sample data has pending todos");

            vm.CompleteAll();

            assert.Equal(0, vm.CurrentTodos.Count, "No pending todos remain");
            assert.Equal(pendingBefore + doneBefore, vm.CompletedCurrentTodos.Count, "All todos are in the completed section");
            assert.Equal(pendingBefore + doneBefore, vm.CompletedCount, "CompletedCount follows the section");
            assert.Equal((pendingBefore + doneBefore).ToString() + " of " + (pendingBefore + doneBefore).ToString() + " done",
                vm.ProgressText, "Progress text shows everything done");
            assert.Equal(pendingBefore + doneBefore, vm.Folders[4].TodoCount, "Completed folder count updated");
        }

        [Test]
        public static void TestReopenAllRestoresPendingTodos(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();
            vm.OnSelectFolder(vm.Folders[3]);

            int total = vm.CurrentTodos.Count + vm.CompletedCurrentTodos.Count;
            vm.CompleteAll();
            vm.ReopenAll();

            assert.Equal(total, vm.CurrentTodos.Count, "Every todo is pending again");
            assert.Equal(0, vm.CompletedCurrentTodos.Count, "Completed section is empty");
            assert.Equal("0 of " + total.ToString() + " done", vm.ProgressText, "Progress text shows nothing done");
            assert.Equal(0, vm.Folders[4].TodoCount, "Completed folder count is zero");
        }

        [Test]
        public static void TestProgressTextTracksSingleToggle(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();
            vm.OnSelectFolder(vm.Folders[3]);

            int total = vm.CurrentTodos.Count + vm.CompletedCurrentTodos.Count;
            int doneBefore = vm.CompletedCurrentTodos.Count;
            var todo = vm.CurrentTodos[0];

            todo.ToggleComplete(null, null);
            assert.Equal((doneBefore + 1).ToString() + " of " + total.ToString() + " done", vm.ProgressText,
                "Completing one todo advances the progress text");

            todo.ToggleComplete(null, null);
            assert.Equal(doneBefore.ToString() + " of " + total.ToString() + " done", vm.ProgressText,
                "Reopening it restores the progress text");
        }

        [Test]
        public static void TestSingleToggleUpdatesFolderCounts(Assert assert)
        {
            var vm = new AppViewModel();
            vm.InitializeWithData();
            vm.OnSelectFolder(vm.Folders[3]);

            var important = vm.Folders[1];
            var completed = vm.Folders[4];
            int importantBefore = important.TodoCount;
            int completedBefore = completed.TodoCount;

            TodoItemViewModel todo = null;
            for (int i = 0; i < vm.CurrentTodos.Count; i++)
            {
                if (!vm.CurrentTodos[i].IsImportant) { todo = vm.CurrentTodos[i]; break; }
            }
            assert.IsTrue(todo != null, "Sample data has a pending, non-important todo");

            todo.ToggleImportant(null, null);
            assert.Equal(importantBefore + 1, important.TodoCount, "Starring one todo bumps the Important count");

            todo.ToggleComplete(null, null);
            assert.Equal(completedBefore + 1, completed.TodoCount, "Completing one todo bumps the Completed count");

            todo.ToggleComplete(null, null);
            todo.ToggleImportant(null, null);
            assert.Equal(importantBefore, important.TodoCount, "Un-starring restores the Important count");
            assert.Equal(completedBefore, completed.TodoCount, "Reopening restores the Completed count");
        }
    }
}
