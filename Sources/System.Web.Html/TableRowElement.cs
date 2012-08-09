//-----------------------------------------------------------------------
// <copyright file="TableRowElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TableRowElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class TableRowElement : Element
    {
        private TableRowElement() { }

        [IntrinsicField]
        public readonly NodeCollection Cells;

        [IntrinsicField]
        public readonly int RowIndex;

        [IntrinsicField]
        public readonly int SectionRowIndex;

        public extern void DeleteCell();

        public extern void DeleteCell(int index);

        public extern TableCellElement InsertCell();

        public extern TableCellElement InsertCell(int index);
    }
}