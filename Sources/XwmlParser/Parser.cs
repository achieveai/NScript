//-----------------------------------------------------------------------
// <copyright file="Parser.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;
    using HtmlAgilityPack;
    using XwmlParser.NodeInfos;
    using Mono.Cecil;

    public enum NodeType
    {
        ExtensibleObject,
        Property,
        Html,
        Text,
        Template,
        Factory,
        Style
    }

    /// <summary>
    /// Definition for Parser
    /// </summary>
    public class Parser
    {
        private ParserContext context;
        private HtmlNode node;
        private List<Dictionary<string, string>> namespaceStack = new List<Dictionary<string, string>>();

        public Parser(HtmlDocument htmlDoc)
        {
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

        /// <summary>
        /// Gets a full node name.
        /// </summary>
        /// <param name="nodeName"> Name of the node. </param>
        /// <returns>
        /// The full node name.
        /// </returns>
        private Tuple<string, string> GetFullNodeName(string nodeName)
        {
            string[] nameParts = nodeName.Split(':');
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

        private HtmlNode ParseNode(
            HtmlNode node)
        {
            try
            {
                this.ParseNamespaces(node);
                var nodeName = this.GetFullNodeName(node.OriginalName);

                return null;
            }
            finally
            {
                // Remove the namespace mapping that was inserted for this node.
                this.namespaceStack.RemoveAt(this.namespaceStack.Count - 1);
            }
        }

        public NodeType GetNodeType(HtmlNode node, NodeInfo parentNodeInfo)
        {
            var nodeName = this.GetFullNodeName(node.OriginalName);

            if (nodeName.Item1 == null && nodeName.Item2 == "style")
            {
                return NodeType.Style;
            }
            if (nodeName.Item1 == null)
            {
                // There are 2 possibilities,
                // 1. This is either a constructor parameter or Property
                // 2. This is a HTML node.
                List<PropertyReference> properties = null;
                if (parentNodeInfo is TypeNodeInfo
                    && (properties = this.context.Resolver.GetPropertyReference(
                            ((TypeNodeInfo)parentNodeInfo).Type,
                            nodeName.Item2)) != null)
                {
                    return NodeType.Property;
                }
                else if (nodeName.Item2 == "template")
                {
                    return NodeType.Template;
                }
                else if (nodeName.Item2 == "factory")
                {
                    return NodeType.Factory;
                }

                return NodeType.Html;
            }
            else if (parentNodeInfo is ControlNodeInfo)
            {
                // When parent is ControlNodeInfo, this really means that parent is anonymous template
                // The exception to this rule is AttachedProperty or property. But for normal Property
                // nodeName.Item1 == null will take care, so only remaining case where ControlNodeInfo
                // really is treated as ControlNodeInfo for AttachedPropertyCase.
                // For all other cases, the node can be ControlNodeInfo/PanelNodeInfo/UIElementNodeInfo/HtmlNodeInfo
            }

            return NodeType.Html;
        }
    }
}