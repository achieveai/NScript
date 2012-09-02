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
    public class DocumentContext
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
        /// Constructor.
        /// </summary>
        /// <param name="parserContext">    Context for the parser. </param>
        public DocumentContext(ParserContext parserContext)
        {
            this.parserContext = parserContext;
        }

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
            // Remove the namespace mapping that was inserted for this node.
            this.namespaceStack.RemoveAt(this.namespaceStack.Count - 1);
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
