//-----------------------------------------------------------------------
// <copyright file="TargetBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for TargetBinder
    /// </summary>
    public class TargetBinder : ITargetBinder
    {
        private object value;

        private INotifyPropertyChanged target;

        private Func<INotifyPropertyChanged, object> getter;

        private Action<INotifyPropertyChanged, object> setter;

        private string propertyName;

        private object defaultValue;

        private ITargetDataBinder dataBinder;

        public TargetBinder(
            string propertyName,
            Func<INotifyPropertyChanged, object> getter,
            Action<INotifyPropertyChanged, object> setter,
            object defaultValue)
        {
            this.propertyName = propertyName;
            this.getter = getter;
            this.setter = setter;
            this.defaultValue = defaultValue;
        }

        public string PropertyName
        {
            get { return this.propertyName; }
        }

        public object Value
        {
            get
            { return this.value; }

            set
            {
                if (this.value != value
                    && this.setter != null)
                {
                    this.value = value;

                    if (this.IsActive)
                    {
                        this.setter(this.target, this.value);
                    }
                }
            }
        }

        public bool IsWriteOnly
        {
            get { return this.getter == null; }
        }

        public bool IsActive
        {
            get { return this.target != null; }
        }

        public INotifyPropertyChanged Target
        {
            get
            { return this.target; }

            set
            {
                if (this.target != value)
                {
                    if (this.target != null)
                    {
                        this.target.RemovePropertyChangedListener(
                            this.propertyName,
                            this.OnTargetUpdated);
                    }

                    this.target = value;
                    this.value = null;

                    if (this.target != null)
                    {
                        this.target.AddPropertyChangedListener(
                            this.propertyName,
                            this.OnTargetUpdated);

                        if (this.getter != null)
                        { this.value = this.getter(this.target); }
                    }
                }
            }
        }

        public void SetDefault()
        {
            this.Value = this.defaultValue;
        }

        public void UseDataBinder(ITargetDataBinder dataBinder)
        {
            this.dataBinder = dataBinder;
        }

        protected void SetPropertySetter(Action<INotifyPropertyChanged, object> setter)
        {
            this.setter = setter;
        }

        private void OnTargetUpdated(INotifyPropertyChanged sender, string propertyName)
        {
            if (this.dataBinder != null
                && this.getter != null)
            {
                this.value = this.getter(this.target);
                this.dataBinder.TargetValueUpdated();
            }
        }
    }
}
