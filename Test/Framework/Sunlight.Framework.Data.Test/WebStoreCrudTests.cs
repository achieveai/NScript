namespace Sunlight.Framework.Data.Test
{
    using System;
    using System.Collections.Generic;
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
        /// <c>ForEach(Query.All, ...)</c> after three sequential UpSerts must
        /// visit every inserted record. This is the streaming replacement for
        /// the old <c>Query(Query.All)</c> full-table scan and validates that
        /// the secondary index declaration does not corrupt the primary scan.
        /// </summary>
        [Test]
        public static void TestForEachAllVisitsEveryRecord(Assert assert)
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
                            var visited = new List<string>();
                            table.ForEach(Query.All, delegate(CrudEntity row)
                            {
                                visited.Add(row.Id);
                                return true;
                            }).Then<bool>(delegate(int count)
                            {
                                assert.Equal(count, 3, "ForEach(All) visit count matches inserted count");
                                assert.Equal(visited.Count, 3, "visitor received every record");
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
        /// <c>Query(Query.All)</c> on the materialising read path must be
        /// rejected with a descriptive error so callers have to opt into either
        /// streaming via <c>ForEach</c> or an explicit <c>QueryBuilder.Limit</c>.
        /// </summary>
        [Test]
        public static void TestQueryAllMaterialisingThrows(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("a", "todo", 1)).Then<bool>(delegate(string k1)
                {
                    table.Query(Query.All).Then<bool, object>(
                        delegate(List<CrudEntity> results)
                        {
                            assert.IsTrue(false, "Query(Query.All) should reject, not resolve");
                            client.Close();
                            done();
                            return true;
                        },
                        delegate(object err)
                        {
                            assert.IsTrue(err != null, "Query(Query.All) rejects with a descriptive error");
                            var message = ((Exception)err).Message;
                            assert.IsTrue(
                                message != null && message.IndexOf("Query.All") >= 0,
                                "rejection message names Query.All so callers can diagnose");
                            assert.IsTrue(
                                message.IndexOf("Limit") >= 0 || message.IndexOf("ForEach") >= 0,
                                "rejection message points callers at Limit or ForEach as the fix");
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
        /// A <c>ForEach</c> visitor that returns <c>false</c> after the second
        /// record must stop iteration immediately — the cursor is abandoned and
        /// the visit count reflects only the records actually seen.
        /// </summary>
        [Test]
        public static void TestForEachStopsEarly(Assert assert)
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
                            int seen = 0;
                            table.ForEach(Query.All, delegate(CrudEntity row)
                            {
                                seen = seen + 1;
                                return seen < 2;
                            }).Then<bool>(delegate(int count)
                            {
                                assert.Equal(count, 2, "visit count equals records seen before stop");
                                assert.Equal(seen, 2, "visitor invoked exactly twice");
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
        /// With an explicit <c>QueryBuilder.Limit(15)</c> and 15 inserted
        /// records, the scan must return all 15. Proves the limit path still
        /// drains the cursor correctly after the silent <c>1 &lt;&lt; 20</c>
        /// default was removed.
        /// </summary>
        [Test]
        public static void TestLimitedQueryRespectsExplicitLimit(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                var seed = new CrudEntity[15];
                for (int i = 0; i < 15; i = i + 1)
                {
                    seed[i] = NewEntity("r" + i, "todo", i);
                }

                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    var query = new QueryBuilder(new string[0])
                        .Limit(15)
                        .Build();

                    table.Query(query).Then<bool>(delegate(List<CrudEntity> results)
                    {
                        assert.Equal(results.Count, 15, "Limit(15) returns all 15 records");
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
        /// With 5 seeded records and <c>QueryBuilder.Limit(2)</c>, the scan must
        /// stop after 2 records. This is the true regression test for the removal
        /// of the silent <c>1 &lt;&lt; 20</c> default — the previous
        /// <c>TestLimitedQueryRespectsExplicitLimit</c> passed both before and
        /// after the fix because <c>Limit == recordCount</c> is a coincidence.
        /// </summary>
        [Test]
        public static void TestLimitSmallerThanRecordCountTruncates(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                var seed = new CrudEntity[5];
                for (int i = 0; i < 5; i = i + 1)
                {
                    seed[i] = NewEntity("s" + i, "todo", i);
                }

                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    var query = new QueryBuilder(new string[0])
                        .Limit(2)
                        .Build();

                    table.Query(query).Then<bool>(delegate(List<CrudEntity> results)
                    {
                        assert.Equal(results.Count, 2, "Limit(2) truncates a 5-record scan to 2");
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
        /// <c>Query</c> with <c>Limit(0)</c> on a non-empty store must return an
        /// empty list without visiting any record. Pins the pre-visit cap check
        /// specifically — distinct from <c>Limit(N)</c> decrement which is
        /// exercised by <c>TestLimitSmallerThanRecordCountTruncates</c>.
        /// </summary>
        [Test]
        public static void TestLimitZeroVisitsNothing(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("a", "todo", 1)).Then<bool>(delegate(string k1)
                {
                    table.UpSert(NewEntity("b", "todo", 2)).Then<bool>(delegate(string k2)
                    {
                        var query = new QueryBuilder(new string[0])
                            .Limit(0)
                            .Build();

                        table.Query(query).Then<bool>(delegate(List<CrudEntity> results)
                        {
                            assert.Equal(results.Count, 0, "Limit(0) returns no records");
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
        /// <c>ForEach</c> with a null visitor must reject through the Promise —
        /// not throw synchronously. This pins the symmetric error surface
        /// established alongside <c>Query(Query.All)</c>: both argument-
        /// validation failures route through <c>reject</c> so callers using
        /// <c>.Then(_, onRejected)</c> see a uniform API.
        /// </summary>
        [Test]
        public static void TestForEachNullVisitRejects(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.ForEach(Query.All, null).Then<bool, object>(
                    delegate(int count)
                    {
                        assert.IsTrue(false, "ForEach(_, null) should reject, not resolve");
                        client.Close();
                        done();
                        return true;
                    },
                    delegate(object err)
                    {
                        assert.IsTrue(err != null, "ForEach(_, null) rejects with an error");
                        var message = ((Exception)err).Message;
                        assert.IsTrue(
                            message != null && message.IndexOf("visit") >= 0,
                            "rejection message names the bad argument");
                        client.Close();
                        done();
                        return true;
                    });
                return true;
            });
        }

        /// <summary>
        /// <c>ForEach(Query.All, ...)</c> on an empty store must resolve with a
        /// visit count of <c>0</c> and never invoke the visitor. Pins the
        /// null-cursor short-circuit in <c>ForEachInternal</c>.
        /// </summary>
        [Test]
        public static void TestForEachEmptyTableVisitsNothing(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                int visits = 0;

                table.ForEach(Query.All, delegate(CrudEntity row)
                {
                    visits = visits + 1;
                    return true;
                }).Then<bool>(delegate(int count)
                {
                    assert.Equal(count, 0, "empty table resolves with count 0");
                    assert.Equal(visits, 0, "visitor was never invoked on empty table");
                    client.Close();
                    done();
                    return true;
                });
                return true;
            });
        }

        /// <summary>
        /// <c>ForEach</c> combined with <c>QueryBuilder.Limit(2)</c> on a
        /// 5-record store must stop after 2 records — invoking the visitor
        /// exactly twice. Pins the interaction between the streaming ForEach
        /// path and the explicit Limit cap, which were introduced together but
        /// previously only tested independently.
        /// </summary>
        [Test]
        public static void TestForEachRespectsExplicitLimit(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                var seed = new CrudEntity[5];
                for (int i = 0; i < 5; i = i + 1)
                {
                    seed[i] = NewEntity("f" + i, "todo", i);
                }

                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    var query = new QueryBuilder(new string[0])
                        .Limit(2)
                        .Build();
                    int visits = 0;

                    table.ForEach(query, delegate(CrudEntity row)
                    {
                        visits = visits + 1;
                        return true;
                    }).Then<bool>(delegate(int count)
                    {
                        assert.Equal(count, 2, "ForEach + Limit(2) reports visit count of 2");
                        assert.Equal(visits, 2, "visitor invoked exactly twice under Limit(2)");
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
        /// <c>Query(Query.All, filter)</c> with a non-null filter must resolve
        /// (not reject) and return only filter-matching rows. Pins the positive
        /// side of the <c>filter == null</c> escape hatch in the Query.All
        /// guard — without this a refactor that dropped the filter check would
        /// silently break filtered scans.
        /// </summary>
        [Test]
        public static void TestQueryAllWithFilterResolvesFiltered(Assert assert)
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
                            table.Query(Query.All, delegate(CrudEntity row)
                            {
                                return row.Category == "todo";
                            }).Then<bool>(delegate(List<CrudEntity> results)
                            {
                                assert.Equal(results.Count, 2, "Query(All, filter) returns only filtered rows");
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
        /// <c>QueryKeys(Query.All)</c> must resolve with every primary key —
        /// the Query.All materialisation guard exists only for the value-read
        /// path (which can blow up memory with large rows). A key-only scan
        /// carries no such risk, so it bypasses the guard via the
        /// <c>isKeyQuery</c> flag.
        /// </summary>
        [Test]
        public static void TestQueryKeysAllResolves(Assert assert)
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
                        table.QueryKeys(Query.All).Then<bool>(delegate(List<string> keys)
                        {
                            assert.Equal(keys.Count, 2, "QueryKeys(All) returns every key (no cap)");
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

        /// <summary>
        /// A <c>ForEach</c> visitor that throws must reject the returned
        /// Promise with the original exception — not hang the caller. Before
        /// WI-41, the throw escaped the IDB <c>OnSuccess</c> handler and the
        /// promise was never settled. The centralised try/catch in
        /// <c>CursorIterator</c> routes the thrown exception to <c>reject</c>.
        /// </summary>
        [Test]
        public static void TestForEachVisitThrowsRejects(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("a", "todo", 1)).Then<bool>(delegate(string k1)
                {
                    table.UpSert(NewEntity("b", "todo", 2)).Then<bool>(delegate(string k2)
                    {
                        int visitCount = 0;
                        table.ForEach(Query.All, delegate(CrudEntity row)
                        {
                            visitCount++;
                            throw new Exception("boom-visit");
                        }).Then<bool, object>(
                            delegate(int count)
                            {
                                assert.IsTrue(false, "throwing visitor should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                assert.Equal(visitCount, 1, "visitor called exactly once before abort");
                                assert.IsTrue(err != null, "rejection has an error");
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("boom-visit") >= 0,
                                    "original thrown exception propagates to reject");
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
        /// A <c>Query</c> filter that throws must reject the returned Promise
        /// with the original exception. Exercises the <c>filter(cursor.Value)</c>
        /// call edge in <c>CursorIterator</c>, distinct from the <c>onIterate</c>
        /// edge covered by <c>TestForEachVisitThrowsRejects</c>.
        /// </summary>
        [Test]
        public static void TestQueryFilterThrowsRejects(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("a", "todo", 1)).Then<bool>(delegate(string k1)
                {
                    table.UpSert(NewEntity("b", "todo", 2)).Then<bool>(delegate(string k2)
                    {
                        table.Query(Query.All, delegate(CrudEntity row)
                        {
                            throw new Exception("boom-filter");
                        }).Then<bool, object>(
                            delegate(List<CrudEntity> results)
                            {
                                assert.IsTrue(false, "throwing filter should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                assert.IsTrue(err != null, "rejection has an error");
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("boom-filter") >= 0,
                                    "original thrown exception propagates to reject");
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
        /// A <c>Delete(query, filter)</c> whose filter throws must reject and
        /// must not remove any record — the cursor aborts before the batched
        /// key list reaches the delete call. Confirms the aborted flag
        /// prevents subsequent cursor events from scheduling a partial delete.
        /// </summary>
        [Test]
        public static void TestDeleteFilterThrowsRejects(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("a", "todo", 1)).Then<bool>(delegate(string k1)
                {
                    table.UpSert(NewEntity("b", "todo", 2)).Then<bool>(delegate(string k2)
                    {
                        var query = new QueryBuilder(new string[0])
                            .Limit(10)
                            .Build();

                        table.Delete(query, delegate(CrudEntity row)
                        {
                            throw new Exception("boom-delete-filter");
                        }).Then<bool, object>(
                            delegate(int count)
                            {
                                assert.IsTrue(false, "throwing delete filter should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                assert.IsTrue(err != null, "rejection has an error");
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("boom-delete-filter") >= 0,
                                    "original thrown exception propagates to reject");

                                // Both records must survive — the cursor aborted before any batched delete ran.
                                table.TryGet("a").Then<bool>(delegate(CrudEntity rowA)
                                {
                                    assert.IsTrue(rowA != null, "record 'a' survives an aborted delete scan");

                                    table.TryGet("b").Then<bool>(delegate(CrudEntity rowB)
                                    {
                                        assert.IsTrue(rowB != null, "record 'b' survives an aborted delete scan");
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
                return true;
            });
        }

        /// <summary>
        /// An <c>Update(query, updateFunc)</c> whose <c>updateFunc</c> throws
        /// must reject and must not mutate any record. Exercises the
        /// <c>updateFunc(cursor.Value)</c> call edge inside the
        /// <c>QueryUpdateOrDeleteInternal</c> <c>onIterate</c> lambda, reaching
        /// the <c>CursorIterator</c> guard transitively.
        /// </summary>
        [Test]
        public static void TestUpdateFuncThrowsRejects(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);

                table.UpSert(NewEntity("a", "todo", 1)).Then<bool>(delegate(string k1)
                {
                    table.UpSert(NewEntity("b", "todo", 2)).Then<bool>(delegate(string k2)
                    {
                        var query = new QueryBuilder(new string[0])
                            .Limit(10)
                            .Build();

                        table.Update(query, delegate(CrudEntity row)
                        {
                            throw new Exception("boom-update");
                        }).Then<bool, object>(
                            delegate(int count)
                            {
                                assert.IsTrue(false, "throwing updateFunc should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                assert.IsTrue(err != null, "rejection has an error");
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("boom-update") >= 0,
                                    "original thrown exception propagates to reject");

                                // Records unchanged — the cursor aborted before the batched update ran.
                                table.Get("a").Then<bool>(delegate(CrudEntity rowA)
                                {
                                    assert.Equal(rowA.Score, 1, "record 'a' score unchanged after aborted update");

                                    table.Get("b").Then<bool>(delegate(CrudEntity rowB)
                                    {
                                        assert.Equal(rowB.Score, 2, "record 'b' score unchanged after aborted update");
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
                return true;
            });
        }

        /// <summary>
        /// First call to <c>QueryPage(Query.All, 3)</c> over 10 rows must return
        /// 3 rows + a non-null <see cref="Page{TValue}.NextCursor"/>. Pins the
        /// "page filled, more available" branch — the cursor signal is what
        /// callers rely on to drive the loop.
        /// </summary>
        [Test]
        public static void TestQueryPageReturnsFirstPageWithCursor(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                SeedRows(client.Table<string, CrudEntity>(TableName), 10).Then<bool>(delegate(bool seeded)
                {
                    var table = client.Table<string, CrudEntity>(TableName);
                    table.QueryPage(Query.All, 3).Then<bool>(delegate(Page<CrudEntity> page)
                    {
                        assert.Equal(page.Items.Count, 3, "first page returns pageSize rows");
                        assert.IsTrue(page.NextCursor != null, "first page hands back a cursor");
                        assert.IsTrue(page.HasMore, "HasMore mirrors NextCursor != null");
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
        /// Resuming a paged scan with the previous page's <see cref="Page{TValue}.NextCursor"/>
        /// must return the next slice with no overlap and no gaps. This is the
        /// core continuity invariant — without it the API is unusable.
        /// </summary>
        [Test]
        public static void TestQueryPageResumesFromCursor(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                SeedRows(client.Table<string, CrudEntity>(TableName), 10).Then<bool>(delegate(bool seeded)
                {
                    var table = client.Table<string, CrudEntity>(TableName);
                    table.QueryPage(Query.All, 3).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        var firstIds = CollectIds(page1.Items);
                        table.QueryPage(Query.All, 3, page1.NextCursor).Then<bool>(delegate(Page<CrudEntity> page2)
                        {
                            assert.Equal(page2.Items.Count, 3, "second page has pageSize rows");
                            var secondIds = CollectIds(page2.Items);
                            assert.IsTrue(NoOverlap(firstIds, secondIds), "no record appears on both pages");
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
        /// After exhausting the table, <see cref="Page{TValue}.NextCursor"/>
        /// must be <c>null</c> and <see cref="Page{TValue}.HasMore"/> false.
        /// Pins the end-of-stream signal — the loop-termination contract.
        /// </summary>
        [Test]
        public static void TestQueryPageEndOfStream(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                SeedRows(client.Table<string, CrudEntity>(TableName), 5).Then<bool>(delegate(bool seeded)
                {
                    var table = client.Table<string, CrudEntity>(TableName);
                    table.QueryPage(Query.All, 5).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        // Page1 may or may not have NextCursor depending on whether the cursor
                        // continued past the last record. If filled exactly, NextCursor is set
                        // (per documented contract). Resume one more time to drain.
                        Action<Page<CrudEntity>> assertDrained = delegate(Page<CrudEntity> finalPage)
                        {
                            assert.IsTrue(finalPage.NextCursor == null, "exhausted scan returns null cursor");
                            assert.IsTrue(!finalPage.HasMore, "HasMore is false after drain");
                            client.Close();
                            done();
                        };

                        if (page1.NextCursor == null)
                        { assertDrained(page1); }
                        else
                        {
                            table.QueryPage(Query.All, 5, page1.NextCursor).Then<bool>(delegate(Page<CrudEntity> page2)
                            {
                                assertDrained(page2);
                                return true;
                            });
                        }
                        return true;
                    });
                    return true;
                });
                return true;
            });
        }

        /// <summary>
        /// <c>QueryPage</c> on an empty table must resolve with an empty
        /// <see cref="Page{TValue}.Items"/> and a <c>null</c> cursor — never
        /// reject. Empty is a valid first-page state.
        /// </summary>
        [Test]
        public static void TestQueryPageEmptyTable(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                table.QueryPage(Query.All, 3).Then<bool>(delegate(Page<CrudEntity> page)
                {
                    assert.Equal(page.Items.Count, 0, "empty table yields empty Items");
                    assert.IsTrue(page.NextCursor == null, "empty table yields null cursor");
                    client.Close();
                    done();
                    return true;
                });
                return true;
            });
        }

        /// <summary>
        /// <c>Cursor.FromToken(c.ToToken())</c> must be functionally equivalent
        /// to <paramref name="c"/> — same continuation behaviour. Pins the
        /// JSON-envelope round-trip (the literal-keyed shape that survives
        /// NScript minification).
        /// </summary>
        [Test]
        public static void TestQueryPageRoundTripsToken(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                SeedRows(client.Table<string, CrudEntity>(TableName), 10).Then<bool>(delegate(bool seeded)
                {
                    var table = client.Table<string, CrudEntity>(TableName);
                    table.QueryPage(Query.All, 3).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        var token = page1.NextCursor.ToToken();
                        assert.IsTrue(token != null && token.Length > 0, "token serialises to a non-empty string");
                        var revived = Cursor.FromToken(token);
                        table.QueryPage(Query.All, 3, revived).Then<bool>(delegate(Page<CrudEntity> page2)
                        {
                            assert.Equal(page2.Items.Count, 3, "revived cursor produces a full second page");
                            var firstIds = CollectIds(page1.Items);
                            var secondIds = CollectIds(page2.Items);
                            assert.IsTrue(NoOverlap(firstIds, secondIds), "revived cursor preserves no-overlap invariant");
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
        /// Reverse iteration with <c>Descending()</c> must paginate in the
        /// expected order — first page gets the highest keys, second page the
        /// next-highest, etc. Pins the descending branch of the resume-skip
        /// comparator.
        /// </summary>
        [Test]
        public static void TestQueryPageDescendingDirection(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                SeedRows(client.Table<string, CrudEntity>(TableName), 10).Then<bool>(delegate(bool seeded)
                {
                    var table = client.Table<string, CrudEntity>(TableName);
                    var query = new QueryBuilder(new string[0])
                        .Descending()
                        .Build();

                    table.QueryPage(query, 3).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        assert.Equal(page1.Items.Count, 3, "descending page returns pageSize rows");
                        // Items must be in descending Id order: r9, r8, r7
                        assert.Equal(page1.Items[0].Id, "r9", "descending first item is greatest key");
                        assert.Equal(page1.Items[1].Id, "r8", "descending second item is next-greatest");
                        assert.Equal(page1.Items[2].Id, "r7", "descending third item is third-greatest");

                        table.QueryPage(query, 3, page1.NextCursor).Then<bool>(delegate(Page<CrudEntity> page2)
                        {
                            assert.Equal(page2.Items[0].Id, "r6", "descending page 2 starts at next-lower key");
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
        /// Non-unique secondary index pagination must tiebreak on primary key
        /// — IDB-spec ordering. With multiple rows sharing the same Category,
        /// pages must contain no duplicates and no skips. This is the canary
        /// for the index-tie skip path in the resume comparator.
        /// </summary>
        [Test]
        public static void TestQueryPageWithIndexAndTies(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                // Seed: 6 rows all with Category=="todo" so the index is fully tied.
                var seed = new CrudEntity[6];
                for (int i = 0; i < 6; i = i + 1)
                { seed[i] = NewEntity("ix" + i, "todo", i); }

                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    var query = new QueryBuilder(new string[] { "Category" })
                        .Equal<string>("todo")
                        .Build();

                    table.QueryPage(query, 2).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        assert.Equal(page1.Items.Count, 2, "index page 1 returns pageSize rows");
                        var firstIds = CollectIds(page1.Items);

                        table.QueryPage(query, 2, page1.NextCursor).Then<bool>(delegate(Page<CrudEntity> page2)
                        {
                            assert.Equal(page2.Items.Count, 2, "index page 2 returns pageSize rows");
                            var secondIds = CollectIds(page2.Items);
                            assert.IsTrue(NoOverlap(firstIds, secondIds), "tied-index pages have no duplicates");

                            table.QueryPage(query, 2, page2.NextCursor).Then<bool>(delegate(Page<CrudEntity> page3)
                            {
                                assert.Equal(page3.Items.Count, 2, "index page 3 drains the remainder");
                                var thirdIds = CollectIds(page3.Items);
                                assert.IsTrue(NoOverlap(firstIds, thirdIds), "page 1 vs page 3 disjoint");
                                assert.IsTrue(NoOverlap(secondIds, thirdIds), "page 2 vs page 3 disjoint");
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
        /// Inserting a row strictly past the page-1 cursor (lexicographically
        /// greater Id) must make it visible on page 2 — see-current-state
        /// contract. Without this, cross-page mutation handling would be
        /// snapshot-style, which IDB semantics don't support.
        /// </summary>
        [Test]
        public static void TestQueryPageMutationBetweenPagesVisible(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                // Seed three rows: r1, r3, r5 (gaps deliberate so we can insert "r6" past the page-1 cursor).
                var seed = new CrudEntity[] {
                    NewEntity("r1", "todo", 1),
                    NewEntity("r3", "todo", 3),
                    NewEntity("r5", "todo", 5),
                };
                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    table.QueryPage(Query.All, 2).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        // Insert "r6" — strictly past the page-1 cursor (which lands at "r3").
                        table.UpSert(NewEntity("r6", "todo", 6)).Then<bool>(delegate(string ignored)
                        {
                            table.QueryPage(Query.All, 5, page1.NextCursor).Then<bool>(delegate(Page<CrudEntity> page2)
                            {
                                bool sawR6 = false;
                                for (int i = 0; i < page2.Items.Count; i = i + 1)
                                {
                                    if (page2.Items[i].Id == "r6")
                                    { sawR6 = true; }
                                }
                                assert.IsTrue(sawR6, "row inserted past the cursor is visible on next page");
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
        /// Inserting a row strictly before the page-1 cursor (lexicographically
        /// lower Id) must NOT appear on page 2 — see-current-state semantics
        /// document this as silently missed. Without this guard a refactor
        /// could make the cursor reset to a snapshot and start re-reading
        /// records before the cursor position.
        /// </summary>
        [Test]
        public static void TestQueryPageMutationBetweenPagesMissedBefore(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                // Seed: r3, r4, r5 — leave room for "r0" to be inserted strictly before the cursor.
                var seed = new CrudEntity[] {
                    NewEntity("r3", "todo", 3),
                    NewEntity("r4", "todo", 4),
                    NewEntity("r5", "todo", 5),
                };
                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    table.QueryPage(Query.All, 2).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        // Insert "r0" — strictly before the page-1 cursor (which lands at "r4").
                        table.UpSert(NewEntity("r0", "todo", 0)).Then<bool>(delegate(string ignored)
                        {
                            table.QueryPage(Query.All, 5, page1.NextCursor).Then<bool>(delegate(Page<CrudEntity> page2)
                            {
                                bool sawR0 = false;
                                for (int i = 0; i < page2.Items.Count; i = i + 1)
                                {
                                    if (page2.Items[i].Id == "r0")
                                    { sawR0 = true; }
                                }
                                assert.IsTrue(!sawR0, "row inserted before the cursor is silently missed");
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
        /// <c>inclFilter</c> rejects half the rows; <c>pageSize=3</c> must
        /// return 3 <i>accepted</i> rows (post-filter), not 3 reads. This is
        /// the "I want N rows, not N reads" contract documented on
        /// <see cref="WebStoreTable{TKey,TValue}.QueryPage"/>.
        /// </summary>
        [Test]
        public static void TestQueryPageWithFilterSkipsRejectedRows(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                // Seed 10 rows alternating Category — filter accepts only "todo".
                var seed = new CrudEntity[10];
                for (int i = 0; i < 10; i = i + 1)
                { seed[i] = NewEntity("r" + i, (i % 2 == 0) ? "todo" : "done", i); }

                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    table.QueryPage(
                        Query.All,
                        3,
                        null,
                        delegate(CrudEntity row) { return row.Category == "todo"; }
                    ).Then<bool>(delegate(Page<CrudEntity> page)
                    {
                        assert.Equal(page.Items.Count, 3, "filtered page returns 3 accepted rows (not 3 reads)");
                        for (int i = 0; i < page.Items.Count; i = i + 1)
                        { assert.Equal(page.Items[i].Category, "todo", "filter only let 'todo' rows through"); }
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
        /// <c>pageSize &le; 0</c> must reject through the Promise — not throw
        /// synchronously. Pins the symmetric error surface with the rest of
        /// the WebStoreTable APIs (callers using <c>.Then(_, onRejected)</c>
        /// see a uniform path).
        /// </summary>
        [Test]
        public static void TestQueryPageRejectsInvalidPageSize(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                table.QueryPage(Query.All, 0).Then<bool, object>(
                    delegate(Page<CrudEntity> p)
                    {
                        assert.IsTrue(false, "pageSize=0 should reject, not resolve");
                        client.Close();
                        done();
                        return true;
                    },
                    delegate(object err)
                    {
                        assert.IsTrue(err != null, "rejection has an error");
                        var message = ((Exception)err).Message;
                        assert.IsTrue(
                            message != null && message.IndexOf("pageSize") >= 0,
                            "rejection message names pageSize");
                        client.Close();
                        done();
                        return true;
                    });
                return true;
            });
        }

        /// <summary>
        /// A token issued for table A then used on table B must reject — the
        /// table-signature check prevents accidental cross-table resume which
        /// would otherwise produce silently wrong results.
        /// </summary>
        [Test]
        public static void TestQueryPageRejectsCrossTableToken(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            // Build a schema with two stores so we can issue a token from one and resume on the other.
            var schemaTwoStore = new WebStoreSchema(
                dbName,
                1,
                new TableSchema(TableName, new KeyInfo("Id", false), new SingleIndexInfo(CategoryIndex, "Category")),
                new TableSchema("other", new KeyInfo("Id", false)));

            factory.Create(schemaTwoStore).Then<bool>(delegate(WebStoreClient client)
            {
                var primary = client.Table<string, CrudEntity>(TableName);
                var other = client.Table<string, CrudEntity>("other");
                SeedRows(primary, 5).Then<bool>(delegate(bool seeded)
                {
                    primary.QueryPage(Query.All, 2).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        // Reuse the token from "items" on "other" — must reject.
                        other.QueryPage(Query.All, 5, page1.NextCursor).Then<bool, object>(
                            delegate(Page<CrudEntity> p)
                            {
                                assert.IsTrue(false, "cross-table cursor should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("different table") >= 0,
                                    "rejection message names the table-signature mismatch");
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
        /// An ascending-direction token used on a descending-direction query
        /// must reject — the comparator in <c>QueryPageInternal</c> would
        /// silently produce wrong results otherwise (skipping in the wrong
        /// direction).
        /// </summary>
        [Test]
        public static void TestQueryPageRejectsDirectionMismatch(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                SeedRows(table, 5).Then<bool>(delegate(bool seeded)
                {
                    // Issue token on ascending Query.All.
                    table.QueryPage(Query.All, 2).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        var descending = new QueryBuilder(new string[0]).Descending().Build();
                        table.QueryPage(descending, 5, page1.NextCursor).Then<bool, object>(
                            delegate(Page<CrudEntity> p)
                            {
                                assert.IsTrue(false, "direction mismatch should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("direction") >= 0,
                                    "rejection message names the direction mismatch");
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
        /// <c>QueryBuilder.Skip</c> combined with a resume cursor must reject —
        /// stacking Skip on top of a cursor would silently create gaps in the
        /// paginated stream that callers cannot diagnose. Pins the rejection
        /// guard in <c>QueryPageInternal</c> (Skip + cursor mutually exclusive).
        /// </summary>
        [Test]
        public static void TestQueryPageRejectsSkipWithCursor(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                SeedRows(table, 5).Then<bool>(delegate(bool seeded)
                {
                    table.QueryPage(Query.All, 2).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        var skipQuery = new QueryBuilder(new string[0]).Skip(1).Build();
                        table.QueryPage(skipQuery, 5, page1.NextCursor).Then<bool, object>(
                            delegate(Page<CrudEntity> p)
                            {
                                assert.IsTrue(false, "Skip + cursor should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("Skip") >= 0,
                                    "rejection message names Skip");
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
        /// <c>QueryBuilder.Limit</c> combined with a resume cursor must reject —
        /// the cursor's pageSize is the only contract honored on resume; a
        /// stacked Limit would cap reads below pageSize and produce premature
        /// short pages indistinguishable from end-of-stream. Pins the
        /// rejection guard in <c>QueryPageInternal</c>.
        /// </summary>
        [Test]
        public static void TestQueryPageRejectsLimitWithCursor(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                SeedRows(table, 5).Then<bool>(delegate(bool seeded)
                {
                    table.QueryPage(Query.All, 2).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        var limitQuery = new QueryBuilder(new string[0]).Limit(3).Build();
                        table.QueryPage(limitQuery, 5, page1.NextCursor).Then<bool, object>(
                            delegate(Page<CrudEntity> p)
                            {
                                assert.IsTrue(false, "Limit + cursor should reject, not resolve");
                                client.Close();
                                done();
                                return true;
                            },
                            delegate(object err)
                            {
                                var message = ((Exception)err).Message;
                                assert.IsTrue(
                                    message != null && message.IndexOf("Limit") >= 0,
                                    "rejection message names Limit");
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
        /// Filter + resume must produce a contiguous, non-overlapping stream
        /// of accepted rows. Seeds 10 alternating Category rows ("todo"/"done"),
        /// fetches page 1 with <c>inclFilter=todo</c> + <c>pageSize=2</c>, then
        /// resumes with the same filter. The combined pages must contain
        /// exactly the expected "todo" rows, in order, with no overlap and no
        /// gap — pinning the interaction between the filter (applied inside
        /// <c>CursorIterator</c>) and the resume-skip predicate (applied
        /// against <c>cursor.PrimaryKey</c>).
        /// </summary>
        [Test]
        public static void TestQueryPageFilterResumeIsContiguous(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                var seed = new CrudEntity[10];
                for (int i = 0; i < 10; i = i + 1)
                { seed[i] = NewEntity("r" + i, (i % 2 == 0) ? "todo" : "done", i); }

                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    table.QueryPage(
                        Query.All,
                        2,
                        null,
                        delegate(CrudEntity row) { return row.Category == "todo"; }
                    ).Then<bool>(delegate(Page<CrudEntity> page1)
                    {
                        assert.Equal(page1.Items.Count, 2, "page 1 returns 2 filtered rows");
                        assert.Equal(page1.Items[0].Id, "r0", "page 1 starts at r0");
                        assert.Equal(page1.Items[1].Id, "r2", "page 1 second row is r2");
                        assert.IsTrue(page1.NextCursor != null, "non-final filtered page has a cursor");

                        table.QueryPage(
                            Query.All,
                            2,
                            page1.NextCursor,
                            delegate(CrudEntity row) { return row.Category == "todo"; }
                        ).Then<bool>(delegate(Page<CrudEntity> page2)
                        {
                            assert.Equal(page2.Items.Count, 2, "page 2 returns 2 filtered rows");
                            assert.Equal(page2.Items[0].Id, "r4", "page 2 resumes at r4 (no overlap, no gap)");
                            assert.Equal(page2.Items[1].Id, "r6", "page 2 second row is r6");
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
        /// Key-only paged variant must return <c>Page&lt;TKey&gt;</c> matching
        /// the ordering of the value-paged scan. Pins the
        /// <c>QueryKeysPage</c> branch — the same code path with
        /// <c>isKeyQuery=true</c>.
        /// </summary>
        [Test]
        public static void TestQueryKeysPageReturnsKeysOnly(Assert assert)
        {
            var done = assert.Async(1);
            var factory = new WebStoreFactory();
            var dbName = NewDbName();

            factory.Create(BuildSchema(dbName)).Then<bool>(delegate(WebStoreClient client)
            {
                var table = client.Table<string, CrudEntity>(TableName);
                SeedRows(table, 6).Then<bool>(delegate(bool seeded)
                {
                    table.QueryKeysPage(Query.All, 3).Then<bool>(delegate(Page<string> keysPage1)
                    {
                        assert.Equal(keysPage1.Items.Count, 3, "key page returns pageSize keys");
                        assert.Equal(keysPage1.Items[0], "r0", "first key is smallest");
                        assert.IsTrue(keysPage1.NextCursor != null, "non-final key page has a cursor");

                        table.QueryKeysPage(Query.All, 3, keysPage1.NextCursor).Then<bool>(delegate(Page<string> keysPage2)
                        {
                            assert.Equal(keysPage2.Items.Count, 3, "second key page returns pageSize keys");
                            assert.Equal(keysPage2.Items[0], "r3", "second page starts at the next key");
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

        private static CrudEntity NewEntity(string id, string category, int score)
        {
            var e = new CrudEntity();
            e.Id = id;
            e.Category = category;
            e.Score = score;
            return e;
        }

        /// <summary>
        /// Seed <paramref name="count"/> rows with Ids "r0"..."r{count-1}".
        /// Lexicographic ordering matches numeric ordering only when count &lt; 10
        /// — pagination tests stay below that threshold to keep ordering
        /// deterministic.
        /// </summary>
        private static Promise<bool> SeedRows(WebStoreTable<string, CrudEntity> table, int count)
        {
            var seed = new CrudEntity[count];
            for (int i = 0; i < count; i = i + 1)
            { seed[i] = NewEntity("r" + i, "todo", i); }

            return new Promise<bool>(delegate(Action<bool> resolve, Action<object> reject)
            {
                table.UpSert(seed).Then<bool>(delegate(string[] keys)
                {
                    resolve(true);
                    return true;
                });
            });
        }

        private static List<string> CollectIds(IList<CrudEntity> items)
        {
            var ids = new List<string>();
            for (int i = 0; i < items.Count; i = i + 1)
            { ids.Add(items[i].Id); }
            return ids;
        }

        private static bool NoOverlap(List<string> a, List<string> b)
        {
            for (int i = 0; i < a.Count; i = i + 1)
            {
                for (int j = 0; j < b.Count; j = j + 1)
                {
                    if (a[i] == b[j])
                    { return false; }
                }
            }
            return true;
        }
    }
}
