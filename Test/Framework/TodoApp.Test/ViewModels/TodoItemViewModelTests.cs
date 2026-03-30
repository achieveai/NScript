namespace TodoApp.Test.ViewModels
{
    using SunlightUnit;
    using System.Web.Html;
    using Sunlight.Framework;
    using Sunlight.Framework.Observables;
    using TodoApp.ViewModels;

    /// <summary>
    /// Unit tests for TodoItemViewModel — covers creation, toggle methods, computed
    /// properties, subtask management, ID generation, and property change notifications.
    /// </summary>
    [TestFixture]
    public class TodoItemViewModelTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestTodoCreation(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.Title = "Test task";
            assert.Equal("Test task", todo.Title, "Title should be set");
            assert.Equal(false, todo.IsCompleted, "Should not be completed by default");
            assert.Equal(false, todo.IsImportant, "Should not be important by default");
        }

        [Test]
        public static void TestCompletionToggle(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.IsCompleted = false;

            todo.ToggleComplete();
            assert.Equal(true, todo.IsCompleted, "Should be completed after toggle");

            todo.ToggleComplete();
            assert.Equal(false, todo.IsCompleted, "Should be uncompleted after second toggle");
        }

        [Test]
        public static void TestImportanceToggle(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.IsImportant = false;

            todo.ToggleImportant();
            assert.Equal(true, todo.IsImportant, "Should be important after toggle");
        }

        [Test]
        public static void TestMyDayToggle(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.IsMyDay = false;

            todo.ToggleMyDay();
            assert.Equal(true, todo.IsMyDay, "Should be in My Day after toggle");
        }

        [Test]
        public static void TestCssClassReflectsCompletion(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.IsCompleted = false;
            assert.Equal("todo-item", todo.CssClass, "Default CSS class");

            todo.IsCompleted = true;
            assert.Equal("todo-item completed", todo.CssClass, "Completed CSS class");
        }

        [Test]
        public static void TestCheckboxClassReflectsCompletion(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.IsCompleted = false;
            assert.Equal("btn-check", todo.CheckboxClass, "Unchecked class");

            todo.IsCompleted = true;
            assert.Equal("btn-check checked", todo.CheckboxClass, "Checked class");
        }

        [Test]
        public static void TestHasDueDateComputed(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.DueDate = "";
            assert.Equal(false, todo.HasDueDate, "Empty string = no due date");

            todo.DueDate = "2026-04-01";
            assert.Equal(true, todo.HasDueDate, "Non-empty = has due date");
        }

        [Test]
        public static void TestDueDateDisplayComputed(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            todo.DueDate = "2026-04-01";
            assert.Equal("Due: 2026-04-01", todo.DueDateDisplay, "Should format due date");
        }

        [Test]
        public static void TestAddSubTask(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            assert.Equal(0, todo.SubTasks.Count, "Should start with no subtasks");

            todo.AddSubTask();
            assert.Equal(1, todo.SubTasks.Count, "Should have 1 subtask after add");
            assert.Equal("New step", todo.SubTasks[0].Title, "Subtask should have default title");
        }

        [Test]
        public static void TestGenerateIdIncrementsCorrectly(Assert assert)
        {
            string id1 = TodoItemViewModel.GenerateId();
            string id2 = TodoItemViewModel.GenerateId();
            assert.NotEqual(id1, id2, "Each generated ID should be unique");
        }

        [Test]
        public static void TestPropertyChangeNotification(Assert assert)
        {
            var appVm = new AppViewModel();
            var todo = new TodoItemViewModel(appVm);
            var changed = false;
            string changedProp = "";

            todo.AddPropertyChangedListener("Title", delegate(INotifyPropertyChanged sender, string prop)
            {
                changed = true;
                changedProp = prop;
            });

            todo.Title = "Updated";
            assert.IsTrue(changed, "PropertyChanged should fire for Title");
            assert.Equal("Title", changedProp, "Changed property name should be Title");
        }
    }
}
