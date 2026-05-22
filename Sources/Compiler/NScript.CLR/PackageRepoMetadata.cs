//-----------------------------------------------------------------------
// <copyright file="PackageRepoMetadata.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Mono.Cecil;

    /// <summary>
    /// Metadata describing the Git repository that produced an NScript framework NuGet
    /// package, captured at pack time and embedded into the package's primary assembly as
    /// a <c>$$NScriptPackageRepo$$</c> resource (mirroring the existing
    /// <c>$$BstInfo$$</c> / <c>$$ResInfo$$</c> pattern). Consumed by the consumer-side
    /// SDK target to auto-resolve secondary source-map URLs without requiring the
    /// consumer to clone the NScript repo locally (work item #97).
    /// </summary>
    /// <remarks>
    /// The primary delivery path for this data is the package's
    /// <c>buildTransitive/&lt;PackageId&gt;.props</c> file, which MSBuild auto-imports
    /// when the package is referenced. This reader exists for MSBuild-less tooling
    /// (e.g. a hypothetical standalone <c>cs2jsc</c> invocation outside the SDK) that
    /// needs to recover the same values directly from the assembly via Mono.Cecil.
    ///
    /// Wire format: UTF-8 text, one <c>key=value</c> pair per line, LF or CRLF
    /// line endings tolerated. The canonical keys are <c>originUrl</c>,
    /// <c>commitSha</c>, and <c>repoRoot</c>. Unknown keys are silently ignored so
    /// future schema additions remain backward-compatible. Blank lines and lines
    /// without an <c>=</c> separator are skipped.
    /// </remarks>
    public sealed class PackageRepoMetadata
    {
        /// <summary>
        /// Name of the embedded resource that carries the serialized metadata. The
        /// `$$` framing mirrors <c>$$BstInfo$$</c> / <c>$$ResInfo$$</c> so the pattern
        /// is consistent across NScript-emitted resources.
        /// </summary>
        public const string ResourceName = "$$NScriptPackageRepo$$";

        private PackageRepoMetadata(string originUrl, string commitSha, string repoRoot)
        {
            this.OriginUrl = originUrl;
            this.CommitSha = commitSha;
            this.RepoRoot = repoRoot;
        }

        /// <summary>
        /// Git remote URL captured at pack time (output of
        /// <c>git remote get-url origin</c>). May contain credentials in the
        /// <c>https://user:token@host/...</c> form on machines where developers have
        /// configured them locally; callers that surface this value into logs MUST
        /// apply the <c>://creds@</c> redaction used elsewhere in the SDK.
        /// </summary>
        public string OriginUrl { get; }

        /// <summary>
        /// Commit SHA captured at pack time (output of <c>git rev-parse HEAD</c>).
        /// The exact SHA of the framework sources baked into this assembly.
        /// </summary>
        public string CommitSha { get; }

        /// <summary>
        /// Worktree root captured at pack time (output of
        /// <c>git rev-parse --show-toplevel</c>). This is a BUILD-MACHINE absolute
        /// path; it does not exist on the consumer's machine. Its purpose is to
        /// match the path prefix baked into the assembly's <c>$$BstInfo$$</c>
        /// source-file references so that
        /// <c>OwaSourceMapper.SourceMap.TryRebaseToRepoRoot</c> can rebase framework
        /// sources to repo-relative form.
        /// </summary>
        public string RepoRoot { get; }

        /// <summary>
        /// Reads the <c>$$NScriptPackageRepo$$</c> resource from <paramref name="assemblyPath"/>,
        /// if present, and parses it into a <see cref="PackageRepoMetadata"/> instance.
        /// </summary>
        /// <param name="assemblyPath">Path to a framework NuGet's primary assembly.</param>
        /// <returns>
        /// Parsed metadata, or <c>null</c> when the resource is absent, malformed, or
        /// the file cannot be opened. Callers should treat <c>null</c> as "no metadata
        /// available" — the consumer build proceeds with legacy local-path source maps.
        /// </returns>
        public static PackageRepoMetadata TryReadFromAssembly(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath) || !File.Exists(assemblyPath))
            {
                return null;
            }

            ModuleDefinition module;
            try
            {
                // No symbol reading, no metadata resolver gymnastics — we just want the
                // resource bytes. Mono.Cecil keeps the file open for the lifetime of the
                // ModuleDefinition, so we read into memory and dispose immediately.
                module = ModuleDefinition.ReadModule(assemblyPath);
            }
            catch (BadImageFormatException) { return null; }
            catch (IOException) { return null; }
            catch (UnauthorizedAccessException) { return null; }

            using (module)
            {
                var resource = module.Resources
                    .OfType<EmbeddedResource>()
                    .FirstOrDefault(r => string.Equals(r.Name, ResourceName, StringComparison.Ordinal));

                if (resource == null)
                {
                    return null;
                }

                byte[] bytes;
                try
                {
                    bytes = resource.GetResourceData();
                }
                catch (IOException) { return null; }

                return TryParse(bytes);
            }
        }

        /// <summary>
        /// Parses the raw resource bytes (UTF-8 key=value lines) into a metadata instance.
        /// Public so MSBuild-less tooling that fetches the resource bytes via another
        /// path (e.g. a CI script that opens a <c>.nupkg</c> directly) can reuse the
        /// canonical parser instead of re-implementing the format.
        /// </summary>
        /// <param name="bytes">Raw resource payload.</param>
        /// <returns>
        /// Parsed metadata, or <c>null</c> when any of the three required keys
        /// (<c>originUrl</c>, <c>commitSha</c>, <c>repoRoot</c>) is missing or empty.
        /// </returns>
        public static PackageRepoMetadata TryParse(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            string text;
            try
            {
                text = Encoding.UTF8.GetString(bytes);
            }
            catch (ArgumentException)
            {
                return null;
            }

            return TryParse(text);
        }

        /// <summary>
        /// Text-level overload of <see cref="TryParse(byte[])"/>. Same contract — returns
        /// <c>null</c> for any input that lacks a non-empty <c>originUrl</c>,
        /// <c>commitSha</c>, and <c>repoRoot</c> pair.
        /// </summary>
        public static PackageRepoMetadata TryParse(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            // MSBuild's WriteLinesToFile task writes UTF-8 WITH a BOM (U+FEFF
            // prefix). Without stripping it, the first key would parse as
            // "﻿originUrl" instead of "originUrl" and the required-key
            // check below would return null. Strip a single leading BOM.
            if (text.Length > 0 && text[0] == '﻿')
            {
                text = text.Substring(1);
            }

            var values = new Dictionary<string, string>(StringComparer.Ordinal);

            // Splitting on '\n' and trimming '\r' tolerates both LF and CRLF without
            // pulling in StringReader. Blank lines and `key`-without-`=` lines are
            // skipped silently — the format is intentionally forgiving so future
            // schema additions remain backward-compatible.
            foreach (var rawLine in text.Split('\n'))
            {
                var line = rawLine.TrimEnd('\r').Trim();
                if (line.Length == 0)
                {
                    continue;
                }

                int eq = line.IndexOf('=');
                if (eq <= 0)
                {
                    continue;
                }

                var key = line.Substring(0, eq).Trim();
                var value = line.Substring(eq + 1).Trim();
                if (key.Length == 0 || value.Length == 0)
                {
                    continue;
                }

                // Last-wins on duplicate keys — matches MSBuild PropertyGroup semantics.
                values[key] = value;
            }

            values.TryGetValue("originUrl", out var originUrl);
            values.TryGetValue("commitSha", out var commitSha);
            values.TryGetValue("repoRoot", out var repoRoot);

            if (string.IsNullOrEmpty(originUrl)
                || string.IsNullOrEmpty(commitSha)
                || string.IsNullOrEmpty(repoRoot))
            {
                return null;
            }

            return new PackageRepoMetadata(originUrl, commitSha, repoRoot);
        }
    }
}
