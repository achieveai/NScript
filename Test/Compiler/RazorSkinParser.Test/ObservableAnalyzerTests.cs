using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class ObservableAnalyzerTests
    {
        private static CSharpCompilation CreateCompilationWithTypes(string source)
        {
            // Minimal type stubs for testing
            var frameworkStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
    public interface IObservableCollection { }
    public class ObservableCollection<T> : ObservableObject, IObservableCollection { }
}
";
            var tree1 = CSharpSyntaxTree.ParseText(frameworkStubs);
            var tree2 = CSharpSyntaxTree.ParseText(source);

            return CSharpCompilation.Create("TestAssembly",
                new[] { tree1, tree2 },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        [TestMethod]
        public void DetectsObservablePropertyOnObservableObject()
        {
            var source = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("TestVM");
            var prop = type.GetMembers("Name")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableProperty(prop).Should().BeTrue();
        }

        [TestMethod]
        public void DetectsNonObservablePropertyOnPlainClass()
        {
            var source = @"
public class PlainObj
{
    public string AppVersion { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("PlainObj");
            var prop = type.GetMembers("AppVersion")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableProperty(prop).Should().BeFalse();
        }

        [TestMethod]
        public void DetectsObservableCollection()
        {
            var source = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public ObservableCollection<string> Items { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("TestVM");
            var prop = type.GetMembers("Items")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableCollection(prop.Type).Should().BeTrue();
        }

        [TestMethod]
        public void DetectsNonObservableList()
        {
            var source = @"
using System.Collections.Generic;
public class TestVM
{
    public List<string> Items { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("TestVM");
            var prop = type.GetMembers("Items")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableCollection(prop.Type).Should().BeFalse();
        }
    }
}
