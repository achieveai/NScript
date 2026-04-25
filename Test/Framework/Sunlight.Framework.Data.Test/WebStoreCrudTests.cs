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
