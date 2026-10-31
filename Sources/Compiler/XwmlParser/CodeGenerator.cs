//-----------------------------------------------------------------------
// <copyright file="CodeGenerator.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.Converter;
    using NScript.Converter.ExpressionsConverter;
    using NScript.Converter.TypeSystemConverter;
    using NScript.JST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using XwmlParser.Binding;
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

        /// <summary>
        /// The converter to method.
        /// </summary>
        Dictionary<ConverterInfo, IIdentifier> converterToMethod =
            new Dictionary<ConverterInfo, IIdentifier>(new ConverterInfoComparer());

        /// <summary>
        /// The CSS initializer mapping.
        /// </summary>
        Dictionary<HtmlParser, IIdentifier> cssInitializerMapping =
            new Dictionary<HtmlParser, IIdentifier>();

        /// <summary>
        /// The CSS initializer code generated.
        /// </summary>
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
        /// HTML documents and template parsers read this build (Probe.XwmlInit). Nothing is
        /// cached across builds, so every document is a miss.
        /// </summary>
        internal int ProbeDocuments;

        internal int ProbeTemplatesParsed;

        /// <summary>Style sheets read this build (Probe.XwmlInit).</summary>
        internal int ProbeStyleSheets => this.parserContext.StyleSheets.Count;

        /// <summary>Style sheets this build found already parsed (<see cref="CssStyleSheet.ParsedSheets"/>).</summary>
        internal int ProbeStyleSheetHits;

        /// <summary>Documents this build took from a kept parse (Probe.XwmlInit Hits).</summary>
        internal int ProbeReusedDocuments;

        /// <summary>
        /// A parse an earlier build of the session kept, offered by the plugin. The first
        /// template request adopts it when every resource it read is unchanged.
        /// </summary>
        private ParserContext keptParse;

        /// <summary>The documents this build asked for, in the order it first asked.</summary>
        private readonly List<string> requestedDocuments = new List<string>();

        private readonly HashSet<string> requestedDocumentSet = new HashSet<string>();

        /// <summary>Gets whether this build uses a parse kept from an earlier build.</summary>
        internal bool ReusedParse { get; private set; }

        /// <summary>Why the offered parse was not used, or null.</summary>
        internal string ReuseMiss { get; private set; }

        /// <summary>
        /// True when this build asked for the documents in the order the parse holds them. A
        /// kept parse registered its documents in the earlier build's order, which sets the CSS
        /// order and the CSS class names: a build that asks in another order (or for fewer)
        /// would not equal a cold build.
        /// </summary>
        internal bool RequestOrderMatchesParse
            => System.Linq.Enumerable.SequenceEqual(this.requestedDocuments, this.parserContext.DocumentOrder);

        /// <summary>
        /// The resource map.
        /// </summary>
        Dictionary<string, Tuple<EmbeddedResource, string>> resourceMap =
            new Dictionary<string, Tuple<EmbeddedResource, string>>();

        /// <summary>The template and style sheet resources of this build, by resource name.</summary>
        internal IReadOnlyDictionary<string, Tuple<EmbeddedResource, string>> ResourceMap => this.resourceMap;

        /// <summary>Dev patch emit: each template parsed again, to the generator it had before.</summary>
        private Dictionary<TemplateParser, SkinCodeGenerator> generatedBefore;

        /// <summary>True while a patch writes the CSS again.</summary>
        private bool patching;

        /// <summary>The other plugins' CSS the full emit wrote.</summary>
        private List<string> cssContributions;

        /// <summary>The CSS parts the last document initializer wrote (<see cref="GetCssParts"/>).</summary>
        private List<(string Key, string Css)> writtenCssParts;

        /// <summary>Whether the document initializer has the live CSS hook (<see cref="CreateLiveCss"/>).</summary>
        private bool liveCssHook;

        private const string CssKey = "__nsCss";

        private const string StylesKey = "__nsStyles";

        /// <summary>
        /// Patch-emit bundles: skins and CSS go live in the open page (<see cref="LiveSkinHook"/>,
        /// <see cref="CreateLiveCss"/>). Plain bundles are as before.
        /// </summary>
        internal bool LiveHook => this.parserContext?.ConverterContext?.LivePatchHook == true;

        /// <summary>Dev bundles: the marker on the line before a CSS part's literal.</summary>
        internal static string CssPartMarker(int part) => MarkerStatement.ForLiteral("XwmlParser.Css." + part);

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
            foreach (var pair in this.ReadResourceMap())
            {
                this.AddResource(pair.Value.Item1, pair.Value.Item2);
            }
        }

        /// <summary>The template and style sheet resources of the loaded modules, by resource name.</summary>
        private Dictionary<string, Tuple<EmbeddedResource, string>> ReadResourceMap()
        {
            var rv = new Dictionary<string, Tuple<EmbeddedResource, string>>();
            foreach (var module in this.scopeManager.Context.ClrContext.Modules)
            {
                foreach (var resource in module.Resources)
                {
                    EmbeddedResource embeddedResource = resource as EmbeddedResource;
                    if (embeddedResource == null) { continue; }

                    var fileName = this.scopeManager.Context.GetResourceFileName(
                        module,
                        embeddedResource.Name);

                    if (fileName != null
                        && (fileName.EndsWith(".html")
                            || fileName.EndsWith(".htm")
                            || fileName.EndsWith(".xhtml")
                            || fileName.EndsWith(".xml")
                            || fileName.EndsWith(".css")
                            || fileName.EndsWith(".less")))
                    {
                        rv.Add(embeddedResource.Name, Tuple.Create(embeddedResource, (string)fileName));
                    }
                }
            }

            return rv;
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
            { throw new ApplicationException(string.Format("PropertySetter not found for {0}", propertyReference)); }
            
            methodRef = methodRef.FixGenericTypeArguments(
                propertyReference.DeclaringType);

            var methodDefinition = methodRef.Resolve();
            return MethodCallExpressionConverter.CreateMethodCallExpression(
                new MethodCallContext(
                    instanceExpression,
                    methodRef,
                    methodDefinition.IsVirtual && !methodDefinition.IsFinal),
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
        /// Adds a resource to 'resourceFileName'.
        /// </summary>
        /// <param name="resource">         The resource. </param>
        /// <param name="resourceFileName"> Filename of the resource file. </param>
        public void AddResource(
            EmbeddedResource resource,
            string resourceFileName)
        {
            resourceMap.Add(
                resource.Name,
                Tuple.Create(
                    resource,
                    resourceFileName));
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

            if (this.keptParse != null)
            {
                this.AdoptKeptParse();
            }

            string[] templateNameSplits = templateParserResourceId.Split(':');
            HtmlParser htmlParser = this.parserContext.GetHtmlParser(templateNameSplits[0]);

            if (htmlParser != null && this.requestedDocumentSet.Add(templateNameSplits[0]))
            {
                // This build's first ask for a document the kept parse holds: report what its parse reported.
                this.requestedDocuments.Add(templateNameSplits[0]);
                this.ProbeReusedDocuments++;
                if (this.parserContext.DocumentWarnings.TryGetValue(templateNameSplits[0], out var keptWarnings))
                {
                    foreach (var warning in keptWarnings)
                    {
                        this.parserContext.ConverterContext.AddError(warning.Item1, warning.Item2, true);
                    }
                }
            }

            if (htmlParser == null)
            {
                Tuple<EmbeddedResource, string> resource;
                if (this.resourceMap.TryGetValue(templateNameSplits[0], out resource))
                {
                    var document = new HtmlAgilityPack.HtmlDocument();
                    using (System.IO.StreamReader reader = this.ReadResource(templateNameSplits[0], resource))
                    {
                        document.Load(reader);
                    }

                    var warnings = this.parserContext.ConverterContext.Warnings;
                    int warningCount = warnings.Count;
                    htmlParser = new HtmlParser(
                        resource.Item2 ?? templateNameSplits[0],
                        document,
                        this.parserContext);

                    this.parserContext.DocumentWarnings[templateNameSplits[0]] =
                        System.Linq.Enumerable.ToList(System.Linq.Enumerable.Skip(warnings, warningCount));
                    this.parserContext.RegisterHtmlParser(
                        templateNameSplits[0],
                        htmlParser);
                    this.requestedDocumentSet.Add(templateNameSplits[0]);
                    this.requestedDocuments.Add(templateNameSplits[0]);
                    this.ProbeDocuments++;
                }

                if (htmlParser == null)
                {
                    throw new ApplicationException(
                        string.Format("Can't find template:'{0}'", templateParserResourceId));
                }
            }

            TemplateParser templateParser = htmlParser.GetTemplateParser(
                templateNameSplits.Length == 2
                    ? templateNameSplits[1]
                    : null);
            if (templateParser == null)
            {
                throw new ApplicationException(
                    string.Format("Can't find template:'{0}'", templateParserResourceId));
            }
            else
            {
                this.templatesToParse.Enqueue(templateParser);
            }

            return templateParser;
        }

        /// <summary>
        /// Offers a parse an earlier build of the session kept. It is adopted at the first
        /// template request, when the resource map is complete, if it is still current.
        /// </summary>
        internal void OfferKeptParse(ParserContext kept)
        {
            this.keptParse = kept;
        }

        /// <summary>The resource name to file name map of this build.</summary>
        internal Dictionary<string, string> GetResourceFileNames()
        {
            var rv = new Dictionary<string, string>(this.resourceMap.Count);
            foreach (var pair in this.resourceMap)
            {
                rv.Add(pair.Key, pair.Value.Item2);
            }

            return rv;
        }

        private void AdoptKeptParse()
        {
            var kept = this.keptParse;
            this.keptParse = null;
            this.ReuseMiss = this.GetReuseMiss(kept);
            if (this.ReuseMiss == null)
            {
                kept.Rebind(
                    this,
                    this.parserContext.ClrResolver,
                    this.parserContext.JsResolver,
                    this.parserContext.ConverterContext);
                this.ReusedParse = true;
            }
        }

        /// <summary>Null when the kept parse still matches every input it read, else why not.</summary>
        private string GetReuseMiss(ParserContext kept)
        {
            if (kept.KnownCssClasses != this.parserContext.KnownCssClasses)
            {
                return "known-css-classes";
            }

            var fileNames = kept.ResourceFileNames;
            if (fileNames == null || fileNames.Count != this.resourceMap.Count)
            {
                return "resource-set";
            }

            foreach (var pair in this.resourceMap)
            {
                if (!fileNames.TryGetValue(pair.Key, out var fileName) || fileName != pair.Value.Item2)
                {
                    return "resource-set";
                }
            }

            foreach (var read in kept.ResourcesRead)
            {
                var hash = System.Security.Cryptography.SHA256.HashData(this.resourceMap[read.Key].Item1.GetResourceData());
                if (!System.Linq.Enumerable.SequenceEqual(hash, read.Value.Item2))
                {
                    return "changed " + read.Key;
                }
            }

            return null;
        }

        /// <summary>A reader over a template or style sheet resource; records its hash for reuse.</summary>
        private System.IO.StreamReader ReadResource(string resourceId, Tuple<EmbeddedResource, string> resource)
        {
            byte[] data = resource.Item1.GetResourceData();
            this.parserContext.ResourcesRead[resourceId] = Tuple.Create(
                resource.Item2,
                System.Security.Cryptography.SHA256.HashData(data));
            return new System.IO.StreamReader(new System.IO.MemoryStream(data, writable: false));
        }

        /// <summary>
        /// Gets style sheet.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="cssResourceId"> Identifier for the CSS resource. </param>
        /// <returns>
        /// The style sheet.
        /// </returns>
        public CssStyleSheet GetStyleSheet(
            string cssResourceId,
            string currentFileId,
            List<CssStyleSheet> styleSheets)
        {
            string relativeCssResourceId = this.GetRelativeResourceId(
                cssResourceId,
                currentFileId);

            CssStyleSheet rv;
            if (this.parserContext.StyleSheets.TryGetValue(relativeCssResourceId, out rv))
            {
                return rv;
            }

            Tuple<EmbeddedResource, string> resource;
            if (this.resourceMap.TryGetValue(relativeCssResourceId, out resource))
            {
                rv = new CssStyleSheet(
                    this.parserContext,
                    resource.Item2 ?? relativeCssResourceId);

                using (System.IO.StreamReader reader = this.ReadResource(relativeCssResourceId, resource))
                {
                    if (rv.AddCss(
                        reader.ReadToEnd(),
                        new Location(
                            rv.ResourceName,
                            0,
                            0),
                        styleSheets))
                    {
                        this.ProbeStyleSheetHits++;
                    }
                }

                this.parserContext.StyleSheets.Add(relativeCssResourceId, rv);
            }
            else
            {
                throw new ApplicationException(
                    string.Format("Can't find StyleSheet: {0}", cssResourceId));
            }

            return rv;
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
                this.ProbeTemplatesParsed++;

                if (templateParser.SkinNodeInfo != null)
                {
                    var skinCodeGenerator = new SkinCodeGenerator(
                        templateParser,
                        this);

                    this.skinCodeGeneratorStorageIndexs.Add(
                        skinCodeGenerator,
                        this.skinCodeGeneratorStorageIndexs.Count);
                    this.skinCodeGenerators.Add(templateParser, skinCodeGenerator);

                    skinCodeGenerator.GenerateCodePass1();
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
            // Validate CSS variables across all HTML files and stylesheets
            this.parserContext.ValidateAllCssVariables();

            this.parserContext.CompressCssNames();

            this.GenerateCode();

            foreach (var kvPair in this.skinCodeGenerators)
            {
                kvPair.Value.GenerateCodeFinalPass();
            }

            return this.generatedCode;
        }

        /// <summary>
        /// Dev patch emit: the generator a template had before its document was parsed again,
        /// whose names the new one keeps. Null otherwise.
        /// </summary>
        internal SkinCodeGenerator GeneratedBefore(TemplateParser templateParser)
        {
            return this.generatedBefore != null && this.generatedBefore.TryGetValue(templateParser, out var rv)
                ? rv
                : null;
        }

        /// <summary>
        /// Dev patch emit: parses the changed style sheets and documents again and adds the code
        /// that replaces theirs to <paramref name="statements"/>, and the page's CSS, when it
        /// changed, to <paramref name="literals"/>, each changed part under its <see cref="CssPartMarker"/>. The templates
        /// keep their names and the classes keep theirs; new skins take storage indexes past
        /// the old ones. Returns null when patched, or why the change needs a full emit.
        /// </summary>
        internal string PatchResources(
            List<Statement> statements,
            Dictionary<string, string> literals,
            Dictionary<string, Tuple<EmbeddedResource, string>> resourceMap = null)
        {
            long probeStart = System.Diagnostics.Stopwatch.GetTimestamp();
            resourceMap ??= this.ReadResourceMap();
            if (resourceMap.Count != this.resourceMap.Count)
            {
                return "xwml resource-set";
            }

            foreach (var pair in resourceMap)
            {
                if (!this.resourceMap.TryGetValue(pair.Key, out var before) || before.Item2 != pair.Value.Item2)
                {
                    return "xwml resource-set";
                }
            }

            this.resourceMap = resourceMap;
            var changed = new HashSet<string>();
            foreach (var read in this.parserContext.ResourcesRead)
            {
                var hash = System.Security.Cryptography.SHA256.HashData(resourceMap[read.Key].Item1.GetResourceData());
                if (!System.Linq.Enumerable.SequenceEqual(hash, read.Value.Item2))
                {
                    changed.Add(read.Key);
                }
            }

            if (changed.Count == 0)
            {
                return null;
            }

            long probeRead = System.Diagnostics.Stopwatch.GetTimestamp();
            int start = this.generatedCode.Count;
            try
            {
                foreach (var key in System.Linq.Enumerable.ToList(this.parserContext.StyleSheets.Keys))
                {
                    if (changed.Contains(key))
                    {
                        string miss = this.ParseStyleSheetAgain(key);
                        if (miss != null)
                        {
                            return miss;
                        }
                    }
                }

                this.generatedBefore = new Dictionary<TemplateParser, SkinCodeGenerator>();
                var replaced = new Dictionary<TemplateParser, TemplateParser>();
                foreach (var key in System.Linq.Enumerable.ToList(this.parserContext.DocumentOrder))
                {
                    if (changed.Contains(key))
                    {
                        string miss = this.ParseDocumentAgain(key, replaced);
                        if (miss != null)
                        {
                            return miss;
                        }
                    }
                }

                var generatorsBefore = System.Linq.Enumerable.ToList(this.skinCodeGenerators);
                // Each skin written again takes a new index; a full emit numbers them from 0 again.
                this.IterateParsing();

                // Same order as before, so the CSS keeps its order; skins asked for anew go last.
                var generators = new Dictionary<TemplateParser, SkinCodeGenerator>();
                foreach (var pair in generatorsBefore)
                {
                    if (replaced.TryGetValue(pair.Key, out var parsedAgain))
                    {
                        if (this.skinCodeGenerators.TryGetValue(parsedAgain, out var generator))
                        {
                            generators.Add(parsedAgain, generator);
                        }
                    }
                    else
                    {
                        generators.Add(pair.Key, pair.Value);
                    }
                }

                foreach (var pair in this.skinCodeGenerators)
                {
                    if (!generators.ContainsKey(pair.Key) && !replaced.ContainsKey(pair.Key))
                    {
                        generators.Add(pair.Key, pair.Value);
                    }
                }

                this.skinCodeGenerators = generators;
                this.parserContext.ValidateAllCssVariables();
            }
            finally
            {
                this.generatedBefore = null;
                this.parserContext.ParsedBefore = null;
            }

            statements.AddRange(System.Linq.Enumerable.Skip(this.generatedCode, start));
            long probeParse = System.Diagnostics.Stopwatch.GetTimestamp();
            int prefixRuns = CssStyleSheet.Compiler.Runs;

            // Each CSS part that changed is written again in place on its marked line. The
            // document initializer stands: its storage size is a hint, and the array grows for
            // the indexes new skins take. A part added or gone needs a new initializer.
            this.patching = true;
            try
            {
                var parts = this.GetCssParts();
                if (!System.Linq.Enumerable.SequenceEqual(
                    System.Linq.Enumerable.Select(parts, part => part.Key),
                    System.Linq.Enumerable.Select(this.writtenCssParts, part => part.Key)))
                {
                    return "xwml css parts";
                }

                for (int iPart = 0; iPart < parts.Count; iPart++)
                {
                    if (parts[iPart].Css != this.writtenCssParts[iPart].Css)
                    {
                        literals[CssPartMarker(iPart)] = parts[iPart].Css;
                    }
                }

                this.writtenCssParts = parts;
            }
            finally
            {
                this.patching = false;
            }

            static long Ms(long ticks) => ticks * 1000 / System.Diagnostics.Stopwatch.Frequency;
            CompilerLog.ForComponent("XwmlParser").Information(
                "Probe.XwmlPatch Changed={Changed} ReadMs={ReadMs} ParseMs={ParseMs} CssMs={CssMs} PrefixRuns={PrefixRuns}",
                changed.Count,
                Ms(probeRead - probeStart),
                Ms(probeParse - probeRead),
                Ms(System.Diagnostics.Stopwatch.GetTimestamp() - probeParse),
                CssStyleSheet.Compiler.Runs - prefixRuns);
            return null;
        }

        /// <summary>Dev patch emit: parses a changed style sheet again, keeping its class names.</summary>
        private string ParseStyleSheetAgain(string key)
        {
            var old = this.parserContext.StyleSheets[key];
            var resource = this.resourceMap[key];
            var parsedAgain = new CssStyleSheet(this.parserContext, resource.Item2 ?? key);
            this.parserContext.ParsedBefore = new Dictionary<string, CssStyleSheet> { [old.ResourceName] = old };
            using (System.IO.StreamReader reader = this.ReadResource(key, resource))
            {
                parsedAgain.AddCss(
                    reader.ReadToEnd(),
                    new Location(parsedAgain.ResourceName, 0, 0),
                    old.PreviousSheets ?? new List<CssStyleSheet>());
            }

            var declared = new HashSet<string>(parsedAgain.DeclaredClassNames);
            foreach (var className in old.DeclaredClassNames)
            {
                if (!declared.Contains(className))
                {
                    return "xwml class removed " + className;
                }
            }

            declared.ExceptWith(old.DeclaredClassNames);
            foreach (var htmlParser in this.parserContext.HtmlParsers)
            {
                var documentCss = htmlParser.DocumentContext.DocumentCss;
                if (documentCss != null && System.Linq.Enumerable.Any(documentCss.ClassNames, declared.Contains))
                {
                    // A cold build may refuse it: the document declares the class too.
                    return "xwml class added " + key;
                }
            }

            foreach (var styleSheet in this.parserContext.StyleSheets.Values)
            {
                if (styleSheet != old && System.Linq.Enumerable.Any(styleSheet.ClassNames, declared.Contains))
                {
                    // Another sheet, and the skins that use it, bound the class to its own name;
                    // a cold build binds them all to this sheet's.
                    return "xwml class added " + key;
                }
            }

            this.parserContext.StyleSheets[key] = parsedAgain;
            foreach (var styleSheet in this.parserContext.StyleSheets.Values)
            {
                styleSheet.ReplaceDependency(old, parsedAgain);
            }

            foreach (var htmlParser in this.parserContext.HtmlParsers)
            {
                htmlParser.DocumentContext.ReplaceStyleSheet(old, parsedAgain);
            }

            return null;
        }

        /// <summary>
        /// Dev patch emit: parses a changed document again. Each template asked for before is
        /// asked for again under the same getter.
        /// </summary>
        private string ParseDocumentAgain(string key, Dictionary<TemplateParser, TemplateParser> replaced)
        {
            var old = this.parserContext.GetHtmlParser(key);
            var resource = this.resourceMap[key];
            var documentCss = old.DocumentContext.DocumentCss;
            this.parserContext.ParsedBefore = documentCss == null
                ? null
                : new Dictionary<string, CssStyleSheet> { [documentCss.ResourceName] = documentCss };

            var document = new HtmlAgilityPack.HtmlDocument();
            using (System.IO.StreamReader reader = this.ReadResource(key, resource))
            {
                document.Load(reader);
            }

            var warnings = this.parserContext.ConverterContext.Warnings;
            int warningCount = warnings.Count;
            var parsedAgain = new HtmlParser(resource.Item2 ?? key, document, this.parserContext);
            this.parserContext.DocumentWarnings[key] =
                System.Linq.Enumerable.ToList(System.Linq.Enumerable.Skip(warnings, warningCount));
            this.parserContext.ReplaceHtmlParser(key, parsedAgain);

            foreach (var pair in System.Linq.Enumerable.ToList(this.templateGetterIds))
            {
                if (pair.Key.HtmlParser != old)
                {
                    continue;
                }

                var templateParser = parsedAgain.GetTemplateParser(pair.Key.TemplateName);
                if (templateParser == null)
                {
                    return "xwml template removed " + pair.Key.GetUniqueTemplateId();
                }

                this.templateGetterIds.Remove(pair.Key);
                this.templateGetterIds.Add(templateParser, pair.Value);
                if (this.skinCodeGenerators.TryGetValue(pair.Key, out var generator))
                {
                    this.generatedBefore.Add(templateParser, generator);
                }

                replaced.Add(pair.Key, templateParser);
                this.templatesToParse.Enqueue(templateParser);
            }

            return null;
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

            var expr = this.GetCsAst("src", path);

            var returnStatement = new NScript.CLR.AST.ReturnStatement(
                this.scopeManager.Context.ClrContext,
                null,
                expr);

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
                NScript.Converter.StatementsConverter.StatementConverterBase.Convert(
                    new MethodScopeConverter(
                        this.scopeManager,
                        this.Resolver,
                        methodScope),
                    returnStatement));

            this.generatedCode.Add(
                new ExpressionStatement(
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

            PropertyReference propertyReference = property as PropertyReference;

            // We do special stuff for DataContext just so that we can check if a binding
            // is setting DataContext. If so we do not set DataContext for the object.
            // To check this we need pointer to DataContext setter method.
            if (propertyReference != null
                && propertyReference.Name == "DataContext"
                && propertyReference.DeclaringType.IsSameDefinition(KnownTypes.ContextBindableObject))
            {
                var identifiers = Resolver.ResolveStaticMember(KnownTypes.DataContextSetter);
                if (identifiers.Count > 1)
                {
                    rv = new CompoundIdentifier(identifiers);
                }
                else
                {
                    rv = identifiers[0];
                }

                this.propertySetterMap.Add(property, rv);
                return rv;
            }

            IdentifierScope methodScope = new IdentifierScope(
                this.scopeManager.Scope,
                new string[]{
                    "tar",
                    "val"
                },
                false);

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

            var methodDefinition = methodReference.Resolve();
            expression =
                MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(
                        expression,
                        methodReference,
                        methodDefinition.IsVirtual && !methodDefinition.IsFinal),
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
                SimpleIdentifier.CreateScopeIdentifier(this.scopeManager.Scope, "delgateGetter", false));

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
        /// Gets to converter identifier.
        /// </summary>
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
        /// <param name="converterInfo"> Information describing the converter. </param>
        /// <returns>
        /// to converter identifier.
        /// </returns>
        internal IIdentifier GetToConverterIdentifier(ConverterInfo converterInfo)
        {
            IIdentifier rv = null;
            if (this.converterToMethod.TryGetValue(converterInfo, out rv))
            {
                return rv;
            }

            DelegateConverterInfo delegateConverterInfo = converterInfo as DelegateConverterInfo;
            if (delegateConverterInfo == null)
            {
                throw new NotSupportedException();
            }

            IdentifierScope methodScope = new IdentifierScope(
                this.scopeManager.Scope,
                new string[] {
                    "from"
                },
                false);

            List<Expression> args = new List<Expression>();
            args.Add(new IdentifierExpression(methodScope.ParameterIdentifiers[0], methodScope));

            if (converterInfo.Arguments != null)
            foreach (var arg in converterInfo.Arguments)
            {
                switch (arg.Item1)
                {
                    case ConverterArgType.Boolean:
                        args.Add(new BooleanLiteralExpression(methodScope, (bool)arg.Item2));
                        break;
                    case ConverterArgType.Enum:
                    case ConverterArgType.Integer:
                        args.Add(new NumberLiteralExpression(methodScope, Convert.ToInt32(arg.Item2)));
                        break;
                    case ConverterArgType.Float:
                        args.Add(new DoubleLiteralExpression(methodScope, Convert.ToDouble(arg.Item2)));
                        break;
                    case ConverterArgType.String:
                        args.Add(new StringLiteralExpression(methodScope, (string)arg.Item2));
                        break;
                    case ConverterArgType.StaticPropInfo:
                        {
                            var expr = this.GetCsAst("tmp", (List<MemberReference>)arg.Item2);
                            args.Add(
                                NScript.Converter.ExpressionsConverter.ExpressionConverterBase.Convert(
                                    new MethodScopeConverter(
                                        this.scopeManager,
                                        this.Resolver,
                                        methodScope),
                                    expr));
                        }
                        break;
                    default:
                        break;
                }
            }


            var returnExpression = new ReturnStatement(
                null,
                methodScope,
                MethodCallExpressionConverter.CreateMethodCallExpression(
                    new MethodCallContext(
                        delegateConverterInfo.MethodReference,
                        null,
                        methodScope),
                    args,
                    this.Resolver,
                    this.scopeManager));

            rv = SimpleIdentifier.CreateScopeIdentifier(
                    this.scopeManager.Scope,
                    "converter",
                    false);

            var functionExpression = new FunctionExpression(
                null,
                this.scopeManager.Scope,
                methodScope,
                methodScope.ParameterIdentifiers,
                rv);

            functionExpression.AddStatement(returnExpression);

            this.converterToMethod.Add(converterInfo, rv);
            this.generatedCode.Add(
                new ExpressionStatement(
                    null,
                    this.scopeManager.Scope,
                    functionExpression));

            return rv;
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

        internal void AddStatement(Statement expressionStatement)
        {
            this.generatedCode.Add(expressionStatement);
        }

        /// <summary>
        /// Gets create structure ast.
        /// </summary>
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
        /// <param name="srcArgName"> Name of the source argument. </param>
        /// <param name="path">       Full pathname of the file. </param>
        /// <returns>
        /// The create structure ast.
        /// </returns>
        private NScript.CLR.AST.Expression GetCsAst(string srcArgName, IList<MemberReference> path)
        {
            var clrContext = this.scopeManager.Context.ClrContext;
            var argVariable = new NScript.CLR.AST.ParameterVariable(
                new ParameterDefinition(srcArgName, ParameterAttributes.None, path[0].DeclaringType),
                null);
            var argExpression = new NScript.CLR.AST.VariableReference(
                clrContext,
                null,
                argVariable);

            NScript.CLR.AST.Expression expr = 
                path[0].IsStatic()
                    ? (NScript.CLR.AST.Expression)null
                    : (NScript.CLR.AST.Expression)argExpression;

            for (int iPath = 0; iPath < path.Count; iPath++)
            {
                MemberReference memberRef = path[iPath];
                FieldReference fieldReference = memberRef as FieldReference;

                if (fieldReference != null)
                {
                    if (expr == null)
                    {
                        expr = new NScript.CLR.AST.FieldReferenceExpression(
                            this.scopeManager.Context.ClrContext,
                            null,
                            fieldReference);
                    }
                    else
                    {
                        expr = new NScript.CLR.AST.FieldReferenceExpression(
                            this.scopeManager.Context.ClrContext,
                            null,
                            fieldReference,
                            expr);
                    }
                }
                else
                {
                    PropertyReference propertyReference = memberRef as PropertyReference;
                    if (propertyReference != null)
                    {
                        if (expr == null)
                        {
                            expr = new NScript.CLR.AST.PropertyReferenceExpression(
                                this.scopeManager.Context.ClrContext,
                                null,
                                propertyReference);
                        }
                        else
                        {
                            expr = new NScript.CLR.AST.PropertyReferenceExpression(
                                this.scopeManager.Context.ClrContext,
                                null,
                                propertyReference,
                                expr);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            return expr;
        }

        /// <summary>
        /// Generates a code.
        /// </summary>
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
                    this.GenerateDocumentInitializerMethod(this.GetCssParts())));

            if (this.LiveHook)
            {
                this.generatedCode.AddRange(this.CreateLiveSkinHelpers(this.scopeManager.Scope));
            }
        }

        /// <summary>The live skin helpers (<see cref="LiveSkinHook.CreateHelpers"/>).</summary>
        private List<Statement> CreateLiveSkinHelpers(IdentifierScope root)
        {
            var skinType = this.KnownTypes.SkinCtor.DeclaringType.Resolve();
            var skinableType = this.KnownTypes.UISkinableElement.Resolve();
            IIdentifier Field(TypeDefinition type, string name)
                => this.Resolver.Resolve(System.Linq.Enumerable.First(type.Fields, field => field.Name == name));

            // function(c, v) { c.Skin = v; }
            var setScope = new IdentifierScope(root, 2);
            var setSkin = new FunctionExpression(null, root, setScope, setScope.ParameterIdentifiers, null);
            setSkin.AddStatement(
                new ExpressionStatement(
                    null,
                    setScope,
                    GetPropertySetterExpression(
                        this.scopeManager,
                        setScope,
                        System.Linq.Enumerable.First(skinableType.Properties, property => property.Name == "Skin"),
                        this.Resolver,
                        new IdentifierExpression(setScope.ParameterIdentifiers[0], setScope),
                        new IdentifierExpression(setScope.ParameterIdentifiers[1], setScope))));

            return LiveSkinHook.CreateHelpers(
                root,
                this.GetDocumentStorageGetter(),
                Field(skinType, "factoryMethod"),
                Field(skinType, "id"),
                Field(this.KnownTypes.SkinInstance, "skinableParent"),
                Field(skinableType, "skinInstance"),
                Field(this.KnownTypes.UIElement.Resolve(), "element"),
                setSkin);
        }

        /// <summary>A skin factory's result, kept on its skin (<see cref="LiveSkinHook.CreateTrack"/>).</summary>
        internal Expression CreateTrack(IdentifierScope scope, Expression instance, Expression skin)
            => LiveSkinHook.CreateTrack(scope, this.GetDocumentStorageGetter(), instance, skin);

        /// <summary>The live form of <c>Name_var = null;</c> (<see cref="LiveSkinHook.CreateReskin"/>).</summary>
        internal Statement CreateReskin(IdentifierScope scope, IIdentifier skinVariable, IIdentifier factory, string id, string name)
            => LiveSkinHook.CreateReskin(scope, this.GetDocumentStorageGetter(), skinVariable, factory, id, name);

        /// <summary>
        /// The live form of a CSS part's patch, run in the bundle's closure: the style element
        /// each document took for the part under <paramref name="marker"/> gets
        /// <paramref name="css"/>, and so does each document made later. Null when the bundle
        /// has no live CSS hook or no such part.
        /// </summary>
        internal List<Statement> CreateLiveCss(string marker, string css, IdentifierScope root)
        {
            int part = this.writtenCssParts == null
                ? -1
                : System.Linq.Enumerable.FirstOrDefault(
                    System.Linq.Enumerable.Range(0, this.writtenCssParts.Count),
                    iPart => CssPartMarker(iPart) == marker,
                    -1);
            if (!this.liveCssHook || part < 0)
            {
                return null;
            }

            return new List<Statement>
            {
                // DocStorageGetter.__nsCss = DocStorageGetter.__nsCss || {};
                ExpressionStatement.CreateAssignmentExpression(
                    this.GetterKey(root, CssKey),
                    new BinaryExpression(
                        null,
                        root,
                        BinaryOperator.LogicalOr,
                        this.GetterKey(root, CssKey),
                        new InlineObjectInitializer(null, root))),

                // DocStorageGetter.__nsCss[part] = "CSS";
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(null, root, this.GetterKey(root, CssKey), new NumberLiteralExpression(root, part)),
                    new StringLiteralExpression(root, css)),

                // (DocStorageGetter.__nsStyles || []).forEach(function(s){...}, DocStorageGetter.__nsCss);
                ExpressionStatement.CreateMethodCallExpression(
                    new IndexExpression(
                        null,
                        root,
                        new BinaryExpression(
                            null,
                            root,
                            BinaryOperator.LogicalOr,
                            this.GetterKey(root, StylesKey),
                            new InlineNewArrayInitialization(null, root, new List<Expression>())),
                        new StringLiteralExpression(root, "forEach")),
                    new ScriptLiteralExpression(null, root, "function(s){if(s.__nsPart===" + part + ")s.textContent=this[" + part + "];}"),
                    this.GetterKey(root, CssKey)),
            };
        }

        private Expression GetterKey(IdentifierScope scope, string key)
            => new IndexExpression(
                null,
                scope,
                new IdentifierExpression(this.GetDocumentStorageGetter(), scope),
                new StringLiteralExpression(scope, key));

        /// <summary>
        /// Generates a document initializer method.
        /// </summary>
        /// <returns>
        /// The document initializer method.
        /// </returns>
        private FunctionExpression GenerateDocumentInitializerMethod(List<(string Key, string Css)> cssParts)
        {
            this.writtenCssParts = cssParts;
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

            // Each CSS part takes its own style element, in cascade order.
            this.liveCssHook = this.parserContext?.ConverterContext?.LivePatchHook == true;
            for (int iPart = 0; iPart < cssParts.Count; iPart++)
            {
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

                // A dev bundle marks the part's CSS line, so a patch writes the part again in place.
                if (this.parserContext?.ConverterContext?.DevMode == true)
                {
                    initializers.Add(new MarkerStatement(methodScope, CssPartMarker(iPart)));
                }

                // style.textContent = "CSS", or with the live hook
                // DocStorageGetter.__nsCss && part in DocStorageGetter.__nsCss ? DocStorageGetter.__nsCss[part] : "CSS":
                // a document made after a live patch takes the patch's CSS, an emptied part too.
                Expression cssValue = new StringLiteralExpression(methodScope, cssParts[iPart].Css);
                if (this.liveCssHook)
                {
                    cssValue = new ConditionalOperatorExpression(
                        null,
                        methodScope,
                        new BinaryExpression(
                            null,
                            methodScope,
                            BinaryOperator.LogicalAnd,
                            this.GetterKey(methodScope, CssKey),
                            new BinaryExpression(
                                null,
                                methodScope,
                                BinaryOperator.In,
                                new NumberLiteralExpression(methodScope, iPart),
                                this.GetterKey(methodScope, CssKey))),
                        new IndexExpression(null, methodScope, this.GetterKey(methodScope, CssKey), new NumberLiteralExpression(methodScope, iPart)),
                        cssValue);
                }

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
                        cssValue));

                // style.__nsPart = part; the live patch of a part finds its elements by it.
                if (this.liveCssHook)
                {
                    initializers.Add(
                        ExpressionStatement.CreateAssignmentExpression(
                            new IndexExpression(
                                null,
                                methodScope,
                                new IdentifierExpression(styleElemIdentifier, methodScope),
                                new StringLiteralExpression(methodScope, "__nsPart")),
                            new NumberLiteralExpression(methodScope, iPart)));
                }

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
                                    "head")),
                            new StringLiteralExpression(
                                methodScope,
                                "appendChild")),
                        new IdentifierExpression(
                            styleElemIdentifier,
                            methodScope)));

                // (DocStorageGetter.__nsStyles || (DocStorageGetter.__nsStyles = [])).push(style)
                if (this.liveCssHook)
                {
                    initializers.Add(
                        ExpressionStatement.CreateMethodCallExpression(
                            new IndexExpression(
                                null,
                                methodScope,
                                new BinaryExpression(
                                    null,
                                    methodScope,
                                    BinaryOperator.LogicalOr,
                                    this.GetterKey(methodScope, StylesKey),
                                    new BinaryExpression(
                                        null,
                                        methodScope,
                                        BinaryOperator.Assignment,
                                        this.GetterKey(methodScope, StylesKey),
                                        new InlineNewArrayInitialization(null, methodScope, new List<Expression>()))),
                                new StringLiteralExpression(methodScope, "push")),
                            new IdentifierExpression(styleElemIdentifier, methodScope)));
                }
            }

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

        /// <summary>
        /// The page's CSS parts, in cascade order: the style sheets by their dependencies, each
        /// document's, then other plugins'. Each part is keyed by where it comes from. A dev
        /// bundle writes a style element per part, so a save writes its own part again. Each
        /// part stays when its CSS is empty, so a save that adds or drops a document's style
        /// patches in place. A release bundle writes one part.
        /// </summary>
        private List<(string Key, string Css)> GetCssParts()
        {
            var pieces = new List<(string Key, string Css)>();
            HashSet<DocumentContext> documentContexts = new HashSet<DocumentContext>();
            List<CssStyleSheet> dependencyOrderedList = new List<CssStyleSheet>();

            Action<CssStyleSheet> looper = null;
            looper = (cssSheet) =>
                {
                    foreach (var dep in cssSheet.Dependencies)
                    {
                        if (dependencyOrderedList.IndexOf(dep) < 0)
                        {
                            looper(dep);
                        }
                    }

                    if (dependencyOrderedList.IndexOf(cssSheet) < 0)
                    {
                        dependencyOrderedList.Add(cssSheet);
                    }
                };

            foreach (var styleSheet in this.parserContext.StyleSheets.Values)
            {
                looper(styleSheet);
            }

            foreach (var styleSheet in dependencyOrderedList)
            {
                pieces.Add((styleSheet.ResourceName, styleSheet.GetCssString() ?? string.Empty));
            }

            foreach (var skinCodeGenerators in this.skinCodeGenerators.Keys)
            {
                var dc = skinCodeGenerators.HtmlParser.DocumentContext;
                if (!documentContexts.Contains(dc))
                {
                    documentContexts.Add(dc);
                    pieces.Add(("document " + dc.ResourceName, dc.GetCssString() ?? string.Empty));
                }
            }

            // Append CSS contributions from other plugins (e.g., Razor templates). A patch
            // starts a build that clears them, so it writes the ones the full emit wrote.
            if (!this.patching && this.parserContext?.ConverterContext != null)
            {
                this.cssContributions = new List<string>(this.parserContext.ConverterContext.GetCssContributions());
            }

            var contributions = this.cssContributions ?? new List<string>();
            for (int iContribution = 0; iContribution < contributions.Count; iContribution++)
            {
                pieces.Add(("plugin " + iContribution, contributions[iContribution] ?? string.Empty));
            }

            // The prefixer caches results by text. Dev mode prefixes each part apart, so a
            // changed sheet is the only text read again; a release build prefixes one block.
            if (this.parserContext?.ConverterContext?.DevMode != true)
            {
                return new List<(string Key, string Css)>
                {
                    (string.Empty, CssStyleSheet.Compiler.Prefix(
                        string.Concat(System.Linq.Enumerable.Select(pieces, piece => piece.Css)),
                        CssStyleSheet.browserSpecification)),
                };
            }

            for (int iPiece = 0; iPiece < pieces.Count; iPiece++)
            {
                if (pieces[iPiece].Css.Length > 0)
                {
                    pieces[iPiece] = (pieces[iPiece].Key, CssStyleSheet.Compiler.Prefix(pieces[iPiece].Css, CssStyleSheet.browserSpecification));
                }
            }

            return pieces;
        }

        private string GetRelativeResourceId(
            string resourceId,
            string baseResourceId)
        {
            if (this.resourceMap.ContainsKey(resourceId))
            {
                return resourceId;
            }

            string directoryName = System.IO.Path.GetDirectoryName(baseResourceId);
            string relativeResouceId = resourceId.Replace("/", "\\");
            while(relativeResouceId.StartsWith("..\\"))
            {
                directoryName = System.IO.Path.GetDirectoryName(directoryName);
                relativeResouceId = relativeResouceId.Substring(3);
            }

            string resourceFileName = System.IO.Path.Combine(directoryName, relativeResouceId);
            foreach (var item in this.resourceMap)
            {
                if (string.Compare(item.Value.Item2, resourceFileName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return item.Key;
                }
            }

            return resourceId;
        }
    }
}
