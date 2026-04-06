# Design: Sub-Control Template Inlining in @foreach Loops

**Date:** 2026-04-06  
**Status:** Proposed  
**ADR Reference:** Extends ADR-0013 (multi-frontend translation) and ADR-0006 (compiler pipeline)

## Problem

`AppShell.skin.cshtml` has near-identical todo item templates duplicated across two `@foreach` loops (active list, completed list). The user wants a reusable `<TodoItemControl />` sub-control that can be referenced in both loops, eliminating template duplication.

The NScript compiler already parses `<TodoItemControl />` tags into `SubControlNode` IR nodes (via `ExtractSubControlsFromHtml`), but code generation is stubbed out — `GraphTopologyBuilder` has `case SubControlNode: break;` and `RazorSkinCodeGenerator` emits only a placeholder span.

## Approach: Compile-Time Template Inlining

**Chosen over** runtime sub-control instantiation (too much risk — requires GraphEngine runtime changes in the framework project) and hybrid approaches (YAGNI).

At compile time, after all `.skin.cshtml` templates are compiled to IR, a new **inlining pass** replaces each `SubControlNode` with the referenced control's compiled template IR. The sub-control's binding expressions are rewritten to use the parent's loop variable name instead of `Model`.

This reuses 100% of the existing foreach/collection pipeline — no runtime changes needed.

## Architecture

### Pipeline Position

```
Initialize():
  1. Compile all .skin.cshtml templates to IR (existing)
  2. ──► NEW: InlineSubControlsPass() ◄──
     Walk each IR tree, find SubControlNodes, resolve to compiled IRs,
     inline children with expression rewriting
  3. Store final IRs in _compiledIRs

GetPostJavascript():
  4. Generate JST from (now-inlined) IRs (existing, unchanged)
```

### Data Flow

```
AppShell.skin.cshtml IR:
  LoopNode (foreach todo in Model.CurrentTodos)
    ItemTemplate:
      SubControlNode { TypeName = "TodoItemControl" }

TodoItemControl.skin.cshtml IR:
  SkinTemplateNode
    Children:
      HtmlNode("<div ")
      ExpressionBindingNode(class = "Model.CssClass")
      HtmlNode(" draggable=\"true\" ")
      EventNode(onclick = "Model.OnSelect")
      EventNode(ondragstart = "Model.OnDragStart")
      HtmlNode(">...")
      ...

After inlining:
  LoopNode (foreach todo in Model.CurrentTodos)
    ItemTemplate:
      HtmlNode("<div ")
      ExpressionBindingNode(class = "todo.CssClass")  ← rewritten
      HtmlNode(" draggable=\"true\" ")
      EventNode(onclick = "todo.OnSelect")             ← rewritten
      EventNode(ondragstart = "todo.OnDragStart")      ← rewritten
      HtmlNode(">...")
      ...
```

### Expression Rewriting

Sub-control templates use `Model.X` for their bindings. When inlined into a foreach body where the loop variable is `todo`, expressions are rewritten:

| Sub-control expression | Rewritten to    | Why                                        |
|------------------------|-----------------|---------------------------------------------|
| `Model.CssClass`       | `todo.CssClass`  | `Model.` → loop variable prefix             |
| `Model.OnSelect`       | `todo.OnSelect`  | Same rewriting for event handlers            |
| `"todo-title"`         | `"todo-title"`   | Static strings unchanged                     |

The `ItemVariablePrefix` stripping in `GraphTopologyBuilder.ProcessLoop` then converts `todo.CssClass` → `CssClass` in the item graph, which is the correct property path on the item's data context.

### Binding Classification Preservation

The sub-control's Roslyn analysis phase already classified bindings against `TodoItemViewModel` (e.g., `CssClass` is `OneWay` because it has `FirePropertyChanged`). These classifications are preserved during inlining since the item type IS `TodoItemViewModel`. No re-analysis needed.

