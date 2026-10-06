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
        private readonly ClrContext _clrContext;
        private readonly string _modelTypeName;
        private readonly string _parentModelTypeName;
        private readonly string _controlTypeName;

        // Numbers the temps behind null-safe mid-path reads (h0, h1, ...) so each is unique within
        // this emitter's getters.
        private int _nullSafeHopTemps;
        private readonly Dictionary<string, IList<IIdentifier>> _resolvedTypeIdentifiers;
        private readonly IEnumerable<string> _usingNamespaces;
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

        /// <summary>
        /// Whether this emitter generates for an item graph (inside a @foreach loop).
        /// Item graphs use tuple DataContext: [parentDC, control, item].
        /// </summary>
        private bool IsItemGraph => !string.IsNullOrEmpty(_topology.ItemVariablePrefix);

        // Resolved field identifiers for GraphDescriptor
        private readonly IIdentifier _nodeCountField;
        private readonly IIdentifier _nodeTypesField;
        private readonly IIdentifier _gettersField;
        private readonly IIdentifier _getterSourceSlotsField;
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
        private readonly IIdentifier _subscriptionChainParentGettersField;

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
        private readonly IIdentifier _subControlHasDataContextBindingField;
        private readonly IIdentifier _subControlPropNodeIdxField;
        private readonly IIdentifier _subControlPropSetterField;
        private readonly IIdentifier _subControlPropTargetNameField;
        private readonly IIdentifier _subControlPropTargetGetterField;
        private readonly IIdentifier _subControlPropSourceSetterField;

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
            ClrContext clrContext,
            string modelTypeName,
            Dictionary<string, IList<IIdentifier>> resolvedTypeIdentifiers = null,
            string parentModelTypeName = null,
            RazorCssManager cssManager = null,
            IEnumerable<string> usingNamespaces = null,
            Location fallbackLocation = null,
            string controlTypeName = null)
        {
            _topology = topology;
            _scope = scope;
            _scopeManager = scopeManager;
            _knownTypes = knownTypes;
            _clrContext = clrContext;
            _modelTypeName = modelTypeName;
            _parentModelTypeName = parentModelTypeName;
            _controlTypeName = controlTypeName;
            _resolvedTypeIdentifiers = resolvedTypeIdentifiers;
            _usingNamespaces = usingNamespaces ?? Enumerable.Empty<string>();
            _typeHelper = new CecilTypeHelper(clrContext);
            _cssManager = cssManager;
            _fallbackLocation = fallbackLocation;
            _getterSourceSlotsField = ResolveFieldId(
                FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.GraphDescriptor"),
                "GetterSourceSlots");
            _subscriptionChainParentGettersField = ResolveFieldId(
                FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.SubscriptionEntry"),
                "ChainParentGetters");

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
                out _collectionItemTemplateField,
                out _subControlMarkerIdxField, out _subControlTypeFactoryField,
                out _subControlSkinFactoryField,
                out _eventElemIdxField, out _eventNameField);

            // LIMIT-006: Resolve sub-control field identifiers
            ResolveSubControlFieldIdentifiers(
                out _subControlsField, out _subControlElemIdxField, out _subControlBindingsField,
                out _subControlHasDataContextBindingField,
                out _subControlPropNodeIdxField, out _subControlPropSetterField,
                out _subControlPropTargetNameField, out _subControlPropTargetGetterField,
                out _subControlPropSourceSetterField);

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
            out IIdentifier scHasDataContextBinding,
            out IIdentifier scpNodeIdx, out IIdentifier scpSetter,
            out IIdentifier scpTargetName, out IIdentifier scpTargetGetter,
            out IIdentifier scpSourceSetter)
        {
            var graphDescType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.GraphDescriptor");
            subControlsField = ResolveFieldId(graphDescType, "SubControls");

            var subControlType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.SubControlInfo");
            scElemIdx = ResolveFieldId(subControlType, "ElemIdx");
            scBindings = ResolveFieldId(subControlType, "Bindings");
            scHasDataContextBinding = ResolveFieldId(subControlType, "HasDataContextBinding");

            var subControlPropType = FindTypeDefinition("Sunlight.Framework.UI.Helpers.BindingGraph.SubControlPropertyInfo");
            scpNodeIdx = ResolveFieldId(subControlPropType, "NodeIdx");
            scpSetter = ResolveFieldId(subControlPropType, "Setter");
            scpTargetName = ResolveFieldId(subControlPropType, "TargetPropertyName");
            scpTargetGetter = ResolveFieldId(subControlPropType, "TargetGetter");
            scpSourceSetter = ResolveFieldId(subControlPropType, "SourceSetter");
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
            AddField(obj, _getterSourceSlotsField, "getterSourceSlots",
                new InlineNewArrayInitialization(null, _scope,
                    _topology.GetterSourceSlots.Select(slot =>
                        (Expression)new NumberLiteralExpression(_scope, slot)).ToList()));
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
        /// getters: [null, function(dc, tp) { return dc.name; }, null, ...]
        /// Getter bodies are JST built from scope-resolved identifiers; an expression that
        /// cannot be resolved fails the build with a diagnostic.
        /// </summary>
        private Expression EmitGetters()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
                items.Add(EmitGetter(_topology.NodeTypes[i], _topology.GetterExpressions[i], i));

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitGetter(int nodeType, string getterExpression, int nodeIndex)
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

                    return EmitEventGetter(getterExpression, nodeIndex);
                }

                case GraphNodeTypeConstants.Property:
                {
                    if (string.IsNullOrEmpty(getterExpression))
                        return new NullLiteralExpression(_scope);

                    // Property nodes may carry the topology's internal "X" / "!X" mini-format
                    // (bare dependency name), which only TryBuildResolvedPropertyGetter knows.
                    // Everything else (bare loop variable, literals, static Type.Member) is C#.
                    return TryBuildResolvedPropertyGetter(getterExpression)
                        ?? BuildBindingExpressionGetter(getterExpression);
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

                    return BuildBindingExpressionGetter(getterExpression);
                }

                default:
                    return new NullLiteralExpression(_scope);
            }
        }

        /// <summary>
        /// Builds <c>function(dc, tp) { return &lt;expr&gt;; }</c> for a C# binding expression.
        /// Every name is resolved through <see cref="ResolveBindingPath"/>; unsupported
        /// forms throw <see cref="RazorSubControlDiagnosticException"/> (build error).
        /// </summary>
        private Expression BuildBindingExpressionGetter(string csharpExpression)
        {
            var getterScope = new IdentifierScope(_scope, new[] { "dc", "tp" }, false);
            var body = BindingExpressionConverter.Convert(
                csharpExpression,
                getterScope,
                segments => ResolveBindingPath(segments, getterScope),
                (receiver, method, arguments) => ResolveInvocation(receiver, method, arguments, getterScope),
                _fallbackLocation);

            var fn = new FunctionExpression(_fallbackLocation, _scope, getterScope,
                getterScope.ParameterIdentifiers, null);
            fn.AddStatement(new ReturnStatement(_fallbackLocation, getterScope, body));
            return fn;
        }

        /// <summary>
        /// Resolves a dotted name path from a binding expression to a JST expression.
        /// Supported shapes (anything else returns null, which the converter reports):
        /// <list type="bullet">
        /// <item>a bare root: <c>Model</c> (dc, or dc[0] in item graphs), <c>Control</c> (tp),
        /// or the loop variable (dc[2]);</item>
        /// <item>an instance property path of any depth on a root: <c>Root.A.B.C</c>, each hop
        /// resolved against the previous hop's declared type. A hop whose type or property cannot
        /// be resolved — for example an [Extended]/[ImportedType] receiver read differently from
        /// a compiled NScript property — fails the build rather than emitting a silent
        /// <c>undefined</c> read;</item>
        /// <item><c>Type.Member</c>: a const/enum field (emitted as a literal) or a static property.</item>
        /// </list>
        /// </summary>
        private Expression ResolveBindingPath(IReadOnlyList<string> segments, IdentifierScope scope)
            => ResolveInstanceValue(segments, segments.Count, scope, out _)
                ?? ResolveStaticMember(segments, scope);

        /// <summary>
        /// Resolves <c>segments[0..count)</c> as an instance value rooted at Model/Control/the loop
        /// variable, walking each property hop against the previous hop's type. Returns the receiver
        /// expression and its static type (<paramref name="finalType"/>), or null when the root is
        /// not a known data root or any hop cannot be resolved. Shared by path reads and by
        /// invocation receivers.
        /// </summary>
        private Expression ResolveInstanceValue(
            IReadOnlyList<string> segments, int count, IdentifierScope scope, out TypeDefinition finalType)
        {
            finalType = null;
            if (_clrContext == null || count == 0)
                return null;

            var (receiver, typeName) = ResolveRoot(segments[0], scope);
            if (receiver == null)
                return null;

            var currentType = FindTypeDefinition(typeName);
            for (int i = 1; i < count; i++)
            {
                var property = currentType != null ? FindProperty(currentType, segments[i]) : null;
                if (property?.GetMethod == null || property.GetMethod.IsStatic)
                {
                    Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve instance property {PropName} on {TypeName}",
                        segments[i], currentType?.FullName ?? typeName);
                    return null;
                }

                receiver = i == 1
                    ? BuildPropertyRead(receiver, currentType, property, scope)
                    : BuildNullSafePropertyRead(receiver, currentType, property, scope);
                // Advance to the property's declared type via the SAME ClrContext instance so the
                // scope manager resolves the next hop's field/getter identifiers correctly.
                currentType = FindTypeDefinition(property.PropertyType.FullName);
            }

            finalType = currentType;
            return receiver;
        }

        /// <summary>
        /// Reads a property off a mid-path receiver that may legitimately be null — an object not
        /// loaded yet, or cleared (<c>Model.Child = null</c>) — as
        /// <c>(h = receiver) == null ? null : h.Prop</c>, so the binding yields null (which DOM
        /// targets replace with their default) instead of throwing and aborting the whole flush.
        /// The temp is a scoped local the getter function declares as <c>var</c>, like the temp
        /// behind <c>??</c>. The first hop needs no guard: the engine never calls a getter on a
        /// null root.
        /// </summary>
        private Expression BuildNullSafePropertyRead(
            Expression receiver, TypeDefinition typeDefinition, PropertyDefinition property,
            IdentifierScope scope)
        {
            var temp = SimpleIdentifier.CreateScopeIdentifier(scope, "h" + _nullSafeHopTemps++, true);
            var assign = new BinaryExpression(null, scope, BinaryOperator.Assignment,
                new IdentifierExpression(temp, scope), receiver);
            var test = new BinaryExpression(null, scope, BinaryOperator.Equals,
                assign, new NullLiteralExpression(scope));
            return new ConditionalOperatorExpression(null, scope, test,
                new NullLiteralExpression(scope),
                BuildPropertyRead(new IdentifierExpression(temp, scope), typeDefinition, property, scope));
        }

        /// <summary>
        /// Maps a binding root name to its receiver expression and declared type name: <c>Model</c>
        /// (the parent model inside an item graph), <c>Control</c>, or the loop variable. Returns
        /// (null, null) for anything else, which the caller treats as a static member.
        /// </summary>
        private (Expression receiver, string typeName) ResolveRoot(string root, IdentifierScope scope)
        {
            var typeName = ResolveRootTypeName(root);
            if (root == "Model")
                return (CreateTupleAccessExpression(scope.ParameterIdentifiers[0], scope, 0), typeName);
            if (root == "Control")
                return (new IdentifierExpression(scope.ParameterIdentifiers[1], scope), typeName);
            if (typeName != null)
                return (CreateTupleAccessExpression(scope.ParameterIdentifiers[0], scope, 2), typeName);

            return (null, null);
        }

        /// <summary>
        /// Declared type name of a binding root: <c>Model</c> (the parent model inside an item
        /// graph), <c>Control</c>, or the loop variable (the item type). Null for anything else.
        /// </summary>
        private string ResolveRootTypeName(string root)
        {
            if (root == "Model") return IsItemGraph ? _parentModelTypeName : _modelTypeName;
            if (root == "Control") return _controlTypeName;

            var itemVariable = IsItemGraph ? _topology.ItemVariablePrefix.TrimEnd('.') : null;
            return itemVariable != null && root == itemVariable ? _modelTypeName : null;
        }

        /// <summary>
        /// Resolves <c>Type.Member</c> (type name may be dotted): a const field becomes a
        /// literal (enum members and [CssClass] consts), a static property becomes a call
        /// to its resolved static getter.
        /// </summary>
        private Expression ResolveStaticMember(IReadOnlyList<string> segments, IdentifierScope scope)
        {
            if (segments.Count < 2)
                return null;

            var typeName = string.Join(".", segments.Take(segments.Count - 1));
            var memberName = segments[segments.Count - 1];
            var type = FindSubControlType(typeName);
            if (type == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve static type {TypeName}", typeName);
                return null;
            }

            var constField = type.Fields.FirstOrDefault(field => field.Name == memberName
                && field.IsLiteral && field.HasConstant);
            if (constField != null)
                return CreateConstantLiteral(constField.Constant, scope);

            var property = FindProperty(type, memberName);
            if (property?.GetMethod == null || !property.GetMethod.IsStatic)
                return null;

            return new MethodCallExpression(null, scope,
                new IdentifierExpression(_scopeManager.ResolveStatic(property.GetMethod), scope),
                System.Array.Empty<Expression>());
        }

        /// <summary>
        /// Resolves an instance method call <c>recv.Method(args)</c>: the receiver resolves to an
        /// instance value, the method is looked up by name and argument count on the receiver's
        /// type, and the call is emitted as a devirtualized static call (ADR-0023) or an instance
        /// call, with all identifiers from the scope manager. Returns null when the receiver or a
        /// matching method cannot be resolved (the converter reports it). Static-type method calls
        /// are not resolved here.
        /// </summary>
        private Expression ResolveInvocation(
            IReadOnlyList<string> receiverSegments, string methodName,
            IReadOnlyList<Expression> arguments, IdentifierScope scope)
        {
            if (_clrContext == null || receiverSegments.Count == 0)
                return null;

            var receiver = ResolveInstanceValue(
                receiverSegments, receiverSegments.Count, scope, out var receiverType);
            if (receiver == null || receiverType == null)
                return null;

            var method = FindInvocableMethod(receiverType, methodName, arguments.Count);
            if (method == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve method {Method}({ArgCount}) on {TypeName}",
                    methodName, arguments.Count, receiverType.FullName);
                return null;
            }

            if (IsMethodDevirtualized(method))
            {
                var callArgs = new List<Expression> { receiver };
                callArgs.AddRange(arguments);
                return new MethodCallExpression(null, scope,
                    new IdentifierExpression(_scopeManager.ResolveStatic(method), scope), callArgs.ToArray());
            }

            return new MethodCallExpression(null, scope,
                new IndexExpression(null, scope, receiver,
                    new IdentifierExpression(_scopeManager.Resolve(method), scope)),
                arguments.ToArray());
        }

        /// <summary>
        /// Finds a public, non-constructor instance method named <paramref name="methodName"/>
        /// taking <paramref name="argumentCount"/> parameters, walking the type hierarchy.
        /// </summary>
        private static MethodDefinition FindInvocableMethod(
            TypeDefinition type, string methodName, int argumentCount)
        {
            for (var currentType = type; currentType != null; currentType = currentType.BaseType?.Resolve())
            {
                var method = currentType.Methods.FirstOrDefault(candidate =>
                    candidate.Name == methodName && candidate.IsPublic && !candidate.IsConstructor
                    && candidate.HasThis && candidate.Parameters.Count == argumentCount);
                if (method != null) return method;
            }
            return null;
        }

        private static Expression CreateConstantLiteral(object value, IdentifierScope scope)
        {
            switch (value)
            {
                case null: return new NullLiteralExpression(scope);
                case string s: return new StringLiteralExpression(scope, s);
                case bool b: return new BooleanLiteralExpression(scope, b);
                case double d: return new DoubleLiteralExpression(scope, d);
                case float f: return new DoubleLiteralExpression(scope, f);
                case sbyte _:
                case byte _:
                case short _:
                case ushort _:
                case int _:
                case uint _:
                case long _:
                    return new NumberLiteralExpression(scope, System.Convert.ToInt64(value));
                default:
                    // char, ulong, decimal: no exact JS literal mapping here.
                    return null;
            }
        }

        /// <summary>
        /// Emits a read of <paramref name="property"/> on <paramref name="receiver"/>:
        /// backing-field access when NScript inlines the getter, a static call when the
        /// getter is devirtualized (ADR-0023), otherwise <c>receiver.get_X()</c>.
        /// All identifiers come from the scope manager.
        /// </summary>
        private Expression BuildPropertyRead(
            Expression receiver, TypeDefinition typeDefinition, PropertyDefinition property,
            IdentifierScope scope)
        {
            // IMPORTANT: Find the backing field on the SAME TypeDefinition from _clrContext
            // (not via IL resolution) to ensure the scope manager returns the correct identifier.
            var backingField = TryFindBackingFieldOnType(typeDefinition, property);
            if (backingField != null)
            {
                return new IndexExpression(null, scope, receiver,
                    new IdentifierExpression(_scopeManager.Resolve(backingField), scope));
            }

            var method = property.GetMethod;
            return IsMethodDevirtualized(method)
                ? new MethodCallExpression(null, scope,
                    new IdentifierExpression(_scopeManager.ResolveStatic(method), scope),
                    new Expression[] { receiver })
                : new MethodCallExpression(null, scope,
                    new IndexExpression(null, scope, receiver,
                        new IdentifierExpression(_scopeManager.Resolve(method), scope)),
                    System.Array.Empty<Expression>());
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
            bool isControl = propertyName.StartsWith("Control.");
            bool isParentModel = IsItemGraph && propertyName.StartsWith("Model.");
            if (isControl)
                propertyName = propertyName.Substring(8);
            else if (propertyName.StartsWith("Model."))
                propertyName = propertyName.Substring(6);

            // Strip item variable prefix for foreach item templates (e.g., "item.Name" -> "Name")
            if (!string.IsNullOrEmpty(_topology.ItemVariablePrefix)
                && propertyName.StartsWith(_topology.ItemVariablePrefix))
                propertyName = propertyName.Substring(_topology.ItemVariablePrefix.Length);

            // Don't handle dotted paths beyond Model. (e.g., "Customer.Address")
            if (propertyName.Contains("."))
                return null;

            // Find the model type
            var typeName = isControl ? _controlTypeName
                : isParentModel ? _parentModelTypeName
                : _modelTypeName;
            var typeDefinition = FindTypeDefinition(typeName);
            if (typeDefinition == null)
            {
                Log.Debug("GraphDescriptorJSTEmitter: Cannot resolve type {TypeName} for getter", typeName);
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
            var getterScope = new IdentifierScope(_scope, new[] { "dc", "tp" }, false);
            var paramIdentifier = getterScope.ParameterIdentifiers[0];

            // For item graphs, access the item element of the tuple: dc[2]
            var dcAccess = isControl
                ? (Expression)new IdentifierExpression(getterScope.ParameterIdentifiers[1], getterScope)
                : CreateTupleAccessExpression(paramIdentifier, getterScope,
                    isParentModel ? 0 : 2);
            var currentExpr = BuildPropertyRead(dcAccess, typeDefinition, property, getterScope);

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

        private static FieldDefinition TryFindTrivialSetterFieldOnType(TypeDefinition type, PropertyDefinition property)
        {
            if (property.CustomAttributes.Any(attr => attr.AttributeType.Name == "AutoFireAttribute"))
                return null;
            var setter = property.SetMethod;
            if (setter?.Body == null) return null;

            var instructions = setter.Body.Instructions
                .Where(instruction => instruction.OpCode != OpCodes.Nop).ToList();
            if (instructions.Count != 4
                || instructions[0].OpCode != OpCodes.Ldarg_0
                || instructions[1].OpCode != OpCodes.Ldarg_1
                || instructions[2].OpCode != OpCodes.Stfld
                || instructions[3].OpCode != OpCodes.Ret)
                return null;

            var fieldName = (instructions[2].Operand as FieldReference)?.Name;
            return type.Fields.FirstOrDefault(field => field.Name == fieldName);
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
        /// Emits a getter function for an EventBinding node.
        /// The handler expression is like "Model.IncrementClick" or a lambda "(e) => Model.IncrementClick()".
        /// For item graphs, uses tuple DataContext: dc[2] for item methods, dc[0] for Model methods.
        /// </summary>
        private Expression EmitEventGetter(string handlerExpression, int nodeIndex)
        {
            if (string.IsNullOrEmpty(handlerExpression))
                return new NullLiteralExpression(_scope);

            var eventLocation = _topology.Events.FirstOrDefault(evt => evt.NodeIdx == nodeIndex)?.Location
                ?? _fallbackLocation;

            // Track whether this is a Model-level method reference (for tuple index selection)
            bool isModelMethodRef = handlerExpression.StartsWith("Model.");
            bool isControlMethodRef = handlerExpression.StartsWith("Control.");

            // Strip Model. or item variable prefix (e.g., "folder.", "todo.")
            var expr = handlerExpression;
            if (expr.StartsWith("Model."))
                expr = expr.Substring(6);
            else if (expr.StartsWith("Control."))
                expr = expr.Substring(8);
            if (!string.IsNullOrEmpty(_topology?.ItemVariablePrefix)
                && expr.StartsWith(_topology.ItemVariablePrefix))
                expr = expr.Substring(_topology.ItemVariablePrefix.Length);

            // For simple method references (no parens, no lambda)
            if (expr.IndexOfAny(new[] { '(', ')', '=', '>' }) < 0)
            {
                if (isControlMethodRef)
                    RequirePublicControlHandler(FindTypeDefinition(_controlTypeName), expr,
                        eventLocation);
                // Resolve method — for Model methods in item graphs, look up on parent type
                var resolveTypeName = isControlMethodRef ? _controlTypeName
                    : (isModelMethodRef && IsItemGraph && !string.IsNullOrEmpty(_parentModelTypeName))
                        ? _parentModelTypeName : null;
                var methodId = TryResolveMethodIdentifier(expr, out bool isDevirtualized, resolveTypeName);
                if (methodId != null)
                {
                    var outerScope = new IdentifierScope(_scope, new[] { "dc", "tp" }, false);
                    var dcParam = outerScope.ParameterIdentifiers[0];
                    var innerScope = new IdentifierScope(outerScope, new[] { "e", "ev" }, false);

                    int tupleIdx = isModelMethodRef ? 0 : 2;
                    var dcRef = isControlMethodRef
                        ? (Expression)new IdentifierExpression(outerScope.ParameterIdentifiers[1], innerScope)
                        : CreateTupleAccessExpression(dcParam, innerScope, tupleIdx);
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

                if (isControlMethodRef)
                    throw new RazorSubControlDiagnosticException(eventLocation,
                        "Cannot resolve public control event handler '" + handlerExpression + "'.");

                throw new RazorSubControlDiagnosticException(eventLocation,
                    "Cannot resolve event handler '" + handlerExpression + "'.");
            }

            // Parent-context method invocation inside a foreach item template:
            // Pattern: "Model.Method(itemVar)" → function(dc) { return function(e, ev) { dc[0].method(dc[2], e, ev); }; }
            var parentInvocation = handlerExpression;
            var lambdaArrow = parentInvocation.IndexOf("=>", StringComparison.Ordinal);
            if (lambdaArrow >= 0)
                parentInvocation = parentInvocation.Substring(lambdaArrow + 2).Trim();
            var parentMethodResult = TryEmitParentMethodInvocation(parentInvocation);
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

            throw new RazorSubControlDiagnosticException(eventLocation,
                "Cannot resolve event handler '" + handlerExpression + "': supported forms are "
                + "Model.M / Control.M / item.M, Model.M(item) inside @foreach, and "
                + "(e) => Model.M().");
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

            var resolvedMethod = FindPublicMethod(typeDefinition, methodName);
            if (resolvedMethod != null)
            {
                isDevirtualized = IsMethodDevirtualized(resolvedMethod);
                if (isDevirtualized)
                    return _scopeManager.ResolveStatic(resolvedMethod);
                return _scopeManager.Resolve(resolvedMethod);
            }

            return null;
        }

        internal static MethodDefinition RequirePublicControlHandler(
            TypeDefinition controlType, string methodName, Location location)
        {
            var method = FindPublicMethod(controlType, methodName);
            if (method == null)
                throw new RazorSubControlDiagnosticException(location,
                    "Cannot resolve public control event handler 'Control." + methodName + "'.");
            return method;
        }

        private static MethodDefinition FindPublicMethod(TypeDefinition type, string methodName)
        {
            for (var currentType = type; currentType != null;
                currentType = currentType.BaseType?.Resolve())
            {
                var method = currentType.Methods.FirstOrDefault(candidate =>
                    candidate.Name == methodName && candidate.IsPublic && !candidate.IsConstructor);
                if (method != null) return method;
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
        /// Resolves the element type of a loop's collection by walking the collection path from
        /// its root (Model, Control or the enclosing loop variable) hop by hop —
        /// <c>Model.Child.Items</c> as well as <c>Model.Items</c> — and reading the final
        /// property's generic argument, e.g. ObservableCollection&lt;RazorItemVM&gt; → "RazorItemVM".
        /// Null when the root or any hop is unknown.
        /// </summary>
        private string ResolveCollectionItemTypeName(CollectionTopology ct)
        {
            if (_clrContext == null) return null;

            var segments = (ct.IrNode.CollectionExpression ?? "").Split('.');
            var rootType = FindTypeDefinition(ResolveRootTypeName(segments[0]));
            return CecilTypeHelper.CollectionItemTypeName(
                _typeHelper.FindPropertyPath(rootType, segments.Skip(1)));
        }

        private static string EscapeJsString(string s)
            => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

        /// <summary>
        /// Emits a Gate targetInfo as a proper GateTargetInfo instance via IIFE.
        /// HTML content is computed from the IR node's branches.
        /// </summary>
        private Expression EmitGateTargetInfo(GateTopology gt)
        {
            var trueHtml = RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(gt.IrNode.TrueBranch);
            var falseHtml = (gt.IrNode.FalseBranch != null && gt.IrNode.FalseBranch.Count > 0)
                ? RazorSkinCodeGenerator.CollectItemTemplateHtmlPublic(gt.IrNode.FalseBranch)
                : "";
            trueHtml = RazorCssManager.ReplaceCssClassNamesInHtml(trueHtml, _cssManager);
            falseHtml = RazorCssManager.ReplaceCssClassNamesInHtml(falseHtml, _cssManager);

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
                    ct.ItemTopology, _scope, _scopeManager, _knownTypes,
                    _clrContext, itemTypeName ?? _modelTypeName,
                    _resolvedTypeIdentifiers,
                    parentModelTypeName: _modelTypeName,
                    cssManager: _cssManager,
                    usingNamespaces: _usingNamespaces,
                    fallbackLocation: ct.IrNode?.Location ?? _fallbackLocation,
                    controlTypeName: _controlTypeName);
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

                return EmitTypedObject(_collectionTargetInfoFactory, fields);
            }

            // Fallback: plain object literal if factory resolution failed
            var info = new InlineObjectInitializer(null, _scope);
            AddField(info, _collectionMarkerIdxField, "MarkerIdx", new NumberLiteralExpression(_scope, ct.MarkerIdx));
            AddField(info, _collectionItemTemplateField, "ItemTemplate", new StringLiteralExpression(_scope, itemHtml));
            if (itemGraphExpr != null)
                AddField(info, _collectionItemGraphField, "ItemGraph", itemGraphExpr);

            return info;
        }

        /// <summary>
        /// Builds a TypeFactory function expression for a sub-control type:
        /// function(elem) { return ControlType_factory(elem); }
        /// The factory creates a new control instance given a DOM element.
        /// </summary>
        private Expression BuildSubControlTypeFactory(TypeDefinition typeDef)
        {
            // Find the constructor that takes an Element parameter
            var ctor = RazorSkinJSTGenerator.FindElementConstructor(typeDef);

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
        /// Resolves a type using the template's imports and ambiguity rules.
        /// </summary>
        private TypeDefinition FindSubControlType(string typeName)
        {
            try
            {
                return RazorSkinJSTGenerator.ResolveSubControlType(
                    _clrContext.GetTypes(), typeName, _usingNamespaces);
            }
            catch (InvalidOperationException ex)
            {
                throw new RazorSubControlDiagnosticException(_fallbackLocation, ex.Message);
            }
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
                        fields.Add((_subscriptionChainParentGettersField, "ChainParentGetters",
                            new InlineNewArrayInitialization(null, _scope, BuildChainParentGetters(sub))));
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
                        AddField(subObj, _subscriptionChainParentGettersField, "ChainParentGetters",
                            new InlineNewArrayInitialization(null, _scope, BuildChainParentGetters(sub)));
                    }

                    items.Add(subObj);
                }
            }

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// Builds the per-segment owner accessors for a chained subscription. Accessor <c>k</c>
        /// returns the object that owns <c>PathSegments[k]</c>: the chain root for <c>k == 0</c>,
        /// otherwise the root walked through the first <c>k</c> segments. Each is a
        /// <c>(dc, tp) =&gt; value</c> function resolved exactly like a node getter, so the runtime
        /// can subscribe to the leaf object and re-target listeners when a mid-path object changes.
        /// </summary>
        private List<Expression> BuildChainParentGetters(SubscriptionInfo sub)
        {
            var getters = new List<Expression>();
            string root = ChainRootToken(sub.SourceSlot);
            for (int k = 0; k < sub.PathSegments.Length; k++)
            {
                string ownerExpression = root;
                for (int j = 0; j < k; j++)
                    ownerExpression += "." + sub.PathSegments[j];
                getters.Add(BuildBindingExpressionGetter(ownerExpression));
            }
            return getters;
        }

        /// <summary>
        /// Maps a subscription source slot to its chain root token: 1 =&gt; Control, 2 =&gt; the loop
        /// variable (item graphs), everything else =&gt; Model. The token is resolved by
        /// <see cref="ResolveRoot"/> the same way node getters resolve their roots.
        /// </summary>
        private string ChainRootToken(int sourceSlot)
        {
            if (sourceSlot == 1) return "Control";
            if (sourceSlot == 2 && IsItemGraph) return _topology.ItemVariablePrefix.TrimEnd('.');
            return "Model";
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
                var typeDef = FindSubControlType(sc.ResolvedTypeName ?? sc.ControlTypeName);
                var typeFactory = typeDef != null ? BuildSubControlTypeFactory(typeDef) : null;
                var skinFactory = typeDef != null ? BuildSubControlSkinFactory(typeDef) : null;

                // Build bindings array
                var bindingItems = new List<Expression>();
                foreach (var propBinding in sc.PropertyBindings.OrderBy(p =>
                    p.TargetPropertyName == "Skin" ? 0 : p.TargetPropertyName == "DataContext" ? 1 : 2))
                {
                    var setter = BuildSubControlPropertySetter(
                        sc.ResolvedTypeName ?? sc.ControlTypeName,
                        propBinding.TargetPropertyName);
                    var targetGetter = !string.IsNullOrEmpty(propBinding.TwoWaySourceProperty)
                        ? BuildSubControlTargetGetter(typeDef, propBinding.TargetPropertyName)
                        : null;
                    var sourceSetter = !string.IsNullOrEmpty(propBinding.TwoWaySourceProperty)
                        ? BuildSubControlSourceSetter(propBinding.TwoWaySourceProperty,
                            propBinding.TwoWaySourceSlot)
                        : null;

                    if (_subControlPropertyInfoFactory != null)
                    {
                        var fields = new List<(IIdentifier, string, Expression)>
                        {
                            (_subControlPropNodeIdxField, "NodeIdx", new NumberLiteralExpression(_scope, propBinding.NodeIdx)),
                            (_subControlPropSetterField, "Setter", setter ?? new NullLiteralExpression(_scope))
                        };
                        if (sourceSetter != null)
                        {
                            fields.Add((_subControlPropTargetNameField, "TargetPropertyName",
                                new StringLiteralExpression(_scope, propBinding.TargetPropertyName)));
                            fields.Add((_subControlPropTargetGetterField, "TargetGetter", targetGetter));
                            fields.Add((_subControlPropSourceSetterField, "SourceSetter", sourceSetter));
                        }
                        bindingItems.Add(EmitTypedObject(_subControlPropertyInfoFactory, fields));
                    }
                    else
                    {
                        var propObj = new InlineObjectInitializer(null, _scope);
                        AddField(propObj, _subControlPropNodeIdxField, "NodeIdx",
                            new NumberLiteralExpression(_scope, propBinding.NodeIdx));
                        AddField(propObj, _subControlPropSetterField, "Setter",
                            setter ?? new NullLiteralExpression(_scope));
                        if (sourceSetter != null)
                        {
                            AddField(propObj, _subControlPropTargetNameField, "TargetPropertyName",
                                new StringLiteralExpression(_scope, propBinding.TargetPropertyName));
                            AddField(propObj, _subControlPropTargetGetterField, "TargetGetter", targetGetter);
                            AddField(propObj, _subControlPropSourceSetterField, "SourceSetter", sourceSetter);
                        }
                        bindingItems.Add(propObj);
                    }
                }

                var bindingsExpr = new InlineNewArrayInitialization(null, _scope, bindingItems);

                if (_subControlInfoFactory != null)
                {
                    var fields = new List<(IIdentifier, string, Expression)>
                    {
                        (_subControlMarkerIdxField, "MarkerIdx", new NumberLiteralExpression(_scope, sc.ElemIdx)),
                        (_subControlTypeFactoryField, "TypeFactory", typeFactory ?? new NullLiteralExpression(_scope)),
                        (_subControlSkinFactoryField, "SkinFactory", skinFactory ?? new NullLiteralExpression(_scope)),
                        (_subControlElemIdxField, "ElemIdx", new NumberLiteralExpression(_scope, sc.ElemIdx)),
                        (_subControlBindingsField, "Bindings", bindingsExpr),
                        (_subControlHasDataContextBindingField, "HasDataContextBinding",
                            new BooleanLiteralExpression(_scope, sc.PropertyBindings.Any(p => p.TargetPropertyName == "DataContext")))
                    };
                    items.Add(EmitTypedObject(_subControlInfoFactory, fields));
                }
                else
                {
                    var scObj = new InlineObjectInitializer(null, _scope);
                    AddField(scObj, _subControlMarkerIdxField, "MarkerIdx",
                        new NumberLiteralExpression(_scope, sc.ElemIdx));
                    AddField(scObj, _subControlTypeFactoryField, "TypeFactory",
                        typeFactory ?? new NullLiteralExpression(_scope));
                    AddField(scObj, _subControlSkinFactoryField, "SkinFactory",
                        skinFactory ?? new NullLiteralExpression(_scope));
                    AddField(scObj, _subControlElemIdxField, "ElemIdx",
                        new NumberLiteralExpression(_scope, sc.ElemIdx));
                    AddField(scObj, _subControlBindingsField, "Bindings", bindingsExpr);
                    AddField(scObj, _subControlHasDataContextBindingField, "HasDataContextBinding",
                        new BooleanLiteralExpression(_scope, sc.PropertyBindings.Any(p => p.TargetPropertyName == "DataContext")));
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

            var typeDef = FindSubControlType(controlTypeName);
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

            var fn = new FunctionExpression(_fallbackLocation, _scope, setterScope, setterScope.ParameterIdentifiers, null);
            var field = TryFindTrivialSetterFieldOnType(property.DeclaringType.Resolve(), property);
            if (field != null)
            {
                fn.AddStatement(ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(null, setterScope,
                        new IdentifierExpression(ctrlParam, setterScope),
                        new IdentifierExpression(_scopeManager.Resolve(field), setterScope)),
                    new IdentifierExpression(valParam, setterScope)));
            }
            else
            {
                var method = property.SetMethod;
                var control = new IdentifierExpression(ctrlParam, setterScope);
                var value = new IdentifierExpression(valParam, setterScope);
                var callExpr = IsMethodDevirtualized(method)
                    ? new MethodCallExpression(null, setterScope,
                        new IdentifierExpression(_scopeManager.ResolveStatic(method), setterScope),
                        new Expression[] { control, value })
                    : new MethodCallExpression(null, setterScope,
                        new IndexExpression(null, setterScope, control,
                            new IdentifierExpression(_scopeManager.Resolve(method), setterScope)),
                        new Expression[] { value });
                fn.AddStatement(new ExpressionStatement(_fallbackLocation, setterScope, callExpr));
            }
            return fn;
        }

        private Expression BuildSubControlTargetGetter(TypeDefinition typeDef, string propertyName)
        {
            var property = FindProperty(typeDef, propertyName);
            if (property?.GetMethod == null)
                throw new InvalidOperationException(
                    $"Two-way target {typeDef.FullName}.{propertyName} has no getter.");

            var scope = new IdentifierScope(_scope, new[] { "ctrl" }, false);
            var control = new IdentifierExpression(scope.ParameterIdentifiers[0], scope);
            Expression value;
            var declaringType = property.DeclaringType.Resolve();
            var field = TryFindBackingFieldOnType(declaringType, property);
            if (field != null)
            {
                value = new IndexExpression(null, scope, control,
                    new IdentifierExpression(_scopeManager.Resolve(field), scope));
            }
            else
            {
                var method = property.GetMethod;
                value = IsMethodDevirtualized(method)
                    ? new MethodCallExpression(null, scope,
                        new IdentifierExpression(_scopeManager.ResolveStatic(method), scope),
                        new Expression[] { control })
                    : new MethodCallExpression(null, scope,
                        new IndexExpression(null, scope, control,
                            new IdentifierExpression(_scopeManager.Resolve(method), scope)),
                        System.Array.Empty<Expression>());
            }
            var fn = new FunctionExpression(_fallbackLocation, _scope, scope,
                scope.ParameterIdentifiers, null);
            fn.AddStatement(new ReturnStatement(_fallbackLocation, scope, value));
            return fn;
        }

        private Expression BuildSubControlSourceSetter(string propertyName, int sourceSlot)
        {
            var sourceTypeName = IsItemGraph && sourceSlot == 0
                ? _parentModelTypeName : _modelTypeName;
            var sourceType = FindTypeDefinition(sourceTypeName);
            var property = sourceType != null ? FindProperty(sourceType, propertyName) : null;
            if (property?.SetMethod == null)
                throw new InvalidOperationException(
                    $"Two-way source {sourceTypeName}.{propertyName} has no setter.");

            var scope = new IdentifierScope(_scope, new[] { "dc", "val" }, false);
            Expression source = new IdentifierExpression(scope.ParameterIdentifiers[0], scope);
            if (IsItemGraph)
                source = new IndexExpression(null, scope, source,
                    new NumberLiteralExpression(scope, sourceSlot));
            var fn = new FunctionExpression(_fallbackLocation, _scope, scope,
                scope.ParameterIdentifiers, null);
            var field = TryFindTrivialSetterFieldOnType(property.DeclaringType.Resolve(), property);
            var value = new IdentifierExpression(scope.ParameterIdentifiers[1], scope);
            if (field != null)
            {
                fn.AddStatement(ExpressionStatement.CreateAssignmentExpression(
                    new IndexExpression(null, scope, source,
                        new IdentifierExpression(_scopeManager.Resolve(field), scope)), value));
            }
            else
            {
                var method = property.SetMethod;
                var call = IsMethodDevirtualized(method)
                    ? new MethodCallExpression(null, scope,
                        new IdentifierExpression(_scopeManager.ResolveStatic(method), scope),
                        new Expression[] { source, value })
                    : new MethodCallExpression(null, scope,
                        new IndexExpression(null, scope, source,
                            new IdentifierExpression(_scopeManager.Resolve(method), scope)),
                        new Expression[] { value });
                fn.AddStatement(new ExpressionStatement(_fallbackLocation, scope, call));
            }
            return fn;
        }
    }
}
