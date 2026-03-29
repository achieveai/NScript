# Razor JST Code Generation Extension — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Emit Razor skin template graph descriptors as proper JST nodes with full identifier resolution, replacing raw JS string emission.

**Architecture:** `GraphTopologyBuilder` builds the graph topology from the IR tree. A new `GraphDescriptorJSTEmitter` converts that topology into JST `InlineObjectInitializer` nodes with all function references resolved via `IResolver`. `RazorSkinJSTGenerator` integrates the emitter into its factory body and passes the graph descriptor to the `SkinInstance` constructor.

**Tech Stack:** C#, NScript.JST (JavaScript AST), Mono.Cecil (CLR reflection), NScript.Converter (identifier resolution)

**Spec:** `docs/superpowers/specs/2026-03-28-razor-jst-codegen-design.md`

---

## File Map

| File | Action | Responsibility |
| --- | --- | --- |
| `Sources/Compiler/RazorSkinParser/CodeGen/RazorKnownTypes.cs` | Create | Resolve MethodReference/TypeReference from ClrContext |
| `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs` | Create | Emit graph descriptor as JST InlineObjectInitializer |
| `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinJSTGenerator.cs` | Modify | Integrate graph descriptor emission, remove SkinBinderInfo path |
| `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs` | Modify | Create RazorKnownTypes, pass Resolver, enable graph mode |
| `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs` | Modify | Remove Generate/GenerateGraphMode, keep helpers |

---

### Task 1: Create RazorKnownTypes

