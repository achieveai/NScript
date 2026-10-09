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
    using NScript.Csc.Lib.Service;
    using NScript.Lib.Service;

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

        /// <summary>Compiles <see cref="Source"/> into <paramref name="dir"/>/Fx.dll + Fx.pdb, embedding the mapped files and $$ResInfo$$.</summary>
        private static void Emit(string dir, Dictionary<string, string> map, out string dllPath, out string pdbPath)
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
                new[] { CSharpSyntaxTree.ParseText(Source) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
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
