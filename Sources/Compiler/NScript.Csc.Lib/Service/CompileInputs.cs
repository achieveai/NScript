//-----------------------------------------------------------------------
// <copyright file="CompileInputs.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// What one stage-1 compile read and wrote, taken from the parsed command line just
    /// before the compile ran. Watch mode records it to know which files to watch, which
    /// outputs a dependent reads, and which content the last compile actually saw.
    /// All paths are full paths.
    /// </summary>
    /// <param name="Output">The output assembly.</param>
    /// <param name="Pdb">The pdb path, or null.</param>
    /// <param name="RefOut">The <c>/refout</c> path, or null.</param>
    /// <param name="Sources">Compiled source files.</param>
    /// <param name="Resources">Embedded resource files (skins, css, xwml, other).</param>
    /// <param name="References">Metadata reference paths.</param>
    /// <param name="Hashes">SHA-256 (hex) of every source and resource that existed before the compile.</param>
    public sealed record CompileInputs(
        string Output,
        string Pdb,
        string RefOut,
        IReadOnlyList<string> Sources,
        IReadOnlyList<string> Resources,
        IReadOnlyList<string> References,
        IReadOnlyDictionary<string, string> Hashes)
    {
        /// <summary>
        /// Reads the parsed command line: sources, resources, references and outputs, with
        /// input hashes taken now, before the compile, so an edit made during the compile shows
        /// up as a difference on the next check (one rebuild too many, never a missed edit).
        /// </summary>
        public static CompileInputs FromArguments(CommandLineArguments args, string workingDir)
        {
            string Full(string path) => string.IsNullOrEmpty(path) ? null : Path.GetFullPath(path, workingDir);

            var sources = args.SourceFiles.Select(f => Full(f.Path)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            // ManifestResources drops the file name of embedded resources (/resource:, how
            // skins, css and xwml are passed); the parsed arguments keep the full path.
            var resources = args.ManifestResourceArguments
                .Select(r => Full(r.FullPath))
                .Where(f => f != null)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var references = args.MetadataReferences.Select(r => Full(r.Reference)).ToList();

            var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in sources.Concat(resources))
            {
                var hash = CompileInputs.HashFile(path);
                if (hash != null)
                {
                    hashes[path] = hash;
                }
            }

            return new CompileInputs(
                Output: Full(Path.Combine(args.OutputDirectory, args.OutputFileName ?? string.Empty)),
                Pdb: Full(args.PdbPath),
                RefOut: Full(args.OutputRefFilePath),
                Sources: sources,
                Resources: resources,
                References: references,
                Hashes: hashes);
        }

        /// <summary>
        /// Hex SHA-256 of a file's bytes, or null when it does not exist. Opens with full
        /// sharing so an editor that holds the file open does not fail the read.
        /// </summary>
        /// <exception cref="IOException">The file exists but cannot be read (for example locked).</exception>
        public static string HashFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
        }
    }
}
