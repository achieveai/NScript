//-----------------------------------------------------------------------
// <copyright file="VisualFilter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Filters
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for VisualFilter
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public class VisualFilter
    {
        internal VisualFilter() { }

        [IntrinsicField]
        public bool Enabled;
    }
}