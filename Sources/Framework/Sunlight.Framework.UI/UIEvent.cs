//-----------------------------------------------------------------------
// <copyright file="UIEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using System;
    using System.Web.Html;

    /// <summary>
    /// Definition for UIEvent
    /// </summary>
    public class UIEvent
    {
        /// <summary>   The inner event. </summary>
        private ElementEvent innerEvent;

        /// <summary>   The element. </summary>
        private UIElement uiElement;

        /// <summary>   Initializes a new instance of the UIEvent class. </summary>
        /// <param name="element">  The element. </param>
        /// <param name="evt">      The event. </param>
        public UIEvent(UIElement element, ElementEvent evt)
        {
            this.innerEvent = evt;
            this.uiElement = element;
        }

        /// <summary>   Gets the element event. </summary>
        /// <value> The element event. </value>
        public ElementEvent ElementEvent
        {
            get
            {
                return this.innerEvent;
            }
        }

        /// <summary>   Gets the element. </summary>
        /// <value> The user interface element. </value>
        public UIElement UIElement
        {
            get
            {
                return this.uiElement;
            }
        }
    }
}
