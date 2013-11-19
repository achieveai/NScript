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