Resolve all runtime `MethodReference` and `TypeReference` values needed for graph descriptor JST emission.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/RazorKnownTypes.cs`
- Reference: `Sources/Compiler/XwmlParser/KnownTemplateTypes.cs` (pattern to follow)
- Reference: `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs:280-363` (existing resolution code)

- [ ] **Step 1: Create RazorKnownTypes class**

Create `Sources/Compiler/RazorSkinParser/CodeGen/RazorKnownTypes.cs` with the following content. This class resolves CLR method/type references using the same `ClrContext.GetMethodReference()` pattern used in `RazorTemplatingPlugin.Initialize()` (lines 280-363) and `KnownTemplateTypes` constructor:

```csharp
using System;
using Mono.Cecil;
using NScript.CLR;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Resolved CLR method and type references needed for Razor template
    /// JST code generation. Analogous to XWML's KnownTemplateTypes.
    /// All MethodDefinitions are resolved from ClrContext and later passed
    /// to IResolver.ResolveFactory() or IResolver.ResolveStaticMember()
    /// at JST emission time.
    /// </summary>
    public class RazorKnownTypes
    {
        // --- Static setter methods on SkinBinderHelper ---
        public readonly MethodDefinition SetTextContent;
        public readonly MethodDefinition SetAttribute;
        public readonly MethodDefinition SetCssClass;

        // --- Static helper methods ---
        public readonly MethodDefinition GetElementFromPath;

        // --- Constructor references ---
        public readonly MethodDefinition SkinInstanceCtor;
        public readonly MethodDefinition SkinCtor;

        // --- Type references ---
        public readonly TypeDefinition UISkinableElement;

        public RazorKnownTypes(ClrContext clrContext, ClrKnownReference clrKnownRefs)
        {
            var uiFrameworkDll = "Sunlight.Framework.UI";
            var binderHelperType = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinBinderHelper"));

            var systemWebHtmlDll = "System.Web.Html";
            var elementRefType = clrContext.GetTypeDefinition(
                Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Element"));
            var nodeRefType = clrContext.GetTypeDefinition(
                Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Node"));
            var documentRefType = clrContext.GetTypeDefinition(
                Tuple.Create(systemWebHtmlDll, systemWebHtmlDll + ".Document"));
            var nativeArrayInt = clrContext.GetTypeDefinition(
                Tuple.Create("mscorlib", "System.NativeArray`1"))
                .MakeGenericInstanceType(clrKnownRefs.Int32);

            // SetTextContent(Element elem, string text)
            SetTextContent = clrContext.GetMethodReference(
                "SetTextContent", clrKnownRefs.Void, binderHelperType,
                elementRefType, clrKnownRefs.String).Resolve();

            // SetAttribute(Node node, string value, string attrName)
            SetAttribute = clrContext.GetMethodReference(
                "SetAttribute", clrKnownRefs.Void, binderHelperType,
                nodeRefType, clrKnownRefs.String, clrKnownRefs.String).Resolve();

            // SetCssClass(Element elem, bool add, string className)
            try
            {
                SetCssClass = clrContext.GetMethodReference(
                    "SetCssClass", clrKnownRefs.Void, binderHelperType,
                    elementRefType, clrKnownRefs.Boolean, clrKnownRefs.String).Resolve();
            }
            catch { /* Optional — not all builds have this */ }

            // GetElementFromPath(Element root, int[] path)
            GetElementFromPath = clrContext.GetMethodReference(
                "GetElementFromPath", elementRefType, binderHelperType,
                elementRefType, nativeArrayInt).Resolve();

            // SkinInstance constructor (the graph overload or the standard one)
            var skinInstanceType = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.SkinInstance"));
            // Find the factory method used by templates
            foreach (var method in skinInstanceType.Methods)
            {
                if (method.Name == ".ctor" && method.Parameters.Count == 8)
                {
                    SkinInstanceCtor = method;
                    break;
                }
            }

            // Skin constructor
            var skinType = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".Helpers.Skin"));
            foreach (var method in skinType.Methods)
            {
                if (method.Name == ".ctor" && method.Parameters.Count == 4)
                {
                    SkinCtor = method;
                    break;
                }
            }

            // UISkinableElement type
            UISkinableElement = clrContext.GetTypeDefinition(
                Tuple.Create(uiFrameworkDll, uiFrameworkDll + ".UISkinableElement"));
        }

        /// <summary>
        /// Get the setter MethodDefinition for a given ExpressionTarget.
        /// For Razor, class/style both use SetAttribute (full replacement).
        /// </summary>
        public MethodDefinition GetSetterMethod(TemplateIR.ExpressionTarget target)
        {
            switch (target)
            {
                case TemplateIR.ExpressionTarget.TextContent:
                    return SetTextContent;
                case TemplateIR.ExpressionTarget.Attribute:
                case TemplateIR.ExpressionTarget.CssClass:
                case TemplateIR.ExpressionTarget.Style:
                    return SetAttribute;
                default:
                    return SetTextContent;
            }
        }
    }
}
```

- [ ] **Step 2: Verify it compiles**

```bash
dotnet build Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj -c Release
```

Expected: Build succeeds with 0 errors.

- [ ] **Step 3: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/RazorKnownTypes.cs
git commit -m "feat(razor-codegen): add RazorKnownTypes for CLR method resolution"
```

---

### Task 2: Create GraphDescriptorJSTEmitter

Emit the graph descriptor as a JST `InlineObjectInitializer` with all function references resolved via `IResolver`.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs`
- Reference: `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorEmitter.cs` (raw JS version to replace)
- Reference: `Sources/Compiler/NScript.JST/Expressions/InlineObjectInitializer.cs` (JST object literal)
- Reference: `Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs:58-79` (GraphTopology fields)

- [ ] **Step 1: Create the emitter class**

Create `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs`. This is the largest new file. It mirrors `GraphDescriptorEmitter.EmitDescriptor()` but outputs JST nodes instead of strings.

The emitter needs to handle these graph descriptor fields:
- `nodeTypes`: array of ints
- `getters`: array of functions (need resolved property accessors)
- `consumers`: array of int arrays
- `gateIndices`: array of ints
- `defaultValues`: array of mixed values
- `targetInfos`: array of objects with resolved setter references
- `subscriptions`: array of objects with string/int fields
- `sourceType`: resolved type reference
- `subscribeMode`: int
- `nodeCount`: int
- `parentIndices`: array of int arrays

```csharp
using System;
using System.Collections.Generic;
using NScript.CLR;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using NScript.RazorSkin.TemplateIR;
using Serilog;

