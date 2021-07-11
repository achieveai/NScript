//-----------------------------------------------------------------------
// <copyright file="INotifyCollectionChanged.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;

    public interface INotifyCollectionChanged
    {
        /// <summary>
        /// Event for collection change
        /// </summary>
        event Action<INotifyCollectionChanged, CollectionChangedEventArgs> CollectionChanged;
    }

    /// <summary>
    /// Definition for INotifyCollectionChanged
    /// </summary>
    public interface INotifyCollectionChanged<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Event for collection change
        /// </summary>
        event Action<INotifyCollectionChanged<T>, CollectionChangedEventArgs<T>> CollectionChanged;
    }
}
