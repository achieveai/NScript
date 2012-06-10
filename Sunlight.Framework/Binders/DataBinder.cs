//-----------------------------------------------------------------------
// <copyright file="DataBinderBase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Binders
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for DataBinderBase
    /// </summary>
    public class DataBinder : ISourceDataBinder, ITargetDataBinder
    {
        SourcePropertyBinder sourceBinder;
        TargetBinder targetBinder;
        IValueConverter converter;
        DataBindingMode bindingMode;
        bool firstBindingSuccessful = false;

        public DataBinder(
            SourcePropertyBinder sourceBinder,
            TargetBinder targetBinder,
            DataBindingMode dataBindingMode,
            IValueConverter converter)
        {
            this.sourceBinder = sourceBinder;
            this.targetBinder = targetBinder;
            this.bindingMode = dataBindingMode;
            this.converter = converter;

            this.targetBinder.UseDataBinder(this);
            this.sourceBinder.UseDataBinder(this);
        }

        public void SetTarget(INotifyPropertyChanged target)
        {
            this.targetBinder.Target = target;
            this.ApplyBinding();
        }

        public void SetSource(object source)
        {
            this.sourceBinder.Source = source;
        }

        public void SourceValueUpdated()
        {
            this.ApplyBinding();
        }

        public void TargetValueUpdated()
        {
            this.ApplyBackBinding();
        }

        private void ApplyBinding()
        {
            if (this.targetBinder.IsActive)
            {
                if (this.bindingMode == DataBindingMode.OneTime)
                {
                    if (!this.firstBindingSuccessful)
                    {
                        if (this.sourceBinder.IsActive)
                        {
                            this.targetBinder.Value = this.converter == null
                                ? this.sourceBinder.Value
                                : this.converter.Convert(this.sourceBinder.Value);
                            this.firstBindingSuccessful = true;
                        }
                        else
                        {
                            this.targetBinder.SetDefault();
                        }
                    }

                    return;
                }

                if (this.sourceBinder.IsActive)
                {
                    if (this.converter != null)
                    {
                        this.targetBinder.Value = this.converter.Convert(this.sourceBinder.Value);
                    }
                    else
                    {
                        this.targetBinder.Value = this.sourceBinder.Value;
                    }
                }
                else
                {
                    this.targetBinder.SetDefault();
                }
            }
        }

        private void ApplyBackBinding()
        {
            if (!this.targetBinder.IsWriteOnly)
            {
                if (this.sourceBinder.IsActive)
                {
                    this.sourceBinder.Value = this.targetBinder.Value;
                }
            }
        }
    }
}