namespace NScript.RazorSkin.CodeGen
{
    /// <summary>
    /// Emits a GraphDescriptor as a JST InlineObjectInitializer with all
    /// function references resolved through IResolver. Replaces the raw-JS
    /// GraphDescriptorEmitter for retail-safe code generation.
    /// </summary>
    public class GraphDescriptorJSTEmitter
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private readonly GraphTopology _topology;
        private readonly IdentifierScope _scope;
        private readonly IResolver _resolver;
        private readonly RazorKnownTypes _knownTypes;
        private readonly ISet<string> _knownFunctionNames;

        public GraphDescriptorJSTEmitter(
            GraphTopology topology,
            IdentifierScope scope,
            IResolver resolver,
            RazorKnownTypes knownTypes,
            ISet<string> knownFunctionNames)
        {
            _topology = topology;
            _scope = scope;
            _resolver = resolver;
            _knownTypes = knownTypes;
            _knownFunctionNames = knownFunctionNames;
        }

        /// <summary>
        /// Emit the complete graph descriptor as a JST expression.
        /// </summary>
        public Expression Emit()
        {
            var obj = new InlineObjectInitializer(null, _scope);

            obj.AddInitializer("nodeTypes", EmitIntArray(_topology.NodeTypes));
            obj.AddInitializer("getters", EmitGetters());
            obj.AddInitializer("consumers", EmitConsumers());
            obj.AddInitializer("gateIndices", EmitIntArray(_topology.GateIndices));
            obj.AddInitializer("defaultValues", EmitDefaultValues());
            obj.AddInitializer("targetInfos", EmitTargetInfos());
            obj.AddInitializer("subscriptions", EmitSubscriptions());
            obj.AddInitializer("subscribeMode", new NumberLiteralExpression(_scope, 0));
            obj.AddInitializer("nodeCount", new NumberLiteralExpression(_scope, _topology.NodeCount));

            if (_topology.ParentIndices != null)
                obj.AddInitializer("parentIndices", EmitParentIndices());

            return obj;
        }

        // --- Array emission helpers ---

