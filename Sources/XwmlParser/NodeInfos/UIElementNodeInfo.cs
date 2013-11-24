//-----------------------------------------------------------------------
// <copyright file="UIElementNodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for UIElementNodeInfo
    /// </summary>
    public class UIElementNodeInfo : ContextBindableNodeInfo, IHtmlNodeGenerator
    {
        /// <summary>
        /// The generated node.
        /// </summary>
        private HtmlNode generatedNode;

        /// <summary>
        /// true if this object has HTML binding.
        /// </summary>
        private bool needHtmlNodeAccess = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">     The type. </param>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        public UIElementNodeInfo(
            TypeReference type,
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(type, node, tagInfo)
        { }

        /// <summary>
        /// Gets or sets the generated node.
        /// </summary>
        /// <value>
        /// The generated node.
        /// </value>
        public HtmlNode GeneratedNode
        {
            get { return this.generatedNode; }
        }

        /// <summary>
        /// Gets or sets a list of names of the class.
        /// </summary>
        /// <value>
        /// A list of names of the class.
        /// </value>
        public IIdentifier[] ClassNames
        { get; set; }

        /// <summary>
        /// Parse node.
        /// </summary>
        /// <param name="parser"> The parser. </param>
        public override void ParseNode(TemplateParser parser)
        {
            this.SetGeneratedNode(parser);

            base.ParseNode(parser);
        }

        /// <summary>
        /// Process the child node described by childNode.
        /// </summary>
        /// <param name="childNode"> The child node. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
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

        /// <summary>
        /// Generates a code.
        /// </summary>
        /// <param name="codeGenerator">     The code generator. </param>
        public override void GenerateCode(SkinCodeGenerator codeGenerator)
        {
            int htmlObjectIndex = -1;
            if (this.needHtmlNodeAccess)
            {
                htmlObjectIndex = codeGenerator.GetObjectIndexForNode(this, true);
            }

            if (htmlObjectIndex >= 0)
            {
                codeGenerator.AddStatement(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IndexExpression(
                            null,
                            codeGenerator.Scope,
                            new IdentifierExpression(
                                codeGenerator.GetObjectStorageIdentifier(),
                                codeGenerator.Scope),
                            new NumberLiteralExpression(
                                codeGenerator.Scope,
                                htmlObjectIndex)),
                        HtmlNodeInfo.GetDomNodeExpression(
                            codeGenerator,
                            this.generatedNode)));
            }

            base.GenerateCode(codeGenerator);
        }

        /// <summary>
        /// Finalize generated node.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="codeGenerator"> The code generator. </param>
        public void FinalizeGeneratedNode(SkinCodeGenerator codeGenerator)
        {
            HtmlNodeInfo.FinalizeClassNamesInGeneratedNode(
                this.generatedNode,
                this.ClassNames);

            foreach (var item in this.ChildNodes)
            {
                IHtmlNodeGenerator nodeGenertaor = item as IHtmlNodeGenerator;
                if (nodeGenertaor != null)
                {
                    nodeGenertaor.FinalizeGeneratedNode(codeGenerator);
                }
            }
        }

        public void MarkAsPart(bool isDomPart)
        {
            if (isDomPart)
            {
                this.needHtmlNodeAccess = true;
            }
        }

        /// <summary>
        /// Sets generated node.
        /// </summary>
        /// <param name="parser"> The parser. </param>
        protected void SetGeneratedNode(TemplateParser parser)
        {
            if (this.generatedNode != null)
            {
                return;
            }

            var knownTypes = parser.DocumentContext.ParserContext.KnownTypes;
            string tagName = "div";
            var typeDef = this.Type.Resolve();
            var customAttributes = typeDef.CustomAttributes;
            var tagNameAttr = typeDef.CustomAttributes.SelectAttribute(knownTypes.TagNameAttribute);
            if (tagNameAttr!=null)
            {
                tagName = (string)tagNameAttr.ConstructorArguments[0].Value;
            }

            this.generatedNode = parser.GeneratedDocument.CreateElement(tagName);
            for (int iAttr = 0; iAttr < customAttributes.Count; iAttr++)
            {
                var customAttr = customAttributes[iAttr];
                if (customAttr.Constructor.DeclaringType.IsSameDefinition(knownTypes.DomAttributeAttribute))
                {
                    this.generatedNode.Attributes.Add(
                        (string)customAttr.ConstructorArguments[0].Value,
                        (string)customAttr.ConstructorArguments[1].Value);
                }
            }
        }

        /// <summary>
        /// Parse attribute.
        /// </summary>
        /// <param name="attribute"> The attribute. </param>
        /// <param name="parser">    The parser. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        protected override bool ParseAttribute(HtmlAttribute attribute, TemplateParser parser)
        {
            if (base.ParseAttribute(attribute, parser))
            {
                return true;
            }

            if (HtmlNodeInfo.IsHtmlAttribute(attribute))
            {
                var bindingInfo =
                    HtmlNodeInfo.ParseHtmlAttribute(
                        this,
                        attribute,
                        parser);

                if (bindingInfo != null)
                {
                    this.AddBinder(bindingInfo);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets object constructor arguments.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        /// <param name="args">          The arguments. </param>
        protected override void GetCtorArgs(
            SkinCodeGenerator codeGenerator,
            List<Expression> args)
        {
            args.Add(
                HtmlNodeInfo.GetDomNodeExpression(
                    codeGenerator,
                    this.generatedNode));

            base.GetCtorArgs(codeGenerator, args);
        }

        /// <summary>
        /// Gets object index for binding.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        /// <param name="binding">       The binding. </param>
        /// <returns>
        /// The object index for binding.
        /// </returns>
        protected override int? GetObjectIndexForBinding(
            SkinCodeGenerator codeGenerator,
            BinderInfo binding)
        {
            if (!this.needHtmlNodeAccess)
            {
                return null;
            }

            var targetBinder = binding.TargetBindingInfo;
            if (targetBinder is Binding.CssClassTargetBindingInfo
                || targetBinder is Binding.AttributeTargetBindingInfo
                || targetBinder is Binding.DomEventTargetBindingInfo
                || targetBinder is Binding.StyleTargetBindingInfo)
            {
                return codeGenerator.GetObjectIndexForNode(
                    this,
                    true);
            }

            return null;
        }

        protected override bool IsConstructorMatch(
            ParserContext parserContext,
            MethodReference ctor,
            int startParameterIndex = 0)
        {
            var knownTypes = parserContext.KnownTypes;

            if (ctor.Parameters.Count > startParameterIndex
                && ctor.Parameters[startParameterIndex].ParameterType.IsSameDefinition(
                    knownTypes.ElementRef))
            {
                return base.IsConstructorMatch(
                    parserContext,
                    ctor,
                    startParameterIndex + 1);
            }

            return false;
        }
    }
}
