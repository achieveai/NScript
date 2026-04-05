//-----------------------------------------------------------------------
// <copyright file="CssClassAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Attributes
{
    using System;

    /// <summary>
    /// Marks a const string field as a CSS class name reference.
    /// The NScript compiler uses this to track CSS class names for minification.
    /// Format: [CssClass("EmbeddedResourceName:CssClassName")]
    /// </summary>
    /// <remarks>
    /// Can only be applied to const string fields. The const value must equal
    /// the CSS class name specified in the attribute. The resource name must
    /// reference an embedded CSS file loaded via @styles directive.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class CssClassAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CssClassAttribute"/> class.
        /// </summary>
        /// <param name="cssClassReference">
        /// CSS class reference in format "EmbeddedResourceName:CssClassName".
        /// </param>
        public CssClassAttribute(string cssClassReference)
        {
            CssClassReference = cssClassReference;
        }

        /// <summary>
        /// Gets the CSS class reference in "ResourceName:ClassName" format.
        /// </summary>
        public string CssClassReference { get; }
    }
}
