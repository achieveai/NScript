using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;
using System.Linq;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class RoslynAnalysisTests
    {
        private const string FrameworkStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
    public interface IObservableCollection { }
    public class ObservableCollection<T> : ObservableObject, IObservableCollection { }
}";

        [TestMethod]
        public void ClassifiesObservablePropertyAsOneWay()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
}";
            var template = "@model TestVM\n\n<div>@Model.Name</div>";
            var ir = BuildAndAnalyze(template, vmSource);

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Classification.Mode.Should().Be(BindingMode.OneWay);
        }

        [TestMethod]
        public void ClassifiesNonObservablePropertyAsOneTime()
        {
            var vmSource = @"
public class TestVM
{
    public string AppVersion { get; set; }
}";
            var template = "@model TestVM\n\n<div>@Model.AppVersion</div>";
            var ir = BuildAndAnalyze(template, vmSource);

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Classification.Mode.Should().Be(BindingMode.OneTime);
        }

        [TestMethod]
        public void DetectsObservableCollectionInForeach()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public ObservableCollection<string> Items { get; set; }
}";
            var template = "@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item</li>\n}";
            var ir = BuildAndAnalyze(template, vmSource);

            var loop = ir.Children.OfType<LoopNode>().First();
            loop.IsObservableCollection.Should().BeTrue();
        }

        private SkinTemplateNode BuildAndAnalyze(string template, string vmSource)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);
            var ir = TemplateIRBuilder.Build("TestSkin", preprocessed, parsed);

            RoslynAnalysisPhase.RefineClassifications(
                ir,
                parsed.GeneratedCSharp,
                new[] { FrameworkStubs, vmSource });

            return ir;
        }
    }
}
