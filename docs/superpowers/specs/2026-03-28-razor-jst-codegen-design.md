# Razor JST Code Generation Extension — Design Spec

## Purpose

Extend `RazorSkinJSTGenerator` to emit graph descriptors as proper JST nodes with full identifier resolution via `Resolver`. All reactivity goes through the graph binding system (`GraphDescriptor` + `GraphEngine` + `GraphBindingStrategy`), not `SkinBinderInfo` (which is XWML-only).

## Current State

**Working (JSTGenerator path, `_useGraphMode = false`)**:
- Factory function, getter function, tmplStore as proper JST
- Text content bindings via `SkinBinderInfo` (legacy, only for existing 12 tests)
- Element path computation, identifier resolution

**Working but raw JS (`_useGraphMode = true`, GenerateGraphMode)**:
- Graph topology building (`GraphTopologyBuilder`)
- Graph descriptor emission (`GraphDescriptorEmitter`) — all binding types
- But outputs raw JS strings with unresolved function names — breaks in retail

**Not working**:
- Combining both: graph descriptors emitted as JST with resolved identifiers

## Design Principles

1. **Graph mode is the target** — all reactivity through `GraphDescriptor`/`GraphEngine`/`GraphBindingStrategy`
2. **SkinBinderInfo is XWML-only** — don't build more on it for Razor templates
3. **Never bypass JST** — all identifiers resolved via `MethodReference` → `Resolver`
4. **Virtual vs non-virtual** — virtual methods are instance calls, non-virtual become static with instance as first arg
5. **GraphDescriptor and sub-types are JSON-shaped** — no constructors, just field assignment object literals

## Architecture

### Core Change: Graph Descriptor as JST Object Literal

The `GraphDescriptorEmitter` currently outputs:

```javascript
// Raw JS string — function references are bare names (BROKEN in retail)
var Template_graph = {
  nodeTypes: [0, 1, 3],
  getters: [null, function(dc) { return dc.get_name(); }, null],
  targetInfos: [null, null, {elem: 0, set: SetTextContent}],  // ← bare name!
  ...
};
```

The fix: emit this as a JST `ObjectLiteralExpression` where `SetTextContent` is a resolved `IdentifierExpression`:

```javascript
// JST output — function references are resolved identifiers (retail-safe)
var Template_graph = {
  nodeTypes: [0, 1, 3],
  getters: [null, function(dc) { return dc.get_name_b(); }, null],
  targetInfos: [null, null, {elem: 0, set: SkinBinderHelper__SetTextContent_cp}],  // ← resolved!
  ...
};
```

### New: `GraphDescriptorJSTEmitter`

Replaces `GraphDescriptorEmitter` (raw JS strings) with JST node emission:

- Input: `GraphTopology` + `IResolver` + `RazorKnownTypes`
- Output: JST `ObjectLiteralExpression` representing the graph descriptor
- Each function reference (getters, setters in targetInfos) becomes a resolved `IdentifierExpression`
- Nested objects (`DomTargetInfo`, `GateTargetInfo`, etc.) are JST object literals
- Arrays are `InlineNewArrayInitialization` nodes

Key fields requiring identifier resolution:

| Field | Contains | Resolution |
| --- | --- | --- |
| `getters[i]` | `function(dc) { return dc.get_propName(); }` | Property getter is virtual → `Resolver.Resolve(getMethod)` for the member name |
| `targetInfos[i].set` | `SetTextContent` / `SetAttribute` | Static method → `Resolver.ResolveStaticMember(methodRef)` |
| `subscriptions[i]` | Property name strings | String literals (no resolution needed) |
| `sourceType` | Type reference | `Resolver.Resolve(typeRef)` |

### New: `RazorKnownTypes`

Resolves all runtime `MethodReference`/`TypeReference` needed for graph descriptor emission:

```
RazorKnownTypes
├── Setter MethodReferences (static on SkinBinderHelper)
│   ├── SetTextContent(Element, string)
│   ├── SetAttribute(Node, string, string)
│   └── SetCssClass(Element, bool, string)
├── Helper MethodReferences (static)
│   ├── GetElementFromPath(Element, int[])
│   ├── DocStorageGetter(Document)
│   └── GraphEngine.PushInitialValues(...)
├── Constructor MethodReferences
│   ├── SkinInstance constructor (graph overload)
│   └── Skin constructor
├── Type references
│   ├── UISkinableElement
│   └── GraphDescriptor, GraphState
└── Element.Bind / Element.UnBind (for event wiring)
```