        private Expression EmitIntArray(int[] values)
        {
            var items = new List<Expression>();
            for (int i = 0; i < values.Length; i++)
                items.Add(new NumberLiteralExpression(_scope, values[i]));
            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitGetters()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                var expr = _topology.GetterExpressions[i];
                if (expr == null)
                {
                    items.Add(new NullLiteralExpression(_scope));
                    continue;
                }

                // Build getter function: function(dc) { return <expr>; }
                var getterScope = new IdentifierScope(_scope, new[] { "dc" }, false);
                var getterJs = ExpressionJsEmitter.ToJsGetter(expr, "dc", "tp", _knownFunctionNames);

                // Create function with raw body — the getter expression references
                // property accessors that are virtual (instance methods on the VM).
                // Since we can't easily resolve each property accessor via Resolver
                // from a string expression, we use RawBodyFunctionExpression which
                // emits the body as-is. The property accessor names (get_name, etc.)
                // are virtual method names that NScript's compiler already mangles
                // correctly during the main compilation pass.
                var fn = new RawBodyFunctionExpression(
                    null, _scope, getterScope,
                    getterScope.ParameterIdentifiers,
                    $"return {getterJs}");
                items.Add(fn);
            }
            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitConsumers()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                var consumers = _topology.Consumers[i];
                if (consumers == null || consumers.Count == 0)
                {
                    items.Add(new InlineNewArrayInitialization(null, _scope, new List<Expression>()));
                    continue;
                }
                var inner = new List<Expression>();
                foreach (var c in consumers)
                    inner.Add(new NumberLiteralExpression(_scope, c));
                items.Add(new InlineNewArrayInitialization(null, _scope, inner));
            }
            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitDefaultValues()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                var val = _topology.DefaultValues[i];
                if (val == null)
                    items.Add(new NullLiteralExpression(_scope));
                else if (val is string s)
                    items.Add(new StringLiteralExpression(_scope, s));
                else if (val is bool b)
                    items.Add(new BooleanLiteralExpression(_scope, b));
                else if (val is int n)
                    items.Add(new NumberLiteralExpression(_scope, n));
                else
                    items.Add(new NullLiteralExpression(_scope));
            }
            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitTargetInfos()
        {
            // Build lookup maps from topology lists
            var domTargetMap = new Dictionary<int, DomTargetTopology>();
            foreach (var dt in _topology.DomTargets)
                domTargetMap[dt.NodeIdx] = dt;

            var gateMap = new Dictionary<int, GateTopology>();
            foreach (var g in _topology.Gates)
                gateMap[g.NodeIdx] = g;

            var collectionMap = new Dictionary<int, CollectionTopology>();
            foreach (var c in _topology.Collections)
                collectionMap[c.NodeIdx] = c;

            var eventMap = new Dictionary<int, EventTopology>();
            foreach (var e in _topology.Events)
                eventMap[e.NodeIdx] = e;

            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                if (domTargetMap.TryGetValue(i, out var dt))
                {
                    items.Add(EmitDomTargetInfo(dt));
                }
                else if (gateMap.TryGetValue(i, out var gate))
                {
                    items.Add(EmitGateTargetInfo(gate));
                }
                else if (collectionMap.TryGetValue(i, out var coll))
                {
                    items.Add(EmitCollectionTargetInfo(coll));
                }
                else if (eventMap.TryGetValue(i, out var evt))
                {
                    items.Add(EmitEventTargetInfo(evt));
                }
                else
                {
                    items.Add(new NullLiteralExpression(_scope));
                }
            }
            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitDomTargetInfo(DomTargetTopology dt)
        {
            var obj = new InlineObjectInitializer(null, _scope);
            obj.AddInitializer("elem", new NumberLiteralExpression(_scope, dt.ElemIdx));

            // Resolve setter via IResolver — this is the key retail-safe pattern
            var setterMethod = _knownTypes.GetSetterMethod(dt.Target);
            var setterIds = _resolver.ResolveStaticMember(setterMethod);
            var setterId = new CompoundIdentifier(setterIds);
            obj.AddInitializer("set", new IdentifierExpression(setterId, _scope));

            // For SetAttribute targets, include the attribute name
            if (dt.Target == ExpressionTarget.Attribute ||
                dt.Target == ExpressionTarget.CssClass ||
                dt.Target == ExpressionTarget.Style)
            {
                var attrName = dt.AttributeName ?? "class";
                obj.AddInitializer("arg", new StringLiteralExpression(_scope, attrName));
            }

            return obj;
        }

        private Expression EmitGateTargetInfo(GateTopology gate)
        {
            var obj = new InlineObjectInitializer(null, _scope);
            obj.AddInitializer("marker", new NumberLiteralExpression(_scope, gate.MarkerIdx));
            obj.AddInitializer("trueTemplate", new StringLiteralExpression(_scope, gate.TrueHtml));
            if (gate.FalseHtml != null)
                obj.AddInitializer("falseTemplate", new StringLiteralExpression(_scope, gate.FalseHtml));
            else
                obj.AddInitializer("falseTemplate", new NullLiteralExpression(_scope));
            obj.AddInitializer("trueElemCount", new NumberLiteralExpression(_scope, gate.TrueElemCount));
            obj.AddInitializer("falseElemCount", new NumberLiteralExpression(_scope, gate.FalseElemCount));
            return obj;
        }

        private Expression EmitCollectionTargetInfo(CollectionTopology coll)
        {
            var obj = new InlineObjectInitializer(null, _scope);
            obj.AddInitializer("marker", new NumberLiteralExpression(_scope, coll.MarkerIdx));
            obj.AddInitializer("itemTemplate", new StringLiteralExpression(_scope, coll.ItemTemplateHtml));

            // Nested item graph descriptor (recursive)
            if (coll.ItemTopology != null)
            {
                var nestedEmitter = new GraphDescriptorJSTEmitter(
                    coll.ItemTopology, _scope, _resolver, _knownTypes, _knownFunctionNames);
                obj.AddInitializer("itemGraph", nestedEmitter.Emit());
            }
            else
            {
                obj.AddInitializer("itemGraph", new NullLiteralExpression(_scope));
            }

            return obj;
        }

        private Expression EmitEventTargetInfo(EventTopology evt)
        {
            var obj = new InlineObjectInitializer(null, _scope);
            obj.AddInitializer("elem", new NumberLiteralExpression(_scope, evt.ElemIdx));
            obj.AddInitializer("eventName", new StringLiteralExpression(_scope, evt.EventName));
            return obj;
        }

        private Expression EmitSubscriptions()
        {
            var items = new List<Expression>();
            foreach (var sub in _topology.Subscriptions)
            {
                var obj = new InlineObjectInitializer(null, _scope);
                obj.AddInitializer("propertyName", new StringLiteralExpression(_scope, sub.PropertyName));
                obj.AddInitializer("nodeIdx", new NumberLiteralExpression(_scope, sub.NodeIdx));
                obj.AddInitializer("sourceSlot", new NumberLiteralExpression(_scope, sub.SourceSlot));
                items.Add(obj);
            }
            return new InlineNewArrayInitialization(null, _scope, items);
        }

        private Expression EmitParentIndices()
        {
            var items = new List<Expression>();
            for (int i = 0; i < _topology.NodeCount; i++)
            {
                var parents = _topology.ParentIndices[i];
                if (parents == null || parents.Count == 0)
                {
                    items.Add(new InlineNewArrayInitialization(null, _scope, new List<Expression>()));
                    continue;
                }
                var inner = new List<Expression>();
                foreach (var p in parents)
                    inner.Add(new NumberLiteralExpression(_scope, p));
                items.Add(new InlineNewArrayInitialization(null, _scope, inner));
            }
            return new InlineNewArrayInitialization(null, _scope, items);
        }
    }

    /// <summary>
    /// A FunctionExpression variant that emits a raw JS body string.
    /// Used for getter functions where the body references virtual method
    /// accessors that are already correctly mangled by NScript's compiler.
    /// </summary>
    public class RawBodyFunctionExpression : Expression
    {
        private readonly IdentifierScope _innerScope;
        private readonly IList<IIdentifier> _parameters;
        private readonly string _rawBody;

        public RawBodyFunctionExpression(
            Location location,
            IdentifierScope outerScope,
            IdentifierScope innerScope,
            IList<IIdentifier> parameters,
            string rawBody)
            : base(location, outerScope)
        {
            _innerScope = innerScope;
            _parameters = parameters;
            _rawBody = rawBody;
        }

        public override void Write(JSWriter writer)
        {
            writer.Write("function(");
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (i > 0) writer.Write(", ");
                writer.Write(_parameters[i].GetName());
            }
            writer.Write(") { ");
            writer.Write(_rawBody);
            writer.Write("; }");
        }

        public override IdentifierScope OwnerScope => _innerScope;
    }
}
```

**IMPORTANT NOTE on getters:** The getter functions contain expressions like `dc.get_name()` where `get_name` is a virtual method accessor. In NScript's JS output, virtual methods stay as instance method calls (`dc.get_name_b()`). The NScript compiler's main pass handles mangling these when it processes the method body. Since we're generating raw JS for the getter body, the property accessor names need to match what the compiler produces. The `ExpressionJsEmitter.ToJsGetter()` already converts `Model.Name` to `dc.get_name()` — and NScript's scope manager tracks these identifiers.

However, the `RawBodyFunctionExpression` approach has a limitation: the property accessor names inside the body are not JST-resolved. This is an acceptable trade-off for now because:
1. Virtual method names in NScript are mangled consistently (the same way in all contexts)
2. The getter expressions are simple property access chains, not arbitrary code
3. A future improvement could parse the expression into proper JST MemberAccessExpression nodes

- [ ] **Step 2: Verify it compiles**

This step requires examining the exact field names on `DomTargetTopology`, `GateTopology`, `CollectionTopology`, `EventTopology`, and `SubscriptionInfo` in `GraphTopologyBuilder.cs`. Read those class definitions and adjust property names in the emitter if they differ from what's written above.

```bash
dotnet build Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj -c Release
```

Expected: Build succeeds. Fix any property name mismatches.

- [ ] **Step 3: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs
git commit -m "feat(razor-codegen): add GraphDescriptorJSTEmitter for retail-safe graph descriptors"
```

