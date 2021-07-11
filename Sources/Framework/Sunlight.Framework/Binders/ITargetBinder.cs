//-----------------------------------------------------------------------
// <copyright file="ITargetBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for ITargetBinder
    /// </summary>
    public interface ITargetBinder
    {
        object Value
        { get; set; }

        bool IsWriteOnly
        { get; }

        bool IsActive
        { get; }

        INotifyPropertyChanged Target
        { get; set; }

        void SetDefault();

        void UseDataBinder(ITargetDataBinder dataBinder);
    }
}
