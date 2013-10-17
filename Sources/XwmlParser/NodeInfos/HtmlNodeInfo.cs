//-----------------------------------------------------------------------
// <copyright file="HtmlNodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using NScript.Converter;
    using NScript.Converter.ExpressionsConverter;
    using NScript.JST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using XwmlParser.Binding;

    /// <summary>
    /// Definition for HtmlNodeInfo
    /// </summary>
    public class HtmlNodeInfo : NodeInfo, IHtmlNodeGenerator
    {
        /// <summary>
        /// The generated node.
        /// </summary>
        HtmlNode generatedNode;

        /// <summary>
        /// List of names of the class.
        /// </summary>
        private IIdentifier[] classNames;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        public HtmlNodeInfo(
            HtmlNode node,
            Tuple<string, string> tagInfo)
            : base(node, tagInfo)
        {
        }

        /// <summary>
        /// Gets or sets the generated node.
        /// </summary>
        /// <value>
        /// The generated node.
        /// </value>
        public HtmlNode GeneratedNode
        {
            get { return this.generatedNode; }

            set
            {
                if (this.generatedNode != null)
                {
                    throw new InvalidOperationException("Can't set generated node more than once");
                }

                this.generatedNode = value;
            }
        }

        /// <summary>
        /// Gets a list of names of the class.
        /// </summary>
        /// <value>
        /// A list of names of the class.
        /// </value>
        public IIdentifier[] ClassNames
        { get { return this.classNames; } }

        /// <summary>
        /// Sets new node and path.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="nodePath"> Full pathname of the node file. </param>
        public void SetNewNodeAndPath(
            HtmlNode node)
        {
            this.generatedNode = node;
        }

        /// <summary>
        /// Parse node.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="parser"> The parser. </param>
        public override void ParseNode(
            TemplateParser parser)
        {
            if (this.Node.NodeType == HtmlNodeType.Text)
            {
                if (BindingParser.IsBindingText(this.Node.InnerText))
                {
                    this.generatedNode = parser.GeneratedDocument.CreateElement("span");
                    this.AddBinder(
                        BindingParser.ParseBinding(
                            new TextContentTargetBinder(
                                parser.DocumentContext.ParserContext.ConverterContext.ClrKnownReferences.String),
                            ((HtmlTextNode)this.Node).Text,
                            parser.DocumentContext,
                            parser.DataContextType,
                            parser.ControlType));
                }
                else
                {
                    string text = ((HtmlTextNode)this.Node).Text;
                    text = text.Trim();
                    if (text.Length == 0)
                    {
                        text = " ";
                    }

                    this.generatedNode = parser.GeneratedDocument.CreateTextNode(text);
                }
            }
            else if (this.Node.NodeType == HtmlNodeType.Element)
            {
                this.generatedNode = parser.GeneratedDocument.CreateElement(this.Node.Name);
                foreach (var attr in this.Node.Attributes)
                {
                    string loweredAttrName = attr.Name.ToLowerInvariant();
                    if (loweredAttrName == "id")
                    {
                        this.PartId = attr.Value;
                    }
                    else if (loweredAttrName.StartsWith("class."))
                    {
                        if (BindingParser.IsBindingText(attr.Value))
                        {
                            string className = attr.Name.Substring("class.".Length);
                            IIdentifier classIdentifier;
                            if (!parser.DocumentContext.TryGetCssClassIdentifier(
                                className,
                                out classIdentifier))
                            {
                                throw new ConverterLocationException(
                                    new Location(
                                        parser.HtmlParser.ResourceName,
                                        attr.Line,
                                        attr.LinePosition),
                                    string.Format("Css Class Name: {0} not found",
                                        className));
                            }

                            this.AddBinder(
                                BindingParser.ParseBinding(
                                    new CssClassTargetBindingInfo(
                                        parser.DocumentContext.ParserContext.ConverterContext.ClrKnownReferences.Boolean,
                                        classIdentifier),
                                    attr.Value,
                                    parser.DocumentContext,
                                    parser.DataContextType,
                                    parser.ControlType));
                        }
                    }
                    else if (loweredAttrName == "class")
                    {
                        if (BindingParser.IsBindingText(attr.Value))
                        {
                            throw new ApplicationException(
                                "class attribute does not support binding. Use class.className (css class binding).");
                        }

                        this.AddClassNames(
                            attr.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                            new Location(
                                parser.HtmlParser.ResourceName,
                                attr.Line,
                                attr.LinePosition),
                            parser.DocumentContext);
                    }
                    else if (loweredAttrName == "style")
                    {
                        // Todo: add support for style binding.
                        this.generatedNode.SetAttributeValue(attr.Name, attr.Value);
                    }
                    else if (BindingParser.IsBindingText(attr.Value))
                    {
                        this.AddBinder(
                            BindingParser.ParseBinding(
                                new AttributeTargetBindingInfo(
                                    parser.DocumentContext.ParserContext.ConverterContext.ClrKnownReferences.Boolean,
                                    attr.Name),
                                attr.Value,
                                parser.DocumentContext,
                                parser.DataContextType,
                                parser.ControlType));
                    }
                    else
                    {
                        this.generatedNode.SetAttributeValue(attr.Name, attr.Value);
                    }
                }
            }
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
            IHtmlNodeGenerator childNodeGenerator = childNode as IHtmlNodeGenerator;
            if (childNodeGenerator == null)
            { return false; }

            this.generatedNode.AppendChild(childNodeGenerator.GeneratedNode);
            this.AddChildNodeInfo(childNode);

            return true;
        }

        /// <summary>
        /// Generates a code.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        public override void GenerateCode(SkinCodeGenerator codeGenerator)
        {
            int objectIndex = -1;
            if (this.Bindings != null
                && this.Bindings.Count > 0)
            {
                objectIndex = codeGenerator.GetObjectIndexForNode(this);
            }

            if (objectIndex >= 0)
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
                                objectIndex)),
                        this.GetCtorExpr(codeGenerator)));

                this.GenerateBindingCode(objectIndex, codeGenerator);
            }
        }

        /// <summary>
        /// Finalize generated node.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        public void FinalizeGeneratedNode(SkinCodeGenerator codeGenerator)
        {
            if (this.classNames != null
                && this.classNames.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int iClass = 0; iClass < this.classNames.Length; iClass++)
                {
                    if (iClass > 0)
                    {
                        sb.Append(' ');
                    }

                    sb.Append(this.classNames[iClass].GetName());
                }

                this.generatedNode.Attributes.Add(
                    "class",
                    sb.ToString());
            }
        }

        /// <summary>
        /// Gets constructor expression.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        /// <returns>
        /// The constructor expression.
        /// </returns>
        private Expression GetCtorExpr(SkinCodeGenerator codeGenerator)
        {
            IIdentifier domIdentifier = codeGenerator.GetDomRootIdentifier();
            var path = SkinCodeGenerator.GetNodePath(
                this.generatedNode,
                null);

            List<Expression> intExpressions = new List<Expression>();
            for (int iPath = 0; iPath < path.Count; iPath++)
            {
                intExpressions.Add(new NumberLiteralExpression(
                    codeGenerator.Scope,
                    path[iPath]));
            }

            var newArrayExpr = new InlineNewArrayInitialization(
                null,
                codeGenerator.Scope,
                intExpressions);

            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    codeGenerator.KnownTypes.ElementFromPathGetter,
                    null,
                    codeGenerator.Scope),
                new Expression[]{
                    new IdentifierExpression(domIdentifier, codeGenerator.Scope),
                    newArrayExpr
                },
                codeGenerator.CodeGenerator.Resolver,
                codeGenerator.CodeGenerator.ScopeManager);
        }

        /// <summary>
        /// Adds the class names.
        /// </summary>
        /// <param name="classNames"> List of names of the class. </param>
        private void AddClassNames(
            string[] classNames,
            Location location,
            DocumentContext documentContext)
        {
            this.classNames = new IIdentifier[classNames.Length];
            for (int iClass = 0; iClass < classNames.Length; iClass++)
            {
                IIdentifier identifier;
                if (!documentContext.TryGetCssClassIdentifier(
                    classNames[iClass], out identifier))
                {
                    throw new ConverterLocationException(
                        location,
                        string.Format("Css Class Name: {0} not found",
                            classNames[iClass]));
                }

                this.classNames[iClass] = identifier;
                identifier.AddUsage(null);
            }
        }

        /*
        * Delete if not needed.
        private List<string> BreakWithBindings(string str)
        {
            List<string> rv = new List<string>();
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (ch == '{')
                {
                    if (i < str.Length - 1 && str[i + 1] != '{')
                    {
                        for (int j = i; j < str.Length - 1; j++)
                        {
                            if (str[j] == '}')
                            {
                                rv.Add(strBuilder.ToString());
                                rv.Add(str.Substring(i + 1, j - i - 1));
                                strBuilder.Clear();
                                i = j;
                                break;
                            }
                        }
                    }
                    else
                    {
                        strBuilder.Append(ch);
                        ++i;
                    }
                }
                else if (ch == '}')
                {
                    if (str[i + 1] != '}')
                    {
                        throw new ApplicationException(
                            string.Format(
                                "String '{0}' has invalid binding construct. double {{ ('{{{{') should be followed by double }} ('}}}}')",
                                str));
                    }

                    strBuilder.Append(ch);
                    ++i;
                }
                else
                {
                    strBuilder.Append(ch);
                }
            }

            rv.Add(strBuilder.ToString());

            return rv;
        }
        */
    }
}
