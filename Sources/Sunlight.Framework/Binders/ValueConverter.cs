//-----------------------------------------------------------------------
// <copyright file="ValueConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;

    /// <summary>
    /// Definition for ValueConverter
    /// </summary>
    public abstract class ValueConverter<TFrom, TTo> : IValueConverter
    {
        public virtual TTo Convert(TFrom fromValue)
        {
            throw new NotSupportedException();
        }

        public virtual TFrom ConvertBack(TTo tovalue)
        {
            throw new NotSupportedException();
        }

        object IValueConverter.Convert(object value)
        {
            return this.Convert((TFrom)value);
        }

        object IValueConverter.ConvertBack(object value)
        {
            return this.ConvertBack((TTo)value);
        }
    }
}
