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
        private readonly IResolver resolver;

        /// <summary>
        /// The code generator.
        /// </summary>
        private readonly CodeGenerator codeGenerator;

        /// <summary>
        /// The document.
        /// </summary>
        private readonly HtmlAgilityPack.HtmlDocument document =
            new HtmlAgilityPack.HtmlDocument();

        /// <summary>
        /// The HTML parsers.
        /// </summary>
        Dictionary<string, HtmlParser> htmlParsers = new Dictionary<string, HtmlParser>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resolver"> The resolver. </param>
        public ParserContext(
            KnownTemplateTypes knownTypes,
            CodeGenerator codeGenerator,
            IResolver resolver)
        {
            this.resolver = resolver;
            this.KnownTypes = knownTypes;
            this.codeGenerator = codeGenerator;
            this.codeGenerator.ParserContext = this;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        public HtmlAgilityPack.HtmlDocument Document
        { get { return this.document; } }

        /// <summary>
        /// Gets or sets a list of types of the knowns.
        /// </summary>
        /// <value>
        /// A list of types of the knowns.
        /// </value>
        public KnownTemplateTypes KnownTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a context for the converter.
        /// </summary>
        /// <value>
        /// The converter context.
        /// </value>
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

        /// <summary>
        /// Gets HTML parser.
        /// </summary>
        /// <param name="templateResourceName"> Name of the template resource. </param>
        /// <returns>
        /// The HTML parser.
        /// </returns>
        internal HtmlParser GetHtmlParser(string templateResourceName)
        {
            HtmlParser rv = null;
            if (this.htmlParsers.TryGetValue(templateResourceName, out rv))
            {
                return rv;
            }

            return null;
        }
    }
}