using OwaSourceMapper.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Expose `internal` helpers (NormalizeRepoRoot, TryRebaseToRepoRoot, AbsolutizeForSources)
// to the unit-test assembly so they can be exercised directly without going through the
// full ToString() pipeline. The csproj has GenerateAssemblyInfo=false so the attribute is
// declared in source rather than via an MSBuild <InternalsVisibleTo> item. WI-19 iter-1 M4.
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SourceMap.Test")]

namespace OwaSourceMapper
{
    public struct SourceMapping
    {
        /// <summary>
        /// Source column.
        /// </summary>
        public int SourceColumn;

        /// <summary>
        /// Source line.
        /// </summary>
        public int SourceLine;

        /// <summary>
        /// Target column.
        /// </summary>
        public int TargetColumn;

        /// <summary>
        /// Target line.
        /// </summary>
        public int TargetLine;

        /// <summary>
        /// Zero-based index of the source file.
        /// </summary>
        public int SourceFileIndex;

        /// <summary>
        /// Zero-based index of the source name.
        /// </summary>
        public int SourceNameIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sLine">      The line. </param>
        /// <param name="sCol">       The col. </param>
        /// <param name="tLine">      The line. </param>
        /// <param name="tCol">       The col. </param>
        /// <param name="sFileIndex"> Zero-based index of the s file. </param>
        /// <param name="sNameIndex"> (optional) zero-based index of the s name. </param>
        public SourceMapping(int sLine, int sCol, int tLine, int tCol, int sFileIndex, int sNameIndex = -1)
        {
            this.SourceLine = sLine;
            this.SourceColumn = sCol;
            this.TargetLine = tLine;
            this.TargetColumn = tCol;
            this.SourceFileIndex = sFileIndex;
            this.SourceNameIndex = sNameIndex;
        }

        /// <summary>
        /// Gets the default mapping.
        /// </summary>
        /// <value>
        /// The default mapping.
        /// </value>
        public static SourceMapping DefaultMapping
        {
            get { return new SourceMapping(0, 0, 0, 0, 0, 0); }
        }

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", this.SourceLine, this.SourceColumn, this.TargetLine, this.TargetColumn);
        }

        /// <summary>
        /// Converts this mapping to its V3 source map segment representation, encoded relative
        /// to the prior mapping state.
        /// </summary>
        /// <param name="previousMapping"> The previous mapping (used for source column, file index,
        ///     target line, target column deltas). </param>
        /// <param name="previousNameIndex"> The last emitted source name index — used to encode
        ///     this mapping's name index relative to it per V3 spec. Starts at 0 and only advances
        ///     when a name is actually written. </param>
        /// <param name="firstSegment"> True when this is the first segment on a line. </param>
        /// <returns>
        /// The Base64 VLQ-encoded segment string.
        /// </returns>
        public string ToStringRelative(SourceMapping previousMapping, int previousNameIndex, bool firstSegment)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(
                Base64VLQ.ConvertToBase64VLQ(
                firstSegment
                    ? this.SourceColumn
                    : this.SourceColumn - previousMapping.SourceColumn));

