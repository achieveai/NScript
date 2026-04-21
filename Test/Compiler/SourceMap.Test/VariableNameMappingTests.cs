using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.Utils;

namespace OwaSourceMapper.Test
{
    /// <summary>
    /// WI-17 / Phase 3c coverage — verifies that original, pre-minification identifier
    /// names flow from <see cref="IIdentifier.OriginalSuggestedName"/> through
    /// <see cref="JSWriter"/> into the V3 source map's <c>names</c> array.
    ///
    /// Browser DevTools uses the <c>names</c> array to display original C# identifiers
    /// for renamed JS symbols during stepping / hover / scope inspection. A missing or
    /// empty <c>names</c> array means minified names leak through unchanged.
    /// </summary>
    [TestClass]
    public class VariableNameMappingTests
    {
        /// <summary>
        /// When two non-enforced identifiers share the same suggested name, the scope
        /// suffixes the second with a slot index — so <c>GetName()</c> returns a renamed
        /// form while <c>OriginalSuggestedName</c> keeps the user-visible original. That
        /// delta is the exact trigger for a <c>names</c>-array entry, and the array must
        /// contain the ORIGINAL name, not the renamed emitted form.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_RenamedIdentifier_PopulatesNamesArray()
        {
            var scope = new IdentifierScope(isExecutionScope: true);
            var first = SimpleIdentifier.CreateScopeIdentifier(scope, "counter", enforceSuggestion: false);
            var second = SimpleIdentifier.CreateScopeIdentifier(scope, "counter", enforceSuggestion: false);

            // Sanity: the second identifier is the one that gets renamed by the scope.
            Assert.AreNotEqual(
                second.OriginalSuggestedName,
                second.GetName(),
                "Test precondition: slot-indexed rename must produce emitted != original.");

            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 10, 5, 10, 13));
            writer.Write(second);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            StringAssert.Contains(
                json,
                "\"names\": [\"counter\"]",
                "Pre-minification identifier name must appear in the names array.\n" + json);
        }

        /// <summary>
        /// An identifier whose emitted name equals its original (not renamed) must NOT
        /// be added to the <c>names</c> array — there is nothing to resolve back to and
        /// an entry would only bloat the map.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_UnrenamedIdentifier_DoesNotPopulateNamesArray()
        {
            var scope = new IdentifierScope(isExecutionScope: true);
            var identifier = SimpleIdentifier.CreateScopeIdentifier(
                scope,
                suggestedName: "counter",
                enforceSuggestion: true);

            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 3, 0, 3, 7));
            writer.Write(identifier);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            Assert.IsFalse(
                json.Contains("\"names\":"),
                "No names should be recorded when emitted name == original.\n" + json);
        }

        /// <summary>
        /// Two independently-renamed identifiers that share the same ORIGINAL name must
        /// collapse to a single <c>names</c>-array entry — verifying the de-duplication
        /// path in <see cref="SourceMap.AddMapping"/>.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_SameOriginalNameTwice_DeduplicatedInNamesArray()
        {
            var outerScope = new IdentifierScope(isExecutionScope: true);
            // Three with the same name — the 2nd and 3rd both get renamed, both
            // trace back to the same original "counter".
            SimpleIdentifier.CreateScopeIdentifier(outerScope, "counter", enforceSuggestion: false);
            var renamedA = SimpleIdentifier.CreateScopeIdentifier(outerScope, "counter", enforceSuggestion: false);
            var renamedB = SimpleIdentifier.CreateScopeIdentifier(outerScope, "counter", enforceSuggestion: false);

            Assert.AreNotEqual(renamedA.OriginalSuggestedName, renamedA.GetName());
            Assert.AreNotEqual(renamedB.OriginalSuggestedName, renamedB.GetName());
            Assert.AreNotEqual(renamedA.GetName(), renamedB.GetName());

            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 1, 0, 1, 7));
            writer.Write(renamedA);
            writer.LeaveLocation();
            writer.EnterLocation(new Location("Program.cs", 2, 0, 2, 7));
            writer.Write(renamedB);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            StringAssert.Contains(json, "\"names\": [\"counter\"]");
            Assert.IsFalse(
                json.Contains("\"counter\",\"counter\""),
                "Duplicate original name must not be repeated in the names array.");
        }

        /// <summary>
        /// <see cref="JSWriter.WriteIdentifier(string)"/> has no <see cref="IIdentifier"/>
        /// context and therefore cannot know the pre-minification name. It must NOT
        /// populate the <c>names</c> array — the test seam continues to emit unnamed
        /// mappings for raw identifier strings.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_RawWriteIdentifier_DoesNotPopulateNamesArray()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 1, 0, 1, 3));
            writer.WriteIdentifier("a");
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            Assert.IsFalse(
                map.ToString().Contains("\"names\":"),
                "WriteIdentifier(string) has no original-name context and must not emit a names entry.");
        }

        /// <summary>
        /// A non-identifier token (keyword, symbol) whose location differs from prior
        /// tokens must never write into the <c>names</c> array, regardless of what
        /// identifiers surround it. Guards the type-dispatch inside the token loop.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_KeywordToken_DoesNotPopulateNamesArray()
        {
            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 1, 0, 1, 6));
            writer.Write(Keyword.Return);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");

            Assert.IsFalse(
                map.ToString().Contains("\"names\":"),
                "Keyword tokens must not add entries to the names array.");
        }

        /// <summary>
        /// Strong structural check — decodes the VLQ <c>mappings</c> payload and confirms
        /// the renamed-identifier segment has exactly 5 fields and that the 5th field
        /// (name index, delta-encoded) is 0 for the first-and-only name. Without this
        /// test the suite asserts only on the <c>names</c> array contents and would miss
        /// a regression where the name index was dropped from the VLQ segment itself.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_RenamedIdentifier_MappingSegmentEncodesNameIndex()
        {
            var scope = new IdentifierScope(isExecutionScope: true);
            SimpleIdentifier.CreateScopeIdentifier(scope, "counter", enforceSuggestion: false);
            var renamed = SimpleIdentifier.CreateScopeIdentifier(scope, "counter", enforceSuggestion: false);

            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 10, 5, 10, 13));
            writer.Write(renamed);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            var segments = DecodeFirstLineSegments(ExtractMappingsField(json));
            Assert.AreEqual(1, segments.Count, "Expected one mapping segment on the first line.\n" + json);
            Assert.AreEqual(
                5,
                segments[0].Count,
                "Renamed identifier's segment must carry all 5 V3 fields (genCol, srcFile, srcLine, srcCol, nameIndex).\n" + json);
            Assert.AreEqual(
                0,
                segments[0][4],
                "First-and-only name: delta from baseline 0 = 0.\n" + json);
        }

        /// <summary>
        /// Integration-level regression for the Phase 1 <c>previousNameIndex</c> fix —
        /// drives it through <see cref="JSWriter.Write(IIdentifier)"/> rather than direct
        /// <see cref="SourceMap.AddMapping"/> calls. Writes two renamed identifiers with
        /// a keyword between them; the second identifier's name-index delta must be
        /// encoded relative to the first identifier's name index, not poisoned by the
        /// intervening nameless keyword mapping.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_KeywordBetweenRenamedIdentifiers_NameIndexBaselinePreserved()
        {
            var scope = new IdentifierScope(isExecutionScope: true);

            // Two separate collision pairs → two distinct renamed identifiers with
            // different original names ("alpha" and "beta").
            SimpleIdentifier.CreateScopeIdentifier(scope, "alpha", enforceSuggestion: false);
            var renamedAlpha = SimpleIdentifier.CreateScopeIdentifier(scope, "alpha", enforceSuggestion: false);
            SimpleIdentifier.CreateScopeIdentifier(scope, "beta", enforceSuggestion: false);
            var renamedBeta = SimpleIdentifier.CreateScopeIdentifier(scope, "beta", enforceSuggestion: false);

            Assert.AreNotEqual(renamedAlpha.OriginalSuggestedName, renamedAlpha.GetName());
            Assert.AreNotEqual(renamedBeta.OriginalSuggestedName, renamedBeta.GetName());

            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 1, 0, 1, 5));
            writer.Write(renamedAlpha);
            writer.LeaveLocation();
            writer.EnterLocation(new Location("Program.cs", 2, 0, 2, 6));
            writer.Write(Keyword.Return);
            writer.LeaveLocation();
            writer.EnterLocation(new Location("Program.cs", 3, 0, 3, 4));
            writer.Write(renamedBeta);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            Assert.IsTrue(
                json.Contains("\"names\": [\"alpha\",\"beta\"]")
                    || json.Contains("\"names\": [\"alpha\", \"beta\"]"),
                "Expected names array containing alpha and beta in the JSON output.");

            var segments = DecodeFirstLineSegments(ExtractMappingsField(json));
            Assert.AreEqual(3, segments.Count, "Expected three segments: alpha, keyword, beta.");
            Assert.AreEqual(5, segments[0].Count, "Renamed alpha segment must carry a name field.");
            Assert.AreEqual(4, segments[1].Count, "Keyword segment must NOT carry a name field.");
            Assert.AreEqual(5, segments[2].Count, "Renamed beta segment must carry a name field.");
            Assert.AreEqual(0, segments[0][4], "alpha: delta from baseline 0 = 0.");
            Assert.AreEqual(
                1,
                segments[2][4],
                "beta: delta from last EMITTED name index (alpha=0) = 1. "
                + "Intervening nameless keyword mapping must not poison the baseline.");
        }

        /// <summary>
        /// <see cref="CompoundIdentifier"/> (member access like <c>counter.field</c>) takes
        /// the <c>else</c> branch of <see cref="JSWriter.Write(IIdentifier)"/> and recursively
        /// writes each sub-identifier. When the leading sub-identifier is renamed, its
        /// original name must still flow into the <c>names</c> array via the recursive
        /// <see cref="SimpleIdentifier"/> path — guarding against future refactoring that
        /// might special-case the compound branch and drop name propagation.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_CompoundIdentifier_RenamedLeadingPart_PopulatesNamesArray()
        {
            var scope = new IdentifierScope(isExecutionScope: true);
            SimpleIdentifier.CreateScopeIdentifier(scope, "counter", enforceSuggestion: false);
            var renamedCounter = SimpleIdentifier.CreateScopeIdentifier(scope, "counter", enforceSuggestion: false);
            var field = SimpleIdentifier.CreateScopeIdentifier(scope, "field", enforceSuggestion: true);

            var compound = new CompoundIdentifier(renamedCounter, field);

            var writer = new JSWriter(isIndented: false, isOptimized: false);
            writer.EnterLocation(new Location("Program.cs", 7, 4, 7, 17));
            writer.Write(compound);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            StringAssert.Contains(
                json,
                "\"names\": [\"counter\"]",
                "Renamed leading sub-identifier of a CompoundIdentifier must populate the names array.\n" + json);

            var segments = DecodeFirstLineSegments(ExtractMappingsField(json));
            var nameBearing = segments.FindAll(s => s.Count == 5);
            Assert.AreEqual(
                1,
                nameBearing.Count,
                "Exactly one mapping segment must carry a name-index field for the renamed leading sub-identifier.\n" + json);
            Assert.AreEqual(
                0,
                nameBearing[0][4],
                "First-and-only name: delta from baseline 0 = 0.\n" + json);
        }

        /// <summary>
        /// When a <see cref="CompoundIdentifier"/>'s sub-identifiers are written under
        /// separate <see cref="JSWriter.EnterLocation"/> spans (the pattern real codegen
        /// uses when each member-access part carries its own C# source span), each renamed
        /// sub-identifier must contribute its own <c>names</c>-array entry. Verifies the
        /// recursive dispatch still flows original names for every sub-identifier.
        /// </summary>
        [TestMethod]
        public void WriteWithMap_CompoundIdentifierParts_PerPartLocation_BothNamesRecorded()
        {
            var scope = new IdentifierScope(isExecutionScope: true);
            SimpleIdentifier.CreateScopeIdentifier(scope, "alpha", enforceSuggestion: false);
            var renamedAlpha = SimpleIdentifier.CreateScopeIdentifier(scope, "alpha", enforceSuggestion: false);
            SimpleIdentifier.CreateScopeIdentifier(scope, "beta", enforceSuggestion: false);
            var renamedBeta = SimpleIdentifier.CreateScopeIdentifier(scope, "beta", enforceSuggestion: false);

            var writer = new JSWriter(isIndented: false, isOptimized: false);

            // Simulate a member-access whose receiver and member live at distinct source
            // spans — each Write-of-sub-identifier happens inside its own EnterLocation.
            writer.EnterLocation(new Location("Program.cs", 4, 0, 4, 5));
            writer.Write((IIdentifier)renamedAlpha);
            writer.LeaveLocation();

            writer.EnterLocation(new Location("Program.cs", 4, 6, 4, 10));
            writer.Write((IIdentifier)renamedBeta);
            writer.LeaveLocation();

            using var stringWriter = new StringWriter();
            var map = writer.WriteWithMap(stringWriter, "out.js");
            string json = map.ToString();

            Assert.IsTrue(
                json.Contains("\"names\": [\"alpha\",\"beta\"]")
                    || json.Contains("\"names\": [\"alpha\", \"beta\"]"),
                "Both sub-identifier original names must appear when each is written under its own location span.\n" + json);
        }

        private static string ExtractMappingsField(string json)
        {
            const string marker = "\"mappings\": \"";
            int start = json.IndexOf(marker, System.StringComparison.Ordinal);
            Assert.IsTrue(start >= 0, "mappings field not found in:\n" + json);
            start += marker.Length;
            int end = json.IndexOf('"', start);
            Assert.IsTrue(end > start, "mappings field not terminated in:\n" + json);
            return json.Substring(start, end - start);
        }

        /// <summary>
        /// Decodes the comma-separated segments on the first line (before any <c>;</c>).
        /// Each segment is returned as its list of decoded VLQ values in V3 order:
        /// [genCol, srcFile, srcLine, srcCol, nameIndex].
        /// </summary>
        private static List<List<int>> DecodeFirstLineSegments(string mappings)
        {
            var result = new List<List<int>>();
            int semi = mappings.IndexOf(';');
            string firstLine = semi >= 0 ? mappings.Substring(0, semi) : mappings;

            foreach (string segment in firstLine.Split(','))
            {
                if (segment.Length == 0)
                {
                    continue;
                }

                var values = new List<int>();
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
