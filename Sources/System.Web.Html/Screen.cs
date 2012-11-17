//-----------------------------------------------------------------------
// <copyright file="Screen.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Screen.
    /// </summary>
    public sealed class Screen
    {
        /// <summary>
        /// Gets the screen.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern Screen();

        /// <summary>
        /// Gets the height of the avail.
        /// </summary>
        /// <value>
        /// The height of the avail.
        /// </value>
        public extern int AvailHeight
        { get; }

        /// <summary>
        /// Gets the width of the avail.
        /// </summary>
        /// <value>
        /// The width of the avail.
        /// </value>
        public extern int AvailWidth
        { get; }

        /// <summary>
        /// Gets the depth of the color.
        /// </summary>
        /// <value>
        /// The depth of the color.
        /// </value>
        public extern int ColorDepth
        { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public extern int Height
        { get; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public extern int Width
        { get; }
    }
}