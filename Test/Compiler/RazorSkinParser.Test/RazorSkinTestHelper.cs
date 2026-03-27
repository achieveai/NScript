using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Helper class for Razor skin template snapshot tests.
    /// Mirrors the XWML Helper pattern: load template -> compile -> compare against expected JS.
    /// </summary>
    public static class RazorSkinTestHelper
    {
        // Framework stubs needed for Roslyn analysis to classify observable vs non-observable.
        public const string FrameworkStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
    public interface IObservableCollection { }
    public class ObservableCollection<T> : ObservableObject, IObservableCollection
    {
        public void Add(T item) { }
        public void Remove(T item) { }
    }
}";

        // Observable ViewModel used by most tests.
        public const string TestVMSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
    public string AppVersion { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public bool IsActive { get; set; }
    public bool IsStatic { get; set; }
    public string CssClass { get; set; }
    public ObservableCollection<ItemVM> Items { get; set; }
    public System.Action OnSubmit { get; set; }
    public void Cancel() { }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
public class ItemVM : ObservableObject
{
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}";

        // Plain (non-observable) ViewModel for OneTime binding tests.
        public const string PlainVMSource = @"
public class PlainVM
{
    public string AppVersion { get; set; }
    public bool IsStatic { get; set; }
}";

        // Control type stub for @Control.* bindings.
        public const string ControlStub = @"
public class MyControl
{
    public string CssClass { get; set; }
}";

        /// <summary>
        /// Reads a .skin.cshtml template from the Templates/ output directory.
        /// </summary>
        public static string ReadTemplate(string templateName)
        {
            var dir = Path.GetDirectoryName(typeof(RazorSkinTestHelper).Assembly.Location);
            var path = Path.Combine(dir, "Templates", templateName + ".skin.cshtml");
            if (!File.Exists(path))
                throw new FileNotFoundException($"Template not found: {path}");
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Compiles a template by name using the full RazorSkinCompiler pipeline.
        /// Uses appropriate ViewModel stubs based on the @model directive in the template.
        /// </summary>
        public static string CompileTemplate(string templateName)
        {
            var templateSource = ReadTemplate(templateName);

            // Determine which VM source to use based on @model directive
            string[] additionalSources;
            if (templateSource.Contains("@model PlainVM"))
            {
                additionalSources = new[] { FrameworkStubs, PlainVMSource };
            }
            else
            {
                additionalSources = new[] { FrameworkStubs, TestVMSource, ControlStub };
            }

            return RazorSkinCompiler.Compile(
                templateName, templateSource, additionalSources);
        }

        /// <summary>
        /// Reads expected JS from the ExpectedOutput/ directory and compares
        /// it to the actual JS using exact string match (trimmed).
        /// </summary>
        public static void CheckCode(string testName, string actualJs)
        {
            var dir = Path.GetDirectoryName(typeof(RazorSkinTestHelper).Assembly.Location);
            var expectedPath = Path.Combine(dir, "ExpectedOutput", testName + ".js");

            if (!File.Exists(expectedPath))
            {
                // If expected file does not exist, write the actual output and fail
                // so the developer can inspect and approve it.
                File.WriteAllText(expectedPath, actualJs);
                Assert.Fail(
                    $"Expected output file not found. Actual output written to: {expectedPath}\n" +
                    $"Inspect and approve the output, then re-run the test.");
                return;
            }

            var expected = File.ReadAllText(expectedPath).Trim();
            var actual = actualJs.Trim();

            if (expected != actual)
            {
                Console.Error.WriteLine("====== Expected ================================> ");
                Console.Error.WriteLine(expected);
                Console.Error.WriteLine("====== Actual ==================================> ");
                Console.Error.WriteLine(actual);
            }

            Assert.AreEqual(expected, actual,
                $"Snapshot mismatch for '{testName}'. " +
                $"If the change is intentional, delete ExpectedOutput/{testName}.js and re-run to regenerate.");
        }

        /// <summary>
        /// Convenience: compile template and check against expected output in one call.
        /// </summary>
        public static void CompileAndCheck(string templateName)
        {
            var actualJs = CompileTemplate(templateName);
            CheckCode(templateName, actualJs);
        }

        /// <summary>
        /// Generates and writes expected output for a template.
        /// Used during test setup to create snapshot baselines.
        /// </summary>
        public static void GenerateExpectedOutput(string templateName)
        {
            var actualJs = CompileTemplate(templateName);
            var dir = Path.GetDirectoryName(typeof(RazorSkinTestHelper).Assembly.Location);
            var outputPath = Path.Combine(dir, "ExpectedOutput", templateName + ".js");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            File.WriteAllText(outputPath, actualJs);
        }
    }
}
