//-----------------------------------------------------------------------
// <copyright file="BindableObject.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Sunlight.Framework.Attributes;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for BindableObject
    /// </summary>
    public class ContextBindableObject : ExtensibleObservableObject, IDisposable
    {
        /// <summary>   Name of the data context property. </summary>
        public const string DataContextPropertyName = "DataContext";

        /// <summary>   Name of the is active property. </summary>
        public const string IsActivePropertyName = "IsActive";

        /// <summary>   The one time binders. </summary>
        List<OneTimeDataBinder> oneTimeBinders = new List<OneTimeDataBinder>();

        /// <summary>   The one time parent binders. </summary>
        List<OneTimeDataBinder> oneTimeParentBinders = new List<OneTimeDataBinder>();

        /// <summary>   The live data binders. </summary>
        List<DataBinder> liveDataBinders =new List<DataBinder>();

        /// <summary>   The live context parent binder. </summary>
        List<DataBinder> liveContextParentBinder = new List<DataBinder>();

        /// <summary>   Dictionary of properties. </summary>
        Dictionary propertyDict = new Dictionary();

        /// <summary>   The data context binder. </summary>
        DataBinder dataContextBinder;

        /// <summary>   true if this object is context binder. </summary>
        bool isContextBinder;

        /// <summary>   The parent. </summary>
        ContextBindableObject parent;

        /// <summary>   Context for the data. </summary>
        Object dataContext;

        /// <summary>   true if data context setter called. </summary>
        bool dataContextSetterCalled = false;

        /// <summary>   true if this object is active. </summary>
        bool isActive = false;

        /// <summary>   true if this object is pre activated. </summary>
        bool isPreActivated = false;

        /// <summary>   true if this object is activated. </summary>
        bool isActivated = false;

        /// <summary>   true if activate is pending and was blocked by one of the derived classes. </summary>
        bool pendingActivate = false;

        /// <summary>   true if this object is disposing. </summary>
        bool isDisposing = false;

        /// <summary>   true if this object is disposed. </summary>
        bool isDisposed = false;

        /// <summary>   Event queue for all listeners interested in OnDisposed events. </summary>
        public event Action OnDisposed;

        /// <summary>   true if this object is inactive if null context. </summary>
        private bool isInactiveIfNullContext = true;

        /// <summary>   Gets or sets the parent. </summary>
        /// <value> The parent. </value>
        public ContextBindableObject Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                if (this.parent != value)
                {
                    if (this.parent != null)
                    {
                        this.parent.RemovePropertyChangedListener(DataContextPropertyName, this.OnParentDataContextUpdated);
                        this.parent.RemovePropertyChangedListener(IsActivePropertyName, this.OnParentDataContextUpdated);
                    }

                    this.parent = value;

                    if (!this.dataContextSetterCalled || this.dataContextBinder != null)
                    {
                        if (this.parent != null)
                        {
                            this.parent.AddPropertyChangedListener(DataContextPropertyName, this.OnParentDataContextUpdated);
                            this.parent.AddPropertyChangedListener(IsActivePropertyName, this.OnParentDataContextUpdated);
                            this.OnParentDataContextUpdated(null, null);
                        }
                        else
                        {
                            this.SetDataContext(null);
                        }
                    }
                }
            }
        }

        /// <summary>   Gets or sets a context for the data. </summary>
        /// <value> The data context. </value>
        [DefaultDataBinding(Mode=DataBindingMode.OneWay, IsStrict=true)]
        public Object DataContext
        {
            get
            {
                return this.dataContext;
            }

            set
            {
                this.dataContextSetterCalled = true;
                this.SetDataContext(value);
            }
        }

        /// <summary>   Gets a value indicating whether this object is active. </summary>
        /// <value> true if this object is active, false if not. </value>
        public bool IsActive
        {
            get
            {
                return this.isActive && !this.ActivationBlocked;
            }
        }

        /// <summary>   Gets or sets a value indicating whether the inactive if null context. </summary>
        /// <value> true if inactive if null context, false if not. </value>
        [DefaultDataBinding(Mode=DataBindingMode.OneTime, IsStrict = true, DefaultValue=true)]
        public bool InactiveIfNullContext
        {
            get
            {
                return this.isInactiveIfNullContext;
            }

            set
            {
                this.isInactiveIfNullContext = value;
                this.FixActivation();
            }
        }

        /// <summary>   Gets a value indicating whether the activation blocked. </summary>
        /// <value> true if activation blocked, false if not. </value>
        protected virtual bool ActivationBlocked
        {
            get
            {
                return this.isInactiveIfNullContext && this.dataContext == null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed && !this.isDisposing)
            {
                this.InternalDispose();
            }
        }

        /// <summary>   Adds a data binder. </summary>
        /// <param name="binder">   The binder. </param>
        public void AddDataBinder(DataBinder binder)
        {
            if (this.AddDataContextBinder(binder, false))
            {
                return;
            }

            this.liveDataBinders.Add(binder);
        }

        /// <summary>   Adds an one time binder. </summary>
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        /// <param name="binder">   The binder. </param>
        public void AddOneTimeBinder(OneTimeDataBinder binder)
        {
            if (this.propertyDict.ContainsKey(binder.PropertyName))
            {
                throw new Exception(binder.PropertyName + " already added.");
            }
            else
            {
                this.propertyDict[binder.PropertyName] = null;
            }

            this.oneTimeBinders.Add(binder);
        }

        /// <summary>   Adds a parent binder. </summary>
        /// <param name="binder">   The binder. </param>
        public void AddParentBinder(DataBinder binder)
        {
            if (this.AddDataContextBinder(binder, false))
            {
                return;
            }

            this.liveContextParentBinder.Add(binder);
        }

        /// <summary>   Adds an one time parent binder. </summary>
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        /// <param name="binder">   The binder. </param>
        public void AddOneTimeParentBinder(OneTimeDataBinder binder)
        {
            if (this.propertyDict.ContainsKey(binder.PropertyName))
            {
                throw new Exception(binder.PropertyName + " already added.");
            }
            else
            {
                this.propertyDict[binder.PropertyName] = null;
            }

            this.oneTimeParentBinders.Add(binder);
        }

        /// <summary>   Activates this object. </summary>
        public void Activate()
        {
            this.isActive = true;
            this.FixActivation();
        }

        /// <summary>   Deactivates this object. </summary>
        public void Deactivate()
        {
            this.isActive = false;
            this.FixActivation();
        }

        /// <summary>   Pre activate. </summary>
        public void PreActivate()
        {
            if (!this.ActivationBlocked)
            {
                this.isPreActivated = true;
                this.OnBeforeFirstActivate();
            }
        }

        /// <summary>   Executes the pre activate action. </summary>
        protected virtual void OnBeforeFirstActivate()
        {
        }

        /// <summary>   Executes the activate action. </summary>
        protected virtual void OnActivate()
        {
        }

        /// <summary>   Executes the deactivate action. </summary>
        protected virtual void OnDeactivate()
        {
        }

        /// <summary>   Fix activation. </summary>
        protected void FixActivation()
        {
            if (!this.ActivationBlocked
                && this.isActive)
            {
                if (!this.isPreActivated)
                {
                    this.isPreActivated = true;
                    this.OnBeforeFirstActivate();
                }

                if (!this.isActivated)
                {
                    this.isActivated = true;
                    this.OnActivate();

                    this.FirePropertyChanged(IsActivePropertyName);
                }
            }
            else
            {
                if (this.isActivated)
                {
                    this.isActivated = false;
                    this.OnDeactivate();

                    this.FirePropertyChanged(IsActivePropertyName);
                }
            }
        }

        /// <summary>   Internal dispose. </summary>
        protected virtual void InternalDispose()
        {
            if (this.OnDisposed != null)
            {
                for (int i = 0, j = this.liveDataBinders.Count; i < j; i++)
                {
                    var dataBinder = this.liveDataBinders[i];
                    dataBinder.SetTarget(null);
                    dataBinder.SetSource(null);
                }

                for (int i = 0, j = this.liveContextParentBinder.Count; i < j; i++)
                {
                    var dataBinder = this.liveContextParentBinder[i];
                    dataBinder.SetTarget(null);
                    dataBinder.SetSource(null);
                }

                this.liveDataBinders.Clear();
                this.liveContextParentBinder.Clear();
                this.oneTimeBinders.Clear();
                this.oneTimeParentBinders.Clear();

                this.Parent = null;

                this.ClearListeners();
                this.OnDisposed();
            }
        }

        /// <summary>   Executes the data context updating action. </summary>
        /// <param name="newValue"> The new value. </param>
        protected virtual void OnDataContextUpdating(object newValue)
        {
        }

        /// <summary>   Executes the data context updated action. </summary>
        /// <param name="oldValue"> The old value. </param>
        protected virtual void OnDataContextUpdated(object oldValue)
        {
        }

        /// <summary>   Adds a data context binder to 'isContextParent'. </summary>
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        /// <param name="binder">           The binder. </param>
        /// <param name="isContextParent">  true if this object is context parent. </param>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        private bool AddDataContextBinder(DataBinder binder, bool isContextParent)
        {
            if (binder.TargetPropertyName == DataContextPropertyName)
            {
                if (this.dataContextBinder != null)
                {
                    throw new Exception("DataContext binder already initialized.");
                }

                this.dataContextBinder = binder;
                this.isContextBinder = isContextParent;

                return true;
            }
            else if (this.propertyDict.ContainsKey(binder.TargetPropertyName))
            {
                throw new Exception(binder.TargetPropertyName + " already added.");
            }
            else
            {
                this.propertyDict[binder.TargetPropertyName] = null;
            }

            return false;
        }

        /// <summary>   Sets a data context. </summary>
        /// <param name="value">    The value. </param>
        private void SetDataContext(object value)
        {
            if (this.dataContext != value)
            {
                this.OnDataContextUpdating(value);

                object oldValue = this.dataContext;
                this.dataContext = value;

                for (int i = 0, j = this.oneTimeBinders.Count; i < j; i++)
                {
                    this.oneTimeBinders[i].ApplySource(value, this);
                }

                for (int i = 0, j = this.liveDataBinders.Count; i < j; i++)
                {
                    this.liveDataBinders[i].SetSource(value);
                }

                this.OnDataContextUpdated(oldValue);
                this.FirePropertyChanged(DataContextPropertyName);
            }
        }

        /// <summary>   Executes the parent data context updated action. </summary>
        /// <param name="sender">       The sender. </param>
        /// <param name="propertyName"> Name of the property. </param>
        private void OnParentDataContextUpdated(INotifyPropertyChanged sender, string propertyName)
        {
            if (this.parent.IsActive
                && (!this.dataContextSetterCalled || this.dataContextBinder != null))
            {
                if (this.dataContextBinder != null)
                {
                    this.dataContextBinder.SetSource(this.parent.dataContext);
                }
                else
                {
                    this.SetDataContext(this.parent.DataContext);
                }
            }

            if (propertyName == IsActivePropertyName
                || propertyName == null)
            {
                if (this.parent.IsActive)
                {
                    this.Activate();
                }
                else
                {
                    this.Deactivate();
                }
            }
        }
    }
}
