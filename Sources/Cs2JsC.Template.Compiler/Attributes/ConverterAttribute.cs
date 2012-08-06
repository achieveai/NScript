//-----------------------------------------------------------------------
// <copyright file="ConverterAttribute.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler.Attributes
{
    using Cs2JsC.PELoader.Reflection;

    /// <summary>
    /// Converter attribute to track convert to and from types.
    /// </summary>
    public class ConverterAttribute : AttributeBase
    {
        /// <summary>
        /// Backing field for FromType.
        /// </summary>
        private readonly TypeReferenceBase fromType;

        /// <summary>
        /// Backing field for ToType.
        /// </summary>
        private readonly TypeReferenceBase toType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterAttribute"/> class.
        /// </summary>
        /// <param name="fromType">From type.</param>
        /// <param name="toType">To   type.</param>
        public ConverterAttribute(
            TypeReferenceBase fromType,
            TypeReferenceBase toType)
        {
            this.fromType = fromType;
            this.toType = toType;
        }

        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>From type.</value>
        public TypeReferenceBase From
        {
            get
            {
                return this.fromType;
            }
        }

        /// <summary>
        /// Gets to Type.
        /// </summary>
        /// <value>To   Type.</value>
        public TypeReferenceBase To
        {
            get
            {
                return this.toType;
            }
        }
    }
}
