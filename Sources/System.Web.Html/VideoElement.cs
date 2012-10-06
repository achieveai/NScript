//-----------------------------------------------------------------------
// <copyright file="VideoElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for VideoElement.
    /// </summary>
    [IgnoreNamespace, PsudoType]
    public class VideoElement : MediaElement
    {
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private VideoElement() { }

        /// <summary>
        /// The height.
        /// </summary>
        [IntrinsicField]
        public string Height;

        /// <summary>
        /// The width.
        /// </summary>
        [IntrinsicField]
        public string Width;

        /// <summary>
        /// The poster.
        /// </summary>
        [IntrinsicField]
        public string Poster;

        /// <summary>
        /// Height of the video.
        /// </summary>
        [IntrinsicField]
        public readonly int VideoHeight;

        /// <summary>
        /// Width of the video.
        /// </summary>
        [IntrinsicField]
        public readonly int VideoWidth;
    }
}
