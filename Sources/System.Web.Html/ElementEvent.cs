//-----------------------------------------------------------------------
// <copyright file="ElementEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ElementEvent.
    /// </summary>
    [IgnoreNamespace]
    public class ElementEvent
    {
        /// <summary>
        /// Gets the element event.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        internal extern ElementEvent();

        /// <summary>
        /// Gets a value indicating whether the alternate key.
        /// </summary>
        /// <value>
        /// true if alternate key, false if not.
        /// </value>
        public extern bool AltKey
        { get; }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <value>
        /// The button.
        /// </value>
        public extern int Button
        { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the cancel bubble.
        /// </summary>
        /// <value>
        /// true if we cancel bubble, false if not.
        /// </value>
        public extern bool CancelBubble
        { get; set; }

        /// <summary>
        /// Gets a value indicating whether the control key.
        /// </summary>
        /// <value>
        /// true if control key, false if not.
        /// </value>
        public extern bool CtrlKey
        { get; }

        /// <summary>
        /// Gets the current target.
        /// </summary>
        /// <value>
        /// The current target.
        /// </value>
        public extern Element CurrentTarget
        { get; }

        /// <summary>
        /// Gets the data transfer.
        /// </summary>
        /// <value>
        /// The data transfer.
        /// </value>
        public extern DataTransfer DataTransfer
        { get; }

        /// <summary>
        /// Gets the detail.
        /// </summary>
        /// <value>
        /// The detail.
        /// </value>
        public extern string Detail
        { get; }

        /// <summary>
        /// Gets from element.
        /// </summary>
        /// <value>
        /// from element.
        /// </value>
        public extern Element FromElement
        { get; }

        /// <summary>
        /// Gets the key code.
        /// </summary>
        /// <value>
        /// The key code.
        /// </value>
        public extern int KeyCode
        { get; }

        /// <summary>
        /// Gets a value indicating whether the meta key.
        /// </summary>
        /// <value>
        /// true if meta key, false if not.
        /// </value>
        public extern bool MetaKey
        { get; }

        /// <summary>
        /// Gets the offset x coordinate.
        /// </summary>
        /// <value>
        /// The offset x coordinate.
        /// </value>
        public extern int OffsetX
        { get; }

        /// <summary>
        /// Gets the offset y coordinate.
        /// </summary>
        /// <value>
        /// The offset y coordinate.
        /// </value>
        public extern int OffsetY
        { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the return value.
        /// </summary>
        /// <value>
        /// true if return value, false if not.
        /// </value>
        public extern bool ReturnValue
        { get; set; }

        /// <summary>
        /// Gets a value indicating whether the shift key.
        /// </summary>
        /// <value>
        /// true if shift key, false if not.
        /// </value>
        public extern bool ShiftKey
        { get; }

        /// <summary>
        /// Gets source element.
        /// </summary>
        /// <value>
        /// The source element.
        /// </value>
        public extern Element SrcElement
        { get; }

        /// <summary>
        /// Gets target for the.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public extern Element Target
        { get; }

        /// <summary>
        /// Gets the Date/Time of the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        public extern DateTime TimeStamp
        { get; }

        /// <summary>
        /// Gets to element.
        /// </summary>
        /// <value>
        /// to element.
        /// </value>
        public extern Element ToElement
        { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public extern string Type
        { get; }

        /// <summary>
        /// Prevent default.
        /// </summary>
        public extern void PreventDefault();

        /// <summary>
        /// Stops an immediate propagation.
        /// </summary>
        public extern void StopImmediatePropagation();

        /// <summary>
        /// Stops a propagation.
        /// </summary>
        public extern void StopPropagation();
    }
}