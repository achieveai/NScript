namespace NScript.Lib
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using NScript.Converter;
    using NScript.Utils;

    /// <summary>
    /// Verify mode (<c>NSCRIPT_VERIFY_INCREMENTAL=1</c>): after a warm dev-mode emit, builds
    /// the same inputs again with a fresh <see cref="Builder"/> into a temp folder and compares
    /// the .js and .map byte for byte. A difference is logged as VerifyIncremental with the
    /// first differing line and printed as a warning; the build's own result is unchanged.
    /// For stress runs and bug hunts only: it doubles the cost of every warm emit.
    /// </summary>
    public static class IncrementalVerifier
    {
        public static bool IsEnabled
            => Environment.GetEnvironmentVariable("NSCRIPT_VERIFY_INCREMENTAL") == "1";

        /// <summary>
        /// Builds <paramref name="jsFileName"/>'s inputs cold through <paramref name="createBuilder"/>
        /// (given the temp output path) and compares; true when both files are equal.
        /// </summary>
        public static bool Verify(
            string jsFileName,
            string lastBuild,
            Func<string, Builder> createBuilder,
            Func<IConverterPlugin[]> createPlugins)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var outputKey = Path.GetFullPath(jsFileName).ToLowerInvariant();
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(outputKey))).Substring(0, 16).ToLowerInvariant();
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "nscript-verify", hash)).FullName;

            // The same file name, so the sourceMappingURL comment and the map's file match.
            var fullJs = Path.Combine(dir, Path.GetFileName(jsFileName));
            bool built;
            using (var builder = createBuilder(fullJs))
            {
                built = builder.Execute(createPlugins());
            }

            string mismatch = !built
                ? "the full build failed"
                : CompareOutputs(jsFileName, fullJs);

            CompilerLog.ForComponent("Verify").Information(
                "VerifyIncremental Match={Match} Build={Build} Output={Output} Full={Full} Mismatch={Mismatch} ElapsedMs={ElapsedMs}",
                mismatch == null,
                lastBuild,
                jsFileName,
                fullJs,
                mismatch,
                sw.ElapsedMilliseconds);

            if (mismatch != null)
            {
                Console.WriteLine("NScript verify: warning: incremental emit differs from a full emit ({0}): {1}", lastBuild, mismatch);
            }

            return mismatch == null;
        }

        /// <summary>
        /// Compares the .js, then its map. The map sits beside the .js as <c>X.map</c>, not
        /// <c>X.js.map</c> (<c>SourceMap.Write</c> drops the .js extension).
        /// </summary>
        internal static string CompareOutputs(string incrementalJs, string fullJs)
            => Compare(incrementalJs, fullJs) ?? Compare(Path.ChangeExtension(incrementalJs, ".map"), Path.ChangeExtension(fullJs, ".map"));

        /// <summary>Null when both files are missing or equal; else where they first differ.</summary>
        internal static string Compare(string incremental, string full)
        {
            bool hasIncremental = File.Exists(incremental), hasFull = File.Exists(full);
            if (!hasIncremental && !hasFull)
            {
                return null;
            }

            if (hasIncremental != hasFull)
            {
                return Path.GetFileName(incremental) + (hasIncremental ? " has no full counterpart" : " is missing");
            }

            var left = File.ReadAllBytes(incremental);
            var right = File.ReadAllBytes(full);
            if (left.AsSpan().SequenceEqual(right))
            {
                return null;
            }

            var leftLines = Encoding.UTF8.GetString(left).Split('\n');
            var rightLines = Encoding.UTF8.GetString(right).Split('\n');
            int line = Enumerable.Range(0, Math.Min(leftLines.Length, rightLines.Length))
                .FirstOrDefault(index => leftLines[index] != rightLines[index], -1);
            if (line < 0)
            {
                return string.Format("{0}: {1} lines vs {2} full", Path.GetFileName(incremental), leftLines.Length, rightLines.Length);
            }

            return string.Format(
                "{0} line {1}: incremental \"{2}\" vs full \"{3}\"",
                Path.GetFileName(incremental),
                line + 1,
                Clip(leftLines[line]),
                Clip(rightLines[line]));
        }

        private static string Clip(string text)
            => text.Length <= 200 ? text.TrimEnd('\r') : text.Substring(0, 200) + "...";
    }
}
