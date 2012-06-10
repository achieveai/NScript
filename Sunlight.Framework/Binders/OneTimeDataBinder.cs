//-----------------------------------------------------------------------
// <copyright file="OneTimeDataBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for OneTimeDataBinder
    /// </summary>
    public class OneTimeDataBinder
    {
        Func<object, object> valueGetter;
        object defaultValue;
        Action<INotifyPropertyChanged, object> setter;
        string propertyName;

        public OneTimeDataBinder(
            string propertyName,
            Action<INotifyPropertyChanged, object> setter,
            Func<object, object> valueGetter,
            object defaultValue)
        {
            this.propertyName = propertyName;
            this.defaultValue = defaultValue;
            this.setter = setter;
            this.valueGetter = valueGetter;
        }

        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        public void ApplySource(
            object source,
            INotifyPropertyChanged target)
        {
            this.setter(target, this.GetValue(source));
        }

        private object GetValue(object source)
        {
            try
            {
                return this.valueGetter(source);
            }
            catch
            {
                return this.defaultValue;
            }
        }
    }
}
