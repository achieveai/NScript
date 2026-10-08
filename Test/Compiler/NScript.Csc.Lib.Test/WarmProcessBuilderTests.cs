namespace NScript.Csc.Lib.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;

    /// <summary>
    /// The build service runs <see cref="Builder.Execute"/> many times in one process.
    /// These contracts guard what a warm process can break and a fresh one cannot:
    /// output drift from process-wide state (the Cecil comparer cache, the sticky Logger,
    /// static counters) and file handles that outlive a build and lock obj/ DLLs.
    /// </summary>
    [TestClass]
    public class WarmProcessBuilderTests
    {
        /// <summary>
        /// Roots the static <c>Main</c> methods of the RealScript types whose names pass a
        /// filter, so the two fixtures convert different code from the same assembly.
        /// </summary>
        private sealed class MainRootsPlugin : IRuntimeConverterPlugin
        {
            private readonly Func<string, bool> typeFilter;
            private ClrContext clrContext;

            public MainRootsPlugin(Func<string, bool> typeFilter) => this.typeFilter = typeFilter;

            public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
                => this.clrContext = clrContext;

            public void ParseArgs(IList<Tuple<string, string>> args) { }

            public List<MethodReference> GetMethodsToEmitPass1()
                => this.clrContext.GetTypeDefinitions()
                    .Where(t => t.Namespace == "RealScript" && this.typeFilter(t.Name))
                    .SelectMany(t => t.Methods)
                    .Where(m => m.IsStatic && m.Name == "Main" && !m.HasParameters)
                    .Cast<MethodReference>()
                    .ToList();

            public List<MethodReference> GetMethodsToEmitPassN() => null;

            public List<Statement> GetPreJavascript() => null;

            public List<Statement> GetPostJavascript() => null;
        }

        private static readonly Func<string, bool> FixtureA = name => name.StartsWith("TestG", StringComparison.Ordinal);
        private static readonly Func<string, bool> FixtureB = name => name.StartsWith("TestA", StringComparison.Ordinal);

        /// <summary>
        /// Copies the fixture assemblies into a private directory, so the lock check below
        /// sees only handles this test's builds could have left open.
        /// </summary>
        private static (string dir, string main, string[] refs) CopyFixtures()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = Path.GetTempPath();
            var dir = Path.Combine(temp, "nscript-warm-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            return (
                dir,
                Copy("realScript.dll"),
                new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") });
        }

        private static byte[] Build(string dir, string main, string[] refs, Func<string, bool> fixture, string name)
        {
            // What the service does before every request.
            Builder.ResetProcessState();
            // Same file name for every build: the name is embedded in the output
            // (sourceMappingURL), so only the directory differs.
            var outDir = Path.Combine(dir, name);
            Directory.CreateDirectory(outDir);
            var outJs = Path.Combine(outDir, "bundle.js");
            var builder = new Builder(
                outJs,
                1,
                main,
                refs,
                new IConverterPlugin[] { new MainRootsPlugin(fixture) },
                (minify: false, uglify: false, optimize: false));

            Assert.IsTrue(builder.Execute(), "Build of fixture " + name + " failed.");
            return File.ReadAllBytes(outJs);
        }

        [TestMethod]
        public void WarmProcess_RepeatedBuildIsByteIdenticalAndReleasesInputFiles()
        {
            var (dir, main, refs) = CopyFixtures();
            try
            {
                byte[] first = Build(dir, main, refs, FixtureA, "a1");
                byte[] other = Build(dir, main, refs, FixtureB, "b1");
                byte[] again = Build(dir, main, refs, FixtureA, "a2");

                Assert.IsTrue(first.Length > 1000, "Fixture A must convert real code, got " + first.Length + " bytes.");
                Assert.IsFalse(first.SequenceEqual(other), "Fixtures A and B must differ, or the alternation proves nothing.");
                CollectionAssert.AreEqual(first, again, "A warm rebuild of fixture A drifted from its first build.");

                // The daemon must not keep obj/ DLLs open between requests: the next
                // compile writes them.
                foreach (var input in refs.Append(main))
                {
                    using var exclusive = File.Open(input, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
