//-----------------------------------------------------------------------
// <copyright file="IValueConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;

    /// <summary>
    /// Definition for IValueConverter
    /// </summary>
    public interface IValueConverter
    {
        object Convert(object value);
        object ConvertBack(object value);
    }
}
