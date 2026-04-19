using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NScript.CLR;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;
using Serilog;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Emits a graph descriptor as a JST <see cref="InlineObjectInitializer"/> with function
    /// references resolved via <see cref="RuntimeScopeManager"/>. Uses proper JST nodes
    /// that participate in NScript's scope-based minification system.
    /// All field names are resolved through Cecil so they receive minified identifiers
    /// that match the runtime's field access patterns.
    /// </summary>
    public class GraphDescriptorJSTEmitter
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private readonly GraphTopology _topology;
        private readonly IdentifierScope _scope;
        private readonly RuntimeScopeManager _scopeManager;
        private readonly RazorKnownTypes _knownTypes;
        private readonly ISet<string> _knownFunctionNames;
        private readonly ClrContext _clrContext;
        private readonly string _modelTypeName;
        private readonly string _parentModelTypeName;
        private readonly Dictionary<string, IList<IIdentifier>> _resolvedTypeIdentifiers;
        private readonly CecilTypeHelper _typeHelper;
        private readonly RazorCssManager _cssManager;

        /// <summary>
        /// Fallback <see cref="Location"/> stamped on emitted JST nodes (function expressions,
        /// return statements, etc.) when no per-binding source position is available. Supplied
        /// by the caller (typically <see cref="RazorSkinJSTGenerator"/>) as the template root
        /// location so the final source map can attribute every generated descriptor expression
        /// back to the originating <c>.skin.cshtml</c> file. Null when the caller has no
        /// location context — downstream emission falls through to passing null, matching
        /// the pre-Phase-3b behavior.
        /// </summary>
        private readonly Location _fallbackLocation;

        // Cached maps for ToJsGetterWithFieldAccess — identical across all invocations
        // since _modelTypeName doesn't change.
        private Dictionary<string, string> _cachedFieldMap;
        private Dictionary<string, string> _cachedMethodMap;

        /// <summary>
        /// Whether this emitter generates for an item graph (inside a @foreach loop).
        /// Item graphs use tuple DataContext: [parentDC, control, item].
        /// </summary>
        private bool IsItemGraph => !string.IsNullOrEmpty(_topology.ItemVariablePrefix);

        // Resolved field identifiers for GraphDescriptor
        private readonly IIdentifier _nodeCountField;
        private readonly IIdentifier _nodeTypesField;
        private readonly IIdentifier _gettersField;
        private readonly IIdentifier _consumersField;
        private readonly IIdentifier _gateIndicesField;
        private readonly IIdentifier _defaultValuesField;
        private readonly IIdentifier _targetInfosField;
        private readonly IIdentifier _subscriptionsField;
        private readonly IIdentifier _sourceTypeField;
        private readonly IIdentifier _subscribeModeField;
        private readonly IIdentifier _parentIndicesField;
        private readonly IIdentifier _rootSourceSlotField;

        // Resolved field identifiers for DomTargetInfo
        private readonly IIdentifier _domTargetElemIdxField;
        private readonly IIdentifier _domTargetSetterField;

        // Resolved field identifiers for SubscriptionEntry
        private readonly IIdentifier _subscriptionPropertyNameField;
        private readonly IIdentifier _subscriptionNodeIdxField;
        private readonly IIdentifier _subscriptionSourceSlotField;
        private readonly IIdentifier _subscriptionPathSegmentsField;

        // Resolved field identifiers for GateTargetInfo
        private readonly IIdentifier _gateMarkerIdxField;
        private readonly IIdentifier _gateTrueTemplateField;
        private readonly IIdentifier _gateFalseTemplateField;
        private readonly IIdentifier _gateTrueElemCountField;
        private readonly IIdentifier _gateFalseElemCountField;
        private readonly IIdentifier _gateTrueChildElemIndicesField;
        private readonly IIdentifier _gateFalseChildElemIndicesField;

        // Resolved field identifiers for CollectionTargetInfo
        private readonly IIdentifier _collectionMarkerIdxField;
        private readonly IIdentifier _collectionItemGraphField;
        private readonly IIdentifier _collectionItemTemplateField;
        private readonly IIdentifier _collectionSubControlInfosField;

        // Resolved field identifiers for SubControlInfo
        private readonly IIdentifier _subControlMarkerIdxField;
        private readonly IIdentifier _subControlTypeFactoryField;
        private readonly IIdentifier _subControlSkinFactoryField;

        // Resolved field identifiers for EventTargetInfo
        private readonly IIdentifier _eventElemIdxField;
        private readonly IIdentifier _eventNameField;

        // LIMIT-006: Resolved field identifiers for SubControlInfo/SubControlPropertyInfo
        private readonly IIdentifier _subControlsField;
        private readonly IIdentifier _subControlElemIdxField;
        private readonly IIdentifier _subControlBindingsField;
        private readonly IIdentifier _subControlPropNodeIdxField;
        private readonly IIdentifier _subControlPropSetterField;

        // Factory identifiers for sub-types (used to emit proper typed instances)
        private readonly IIdentifier _domTargetInfoFactory;
        private readonly IIdentifier _subscriptionEntryFactory;
        private readonly IIdentifier _gateTargetInfoFactory;
        private readonly IIdentifier _collectionTargetInfoFactory;
        private readonly IIdentifier _eventTargetInfoFactory;
        private readonly IIdentifier _subControlInfoFactory;
        private readonly IIdentifier _subControlPropertyInfoFactory;

        public GraphDescriptorJSTEmitter(
            GraphTopology topology,
            IdentifierScope scope,
            RuntimeScopeManager scopeManager,
            RazorKnownTypes knownTypes,
            ISet<string> knownFunctionNames,
            ClrContext clrContext,
            string modelTypeName,
            Dictionary<string, IList<IIdentifier>> resolvedTypeIdentifiers = null,
            string parentModelTypeName = null,
            RazorCssManager cssManager = null,
            Location fallbackLocation = null)
        {
            _topology = topology;
            _scope = scope;
            _scopeManager = scopeManager;
            _knownTypes = knownTypes;
            _knownFunctionNames = knownFunctionNames;
            _clrContext = clrContext;
            _modelTypeName = modelTypeName;
            _parentModelTypeName = parentModelTypeName;
            _resolvedTypeIdentifiers = resolvedTypeIdentifiers;
            _typeHelper = new CecilTypeHelper(clrContext);
            _cssManager = cssManager;
            _fallbackLocation = fallbackLocation;

            // Resolve all field identifiers at construction time
            ResolveFieldIdentifiers(
                out _nodeCountField, out _nodeTypesField, out _gettersField,
                out _consumersField, out _gateIndicesField, out _defaultValuesField,
                out _targetInfosField, out _subscriptionsField, out _sourceTypeField,
                out _subscribeModeField, out _parentIndicesField, out _rootSourceSlotField,
                out _domTargetElemIdxField, out _domTargetSetterField,
                out _subscriptionPropertyNameField, out _subscriptionNodeIdxField,
                out _subscriptionSourceSlotField, out _subscriptionPathSegmentsField,
                out _gateMarkerIdxField, out _gateTrueTemplateField, out _gateFalseTemplateField,
                out _gateTrueElemCountField, out _gateFalseElemCountField,
                out _gateTrueChildElemIndicesField, out _gateFalseChildElemIndicesField,
                out _collectionMarkerIdxField, out _collectionItemGraphField,
                out _collectionItemTemplateField, out _collectionSubControlInfosField,
                out _subControlMarkerIdxField, out _subControlTypeFactoryField,
                out _subControlSkinFactoryField,
                out _eventElemIdxField, out _eventNameField);

            // LIMIT-006: Resolve sub-control field identifiers
            ResolveSubControlFieldIdentifiers(
                out _subControlsField, out _subControlElemIdxField, out _subControlBindingsField,
                out _subControlPropNodeIdxField, out _subControlPropSetterField);

            // Resolve factory identifiers for sub-types so we can emit proper typed instances
            ResolveFactoryIdentifiers(
                out _domTargetInfoFactory, out _subscriptionEntryFactory,
                out _gateTargetInfoFactory, out _collectionTargetInfoFactory,
                out _eventTargetInfoFactory,
                out _subControlInfoFactory, out _subControlPropertyInfoFactory);
        }

        /// <summary>
        /// Resolves all field identifiers for GraphDescriptor and its sub-types via Cecil.
        /// Each field is looked up on the appropriate TypeDefinition, then resolved through
        /// the RuntimeScopeManager so identifiers participate in minification.
        /// </summary>
        private void ResolveFieldIdentifiers(
            out IIdentifier nodeCount, out IIdentifier nodeTypes, out IIdentifier getters,
            out IIdentifier consumers, out IIdentifier gateIndices, out IIdentifier defaultValues,
            out IIdentifier targetInfos, out IIdentifier subscriptions, out IIdentifier sourceType,
            out IIdentifier subscribeMode, out IIdentifier parentIndices, out IIdentifier rootSourceSlot,
            out IIdentifier domElemIdx, out IIdentifier domSetter,
            out IIdentifier subPropertyName, out IIdentifier subNodeIdx, out IIdentifier subSourceSlot,
            out IIdentifier subPathSegments,
            out IIdentifier gateMarkerIdx, out IIdentifier gateTrueTemplate, out IIdentifier gateFalseTemplate,
            out IIdentifier gateTrueElemCount, out IIdentifier gateFalseElemCount,
            out IIdentifier gateTrueChildElemIndices, out IIdentifier gateFalseChildElemIndices,
            out IIdentifier collMarkerIdx, out IIdentifier collItemGraph, out IIdentifier collItemTemplate,
            out IIdentifier collSubControlInfos,
            out IIdentifier scMarkerIdx, out IIdentifier scTypeFactory, out IIdentifier scSkinFactory,
            out IIdentifier eventElemIdx, out IIdentifier eventName)
        {
            // GraphDescriptor fields
            var graphDescType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.GraphDescriptor");
            nodeCount = ResolveFieldId(graphDescType, "NodeCount");
            nodeTypes = ResolveFieldId(graphDescType, "NodeTypes");
            getters = ResolveFieldId(graphDescType, "Getters");
            consumers = ResolveFieldId(graphDescType, "Consumers");
            gateIndices = ResolveFieldId(graphDescType, "GateIndices");
            defaultValues = ResolveFieldId(graphDescType, "DefaultValues");
            targetInfos = ResolveFieldId(graphDescType, "TargetInfos");
            subscriptions = ResolveFieldId(graphDescType, "Subscriptions");
            sourceType = ResolveFieldId(graphDescType, "SourceType");
            subscribeMode = ResolveFieldId(graphDescType, "SubscribeMode");
            parentIndices = ResolveFieldId(graphDescType, "ParentIndices");
            rootSourceSlot = ResolveFieldId(graphDescType, "RootSourceSlot");

            // DomTargetInfo fields
            var domTargetType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.DomTargetInfo");
            domElemIdx = ResolveFieldId(domTargetType, "ElemIdx");
            domSetter = ResolveFieldId(domTargetType, "Setter");

            // SubscriptionEntry fields
            var subEntryType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.SubscriptionEntry");
            subPropertyName = ResolveFieldId(subEntryType, "PropertyName");
            subNodeIdx = ResolveFieldId(subEntryType, "NodeIdx");
            subSourceSlot = ResolveFieldId(subEntryType, "SourceSlot");
            subPathSegments = ResolveFieldId(subEntryType, "PathSegments");

            // GateTargetInfo fields
            var gateType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.GateTargetInfo");
            gateMarkerIdx = ResolveFieldId(gateType, "MarkerIdx");
            gateTrueTemplate = ResolveFieldId(gateType, "TrueTemplate");
            gateFalseTemplate = ResolveFieldId(gateType, "FalseTemplate");
            gateTrueElemCount = ResolveFieldId(gateType, "TrueElemCount");
            gateFalseElemCount = ResolveFieldId(gateType, "FalseElemCount");
            gateTrueChildElemIndices = ResolveFieldId(gateType, "TrueChildElemIndices");
            gateFalseChildElemIndices = ResolveFieldId(gateType, "FalseChildElemIndices");

            // CollectionTargetInfo fields
            var collType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.CollectionTargetInfo");
            collMarkerIdx = ResolveFieldId(collType, "MarkerIdx");
            collItemGraph = ResolveFieldId(collType, "ItemGraph");
            collItemTemplate = ResolveFieldId(collType, "ItemTemplate");
            collSubControlInfos = ResolveFieldId(collType, "SubControlInfos");

            // SubControlInfo fields
            var scType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.SubControlInfo");
            scMarkerIdx = ResolveFieldId(scType, "MarkerIdx");
            scTypeFactory = ResolveFieldId(scType, "TypeFactory");
            scSkinFactory = ResolveFieldId(scType, "SkinFactory");

            // EventTargetInfo fields
            var eventType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.EventTargetInfo");
            eventElemIdx = ResolveFieldId(eventType, "ElemIdx");
            eventName = ResolveFieldId(eventType, "EventName");
        }

        /// <summary>
        /// LIMIT-006: Resolves field identifiers for SubControlInfo and SubControlPropertyInfo.
        /// </summary>
        private void ResolveSubControlFieldIdentifiers(
            out IIdentifier subControlsField, out IIdentifier scElemIdx, out IIdentifier scBindings,
            out IIdentifier scpNodeIdx, out IIdentifier scpSetter)
        {
            var graphDescType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.GraphDescriptor");
            subControlsField = ResolveFieldId(graphDescType, "SubControls");

            var subControlType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.SubControlInfo");
            scElemIdx = ResolveFieldId(subControlType, "ElemIdx");
            scBindings = ResolveFieldId(subControlType, "Bindings");

            var subControlPropType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.SubControlPropertyInfo");
            scpNodeIdx = ResolveFieldId(subControlPropType, "NodeIdx");
            scpSetter = ResolveFieldId(subControlPropType, "Setter");
        }

        /// <summary>
        /// Resolves factory (constructor) identifiers for sub-types so that emitted objects
        /// are proper NScript typed instances instead of plain object literals.
        /// The runtime casts these with Type__CastType_d which requires type metadata.
        /// </summary>
        private void ResolveFactoryIdentifiers(
            out IIdentifier domTargetInfoFactory, out IIdentifier subscriptionEntryFactory,
            out IIdentifier gateTargetInfoFactory, out IIdentifier collectionTargetInfoFactory,
            out IIdentifier eventTargetInfoFactory,
            out IIdentifier subControlInfoFactory, out IIdentifier subControlPropertyInfoFactory)
        {
            domTargetInfoFactory = ResolveFactoryForType("Sunlight.Framework.UI.Helpers.BindingGraph.DomTargetInfo");
            subscriptionEntryFactory = ResolveFactoryForType("Sunlight.Framework.UI.Helpers.BindingGraph.SubscriptionEntry");
            gateTargetInfoFactory = ResolveFactoryForType("Sunlight.Framework.UI.Helpers.BindingGraph.GateTargetInfo");
            collectionTargetInfoFactory = ResolveFactoryForType("Sunlight.Framework.UI.Helpers.BindingGraph.CollectionTargetInfo");
            eventTargetInfoFactory = ResolveFactoryForType("Sunlight.Framework.UI.Helpers.BindingGraph.EventTargetInfo");
            subControlInfoFactory = ResolveFactoryForType("Sunlight.Framework.UI.Helpers.BindingGraph.SubControlInfo");
            subControlPropertyInfoFactory = ResolveFactoryForType("Sunlight.Framework.UI.Helpers.BindingGraph.SubControlPropertyInfo");
        }

        /// <summary>
        /// Resolves the type constructor for creating instances via 'new Type()'.
        /// Uses ResolveType which returns the type's JS constructor identifier.
        /// </summary>
        private IIdentifier ResolveFactoryForType(string fullTypeName)
        {
            var typeDef = FindTypeDefinition(fullTypeName);
            if (typeDef == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot find type {TypeName} for constructor resolution", fullTypeName);
                return null;
            }

            var identifiers = _scopeManager.ResolveType(typeDef);
            if (identifiers == null || identifiers.Count == 0)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve type {TypeName}", fullTypeName);
                return null;
            }

            // For simple (non-generic) types, the first identifier IS the constructor
            return identifiers[0];
        }

        /// <summary>
        /// Resolves a field on a type definition to an IIdentifier via the scope manager.
        /// Returns null if the type or field cannot be found (will fall back to string keys).
        /// </summary>
        private IIdentifier ResolveFieldId(TypeDefinition type, string fieldName)
        {
            if (type == null) return null;

            var fieldDef = type.Fields.FirstOrDefault(f => f.Name == fieldName);
            if (fieldDef == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot find field {FieldName} on {TypeName}",
                    fieldName, type.FullName);
                return null;
            }

            return _scopeManager.Resolve(fieldDef);
        }

        /// <summary>
        /// Emits the complete graph descriptor as an InlineObjectInitializer JST node.
        /// Fields: nodeTypes, getters, consumers, gateIndices, defaultValues,
        /// targetInfos, subscriptions, subscribeMode, nodeCount, parentIndices.
        /// All field names use resolved IIdentifiers for correct minification.
        /// </summary>
        public InlineObjectInitializer Emit()
        {
            var obj = new InlineObjectInitializer(null, _scope);

            AddField(obj, _nodeTypesField, "nodeTypes", EmitNodeTypes());
            AddField(obj, _gettersField, "getters", EmitGetters());
            AddField(obj, _consumersField, "consumers", EmitConsumers());
            AddField(obj, _gateIndicesField, "gateIndices", EmitGateIndices());
            AddField(obj, _defaultValuesField, "defaultValues", EmitDefaultValues());
            AddField(obj, _targetInfosField, "targetInfos", EmitTargetInfos());
            AddField(obj, _subscriptionsField, "subscriptions", EmitSubscriptions());
            AddField(obj, _subscribeModeField, "subscribeMode", new NumberLiteralExpression(_scope, 0));
            AddField(obj, _nodeCountField, "nodeCount", new NumberLiteralExpression(_scope, _topology.NodeCount));

            // Skip SourceType for item graphs — DataContext is a tuple, not the model type.
            if (!string.IsNullOrEmpty(_topology.ModelTypeName) && !IsItemGraph)
                AddField(obj, _sourceTypeField, "sourceType", EmitSourceType(_topology.ModelTypeName));

            AddField(obj, _parentIndicesField, "parentIndices", EmitParentIndices());
            AddField(obj, _rootSourceSlotField, "rootSourceSlot", new NumberLiteralExpression(_scope, 0));

            // LIMIT-006: Emit sub-control entries if any exist
            if (_topology.SubControls.Count > 0)
                AddField(obj, _subControlsField, "subControls", EmitSubControls());

            return obj;
        }

        /// <summary>
        /// Adds a field to an InlineObjectInitializer using the resolved identifier.
        /// Logs a warning if resolution failed — string keys break minification.
        /// </summary>
        private void AddField(InlineObjectInitializer obj, IIdentifier resolvedId, string fallbackName, Expression value)
        {
            if (resolvedId != null)
            {
                obj.AddInitializer(resolvedId, value);
            }
            else
            {
                Log.Warning("GraphDescriptorJSTEmitter: Field '{FieldName}' not resolved — using string key (WILL BREAK in retail/minified builds)", fallbackName);
                obj.AddInitializer(fallbackName, value);
            }
        }

        /// <summary>
        /// Emits a resolved type expression for the sourceType field.
        /// Uses the resolved type identifiers dictionary to find the minified type reference,
        /// falling back to a string literal if resolution is not available.
        /// </summary>
        private Expression EmitSourceType(string modelTypeName)
        {
            if (_resolvedTypeIdentifiers != null && !string.IsNullOrEmpty(modelTypeName))
            {
                var mangledName = modelTypeName.Replace(".", "__");
                if (_resolvedTypeIdentifiers.TryGetValue(mangledName, out var identifiers)
                    && identifiers.Count > 0)
                {
                    return IdentifierExpression.Create(null, _scope, identifiers);
                }
            }

            // Fallback: emit null — a string would crash at runtime when
            // GraphEngine calls desc.SourceType.IsInstanceOfType().
            Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve sourceType {TypeName} — emitting null (type check disabled)", modelTypeName);
            return new NullLiteralExpression(_scope);
        }

        /// <summary>nodeTypes: [0, 1, 3, ...]</summary>
        private Expression EmitNodeTypes()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
                items.Add(new NumberLiteralExpression(_scope, _topology.NodeTypes[i]));

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// getters: [null, function(dc) { return dc.get_name(); }, null, ...]
        /// Uses ScriptLiteralExpression for getter bodies since getter
        /// expressions reference virtual method accessors that are already correctly mangled.
        /// </summary>
        private Expression EmitGetters()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
                items.Add(EmitGetter(_topology.NodeTypes[i], _topology.GetterExpressions[i]));

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitGetter(int nodeType, string getterExpression)
        {
            switch (nodeType)
            {
                case GraphNodeTypeConstants.Source:
                case GraphNodeTypeConstants.DomTarget:
                    return new NullLiteralExpression(_scope);

                case GraphNodeTypeConstants.EventBinding:
                {
                    // EventBinding needs a getter to extract the method reference from the source.
                    if (string.IsNullOrEmpty(getterExpression))
                        return new NullLiteralExpression(_scope);

                    return EmitEventGetter(getterExpression);
                }

                case GraphNodeTypeConstants.Property:
                {
                    if (string.IsNullOrEmpty(getterExpression))
                        return new NullLiteralExpression(_scope);

                    // Try building a fully resolved JST getter via Cecil type lookup
                    var resolved = TryBuildResolvedPropertyGetter(getterExpression);
                    if (resolved != null)
                        return resolved;

                    // Fallback: known function names stay as-is, others get getter prefix.
                    var fallbackExpr = getterExpression;
                    bool fallbackIsModel = fallbackExpr.StartsWith("Model.");
                    if (fallbackIsModel)
                        fallbackExpr = fallbackExpr.Substring(6);
                    if (!string.IsNullOrEmpty(_topology.ItemVariablePrefix)
                        && fallbackExpr.StartsWith(_topology.ItemVariablePrefix))
                        fallbackExpr = fallbackExpr.Substring(_topology.ItemVariablePrefix.Length);

                    // For item graphs: dc[2] for item props, dc[0] for Model props
                    string dcRef = IsItemGraph ? (fallbackIsModel ? "dc[0]" : "dc[2]") : "dc";
                    string body;
                    if (_knownFunctionNames != null && _knownFunctionNames.Contains(fallbackExpr))
                        body = "return " + dcRef + "." + fallbackExpr;
                    else
                    {
                        var getterName = ExpressionJsEmitter.PropertyToGetterName(fallbackExpr);
                        body = "return " + dcRef + "." + getterName + "()";
                    }

                    return CreateRawGetterFunction(body);
                }

                case GraphNodeTypeConstants.Gate:
                    // Gate nodes use their parent Property node's value directly as the
                    // condition. No getter needed — the engine passes through parentVal.
                    return new NullLiteralExpression(_scope);

                case GraphNodeTypeConstants.Computed:
                case GraphNodeTypeConstants.CollectionManager:
                {
                    if (string.IsNullOrEmpty(getterExpression))
                        return new NullLiteralExpression(_scope);

                    // For simple single-property expressions, try resolved field access first.
                    // This handles inlined getters where get_X() doesn't exist at runtime.
                    var resolved = TryBuildResolvedPropertyGetter(getterExpression);
                    if (resolved != null)
                        return resolved;

                    // Try building a proper JST expression tree for arithmetic expressions.
                    // This ensures field names are resolved through the scope system, avoiding
                    // issues with raw body strings that can't access the final minified names.
                    var jstExpr = TryBuildComputedJSTExpression(getterExpression);
                    if (jstExpr != null)
                        return jstExpr;

                    // Try building a proper JST expression tree for ternary expressions.
                    // Ternary expressions like "item.IsComplete ? \"done\" : \"pending\""
                    // need resolved field identifiers to produce correct minified names.
                    var ternaryExpr = TryBuildTernaryJSTExpression(getterExpression);
                    if (ternaryExpr != null)
                        return ternaryExpr;

                    // Final fallback: use raw body with field-access replacement
                    var jsExpr = ToJsGetterWithFieldAccess(
                        getterExpression, "dc", "tp", _knownFunctionNames);
                    return CreateRawGetterFunction("return " + jsExpr);
                }

                default:
                    return new NullLiteralExpression(_scope);
            }
        }

        /// <summary>
        /// Builds a fully resolved JST getter function for a simple property access.
        /// The getter expression is the property name (e.g., "PropStr1") which is looked
        /// up on the model type via Cecil. The getter method is resolved through the scope
        /// manager so all identifiers participate in minification.
        /// Returns: function(dc) { return dc.get_propStr1(); } with all identifiers resolved.
        /// Returns null if the property cannot be resolved (falls back to raw string).
        /// </summary>
        private Expression TryBuildResolvedPropertyGetter(string propertyName)
        {
            if (_clrContext == null || string.IsNullOrEmpty(_modelTypeName))
                return null;

            // Check for negation prefix (from gate conditions like "!IsCollapsed")
            bool isNegated = false;
            if (propertyName.StartsWith("!"))
            {
                isNegated = true;
                propertyName = propertyName.Substring(1);
            }

            // Strip "Model." prefix — in Razor templates, Model IS the DataContext.
            // OneTime bindings pass the full CSharpExpression (e.g., "Model.AppVersion").
            if (propertyName.StartsWith("Model."))
                propertyName = propertyName.Substring(6);

            // Strip item variable prefix for foreach item templates (e.g., "item.Name" -> "Name")
            if (!string.IsNullOrEmpty(_topology.ItemVariablePrefix)
                && propertyName.StartsWith(_topology.ItemVariablePrefix))
                propertyName = propertyName.Substring(_topology.ItemVariablePrefix.Length);

            // Don't handle dotted paths beyond Model. (e.g., "Customer.Address")
            if (propertyName.Contains("."))
                return null;

            // Find the model type
            var typeDefinition = FindTypeDefinition(_modelTypeName);
            if (typeDefinition == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve type {TypeName} for getter", _modelTypeName);
                return null;
            }

            // Find the property on the type
            var property = FindProperty(typeDefinition, propertyName);
            if (property?.GetMethod == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot find property getter {PropName} on {TypeName}",
                    propertyName, typeDefinition.FullName);
                return null;
            }

            // Create a scope with "dc" parameter (no enforceSuggestion — let minification work)
            var getterScope = new IdentifierScope(_scope, new[] { "dc" }, false);
            var paramIdentifier = getterScope.ParameterIdentifiers[0];

            // NScript inlines simple field-return getters — the getter method won't exist at
            // runtime. Detect this and emit field access: dc.fieldName instead of dc.get_propName().
            // IMPORTANT: Find the backing field on the SAME TypeDefinition from _clrContext
            // (not via IL resolution) to ensure the scope manager returns the correct identifier.
            var backingField = TryFindBackingFieldOnType(typeDefinition, property);
            // For item graphs, access the item element of the tuple: dc[2]
            var dcAccess = CreateTupleAccessExpression(paramIdentifier, getterScope);
            Expression currentExpr;
            if (backingField != null)
            {
                // Simple getter inlined by NScript — use field access: dc.fieldName (or dc[2].fieldName)
                var fieldId = _scopeManager.Resolve(backingField);
                currentExpr = new IndexExpression(
                    null,
                    getterScope,
                    dcAccess,
                    new IdentifierExpression(fieldId, getterScope));
            }
            else
            {
                // Complex getter — use method call: dc.get_propName() (or dc[2].get_propName())
                var getterMethodId = _scopeManager.Resolve(property.GetMethod);
                currentExpr = new MethodCallExpression(
                    null,
                    getterScope,
                    new IndexExpression(
                        null,
                        getterScope,
                        dcAccess,
                        new IdentifierExpression(getterMethodId, getterScope)),
                    System.Array.Empty<Expression>());
            }

            // Apply negation for gate conditions like "!IsCollapsed"
            if (isNegated)
            {
                currentExpr = new UnaryExpression(
                    null, getterScope, UnaryOperator.LogicalNot, currentExpr);
            }

            // Wrap in: function(dc) { return <expr>; }
            var fn = new FunctionExpression(_fallbackLocation, _scope, getterScope, getterScope.ParameterIdentifiers, null);
            fn.AddStatement(new ReturnStatement(_fallbackLocation, getterScope, currentExpr));
            return fn;
        }

        /// <summary>
        /// Finds the backing field for a property by analyzing the getter's IL to get the
        /// field name, then looking it up on the SAME TypeDefinition from _clrContext.
        /// This is critical: we must use the same TypeDefinition that the scope manager
        /// processed, otherwise Resolve() creates a new (wrong) identifier.
        /// Returns null if the getter is not a simple field-return.
        /// </summary>
        private static FieldDefinition TryFindBackingFieldOnType(TypeDefinition type, PropertyDefinition property)
        {
            var getter = property.GetMethod;
            if (getter?.Body == null)
                return null;

            // Analyze IL to find the field name referenced by the getter
            string fieldName = null;
            var instructions = getter.Body.Instructions;
            bool hasLdarg0 = false;

            foreach (var instr in instructions)
            {
                var op = instr.OpCode;
                if (op == OpCodes.Nop || op == OpCodes.Stloc_0 || op == OpCodes.Ldloc_0
                    || op == OpCodes.Br_S || op == OpCodes.Ret)
                    continue;

                if (op == OpCodes.Ldarg_0)
                {
                    hasLdarg0 = true;
                    continue;
                }

                if (op == OpCodes.Ldfld && hasLdarg0 && fieldName == null)
                {
                    fieldName = (instr.Operand as FieldReference)?.Name;
                    continue;
                }

                // Any other instruction means this isn't a simple field getter
                return null;
            }

            if (fieldName == null)
                return null;

            // Find the field by NAME on the type definition from _clrContext
            // Walk up the hierarchy in case the field is declared on a base type
            var current = type;
            while (current != null)
            {
                var field = current.Fields.FirstOrDefault(f => f.Name == fieldName);
                if (field != null) return field;
                try { current = current.BaseType?.Resolve(); }
                catch (Mono.Cecil.AssemblyResolutionException) { break; }
                catch (System.Exception) { break; } // Cecil resolution — external assembly not loaded
            }

            return null;
        }

        /// <summary>
        /// Fallback: creates a getter function with a raw JS body string.
        /// Used for computed/gate/collection getters whose bodies contain operators
        /// and complex expressions that can't be resolved through Cecil.
        /// Uses enforceSuggestion=true so "dc" stays as-is since the raw body references it literally.
        /// </summary>
        private ScriptLiteralExpression CreateRawGetterFunction(string rawBody)
        {
            // Emit the entire function as a raw script literal to avoid the JST
            // TransformerVisitor replacing it with an empty FunctionExpression.
            // Parameter "dc" is the data-context array, referenced literally in rawBody.
            return new ScriptLiteralExpression(null, _scope, $"function(dc){{{rawBody};}}");
        }

        /// <summary>
        /// Creates a JST expression to access a DataContext element.
        /// For root graphs: returns the dc parameter directly.
        /// For item graphs: returns dc[tupleIndex] — tuple layout: [0]=parentDC, [1]=control, [2]=item.
        /// </summary>
        private Expression CreateTupleAccessExpression(
            IIdentifier dcParam, IdentifierScope scope, int tupleIndex = 2)
        {
            Expression dcExpr = new IdentifierExpression(dcParam, scope);
            if (IsItemGraph)
            {
                dcExpr = new IndexExpression(null, scope, dcExpr,
                    new NumberLiteralExpression(scope, tupleIndex));
            }
            return dcExpr;
        }

        /// <summary>
        /// Like ExpressionJsEmitter.ToJsGetter, but uses backing-field access instead of
        /// getter method calls. This handles inlined getters where get_X() doesn't exist.
        /// Falls back to ExpressionJsEmitter.ToJsGetter if field resolution fails.
        /// </summary>
        private string ToJsGetterWithFieldAccess(
            string csharpExpression,
            string dataContextParam,
            string templateParentParam,
            ISet<string> knownFunctionNames)
        {
            // Build property-to-field name map for the model type
            var fieldMap = BuildPropertyFieldNameMap();

            if (fieldMap == null || fieldMap.Count == 0)
            {
                return ExpressionJsEmitter.ToJsGetter(
                    csharpExpression, dataContextParam, templateParentParam, knownFunctionNames);
            }

            // Replace "Model.", "Control.", and the item variable prefix (e.g. "folder.")
            // with the appropriate function parameter names.
            // For item graphs (tuple DataContext): Model. → dc[0]., itemVar. → dc[2].
            // For root graphs: Model. → dc.
            string modelRef = IsItemGraph ? dataContextParam + "[0]." : dataContextParam + ".";
            string itemRef = IsItemGraph ? dataContextParam + "[2]." : dataContextParam + ".";
            var expr = csharpExpression
                .Replace("Model.", modelRef)
                .Replace("Control.", templateParentParam + ".");

            // For foreach item templates, the item variable prefix (e.g. "folder.") must
            // be replaced with the appropriate dc reference.
            if (!string.IsNullOrEmpty(_topology?.ItemVariablePrefix))
                expr = expr.Replace(_topology.ItemVariablePrefix, itemRef);

            // Build method name map for resolving method calls
            var methodMap = BuildMethodNameMap();

            // Convert property accesses to field accesses using the map.
            // Also handle method calls: .MethodName() should use resolved method name.
            expr = System.Text.RegularExpressions.Regex.Replace(expr, @"\.([A-Z])(\w*)(\(\))?",
                match =>
                {
                    var propName = match.Groups[1].Value + match.Groups[2].Value;
                    var hasParens = match.Groups[3].Success; // matched "()"

                    if (knownFunctionNames != null && knownFunctionNames.Contains(propName))
                        return "." + propName + (hasParens ? "()" : "");

                    // If followed by (), it's a method call — use resolved method name
                    if (hasParens && methodMap != null && methodMap.TryGetValue(propName, out var resolvedMethodName))
                        return "." + resolvedMethodName + "()";

                    if (fieldMap.TryGetValue(propName, out var fieldName))
                        return "." + fieldName;

                    // Fallback to getter call pattern for properties
                    if (hasParens)
                        return $".{match.Groups[1].Value.ToLower()}{match.Groups[2].Value}()";
                    return $".get_{match.Groups[1].Value.ToLower()}{match.Groups[2].Value}()";
                });

            return expr;
        }

        /// <summary>
        /// Builds a map of C# property name -> JS field name for the model type's
        /// simple field-return properties. Uses enforceSuggestion=true identifiers
        /// since these will be embedded in raw body strings.
        /// </summary>
        private Dictionary<string, string> BuildPropertyFieldNameMap()
        {
            if (_cachedFieldMap != null) return _cachedFieldMap;

            if (_clrContext == null || string.IsNullOrEmpty(_modelTypeName))
                return null;

            var typeDefinition = FindTypeDefinition(_modelTypeName);
            if (typeDefinition == null)
                return null;

            var map = new Dictionary<string, string>();
            foreach (var prop in typeDefinition.Properties)
            {
                if (prop.GetMethod == null) continue;
                var field = TryFindBackingFieldOnType(typeDefinition, prop);
                if (field == null) continue;

                // Get the minified field name. Since we're building raw body text,
                // we need the actual text the identifier produces.
                var fieldId = _scopeManager.Resolve(field);
                if (fieldId is SimpleIdentifier simpleId)
                    map[prop.Name] = simpleId.GetName();
                else if (fieldId != null)
                    map[prop.Name] = fieldId.SuggestedName;
            }

            _cachedFieldMap = map;
            return _cachedFieldMap;
        }

        /// <summary>
        /// Builds a map of C# method name -> JS method name for the model type's public methods.
        /// Uses enforceSuggestion=true identifiers for raw body embedding.
        /// </summary>
        private Dictionary<string, string> BuildMethodNameMap()
        {
            if (_cachedMethodMap != null) return _cachedMethodMap;

            if (_clrContext == null || string.IsNullOrEmpty(_modelTypeName))
                return null;

            var typeDefinition = FindTypeDefinition(_modelTypeName);
            if (typeDefinition == null)
                return null;

            var map = new Dictionary<string, string>();
            foreach (var method in typeDefinition.Methods)
            {
                if (!method.IsPublic || method.IsConstructor || method.IsGetter || method.IsSetter)
                    continue;
                var methodId = _scopeManager.Resolve(method);
                if (methodId is SimpleIdentifier simpleId)
                    map[method.Name] = simpleId.GetName();
                else if (methodId != null)
                    map[method.Name] = methodId.SuggestedName;
            }

            _cachedMethodMap = map;
            return _cachedMethodMap;
        }

        private TypeDefinition FindTypeDefinition(string fullTypeName)
            => _typeHelper.FindTypeDefinition(fullTypeName);

        private PropertyDefinition FindProperty(TypeDefinition type, string propertyName)
            => _typeHelper.FindProperty(type, propertyName);

        /// <summary>consumers: [[1], [2], [], ...]</summary>
        private Expression EmitConsumers()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                var consumerExprs = new List<Expression>();
                foreach (int c in _topology.Consumers[i])
                    consumerExprs.Add(new NumberLiteralExpression(_scope, c));

                items.Add(new InlineNewArrayInitialization(null, _scope, consumerExprs));
            }

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>gateIndices: [-1, -1, 2, ...]</summary>
        private Expression EmitGateIndices()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
                items.Add(new NumberLiteralExpression(_scope, _topology.GateIndices[i]));

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>defaultValues: [null, null, "", false, ...]</summary>
        private Expression EmitDefaultValues()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
                items.Add(EmitDefaultValue(_topology.DefaultValues[i]));

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitDefaultValue(object value)
        {
            if (value == null) return new NullLiteralExpression(_scope);
            if (value is bool b) return new BooleanLiteralExpression(_scope, b);
            if (value is string s) return new StringLiteralExpression(_scope, s);
            if (value is int n) return new NumberLiteralExpression(_scope, n);
            if (value is long l) return new NumberLiteralExpression(_scope, l);
            return new StringLiteralExpression(_scope, value.ToString());
        }

        /// <summary>
        /// targetInfos: [null, null, {elem: 0, set: SetTextContent}, ...]
        /// For setter references, resolves the MethodDefinition to a scope-resolved IIdentifier
        /// via RuntimeScopeManager.ResolveStatic, ensuring proper minification.
        /// </summary>
        private Expression EmitTargetInfos()
        {
            // Build a lookup from NodeIdx to DomTargetTopology
            var domTargetMap = new Dictionary<int, DomTargetTopology>();
            foreach (var dt in _topology.DomTargets)
                domTargetMap[dt.NodeIdx] = dt;

            // Build a lookup from NodeIdx to GateTopology
            var gateMap = new Dictionary<int, GateTopology>();
            foreach (var gt in _topology.Gates)
                gateMap[gt.NodeIdx] = gt;

            // Build a lookup from NodeIdx to CollectionTopology
            var collectionMap = new Dictionary<int, CollectionTopology>();
            foreach (var ct in _topology.Collections)
                collectionMap[ct.NodeIdx] = ct;

            // Build a lookup from NodeIdx to EventTopology
            var eventMap = new Dictionary<int, EventTopology>();
            foreach (var et in _topology.Events)
                eventMap[et.NodeIdx] = et;

            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                if (domTargetMap.TryGetValue(i, out var dt))
                {
                    items.Add(EmitDomTargetInfo(dt));
                }
                else if (gateMap.TryGetValue(i, out var gt))
                {
                    items.Add(EmitGateTargetInfo(gt));
                }
                else if (collectionMap.TryGetValue(i, out var ct))
                {
                    items.Add(EmitCollectionTargetInfo(ct));
                }
                else if (eventMap.TryGetValue(i, out var et))
                {
                    items.Add(EmitEventTargetInfo(et));
                }
                else
                {
                    items.Add(new NullLiteralExpression(_scope));
                }
            }

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// Emits a typed object as an IIFE (Immediately Invoked Function Expression) that
        /// creates a proper NScript typed instance instead of a plain object literal.
        /// The runtime uses Type__CastType_d to check type metadata, so plain {} fails.
        /// Pattern: (function(){var o=new TypeFactory();o.field1=val1;o.field2=val2;return o})()
        /// </summary>
        private Expression EmitTypedObject(IIdentifier factoryId, List<(IIdentifier field, string fallbackName, Expression value)> fields)
        {
            // Create inner scope for the IIFE (no parameters)
            var innerScope = new IdentifierScope(_scope, 0);
            var objVar = SimpleIdentifier.CreateScopeIdentifier(innerScope, "o", false);

            var stmts = new List<Statement>();

            // var o = new TypeFactory();
            var factoryExpr = new IdentifierExpression(factoryId, innerScope);
            var newExpr = new NewObjectExpression(null, innerScope, factoryExpr);
            stmts.Add(ExpressionStatement.CreateAssignmentExpression(
                new IdentifierExpression(objVar, innerScope),
                newExpr));

            // o.field = value;
            foreach (var (fieldId, fallbackName, value) in fields)
            {
                Expression fieldAccess;
                if (fieldId != null)
                {
                    fieldAccess = new IndexExpression(null, innerScope,
                        new IdentifierExpression(objVar, innerScope),
                        new IdentifierExpression(fieldId, innerScope));
                }
                else
                {
                    fieldAccess = new IndexExpression(null, innerScope,
                        new IdentifierExpression(objVar, innerScope),
                        new StringLiteralExpression(innerScope, fallbackName));
                }

                stmts.Add(ExpressionStatement.CreateAssignmentExpression(fieldAccess, value));
            }

            // return o;
            stmts.Add(new ReturnStatement(_fallbackLocation, innerScope,
                new IdentifierExpression(objVar, innerScope)));

            // Build the IIFE: (function() { ... })()
            var fn = new FunctionExpression(_fallbackLocation, _scope, innerScope,
                innerScope.ParameterIdentifiers, null);
            fn.AddStatements(stmts);

            return new MethodCallExpression(_fallbackLocation, _scope, fn);
        }

        /// <summary>
        /// Emits a DomTarget targetInfo as a proper DomTargetInfo instance via IIFE.
        /// The setter is resolved via RuntimeScopeManager.ResolveStatic for minification.
        /// Field names use resolved IIdentifiers from DomTargetInfo type.
        /// The attribute name is baked into the setter function itself (SetAttribute is called
        /// with the attribute name), so no separate field is needed.
        /// </summary>
        private Expression EmitDomTargetInfo(DomTargetTopology dt)
        {
            // Build the setter expression based on target type.
            // GraphEngine calls setter(elem, value) — 2 params. But SetCssClass and SetAttribute
            // expect 3 params. We emit inline wrapper functions for these cases.
            Expression setterExpr;
            switch (dt.Target)
            {
                case ExpressionTarget.CssClass:
                    // Emit: function(e, v) { e.className = v || ""; }
                    setterExpr = CreateRawSetterFunction("e.className = v || \"\"");
                    break;

                case ExpressionTarget.Attribute:
                {
                    var attrName = EscapeJsString(dt.AttributeName ?? "");
                    // "value" attribute must use the DOM property (e.value), not setAttribute.
                    // setAttribute("value", x) only updates the HTML attribute, not the displayed
                    // input value after user interaction. Browsers render .value, not the attribute.
                    if (string.Equals(dt.AttributeName, "value", System.StringComparison.OrdinalIgnoreCase))
                    {
                        setterExpr = CreateRawSetterFunction("e.value = v || \"\"");
                    }
                    else
                    {
                        // Emit: function(e, v) { if (v != null) e.setAttribute("attrName", v); else e.removeAttribute("attrName"); }
                        setterExpr = CreateRawSetterFunction(
                            $"if (v != null) e.setAttribute(\"{attrName}\", v); else e.removeAttribute(\"{attrName}\")");
                    }
                    break;
                }

                case ExpressionTarget.Style:
                {
                    // Use setAttribute("style", ...) instead of style.cssText to avoid
                    // browser normalization issues. Include the static prefix if present.
                    var stylePrefix = EscapeJsString(dt.AttributePrefix ?? "");
                    if (!string.IsNullOrEmpty(stylePrefix))
                        setterExpr = CreateRawSetterFunction($"e.setAttribute(\"style\", \"{stylePrefix}\" + (v || \"\"))");
                    else
                        setterExpr = CreateRawSetterFunction("e.setAttribute(\"style\", v || \"\")");
                    break;
                }

                default:
                {
                    // TextContent: use SetTextContent directly — it has the right (elem, value) signature
                    var setterMethod = _knownTypes.GetSetterMethod(dt.Target);
                    var setterId = _scopeManager.ResolveStatic(setterMethod);
                    setterExpr = new IdentifierExpression(setterId, _scope);
                    break;
                }
            }

            if (_domTargetInfoFactory != null)
            {
                var fields = new List<(IIdentifier, string, Expression)>
                {
                    (_domTargetElemIdxField, "ElemIdx", new NumberLiteralExpression(_scope, dt.ElemIdx)),
                    (_domTargetSetterField, "Setter", setterExpr)
                };
                return EmitTypedObject(_domTargetInfoFactory, fields);
            }

            // Fallback: plain object literal if factory resolution failed
            var info = new InlineObjectInitializer(null, _scope);
            AddField(info, _domTargetElemIdxField, "ElemIdx", new NumberLiteralExpression(_scope, dt.ElemIdx));
            AddField(info, _domTargetSetterField, "Setter", setterExpr);
            return info;
        }

        /// <summary>
        /// Tries to build a proper JST expression tree for a computed expression like
        /// "Model.Price * Model.Quantity". Uses resolved field identifiers so the output
        /// participates in NScript's minification system.
        /// Returns function(dc) { return dc.price_I * dc.quantity_J; } with resolved identifiers.
        /// Returns null if the expression cannot be parsed.
        /// </summary>
        private Expression TryBuildComputedJSTExpression(string expression)
        {
            if (_clrContext == null || string.IsNullOrEmpty(_modelTypeName))
                return null;

            // Tokenize the expression: split on arithmetic operators while preserving them
            var tokens = System.Text.RegularExpressions.Regex.Split(
                expression.Trim(), @"(\s*[+\-*/]\s*)");

            if (tokens.Length < 3) return null; // Need at least operand operator operand

            var getterScope = new IdentifierScope(_scope, new[] { "dc" }, false);
            var paramIdentifier = getterScope.ParameterIdentifiers[0];

            Expression result = null;
            BinaryOperator? pendingOp = null;

            foreach (var token in tokens)
            {
                var t = token.Trim();
                if (string.IsNullOrEmpty(t)) continue;

                // Check if it's an operator
                if (t == "*" || t == "/" || t == "+" || t == "-")
                {
                    pendingOp = t == "*" ? BinaryOperator.Mul
                        : t == "/" ? BinaryOperator.Div
                        : t == "+" ? BinaryOperator.Plus
                        : BinaryOperator.Minus;
                    continue;
                }

                // It's a property reference — resolve it
                Expression operand = TryResolvePropertyToFieldAccess(t, getterScope, paramIdentifier);
                if (operand == null) return null; // Can't resolve — bail

                if (result == null)
                {
                    result = operand;
                }
                else if (pendingOp.HasValue)
                {
                    result = new BinaryExpression(null, getterScope, pendingOp.Value, result, operand);
                    pendingOp = null;
                }
                else
                {
                    return null; // Unexpected token
                }
            }

            if (result == null) return null;

            // Wrap in: function(dc) { return <expr>; }
            var fn = new FunctionExpression(_fallbackLocation, _scope, getterScope, getterScope.ParameterIdentifiers, null);
            fn.AddStatement(new ReturnStatement(_fallbackLocation, getterScope, result));
            return fn;
        }

        /// <summary>
        /// Tries to build a proper JST expression tree for a ternary expression like
        /// "Model.IsActive ? \"yes\" : \"no\"" or "item.IsComplete ? \"done\" : \"pending\"".
        /// Uses resolved field identifiers so the output participates in NScript's minification system.
        /// Returns function(dc) { return dc.isActive_I ? "yes" : "no"; } with resolved identifiers.
        /// Returns null if the expression cannot be parsed as a ternary.
        /// </summary>
        private Expression TryBuildTernaryJSTExpression(string expression)
        {
            if (_clrContext == null || string.IsNullOrEmpty(_modelTypeName))
                return null;

            // Match pattern: <propertyExpr> ? <trueExpr> : <falseExpr>
            // The condition must be a simple property access (possibly negated).
            // True/false branches can be string literals or property accesses.
            // NOTE: Nested ternaries (e.g., "A ? B ? c : d : e") are NOT supported —
            // the non-greedy capture will mis-split them. They fall through to raw emission.
            var match = System.Text.RegularExpressions.Regex.Match(
                expression.Trim(),
                @"^(!?\s*(?:Model\.|" +
                System.Text.RegularExpressions.Regex.Escape(_topology?.ItemVariablePrefix ?? "NOMATCH") +
                @")?\s*[A-Z]\w*)\s*\?\s*(.*?)\s*:\s*(.*?)\s*$");

            if (!match.Success) return null;

            var conditionPart = match.Groups[1].Value.Trim();
            var truePart = match.Groups[2].Value.Trim();
            var falsePart = match.Groups[3].Value.Trim();

            var getterScope = new IdentifierScope(_scope, new[] { "dc" }, false);
            var paramIdentifier = getterScope.ParameterIdentifiers[0];

            // Build condition expression (property field access, possibly negated)
            bool isNegated = conditionPart.StartsWith("!");
            if (isNegated)
                conditionPart = conditionPart.Substring(1).Trim();

            Expression conditionExpr = TryResolvePropertyToFieldAccess(conditionPart, getterScope, paramIdentifier);
            if (conditionExpr == null) return null;

            if (isNegated)
                conditionExpr = new UnaryExpression(null, getterScope, UnaryOperator.LogicalNot, conditionExpr);

            // Build true/false branch expressions
            Expression trueExpr = TryParseLiteralOrProperty(truePart, getterScope, paramIdentifier);
            if (trueExpr == null) return null;

            Expression falseExpr = TryParseLiteralOrProperty(falsePart, getterScope, paramIdentifier);
            if (falseExpr == null) return null;

            // Build: condition ? trueExpr : falseExpr
            var ternary = new ConditionalOperatorExpression(null, getterScope, conditionExpr, trueExpr, falseExpr);

            // Wrap in: function(dc) { return <ternary>; }
            var fn = new FunctionExpression(_fallbackLocation, _scope, getterScope, getterScope.ParameterIdentifiers, null);
            fn.AddStatement(new ReturnStatement(_fallbackLocation, getterScope, ternary));
            return fn;
        }

        /// <summary>
        /// Parses a ternary branch value as either a string literal or a property field access.
        /// String literals are enclosed in double quotes: "someValue"
        /// Property accesses are Model.Prop or item.Prop references.
        /// </summary>
        private Expression TryParseLiteralOrProperty(
            string value, IdentifierScope scope, IIdentifier dcParam)
        {
            if (string.IsNullOrEmpty(value)) return null;

            // String literal: "value"
            if (value.StartsWith("\"") && value.EndsWith("\"") && value.Length >= 2)
            {
                var unquoted = value.Substring(1, value.Length - 2);
                return new StringLiteralExpression(scope, unquoted);
            }

            // Boolean literals
            if (value == "true")
                return new BooleanLiteralExpression(scope, true);
            if (value == "false")
                return new BooleanLiteralExpression(scope, false);

            // Numeric literals
            if (int.TryParse(value, out var intVal))
                return new NumberLiteralExpression(scope, intVal);

            // null
            if (value == "null")
                return new NullLiteralExpression(scope);

            // Property access
            return TryResolvePropertyToFieldAccess(value, scope, dcParam);
        }

        /// <summary>
        /// Resolves a property expression like "Model.Price" to a field access JST expression.
        /// Returns: dc.price_I (using the resolved field identifier).
        /// </summary>
        private Expression TryResolvePropertyToFieldAccess(
            string expression, IdentifierScope scope, IIdentifier dcParam)
        {
            var propName = expression;
            if (propName.StartsWith("Model.")) propName = propName.Substring(6);
            if (!string.IsNullOrEmpty(_topology.ItemVariablePrefix)
                && propName.StartsWith(_topology.ItemVariablePrefix))
                propName = propName.Substring(_topology.ItemVariablePrefix.Length);
            if (propName.Contains(".")) return null;

            var typeDefinition = FindTypeDefinition(_modelTypeName);
            if (typeDefinition == null) return null;

            var property = FindProperty(typeDefinition, propName);
            if (property == null) return null;

            var backingField = TryFindBackingFieldOnType(typeDefinition, property);
            // For item graphs, access the item element of the tuple: dc[2]
            var dcAccess = CreateTupleAccessExpression(dcParam, scope);
            if (backingField != null)
            {
                var fieldId = _scopeManager.Resolve(backingField);
                return new IndexExpression(null, scope,
                    dcAccess,
                    new IdentifierExpression(fieldId, scope));
            }

            // Try getter method
            if (property.GetMethod != null)
            {
                var getterId = _scopeManager.Resolve(property.GetMethod);
                return new MethodCallExpression(null, scope,
                    new IndexExpression(null, scope,
                        dcAccess,
                        new IdentifierExpression(getterId, scope)),
                    System.Array.Empty<Expression>());
            }

            return null;
        }

        /// <summary>
        /// Emits a getter function for an EventBinding node.
        /// The handler expression is like "Model.IncrementClick" or a lambda "(e) => Model.IncrementClick()".
        /// For item graphs, uses tuple DataContext: dc[2] for item methods, dc[0] for Model methods.
        /// </summary>
        private Expression EmitEventGetter(string handlerExpression)
        {
            if (string.IsNullOrEmpty(handlerExpression))
                return new NullLiteralExpression(_scope);

            // Track whether this is a Model-level method reference (for tuple index selection)
            bool isModelMethodRef = handlerExpression.StartsWith("Model.");

            // Strip Model. or item variable prefix (e.g., "folder.", "todo.")
            var expr = handlerExpression;
            if (expr.StartsWith("Model."))
                expr = expr.Substring(6);
            if (!string.IsNullOrEmpty(_topology?.ItemVariablePrefix)
                && expr.StartsWith(_topology.ItemVariablePrefix))
                expr = expr.Substring(_topology.ItemVariablePrefix.Length);

            // For simple method references (no parens, no lambda)
            if (expr.IndexOfAny(new[] { '(', ')', '=', '>' }) < 0)
            {
                // Resolve method — for Model methods in item graphs, look up on parent type
                var resolveTypeName = (isModelMethodRef && IsItemGraph && !string.IsNullOrEmpty(_parentModelTypeName))
                    ? _parentModelTypeName : null;
                var methodId = TryResolveMethodIdentifier(expr, out bool isDevirtualized, resolveTypeName);
                if (methodId != null)
                {
                    var outerScope = new IdentifierScope(_scope, new[] { "dc" }, false);
                    var dcParam = outerScope.ParameterIdentifiers[0];
                    var innerScope = new IdentifierScope(outerScope, new[] { "e", "ev" }, false);

                    int tupleIdx = isModelMethodRef ? 0 : 2;
                    var dcRef = CreateTupleAccessExpression(dcParam, innerScope, tupleIdx);
                    var eParam = new IdentifierExpression(innerScope.ParameterIdentifiers[0], innerScope);
                    var evParam = new IdentifierExpression(innerScope.ParameterIdentifiers[1], innerScope);

                    MethodCallExpression methodCall;
                    if (isDevirtualized)
                    {
                        // Devirtualized: method(instance, e, ev)
                        var methodRef = new IdentifierExpression(methodId, innerScope);
                        methodCall = new MethodCallExpression(null, innerScope, methodRef, dcRef, eParam, evParam);
                    }
                    else
                    {
                        // Virtual/instance: instance.method(e, ev)
                        var methodAccess = new IndexExpression(null, innerScope, dcRef,
                            new IdentifierExpression(methodId, innerScope));
                        methodCall = new MethodCallExpression(null, innerScope, methodAccess, eParam, evParam);
                    }

                    var innerFn = new FunctionExpression(_fallbackLocation, outerScope, innerScope,
                        innerScope.ParameterIdentifiers, null);
                    innerFn.AddStatement(new ExpressionStatement(_fallbackLocation, innerScope, methodCall));

                    var outerFn = new FunctionExpression(_fallbackLocation, _scope, outerScope,
                        outerScope.ParameterIdentifiers, null);
                    outerFn.AddStatement(new ReturnStatement(_fallbackLocation, outerScope, innerFn));
                    return outerFn;
                }

                // Fallback: raw body (unresolved name — may not match minification)
                var methodName = char.ToLower(expr[0]) + expr.Substring(1);
                var rawPrefix = IsItemGraph ? (isModelMethodRef ? "dc[0]" : "dc[2]") : "dc";
                return CreateRawGetterFunction(
                    "return function(e,ev){" + rawPrefix + "." + methodName + "()}");
            }

            // Parent-context method invocation inside a foreach item template:
            // Pattern: "Model.Method(itemVar)" → function(dc) { return function(e, ev) { dc[0].method(dc[2], e, ev); }; }
            var parentMethodResult = TryEmitParentMethodInvocation(handlerExpression);
            if (parentMethodResult != null)
                return parentMethodResult;

            // Lambda expression: try to extract the method call and build proper JST.
            var lambdaMethodName = TryExtractLambdaMethodName(handlerExpression);
            if (lambdaMethodName != null)
            {
                // For lambdas in item graphs, resolve on parent type (lambdas reference Model methods)
                var resolveTypeName = (IsItemGraph && !string.IsNullOrEmpty(_parentModelTypeName))
                    ? _parentModelTypeName : null;
                var lambdaMethodId = TryResolveMethodIdentifier(lambdaMethodName, out bool lambdaIsDevirt, resolveTypeName);
                if (lambdaMethodId != null)
                {
                    var outerScope = new IdentifierScope(_scope, new[] { "dc" }, false);
                    var dcParam = outerScope.ParameterIdentifiers[0];
                    var innerScope = new IdentifierScope(outerScope, new[] { "e", "ev" }, false);

                    // Lambdas reference Model methods → tuple index 0 for item graphs
                    var dcRef = CreateTupleAccessExpression(dcParam, innerScope, 0);

                    MethodCallExpression methodCall;
                    if (lambdaIsDevirt)
                    {
                        var methodRef = new IdentifierExpression(lambdaMethodId, innerScope);
                        methodCall = new MethodCallExpression(null, innerScope, methodRef, dcRef);
                    }
                    else
                    {
                        var methodAccess = new IndexExpression(null, innerScope, dcRef,
                            new IdentifierExpression(lambdaMethodId, innerScope));
                        methodCall = new MethodCallExpression(null, innerScope, methodAccess);
                    }

                    var innerFn = new FunctionExpression(_fallbackLocation, outerScope, innerScope,
                        innerScope.ParameterIdentifiers, null);
                    innerFn.AddStatement(new ExpressionStatement(_fallbackLocation, innerScope, methodCall));

                    var outerFn = new FunctionExpression(_fallbackLocation, _scope, outerScope,
                        outerScope.ParameterIdentifiers, null);
                    outerFn.AddStatement(new ReturnStatement(_fallbackLocation, outerScope, innerFn));
                    return outerFn;
                }
            }

            // Fallback: raw body with field access replacement (for complex expressions)
            var jsExpr = ToJsGetterWithFieldAccess(
                handlerExpression, "dc", "tp", _knownFunctionNames);
            return CreateRawGetterFunction("return " + jsExpr);
        }

        /// <summary>
        /// Extracts a simple method name from a lambda event handler expression.
        /// E.g., "(e) => Model.IncrementClick()" returns "IncrementClick".
        /// Returns null for complex lambdas.
        /// </summary>
        private static string TryExtractLambdaMethodName(string handlerExpression)
        {
            // Pattern: (params) => Model.MethodName()
            var arrowIdx = handlerExpression.IndexOf("=>");
            if (arrowIdx < 0) return null;

            var body = handlerExpression.Substring(arrowIdx + 2).Trim();

            // Strip "Model." prefix
            if (body.StartsWith("Model."))
                body = body.Substring(6);

            // Check for simple method call: MethodName()
            if (body.EndsWith("()"))
            {
                var name = body.Substring(0, body.Length - 2).Trim();
                if (name.Length > 0 && !name.Contains(".") && !name.Contains("("))
                    return name;
            }

            return null;
        }

        /// <summary>
        /// Resolves a method identifier through the scope system.
        /// Returns the IIdentifier that tracks minification, or null if not found.
        /// </summary>
        private IIdentifier TryResolveMethodIdentifier(string handlerExpression, string typeNameOverride = null)
        {
            return TryResolveMethodIdentifier(handlerExpression, out _, typeNameOverride);
        }

        private IIdentifier TryResolveMethodIdentifier(string handlerExpression, out bool isDevirtualized, string typeNameOverride = null)
        {
            isDevirtualized = false;
            var typeName = typeNameOverride ?? _modelTypeName;
            if (_clrContext == null || string.IsNullOrEmpty(typeName))
                return null;

            var methodName = handlerExpression;
            if (methodName.StartsWith("Model."))
                methodName = methodName.Substring(6);
            // Strip item variable prefix for foreach item templates
            if (!string.IsNullOrEmpty(_topology?.ItemVariablePrefix)
                && methodName.StartsWith(_topology.ItemVariablePrefix))
                methodName = methodName.Substring(_topology.ItemVariablePrefix.Length);

            var typeDefinition = FindTypeDefinition(typeName);
            if (typeDefinition == null)
                return null;

            foreach (var method in typeDefinition.Methods)
            {
                if (method.Name == methodName && method.IsPublic && !method.IsConstructor)
                {
                    isDevirtualized = IsMethodDevirtualized(method);
                    if (isDevirtualized)
                        return _scopeManager.ResolveStatic(method);
                    return _scopeManager.Resolve(method);
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether a method is devirtualized to a free static function
        /// (called as method(instance, args)) rather than an instance method on the
        /// prototype (called as instance.method(args)). Mirrors the logic in
        /// MethodCallExpressionConverter.IsMethodInstanceCall.
        /// </summary>
        private bool IsMethodDevirtualized(MethodDefinition method)
        {
            if (!method.HasThis)
                return false; // Already static

            bool isVirtualCall = method.IsVirtual && !method.IsFinal;
            if (isVirtualCall)
                return false; // Virtual methods stay on prototype

            var declaringType = method.DeclaringType;
            if (declaringType.HasGenericParameters || declaringType.IsGenericInstance)
                return false; // Generic types keep instance methods

            if (declaringType.IsInterface)
                return false;

            if (!_scopeManager.ImplementInstanceAsStatic)
                return false;

            return true;
        }

        /// <summary>
        /// Tries to emit a parent-context method invocation for event handlers inside foreach item templates.
        /// Handles the pattern: "Model.MethodName(itemVar)" where the method lives on the parent ViewModel
        /// and the argument is the loop variable (the item itself).
        /// 
        /// Generated JS: function(dc) { return function(e, ev) { dc[0].method(dc[2], e, ev); }; }
        /// where dc[0] = parent DataContext (tuple element 0), dc[2] = loop item (tuple element 2).
        /// </summary>
        private Expression TryEmitParentMethodInvocation(string handlerExpression)
        {
            // Only applies inside foreach item templates with known parent type
            if (!IsItemGraph || string.IsNullOrEmpty(_parentModelTypeName) || _clrContext == null)
                return null;

            // Parse "Model.MethodName(argName)" pattern
            var expr = handlerExpression;
            if (!expr.StartsWith("Model."))
                return null;
            expr = expr.Substring(6); // strip "Model."

            var parenOpen = expr.IndexOf('(');
            var parenClose = expr.LastIndexOf(')');
            if (parenOpen < 1 || parenClose <= parenOpen)
                return null;

            var methodName = expr.Substring(0, parenOpen);
            var argName = expr.Substring(parenOpen + 1, parenClose - parenOpen - 1).Trim();

            // Verify the argument matches the loop variable
            var itemVarName = _topology.ItemVariablePrefix.TrimEnd('.');
            if (argName != itemVarName)
                return null;

            // Look up the method on the parent type
            var parentTypeDef = FindTypeDefinition(_parentModelTypeName);
            if (parentTypeDef == null)
                return null;

            MethodDefinition targetMethod = null;
            foreach (var m in parentTypeDef.Methods)
            {
                if (m.Name == methodName && m.IsPublic && !m.IsConstructor)
                {
                    targetMethod = m;
                    break;
                }
            }
            if (targetMethod == null)
                return null;

            bool isDevirt = IsMethodDevirtualized(targetMethod);
            var methodId = isDevirt
                ? _scopeManager.ResolveStatic(targetMethod)
                : _scopeManager.Resolve(targetMethod);
            if (methodId == null)
                return null;

            // Build: function(dc) { return function(e, ev) { method(dc[0], dc[2], e, ev); }; }  (devirtualized)
            //   or:  function(dc) { return function(e, ev) { dc[0].method(dc[2], e, ev); }; }  (virtual)
            var outerScope = new IdentifierScope(_scope, new[] { "dc" }, false);
            var dcParam = outerScope.ParameterIdentifiers[0];
            var innerScope = new IdentifierScope(outerScope, new[] { "e", "ev" }, false);

            var parentAccess = CreateTupleAccessExpression(dcParam, innerScope, 0);
            var itemAccess = CreateTupleAccessExpression(dcParam, innerScope, 2);
            var eParam = new IdentifierExpression(innerScope.ParameterIdentifiers[0], innerScope);
            var evParam = new IdentifierExpression(innerScope.ParameterIdentifiers[1], innerScope);

            MethodCallExpression methodCall;
            if (isDevirt)
            {
                // Devirtualized: method(dc[0], dc[2], e, ev)
                var methodRef = new IdentifierExpression(methodId, innerScope);
                methodCall = new MethodCallExpression(null, innerScope, methodRef, parentAccess, itemAccess, eParam, evParam);
            }
            else
            {
                // Virtual: dc[0].method(dc[2], e, ev)
                var methodAccess = new IndexExpression(null, innerScope, parentAccess,
                    new IdentifierExpression(methodId, innerScope));
                methodCall = new MethodCallExpression(null, innerScope, methodAccess, itemAccess, eParam, evParam);
            }

            var innerFn = new FunctionExpression(_fallbackLocation, outerScope, innerScope,
                innerScope.ParameterIdentifiers, null);
            innerFn.AddStatement(new ExpressionStatement(_fallbackLocation, innerScope, methodCall));

            var outerFn = new FunctionExpression(_fallbackLocation, _scope, outerScope,
                outerScope.ParameterIdentifiers, null);
            outerFn.AddStatement(new ReturnStatement(_fallbackLocation, outerScope, innerFn));
            return outerFn;
        }

        /// <summary>
        /// Creates a raw setter function: function(e, v) { body }
        /// </summary>
        private ScriptLiteralExpression CreateRawSetterFunction(string rawBody)
        {
            // Emit the entire function as a raw script literal to avoid the JST
            // TransformerVisitor replacing it with an empty FunctionExpression.
            // Parameters "e" (element) and "v" (value) are referenced literally in rawBody.
            return new ScriptLiteralExpression(null, _scope, $"function(e,v){{{rawBody};}}");
        }

        /// <summary>
        /// Resolves the element type of a collection property by analyzing the property's
        /// return type's generic arguments. E.g., ObservableCollection&lt;RazorItemVM&gt; → "RazorItemVM".
        /// </summary>
        private string ResolveCollectionItemTypeName(CollectionTopology ct)
        {
            if (_clrContext == null || string.IsNullOrEmpty(_modelTypeName))
                return null;

            // The collection expression is like "Model.Items"
            var collExpr = ct.IrNode.CollectionExpression ?? "";
            if (collExpr.StartsWith("Model."))
                collExpr = collExpr.Substring(6);
            if (collExpr.Contains("."))
                return null; // Don't handle chained paths

            var modelType = FindTypeDefinition(_modelTypeName);
            if (modelType == null) return null;

            var property = FindProperty(modelType, collExpr);
            if (property == null) return null;

            // Get the generic instance type (e.g., ObservableCollection<RazorItemVM>)
            var returnType = property.PropertyType as Mono.Cecil.GenericInstanceType;
            if (returnType != null && returnType.GenericArguments.Count > 0)
            {
                var itemType = returnType.GenericArguments[0];
                return itemType.FullName;
            }

            return null;
        }

        private static string EscapeJsString(string s)
            => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

        /// <summary>
        /// Emits a Gate targetInfo as a proper GateTargetInfo instance via IIFE.
        /// HTML content is computed from the IR node's branches.
        /// </summary>
        private Expression EmitGateTargetInfo(GateTopology gt)
        {
            var trueHtml = RazorSkinCodeGenerator.CollectHtmlPublic(gt.IrNode.TrueBranch);
            var falseHtml = (gt.IrNode.FalseBranch != null && gt.IrNode.FalseBranch.Count > 0)
                ? RazorSkinCodeGenerator.CollectHtmlPublic(gt.IrNode.FalseBranch)
                : "";

            if (_gateTargetInfoFactory != null)
            {
                var fields = new List<(IIdentifier, string, Expression)>
                {
                    (_gateMarkerIdxField, "MarkerIdx", new NumberLiteralExpression(_scope, gt.MarkerIdx)),
                    (_gateTrueTemplateField, "TrueTemplate", new StringLiteralExpression(_scope, trueHtml)),
                    (_gateFalseTemplateField, "FalseTemplate", new StringLiteralExpression(_scope, falseHtml))
                };
                if (gt.TrueChildElemIndices != null && gt.TrueChildElemIndices.Length > 0)
                    fields.Add((_gateTrueChildElemIndicesField, "TrueChildElemIndices", EmitIntArray(gt.TrueChildElemIndices)));
                if (gt.FalseChildElemIndices != null && gt.FalseChildElemIndices.Length > 0)
                    fields.Add((_gateFalseChildElemIndicesField, "FalseChildElemIndices", EmitIntArray(gt.FalseChildElemIndices)));
                return EmitTypedObject(_gateTargetInfoFactory, fields);
            }

            // Fallback: plain object literal if factory resolution failed
            var info = new InlineObjectInitializer(null, _scope);
            AddField(info, _gateMarkerIdxField, "MarkerIdx", new NumberLiteralExpression(_scope, gt.MarkerIdx));
            AddField(info, _gateTrueTemplateField, "TrueTemplate", new StringLiteralExpression(_scope, trueHtml));
            AddField(info, _gateFalseTemplateField, "FalseTemplate", new StringLiteralExpression(_scope, falseHtml));
            if (gt.TrueChildElemIndices != null && gt.TrueChildElemIndices.Length > 0)
                AddField(info, _gateTrueChildElemIndicesField, "TrueChildElemIndices", EmitIntArray(gt.TrueChildElemIndices));
            if (gt.FalseChildElemIndices != null && gt.FalseChildElemIndices.Length > 0)
                AddField(info, _gateFalseChildElemIndicesField, "FalseChildElemIndices", EmitIntArray(gt.FalseChildElemIndices));
            return info;
        }

        /// <summary>
        /// Emits a literal int[] as an inline array expression: [1, 2, 3].
        /// </summary>
        private Expression EmitIntArray(int[] values)
        {
            var elements = new List<Expression>();
            foreach (var v in values)
                elements.Add(new NumberLiteralExpression(_scope, v));
            return new InlineNewArrayInitialization(null, _scope, elements);
        }

        /// <summary>
        /// Emits a Collection targetInfo as a proper CollectionTargetInfo instance via IIFE.
        /// If the collection has an item topology, it is recursively emitted.
        /// </summary>
        private Expression EmitCollectionTargetInfo(CollectionTopology ct)
        {
            // Use CollectItemTemplateHtmlPublic to preserve data-evt-idx markers
            // for runtime event element resolution in item graphs.
            var itemHtml = (ct.IrNode.ItemTemplate != null && ct.IrNode.ItemTemplate.Count > 0)
                ? RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(ct.IrNode.ItemTemplate)
                : "";

            // Replace CSS class names in item template HTML (same as main template)
            if (!string.IsNullOrEmpty(itemHtml))
            {
                itemHtml = RazorCssManager.ReplaceCssClassNamesInHtml(itemHtml, _cssManager);
            }

            Expression itemGraphExpr = null;
            if (ct.ItemTopology != null)
            {
                // Resolve the item type from the collection property's generic argument.
                // E.g., for ObservableCollection<RazorItemVM>, the item type is RazorItemVM.
                string itemTypeName = ResolveCollectionItemTypeName(ct);

                var nestedEmitter = new GraphDescriptorJSTEmitter(
                    ct.ItemTopology, _scope, _scopeManager, _knownTypes, _knownFunctionNames,
                    _clrContext, itemTypeName ?? _modelTypeName,
                    _resolvedTypeIdentifiers,
                    parentModelTypeName: _modelTypeName,
                    cssManager: _cssManager,
                    fallbackLocation: ct.IrNode?.Location ?? _fallbackLocation);
                itemGraphExpr = nestedEmitter.Emit();
            }

            if (_collectionTargetInfoFactory != null)
            {
                var fields = new List<(IIdentifier, string, Expression)>
                {
                    (_collectionMarkerIdxField, "MarkerIdx", new NumberLiteralExpression(_scope, ct.MarkerIdx)),
                    (_collectionItemTemplateField, "ItemTemplate", new StringLiteralExpression(_scope, itemHtml))
                };
                if (itemGraphExpr != null)
                    fields.Add((_collectionItemGraphField, "ItemGraph", itemGraphExpr));

                // Emit SubControlInfos for sub-controls inside the collection item template
                var subControlInfosExpr = EmitCollectionSubControlInfos(ct);
                if (subControlInfosExpr != null)
                    fields.Add((_collectionSubControlInfosField, "SubControlInfos", subControlInfosExpr));

                return EmitTypedObject(_collectionTargetInfoFactory, fields);
            }

            // Fallback: plain object literal if factory resolution failed
            var info = new InlineObjectInitializer(null, _scope);
            AddField(info, _collectionMarkerIdxField, "MarkerIdx", new NumberLiteralExpression(_scope, ct.MarkerIdx));
            AddField(info, _collectionItemTemplateField, "ItemTemplate", new StringLiteralExpression(_scope, itemHtml));
            if (itemGraphExpr != null)
                AddField(info, _collectionItemGraphField, "ItemGraph", itemGraphExpr);

            // Emit SubControlInfos for fallback path too
            var fallbackSubControlInfos = EmitCollectionSubControlInfos(ct);
            if (fallbackSubControlInfos != null)
                AddField(info, _collectionSubControlInfosField, "SubControlInfos", fallbackSubControlInfos);

            return info;
        }

        /// <summary>
        /// Emits SubControlInfos array for sub-controls inside a collection item template.
        /// Each SubControlInfo has MarkerIdx, TypeFactory, and SkinFactory so the GraphEngine
        /// can instantiate sub-controls when rendering collection items.
        /// Returns null if no sub-controls exist in the item topology.
        /// </summary>
        private Expression EmitCollectionSubControlInfos(CollectionTopology ct)
        {
            if (ct.ItemTopology?.SubControls == null || ct.ItemTopology.SubControls.Count == 0)
                return null;

            if (_clrContext == null)
                return null;

            var items = new List<Expression>();
            int markerIdx = 0;

            foreach (var sc in ct.ItemTopology.SubControls)
            {
                var typeName = sc.ResolvedTypeName ?? sc.ControlTypeName;
                if (string.IsNullOrEmpty(typeName))
                {
                    markerIdx++;
                    continue;
                }

                var typeDef = FindSubControlType(typeName);
                if (typeDef == null)
                {
                    Log.Debug("EmitCollectionSubControlInfos: Cannot find type {TypeName}", typeName);
                    markerIdx++;
                    continue;
                }

                var typeFactoryExpr = BuildSubControlTypeFactory(typeDef);
                var skinFactoryExpr = BuildSubControlSkinFactory(typeDef);

                if (typeFactoryExpr == null || skinFactoryExpr == null)
                {
                    Log.Debug("EmitCollectionSubControlInfos: Cannot build factories for {TypeName}", typeName);
                    markerIdx++;
                    continue;
                }

                if (_subControlInfoFactory != null)
                {
                    var fields = new List<(IIdentifier, string, Expression)>
                    {
                        (_subControlMarkerIdxField, "MarkerIdx", new NumberLiteralExpression(_scope, markerIdx)),
                        (_subControlTypeFactoryField, "TypeFactory", typeFactoryExpr),
                        (_subControlSkinFactoryField, "SkinFactory", skinFactoryExpr)
                    };
                    items.Add(EmitTypedObject(_subControlInfoFactory, fields));
                }
                else
                {
                    var scObj = new InlineObjectInitializer(null, _scope);
                    AddField(scObj, _subControlMarkerIdxField, "MarkerIdx",
                        new NumberLiteralExpression(_scope, markerIdx));
                    AddField(scObj, _subControlTypeFactoryField, "TypeFactory", typeFactoryExpr);
                    AddField(scObj, _subControlSkinFactoryField, "SkinFactory", skinFactoryExpr);
                    items.Add(scObj);
                }

                markerIdx++;
            }

            if (items.Count == 0)
                return null;

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// Builds a TypeFactory function expression for a sub-control type:
        /// function(elem) { return ControlType_factory(elem); }
        /// The factory creates a new control instance given a DOM element.
        /// </summary>
        private Expression BuildSubControlTypeFactory(TypeDefinition typeDef)
        {
            // Find the constructor that takes an Element parameter
            var ctor = typeDef.Methods.FirstOrDefault(m =>
                m.IsConstructor && !m.IsStatic && m.Parameters.Count == 1);

            if (ctor == null)
            {
                Log.Debug("BuildSubControlTypeFactory: No single-param constructor on {TypeName}", typeDef.FullName);
                return null;
            }

            var factoryId = _scopeManager.ResolveFactory(ctor.Resolve());

            // Build: function(elem) { return factory(elem); }
            var innerScope = new IdentifierScope(_scope, new[] { "elem" }, false);
            var elemParam = innerScope.ParameterIdentifiers[0];

            var callExpr = new MethodCallExpression(
                null, innerScope,
                new IdentifierExpression(factoryId, innerScope),
                new Expression[] { new IdentifierExpression(elemParam, innerScope) });

            var fn = new FunctionExpression(_fallbackLocation, _scope, innerScope, innerScope.ParameterIdentifiers, null);
            fn.AddStatement(new ReturnStatement(_fallbackLocation, innerScope, callExpr));
            return fn;
        }

        /// <summary>
        /// Builds a SkinFactory function expression for a sub-control type:
        /// function() { return ControlType__get_DefaultSkin(); }
        /// Finds the static DefaultSkin property (annotated with [Skin]) and resolves its getter.
        /// </summary>
        private Expression BuildSubControlSkinFactory(TypeDefinition typeDef)
        {
            // Find the static DefaultSkin property (has [Skin] attribute)
            PropertyDefinition skinProp = null;
            foreach (var prop in typeDef.Properties)
            {
                if (!prop.GetMethod?.IsStatic == true) continue;
                if (prop.GetMethod == null || !prop.GetMethod.IsStatic) continue;

                foreach (var attr in prop.CustomAttributes)
                {
                    if (attr.AttributeType.Name == "SkinAttribute")
                    {
                        skinProp = prop;
                        break;
                    }
                }
                if (skinProp != null) break;
            }

            if (skinProp?.GetMethod == null)
            {
                Log.Debug("BuildSubControlSkinFactory: No [Skin] property on {TypeName}", typeDef.FullName);
                return null;
            }

            var getterId = _scopeManager.ResolveStatic(skinProp.GetMethod.Resolve());

            // Build: function() { return get_DefaultSkin(); }
            var innerScope = new IdentifierScope(_scope, 0);

            var callExpr = new MethodCallExpression(
                null, innerScope,
                new IdentifierExpression(getterId, innerScope),
                new Expression[0]);

            var fn = new FunctionExpression(_fallbackLocation, _scope, innerScope, innerScope.ParameterIdentifiers, null);
            fn.AddStatement(new ReturnStatement(_fallbackLocation, innerScope, callExpr));
            return fn;
        }

        /// <summary>
        /// Searches loaded assemblies for a type matching the given name.
        /// Supports both short names ("TodoItemControl") and fully qualified names.
        /// </summary>
        private TypeDefinition FindSubControlType(string typeName)
        {
            // Try fully qualified first
            var result = FindTypeDefinition(typeName);
            if (result != null) return result;

            // Search all types for a match by short name
            foreach (var type in _clrContext.GetTypes())
            {
                if (type.Name == typeName)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// subscriptions: array of proper SubscriptionEntry instances.
        /// Each entry is emitted via IIFE to create a typed instance that passes
        /// the runtime's Type__CastType_d(SubscriptionEntry, ...) check.
        /// </summary>
        private Expression EmitSubscriptions()
        {
            var items = new List<Expression>();
            foreach (var sub in _topology.Subscriptions)
            {
                if (_subscriptionEntryFactory != null)
                {
                    var fields = new List<(IIdentifier, string, Expression)>
                    {
                        (_subscriptionPropertyNameField, "PropertyName", new StringLiteralExpression(_scope, sub.PropertyName)),
                        (_subscriptionNodeIdxField, "NodeIdx", new NumberLiteralExpression(_scope, sub.NodeIdx)),
                        (_subscriptionSourceSlotField, "SourceSlot", new NumberLiteralExpression(_scope, sub.SourceSlot))
                    };

                    // Emit PathSegments array for chained property paths
                    if (sub.PathSegments != null && sub.PathSegments.Length > 1)
                    {
                        var pathArray = new List<Expression>();
                        foreach (var segment in sub.PathSegments)
                        {
                            pathArray.Add(new StringLiteralExpression(_scope, segment));
                        }
                        fields.Add((_subscriptionPathSegmentsField, "PathSegments",
                            new InlineNewArrayInitialization(null, _scope, pathArray)));
                    }

                    items.Add(EmitTypedObject(_subscriptionEntryFactory, fields));
                }
                else
                {
                    // Fallback: plain object literal if factory resolution failed
                    var subObj = new InlineObjectInitializer(null, _scope);
                    AddField(subObj, _subscriptionPropertyNameField, "PropertyName",
                        new StringLiteralExpression(_scope, sub.PropertyName));
                    AddField(subObj, _subscriptionNodeIdxField, "NodeIdx",
                        new NumberLiteralExpression(_scope, sub.NodeIdx));
                    AddField(subObj, _subscriptionSourceSlotField, "SourceSlot",
                        new NumberLiteralExpression(_scope, sub.SourceSlot));

                    // Emit PathSegments array for chained property paths
                    if (sub.PathSegments != null && sub.PathSegments.Length > 1)
                    {
                        var pathArray = new List<Expression>();
                        foreach (var segment in sub.PathSegments)
                        {
                            pathArray.Add(new StringLiteralExpression(_scope, segment));
                        }
                        AddField(subObj, _subscriptionPathSegmentsField, "PathSegments",
                            new InlineNewArrayInitialization(null, _scope, pathArray));
                    }

                    items.Add(subObj);
                }
            }

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// Emits an EventBinding targetInfo as a proper EventTargetInfo instance via IIFE.
        /// </summary>
        private Expression EmitEventTargetInfo(EventTopology et)
        {
            if (_eventTargetInfoFactory != null)
            {
                var fields = new List<(IIdentifier, string, Expression)>
                {
                    (_eventElemIdxField, "ElemIdx", new NumberLiteralExpression(_scope, et.ElemIdx)),
                    (_eventNameField, "EventName", new StringLiteralExpression(_scope, et.EventName))
                };
                return EmitTypedObject(_eventTargetInfoFactory, fields);
            }

            // Fallback: plain object literal if factory resolution failed
            var info = new InlineObjectInitializer(null, _scope);
            AddField(info, _eventElemIdxField, "ElemIdx", new NumberLiteralExpression(_scope, et.ElemIdx));
            AddField(info, _eventNameField, "EventName", new StringLiteralExpression(_scope, et.EventName));
            return info;
        }

        /// <summary>parentIndices: [[], [0], [1], [0, 1], ...]</summary>
        private Expression EmitParentIndices()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                var parentExprs = new List<Expression>();
                foreach (int p in _topology.ParentIndices[i])
                    parentExprs.Add(new NumberLiteralExpression(_scope, p));

                items.Add(new InlineNewArrayInitialization(null, _scope, parentExprs));
            }

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// LIMIT-006: Emits the subControls array for the graph descriptor.
        /// Each entry is a SubControlInfo with ElemIdx and Bindings array.
        /// </summary>
        private Expression EmitSubControls()
        {
            var items = new List<Expression>();
            foreach (var sc in _topology.SubControls)
            {
                // Build bindings array
                var bindingItems = new List<Expression>();
                foreach (var propBinding in sc.PropertyBindings)
                {
                    var setter = BuildSubControlPropertySetter(
                        sc.ResolvedTypeName ?? sc.ControlTypeName,
                        propBinding.TargetPropertyName);

                    if (_subControlPropertyInfoFactory != null)
                    {
                        var fields = new List<(IIdentifier, string, Expression)>
                        {
                            (_subControlPropNodeIdxField, "NodeIdx", new NumberLiteralExpression(_scope, propBinding.NodeIdx)),
                            (_subControlPropSetterField, "Setter", setter ?? new NullLiteralExpression(_scope))
                        };
                        bindingItems.Add(EmitTypedObject(_subControlPropertyInfoFactory, fields));
                    }
                    else
                    {
                        var propObj = new InlineObjectInitializer(null, _scope);
                        AddField(propObj, _subControlPropNodeIdxField, "NodeIdx",
                            new NumberLiteralExpression(_scope, propBinding.NodeIdx));
                        AddField(propObj, _subControlPropSetterField, "Setter",
                            setter ?? new NullLiteralExpression(_scope));
                        bindingItems.Add(propObj);
                    }
                }

                var bindingsExpr = new InlineNewArrayInitialization(null, _scope, bindingItems);

                if (_subControlInfoFactory != null)
                {
                    var fields = new List<(IIdentifier, string, Expression)>
                    {
                        (_subControlElemIdxField, "ElemIdx", new NumberLiteralExpression(_scope, sc.ElemIdx)),
                        (_subControlBindingsField, "Bindings", bindingsExpr)
                    };
                    items.Add(EmitTypedObject(_subControlInfoFactory, fields));
                }
                else
                {
                    var scObj = new InlineObjectInitializer(null, _scope);
                    AddField(scObj, _subControlElemIdxField, "ElemIdx",
                        new NumberLiteralExpression(_scope, sc.ElemIdx));
                    AddField(scObj, _subControlBindingsField, "Bindings", bindingsExpr);
                    items.Add(scObj);
                }
            }

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// LIMIT-006: Builds a setter function for a sub-control property.
        /// Emits: function(ctrl, val) { ctrl.set_PropertyName(val); }
        /// </summary>
        private Expression BuildSubControlPropertySetter(string controlTypeName, string propertyName)
        {
            if (_clrContext == null || string.IsNullOrEmpty(controlTypeName))
                return null;

            var typeDef = FindTypeDefinition(controlTypeName);
            if (typeDef == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve sub-control type {TypeName}", controlTypeName);
                return null;
            }

            var property = FindProperty(typeDef, propertyName);
            if (property?.SetMethod == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot find setter for {PropName} on {TypeName}",
                    propertyName, controlTypeName);
                return null;
            }

            // Build: function(ctrl, val) { ctrl.set_PropertyName(val); }
            var setterScope = new IdentifierScope(_scope, new[] { "ctrl", "val" }, false);
            var ctrlParam = setterScope.ParameterIdentifiers[0];
            var valParam = setterScope.ParameterIdentifiers[1];

            var setterMethodId = _scopeManager.Resolve(property.SetMethod);
            var callExpr = new MethodCallExpression(
                null, setterScope,
                new IndexExpression(
                    null, setterScope,
                    new IdentifierExpression(ctrlParam, setterScope),
                    new IdentifierExpression(setterMethodId, setterScope)),
                new Expression[] { new IdentifierExpression(valParam, setterScope) });

            var fn = new FunctionExpression(_fallbackLocation, _scope, setterScope, setterScope.ParameterIdentifiers, null);
            fn.AddStatement(new ExpressionStatement(_fallbackLocation, setterScope, callExpr));
            return fn;
        }
    }
}
