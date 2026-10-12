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

        private const string CounterSource =
            "namespace RealScript { public interface ICount { int Next(); } public class Counter : ICount { private int value; public Counter() { System.Func<int> zero = () => 0; value = zero(); } public Counter(int start) { value = start; } public int Next() { System.Func<int, int> add = x => x + STEP; value = add(value); return value; } public void Reset() { value = 0; } } }";

        /// <summary>
        /// The text a patch put before each type marker, by type: <paramref name="patched"/>
        /// must be <paramref name="js"/> with text added only at the ends of marker sections.
        /// </summary>
        private static Dictionary<string, string> PatchFragments(string js, string patched, string what)
        {
            const string Needle = "\r\n/*@T ";
            var before = js.Split(Needle);
            var after = patched.Split(Needle);
            Assert.AreEqual(before.Length, after.Length, what + ": the patch changed the markers.");
            var fragments = new Dictionary<string, string>();
            for (int i = 0; i < before.Length; i++)
            {
                StringAssert.StartsWith(after[i], before[i], what + ": the patch changed a section's text.");
                if (after[i].Length > before[i].Length)
                {
                    Assert.IsTrue(i + 1 < before.Length, what + ": text after the last marker.");
                    fragments.Add(
                        after[i + 1].Substring(0, after[i + 1].IndexOf("*/", StringComparison.Ordinal)),
                        after[i].Substring(before[i].Length));
                }
            }

            return fragments;
        }

        /// <summary>
        /// Dev patch emit (<c>NSCRIPT_PATCH_EMIT=1</c>): after a full emit, a body-only edit
        /// writes only the changed methods, each before its type's marker, and leaves every
        /// other byte. A global static is declared again as a cold build declares it; an
        /// instance method's prototype slot gets the cold build's function. A method the bundle
        /// never emitted is added the same way when a new body calls it. A body that needs a
        /// type the bundle never emitted is a miss: the build emits in full, equal to a cold
        /// build, and the next body edit patches again. So is a body that calls a constructor
        /// overload the bundle never emitted (its factory is a new root variable). An instance
        /// method that implements an interface is copied into the interface slot again. A
        /// lambda keeps its name when its method, or another method of its type, converts again.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 10 builds of a small library.
        public void Session_PatchEmit_BodyEditPatchesBeforeMarker_NewTypeEmitsInFull()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-patch-" + Guid.NewGuid().ToString("N"));
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
            const string Helper =
                "namespace RealScript { public static class Helper { public static int Say(string text) { return text.Length; } public static int Unused(int value) { return value + 1; } } public class Spare { public int Get() { return 7; } } }";
            string Compile(string body, int step) => CompileLib(
                dir,
                refs,
                ("Program.cs", ProgramSource(body)),
                ("Helper.cs", Helper),
                ("Counter.cs", CounterSource.Replace("STEP", step.ToString())));

            string Cold(string main)
            {
                using var fresh = new Builder(
                    coldJs,
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                BuildOnce(fresh, coldJs, out _, out _);
                return File.ReadAllText(coldJs);
            }

            var previous = Environment.GetEnvironmentVariable("NSCRIPT_PATCH_EMIT");
            Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", "1");
            var main = Compile("ICount counted = new Counter(); return counted.Next() + new Counter().Next() + Helper.Say(\"v1\");", 1);
            try
            {
                using var builder = new Builder(
                    outJs,
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("cold", builder.LastBuildKind);
                var js = File.ReadAllText(outJs);
                StringAssert.StartsWith(
                    js.TrimStart('﻿'),
                    "(function(){if(typeof window!==\"undefined\")(window.__nsPatch||(window.__nsPatch={}))[\"bundle.js\"]=function(f){eval(f);};",
                    "A patchable dev bundle registers its live patch hook.");

                // A patch build converts with the kept plugins, so the new one is never asked.
                Dictionary<string, string> Patched(int methods, string what)
                {
                    Builder.ResetProcessState();
                    Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), what + ": the build failed.");
                    Assert.AreEqual("bodies-refreshed; patched " + methods + " methods", builder.LastBuildReason, what + ": " + builder.LastPatchMiss);
                    var patched = File.ReadAllText(outJs);
                    var fragments = PatchFragments(js, patched, what);
                    js = patched;
                    return fragments;
                }

                // The function a prototype-slot fragment assigns, before any slot copy.
                const string SlotCopy = "\r\nRealScript_Counter$.prototype.";
                static string Assigned(string fragment)
                {
                    var function = fragment.Substring(fragment.IndexOf(" = function ", StringComparison.Ordinal) + 3);
                    int copy = function.IndexOf(SlotCopy, StringComparison.Ordinal);
                    return (copy < 0 ? function : function.Substring(0, copy)).TrimEnd(';');
                }

                main = Compile("ICount counted = new Counter(); return counted.Next() + new Counter().Next() + Helper.Say(\"v2\");", 1);
                var fragments = Patched(1, "A static body edit");
                CollectionAssert.AreEquivalent(new[] { "RealScript.Program" }, fragments.Keys.ToList());
                StringAssert.StartsWith(fragments["RealScript.Program"], "\r\nfunction RealScript_Program$Main(");
                StringAssert.Contains(Cold(main), fragments["RealScript.Program"], "The static's fragment must be its cold declaration.");

                main = Compile("ICount counted = new Counter(); return counted.Next() + new Counter().Next() + Helper.Say(\"v2\");", 2);
                fragments = Patched(1, "An instance body edit");
                CollectionAssert.AreEquivalent(new[] { "RealScript.Counter" }, fragments.Keys.ToList());
                StringAssert.StartsWith(fragments["RealScript.Counter"], "\r\nRealScript_Counter$.prototype.");
                var counterFragment = fragments["RealScript.Counter"];
                var cold = Cold(main);
                Assert.IsTrue(cold.Contains(Assigned(counterFragment)), "The slot's function must be the cold build's.");

                // The interface slot ("Next_" + typeId, folded to a name) copies the new function.
                int copyAt = counterFragment.IndexOf(SlotCopy, SlotCopy.Length, StringComparison.Ordinal);
                Assert.IsTrue(copyAt > 0, "The interface slot must take the new function again.");
                Assert.IsTrue(
                    cold.Contains(counterFragment.Substring(copyAt).Replace("RealScript_Counter$.prototype.", "$$ptyp.")),
                    "The interface slot copy must be the cold section's copy.");

                main = Compile("return new Counter().Next() + Helper.Unused(2);", 2);
                fragments = Patched(2, "A body that calls a static the bundle never emitted");
                CollectionAssert.AreEquivalent(new[] { "RealScript.Program", "RealScript.Helper" }, fragments.Keys.ToList());
                StringAssert.StartsWith(fragments["RealScript.Helper"], "\r\nfunction RealScript_Helper$Unused(");
                cold = Cold(main);
                StringAssert.Contains(cold, fragments["RealScript.Helper"], "The new static must be its cold declaration.");
                StringAssert.Contains(cold, fragments["RealScript.Program"]);

                main = Compile("var counter = new Counter(); counter.Reset(); return counter.Next() + Helper.Unused(2);", 2);
                fragments = Patched(2, "A body that calls an instance method the bundle never emitted");
                CollectionAssert.AreEquivalent(new[] { "RealScript.Program", "RealScript.Counter" }, fragments.Keys.ToList());
                StringAssert.StartsWith(fragments["RealScript.Counter"], "\r\nRealScript_Counter$.prototype.");
                StringAssert.Contains(Cold(main), Assigned(fragments["RealScript.Counter"]), "The new slot's function must be the cold build's.");

                main = Compile("return new Spare().Get();", 2);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("bodies-refreshed", builder.LastBuildReason, "A body that needs a new type must emit in full.");
                js = File.ReadAllText(outJs);
                Assert.AreEqual(Cold(main), js, "The full emit after a miss differs from a cold build.");

                main = Compile("return new Spare().Get() + 1;", 2);
                fragments = Patched(1, "A body edit after a miss");
                CollectionAssert.AreEquivalent(new[] { "RealScript.Program" }, fragments.Keys.ToList());

                main = Compile("return new Spare().Get() + new Counter(5).Next();", 2);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("bodies-refreshed", builder.LastBuildReason, "A body that needs a new constructor factory must emit in full.");
                Assert.IsNotNull(builder.LastPatchMiss, "A new constructor overload must miss.");

                // Compaction: once the patches grow the bundle past the limit, the next build
                // emits in full and drops the old code.
                js = File.ReadAllText(outJs);
                Assert.AreEqual(Cold(main), js, "The full emit after a factory miss differs from a cold build.");
                main = Compile("return new Spare().Get() + new Counter(6).Next();", 2);
                fragments = Patched(1, "A body edit before compaction");
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT", "0");
                main = Compile("return new Spare().Get() + new Counter(7).Next();", 2);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("compaction", builder.LastPatchMiss);
                Assert.AreEqual(Cold(main), File.ReadAllText(outJs), "The compaction emit differs from a cold build.");

                // A body edit and a resource edit in one image (one watch batch): the plugins
                // are asked for their sections too. RootsPlugin has none, so it emits in full.
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT", null);
                main = Compile("return new Spare().Get() + new Counter(8).Next();", 2);
                SetOracleResource(main, "v2");
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("plugin RootsPlugin", builder.LastPatchMiss, "A resource changed with the code must reach the plugins.");
                Assert.AreEqual(Cold(main), File.ReadAllText(outJs), "The full emit after a resource miss differs from a cold build.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", previous);
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT", null);
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

        /// <summary>
        /// Dev patch emit of a type with a method a plugin writes in place of its body (a skin
        /// getter): a body edit elsewhere in the type patches, and the written method stands.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 2 builds of a small library.
        public void Session_PatchEmit_TypeWithOverwrittenMethod_PatchesOtherBodies()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-patch-overwrite-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            var refs = new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") };
            var outJs = Path.Combine(dir, "bundle.js");
            string Compile(int step) => CompileLib(
                dir,
                refs,
                ("Program.cs", ProgramSource("var view = new View(); view.Skin(); return view.Next();")),
                ("View.cs", "namespace RealScript { public class View { private int value; public void Skin() { value = 9; } public int Next() { value = value + " + step + "; return value; } } }"));

            var previous = Environment.GetEnvironmentVariable("NSCRIPT_PATCH_EMIT");
            Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", "1");
            try
            {
                using var builder = new Builder(
                    outJs,
                    1,
                    Compile(1),
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                Builder.ResetProcessState();
                var plugin = new OverwritePlugin { MethodName = "Skin" };
                Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin(), plugin }), "The full build failed.");
                Assert.AreEqual(1, plugin.Overwrites, "The plugin must write Skin.");
                var full = File.ReadAllText(outJs);

                Compile(2);
                Builder.ResetProcessState();
                Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), "The patch build failed.");
                Assert.AreEqual("bodies-refreshed; patched 1 methods", builder.LastBuildReason, builder.LastPatchMiss);
                Assert.AreEqual(1, plugin.Overwrites, "The written Skin stands; the plugin is not asked again.");
                var patched = File.ReadAllText(outJs);
                StringAssert.StartsWith(patched, full.Substring(0, full.LastIndexOf("/*@T RealScript.View*/", StringComparison.Ordinal) - 2));
                Assert.IsFalse(full.Contains(" + 2;"));
                Assert.IsTrue(patched.Contains(" + 2;"), "The edited body must be written: " + patched);
            }
            finally
            {
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", previous);
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// Overwrites every RealScript <see cref="MethodName"/> (<c>Main</c>) with <c>return;</c> and reports the build's
        /// method cache counts. Its conversion is replayable when <see cref="Replayable"/>.
        /// </summary>
        private sealed class OverwritePlugin : IRuntimeConverterPlugin, IMethodConverterPlugin
        {
            private RuntimeScopeManager runtimeScopeManager;

            public bool Replayable { get; set; }

            /// <summary>The name of the RealScript methods it overwrites.</summary>
            public string MethodName { get; set; } = "Main";

            public int Hits { get; private set; }

            public int Misses { get; private set; }

            public int UncacheablePlugin { get; private set; }

            public int Overwrites { get; private set; }

            public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
                => this.runtimeScopeManager = runtimeScopeManager;

            public void ParseArgs(IList<Tuple<string, string>> args) { }

            public List<MethodReference> GetMethodsToEmitPass1() => null;

            public List<MethodReference> GetMethodsToEmitPassN() => null;

            public List<Statement> GetPreJavascript() => null;

            public List<Statement> GetPostJavascript()
            {
                var cache = this.runtimeScopeManager.Context.MethodCache;
                this.Hits = cache.Hits;
                this.Misses = cache.Misses;
                this.UncacheablePlugin = cache.Uncacheable.TryGetValue("plugin", out var count) ? count : 0;
                return null;
            }

            public IntrestLevel GetInterestLevel(MethodDefinition methodDefinition, ConverterContext converterContext)
                => methodDefinition.DeclaringType.Namespace == "RealScript" && methodDefinition.Name == this.MethodName
                    ? IntrestLevel.Overwrite
                    : IntrestLevel.None;

            public bool CanReplayConversion(MethodDefinition methodDefinition, ConverterContext converterContext) => this.Replayable;

            public List<Statement> GetPreInsertionStatements(MethodConverter methodConverter) => null;

            public List<Statement> GetPostInsertionStatements(MethodConverter methodConverter) => null;

            public List<Statement> GetEncapsulationStatements(MethodConverter methodConverter, List<Statement> methodStatments)
                => methodStatments;

            public List<Statement> GetOverwrite(MethodConverter methodConverter)
            {
                this.Overwrites++;
                return new List<Statement> { new ReturnStatement(null, methodConverter.Scope, null) };
            }
        }

        /// <summary>
        /// A plugin conversion that says it can replay (e.g. an [AutoFire] accessor) is stored by
        /// the method cache and hit on the next warm build, which equals the cold build. One that
        /// cannot replay (a skin getter) stays uncacheable.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 4 builds.
        public void Session_ReplayablePluginConversion_IsCachedAndHit_EqualsCold()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-plugin-cache-" + Guid.NewGuid().ToString("N"));
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
                foreach (var replayable in new[] { true, false })
                {
                    using var builder = new Builder(
                        outJs,
                        1,
                        main,
                        refs,
                        Array.Empty<IConverterPlugin>(),
                        (minify: false, uglify: false, optimize: false),
                        devMode: true);
                    byte[] Build(OverwritePlugin plugin)
                    {
                        Builder.ResetProcessState();
                        Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin(), plugin }), "Dev-mode build failed.");
                        return File.ReadAllBytes(outJs);
                    }

                    var cold = new OverwritePlugin { Replayable = replayable };
                    var coldJs = Build(cold);
                    var warm = new OverwritePlugin { Replayable = replayable };
                    var warmJs = Build(warm);
                    Assert.AreEqual("warm", builder.LastBuildKind);
                    Assert.IsTrue(cold.Overwrites > 0, "The fixture has no RealScript Main to overwrite.");
                    CollectionAssert.AreEqual(coldJs, warmJs, "replayable=" + replayable + ": the warm build differs from the cold build.");
                    if (replayable)
                    {
                        Assert.AreEqual(0, cold.UncacheablePlugin, "A replayable plugin conversion was not stored.");
                        Assert.AreEqual(0, warm.Overwrites, "A replayable plugin conversion was converted again instead of replayed.");
                        Assert.AreEqual(0, warm.Misses, "The warm build missed the cache.");
                    }
                    else
                    {
                        Assert.AreEqual(cold.Overwrites, cold.UncacheablePlugin, "A plugin conversion that cannot replay must not be stored.");
                        Assert.AreEqual(cold.Overwrites, warm.Overwrites, "A plugin conversion that cannot replay must convert every build.");
                    }
                }
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
