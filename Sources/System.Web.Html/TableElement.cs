//-----------------------------------------------------------------------
// <copyright file="TableElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TableElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class TableElement : Element
    {
        /// <summary>
        /// Gets the table element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern TableElement();

        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public extern NodeCollection Cells { get; }

        /// <summary>
        /// Gets the rows.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public extern NodeCollection Rows { get; }

        /// <summary>
        /// Gets the bodies.
        /// </summary>
        /// <value>
        /// The t bodies.
        /// </value>
        public extern NodeCollection tBodies { get; }

        /// <summary>
        /// Gets the foot.
        /// </summary>
        /// <value>
        /// The t foot.
        /// </value>
        public extern Element tFoot { get; }

        /// <summary>
        /// Gets the head.
        /// </summary>
        /// <value>
        /// The t head.
        /// </value>
        public extern NodeCollection tHead { get; }

        /// <summary>
        /// Deletes the row.
        /// </summary>
        public extern void DeleteRow();

        /// <summary>
        /// Deletes the row described by index.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        public extern void DeleteRow(int index);

        /// <summary>
        /// Inserts a row.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern TableRowElement InsertRow();

        /// <summary>
        /// Inserts a row described by index.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern TableRowElement InsertRow(int index);
    }
}