//-----------------------------------------------------------------------
// <copyright file="DefaultTagNameAttribute.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Default Tag name attribute reader.
    /// </summary>
    public class DefaultTagNameAttribute : AttributeBase
    {
        /// <summary>
        /// Backing field for DefaultTagName.
        /// </summary>
        private readonly string defaultTagName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTagNameAttribute"/> class.
        /// </summary>
        /// <param name="defaultTagName">Default name of the tag.</param>
        public DefaultTagNameAttribute(string defaultTagName)
        {
            this.defaultTagName = defaultTagName;
        }

        /// <summary>
        /// Gets the default name of the tag.
        /// </summary>
        /// <value>The default name of the tag.</value>
        public string DefaultTagName
        {
            get { return this.defaultTagName; }
        }
    }
}