Created in `RazorTemplatingPlugin.Initialize()` from `ClrContext`. Passed to `RazorSkinJSTGenerator` and `GraphDescriptorJSTEmitter`.

### Modified: `RazorSkinJSTGenerator`

- Receives `RazorKnownTypes` + `IResolver`
- `BuildFactoryBody` generates:
  1. DOM template initialization (existing — works)
  2. Graph descriptor as JST object literal (NEW — via `GraphDescriptorJSTEmitter`)
  3. Element references in objStorage (existing — works)
  4. `return SkinInstance_factory(...)` passing graph descriptor instead of binder array
- Remove dependency on `BinderEmitter` (SkinBinderInfo is XWML-only)
- Enable `_useGraphMode = true` in plugin once JST emission is ready

### Modified: `RazorSkinCodeGenerator`

- Remove `Generate()` — replaced by JSTGenerator
- Remove `GenerateGraphMode()` — replaced by JSTGenerator + GraphDescriptorJSTEmitter
- Keep `CollectHtmlWithPaths`, `CollectBindings`, `CollectEvents` helpers
- Fix `CollectHtmlWithMarkers` for attribute bindings

### Modified: `RazorTemplatingPlugin`

- Create `RazorKnownTypes` in `Initialize()`
- Pass `RazorKnownTypes` + `Resolver` to JSTGenerator
- Remove `_resolvedIdentifiers` string dictionary
- Set `_useGraphMode = true` (graph mode becomes the only path)

### Removed (dead code after migration)

- `BinderEmitter.cs` — SkinBinderInfo string emission (XWML-only)
- `GraphDescriptorEmitter.cs` — raw JS string emission (replaced by JST)
- `RawJavaScriptStatement.cs` / `ResolvedJavaScriptStatement.cs` — raw JS wrappers

## Graph Descriptor Field Details

### Getters

Each getter is a `FunctionExpression` whose body accesses a property on the DataContext/TemplateParent. Property access must use the proper resolved identifier:

- Observable property `Name` → `dc.get_name()` (virtual getter, resolved via `Resolver.Resolve(getMethod)`)
- Computed expression `Price * Quantity` → `dc.get_price() * dc.get_quantity()`

### TargetInfos

Per-node target info as JST object literals:

**DomTarget** (`{elem: idx, set: setter}`):
- `elem`: Number literal (element index in objStorage)
- `set`: Resolved identifier for `SetTextContent` / `SetAttribute`
- For `SetAttribute`: additional `arg` field with attribute name string

**Gate** (`{marker: idx, trueTemplate: html, falseTemplate: html, ...}`):
- `marker`: Element index for the anchor node
- `trueTemplate` / `falseTemplate`: HTML strings (the gate DOM operations clone these at runtime)

**CollectionManager** (`{marker: idx, itemGraph: descriptor, itemTemplate: html}`):
- `itemGraph`: Nested `GraphDescriptor` for each collection item
- `itemTemplate`: HTML string for the item template

**EventBinding** (`{elem: idx, eventName: name}`):
- `elem`: Element index
- `eventName`: String literal ("click", "change", etc.)

### Subscriptions

Array of `{propertyName: "Name", nodeIdx: 1, sourceSlot: 0}` — all string/number literals, no identifier resolution needed.

## Testing Strategy

The 42 browser tests validate each binding type end-to-end. The 125 compiler tests validate IR and topology building. No new test files needed — the existing tests ARE the validation.

## Success Criteria

1. All 42 new browser tests pass in QUnit
2. All 12 existing browser tests pass (no regressions)
3. All 125 compiler tests pass
4. No hardcoded identifier names — all resolved via Resolver
5. `_useGraphMode = true` as the only code path for Razor templates
6. Works in Release/retail builds (minified names)

## Implementation Order

1. `RazorKnownTypes` — resolve all needed MethodReferences
2. `GraphDescriptorJSTEmitter` — emit graph descriptor as JST object literal
3. Integrate into `RazorSkinJSTGenerator.BuildFactoryBody`
4. Update `RazorTemplatingPlugin` — wire everything, enable graph mode
5. Fix `CollectHtmlWithMarkers` for attribute bindings
6. Clean up dead code (BinderEmitter, GraphDescriptorEmitter, RawJavaScriptStatement)
7. Verify all tests pass
