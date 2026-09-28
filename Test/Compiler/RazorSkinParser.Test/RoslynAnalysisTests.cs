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

        private static System.Collections.Generic.IEnumerable<T> FindNodes<T>(IRNode root) where T : IRNode
        {
            if (root is T match) yield return match;
            foreach (var child in root.Children)
                foreach (var found in FindNodes<T>(child))
                    yield return found;
            if (root is ConditionalNode cond)
            {
                foreach (var child in cond.TrueBranch)
                    foreach (var found in FindNodes<T>(child))
                        yield return found;
                foreach (var child in cond.FalseBranch)
                    foreach (var found in FindNodes<T>(child))
                        yield return found;
            }
            else if (root is LoopNode loop)
            {
                foreach (var child in loop.ItemTemplate)
                    foreach (var found in FindNodes<T>(child))
                        yield return found;
            }
        }

        // --- LIMIT-006: Sub-control property binding analysis ---

        [TestMethod]
        public void SubControlBinding_PromotedToOneWay_WhenSourceIsObservable()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string SearchQuery { get; set; }
}";
            var template = @"
@model TestVM
<div><SearchBox Query=""Model.SearchQuery"" /></div>";

            var ir = BuildAndAnalyze(template, vmSource);

            // Recursively find SubControlNode in the tree
            var subControl = FindNodes<SubControlNode>(ir).FirstOrDefault();

            subControl.Should().NotBeNull("template should contain a SubControlNode");
            subControl.PropertyBindings.Should().HaveCount(1);
            subControl.PropertyBindings[0].Classification.Mode.Should().Be(BindingMode.OneWay,
                "observable property binding should be promoted to OneWay");
            subControl.PropertyBindings[0].Classification.Dependencies.Should().HaveCount(1);
            subControl.PropertyBindings[0].Classification.Dependencies[0].PropertyName.Should().Be("SearchQuery");
        }

        [TestMethod]
        public void SubControlBinding_StaysOneTime_WhenSourceIsNotObservable()
        {
            var vmSource = @"
public class PlainVM
{
    public string SearchQuery { get; set; }
}";
            var template = @"
@model PlainVM
<div><SearchBox Query=""Model.SearchQuery"" /></div>";

            var ir = BuildAndAnalyze(template, vmSource);

            var subControl = FindNodes<SubControlNode>(ir).FirstOrDefault();

            subControl.Should().NotBeNull();
            subControl.PropertyBindings.Should().HaveCount(1);
            subControl.PropertyBindings[0].Classification.Mode.Should().Be(BindingMode.OneTime,
                "non-observable property binding should stay OneTime");
        }

        [TestMethod]
        public void ForeachSubControlRecognizesParentAndItemObservableSources()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
    public ObservableCollection<ItemVM> Items { get; set; }
}
public class ItemVM : ObservableObject
{
    public string Name { get; set; }
}";
            var template = @"
@model TestVM
@foreach (var item in Model.Items)
{
    <SearchBox ParentName=""@Model.Name"" ItemName=""@item.Name"" />
}";

            var ir = BuildAndAnalyze(template, vmSource);
            var bindings = FindNodes<SubControlNode>(ir).Single().PropertyBindings;

            bindings.Should().HaveCount(2);
            bindings.Should().OnlyContain(binding =>
                binding.Classification.Mode == BindingMode.OneWay
                && binding.Classification.Dependencies.Single().PropertyName == "Name");
            var topology = NScript.RazorSkin.CodeGen.GraphTopologyBuilder.Build(ir)
                .Collections.Single().ItemTopology;
            topology.Subscriptions.Should().Contain(entry =>
                entry.PropertyName == "Name" && entry.SourceSlot == 0);
            topology.Subscriptions.Should().Contain(entry =>
                entry.PropertyName == "Name" && entry.SourceSlot == 2);
        }

        [TestMethod]
        public void ForeachSubControlMixedExpressionRetainsBothSourceIdentities()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
    public ObservableCollection<ItemVM> Items { get; set; }
}
public class ItemVM : ObservableObject
{
    public string Name { get; set; }
}";
            var template = @"
@model TestVM
@foreach (var item in Model.Items)
{
    <SearchBox Text=""@(Model.Name + item.Name)"" />
}";

            var ir = BuildAndAnalyze(template, vmSource);
            var binding = FindNodes<SubControlNode>(ir).Single().PropertyBindings.Single();
            binding.Classification.Dependencies.Should().HaveCount(2);
            var topology = NScript.RazorSkin.CodeGen.GraphTopologyBuilder.Build(ir)
                .Collections.Single().ItemTopology;
            topology.Subscriptions.Should().Contain(entry =>
                entry.PropertyName == "Name" && entry.SourceSlot == 0);
            topology.Subscriptions.Should().Contain(entry =>
                entry.PropertyName == "Name" && entry.SourceSlot == 2);
            topology.GetterExpressions.Should().Contain("Model.Name").And.Contain("item.Name");
        }

        // --- LIMIT-001: Getter-only observable property classification ---

        [TestMethod]
        public void GetterOnlyObservableProperty_PromotedToOneWay()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
    public bool HasName => !string.IsNullOrEmpty(Name);
}";
            var template = @"
@model TestVM
<span>@Model.HasName</span>";

            var ir = BuildAndAnalyze(template, vmSource);

            var binding = ir.Children.OfType<ExpressionBindingNode>()
                .Concat(ir.Children.SelectMany(n => n.Children.OfType<ExpressionBindingNode>()))
                .FirstOrDefault(b => b.Classification.CSharpExpression.Contains("HasName"));

            binding.Should().NotBeNull("template should contain a HasName binding");
            binding.Classification.Mode.Should().Be(BindingMode.OneWay,
                "getter-only property on observable type should be promoted to OneWay (LIMIT-001)");
        }
    }
}