            if (this.SourceFileIndex >= 0)
            {
                sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceFileIndex - previousMapping.SourceFileIndex));
                sb.Append(Base64VLQ.ConvertToBase64VLQ(this.TargetLine - previousMapping.TargetLine));
                sb.Append(Base64VLQ.ConvertToBase64VLQ(this.TargetColumn - previousMapping.TargetColumn));

                if (this.SourceNameIndex != -1)
                {
                    sb.Append(Base64VLQ.ConvertToBase64VLQ(this.SourceNameIndex - previousNameIndex));
                }
            }

            return sb.ToString();
        }
    }

    public class SourceMap
    {
        /// <summary>
        /// The mappings.
        /// </summary>
        private SortedList<Tuple<int, int>, SourceMapping> mappings;

        /// <summary>
        /// The files.
        /// </summary>
        private List<string> files = new List<string>();

        /// <summary>
        /// The names.
        /// </summary>
        private List<string> names = new List<string>();

        /// <summary>
        /// The version.
        /// </summary>
        private int version = 3;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SourceMap()
        {
            mappings = new SortedList<Tuple<int, int>, SourceMapping>();
        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public string File { get; set; }

        public string MapFile
        {
            get
            {
                if (this.File != null)
                {
                    return Path.GetFileNameWithoutExtension(this.File) + ".map";
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets source root.
        /// </summary>
        /// <value>
        /// The source root.
        /// </value>
        public string SourceRoot { get; set; }

        /// <summary>
        /// When true, <see cref="Write(string)"/> drops the embedded legacy
        /// <c>SrcMapper.ashx</c> handler alongside the generated map. Defaults to <c>true</c>
        /// so existing IIS-based deployments keep working; callers that ship a modern
        /// ASP.NET Core source handler (or a repo-URL <see cref="SourceRoot"/>) should set
        /// this to <c>false</c>.
        /// </summary>
        public bool EmitLegacyAshxHandler { get; set; } = true;

        /// <summary>
        /// Optional absolute path to the repository root (output of <c>git rev-parse --show-toplevel</c>).
        /// When set, <see cref="ToString"/> emits <c>sources[i]</c> as forward-slash, repo-relative
        /// paths so they combine cleanly with a remote-repo <see cref="SourceRoot"/>. Files outside
        /// the repo root remain in the legacy absolutized form.
        /// </summary>
        public string RepoRoot { get; set; }

        /// <summary>
        /// Mapping comparison.
        /// </summary>
        /// <param name="lineCol1"> The first line col. </param>
        /// <param name="lineCol2"> The second line col. </param>
        /// <returns>
        /// .
        /// </returns>
        /// <summary>
        /// Normalizes <paramref name="repoRoot"/> to a full path with a trailing directory separator
        /// for cheap startsWith comparisons. Returns null on null/empty input so callers can short-
        /// circuit the rebase.
        /// </summary>
        internal static string NormalizeRepoRoot(string repoRoot)
        {
            if (string.IsNullOrWhiteSpace(repoRoot))
            {
                return null;
            }

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(repoRoot);
            }
            catch (ArgumentException)
            {
                // GetFullPath rejects malformed input; treat as "no rebase".
                return null;
            }
            catch (PathTooLongException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (System.Security.SecurityException)
            {
                return null;
            }

            if (!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString())
                && !fullPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                fullPath += Path.DirectorySeparatorChar;
            }

            return fullPath;
        }

        /// <summary>
        /// If <paramref name="rawFile"/> sits under <paramref name="normalizedRepoRoot"/>, returns
        /// the forward-slash, repo-relative form; otherwise returns null so the caller falls back
        /// to the legacy absolutized form.
        /// </summary>
        /// <param name="rawFile">              Source file path as the compiler stored it
        ///     (may contain escaped backslashes from <see cref="AddMapping"/>). </param>
        /// <param name="normalizedRepoRoot">   Normalized repo root with a trailing separator,
        ///     or null to skip rebase. </param>
        internal static string TryRebaseToRepoRoot(string rawFile, string normalizedRepoRoot)
        {
            if (normalizedRepoRoot == null || string.IsNullOrEmpty(rawFile))
            {
                return null;
            }

            string unescaped = rawFile.Replace("\\\\", "\\");
            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(unescaped);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (PathTooLongException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (System.Security.SecurityException)
            {
                return null;
            }

            // Match the SourceMapFileHandler.IsContained pattern: filesystem case sensitivity
            // is platform-dependent. Folding case unconditionally on Linux/macOS would cause a
            // file under e.g. `/repo/Source/Foo.cs` to incorrectly match a `/repo/SOURCE/`
            // alias and produce a corrupt rebase. On Windows the legacy ignore-case behaviour
            // remains.
            StringComparison cmp = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (!fullPath.StartsWith(normalizedRepoRoot, cmp))
            {
                return null;
            }

            string relative = fullPath.Substring(normalizedRepoRoot.Length);
            return relative.Replace("\\", "/");
        }

        /// <summary>
        /// Legacy fallback that produces the absolutized, drive-escaped, forward-slashed
        /// form of <paramref name="rawFile"/> for emission into the <c>sources[]</c> array
        /// when no repo-rebase applies. Wrapped in the same 4-exception set used elsewhere
        /// (NormalizeRepoRoot, TryRebaseToRepoRoot) so that a malformed legacy path doesn't
        /// take down map generation — we degrade to the unprocessed (forward-slashed) form.
        /// </summary>
        private static string AbsolutizeForSources(string rawFile)
        {
            try
            {
                return Path.GetFullPath(rawFile).Replace(":", "$").Replace("\\", "/");
            }
            catch (ArgumentException)
            {
                return rawFile.Replace("\\\\", "/");
            }
            catch (PathTooLongException)
            {
                return rawFile.Replace("\\\\", "/");
            }
            catch (NotSupportedException)
            {
                return rawFile.Replace("\\\\", "/");
            }
            catch (System.Security.SecurityException)
            {
                return rawFile.Replace("\\\\", "/");
            }
        }

        public static int MappingComparison(Tuple<int, int> lineCol1, Tuple<int, int> lineCol2)
        {
            if (lineCol1.Item1 != lineCol2.Item1)
            {
                return lineCol1.Item1.CompareTo(lineCol2.Item1);
            }
            else
            {
                return lineCol1.Item2.CompareTo(lineCol2.Item2);
            }
        }

        /// <summary>
        /// Adds a mapping.
        /// </summary>
        /// <param name="sLine"> The line. </param>
        /// <param name="sCol">  The col. </param>
        /// <param name="tLine"> The line. </param>
        /// <param name="tCol">  The col. </param>
        /// <param name="file">  The file. </param>
        /// <param name="name">  (optional) the name. </param>
        public void AddMapping(int sLine, int sCol, int tLine, int tCol, string file, string name = null)
        {
            if (file != null)
            {
                file = file.Replace("\\", "\\\\");
            }

            int sFileIndex = this.files.IndexOf(file);

            if (sFileIndex == -1
                && file != null)
            {
                this.files.Add(file);
                sFileIndex = this.files.Count - 1;
            }

            int sNameIndex = -1;

            if (name != null)
            {
                sNameIndex = this.names.IndexOf(name);
                if (sNameIndex == -1)
                {
                    this.names.Add(name);
                    sNameIndex = this.names.Count - 1;
                }
            }

            SourceMapping mapping = new SourceMapping(
                sLine,
                sCol,
                tLine,
                tCol,
                sFileIndex,
                sNameIndex);

            Tuple<int, int> key = Tuple.Create<int, int>(sLine, sCol);

            if (mappings.ContainsKey(key))
            {
                mappings[key] = mapping;
            }
            else
            {
                mappings.Add(key, mapping);
            }
        }

        /// <summary>
        /// Convert this object into a string representation.
        /// </summary>
        /// <returns>
        /// This object as a string.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{" +
                "\t\"version\": \"" + this.version + "\",\n");

            if (this.File != null)
            {
                sb.Append("\t\"file\": \"" + this.File + "\",\n");
            }

            // Prefer explicitly configured SourceRoot (e.g. repo URL, custom handler path);
            // fall back to the legacy "{file}.ashx" default for backward compatibility with
            // deployments that still use the bundled SrcMapper.ashx handler.
            string sourceRoot = this.SourceRoot
                ?? (this.File != null
                    ? Path.GetFileNameWithoutExtension(this.File) + ".ashx"
                    : string.Empty);
            sb.Append("\t\"sourceRoot\": \"" + sourceRoot + "\",\n");

            if (this.files.Count > 0)
            {
                Dictionary<string, int> fileMap = new Dictionary<string, int>(this.files.Count);
                string normalizedRepoRoot = NormalizeRepoRoot(this.RepoRoot);
                sb.Append("\t\"sources\": [");
                for (int i = 0; i < this.files.Count; i++)
                {
                    var fileName = TryRebaseToRepoRoot(this.files[i], normalizedRepoRoot)
                        ?? AbsolutizeForSources(this.files[i]);
                    if (fileMap.TryGetValue(fileName, out var tmp))
                    { fileMap[fileName] = tmp + 1; fileName = fileName + tmp + 1; }
                    else
                    { fileMap[fileName] = 1; }

                    if (i > 0)
                    { sb.Append(",\n\t\t"); }

                    sb.Append("\"" + fileName + "\"");
                }

                sb.Append("],\n");
                sb.Append("\t\"sourcesLong\": [\"" + string.Join("\",\n\t\t\"", this.files) + "\"],\n");
            }

            if (this.names.Count > 0)
            {
                sb.Append("\t\"names\": [\"" + string.Join("\",\"", this.names) + "\"],\n");
            }
            
            sb.Append("\t\"mappings\": \"");

            SourceMapping previousMapping = SourceMapping.DefaultMapping;
            // Per V3 spec, name indices are encoded relative to the LAST EMITTED name index
            // (not the previous mapping's name field) and start at 0. Tracking this separately
            // prevents mappings without names from poisoning the delta baseline.
            int previousNameIndex = 0;

            int currentSourceLine = 0;
            bool firstSegment = true;

            StringBuilder mappingSb = new StringBuilder();
            foreach (var mappingKeyPair in mappings)
            {
                var mapping = mappingKeyPair.Value;

                // Fill missed semicolons
                if (currentSourceLine != mapping.SourceLine)
                {
                    mappingSb.Append(new string(';', mapping.SourceLine - currentSourceLine));
                    firstSegment = true;
                    // previousMapping = SourceMapping.DefaultMapping;
                }
                else if (mapping.SourceColumn > 0)
                {
                    mappingSb.Append(',');
                    firstSegment = false;
                }

                mappingSb.Append(mapping.ToStringRelative(previousMapping, previousNameIndex, firstSegment));
                currentSourceLine = mapping.SourceLine;
                if (mapping.SourceFileIndex >= 0)
                {
                    previousMapping = mapping;
                    if (mapping.SourceNameIndex >= 0)
                    {
                        previousNameIndex = mapping.SourceNameIndex;
                    }
                }
            }

            sb.Append(mappingSb);
            sb.Append("\"\n}");

            return sb.ToString();
        }

        /// <summary>
        /// Writes the <c>.map</c> file to <paramref name="dirctory"/>. When no explicit
        /// <see cref="SourceRoot"/> has been configured, also drops the legacy
        /// <c>SrcMapper.ashx</c> handler sidecar next to the map for backward compatibility
        /// with deployments that still rely on it.
        /// </summary>
        /// <param name="dirctory"> The directory into which the map (and legacy handler) are written. </param>
        public void Write(string dirctory)
        {
            string fileName = Path.Combine(
                dirctory,
                Path.GetFileNameWithoutExtension(this.File));
            using (StreamWriter mapWriter = new StreamWriter(fileName + ".map", false, System.Text.Encoding.ASCII))
                mapWriter.Write(this.ToString());

            if (!this.EmitLegacyAshxHandler)
            {
                return;
            }

            using (var stream = typeof(SourceMap)
                .Assembly.GetManifestResourceStream("SourceMap.SrcMapper.ashx"))
            {
                if (stream != null)
                {
                    System.IO.TextReader reader = new System.IO.StreamReader(stream);
                    System.IO.File.WriteAllText(
                        fileName + ".ashx",
                        reader.ReadToEnd().Trim());
                }
            }
        }
    }
}
