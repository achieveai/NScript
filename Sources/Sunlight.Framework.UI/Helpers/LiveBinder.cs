//-----------------------------------------------------------------------
// <copyright file="LiveBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Observables;
    using System;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Definition for LiveBinder.
    /// </summary>
    public class LiveBinder
    {
        /// <summary>
        /// Information describing the binder.
        /// </summary>
        private readonly SkinBinderInfo binderInfo;

        /// <summary>
        /// true if this object is active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Source for the.
        /// </summary>
        private object source;

        /// <summary>
        /// Target for the.
        /// </summary>
        private object target;

        /// <summary>
        /// The live objects.
        /// </summary>
        private object[] liveObjects;

        /// <summary>
        /// The path traversed.
        /// </summary>
        private int pathTraversed;

        /// <summary>
        /// true to updating.
        /// </summary>
        private bool updating;

        /// <summary>
        /// Array of extra objects.
        /// </summary>
        private NativeArray extraObjectArray;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="binderInfo"> Information describing the binder. </param>
        public LiveBinder(SkinBinderInfo binderInfo, NativeArray extraObjectArray = null)
        {
            this.binderInfo = binderInfo;
            this.extraObjectArray = extraObjectArray;
        }

        /// <summary>
        /// Gets the information describing the binder.
        /// </summary>
        /// <value>
        /// Information describing the binder.
        /// </value>
        public SkinBinderInfo BinderInfo
        { get { return this.binderInfo; } }

        /// <summary>
        /// Gets or sets source for the.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public object Source
        {
            get
            {
                return this.liveObjects != null
                    ? this.liveObjects[0]
                    : null;
            }

            set
            {
                if (this.source != value)
                {
                    this.source = value;
                    this.FlowValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets target for the.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public Object Target
        {
            get
            {
                return this.target;
            }

            set
            {
                if (this.target != value)
                {
                    if (this.target != null
                        && this.binderInfo.Mode == Binders.DataBindingMode.TwoWay)
                    {
                        ((INotifyPropertyChanged)this.target).RemovePropertyChangedListener(
                            this.binderInfo.TargetPropertyName,
                            this.OnTargetPropertyChanged);
                    }

                    this.target = value;

                    if (this.target != null
                        && this.binderInfo.Mode == Binders.DataBindingMode.TwoWay)
                    {
                        ((INotifyPropertyChanged)this.target).AddPropertyChangedListener(
                            this.binderInfo.TargetPropertyName,
                            this.OnTargetPropertyChanged);
                    }

                    this.FlowValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object is active.
        /// </summary>
        /// <value>
        /// true if this object is active, false if not.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;

                    if (this.isActive)
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

        /// <summary>
        /// Deactivate later.
        /// </summary>
        public void Cleanup()
        {
            if (!this.isActive)
            {
                this.pathTraversed = 0;
                this.CleanRegistrations();
            }
        }

        /// <summary>
        /// Activates this object.
        /// </summary>
        private void Activate()
        {
            this.FlowValue();
        }

        /// <summary>
        /// Deactivates this object.
        /// </summary>
        private void Deactivate()
        {
            this.isActive = false;
        }

        /// <summary>
        /// Flow value.
        /// </summary>
        private void FlowValue()
        {
            if (this.target == null || this.updating || !this.isActive)
            {
                return;
            }

            if (this.liveObjects == null)
            {
                this.liveObjects = new object[this.binderInfo.PropertyGetterPath.Length + 1];
            }

            if (this.liveObjects[0] != this.source)
            {
                if (this.liveObjects[0] != null)
                {
                    this.pathTraversed = 0;
                    this.CleanRegistrations();
                }

                this.liveObjects[0] = this.source;

                if (this.liveObjects[0] != null)
                {
                    ((INotifyPropertyChanged)this.liveObjects[0]).AddPropertyChangedListener(
                        this.binderInfo.PropertyNames[0],
                        this.OnSourcePropertyChanged);
                }
            }

            this.SetTargetProperty(this.GetValue());
        }

        /// <summary>
        /// Sets target property.
        /// </summary>
        /// <param name="value"> The value. </param>
        private void SetTargetProperty(object value)
        {
            try
            {
                this.updating = true;
                this.binderInfo.SetTargetValue(
                    this.target,
                    value,
                    this.extraObjectArray);
            }
            finally
            {
                this.updating = false;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>
        /// The value.
        /// </returns>
        private object GetValue()
        {
            object rv;
            try
            {
                rv = this.GetValueInternal();
            }
            catch
            {
                rv = this.binderInfo.DefaultValue;
            }

            if (this.pathTraversed < this.liveObjects.Length)
            {
                this.CleanRegistrations();
            }

            return rv;
        }

        /// <summary>
        /// Gets value internal.
        /// </summary>
        /// <returns>
        /// The value internal.
        /// </returns>
        private object GetValueInternal()
        {
            var binderInfo = this.binderInfo;
            var liveObjects = this.liveObjects;
            var src = liveObjects[0];
            var propertyGetterPath = binderInfo.PropertyGetterPath;
            int iPath = 1, pathLength = propertyGetterPath.Length + 1;
            var propertyNames = binderInfo.PropertyNames;
            this.pathTraversed = 1;

            for (; iPath < pathLength; iPath++)
            {
                if (src != null
                    || (iPath == 1 && (binderInfo.BinderType & BinderType.Static) == BinderType.Static))
                {
                    src = propertyGetterPath[iPath - 1](src);
                    if (liveObjects[iPath] != src)
                    {
                        if (liveObjects[iPath] != null
                            && iPath < pathLength - 1)
                        {
                            ((INotifyPropertyChanged)liveObjects[iPath]).RemovePropertyChangedListener(
                                propertyNames[iPath],
                                this.OnSourcePropertyChanged);
                        }

                        liveObjects[iPath] = src;

                        if (src != null && iPath < pathLength - 1)
                        {
                            ((INotifyPropertyChanged)src).AddPropertyChangedListener(
                                binderInfo.PropertyNames[iPath],
                                this.OnSourcePropertyChanged);
                        }
                    }

                    ++this.pathTraversed;
                }
            }

            if (this.pathTraversed < pathLength)
            {
                return binderInfo.DefaultValue;
            }
            else if (binderInfo.ForwardConverter != null)
            {
                return binderInfo.ForwardConverter(src);
            }
            else
            {
                return src;
            }
        }

        /// <summary>
        /// Clean registrations.
        /// </summary>
        private void CleanRegistrations()
        {
            object[] liveObjects = this.liveObjects;
            if (this.pathTraversed < this.liveObjects.Length)
            {
                liveObjects[liveObjects.Length - 1] = null;
                for (int iPath = this.binderInfo.PropertyGetterPath.Length - 2, till = this.pathTraversed;
                    iPath >= till; iPath--)
                {
                    ((INotifyPropertyChanged)liveObjects[iPath]).RemovePropertyChangedListener(
                        this.binderInfo.PropertyNames[iPath],
                        this.OnSourcePropertyChanged);
                    liveObjects[iPath] = null;
                }
            }
        }

        /// <summary>
        /// Executes the source property changed action.
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <param name="str"> The. </param>
        private void OnSourcePropertyChanged(
            INotifyPropertyChanged obj,
            string str)
        {
            this.FlowValue();
        }

        /// <summary>
        /// Executes the target property changed action.
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <param name="str"> The. </param>
        private void OnTargetPropertyChanged(
            INotifyPropertyChanged obj,
            string str)
        {
            if (this.updating || !this.isActive)
            {
                return;
            }

            try
            {
                var binderInfo = this.binderInfo;
                var target = this.target;
                var liveObjects = this.liveObjects;
                this.updating = true;
                if (target == obj
                    && this.source != null
                    && (liveObjects.Length < 2 || liveObjects[liveObjects.Length - 2] != null))
                {
                    object value = binderInfo.TargetPropertyGetter(target);
                    if (binderInfo.BackwardConverter != null)
                    {
                        value = binderInfo.BackwardConverter(value);
                    }

                    binderInfo.PropertySetter(
                        this.liveObjects.Length < 2
                            ? this.source
                            : this.liveObjects[this.liveObjects.Length - 2],
                        value);
                }
            }
            finally
            {
                this.updating = false;
            }
        }
    }
}
