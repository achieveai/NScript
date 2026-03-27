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
    /// Watches boolean conditions on the DataContext and swaps DOM fragments.
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
        /// Names of the properties to watch for condition changes.
        /// </summary>
        private string[] conditionPropertyNames;

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
        /// Anchor node that marks the insertion point for conditional content.
        /// </summary>
        private Node anchorNode;

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
        /// <param name="conditionPropertyNames"> Names of the properties to watch. </param>
        /// <param name="parentElement"> The parent DOM element. </param>
        /// <param name="trueTemplate"> Template element for true condition. </param>
        /// <param name="falseTemplate"> Template element for false condition. </param>
        public ConditionalBinder(
            Func<object, bool> conditionGetter,
            string[] conditionPropertyNames,
            Element parentElement,
            Element trueTemplate,
            Element falseTemplate)
        {
            this.conditionGetter = conditionGetter;
            this.conditionPropertyNames = conditionPropertyNames;
            this.parentElement = parentElement;
            this.trueTemplate = trueTemplate;
            this.falseTemplate = falseTemplate;
            this.callback = this.OnPropertyChanged;
            this.anchorNode = parentElement.OwnerDocument.CreateTextNode("");
            this.parentElement.AppendChild((Element)this.anchorNode);
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
                        this.RegisterListeners((INotifyPropertyChanged)this.source);
                    }

                    this.Evaluate();
                }
                else
                {
                    if (!object.IsNullOrUndefined(this.source))
                    {
                        this.UnregisterListeners((INotifyPropertyChanged)this.source);
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
            this.source = null;
        }

        /// <summary>
        /// Registers property changed listeners for all watched properties.
        /// </summary>
        /// <param name="notify"> The notify object to register on. </param>
        private void RegisterListeners(INotifyPropertyChanged notify)
        {
            for (int i = 0; i < this.conditionPropertyNames.Length; i++)
            {
                notify.AddPropertyChangedListener(
                    this.conditionPropertyNames[i],
                    this.callback);
            }
        }

        /// <summary>
        /// Unregisters property changed listeners for all watched properties.
        /// </summary>
        /// <param name="notify"> The notify object to unregister from. </param>
        private void UnregisterListeners(INotifyPropertyChanged notify)
        {
            for (int i = 0; i < this.conditionPropertyNames.Length; i++)
            {
                notify.RemovePropertyChangedListener(
                    this.conditionPropertyNames[i],
                    this.callback);
            }
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
            if (!this.isActive)
            {
                return;
            }

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
                this.parentElement.InsertBefore(this.currentElement, this.anchorNode);
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
