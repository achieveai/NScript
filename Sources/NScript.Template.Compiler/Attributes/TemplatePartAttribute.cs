//-----------------------------------------------------------------------
// <copyright file="TemplatePartAttribute.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using NScript.PELoader.Reflection;
namespace NScript.Template.Compiler.Attributes
{
    /// <summary>
    /// Template part attribute to track the template parts.
    /// </summary>
    public class TemplatePartAttribute : AttributeBase
    {
        /// <summary>
        /// Backing field for required.
        /// </summary>
        private readonly bool required;

        /// <summary>
        /// Backing field for typeReference.
        /// </summary>
        private readonly TypeReference typeReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePartAttribute"/> class.
        /// </summary>
        /// <param name="typeReference">The type info.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        public TemplatePartAttribute(
            TypeReference typeReference,
            bool required)
        {
            this.required = required;
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TemplatePartAttribute"/> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        public bool Required
        {
            get
            {
                return this.required;
            }
        }

        /// <summary>
        /// Gets the type info.
        /// </summary>
        /// <value>The type info.</value>
        public TypeReference Type
        {
            get
            {
                return this.typeReference;
            }
        }
    }
}
