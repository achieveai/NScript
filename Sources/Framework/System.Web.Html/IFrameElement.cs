//-----------------------------------------------------------------------
// <copyright file="IFrameElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
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

        /// <summary>
        /// The frame's name, the target a link or form can name.
        /// </summary>
        public extern string Name
        { get; set; }

        /// <summary>
        /// Raised when the frame's document has loaded.
        /// </summary>
        public extern event Action<IFrameElement, ElementEvent> OnLoad;
    }
}