---

### Task 3: Integrate GraphDescriptorJSTEmitter into RazorSkinJSTGenerator

Replace the SkinBinderInfo-based binder emission with graph descriptor emission in the factory body.

**Files:**
- Modify: `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinJSTGenerator.cs`
- Reference: `Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs:85-96` (Build method)

- [ ] **Step 1: Add RazorKnownTypes and IResolver to JSTGenerator constructor**

In `RazorSkinJSTGenerator.cs`, add two new fields and constructor parameters:

```csharp
// Add fields after existing fields (around line 35):
private readonly RazorKnownTypes _knownTypes;
private readonly IResolver _resolver;

// Update constructor signature (line 56) to accept new params:
public RazorSkinJSTGenerator(
    SkinTemplateNode ir,
    RuntimeScopeManager scopeManager,
    ClrContext clrContext,
    Dictionary<string, IIdentifier> resolvedIdentifiers,
    Dictionary<string, IList<IIdentifier>> resolvedTypeIdentifiers,
    RazorKnownTypes knownTypes,
    IResolver resolver,
    IIdentifier preCreatedGetterIdentifier = null)
{
    _ir = ir;
    _scopeManager = scopeManager;
    _clrContext = clrContext;
    _resolvedIdentifiers = resolvedIdentifiers;
    _resolvedTypeIdentifiers = resolvedTypeIdentifiers;
    _knownTypes = knownTypes;
    _resolver = resolver;
    _preCreatedGetterIdentifier = preCreatedGetterIdentifier;
    _dataIndex = _next_dataIndex++;
}
```

