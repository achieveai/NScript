//-----------------------------------------------------------------------
// <copyright file="ParserContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
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
        private readonly IClrResolver clrResolver;

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
        /// List of identifiers for the known CSS class.
        /// </summary>
        Dictionary<string, IIdentifier> knownCssClassIdentifiers = new Dictionary<string, IIdentifier>();

        /// <summary>
        /// The CSS names scope.
        /// </summary>
        private readonly IdentifierScope cssNamesScope = new IdentifierScope(false);

        /// <summary>
        /// The js resolver.
        /// </summary>
        private IResolver jsResolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clrResolver"> The resolver. </param>
        public ParserContext(
            KnownTemplateTypes knownTypes,
            CodeGenerator codeGenerator,
            IClrResolver clrResolver,
            IResolver jsResolver,
            IList<string> knownCssClasses)
        {
            this.clrResolver = clrResolver;
            this.jsResolver = jsResolver;
            this.KnownTypes = knownTypes;
            this.codeGenerator = codeGenerator;
            this.codeGenerator.ParserContext = this;

            foreach (var cssClassName in knownCssClasses)
            {
                if (!knownCssClassIdentifiers.ContainsKey(cssClassName))
                {
                    knownCssClassIdentifiers[cssClassName] =
                        SimpleIdentifier.CreateScopeIdentifier(
                            this.cssNamesScope,
                            cssClassName,
                            true);
                }
            }
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
        /// Gets the CLR resolver.
        /// </summary>
        /// <value>
        /// The CLR resolver.
        /// </value>
        public IClrResolver ClrResolver
        { get { return this.clrResolver; } }

        /// <summary>
        /// Gets the js resolver.
        /// </summary>
        /// <value>
        /// The js resolver.
        /// </value>
        public IResolver JsResolver
        { get { return this.jsResolver; } }

        /// <summary>
        /// Registers the CSS class name described by cssClassName.
        /// </summary>
        /// <param name="cssClassName"> Name of the CSS class. </param>
        /// <returns>
        /// .
        /// </returns>
        public IIdentifier RegisterCssClassName(string cssClassName)
        {
            return SimpleIdentifier.CreateScopeIdentifier(
                this.cssNamesScope,
                cssClassName,
                false);
        }

        /// <summary>
        /// Try get CSS class identifier.
        /// </summary>
        /// <param name="cssClassName"> Name of the CSS class. </param>
        /// <param name="identifier">   [out] The identifier. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool TryGetCssClassIdentifier(string cssClassName, out IIdentifier identifier)
        {
            return this.knownCssClassIdentifiers.TryGetValue(cssClassName, out identifier);
        }

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

        /// <summary>
        /// Registers the HTML parser.
        /// </summary>
        /// <param name="templateResourceName"> Name of the template resource. </param>
        /// <param name="parser">               The parser. </param>
        public void RegisterHtmlParser(string templateResourceName, HtmlParser parser)
        {
            this.htmlParsers.Add(templateResourceName, parser);
        }

        /// <summary>
        /// Compress CSS names.
        /// </summary>
        internal void CompressCssNames()
        {
            this.cssNamesScope.Optimize();
        }
    }
}