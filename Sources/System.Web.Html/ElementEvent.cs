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
    [IgnoreNamespace, ScriptName("Object")]
    public class ElementEvent : Event
    {
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
        /// Gets a value indicating whether the control key.
        /// </summary>
        /// <value>
        /// true if control key, false if not.
        /// </value>
        public extern bool CtrlKey
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
        public new extern Element SrcElement
        { get; }

        /// <summary>
        /// Gets target for the.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public new extern Element Target
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

        public extern Number ClientX
        { get; }

        public extern Number ClientY
        { get; }
    }
}