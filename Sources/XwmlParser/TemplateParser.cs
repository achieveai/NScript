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
    using XwmlParser.AttributeInfos;
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

        private NodeInfo ParseNode(
            HtmlNode node,
            NodeInfo parentNodeInfo)
        {
            try
            {
                this.context.PushNode(node);
                var nodeName = this.parentParser.GetFullName(node.OriginalName);
                NodeType nodeType = this.parentParser.GetNodeType(node, parentNodeInfo);
                NodeInfo rv = null;
                switch (nodeType)
                {
                    case NodeType.ExtensibleObject:
                        rv = this.ParseObjectNode(node);
                        break;
                    case NodeType.Object:
                        rv = this.ParseObjectNode(node);
                        break;
                    case NodeType.ObservableObject:
                        rv = this.ParseObservableObject(node);
                        break;
                    case NodeType.ContextBindableObject:
                        rv = this.ParseContextBindableObject(node);
                        break;
                    case NodeType.UIElement:
                        rv = this.ParseElementNode(node);
                        break;
                    case NodeType.SkinableElement:
                        rv = this.ParseSkinableElementNode(node);
                        break;
                    case NodeType.Panel:
                        rv = this.ParsePanelElementNode(node);
                        break;
                    case NodeType.Property:
                        break;
                    case NodeType.AttachedProperty:
                        break;
                    case NodeType.Html:
                        rv = this.ParseHtmlNode(node);
                        break;
                    case NodeType.Text:
                        rv = this.ParseHtmlNode(node);
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

        /// <summary>
        /// Parse html node.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// HtmlNodeInfo.
        /// </returns>
        private HtmlNodeInfo ParseHtmlNode(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                return new HtmlNodeInfo(
                    node,
                    this.parentParser.GetFullName(node.OriginalName))
                    {
                        GeneratedNode = this.document.CreateTextNode(((HtmlTextNode)node).Text)
                    };
            }
            else if (node.NodeType == HtmlNodeType.Element)
            {
                // TOOD: add id tracking and munging.
                HtmlNodeInfo htmlNodeInfo = new HtmlNodeInfo(node, this.parentParser.GetFullName(node.OriginalName));
                HtmlNode rv = this.document.CreateElement(node.OriginalName);
                foreach (var attr in node.Attributes)
                {
                    rv.SetAttributeValue(attr.OriginalName, attr.Value);
                }

                foreach (var childNode in node.ChildNodes)
                {
                    var tmpNode = this.ParseNode(childNode, null);

                    IHtmlNodeGenerator generatorNode = tmpNode as IHtmlNodeGenerator;

                    if (generatorNode != null)
                    {
                        rv.AppendChild(generatorNode.GeneratedNode);
                    }
                }

                return new HtmlNodeInfo(
                    node,
                    this.parentParser.GetFullName(node.OriginalName))
                    { GeneratedNode = rv };
            }

            return null;
        }

        /// <summary>
        /// Parse attributes.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <exception cref="ApplicationException">    Thrown when an application error condition
        ///     occurs. </exception>
        /// <param name="node">           The node. </param>
        /// <param name="parentNodeType"> Type of the parent node. </param>
        /// <returns>
        /// A list of.
        /// </returns>
        private IList<AttributeInfo> ParseAttributes(
            HtmlNode node,
            TypeReference parentNodeType)
        {
            List<AttributeInfo> rv = new List<AttributeInfo>();
            foreach (var attribute in node.Attributes)
            {
                var nameInfo = this.parentParser.GetFullName(attribute.OriginalName);

                if (nameInfo.Item1 != null)
                {
                    throw new NotImplementedException();
                }

                var property = this.context.ParserContext.Resolver.GetPropertyReference(
                    parentNodeType,
                    nameInfo.Item2);

                if (property != null)
                {
                    throw new NotImplementedException();
                }

                if (!this.IsAllowedAttributeToSet(nameInfo.Item2))
                {
                    throw new ApplicationException();
                }

                rv.Add(
                    new HtmlAttributeInfo(attribute)
                    {
                        GeneratedAttribute = this.document.CreateAttribute(nameInfo.Item2, attribute.Value)
                    });
            }

            return rv;
        }

        private bool IsAllowedAttributeToSet(string attributeName)
        {
            switch (attributeName)
            {
                case "id":
                case "style":
                case "type":
                    return false;
                default:
                    return true;
            }
        }

        private TypeNodeInfo ParseObjectNode(HtmlNode node)
        {
            var tuple = this.GetTypeReference(node);
            TypeNodeInfo rv = new TypeNodeInfo(
                tuple.Item1,
                node,
                tuple.Item2);

            this.ParseObjectNode(node, rv);

            return rv;
        }

        private void ParseObjectNode(HtmlNode node, TypeNodeInfo typeNodeInfo)
        {
        }

        private TypeNodeInfo ParseObservableObject(HtmlNode node)
        {
            var tuple = this.GetTypeReference(node);
            TypeNodeInfo rv = new TypeNodeInfo(
                tuple.Item1,
                node,
                tuple.Item2);

            this.ParseObservableObject(node, rv);

            return rv;
        }

        private void ParseObservableObject(HtmlNode node, TypeNodeInfo typeNodeInfo)
        {
            this.ParseObjectNode(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        private ContextBindableNodeInfo ParseContextBindableObject(HtmlNode node)
        {
            var tuple = this.GetTypeReference(node);
            ContextBindableNodeInfo rv = new ContextBindableNodeInfo(
                tuple.Item1,
                node,
                tuple.Item2);

            this.ParseContextBindableObject(node, rv);

            return rv;
        }

        private void ParseContextBindableObject(HtmlNode node, ContextBindableNodeInfo typeNodeInfo)
        {
            this.ParseObservableObject(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse element node.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// UIElementNodeInfo for given node.
        /// </returns>
        private UIElementNodeInfo ParseElementNode(HtmlNode node)
        {
            var tuple = this.GetTypeReference(node);
            UIElementNodeInfo rv = new UIElementNodeInfo(
                tuple.Item1,
                node,
                tuple.Item2);

            this.ParseElementNode(node, rv);

            return rv;
        }

        private void ParseElementNode(HtmlNode node, UIElementNodeInfo typeNodeInfo)
        {
            this.ParseContextBindableObject(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        private NodeInfo ParsePanelElementNode(HtmlNode node)
        {
            var tuple = this.GetTypeReference(node);
            PanelNodeInfo rv = new PanelNodeInfo(
                tuple.Item1,
                node,
                tuple.Item2);

            this.ParsePanelElementNode(node, rv);

            return rv;
        }

        private void ParsePanelElementNode(HtmlNode node, PanelNodeInfo typeNodeInfo)
        {
            this.ParseElementNode(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        private NodeInfo ParseSkinableElementNode(HtmlNode node)
        {
            var tuple = this.GetTypeReference(node);
            SkinnableNodeInfo rv = new SkinnableNodeInfo(
                tuple.Item1,
                node,
                tuple.Item2);

            this.ParseSkinableElementNode(node, rv);

            return rv;
        }

        private void ParseSkinableElementNode(HtmlNode node, SkinnableNodeInfo typeNodeInfo)
        {
            this.ParseElementNode(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        private Tuple<TypeReference, Tuple<string, string>> GetTypeReference(HtmlNode node)
        {
            var fullNameTuple = this.parentParser.GetFullName(node.Name);
            var fullName = fullNameTuple.Item1 + "." + fullNameTuple.Item2;

            return Tuple.Create(
                this.context.ParserContext.Resolver.GetTypeReference(fullName),
                fullNameTuple);
        }
    }
}
