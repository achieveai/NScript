using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NScript.CLR;
using NScript.Converter;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    /// <summary>
    /// A skin reads a simple getter as its backing field, so the getter itself is never
    /// called. A dev patch of a body-only change cannot keep that read once the getter's body
    /// changed: the plugin must report a miss, so the build emits the skins in full. Each test
    /// edits a getter's IL in its own context, as a body-only recompile leaves it.
    /// </summary>
    [TestClass]
    public class RazorInlinedGetterPatchTests
    {
        private const string Probe = "Sunlight.Framework.UI.Test.RazorValueProbeControl";
        private const string ViewModel = "Sunlight.Framework.UI.Test.TestViewModelA";

        private static (RazorTemplatingPlugin Plugin, ClrContext Context) BuildSkins(System.Action<ClrContext> before = null)
        {
            var clrContext = UiTestAssemblies.Load();
            before?.Invoke(clrContext);
            var plugin = new RazorTemplatingPlugin();
            UiTestAssemblies.Build(plugin, clrContext, new ConverterContext(clrContext, null, null));
            return (plugin, clrContext);
        }

        private static TypeDefinition Type(ClrContext clrContext, string fullName)
            => clrContext.GetTypeDefinitions().Single(type => type.FullName == fullName);

        private static MethodBody Getter(TypeDefinition type, string property)
            => type.Properties.Single(p => p.Name == property).GetMethod.Body;

        [TestMethod]
        public void TwoWayTargetGetter_BodyChanged_IsAMiss()
        {
            var (plugin, clrContext) = BuildSkins();
            var probe = Type(clrContext, Probe);
            plugin.TryPatchBodies(new[] { probe }).Should().BeNull("an unchanged getter keeps the skin's field read");

            // get { return this.value; } becomes a body that does more than the read.
            var body = Getter(probe, "Value");
            var ret = body.Instructions.Last();
            var il = body.GetILProcessor();
            il.InsertBefore(ret, il.Create(OpCodes.Dup));
            il.InsertBefore(ret, il.Create(OpCodes.Pop));

            plugin.TryPatchBodies(new[] { probe }).Should().Be("razor inlined getter " + Probe + ".Value");
            plugin.TryPatchBodies(new[] { Type(clrContext, ViewModel) })
                .Should().BeNull("only a getter of a changed type can have changed");
        }

        /// <summary>
        /// A skin also writes a sub-control's simple setter as its field. No UI test control
        /// has one, so the probe's setter is made a plain store before the skins are built.
        /// </summary>
        [TestMethod]
        public void SubControlSetter_BodyChanged_IsAMiss()
        {
            var (plugin, clrContext) = BuildSkins(context =>
            {
                // set { this.value = value; }
                var probe = Type(context, Probe);
                var body = probe.Properties.Single(p => p.Name == "Value").SetMethod.Body;
                body.Instructions.Clear();
                body.Variables.Clear();
                body.ExceptionHandlers.Clear();
                var il = body.GetILProcessor();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, probe.Fields.Single(field => field.Name == "value"));
                il.Emit(OpCodes.Ret);
            });
            var probe = Type(clrContext, Probe);
            plugin.TryPatchBodies(new[] { probe }).Should().BeNull("an unchanged setter keeps the skin's field write");

            var setter = probe.Properties.Single(p => p.Name == "Value").SetMethod.Body;
            var ret = setter.Instructions.Last();
            var edit = setter.GetILProcessor();
            edit.InsertBefore(ret, edit.Create(OpCodes.Ldc_I4_0));
            edit.InsertBefore(ret, edit.Create(OpCodes.Pop));

            plugin.TryPatchBodies(new[] { probe }).Should().Be("razor inlined setter " + Probe + ".Value");
        }

        [TestMethod]
        public void OneWayGetter_ReadsAnotherField_IsAMiss()
        {
            var (plugin, clrContext) = BuildSkins();
            var viewModel = Type(clrContext, ViewModel);
            plugin.TryPatchBodies(new[] { viewModel }).Should().BeNull("an unchanged getter keeps the skin's field read");

            // get { return this.str1; } becomes get { return this.testVMA; }: still a simple
            // read, of a field the skin does not read.
            var load = Getter(viewModel, "PropStr1").Instructions.Single(instruction => instruction.OpCode == OpCodes.Ldfld);
            load.Operand = viewModel.Fields.Single(field => field.Name == "testVMA");

            plugin.TryPatchBodies(new[] { viewModel }).Should().Be("razor inlined getter " + ViewModel + ".PropStr1");
        }
    }
}
