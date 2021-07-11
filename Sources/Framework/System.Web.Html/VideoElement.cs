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
    [IgnoreNamespace]
    public class VideoElement : MediaElement
    {
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private extern VideoElement();

        /// <summary>
        /// The height.
        /// </summary>
        public extern string Height {get; set; }

        /// <summary>
        /// The width.
        /// </summary>
        public extern string Width {get; set; }

        /// <summary>
        /// The poster.
        /// </summary>
        public extern string Poster {get; set; }

        /// <summary>
        /// Height of the video.
        /// </summary>
        public extern int VideoHeight { get; }

        /// <summary>
        /// Width of the video.
        /// </summary>
        public extern int VideoWidth { get; }
    }
}
