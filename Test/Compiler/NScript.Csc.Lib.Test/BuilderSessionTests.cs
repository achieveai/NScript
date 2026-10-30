namespace NScript.Csc.Lib.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
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

            /// <summary>What every RootsPlugin answers a dev patch, as a skin plugin whose inlined code changed.</summary>
            public static string PatchMiss { get; set; }

            public string TryPatchBodies(IReadOnlyList<TypeDefinition> changedTypes) => PatchMiss;
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

                    // A reference whose resource set changes (a framework rebuild) is cold, and the
                    // reset drops the old context. Then same length and same mtime, different
                    // content (cp -p, restored packages, two writes in one tick): one byte of the
                    // PE TimeDateStamp of the entry DLL. The hash sees it, and with the same MVID
                    // and resources the session takes it as unchanged (#125).
                    static void FlipTimeDateStamp(string path)
                    {
                        var bytes = File.ReadAllBytes(path);
                        bytes[BitConverter.ToInt32(bytes, 0x3C) + 8] ^= 0x5A;
                        var mtime = File.GetLastWriteTimeUtc(path);
                        File.WriteAllBytes(path, bytes);
                        File.SetLastWriteTimeUtc(path, mtime);
                    }

                    SetOracleResource(refs[2], "a new resource");
                    BuildOnce(builder, outJs, out var refChangedJs, out _);
                    Assert.AreEqual("cold", builder.LastBuildKind, "A changed reference DLL must not reuse the session.");
                    CollectionAssert.AreEqual(coldJs, refChangedJs, "An unused resource in a reference must not change the output.");

                    CollectGarbage();
                    Assert.IsFalse(cold.context.IsAlive, "After a reset the session still holds the old ConverterContext.");

                    FlipTimeDateStamp(main);
                    var again = BuildOnce(builder, outJs, out var againJs, out _);
                    Assert.AreEqual("warm", builder.LastBuildKind, "A same-MVID rewrite with the same resources changes nothing.");
                    Assert.AreEqual("resources-unchanged", builder.LastBuildReason);
                    CollectionAssert.AreEqual(coldJs, againJs, "The TimeDateStamp byte must not change the output.");
                    Assert.IsTrue(again.context.IsAlive, "The session must keep its ConverterContext.");
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
        /// the session took the new stamps. A same-MVID change with the same resources is
        /// warm and unchanged (Session_WarmBuildEqualsCold...: TimeDateStamp flip, #125).
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
            => CompileNamed(dir, "lib", refs, files);

        /// <summary>Like <see cref="CompileLib"/>, into <paramref name="assembly"/>.dll.</summary>
        private static string CompileNamed(string dir, string assembly, string[] refs, params (string name, string text)[] files)
        {
            var path = Path.Combine(dir, assembly + ".dll");
            File.Delete(path);
            var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
                assembly,
                files.Select(file => Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(
                    file.text,
                    Microsoft.CodeAnalysis.CSharp.CSharpParseOptions.Default,
                    Path.Combine(dir, file.name),
                    Encoding.UTF8)),
                refs.Select(reference => Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(reference)),
                new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(
                    Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary,
                    concurrentBuild: false));
            Assert.IsNotNull(SerializationHelper.ExpressionVisitMap(compilation, dir, assembly + ".dll"), "The fixture library must compile.");
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
        /// <summary>
        /// The text a patch put before each marker, by marker: a type's name for its section
        /// (<c>/*@T</c>), <c>"G " + name</c> inside a generic type's factory (<c>/*@G</c>).
        /// </summary>
        private static Dictionary<string, string> PatchFragments(string js, string patched, string what)
        {
            var before = MarkerLine.Split(Unstamped(js));
            var after = MarkerLine.Split(Unstamped(patched));
            Assert.AreEqual(before.Length, after.Length, what + ": the patch changed the markers.");
            var fragments = new Dictionary<string, string>();
            for (int i = 0; i < before.Length; i++)
            {
                StringAssert.StartsWith(after[i], before[i], what + ": the patch changed a section's text.");
                if (after[i].Length > before[i].Length)
                {
                    Assert.IsTrue(i + 1 < before.Length, what + ": text after the last marker.");
                    var marker = after[i + 1].Substring(3, after[i + 1].IndexOf("*/", StringComparison.Ordinal) - 3);
                    fragments.Add(
                        marker.StartsWith("T ", StringComparison.Ordinal) ? marker.Substring(2) : marker,
                        after[i].Substring(before[i].Length));
                }
            }

            return fragments;
        }

        // The line break (and a factory's indent) before a type or factory marker.
        private static readonly Regex MarkerLine = new Regex("\r\n *(?=/\\*@[TG] )");

        // The live patch client's boot call: "file|bundle id|url|token|gen:seq". The id hashes
        // the output path and the gen is random per full emit, so a cold build in another
        // folder differs in both; seq counts the patches since the full emit.
        private static readonly Regex BootStamp = new Regex("(__nsHmrBoot\\(\"[^|\"]*\\|)([0-9a-f]{12})(\\|[^|\"]*\\|[^|\"]*\\|)([0-9a-f]{8}:[0-9]+)(\"\\);)");

        private static string Stamp(string bundle)
        {
            var matches = BootStamp.Matches(bundle);
            Assert.AreEqual(1, matches.Count, "A patchable dev bundle has one boot call.");
            return matches[0].Groups[4].Value;
        }

        private static string Unstamped(string bundle) => BootStamp.Replace(bundle, "${1}id${3}gen:seq$5");

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
            // seed's initializer is Helper's static constructor.
            const string Helper =
                "namespace RealScript { public static class Helper { private static int seed = SEED; public static int Say(string text) { return text.Length + seed; } public static int Unused(int value) { return value + BUMP; } } public class Spare { public int Get() { return 7; } } }";
            string Compile(string body, int step, int seed = 1, int bump = 1) => CompileLib(
                dir,
                refs,
                ("Program.cs", ProgramSource(body)),
                ("Helper.cs", Helper.Replace("SEED", seed.ToString()).Replace("BUMP", bump.ToString())),
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
                return Unstamped(File.ReadAllText(coldJs));
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
                    "try{(function(){if(typeof window!==\"undefined\")(window.__nsPatch||(window.__nsPatch={}))[\"bundle.js\"]=function(f){eval(f);};",
                    "A patchable dev bundle registers its live patch hook.");
                StringAssert.Contains(
                    js,
                    "})();}catch(e){if(typeof window!==\"undefined\")(window.__nsHmrFailed||(window.__nsHmrFailed={}))[\"bundle.js\"]=String(e&&e.message||e);throw e;}finally{\r\n(function(){",
                    "A closure that throws while it loads still boots the client, which learns why.");
                StringAssert.Contains(js, "\");\r\n}", "The finally ends after the boot call.");

                // A full emit stamps a new gen at seq 0; each patch keeps the gen and counts on.
                string gen = null;
                int seq = 0;
                void Stamped(string bundle, bool full, string what)
                {
                    var stamp = Stamp(bundle).Split(':');
                    if (full)
                    {
                        Assert.AreNotEqual(gen, stamp[0], what + ": a full emit must start a new gen.");
                        gen = stamp[0];
                        seq = 0;
                    }
                    else
                    {
                        Assert.AreEqual(gen, stamp[0], what + ": a patch keeps the gen.");
                        seq++;
                    }

                    Assert.AreEqual(seq.ToString(), stamp[1], what + ": the stamp must count the patches.");
                }

                string Full(string what)
                {
                    var bundle = File.ReadAllText(outJs);
                    Stamped(bundle, full: true, what);
                    return bundle;
                }

                Stamped(js, full: true, "The first build");

                // What the build service sends open pages: the patch's own gen:seq, and either
                // the script they run or why they must reload.
                LivePatch Live(string what, string reload = null)
                {
                    var live = builder.LastLivePatch;
                    Assert.IsNotNull(live, what + ": no live patch.");
                    Assert.AreEqual(gen + ":" + seq, live.Gen + ":" + live.Seq, what + ": the live patch's stamp.");
                    Assert.AreEqual(reload, live.Reload, what + ": the reload reason.");
                    if (reload == null)
                    {
                        // Not StringAssert: its message formats the braces in the script.
                        var url = "\r\n//# sourceURL=nscript-hmr:///bundle.js/" + gen + "-" + seq + ".js";
                        Assert.AreEqual(url, live.Js.Substring(Math.Max(0, live.Js.Length - url.Length)), what + ": the sourceURL.");

                        // The skins the patch gave new factories build their open controls last, once each.
                        var flush = "globalThis.__nsFlush&&globalThis.__nsFlush();";
                        var body = live.Js.Substring(0, live.Js.Length - url.Length).TrimEnd();
                        Assert.IsTrue(body.EndsWith(flush), what + ": the patch ends with the re-skin flush.");
                    }
                    else
                    {
                        Assert.IsNull(live.Js, what + ": a reload sends no script.");
                    }

                    return live;
                }

                Assert.AreEqual("new build", Live("The first build", "new build").Reload);

                // A patch build converts with the kept plugins, so the new one is never asked.
                Dictionary<string, string> Patched(int methods, string what)
                {
                    Builder.ResetProcessState();
                    Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), what + ": the build failed.");
                    Assert.AreEqual("bodies-refreshed; patched " + methods + " methods", builder.LastBuildReason, what + ": " + builder.LastPatchMiss);
                    var patched = File.ReadAllText(outJs);
                    var fragments = PatchFragments(js, patched, what);
                    Stamped(patched, full: false, what);
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

                // In the page's eval a declaration would stay local to the hook: it is assigned.
                var staticLive = Live("A static body edit");
                Assert.AreEqual(1, staticLive.Methods);
                const string MainAssigned = "RealScript_Program$Main = function RealScript_Program$Main(";
                Assert.AreEqual(MainAssigned, staticLive.Js.TrimStart().Substring(0, MainAssigned.Length));
                Assert.AreEqual(0, staticLive.Notes.Count);

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

                // Next makes a lambda: functions it made before keep the old code.
                var instanceLive = Live("An instance body edit");
                const string SlotAssigned = "RealScript_Counter$.prototype.";
                Assert.AreEqual(SlotAssigned, instanceLive.Js.TrimStart().Substring(0, SlotAssigned.Length));
                CollectionAssert.AreEqual(new[] { "closure Counter.Next" }, instanceLive.Notes.ToList());

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
                Live("A body that calls an instance method the bundle never emitted");

                // RL3b: an initializer that already exists is in the static constructor, which
                // ran when the bundle loaded: the bundle is patched, open pages reload.
                main = Compile("var counter = new Counter(); counter.Reset(); return counter.Next() + Helper.Unused(2) + Helper.Say(\"s\");", 2);
                Patched(1, "A body that calls Say again");
                main = Compile("var counter = new Counter(); counter.Reset(); return counter.Next() + Helper.Unused(2) + Helper.Say(\"s\");", 2, seed: 2);
                Patched(1, "A static initializer edit");
                Live("A static initializer edit", "load-time Helper..cctor");

                // Dev mode binds a virtual method late: a delegate made before the patch reads
                // the new function at its next call, so the page takes the patch live.
                main = Compile("var counter = new Counter(); System.Func<int> next = counter.Next; counter.Reset(); return next() + Helper.Unused(2) + Helper.Say(\"s\");", 2, seed: 2);
                BuildOnce(builder, outJs, out _, out _);
                StringAssert.StartsWith(builder.LastPatchMiss, "member System.Delegate System.Delegate::CreateLateBound(", "The bundle had no Delegate.CreateLateBound yet.");
                js = Full("A body that binds Counter.Next");
                main = Compile("var counter = new Counter(); System.Func<int> next = counter.Next; counter.Reset(); return next() + Helper.Unused(2) + Helper.Say(\"s\");", 3, seed: 2);
                Patched(1, "An edit to a late-bound method");
                Live("An edit to a late-bound method");

                // A delegate to a static is the function itself: it keeps the old code, noted.
                const string StoresUnused = "var counter = new Counter(); System.Func<int> next = counter.Next; System.Func<int, int> bump = Helper.Unused; counter.Reset(); return next() + bump(2) + Helper.Say(\"s\");";
                main = Compile(StoresUnused, 3, seed: 2);
                Patched(1, "A body that stores Helper.Unused");
                main = Compile(StoresUnused, 3, seed: 2, bump: 2);
                Patched(1, "An edit to a stored static");
                CollectionAssert.AreEqual(new[] { "stored Helper.Unused" }, Live("An edit to a stored static").Notes.ToList());

                main = Compile("return new Spare().Get();", 2);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("bodies-refreshed", builder.LastBuildReason, "A body that needs a new type must emit in full.");
                js = Full("The full emit after a miss");
                Assert.AreEqual(Cold(main), Unstamped(js), "The full emit after a miss differs from a cold build.");
                Live("The full emit after a miss", "new build");

                main = Compile("return new Spare().Get() + 1;", 2);
                fragments = Patched(1, "A body edit after a miss");
                CollectionAssert.AreEquivalent(new[] { "RealScript.Program" }, fragments.Keys.ToList());

                // A plugin that wrote a body's old code into its own output (a skin's inlined
                // getter) misses the patch: the build emits in full.
                RootsPlugin.PatchMiss = "razor inlined getter RealScript.Spare.Get";
                main = Compile("return new Spare().Get() + 2;", 2);
                Builder.ResetProcessState();
                Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), "A build after a plugin miss failed.");
                RootsPlugin.PatchMiss = null;
                Assert.AreEqual("razor inlined getter RealScript.Spare.Get", builder.LastPatchMiss, "The plugin's miss must stop the patch.");
                js = Full("The full emit after a plugin miss");
                Assert.AreEqual(Cold(main), Unstamped(js), "The full emit after a plugin miss differs from a cold build.");

                main = Compile("return new Spare().Get() + new Counter(5).Next();", 2);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("bodies-refreshed", builder.LastBuildReason, "A body that needs a new constructor factory must emit in full.");
                Assert.IsNotNull(builder.LastPatchMiss, "A new constructor overload must miss.");

                // Compaction: once the patches grow the bundle past the limit, the next build
                // emits in full and drops the old code.
                js = Full("The full emit after a factory miss");
                Assert.AreEqual(Cold(main), Unstamped(js), "The full emit after a factory miss differs from a cold build.");

                // A closed generic the bundle saw only as a declaring type has no type variable
                // yet: only a full emit writes it.
                main = Compile("List<string> none = null; return new Spare().Get() + new Counter(5).Next() + new List<int>().Count + (none == null ? 0 : none.Count);", 2);
                BuildOnce(builder, outJs, out _, out _);
                main = Compile("return new Spare().Get() + new Counter(5).Next() + new List<int>().Count + new List<string>().Count;", 2);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("bodies-refreshed", builder.LastBuildReason, "A body that constructs a new closed generic must emit in full.");
                StringAssert.StartsWith(builder.LastPatchMiss, "new root variable ");
                js = Full("The full emit after a closed-generic miss");
                Assert.AreEqual(Cold(main), Unstamped(js), "The full emit after a closed-generic miss differs from a cold build.");
                main = Compile("return new Spare().Get() + new Counter(6).Next();", 2);
                fragments = Patched(1, "A body edit before compaction");
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT", "0");
                main = Compile("return new Spare().Get() + new Counter(7).Next();", 2);
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("compaction", builder.LastPatchMiss);
                Assert.AreEqual(Cold(main), Unstamped(Full("The compaction emit")), "The compaction emit differs from a cold build.");

                // A body edit and a resource edit in one image (one watch batch): the plugins
                // are asked for their sections too. RootsPlugin has none, so it emits in full.
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT", null);
                main = Compile("return new Spare().Get() + new Counter(8).Next();", 2);
                SetOracleResource(main, "v2");
                BuildOnce(builder, outJs, out _, out _);
                Assert.AreEqual("plugin RootsPlugin", builder.LastPatchMiss, "A resource changed with the code must reach the plugins.");
                Assert.AreEqual(Cold(main), Unstamped(Full("The full emit after a resource miss")), "The full emit after a resource miss differs from a cold build.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", previous);
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_COMPACT_PERCENT", null);
                RootsPlugin.PatchMiss = null;
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// S3: a generic type is a factory; each instantiation builds its prototype in its own
        /// closure. A body edit goes before the marker inside the factory, as the cold factory
        /// writes the method's function and its interface slot copies, so every instantiation
        /// and every later load runs it. The live form runs the same text in each
        /// instantiation's closure and keeps it for later ones. A body that needs a closure
        /// local the factory never declared misses and emits in full; a static initializer
        /// edit patches the bundle and reloads open pages. Without patch emit the factory has
        /// no hook.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 8 builds of a small library.
        public void Session_PatchEmit_GenericTypeBodyEdit_PatchesInsideItsFactory()
        {
            TestAssemblyLoader.LoadAssemblies();
            // Not StringAssert: its message formats the braces in the script. A miss keeps
            // the text for a look.
            static void Has(string text, string part, string what)
            {
                if (!text.Contains(part, StringComparison.Ordinal))
                {
                    var keep = Path.Combine(Path.GetTempPath(), "nscript-generic-patch-miss.js");
                    File.WriteAllText(keep, text);
                    Assert.Fail((what + ": missing " + part + " in " + keep).Replace("{", "(").Replace("}", ")"));
                }
            }

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
            var plainJs = Path.Combine(Directory.CreateDirectory(Path.Combine(dir, "plain")).FullName, "bundle.js");
            const string Bag =
                "namespace RealScript { public interface IBag<T> { void Put(T item); int Size(); } public class Bag<T> : IBag<T> { private static int baseline = BASE; private System.Collections.Generic.List<T> items = new System.Collections.Generic.List<T>(); public void Put(T item) { PUT } public int Size() { return items.Count + GROW + baseline; } public static int Twice(int value) { return value * TWICE; } } }";
            string Compile(int grow = 1, int twice = 2, int baseline = 1, string put = "items.Add(item);") => CompileLib(
                dir,
                refs,
                ("Program.cs", ProgramSource("IBag<int> bag = new Bag<int>(); bag.Put(1); return bag.Size() + new Bag<string>().Size() + Bag<int>.Twice(1) + new List<int>().ToArray().Length;")),
                ("Bag.cs", Bag.Replace("GROW", grow.ToString()).Replace("TWICE", twice.ToString()).Replace("BASE", baseline.ToString()).Replace("PUT", put)));

            string Build(string js, string main)
            {
                using var fresh = new Builder(
                    js,
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                BuildOnce(fresh, js, out _, out _);
                return Unstamped(File.ReadAllText(js));
            }

            string Cold(string main) => Build(coldJs, main);

            var previous = Environment.GetEnvironmentVariable("NSCRIPT_PATCH_EMIT");
            Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", null);
            var main = Compile();
            try
            {
                var plain = Build(plainJs, main);
                Has(plain, "Bag$1", "The plain dev bundle has the generic factory.");
                Assert.IsFalse(plain.Contains("__nsEval") || plain.Contains("/*@G "), "Without patch emit a factory has no hook.");

                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", "1");
                using var builder = new Builder(
                    outJs,
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                BuildOnce(builder, outJs, out _, out _);
                var js = File.ReadAllText(outJs);
                Assert.AreEqual(1, Regex.Matches(js, "\r\n +/\\*@G RealScript\\.Bag`1\\*/\r\n").Count, "The factory has its marker on its own line.");
                Has(js, ".__nsEval = function(){return eval(arguments[0]);};", "The eval hook.");
                Has(js, ".__nsInsts = [])).push(", "The instantiation list.");
                Has(js, ".__nsPatches.forEach(", "The replay.");

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

                // An instance method and its interface slot copy, written with the closure's
                // type variable: the cold factory writes them with its prototype variable.
                main = Compile(grow: 2);
                var fragments = Patched(1, "A generic instance body edit");
                CollectionAssert.AreEquivalent(new[] { "G RealScript.Bag`1" }, fragments.Keys.ToList());
                var fragment = fragments["G RealScript.Bag`1"];
                var prototype = fragment.Substring(2, fragment.IndexOf(".prototype", StringComparison.Ordinal) + ".prototype".Length - 2);
                var parts = fragment.Split("\r\n" + prototype);
                Assert.AreEqual(3, parts.Length, "The method and its one interface slot copy: " + fragment);
                // The factory indents its body one level deeper than a fragment.
                static string Flat(string text) => Regex.Replace(text, @"\s+", " ");
                var cold = Flat(Cold(main));
                Has(cold, Flat("$$ptyp" + parts[1]), "The method must be the cold factory's.");
                Has(cold, Flat("$$ptyp" + parts[2].Replace(prototype, "$$ptyp")), "The interface slot copy must be the cold factory's.");
                var live = builder.LastLivePatch;
                Assert.IsNull(live.Reload, "A generic body edit goes live.");
                Has(live.Js, ".__nsPatches = [])).push(\"", "The live push.");
                Has(live.Js, ".__nsInsts = [])).forEach(function(t){t[\"__nsEval\"](\"", "The live run per instantiation.");
                Has(live.Js, NScript.JST.Utils.ToJSString(prototype + parts[1]), "Each instantiation runs the method's text.");

                // A static is assigned per instantiation.
                main = Compile(grow: 2, twice: 3);
                fragments = Patched(1, "A generic static body edit");
                fragment = fragments["G RealScript.Bag`1"];
                var typeVariable = prototype.Substring(0, prototype.Length - ".prototype".Length);
                StringAssert.StartsWith(fragment, "\r\n" + typeVariable + ".");
                Has(Flat(Cold(main)), Flat(fragment.Substring(2)), "The static must be the cold factory's.");
                Assert.IsNull(builder.LastLivePatch.Reload, "A generic static body edit goes live.");

                // G7: the static constructor ran when the factory first ran: open pages reload.
                main = Compile(grow: 2, twice: 3, baseline: 2);
                Patched(1, "A generic static initializer edit");
                Assert.AreEqual("load-time Bag`1..cctor", builder.LastLivePatch.Reload);

                // G3: a type the factory never declared a local for (T[], though List<T> has
                // one): only a full emit adds it. Each closure's T is its own type, so the new
                // type is the miss; a new closure local by any other way is one too.
                main = Compile(grow: 2, twice: 3, baseline: 2, put: "items.Add(item); T[] one = new T[] { item };");
                Builder.ResetProcessState();
                Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), "A build after a generic local miss failed.");
                Assert.AreEqual("type System.ArrayG`1<T>", builder.LastPatchMiss, "The generic local miss.");
                Assert.AreEqual(Cold(main), Unstamped(File.ReadAllText(outJs)), "The full emit after a generic local miss differs from a cold build.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", previous);
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
        /// #125: a watch batch that ran after another batch already patched the bundle finds
        /// nothing new. With a patch session that build writes nothing and sends nothing: no new
        /// gen, so open pages do not reload. That holds for inputs with the same stamps, and for
        /// a rewritten entry DLL with the same MVID and resources (a resource patch that wrote
        /// the same bytes again). A bundle changed on disk still emits in full, and a later
        /// body edit still patches on the same gen.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 6 builds of a small library.
        public void Session_PatchEmit_NothingChanged_WritesNothing_KeepsGen()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-nochange-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            var refs = new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") };
            var outJs = Path.Combine(dir, "bundle.js");
            string Compile(string text) => CompileLib(dir, refs, ("Program.cs", ProgramSource("return \"" + text + "\".Length;")));

            var previous = Environment.GetEnvironmentVariable("NSCRIPT_PATCH_EMIT");
            Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", "1");
            var main = Compile("v1");
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
                void Build(string what)
                {
                    Builder.ResetProcessState();
                    Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), what + ": the build failed.");
                }

                Build("The first build");
                Assert.AreEqual("cold", builder.LastBuildKind);
                var gen = Stamp(File.ReadAllText(outJs)).Split(':')[0];

                Compile("v2");
                Build("A body edit");
                Assert.AreEqual("bodies-refreshed; patched 1 methods", builder.LastBuildReason, builder.LastPatchMiss);
                var patched = File.ReadAllText(outJs);
                Assert.AreEqual(gen + ":1", Stamp(patched));

                void Nothing(string reason, string what)
                {
                    var written = File.GetLastWriteTimeUtc(outJs);
                    Build(what);
                    Assert.AreEqual("warm", builder.LastBuildKind, what);
                    Assert.AreEqual(reason + "; nothing to emit", builder.LastBuildReason, what);
                    Assert.IsNull(builder.LastLivePatch, what + ": open pages must get nothing, so they keep the gen.");
                    Assert.AreEqual(patched, File.ReadAllText(outJs), what + ": the bundle must stay as the patch wrote it.");
                    Assert.AreEqual(written, File.GetLastWriteTimeUtc(outJs), what + ": the bundle must not be written again.");
                }

                Nothing("unchanged", "A build with the same inputs");

                // Same MVID and resources, other bytes: the PE TimeDateStamp.
                var bytes = File.ReadAllBytes(main);
                bytes[BitConverter.ToInt32(bytes, 0x3C) + 8] ^= 0x5A;
                File.WriteAllBytes(main, bytes);
                Nothing("resources-unchanged", "A rewritten DLL with the same MVID and resources");

                Compile("v3");
                Build("A body edit after builds with nothing to emit");
                Assert.AreEqual("bodies-refreshed; patched 1 methods", builder.LastBuildReason, builder.LastPatchMiss);
                patched = File.ReadAllText(outJs);
                Assert.AreEqual(gen + ":2", Stamp(patched), "The patch must keep the gen and count on.");

                File.AppendAllText(outJs, "\r\n");
                Build("A build after the bundle changed on disk");
                Assert.AreEqual("unchanged", builder.LastBuildReason, "A bundle changed on disk must be emitted in full.");
                Assert.AreEqual("new build", builder.LastLivePatch?.Reload);
                Assert.AreNotEqual(gen, Stamp(File.ReadAllText(outJs)).Split(':')[0], "A full emit starts a new gen.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", previous);
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// One watch batch where a reference is recompiled (a second watch rebuilt the shared
        /// framework) and the entry DLL takes a resource patch (a CSS save). The patch must
        /// give the resource change to the plugins, not only patch the changed bodies: before
        /// the fix the CSS change was dropped and the page kept the old style. RootsPlugin
        /// cannot patch resources, so here the change reaching it is a miss and a full emit.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 2 builds of two small libraries.
        public void Session_PatchEmit_RecompiledReferenceAndResourcePatch_PatchesResources()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-mixed-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            var refs = new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") };
            var outJs = Path.Combine(dir, "bundle.js");
            string CompileHelper(string source) => CompileNamed(dir, "helper", refs, ("Helper.cs", source));

            var previous = Environment.GetEnvironmentVariable("NSCRIPT_PATCH_EMIT");
            Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", "1");
            var helper = CompileHelper(HelperSource);
            var appRefs = refs.Append(helper).ToArray();
            var main = CompileLib(dir, appRefs, ("Program.cs", ProgramSource("return Helper.Say(\"v1\");")));
            SetOracleResource(main, "v1");
            try
            {
                using var builder = new Builder(
                    outJs,
                    1,
                    main,
                    appRefs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                void Build(string what)
                {
                    Builder.ResetProcessState();
                    Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), what + ": the build failed.");
                }

                Build("The first build");
                Assert.AreEqual("cold", builder.LastBuildKind);

                CompileHelper(HelperSource.Replace("return text.Length;", "return text.Length + 1;"));
                SetOracleResource(main, "v2");
                Build("A recompiled reference and a resource patch in one batch");
                Assert.AreEqual("warm", builder.LastBuildKind);
                Assert.AreEqual("plugin RootsPlugin", builder.LastPatchMiss, "The resource change must reach the plugins.");
                Assert.AreEqual("v2", RootsPlugin.LastOracleText);
            }
            finally
            {
                Environment.SetEnvironmentVariable("NSCRIPT_PATCH_EMIT", previous);
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
        /// So does an unchanged method with no prototype slot to assign ([MakeStaticUsage]).
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
                ("Program.cs", ProgramSource("var view = new View(); view.Skin(); return view.Next() + view.Twice();")),
                ("View.cs", "namespace RealScript { public class View { private int value; public void Skin() { value = 9; } public int Next() { value = value + " + step + "; return value; } [System.Runtime.CompilerServices.MakeStaticUsage] public int Twice() { return value * 2; } } }"));

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
                var plugin = new OverwritePlugin { MethodName = "Skin", WarnOnAsk = true };
                Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin(), plugin }), "The full build failed.");
                Assert.AreEqual(1, plugin.Overwrites, "The plugin must write Skin.");
                var full = File.ReadAllText(outJs);

                Compile(2);
                Builder.ResetProcessState();
                var console = Console.Out;
                var output = new StringWriter();
                Console.SetOut(output);
                try
                {
                    Assert.IsTrue(builder.Execute(new IConverterPlugin[] { new RootsPlugin() }), "The patch build failed.");
                }
                finally
                {
                    Console.SetOut(console);
                }

                Assert.AreEqual("bodies-refreshed; patched 1 methods", builder.LastBuildReason, builder.LastPatchMiss);

                // The full build warned for every method; the patch reports the warnings of
                // the one it converted, not what is left past the full build's count.
                StringAssert.Contains(output.ToString(), "View.cs(1,1): warning WRN0123: asked Next");
                Assert.IsFalse(output.ToString().Contains("asked Main"), "Only the patch's own warnings: " + output);
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

            /// <summary>Adds a warning naming each RealScript method it is asked about.</summary>
            public bool WarnOnAsk { get; set; }

            public IntrestLevel GetInterestLevel(MethodDefinition methodDefinition, ConverterContext converterContext)
            {
                if (methodDefinition.DeclaringType.Namespace != "RealScript")
                {
                    return IntrestLevel.None;
                }

                if (this.WarnOnAsk)
                {
                    converterContext.AddError(new Location("View.cs", 1, 1), "asked " + methodDefinition.Name, true);
                }

                return methodDefinition.Name == this.MethodName ? IntrestLevel.Overwrite : IntrestLevel.None;
            }

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

        /// <summary>
        /// A plugin that asks for a retry on every attempt: the build stops after 2 retries and
        /// fails with ERR0124, as the attempt stopped before printing any error.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")] // ~17 s fixture setup plus 3 attempts.
        public void Execute_PluginAsksForARetryEveryAttempt_FailsWithErr0124AfterTwoRetries()
        {
            TestAssemblyLoader.LoadAssemblies();
            var temp = TestResources.FixtureDirectory;
            var dir = Path.Combine(temp, "nscript-retry-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            string Copy(string name)
            {
                var target = Path.Combine(dir, name);
                File.Copy(Path.Combine(temp, name), target);
                return target;
            }

            var main = Copy("realScript.dll");
            var refs = new[] { Copy("mscorlib.dll"), Copy("system.core.dll"), Copy("microsoft.csharp.dll") };
            var console = Console.Out;
            var output = new StringWriter();
            try
            {
                using var builder = new Builder(
                    Path.Combine(dir, "bundle.js"),
                    1,
                    main,
                    refs,
                    Array.Empty<IConverterPlugin>(),
                    (minify: false, uglify: false, optimize: false),
                    devMode: true);
                Builder.ResetProcessState();
                var plugin = new RetryPlugin();
                Console.SetOut(output);
                Assert.IsFalse(builder.Execute(new IConverterPlugin[] { new RootsPlugin(), plugin }), "A build that always asks for a retry must fail.");
                Console.SetOut(console);
                Assert.AreEqual(3, plugin.Attempts, "The first attempt and 2 retries.");
                StringAssert.Contains(output.ToString(), "(0,0): error ERR0124: the build still asked for a retry after 2 retries.");
            }
            finally
            {
                Console.SetOut(console);
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>Asks for a retry at the end of every attempt.</summary>
        private sealed class RetryPlugin : IRuntimeConverterPlugin
        {
            private RuntimeScopeManager runtimeScopeManager;

            public int Attempts { get; private set; }

            public void Initialize(ClrContext clrContext, RuntimeScopeManager runtimeScopeManager)
                => this.runtimeScopeManager = runtimeScopeManager;

            public void ParseArgs(IList<Tuple<string, string>> args) { }

            public List<MethodReference> GetMethodsToEmitPass1() => null;

            public List<MethodReference> GetMethodsToEmitPassN() => null;

            public List<Statement> GetPreJavascript() => null;

            public List<Statement> GetPostJavascript()
            {
                this.Attempts++;
                this.runtimeScopeManager.Context.RequestRetry("always");
                return null;
            }
        }
    }
}
