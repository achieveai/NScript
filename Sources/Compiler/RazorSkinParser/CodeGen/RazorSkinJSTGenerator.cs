using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NScript.CLR;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;
using Serilog;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Generates proper JST (JavaScript Syntax Tree) nodes for Razor skin templates,
    /// mirroring the pattern used by XwmlParser's SkinCodeGenerator. This ensures that
    /// all identifier references participate in NScript's scope-based minification system,
    /// preventing "function not defined" errors at runtime.
    /// </summary>
    public class RazorSkinJSTGenerator
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private readonly SkinTemplateNode _ir;
        private readonly RuntimeScopeManager _scopeManager;
        private readonly ClrContext _clrContext;
        private readonly Dictionary<string, IIdentifier> _resolvedIdentifiers;
        private readonly Dictionary<string, IList<IIdentifier>> _resolvedTypeIdentifiers;
        private readonly RazorKnownTypes _knownTypes;
        private readonly CecilTypeHelper _typeHelper;

        // Topology reference for marker path computation
        private GraphTopology _topology;

        // Event element paths computed from data-evt-idx markers
        private List<List<int>> _eventPaths;

        // Scope for the factory function body (has "skinFactory" and "doc" parameters)
        private IdentifierScope _factoryScope;

        // Scope identifiers created during generation
        private SimpleIdentifier _factoryMethodIdentifier;
        private SimpleIdentifier _skinStorageVariable;
        private SimpleIdentifier _getterMethodIdentifier;
        private IIdentifier _tmplStoreIdentifier;
        private IIdentifier _domStoreIdentifier;
        private IIdentifier _htmlRootIdentifier;
        private IIdentifier _objStorageIdentifier;

        // Data index for doc.stateStore — must be unique across ALL templates (XWML + Razor).
        // The starting offset is passed in from the plugin to avoid collision with XWML indices.
        private readonly int _dataIndex;

        /// <summary>
        /// Optional CSS manager for templates with @styles directives.
        /// When set, class names in HTML output are replaced with minified versions.
        /// </summary>
        private readonly RazorCssManager _cssManager;


        /// <summary>
        /// Pre-created getter identifier from Initialize(), so GetOverwrite can reference
        /// the same identifier that the getter function is registered under.
        /// </summary>
        private readonly IIdentifier _preCreatedGetterIdentifier;

        public RazorSkinJSTGenerator(
            SkinTemplateNode ir,
            RuntimeScopeManager scopeManager,
            ClrContext clrContext,
            Dictionary<string, IIdentifier> resolvedIdentifiers,
            Dictionary<string, IList<IIdentifier>> resolvedTypeIdentifiers,
            RazorKnownTypes knownTypes,
            int dataIndex,
            IIdentifier preCreatedGetterIdentifier = null,
            RazorCssManager cssManager = null)
        {
            _ir = ir;
            _scopeManager = scopeManager;
            _clrContext = clrContext;
            _resolvedIdentifiers = resolvedIdentifiers;
            _resolvedTypeIdentifiers = resolvedTypeIdentifiers;
            _knownTypes = knownTypes;
            _typeHelper = new CecilTypeHelper(clrContext);
            _preCreatedGetterIdentifier = preCreatedGetterIdentifier;
            _dataIndex = dataIndex;
            _cssManager = cssManager;
        }

        /// <summary>
        /// Returns a non-null <see cref="Location"/> anchored at the originating
        /// <c>.skin.cshtml</c> template. Uses the root <see cref="SkinTemplateNode.Location"/>
        /// wired by <c>TemplateIRBuilder</c>; falls back to a line 1 anchor on
        /// <c>TemplateName</c> so the source map always traces generated JST back
        /// to the template file rather than reporting null positions (which would
        /// drop the mapping entry entirely per V3 source-map semantics).
        /// The <c>_ir</c> field is populated by the ctor and <c>_ir.TemplateName</c>
        /// is always set by <c>TemplateIRBuilder</c>, so no null-defensive branches
        /// are needed — callers always hit this after <see cref="Generate"/> starts.
        /// </summary>
        private Location GetTemplateLocation()
        {
            return _ir.Location ?? new Location(_ir.TemplateName, 1, 0);
        }

        /// <summary>
        /// Generates a list of JST statements representing the complete template output:
        /// 1. tmplStore declaration
        /// 2. Factory function
        /// 3. Storage variable (TemplateName_var = null)
        /// 4. Getter function
        /// </summary>
        public List<Statement> Generate()
        {
            var statements = new List<Statement>();

            // Create the factory function scope with parameters "skinFactory" and "doc"
            _factoryScope = new IdentifierScope(
                _scopeManager.Scope,
                new string[] { "skinFactory", "doc" },
                false);

            // Create scope identifiers for template-level names
            _factoryMethodIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                _scopeManager.Scope,
                _ir.TemplateName + "_factory",
                false);

            _skinStorageVariable = SimpleIdentifier.CreateScopeIdentifier(
                _scopeManager.Scope,
                _ir.TemplateName + "_var",
                false);

            // Use pre-created identifier if available (from Initialize), otherwise create new
            _getterMethodIdentifier = (_preCreatedGetterIdentifier as SimpleIdentifier)
                ?? SimpleIdentifier.CreateScopeIdentifier(
                    _scopeManager.Scope,
                    _ir.TemplateName,
                    false);

            // Create the tmplStore variable — use a scope identifier like XWML's GetGlobalStateVariable
            _tmplStoreIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                _scopeManager.Scope,
                _ir.TemplateName + "_tmplStore",
                false);

            // Local variables within factory scope
            _domStoreIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                _factoryScope, "domStore", false);
            _htmlRootIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                _factoryScope, "htmlRoot", false);
            _objStorageIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                _factoryScope, "objStorage", false);

            // Resolve sub-control attributes (TagName, DomAttributes) via Cecil
            // Must happen before CollectHtmlWithPathsPublic which reads SubControlNode.TagName
            ResolveSubControlAttributes(_ir.Children);

            // Collect bindings, events, and HTML content
            var bindings = RazorSkinCodeGenerator.CollectBindingsPublic(_ir.Children);
            var events = RazorSkinCodeGenerator.CollectEventsPublic(_ir.Children);
            var elementPaths = new List<List<int>>();
            var eventPaths = new List<List<int>>();
            var htmlContent = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(
                _ir.Children, events, elementPaths, eventPaths);

            // Replace CSS class names with minified versions when @styles are active
            if (_cssManager != null && _cssManager.HasStylesheets)
            {
                htmlContent = ReplaceCssClassNamesInHtml(htmlContent);
            }

            // Build graph topology from IR
            var topology = GraphTopologyBuilder.Build(_ir);
            _topology = topology;

            // Build known function names from @functions blocks
            var knownFunctionNames = new HashSet<string>();
            if (_ir.Functions != null)
            {
                foreach (var func in _ir.Functions)
                {
                    if (func.FunctionName != "functions_block")
                        knownFunctionNames.Add(func.FunctionName);
                }
            }

            var templateLocation = GetTemplateLocation();

            // 1. tmplStore = new Array(1)
            statements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(_tmplStoreIdentifier, _scopeManager.Scope),
                    new MethodCallExpression(
                        templateLocation,
                        _scopeManager.Scope,
                        new IdentifierExpression(
                            RawNameIdentifier.Create(_scopeManager.Scope, "Array"),
                            _scopeManager.Scope),
                        new NumberLiteralExpression(_scopeManager.Scope, 1))));

            // 2. Factory function
            var factoryStatements = BuildFactoryBody(
                bindings, events, htmlContent, elementPaths, eventPaths, knownFunctionNames, topology);

            var factoryFunction = new FunctionExpression(
                templateLocation,
                _scopeManager.Scope,
                _factoryScope,
                _factoryScope.ParameterIdentifiers,
                _factoryMethodIdentifier);

            factoryFunction.AddStatements(factoryStatements);

            statements.Add(
                new ExpressionStatement(
                    templateLocation,
                    _scopeManager.Scope,
                    factoryFunction));

            // 3. Storage variable: TemplateName_var = null
            statements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(_skinStorageVariable, _scopeManager.Scope),
                    new NullLiteralExpression(_scopeManager.Scope)));

            // 4. Getter function
            statements.Add(BuildGetterFunction());

            Log.Debug("RazorSkinJSTGenerator produced {StatementCount} JST statements for template {TemplateName}",
                statements.Count, _ir.TemplateName);

            return statements;
        }

        /// <summary>
        /// Builds the factory function body statements.
        /// </summary>
        private List<Statement> BuildFactoryBody(
            List<ExpressionBindingNode> bindings,
            List<EventNode> events,
            string htmlContent,
            List<List<int>> elementPaths,
            List<List<int>> eventPaths,
            HashSet<string> knownFunctionNames,
            GraphTopology topology)
        {
            _eventPaths = eventPaths;
            var stmts = new List<Statement>();
            var templateLocation = GetTemplateLocation();

            // Get the "doc" parameter identifier
            IIdentifier docParam = _factoryScope.ParameterIdentifiers[1];

            // Resolve DocStorageGetter identifier
            IIdentifier docStorageGetterId = GetResolvedIdentifier("DocStorageGetter");

            // Build the if-block: if (!(domStore = DocStorageGetter(doc))[0]) { ... }
            Expression checkStateInitialized =
                new UnaryExpression(
                    templateLocation,
                    _factoryScope,
                    UnaryOperator.LogicalNot,
                    new IndexExpression(
                        null,
                        _factoryScope,
                        new BinaryExpression(
                            null,
                            _factoryScope,
                            BinaryOperator.Assignment,
                            new IdentifierExpression(_domStoreIdentifier, _factoryScope),
                            new MethodCallExpression(
                                null,
                                _factoryScope,
                                new IdentifierExpression(docStorageGetterId, _factoryScope),
                                new IdentifierExpression(docParam, _factoryScope))),
                        new NumberLiteralExpression(_factoryScope, _dataIndex)));

            var initStatements = new List<Statement>();

            // domStore[0] = doc.createElement("div")
            initStatements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        _factoryScope,
                        new IdentifierExpression(_domStoreIdentifier, _factoryScope),
                        new NumberLiteralExpression(_factoryScope, _dataIndex)),
                    new MethodCallExpression(
                        null,
                        _factoryScope,
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(docParam, _factoryScope),
                            new StringLiteralExpression(_factoryScope, "createElement")),
                        new StringLiteralExpression(_factoryScope, "div"))));

            // domStore[0].innerHTML = "<html content>"
            initStatements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        _factoryScope,
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_domStoreIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, _dataIndex)),
                        new StringLiteralExpression(_factoryScope, "innerHTML")),
                    new StringLiteralExpression(_factoryScope, htmlContent)));

            // tmplStore[dataIndex] = tmplStore[dataIndex] ? tmplStore[dataIndex] : graphDescriptor
            var graphEmitter = new GraphDescriptorJSTEmitter(
                topology, _factoryScope, _scopeManager, _knownTypes, knownFunctionNames,
                _clrContext, _ir.ModelTypeName, _resolvedTypeIdentifiers,
                cssManager: _cssManager,
                fallbackLocation: GetTemplateLocation());
            var graphDescriptorExpr = graphEmitter.Emit();

            initStatements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        _factoryScope,
                        new IdentifierExpression(_tmplStoreIdentifier, _factoryScope),
                        new NumberLiteralExpression(_factoryScope, _dataIndex)),
                    new ConditionalOperatorExpression(
                        null,
                        _factoryScope,
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_tmplStoreIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, _dataIndex)),
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_tmplStoreIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, _dataIndex)),
                        graphDescriptorExpr)));

            // Wrap in if block
            stmts.Add(
                new IfBlockStatement(
                    templateLocation,
                    _factoryScope,
                    checkStateInitialized,
                    new ScopeBlock(templateLocation, _factoryScope, initStatements),
                    null));

            // htmlRoot = domStore[0].cloneNode(true)
            stmts.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(_htmlRootIdentifier, _factoryScope),
                    new MethodCallExpression(
                        null,
                        _factoryScope,
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IndexExpression(
                                null,
                                _factoryScope,
                                new IdentifierExpression(_domStoreIdentifier, _factoryScope),
                                new NumberLiteralExpression(_factoryScope, _dataIndex)),
                            new StringLiteralExpression(_factoryScope, "cloneNode")),
                        new BooleanLiteralExpression(_factoryScope, true))));

            // objStorage = new Array(totalSlots)
            // The topology assigns element indices for ALL element types:
            // DomTarget (text span markers + attr-bound elements), Events, Gate markers, Collection markers
            int totalSlots = topology.TotalElemSlots;
            if (totalSlots > 0)
            {
                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                        new NewArrayExpression(
                            null,
                            _factoryScope,
                            new NumberLiteralExpression(_factoryScope, totalSlots))));
            }
            else
            {
                // Empty array for templates with no bindings
                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                        new InlineNewArrayInitialization(null, _factoryScope, new List<Expression>())));
            }

            IIdentifier getElementFromPathId = GetResolvedIdentifier(
                "Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath");

            // Elements inside gate branches don't exist in static HTML — they're resolved
            // at runtime when the gate renders its template. Skip their objStorage entries.
            var gatedElemIndices = topology.GetGatedElemIndices();

            // Assign element references for DomTargets using computed element paths
            for (int i = 0; i < bindings.Count; i++)
            {
                var dt = topology.DomTargets.FirstOrDefault(d => d.NodeIdx == FindDomTargetNodeIdx(topology, i));
                int elemIdx = dt?.ElemIdx ?? i;
                if (gatedElemIndices.Contains(elemIdx)) continue; // resolved at runtime by gate

                var path = i < elementPaths.Count ? elementPaths[i] : new List<int> { 0 };
                var pathElements = new List<Expression>();
                foreach (var p in path)
                    pathElements.Add(new NumberLiteralExpression(_factoryScope, p));

                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, elemIdx)),
                        new MethodCallExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(getElementFromPathId, _factoryScope),
                            new IdentifierExpression(_htmlRootIdentifier, _factoryScope),
                            new InlineNewArrayInitialization(
                                null,
                                _factoryScope,
                                pathElements))));
            }

            // Assign element references for gate markers.
            // Skip nested gate markers — they're inside a parent gate's template
            // and will be resolved at runtime when the parent gate renders.
            foreach (var gate in topology.Gates)
            {
                if (gatedElemIndices.Contains(gate.MarkerIdx)) continue;

                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, gate.MarkerIdx)),
                        new MethodCallExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(getElementFromPathId, _factoryScope),
                            new IdentifierExpression(_htmlRootIdentifier, _factoryScope),
                            BuildGateMarkerPath(htmlContent, gate))));
            }

            // Assign element references for collection markers (same pattern as gates)
            foreach (var coll in topology.Collections)
            {
                if (gatedElemIndices.Contains(coll.MarkerIdx)) continue;

                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, coll.MarkerIdx)),
                        new MethodCallExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(getElementFromPathId, _factoryScope),
                            new IdentifierExpression(_htmlRootIdentifier, _factoryScope),
                            BuildCollectionMarkerPath(htmlContent, coll))));
            }

            // Assign element references for events.
            foreach (var evt in topology.Events)
            {
                if (gatedElemIndices.Contains(evt.ElemIdx)) continue;

                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, evt.ElemIdx)),
                        BuildEventElementRef(htmlContent, evt)));
            }

            // Part ID mapping — not yet implemented for Razor templates (always null).
            // XWML uses a custom expression for this; Razor will add support when
            // id-based part access is needed.
            var partIdExpr = new NullLiteralExpression(_factoryScope);

            // return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], partMap, liveBinderCount, 0)
            IIdentifier skinInstanceFactoryId = GetResolvedIdentifier(
                "Sunlight__Framework__UI__Helpers__SkinInstance_factory");

            stmts.Add(
                new ReturnStatement(
                    templateLocation,
                    _factoryScope,
                    new MethodCallExpression(
                        templateLocation,
                        _factoryScope,
                        new IdentifierExpression(skinInstanceFactoryId, _factoryScope),
                        // skinFactory param
                        new IdentifierExpression(
                            _factoryScope.ParameterIdentifiers[0],
                            _factoryScope),
                        // htmlRoot
                        new IdentifierExpression(_htmlRootIdentifier, _factoryScope),
                        // UIElements (empty array for simple templates)
                        new InlineNewArrayInitialization(
                            null,
                            _factoryScope,
                            new List<Expression>()),
                        // objStorage
                        new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                        // binders from tmplStore
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_tmplStoreIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, _dataIndex)),
                        // partMap
                        partIdExpr,
                        // liveBinderCount (0 — graph engine handles reactivity)
                        new NumberLiteralExpression(_factoryScope, 0),
                        // extraObjectCount
                        new NumberLiteralExpression(_factoryScope, 0))));

            return stmts;
        }

        /// <summary>
        /// Builds the getter function that lazily creates the Skin instance.
        /// </summary>
        private Statement BuildGetterFunction()
        {
            var methodScope = new IdentifierScope(_scopeManager.Scope, 0);
            var templateLocation = GetTemplateLocation();

            var getterFunction = new FunctionExpression(
                templateLocation,
                _scopeManager.Scope,
                methodScope,
                methodScope.ParameterIdentifiers,
                _getterMethodIdentifier);

            // Resolve Skin_factory identifier
            IIdentifier skinFactoryId = GetResolvedIdentifier(
                "Sunlight__Framework__UI__Skin_factory");

            // Resolve control type and model type
            Expression controlTypeExpr = GetResolvedTypeExpression(
                _ir.ControlTypeName, methodScope);
            Expression modelTypeExpr = GetResolvedTypeExpression(
                _ir.ModelTypeName, methodScope);

            // TemplateName_var = Skin_factory(ControlType, ModelType, factory, "0")
            var initialization = ExpressionStatement.CreateAssignmentExpression(
                new IdentifierExpression(_skinStorageVariable, methodScope),
                new MethodCallExpression(
                    templateLocation,
                    methodScope,
                    new IdentifierExpression(skinFactoryId, methodScope),
                    controlTypeExpr,
                    modelTypeExpr,
                    new IdentifierExpression(_factoryMethodIdentifier, methodScope),
                    new StringLiteralExpression(methodScope, _dataIndex.ToString())));

            // if (!TemplateName_var)
            var initIfStatement = new IfBlockStatement(
                templateLocation,
                methodScope,
                new UnaryExpression(
                    templateLocation,
                    methodScope,
                    UnaryOperator.LogicalNot,
                    new IdentifierExpression(_skinStorageVariable, methodScope)),
                new ScopeBlock(
                    templateLocation,
                    methodScope,
                    new List<Statement> { initialization }),
                null);

            getterFunction.AddStatement(initIfStatement);
            getterFunction.AddStatement(
                new ReturnStatement(
                    templateLocation,
                    methodScope,
                    new IdentifierExpression(_skinStorageVariable, methodScope)));

            return new ExpressionStatement(
                templateLocation,
                _scopeManager.Scope,
                getterFunction);
        }

        /// <summary>
        /// Retrieves a resolved identifier by its mangled name, falling back to
        /// a raw name identifier if the identifier was not resolved during initialization.
        /// </summary>
        private IIdentifier GetResolvedIdentifier(string mangledName)
        {
            if (_resolvedIdentifiers.TryGetValue(mangledName, out var id))
                return id;

            Log.Debug("Identifier {MangledName} not resolved, using raw name fallback", mangledName);
            return RawNameIdentifier.Create(_scopeManager.Scope, mangledName);
        }

        /// <summary>
        /// Builds a type reference expression using resolved type identifiers,
        /// falling back to a raw name if not resolved.
        /// </summary>
        private Expression GetResolvedTypeExpression(string csharpTypeName, IdentifierScope scope)
        {
            if (string.IsNullOrEmpty(csharpTypeName))
                return new NullLiteralExpression(scope);

            var mangledName = csharpTypeName.Replace(".", "__");

            if (_resolvedTypeIdentifiers.TryGetValue(mangledName, out var identifiers)
                && identifiers.Count > 0)
            {
                return IdentifierExpression.Create(null, scope, identifiers);
            }

            // Fallback: use the mangled name as-is
            Log.Debug("Type {TypeName} not resolved, using raw name fallback", csharpTypeName);
            return new IdentifierExpression(
                RawNameIdentifier.Create(_scopeManager.Scope, mangledName), scope);
        }

        /// <summary>
        /// Gets the getter method identifier. This is used by the plugin to build
        /// GetOverwrite statements that reference the getter by scope identifier.
        /// </summary>
        public IIdentifier GetGetterIdentifier() => _getterMethodIdentifier;

        /// <summary>
        /// Builds a proper JST expression for the getter function body by resolving
        /// property getter methods through the Cecil type system. This ensures that
        /// the generated JS uses minified method names (e.g., get_propStr1_c) instead
        /// of raw unminified names (e.g., get_propStr1).
        ///
        /// For simple expressions like "Model.PropStr1", builds:
        ///   dc.get_propStr1_c()
        ///
        /// For property chains like "Model.Customer.Name", builds:
        ///   dc.get_customer_x().get_name_y()
        ///
        /// Returns null if the expression cannot be resolved (complex/computed).
        /// </summary>
        private Expression TryBuildResolvedGetterExpression(
            ExpressionBindingNode binding,
            IdentifierScope getterScope,
            IIdentifier paramIdentifier)
        {
            if (_clrContext == null) return null;

            var expr = binding.Classification.CSharpExpression;
            if (string.IsNullOrEmpty(expr)) return null;

            // Determine the source prefix and type name
            string prefix;
            string typeName;
            if (expr.StartsWith("Model."))
            {
                prefix = "Model.";
                typeName = _ir.ModelTypeName;
            }
            else if (expr.StartsWith("Control."))
            {
                prefix = "Control.";
                typeName = _ir.ControlTypeName;
            }
            else
            {
                return null; // Not a simple Model/Control property access
            }

            // Extract the property path after the prefix
            var propertyPath = expr.Substring(prefix.Length);

            // Bail out for operators or complex expressions (computed, ternary, etc.)
            if (propertyPath.IndexOfAny(new[] { ' ', '(', '+', '-', '*', '/', '?', '!' }) >= 0)
                return null;

            // Split property chain (e.g., "Customer.Name" -> ["Customer", "Name"])
            var properties = propertyPath.Split('.');
            if (properties.Length == 0) return null;

            // Resolve the source type
            var typeDefinition = FindTypeDefinition(typeName);
            if (typeDefinition == null)
            {
                Log.Debug("Cannot resolve type {TypeName} for getter expression", typeName);
                return null;
            }

            // Build a chain of method calls: dc.get_prop1().get_prop2()...
            Expression currentExpr = new IdentifierExpression(paramIdentifier, getterScope);
            var currentType = typeDefinition;

            foreach (var propName in properties)
            {
                var property = FindProperty(currentType, propName);
                if (property?.GetMethod == null)
                {
                    Log.Debug("Cannot find property getter {PropName} on type {TypeName}",
                        propName, currentType.FullName);
                    return null;
                }

                // Resolve the getter method to get the minified identifier
                var getterMethodId = _scopeManager.Resolve(property.GetMethod);

                // Build: currentExpr.get_propName()
                currentExpr = new MethodCallExpression(
                    null,
                    getterScope,
                    new IndexExpression(
                        null,
                        getterScope,
                        currentExpr,
                        new IdentifierExpression(getterMethodId, getterScope)),
                    System.Array.Empty<Expression>());

                // Advance to the property's return type for chained access
                currentType = property.PropertyType?.Resolve();
                if (currentType == null && properties.Length > 1)
                {
                    Log.Debug("Cannot resolve return type of {PropName} for chain traversal", propName);
                    return null;
                }
            }

            return currentExpr;
        }

        private TypeDefinition FindTypeDefinition(string fullTypeName)
            => _typeHelper.FindTypeDefinition(fullTypeName);

        private PropertyDefinition FindProperty(TypeDefinition type, string propertyName)
            => _typeHelper.FindProperty(type, propertyName);

        /// <summary>
        /// Finds the DomTarget node index for the i-th binding (in document order).
        /// DomTargets are added in the same order as bindings are walked.
        /// </summary>
        private static int FindDomTargetNodeIdx(GraphTopology topology, int bindingIndex)
        {
            if (bindingIndex < topology.DomTargets.Count)
                return topology.DomTargets[bindingIndex].NodeIdx;
            return -1;
        }

        /// <summary>
        /// Computes the DOM path for a gate marker span in the cleaned HTML.
        /// Parses the HTML string to find the N-th empty span (gate marker).
        /// </summary>
        private Expression BuildGateMarkerPath(string htmlContent, GateTopology gate)
        {
            var path = FindEmptySpanPath(htmlContent, gate.MarkerIdx, "gate");
            var pathElements = new List<Expression>();
            foreach (var p in path)
                pathElements.Add(new NumberLiteralExpression(_factoryScope, p));
            return new InlineNewArrayInitialization(null, _factoryScope, pathElements);
        }

        /// <summary>
        /// Computes the DOM path for a collection marker span in the cleaned HTML.
        /// </summary>
        private Expression BuildCollectionMarkerPath(string htmlContent, CollectionTopology coll)
        {
            var path = FindEmptySpanPath(htmlContent, coll.MarkerIdx, "collection");
            var pathElements = new List<Expression>();
            foreach (var p in path)
                pathElements.Add(new NumberLiteralExpression(_factoryScope, p));
            return new InlineNewArrayInitialization(null, _factoryScope, pathElements);
        }

        /// <summary>
        /// Finds the DOM path for the marker with the given elemIdx.
        /// Gate and collection markers are empty <span></span> elements in the static HTML.
        /// Text content binding markers are also empty <span></span> elements.
        /// We count ALL empty spans to determine the ordinal position of the target marker.
        /// Elements inside gate branches are excluded (not in static HTML).
        /// </summary>
        private List<int> FindEmptySpanPath(string html, int targetIdx, string kind)
        {
            // Collect ALL elem indices that produce empty <span></span> in static HTML:
            // text content bindings, gate markers, collection markers.
            // Exclude gated elements (inside gate branches — not in static HTML).
            var gatedIndices = _topology.GetGatedElemIndices();
            var allEmptySpanIndices = new List<int>();

            // Text content bindings produce empty spans
            foreach (var dt in _topology.DomTargets)
            {
                if (dt.Target == TemplateIR.ExpressionTarget.TextContent
                    && !gatedIndices.Contains(dt.ElemIdx))
                {
                    allEmptySpanIndices.Add(dt.ElemIdx);
                }
            }

            // Gate markers produce empty spans
            foreach (var g in _topology.Gates)
            {
                if (!gatedIndices.Contains(g.MarkerIdx))
                    allEmptySpanIndices.Add(g.MarkerIdx);
            }

            // Collection markers produce empty spans
            foreach (var c in _topology.Collections)
            {
                if (!gatedIndices.Contains(c.MarkerIdx))
                    allEmptySpanIndices.Add(c.MarkerIdx);
            }

            allEmptySpanIndices.Sort();

            // Find which ordinal position among all empty spans this target is at
            int ordinal = allEmptySpanIndices.IndexOf(targetIdx);
            if (ordinal < 0) ordinal = 0;

            return FindNthEmptySpanPath(html, ordinal);
        }

        /// <summary>
        /// Parses HTML to find the N-th empty span element and returns its DOM path.
        /// </summary>
        private static List<int> FindNthEmptySpanPath(string html, int n)
        {
            var indexStack = new List<int>();
            var childCountStack = new List<int> { 0 };
            int emptySpanCount = 0;
            bool hasText = false;

            int i = 0;
            while (i < html.Length)
            {
                if (html[i] == '<')
                {
                    // Flush text node
                    if (hasText) { childCountStack[childCountStack.Count - 1]++; hasText = false; }

                    if (i + 1 < html.Length && html[i + 1] == '/')
                    {
                        var closeEnd = html.IndexOf('>', i);
                        if (closeEnd < 0) break;
                        if (indexStack.Count > 0)
                        {
                            indexStack.RemoveAt(indexStack.Count - 1);
                            childCountStack.RemoveAt(childCountStack.Count - 1);
                        }
                        i = closeEnd + 1;
                        hasText = false;
                        continue;
                    }

                    var tagEnd = html.IndexOf('>', i);
                    if (tagEnd < 0) break;
                    var tagContent = html.Substring(i + 1, tagEnd - i - 1);
                    bool selfClosing = tagContent.EndsWith("/");
                    int myIndex = childCountStack[childCountStack.Count - 1];
                    childCountStack[childCountStack.Count - 1]++;

                    // Check if this is <span></span> (empty span = gate/collection marker)
                    var tagName = tagContent.Split(new[] { ' ', '/' }, System.StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault()?.ToLower() ?? "";
                    if (tagName == "span" && !selfClosing)
                    {
                        // Check if immediately followed by </span>
                        var closeSpanIdx = html.IndexOf("</span>", tagEnd + 1);
                        if (closeSpanIdx == tagEnd + 1)
                        {
                            // Empty span found
                            if (emptySpanCount == n)
                            {
                                return new List<int>(indexStack) { myIndex };
                            }
                            emptySpanCount++;

                            // Skip past the closing tag
                            indexStack.Add(myIndex);
                            childCountStack.Add(0);
                            indexStack.RemoveAt(indexStack.Count - 1);
                            childCountStack.RemoveAt(childCountStack.Count - 1);
                            i = closeSpanIdx + 7; // past </span>
                            continue;
                        }
                    }

                    if (!selfClosing)
                    {
                        indexStack.Add(myIndex);
                        childCountStack.Add(0);
                    }
                    i = tagEnd + 1;
                    hasText = false;
                }
                else
                {
                    hasText = true;
                    i++;
                }
            }

            // Fallback
            return new List<int> { 0 };
        }

        /// <summary>
        /// Builds an element reference expression for an event target.
        /// Finds the event target element in the HTML by looking for the element
        /// that originally had the onclick/onX attribute.
        /// </summary>
        private Expression BuildEventElementRef(string htmlContent, EventTopology evt)
        {
            // Events target specific elements in the template HTML.
            // The event target element was identified during IR building and marked
            // with data-evt-idx attributes. The paths were computed from those markers
            // by ComputePathsFromHtml and passed via _eventPaths.
            // The topology assigns event element indices in document order.
            // Find the ordinal position of this event among all events.
            int ordinal = 0;
            foreach (var e in _topology.Events)
            {
                if (e.ElemIdx == evt.ElemIdx) break;
                ordinal++;
            }

            // Use the marker-computed path if available, otherwise fall back to heuristic
            List<int> path;
            if (_eventPaths != null && ordinal < _eventPaths.Count)
            {
                path = _eventPaths[ordinal];
            }
            else
            {
                path = FindNthInteractiveElementPath(htmlContent, ordinal);
            }

            var pathElements = new List<Expression>();
            foreach (var p in path)
                pathElements.Add(new NumberLiteralExpression(_factoryScope, p));

            IIdentifier getElementFromPathId = GetResolvedIdentifier(
                "Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath");

            return new MethodCallExpression(
                null,
                _factoryScope,
                new IdentifierExpression(getElementFromPathId, _factoryScope),
                new IdentifierExpression(_htmlRootIdentifier, _factoryScope),
                new InlineNewArrayInitialization(null, _factoryScope, pathElements));
        }

        /// <summary>
        /// Finds the DOM path for the N-th interactive element (button, a, input) in the HTML.
        /// </summary>
        private static List<int> FindNthInteractiveElementPath(string html, int n)
        {
            var indexStack = new List<int>();
            var childCountStack = new List<int> { 0 };
            int interactiveCount = 0;
            bool hasText = false;

            int i = 0;
            while (i < html.Length)
            {
                if (html[i] == '<')
                {
                    if (hasText) { childCountStack[childCountStack.Count - 1]++; hasText = false; }

                    if (i + 1 < html.Length && html[i + 1] == '/')
                    {
                        var closeEnd = html.IndexOf('>', i);
                        if (closeEnd < 0) break;
                        if (indexStack.Count > 0)
                        {
                            indexStack.RemoveAt(indexStack.Count - 1);
                            childCountStack.RemoveAt(childCountStack.Count - 1);
                        }
                        i = closeEnd + 1;
                        hasText = false;
                        continue;
                    }

                    var tagEnd = html.IndexOf('>', i);
                    if (tagEnd < 0) break;
                    var tagContent = html.Substring(i + 1, tagEnd - i - 1);
                    bool selfClosing = tagContent.EndsWith("/");
                    int myIndex = childCountStack[childCountStack.Count - 1];
                    childCountStack[childCountStack.Count - 1]++;

                    var tagName = tagContent.Split(new[] { ' ', '/' }, System.StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault()?.ToLower() ?? "";

                    if (tagName == "button" || tagName == "a" || tagName == "input")
                    {
                        if (interactiveCount == n)
                        {
                            return new List<int>(indexStack) { myIndex };
                        }
                        interactiveCount++;
                    }

                    if (!selfClosing)
                    {
                        indexStack.Add(myIndex);
                        childCountStack.Add(0);
                    }
                    i = tagEnd + 1;
                    hasText = false;
                }
                else
                {
                    hasText = true;
                    i++;
                }
            }

            // Fallback
            return new List<int> { 0 };
        }

        /// <summary>
        /// Replaces CSS class names in HTML class="..." attributes with their minified versions.
        /// Uses the same regex pattern as TemplateIRBuilder.ValidateCssClassesInHtml.
        /// </summary>
        private string ReplaceCssClassNamesInHtml(string html)
        {
            return RazorCssManager.ReplaceCssClassNamesInHtml(html, _cssManager);
        }

        /// <summary>
        /// Recursively resolves TagName and DomAttributes on SubControlNode instances
        /// by looking up the control type via Cecil and reading [TagName] and [DomAttribute] attributes.
        /// </summary>
        private void ResolveSubControlAttributes(List<IRNode> nodes)
        {
            if (nodes == null) return;

            foreach (var node in nodes)
            {
                if (node is SubControlNode sub)
                {
                    ResolveSubControlTagInfo(sub);
                }

                if (node.Children?.Count > 0)
                    ResolveSubControlAttributes(node.Children);

                if (node is ConditionalNode cond)
                {
                    ResolveSubControlAttributes(cond.TrueBranch);
                    ResolveSubControlAttributes(cond.FalseBranch);
                }

                if (node is LoopNode loop)
                {
                    ResolveSubControlAttributes(loop.ItemTemplate);
                }
            }
        }

        /// <summary>
        /// Resolves tag name and DOM attributes for a single SubControlNode by looking up its
        /// CLR type via Cecil and reading [TagName] and [DomAttribute] custom attributes.
        /// </summary>
        private void ResolveSubControlTagInfo(SubControlNode sub)
        {
            if (_clrContext == null || _knownTypes == null) return;

            var typeDef = FindSubControlType(sub.TypeName);
            if (typeDef == null)
            {
                Log.Debug("ResolveSubControlTagInfo: Cannot find type for {TypeName}", sub.TypeName);
                return;
            }

            sub.ResolvedTypeName = typeDef.FullName;

            // Read [TagName("xxx")] attribute
            if (_knownTypes.TagNameAttribute != null)
            {
                foreach (var attr in typeDef.CustomAttributes)
                {
                    if (attr.AttributeType.FullName == _knownTypes.TagNameAttribute.FullName
                        && attr.ConstructorArguments.Count > 0)
                    {
                        sub.TagName = attr.ConstructorArguments[0].Value as string ?? "div";
                        break;
                    }
                }
            }

            // Read [DomAttribute("name", "value")] attributes
            if (_knownTypes.DomAttributeAttribute != null)
            {
                foreach (var attr in typeDef.CustomAttributes)
                {
                    if (attr.AttributeType.FullName == _knownTypes.DomAttributeAttribute.FullName
                        && attr.ConstructorArguments.Count >= 2)
                    {
                        var name = attr.ConstructorArguments[0].Value as string;
                        var value = attr.ConstructorArguments[1].Value as string;
                        if (name != null)
                        {
                            if (sub.DomAttributes == null)
                                sub.DomAttributes = new List<KeyValuePair<string, string>>();
                            sub.DomAttributes.Add(new KeyValuePair<string, string>(name, value ?? ""));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds a SubControlNode's TypeDefinition by trying the type name directly,
        /// then trying each using namespace as a prefix.
        /// </summary>
        private TypeDefinition FindSubControlType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;

            var typeDef = FindTypeDefinitionByName(typeName);
            if (typeDef != null) return typeDef;

            if (_ir.UsingNamespaces != null)
            {
                foreach (var ns in _ir.UsingNamespaces)
                {
                    typeDef = FindTypeDefinitionByName(ns + "." + typeName);
                    if (typeDef != null) return typeDef;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a TypeDefinition by full name across all loaded assemblies.
        /// </summary>
        private TypeDefinition FindTypeDefinitionByName(string fullName)
        {
            try
            {
                foreach (var type in _clrContext.GetTypeDefinitions())
                {
                    if (type.FullName == fullName)
                        return type;
                }
            }
            catch { }

            return null;
        }
    }

    /// <summary>
    /// Helper to create fallback identifiers that output a fixed name string.
    /// Uses SimpleIdentifier with enforceSuggestion=true so the JSWriter can handle them.
    /// </summary>
    internal static class RawNameIdentifier
    {
        /// <summary>
        /// Creates a SimpleIdentifier on the given scope with an enforced name.
        /// The name will be output as-is without minification.
        /// </summary>
        public static SimpleIdentifier Create(IdentifierScope scope, string name)
        {
            return SimpleIdentifier.CreateScopeIdentifier(scope, name, true);
        }
    }

}
