//-----------------------------------------------------------------------
// <copyright file="TouchEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TouchEvent.
    /// </summary>
    [IgnoreNamespace]
    public sealed class TouchEvent : ElementEvent
    {
        /// <summary>
        /// Gets the touch event.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern TouchEvent();

        /// <summary>
        /// Gets the touches.
        /// </summary>
        /// <value>
        /// The touches.
        /// </value>
        public extern TouchInfo[] Touches { get; }

        /// <summary>
        /// Gets target touches.
        /// </summary>
        /// <value>
        /// The target touches.
        /// </value>
        public extern TouchInfo[] TargetTouches { get; }

        /// <summary>
        /// Gets the changed touches.
        /// </summary>
        /// <value>
        /// The changed touches.
        /// </value>
        public extern TouchInfo[] ChangedTouches { get; }
    }
}