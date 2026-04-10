namespace TodoApp
{
    using System.Runtime.CompilerServices;
    using System.Web.Html;
    using Sunlight.Framework;
    using Sunlight.Framework.UI;
    using TodoApp.ViewModels;
    using TodoApp.Services;
    using TodoApp.Skins;

    /// <summary>
    /// Application entry point for the Microsoft To Do clone SPA.
    /// Boot sequence: open IndexedDB -> load persisted data -> activate UI.
    /// </summary>
    public class Program
    {
        [EntryPoint]
        public static void Main()
        {
            TaskScheduler.Instance = new TaskScheduler(new WindowTimer(), 10, 10);

            var dbService = new IndexedDbService();
            var dataService = new TodoDataService(dbService);

            dataService.Initialize().Then<bool>(delegate(bool ok)
            {
                BootUI(dataService);
                return true;
            });
        }

        /// <summary>
        /// Creates and activates the UI shell after the database is ready.
        /// </summary>
        private static void BootUI(TodoDataService dataService)
        {
            var doc = Window.Instance.Document;
            var appElement = doc.GetElementById("app");
            var shell = new UISkinableElement(appElement);

            var vm = new AppViewModel();
            vm.DataService = dataService;
            vm.InitializeWithData();

            shell.DataContext = vm;
            shell.Skin = TodoAppSkins.AppShell;
            shell.Activate();
        }
    }
}
