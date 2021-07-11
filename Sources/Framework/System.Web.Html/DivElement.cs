//-----------------------------------------------------------------------
// <copyright file="DivElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for DivElement.
    /// </summary>
    [IgnoreNamespace, ScriptName("HTMLDivElement")]
    public sealed class DivElement : Element
    {
        /// <summary>
        /// Gets the div element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern DivElement();

        /// <summary>
        /// Gets or sets the align.
        /// </summary>
        /// <value>
        /// The align.
        /// </value>
        public extern string Align
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the no wrap.
        /// </summary>
        /// <value>
        /// true if no wrap, false if not.
        /// </value>
        public extern bool NoWrap
        { get; set; }
    }
}