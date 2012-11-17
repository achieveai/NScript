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
    [IgnoreNamespace]
    public sealed class TableSectionElement : Element
    {
        private extern TableSectionElement();

        public extern TableRowElement[] Rows { get; }
    }
}