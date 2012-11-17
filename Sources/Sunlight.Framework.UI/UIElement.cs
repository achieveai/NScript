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
        Element element;

        StringDictionary<Action<UIEvent>> eventRegistrationDict = new StringDictionary<Action<UIEvent>>();

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
        /// Gets or sets the css class.
        /// </summary>
        /// <value>
        /// The css class.
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
            int idx = this.CssClassIndex(className);
            if (idx >= 0)
            {
                string cssClassName = this.element.ClassName;

                cssClassName = (idx > 0 ? cssClassName.Substring(0, idx - 1) : String.Empty)
                    + (idx < cssClassName.Length - className.Length ? cssClassName.Substring(idx + cssClassName.Length + 1) : String.Empty);

                this.element.ClassName = cssClassName;
            }
        }

        /// <summary>
        /// Adds a CSS class.
        /// </summary>
        /// <param name="className"> Name of the CSS class. </param>
        public void AddClass(string className)
        {
            if (this.CssClassIndex(className) >= 0)
            {
                return;
            }

            string cssClassName = this.element.ClassName;

            if (cssClassName != null && cssClassName.Length > 0)
            {
                cssClassName = cssClassName + " " + className;
            }
            else
            {
                cssClassName = className;
            }

            this.element.ClassName = cssClassName;
        }

        /// <summary>
        /// Css class index.
        /// </summary>
        /// <param name="className"> Name of the CSS class. </param>
        /// <returns>
        /// Index of className in CssClass of this element.
        /// </returns>
        public int CssClassIndex(string className)
        {
            int idx = -1;
            string cssClassName = this.element.ClassName;

            if (cssClassName == null || cssClassName.Length < className.Length)
            {
                return -1;
            }

            do
            {
                idx = cssClassName.IndexOf(className, idx + 1);
                if (idx == 0 || (idx > 0 && cssClassName[idx - 1] == ' '))
                {
                    if (idx == cssClassName.Length - className.Length
                        || (cssClassName[idx + className.Length + 1] == ' '))
                    {
                        break;
                    }
                }

            } while (idx != -1);

            return idx;
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
