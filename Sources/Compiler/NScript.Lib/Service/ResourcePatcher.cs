namespace NScript.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text.Json;
    using Mono.Cecil;
    using Mono.Cecil.Cil;

    /// <summary>What a resource patch did.</summary>
    public enum ResourcePatchOutcome
    {
        /// <summary>The DLL's resources equal the files now (rewritten, or already equal).</summary>
        Ok,

        /// <summary>The DLL cannot be patched (missing, unmapped resource, unreadable image); compile instead.</summary>
        Fallback,

        /// <summary>Writing or moving the patched files failed; the DLL on disk is unchanged.</summary>
        IoFailed,
    }

    /// <summary>One embedded resource the patch replaced.</summary>
    public sealed record PatchedResource(string Name, string Path, int Bytes);

    /// <summary>The result of <see cref="ResourcePatcher.Patch"/>.</summary>
    public sealed class ResourcePatchResult
    {
        private ResourcePatchResult(ResourcePatchOutcome outcome, string? reason)
        {
            this.Outcome = outcome;
            this.Reason = reason;
        }

        public ResourcePatchOutcome Outcome { get; }

        /// <summary>Why the patch fell back or failed; null when it succeeded.</summary>
        public string? Reason { get; }

        /// <summary>The resources whose bytes were replaced (empty when the DLL already matched).</summary>
        public List<PatchedResource> Replaced { get; } = new List<PatchedResource>();

        /// <summary>Content hash (<see cref="NScript.Csc.Lib.Service.CompileInputs.HashFile"/> format) of every resource file the DLL now embeds.</summary>
        public Dictionary<string, string> Hashes { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Guid MvidBefore { get; internal set; }

        public Guid MvidAfter { get; internal set; }

        internal static ResourcePatchResult Ok() => new ResourcePatchResult(ResourcePatchOutcome.Ok, null);

        internal static ResourcePatchResult Fallback(string reason) => new ResourcePatchResult(ResourcePatchOutcome.Fallback, reason);

        internal static ResourcePatchResult IoFailed(string reason) => new ResourcePatchResult(ResourcePatchOutcome.IoFailed, reason);
    }

    /// <summary>
    /// Watch mode's skin/CSS/XWML fast path: replaces embedded resources in a compiled DLL
    /// with the current file bytes instead of recompiling. Stage 1 embeds those files
    /// byte for byte (Csc.GetResourceFilePaths), and the DLL's own <c>$$ResInfo$$</c> maps
    /// each resource name to its file. The DLL is written to a temp file and moved over the
    /// original, so a reader never sees a torn DLL.
    /// The patched DLL claims no PDB: Cecil renumbers method rows (nested types move next to
    /// their parent), so the old PDB no longer describes it, and Cecil 0.10.1's portable PDB
    /// writer drops the #Pdb stream name, so a new PDB would be unreadable. The old PDB stays
    /// on disk, older than the inputs. A build the daemon syncs keeps the patched DLL and skips
    /// csc (Sdk.targets, _NScriptWatchSyncedSkipCsc); any other MSBuild build recompiles the
    /// pair. Stage 2 never reads symbols.
    /// </summary>
    public static class ResourcePatcher
    {
        /// <summary>The resource stage 1 adds with {resource name: full file path}.</summary>
        public const string ResInfoName = "$$ResInfo$$";

        private const string TempSuffix = ".nscript-patch.tmp";

        /// <summary>
        /// Makes every resource in <paramref name="dllPath"/> equal its file. Falls back when
        /// a file in <paramref name="resourceInputs"/> is not mapped by <c>$$ResInfo$$</c>,
        /// when a mapped file is missing, or when Cecil cannot read the image. The reference
        /// assembly is left alone: it has no resources.
        /// </summary>
        public static ResourcePatchResult Patch(string dllPath, IEnumerable<string> resourceInputs)
        {
            if (!File.Exists(dllPath))
            {
                return ResourcePatchResult.Fallback("output missing: " + dllPath);
            }

            byte[] dllBytes;
            try
            {
                dllBytes = ReadShared(dllPath);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                return ResourcePatchResult.IoFailed("read output: " + ex.Message);
            }

            ModuleDefinition module;
            try
            {
                // Deferred: Immediate resolves referenced assemblies, which a patch never needs.
                module = ModuleDefinition.ReadModule(new MemoryStream(dllBytes), new ReaderParameters(ReadingMode.Deferred));
            }
            catch (Exception ex) when (ex is BadImageFormatException || ex is InvalidOperationException || ex is ArgumentException || ex is NotSupportedException)
            {
                return ResourcePatchResult.Fallback("unreadable output: " + ex.GetType().Name + ": " + ex.Message);
            }

            using (module)
            {
                var result = ResourcePatchResult.Ok();
                result.MvidBefore = module.Mvid;
                result.MvidAfter = module.Mvid;

                var resInfo = module.Resources.OfType<EmbeddedResource>().FirstOrDefault(r => r.Name == ResInfoName);
                if (resInfo == null)
                {
                    return ResourcePatchResult.Fallback("no " + ResInfoName + " in " + dllPath);
                }

                var files = new Dictionary<string, string>(StringComparer.Ordinal);
                try
                {
                    using var json = JsonDocument.Parse(resInfo.GetResourceData());
                    foreach (var entry in json.RootElement.EnumerateObject())
                    {
                        files[entry.Name] = entry.Value.GetString() ?? string.Empty;
                    }
                }
                catch (Exception ex) when (ex is JsonException || ex is InvalidOperationException)
                {
                    return ResourcePatchResult.Fallback("unreadable " + ResInfoName + ": " + ex.Message);
                }

                var mapped = new HashSet<string>(files.Values, StringComparer.OrdinalIgnoreCase);
                var unmapped = resourceInputs.FirstOrDefault(path => !mapped.Contains(path));
                if (unmapped != null)
                {
                    return ResourcePatchResult.Fallback("resource not in " + ResInfoName + ": " + unmapped);
                }

                for (int index = 0; index < module.Resources.Count; index++)
                {
                    if (!(module.Resources[index] is EmbeddedResource resource) || !files.TryGetValue(resource.Name, out var path))
                    {
                        continue;
                    }

                    byte[] bytes;
                    try
                    {
                        if (!File.Exists(path))
                        {
                            return ResourcePatchResult.Fallback("resource file missing: " + path);
                        }

                        bytes = ReadShared(path);
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        return ResourcePatchResult.IoFailed("read " + path + ": " + ex.Message);
                    }

                    result.Hashes[path] = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
                    if (!bytes.AsSpan().SequenceEqual(resource.GetResourceData()))
                    {
                        module.Resources[index] = new EmbeddedResource(resource.Name, resource.Attributes, bytes);
                        result.Replaced.Add(new PatchedResource(resource.Name, path, bytes.Length));
                    }
                }

                if (files.Keys.FirstOrDefault(name => !module.Resources.Any(r => r.Name == name)) is string missingName)
                {
                    return ResourcePatchResult.Fallback(ResInfoName + " names a resource the DLL lacks: " + missingName);
                }

                if (result.Replaced.Count == 0)
                {
                    return result;
                }

                var dllOut = new MemoryStream();
                try
                {
                    // The debug directory comes from the symbol writer: "writing symbols" with
                    // NoPdb keeps the original entries that do not name a PDB.
                    module.Write(dllOut, new WriterParameters { WriteSymbols = true, SymbolWriterProvider = new NoPdb() });
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is NotSupportedException || ex is ArgumentException || ex is AssemblyResolutionException)
                {
                    return ResourcePatchResult.Fallback("cannot rewrite output: " + ex.GetType().Name + ": " + ex.Message);
                }

                string dllTemp = dllPath + TempSuffix;
                try
                {
                    File.WriteAllBytes(dllTemp, dllOut.ToArray());
                    File.Move(dllTemp, dllPath, overwrite: true);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    TryDelete(dllTemp);
                    return ResourcePatchResult.IoFailed("write output: " + ex.Message);
                }

                return result;
            }
        }

        /// <summary>
        /// Writes no symbols, and gives the written DLL the debug directory it was read with
        /// minus the entries that tie it to a PDB (CodeView, PDB checksum, embedded PDB).
        /// </summary>
        private sealed class NoPdb : ISymbolWriterProvider, ISymbolWriter
        {
            private const int CodeView = 2;
            private const int Reproducible = 16;
            private const int EmbeddedPortablePdb = 17;
            private const int PdbChecksum = 19;

            private ImageDebugHeader header = new ImageDebugHeader();

            public ISymbolWriter GetSymbolWriter(ModuleDefinition module, string fileName) => this.For(module);

            public ISymbolWriter GetSymbolWriter(ModuleDefinition module, Stream symbolStream) => this.For(module);

            public ISymbolReaderProvider GetReaderProvider() => new PortablePdbReaderProvider();

            public ImageDebugHeader GetDebugHeader() => this.header;

            public void Write(MethodDebugInformation info)
            {
            }

            public void Dispose()
            {
            }

            private ISymbolWriter For(ModuleDefinition module)
            {
                // Reproducible is dropped too: Cecil adds its own for a deterministic module,
                // and keeping the original as well would list it twice.
                var kept = module.HasDebugHeader
                    ? module.GetDebugHeader().Entries
                        .Where(e => (int)e.Directory.Type is not (CodeView or Reproducible or EmbeddedPortablePdb or PdbChecksum))
                        .ToArray()
                    : Array.Empty<ImageDebugHeaderEntry>();
                this.header = new ImageDebugHeader(kept);
                return this;
            }
        }

        private static byte[] ReadShared(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            var bytes = new byte[stream.Length];
            stream.ReadExactly(bytes);
            return bytes;
        }

        private static void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // The temp file is ours and harmless; the next patch overwrites it.
            }
        }
    }
}
