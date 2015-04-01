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
    [IgnoreNamespace, ScriptName("HTMLIFrameElement")]
    public sealed class IFrameElement : Element
    {
        private extern IFrameElement();

        public extern Window ContentWindow
        { get; }

        public extern string FrameBorder
        { get; set; }

        public extern string Scrolling
        { get; set; }

        public extern string Src
        { get; set; }
    }
}