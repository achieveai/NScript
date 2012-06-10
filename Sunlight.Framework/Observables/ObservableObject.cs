//-----------------------------------------------------------------------
// <copyright file="ObservableObject.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ObservableObject
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Collection of event handlers
        /// </summary>
        private StringDictionary<Action<INotifyPropertyChanged, string>> eventHandlers = null;

        /// <summary>
        /// Add a callback listening for property changes
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="callback">Callback method</param>
        public void AddPropertyChangedListener(
            string propertyName,
            Action<INotifyPropertyChanged, string> callback)
        {
            Action<INotifyPropertyChanged, string> cb;

            if (this.eventHandlers == null)
            {
                this.eventHandlers = new StringDictionary<Action<INotifyPropertyChanged, string>>();
            }

            if (!this.eventHandlers.TryGetValue(propertyName, out cb))
            {
                cb = callback;
            }
            else
            {
                cb += callback;
            }

            this.eventHandlers[propertyName] = cb;
        }

        /// <summary>
        /// Removes a callback listening for property changes
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="callback">Callback method</param>
        public void RemovePropertyChangedListener(
            string propertyName,
            Action<INotifyPropertyChanged, string> callback)
        {
            if (this.eventHandlers == null)
            {
                return;
            }

            Action<INotifyPropertyChanged, string> cb;
            if (this.eventHandlers.TryGetValue(propertyName, out cb))
            {
                cb = (Action<INotifyPropertyChanged, string>)Delegate.Remove(cb, callback);
                if (cb != null)
                {
                    this.eventHandlers[propertyName] = cb;
                }
                else
                {
                    this.eventHandlers.Remove(propertyName);
                }
            }
        }

        /// <summary>
        /// Clears all listeners
        /// </summary>
        protected void ClearListeners()
        {
            this.eventHandlers = null;
        }

        /// <summary>
        /// Fires the property changed1.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void FirePropertyChanged(
            string propertyName)
        {
            if (this.eventHandlers != null)
            {
                Action<INotifyPropertyChanged, string> cb;
                if (this.eventHandlers.TryGetValue(propertyName, out cb))
                {
                    cb(this, propertyName);
                }
            }
        }

        /// <summary>
        /// Fires the property changed2.
        /// </summary>
        /// <param name="propertyName1">The property name1.</param>
        /// <param name="propertyName2">The property name2.</param>
        protected void FirePropertyChanged(
            string propertyName1,
            string propertyName2)
        {
            this.FirePropertyChanged(propertyName1);
            this.FirePropertyChanged(propertyName2);
        }

        /// <summary>
        /// Fires the property changed3.
        /// </summary>
        /// <param name="propertyName1">The property name1.</param>
        /// <param name="propertyName2">The property name2.</param>
        /// <param name="propertyName3">The property name3.</param>
        protected void FirePropertyChanged(
            string propertyName1,
            string propertyName2,
            string propertyName3)
        {
            this.FirePropertyChanged(propertyName1);
            this.FirePropertyChanged(propertyName2);
            this.FirePropertyChanged(propertyName3);
        }

        /// <summary>
        /// Raises PropertyChanged event
        /// </summary>
        /// <param name="propertyNames">An array of properties that are changed.</param>
        protected void FirePropertyChanged(
            string[] propertyNames)
        {
            if (this.eventHandlers != null)
            {
                Action<INotifyPropertyChanged, string> cb;

                for (int iProp = 0; iProp < propertyNames.Length; iProp++)
                {
                    string propertyName = propertyNames[iProp];
                    if (this.eventHandlers.TryGetValue(propertyName, out cb))
                    {
                        cb(this, propertyName);
                    }
                }
            }
        }
    }
}