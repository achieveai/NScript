//-----------------------------------------------------------------------
// <copyright file="AttachedPropertyAttribute.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Template.Compiler.Attributes
{
    using System;
    using System.Collections.Generic;
    using NScript.PELoader.Reflection;

    /// <summary>
    /// Definition for AttachedPropertyAttribute
    /// </summary>
    public class AttachedPropertyAttribute : AttributeBase
    {
        /// <summary>
        /// Backing field for TypeReference.
        /// </summary>
        private readonly TypeReference typeReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachedPropertyAttribute"/> class.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        public AttachedPropertyAttribute(TypeReference typeReference)
        {
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Gets the type reference.
        /// </summary>
        /// <value>The type reference.</value>
        public TypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
        }
    }
}
