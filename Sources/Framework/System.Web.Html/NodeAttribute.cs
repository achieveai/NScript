//-----------------------------------------------------------------------
// <copyright file="NodeAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for NodeAttribute.
    /// </summary>
    [IgnoreNamespace, ScriptName("Attr")]
    public sealed class NodeAttribute
    {
        /// <summary>
        /// Gets the node attribute.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        private extern NodeAttribute();

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public extern string Name
        { get; }

        /// <summary>
        /// Gets a value indicating whether the specified.
        /// </summary>
        /// <value>
        /// true if specified, false if not.
        /// </value>
        public extern bool Specified
        { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public extern string Value
        { get; set; }
    }
}