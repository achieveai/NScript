namespace Sunlight.Framework.Data.Test
{
    using Sunlight.Framework.Data.WebStore;
    using SunlightUnit;

    /// <summary>
    /// Regression tests for the three defects fixed during the MCQdb port:
    /// <list type="number">
    /// <item>IndexInfoBase ctor was wrongly assigning literal false to both
    ///   IsUnique and IsMultiEntry instead of the constructor parameters.</item>
    /// <item>TableSchema.GetIndex never updated bestScore, so it always picked
    ///   the first serving index instead of the best-fitting one.</item>
    /// <item>IDBIndexParameters was missing [JsonType], breaking minified
    ///   field name access in the emitted JS.</item>
    /// </list>
    /// </summary>
    [TestFixture]
    public class DefectRegressionTests
    {
        /// <summary>
        /// Defect #1 — SingleIndexInfo with explicit isUnique/isMultiEntry must
        /// round-trip those flags through the base ctor. Before the fix, both
        /// fields were clobbered to false regardless of the ctor args.
        /// </summary>
        [Test]
        public static void TestSingleIndexInfoCtorAssignsFlags(Assert assert)
        {
            var both = new SingleIndexInfo("idx-both", "FolderId", true, true);
            assert.IsTrue(both.IsUnique, "SingleIndexInfo: IsUnique is true when ctor said true");
            assert.IsTrue(both.IsMultiEntry, "SingleIndexInfo: IsMultiEntry is true when ctor said true");

            var uniqueOnly = new SingleIndexInfo("idx-u", "FolderId", true, false);
            assert.IsTrue(uniqueOnly.IsUnique, "SingleIndexInfo: IsUnique=true, IsMultiEntry=false → unique round-trips");
            assert.IsTrue(!uniqueOnly.IsMultiEntry, "SingleIndexInfo: IsUnique=true, IsMultiEntry=false → multiEntry stays false");

            var multiOnly = new SingleIndexInfo("idx-m", "FolderId", false, true);
            assert.IsTrue(!multiOnly.IsUnique, "SingleIndexInfo: IsUnique=false, IsMultiEntry=true → unique stays false");
            assert.IsTrue(multiOnly.IsMultiEntry, "SingleIndexInfo: IsUnique=false, IsMultiEntry=true → multiEntry round-trips");

            var neither = new SingleIndexInfo("idx-none", "FolderId");
            assert.IsTrue(!neither.IsUnique, "SingleIndexInfo: defaults → IsUnique=false");
            assert.IsTrue(!neither.IsMultiEntry, "SingleIndexInfo: defaults → IsMultiEntry=false");
        }

        /// <summary>
        /// Defect #1 — MultiColumnIndexInfo also went through the buggy base ctor.
        /// Same flags assertion through the composite subtype.
        /// </summary>
        [Test]
        public static void TestMultiColumnIndexInfoCtorAssignsFlags(Assert assert)
        {
            var multi = new MultiColumnIndexInfo(
                "idx-multi",
                new string[] { "FolderId", "DueDate" },
                true,
                true);
            assert.IsTrue(multi.IsUnique, "MultiColumnIndexInfo: IsUnique round-trips");
            assert.IsTrue(multi.IsMultiEntry, "MultiColumnIndexInfo: IsMultiEntry round-trips");
        }

        /// <summary>
        /// Defect #2 — when multiple indexes can serve a query, <see cref="TableSchema.GetIndex"/>
        /// must pick the lowest score. Before the fix, bestScore never decreased,
        /// so the first serving index in declaration order always won. We
        /// construct a schema where a MultiColumnIndexInfo (imperfect fit, score = 1)
        /// appears before a SingleIndexInfo (perfect fit, score = 0) on the
        /// same key path — the single-column index must be selected.
        /// </summary>
        [Test]
        public static void TestGetIndexPicksLowestScoringIndex(Assert assert)
        {
            var schema = new TableSchema(
                "todos",
                new KeyInfo("Id", false),
                new MultiColumnIndexInfo("folderThenDue", new string[] { "FolderId", "DueDate" }),
                new SingleIndexInfo("folderId", "FolderId"));

            var query = new QueryBuilder(new string[] { "FolderId" }).Equal<string>("f1").Build();
            var selected = schema.GetIndex(query);
            assert.IsTrue(selected != null, "GetIndex returns a matching index");
            assert.Equal(selected.Name, "folderId", "best-scoring index (SingleIndexInfo with score 0) is selected");
        }

        /// <summary>
        /// Defect #2 — same scenario but declared in the other order to prove
        /// GetIndex is genuinely ranking, not just preferring later entries.
        /// SingleIndexInfo first should still win because its score (0) is
        /// lower than MultiColumnIndexInfo's partial-fit score (1).
        /// </summary>
        [Test]
        public static void TestGetIndexRankingIsDeclarationOrderIndependent(Assert assert)
        {
            var schema = new TableSchema(
                "todos",
                new KeyInfo("Id", false),
                new SingleIndexInfo("folderId", "FolderId"),
                new MultiColumnIndexInfo("folderThenDue", new string[] { "FolderId", "DueDate" }));

            var query = new QueryBuilder(new string[] { "FolderId" }).Equal<string>("f1").Build();
            var selected = schema.GetIndex(query);
            assert.IsTrue(selected != null, "GetIndex returns a matching index regardless of declaration order");
            assert.Equal(selected.Name, "folderId", "SingleIndexInfo still wins when declared first");
        }

        /// <summary>
        /// Defect #2 — when no index matches (query key path is unrelated),
        /// GetIndex must return null rather than an arbitrary index.
        /// </summary>
        [Test]
        public static void TestGetIndexReturnsNullWhenNothingMatches(Assert assert)
        {
            var schema = new TableSchema(
                "todos",
                new KeyInfo("Id", false),
                new SingleIndexInfo("folderId", "FolderId"));

            var query = new QueryBuilder(new string[] { "Unknown" }).Equal<string>("x").Build();
            var selected = schema.GetIndex(query);
            assert.IsTrue(selected == null, "GetIndex returns null when no index can serve the query");
        }
    }
}
