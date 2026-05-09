//-----------------------------------------------------------------------
// <copyright file="ICollection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    /// <summary>
    /// Definition for ICollection
    /// </summary>
    public interface ICollection<T> : ICollection, IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <value>
        /// The number of elements contained in the <see cref="ICollection{T}"/>.
        /// </value>
        /// <remarks>
        /// Roslyn's collection-expression binder requires
        /// <c>System.Collections.Generic.ICollection`1.Count</c> as a
        /// well-known member when target-typing to <c>ICollection&lt;T&gt;</c>
        /// (or any list-shaped interface that derives from it). The non-
        /// generic <c>System.Collections.ICollection.Count</c> is not enough
        /// — the binder anchors on the generic interface specifically. Phase
        /// F5 of WI-47 added this declaration so collection expressions can
        /// bind cleanly when target-typed to one of the five list-shaped
        /// BCL interfaces.
        /// </remarks>
        new int Count { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        bool IsReadOnly
        { get; }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void Add(T item);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether this collection contains the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// <c>true</c> if this collection contains the specified item; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="arr">The arr.</param>
        /// <param name="index">The index.</param>
        void CopyTo(T[] arr, int index);

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if item was present and remove; otherwise false.</returns>
        bool Remove(T item);
    }
}