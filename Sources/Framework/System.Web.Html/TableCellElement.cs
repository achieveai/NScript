//-----------------------------------------------------------------------
// <copyright file="TableCellElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TableCellElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class TableCellElement : Element
    {
        /// <summary>
        /// Gets the table cell element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern TableCellElement();

        /// <summary>
        /// Gets or sets the col span.
        /// </summary>
        /// <value>
        /// The col span.
        /// </value>
        public extern int ColSpan {get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the no wrap.
        /// </summary>
        /// <value>
        /// true if no wrap, false if not.
        /// </value>
        public extern bool NoWrap {get; set; }

        /// <summary>
        /// Gets or sets the row span.
        /// </summary>
        /// <value>
        /// The row span.
        /// </value>
        public extern int RowSpan {get; set; }
    }
}