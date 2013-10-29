//-----------------------------------------------------------------------
// <copyright file="CodeGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter.ExpressionsConverter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using XwmlParser.NodeInfos;

    /// <summary>
    /// Definition for CodeGenerator.
    /// </summary>
    public class CodeGenerator
    {
        /// <summary>
        /// Manager for scope.
        /// </summary>
        private RuntimeScopeManager scopeManager;

        /// <summary>
        /// The known references.
        /// </summary>
        private KnownTemplateTypes knownReferences;

        /// <summary>
        /// Context for the parser.
        /// </summary>
        private ParserContext parserContext;

        /// <summary>
        /// The CSS name scope.
        /// </summary>
        private IdentifierScope cssNameScope = new IdentifierScope(false);

        /// <summary>
        /// The text content setter.
        /// </summary>
        private IIdentifier textContentSetter;

        /// <summary>
        /// The attribute setter.
        /// </summary>
        private IIdentifier attributeSetter;

        /// <summary>
        /// The CSS class setter.
        /// </summary>
        private IIdentifier cssClassSetter;

        /// <summary>
        /// Identifier for the storage array.
        /// </summary>
        private IIdentifier storageArrayIdentifier;

        /// <summary>
        /// Identifier for the HTML storage initializer.
        /// </summary>
        private IIdentifier htmlStorageInitializerIdentifier;

        private IIdentifier documentStateStorage;

        private IIdentifier documentStorageGetter;

        /// <summary>
        /// List of identifiers for the template getters.
        /// </summary>
        Dictionary<TemplateParser, IIdentifier> templateGetterIds =
            new Dictionary<TemplateParser, IIdentifier>();

        /// <summary>
        /// Identifier for the templates by.
        /// </summary>
        Dictionary<string, TemplateParser> templatesById =
            new Dictionary<string, TemplateParser>();

        /// <summary>
        /// The skin code generators.
        /// </summary>
        Dictionary<TemplateParser, SkinCodeGenerator> skinCodeGenerators =
            new Dictionary<TemplateParser, SkinCodeGenerator>();

        /// <summary>
        /// The skin code generator storage indexs.
        /// </summary>
        Dictionary<SkinCodeGenerator, int> skinCodeGeneratorStorageIndexs =
            new Dictionary<SkinCodeGenerator, int>();

        /// <summary>
        /// Property -> Value Getter map.
        /// </summary>
        Dictionary<List<MemberReference>, IIdentifier> propertyToGetterMap =
            new Dictionary<List<MemberReference>, IIdentifier>(
                new ListEqualityComparer<MemberReference>(MemberReferenceComparer.Instance));

        /// <summary>
        /// The property setter map.
        /// </summary>
        Dictionary<MemberReference, IIdentifier> propertySetterMap =
            new Dictionary<MemberReference, IIdentifier>(
                MemberReferenceComparer.Instance);

        /// <summary>
        /// Property Path Name -> Identifier for array map;
        /// </summary>
        Dictionary<IList<string>, IIdentifier> propertyPathNamesMap =
            new Dictionary<IList<string>,IIdentifier>(
                new ListEqualityComparer<string>(EqualityComparer<string>.Default));

        /// <summary>
        /// Property Path Getter Map
        /// </summary>
        Dictionary<IList<MemberReference>, IIdentifier> propertyPathGetterMap =
            new Dictionary<IList<MemberReference>, IIdentifier>(
                new ListEqualityComparer<MemberReference>(MemberReferenceComparer.Instance));

        /// <summary>
        /// Delegate getter map.
        /// </summary>
        Dictionary<Tuple<IIdentifier, MethodReference>, IIdentifier> delegateGetterMap =
            new Dictionary<Tuple<IIdentifier, MethodReference>, IIdentifier>(
                new TupleEqualityComparer<IIdentifier, MethodReference>(
                    EqualityComparer<IIdentifier>.Default,
                    MemberReferenceComparer.Instance));

        Dictionary<HtmlParser, IIdentifier> cssInitializerMapping =
            new Dictionary<HtmlParser, IIdentifier>();

        Dictionary<HtmlParser, bool> cssInitializerCodeGenerated =
            new Dictionary<HtmlParser, bool>();

        /// <summary>
        /// The style setter mapping.
        /// </summary>
        Dictionary<string, IIdentifier> styleSetterMapping =
            new Dictionary<string, IIdentifier>();

        /// <summary>
        /// The generated code.
        /// </summary>
        List<Statement> generatedCode = new List<Statement>();

        /// <summary>
        /// The templates to parser.
        /// </summary>
        Queue<TemplateParser> templatesToParse = new Queue<TemplateParser>();

        /// <summary>
        /// The resource map.
        /// </summary>
        Dictionary<string, EmbeddedResource> resourceMap =
            new Dictionary<string, EmbeddedResource>();

        /// <summary>
        /// The CSS registry methods.
        /// </summary>
        Dictionary<DocumentContext, IIdentifier> cssRegistryMethods =
            new Dictionary<DocumentContext, IIdentifier>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="scopeManager"> Manager for scope. </param>
        public CodeGenerator(RuntimeScopeManager scopeManager, KnownTemplateTypes knownReferences)
        {
            this.scopeManager = scopeManager;
            this.knownReferences = knownReferences;
            foreach (var module in this.scopeManager.Context.ClrContext.Modules)
            {
                foreach (var resource in module.Resources)
                {
                    EmbeddedResource embeddedResource = resource as EmbeddedResource;
                    if (embeddedResource != null
                        && (resource.Name.EndsWith(".html") || resource.Name.EndsWith(".htm")))
                    {
                        resourceMap.Add(resource.Name, embeddedResource);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a context for the parser.
        /// </summary>
        /// <value>
        /// The parser context.
        /// </value>
        public ParserContext ParserContext
        {
            get { return this.parserContext; }

            set { this.parserContext = value; }
        }

        /// <summary>
        /// Gets the resolver.
        /// </summary>
        /// <value>
        /// The resolver.
        /// </value>
        public IClrResolver ClrResolver
        { get { return this.parserContext.ClrResolver; } }

        /// <summary>
        /// Gets the resolver.
        /// </summary>
        /// <value>
        /// The resolver.
        /// </value>
        public IResolver Resolver
        { get { return this.parserContext.JsResolver; } }

        /// <summary>
        /// Gets the manager for scope.
        /// </summary>
        /// <value>
        /// The scope manager.
        /// </value>
        public RuntimeScopeManager ScopeManager
        { get { return this.scopeManager; } }

        /// <summary>
        /// Gets a list of types of the knowns.
        /// </summary>
        /// <value>
        /// A list of types of the knowns.
        /// </value>
        public KnownTemplateTypes KnownTypes
        { get { return this.parserContext.KnownTypes; } }

        /// <summary>
        /// Gets property setter expression.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="runtimeScopeManager"> Manager for runtime scope. </param>
        /// <param name="scope">               The scope. </param>
        /// <param name="propertyReference">   The property reference. </param>
        /// <param name="resolver">            The resolver. </param>
        /// <param name="instanceExpression">  The instance expression. </param>
        /// <param name="valueExpression">     The value expression. </param>
        /// <returns>
        /// The property setter expression.
        /// </returns>
        internal static Expression GetPropertySetterExpression(
            RuntimeScopeManager runtimeScopeManager,
            IdentifierScope scope,
            PropertyReference propertyReference,
            IResolver resolver,
            Expression instanceExpression,
            Expression valueExpression)
        {
            if (propertyReference == null)
            {
                throw new InvalidOperationException();
            }

            MethodReference methodRef = propertyReference.Resolve().SetMethod;
            if (methodRef == null)
            {
                throw new InvalidOperationException();
            }
            
            methodRef = methodRef.FixGenericArguments(
                propertyReference.DeclaringType);

            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    instanceExpression,
                    methodRef,
                    methodRef.Resolve().IsVirtual),
                new Expression[] {
                    valueExpression
                },
                resolver,
                runtimeScopeManager);
        }

        /// <summary>
        /// Gets field setter expression.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="runtimeScopeManager"> Manager for runtime scope. </param>
        /// <param name="scope">               The scope. </param>
        /// <param name="propertyReference">   The property reference. </param>
        /// <param name="resolver">            The resolver. </param>
        /// <param name="instanceExpression">  The instance expression. </param>
        /// <param name="valueExpression">     The value expression. </param>
        /// <returns>
        /// The field setter expression.
        /// </returns>
        internal static Expression GetFieldSetterExpression(
            RuntimeScopeManager runtimeScopeManager,
            IdentifierScope scope,
            FieldReference propertyReference,
            IResolver resolver,
            Expression instanceExpression,
            Expression valueExpression)
        {
            if (propertyReference == null)
            {
                throw new InvalidOperationException();
            }

            return new BinaryExpression(
                null,
                scope,
                BinaryOperator.Assignment,
                new IndexExpression(
                    null,
                    scope,
                    instanceExpression,
                    new IdentifierExpression(
                        resolver.Resolve(propertyReference),
                        scope)),
                valueExpression);
        }

        /// <summary>
        /// Gets template getter identifier.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="templateParserResourceId"> Identifier for the template parser resource. </param>
        /// <returns>
        /// The template getter identifier.
        /// </returns>
        public TemplateParser GetTemplateParser(string templateParserResourceId)
        {
            TemplateParser rv;
            if (this.templatesById.TryGetValue(templateParserResourceId, out rv))
            {
                return rv;
            }

            string[] templateNameSplits = templateParserResourceId.Split(':');
            HtmlParser htmlParser = this.parserContext.GetHtmlParser(templateNameSplits[0]);

            if (htmlParser == null)
            {
                EmbeddedResource resource;
                if (this.resourceMap.TryGetValue(templateNameSplits[0], out resource))
                {
                    var document = new HtmlAgilityPack.HtmlDocument();
                    using (System.IO.Stream stream = resource.GetResourceStream())
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        document.Load(reader);
                    }

                    htmlParser = new HtmlParser(
                        templateNameSplits[0],
                        document,
                        this.parserContext);

                    this.parserContext.RegisterHtmlParser(
                        templateNameSplits[0],
                        htmlParser);
                }

                if (htmlParser == null)
                {
                    throw new ApplicationException(
                        string.Format("Can't find template:'{0}'", templateParserResourceId));
                }
            }

            TemplateParser templateParser = htmlParser.GetTemplateParser(templateNameSplits.Length == 2 ? templateNameSplits[1] : null);
            this.templatesToParse.Enqueue(templateParser);

            return templateParser;
        }

        /// <summary>
        /// Gets template getter identifier.
        /// </summary>
        /// <param name="templateParserResourceId"> Identifier for the template parser resource. </param>
        /// <returns>
        /// The template getter identifier.
        /// </returns>
        public IIdentifier GetTemplateGetterIdentifier(string templateParserResourceId)
        {
            return this.GetGetterMethodIdentifier(
                this.GetTemplateParser(
                    templateParserResourceId));
        }

        /// <summary>
        /// Gets getter method identifier.
        /// </summary>
        /// <param name="templateParser"> The template parser. </param>
        /// <returns>
        /// The getter method identifier.
        /// </returns>
        public IIdentifier GetGetterMethodIdentifier(TemplateParser templateParser)
        {
            IIdentifier rv;
            if (this.templateGetterIds.TryGetValue(templateParser, out rv))
            {
                return rv;
            }

            rv = SimpleIdentifier.CreateScopeIdentifier(
                this.scopeManager.Scope,
                templateParser.GetUniqueTemplateId(),
                false);

            this.templateGetterIds.Add(templateParser, rv);

            return rv;
        }

        /// <summary>
        /// Iterate parsing.
        /// </summary>
        public void IterateParsing()
        {
            while (this.templatesToParse.Count > 0)
            {
                TemplateParser templateParser =
                    this.templatesToParse.Dequeue();

                templateParser.Parse();

                if (templateParser.SkinNodeInfo != null)
                {
                    var skinCodeGenerator = new SkinCodeGenerator(
                        templateParser,
                        this);

                    skinCodeGenerator.GenerateCodePass1();

                    this.skinCodeGenerators.Add(templateParser, skinCodeGenerator);
                    this.skinCodeGeneratorStorageIndexs.Add(
                        skinCodeGenerator,
                        this.skinCodeGeneratorStorageIndexs.Count);
                }
            }
        }

        /// <summary>
        /// Gets all template statements.
        /// </summary>
        /// <returns>
        /// all template statements.
        /// </returns>
        public List<Statement> GetAllTemplateStatements()
        {
            this.parserContext.CompressCssNames();

            this.GenerateCode();

            foreach (var kvPair in this.skinCodeGenerators)
            {
                kvPair.Value.GenerateCodeFinalPass();
            }

            return this.generatedCode;
        }

        /// <summary>
        /// Gets getter for property path.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <exception cref="NotSupportedException">     Thrown when the requested operation is not
        ///     supported. </exception>
        /// <param name="path"> Full pathname of the file. </param>
        /// <returns>
        /// The getter for property path.
        /// </returns>
        internal IIdentifier GetGetterForPropertyPath(IList<MemberReference> path)
        {
            IIdentifier rv;
            if (this.propertyPathGetterMap.TryGetValue(path, out rv))
            {
                return rv;
            }

            IdentifierScope methodScope = new IdentifierScope(
                this.scopeManager.Scope,
                new string[] {
                    "src"
                },
                false);

            Expression expression = new IdentifierExpression(
                methodScope.ParameterIdentifiers[0],
                methodScope);

            for (int iPath = 0; iPath < path.Count; iPath++)
            {
                MemberReference memberRef = path[iPath];
                FieldReference fieldReference = memberRef as FieldReference;

                if (fieldReference != null)
                {
                    if (fieldReference.Resolve().IsStatic)
                    {
                        if (iPath == 0)
                        {
                            expression = IdentifierExpression.Create(
                                null,
                                methodScope,
                                this.Resolver.ResolveStaticMember(fieldReference),
                                -1);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        expression = new IndexExpression(
                            null,
                            methodScope,
                            expression,
                            new IdentifierExpression(
                                this.Resolver.Resolve(fieldReference),
                                methodScope));
                    }
                    continue;
                }

                PropertyReference propertyReference = memberRef as PropertyReference;
                MethodReference methodRef = null;
                if (propertyReference != null)
                {
                    methodRef = propertyReference.Resolve().GetMethod.FixGenericArguments(
                        propertyReference.DeclaringType);

                    if (propertyReference.Resolve().IsStatic())
                    {
                        if (iPath == 0)
                        {
                            expression =
                                MethodCallExpressionConverter.CreateMethodCallExpression(
                                    new MethodCallContext(
                                        null,
                                        methodRef,
                                        false),
                                    null,
                                    this.Resolver,
                                    this.scopeManager);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        expression =
                            MethodCallExpressionConverter.CreateMethodCallExpression(
                                new MethodCallContext(
                                    expression,
                                    methodRef,
                                    methodRef.Resolve().IsVirtual),
                                null,
                                this.Resolver,
                                this.scopeManager);
                    }

                    continue;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            FunctionExpression methodExpression = new FunctionExpression(
                null,
                this.scopeManager.Scope,
                methodScope,
                methodScope.ParameterIdentifiers,
                SimpleIdentifier.CreateScopeIdentifier(
                    this.scopeManager.Scope,
                    "getter",
                    false));

            methodExpression.AddStatement(
                new ExpressionStatement(
                    null,
                    methodScope,
                    expression));

            this.generatedCode.Add(new ExpressionStatement(
                null,
                this.scopeManager.Scope,
                methodExpression));

            this.propertyPathGetterMap.Add(path, methodExpression.Name);
            return methodExpression.Name;
        }

        /// <summary>
        /// Gets setter for property.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="property"> The property. </param>
        /// <returns>
        /// The setter for property.
        /// </returns>
        internal IIdentifier GetSetterForProperty(MemberReference property)
        {
            IIdentifier rv;
            if (this.propertySetterMap.TryGetValue(property, out rv))
            {
                return rv;
            }

            IdentifierScope methodScope = new IdentifierScope(
                this.scopeManager.Scope,
                new string[]{
                    "tar",
                    "val"
                },
                false);

            PropertyReference propertyReference = property as PropertyReference;
            Expression setterExpression = null;
            if (propertyReference != null)
            {
                setterExpression = CodeGenerator.GetPropertySetterExpression(
                    this.scopeManager,
                    methodScope,
                    property as PropertyReference,
                    this.Resolver,
                    new IdentifierExpression(
                        methodScope.ParameterIdentifiers[0],
                        methodScope),
                    new IdentifierExpression(
                        methodScope.ParameterIdentifiers[1],
                        methodScope));
            }
            else
            {
                FieldReference fieldReference = property as FieldReference;
                if (fieldReference != null)
                {
                    setterExpression = CodeGenerator.GetFieldSetterExpression(
                        this.scopeManager,
                        methodScope,
                        fieldReference,
                        this.Resolver,
                        new IdentifierExpression(
                            methodScope.ParameterIdentifiers[0],
                            methodScope),
                        new IdentifierExpression(
                            methodScope.ParameterIdentifiers[1],
                            methodScope));
                }
                else
                {
                    throw new InvalidProgramException();
                }
            }

            FunctionExpression method = new FunctionExpression(
                null,
                this.scopeManager.Scope,
                methodScope,
                methodScope.ParameterIdentifiers,
                SimpleIdentifier.CreateScopeIdentifier(
                    this.scopeManager.Scope,
                    "setter",
                    false));

            method.AddStatement(
                new ExpressionStatement(
                    null,
                    methodScope,
                    setterExpression));

            this.generatedCode.Add(
                new ExpressionStatement(
                    null,
                    this.scopeManager.Scope,
                    method));

            this.propertySetterMap.Add(property, method.Name);

            return method.Name;
        }

        /// <summary>
        /// Gets getter for delegate.
        /// </summary>
        /// <param name="identifier">      The identifier. </param>
        /// <param name="methodReference"> The method reference. </param>
        /// <returns>
        /// The getter for delegate.
        /// </returns>
        internal IIdentifier GetGetterForDelegate(
            IIdentifier identifier,
            MethodReference methodReference)
        {
            IIdentifier rv;
            var id = Tuple.Create(identifier, methodReference);
            if (this.delegateGetterMap.TryGetValue(id, out rv))
            {
                return rv;
            }

            IdentifierScope methodScope = new IdentifierScope(
                this.scopeManager.Scope,
                new string[] {
                    "src"
                },
                false);

            IdentifierScope delegateScope = new IdentifierScope(
                methodScope,
                methodReference.Parameters.Count);

            Expression expression = new IdentifierExpression(
                methodScope.ParameterIdentifiers[0],
                delegateScope);

            if (identifier != null)
            {
                expression = new MethodCallExpression(
                    null,
                    delegateScope,
                    new IdentifierExpression(
                        identifier,
                        delegateScope),
                    expression);
            }

            List<Expression> args = null;
            if (delegateScope.ParameterIdentifiers.Count > 0)
            {
                args = new List<Expression>();
                for (int iArg = 0; iArg < delegateScope.ParameterIdentifiers.Count; iArg++)
                {
                    args.Add(
                        new IdentifierExpression(
                            delegateScope.ParameterIdentifiers[iArg], delegateScope));
                }
            }

            expression =
                MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(
                        expression,
                        methodReference,
                        methodReference.Resolve().IsVirtual),
                    args,
                    this.Resolver,
                    this.scopeManager);

            var delegateExpression = new FunctionExpression(
                null,
                methodScope,
                delegateScope,
                delegateScope.ParameterIdentifiers,
                null);

            delegateExpression.AddStatement(
                new ReturnStatement(
                    null,
                    delegateScope,
                    expression));

            FunctionExpression methodExpression = new FunctionExpression(
                null,
                this.scopeManager.Scope,
                methodScope,
                methodScope.ParameterIdentifiers,
                SimpleIdentifier.CreateScopeIdentifier(methodScope, "delgateGetter", false));

            methodExpression.AddStatement(
                new ReturnStatement(
                    null,
                    methodScope,
                    delegateExpression));

            this.generatedCode.Add(
                new ExpressionStatement(
                    null,
                    this.scopeManager.Scope,
                    methodExpression));

            this.delegateGetterMap.Add(id, methodExpression.Name);
            return methodExpression.Name;
        }

        /// <summary>
        /// Gets style setter.
        /// </summary>
        /// <param name="styleName"> Name of the style. </param>
        /// <returns>
        /// The style setter.
        /// </returns>
        internal IIdentifier GetStyleSetter(string styleName)
        {
            IIdentifier rv;
            if (this.styleSetterMapping.TryGetValue(styleName, out rv))
            {
                return rv;
            }

            IdentifierScope methodScope = new IdentifierScope(
                this.scopeManager.Scope,
                new string[]{
                    "dom",
                    "val"
                },
                false);

            Statement setterStatement =
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        methodScope,
                        new IndexExpression(
                            null,
                            methodScope,
                            new IdentifierExpression(
                                methodScope.ParameterIdentifiers[0],
                                methodScope),
                            new StringLiteralExpression(
                                methodScope,
                                "style")),
                        new StringLiteralExpression(
                            methodScope,
                            styleName)),
                    new IdentifierExpression(
                        methodScope.ParameterIdentifiers[1],
                        methodScope));

            FunctionExpression method = new FunctionExpression(
                null,
                this.scopeManager.Scope,
                methodScope,
                methodScope.ParameterIdentifiers,
                SimpleIdentifier.CreateScopeIdentifier(
                    this.scopeManager.Scope,
                    "styleSetter",
                    false));

            method.AddStatement(setterStatement);

            this.generatedCode.Add(new ExpressionStatement(
                null,
                this.scopeManager.Scope,
                method));

            this.styleSetterMapping.Add(styleName, method.Name);

            return method.Name;
        }

        /// <summary>
        /// Gets attribute setter.
        /// </summary>
        /// <param name="attributeName"> Name of the attribute. </param>
        /// <returns>
        /// The attribute setter.
        /// </returns>
        internal IIdentifier GetAttributeSetter()
        {
            if (this.attributeSetter != null)
            {
                return this.attributeSetter;
            }

            this.attributeSetter =
                new CompoundIdentifier(
                    this.Resolver.ResolveStaticMember(
                        this.KnownTypes.AttributeSetter));

            return this.attributeSetter;
        }

        /// <summary>
        /// Gets text content setter.
        /// </summary>
        /// <param name="styleName"> Name of the style. </param>
        /// <returns>
        /// The text content setter.
        /// </returns>
        internal IIdentifier GetTextContentSetter()
        {
            if (this.textContentSetter != null)
            {
                return this.textContentSetter;
            }

            this.textContentSetter =
                new CompoundIdentifier(
                    this.Resolver.ResolveStaticMember(
                    this.KnownTypes.TextContentSetter));

            return this.textContentSetter;
        }

        /// <summary>
        /// Gets CSS class setter.
        /// </summary>
        /// <returns>
        /// The CSS class setter.
        /// </returns>
        internal IIdentifier GetCssClassSetter()
        {
            if (this.cssClassSetter != null)
            {
                return this.cssClassSetter;
            }

            this.cssClassSetter =
                new CompoundIdentifier(
                    this.Resolver.ResolveStaticMember(
                        this.KnownTypes.CssClassSetter));

            return this.cssClassSetter;
        }

        internal IIdentifier GetDocumentStorageGetter()
        {
            if (this.documentStateStorage == null)
            {
                this.documentStateStorage =
                    this.scopeManager.GetTypeScope(
                        this.knownReferences.DocumentRef).GetIdentifier(
                            "stateStore",
                            true,
                            false);

                this.documentStorageGetter =
                    SimpleIdentifier.CreateScopeIdentifier(
                        this.scopeManager.Scope,
                        "DocStorageGetter",
                        false);
            }

            return this.documentStorageGetter;
        }

        internal IIdentifier GetCssInitializer(HtmlParser htmlParser)
        {
            IIdentifier rv;
            if (!this.cssInitializerMapping.TryGetValue(htmlParser, out rv))
            {
                rv = SimpleIdentifier.CreateScopeIdentifier(
                    this.scopeManager.Scope,
                    "CssInitializer",
                    false);
            }

            return rv;
        }

        internal int GetDataStorageIndex(SkinCodeGenerator skinCodeGenerator)
        {
            return this.skinCodeGeneratorStorageIndexs[skinCodeGenerator];
        }

        internal IIdentifier GetGlobalStateVariable()
        {
            if (this.storageArrayIdentifier == null)
            {
                this.storageArrayIdentifier =
                    SimpleIdentifier.CreateScopeIdentifier(
                        this.scopeManager.Scope,
                        "tmplStore",
                        false);
            }

            return this.storageArrayIdentifier;
        }

        internal void AddStatement(ExpressionStatement expressionStatement)
        {
            this.generatedCode.Add(expressionStatement);
        }

        private void GenerateCode()
        {
            this.generatedCode.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(
                        this.GetGlobalStateVariable(),
                        this.scopeManager.Scope),
                    new NewArrayExpression(
                        null,
                        this.scopeManager.Scope,
                        new NumberLiteralExpression(
                            this.scopeManager.Scope,
                            this.skinCodeGeneratorStorageIndexs.Count))));

            this.generatedCode.Add(
                new ExpressionStatement(
                    null,
                    this.scopeManager.Scope,
                    this.GenerateDocumentInitializerMethod()));
        }

        private FunctionExpression GenerateDocumentInitializerMethod()
        {
            var methodScope = new IdentifierScope(
                this.scopeManager.Scope,
                new string[]{
                    "doc"
                },
                false);

            var rv = new FunctionExpression(
                null,
                this.scopeManager.Scope,
                methodScope,
                methodScope.ParameterIdentifiers,
                this.GetDocumentStorageGetter());

            // var style;
            var styleElemIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                methodScope,
                "style",
                false);

            // doc.storage = new Array(##);
            List<Statement> initializers = new List<Statement>();
            initializers.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    IdentifierExpression.Create(
                        null,
                        methodScope,
                        new IIdentifier[] {
                            methodScope.ParameterIdentifiers[0],
                            this.documentStateStorage
                        }),
                    new NewArrayExpression(
                        null,
                        methodScope,
                        new NumberLiteralExpression(
                            methodScope,
                            this.skinCodeGeneratorStorageIndexs.Count))));

            // style = doc.createElement('style')
            initializers.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(
                        styleElemIdentifier,
                        methodScope),
                    new MethodCallExpression(
                        null,
                        methodScope,
                        new IndexExpression(
                            null,
                            methodScope,
                            new IdentifierExpression(
                                methodScope.ParameterIdentifiers[0],
                                methodScope),
                            new StringLiteralExpression(
                                methodScope,
                                "createElement")),
                        new StringLiteralExpression(
                            methodScope,
                            "style"))));

            // style.textContent = "CSS"
            initializers.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        methodScope,
                        new IdentifierExpression(
                            styleElemIdentifier,
                            methodScope),
                        new StringLiteralExpression(
                            methodScope,
                            "textContent")),
                    new StringLiteralExpression(
                        methodScope,
                        this.GetAllCss())));

            // doc.body.appendChild(style)
            initializers.Add(
                ExpressionStatement.CreateMethodCallExpression(
                    new IndexExpression(
                        null,
                        methodScope,
                        new IndexExpression(
                            null,
                            methodScope,
                            new IdentifierExpression(
                                methodScope.ParameterIdentifiers[0],
                                methodScope),
                            new StringLiteralExpression(
                                methodScope,
                                "body")),
                        new StringLiteralExpression(
                            methodScope,
                            "appendChild")),
                    new IdentifierExpression(
                        styleElemIdentifier,
                        methodScope)));

            // if (!doc.storage) {...}
            var ifStatement = new IfBlockStatement(
                null,
                methodScope,
                new UnaryExpression(
                    null,
                    methodScope,
                    UnaryOperator.LogicalNot,
                    IdentifierExpression.Create(
                        null,
                        methodScope,
                        new IIdentifier[] {
                            methodScope.ParameterIdentifiers[0],
                            this.documentStateStorage
                        })),
                new ScopeBlock(
                    null,
                    methodScope,
                    initializers),
                null);

            rv.AddStatement(ifStatement);

            rv.AddStatement(
                new ReturnStatement(
                    null,
                    methodScope,
                    IdentifierExpression.Create(
                        null,
                        methodScope,
                        new IIdentifier[] {
                            methodScope.ParameterIdentifiers[0],
                            this.documentStateStorage
                        })));

            return rv;
        }

        private string GetAllCss()
        {
            StringBuilder sb = new StringBuilder();
            HashSet<DocumentContext> documentContexts = new HashSet<DocumentContext>();
            foreach (var skinCodeGenerators in this.skinCodeGenerators.Keys)
            {
                var dc = skinCodeGenerators.HtmlParser.DocumentContext;
                if (!documentContexts.Contains(dc))
                {
                    documentContexts.Add(dc);
                    sb.Append(dc.GetCssString());
                }
            }

            return sb.ToString();
        }
    }
}
