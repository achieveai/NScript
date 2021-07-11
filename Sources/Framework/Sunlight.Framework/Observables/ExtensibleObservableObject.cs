//-----------------------------------------------------------------------
// <copyright file="ExtensibleObservableObject.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
    using System.Collections;

    /// <summary>
    /// Definition for ExtensibleObservableObject
    /// </summary>
    public class ExtensibleObservableObject : ObservableObject
    {
        Dictionary propertyMap = new Dictionary();

        public void SetValue<T>(AttachedProperty<T> property, T value)
        {
            T oldValue = this.GetValue(property);

            if (!object.Equals(oldValue, value))
            {
                this.propertyMap[property.Name] = value;
                base.FirePropertyChanged(property.Name);
            }
        }

        public T GetValue<T>(AttachedProperty<T> property)
        {
            if (this.propertyMap.ContainsKey(property.Name))
            {
                return (T)this.propertyMap[property.Name];
            }

            return default(T);
        }

        public void DeleteValue<T>(AttachedProperty<T> property)
        {
            if (this.propertyMap.ContainsKey(property.Name))
            {
                this.propertyMap.Remove(property.Name);
                this.FirePropertyChanged(property.Name);
            }
        }
    }
}
