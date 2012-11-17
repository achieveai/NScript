//-----------------------------------------------------------------------
// <copyright file="SourceElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for SourceElement.
    /// </summary>
    [IgnoreNamespace]
    public class SourceElement : Element
    {
        /// <summary>
        /// Gets the source element.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        protected extern SourceElement();

        /// <summary>
        /// Gets or sets source for the.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public extern string Src {get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public extern string Type {get; set; }
    }
}
