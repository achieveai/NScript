using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using NScript.CLR;
using NScript.JST;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;
using static RazorSkinParser.Test.TopologyTestHelpers;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// Issue #102 emitter contracts that need no RuntimeScopeManager: a constant static member
    /// becomes a literal, and unsupported binding paths or event handlers fail the build at the
    /// template location instead of emitting a raw-text function.
    /// Types come from a tiny Cecil assembly loaded into a real ClrContext.
    /// </summary>
    [TestClass]
    public class GraphDescriptorJSTEmitterBindingTests
    {
        private static readonly Location TemplateLocation = new Location("Probe.skin.cshtml", 3, 5);
        private static readonly Location HandlerLocation = new Location("Probe.skin.cshtml", 9, 12);
        private static ClrContext _clrContext;

        [ClassInitialize]
        public static void LoadProbeAssembly(TestContext _)
        {
            var module = ModuleDefinition.CreateModule("EmitterProbe", ModuleKind.Dll);
            // ClrContext skips assemblies that were not produced by the NScript compiler.
            module.Resources.Add(new EmbeddedResource("$$BstInfo$$", ManifestResourceAttributes.Public, new byte[0]));

            var model = new TypeDefinition("Tests", "ProbeModel", TypeAttributes.Public, module.TypeSystem.Object);
            var getter = new MethodDefinition("get_Inner", MethodAttributes.Public | MethodAttributes.SpecialName, model);
            model.Methods.Add(getter);
            model.Properties.Add(new PropertyDefinition("Inner", PropertyAttributes.None, model) { GetMethod = getter });
            module.Types.Add(model);

            var limits = new TypeDefinition("Tests", "Limits",
                TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed, module.TypeSystem.Object);
            limits.Fields.Add(new FieldDefinition("Max",
                FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal, module.TypeSystem.Int32)
            { Constant = 3 });
            module.Types.Add(limits);

            // Cecil keeps the file open for the life of the module, so it is left in %TEMP%.
            var directory = Path.Combine(Path.GetTempPath(), "nscript-emitter-probe-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, "EmitterProbe.dll");
            module.Write(path);

            _clrContext = new ClrContext();
            _clrContext.LoadAssembly(path);
        }

        private static string Emit(GraphTopology topology)
        {
            var descriptor = new GraphDescriptorJSTEmitter(topology, new IdentifierScope(true), null, null,
                _clrContext, "Tests.ProbeModel", fallbackLocation: TemplateLocation).Emit();
            var builder = new StringBuilder();
            var jsWriter = new JSWriter(true, false);
            descriptor.Write(jsWriter);
            jsWriter.Write(new StringWriter(builder));
            return builder.ToString();
        }

        private static string EmitBinding(string bindingExpression)
        {
            var topology = GraphTopologyBuilder.Build(MakeTemplate(
                MakeBinding(bindingExpression, BindingMode.OneTime, ExpressionTarget.TextContent, "e0", "X")));
            // Target infos need the runtime DomTargetInfo factory; only the getters matter here.
            topology.DomTargets.Clear();
            return Emit(topology);
        }

        private static string EmitHandler(string handlerExpression)
        {
            var handler = MakeEvent("click", handlerExpression);
            handler.Location = HandlerLocation;
            return Emit(GraphTopologyBuilder.Build(MakeTemplate(handler)));
        }

        [TestMethod]
        public void ConstantStaticMember_IsEmittedAsLiteral()
        {
            EmitBinding("Tests.Limits.Max == 3").Should().Contain("return 3 == 3;");
        }

        [TestMethod]
        public void UnsupportedBindingPaths_FailAtTemplateLocation()
        {
            foreach (var expression in new[] { "Model.Inner.Inner", "Nope.Thing == 1" })
            {
                Action emit = () => EmitBinding(expression);
                var error = emit.Should().Throw<RazorSubControlDiagnosticException>().Which;
                error.Message.Should().Contain("'" + expression + "'").And.Contain("cannot be resolved");
                error.Location.Should().BeSameAs(TemplateLocation);
            }
        }

        [TestMethod]
        public void UnresolvableEventHandlers_FailAtHandlerLocation()
        {
            Action simple = () => EmitHandler("Model.Missing");
            simple.Should().Throw<RazorSubControlDiagnosticException>()
                .Which.Location.Should().BeSameAs(HandlerLocation);

            Action lambda = () => EmitHandler("(e) => Model.Missing(1, 2)");
            lambda.Should().Throw<RazorSubControlDiagnosticException>()
                .WithMessage("*Cannot resolve event handler*supported forms*")
                .Which.Location.Should().BeSameAs(HandlerLocation);
        }
    }
}
