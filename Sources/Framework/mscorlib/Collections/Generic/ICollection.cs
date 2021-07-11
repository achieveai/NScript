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