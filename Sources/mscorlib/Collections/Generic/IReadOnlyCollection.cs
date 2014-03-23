//-----------------------------------------------------------------------
// <copyright file="IReadOnlyCollection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Collections.Generic
{
    using System;

    // Summary:
    //     Represents a strongly-typed, read-only collection of elements.
    //
    // Type parameters:
    //   T:
    //     The type of the elements.This type parameter is covariant. That is, you can
    //     use either the type you specified or any type that is more derived. For more
    //     information about covariance and contravariance, see Covariance and Contravariance
    //     in Generics.
    public interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable
    {
        // Summary:
        //     Gets the number of elements in the collection.
        //
        // Returns:
        //     The number of elements in the collection.
        int Count { get; }
    }
}
