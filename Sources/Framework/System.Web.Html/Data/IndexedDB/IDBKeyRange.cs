namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a contiguous range of keys. Used to scope cursors and queries.
    /// </summary>
    [IgnoreNamespace, IgnoreGenericArguments]
    public class IDBKeyRange
    {
        public extern object Lower
        { get; }

        public extern bool LowerOpen
        { get; }

        public extern object Upper
        { get; }

        public extern bool UpperOpen
        { get; }

        /// <summary>A range that spans [lower, upper], inclusive on both sides.</summary>
        public static extern IDBKeyRange Bound(
            object lower,
            object upper);

        /// <summary>A range that spans (lower, upper) with configurable openness.</summary>
        public static extern IDBKeyRange Bound(
            object lower,
            object upper,
            bool lowerOpen,
            bool upperOpen);

        public static extern IDBKeyRange LowerBound(
            object lower,
            bool? open = null);

        public static extern IDBKeyRange UpperBound(
            object upper,
            bool? open = null);

        /// <summary>A range that matches a single key.</summary>
        public static extern IDBKeyRange Only(object any);
    }
}
