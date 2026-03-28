using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, IIdentifier> _resolvedIdentifiers;
        private readonly Dictionary<string, IList<IIdentifier>> _resolvedTypeIdentifiers;

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

        // Data index (always 0 for Razor templates — single template per resource)
        private const int DataIndex = 0;

        public RazorSkinJSTGenerator(
            SkinTemplateNode ir,
            RuntimeScopeManager scopeManager,
            Dictionary<string, IIdentifier> resolvedIdentifiers,
            Dictionary<string, IList<IIdentifier>> resolvedTypeIdentifiers)
        {
            _ir = ir;
            _scopeManager = scopeManager;
            _resolvedIdentifiers = resolvedIdentifiers;
            _resolvedTypeIdentifiers = resolvedTypeIdentifiers;
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

            _getterMethodIdentifier = SimpleIdentifier.CreateScopeIdentifier(
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
            int liveBinderCount = bindings.Count(b => b.Classification.Mode == BindingMode.OneWay);

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
                bindings, events, htmlContent, elementPaths, liveBinderCount, knownFunctionNames);

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
            int liveBinderCount,
            HashSet<string> knownFunctionNames)
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
                        new NumberLiteralExpression(_factoryScope, DataIndex)));

            var initStatements = new List<Statement>();

            // domStore[0] = doc.createElement("div")
            initStatements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        _factoryScope,
                        new IdentifierExpression(_domStoreIdentifier, _factoryScope),
                        new NumberLiteralExpression(_factoryScope, DataIndex)),
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
                            new NumberLiteralExpression(_factoryScope, DataIndex)),
                        new StringLiteralExpression(_factoryScope, "innerHTML")),
                    new StringLiteralExpression(_factoryScope, htmlContent)));

            // tmplStore[0] = tmplStore[0] ? tmplStore[0] : [binders...]
            var binderExpressions = BuildBinderExpressions(bindings, knownFunctionNames);
            initStatements.Add(
                ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(
                        null,
                        _factoryScope,
                        new IdentifierExpression(_tmplStoreIdentifier, _factoryScope),
                        new NumberLiteralExpression(_factoryScope, DataIndex)),
                    new ConditionalOperatorExpression(
                        null,
                        _factoryScope,
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_tmplStoreIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, DataIndex)),
                        new IndexExpression(
                            null,
                            _factoryScope,
                            new IdentifierExpression(_tmplStoreIdentifier, _factoryScope),
                            new NumberLiteralExpression(_factoryScope, DataIndex)),
                        new InlineNewArrayInitialization(
                            null,
                            _factoryScope,
                            binderExpressions))));

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
                                new NumberLiteralExpression(_factoryScope, DataIndex)),
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

            // Build part ID mapping expression
            var partIdMapping = RazorSkinCodeGenerator.BuildPartIdMappingPublic(_ir.Children);
            Expression partIdExpr;
            if (partIdMapping.Count > 0)
            {
                // Build an object literal as raw JS (part IDs are simple string->int maps)
                var entries = new List<Expression>();
                foreach (var kvp in partIdMapping)
                {
                    entries.Add(new StringLiteralExpression(_factoryScope, kvp.Key));
                    entries.Add(new NumberLiteralExpression(_factoryScope, kvp.Value));
                }
                // Use null for now since XWML also uses a custom expression for this
                partIdExpr = new NullLiteralExpression(_factoryScope);
            }
            else
            {
                partIdExpr = new NullLiteralExpression(_factoryScope);
            }

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
                            new NumberLiteralExpression(_factoryScope, DataIndex)),
                        // partMap
                        partIdExpr,
                        // liveBinderCount
                        new NumberLiteralExpression(_factoryScope, liveBinderCount),
                        // extraObjectCount
                        new NumberLiteralExpression(_factoryScope, 0))));

            return stmts;
        }

        /// <summary>
        /// Builds JST expressions for SkinBinderInfo_factory calls for each binding.
        /// </summary>
        private List<Expression> BuildBinderExpressions(
            List<ExpressionBindingNode> bindings,
            HashSet<string> knownFunctionNames)
        {
            var result = new List<Expression>();
            IIdentifier skinBinderInfoFactoryId = GetResolvedIdentifier(
                "Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory");

            for (int i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                var deps = binding.Classification.Dependencies;
                var expr = binding.Classification.CSharpExpression;

                // Getter function: function(dc) { return <jsExpr>; }
                var getterJs = ExpressionJsEmitter.ToJsGetter(expr, "dc", "tp", knownFunctionNames);
                var paramName = binding.Classification.SourceKind == BindingSourceKind.TemplateParent
                    ? "tp" : "dc";

                // Build getter function scope with the correct parameter name.
                // The raw JS body uses "dc" or "tp" as the parameter name, so we must
                // create the scope with that exact name enforced to prevent renaming.
                var getterScope = new IdentifierScope(
                    _factoryScope,
                    new string[] { paramName },
                    true);
                var getterFunc = new FunctionExpression(
                    null,
                    _factoryScope,
                    getterScope,
                    getterScope.ParameterIdentifiers,
                    null);

                // The getter function returns the JS expression.
                // Since the expression is text-based from the Razor parser, we use a RawJsExpression.
                getterFunc.AddStatement(
                    new ReturnStatement(
                        null,
                        getterScope,
                        new RawJsExpression(getterJs, getterScope)));

                var getterArray = new InlineNewArrayInitialization(
                    null,
                    _factoryScope,
                    new List<Expression> { getterFunc });

                // Property names array
                var propNameExprs = new List<Expression>();
                foreach (var dep in deps)
                    propNameExprs.Add(new StringLiteralExpression(_factoryScope, dep.PropertyName));
                var propNamesArray = new InlineNewArrayInitialization(
                    null,
                    _factoryScope,
                    propNameExprs);

                // Target setter
                IIdentifier setterId = GetSetterIdentifier(binding.Target);

                // Binder type flags
                bool isOneWay = binding.Classification.Mode == BindingMode.OneWay;
                int flags;
                if (binding.Classification.SourceKind == BindingSourceKind.TemplateParent)
                    flags = isOneWay ? 0x13 : 0x03;
                else
                    flags = isOneWay ? 0x11 : 0x01;

                // SkinBinderInfo_factory(getters, propNames, setter, flags, objIdx, binderIdx, converter, default)
                result.Add(
                    new MethodCallExpression(
                        null,
                        _factoryScope,
                        new IdentifierExpression(skinBinderInfoFactoryId, _factoryScope),
                        getterArray,
                        propNamesArray,
                        new IdentifierExpression(setterId, _factoryScope),
                        new NumberLiteralExpression(_factoryScope, flags),
                        new NumberLiteralExpression(_factoryScope, i),
                        new NumberLiteralExpression(_factoryScope, i),
                        new NullLiteralExpression(_factoryScope),
                        new StringLiteralExpression(_factoryScope, "")));
            }

            return result;
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
                    new StringLiteralExpression(methodScope, DataIndex.ToString())));

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
        /// Gets the resolved setter identifier for a binding target.
        /// </summary>
        private IIdentifier GetSetterIdentifier(ExpressionTarget target)
        {
            var mangledName = target switch
            {
                ExpressionTarget.TextContent => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent",
                ExpressionTarget.Attribute => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute",
                ExpressionTarget.CssClass => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetClassName",
                ExpressionTarget.Style => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetStyle",
                _ => "Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent"
            };

            return GetResolvedIdentifier(mangledName);
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

    /// <summary>
    /// A minimal JST Expression that outputs raw JavaScript text.
    /// Used for Razor-generated expressions (getter function bodies) that are
    /// text-based and cannot be decomposed into individual JST nodes.
    /// The surrounding function/identifiers are still proper JST nodes.
    /// </summary>
    internal class RawJsExpression : Expression
    {
        private readonly string _jsText;

        public RawJsExpression(string jsText, IdentifierScope scope)
            : base(null, scope)
        {
            _jsText = jsText;
        }

        public override Precedence Precedence => Precedence.Primary;

        public override void Serialize(NScript.Utils.ICustomSerializer serializer)
        {
            serializer.AddValue("raw", _jsText);
        }

        public override void Write(JSWriter writer)
        {
            writer.WriteIdentifier(_jsText);
        }
    }
}
