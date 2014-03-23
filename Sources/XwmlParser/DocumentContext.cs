//-----------------------------------------------------------------------
// <copyright file="DocumentContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using HtmlAgilityPack;
    using NScript.Converter;
    using NScript.JST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Definition for DocumentContext
    /// </summary>
    public class DocumentContext : IDocumentContext
    {
        /// <summary>
        /// Context for the parser.
        /// </summary>
        ParserContext parserContext;

        /// <summary>
        /// Stack of namespaces.
        /// </summary>
        private List<Dictionary<string, string>> namespaceStack = new List<Dictionary<string, string>>();

        /// <summary>
        /// The applicable CSS scopes.
        /// </summary>
        private List<CssStyleSheet> applicableCssScopes = new List<CssStyleSheet>();

        /// <summary>
        /// The document CSS scope.
        /// </summary>
        private CssStyleSheet documentCssScope;

        private string resourceName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parserContext">    Context for the parser. </param>
        public DocumentContext(ParserContext parserContext, string resourceName)
        {
            this.parserContext = parserContext;
            this.resourceName = resourceName;
        }

        /// <summary>
        /// Gets the resolver.
        /// </summary>
        /// <value>
        /// The resolver.
        /// </value>
        public IClrResolver Resolver
        { get { return this.parserContext.ClrResolver; } }

        /// <summary>
        /// Gets a context for the parser.
        /// </summary>
        /// <value>
        /// The parser context.
        /// </value>
        public ParserContext ParserContext
        { get { return this.parserContext; } }

        /// <summary>
        /// Pushes a node.
        /// </summary>
        /// <param name="node"> The node. </param>
        public void PushNode(HtmlNode node)
        {
            this.ParseNamespaces(node);
        }

        /// <summary>
        /// Pops the node.
        /// </summary>
        public void PopNode()
        {
            if (this.namespaceStack.Count > 1)
            {
                // Remove the namespace mapping that was inserted for this node.
                this.namespaceStack.RemoveAt(this.namespaceStack.Count - 1);
            }
        }

        /// <summary>
        /// Gets full name.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// The full name.
        /// </returns>
        public Tuple<string, string> GetFullName(string name)
        {
            string[] nameParts = name.Split(':');
            if (nameParts.Length > 2)
            {
                throw new ApplicationException(
                    "Invalid name");
            }
            else if (nameParts.Length == 1)
            {
                return Tuple.Create((string)null, nameParts[0]);
            }

            for (int iNamespaceMap = this.namespaceStack.Count - 1; iNamespaceMap >= 0; iNamespaceMap--)
            {
                if (this.namespaceStack[iNamespaceMap] != null)
                {
                    string ns;
                    if (this.namespaceStack[iNamespaceMap].TryGetValue(nameParts[0], out ns))
                    {
                        return Tuple.Create(ns, nameParts[1]);
                    }
                }
            }

            throw new ApplicationException(string.Format("Can't resolve name space: {0}", nameParts[0]));
        }

        /// <summary>
        /// Try get CSS class identifier.
        /// </summary>
        /// <param name="className">  Name of the class. </param>
        /// <param name="identifier"> [out] The identifier. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool TryGetCssClassIdentifier(string className, out IIdentifier identifier)
        {
            if (this.documentCssScope != null && this.documentCssScope.TryGetCssClassIdentifier(className, out identifier))
            {
                return true;
            }

            for (int iCssScope = 0; iCssScope < this.applicableCssScopes.Count; iCssScope++)
            {
                if (this.applicableCssScopes[iCssScope].TryGetCssClassIdentifier(
                    className,
                    out identifier))
                {
                    return true;
                }
            }

            return this.parserContext.TryGetCssClassIdentifier(className, out identifier);
        }

        /// <summary>
        /// Gets CSS string.
        /// </summary>
        /// <returns>
        /// The CSS string.
        /// </returns>
        public string GetCssString()
        {
            if (this.documentCssScope != null)
            {
                return this.documentCssScope.GetCssString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Adds the CSS.
        /// </summary>
        /// <exception cref="ConverterLocationException"> Thrown when a Converter Location error condition
        ///     occurs. </exception>
        /// <param name="styleNode"> The style node. </param>
        public void AddCss(HtmlNode styleNode)
        {
            if (!string.IsNullOrWhiteSpace(styleNode.InnerText))
            {
                this.documentCssScope =
                    this.documentCssScope
                    ?? new CssStyleSheet(this.parserContext, this.resourceName);

                if (styleNode.Attributes["type"] != null
                    && styleNode.Attributes["type"].Value.ToLowerInvariant() == "text/less")
                {
                    this.documentCssScope.AddLess(styleNode.InnerText);
                }
                else
                {
                    this.documentCssScope.AddCss(styleNode.InnerText);
                }

                foreach (var className in this.documentCssScope.ClassNames)
                {
                    IIdentifier tmp;
                    for (int iStyleSheet = 0; iStyleSheet < this.applicableCssScopes.Count; iStyleSheet++)
                    {
                        if (this.applicableCssScopes[iStyleSheet].TryGetCssClassIdentifier(
                            className,
                            out tmp))
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    styleNode.Line,
                                    styleNode.LinePosition),
                                string.Format(
                                    "Can't overwrite className: {0}, it's already defined in styleSheet: {1}",
                                    className,
                                    this.applicableCssScopes[iStyleSheet].ResourceName));
                        }
                    }
                }
            }
            else if (styleNode.Attributes.Contains("src"))
            {
                try
                {
                    var styleSheet = this.parserContext.GetStyleSheet(styleNode.Attributes["src"].Value);

                    if (this.documentCssScope != null)
                    {
                        foreach (var cssClassName in this.documentCssScope.ClassNames)
                        {
                            IIdentifier tmp;
                            if (styleSheet.TryGetCssClassIdentifier(cssClassName, out tmp))
                            {
                                throw new ConverterLocationException(
                                    new Location(
                                        this.resourceName,
                                        styleNode.Line,
                                        styleNode.LinePosition),
                                    string.Format(
                                        "Can't overwrite className: {0}, it's already defined in local StyleSheet",
                                        cssClassName));
                            }
                        }
                    }

                    this.applicableCssScopes.Add(styleSheet);
                }
                catch(ConverterLocationException locEx)
                {
                    throw locEx;
                }
                catch(ApplicationException ex)
                {
                    throw new ConverterLocationException(
                        new Location(
                            this.resourceName,
                            styleNode.Line,
                            styleNode.LinePosition),
                        string.Format(
                            "base StyleSheet:{0} not found.",
                            styleNode.Attributes["src"].Value));
                }
            }
            else
            {
                throw new ConverterLocationException(
                    new Location(
                        this.resourceName,
                        styleNode.Line,
                        styleNode.LinePosition),
                    "Don't know what to do with style block");
            }
        }

        /// <summary>
        /// Parse namespaces.
        /// </summary>
        /// <param name="node"> The node. </param>
        private void ParseNamespaces(HtmlNode node)
        {
            Dictionary<string, string> mapping = null;
            foreach (var attr in node.Attributes)
            {
                if (attr.OriginalName.StartsWith("xmlns:"))
                {
                    if (mapping == null)
                    {
                        mapping = new Dictionary<string, string>();
                    }

                    mapping[attr.OriginalName.Substring("xmlns:".Length)] = attr.Value;
                }
            }

            this.namespaceStack.Add(mapping);
        }
    }
}
