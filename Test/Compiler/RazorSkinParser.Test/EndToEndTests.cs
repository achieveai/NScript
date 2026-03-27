using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class EndToEndTests
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

        private const string TestVMSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public bool IsActive { get; set; }
    public ObservableCollection<ItemVM> Items { get; set; }
}
public class ItemVM : ObservableObject
{
    public string Name { get; set; }
}";

        [TestMethod]
        public void SimpleTextBindingProducesValidJs()
        {
            var template = "@model TestVM\n\n<div>@Model.Name</div>";
            var js = RazorSkinCompiler.Compile(
                "TextBinding", template,
                new[] { FrameworkStubs, TestVMSource });

            js.Should().Contain("TextBinding_factory");
            js.Should().Contain("function TextBinding()");
            js.Should().Contain("get_name");
            js.Should().Contain("\"Name\"");
            js.Should().Contain("SkinInstance");
        }

        [TestMethod]
        public void ComputedExpressionBindsMultipleProperties()
        {
            var template = "@model TestVM\n\n<span>@(Model.Price * Model.Quantity)</span>";
            var js = RazorSkinCompiler.Compile(
                "ComputedExpr", template,
                new[] { FrameworkStubs, TestVMSource });

            js.Should().Contain("get_price");
            js.Should().Contain("get_quantity");
            js.Should().Contain("\"Price\"");
            js.Should().Contain("\"Quantity\"");
        }

        [TestMethod]
        public void FullPipelineProducesNonEmptyJs()
        {
            var template = "@model TestVM\n@control UISkinableElement\n\n<div>\n    <h1>@Model.Name</h1>\n    <span>@(Model.Price * Model.Quantity)</span>\n</div>";
            var js = RazorSkinCompiler.Compile(
                "OrderSkin", template,
                new[] { FrameworkStubs, TestVMSource });

            js.Should().NotBeNullOrEmpty();
            js.Should().Contain("OrderSkin_factory");
            js.Should().Contain("OrderSkin_var");
            js.Should().Contain("function OrderSkin()");
        }
    }
}