## Implementation

### Files Modified

1. **`RazorTemplatingPlugin.cs`** — Add inlining pass (~80 lines)
   - `InlineSubControlsPass()` — iterates all compiled IRs, calls recursive walker
   - `InlineSubControlsRecursive(children, itemVarName)` — finds SubControlNodes, resolves, inlines
   - `ResolveSubControlIR(typeName)` — looks up `_compiledIRs[typeName]`
   - `CloneAndRewriteIRNodes(nodes, fromPrefix, toPrefix)` — deep-clones IR nodes with expression rewriting
   - Called from `Initialize()` after the template compilation loop

2. **`AppShell.skin.cshtml`** — Replace both inline foreach bodies with `<TodoItemControl />`

### Files NOT Modified (intentionally)

- **`GraphTopologyBuilder.cs`** — SubControlNodes are eliminated before graph building
- **`RazorSkinCodeGenerator.cs`** — Only standard IR nodes after inlining
- **`GraphEngine.cs`** — Runtime completely untouched
- **`IRNode.cs`** — Existing SubControlNode class sufficient
- **`TemplateIRBuilder.cs`** — Sub-control extraction already works

### IR Node Cloning

Each IR node type needs deep cloning:
- `HtmlNode` → copy `Content`
- `ExpressionBindingNode` → copy `Classification` (deep), `Target`
- `EventNode` → copy `DomEventName`, `HandlerExpression`, `IsLambda`
- `ConditionalNode` → clone `TrueChildren`, `FalseChildren`, `Condition`
- `LoopNode` → clone `ItemTemplate`, rewrite `CollectionExpression`

Expression rewriting applies to:
- `ExpressionBindingNode.Classification.CSharpExpression`
- `ExpressionBindingNode.Classification.Dependencies[*].PropertyPath`
- `EventNode.HandlerExpression`
- `ConditionalNode.Condition`
- `LoopNode.CollectionExpression`

### Resolution Strategy

`SubControlNode.TypeName` (e.g., "TodoItemControl") is matched against `_compiledIRs` keys. Templates are registered under their short name (e.g., "TodoItemControl" from filename "TodoItemControl.skin.cshtml"). If no match, the SubControlNode is left as-is (existing no-op behavior).

### Top-Level Sub-Controls (Non-Foreach)

Sub-controls at the top level (not inside a foreach) use `Model.` as both source and target prefix — no rewriting needed. The inlined bindings reference `Model.X` which is already the parent's model. This is handled by passing `null` as `itemVariableName` and defaulting the target prefix to `Model`.

## Constraints & Limitations

1. **No runtime component boundary** — the sub-control class is never instantiated. No lifecycle hooks (OnActivate, etc.). This is template reuse, not component composition.
2. **No property pass-through** — `<TodoItemControl SomeExtraProp="value" />` property bindings on the sub-control tag are ignored (only the template is inlined).
3. **Single level** — nested sub-controls (a sub-control using another sub-control) would need recursive inlining. Supported by the recursive walker but untested.
4. **CSS sharing** — sub-control template must reference the same `@styles` as the parent (or inherit parent's CSS manager, which the plugin already handles at line 1314-1317).

## Testing Strategy

1. **Unit test**: Build IR with SubControlNode inside LoopNode.ItemTemplate, run inlining pass, verify SubControlNode replaced with correct IR nodes and expressions rewritten
2. **Integration test**: Compile TodoApp with `<TodoItemControl />` in both foreach loops, verify generated JS matches hand-inlined version
3. **E2E tests**: All 37 existing tests pass unchanged — the DOM structure should be identical

## Future Upgrade Path

This approach can be upgraded to runtime instantiation (Approach B) later:
- Instead of inlining, emit `SubControlInfo` in the graph descriptor
- `GraphEngine.RenderCollectionItems` creates control instances
- The template syntax (`<TodoItemControl />`) remains identical — no user-facing change
