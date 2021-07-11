//-----------------------------------------------------------------------
// <copyright file="AreaElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for AreaElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class AreaElement : Element
    {
        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private extern AreaElement();

        /// <summary>
        /// Gets or sets the shape.
        /// </summary>
        /// <value>
        /// The shape.
        /// </value>
        public extern string Shape
        { get; set; }

        /// <summary>
        /// Gets or sets the coords.
        /// </summary>
        /// <value>
        /// The coords.
        /// </value>
        public extern string Coords
        { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public extern string Name
        { get; set; }

        /// <summary>
        /// Gets or sets the alternate.
        /// </summary>
        /// <value>
        /// The alternate.
        /// </value>
        public extern string Alt
        { get; set; }

        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        public extern string Href
        { get; set; }

        /// <summary>
        /// Gets or sets the no href.
        /// </summary>
        /// <value>
        /// The no href.
        /// </value>
        public extern string NoHref
        { get; set; }
    }
}