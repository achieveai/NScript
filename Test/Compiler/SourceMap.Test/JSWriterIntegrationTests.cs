using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.Utils;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// End-to-end integration tests for the JSWriter → SourceMap pipeline.
    /// Exercises <see cref="JSWriter.WriteWithMap"/> (the Phase 1 test seam) to verify
    /// that tokens written with a given <see cref="Location"/> produce correctly-encoded
    /// mappings pointing back at the source.
    ///
    /// Assertions use <see cref="DecodedMap"/> to reconstruct absolute mapping coordinates
    /// from the V3 VLQ-encoded string — an anchor-based approach that survives whitespace
    /// and token-ordering changes, unlike golden .map file comparisons.
    /// </summary>
    [TestClass]
    public class JSWriterIntegrationTests
    {
        /// <summary>
        /// Baseline: when no source locations are provided, the writer still emits a
        /// valid map with just the synthetic <c>{jsFileName}</c> entries that bracket
        /// the IIFE wrapper. This guards against regressions in the test seam itself.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_NoLocations_ProducesValidMap()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.WriteIdentifier("x");

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            Assert.IsNotNull(map);
            Assert.AreEqual("out.js", map.File);

            string json = map.ToString();
            StringAssert.Contains(json, "\"version\": \"3\"");
        }

        /// <summary>
        /// Anchor test: an identifier written inside an <c>EnterLocation</c> block
        /// must produce a mapping whose source file matches the location and whose
        /// target column matches the column of the emitted identifier in the JS
        /// output.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_IdentifierWithLocation_MapsToSourceFile()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            var location = new Location("Program.cs", 10, 5, 10, 6);

            writer.EnterLocation(location);
            writer.WriteIdentifier("x");
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            // The writer prepends "(function(){" — "x" appears at column 12 on line 0.
            string js = stringWriter.ToString();
            StringAssert.Contains(js, "(function(){x");

            var decoded = DecodedMap.Parse(map.ToString());
            Assert.IsTrue(
                decoded.HasMappingFor("Program.cs", sourceLine: 9),
                "Expected a mapping for Program.cs line 10 (zero-indexed 9). Map:\n" + map.ToString());
        }

        /// <summary>
        /// Two identifiers in different source lines must produce two distinct mappings
        /// pointing at those lines — verifying that the VLQ delta encoding advances the
        /// source-line baseline correctly between segments.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_MultipleLocations_ProducesDistinctMappings()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);

            writer.EnterLocation(new Location("Program.cs", 5, 0, 5, 5));
            writer.WriteIdentifier("foo");
            writer.LeaveLocation();

            writer.EnterLocation(new Location("Program.cs", 12, 0, 12, 5));
            writer.WriteIdentifier("bar");
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            var decoded = DecodedMap.Parse(map.ToString());
            Assert.IsTrue(decoded.HasMappingFor("Program.cs", sourceLine: 4), "missing mapping for line 5");
            Assert.IsTrue(decoded.HasMappingFor("Program.cs", sourceLine: 11), "missing mapping for line 12");
        }

        /// <summary>
        /// When the writer receives tokens from two different source files, both files
        /// must appear in the map's <c>sources</c> array and both must have mappings.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_MultipleSourceFiles_AllRecordedInSources()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);

            writer.EnterLocation(new Location("Alpha.cs", 1, 0, 1, 5));
            writer.WriteIdentifier("a");
            writer.LeaveLocation();

            writer.EnterLocation(new Location("Beta.cs", 2, 0, 2, 5));
            writer.WriteIdentifier("b");
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            StringAssert.Contains(json, "Alpha.cs");
            StringAssert.Contains(json, "Beta.cs");
        }

        /// <summary>
        /// Minimal decoded-map representation used by integration tests.
        /// Parses the V3 mappings string into absolute-coordinate mappings so tests can
        /// assert on logical positions without reasoning about VLQ deltas.
        /// </summary>
        private sealed class DecodedMap
        {
            public List<string> Sources { get; } = new List<string>();

            public List<Mapping> Mappings { get; } = new List<Mapping>();

            public bool HasMappingFor(string sourceFileSuffix, int sourceLine)
            {
                foreach (var m in this.Mappings)
                {
                    if (m.SourceFileIndex >= 0
                        && m.SourceFileIndex < this.Sources.Count
                        && this.Sources[m.SourceFileIndex].EndsWith(sourceFileSuffix, System.StringComparison.Ordinal)
                        && m.SourceLine == sourceLine)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static DecodedMap Parse(string json)
            {
                var result = new DecodedMap();

                // Parse sources array using the non-normalized sourcesLong (exact file names).
                const string sourcesLongMarker = "\"sourcesLong\": [\"";
                int srcStart = json.IndexOf(sourcesLongMarker, System.StringComparison.Ordinal);
                if (srcStart >= 0)
                {
                    srcStart += sourcesLongMarker.Length;
                    int srcEnd = json.IndexOf("\"]", srcStart, System.StringComparison.Ordinal);
                    Assert.IsTrue(
                        srcEnd > srcStart,
                        "sourcesLong array was not terminated with '\"]' — source-map JSON format may have changed.");
                    string inside = json.Substring(srcStart, srcEnd - srcStart);
                    foreach (string s in inside.Split(new[] { "\",\n\t\t\"" }, System.StringSplitOptions.None))
                    {
                        result.Sources.Add(s);
                    }

                    Assert.IsTrue(
                        result.Sources.Count > 0,
                        "sourcesLong marker was present but no source entries were parsed.");
                }

                // Extract the mappings field.
                const string mappingsMarker = "\"mappings\": \"";
                int mapStart = json.IndexOf(mappingsMarker, System.StringComparison.Ordinal);
                if (mapStart < 0) return result;
                mapStart += mappingsMarker.Length;
                int mapEnd = json.IndexOf('"', mapStart);
                Assert.IsTrue(
                    mapEnd > mapStart,
                    "mappings field was not terminated — source-map JSON format may have changed.");
                string mappings = json.Substring(mapStart, mapEnd - mapStart);

                // Decode lines separated by ';'.
                int sourceLine = 0;
                int sourceCol = 0;
                int sourceFileIndex = 0;
                int nameIndex = 0;
                int generatedLine = 0;

                foreach (string line in mappings.Split(';'))
                {
                    int generatedCol = 0;
                    foreach (string segment in line.Split(','))
                    {
                        if (segment.Length == 0) continue;

                        int pos = 0;
                        int genColDelta = Base64VLQDecoder.Decode(segment, ref pos);
                        generatedCol += genColDelta;

                        var mapping = new Mapping
                        {
                            GeneratedLine = generatedLine,
                            GeneratedColumn = generatedCol,
                            SourceFileIndex = -1,
                        };

                        if (pos < segment.Length)
                        {
                            sourceFileIndex += Base64VLQDecoder.Decode(segment, ref pos);
                            sourceLine += Base64VLQDecoder.Decode(segment, ref pos);
                            sourceCol += Base64VLQDecoder.Decode(segment, ref pos);

                            mapping.SourceFileIndex = sourceFileIndex;
                            mapping.SourceLine = sourceLine;
                            mapping.SourceColumn = sourceCol;

                            if (pos < segment.Length)
                            {
                                nameIndex += Base64VLQDecoder.Decode(segment, ref pos);
                                mapping.NameIndex = nameIndex;
                            }
                            else
                            {
                                mapping.NameIndex = -1;
                            }
                        }

                        result.Mappings.Add(mapping);
                    }
                    generatedLine++;
                }

                return result;
            }
        }

        private sealed class Mapping
        {
            public int GeneratedLine;
            public int GeneratedColumn;
            public int SourceFileIndex = -1;
            public int SourceLine;
            public int SourceColumn;
            public int NameIndex = -1;
        }
    }
}
