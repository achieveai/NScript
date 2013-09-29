//-----------------------------------------------------------------------
// <copyright file="DocumentContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for DocumentContext
    /// </summary>
    public class DocumentContext : IDocumentContext
    {
        /// <summary>
        /// Context for the parser.
        /// </summary>
        ParserContext parserContext;

        List<CssParser.CssRule> cssRules = new List<CssParser.CssRule>();

        /// <summary>
        /// Stack of namespaces.
        /// </summary>
        private List<Dictionary<string, string>> namespaceStack = new List<Dictionary<string, string>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parserContext">    Context for the parser. </param>
        public DocumentContext(ParserContext parserContext)
        {
            this.parserContext = parserContext;
        }

        /// <summary>
        /// Gets the resolver.
        /// </summary>
        /// <value>
        /// The resolver.
        /// </value>
        public IResolver Resolver
        { get { return this.parserContext.Resolver; } }

        /// <summary>
        /// Gets a context for the parser.
        /// </summary>
        /// <value>
        /// The parser context.
        /// </value>
        public ParserContext ParserContext
        { get { return this.parserContext; } }

        /// <summary>
        /// Adds the CSS rules.
        /// </summary>
        /// <param name="rules"> The rules. </param>
        public void AddCssRules(List<CssParser.CssRule> rules)
        {
            this.cssRules.AddRange(rules);
        }

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
            // Remove the namespace mapping that was inserted for this node.
            this.namespaceStack.RemoveAt(this.namespaceStack.Count - 1);
        }

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
