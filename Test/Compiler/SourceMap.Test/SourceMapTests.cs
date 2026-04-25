using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// Unit tests for the <see cref="SourceMap"/> class — verify V3 JSON structure,
    /// the <c>sourceRoot</c> fallback/override behaviour, and the name-index relative
    /// encoding fix that is the prerequisite for WI-12 Phase 3c (variable name mapping).
    /// </summary>
    [TestClass]
    public class SourceMapTests
    {
        /// <summary>
        /// The generated JSON must include the V3 version, the file, and the
        /// <c>mappings</c> field. The mappings field is validated in a separate,
        /// focused test; here we only check that the scaffold is present.
        /// </summary>
        [TestMethod]
        public void ToString_EmptyMap_ProducesV3Scaffold()
        {
            var map = new SourceMap { File = "out.js" };

            string json = map.ToString();

            StringAssert.Contains(json, "\"version\": \"3\"");
            StringAssert.Contains(json, "\"file\": \"out.js\"");
            StringAssert.Contains(json, "\"mappings\": ");
        }

        /// <summary>
        /// When <see cref="SourceMap.SourceRoot"/> is not set, the legacy
        /// <c>{file}.ashx</c> fallback is used for backward compatibility with the
        /// bundled <c>SrcMapper.ashx</c> handler.
        /// </summary>
        [TestMethod]
        public void ToString_NoSourceRootOverride_FallsBackToAshxHandler()
        {
            var map = new SourceMap { File = "Out.js" };

            string json = map.ToString();

            StringAssert.Contains(json, "\"sourceRoot\": \"Out.ashx\"");
        }

        /// <summary>
        /// When <see cref="SourceMap.SourceRoot"/> is set explicitly, the configured
        /// value flows into the generated JSON unchanged — enabling Phase 4a/4b
        /// (custom handler path, repo URL).
        /// </summary>
        [TestMethod]
        public void ToString_SourceRootSet_UsesConfiguredValue()
        {
            var map = new SourceMap
            {
                File = "Out.js",
                SourceRoot = "https://example.com/src/",
            };

            string json = map.ToString();

            StringAssert.Contains(json, "\"sourceRoot\": \"https://example.com/src/\"");
            Assert.IsFalse(
                json.Contains("Out.ashx"),
                "Configured SourceRoot must override the .ashx fallback");
        }

        /// <summary>
        /// The WI-16 sidecar flag defaults to <c>true</c> so existing deployments that
        /// depend on the bundled <c>SrcMapper.ashx</c> handler are unaffected by the
        /// new ASP.NET Core source handler.
        /// </summary>
        [TestMethod]
        public void EmitLegacyAshxHandler_DefaultsToTrue_ForBackCompat()
        {
            var map = new SourceMap();

            Assert.IsTrue(map.EmitLegacyAshxHandler);
        }

        /// <summary>
        /// When <see cref="SourceMap.EmitLegacyAshxHandler"/> is <c>false</c>,
        /// <see cref="SourceMap.Write(string)"/> must write the <c>.map</c> file but
        /// NOT emit the legacy <c>.ashx</c> sidecar. This is the on-disk side of the
        /// WI-16 handler replacement.
        /// </summary>
        [TestMethod]
        public void Write_EmitLegacyAshxHandlerFalse_OmitsAshxSidecar()
        {
            string dir = Path.Combine(Path.GetTempPath(), "SourceMapTests_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var map = new SourceMap
                {
                    File = "Out.js",
                    SourceRoot = "/sourcemap/Out/",
                    EmitLegacyAshxHandler = false,
                };

                map.Write(dir);

                Assert.IsTrue(System.IO.File.Exists(Path.Combine(dir, "Out.map")), "Expected .map file to be written");
                Assert.IsFalse(
                    System.IO.File.Exists(Path.Combine(dir, "Out.ashx")),
                    "Legacy .ashx handler must NOT be emitted when EmitLegacyAshxHandler is false");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// When <see cref="SourceMap.EmitLegacyAshxHandler"/> remains <c>true</c> (the
        /// default), the sidecar must still be written — guaranteeing backward
        /// compatibility for deployments that still rely on the embedded WebForms handler.
        /// </summary>
        [TestMethod]
        public void Write_EmitLegacyAshxHandlerTrue_EmitsAshxSidecar()
        {
            string dir = Path.Combine(Path.GetTempPath(), "SourceMapTests_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                var map = new SourceMap { File = "Out.js" };

                map.Write(dir);

                Assert.IsTrue(System.IO.File.Exists(Path.Combine(dir, "Out.map")));
                Assert.IsTrue(
                    System.IO.File.Exists(Path.Combine(dir, "Out.ashx")),
                    "Legacy .ashx sidecar must still be emitted by default (backward compat)");
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        /// <summary>
        /// Source file paths are normalized (backslashes → forward slashes, <c>:</c>
        /// escaped to <c>$</c>) and the full path appears in <c>sourcesLong</c>.
        /// </summary>
        [TestMethod]
        public void AddMapping_RecordsSourceFileInSourcesArray()
        {
            var map = new SourceMap { File = "out.js" };
            map.AddMapping(sLine: 0, sCol: 0, tLine: 5, tCol: 0, file: "Program.cs");

            string json = map.ToString();

            StringAssert.Contains(json, "\"sources\":");
            StringAssert.Contains(json, "Program.cs");
        }

        /// <summary>
        /// When a name is passed to <see cref="SourceMap.AddMapping"/>, it must be
        /// recorded in the <c>names</c> array so downstream consumers can look it up
        /// via the VLQ-encoded name index.
        /// </summary>
        [TestMethod]
        public void AddMapping_WithName_PopulatesNamesArray()
        {
            var map = new SourceMap { File = "out.js" };
            map.AddMapping(0, 0, 1, 0, "Program.cs", name: "myVariable");

            string json = map.ToString();

            StringAssert.Contains(json, "\"names\": [\"myVariable\"]");
        }

        /// <summary>
        /// Critical V3-spec regression test — the name index must be encoded relative
        /// to the PREVIOUS emitted name index (not absolute, not relative to the prior
        /// mapping's name field). Two mappings with different names should produce a
        /// positive delta of 1 for the second, encoded as VLQ "C".
        /// </summary>
        [TestMethod]
        public void ToString_MultipleMappingsWithNames_EncodesNameIndexRelative()
        {
            var map = new SourceMap { File = "out.js" };
            map.AddMapping(0, 0, 5, 0, "Program.cs", name: "first");
            map.AddMapping(0, 1, 6, 0, "Program.cs", name: "second");

            string json = map.ToString();
            string mappings = ExtractMappingsField(json);

            var segments = DecodeFirstLineSegments(mappings);
            Assert.AreEqual(2, segments.Count, "Expected two segments on the first line");

            // Segment layout per V3 spec: [genCol, srcFile, srcLine, srcCol, nameIndex].
            Assert.AreEqual(0, segments[0][4], "First mapping's name index is delta-from-zero = 0 ('first' at index 0).");
            Assert.AreEqual(1, segments[1][4], "Second mapping's name index must be encoded as a DELTA (second - first = 1).");
        }

        /// <summary>
        /// Strong regression test for the V3 name-index delta encoding — uses three
        /// distinct names so the third name's delta (1) differs from its absolute
        /// index (2). Under the old buggy encoding that emitted absolute indices this
        /// test would fail on the third segment.
        /// </summary>
        [TestMethod]
        public void ToString_ThreeDistinctNames_ThirdEncodedRelativeToSecond()
        {
            var map = new SourceMap { File = "out.js" };
            map.AddMapping(0, 0, 0, 0, "Program.cs", name: "alpha");  // index 0
            map.AddMapping(0, 1, 0, 5, "Program.cs", name: "beta");   // index 1
            map.AddMapping(0, 2, 0, 10, "Program.cs", name: "gamma"); // index 2

            var segments = DecodeFirstLineSegments(ExtractMappingsField(map.ToString()));

            Assert.AreEqual(3, segments.Count, "Expected three segments on the first line");
            Assert.AreEqual(0, segments[0][4], "alpha: delta from 0 = 0");
            Assert.AreEqual(1, segments[1][4], "beta: delta from 0 = 1");
            Assert.AreEqual(1, segments[2][4], "gamma: delta from beta(1) = 1, NOT absolute 2");
        }

        /// <summary>
        /// When a mapping has no name, subsequent name-bearing mappings must still be
        /// encoded relative to the last emitted name index — NOT poisoned by the
        /// nameless mapping in between. Uses three named mappings so the final delta
        /// (1) differs from the absolute index (2), making the test fail under the
        /// old buggy encoding.
        /// </summary>
        [TestMethod]
        public void ToString_NamelessMappingBetween_DoesNotPoisonNameIndexBaseline()
        {
            var map = new SourceMap { File = "out.js" };
            map.AddMapping(0, 0, 0, 0, "Program.cs", name: "alpha");   // name index 0
            map.AddMapping(0, 1, 0, 5, "Program.cs", name: "beta");    // name index 1
            map.AddMapping(0, 2, 0, 10, "Program.cs");                 // nameless
            map.AddMapping(0, 3, 0, 15, "Program.cs", name: "gamma");  // name index 2

            string json = map.ToString();
            string mappings = ExtractMappingsField(json);
            var segments = DecodeFirstLineSegments(mappings);

            Assert.AreEqual(4, segments.Count);
            Assert.AreEqual(5, segments[0].Count, "Named segment must emit 5 fields (last is name index)");
            Assert.AreEqual(5, segments[1].Count, "Named segment must emit 5 fields");
            Assert.AreEqual(4, segments[2].Count, "Nameless middle segment must NOT emit a name field");
            Assert.AreEqual(5, segments[3].Count, "Named segment must emit 5 fields");
            Assert.AreEqual(0, segments[0][4], "alpha: delta from 0 = 0");
            Assert.AreEqual(1, segments[1][4], "beta: delta from 0 = 1");
            Assert.AreEqual(
                1,
                segments[3][4],
                "gamma: delta from last emitted name index (beta=1) = 1, NOT absolute 2. "
                + "Nameless segment must not poison the baseline.");
        }

        /// <summary>
        /// When the same name is added twice, the <c>names</c> array must contain a
        /// single entry and both mappings must reference the same name index.
        /// </summary>
        [TestMethod]
        public void AddMapping_DuplicateName_ReusesSingleNamesEntry()
        {
            var map = new SourceMap { File = "out.js" };
            map.AddMapping(0, 0, 0, 0, "Program.cs", name: "shared");
            map.AddMapping(0, 1, 0, 5, "Program.cs", name: "shared");

            string json = map.ToString();

            StringAssert.Contains(json, "\"names\": [\"shared\"]");
            Assert.IsFalse(
                json.Contains("\"shared\", \"shared\""),
                "Duplicate name must not be appended a second time to the names array");

            var segments = DecodeFirstLineSegments(ExtractMappingsField(json));
            Assert.AreEqual(2, segments.Count);
            Assert.AreEqual(0, segments[0][4], "First occurrence: delta from 0 = 0");
            Assert.AreEqual(
                0,
                segments[1][4],
                "Second occurrence of the same name must reuse the existing index (delta = 0)");
        }

        /// <summary>
        /// When neither <see cref="SourceMap.File"/> nor <see cref="SourceMap.SourceRoot"/>
        /// is set, the emitted <c>sourceRoot</c> must fall back to <see cref="string.Empty"/>
        /// rather than producing a broken <c>.ashx</c> reference or throwing.
        /// </summary>
        [TestMethod]
        public void ToString_NoFileAndNoSourceRoot_FallsBackToEmptyString()
        {
            var map = new SourceMap();

            string json = map.ToString();

            StringAssert.Contains(json, "\"sourceRoot\": \"\"");
            Assert.IsFalse(json.Contains(".ashx"), "Without a File there is nothing to base a .ashx fallback on");
        }

        /// <summary>
        /// When <see cref="SourceMap.RepoRoot"/> is unset, source paths keep the legacy
        /// absolutized form (drive letter escaped to <c>$</c>, backslashes flipped). This
        /// guarantees existing consumers see no change unless they opt in.
        /// </summary>
        [TestMethod]
        public void ToString_NoRepoRoot_UsesLegacyAbsolutizedSources()
        {
            var map = new SourceMap { File = "out.js" };
            string fakeFile = Path.Combine(Path.GetTempPath(), "Sources", "Foo.cs");
            map.AddMapping(0, 0, 0, 0, fakeFile);

            string json = map.ToString();

            Assert.IsFalse(
                json.Contains("\"sources\": [\"Sources/Foo.cs\""),
                "Without RepoRoot, sources[] must remain in the absolutized legacy form");
        }

        /// <summary>
        /// When <see cref="SourceMap.RepoRoot"/> is set to an ancestor of the source file,
        /// the <c>sources[]</c> entry is emitted as a forward-slash, repo-relative path so
        /// the browser can combine it with a remote-repo <c>sourceRoot</c> URL.
        /// </summary>
        [TestMethod]
        public void ToString_WithRepoRoot_EmitsRepoRelativeForwardSlashSource()
        {
            string repoRoot = Path.Combine(Path.GetTempPath(), "wi19-repo-" + System.Guid.NewGuid().ToString("N"));
            string fakeFile = Path.Combine(repoRoot, "Sources", "Compiler", "Foo.cs");

            var map = new SourceMap
            {
                File = "out.js",
                RepoRoot = repoRoot,
            };
            map.AddMapping(0, 0, 0, 0, fakeFile);

            string json = map.ToString();

            StringAssert.Contains(json, "\"Sources/Compiler/Foo.cs\"");
        }

        /// <summary>
        /// Files that live outside the configured <see cref="SourceMap.RepoRoot"/> stay in
        /// the legacy absolutized form — the rebase only fires for files actually under the
        /// repo. Mixing in-repo and out-of-repo sources in the same map is supported.
        /// </summary>
        [TestMethod]
        public void ToString_FileOutsideRepoRoot_StaysAbsolutized()
        {
            string repoRoot = Path.Combine(Path.GetTempPath(), "wi19-inside-" + System.Guid.NewGuid().ToString("N"));
            string outsideRoot = Path.Combine(Path.GetTempPath(), "wi19-outside-" + System.Guid.NewGuid().ToString("N"));
            string outsideFile = Path.Combine(outsideRoot, "External", "Bar.cs");

            var map = new SourceMap
            {
                File = "out.js",
                RepoRoot = repoRoot,
            };
            map.AddMapping(0, 0, 0, 0, outsideFile);

            string json = map.ToString();

            // The file should NOT appear as a clean repo-relative path.
            Assert.IsFalse(json.Contains("\"External/Bar.cs\""),
                "File outside RepoRoot must NOT be emitted as a bare repo-relative path");

            // sourcesLong must still preserve the original path.
            StringAssert.Contains(json, "\"sourcesLong\":");
        }

        /// <summary>
        /// Repo-rebase preserves the long form unchanged in <c>sourcesLong</c>; only the
        /// browser-facing <c>sources[]</c> array is rebased. This keeps the local-disk
        /// resolution path (used by the ASP.NET Core handler when a file IS available
        /// locally) working.
        /// </summary>
        [TestMethod]
        public void ToString_WithRepoRoot_PreservesSourcesLongForLocalResolution()
        {
            string repoRoot = Path.Combine(Path.GetTempPath(), "wi19-long-" + System.Guid.NewGuid().ToString("N"));
            string fakeFile = Path.Combine(repoRoot, "Sources", "Foo.cs");

            var map = new SourceMap
            {
                File = "out.js",
                RepoRoot = repoRoot,
            };
            map.AddMapping(0, 0, 0, 0, fakeFile);

            string json = map.ToString();

            // sourcesLong must still contain the absolute, escaped form (with backslashes
            // doubled as the AddMapping path requires).
            StringAssert.Contains(json, "\"sourcesLong\":");
            StringAssert.Contains(json, "Foo.cs");
        }

        private static string ExtractMappingsField(string json)
        {
            const string marker = "\"mappings\": \"";
            int start = json.IndexOf(marker, System.StringComparison.Ordinal);
            Assert.IsTrue(start >= 0, "mappings field not found");
            start += marker.Length;
            int end = json.IndexOf('"', start);
            Assert.IsTrue(end > start, "mappings field not terminated");
            return json.Substring(start, end - start);
        }

        /// <summary>
        /// Decodes the comma-separated segments on the first line (before any <c>;</c>).
        /// Each segment is returned as its list of decoded VLQ values.
        /// </summary>
        private static System.Collections.Generic.List<System.Collections.Generic.List<int>> DecodeFirstLineSegments(string mappings)
        {
            var result = new System.Collections.Generic.List<System.Collections.Generic.List<int>>();
            int semi = mappings.IndexOf(';');
            string firstLine = semi >= 0 ? mappings.Substring(0, semi) : mappings;

            foreach (string segment in firstLine.Split(','))
            {
                if (segment.Length == 0)
                {
                    continue;
                }

                var values = new System.Collections.Generic.List<int>();
                int pos = 0;
                while (pos < segment.Length)
                {
                    values.Add(Base64VLQDecoder.Decode(segment, ref pos));
                }
                result.Add(values);
            }

            return result;
        }
    }
}
