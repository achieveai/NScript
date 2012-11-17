//-----------------------------------------------------------------------
// <copyright file="AnchorElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for AnchorElement
    /// </summary>
    [IgnoreNamespace]
    public sealed class AnchorElement : Element
    {
        private extern AnchorElement();

        public extern string Href
        { get; set; }

        public extern string Rel
        { get; set; }

        public extern string Target
        { get; set; }

        public extern string Download
        { get; set; }
    }
}