//-----------------------------------------------------------------------
// <copyright file="SourcePropertyBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for PropertyBinder
    /// </summary>
    public class SourcePropertyBinder
    {
        private readonly int chainLength;

        /// <summary>
        /// Property part names.
        /// </summary>
        private string[] propertyPartNames;

        /// <summary>
        /// Property getter chain.
        /// </summary>
        private Func<object, object>[] propertyGetterChain;

        private Action<INotifyPropertyChanged, string>[] changeRegistrations;

        /// <summary>
        /// Object chain.
        /// </summary>
        private object[] objectChain;

        /// <summary>
        /// PropertySetter for leaf property.
        /// </summary>
        private Action<object, object> propertySetter;

        /// <summary>
        /// Base dataBinder.
        /// </summary>
        private ISourceDataBinder dataBinderBase;

        /// <summary>
        /// Property value at the end.
        /// </summary>
        private object value;

        /// <summary>
        /// Tracker to track if current binding is active or not.
        /// </summary>
        private bool isActive;

        public SourcePropertyBinder(
            string[] propertyPartNames,
            Func<object, object>[] propertyGetterChain,
            Action<object, object> propertySetter)
        {
            this.propertyPartNames = propertyPartNames;
            this.chainLength = this.propertyPartNames.Length;
            this.propertyGetterChain = propertyGetterChain;
            this.propertySetter = propertySetter;
            this.objectChain = new object[this.chainLength];
            this.changeRegistrations =
                new Action<INotifyPropertyChanged, string>[this.chainLength];

            for (int i = this.chainLength - 1; i >= 0; i--)
            {
                this.changeRegistrations[i] =
                    this.GetChangeTrackerAt(i);
            }
        }

        public object Source
        {
            get
            {
                return this.objectChain[0];
            }

            set
            {
                if (this.objectChain[0] != value)
                {
                    this.SetObjectChainElementValue(0, value);
                    this.CalculateValueFrom(0);
                }
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.isActive &&
                    this.propertySetter != null)
                {
                    this.propertySetter(
                        this.objectChain[this.objectChain.Length - 1],
                        value);
                }
            }
        }

        public bool IsActive
        { get { return this.isActive; } }

        public void UseDataBinder(ISourceDataBinder dataBinderBase)
        {
            this.dataBinderBase = dataBinderBase;
        }

        private void CalculateValueFrom(int index)
        {
            for (int i = index; i < this.chainLength; i++)
            {
                if (this.objectChain[i] == null)
                {
                    for (int j = this.chainLength - 1; j >= i; j--)
                    {
                        this.SetObjectChainElementValue(j, null);
                    }

                    if (this.value != null || this.isActive)
                    {
                        this.isActive = false;
                        this.value = null;

                        if (this.dataBinderBase != null)
                        {
                            this.dataBinderBase.SourceValueUpdated();
                        }
                    }

                    break;
                }
                else
                {
                    object nextValue =
                        this.propertyGetterChain[i](this.objectChain[i]);

                    if (i < this.objectChain.Length - 1)
                    {
                        this.SetObjectChainElementValue(
                            i + 1,
                            nextValue);
                    }
                    else if (this.value != nextValue || !this.isActive)
                    {
                        this.isActive = true;
                        this.value = nextValue;

                        if (this.dataBinderBase != null)
                        {
                            this.dataBinderBase.SourceValueUpdated();
                        }
                    }
                }
            }
        }

        private void SetObjectChainElementValue(int index, object value)
        {
            if (index > this.chainLength)
            { return; }

            INotifyPropertyChanged oldValue = this.objectChain[index] as INotifyPropertyChanged;
            if (oldValue != null)
            {
                oldValue.RemovePropertyChangedListener(
                    this.propertyPartNames[index],
                    this.changeRegistrations[index]);
            }

            this.objectChain[index] = value;

            INotifyPropertyChanged newNotifiableValue = value as INotifyPropertyChanged;
            if (newNotifiableValue != null)
            {
                newNotifiableValue.AddPropertyChangedListener(
                    this.propertyPartNames[index],
                    this.changeRegistrations[index]);
            }
        }

        private Action<INotifyPropertyChanged, string> GetChangeTrackerAt(int index)
        {
            return delegate(INotifyPropertyChanged sender, string prop)
            {
                this.CalculateValueFrom(index);
            };
        }
    }
}
