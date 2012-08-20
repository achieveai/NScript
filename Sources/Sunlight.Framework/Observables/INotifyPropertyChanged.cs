//-----------------------------------------------------------------------
// <copyright file="INotifyPropertyChanged.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;

    /// <summary>
    /// Interface for property change contract
    /// </summary>
    public interface INotifyPropertyChanged
    {
        /// <summary>
        /// Add a callback listening for property changes
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="callback">Callback method</param>
        void AddPropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback);

        /// <summary>
        /// Remove a callback listening for property changes
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="callback">Callback method</param>
        void RemovePropertyChangedListener(string propertyName, Action<INotifyPropertyChanged, string> callback);
    }
}
