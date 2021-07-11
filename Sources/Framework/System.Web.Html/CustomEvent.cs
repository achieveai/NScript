//-----------------------------------------------------------------------
// <copyright file="CustomEvent.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for CustomEvent.
    /// </summary>
    [IgnoreNamespace]
    public sealed class CustomEvent : ElementEvent
    {
        /// <summary>
        /// Gets the custom event.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        internal extern CustomEvent();

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public extern object Data
        { get; set; }
    }
}