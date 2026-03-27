//-----------------------------------------------------------------------
// <copyright file="ConditionalBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Observables;
    using System;
    using System.Web.Html;

    /// <summary>
    /// Runtime binder for reactive @if/@else blocks.
    /// Watches a boolean condition on the DataContext and swaps DOM fragments.
    /// </summary>
    public class ConditionalBinder
    {
        /// <summary>
        /// Source for the binding.
        /// </summary>
        private object source;

        /// <summary>
        /// Getter function that evaluates the boolean condition from the source.
        /// </summary>
        private Func<object, bool> conditionGetter;

        /// <summary>
        /// Name of the property to watch for condition changes.
        /// </summary>
        private string conditionPropertyName;

        /// <summary>
        /// The parent DOM element that contains the conditional content.
        /// </summary>
        private Element parentElement;

        /// <summary>
        /// Template element to clone when condition is true.
        /// </summary>
        private Element trueTemplate;

        /// <summary>
        /// Template element to clone when condition is false.
        /// </summary>
        private Element falseTemplate;

        /// <summary>
        /// The currently active DOM element (cloned from a template).
        /// </summary>
        private Element currentElement;

        /// <summary>
        /// true if this binder is active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// The current evaluated condition value.
        /// </summary>
        private bool currentCondition;

        /// <summary>
        /// Cached callback delegate for property change notifications.
        /// </summary>
        private Action<INotifyPropertyChanged, string> callback;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="conditionGetter"> Getter function that evaluates the boolean condition. </param>
        /// <param name="conditionPropertyName"> Name of the property to watch. </param>
        /// <param name="parentElement"> The parent DOM element. </param>
        /// <param name="trueTemplate"> Template element for true condition. </param>
        /// <param name="falseTemplate"> Template element for false condition. </param>
        public ConditionalBinder(
            Func<object, bool> conditionGetter,
            string conditionPropertyName,
            Element parentElement,
            Element trueTemplate,
            Element falseTemplate)
        {
            this.conditionGetter = conditionGetter;
            this.conditionPropertyName = conditionPropertyName;
            this.parentElement = parentElement;
            this.trueTemplate = trueTemplate;
            this.falseTemplate = falseTemplate;
            this.callback = this.OnPropertyChanged;
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
                    ((INotifyPropertyChanged)this.source).RemovePropertyChangedListener(
                        this.conditionPropertyName, this.callback);
                }

                this.source = value;

                if (this.isActive && !object.IsNullOrUndefined(this.source))
                {
                    ((INotifyPropertyChanged)this.source).AddPropertyChangedListener(
                        this.conditionPropertyName, this.callback);
                    this.Evaluate();
                }
            }
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
                        ((INotifyPropertyChanged)this.source).AddPropertyChangedListener(
                            this.conditionPropertyName, this.callback);
                    }

                    this.Evaluate();
                }
                else
                {
                    if (!object.IsNullOrUndefined(this.source))
                    {
                        ((INotifyPropertyChanged)this.source).RemovePropertyChangedListener(
                            this.conditionPropertyName, this.callback);
                    }

                    this.RemoveCurrent();
                }
            }
        }

        /// <summary>
        /// Disposes this binder, deactivating and clearing references.
        /// </summary>
        public void Dispose()
        {
            this.IsActive = false;
            this.RemoveCurrent();
            this.source = null;
        }

        /// <summary>
        /// Executes the property changed action.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="name"> The property name. </param>
        private void OnPropertyChanged(INotifyPropertyChanged sender, string name)
        {
            this.Evaluate();
        }

        /// <summary>
        /// Evaluates the condition and swaps DOM fragments accordingly.
        /// </summary>
        private void Evaluate()
        {
            if (object.IsNullOrUndefined(this.source))
            {
                return;
            }

            var condition = this.conditionGetter(this.source);
            if (condition == this.currentCondition && !object.IsNullOrUndefined(this.currentElement))
            {
                return;
            }

            this.currentCondition = condition;
            this.RemoveCurrent();

            var template = condition ? this.trueTemplate : this.falseTemplate;
            if (!object.IsNullOrUndefined(template))
            {
                this.currentElement = template.CloneNode(true);
                this.parentElement.AppendChild(this.currentElement);
            }
        }

        /// <summary>
        /// Removes the currently active DOM element.
        /// </summary>
        private void RemoveCurrent()
        {
            if (!object.IsNullOrUndefined(this.currentElement))
            {
                this.currentElement.Remove();
                this.currentElement = null;
            }
        }
    }
}
