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
        /// true to need dom access.
        /// </summary>
        bool needDomAccess = false;

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

        public IIdentifier[] ClassNames
        {
            get { return this.classNames; }
            set { this.classNames = value; }
        }

        public static bool IsHtmlAttribute(
            HtmlAttribute attr)
        {
            return !attr.OriginalName.Contains(":")
                && !char.IsUpper(attr.OriginalName, 0);
        }

        public static BinderInfo ParseHtmlAttribute(
            IHtmlNodeGenerator htmlNodeGenerator,
            HtmlAgilityPack.HtmlAttribute attr,
            TemplateParser parser)
        {
            if (attr.OriginalName.Contains(":")
                || char.IsUpper(attr.OriginalName, 0))
            {
                return null;
            }

            string attrName = attr.OriginalName;
            BinderInfo binderInfo = null;
            if (attrName == "id")
            {
                parser.SkinNodeInfo.RegisterPart(
                    attr.Value,
                    htmlNodeGenerator);

                return null;
            }

            if (attrName.StartsWith("class."))
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

                    binderInfo =
                        BindingParser.ParseBinding(
                            new CssClassTargetBindingInfo(
                                parser.DocumentContext.ParserContext.ConverterContext.ClrKnownReferences.Boolean,
                                classIdentifier),
                            attr.Value,
                            parser.DocumentContext,
                            parser.DataContextType,
                            parser.ControlType);
                }
            }
            else if (attrName == "class")
            {
                if (BindingParser.IsBindingText(attr.Value))
                {
                    throw new ApplicationException(
                        "class attribute does not support binding. Use class.className (css class binding).");
                }

                var classNames = attr.Value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var classIdentifiers = new IIdentifier[classNames.Length];
                for (int iClass = 0; iClass < classNames.Length; iClass++)
                {
                    IIdentifier identifier;
                    if (!parser.DocumentContext.TryGetCssClassIdentifier(
                        classNames[iClass], out identifier))
                    {
                        throw new ConverterLocationException(
                            new Location(
                                parser.HtmlParser.ResourceName,
                                attr.Line,
                                attr.LinePosition),
                            string.Format("Css Class Name: {0} not found",
                                classNames[iClass]));
                    }

                    classIdentifiers[iClass] = identifier;
                    identifier.AddUsage(null);
                }

                htmlNodeGenerator.ClassNames = classIdentifiers;
            }
            else if (attrName.StartsWith("style."))
            {
                binderInfo =
                    BindingParser.ParseBinding(
                        new StyleTargetBindingInfo(
                            parser.DocumentContext.ParserContext.ConverterContext.ClrKnownReferences.Boolean,
                            attr.OriginalName.Substring("style.".Length)),
                        attr.Value,
                        parser.DocumentContext,
                        parser.DataContextType,
                        parser.ControlType);
            }
            else if (attrName == "style")
            {
                // Todo: add support for style binding.
                htmlNodeGenerator.GeneratedNode.SetAttributeValue(attr.OriginalName, attr.Value);
            }
            else if (attrName.StartsWith("event."))
            {
                binderInfo =
                    BindingParser.ParseBinding(
                        new DomEventTargetBindingInfo(
                            parser.DocumentContext.ParserContext.KnownTypes.DomEventType2,
                            attr.OriginalName.Substring("style.".Length)),
                        attr.Value,
                        parser.DocumentContext,
                        parser.DataContextType,
                        parser.ControlType);
            }
            else if (BindingParser.IsBindingText(attr.Value))
            {
                binderInfo =
                    BindingParser.ParseBinding(
                        new AttributeTargetBindingInfo(
                            parser.DocumentContext.ParserContext.ConverterContext.ClrKnownReferences.Boolean,
                            attr.OriginalName),
                        attr.Value,
                        parser.DocumentContext,
                        parser.DataContextType,
                        parser.ControlType);
            }
            else
            {
                htmlNodeGenerator.GeneratedNode.SetAttributeValue(attr.OriginalName, attr.Value);
            }

            return binderInfo;
        }

        public static void FinalizeClassNamesInGeneratedNode(
            HtmlNode node,
            IList<IIdentifier> classNames)
        {
            if (classNames == null
                || classNames.Count == 0)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            for (int iClass = 0, classNamesLength = classNames.Count; iClass < classNamesLength; iClass++)
            {
                if (iClass > 0)
                {
                    sb.Append(' ');
                }

                sb.Append(classNames[iClass].GetName());
            }

            node.Attributes.Add(
                "class",
                sb.ToString());
        }

        public static Expression GetDomNodeExpression(
            SkinCodeGenerator codeGenerator,
            HtmlNode generatedNode)
        {
            IIdentifier domIdentifier = codeGenerator.GetDomRootIdentifier();
            var path = SkinCodeGenerator.GetNodePath(
                generatedNode,
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
                    this.needDomAccess = true;
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
                this.generatedNode = parser.GeneratedDocument.CreateElement(this.Node.OriginalName);
                foreach (var attr in this.Node.Attributes)
                {
                    var binderInfo = 
                        HtmlNodeInfo.ParseHtmlAttribute(
                            this,
                            attr,
                            parser);

                    if (binderInfo != null)
                    {
                        this.needDomAccess = true;
                        this.AddBinder(binderInfo);
                    }
                }
            }
        }

        public void MarkAsPart(bool isDomPart)
        {
            this.needDomAccess = true;
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
            if (this.needDomAccess)
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
                        HtmlNodeInfo.GetDomNodeExpression(
                            codeGenerator,
                            this.generatedNode)));

                this.GenerateBindingCode(objectIndex, codeGenerator);
            }
        }

        /// <summary>
        /// Finalize generated node.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        public void FinalizeGeneratedNode(SkinCodeGenerator codeGenerator)
        {
            HtmlNodeInfo.FinalizeClassNamesInGeneratedNode(
                this.generatedNode,
                this.ClassNames);
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
            return HtmlNodeInfo.GetDomNodeExpression(
                codeGenerator,
                this.generatedNode);
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
