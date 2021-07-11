//-----------------------------------------------------------------------
// <copyright file="GestureEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for GestureEvent.
    /// </summary>
    [IgnoreNamespace, ScriptName("Object")]
    public sealed class GestureEvent : ElementEvent
    {
        /// <summary>
        /// Gets the gesture event.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern GestureEvent();

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public extern double Rotation
        { get; }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public extern double Scale
        { get; }
    }
}