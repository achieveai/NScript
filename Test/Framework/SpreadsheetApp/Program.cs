namespace SpreadsheetApp
{
    using System.Runtime.CompilerServices;
    using System.Web.Html;
    using Sunlight.Framework;
    using Sunlight.Framework.UI;
    using Sunlight.Framework.UI.Helpers.BindingGraph;
    using SpreadsheetApp.Services;
    using SpreadsheetApp.Skins;
    using SpreadsheetApp.ViewModels;

    /// <summary>
    /// Spreadsheet demo: formula cells with several inputs and long dependency
    /// chains, plus a built-in benchmark that runs the same edits with the
    /// batched binding flush off and on.
    /// </summary>
    public class Program
    {
        [EntryPoint]
        public static void Main()
        {
            TaskScheduler.Instance = new TaskScheduler(new WindowTimer(), 10, 10);
            GraphFlushCoordinator.BatchingEnabled = true;

            var doc = Window.Instance.Document;
            var appElement = doc.GetElementById("app");
            var shell = new UISkinableElement(appElement);

            var vm = new SheetViewModel(appElement);
            vm.Build(50);

            shell.DataContext = vm;
            shell.Skin = SpreadsheetSkins.Sheet;
            shell.Activate();

            Perf.Expose(vm.RunBenchmark, vm.IsBenchmarkRunning, vm.ResultsJson, vm.SetBatching, vm.RunOp);
        }
    }
}
