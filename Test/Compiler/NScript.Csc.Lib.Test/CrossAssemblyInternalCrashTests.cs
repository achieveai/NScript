namespace NScript.Csc.Lib.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Regression coverage for WI-93: cross-assembly <c>internal</c> access
    /// (and any other Roslyn-binding-error path) used to terminate the
    /// process with <see cref="NotSupportedException"/> from inside
    /// <c>SymbolSerializer.Serialize(TypeSymbol)</c>, because Roslyn invokes
    /// the <c>OnBoundExpressionGenerated</c> callback even on bodies that
    /// already carry binding errors. The fix gates the callback on
    /// <c>boundBody.HasErrors</c>/<c>initializers.HasErrors</c>, so
    /// <c>Compilation.Emit</c> can complete and return its
    /// <see cref="Diagnostic"/> collection normally.
    ///
    /// These tests use Roslyn directly (no MSBuild SDK / NScript framework
    /// references) so they are portable across Linux and Windows CI.
    /// </summary>
    [TestClass]
    public class CrossAssemblyInternalCrashTests
    {
        private const string ProducerSource = @"
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""Consumer"")]
namespace Producer
{
    public sealed class Thing
    {
        internal static string Hello() { return ""hi""; }
    }
}";

        // Producer WITHOUT [InternalsVisibleTo]. A consumer that calls the
        // internal member binds to an ErrorTypeSymbol — the exact shape the
        // probe captured under WI-93.
        private const string ProducerSourceNoIvt = @"
namespace Producer
{
    public sealed class Thing
    {
        internal static string Hello() { return ""hi""; }
    }
}";

        private const string ConsumerSource = @"
namespace Consumer
{
    public static class Caller
    {
        public static void Go()
        {
            var s = Producer.Thing.Hello();
        }
    }
}";

        [TestMethod]
        public void Crash_Regression_BindingErrorDoesNotThrow()
        {
            // Compile Producer without IVT; Consumer call to internal Hello()
            // therefore fails to bind and the local in `var s = ...` lands
            // with Kind == ErrorType. Before WI-93 the serializer threw
            // unconditionally; after the fix Roslyn surfaces its normal
            // CS-coded diagnostic and Emit returns Success == false.
            var producer = CompileToBytes(ProducerSourceNoIvt, "Producer");
            var producerRef = MetadataReference.CreateFromImage(producer);
            var consumer = CreateConsumerCompilation(producerRef);

            var outcome = InvokePipeline(consumer);

            Assert.IsNull(
                outcome.ThrownException,
                "InjectIntoCompilation/Emit must NOT throw on binding errors. WI-93 regression. Thrown: "
                    + outcome.ThrownException);
            Assert.IsFalse(outcome.Success, "Emit must report failure when the consumer fails to bind.");
            Assert.IsTrue(
                outcome.Diagnostics.Any(d =>
                    d.Severity == DiagnosticSeverity.Error
                    && (d.Id == "CS0117" || d.Id == "CS0122" || d.Id == "CS0103" || d.Id == "CS0246")),
                "Expected a Roslyn binding-error diagnostic. Actual diagnostics: "
                    + string.Join(", ", outcome.Diagnostics.Select(d => d.Id + ":" + d.GetMessage())));
        }

        [TestMethod]
        public void Healthy_CrossAssemblyInternal_WithIvt_StillCompiles()
        {
            // With [InternalsVisibleTo("Consumer")] declared, the consumer
            // binds the internal member successfully. The whole pipeline
            // must continue to produce a complete bound-body map — the
            // HasErrors gate must not regress healthy paths.
            var producer = CompileToBytes(ProducerSource, "Producer");
            var producerRef = MetadataReference.CreateFromImage(producer);
            var consumer = CreateConsumerCompilation(producerRef);

            var outcome = InvokePipeline(consumer);

            Assert.IsNull(outcome.ThrownException, "Pipeline must not throw on healthy IVT path. Thrown: " + outcome.ThrownException);
            Assert.IsTrue(
                outcome.Success,
                "Healthy IVT consumer should emit successfully. Diagnostics: "
                    + string.Join(", ", outcome.Diagnostics.Select(d => d.Id + ":" + d.GetMessage())));
            Assert.IsTrue(
                outcome.MethodCount > 0,
                "Healthy compilation must still capture at least one bound method body.");
        }

        [TestMethod]
        public void Healthy_NoBindingErrors_AllMethodBodiesCaptured()
        {
            // Pin that the HasErrors skip only drops error-bearing bodies.
            // A clean compilation must capture every user-declared method.
            const string cleanSource = @"
namespace Clean
{
    public static class A
    {
        public static int Add(int x, int y) { return x + y; }
        public static int Sub(int x, int y) { return x - y; }
        public static int Mul(int x, int y) { return x * y; }
    }
}";
            var tree = CSharpSyntaxTree.ParseText(cleanSource, path: "clean.cs");
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(
                "Clean",
                syntaxTrees: new[] { tree },
                references: new[] { mscorlib },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var outcome = InvokePipeline(compilation);

            Assert.IsNull(outcome.ThrownException);
            Assert.IsTrue(outcome.Success);
            Assert.AreEqual(
                3,
                outcome.MethodCount,
                "Every method in a healthy compilation must be captured by OnBoundExpressionGenerated.");
        }

        private static CSharpCompilation CreateConsumerCompilation(MetadataReference producerRef)
        {
            var tree = CSharpSyntaxTree.ParseText(ConsumerSource, path: "consumer.cs");
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            return CSharpCompilation.Create(
                "Consumer",
                syntaxTrees: new[] { tree },
                references: new[] { mscorlib, producerRef },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        private static byte[] CompileToBytes(string source, string assemblyName)
        {
            var tree = CSharpSyntaxTree.ParseText(source, path: assemblyName + ".cs");
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { tree },
                references: new[] { mscorlib },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            Assert.IsTrue(
                result.Success,
                "Producer compilation must succeed. Diagnostics: "
                    + string.Join(", ", result.Diagnostics.Select(d => d.Id + ":" + d.GetMessage())));
            return ms.ToArray();
        }

        private sealed class PipelineOutcome
        {
            public bool Success { get; init; }
            public System.Collections.Immutable.ImmutableArray<Diagnostic> Diagnostics { get; init; }
                = System.Collections.Immutable.ImmutableArray<Diagnostic>.Empty;
            public int MethodCount { get; init; }
            public Exception ThrownException { get; init; }
        }

        private static PipelineOutcome InvokePipeline(CSharpCompilation compilation)
        {
            try
            {
                var (resources, rv) = SerializationHelper.InjectIntoCompilation(compilation);
                using var output = new MemoryStream();
                using var pdb = new MemoryStream();
                var result = compilation.Emit(
                    output,
                    pdbStream: pdb,
                    manifestResources: resources);
                return new PipelineOutcome
                {
                    Success = result.Success,
                    Diagnostics = result.Diagnostics,
                    MethodCount = rv.Count,
                };
            }
            catch (Exception ex)
            {
                return new PipelineOutcome { ThrownException = ex };
            }
        }
    }
}
