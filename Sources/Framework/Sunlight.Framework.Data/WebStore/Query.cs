namespace Sunlight.Framework.Data.WebStore
{
    using System.Web.Html.Data.IndexedDB;

    /// <summary>
    /// Immutable description of a key-range query. Build via <see cref="QueryBuilder"/>,
    /// then pass to <see cref="WebStoreTable{TKey,TValue}"/> query / update / delete APIs.
    /// </summary>
    public class Query
    {
        private readonly bool _isDescending;

        public Query(
            string[] keyNames,
            IDBKeyRange range,
            IDBKeyRange singleColumnRange,
            bool isDescending,
            int? skip,
            int? limit)
        {
            SingleColumnRange = singleColumnRange;
            KeyPaths = keyNames;
            Range = range;
            Limit = limit;
            Skip = skip;
            _isDescending = isDescending;
        }

        /// <summary>Key paths that index selection and sort order are based on.</summary>
        public string[] KeyPaths { get; }

        /// <summary>Full multi-column key range, used for composite-index queries.</summary>
        public IDBKeyRange Range { get; }

        /// <summary>Single-column key range shortcut; used when a single-column index suffices.</summary>
        public IDBKeyRange SingleColumnRange { get; }

        public int? Limit { get; }

        public int? Skip { get; }

        /// <summary>IDB cursor direction string ("next" or "prev").</summary>
        public string Direction => _isDescending ? "prev" : "next";

        /// <summary>Unbounded query — scans all records in primary-key order.</summary>
        public static Query All
        { get; } = new Query(new string[0], null, null, false, null, null);
    }
}
