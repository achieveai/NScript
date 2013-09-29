//-----------------------------------------------------------------------
// <copyright file="CodeGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CodeGenerator.
    /// </summary>
    public class CodeGenerator
    {
        /// <summary>
        /// Manager for scope.
        /// </summary>
        RuntimeScopeManager scopeManager;

        /// <summary>
        /// Context for the parser.
        /// </summary>
        ParserContext parserContext;

        /// <summary>
        /// List of identifiers for the template getters.
        /// </summary>
        Dictionary<TemplateParser, IIdentifier> templateGetterIds =
            new Dictionary<TemplateParser, IIdentifier>();

        /// <summary>
        /// Identifier for the templates by.
        /// </summary>
        Dictionary<string, TemplateParser> templatesById =
            new Dictionary<string, TemplateParser>();

        /// <summary>
        /// The generated code.
        /// </summary>
        List<Statement> generatedCode = new List<Statement>();

        /// <summary>
        /// The templates to parser.
        /// </summary>
        Queue<TemplateParser> templatesToParse = new Queue<TemplateParser>();

        /// <summary>
        /// The resource map.
        /// </summary>
        Dictionary<string, EmbeddedResource> resourceMap =
            new Dictionary<string, EmbeddedResource>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="scopeManager"> Manager for scope. </param>
        public CodeGenerator(RuntimeScopeManager scopeManager)
        {
            this.scopeManager = scopeManager;
            foreach (var module in this.scopeManager.Context.ClrContext.Modules)
            {
                foreach (var resource in module.Resources)
                {
                    EmbeddedResource embeddedResource = resource as EmbeddedResource;
                    if (embeddedResource != null
                        && (resource.Name.EndsWith(".html") || resource.Name.EndsWith(".htm")))
                    {
                        resourceMap.Add(resource.Name, embeddedResource);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a context for the parser.
        /// </summary>
        /// <value>
        /// The parser context.
        /// </value>
        public ParserContext ParserContext
        {
            get { return this.parserContext; }

            set { this.parserContext = value; }
        }

        /// <summary>
        /// Gets template getter identifier.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="templateParserResourceId"> Identifier for the template parser resource. </param>
        /// <returns>
        /// The template getter identifier.
        /// </returns>
        public TemplateParser GetTemplateParser(string templateParserResourceId)
        {
            TemplateParser rv;
            if (this.templatesById.TryGetValue(templateParserResourceId, out rv))
            {
                return rv;
            }

            string[] templateNameSplits = templateParserResourceId.Split(':');
            HtmlParser htmlParser = this.parserContext.GetHtmlParser(templateNameSplits[0]);

            if (htmlParser == null)
            {
                EmbeddedResource resource;
                if (this.resourceMap.TryGetValue(templateNameSplits[0], out resource))
                {
                    var document = new HtmlAgilityPack.HtmlDocument();
                    using (System.IO.Stream stream = resource.GetResourceStream())
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        document.Load(reader);
                    }

                    htmlParser = new HtmlParser(
                        templateNameSplits[0],
                        document,
                        this.parserContext);
                }

                if (htmlParser == null)
                {
                    throw new ApplicationException(
                        string.Format("Can't find template:'{0}'", templateParserResourceId));
                }
            }

            TemplateParser templateParser = htmlParser.GetTemplateParser(templateNameSplits.Length == 2 ? templateNameSplits[1] : null);
            this.templatesToParse.Enqueue(templateParser);

            return templateParser;
        }

        /// <summary>
        /// Gets template getter identifier.
        /// </summary>
        /// <param name="templateParserResourceId"> Identifier for the template parser resource. </param>
        /// <returns>
        /// The template getter identifier.
        /// </returns>
        public IIdentifier GetTemplateGetterIdentifier(string templateParserResourceId)
        {
            return this.GetGetterMethodIdentifier(
                this.GetTemplateParser(
                    templateParserResourceId));
        }

        /// <summary>
        /// Gets getter method identifier.
        /// </summary>
        /// <param name="templateParser"> The template parser. </param>
        /// <returns>
        /// The getter method identifier.
        /// </returns>
        public IIdentifier GetGetterMethodIdentifier(TemplateParser templateParser)
        {
            IIdentifier rv;
            if (this.templateGetterIds.TryGetValue(templateParser, out rv))
            {
                return rv;
            }

            string templateName = templateParser.HtmlParser.ResourceName;

            if (templateName.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase))
            {
                templateName = templateName.Substring(0, templateName.Length - 5);
            }
            else if (templateName.EndsWith(".htm", StringComparison.InvariantCultureIgnoreCase))
            {
                templateName = templateName.Substring(0, templateName.Length - 4);
            }

            if (templateName.LastIndexOf('.') != -1)
            {
                templateName = templateName.Substring(templateName.LastIndexOf('.') + 1);
            }

            if (templateParser.TemplateName != null)
            {
                templateName += "_" + templateParser.TemplateName;
            }
            else
            {
                templateName += "_private";
            }

            rv = SimpleIdentifier.CreateScopeIdentifier(
                this.scopeManager.Scope,
                templateName,
                false);
            this.templateGetterIds.Add(templateParser, rv);

            return rv;
        }

        /// <summary>
        /// Iterate parsing.
        /// </summary>
        public void IterateParsing()
        {
            while (this.templatesToParse.Count > 0)
            {
                TemplateParser templateParser =
                    this.templatesToParse.Dequeue();

                templateParser.Parse();
            }
        }
    }
}
