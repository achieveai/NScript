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
        /// The linked properties.
        /// </summary>
        private StringDictionary<List<string>> linkedProperties = null;

        public event Action<INotifyPropertyChanged, string> AnyPropertyListener;

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
            this.AnyPropertyListener = null;
        }

        /// <summary>
        /// Adds a linked property to 'otherProperty'.
        /// </summary>
        /// <param name="sourceProperty"> Source property. </param>
        /// <param name="otherProperty">  The other property. </param>
        protected void AddLinkedProperty(string sourceProperty, string otherProperty)
        {
            if (this.linkedProperties == null)
            { this.linkedProperties = new StringDictionary<List<string>>(); }

            List<string> propertyNames = null;
            if (!this.linkedProperties.TryGetValue(sourceProperty, out propertyNames))
            {
                propertyNames = new List<string>();
                this.linkedProperties.Add(sourceProperty, propertyNames);
            }

            propertyNames.Add(otherProperty);
        }

        /// <summary>
        /// Fires properties changed for all handlers.
        /// </summary>
        protected void FireAllPropertiesChanged()
        {
            if (this.eventHandlers != null)
            {
                var keys = this.eventHandlers.Keys;
                foreach (var key in keys)
                {
                    var value = this.eventHandlers[key];
                    if (value != null)
                    { value(this, key); }
                }
            }

            if (AnyPropertyListener != null)
            { this.AnyPropertyListener(this, null); }
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

                if (this.linkedProperties != null)
                {
                    List<string> linkedProperties;
                    if (this.linkedProperties.TryGetValue(propertyName, out linkedProperties))
                    {
                        for (int iProp = 0; iProp < linkedProperties.Count; iProp++)
                        {
                            if (this.eventHandlers.TryGetValue(linkedProperties[iProp], out cb))
                            {
                                cb(this, propertyName);
                            }
                        }
                    }
                }
            }

            if (this.AnyPropertyListener != null)
            { this.AnyPropertyListener(this, propertyName); }
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
        /// Fires the property changed3.
        /// </summary>
        /// <param name="propertyName1"> The property name1. </param>
        /// <param name="propertyName2"> The property name2. </param>
        /// <param name="propertyName3"> The property name3. </param>
        /// <param name="propertyName4"> The fourth property name. </param>
        protected void FirePropertyChanged(
            string propertyName1,
            string propertyName2,
            string propertyName3,
            string propertyName4)
        {
            this.FirePropertyChanged(propertyName1);
            this.FirePropertyChanged(propertyName2);
            this.FirePropertyChanged(propertyName3);
            this.FirePropertyChanged(propertyName4);
        }

        /// <summary>
        /// Fires the property changed3.
        /// </summary>
        /// <param name="propertyName1"> The property name1. </param>
        /// <param name="propertyName2"> The property name2. </param>
        /// <param name="propertyName3"> The property name3. </param>
        /// <param name="propertyName4"> The fourth property name. </param>
        /// <param name="propertyName5"> The fifth property name. </param>
        protected void FirePropertyChanged(
            string propertyName1,
            string propertyName2,
            string propertyName3,
            string propertyName4,
            string propertyName5)
        {
            this.FirePropertyChanged(propertyName1);
            this.FirePropertyChanged(propertyName2);
            this.FirePropertyChanged(propertyName3);
            this.FirePropertyChanged(propertyName4);
            this.FirePropertyChanged(propertyName5);
        }

        /// <summary>
        /// Raises PropertyChanged event
        /// </summary>
        /// <param name="propertyNames">An array of properties that are changed.</param>
        protected void FirePropertyChanged(
            params string[] propertyNames)
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

                if (this.linkedProperties != null)
                {
                    List<string> linkedProperties;
                    for (int iProp = 0; iProp < propertyNames.Length; iProp++)
                    {
                        string propertyName = propertyNames[iProp];
                        if (this.linkedProperties.TryGetValue(propertyName, out linkedProperties))
                        {
                            for (int jProp = 0; jProp < linkedProperties.Count; jProp++)
                            {
                                if (this.eventHandlers.TryGetValue(linkedProperties[jProp], out cb))
                                {
                                    cb(this, propertyName);
                                }
                            }
                        }
                    }
                }
            }

            if (this.AnyPropertyListener != null)
            {
                for (int iProp = 0; iProp < propertyNames.Length; iProp++)
                { this.AnyPropertyListener(this, propertyNames[iProp]); }

            }
        }
    }
}