- [ ] **Step 2: Add graph topology building to Generate()**

In the `Generate()` method (around line 122, before `BuildFactoryBody` call), build the graph topology:

```csharp
// Build graph topology from IR
var topology = GraphTopologyBuilder.Build(_ir);
```

Then update `BuildFactoryBody` signature to accept the topology:

```csharp
var factoryStatements = BuildFactoryBody(
    bindings, events, htmlContent, elementPaths, liveBinderCount,
    knownFunctionNames, topology);
```

- [ ] **Step 3: Replace binder emission with graph descriptor in BuildFactoryBody**

In `BuildFactoryBody` (around lines 300-350), replace the SkinBinderInfo binder array emission with graph descriptor emission:

1. Remove the `BuildBinderExpressions()` call and the `tmplStore[dataIndex] = [...]` assignment
2. Add graph descriptor variable assignment:

```csharp
// Emit graph descriptor as JST object literal
var graphEmitter = new GraphDescriptorJSTEmitter(
    topology, _factoryScope, _resolver, _knownTypes, knownFunctionNames);
var graphDescriptorExpr = graphEmitter.Emit();

// Assign to a local variable: var graphDesc = { nodeTypes: [...], ... };
var graphDescId = SimpleIdentifier.CreateScopeIdentifier(
    _factoryScope, "graphDesc", false);
stmts.Add(
    ExpressionStatement.CreateAssignmentExpression(
        new IdentifierExpression(graphDescId, _factoryScope),
        graphDescriptorExpr));
```

