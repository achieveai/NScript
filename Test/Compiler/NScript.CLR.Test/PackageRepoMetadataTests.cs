//-----------------------------------------------------------------------
// <copyright file="PackageRepoMetadataTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.CLR;

    /// <summary>
    /// Tests for <see cref="PackageRepoMetadata"/> — the parser that recovers
    /// origin URL / commit SHA / repo root from the <c>$$NScriptPackageRepo$$</c>
    /// resource embedded in NScript framework NuGet assemblies (work item #97).
    ///
    /// These tests stay at the parser level (text/byte input) so they don't need
    /// a real DLL to drive. The <c>TryReadFromAssembly</c> path is exercised by
    /// the SDK MSBuild integration test on the consumer side, which pack/restore
    /// cycles a fixture package end-to-end.
    /// </summary>
    [TestClass]
    public class PackageRepoMetadataTests
    {
        [TestMethod]
        public void TryParse_ValidLfText_ProducesAllThreeFields()
        {
            string text =
                "originUrl=https://github.com/achieveai/NScript.git\n" +
                "commitSha=abc123def456\n" +
                "repoRoot=B:\\sources\\NScript\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/achieveai/NScript.git", meta.OriginUrl);
            Assert.AreEqual("abc123def456", meta.CommitSha);
            Assert.AreEqual("B:\\sources\\NScript", meta.RepoRoot);
        }

        [TestMethod]
        public void TryParse_CrlfText_HandlesWindowsLineEndings()
        {
            string text =
                "originUrl=https://github.com/owner/repo.git\r\n" +
                "commitSha=deadbeef\r\n" +
                "repoRoot=/home/build/repo\r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/owner/repo.git", meta.OriginUrl);
            Assert.AreEqual("deadbeef", meta.CommitSha);
            Assert.AreEqual("/home/build/repo", meta.RepoRoot);
        }

        [TestMethod]
        public void TryParse_UnknownKeys_IgnoredButValidFieldsRecovered()
        {
            // Forward-compat: unknown keys must NOT cause the parser to fail. Old
            // tooling reading new metadata should silently skip the extras and
            // recover the known fields.
            string text =
                "schemaVersion=2\n" +
                "originUrl=https://github.com/a/b.git\n" +
                "futureField=futureValue\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/a/b.git", meta.OriginUrl);
            Assert.AreEqual("sha", meta.CommitSha);
            Assert.AreEqual("/r", meta.RepoRoot);
        }

        [TestMethod]
        public void TryParse_BlankLines_Skipped()
        {
            string text =
                "\n" +
                "originUrl=https://github.com/a/b.git\n" +
                "\n" +
                "commitSha=sha\n" +
                "  \n" +
                "repoRoot=/r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/a/b.git", meta.OriginUrl);
        }

        [TestMethod]
        public void TryParse_DuplicateKeys_LastWins()
        {
            // MSBuild PropertyGroup semantics: a later definition replaces an earlier
            // one. The parser must match so that future schema migrations can ship
            // both old and new keys for a transition window without breaking either.
            string text =
                "originUrl=https://example/old.git\n" +
                "originUrl=https://example/new.git\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://example/new.git", meta.OriginUrl);
        }

        [TestMethod]
        public void TryParse_ValueContainsEquals_Preserved()
        {
            // The parser splits each line on the FIRST '=' only — anything after
            // is the value verbatim. Documents the split semantics so a future
            // refactor that uses Split('=') gets caught here.
            string text =
                "originUrl=https://example.com/?token=abc=def&path=/x\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://example.com/?token=abc=def&path=/x", meta.OriginUrl);
        }

        [TestMethod]
        public void TryParse_MissingOriginUrl_ReturnsNull()
        {
            string text =
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            Assert.IsNull(PackageRepoMetadata.TryParse(text));
        }

        [TestMethod]
        public void TryParse_MissingCommitSha_ReturnsNull()
        {
            string text =
                "originUrl=https://github.com/a/b.git\n" +
                "repoRoot=/r\n";

            Assert.IsNull(PackageRepoMetadata.TryParse(text));
        }

        [TestMethod]
        public void TryParse_MissingRepoRoot_ReturnsNull()
        {
            string text =
                "originUrl=https://github.com/a/b.git\n" +
                "commitSha=sha\n";

            Assert.IsNull(PackageRepoMetadata.TryParse(text));
        }

        [TestMethod]
        public void TryParse_EmptyValue_TreatedAsMissing()
        {
            // An empty value behaves the same as a missing key — the consumer-side
            // SDK target can't use an empty origin URL anyway, so failing fast at
            // the parser layer keeps downstream code simpler.
            string text =
                "originUrl=\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            Assert.IsNull(PackageRepoMetadata.TryParse(text));
        }

        [TestMethod]
        public void TryParse_NullOrEmpty_ReturnsNull()
        {
            Assert.IsNull(PackageRepoMetadata.TryParse((string)null));
            Assert.IsNull(PackageRepoMetadata.TryParse(string.Empty));
            Assert.IsNull(PackageRepoMetadata.TryParse((byte[])null));
            Assert.IsNull(PackageRepoMetadata.TryParse(new byte[0]));
        }

        [TestMethod]
        public void TryParse_MalformedNoEquals_LinesSkipped()
        {
            // Lines without '=' are silently skipped (treated as garbage). If the
            // required keys still appear elsewhere in the input, parsing succeeds.
            string text =
                "garbage line with no equals\n" +
                "originUrl=https://github.com/a/b.git\n" +
                "another garbage line\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/a/b.git", meta.OriginUrl);
        }

        [TestMethod]
        public void TryParse_Utf8BomPrefix_Stripped()
        {
            // MSBuild's WriteLinesToFile writes UTF-8 with a BOM. The actual
            // resource bytes embedded in NScript framework DLLs start with
            // U+FEFF, so the parser must transparently strip it or the first
            // key parses as "﻿originUrl" (not "originUrl") and the whole
            // metadata read fails — exactly the regression caught during
            // initial end-to-end verification on the worktree build.
            string text =
                "﻿originUrl=https://github.com/a/b.git\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/a/b.git", meta.OriginUrl);
        }

        [TestMethod]
        public void TryParse_BytesUtf8WithBom_RoundTrips()
        {
            // Exercise the byte path too — Encoding.UTF8.GetPreamble() returns
            // the BOM, so producing the bytes the way MSBuild does and feeding
            // them straight in must work the same as the text overload.
            string text =
                "originUrl=https://github.com/a/b.git\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";
            byte[] body = Encoding.UTF8.GetBytes(text);
            byte[] bom = Encoding.UTF8.GetPreamble();
            byte[] bytes = new byte[bom.Length + body.Length];
            System.Buffer.BlockCopy(bom, 0, bytes, 0, bom.Length);
            System.Buffer.BlockCopy(body, 0, bytes, bom.Length, body.Length);

            var meta = PackageRepoMetadata.TryParse(bytes);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/a/b.git", meta.OriginUrl);
        }

        [TestMethod]
        public void TryParse_BytesUtf8_RoundTripsWithText()
        {
            string text =
                "originUrl=https://github.com/a/b.git\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            var meta = PackageRepoMetadata.TryParse(bytes);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://github.com/a/b.git", meta.OriginUrl);
            Assert.AreEqual("sha", meta.CommitSha);
            Assert.AreEqual("/r", meta.RepoRoot);
        }

        [TestMethod]
        public void TryParse_RedactedOriginUrl_PreservedAsIs()
        {
            // After PR #98 review feedback, the pack-time target redacts the origin URL
            // via the same `://[^/@]+@` -> `://***@` regex used for log output, BEFORE
            // embedding into the .bin and buildTransitive .props. The parser must
            // accept the redacted form unchanged (i.e. not try to "un-redact" or
            // reject it) — `***` is a perfectly valid URL component as far as the
            // text format is concerned, and the parser stays format-agnostic about
            // the URL contents.
            string text =
                "originUrl=https://***@github.com/achieveai/NScript.git\n" +
                "commitSha=sha\n" +
                "repoRoot=/r\n";

            var meta = PackageRepoMetadata.TryParse(text);

            Assert.IsNotNull(meta);
            Assert.AreEqual("https://***@github.com/achieveai/NScript.git", meta.OriginUrl);
            Assert.AreEqual("sha", meta.CommitSha);
            Assert.AreEqual("/r", meta.RepoRoot);
        }

        [TestMethod]
        public void TryReadFromAssembly_NonexistentPath_ReturnsNull()
        {
            // Defensive: a stale references list pointing to a missing DLL should
            // not crash — the consumer build relies on this returning null
            // gracefully for the "no metadata" fallback path.
            string fakePath = Path.Combine(Path.GetTempPath(), "definitely-not-a-real-assembly-" + System.Guid.NewGuid() + ".dll");
            Assert.IsNull(PackageRepoMetadata.TryReadFromAssembly(fakePath));
        }

        [TestMethod]
        public void TryReadFromAssembly_NullOrEmptyPath_ReturnsNull()
        {
            Assert.IsNull(PackageRepoMetadata.TryReadFromAssembly(null));
            Assert.IsNull(PackageRepoMetadata.TryReadFromAssembly(string.Empty));
        }

        /// <summary>
        /// End-to-end check: when the framework Release build has run, the
        /// `Sunlight.Framework.dll` artifact in NScriptToolSet should carry the
        /// `$$NScriptPackageRepo$$` resource with our three keys, and reading it
        /// via the Cecil path should succeed. This verifies the pack-time
        /// embedding pipeline (NScript.PackageMetadata.targets) actually produced
        /// the resource the consumer-side flow depends on. If the Release build
        /// has not been produced yet, the test is a no-op so dev-loop runs that
        /// only build Debug don't fail.
        /// </summary>
        [TestMethod]
        public void TryReadFromAssembly_RealFrameworkAssembly_RoundTripsMetadata()
        {
            string assemblyPath = Path.Combine(
                GetRepoRootForRealAssembly(),
                "NScriptToolSet", "lib", "Release", "Sunlight.Framework.dll");

            if (!File.Exists(assemblyPath))
            {
                Assert.Inconclusive(
                    $"Release framework artifact not found at {assemblyPath}. " +
                    "Run `dotnet build NScript_Full.sln -c Release` first.");
            }

            var meta = PackageRepoMetadata.TryReadFromAssembly(assemblyPath);

            Assert.IsNotNull(meta,
                $"Expected $$NScriptPackageRepo$$ resource in {assemblyPath}. " +
                "If the build succeeded but this is null, the EmbeddedResource pipeline " +
                "is broken (see NScript.PackageMetadata.targets BeforeTargets ordering).");
            Assert.IsFalse(string.IsNullOrEmpty(meta.OriginUrl), "OriginUrl should be non-empty");
            Assert.IsFalse(string.IsNullOrEmpty(meta.CommitSha), "CommitSha should be non-empty");
            Assert.IsFalse(string.IsNullOrEmpty(meta.RepoRoot), "RepoRoot should be non-empty");

            // SHA shape sanity check — full 40-char hex string from git rev-parse HEAD.
            Assert.IsTrue(meta.CommitSha.Length == 40,
                $"Expected 40-char SHA, got '{meta.CommitSha}' ({meta.CommitSha.Length} chars)");
        }

        private static string GetRepoRootForRealAssembly()
        {
            string dir = Path.GetDirectoryName(typeof(PackageRepoMetadataTests).Assembly.Location);
            for (int i = 0; i < 4; i++)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return dir;
        }
    }
}
