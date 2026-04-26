namespace Sunlight.Framework.Data.WebStore
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Web.Html;
    using System.Web.Html.Data.IndexedDB;
    using QueryDef = Sunlight.Framework.Data.WebStore.Query;

    internal enum CursorOrCountRequestMode
    {
        KeyCursor,
        Cursor,
        Count,
    }

    /// <summary>
    /// Bridge helpers written as raw JS. Used by <see cref="WebStoreTable{TKey, TValue}"/>
    /// to keep records compatible with IndexedDB's structured-clone algorithm.
    /// </summary>
    internal static class StructuredCloneHelper
    {
        /// <summary>
        /// [JsonType] entities carry a lazy <c>importedExtension</c> wrapper on
        /// their JS shape that holds a <c>toJSON</c> function. IndexedDB's
        /// structured-clone algorithm cannot clone <c>Function</c> values, so the
        /// wrapper must be removed before handing the record to <c>put</c> or
        /// <c>add</c>. Only strips the top-level wrapper on <paramref name="value"/>
        /// itself — callers are responsible for recursing into array elements or
        /// nested JSON types if they also carry the wrapper. Mutates
        /// <paramref name="value"/> in place.
        /// </summary>
        [Script("delete value.importedExtension;")]
        public static extern void StripImportedExtension(object value);
    }

    /// <summary>
    /// Typed handle to a single object store. Obtained via
    /// <see cref="WebStoreClient.Table{TKey,TValue}(string)"/>. Every method
    /// returns a <see cref="Promise"/> that resolves on transaction completion.
    /// </summary>
    /// <typeparam name="TKey">Type of the primary key.</typeparam>
    /// <typeparam name="TValue">Type of the stored records.</typeparam>
    public class WebStoreTable<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        private readonly NamedLogger _logger;
        private readonly WebStoreClient _storeClient;
        private readonly TableSchema _tableSchema;

        public WebStoreTable(
            TableSchema tableSchema,
            WebStoreClient storeClient)
        {
            _tableSchema = tableSchema;
            _logger = Logger.ForCategory("WebStoreTable");
            _storeClient = storeClient;
        }

        /// <summary>Inserts a new record. Fails if a record with the same key exists.</summary>
        public Promise<TKey> Add(
            TValue value,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TKey>(
                (Action<TKey> resolve, Action<object> reject) =>
                    this.AddInternal(
                        transaction,
                        value,
                        resolve,
                        reject));
        }

        /// <summary>Inserts a batch of new records atomically under a single transaction.</summary>
        public Promise<TKey[]> AddBatch(
            TValue[] values,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TKey[]>((resolve, reject) =>
                this.AddBatchInternal(
                    transaction,
                    values,
                    resolve,
                    reject));
        }

        /// <summary>Counts records matching <paramref name="query"/>.</summary>
        public Promise<int> Count(
            Query query,
            WebStoreTransaction transaction = null)
        {
            return new Promise<int>(
                (Action<int> resolve, Action<object> reject) =>
                    this.CountInternal(
                        transaction,
                        query,
                        resolve,
                        reject));
        }

        /// <summary>Deletes multiple records by key. Resolves with the count deleted.</summary>
        public Promise<int> DeleteBatch(
            TKey[] keys,
            WebStoreTransaction transaction = null)
        {
            return new Promise<int>((resolve, reject) =>
                RemoveBatchInternal(
                    transaction,
                    keys,
                    resolve,
                    reject));
        }

        /// <summary>Deletes every record matching <paramref name="query"/> (and optional <paramref name="inclFilter"/>).</summary>
        public Promise<int> Delete(
            Query query,
            Func<TValue, bool> inclFilter = null,
            WebStoreTransaction transaction = null)
        {
            return new Promise<int>((resolve, reject) =>
                this.QueryDeleteInternal(
                    transaction,
                    query,
                    inclFilter,
                    resolve,
                    reject));
        }

        /// <summary>Deletes a single record by key.</summary>
        public Promise<bool> Delete(
            TKey key,
            WebStoreTransaction transaction = null)
        {
            return new Promise<bool>(
                (Action<bool> resolve, Action<object> reject) =>
                    this.RemoveInternal(
                        transaction,
                        key,
                        resolve,
                        reject));
        }

        /// <summary>Loads a single record by key. Rejects if not found.</summary>
        public Promise<TValue> Get(
            TKey key,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TValue>(
                (Action<TValue> resolve, Action<object> reject) =>
                    this.GetInternal(
                        transaction,
                        key,
                        resolve,
                        reject,
                        false));
        }

        /// <summary>Loads multiple records by key. Missing records appear as null in the result.</summary>
        public Promise<TValue[]> GetBatch(
            TKey[] keys,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TValue[]>(
                (Action<TValue[]> resolve, Action<object> reject) =>
                    this.GetBatchInternal(
                        transaction,
                        keys,
                        resolve,
                        reject,
                        false));
        }

        /// <summary>
        /// Runs <paramref name="query"/> and returns all matching records
        /// (filtered by <paramref name="inclFilter"/> when supplied).
        /// <para>
        /// The unbounded <see cref="WebStore.Query.All"/> singleton is rejected on
        /// this materialising path — it would buffer the entire table into a
        /// <see cref="List{T}"/> with no row ceiling. Use
        /// <see cref="ForEach(Query, Func{TValue, bool}, WebStoreTransaction)"/>
        /// to stream the full table, or build a query with an explicit
        /// <c>QueryBuilder.Limit(n)</c> to bound the result set.
        /// </para>
        /// </summary>
        public Promise<List<TValue>> Query(
            Query query,
            Func<TValue, bool> inclFilter = null,
            WebStoreTransaction transaction = null)
        {
            return new Promise<List<TValue>>((resolve, reject) =>
                this.QueryInternal<TValue>(
                    transaction,
                    query,
                    inclFilter,
                    resolve,
                    reject,
                    false));
        }

        /// <summary>
        /// Streams every record matching <paramref name="query"/> through
        /// <paramref name="visit"/> without materialising the result set. The
        /// visitor returns <c>true</c> to continue, <c>false</c> to stop early.
        /// Resolves with the number of records visited (including the one the
        /// visitor returned <c>false</c> for, if any).
        /// </summary>
        public Promise<int> ForEach(
            Query query,
            Func<TValue, bool> visit,
            WebStoreTransaction transaction = null)
        {
            return new Promise<int>((resolve, reject) =>
            {
                if (visit == null)
                {
                    reject(new Exception("visit can't be null"));
                    return;
                }
                this.ForEachInternal(
                    transaction,
                    query,
                    visit,
                    resolve,
                    reject);
            });
        }

        /// <summary>Same as <see cref="Query"/> but returns primary keys only (key cursor).</summary>
        public Promise<List<TKey>> QueryKeys(
            Query query,
            WebStoreTransaction transaction = null)
        {
            return new Promise<List<TKey>>((resolve, reject) =>
                this.QueryInternal<TKey>(
                    transaction,
                    query,
                    null,
                    resolve,
                    reject,
                    true));
        }

        /// <summary>
        /// Materialise a single bounded page of records matching
        /// <paramref name="query"/>. Pagination semantics:
        /// <list type="bullet">
        /// <item><description><paramref name="pageSize"/> must be &ge; 1 and counts
        /// records that pass <paramref name="inclFilter"/> — "I want N rows", not
        /// "N reads". The returned <see cref="Page{TValue}.Items"/> is capped at
        /// <paramref name="pageSize"/>.</description></item>
        /// <item><description><see cref="Page{TValue}.NextCursor"/> is <c>null</c>
        /// when the iterator drained the scan range before the page filled — that
        /// page is the final page. A non-<c>null</c> cursor signals at least one
        /// additional record may exist.</description></item>
        /// <item><description>Pass <paramref name="resumeFrom"/> as the previous
        /// page's <see cref="Page{TValue}.NextCursor"/> to fetch the next page.
        /// Cross-transaction <i>see-current-state</i> semantics: records inserted
        /// strictly past the cursor become visible on the next page; records
        /// inserted strictly before are silently missed (cursor moved past);
        /// deleted records are skipped naturally as the cursor walks current
        /// state.</description></item>
        /// </list>
        /// Rejects when <paramref name="pageSize"/> &le; 0, when
        /// <paramref name="resumeFrom"/> is combined with <c>Query.Skip</c> or
        /// <c>Query.Limit</c>, when the cursor was issued for a different table,
        /// or when its direction doesn't match <paramref name="query"/>'s.
        /// </summary>
        public Promise<Page<TValue>> QueryPage(
            Query query,
            int pageSize,
            Cursor resumeFrom = null,
            Func<TValue, bool> inclFilter = null,
            WebStoreTransaction transaction = null)
        {
            return new Promise<Page<TValue>>((resolve, reject) =>
                this.QueryPageInternal<TValue>(
                    transaction,
                    query,
                    pageSize,
                    resumeFrom,
                    inclFilter,
                    resolve,
                    reject,
                    false));
        }

        /// <summary>
        /// Key-only paged variant of <see cref="QueryPage"/> — returns primary
        /// keys instead of values. Same pagination semantics; same validation
        /// surface. No <c>inclFilter</c> overload because key cursors don't
        /// materialise values.
        /// </summary>
        public Promise<Page<TKey>> QueryKeysPage(
            Query query,
            int pageSize,
            Cursor resumeFrom = null,
            WebStoreTransaction transaction = null)
        {
            return new Promise<Page<TKey>>((resolve, reject) =>
                this.QueryPageInternal<TKey>(
                    transaction,
                    query,
                    pageSize,
                    resumeFrom,
                    null,
                    resolve,
                    reject,
                    true));
        }

        /// <summary>Like <see cref="Get"/> but resolves with null when the record is missing instead of rejecting.</summary>
        public Promise<TValue> TryGet(
            TKey key,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TValue>(
                (Action<TValue> resolve, Action<object> reject) =>
                    this.GetInternal(
                        transaction,
                        key,
                        resolve,
                        reject,
                        true));
        }

        /// <summary>Updates an existing record. Rejects if the record does not already exist.</summary>
        public Promise<TKey> Update(
            TValue value,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TKey>(
                (Action<TKey> resolve, Action<object> reject) =>
                    this.UpdateInternal(
                        transaction,
                        value,
                        resolve,
                        reject));
        }

        /// <summary>Runs <paramref name="updateFunc"/> against every record matching <paramref name="query"/> and persists the changes.</summary>
        public Promise<int> Update(
            Query query,
            Func<TValue, bool> updateFunc,
            WebStoreTransaction transaction = null)
        {
            if (updateFunc == null)
            { throw new Exception("updateFunc can't be null"); }

            return new Promise<int>((resolve, reject) =>
                this.QueryUpdateOrDeleteInternal(
                    transaction,
                    query,
                    null,
                    updateFunc,
                    resolve,
                    reject));
        }

        /// <summary>Inserts a record or replaces the existing one if present.</summary>
        public Promise<TKey> UpSert(
            TValue value,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TKey>(
                (Action<TKey> resolve, Action<object> reject) =>
                    this.UpSertInternal(
                        transaction,
                        value,
                        resolve,
                        reject));
        }

        /// <summary>Batch variant of <see cref="UpSert(TValue, WebStoreTransaction)"/>.</summary>
        public Promise<TKey[]> UpSert(
            TValue[] values,
            WebStoreTransaction transaction = null)
        {
            return new Promise<TKey[]>(
                (Action<TKey[]> resolve, Action<object> reject) =>
                    this.UpSertInternal(
                        transaction,
                        values,
                        resolve,
                        reject));
        }

        private static Action<IDBRequest, Event> HandleError(
            Action<object> reject)
        {
            return (req, evt) =>
            {
                evt.PreventDefault();
                reject(new EventBasedException(
                    req.Error.ExceptionMessage(),
                    evt));
            };
        }

        private void AddBatchInternal(
                    WebStoreTransaction transaction,
            TValue[] values,
            Action<TKey[]> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(transaction, TransactionKind.ReadWrite);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            TKey[] rv = new TKey[values.Length];

            int iItem = values.Length;
            bool requestAborted = false;
            Action<IDBRequest, Event> onError = (req, evt) =>
            {
                // Prevent window.onerror from firing.
                evt.PreventDefault();

                if (requestAborted)
                {
                    return;
                }

                requestAborted = true;

                transaction.Abort();
                reject(new EventBasedException(req.Error.ExceptionMessage(), evt));
            };

            Action<int, IDBRequest, Event> onSuccess = null;
            onSuccess = (idx, req, evt) =>
             {
                 if (requestAborted)
                 {
                     return;
                 }

                 rv[idx] = Type.AS<object, TKey>(req.Result);

                 if (--iItem == 0)
                 { resolve(rv); }
             };

            Action<int, IDBRequest> registerCallbacks = (idx, req) =>
            {
                req.OnError += (req2, evt) => onError(req2, evt);
                req.OnSuccess += (req2, evt) => onSuccess(idx, req2, evt);
            };

            for (int idx = values.Length - 1; idx >= 0; idx--)
            {
                StructuredCloneHelper.StripImportedExtension(values[idx]);
                registerCallbacks(idx, table.Add(values[idx]));
            }
        }

        private void AddInternal(
            WebStoreTransaction transaction,
            TValue value,
            Action<TKey> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(
                transaction,
                TransactionKind.ReadWrite);

            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            StructuredCloneHelper.StripImportedExtension(value);
            var request = table.Add(value);

            request.OnSuccess += (req, evt) =>
                resolve(Type.AS<object, TKey>(req.Result));

            request.OnError += HandleError(reject);
        }

        private void BatchUpdateOrDeleteWithKeys(
            WebStoreTransaction transaction,
            TKey[] keys,
            TValue[] values,
            IDBObjectStore<TKey, TValue> objectStore,
            Action<int> resolve,
            Action<object> reject)
        {
            if (keys.Length == 0)
            {
                resolve(0);
                return;
            }

            int pendingRequests = 0;
            bool aborted = false;

            Action<int> doWork = (idx) =>
            {
                pendingRequests++;
                IDBRequest request;
                if (values == null)
                {
                    request = objectStore.Delete(keys[idx]);
                }
                else
                {
                    StructuredCloneHelper.StripImportedExtension(values[idx]);
                    request = objectStore.Put(values[idx]);
                }

                request.OnSuccess += (req, evt) =>
                {
                    if (aborted)
                    {
                        return;
                    }

                    if (--pendingRequests == 0)
                    { resolve(keys.Length); }
                };

                request.OnError += (req, evt) =>
                {
                    evt.PreventDefault();
                    if (aborted)
                    {
                        return;
                    }

                    aborted = true;
                    transaction.Abort();
                    reject(
                        new EventBasedException(
                            req.Error.ExceptionMessage(),
                            evt));
                };
            };

            for (int iKey = 0; iKey < keys.Length; iKey++)
            { doWork(iKey); }
        }

        private void CountInternal(
            WebStoreTransaction transaction,
            Query query,
            Action<int> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(
                transaction,
                TransactionKind.Read);

            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            var request = this.GetCursor(
                table,
                query,
                reject,
                CursorOrCountRequestMode.Count);

            if (request == null)
            { return; }

            request.OnSuccess += (req, evt) => resolve(Type.AS<object, int>(req.Result));
            request.OnError += HandleError(reject);
        }

        private void CursorIterator(
            WebStoreTransaction transaction,
            Query query,
            Func<TValue, bool> filter,
            Func<IDBCursor<TKey, TValue>, bool> onIterate,
            Action<object> reject,
            bool isKeyQuery = false)
        {
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            var request = this.GetCursor(
                table,
                query,
                reject,
                isKeyQuery
                    ? CursorOrCountRequestMode.KeyCursor
                    : CursorOrCountRequestMode.Cursor);

            if (request == null)
            { return; }

            var skip = query.Skip ?? 0;
            int? remaining = query.Limit;
            bool firstLoop = skip > 0;
            bool aborted = false;

            request.OnSuccess += (req, evt) =>
            {
                if (aborted)
                { return; }

                var cursor = Type.AS<object, IDBCursor<TKey, TValue>>(req.Result);
                if (cursor == null)
                {
                    try { _ = onIterate(null); }
                    catch (Exception ex) { aborted = true; reject(ex); }
                    return;
                }

                if (firstLoop)
                {
                    try { cursor.Advance(skip); }
                    catch (Exception ex) { aborted = true; reject(ex); return; }
                    firstLoop = false;
                    return;
                }

                bool accept;
                try { accept = filter == null || filter(cursor.Value); }
                catch (Exception ex) { aborted = true; reject(ex); return; }

                if (accept)
                {
                    if (remaining.HasValue && remaining.Value <= 0)
                    {
                        try { _ = onIterate(null); }
                        catch (Exception ex) { aborted = true; reject(ex); }
                        return;
                    }

                    bool cont;
                    try { cont = onIterate(cursor); }
                    catch (Exception ex) { aborted = true; reject(ex); return; }
                    if (!cont)
                    { return; }

                    if (remaining.HasValue)
                    {
                        remaining = remaining.Value - 1;
                        if (remaining.Value == 0)
                        {
                            try { _ = onIterate(null); }
                            catch (Exception ex) { aborted = true; reject(ex); }
                            return;
                        }
                    }
                }

                try { cursor.Continue(); }
                catch (Exception ex) { aborted = true; reject(ex); }
            };

            request.OnError += (req, evt) =>
            {
                evt.PreventDefault();
                if (aborted)
                { return; }
                aborted = true;
                reject(new EventBasedException(
                    req.Error.ExceptionMessage(),
                    evt));
            };
        }

        private IDBRequest GetCursor(
            IDBObjectStore<TKey, TValue> objectStore,
            Query query,
            Action<object> reject,
            CursorOrCountRequestMode mode)
        {
            if (_tableSchema.CanUsePrimaryIndex(query))
            {
                return GetCursorOrCountRequest(
                    objectStore,
                    query.SingleColumnRange ?? query.Range,
                    mode,
                    query.Direction);
            }

            var tableIndex = _tableSchema.GetIndex(query);
            if (tableIndex == null)
            {
                reject(new Exception("No suitable index found"));
                return null;
            }

            var index = objectStore.Index(tableIndex.Name);

            if (index.KeyPath is String)
            {
                return GetCursorOrCountRequest(
                    index,
                    query.SingleColumnRange ?? query.Range,
                    mode,
                    query.Direction);
            }
            else
            {
                return GetCursorOrCountRequest(
                    index,
                    query.Range,
                    mode,
                    query.Direction);
            }
        }

        private IDBRequest GetCursorOrCountRequest(
            IDBIndex<TKey, TValue> index,
            IDBKeyRange range,
            CursorOrCountRequestMode mode,
            string direction)
        {
            switch (mode)
            {
                case CursorOrCountRequestMode.KeyCursor:
                    return index.OpenKeyCursor(
                        range,
                        direction);

                case CursorOrCountRequestMode.Cursor:
                    return index.OpenCursor(
                        range,
                        direction);

                case CursorOrCountRequestMode.Count:
                    return index.Count(range);

                default:
                    return null;
            }
        }

        private IDBRequest GetCursorOrCountRequest(
            IDBObjectStore<TKey, TValue> objectStore,
            IDBKeyRange range,
            CursorOrCountRequestMode mode,
            string direction)
        {
            switch (mode)
            {
                case CursorOrCountRequestMode.KeyCursor:
                    return objectStore.HasOpenKeyCursor
                        ? objectStore.OpenKeyCursor(
                            range,
                            direction)
                        : objectStore.OpenCursor(
                            range,
                            direction);

                case CursorOrCountRequestMode.Cursor:
                    return objectStore.OpenCursor(
                        range,
                        direction);

                case CursorOrCountRequestMode.Count:
                    return objectStore.Count(range);

                default:
                    return null;
            }
        }

        private void GetBatchInternal(
            WebStoreTransaction transaction,
            TKey[] keys,
            Action<TValue[]> resolve,
            Action<object> reject,
            bool isTry)
        {
            if (keys.Length == 0)
            {
                resolve(new TValue[0]);
                return;
            }

            transaction = this.TransactionOrDefault(transaction, TransactionKind.Read);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);

            var idx = 0;
            var rv = new TValue[keys.Length];
            Action<IDBRequest, Event> onError = HandleError(reject);
            Action<IDBRequest, Event> onSuccess = null;
            onSuccess = (req, evt) =>
            {
                var value = Type.AS<object, TValue>(req.Result);
                if (Object.IsNullOrUndefined(value))
                { value = null; }

                rv[idx++] = Type.AS<object, TValue>(value);

                if (idx == rv.Length)
                { resolve(rv); }
                else
                {
                    req = table.Get(keys[idx]);
                    req.OnSuccess += onSuccess;
                    req.OnError += onError;
                }
            };

            var request = table.Get(keys[idx]);
            request.OnSuccess += onSuccess;
            request.OnError += onError;
        }

        private void GetInternal(
            WebStoreTransaction transaction,
            TKey key,
            Action<TValue> resolve,
            Action<object> reject,
            bool isTry)
        {
            transaction = this.TransactionOrDefault(transaction, TransactionKind.Read);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            var request = table.Get(key);

            request.OnSuccess += (req, evt) =>
            {
                var value = Type.AS<object, TValue>(req.Result);
                if (Object.IsNullOrUndefined(value))
                {
                    if (isTry)
                    { resolve(null); }
                    else
                    { reject(new Exception("Object not found")); }
                }
                else
                { resolve(Type.AS<object, TValue>(value)); }
            };

            request.OnError += HandleError(reject);
        }

        private void QueryDeleteInternal(
            WebStoreTransaction transaction,
            Query query,
            Func<TValue, bool> inclFilter,
            Action<int> resolve,
            Action<object> reject)
        {
            this.QueryUpdateOrDeleteInternal(
                transaction,
                query,
                inclFilter,
                null,
                resolve,
                reject);
        }

        private void ForEachInternal(
            WebStoreTransaction transaction,
            Query query,
            Func<TValue, bool> visit,
            Action<int> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(
                transaction,
                TransactionKind.Read);

            int count = 0;
            this.CursorIterator(
                transaction,
                query,
                null,
                (cursor) =>
                {
                    if (cursor == null)
                    {
                        resolve(count);
                        return true;
                    }

                    count = count + 1;
                    if (!visit(cursor.Value))
                    {
                        resolve(count);
                        return false;
                    }

                    return true;
                },
                reject,
                false);
        }

        private void QueryInternal<U>(
            WebStoreTransaction transaction,
            Query query,
            Func<TValue, bool> filter,
            Action<List<U>> resolve,
            Action<object> reject,
            bool isKeyQuery)
        {
            if (Object.ReferenceEquals(query, QueryDef.All) && filter == null && !isKeyQuery)
            {
                reject(new Exception(
                    "Query(Query.All) is disallowed because it can materialise an "
                    + "unbounded result set into memory. Use ForEach for streaming, "
                    + "or set an explicit cap via QueryBuilder.Limit(n)."));
                return;
            }

            transaction = this.TransactionOrDefault(
                transaction,
                TransactionKind.Read);

            List<U> rv = new List<U>();
            this.CursorIterator(
                transaction,
                query,
                filter,
                (cursor) =>
                {
                    if (cursor == null)
                    { resolve(rv); }
                    else
                    {
                        if (isKeyQuery)
                        { rv.Add(Type.AS<TKey, U>(cursor.PrimaryKey)); }
                        else
                        { rv.Add(Type.AS<TValue, U>(cursor.Value)); }
                    }

                    return true;
                },
                reject,
                isKeyQuery);
        }

        private void QueryPageInternal<U>(
            WebStoreTransaction transaction,
            Query query,
            int pageSize,
            Cursor resumeFrom,
            Func<TValue, bool> inclFilter,
            Action<Page<U>> resolve,
            Action<object> reject,
            bool isKeyQuery)
        {
            if (pageSize <= 0)
            {
                reject(new Exception("pageSize must be >= 1"));
                return;
            }

            if (resumeFrom != null && query.Skip != null)
            {
                reject(new Exception(
                    "Skip and resume cursor are mutually exclusive"));
                return;
            }

            if (resumeFrom != null && query.Limit != null)
            {
                reject(new Exception(
                    "cursor pagination supersedes Query.Limit; use pageSize"));
                return;
            }

            if (resumeFrom != null && resumeFrom.TableSig != _tableSchema.Name)
            {
                reject(new Exception(
                    "cursor was issued for a different table (expected '"
                    + _tableSchema.Name + "', got '" + resumeFrom.TableSig + "')"));
                return;
            }

            if (resumeFrom != null && resumeFrom.Direction != query.Direction)
            {
                reject(new Exception(
                    "cursor direction mismatch (expected '"
                    + query.Direction + "', got '" + resumeFrom.Direction + "')"));
                return;
            }

            transaction = this.TransactionOrDefault(
                transaction,
                TransactionKind.Read);

            bool isIndex = !_tableSchema.CanUsePrimaryIndex(query);
            bool resumed = resumeFrom == null;
            bool descending = query.Direction == "prev";
            List<U> items = new List<U>();
            object lastIndexKey = null;
            object lastPrimaryKey = null;
            bool pageFull = false;

            this.CursorIterator(
                transaction,
                query,
                inclFilter,
                (cursor) =>
                {
                    if (cursor == null)
                    {
                        if (!pageFull)
                        { resolve(new Page<U>(items, null)); }
                        return true;
                    }

                    object curIndexKey = isIndex
                        ? Type.AS<IDBKeyRange, object>(cursor.KeyRange)
                        : null;
                    object curPrimaryKey = Type.AS<TKey, object>(cursor.PrimaryKey);

                    if (!resumed)
                    {
                        int cmp = ComparePagePair(
                            isIndex,
                            curIndexKey,
                            curPrimaryKey,
                            resumeFrom.IndexKey,
                            resumeFrom.PrimaryKey);
                        bool past = descending ? cmp < 0 : cmp > 0;
                        if (!past)
                        { return true; }
                        resumed = true;
                    }

                    if (isKeyQuery)
                    { items.Add(Type.AS<TKey, U>(cursor.PrimaryKey)); }
                    else
                    { items.Add(Type.AS<TValue, U>(cursor.Value)); }

                    lastIndexKey = curIndexKey;
                    lastPrimaryKey = curPrimaryKey;

                    if (items.Count >= pageSize)
                    {
                        pageFull = true;
                        resolve(new Page<U>(
                            items,
                            new Cursor(
                                lastIndexKey,
                                lastPrimaryKey,
                                query.Direction,
                                _tableSchema.Name)));
                        return false;
                    }

                    return true;
                },
                reject,
                isKeyQuery);
        }

        private static int ComparePagePair(
            bool isIndex,
            object aIdx,
            object aPk,
            object bIdx,
            object bPk)
        {
            if (isIndex && aIdx != null && bIdx != null)
            {
                int c = (int)IDBFactory.Instance.Cmp(aIdx, bIdx);
                if (c != 0)
                { return c; }
            }
            return (int)IDBFactory.Instance.Cmp(aPk, bPk);
        }

        private void QueryUpdateOrDeleteInternal(
            WebStoreTransaction transaction,
            Query query,
            Func<TValue, bool> inclFilter,
            Func<TValue, bool> updateFunc,
            Action<int> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(
                transaction,
                TransactionKind.ReadWrite);

            List<TKey> keys = new List<TKey>();
            List<TValue> valuesToUpdate = updateFunc != null ? new List<TValue>() : null;
            this.CursorIterator(
                transaction,
                query,
                inclFilter,
                (cursor) =>
                {
                    if (cursor == null)
                    {
                        this.BatchUpdateOrDeleteWithKeys(
                            transaction,
                            keys.ToArray(),
                            valuesToUpdate != null ? valuesToUpdate.ToArray() : null,
                            transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name),
                            resolve,
                            reject);

                        return false;
                    }

                    if (updateFunc != null && !updateFunc(cursor.Value))
                    { return true; }
                    else if (updateFunc != null)
                    { valuesToUpdate.Add(cursor.Value); }

                    keys.Add(cursor.PrimaryKey);
                    return true;
                },
                reject,
                false);
        }

        private void RemoveBatchInternal(
            WebStoreTransaction transaction,
            TKey[] keys,
            Action<int> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(transaction, TransactionKind.ReadWrite);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            this.BatchUpdateOrDeleteWithKeys(
                transaction,
                keys,
                null,
                table,
                resolve,
                reject);
        }

        private void RemoveInternal(
            WebStoreTransaction transaction,
            TKey key,
            Action<bool> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(transaction, TransactionKind.ReadWrite);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            var request = table.Delete(key);

            request.OnSuccess += (req, evt) => resolve(true);
            request.OnError += HandleError(reject);
        }

        private WebStoreTransaction TransactionOrDefault(
            WebStoreTransaction transaction,
            TransactionKind kind)
        {
            return transaction
                ?? _storeClient.StartTransaction(
                    kind,
                    new string[] { _tableSchema.Name });
        }

        private void UpdateInternal(
            WebStoreTransaction transaction,
            TValue value,
            Action<TKey> resolve,
            Action<object> reject)
        {
            var key = KeyInfo.GetKeyValue<TKey, TValue>(_tableSchema.Key, value);

            if (Object.IsNullOrUndefined(key))
            {
                reject(new Exception("Object not found"));
                return;
            }

            transaction = this.TransactionOrDefault(transaction, TransactionKind.ReadWrite);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            var request = table.Get(key);

            request.OnSuccess += (req, evt) =>
            {
                if (Object.IsNullOrUndefined(req.Result))
                {
                    reject(new Exception("Object not found"));
                    return;
                }

                StructuredCloneHelper.StripImportedExtension(value);
                request = table.Put(value);
                request.OnSuccess += (req2, evt2) => resolve(Type.AS<object, TKey>(request.Result));
                request.OnError += HandleError(reject);
            };

            request.OnError += HandleError(reject);
        }

        private void UpSertInternal(
            WebStoreTransaction transaction,
            TValue value,
            Action<TKey> resolve,
            Action<object> reject)
        {
            transaction = this.TransactionOrDefault(transaction, TransactionKind.ReadWrite);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);
            StructuredCloneHelper.StripImportedExtension(value);
            var request = table.Put(value);

            request.OnSuccess += (req, evt) => resolve(Type.AS<object, TKey>(req.Result));
            request.OnError += HandleError(reject);
        }

        private void UpSertInternal(
            WebStoreTransaction transaction,
            TValue[] values,
            Action<TKey[]> resolve,
            Action<object> reject)
        {
            if (values.Length == 0)
            {
                resolve(new TKey[0]);
                return;
            }

            var transactionCreated = transaction == null;
            transaction = this.TransactionOrDefault(transaction, TransactionKind.ReadWrite);
            var table = transaction.Transaction.ObjectStore<TKey, TValue>(_tableSchema.Name);

            TKey[] rv = new TKey[values.Length];

            int idx = 0;
            var onErrorFwd = HandleError(reject);
            Action<IDBRequest, Event> onError = (req, evt) =>
            {
                if (transactionCreated)
                { transaction.Abort(); }

                onErrorFwd(req, evt);
            };

            Action<IDBRequest, Event> onSuccess = null;
            onSuccess = (req, evt) =>
            {
                rv[idx++] = Type.AS<object, TKey>(req.Result);
                if (idx == values.Length)
                {
                    resolve(rv);
                    return;
                }
                else
                {
                    StructuredCloneHelper.StripImportedExtension(values[idx]);
                    req = table.Put(values[idx]);
                    req.OnSuccess += onSuccess;
                    req.OnError += onError;
                }
            };

            StructuredCloneHelper.StripImportedExtension(values[idx]);
            var request = table.Put(values[idx]);
            request.OnSuccess += onSuccess;
            request.OnError += onError;
        }
    }
}
