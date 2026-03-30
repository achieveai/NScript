namespace TodoApp.Test.ViewModels
{
    using SunlightUnit;
    using Sunlight.Framework;
    using TodoApp.ViewModels;

    /// <summary>
    /// Unit tests for FolderViewModel — covers creation, selection state, CSS class
    /// computation, and todo count tracking.
    /// </summary>
    [TestFixture]
    public class FolderViewModelTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(new TestWindowTimer(), 10, 10);
        }

        [Test]
        public static void TestFolderCreation(Assert assert)
        {
            var appVm = new AppViewModel();
            var folder = new FolderViewModel(appVm);
            folder.Name = "Work";
            folder.Icon = "\uD83D\uDCCB";
            assert.Equal("Work", folder.Name, "Name should be set");
            assert.Equal("\uD83D\uDCCB", folder.Icon, "Icon should be set");
        }

        [Test]
        public static void TestFolderCssClassReflectsSelection(Assert assert)
        {
            var appVm = new AppViewModel();
            var folder = new FolderViewModel(appVm);
            assert.Equal("folder-item", folder.CssClass, "Default CSS class");

            folder.IsSelected = true;
            assert.Equal("folder-item selected", folder.CssClass, "Selected CSS class");
        }

        [Test]
        public static void TestFolderTodoCount(Assert assert)
        {
            var appVm = new AppViewModel();
            var folder = new FolderViewModel(appVm);
            folder.TodoCount = 0;
            assert.Equal(0, folder.TodoCount, "Initial count");

            folder.TodoCount = 5;
            assert.Equal(5, folder.TodoCount, "Updated count");
        }
    }
}
