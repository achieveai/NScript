//-----------------------------------------------------------------------
// <copyright file="TemplateParser.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
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
        private readonly HtmlParser parentParser;

        /// <summary>
        /// The context.
        /// </summary>
        private readonly DocumentContext context;

        /// <summary>
        /// Type of the control.
        /// </summary>
        private readonly TypeReference controlType;

        /// <summary>
        /// Type of the data context.
        /// </summary>
        private readonly TypeReference dataContextType;

        /// <summary>
        /// The node.
        /// </summary>
        private readonly HtmlNode node;

        /// <summary>
        /// The document.
        /// </summary>
        private readonly HtmlDocument generatedDocument;

        /// <summary>
        /// The new node.
        /// </summary>
        private readonly HtmlNode generatedNode;

        /// <summary>
        /// The resolver.
        /// </summary>
        private readonly IClrResolver resolver;

        /// <summary>
        /// The sub template.
        /// </summary>
        private readonly bool subTemplate = false;

        /// <summary>
        /// Name of the template.
        /// </summary>
        private readonly string templateName;

        /// <summary>
        /// Information describing the root template node.
        /// </summary>
        private TemplateNodeInfo rootTemplateNodeInfo;

        /// <summary>
        /// Information describing the root skin node.
        /// </summary>
        private SkinNodeInfo rootSkinNodeInfo;

        /// <summary>
        /// true if this object is template.
        /// </summary>
        private readonly bool isTemplate;

        /// <summary>
        /// true if this object is parsed.
        /// </summary>
        private bool isParsed = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="htmlNode">         The HTML node. </param>
        /// <param name="parentNodeInfo">   Information describing the parent node. </param>
        public TemplateParser(
            HtmlParser htmlParser,
            HtmlNode htmlNode,
            bool isSkin,
            NodeInfo parentNodeInfo)
        {
            this.parentParser = htmlParser;
            this.context = htmlParser.DocumentContext;
            this.node = htmlNode;
            this.resolver = this.context.ParserContext.ClrResolver;
            this.generatedDocument = this.context.ParserContext.Document;
            this.generatedNode = this.generatedDocument.CreateElement("div");

            if (parentNodeInfo != null)
            {
                this.subTemplate = true;
            }
            else
            {
                this.templateName = htmlNode.GetAttributeValue("Id", (string)null);
            }

            var attr = htmlNode.Attributes["ControlType"];
            if (attr == null)
            {
                throw new ConverterLocationException(
                    new NScript.Utils.Location(
                        this.HtmlParser.ResourceName,
                        htmlNode.Line,
                        htmlNode.LinePosition),
                    "Template/Skin does not have ControlType attribute");
            }

            this.controlType = this.resolver.GetTypeReference(
                this.DocumentContext.GetFullName(attr.Value));
            if (this.controlType == null)
            {
                throw new ConverterLocationException(
                    new NScript.Utils.Location(
                        this.HtmlParser.ResourceName,
                        attr.Line,
                        attr.LinePosition),
                        string.Format(
                            "Can't resolve ControlType:{0}.",
                            attr.Value));
            }

            attr = htmlNode.Attributes["DataContextType"];
            if (attr == null)
            {
                throw new ConverterLocationException(
                    new NScript.Utils.Location(
                        this.HtmlParser.ResourceName,
                        htmlNode.Line,
                        htmlNode.LinePosition),
                    "Template/Skin does not have DataContextType attribute");
            }

            this.dataContextType = this.resolver.GetTypeReference(
                this.DocumentContext.GetFullName(attr.Value));
            if (this.dataContextType == null)
            {
                throw new ConverterLocationException(
                    new NScript.Utils.Location(
                        this.HtmlParser.ResourceName,
                        attr.Line,
                        attr.LinePosition),
                    string.Format(
                        "Can't resolve DataContextType:{0}.",
                        attr.Value));
            }

            if (isSkin)
            {
                var skinNodeInfo = new SkinNodeInfo(
                    dataContextType,
                    controlType,
                    this.node);

                skinNodeInfo.ParseNode(this);
                this.rootSkinNodeInfo = skinNodeInfo;
            }
            else
            {
                this.rootTemplateNodeInfo = new TemplateNodeInfo(
                    controlType,
                    dataContextType,
                    this.node);
            }
        }

        /// <summary>
        /// Gets the type of the data context.
        /// </summary>
        /// <value>
        /// The type of the data context.
        /// </value>
        public TypeReference DataContextType
        { get { return this.dataContextType; } }

        /// <summary>
        /// Gets the type of the control.
        /// </summary>
        /// <value>
        /// The type of the control.
        /// </value>
        public TypeReference ControlType
        { get { return this.controlType; } }

        /// <summary>
        /// Gets a context for the document.
        /// </summary>
        /// <value>
        /// The document context.
        /// </value>
        public DocumentContext DocumentContext
        { get { return this.context; } }

        /// <summary>
        /// Gets the generated document.
        /// </summary>
        /// <value>
        /// The generated document.
        /// </value>
        public HtmlDocument GeneratedDocument
        { get { return this.generatedDocument; } }

        /// <summary>
        /// Gets the HTML parser.
        /// </summary>
        /// <value>
        /// The HTML parser.
        /// </value>
        public HtmlParser HtmlParser
        { get { return this.parentParser; } }

        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        /// <value>
        /// The name of the template.
        /// </value>
        public string TemplateName
        { get { return this.templateName; } }

        /// <summary>
        /// Gets a value indicating whether this object is sub template.
        /// </summary>
        /// <value>
        /// true if this object is sub template, false if not.
        /// </value>
        public bool IsSubTemplate
        { get { return this.subTemplate; } }

        public SkinNodeInfo SkinNodeInfo
        { get { return this.rootSkinNodeInfo; } }

        public TemplateNodeInfo TemplateNodeInfo
        { get { return this.rootTemplateNodeInfo; } }

        /// <summary>
        /// Gets unique template identifier.
        /// </summary>
        /// <returns>
        /// The unique template identifier.
        /// </returns>
        public string GetUniqueTemplateId()
        {
            string templateName = this.HtmlParser.ResourceName;

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

            if (this.TemplateName != null)
            {
                templateName += "_" + this.TemplateName;
            }
            else if (this.IsSubTemplate)
            {
                templateName += "_private";
            }

            return templateName;
        }

        /// <summary>
        /// Parses this object.
        /// </summary>
        internal void Parse()
        {
            if (this.isParsed)
            {
                return;
            }

            this.isParsed = true;
            NodeInfo nodeInfo =
                ((NodeInfo)this.rootTemplateNodeInfo) ?? this.rootSkinNodeInfo;

            IHtmlNodeGenerator rootHtmlNode = nodeInfo as IHtmlNodeGenerator;

            foreach (var childNode in node.ChildNodes)
            {
                var childNodeInfo = this.ParseNode(childNode, nodeInfo);
                if (!nodeInfo.ProcessChildNode(childNodeInfo))
                {
                    throw new ApplicationException(
                        "Incompatible nested nodes");
                }
            }
        }

        /// <summary>
        /// Parse node.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="node">           The node. </param>
        /// <param name="parentNodeInfo"> Information describing the parent node. </param>
        /// <returns>
        /// .
        /// </returns>
        private NodeInfo ParseNode(
            HtmlNode node,
            NodeInfo parentNodeInfo)
        {
            try
            {
                this.context.PushNode(node);
                var nodeName = this.context.GetFullName(node.OriginalName);
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
                        rv = this.ParseUIElementNode(node);
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
                    case NodeType.Skin:
                        throw new ApplicationException("can't declare Template or Skin in middle of other Skins or templates");
                        break;
                    case NodeType.CssStyle:
                        throw new ApplicationException(
                            string.Format("can't have style tag in middle of a template"));
                    default:
                        break;
                }

                foreach (var childNode in node.ChildNodes)
                {
                    var childNodeInfo = this.ParseNode(childNode, rv);
                    if (!rv.ProcessChildNode(childNodeInfo))
                    {
                        throw new ApplicationException(
                            "Incompatible nested nodes");
                    }
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
        /// Parse template node.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// .
        /// </returns>
        private NodeInfo ParseTemplateNode(HtmlNode node)
        {
            /*
            TemplateNodeInfo rv = new TemplateNodeInfo(
                this.controlType,
                this.dataContextType,
                node);

            return rv;
            */

            throw new InvalidOperationException();
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
            HtmlNodeInfo rv = new HtmlNodeInfo(
                node,
                this.DocumentContext.GetFullName(node.OriginalName));

            rv.ParseNode(this);

            return rv;
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
                var nameInfo = this.context.GetFullName(attribute.OriginalName);

                if (nameInfo.Item1 != null)
                {
                    throw new NotImplementedException();
                }

                var property = this.context.ParserContext.ClrResolver.GetPropertyReference(
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
                        GeneratedAttribute = this.generatedDocument.CreateAttribute(nameInfo.Item2, attribute.Value)
                    });
            }

            return rv;
        }

        /// <summary>
        /// Query if 'attributeName' is allowed attribute to set.
        /// </summary>
        /// <param name="attributeName"> Name of the attribute. </param>
        /// <returns>
        /// true if allowed attribute to set, false if not.
        /// </returns>
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

        /// <summary>
        /// Parse object node.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// .
        /// </returns>
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

        /// <summary>
        /// Parse object node.
        /// </summary>
        /// <param name="node">         The node. </param>
        /// <param name="typeNodeInfo"> Information describing the type node. </param>
        private void ParseObjectNode(HtmlNode node, TypeNodeInfo typeNodeInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse observable object.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// .
        /// </returns>
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

        /// <summary>
        /// Parse observable object.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="node">         The node. </param>
        /// <param name="typeNodeInfo"> Information describing the type node. </param>
        private void ParseObservableObject(HtmlNode node, TypeNodeInfo typeNodeInfo)
        {
            this.ParseObjectNode(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse context bindable object.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// .
        /// </returns>
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

        /// <summary>
        /// Parse context bindable object.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="node">         The node. </param>
        /// <param name="typeNodeInfo"> Information describing the type node. </param>
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
        private UIElementNodeInfo ParseUIElementNode(HtmlNode node)
        {
            var tuple = this.GetTypeReference(node);
            UIElementNodeInfo rv = new UIElementNodeInfo(
                tuple.Item1,
                node,
                tuple.Item2);

            rv.ParseNode(this);

            return rv;
        }

        /// <summary>
        /// Parse element node.
        /// </summary>
        /// <param name="node">         The node. </param>
        /// <param name="typeNodeInfo"> Information describing the type node. </param>
        private void ParseElementNode(HtmlNode node, UIElementNodeInfo typeNodeInfo)
        {
            this.ParseContextBindableObject(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse panel element node.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// .
        /// </returns>
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

        /// <summary>
        /// Parse panel element node.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="node">         The node. </param>
        /// <param name="typeNodeInfo"> Information describing the type node. </param>
        private void ParsePanelElementNode(HtmlNode node, PanelNodeInfo typeNodeInfo)
        {
            this.ParseElementNode(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse skinable element node.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// .
        /// </returns>
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

        /// <summary>
        /// Parse skinable element node.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="node">         The node. </param>
        /// <param name="typeNodeInfo"> Information describing the type node. </param>
        private void ParseSkinableElementNode(HtmlNode node, SkinnableNodeInfo typeNodeInfo)
        {
            this.ParseElementNode(node, typeNodeInfo);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets type reference.
        /// </summary>
        /// <param name="node"> The node. </param>
        /// <returns>
        /// The type reference.
        /// </returns>
        private Tuple<TypeReference, Tuple<string, string>> GetTypeReference(HtmlNode node)
        {
            var fullNameTuple = this.context.GetFullName(node.OriginalName);
            var fullName = fullNameTuple.Item1 + "." + fullNameTuple.Item2;

            return Tuple.Create(
                this.context.ParserContext.ClrResolver.GetTypeReference(fullName),
                fullNameTuple);
        }
    }
}
