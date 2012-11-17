//-----------------------------------------------------------------------
// <copyright file="TableRowElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TableRowElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class TableRowElement : Element
    {
        /// <summary>
        /// Gets the table row element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern TableRowElement();

        /// <summary>
        /// Gets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public extern TableCellElement[] Cells { get; }

        /// <summary>
        /// Gets zero-based index of the row.
        /// </summary>
        /// <value>
        /// The row index.
        /// </value>
        public extern int RowIndex { get; }

        /// <summary>
        /// Gets zero-based index of the section row.
        /// </summary>
        /// <value>
        /// The section row index.
        /// </value>
        public extern int SectionRowIndex { get; }

        /// <summary>
        /// Deletes the cell.
        /// </summary>
        public extern void DeleteCell();

        /// <summary>
        /// Deletes the cell described by index.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        public extern void DeleteCell(int index);

        /// <summary>
        /// Inserts a cell.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern TableCellElement InsertCell();

        /// <summary>
        /// Inserts a cell described by index.
        /// </summary>
        /// <param name="index"> Zero-based index of the. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern TableCellElement InsertCell(int index);
    }
}