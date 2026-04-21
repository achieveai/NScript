namespace Sunlight.Framework.Data.Test
{
    using System;
    using System.Runtime.CompilerServices;
    using Sunlight.Framework.Data.WebStore;
    using SunlightUnit;

    /// <summary>
    /// Entity stored by <see cref="WebStoreCrudTests"/>. Plain <see cref="JsonType"/>
    /// POCO — these tests exercise the real browser IndexedDB through the typed
    /// WebStore layer end-to-end.
    /// </summary>
    [JsonType]
    public class CrudEntity
    {
        [ScriptName("Id")]
        public extern string Id { get; set; }

        [ScriptName("Category")]
        public extern string Category { get; set; }

        [ScriptName("Score")]
        public extern int Score { get; set; }
    }

    /// <summary>
    /// End-to-end CRUD smoke tests against a real IndexedDB, exercised through
    /// <see cref="WebStoreClient"/>. Each test uses a uniquely-named database so
    /// parallel QUnit runs stay isolated and don't collide on the open-client
    /// tracker maintained by <see cref="WebStoreFactory"/>.
    /// </summary>
    [TestFixture]
    public class WebStoreCrudTests
    {
        private const string TableName = "items";
        private const string CategoryIndex = "category";
        private static int dbNameCounter;

        /// <summary>
        /// Build a fresh schema with a single-column non-unique secondary index.
        /// Using a per-test database name avoids open-client conflicts across
        /// the shared <see cref="WebStoreFactory.OpenClientTracker"/>.
        /// </summary>
        private static WebStoreSchema BuildSchema(string dbName)
        {
            return new WebStoreSchema(
                dbName,
                1,
                new TableSchema(
                    TableName,
                    new KeyInfo("Id", false),
                    new SingleIndexInfo(CategoryIndex, "Category")));
        }

        private static string NewDbName()
        {
            dbNameCounter = dbNameCounter + 1;
            return "sfdTest_" + dbNameCounter + "_" + GetTimeSuffix();
        }

        [Script(@"return @:Date.now();")]
        private static extern int GetTimeSuffix();

        /// <summary>
        /// Open a fresh DB, UpSert a record, Get it back, and verify field
        /// round-tripping works through structured clone. A round-trip failure
        /// here catches regressions in [JsonType] attribute plumbing because
        /// field name mismatches would surface as null reads.
        /// </summary>
        [Test]
        public static void TestUpSertThenGetRoundTripsFields(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                var entity = new CrudEntity();
                entity.Id = "e1";
                entity.Category = "todo";
                entity.Score = 42;

                table.UpSert(entity).Then<bool>(delegate(string key)
                {
                    assert.Equal(key, "e1", "UpSert resolves with the primary key");

                    table.Get("e1").Then<bool>(delegate(CrudEntity fetched)
                    {
                        assert.IsTrue(fetched != null, "Get returns a record");
                        assert.Equal(fetched.Id, "e1", "Id round-trips");
                        assert.Equal(fetched.Category, "todo", "Category round-trips");
                        assert.Equal(fetched.Score, 42, "Score round-trips");
                        client.Close();
                        done();
                        return true;
                    });
                    return true;
                });
                return true;
            });
        }

        /// <summary>
        /// After deletion, Get should reject (record missing) while TryGet
        /// resolves to null. This guards the "not found" branch of the typed
        /// cursor reader.
        /// </summary>
        [Test]
        public static void TestDeleteRemovesRecord(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                var entity = new CrudEntity();
                entity.Id = "e-del";
                entity.Category = "x";
                entity.Score = 1;

                table.UpSert(entity).Then<bool>(delegate(string ignored)
                {
                    table.Delete("e-del").Then<bool>(delegate(bool deleted)
                    {
                        assert.IsTrue(deleted, "Delete resolves true");

                        table.TryGet("e-del").Then<bool>(delegate(CrudEntity missing)
                        {
                            assert.IsTrue(missing == null, "TryGet returns null after Delete");
                            client.Close();
                            done();
                            return true;
                        });
                        return true;
                    });
                    return true;
                });
                return true;
            });
        }

        /// <summary>
        /// Query(Query.All) after three sequential UpSerts must return every
        /// inserted record. Exercises the cursor scan path and validates that
        /// the secondary index declaration does not corrupt the primary scan.
        /// </summary>
        [Test]
        public static void TestQueryAllReturnsEveryRecord(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("a", "todo", 1)).Then<bool>(delegate(string k1)
                {
                    table.UpSert(NewEntity("b", "done", 2)).Then<bool>(delegate(string k2)
                    {
                        table.UpSert(NewEntity("c", "todo", 3)).Then<bool>(delegate(string k3)
                        {
                            table.Query(Query.All).Then<bool>(delegate(System.Collections.Generic.List<CrudEntity> results)
                            {
                                assert.Equal(results.Count, 3, "Query(All) returns every record");
                                client.Close();
                                done();
                                return true;
                            });
                            return true;
                        });
                        return true;
                    });
                    return true;
                });
                return true;
            });
        }

        /// <summary>
        /// Defect #3 regression — with the missing [JsonType] on IDBIndexParameters,
        /// the <c>Unique</c> field name would not reach the native IDB call so
        /// uniqueness was never enforced. Declaring a UNIQUE single-column
        /// index and trying to insert two records with the same indexed value
        /// must reject the second insert when the attribute is present.
        /// </summary>
        [Test]
        public static void TestUniqueIndexEnforcesConstraint_DefectThree(Assert assert)
        {
            var done = assert.Async(1);
            var dbName = NewDbName();
            var uniqueSchema = new WebStoreSchema(
                dbName,
                1,
                new TableSchema(
                    TableName,
                    new KeyInfo("Id", false),
                    new SingleIndexInfo(CategoryIndex, "Category", true, false)));

            var factory = new WebStoreFactory();
            factory.Create(uniqueSchema).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("one", "shared", 1)).Then<bool>(delegate(string first)
                {
                    assert.Equal(first, "one", "first insert with the indexed category succeeds");

                    table.Add(NewEntity("two", "shared", 2)).Then<bool, object>(
                        delegate(string success)
                        {
                            assert.IsTrue(false, "duplicate unique-index value should have been rejected");
                            client.Close();
                            done();
                            return true;
                        },
                        delegate(object err)
                        {
                            assert.IsTrue(err != null, "duplicate insert rejects when Unique reaches IDB (defect #3)");
                            client.Close();
                            done();
                            return true;
                        });
                    return true;
                });
                return true;
            });
        }

        private static CrudEntity NewEntity(string id, string category, int score)
        {
            var e = new CrudEntity();
            e.Id = id;
            e.Category = category;
            e.Score = score;
            return e;
        }
    }
}
