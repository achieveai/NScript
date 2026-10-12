namespace NScript.Csc.Lib.Test
{
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The compile wraps its source resolver in <see cref="CachingSourceReferenceResolver"/>:
    /// Roslyn's own 16-entry debug-document cache thrashes on a large project, so it
    /// normalized a path (Path.GetFullPath) per method body.
    /// </summary>
    [TestClass]
    public class CachingSourceReferenceResolverTests
    {
        [TestMethod]
        public void Compile_WithCachingResolver_EmitsSameBytes_NormalizesEachPathOnce()
        {
            var dir = Path.Combine(Path.GetTempPath(), "nscript-resolver-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                // More files than Roslyn's 16 cache slots, several methods each.
                var trees = Enumerable.Range(0, 40)
                    .Select(file => CSharpSyntaxTree.ParseText(
                        SourceText.From(
                            "class C" + file + " {" + string.Concat(Enumerable.Range(0, 5).Select(m => " int M" + m + "(int x) { return x + " + m + "; }")) + " }",
                            Encoding.UTF8),
                        path: Path.Combine(dir, "Sub", "..", "F" + file + ".cs")))
                    .ToArray();

                var plain = new CountingResolver(new SourceFileResolver(ImmutableArray<string>.Empty, dir));
                var counted = new CountingResolver(new SourceFileResolver(ImmutableArray<string>.Empty, dir));
                var (plainDll, plainPdb) = Emit(trees, plain);
                var (cachedDll, cachedPdb) = Emit(trees, new CachingSourceReferenceResolver(counted));

                CollectionAssert.AreEqual(plainDll, cachedDll, "dll differs");
                CollectionAssert.AreEqual(plainPdb, cachedPdb, "pdb differs");
                Assert.IsTrue(plain.Calls > trees.Length, "the test no longer exercises a per-method normalization: " + plain.Calls);
                Assert.AreEqual(counted.Calls, counted.Paths, "a path was normalized more than once");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        private static (byte[] dll, byte[] pdb) Emit(SyntaxTree[] trees, SourceReferenceResolver resolver)
        {
            var compilation = CSharpCompilation.Create(
                "Resolved",
                trees,
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, deterministic: true, sourceReferenceResolver: resolver));
            using var dll = new MemoryStream();
            using var pdb = new MemoryStream();
            var result = compilation.Emit(dll, pdb, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb));
            Assert.IsTrue(result.Success, string.Join("\n", result.Diagnostics));
            return (dll.ToArray(), pdb.ToArray());
        }

        private sealed class CountingResolver : SourceReferenceResolver
        {
            private readonly SourceReferenceResolver inner;

            private readonly System.Collections.Concurrent.ConcurrentDictionary<(string, string), byte> seen =
                new System.Collections.Concurrent.ConcurrentDictionary<(string, string), byte>();

            private int calls;

            public CountingResolver(SourceReferenceResolver inner)
            {
                this.inner = inner;
            }

            public int Calls => this.calls;

            public int Paths => this.seen.Count;

            public override string NormalizePath(string path, string baseFilePath)
            {
                System.Threading.Interlocked.Increment(ref this.calls);
                this.seen.TryAdd((path, baseFilePath), 0);
                return this.inner.NormalizePath(path, baseFilePath);
            }

            public override string ResolveReference(string path, string baseFilePath) => this.inner.ResolveReference(path, baseFilePath);

            public override Stream OpenRead(string resolvedPath) => this.inner.OpenRead(resolvedPath);

            public override bool Equals(object other) => ReferenceEquals(this, other);

            public override int GetHashCode() => this.inner.GetHashCode();
        }
    }
}
