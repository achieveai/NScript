using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NScript.CLR;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.RazorSkin.TemplateIR;
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
        // XWML templates use sequential indices starting from 0 (one per skin).
        // We start at 100 to avoid collision. If a project has 100+ XWML templates,
        // this offset must be increased. TODO: Coordinate with XWML's CodeGenerator
        // to allocate from a shared counter instead of a hardcoded offset.
        private const int RazorDataIndexOffset = 100;
        private static int _next_dataIndex = RazorDataIndexOffset;
        private readonly int _dataIndex;


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
            IIdentifier preCreatedGetterIdentifier = null)
        {
            _ir = ir;
            _scopeManager = scopeManager;
            _clrContext = clrContext;
            _resolvedIdentifiers = resolvedIdentifiers;
            _resolvedTypeIdentifiers = resolvedTypeIdentifiers;
            _knownTypes = knownTypes;
            _preCreatedGetterIdentifier = preCreatedGetterIdentifier;
            _dataIndex = _next_dataIndex++;
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

            // Collect bindings, events, and HTML content
            var bindings = RazorSkinCodeGenerator.CollectBindingsPublic(_ir.Children);
            var events = RazorSkinCodeGenerator.CollectEventsPublic(_ir.Children);
            var elementPaths = new List<List<int>>();
            var htmlContent = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(
                _ir.Children, events, elementPaths);
            // Build graph topology from IR
            var topology = GraphTopologyBuilder.Build(_ir);

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

            // 1. tmplStore = new Array(1)
            statements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(_tmplStoreIdentifier, _scopeManager.Scope),
                    new MethodCallExpression(
                        null,
                        _scopeManager.Scope,
                        new IdentifierExpression(
                            RawNameIdentifier.Create(_scopeManager.Scope, "Array"),
                            _scopeManager.Scope),
                        new NumberLiteralExpression(_scopeManager.Scope, 1))));

            // 2. Factory function
            var factoryStatements = BuildFactoryBody(
                bindings, events, htmlContent, elementPaths, knownFunctionNames, topology);

            var factoryFunction = new FunctionExpression(
                null,
                _scopeManager.Scope,
                _factoryScope,
                _factoryScope.ParameterIdentifiers,
                _factoryMethodIdentifier);

            factoryFunction.AddStatements(factoryStatements);

            statements.Add(
                new ExpressionStatement(
                    null,
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
            HashSet<string> knownFunctionNames,
            GraphTopology topology)
        {
            var stmts = new List<Statement>();

            // Get the "doc" parameter identifier
            IIdentifier docParam = _factoryScope.ParameterIdentifiers[1];

            // Resolve DocStorageGetter identifier
            IIdentifier docStorageGetterId = GetResolvedIdentifier("DocStorageGetter");

            // Build the if-block: if (!(domStore = DocStorageGetter(doc))[0]) { ... }
            Expression checkStateInitialized =
                new UnaryExpression(
                    null,
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
                topology, _factoryScope, _scopeManager, _knownTypes, knownFunctionNames);
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
                    null,
                    _factoryScope,
                    checkStateInitialized,
                    new ScopeBlock(null, _factoryScope, initStatements),
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
            int totalSlots = bindings.Count + events.Count;
            stmts.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                    new NewArrayExpression(
                        null,
                        _factoryScope,
                        new NumberLiteralExpression(_factoryScope, totalSlots))));

            // objStorage[i] = GetElementFromPath(htmlRoot, [path])
            IIdentifier getElementFromPathId = GetResolvedIdentifier(
                "Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath");

            for (int i = 0; i < bindings.Count; i++)
            {
                var path = i < elementPaths.Count ? elementPaths[i] : new List<int> { i + 1 };
                var pathElements = new List<Expression>();
                foreach (var p in path)
                    pathElements.Add(new NumberLiteralExpression(_factoryScope, p));

                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, i)),
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

            // Event element paths (events target htmlRoot for now)
            for (int i = 0; i < events.Count; i++)
            {
                int elemIdx = bindings.Count + i;
                stmts.Add(
                    ExpressionStatement.CreateAssignmentExpression(
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_objStorageIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, elemIdx)),
                        new IdentifierExpression(_htmlRootIdentifier, _factoryScope)));
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
                    null,
                    _factoryScope,
                    new MethodCallExpression(
                        null,
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

            var getterFunction = new FunctionExpression(
                null,
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
                    null,
                    methodScope,
                    new IdentifierExpression(skinFactoryId, methodScope),
                    controlTypeExpr,
                    modelTypeExpr,
                    new IdentifierExpression(_factoryMethodIdentifier, methodScope),
                    new StringLiteralExpression(methodScope, _dataIndex.ToString())));

            // if (!TemplateName_var)
            var initIfStatement = new IfBlockStatement(
                null,
                methodScope,
                new UnaryExpression(
                    null,
                    methodScope,
                    UnaryOperator.LogicalNot,
                    new IdentifierExpression(_skinStorageVariable, methodScope)),
                new ScopeBlock(
                    null,
                    methodScope,
                    new List<Statement> { initialization }),
                null);

            getterFunction.AddStatement(initIfStatement);
            getterFunction.AddStatement(
                new ReturnStatement(
                    null,
                    methodScope,
                    new IdentifierExpression(_skinStorageVariable, methodScope)));

            return new ExpressionStatement(
                null,
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

        /// <summary>
        /// Finds a TypeDefinition by fully qualified name across all loaded assemblies.
        /// </summary>
        private TypeDefinition FindTypeDefinition(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName)) return null;

            return _clrContext.GetTypes()
                .FirstOrDefault(t => t.FullName == fullTypeName);
        }

        /// <summary>
        /// Finds a property on a type, walking up the inheritance hierarchy.
        /// </summary>
        private static PropertyDefinition FindProperty(TypeDefinition type, string propertyName)
        {
            var current = type;
            while (current != null)
            {
                var prop = current.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (prop != null) return prop;

                try { current = current.BaseType?.Resolve(); }
                catch { break; }
            }
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
