using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using NScript.CLR;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// The Roslyn analysis phase sees the model only through C# stubs generated from Cecil
    /// metadata. A chained loop source (@foreach over Model.SelectedTodo.SubTasks) can only type
    /// its loop variable when the type of SelectedTodo is stubbed too, so the generator must
    /// follow observable property types transitively, not just the model's own collection
    /// element types. Without that the item bindings silently stay OneTime (the subtask row never
    /// updated when a subtask was toggled in the TodoApp).
    /// </summary>
    [TestClass]
    public class CecilModelStubGeneratorTests
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

        private static CecilModelStubGenerator _generator;

        [ClassInitialize]
        public static void LoadProbeAssembly(TestContext _)
        {
            var module = ModuleDefinition.CreateModule("StubProbe", ModuleKind.Dll);
            // ClrContext skips assemblies that were not produced by the NScript compiler.
            module.Resources.Add(new EmbeddedResource("$$BstInfo$$", ManifestResourceAttributes.Public, new byte[0]));

            // The observable base and collection live in the probe so Cecil base-type walks resolve.
            var observable = AddClass(module, "Sunlight.Framework.Observables", "ObservableObject", module.TypeSystem.Object);
            var collection = new TypeDefinition("Sunlight.Framework.Observables", "ObservableCollection`1",
                TypeAttributes.Public, observable);
            collection.GenericParameters.Add(new GenericParameter("T", collection));
            module.Types.Add(collection);

            var subTask = AddClass(module, "Probe", "SubTaskVM", observable);
            AddProperty(subTask, "IsCompleted", module.TypeSystem.Boolean);

            var todo = AddClass(module, "Probe", "TodoVM", observable);
            AddProperty(todo, "Title", module.TypeSystem.String);
            AddProperty(todo, "SubTasks", Collection(collection, subTask));

            var service = AddClass(module, "Probe", "DataService", module.TypeSystem.Object);
            AddProperty(service, "Secret", module.TypeSystem.String);

            var model = AddClass(module, "Probe", "AppVM", observable);
            AddProperty(model, "SelectedTodo", todo);
            AddProperty(model, "Service", service);
            AddProperty(model, "Self", model);

            // Cecil keeps the file open for the life of the module, so it is left in %TEMP%.
            var directory = Path.Combine(Path.GetTempPath(), "nscript-stub-probe-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "StubProbe.dll");
            module.Write(path);

            var clrContext = new ClrContext();
            clrContext.LoadAssembly(path);
            _generator = new CecilModelStubGenerator(clrContext);
        }

        [TestMethod]
        public void StubsObservablePropertyTypesTransitively()
        {
            var stub = _generator.GenerateModelTypeStub("@model Probe.AppVM\n<div></div>");

            stub.Should().Contain("public class AppVM : Sunlight.Framework.Observables.ObservableObject");
            stub.Should().Contain("public Probe.TodoVM SelectedTodo { get; set; }");
            // SelectedTodo's type is reached through a plain (non-collection) property …
            stub.Should().Contain("public class TodoVM : Sunlight.Framework.Observables.ObservableObject");
            stub.Should().Contain("public Sunlight.Framework.Observables.ObservableCollection<Probe.SubTaskVM> SubTasks { get; set; }");
            // … and its collection element type is reached through it.
            stub.Should().Contain("public class SubTaskVM : Sunlight.Framework.Observables.ObservableObject");
            stub.Should().Contain("public bool IsCompleted { get; set; }");
        }

        [TestMethod]
        public void DoesNotStubNonObservablePropertyTypesOrRepeatTypes()
        {
            var stub = _generator.GenerateModelTypeStub("@model Probe.AppVM\n<div></div>");

            stub.Should().NotContain("class DataService", "a non-observable object-typed property is not followed");
            CountOf(stub, "public class AppVM ").Should().Be(1, "a self-referencing property must not re-stub the model");
            CountOf(stub, "public class TodoVM ").Should().Be(1);
        }

        [TestMethod]
        public void ChainedLoopVariableIsTypedThroughTheGeneratedStubs()
        {
            // End to end through the Roslyn phase: the exact TodoApp shape.
            var template = "@model Probe.AppVM\n@foreach (var sub in Model.SelectedTodo.SubTasks)\n{\n    <div class=\"@(sub.IsCompleted ? \"done\" : \"open\")\">x</div>\n}";
            var stub = _generator.GenerateModelTypeStub(template);

            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("Probe", preprocessed.CleanedTemplate);
            var ir = TemplateIRBuilder.Build("Probe", preprocessed, parsed);
            RoslynAnalysisPhase.RefineClassifications(ir, parsed.GeneratedCSharp, new[] { FrameworkStubs, stub });

            var loop = ir.Children.OfType<LoopNode>().Single();
            loop.IsObservableCollection.Should().BeTrue();
            var binding = loop.ItemTemplate.OfType<ExpressionBindingNode>()
                .Concat(loop.ItemTemplate.SelectMany(n => n.Children.OfType<ExpressionBindingNode>()))
                .Single();
            binding.Classification.Mode.Should().Be(BindingMode.OneWay,
                "sub is typed as SubTaskVM, whose IsCompleted is observable");
            binding.Classification.Dependencies.Single().PropertyName.Should().Be("IsCompleted");
        }

        private static int CountOf(string text, string needle)
            => (text.Length - text.Replace(needle, "").Length) / needle.Length;

        private static TypeDefinition AddClass(ModuleDefinition module, string ns, string name, TypeReference baseType)
        {
            var type = new TypeDefinition(ns, name, TypeAttributes.Public, baseType);
            module.Types.Add(type);
            return type;
        }

        private static void AddProperty(TypeDefinition owner, string name, TypeReference propertyType)
        {
            var getter = new MethodDefinition("get_" + name,
                MethodAttributes.Public | MethodAttributes.SpecialName, propertyType);
            var setter = new MethodDefinition("set_" + name,
                MethodAttributes.Public | MethodAttributes.SpecialName, owner.Module.TypeSystem.Void);
            setter.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, propertyType));
            owner.Methods.Add(getter);
            owner.Methods.Add(setter);
            owner.Properties.Add(new PropertyDefinition(name, PropertyAttributes.None, propertyType)
            {
                GetMethod = getter, SetMethod = setter
            });
        }

        private static GenericInstanceType Collection(TypeDefinition collection, TypeReference element)
        {
            var instance = new GenericInstanceType(collection);
            instance.GenericArguments.Add(element);
            return instance;
        }
    }
}
