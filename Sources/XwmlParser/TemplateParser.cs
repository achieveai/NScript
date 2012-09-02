//-----------------------------------------------------------------------
// <copyright file="TemplateParser.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using XwmlParser.NodeInfos;

    /// <summary>
    /// Definition for TemplateParser
    /// </summary>
    public class TemplateParser
    {
        /// <summary>
        /// The parent parser.
        /// </summary>
        private HtmlParser parentParser;

        /// <summary>
        /// The context.
        /// </summary>
        private DocumentContext context;

        /// <summary>
        /// The document.
        /// </summary>
        private HtmlDocument document;

        /// <summary>
        /// Type of the control.
        /// </summary>
        private TypeReference controlType;

        /// <summary>
        /// Type of the data context.
        /// </summary>
        private TypeReference dataContextType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="htmlNode">         The HTML node. </param>
        /// <param name="parentNodeInfo">   Information describing the parent node. </param>
        public TemplateParser(
            HtmlNode htmlNode,
            NodeInfo parentNodeInfo)
        {
        }

        private HtmlNode ParseNode(
            HtmlNode node,
            NodeInfo parentNodeInfo)
        {
            try
            {
                this.context.PushNode(node);
                var nodeName = this.parentParser.GetFullNodeName(node.OriginalName);
                NodeType nodeType = this.parentParser.GetNodeType(node, parentNodeInfo);
                HtmlNode rv = null;
                switch (nodeType)
                {
                    case NodeType.ExtensibleObject:
                        break;
                    case NodeType.Property:
                        break;
                    case NodeType.Object:
                        break;
                    case NodeType.ObservableObject:
                        break;
                    case NodeType.ContextBindableObject:
                        break;
                    case NodeType.UIElement:
                        break;
                    case NodeType.SkinableElement:
                        break;
                    case NodeType.Panel:
                        break;
                    case NodeType.AttachedProperty:
                        break;
                    case NodeType.Html:
                        rv = this.ParseHtmlNode(node);
                        break;
                    case NodeType.Text:
                        break;
                    case NodeType.Template:
                        break;
                    case NodeType.Skin:
                        break;
                    case NodeType.CssStyle:
                        throw new ApplicationException(
                            string.Format("can't have style tag in middle of a template"));
                    default:
                        break;
                }

                return rv;
            }
            finally
            {
                // Remove the namespace mapping that was inserted for this node.
                this.context.PopNode();
            }
        }

        private UIElementNodeInfo ParseElementNode(HtmlNode node)
        {
            return null;
        }

        private HtmlNode ParseHtmlNode(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                return this.document.CreateTextNode(((HtmlTextNode)node).Text);
            }
            else if (node.NodeType == HtmlNodeType.Element)
            {
                // TOOD: add id tracking and munjing.
                HtmlNodeInfo htmlNodeInfo = new HtmlNodeInfo(node, this.parentParser.GetFullNodeName(node.OriginalName));
                HtmlNode rv = this.document.CreateElement(node.OriginalName);
                foreach (var attr in node.Attributes)
                {
                    rv.SetAttributeValue(attr.OriginalName, attr.Value);
                }

                foreach (var childNode in node.ChildNodes)
                {
                    var tmpNode = this.ParseNode(childNode, null);
                    if (tmpNode != null)
                    {
                        rv.AppendChild(tmpNode);
                    }
                }

                return rv;
            }

            return null;
        }
    }
}
