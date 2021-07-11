//-----------------------------------------------------------------------
// <copyright file="SkinNodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for SkinNodeInfo
    /// </summary>
    public class SkinNodeInfo : NodeInfo, IHtmlNodeGenerator
    {
        /// <summary>
        /// The generated node.
        /// </summary>
        private HtmlNode generatedNode;

        /// <summary>
        /// The element reference.
        /// </summary>
        private TypeReference elementRef;

        /// <summary>
        /// The part requirements.
        /// </summary>
        private Dictionary<string, Tuple<TypeReference, bool>> partRequirements = null;

        /// <summary>
        /// The parts found.
        /// </summary>
        private Dictionary<string, Tuple<NodeInfo, bool>> partsFound
            = new Dictionary<string, Tuple<NodeInfo, bool>>();

        public SkinNodeInfo(
            TypeReference dataContextType,
            TypeReference controlType,
            HtmlNode node)
            : base(node, null)
        {
            this.DataContextType = dataContextType;
            this.ControlType = controlType;
        }

        /// <summary>
        /// Gets or sets the type of the data context.
        /// </summary>
        /// <value>
        /// The type of the data context.
        /// </value>
        public TypeReference DataContextType
        { get; private set; }

        /// <summary>
        /// Gets or sets the type of the control.
        /// </summary>
        /// <value>
        /// The type of the control.
        /// </value>
        public TypeReference ControlType
        { get; private set; }

        public IIdentifier[] ClassNames
        { get; set; }

        public override void ParseNode(TemplateParser parser)
        {
            this.generatedNode = parser.GeneratedDocument.CreateElement("div");
            this.FindPartIds(parser.HtmlParser);
        }

        public override bool ProcessChildNode(NodeInfo childNode)
        {
            IHtmlNodeGenerator nodeGenerator = childNode as IHtmlNodeGenerator;

            if (nodeGenerator == null)
            {
                return false;
            }

            this.AddChildNodeInfo(childNode);
            this.generatedNode.AppendChild(nodeGenerator.GeneratedNode);
            return true;
        }

        public HtmlAgilityPack.HtmlNode GeneratedNode
        {
            get { return this.generatedNode; }
        }

        public void SetNewNodeAndPath(HtmlAgilityPack.HtmlNode node)
        {
            this.generatedNode = node;
        }

        public void FinalizeGeneratedNode(SkinCodeGenerator codeGenerator)
        {
        }

        public Expression GetPartDictionary(SkinCodeGenerator codeGenerator)
        {
            foreach (var kvPair in this.partRequirements)
            {
                if (!this.partsFound.ContainsKey(kvPair.Key))
                {
                    if (kvPair.Value.Item2)
                    {
                        throw new ConverterLocationException(
                            new NScript.Utils.Location(
                                codeGenerator.Parser.HtmlParser.ResourceName,
                                this.Node.Line,
                                this.Node.LinePosition),
                            string.Format("PartId {0} for control {1} not found",
                                kvPair.Key,
                                this.ControlType));
                    }
                }
            }

            if (this.partsFound.Count == 0)
            {
                return new NullLiteralExpression(codeGenerator.Scope);
            }

            var rv = new InlineObjectInitializer(
                null,
                codeGenerator.Scope);
            foreach (var kvPair in this.partsFound)
            {
                rv.AddInitializer(
                    kvPair.Key,
                    new NumberLiteralExpression(
                        codeGenerator.Scope,
                        codeGenerator.GetObjectIndexForNode(
                            kvPair.Value.Item1,
                            kvPair.Value.Item2)));
            }

            return rv;
        }

        public void RegisterPart(
            string partId,
            IHtmlNodeGenerator htmlNodeGenerator)
        {
            Tuple<TypeReference, bool> partInfo;
            if (!this.partRequirements.TryGetValue(partId, out partInfo))
            {
                throw new ApplicationException(
                    string.Format("ControlType {0} does not have part with partId {1}",
                        this.ControlType,
                        partId));
            }

            if (this.partsFound.ContainsKey(partId))
            {
                throw new ApplicationException(
                    string.Format("PartId {0} already defined before in template",
                        partId));
            }

            TypeReference partTypeRef;
            var elementRef = this.elementRef;
            if (htmlNodeGenerator is HtmlNodeInfo)
            {
                partTypeRef = elementRef;
            }
            else if (htmlNodeGenerator is UIElementNodeInfo)
            {
                partTypeRef = ((UIElementNodeInfo)htmlNodeGenerator).Type;
                if (partTypeRef.ExtendsType(partInfo.Item1))
                {
                    this.partsFound.Add(
                        partId,
                        Tuple.Create(
                            (NodeInfo)htmlNodeGenerator,
                            false));
                    htmlNodeGenerator.MarkAsPart(false);

                    return;
                }
            }
            else
            {
                throw new ApplicationException(
                    string.Format("Template Parts can only be UIElement or HtmlElement"));
            }

            if (partInfo.Item1.ExtendsType(elementRef))
            {
                this.partsFound.Add(
                    partId,
                    Tuple.Create(
                        (NodeInfo)htmlNodeGenerator,
                        elementRef != partTypeRef));
                htmlNodeGenerator.MarkAsPart(elementRef != partTypeRef);
            }
            else
            {
                throw new ApplicationException(
                    string.Format("Template Part with Id {0} does not match type {1} or System.Web.Html.Element.",
                        partId,
                        partTypeRef));
            }
        }

        private void FindPartIds(HtmlParser htmlParser)
        {
            if (this.partRequirements != null)
            {
                return;
            }

            var currentType = this.ControlType;
            var currentTypeDef = currentType.Resolve();
            var parserContext = htmlParser.ParserContext;
            var knownTypes = parserContext.KnownTypes;
            var uiElement = knownTypes.UIElement;
            var skinPartAttr = knownTypes.SkinPartAttribute;
            var stringType = parserContext.ConverterContext.ClrKnownReferences.String;
            this.elementRef = knownTypes.ElementRef;
            this.partRequirements = new Dictionary<string, Tuple<TypeReference, bool>>();
            while(currentTypeDef != null
                && !currentTypeDef.IsSameDefinition(uiElement))
            {
                foreach (var field in currentTypeDef.Fields)
                {
                    if (!field.HasConstant
                        || !field.FieldType.IsSameDefinition(stringType))
                    {
                        continue;
                    }

                    var partAttribute = field.CustomAttributes.SelectAttribute(skinPartAttr);
                    if (partAttribute != null)
                    {
                        TypeReference typeRef = (TypeReference)partAttribute.ConstructorArguments[0].Value;
                        bool isRequired = partAttribute.ConstructorArguments.Count == 2
                            ? (bool)partAttribute.ConstructorArguments[1].Value
                            : true;
                        string partId = field.Constant as string;

                        if (this.partRequirements.ContainsKey(partId))
                        {
                            throw new ConverterLocationException(
                                new NScript.Utils.Location(
                                    htmlParser.ResourceName,
                                    this.Node.Line,
                                    this.Node.LinePosition),
                                string.Format("Given control ({0}) has more than one parts with same id '{1}'.",
                                    this.ControlType,
                                    partId));
                        }

                        this.partRequirements.Add(
                            partId,
                            Tuple.Create(typeRef, isRequired));
                    }
                }

                currentType = currentType.GetBaseType();
                currentTypeDef = currentType.Resolve();
            }
        }

        void IHtmlNodeGenerator.MarkAsPart(bool isHtmlPart)
        {
            throw new InvalidOperationException();
        }
    }
}
