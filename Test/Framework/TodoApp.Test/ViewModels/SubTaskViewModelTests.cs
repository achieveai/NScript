namespace TodoApp.Test.ViewModels
{
    using SunlightUnit;
    using Sunlight.Framework;
    using TodoApp.ViewModels;

    /// <summary>
    /// Unit tests for SubTaskViewModel — covers creation, toggle, and CSS class computation.
    /// </summary>
    [TestFixture]
    public class SubTaskViewModelTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestSubTaskCreation(Assert assert)
        {
            var sub = new SubTaskViewModel();
            sub.Title = "Step 1";
            assert.Equal("Step 1", sub.Title, "Title should be set");
            assert.Equal(false, sub.IsCompleted, "Should not be completed by default");
        }

        [Test]
        public static void TestSubTaskToggle(Assert assert)
        {
            var sub = new SubTaskViewModel();
            sub.ToggleComplete();
            assert.Equal(true, sub.IsCompleted, "Should be completed after toggle");
        }

        [Test]
        public static void TestSubTaskCssClass(Assert assert)
        {
            var sub = new SubTaskViewModel();
            assert.Equal("subtask-item", sub.CssClass, "Default class");

            sub.IsCompleted = true;
            assert.Equal("subtask-item completed", sub.CssClass, "Completed class");
        }
    }
}
