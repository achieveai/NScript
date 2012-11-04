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
    [Extended]
    [IgnoreNamespace]
    public sealed class AnchorElement : Element
    {
        private AnchorElement() { }

        [IntrinsicField]
        public string Href;

        [IntrinsicField]
        public string Rel;

        [IntrinsicField]
        public string Target;

        [IntrinsicField]
        public string Download;
    }
}