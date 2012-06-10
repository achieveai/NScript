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
    [Imported]
    [IgnoreNamespace]
    public sealed class ImageData
    {
        private ImageData() { }

        [IntrinsicField]
        public readonly NativeArray Data;

        [IntrinsicField]
        public readonly int Height;

        [IntrinsicField]
        public readonly int Width;
    }
}