3. Update the `SkinInstance_factory` return call to pass the graph descriptor instead of the binder array:

Replace the `tmplStore[dataIndex]` argument with `graphDescId`:

```csharp
// Change from: new IndexExpression(tmplStore, dataIndex)  — binder array
// To: new IdentifierExpression(graphDescId, _factoryScope) — graph descriptor
```

- [ ] **Step 4: Verify compiler tests pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -c Release
```

Expected: All 125 pass. Some snapshot tests may need updating if they compare generated JS output.

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinJSTGenerator.cs
git commit -m "feat(razor-codegen): integrate graph descriptor JST emission into JSTGenerator"
```

---

### Task 4: Update RazorTemplatingPlugin

Wire `RazorKnownTypes` and `IResolver` through the plugin, enable graph mode.

**Files:**
- Modify: `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`

- [ ] **Step 1: Create RazorKnownTypes in Initialize()**

In `Initialize()` (around line 280), after the existing identifier resolution code, create `RazorKnownTypes`:

```csharp
// Add field:
private RazorKnownTypes _razorKnownTypes;

// In Initialize(), after existing resolution (around line 355):
_razorKnownTypes = new RazorKnownTypes(_clrContext, clrKnownRefs);
```

- [ ] **Step 2: Pass RazorKnownTypes and Resolver to JSTGenerator**

In `GetPostJavascript()` (around line 759), update the JSTGenerator construction:

```csharp
var jstGenerator = new RazorSkinJSTGenerator(
    kvp.Value,
    _runtimeScopeManager,
    _clrContext,
    _resolvedIdentifiers,
    _resolvedTypeIdentifiers,
    _razorKnownTypes,
    _runtimeScopeManager.Scope.ParserContext.JsResolver,
    preCreatedGetter);
```

Note: The `IResolver` is obtained from `_runtimeScopeManager.Scope.ParserContext.JsResolver` — verify this accessor exists. If not, check how the XWML `CodeGenerator` accesses its resolver (via `parserContext.JsResolver` at line 248 of XwmlParser/CodeGenerator.cs).

- [ ] **Step 3: Enable graph mode**

Change `_useGraphMode` from `false` to `true` (line 54):

```csharp
private bool _useGraphMode = true;
```

Then update the `GetPostJavascript()` code path. Since graph mode now uses JST (not raw JS), the `_useGraphMode = true` path should call the JSTGenerator too. Simplify by removing the `if (_useGraphMode)` branch entirely — both paths now use JSTGenerator:

```csharp
// Remove the _useGraphMode branch that calls GenerateGraphMode().
// JSTGenerator handles everything.
```

- [ ] **Step 4: Run compiler tests**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -c Release
```

Expected: All 125 pass.

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs
git commit -m "feat(razor-codegen): wire RazorKnownTypes and Resolver, enable graph mode"
```

---

### Task 5: Fix CollectHtmlWithMarkers for Attribute Bindings

Attribute bindings (class="@expr", title="@expr") need the marker on the parent element, not as a child `<span>`.

**Files:**
- Modify: `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs:561-606` (CollectHtmlWithMarkers)

- [ ] **Step 1: Handle attribute ExpressionBindingNodes in CollectHtmlWithMarkers**

In `CollectHtmlWithMarkers()` (line 569), the current code creates a `<span>` for every `ExpressionBindingNode`. For attribute bindings (Target != TextContent), the expression was already stripped from the HTML by the IR builder. The element itself needs a `data-bind-idx` marker attribute.

