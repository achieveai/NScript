//-----------------------------------------------------------------------
// <copyright file="TemplatePartAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Attributes
{
    using System;

    /// <summary>
    /// Definition for TemplatePartAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TemplatePartAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="partId">      The identifier of the part. </param>
        /// <param name="elementType"> The type of the element. </param>
        public TemplatePartAttribute(Type elementType, bool required = false)
        {
            this.ElementType = elementType;
            this.Required = required;
        }

        /// <summary>
        /// Gets or sets the type of the element.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public Type ElementType { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the required.
        /// </summary>
        /// <value>
        /// true if required, false if not.
        /// </value>
        public bool Required { get; private set; }
    }
}
