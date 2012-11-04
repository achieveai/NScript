//-----------------------------------------------------------------------
// <copyright file="TableSectionElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TableSectionElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class TableSectionElement : Element
    {
        private TableSectionElement() { }

        [IntrinsicField]
        public readonly NodeCollection Rows;
    }
}