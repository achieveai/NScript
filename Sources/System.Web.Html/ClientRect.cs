//-----------------------------------------------------------------------
// <copyright file="ClientRect.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for ClientRect.
    /// </summary>
    [IgnoreNamespace]
    public sealed class ClientRect
    {
        /// <summary>
        /// Gets the client rectangle.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern ClientRect();

        /// <summary>
        /// Gets the bottom.
        /// </summary>
        /// <value>
        /// The bottom.
        /// </value>
        public extern double Bottom
        { get; }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public extern double Left
        { get; }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public extern double Right
        { get; }

        /// <summary>
        /// Gets the top.
        /// </summary>
        /// <value>
        /// The top.
        /// </value>
        public extern double Top
        { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public extern double? Height
        { get; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public extern double? Width
        { get; }
    }
}
