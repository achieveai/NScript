//-----------------------------------------------------------------------
// <copyright file="TextElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TextElement
    /// </summary>
    [IgnoreNamespace]
    public sealed class TextElement : InputElement
    {
        /// <summary>
        /// Gets the text element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        internal extern TextElement();

        /// <summary>
        /// Gets or sets the length of the maximum.
        /// </summary>
        /// <value>
        /// The length of the maximum.
        /// </value>
        public extern int MaxLength {get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the only should be read.
        /// </summary>
        /// <value>
        /// true if read only, false if not.
        /// </value>
        public extern bool ReadOnly {get; set; }
    }
}