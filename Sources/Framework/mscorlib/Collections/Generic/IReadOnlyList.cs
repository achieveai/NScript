//-----------------------------------------------------------------------
// <copyright file="IReadOnlyList.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System;

    // Summary:
    //     Represents a read-only collection of elements that can be accessed by index.
    //
    // Type parameters:
    //   T:
    //     The type of elements in the read-only list. This type parameter is covariant.
    //     That is, you can use either the type you specified or any type that is more
    //     derived. For more information about covariance and contravariance, see Covariance
    //     and Contravariance in Generics.
    public interface IReadOnlyList<out T> : IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
    {
        // Summary:
        //     Gets the element at the specified index in the read-only list.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to get.
        //
        // Returns:
        //     The element at the specified index in the read-only list.
        T this[int index] { get; }
    }
}
