//-----------------------------------------------------------------------
// <copyright file="ImageData.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Graphics
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ImageData
    /// </summary>
    [IgnoreNamespace]
    public sealed class ImageData
    {
        private extern ImageData();

        public extern NativeArray Data { get; }

        public extern int Height { get; }

        public extern int Width { get; }
    }
}