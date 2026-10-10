namespace NScript.Csc.Lib.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using NScript.Utils;

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

            /// <summary>The text of the main module's <see cref="OracleResource"/> the last build's plugin read, if any.</summary>
            public static string LastOracleText { get; set; }

            public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
            {
                this.clrContext = clrContext;
                this.runtimeScopeManager = runtimeScopeManager;
                var resource = clrContext.Modules
                    .SelectMany(m => m.Resources.OfType<EmbeddedResource>())
                    .FirstOrDefault(r => r.Name == OracleResource);
                LastOracleText = resource == null ? null : Encoding.UTF8.GetString(resource.GetResourceData());
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

        private const string OracleResource = "Oracle.txt";

        /// <summary>
        /// Sets <see cref="OracleResource"/> in the DLL at <paramref name="path"/> through Cecil, which
        /// keeps the MVID and every other resource: the shape of a watch-mode resource patch.
        /// </summary>
        private static void SetOracleResource(string path, string text)
        {
            using var module = ModuleDefinition.ReadModule(new MemoryStream(File.ReadAllBytes(path)));
            var resource = new EmbeddedResource(OracleResource, ManifestResourceAttributes.Public, Encoding.UTF8.GetBytes(text));
            var index = module.Resources.ToList().FindIndex(r => r.Name == OracleResource);
            if (index < 0)
            {
                module.Resources.Add(resource);
            }
            else
            {
                module.Resources[index] = resource;
            }

            module.Write(path);
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

        /// <summary>
        /// M3 slice 3 (L1): a change to the entry DLL that only changes a resource keeps the
        /// session. The build is warm on the same ConverterContext, the plugin reads the new
        /// resource, the output equals the cold build, and the next unchanged build is warm, so
        /// the session took the new stamps. A same-MVID change that is not a resource change
        /// stays cold (Session_WarmBuildEqualsCold...: TimeDateStamp flip).
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 4 builds.
        public void Session_ResourceOnlyChange_RefreshesWarm_EqualsCold_KeepsNewStamps()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-refresh-" + Guid.NewGuid().ToString("N"));
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
            SetOracleResource(main, "v1");
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
                    Assert.AreEqual("v1", RootsPlugin.LastOracleText);

                    SetOracleResource(main, "v2");
                    var refreshed = BuildOnce(builder, outJs, out var refreshedJs, out var refreshedMap);
                    Assert.AreEqual("warm", builder.LastBuildKind, "A resource-only change must keep the session.");
                    Assert.AreEqual("resources-refreshed", builder.LastBuildReason, "watch.log shows this reason.");
                    Assert.IsTrue(SameTarget(cold.context, refreshed.context), "A refreshed build must reuse the session's ConverterContext.");
                    Assert.AreEqual("v2", RootsPlugin.LastOracleText, "The plugin must read the new resource.");
                    CollectionAssert.AreEqual(coldJs, refreshedJs, "A refreshed build's .js differs from the cold build.");
                    CollectionAssert.AreEqual(coldMap, refreshedMap, "A refreshed build's .map differs from the cold build.");

                    BuildOnce(builder, outJs, out var againJs, out _);
                    Assert.AreEqual("warm", builder.LastBuildKind, "After a refresh the session must hold the new stamps.");
                    Assert.AreEqual("v2", RootsPlugin.LastOracleText);
                    CollectionAssert.AreEqual(coldJs, againJs);
                }

                using (var fresh = new Builder(
                    outJs,
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true))
                {
                    BuildOnce(fresh, outJs, out _, out _);
                    Assert.AreEqual("cold", fresh.LastBuildKind);
                    Assert.AreEqual("v2", RootsPlugin.LastOracleText, "A cold build of the patched DLL reads the same resource.");
                }
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// Compiles <paramref name="files"/> (name, text) into <c>lib.dll</c> in
        /// <paramref name="dir"/> the way the NScript csc does: bound bodies in $$BstInfo$$ and
        /// source checksums in $$SrcInfo$$.
        /// </summary>
        private static string CompileLib(string dir, string[] refs, params (string name, string text)[] files)
        {
            var path = Path.Combine(dir, "lib.dll");
            File.Delete(path);
            var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
                "lib",
                files.Select(file => Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(
                    file.text,
                    Microsoft.CodeAnalysis.CSharp.CSharpParseOptions.Default,
                    Path.Combine(dir, file.name),
                    Encoding.UTF8)),
                refs.Select(reference => Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(reference)),
                new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(
                    Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary,
                    concurrentBuild: false));
            Assert.IsNotNull(SerializationHelper.ExpressionVisitMap(compilation, dir, "lib.dll"), "The fixture library must compile.");
            return path;
        }

        private const string HelperSource =
            "namespace RealScript { public static class Helper { public static int Say(string text) { return text.Length; } } }";

        private static string ProgramSource(string body) =>
            "namespace RealScript { using System; using System.Collections.Generic; public static class Program { public static int Main() { " + body + " } } }";

        /// <summary>
        /// Dot 3 of the save-to-JS plan: a recompile that changes only method bodies keeps the
        /// session; the kept module takes the new bodies in place. The edit adds a lambda (a
        /// new generated member) and calls referenced members whose signatures hold primitives
        /// (<c>List&lt;int&gt;.Count</c>, <c>string.IndexOf</c>): a remapped reference built
        /// without the primitive's element type did not resolve. The output must equal a cold
        /// build of the same DLL. Removing the lambda refreshes too; a signature change is a
        /// refresh miss, so the build is cold and still equal.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 7 builds of a small library.
        public void Session_BodyOnlyRecompile_RefreshesBodies_EqualsCold_SignatureChangeIsCold()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-bodies-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            var refs = new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") };
            var outJs = Path.Combine(dir, "bundle.js");
            // Same file name, so the sourceMappingURL line matches.
            var coldJs = Path.Combine(Directory.CreateDirectory(Path.Combine(dir, "cold")).FullName, "bundle.js");
            string Compile(string body) =>
                CompileLib(dir, refs, ("Program.cs", ProgramSource(body)), ("Helper.cs", HelperSource));

            byte[] Cold(string main)
            {
                using var fresh = new Builder(
                    coldJs,
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                BuildOnce(fresh, coldJs, out var js, out _);
                Assert.AreEqual("cold", fresh.LastBuildKind);
                return js;
            }

            var main = Compile("return Helper.Say(\"v1\");");
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
                    var first = BuildOnce(builder, outJs, out _, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind);

                    void AssertRefreshed(string body, string what)
                    {
                        Compile(body);
                        var refreshed = BuildOnce(builder, outJs, out var js, out _);
                        Assert.AreEqual("warm", builder.LastBuildKind, what + " must keep the session.");
                        Assert.AreEqual("bodies-refreshed", builder.LastBuildReason, what);
                        Assert.IsTrue(SameTarget(first.context, refreshed.context), what + " must reuse the session's ConverterContext.");
                        CollectionAssert.AreEqual(Cold(main), js, what + ": the refreshed .js differs from a cold build.");
                    }

                    AssertRefreshed(
                        "var list = new List<int>(); list.Add(\"v2\".IndexOf(\"2\")); Func<int, int> twice = value => value * 2; return twice(list.Count) + Helper.Say(\"v2\");",
                        "A body edit that adds a lambda");
                    AssertRefreshed("return Helper.Say(\"v3\");", "A body edit that removes the lambda");

                    BuildOnce(builder, outJs, out _, out _);
                    Assert.AreEqual("warm", builder.LastBuildKind, "After a refresh the session must hold the new stamps.");
                    Assert.AreEqual("unchanged", builder.LastBuildReason);

                    CompileLib(
                        dir,
                        refs,
                        ("Program.cs", ProgramSource("return Helper.Say(\"v4\");")),
                        ("Helper.cs", HelperSource.Replace("public static int Say", "public static int Twice(int value) { return value * 2; } public static int Say")));
                    BuildOnce(builder, outJs, out var changedJs, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind, "A new member changes the surface, so the build must be cold.");
                    StringAssert.StartsWith(builder.LastBuildReason, "inputs-changed; refresh miss: ");
                    CollectionAssert.AreEqual(Cold(main), changedJs, "The cold build after a signature change differs from a fresh one.");
                }
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// F-M: <c>Program.cs</c> returns <c>Chosen.Value</c>, a const reached through a global
        /// alias in <c>GlobalUsings.cs</c>, a file that declares no type. Changing only the
        /// alias changes what Program binds to, though Program's file is unchanged: the build
        /// must be cold (not a body refresh that keeps the old literal) and equal a cold build.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 4 builds of a small library.
        public void Session_TypelessFileChange_IsARefreshMiss_EqualsCold()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-typeless-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            var refs = new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") };
            var outJs = Path.Combine(dir, "bundle.js");
            var coldJs = Path.Combine(Directory.CreateDirectory(Path.Combine(dir, "cold")).FullName, "bundle.js");
            const string Demo =
                "namespace RealScript { public static class First { public const int Value = 1; } public static class Second { public const int Value = 2; } }";
            string Compile(string chosen) => CompileLib(
                dir,
                refs,
                ("Program.cs", ProgramSource("return Chosen.Value;")),
                ("Demo.cs", Demo),
                ("GlobalUsings.cs", "global using Chosen = RealScript." + chosen + ";"));
            Builder Create(string js, string main) => new Builder(
                js,
                1,
                main,
                refs,
                Array.Empty<IConverterPlugin>(),
                (minify: false, uglify: false, optimize: false),
                devMode: true);

            var main = Compile("First");
            try
            {
                using (var builder = Create(outJs, main))
                {
                    BuildOnce(builder, outJs, out _, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind);

                    Compile("Second");
                    BuildOnce(builder, outJs, out var changedJs, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind, "A changed global alias must not refresh bodies.");
                    StringAssert.Contains(builder.LastBuildReason, "typeless-file GlobalUsings.cs");

                    using var fresh = Create(coldJs, main);
                    BuildOnce(fresh, coldJs, out var freshJs, out _);
                    CollectionAssert.AreEqual(freshJs, changedJs, "The build after the alias change differs from a cold build.");
                }
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// The shortest prefix of <paramref name="whole"/> (in 61-byte steps) that Cecil fails to
        /// read with an exception other than BadImageFormatException: a cut inside the headers
        /// (EndOfStreamException or IndexOutOfRangeException), which the old catch let escape.
        /// </summary>
        private static byte[] TruncateUnreadably(byte[] whole)
        {
            for (int keep = 64; keep < whole.Length; keep += 61)
            {
                var cut = whole.Take(keep).ToArray();
                try
                {
                    using var module = ModuleDefinition.ReadModule(new MemoryStream(cut), new ReaderParameters(ReadingMode.Deferred));
                    foreach (var resource in module.Resources.OfType<EmbeddedResource>())
                    {
                        resource.GetResourceData();
                    }
                }
                catch (BadImageFormatException)
                {
                }
                catch (Exception)
                {
                    return cut;
                }
            }

            Assert.Fail("No prefix of the " + whole.Length + "-byte fixture fails to read with a non-BadImageFormat exception.");
            return null;
        }

        /// <summary>
        /// M3 slice 3 (L1): the refresh is an optimisation and must never be a new way to fail.
        /// A changed input Cecil cannot read (a truncated DLL, not a BadImageFormatException) is a
        /// refresh miss with its reason logged; the build then fails only where a cold build
        /// fails, loading the DLL. With the DLL whole again the next build is cold and green.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 3 builds.
        public void Session_UnreadableChangedInput_IsARefreshMiss_ThenColdAndGreen()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-refresh-miss-" + Guid.NewGuid().ToString("N"));
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
            var logPath = Path.Combine(dir, "build.jsonl");
            CompilerLog.Shutdown();
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
                    BuildOnce(builder, outJs, out var coldJs, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind);

                    var whole = File.ReadAllBytes(main);
                    File.WriteAllBytes(main, TruncateUnreadably(whole));
                    CompilerLog.Initialize(logPath, "test");
                    bool built;
                    try
                    {
                        Builder.ResetProcessState();
                        built = builder.Execute(new IConverterPlugin[] { new RootsPlugin() });
                    }
                    catch (Exception ex) when (!(ex is AssertFailedException))
                    {
                        built = false;
                    }
                    finally
                    {
                        CompilerLog.Shutdown();
                    }

                    Assert.IsFalse(built, "A truncated entry DLL cannot build.");
                    var refresh = File.ReadAllLines(logPath)
                        .Select(line => JsonDocument.Parse(line).RootElement)
                        .Where(e => e.GetProperty("@mt").GetString().StartsWith("Session.Refresh ", StringComparison.Ordinal))
                        .ToList();
                    Assert.AreEqual(1, refresh.Count, "The refresh must log its miss instead of throwing.");
                    Assert.IsFalse(refresh[0].GetProperty("Refreshed").GetBoolean());
                    var miss = refresh[0].GetProperty("Miss").GetString();
                    // The refresh planner reads the changed image first, so it is usually the one that fails.
                    Assert.IsTrue(
                        miss.StartsWith("plan-error ", StringComparison.Ordinal) || miss.StartsWith("error ", StringComparison.Ordinal),
                        "Miss: " + miss);
                    Assert.AreEqual("cold", builder.LastBuildKind, "The failed build is reported, not the one before it.");
                    Assert.AreEqual("inputs-changed; refresh miss: " + miss, builder.LastBuildReason);

                    File.WriteAllBytes(main, whole);
                    BuildOnce(builder, outJs, out var againJs, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind, "The failed build dropped the session.");
                    CollectionAssert.AreEqual(coldJs, againJs);
                }
            }
            finally
            {
                CompilerLog.Shutdown();
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
