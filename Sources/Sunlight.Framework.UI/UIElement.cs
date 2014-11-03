//-----------------------------------------------------------------------
// <copyright file="UIElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Html;
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.UI.Attributes;

    /// <summary>
    /// Definition for UIElement
    /// </summary>
    public class UIElement : ContextBindableObject
    {
        private Element element;
        private bool isHidden = false;
        private IocContainer container;
        private StringDictionary<Action<UIEvent>> eventRegistrationDict = new StringDictionary<Action<UIEvent>>();

        public UIElement(Element element)
        {
            this.element = element;
        }

        public event Action<UIEvent> OnClick
        {
            add
            {
                this.AttachEvent(EventNames.Click, value);
            }

            remove
            {
                this.DetackEvent(EventNames.Click, value);
            }
        }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        [CssName]
        public string CssClass
        {
            get
            {
                return this.element.ClassName;
            }

            set
            {
                if (this.element.ClassName != value)
                {
                    this.element.ClassName = value;
                    this.FirePropertyChanged("CssClass");
                }
            }
        }

        public IocContainer Container
        {
            get { return this.container; }
            set
            {
                if (this.container != null)
                {
                    throw new Exception("can't set container once set");
                }

                this.container = value;
            }
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        public Element Element
        {
            get { return this.element; }
        }

        /// <summary>
        /// Gets the skinned parent.
        /// </summary>
        /// <value>
        /// The skinned parent.
        /// </value>
        public UISkinableElement SkinnedParent
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public bool IsHidden
        {
            get
            {
                return this.isHidden;
            }

            set
            {
                if (this.isHidden != value)
                {
                    this.isHidden = value;
                    if (value)
                    {
                        this.element.Style.Display = "none";
                    }
                    else
                    {
                        this.element.Style.Display = "";
                    }

                    this.FirePropertyChanged("IsHidden");

                    this.FixActivation();
                }
            }
        }

        protected override bool ActivationBlocked
        {
            get
            {
                return base.ActivationBlocked || this.isHidden;
            }
        }

        /// <summary>
        /// Attach event.
        /// </summary>
        /// <param name="eventName"> Name of the event. </param>
        /// <param name="callback">  The callback. </param>
        public void AttachEvent(string eventName, Action<UIEvent> callback)
        {
            Action<UIEvent> act;
            if (!this.eventRegistrationDict.TryGetValue(eventName, out act))
            {
                this.element.Bind(
                    eventName,
                    this.EventHandler);

                act = callback;
            }
            else
            {
                act += callback;
            }

            this.eventRegistrationDict[eventName] = act;
        }

        /// <summary>
        /// Detack event.
        /// </summary>
        /// <param name="eventName"> Name of the event. </param>
        /// <param name="callback">  The callback. </param>
        public void DetackEvent(string eventName, Action<UIEvent> callback)
        {
            Action<UIEvent> act;
            if (this.eventRegistrationDict.TryGetValue(eventName, out act))
            {
                act -= callback;

                if (act == null)
                {
                    this.element.UnBind(
                        eventName,
                        this.EventHandler);

                    this.eventRegistrationDict.Remove(eventName);
                }
                else
                {
                    this.eventRegistrationDict[eventName] = act;
                }
            }
        }

        /// <summary>
        /// Removes the CSS class described by className.
        /// </summary>
        /// <param name="className"> Name of the CSS class. </param>
        public void RemoveClass(string className)
        {
            this.element.RemoveClassName(className);
        }

        /// <summary>
        /// Adds a CSS class.
        /// </summary>
        /// <param name="className"> Name of the CSS class. </param>
        public void AddClass(string className)
        {
            this.element.AddClassName(className);
        }

        /// <summary>
        /// Internal dispose.
        /// </summary>
        protected override void InternalDispose()
        {
            foreach (var kvPair in this.eventRegistrationDict)
            {
                this.element.UnBind(kvPair.Key);
            }

            this.eventRegistrationDict.Clear();

            base.InternalDispose();
        }

        /// <summary>
        /// Handler, called when the event.
        /// </summary>
        /// <param name="evt"> The event. </param>
        private void EventHandler(Element sender, ElementEvent evt)
        {
            string eventName = evt.Type;

            Action<UIEvent> callback;
            if (this.eventRegistrationDict.TryGetValue(eventName, out callback))
            {
                callback(new UIEvent(this, evt));
            }
        }
    }
}
