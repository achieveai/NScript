//-----------------------------------------------------------------------
// <copyright file="IFrameElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for IFrameElement
    /// </summary>
    [Extended]
    [IgnoreNamespace]
    public sealed class IFrameElement : Element
    {
        private IFrameElement() { }

        [IntrinsicField]
        public readonly Window ContentWindow;

        [IntrinsicField]
        public string FrameBorder;

        [IntrinsicField]
        public string Scrolling;

        [IntrinsicField]
        public string Src;
    }
}