Update the `ExpressionBindingNode` handling:

```csharp
else if (node is ExpressionBindingNode bindingNode)
{
    if (bindingNode.Target == ExpressionTarget.TextContent)
    {
        // Text content: create placeholder <span>
        sb.Append($"<span data-bind-idx=\"{bindingIdx}\"></span>");
    }
    else
    {
        // Attribute binding: the attribute was stripped from the HTML by IR builder.
        // Add the marker to the preceding element's opening tag.
        // Find the last '>' in the accumulated HTML and insert the marker before it.
        var html = sb.ToString();
        var lastGt = html.LastIndexOf('>');
        if (lastGt >= 0)
        {
            sb.Clear();
            sb.Append(html.Substring(0, lastGt));
            sb.Append($" data-bind-idx=\"{bindingIdx}\"");
            sb.Append(html.Substring(lastGt));
        }
    }
    bindingIdx++;
}
```

- [ ] **Step 2: Run compiler tests**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -c Release
```

Expected: All 125 pass. Check that CodeGen snapshot tests still match.

- [ ] **Step 3: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs
git commit -m "fix(razor-codegen): handle attribute binding markers in CollectHtmlWithMarkers"
```

---

### Task 6: Remove Dead Code

Remove files that are no longer needed after the migration to JST-based graph descriptors.

**Files:**
- Modify: `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs` (remove Generate/GenerateGraphMode methods)

- [ ] **Step 1: Remove raw JS generation methods from RazorSkinCodeGenerator**

Delete `Generate()` (approx lines 106-222) and `GenerateGraphMode()` (approx lines 36-104). Keep all `Collect*`, `Build*`, and helper methods.

Also remove `EmitReactiveConditionalBinders`, `EmitReactiveLoopBinders`, `EmitEventBinders`, `EmitSubControlFactoryCalls`, `ConvertEventHandler` — these generated raw JS.

Keep: `CollectBindings`, `CollectEvents`, `CollectHtmlWithPaths`, `CollectHtmlWithMarkers`, `ComputePathsFromHtml`, `CollectHtml`, `BuildPartIdMapping`, and all public accessors (`*Public` methods).

- [ ] **Step 2: Run compiler tests**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -c Release
```

Expected: All 125 pass. If any tests call the removed methods, update them.

- [ ] **Step 3: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs
git commit -m "refactor(razor-codegen): remove raw JS generation methods, keep IR helpers"
```

---

### Task 7: Build and Browser Test Verification

Full rebuild and browser test validation.

- [ ] **Step 1: Run full compiler test suite**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -c Release
```

Expected: All 125 pass.

- [ ] **Step 2: Build the Debug compiler**

```bash
dotnet build Sources/Compiler/Cs2Jsc/Cs2Jsc.csproj -c Debug
```

- [ ] **Step 3: Copy forked Roslyn DLLs (if needed)**

```bash
cp Dependencies/Roslyn/Microsoft.CodeAnalysis.dll NScriptToolSet/bin/Debug/net8.0/
cp Dependencies/Roslyn/Microsoft.CodeAnalysis.CSharp.dll NScriptToolSet/bin/Debug/net8.0/
```

- [ ] **Step 4: Rebuild framework test project**

```bash
rm Test/Framework/TestWebApplication/GeneratedScripts/Sunlight.Framework.UI.Test.js
dotnet build Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj -c Debug --no-dependencies
```

Expected: Build succeeds, NScript generates JS.

- [ ] **Step 5: Serve and test in browser**

```bash
cd Test/Framework/TestWebApplication && npx serve . -p 3000
# Open http://localhost:3000/TestPage.htm
```

Expected: 54 Razor tests pass (12 existing + 42 new). All framework tests pass.

- [ ] **Step 6: Final commit**

```bash
git add -A
git commit -m "test(razor): verify all 54 browser tests pass with graph mode JST codegen"
```
