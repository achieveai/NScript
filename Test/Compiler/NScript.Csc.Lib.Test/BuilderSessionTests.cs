namespace NScript.Csc.Lib.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;

    /// <summary>
    /// Slice 2, Inc 3: a dev-mode <see cref="Builder"/> keeps a session (loaded modules and
    /// converter context) between builds. A warm build must equal a cold one byte for byte,
    /// and between builds the session must hold no input file open and no plugin of an
    /// earlier build (plugins hold that build's RuntimeScopeManager).
    /// </summary>
    [TestClass]
    public class BuilderSessionTests
    {
        /// <summary>
        /// Roots every static parameterless <c>Main</c> of the RealScript types. It is also a
        /// method plugin, so the ConverterContext holds it during the build.
        /// </summary>
        private sealed class RootsPlugin : IRuntimeConverterPlugin, IMethodConverterPlugin
        {
            private ClrContext clrContext;
            private RuntimeScopeManager runtimeScopeManager;

            public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
            {
                this.clrContext = clrContext;
                this.runtimeScopeManager = runtimeScopeManager;
            }

            public void ParseArgs(IList<Tuple<string, string>> args) { }

            public List<MethodReference> GetMethodsToEmitPass1()
                => this.clrContext.GetTypeDefinitions()
                    .Where(t => t.Namespace == "RealScript")
                    .SelectMany(t => t.Methods)
                    .Where(m => m.IsStatic && m.Name == "Main" && !m.HasParameters)
                    .Cast<MethodReference>()
                    .ToList();

            public List<MethodReference> GetMethodsToEmitPassN() => null;

            public List<Statement> GetPreJavascript() => null;

            public List<Statement> GetPostJavascript() => null;

            /// <summary>The ConverterContext of the build that used this plugin.</summary>
            public WeakReference Context { get; private set; }

            public IntrestLevel GetInterestLevel(MethodDefinition methodDefinition, ConverterContext converterContext)
            {
                this.Context ??= new WeakReference(converterContext);
                return IntrestLevel.None;
            }

            public List<Statement> GetPreInsertionStatements(MethodConverter methodConverter) => null;

            public List<Statement> GetPostInsertionStatements(MethodConverter methodConverter) => null;

            public List<Statement> GetEncapsulationStatements(MethodConverter methodConverter, List<Statement> methodStatments)
                => methodStatments;

            public List<Statement> GetOverwrite(MethodConverter methodConverter) => null;
        }

        /// <summary>
        /// Builds once with a new plugin and returns weak references to it and to the build's
        /// ConverterContext. Not inlined, so no local of the caller roots either.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static (WeakReference plugin, WeakReference context) BuildOnce(
            Builder builder, string outJs, out byte[] js, out byte[] map)
        {
            Builder.ResetProcessState();
            var plugin = new RootsPlugin();
            Assert.IsTrue(builder.Execute(new IConverterPlugin[] { plugin }), "Dev-mode build failed.");
            Assert.IsNotNull(plugin.Context, "The build never asked the method plugin.");
            js = File.ReadAllBytes(outJs);
            map = File.ReadAllBytes(Path.ChangeExtension(outJs, ".map"));
            return (new WeakReference(plugin), plugin.Context);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool SameTarget(WeakReference a, WeakReference b) => ReferenceEquals(a.Target, b.Target);

        private static void CollectGarbage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 3 builds; the only end-to-end check of a kept session.
        public void Session_WarmBuildEqualsColdAndHoldsNoFilesPluginsOrOldContext()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-session-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            var main = Copy("realScript.dll");
            var refs = new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") };
            var outJs = Path.Combine(dir, "bundle.js");
            try
            {
                using (var builder = new Builder(
                    outJs,
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true))
                {
                    var cold = BuildOnce(builder, outJs, out var coldJs, out var coldMap);
                    Assert.AreEqual("cold", builder.LastBuildKind);
                    Assert.IsTrue(coldJs.Length > 1000, "The fixture must convert real code, got " + coldJs.Length + " bytes.");

                    // The session lives on, but the next compile must be able to write its inputs.
                    foreach (var input in refs.Append(main))
                    {
                        using var exclusive = File.Open(input, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    }

                    var warm = BuildOnce(builder, outJs, out var warmJs, out var warmMap);
                    Assert.AreEqual("warm", builder.LastBuildKind);
                    Assert.IsTrue(SameTarget(cold.context, warm.context), "A warm build must reuse the session's ConverterContext.");
                    CollectionAssert.AreEqual(coldJs, warmJs, "A warm build's .js differs from the cold build.");
                    CollectionAssert.AreEqual(coldMap, warmMap, "A warm build's .map differs from the cold build.");

                    CollectGarbage();
                    Assert.IsFalse(cold.plugin.IsAlive, "The session still holds the first build's plugin.");
                    Assert.IsFalse(warm.plugin.IsAlive, "Between builds the session still holds the last build's plugin.");

                    // A touch with the same content keeps the session: stamps are content hashes.
                    File.SetLastWriteTimeUtc(main, File.GetLastWriteTimeUtc(main).AddSeconds(5));
                    BuildOnce(builder, outJs, out var touchedJs, out _);
                    Assert.AreEqual("warm", builder.LastBuildKind, "A touch that keeps the content must reuse the session.");
                    CollectionAssert.AreEqual(coldJs, touchedJs, "A warm build after a touch differs from the cold build.");

                    // Same length and same mtime, different content (cp -p, restored packages, two
                    // writes in one tick): one byte of the PE TimeDateStamp, which the output does
                    // not use. First a reference (a framework rebuild), then the entry DLL. Each
                    // build must be cold, and the reset drops the old context.
                    static void FlipTimeDateStamp(string path)
                    {
                        var bytes = File.ReadAllBytes(path);
                        bytes[BitConverter.ToInt32(bytes, 0x3C) + 8] ^= 0x5A;
                        var mtime = File.GetLastWriteTimeUtc(path);
                        File.WriteAllBytes(path, bytes);
                        File.SetLastWriteTimeUtc(path, mtime);
                    }

                    FlipTimeDateStamp(refs[2]);
                    BuildOnce(builder, outJs, out var refChangedJs, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind, "A changed reference DLL must not reuse the session.");
                    CollectionAssert.AreEqual(coldJs, refChangedJs, "The reference's TimeDateStamp byte must not change the output.");

                    FlipTimeDateStamp(main);
                    var again = BuildOnce(builder, outJs, out var againJs, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind, "A same-length, same-mtime rewrite must not reuse the session.");
                    CollectionAssert.AreEqual(coldJs, againJs, "The TimeDateStamp byte must not change the output.");
                    CollectGarbage();
                    Assert.IsFalse(cold.context.IsAlive, "After a reset the session still holds the old ConverterContext.");
                    Assert.IsTrue(again.context.IsAlive, "The new session must keep its ConverterContext.");
                }
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
