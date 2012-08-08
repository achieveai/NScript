//-----------------------------------------------------------------------
// <copyright file="Screen.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Screen
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class Screen
    {
        private Screen() { }

        [IntrinsicField]
        public readonly int AvailHeight;

        [IntrinsicField]
        public readonly int AvailWidth;

        [IntrinsicField]
        public readonly int ColorDepth;

        [IntrinsicField]
        public readonly int Height;

        [IntrinsicField]
        public readonly int Width;
    }
}