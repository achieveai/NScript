//-----------------------------------------------------------------------
// <copyright file="ICollection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections
{
    using System;

    /// <summary>
    /// Definition for ICollection
    /// </summary>
    public interface ICollection : IEnumerable
    {
        // Summary:
        //     Gets the number of elements contained in the System.Collections.ICollection.
        //
        // Returns:
        //     The number of elements contained in the System.Collections.ICollection.
        int Count { get; }

        // Summary:
        //     Gets a value indicating whether access to the System.Collections.ICollection
        //     is synchronized (thread safe).
        //
        // Remarks:
        //     Roslyn's collection-expression binder requires
        //     System.Collections.ICollection.IsSynchronized as a well-known
        //     member when target-typing to any list-shaped interface. Phase F5
        //     of WI-47 added this declaration so collection expressions can
        //     bind cleanly when target-typed to one of the five list-shaped
        //     BCL interfaces. The transpiler emits single-threaded JavaScript;
        //     all implementers return false.
        bool IsSynchronized { get; }

        // Summary:
        //     Gets an object that can be used to synchronize access to the
        //     System.Collections.ICollection.
        //
        // Remarks:
        //     Companion well-known member to IsSynchronized — Roslyn's binder
        //     resolves both together. Phase F5 of WI-47 added this declaration.
        //     The JS runtime is single-threaded; implementers return `this`.
        object SyncRoot { get; }

        // Summary:
        //     Copies the elements of the System.Collections.ICollection to an System.Array,
        //     starting at a particular System.Array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from System.Collections.ICollection. The System.Array must have zero-based
        //     indexing.
        //
        //   index:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     index is less than zero.
        //
        //   System.ArgumentException:
        //     array is multidimensional.-or- The number of elements in the source System.Collections.ICollection
        //     is greater than the available space from index to the end of the destination
        //     array.-or-The type of the source System.Collections.ICollection cannot be
        //     cast automatically to the type of the destination array.
        void CopyTo(Array array, int index);
    }
}
