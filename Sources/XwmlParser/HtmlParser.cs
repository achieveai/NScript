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
    using NScript.Utils;
    using NScript.Converter;

    public enum NodeType
    {
        Head,
        Object,
        ObservableObject,
        ExtensibleObject,
        ContextBindableObject,
        UIElement,
        SkinableElement,
        Panel,
        AttachedProperty,
        Property,
        Html,
        Text,
        Template,
        Skin,
        CssStyle
    }

    /// <summary>
    /// Definition for Parser
    /// </summary>
    public class HtmlParser
    {
        /// <summary>
        /// Context for the document.
        /// </summary>
        private readonly DocumentContext documentContext;

        /// <summary>
        /// The context.
        /// </summary>
        private readonly ParserContext context;

        /// <summary>
        /// The node.
        /// </summary>
        private readonly HtmlNode node;

        /// <summary>
        /// Name of the resource.
        /// </summary>
        private readonly string resourceName;

        /// <summary>
        /// The template parsers.
        /// </summary>
        private readonly Dictionary<string, TemplateParser> templateParsers = new Dictionary<string, TemplateParser>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fullResourceName"> Name of the full resource. </param>
        /// <param name="htmlDoc">          The HTML document. </param>
        /// <param name="context">          The context. </param>
        public HtmlParser(
            string fullResourceName,
            HtmlDocument htmlDoc,
            ParserContext context)
        {
            this.resourceName = fullResourceName;
            this.context = context;
            this.documentContext = new DocumentContext(context);
            this.node = htmlDoc.DocumentNode;
            this.documentContext.PushNode(this.node);
        }

        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        public string ResourceName
        { get { return this.resourceName; } }

        /// <summary>
        /// Process the node.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="node">       The node. </param>
        /// <param name="parentNode"> The parent node. </param>
        /// <param name="callback">   The callback. </param>
        public void ProcessNode(
            HtmlNode node,
            NodeInfo parentNode,
            Action<HtmlNode, NodeInfo, NodeType> callback)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse node.
        /// </summary>
        /// <param name="node">           The node. </param>
        /// <param name="parentNodeInfo"> Information describing the parent node. </param>
        /// <returns>
        /// .
        /// </returns>
        private HtmlNode ParseNode(
            HtmlNode node,
            NodeInfo parentNodeInfo)
        {
            try
            {
                var nodeName = this.documentContext.GetFullName(node.OriginalName);
                NodeType nodeType = this.GetNodeType(node, parentNodeInfo);
                if (nodeType == NodeType.Html)
                {
                }

                return null;
            }
            finally
            {
                // Remove the namespace mapping that was inserted for this node.
                this.documentContext.PopNode();
            }
        }

        /// <summary>
        /// Gets template parser.
        /// </summary>
        /// <param name="templateId"> Identifier for the template. </param>
        /// <returns>
        /// The template parser.
        /// </returns>
        public TemplateParser GetTemplateParser(string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
            {
                templateId = "::";
            }

            TemplateParser rv;
            if (this.templateParsers.TryGetValue(templateId, out rv))
            {
                return rv;
            }

            return null;
        }

        /// <summary>
        /// Gets node type.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="node">           The node. </param>
        /// <param name="parentNodeInfo"> Information describing the parent node. </param>
        /// <returns>
        /// The node type.
        /// </returns>
        public NodeType GetNodeType(HtmlNode node, NodeInfo parentNodeInfo)
        {
            var nodeName = this.documentContext.GetFullName(node.OriginalName);

            if (nodeName.Item2 == "head"
                && nodeName.Item1 == null)
            {
                return NodeType.Head;
            }
            if (nodeName.Item1 == null && nodeName.Item2 == Strings.CssStypeTag)
            {
                return NodeType.CssStyle;
            }
            if (nodeName.Item1 == null)
            {
                // There are 2 possibilities,
                // 1. This is either a constructor parameter or Property
                // 2. This is a HTML node.
                PropertyReference property = null;
                if (parentNodeInfo is TypeNodeInfo
                    && (property = this.context.ClrResolver.GetPropertyReference(
                            ((TypeNodeInfo)parentNodeInfo).Type,
                            nodeName.Item2)) != null)
                {
                    return NodeType.Property;
                }
                else if (nodeName.Item2 == Strings.Template)
                {
                    return NodeType.Template;
                }
                else if (nodeName.Item2 == Strings.Skin)
                {
                    return NodeType.Skin;
                }

                return NodeType.Html;
            }
            else
            {
                string[] nameParts = nodeName.Item2.Split('.');
                TypeReference typeReference = this.context.ClrResolver.GetTypeReference(nodeName.Item1 + '.' + nameParts[0]);

                if (typeReference == null)
                {
                    throw new ApplicationException(
                        string.Format("Can't resolve type {0}.{1}", nodeName.Item1, nameParts[0]));
                }

                if (nameParts.Length > 2)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Only Html tags, types or Attached Properties names are supported as tags, unsupported tag: {0}.{1}",
                            nodeName.Item1,
                            nodeName.Item2));
                }

                if (nameParts.Length == 2)
                {
                    var property = this.context.ClrResolver.GetPropertyReference(typeReference, nameParts[1]);

                    if (property == null)
                    {
                        throw new ApplicationException(
                            string.Format("Can't resolve proeprty {0}.{1}", nodeName.Item1, nodeName.Item2));
                    }

                    return NodeType.AttachedProperty;
                }

                if (this.context.ClrResolver.TypeInherits(typeReference, this.context.KnownTypes.UISkinableElement))
                {
                    return NodeType.SkinableElement;
                }
                else if (this.context.ClrResolver.TypeInherits(typeReference, this.context.KnownTypes.UIPanel))
                {
                    return NodeType.Panel;
                }
                else if (this.context.ClrResolver.TypeInherits(typeReference, this.context.KnownTypes.UIElement))
                {
                    return NodeType.UIElement;
                }
                else if (this.context.ClrResolver.TypeInherits(typeReference, this.context.KnownTypes.ContextBindableObject))
                {
                    return NodeType.ContextBindableObject;
                }
                else if (this.context.ClrResolver.TypeInherits(typeReference, this.context.KnownTypes.ObservableObject))
                {
                    return NodeType.ObservableObject;
                }
            }

            if (parentNodeInfo is SkinnableNodeInfo)
            {
                // When parent is ControlNodeInfo, this really means that parent is anonymous template
                // The exception to this rule is AttachedProperty or property. But for normal Property
                // nodeName.Item1 == null will take care, so only remaining case where ControlNodeInfo
                // really is treated as ControlNodeInfo for AttachedPropertyCase.
                // For all other cases, the node can be ControlNodeInfo/PanelNodeInfo/UIElementNodeInfo/HtmlNodeInfo
            }

            return NodeType.Html;
        }

        /// <summary>
        /// Gets a context for the parser.
        /// </summary>
        /// <value>
        /// The parser context.
        /// </value>
        public ParserContext ParserContext
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets a context for the document.
        /// </summary>
        /// <value>
        /// The document context.
        /// </value>
        public DocumentContext DocumentContext
        { get { return this.documentContext; } }

        /// <summary>
        /// Gets the resolver.
        /// </summary>
        /// <value>
        /// The resolver.
        /// </value>
        public IClrResolver Resolver
        {
            get { return this.context.ClrResolver; }
        }

        /// <summary>
        /// Parse document.
        /// </summary>
        /// <exception cref="ConverterLocationException"> Thrown when a Converter Location error condition
        ///     occurs. </exception>
        /// <param name="node">        The node. </param>
        /// <param name="parsingHead"> (optional) the parsing head. </param>
        private void ParseDocument(HtmlNode node, bool parsingHead = false)
        {
            bool headParsed = false;
            foreach (var item in this.node.ChildNodes)
            {
                try
                {
                    string nodeName = item.OriginalName.ToLowerInvariant();
                    this.documentContext.PushNode(item);
                    var nodeType = this.GetNodeType(node, null);
                    if (nodeType == NodeType.Head)
                    {
                        if (headParsed)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    node.Line,
                                    node.LinePosition),
                                "Head node present more than once.");
                        }

                        this.ParseDocument(item, true);
                        headParsed = true;
                    }
                    else if (nodeType == NodeType.CssStyle)
                    {
                        // Parse Css.
                        var grammer = new CssParser.CssGrammer(node.InnerText);
                        this.documentContext.AddCssRules(grammer.Rules);
                    }
                    else if (nodeType == NodeType.Template
                        || nodeType == NodeType.Skin)
                    {
                        if (parsingHead)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    node.Line,
                                    node.LinePosition),
                                "Head node can't contain template.");
                        }

                        // Parse Template.
                        TemplateParser parser = new TemplateParser(
                            this,
                            node,
                            nodeType == NodeType.Skin,
                            null);

                        string templateName = parser.TemplateName ?? "::";
                        if (this.templateParsers.ContainsKey(templateName))
                        {
                            if (templateName == "::")
                            {
                                throw new ConverterLocationException(
                                    new Location(
                                        this.resourceName,
                                        node.Line,
                                        node.LinePosition),
                                    "There can't be more than 1 default template in one template file.");
                            }
                            else
                            {
                                throw new ConverterLocationException(
                                    new Location(
                                        this.resourceName,
                                        node.Line,
                                        node.LinePosition),
                                    string.Format(
                                        "There can't be more than 1 default template in one template file.",
                                        templateName));
                            }
                        }

                        this.templateParsers.Add(parser.TemplateName, parser);
                        parser.Parse();
                    }
                    else
                    {
                        throw new ConverterLocationException(
                            new Location(
                                this.resourceName,
                                node.Line,
                                node.LinePosition),
                            string.Format(
                                "Don't know how to parse {0} node.",
                                nodeName));
                    }
                }
                finally
                {
                    this.documentContext.PopNode();
                }
            }
        }
    }
}