//-----------------------------------------------------------------------
// <copyright file="TouchInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TouchInfo.
    /// </summary>
    [IgnoreNamespace]
    public sealed class TouchInfo
    {
        /// <summary>
        /// Gets the client x coordinate.
        /// </summary>
        /// <value>
        /// The client x coordinate.
        /// </value>
        public extern int ClientX { get; }

        /// <summary>
        /// Gets the client y coordinate.
        /// </summary>
        /// <value>
        /// The client y coordinate.
        /// </value>
        public extern int ClientY { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public extern int Identifier { get; }

        /// <summary>
        /// Gets the page x coordinate.
        /// </summary>
        /// <value>
        /// The page x coordinate.
        /// </value>
        public extern int PageX { get; }

        /// <summary>
        /// Gets the page y coordinate.
        /// </summary>
        /// <value>
        /// The page y coordinate.
        /// </value>
        public extern int PageY { get; }

        /// <summary>
        /// Gets the screen x coordinate.
        /// </summary>
        /// <value>
        /// The screen x coordinate.
        /// </value>
        public extern int ScreenX { get; }

        /// <summary>
        /// Gets the screen y coordinate.
        /// </summary>
        /// <value>
        /// The screen y coordinate.
        /// </value>
        public extern long ScreenY { get; }

        /// <summary>
        /// Gets target for the.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public extern Element Target { get; }
    }
}