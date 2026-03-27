//-----------------------------------------------------------------------
// <copyright file="MultiDependencyBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Observables;
    using System;

    /// <summary>
    /// Runtime binder that watches N observable properties on a source and
    /// recomputes a value via a getter function when any dependency changes.
    /// Used for Razor template computed expressions like @(Model.Price * Model.Quantity).
    /// </summary>
    public class MultiDependencyBinder
    {
        /// <summary>
        /// Source for the binding.
        /// </summary>
        private object source;

        /// <summary>
        /// Target for the binding.
        /// </summary>
        private object target;

        /// <summary>
        /// Getter function that computes the value from the source.
        /// </summary>
        private Func<object, object> getter;

        /// <summary>
        /// Setter action that applies the computed value to the target.
        /// </summary>
        private Action<object, object> setter;

        /// <summary>
        /// Names of the properties to watch on the source.
        /// </summary>
        private string[] propertyNames;

        /// <summary>
        /// true if this binder is active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// true if currently updating to prevent re-entrant flows.
        /// </summary>
        private bool updating;

        /// <summary>
        /// Cached callback delegate for property change notifications.
        /// </summary>
        private Action<INotifyPropertyChanged, string> propertyChangedCallback;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="getter"> Getter function that computes the value from the source. </param>
        /// <param name="setter"> Setter action that applies the computed value to the target. </param>
        /// <param name="propertyNames"> Names of the properties to watch on the source. </param>
        public MultiDependencyBinder(
            Func<object, object> getter,
            Action<object, object> setter,
            string[] propertyNames)
        {
            this.getter = getter;
            this.setter = setter;
            this.propertyNames = propertyNames;
            this.propertyChangedCallback = this.OnSourcePropertyChanged;
        }

        /// <summary>
        /// Gets or sets source for the binding.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public object Source
        {
            get { return this.source; }
            set
            {
                if (this.source == value)
                {
                    return;
                }

                if (this.isActive && !object.IsNullOrUndefined(this.source))
                {
                    this.UnregisterListeners((INotifyPropertyChanged)this.source);
                }

                this.source = value;

                if (this.isActive && !object.IsNullOrUndefined(this.source))
                {
                    this.RegisterListeners((INotifyPropertyChanged)this.source);
                    this.FlowValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets target for the binding.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public object Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this binder is active.
        /// </summary>
        /// <value>
        /// true if this binder is active, false if not.
        /// </value>
        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive == value)
                {
                    return;
                }

                this.isActive = value;

                if (value)
                {
                    if (!object.IsNullOrUndefined(this.source))
                    {
                        this.RegisterListeners((INotifyPropertyChanged)this.source);
                    }

                    this.FlowValue();
                }
                else
                {
                    if (!object.IsNullOrUndefined(this.source))
                    {
                        this.UnregisterListeners((INotifyPropertyChanged)this.source);
                    }
                }
            }
        }

        /// <summary>
        /// Disposes this binder, deactivating and clearing references.
        /// </summary>
        public void Dispose()
        {
            this.IsActive = false;
            this.source = null;
            this.target = null;
        }

        /// <summary>
        /// Registers property changed listeners for all watched properties.
        /// </summary>
        /// <param name="notify"> The notify object to register on. </param>
        private void RegisterListeners(INotifyPropertyChanged notify)
        {
            for (int i = 0; i < this.propertyNames.Length; i++)
            {
                notify.AddPropertyChangedListener(
                    this.propertyNames[i],
                    this.propertyChangedCallback);
            }
        }

        /// <summary>
        /// Unregisters property changed listeners for all watched properties.
        /// </summary>
        /// <param name="notify"> The notify object to unregister from. </param>
        private void UnregisterListeners(INotifyPropertyChanged notify)
        {
            for (int i = 0; i < this.propertyNames.Length; i++)
            {
                notify.RemovePropertyChangedListener(
                    this.propertyNames[i],
                    this.propertyChangedCallback);
            }
        }

        /// <summary>
        /// Executes the source property changed action.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="propertyName"> The property name. </param>
        private void OnSourcePropertyChanged(INotifyPropertyChanged sender, string propertyName)
        {
            if (this.updating)
            {
                return;
            }

            this.FlowValue();
        }

        /// <summary>
        /// Computes the value from source and applies it to target.
        /// </summary>
        private void FlowValue()
        {
            if (object.IsNullOrUndefined(this.source) || object.IsNullOrUndefined(this.target))
            {
                return;
            }

            this.updating = true;
            try
            {
                var value = this.getter(this.source);
                this.setter(this.target, value);
            }
            finally
            {
                this.updating = false;
            }
        }
    }
}
