//-----------------------------------------------------------------------
// <copyright file="AreaElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for AreaElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class AreaElement : Element
    {
        private AreaElement() { }

        [IntrinsicField]
        public string Shape;

        [IntrinsicField]
        public string Coords;

        [IntrinsicField]
        public string Name;

        [IntrinsicField]
        public string Alt;

        [IntrinsicField]
        public string Href;

        [IntrinsicField]
        public string NoHref;
    }
}