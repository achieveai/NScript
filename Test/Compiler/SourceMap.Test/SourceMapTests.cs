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
        /// When a mapping has no name, subsequent name-bearing mappings must still be
        /// encoded relative to the last emitted name index — NOT poisoned by the
        /// nameless mapping in between.
        /// </summary>
        [TestMethod]
        public void ToString_NamelessMappingBetween_DoesNotPoisonNameIndexBaseline()
        {
            var map = new SourceMap { File = "out.js" };
            map.AddMapping(0, 0, 0, 0, "Program.cs", name: "alpha");
            map.AddMapping(0, 1, 0, 5, "Program.cs");
            map.AddMapping(0, 2, 0, 10, "Program.cs", name: "beta");

            string json = map.ToString();
            string mappings = ExtractMappingsField(json);
            var segments = DecodeFirstLineSegments(mappings);

            Assert.AreEqual(3, segments.Count);
            Assert.AreEqual(5, segments[0].Count, "Named segment must emit 5 fields (last is name index)");
            Assert.AreEqual(4, segments[1].Count, "Nameless middle segment must NOT emit a name field");
            Assert.AreEqual(5, segments[2].Count, "Named segment must emit 5 fields");
            Assert.AreEqual(1, segments[2][4], "Name index for 'beta' must be 1 (relative to 'alpha'=0)");
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
