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
    using System.Text.RegularExpressions;

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
        CssStyle,
        Comment,
        Meta,
        Body,
        Link
    }

    /// <summary>
    /// Definition for Parser
    /// </summary>
    public class HtmlParser
    {
        private static Regex textBreakingRegEx
            = new Regex(
                "^(?<text>(?:({{)|[^{])*)(?:(?<binding>{[^{}]+})(?<text>(?:[^{]|({{))*))*$",
                RegexOptions.Multiline | RegexOptions.Compiled);

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
            HtmlParser.RemoveComments(htmlDoc);
            HtmlParser.SplitTextBindingNodes(htmlDoc);
            this.resourceName = fullResourceName;
            this.context = context;
            this.documentContext = new DocumentContext(
                context,
                fullResourceName);
            this.node = htmlDoc.DocumentNode;
            this.documentContext.PushNode(this.node);

            foreach (var node in this.node.ChildNodes)
            {
                if (node.OriginalName == "html")
                {
                    try
                    {
                        documentContext.PushNode(node);
                        this.ParseDocument(node);
                    }
                    finally
                    {
                        documentContext.PopNode();
                    }
                    break;
                }
            }
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

            if (node is HtmlCommentNode)
            {
                return NodeType.Comment;
            }
            if (node is HtmlTextNode)
            {
                return NodeType.Text;
            }
            if (nodeName.Item2 == "head"
                && nodeName.Item1 == null)
            {
                return NodeType.Head;
            }
            if (nodeName.Item1 == null && nodeName.Item2 == Strings.CssStyleTag)
            {
                return NodeType.CssStyle;
            }

            if (nodeName.Item1 == null && nodeName.Item2 == Strings.LinkTag)
            {
                return NodeType.Link;
            }

            if (nodeName.Item1 == null &&
                (nodeName.Item2 == Strings.Meta
                || nodeName.Item2 == Strings.Title))
            {
                return NodeType.Meta;
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
                else if (nodeName.Item2 == Strings.Body)
                {
                    return NodeType.Body;
                }

                return NodeType.Html;
            }
            else
            {
                string[] nameParts = nodeName.Item2.Split('.');
                TypeReference typeReference = this.context.ClrResolver.GetTypeReference(nodeName.Item1 + '.' + nameParts[0]);

                if (typeReference == null)
                {
                    throw new ConverterLocationException(
                        new Location(
                            this.ResourceName,
                            node.Line,
                            node.LinePosition),
                        string.Format("Can't resolve type {0}.{1}", nodeName.Item1, nameParts[0]));
                }

                if (nameParts.Length > 2)
                {
                    throw new ConverterLocationException(
                        new Location(
                            this.ResourceName,
                            node.Line,
                            node.LinePosition),
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
        /// Removes the comments described by htmlDoc.
        /// </summary>
        /// <param name="htmlDoc"> The HTML document. </param>
        public static void RemoveComments(HtmlDocument htmlDoc)
        {
            var nodes = htmlDoc.DocumentNode.SelectNodes("//comment()");
            if (nodes != null)
            {
                foreach (var comment in nodes)
                {
                    if (comment.PreviousSibling != null
                        && comment.NextSibling != null
                        && comment.NextSibling.NodeType == HtmlNodeType.Text
                        && comment.PreviousSibling.NodeType == HtmlNodeType.Text)
                    {
                        ((HtmlTextNode)comment.PreviousSibling).Text += comment.NextSibling.InnerText;
                        comment.ParentNode.RemoveChild(comment.NextSibling);
                    }

                    comment.ParentNode.RemoveChild(comment);
                }
            }
        }

        public static void SplitTextBindingNodes(HtmlDocument htmlDoc)
        {
            var nodes = htmlDoc.DocumentNode.SelectNodes("//body//text()");

            if (nodes != null)
            {
                foreach(var node in nodes)
                {
                    HtmlTextNode textNode = (HtmlTextNode)node;

                    var match = HtmlParser.textBreakingRegEx.Match(textNode.Text);
                    var bindingGroup = match.Groups["binding"];
                    if (match.Success
                        && bindingGroup.Captures.Count >= 1
                        && bindingGroup.Captures[0].Length < textNode.Text.Length)
                    {
                        var textGroups = match.Groups["text"];
                        string text = textNode.Text;
                        List<HtmlTextNode> textNodes = new List<HtmlTextNode>();
                        Capture capture;
                        for (int iCapture = 0; iCapture < bindingGroup.Captures.Count; iCapture++)
                        {
                            capture = textGroups.Captures[iCapture];
                            HtmlParser.AddTextNode(
                                textNodes,
                                capture,
                                text,
                                textNode.Line,
                                textNode.LinePosition);

                            capture = bindingGroup.Captures[iCapture];
                            HtmlParser.AddTextNode(
                                textNodes,
                                capture,
                                text,
                                textNode.Line,
                                textNode.LinePosition);
                        }

                        capture = textGroups.Captures[bindingGroup.Captures.Count];
                        HtmlParser.AddTextNode(
                            textNodes,
                            capture,
                            text,
                            textNode.Line,
                            textNode.LinePosition);

                        if (textNodes.Count > 1)
                        {
                            for (int iTextNode = 0; iTextNode < textNodes.Count; iTextNode++)
                            {
                                textNode.ParentNode.InsertBefore(textNodes[iTextNode], textNode);
                            }

                            textNode.ParentNode.RemoveChild(textNode);
                        }
                    }
                }
            }
        }

        private static void AddTextNode(
            List<HtmlTextNode> textNodes,
            Capture capture,
            string text,
            int startPositionLine,
            int startPositionCol)
        {
            if (capture.Length == null)
            { return; }

            int lineDelta = startPositionLine;
            int positionDelta = startPositionCol;

            for (int iPos = 0; iPos < capture.Index; iPos++)
            {
                if (text[iPos] == '\n')
                {
                    lineDelta++;
                    positionDelta = 0;
                }
                else
                {
                    positionDelta++;
                }
            }

            var rv = (HtmlTextNode)HtmlTextNode.CreateNode(capture.Value);
            typeof(HtmlNode).GetProperty("Line").SetValue(rv, lineDelta);
            typeof(HtmlNode).GetProperty("LinePosition").SetValue(rv, positionDelta);
            textNodes.Add(rv);
        }

        private static void SetLinePosition(HtmlNode node, int line, int position)
        {
        }

        /// <summary>
        /// Parse document.
        /// </summary>
        /// <exception cref="ConverterLocationException"> Thrown when a Converter Location error condition
        ///     occurs. </exception>
        /// <param name="nodeToProcess">        The node. </param>
        /// <param name="parsingHead"> (optional) the parsing head. </param>
        private void ParseDocument(
            HtmlNode nodeToProcess,
            bool parsingHead = false,
            bool parsingBody = false)
        {
            bool headParsed = false;
            bool bodyParsed = false;
            foreach (var childNode in nodeToProcess.ChildNodes)
            {
                try
                {
                    string nodeName = childNode.OriginalName.ToLowerInvariant();
                    this.documentContext.PushNode(childNode);
                    var nodeType = this.GetNodeType(childNode, null);
                    if (nodeType == NodeType.Head)
                    {
                        if (headParsed
                            || parsingHead)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    childNode.Line,
                                    childNode.LinePosition),
                                "Head node present more than once.");
                        }
                        else if (parsingBody)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    childNode.Line,
                                    childNode.LinePosition),
                                "Head node present in body.");
                        }

                        this.ParseDocument(childNode, true);
                        headParsed = true;
                    }
                    else if (nodeType == NodeType.Body)
                    {
                        if (bodyParsed
                            || parsingBody)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    childNode.Line,
                                    childNode.LinePosition),
                                "Body node present more than once.");
                        }
                        else if (parsingHead)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    childNode.Line,
                                    childNode.LinePosition),
                                "Body node present in head.");
                        }

                        this.ParseDocument(childNode, false, true);
                        bodyParsed = true;
                    }
                    else if (nodeType == NodeType.CssStyle)
                    {
                        if (!parsingHead)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    childNode.Line,
                                    childNode.LinePosition),
                                "Css style can only be defined in Head.");
                        }

                        this.documentContext.AddCss(childNode);
                    }
                    else if (nodeType == NodeType.Link)
                    {
                        if (!parsingHead)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    childNode.Line,
                                    childNode.LinePosition),
                                "Css style can only be defined in Head.");
                        }

                        this.documentContext.AddCssLink(childNode);
                    }
                    else if (nodeType == NodeType.Template || nodeType == NodeType.Skin)
                    {
                        if (!parsingBody)
                        {
                            throw new ConverterLocationException(
                                new Location(
                                    this.resourceName,
                                    childNode.Line,
                                    childNode.LinePosition),
                                "Template and Skin can only be defined in Body.");
                        }

                        // Parse Template.
                        TemplateParser parser = new TemplateParser(
                            this,
                            childNode,
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
                                        childNode.Line,
                                        childNode.LinePosition),
                                    "There can't be more than 1 default template in one template file.");
                            }
                            else
                            {
                                throw new ConverterLocationException(
                                    new Location(
                                        this.resourceName,
                                        childNode.Line,
                                        childNode.LinePosition),
                                    string.Format(
                                        "There can't be more than 1 default template in one template file.",
                                        templateName));
                            }
                        }

                        this.templateParsers.Add(templateName, parser);
                        parser.Parse();
                    }
                    else if (nodeType == NodeType.Comment
                        || nodeType == NodeType.Meta)
                    {
                        continue;
                    }
                    else if (nodeType == NodeType.Text
                        && string.IsNullOrWhiteSpace(((HtmlTextNode)childNode).Text))
                    {
                        continue;
                    }
                    else
                    {
                        throw new ConverterLocationException(
                            new Location(
                                this.resourceName,
                                childNode.Line,
                                childNode.LinePosition),
                            string.Format(
                                "Don't know how to parse {0} node.",
                                nodeName));
                    }
                }
                catch(ConverterLocationException ex)
                {
                    ParserContext.ConverterContext.AddError(
                        ex.Location,
                        ex.Message,
                        false);
                }
                catch (ApplicationException ex)
                {
                    ParserContext.ConverterContext.AddError(
                        new Location(
                            this.resourceName,
                            childNode.Line,
                            childNode.LinePosition),
                        ex.Message,
                        false);
                }
                finally
                {
                    this.documentContext.PopNode();
                }
            }
        }
    }
}