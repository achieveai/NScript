//-----------------------------------------------------------------------
// <copyright file="ImageElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ImageElement
    /// </summary>
    [IgnoreNamespace]
    public sealed class ImageElement : Element
    {
        private extern ImageElement();

        public extern bool Complete
        { get; }

        public extern string Alt
        { get; set; }

        public extern string Src
        { get; set; }

        public extern int Height
        { get; set; }

        public extern int Width
        { get; set; }

        public extern int? NaturalHeight
        { get; }

        public extern int? NaturalWidth
        { get; }

        public extern event Action<ImageElement, ElementEvent> OnLoad;

        public extern event Action<ImageElement, ElementEvent> OnError;

        public extern event Action<ImageElement, ElementEvent> OnAbort;
    }
}