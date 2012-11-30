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
    public class HtmlParser : IDocumentContext
    {
        private ParserContext context;
        private HtmlNode node;
        private List<Dictionary<string, string>> namespaceStack = new List<Dictionary<string, string>>();

        public HtmlParser(
            HtmlDocument htmlDoc,
            ParserContext context)
        {
            this.context = context;
        }

        public void ProcessNode(
            HtmlNode node,
            NodeInfo parentNode,
            Action<HtmlNode, NodeInfo, NodeType> callback)
        {
            throw new NotImplementedException();
        }

        private HtmlNode ParseNode(
            HtmlNode node,
            NodeInfo parentNodeInfo)
        {
            try
            {
                var nodeName = this.GetFullName(node.OriginalName);
                NodeType nodeType = this.GetNodeType(node, parentNodeInfo);
                if (nodeType == NodeType.Html)
                {
                }

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
            var nodeName = this.GetFullName(node.OriginalName);

            if (nodeName.Item1 == null && nodeName.Item2 == Strings.CssStypeTag)
            {
                return NodeType.CssStyle;
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
                TypeReference typeReference = this.context.Resolver.GetTypeReference(nodeName.Item1 + '.' + nameParts[0]);

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
                    var property = this.context.Resolver.GetPropertyReference(typeReference, nameParts[1]);

                    if (property == null
                        || property.Count == 0)
                    {
                        throw new ApplicationException(
                            string.Format("Can't resolve proeprty {0}.{1}", nodeName.Item1, nodeName.Item2));
                    }

                    return NodeType.AttachedProperty;
                }

                if (this.context.Resolver.TypeInherits(typeReference, this.context.KnownTypes.UISkinableElement))
                {
                    return NodeType.SkinableElement;
                }
                else if (this.context.Resolver.TypeInherits(typeReference, this.context.KnownTypes.UIPanel))
                {
                    return NodeType.Panel;
                }
                else if (this.context.Resolver.TypeInherits(typeReference, this.context.KnownTypes.UIElement))
                {
                    return NodeType.UIElement;
                }
                else if (this.context.Resolver.TypeInherits(typeReference, this.context.KnownTypes.ContextBindableObject))
                {
                    return NodeType.ContextBindableObject;
                }
                else if (this.context.Resolver.TypeInherits(typeReference, this.context.KnownTypes.ObservableObject))
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

        public ParserContext ParserContext
        {
            get { return this.context; }
        }

        public IResolver Resolver
        {
            get { return this.context.Resolver; }
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
    }
}