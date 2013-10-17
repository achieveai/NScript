//-----------------------------------------------------------------------
// <copyright file="DocumentContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using HtmlAgilityPack;
    using NScript.JST;
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
        /// List of names of the class.
        /// </summary>
        Dictionary<string, IIdentifier> classNames = new Dictionary<string, IIdentifier>();

        /// <summary>
        /// The CSS rules.
        /// </summary>
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
        /// Adds the CSS rules.
        /// </summary>
        /// <param name="rules"> The rules. </param>
        public void AddCssRules(List<CssParser.CssRule> rules)
        {
            this.cssRules.AddRange(rules);

            for (int iRule = 0; iRule < rules.Count; iRule++)
            {
                var selectors = rules[iRule].Selectors;
                for (int iSelector = 0; iSelector < selectors.Count; iSelector++)
                {
                    var selector = selectors[iSelector];
                    CssParser.CssClass classSelector = selector as CssParser.CssClass;
                    if (classSelector == null)
                    {
                        throw new ApplicationException(
                            "Can't use any other selector than Class name selector in Css");
                    }
                    if (!this.classNames.ContainsKey(classSelector.ClassName))
                    {
                        this.classNames[classSelector.ClassName] =
                            parserContext.RegisterCssClassName(classSelector.ClassName);
                    }

                    this.classNames[classSelector.ClassName].AddUsage(null);
                }
            }
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
            if (!this.classNames.TryGetValue(className, out identifier))
            {
                return this.parserContext.TryGetCssClassIdentifier(className, out identifier);
            }

            return true;
        }

        /// <summary>
        /// Gets CSS string.
        /// </summary>
        /// <returns>
        /// The CSS string.
        /// </returns>
        public string GetCssString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cssRule in this.cssRules)
            {
                bool first = true;
                foreach (var selector in cssRule.Selectors)
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }

                    CssParser.CssClass classSelector = (CssParser.CssClass)selector;
                    sb.Append('.');
                    IIdentifier identifier;
                    this.TryGetCssClassIdentifier(classSelector.ClassName, out identifier);
                    sb.Append(identifier.GetName());
                    first = false;
                }

                sb.Append('{');
                foreach (var prop in cssRule.Properties)
                {
                    sb.Append(prop.PropertyName);
                    sb.Append(':');
                    sb.Append(prop.PropertyArgs);
                    sb.Append(';');
                }

                sb.Append('}');
            }

            return sb.ToString();
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
