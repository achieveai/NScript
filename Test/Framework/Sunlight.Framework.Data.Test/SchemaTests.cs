namespace Sunlight.Framework.Data.Test
{
    using Sunlight.Framework.Data.WebStore;
    using SunlightUnit;

    /// <summary>
    /// Pure-logic smoke tests for schema primitives — no IndexedDB access.
    /// Guards the lookup contract callers rely on.
    /// </summary>
    [TestFixture]
    public class SchemaTests
    {
        [Test]
        public static void TestWebStoreSchemaLookup(Assert assert)
        {
            var folders = new TableSchema("folders", new KeyInfo("Id", false));
            var todos = new TableSchema("todos", new KeyInfo("Id", false));

            var schema = new WebStoreSchema("testDb", 1, folders, todos);

            assert.Equal(schema.DatabaseName, "testDb", "DatabaseName round-trips");
            assert.Equal(schema.VersionId, 1, "VersionId round-trips");
            assert.IsTrue(schema.HasTableSchema("folders"), "HasTableSchema finds a declared table");
            assert.IsTrue(schema.HasTableSchema("todos"), "HasTableSchema finds the second table");
            assert.IsTrue(!schema.HasTableSchema("missing"), "HasTableSchema returns false for undeclared tables");

            var folderSchema = schema.GetTableSchema("folders");
            assert.IsTrue(folderSchema == folders, "GetTableSchema returns the exact declared instance");
        }

        [Test]
        public static void TestKeyInfoRoundTripsParameters(Assert assert)
        {
            var key = new KeyInfo("MyKey", true);
            assert.Equal(key.KeyPath, "MyKey", "KeyPath round-trips");
            assert.IsTrue(key.AutoIncrement == true, "AutoIncrement round-trips when true");

            var nonAuto = new KeyInfo("Id", false);
            assert.IsTrue(nonAuto.AutoIncrement == false, "AutoIncrement round-trips when false");
        }

        [Test]
        public static void TestCanUsePrimaryIndexForEmptyQuery(Assert assert)
        {
            var schema = new TableSchema(
                "todos",
                new KeyInfo("Id", false),
                new SingleIndexInfo("folderId", "FolderId"));

            assert.IsTrue(
                schema.CanUsePrimaryIndex(Query.All),
                "empty KeyPaths uses the primary index");
            assert.IsTrue(
                schema.CanUsePrimaryIndex(
                    new QueryBuilder(new string[] { "Id" }).Equal<string>("x").Build()),
                "Query on the primary keyPath uses the primary index");
            assert.IsTrue(
                !schema.CanUsePrimaryIndex(
                    new QueryBuilder(new string[] { "FolderId" }).Equal<string>("f1").Build()),
                "Query on a secondary keyPath does NOT use the primary index");
        }
    }
}
