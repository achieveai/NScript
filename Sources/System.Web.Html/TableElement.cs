//-----------------------------------------------------------------------
// <copyright file="TableElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TableElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class TableElement : Element
    {
        private TableElement() { }

        [IntrinsicField]
        public readonly NodeCollection Cells;

        [IntrinsicField]
        public readonly NodeCollection Rows;

        [IntrinsicField]
        public readonly NodeCollection tBodies;

        [IntrinsicField]
        public readonly Element tFoot;

        [IntrinsicField]
        public readonly NodeCollection tHead;

        public extern void DeleteRow();

        public extern void DeleteRow(int index);

        public extern TableRowElement InsertRow();

        public extern TableRowElement InsertRow(int index);
    }
}