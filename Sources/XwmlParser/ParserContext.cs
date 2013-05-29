//-----------------------------------------------------------------------
// <copyright file="ParserContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using NScript.Converter;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ParserContext
    /// </summary>
    public class ParserContext
    {
        /// <summary>
        /// The resolver.
        /// </summary>
        IResolver resolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resolver"> The resolver. </param>
        public ParserContext(
            IResolver resolver)
        {
        }

        /// <summary>
        /// Gets or sets a list of types of the attributes.
        /// </summary>
        /// <value>
        /// A list of types of the attributes.
        /// </value>
        public KnownAttributeTypes AttributeTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of types of the knowns.
        /// </summary>
        /// <value>
        /// A list of types of the knowns.
        /// </value>
        public KnownTemplateTypes KnownTypes
        {
            get;
            set;
        }

        public ConverterContext ConverterContext
        { get; set; }

        /// <summary>
        /// Gets the resolver.
        /// </summary>
        /// <value>
        /// The resolver.
        /// </value>
        public IResolver Resolver
        { get { return this.resolver; } }
    }
}