namespace Sunlight.Framework.Data.WebStore
{
    using System.Collections.Generic;

    /// <summary>
    /// One page of records returned by
    /// <see cref="WebStoreTable{TKey,TValue}.QueryPage"/> or
    /// <see cref="WebStoreTable{TKey,TValue}.QueryKeysPage"/>. Carries the
    /// items collected for this page plus an opaque <see cref="NextCursor"/>
    /// that callers pass back to fetch the following page.
    /// </summary>
    /// <typeparam name="TValue">Element type of <see cref="Items"/>.</typeparam>
    public sealed class Page<TValue>
    {
        public Page(IList<TValue> items, Cursor nextCursor)
        {
            Items = items;
            NextCursor = nextCursor;
        }

        /// <summary>Records collected for this page (after filter, capped at page size).</summary>
        public IList<TValue> Items { get; }

        /// <summary>
        /// Opaque continuation token. <c>null</c> when the iterator drained the
        /// scan range before the page filled — that page is the final page. A
        /// non-<c>null</c> cursor signals that at least one additional record
        /// MAY exist; callers should pass it back via the <c>resumeFrom</c>
        /// argument to fetch the next page.
        /// </summary>
        public Cursor NextCursor { get; }

        /// <summary>True iff <see cref="NextCursor"/> is non-<c>null</c>.</summary>
        public bool HasMore => NextCursor != null;
    }
}
