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
    [PsudoType]
    [IgnoreNamespace]
    public sealed class ImageElement : Element
    {
        private ImageElement() { }

        [IntrinsicField]
        public string Alt;

        [IntrinsicField]
        public bool Complete;

        [IntrinsicField]
        public string Src;

        [IntrinsicField]
        public int Height;

        [IntrinsicField]
        public int Width;

        [IntrinsicField]
        public int NaturalHeight;

        [IntrinsicField]
        public int NaturalWidth;
    }
}