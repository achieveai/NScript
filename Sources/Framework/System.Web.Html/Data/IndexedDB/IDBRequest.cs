namespace System.Web.Html.Data.IndexedDB
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Html;

    /// <summary>
    /// Asynchronous request object returned by most IDB operations. Subscribe to
    /// <see cref="OnSuccess"/> / <see cref="OnError"/> to observe completion.
    /// </summary>
    [IgnoreNamespace, ImportedType]
    public class IDBRequest : EventTarget
    {
        public extern event Action<IDBRequest, Event> OnError;

        public extern event Action<IDBRequest, Event> OnSuccess;

        /// <summary>Error returned by the underlying browser API, if any.</summary>
        public extern DomError Error
        { get; }

        /// <summary>State of the request ("pending" or "done").</summary>
        public extern string ReadyState
        { get; }

        /// <summary>Result payload on success. Shape depends on the operation.</summary>
        public extern object Result
        { get; }

        /// <summary>
        /// Source that issued this request — either an <see cref="IDBObjectStore{K, T}"/>
        /// or <see cref="IDBIndex{K, T}"/>.
        /// </summary>
        public extern object Source
        { get; }

        public extern IDBTransaction Transaction
        { get; }
    }
}
