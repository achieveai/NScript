//-----------------------------------------------------------------------
// <copyright file="TextAreaElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TextAreaElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class TextAreaElement : InputElement
    {
        /// <summary>
        /// Gets the text area element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern TextAreaElement();

        /// <summary>
        /// Gets or sets the cols.
        /// </summary>
        /// <value>
        /// The cols.
        /// </value>
        public extern int Cols {get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the only should be read.
        /// </summary>
        /// <value>
        /// true if read only, false if not.
        /// </value>
        public extern bool ReadOnly {get; set; }

        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public extern int Rows {get; set; }
    }
}