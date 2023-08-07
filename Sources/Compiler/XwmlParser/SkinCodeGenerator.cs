//-----------------------------------------------------------------------
// <copyright file="SkinCodeGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using HtmlAgilityPack;
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using XwmlParser.NodeInfos;

    /// <summary>
    /// Definition for SkinCodeGenerator
    /// </summary>
    public class SkinCodeGenerator
    {
        /// <summary>
        /// The parser.
        /// </summary>
        TemplateParser parser;

        /// <summary>
        /// The code generator.
        /// </summary>
        CodeGenerator codeGenerator;

        /// <summary>
        /// Information describing the skin node.
        /// </summary>
        SkinNodeInfo skinNodeInfo;

        /// <summary>
        /// The code.
        /// </summary>
        List<Statement> code = new List<Statement>();

        /// <summary>
        /// The bindings.
        /// </summary>
        List<Expression> bindings = new List<Expression>();

        /// <summary>
        /// Identifier for the root HTML node.
        /// </summary>
        IIdentifier rootHtmlNodeIdentifier;

        /// <summary>
        /// Identifier for the object storage.
        /// </summary>
        IIdentifier objectStorageIdentifier;

        /// <summary>
        /// Zero-based index of the data.
        /// </summary>
        int dataIndex = -1;

        /// <summary>
        /// The node to storage index map.
        /// </summary>
        Dictionary<Tuple<NodeInfo, bool>, int> nodeToStorageIndexMap = new Dictionary<Tuple<NodeInfo, bool>, int>();

        /// <summary>
        /// The binder information to index map.
        /// </summary>
        Dictionary<BinderInfo, int> binderInfoToIndexMap = new Dictionary<BinderInfo, int>();

        /// <summary>
        /// The binder information to extra object map.
        /// </summary>
        Dictionary<BinderInfo, int> binderInfoToExtraObjectMap = new Dictionary<BinderInfo, int>();

        /// <summary>
        /// Identifier for the skin factory method.
        /// </summary>
        private SimpleIdentifier skinFactoryMethodIdentifier;

        /// <summary>
        /// The skin storage variable.
        /// </summary>
        private SimpleIdentifier skinStorageVariable;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parser">        The parser. </param>
        /// <param name="codeGenerator"> The code generator. </param>
        public SkinCodeGenerator(
            TemplateParser parser,
            CodeGenerator codeGenerator)
        {
            this.parser = parser;
            this.codeGenerator = codeGenerator;
            this.skinNodeInfo = parser.SkinNodeInfo;
        }

        public TemplateParser Parser
        { get { return this.parser; } }

        public CodeGenerator CodeGenerator
        { get { return this.codeGenerator; } }

        public KnownTemplateTypes KnownTypes
        { get { return this.codeGenerator.ParserContext.KnownTypes; } }

        public void GenerateCodePass1()
        {
            this.Scope = new IdentifierScope(
                this.codeGenerator.ScopeManager.Scope,
                new string[] {
                    "skinFactory",
                    "doc"
                },
                false);

            this.IterateChildNodes(this.skinNodeInfo);
            this.GenerateCodeFinalPass();
        }

        public void GenerateCodeFinalPass()
        {
            if (dataIndex >= 0)
            {
                return;
            }

            this.dataIndex = this.codeGenerator.GetDataStorageIndex(this);
            this.IterateChildNodesForHtml(this.skinNodeInfo);
            this.GenerateSkinFactory();
            this.GenerateSkinGetter();
        }

        public static List<int> GetNodePath(HtmlNode node, HtmlNode rootNode)
        {
            List<int> rv = new List<int>(4);

            while (node.ParentNode != null
                && node != rootNode)
            {
                var parent = node.ParentNode;
                for (int iChild = 0; iChild < parent.ChildNodes.Count; iChild++)
                {
                    if (node == parent.ChildNodes[iChild])
                    {
                        rv.Insert(0, iChild);
                        break;
                    }
                }

                node = parent;
            }

            return rv;
        }

        public HashSet<string> GetUsedClasses()
        {
            var usedClasses = new HashSet<string>();

            foreach (var childNode in skinNodeInfo.ChildNodes)
            {
                IterateChildNodesForClasses(childNode.Node, usedClasses);
            }

            return usedClasses;
        }

        public IdentifierScope Scope { get; set; }

        internal int GetObjectIndexForNode(
            NodeInfo htmlNodeInfo,
            bool domElementIndex = false)
        {
            int rv;
            var tupl = Tuple.Create(htmlNodeInfo, domElementIndex);
            if (this.nodeToStorageIndexMap.TryGetValue(tupl, out rv))
            {
                return rv;
            }

            rv = this.nodeToStorageIndexMap.Count;
            this.nodeToStorageIndexMap[tupl] = rv;

            return rv;
        }

        internal IIdentifier GetDomRootIdentifier()
        {
            if (this.rootHtmlNodeIdentifier == null)
            {
                this.rootHtmlNodeIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                    this.Scope,
                    "htmlRoot",
                    false);
            }

            return this.rootHtmlNodeIdentifier;
        }

        internal IIdentifier GetObjectStorageIdentifier()
        {
            if (this.objectStorageIdentifier == null)
            {
                this.objectStorageIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                    this.Scope,
                    "objStorage",
                    false);
            }

            return this.objectStorageIdentifier;
        }

        internal void AddStatement(Statement statement)
        {
            this.code.Add(statement);
        }

        internal void AddBinding(Expression expression)
        {
            this.bindings.Add(expression);
        }

        private void IterateChildNodes(NodeInfo nodeInfo)
        {
            foreach (var childNode in nodeInfo.ChildNodes)
            {
                try
                {
                    childNode.GenerateCode(this);
                    this.IterateChildNodes(childNode);
                } catch (Exception ex)
                {
                    if (ex is ApplicationException)
                    { throw; }

                    throw new ConverterLocationException(
                        new Location(
                            this.Parser.HtmlParser.ResourceName,
                            childNode.Node.Line,
                            childNode.Node.LinePosition),
                        "Unknown error generating code",
                        ex);
                }
            }
        }

        private void IterateChildNodesForHtml(NodeInfo nodeInfo)
        {
            foreach (var childNode in nodeInfo.ChildNodes)
            {
                IHtmlNodeGenerator htmlNode = childNode as IHtmlNodeGenerator;
                if (htmlNode != null)
                {
                    htmlNode.FinalizeGeneratedNode(this);
                }
            }
        }

        private void GenerateSkinFactory()
        {
            List<Statement> statements = new List<Statement>();
            this.GetInitializerBlock(statements);
            this.code.InsertRange(0, statements);
            this.code.Add(this.GetSkinInstanceCtorExpression());

            this.skinFactoryMethodIdentifier =
                SimpleIdentifier.CreateScopeIdentifier(
                    this.CodeGenerator.ScopeManager.Scope,
                    this.parser.GetUniqueTemplateId() + "_factory",
                    false);

            var factoryFunction = new FunctionExpression(
                null,
                this.CodeGenerator.ScopeManager.Scope,
                this.Scope,
                this.Scope.ParameterIdentifiers,
                skinFactoryMethodIdentifier);

            factoryFunction.AddStatements(this.code);

            this.codeGenerator.AddStatement(
                new ExpressionStatement(
                    null,
                    this.codeGenerator.ScopeManager.Scope,
                    factoryFunction));
        }

        private void GenerateSkinGetter()
        {
            this.skinStorageVariable =
                SimpleIdentifier.CreateScopeIdentifier(
                    codeGenerator.ScopeManager.Scope,
                    this.parser.GetUniqueTemplateId() + "_var",
                    false);

            this.codeGenerator.AddStatement(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(this.skinStorageVariable, codeGenerator.ScopeManager.Scope),
                    new NullLiteralExpression(codeGenerator.ScopeManager.Scope)));

            IdentifierScope methodScope = new IdentifierScope(
                this.codeGenerator.ScopeManager.Scope,
                0);

            var skinGetterFunction =
                new FunctionExpression(
                    null,
                    this.codeGenerator.ScopeManager.Scope,
                    methodScope,
                    methodScope.ParameterIdentifiers,
                    this.codeGenerator.GetGetterMethodIdentifier(this.parser));

            var initialization = ExpressionStatement.CreateAssignmentExpression(
                new IdentifierExpression(
                    this.skinStorageVariable,
                    methodScope),
                new MethodCallExpression(
                    null,
                    methodScope,
                    IdentifierExpression.Create(
                        null,
                        methodScope,
                        this.codeGenerator.Resolver.ResolveFactory(
                            this.codeGenerator.KnownTypes.SkinCtor)),
                    IdentifierExpression.Create(
                        null,
                        methodScope,
                        this.codeGenerator.Resolver.Resolve(this.parser.ControlType)),
                    IdentifierExpression.Create(
                        null,
                        methodScope,
                        this.codeGenerator.Resolver.Resolve(
                            this.parser.DataContextType)),
                    new IdentifierExpression(
                        this.skinFactoryMethodIdentifier,
                        methodScope),
                    new StringLiteralExpression(
                        methodScope,
                        this.dataIndex.ToString())));

            var initializationIfStatement = new IfBlockStatement(
                null,
                methodScope,
                new UnaryExpression(
                    null,
                    methodScope,
                    UnaryOperator.LogicalNot,
                    new IdentifierExpression(skinStorageVariable, methodScope)),
                new ScopeBlock(
                    null,
                    methodScope,
                    new List<Statement>{initialization}),
                null);

            skinGetterFunction.AddStatement(initializationIfStatement);
            skinGetterFunction.AddStatement(
                new ReturnStatement(
                    null,
                    methodScope,
                    new IdentifierExpression(skinStorageVariable, methodScope)));

            this.codeGenerator.AddStatement(
                new ExpressionStatement(
                    null,
                    this.codeGenerator.ScopeManager.Scope,
                    skinGetterFunction));
        }

        private void GetLocalInitializers(
            List<Statement> statements,
            IIdentifier domStore,
            IIdentifier globalStateVariable)
        {
            statements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(
                        this.GetDomRootIdentifier(),
                        this.Scope),
                    new MethodCallExpression(
                        null,
                        this.Scope,
                        new IndexExpression(
                            null,
                            this.Scope,
                            new IndexExpression(
                                null,
                                this.Scope,
                                new IdentifierExpression(
                                    domStore,
                                    this.Scope),
                                new NumberLiteralExpression(
                                    this.Scope,
                                    this.dataIndex)),
                            new StringLiteralExpression(
                                this.Scope,
                                "cloneNode")),
                        new BooleanLiteralExpression(
                            this.Scope,
                            true))));

            statements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(
                        this.GetObjectStorageIdentifier(),
                        this.Scope),
                    new NewArrayExpression(
                        null,
                        this.Scope,
                        new NumberLiteralExpression(
                            this.Scope,
                            this.nodeToStorageIndexMap.Count))));
        }

        private void GetInitializerBlock(List<Statement> statements)
        {
            IIdentifier globalStateVariable = this.codeGenerator.GetGlobalStateVariable();
            IIdentifier domInitializerMetod = this.codeGenerator.GetDocumentStorageGetter();
            IIdentifier domStore = SimpleIdentifier.CreateScopeIdentifier(
                this.Scope,
                "domStore",
                false);

            // if (!(domStore = DomStorageInitializer(doc))[iDoc])
            Expression checkStateInitialized =
                new UnaryExpression(
                    null,
                    this.Scope,
                    UnaryOperator.LogicalNot,
                    new IndexExpression(
                        null,
                        this.Scope,
                        new BinaryExpression(
                            null,
                            this.Scope,
                            BinaryOperator.Assignment,
                            new IdentifierExpression(
                                domStore,
                                this.Scope),
                            new MethodCallExpression(
                                null,
                                this.Scope,
                                new IdentifierExpression(
                                    domInitializerMetod,
                                    this.Scope),
                                new IdentifierExpression(
                                    this.Scope.ParameterIdentifiers[1],
                                    this.Scope))),
                        new NumberLiteralExpression(
                            this.Scope,
                            this.dataIndex)));

            IIdentifier documentIdentifier = this.Scope.ParameterIdentifiers[1];

            List<Statement> scopeStatements = new List<Statement>();
            scopeStatements.Add(ExpressionStatement.CreateAssignmentExpression(
                new IndexExpression(
                    null,
                    this.Scope,
                    new IdentifierExpression(
                        domStore,
                        this.Scope),
                    new NumberLiteralExpression(
                        this.Scope,
                        this.dataIndex)),
                new MethodCallExpression(
                    null,
                    this.Scope,
                    new IndexExpression(
                        null,
                        this.Scope,
                        new IdentifierExpression(
                            documentIdentifier,
                            this.Scope),
                        new StringLiteralExpression(
                            this.Scope,
                            "createElement")),
                    new StringLiteralExpression(
                        this.Scope,
                        "div"))));

            scopeStatements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        this.Scope,
                        new IndexExpression(
                            null,
                            this.Scope,
                            new IdentifierExpression(
                                domStore,
                                this.Scope),
                            new NumberLiteralExpression(
                                this.Scope,
                                this.dataIndex)),
                        new StringLiteralExpression(
                            this.Scope,
                            "innerHTML")),
                    new StringLiteralExpression(
                        this.Scope,
                        this.skinNodeInfo.GeneratedNode.InnerHtml)));

            scopeStatements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        this.Scope,
                        new IdentifierExpression(
                            globalStateVariable,
                            this.Scope),
                        new NumberLiteralExpression(
                            this.Scope,
                            this.dataIndex)),
                    new ConditionalOperatorExpression(
                        null,
                        this.Scope,
                        new IndexExpression(
                            null,
                            this.Scope,
                            new IdentifierExpression(
                                globalStateVariable,
                                this.Scope),
                            new NumberLiteralExpression(
                                this.Scope,
                                this.dataIndex)),
                        new IndexExpression(
                            null,
                            this.Scope,
                            new IdentifierExpression(
                                globalStateVariable,
                                this.Scope),
                            new NumberLiteralExpression(
                                this.Scope,
                                this.dataIndex)),
                        new InlineNewArrayInitialization(
                            null,
                            this.Scope,
                            this.bindings))));

            ScopeBlock initializationBlock = new ScopeBlock(
                null,
                this.Scope,
                scopeStatements);

            statements.Add(
                new IfBlockStatement(
                    null,
                    this.Scope,
                    checkStateInitialized,
                    initializationBlock,
                    null));

            this.GetLocalInitializers(statements, domStore, globalStateVariable);
        }

        private Statement GetSkinInstanceCtorExpression()
        {
            List<Expression> uiElemArrayExpression = new List<Expression>();
            foreach (var kvPair in this.nodeToStorageIndexMap)
            {
                if (kvPair.Key.Item1 is UIElementNodeInfo
                    && !kvPair.Key.Item2)
                {
                    uiElemArrayExpression.Add(
                        new NumberLiteralExpression(
                            this.Scope,
                            kvPair.Value));
                }
            }

            return new ReturnStatement(
                null,
                this.Scope,
                new MethodCallExpression(
                    null,
                    this.Scope,
                    IdentifierExpression.Create(
                        null,
                        this.Scope,
                        this.CodeGenerator.Resolver.ResolveFactory(
                            this.CodeGenerator.KnownTypes.SkinInstanceCtor)),
                    // Skin
                    new IdentifierExpression(
                        this.Scope.ParameterIdentifiers[0],
                        this.Scope),
                    // Root Html Node
                    new IdentifierExpression(
                        this.rootHtmlNodeIdentifier,
                        this.Scope),
                    // UIElements
                    uiElemArrayExpression == null
                        ? (Expression)new NullLiteralExpression(this.Scope)
                        : new InlineNewArrayInitialization(
                            null,
                            this.Scope,
                            uiElemArrayExpression),
                    // All Objects for binding
                    new IdentifierExpression(
                        this.objectStorageIdentifier, this.Scope),
                    // Binders
                    new IndexExpression(
                        null,
                        this.Scope,
                        new IdentifierExpression(
                            this.codeGenerator.GetGlobalStateVariable(),
                            this.Scope),
                        new NumberLiteralExpression(
                            this.Scope,
                            this.codeGenerator.GetDataStorageIndex(this))),
                    this.skinNodeInfo.GetPartDictionary(this),
                    new NumberLiteralExpression(
                        this.Scope,
                        this.binderInfoToIndexMap.Count),
                    new NumberLiteralExpression(
                        this.Scope,
                        this.binderInfoToExtraObjectMap.Count)));
        }

        private void IterateChildNodesForClasses(HtmlNode node, HashSet<string> usedClasses)
        {
            if (node.HasAttributes)
            {
                //check if "class" attribute is present
                if (node.Attributes["class"] != null)
                {
                    var classes = node.Attributes["class"].Value;

                    if (classes.Contains(' '))
                    {
                        usedClasses.UnionWith(classes.Split(' ').ToList());
                    }
                    else
                    {
                        usedClasses.Add(classes);
                    }
                }

                foreach (var kvpair in node.Attributes)
                {
                    //check if contional class attribute is present eg. "class.hidden = {<conditon>}"
                    var key = kvpair.OriginalName;
                    var value = kvpair.Value.Trim();

                    if (key.StartsWith("class."))
                    {
                        usedClasses.Add(key.Split('.')[1]);
                    }

                    //check if ItemCssClassName property is present
                    if (key == "ItemCssClassName")
                    {
                        if(value.Contains(" "))
                        {
                            usedClasses.UnionWith(value.Split(" "));
                        }
                        else
                        {
                            usedClasses.Add(value);
                        }
                    }
                }
            }

            if (node.ChildNodes.Count == 0)
            {
                return;
            }

            foreach (var child in node.ChildNodes)
            {
                IterateChildNodesForClasses(child, usedClasses);
            }
        }

        internal int GetBinderIndex(BinderInfo binderInfo)
        {
            int rv;
            if (!this.binderInfoToIndexMap.TryGetValue(binderInfo, out rv))
            {
                rv = this.binderInfoToIndexMap.Count;
                this.binderInfoToIndexMap.Add(
                    binderInfo,
                    rv);
            }

            return rv;
        }

        internal int GetExtraObjectIndex(BinderInfo binderInfo)
        {
            int rv;
            if (!this.binderInfoToExtraObjectMap.TryGetValue(binderInfo, out rv))
            {
                rv = this.binderInfoToExtraObjectMap.Count;
                this.binderInfoToExtraObjectMap.Add(
                    binderInfo,
                    rv);
            }

            return rv;
        }
    }
}
