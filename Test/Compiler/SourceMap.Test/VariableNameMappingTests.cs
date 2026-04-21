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
    }
}
