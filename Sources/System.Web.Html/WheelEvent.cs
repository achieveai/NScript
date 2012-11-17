//-----------------------------------------------------------------------
// <copyright file="WheelEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for WheelEvent.
    /// </summary>
    [IgnoreNamespace]
    public class WheelEvent : ElementEvent
    {
        /// <summary>
        /// Gets the wheel event.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern WheelEvent();

        /// <summary>
        /// Gets the wheel delta.
        /// </summary>
        /// <value>
        /// The wheel delta.
        /// </value>
        public extern int WheelDelta { get; }

        /// <summary>
        /// Gets the wheel delta x coordinate.
        /// </summary>
        /// <value>
        /// The wheel delta x coordinate.
        /// </value>
        public extern int WheelDeltaX { get; }

        /// <summary>
        /// Gets the wheel delta y coordinate.
        /// </summary>
        /// <value>
        /// The wheel delta y coordinate.
        /// </value>
        public extern int WheelDeltaY { get; }
    }
}
