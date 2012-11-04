//-----------------------------------------------------------------------
// <copyright file="TableCellElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TableCellElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class TableCellElement : Element
    {
        private TableCellElement() { }

        [IntrinsicField]
        public int ColSpan;

        [IntrinsicField]
        public bool NoWrap;

        [IntrinsicField]
        public int RowSpan;
    }
}