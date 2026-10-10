namespace NScript.Utils.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata;
    using System.Reflection.PortableExecutable;
    using System.Text;
    using System.Text.Json;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.CLR;
    using NScript.Csc.Lib.Service;
    using NScript.Lib.Service;
    using Cecil = Mono.Cecil;

    /// <summary>
    /// Contracts of the watch-mode resource patch (M3-1) on a real Roslyn DLL shaped like a
    /// stage-1 output: deterministic, a portable PDB beside it, a nested type and a lambda
    /// (so a Cecil rewrite renumbers rows), $$BstInfo$$, $$ResInfo$$ and two mapped files.
    /// </summary>
    [TestClass]
    public class ResourcePatcherTests
    {
        private const string Source = @"namespace Fx
{
    public class Outer
    {
        public class Inner { public int V; }
        public System.Func<int, int> Make(int k) => x => x + k;
        public string Text() => ""fixture"";
    }

    public class Second { public int M() => new Outer.Inner().V; }
}";

        private static readonly byte[] BstInfo = Encoding.UTF8.GetBytes("fake bound tree");

        private string dir;
        private string dll;
        private string pdb;
        private string refint;
        private string skin;
        private string css;

        [ClassInitialize]
        public static void WarmUp(TestContext context)
        {
            // The first Roslyn emit in a process JITs the compiler; keep it out of the tests.
            var warmup = Path.Combine(Path.GetTempPath(), "nscript-patch-warmup-" + Guid.NewGuid().ToString("N"));
            Emit(warmup, new Dictionary<string, string>(), out _, out _);
            Directory.Delete(warmup, recursive: true);
        }

        [TestInitialize]
        public void Setup()
        {
            this.dir = Path.Combine(Path.GetTempPath(), "nscript-patch-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(this.dir, "refint"));
            this.skin = Path.Combine(this.dir, "Shell.skin.cshtml");
            this.css = Path.Combine(this.dir, "Site.css");
            File.WriteAllText(this.skin, "<div>v1</div>");
            File.WriteAllText(this.css, ".a { color: red; }");
            var map = new Dictionary<string, string> { ["Fx.Shell.skin.cshtml"] = this.skin, ["Fx.Site.css"] = this.css };
            Emit(this.dir, map, out this.dll, out this.pdb);
            this.refint = Path.Combine(this.dir, "refint", "Fx.dll");
            File.Copy(this.dll, this.refint);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(this.dir))
            {
                Directory.Delete(this.dir, recursive: true);
            }
        }

        /// <summary>
        /// An edited skin replaces only its resource; MVID, $$BstInfo$$ and the method set stay;
        /// the debug directory loses the PDB link (exactly one Reproducible, also after a second
        /// patch); PDB and refint are not touched; no temp file is left; the recorded hashes are
        /// the watch probe's hashes of the embedded files.
        /// </summary>
        [TestMethod]
        public void Patch_EditedSkin_ReplacesOnlyIt_KeepsIdentity_DropsPdbLink_TouchesNothingElse()
        {
            var before = Image.Read(this.dll);
            CollectionAssert.AreEqual(new[] { "CodeView", "PdbChecksum", "Reproducible" }, before.Debug, "Fixture is not shaped like a stage-1 output.");
            var pdbBytes = File.ReadAllBytes(this.pdb);
            var pdbTime = File.GetLastWriteTimeUtc(this.pdb);
            var refintBytes = File.ReadAllBytes(this.refint);
            var refintTime = File.GetLastWriteTimeUtc(this.refint);

            File.WriteAllText(this.skin, "<div>v2</div>");
            var result = ResourcePatcher.Patch(this.dll, new[] { this.skin, this.css });

            Assert.AreEqual(ResourcePatchOutcome.Ok, result.Outcome, result.Reason);
            CollectionAssert.AreEqual(new[] { "Fx.Shell.skin.cshtml" }, result.Replaced.Select(r => r.Name).ToArray());
            var after = Image.Read(this.dll);
            Assert.AreEqual(before.Mvid, after.Mvid);
            CollectionAssert.AreEqual(before.Methods, after.Methods);
            CollectionAssert.AreEqual(before.Resources.Keys.ToArray(), after.Resources.Keys.ToArray());
            CollectionAssert.AreEqual(BstInfo, after.Resources["$$BstInfo$$"]);
            CollectionAssert.AreEqual(before.Resources["$$ResInfo$$"], after.Resources["$$ResInfo$$"]);
            CollectionAssert.AreEqual(File.ReadAllBytes(this.skin), after.Resources["Fx.Shell.skin.cshtml"]);
            CollectionAssert.AreEqual(File.ReadAllBytes(this.css), after.Resources["Fx.Site.css"]);
            CollectionAssert.AreEqual(new[] { "Reproducible" }, after.Debug);
            Assert.AreEqual(CompileInputs.HashFile(this.skin), result.Hashes[this.skin]);
            Assert.AreEqual(CompileInputs.HashFile(this.css), result.Hashes[this.css]);

            File.WriteAllText(this.css, ".a { color: blue; }");
            Assert.AreEqual(ResourcePatchOutcome.Ok, ResourcePatcher.Patch(this.dll, new[] { this.css }).Outcome);
            var second = Image.Read(this.dll);
            CollectionAssert.AreEqual(new[] { "Reproducible" }, second.Debug, "A second patch must not add a Reproducible entry.");
            CollectionAssert.AreEqual(File.ReadAllBytes(this.css), second.Resources["Fx.Site.css"]);
            CollectionAssert.AreEqual(File.ReadAllBytes(this.skin), second.Resources["Fx.Shell.skin.cshtml"]);

            CollectionAssert.AreEqual(pdbBytes, File.ReadAllBytes(this.pdb));
            Assert.AreEqual(pdbTime, File.GetLastWriteTimeUtc(this.pdb));
            CollectionAssert.AreEqual(refintBytes, File.ReadAllBytes(this.refint));
            Assert.AreEqual(refintTime, File.GetLastWriteTimeUtc(this.refint));
            CollectionAssert.AreEqual(new[] { this.dll }, Directory.GetFiles(this.dir, "Fx.dll*"));
        }

        /// <summary>Files equal to the embedded bytes: Ok, nothing replaced, the DLL is not rewritten.</summary>
        [TestMethod]
        public void Patch_NoChange_LeavesDllUntouched()
        {
            var bytes = File.ReadAllBytes(this.dll);
            var time = File.GetLastWriteTimeUtc(this.dll);

            var result = ResourcePatcher.Patch(this.dll, new[] { this.skin, this.css });

            Assert.AreEqual(ResourcePatchOutcome.Ok, result.Outcome, result.Reason);
            Assert.AreEqual(0, result.Replaced.Count);
            Assert.AreEqual(2, result.Hashes.Count);
            CollectionAssert.AreEqual(bytes, File.ReadAllBytes(this.dll));
            Assert.AreEqual(time, File.GetLastWriteTimeUtc(this.dll));
        }

        /// <summary>A missing DLL or a resource input $$ResInfo$$ does not map falls back to a compile, writing nothing.</summary>
        [TestMethod]
        public void Patch_MissingDllOrUnmappedInput_FallsBack()
        {
            var bytes = File.ReadAllBytes(this.dll);
            File.WriteAllText(this.skin, "<div>v2</div>");

            var missing = ResourcePatcher.Patch(Path.Combine(this.dir, "Nope.dll"), new[] { this.skin });
            var unmapped = ResourcePatcher.Patch(this.dll, new[] { this.skin, Path.Combine(this.dir, "New.css") });

            Assert.AreEqual(ResourcePatchOutcome.Fallback, missing.Outcome);
            Assert.AreEqual(ResourcePatchOutcome.Fallback, unmapped.Outcome);
            StringAssert.Contains(unmapped.Reason, "New.css");
            CollectionAssert.AreEqual(bytes, File.ReadAllBytes(this.dll));
        }

        /// <summary>
        /// F-F: the patch rewrites the DLL without a signing key, so a strong-name signed DLL
        /// falls back to a compile, writing nothing.
        /// </summary>
        [TestMethod]
        public void Patch_StrongNameSignedDll_FallsBack()
        {
            using (var signed = Cecil.ModuleDefinition.ReadModule(new MemoryStream(File.ReadAllBytes(this.dll))))
            {
                signed.Attributes |= Cecil.ModuleAttributes.StrongNameSigned;
                signed.Assembly.Name.PublicKey = Enumerable.Range(0, 160).Select(i => (byte)i).ToArray();
                signed.Write(this.dll);
            }

            var bytes = File.ReadAllBytes(this.dll);
            File.WriteAllText(this.skin, "<div>v2</div>");

            var result = ResourcePatcher.Patch(this.dll, new[] { this.skin });

            Assert.AreEqual(ResourcePatchOutcome.Fallback, result.Outcome);
            StringAssert.StartsWith(result.Reason, "strong-name signed");
            CollectionAssert.AreEqual(bytes, File.ReadAllBytes(this.dll));
        }

        /// <summary>A DLL held open without delete sharing (a reader): IoFailed, the DLL is unchanged, no temp file is left.</summary>
        [TestMethod]
        public void Patch_LockedDll_IoFailed_DllUnchanged_NoTemp()
        {
            var bytes = File.ReadAllBytes(this.dll);
            File.WriteAllText(this.skin, "<div>v2</div>");

            ResourcePatchResult result;
            using (new FileStream(this.dll, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                result = ResourcePatcher.Patch(this.dll, new[] { this.skin });
            }

            Assert.AreEqual(ResourcePatchOutcome.IoFailed, result.Outcome);
            CollectionAssert.AreEqual(bytes, File.ReadAllBytes(this.dll));
            CollectionAssert.AreEqual(new[] { this.dll }, Directory.GetFiles(this.dir, "Fx.dll*"));
        }

        /// <summary>
        /// Build-session refresh (M3 slice 3, L1) on a loaded module. A patched image gives the
        /// module its new resources, and so does the patch that reverts it. Nothing changes, and
        /// the reason says why, for: the same image again, a recompile (new MVID), a changed
        /// $$BstInfo$$, an added, renamed or re-attributed resource (all with the MVID kept, as a
        /// Cecil rewrite does), or an image of a module that is not loaded.
        /// </summary>
        [TestMethod]
        public void Refresh_TakesPatchedResources_RefusesRecompileBstInfoOrResourceSetChange()
        {
            using var clr = new ClrContext();
            clr.LoadAssembly(this.dll);
            Assert.IsTrue(clr.TryGetModuleDefinition("Fx", out var module));
            var original = File.ReadAllBytes(this.skin);

            File.WriteAllText(this.skin, "<div>v2</div>");
            Assert.AreEqual(ResourcePatchOutcome.Ok, ResourcePatcher.Patch(this.dll, new[] { this.skin }).Outcome);
            var patched = File.ReadAllBytes(this.dll);
            Assert.IsTrue(clr.TryRefreshResources(new[] { patched }, out var replaced, out var reason), reason);
            Assert.AreEqual(1, replaced);
            CollectionAssert.AreEqual(File.ReadAllBytes(this.skin), Resource(module, "Fx.Shell.skin.cshtml"));
            CollectionAssert.AreEqual(BstInfo, Resource(module, "$$BstInfo$$"));

            void Refused(byte[] image, string expected)
            {
                Assert.IsFalse(clr.TryRefreshResources(new[] { image }, out var none, out var why), expected);
                Assert.AreEqual(0, none);
                StringAssert.StartsWith(why, expected);
                CollectionAssert.AreEqual(File.ReadAllBytes(this.skin), Resource(module, "Fx.Shell.skin.cshtml"), expected);
            }

            Refused(patched, "no-resource-change");

            var map = new Dictionary<string, string> { ["Fx.Shell.skin.cshtml"] = this.skin, ["Fx.Site.css"] = this.css };
            Emit(Path.Combine(this.dir, "recompiled"), map, out var recompiled, out _, Source.Replace("fixture", "fixture 2"));
            Refused(File.ReadAllBytes(recompiled), "mvid");

            static byte[] Rewrite(byte[] image, Action<Cecil.ModuleDefinition> edit)
            {
                using var rewritten = Cecil.ModuleDefinition.ReadModule(new MemoryStream(image));
                edit(rewritten);
                var output = new MemoryStream();
                rewritten.Write(output);
                return output.ToArray();
            }

            var bstChanged = Rewrite(patched, m =>
            {
                var index = m.Resources.IndexOf(m.Resources.Single(r => r.Name == "$$BstInfo$$"));
                m.Resources[index] = new Cecil.EmbeddedResource("$$BstInfo$$", m.Resources[index].Attributes, Encoding.UTF8.GetBytes("other tree"));
                var skinIndex = m.Resources.IndexOf(m.Resources.Single(r => r.Name == "Fx.Shell.skin.cshtml"));
                m.Resources[skinIndex] = new Cecil.EmbeddedResource("Fx.Shell.skin.cshtml", m.Resources[skinIndex].Attributes, Encoding.UTF8.GetBytes("<div>v3</div>"));
            });
            Refused(bstChanged, "$$BstInfo$$");

            var added = Rewrite(patched, m => m.Resources.Add(new Cecil.EmbeddedResource("Fx.New.css", Cecil.ManifestResourceAttributes.Public, Encoding.UTF8.GetBytes(".n {}"))));
            Refused(added, "resource-set");

            static void Replace(Cecil.ModuleDefinition m, string name, Func<Cecil.EmbeddedResource, Cecil.EmbeddedResource> make)
            {
                var index = m.Resources.IndexOf(m.Resources.Single(r => r.Name == name));
                m.Resources[index] = make((Cecil.EmbeddedResource)m.Resources[index]);
            }

            var renamed = Rewrite(patched, m => Replace(m, "Fx.Site.css", r => new Cecil.EmbeddedResource("Fx.Other.css", r.Attributes, r.GetResourceData())));
            Refused(renamed, "resource-set");

            var reattributed = Rewrite(patched, m => Replace(m, "Fx.Site.css", r => new Cecil.EmbeddedResource(r.Name, r.Attributes ^ Cecil.ManifestResourceAttributes.Public ^ Cecil.ManifestResourceAttributes.Private, r.GetResourceData())));
            Refused(reattributed, "resource-set");

            var otherModule = Rewrite(patched, m => m.Name = "Fy.dll");
            Refused(otherModule, "not-loaded");

            File.WriteAllBytes(this.skin, original);
            Assert.AreEqual(ResourcePatchOutcome.Ok, ResourcePatcher.Patch(this.dll, new[] { this.skin }).Outcome);
            Assert.IsTrue(clr.TryRefreshResources(new[] { File.ReadAllBytes(this.dll) }, out replaced, out reason), reason);
            CollectionAssert.AreEqual(original, Resource(module, "Fx.Shell.skin.cshtml"));
        }

        private static byte[] Resource(Cecil.ModuleDefinition module, string name)
            => module.Resources.OfType<Cecil.EmbeddedResource>().Single(r => r.Name == name).GetResourceData();

        /// <summary>
        /// Writes at <paramref name="dllPath"/> a stage-1-shaped DLL embedding the mapped files,
        /// whose code says <paramref name="code"/>: another <paramref name="code"/>, another MVID.
        /// </summary>
        internal static void EmitAt(string dllPath, string code, Dictionary<string, string> map)
        {
            var temp = Path.Combine(Path.GetTempPath(), "nscript-emit-" + Guid.NewGuid().ToString("N"));
            Emit(temp, map, out var emitted, out _, Source.Replace("fixture", code, StringComparison.Ordinal));
            Directory.CreateDirectory(Path.GetDirectoryName(dllPath));
            File.Copy(emitted, dllPath, overwrite: true);
            Directory.Delete(temp, recursive: true);
        }

        /// <summary>Compiles <see cref="Source"/> into <paramref name="dir"/>/Fx.dll + Fx.pdb, embedding the mapped files and $$ResInfo$$.</summary>
        private static void Emit(string dir, Dictionary<string, string> map, out string dllPath, out string pdbPath, string source = Source, params string[] references)
        {
            Directory.CreateDirectory(dir);
            dllPath = Path.Combine(dir, "Fx.dll");
            pdbPath = Path.Combine(dir, "Fx.pdb");
            var resources = new List<ResourceDescription>
            {
                new ResourceDescription("$$BstInfo$$", () => new MemoryStream(BstInfo), isPublic: true),
                new ResourceDescription("$$ResInfo$$", () => new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(map)), isPublic: true),
            };
            foreach (var pair in map)
            {
                var bytes = File.ReadAllBytes(pair.Value);
                resources.Add(new ResourceDescription(pair.Key, () => new MemoryStream(bytes), isPublic: true));
            }

            var compilation = CSharpCompilation.Create(
                "Fx",
                new[] { CSharpSyntaxTree.ParseText(source) },
                references.Append(typeof(object).Assembly.Location).Select(path => MetadataReference.CreateFromFile(path)),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, deterministic: true));
            using var dllStream = File.Create(dllPath);
            using var pdbStream = File.Create(pdbPath);
            var emitted = compilation.Emit(
                dllStream,
                pdbStream,
                manifestResources: resources,
                options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb, pdbFilePath: pdbPath));
            Assert.IsTrue(emitted.Success, string.Join(Environment.NewLine, emitted.Diagnostics));
        }

        /// <summary>
        /// A parameter defaulting to an enum of another assembly: Cecil resolves that enum to
        /// write the default, so the patch needs the compile's references (MCQdbDev's McqdbClient).
        /// </summary>
        [TestMethod]
        public void Patch_EnumDefaultFromAReference_ResolvesThroughTheReferences()
        {
            var kindsDir = Path.Combine(this.dir, "kinds");
            Directory.CreateDirectory(kindsDir);
            var kinds = Path.Combine(kindsDir, "Kinds.dll");
            var kindsCompilation = CSharpCompilation.Create(
                "Kinds",
                new[] { CSharpSyntaxTree.ParseText("namespace Kinds { public enum Kind { A, B } }") },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, deterministic: true));
            Assert.IsTrue(kindsCompilation.Emit(kinds).Success);

            var appDir = Path.Combine(this.dir, "app");
            var map = new Dictionary<string, string> { ["Fx.Shell.skin.cshtml"] = this.skin };
            Emit(appDir, map, out var app, out _, "namespace Fx { public class C { public void M(Kinds.Kind k = Kinds.Kind.B) { } } }", kinds);
            File.WriteAllText(this.skin, "<div>v2</div>");

            Assert.AreEqual(ResourcePatchOutcome.Fallback, ResourcePatcher.Patch(app, new[] { this.skin }).Outcome, "Without references the write cannot resolve Kinds.");
            var result = ResourcePatcher.Patch(app, new[] { this.skin }, new[] { kinds });

            Assert.AreEqual(ResourcePatchOutcome.Ok, result.Outcome, result.Reason);
            CollectionAssert.AreEqual(Encoding.UTF8.GetBytes("<div>v2</div>"), Image.Read(app).Resources["Fx.Shell.skin.cshtml"]);
        }

        /// <summary>What the patch must keep or change, read with System.Reflection.Metadata (not Cecil, the writer).</summary>
        private sealed class Image
        {
            public Guid Mvid { get; private set; }

            public string[] Debug { get; private set; }

            public string[] Methods { get; private set; }

            public Dictionary<string, byte[]> Resources { get; } = new Dictionary<string, byte[]>();

            public static Image Read(string path)
            {
                using var pe = new PEReader(new MemoryStream(File.ReadAllBytes(path)));
                var md = pe.GetMetadataReader();
                var image = new Image
                {
                    Mvid = md.GetGuid(md.GetModuleDefinition().Mvid),
                    Debug = pe.ReadDebugDirectory().Select(e => e.Type.ToString()).ToArray(),
                    Methods = md.MethodDefinitions
                        .Select(h => md.GetMethodDefinition(h))
                        .Select(m => md.GetString(md.GetTypeDefinition(m.GetDeclaringType()).Name) + "::" + md.GetString(m.Name))
                        .OrderBy(n => n, StringComparer.Ordinal)
                        .ToArray(),
                };
                var resourcesRva = pe.PEHeaders.CorHeader.ResourcesDirectory.RelativeVirtualAddress;
                foreach (var handle in md.ManifestResources)
                {
                    var resource = md.GetManifestResource(handle);
                    var reader = pe.GetSectionData(resourcesRva + (int)resource.Offset).GetReader();
                    image.Resources[md.GetString(resource.Name)] = reader.ReadBytes(reader.ReadInt32());
                }

                return image;
            }
        }
    }
}
