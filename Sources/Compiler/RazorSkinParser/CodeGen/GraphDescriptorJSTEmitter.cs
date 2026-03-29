using System.Collections.Generic;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Emits a graph descriptor as a JST <see cref="InlineObjectInitializer"/> with function
    /// references resolved via <see cref="RuntimeScopeManager"/>. Uses proper JST nodes
    /// that participate in NScript's scope-based minification system.
    /// </summary>
    public class GraphDescriptorJSTEmitter
    {
        private readonly GraphTopology _topology;
        private readonly IdentifierScope _scope;
        private readonly RuntimeScopeManager _scopeManager;
        private readonly RazorKnownTypes _knownTypes;
        private readonly ISet<string> _knownFunctionNames;

        public GraphDescriptorJSTEmitter(
            GraphTopology topology,
            IdentifierScope scope,
            RuntimeScopeManager scopeManager,
            RazorKnownTypes knownTypes,
            ISet<string> knownFunctionNames)
        {
            _topology = topology;
            _scope = scope;
            _scopeManager = scopeManager;
            _knownTypes = knownTypes;
            _knownFunctionNames = knownFunctionNames;
        }

        /// <summary>
        /// Emits the complete graph descriptor as an InlineObjectInitializer JST node.
        /// Fields: nodeTypes, getters, consumers, gateIndices, defaultValues,
        /// targetInfos, subscriptions, subscribeMode, nodeCount, parentIndices.
        /// </summary>
        public InlineObjectInitializer Emit()
        {
            var obj = new InlineObjectInitializer(null, _scope);

            obj.AddInitializer("nodeTypes", EmitNodeTypes());
            obj.AddInitializer("getters", EmitGetters());
            obj.AddInitializer("consumers", EmitConsumers());
            obj.AddInitializer("gateIndices", EmitGateIndices());
            obj.AddInitializer("defaultValues", EmitDefaultValues());
            obj.AddInitializer("targetInfos", EmitTargetInfos());
            obj.AddInitializer("subscriptions", EmitSubscriptions());
            obj.AddInitializer("subscribeMode", new NumberLiteralExpression(_scope, 0));
            obj.AddInitializer("nodeCount", new NumberLiteralExpression(_scope, _topology.NodeCount));

            if (!string.IsNullOrEmpty(_topology.ModelTypeName))
                obj.AddInitializer("sourceType", new StringLiteralExpression(_scope, _topology.ModelTypeName));

            obj.AddInitializer("parentIndices", EmitParentIndices());

            return obj;
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
        /// Uses <see cref="RawBodyFunctionExpression"/> for getter bodies since getter
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
                case GraphNodeTypeConstants.EventBinding:
                    return new NullLiteralExpression(_scope);

                case GraphNodeTypeConstants.Property:
                {
                    if (string.IsNullOrEmpty(getterExpression))
                        return new NullLiteralExpression(_scope);

                    string body;
                    if (_knownFunctionNames != null && _knownFunctionNames.Contains(getterExpression))
                        body = "return dc." + getterExpression;
                    else
                    {
                        var getterName = ExpressionJsEmitter.PropertyToGetterName(getterExpression);
                        body = "return dc." + getterName + "()";
                    }

                    return CreateGetterFunction(body);
                }

                case GraphNodeTypeConstants.Computed:
                case GraphNodeTypeConstants.Gate:
                case GraphNodeTypeConstants.CollectionManager:
                {
                    if (string.IsNullOrEmpty(getterExpression))
                        return new NullLiteralExpression(_scope);

                    var jsExpr = ExpressionJsEmitter.ToJsGetter(
                        getterExpression, "dc", "tp", _knownFunctionNames);
                    return CreateGetterFunction("return " + jsExpr);
                }

                default:
                    return new NullLiteralExpression(_scope);
            }
        }

        /// <summary>
        /// Creates a getter function: function(dc) { body; }
        /// Uses <see cref="RawBodyFunctionExpression"/> to emit the raw JS body text.
        /// </summary>
        private RawBodyFunctionExpression CreateGetterFunction(string rawBody)
        {
            var innerScope = new IdentifierScope(
                _scope,
                new string[] { "dc" },
                false);

            return new RawBodyFunctionExpression(
                null,
                _scope,
                innerScope,
                innerScope.ParameterIdentifiers,
                rawBody);
        }

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
                else
                {
                    items.Add(new NullLiteralExpression(_scope));
                }
            }

            return new InlineNewArrayInitialization(null, _scope, items);
        }

        /// <summary>
        /// Emits a DomTarget targetInfo object: { elem: N, set: SetterFunc }
        /// The setter is resolved via RuntimeScopeManager.ResolveStatic for minification.
        /// </summary>
        private Expression EmitDomTargetInfo(DomTargetTopology dt)
        {
            var info = new InlineObjectInitializer(null, _scope);
            info.AddInitializer("elem", new NumberLiteralExpression(_scope, dt.ElemIdx));

            var setterMethod = _knownTypes.GetSetterMethod(dt.Target);
            var setterId = _scopeManager.ResolveStatic(setterMethod);
            info.AddInitializer("set", new IdentifierExpression(setterId, _scope));

            // For attribute/class/style targets, include the attribute name
            if (dt.Target == ExpressionTarget.Attribute ||
                dt.Target == ExpressionTarget.CssClass ||
                dt.Target == ExpressionTarget.Style)
            {
                var attrName = dt.AttributeName ?? (dt.Target == ExpressionTarget.CssClass ? "class" : "style");
                info.AddInitializer("arg", new StringLiteralExpression(_scope, attrName));
            }

            return info;
        }

        /// <summary>
        /// Emits a Gate targetInfo object: { marker: N, trueHtml: "...", falseHtml: "..." }
        /// HTML content is computed from the IR node's branches.
        /// </summary>
        private Expression EmitGateTargetInfo(GateTopology gt)
        {
            var info = new InlineObjectInitializer(null, _scope);
            info.AddInitializer("marker", new NumberLiteralExpression(_scope, gt.MarkerIdx));

            var trueHtml = RazorSkinCodeGenerator.CollectHtmlPublic(gt.IrNode.TrueBranch);
            info.AddInitializer("trueHtml", new StringLiteralExpression(_scope, trueHtml));

            var falseHtml = (gt.IrNode.FalseBranch != null && gt.IrNode.FalseBranch.Count > 0)
                ? RazorSkinCodeGenerator.CollectHtmlPublic(gt.IrNode.FalseBranch)
                : "";
            info.AddInitializer("falseHtml", new StringLiteralExpression(_scope, falseHtml));

            return info;
        }

        /// <summary>
        /// Emits a Collection targetInfo object: { marker: N, itemHtml: "..." }
        /// If the collection has an item topology, it is recursively emitted.
        /// </summary>
        private Expression EmitCollectionTargetInfo(CollectionTopology ct)
        {
            var info = new InlineObjectInitializer(null, _scope);
            info.AddInitializer("marker", new NumberLiteralExpression(_scope, ct.MarkerIdx));

            var itemHtml = (ct.IrNode.ItemTemplate != null && ct.IrNode.ItemTemplate.Count > 0)
                ? RazorSkinCodeGenerator.CollectHtmlPublic(ct.IrNode.ItemTemplate)
                : "";
            info.AddInitializer("itemHtml", new StringLiteralExpression(_scope, itemHtml));

            if (ct.ItemTopology != null)
            {
                var nestedEmitter = new GraphDescriptorJSTEmitter(
                    ct.ItemTopology, _scope, _scopeManager, _knownTypes, _knownFunctionNames);
                info.AddInitializer("itemGraph", nestedEmitter.Emit());
            }

            return info;
        }

        /// <summary>subscriptions: [["Name", 1], ...]</summary>
        private Expression EmitSubscriptions()
        {
            var items = new List<Expression>();
            foreach (var sub in _topology.Subscriptions)
            {
                var pair = new InlineNewArrayInitialization(
                    null,
                    _scope,
                    new List<Expression>
                    {
                        new StringLiteralExpression(_scope, sub.PropertyName),
                        new NumberLiteralExpression(_scope, sub.NodeIdx)
                    });
                items.Add(pair);
            }

            return new InlineNewArrayInitialization(null, _scope, items);
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
    }

    /// <summary>
    /// A function expression that writes a raw JS body string. Used for graph descriptor
    /// getter functions whose bodies contain property accessor calls that are already
    /// correctly mangled (e.g., "return dc.get_name()"). The function wrapper and
    /// parameter list are proper JST nodes; only the body is raw text.
    /// </summary>
    public class RawBodyFunctionExpression : Expression
    {
        private readonly IdentifierScope _innerScope;
        private readonly IList<SimpleIdentifier> _parameters;
        private readonly string _rawBody;

        /// <summary>
        /// Creates a new raw-body function expression.
        /// </summary>
        /// <param name="location">Source location (may be null).</param>
        /// <param name="outerScope">The enclosing scope.</param>
        /// <param name="innerScope">The function's own scope (contains parameter identifiers).</param>
        /// <param name="parameters">The function parameter identifiers.</param>
        /// <param name="rawBody">The raw JS function body text (without braces or semicolons).</param>
        public RawBodyFunctionExpression(
            Location location,
            IdentifierScope outerScope,
            IdentifierScope innerScope,
            IList<SimpleIdentifier> parameters,
            string rawBody)
            : base(location, outerScope)
        {
            _innerScope = innerScope;
            _parameters = parameters;
            _rawBody = rawBody;
        }

        public override Precedence Precedence => Precedence.Assignment;

        public override bool IsLeftToRight => false;

        public override void Serialize(ICustomSerializer serializer)
        {
            serializer.AddValue("rawBody", _rawBody);
        }

        /// <summary>
        /// Writes: function(param1, param2) { rawBody; }
        /// Parameters use proper JST identifiers; the body is emitted as raw text.
        /// </summary>
        public override void Write(JSWriter writer)
        {
            writer.Write(Keyword.Function);
            writer.Write(Symbols.BracketOpenRound);

            for (int i = 0; i < _parameters.Count; i++)
            {
                if (i > 0) writer.Write(Symbols.Comma);
                writer.Write(_parameters[i]);
            }

            writer.Write(Symbols.BracketCloseRound);
            writer.Write(Symbols.BracketOpenCurly);
            writer.WriteIdentifier(_rawBody);
            writer.Write(Symbols.SemiColon);
            writer.Write(Symbols.BracketCloseCurly);
        }
    }
}
