//-----------------------------------------------------------------------
// <copyright file="MapElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for MapElement.
    /// </summary>
    [IgnoreNamespace]
    public sealed class MapElement : Element
    {
        /// <summary>
        /// Gets the map element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern MapElement();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public extern string Name
        { get; set; }
    }
}