# TodoApp — Known Bugs in Razor Template System

Bugs discovered while building the Microsoft To Do clone SPA.
These are all **compiler-level or framework-level bugs** in the Razor template system, not application bugs.

---

## BUG-001: Event bindings inside @foreach are silently dropped (CRITICAL) — FIXED

**Component**: Compiler — `TemplateIRBuilder.ParseForeachBlock()`
**File**: `Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: `onclick="@folder.OnSelect"` on elements inside `@foreach` item templates produces no EventNode in the IR. The generated item graph has zero event handler nodes.

**Root Cause**: `ParseForeachBlock` (lines 540-553) only handles `HtmlContentIntermediateNode` and `CSharpExpressionIntermediateNode`. It completely ignores `HtmlAttributeIntermediateNode`, which is how Razor structures `onclick="@expr"` bindings.

**Fix applied**: Added `HtmlAttributeIntermediateNode` handling to `ParseForeachBlock` — detects `on*` attributes as events, other attributes as expression bindings. Added `lastHtmlContent` tracking and `DetectEventAttributeContext` for inline expressions.

**Unit test**: Compile a Razor template with `onclick` inside `@foreach`, verify the item graph descriptor has EventBinding nodes with correct getters and EventTargetInfo.

---

## BUG-002: Outer template event elements resolve to wrong DOM path (CRITICAL) — FIXED

**Component**: Compiler — `RazorSkinJSTGenerator.FindNthInteractiveElementPath()`
**Files**: `RazorSkinCodeGenerator.cs`, `RazorSkinJSTGenerator.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: Events on `<div>` elements (like `<div class="btn-toggle-left" onclick="@Model.ToggleLeftPane">`) are attached to the wrong DOM element. Clicking anywhere fires AddTodo instead of the expected handler.

**Root Cause**: `FindNthInteractiveElementPath` (line 978) only looks for `button`, `a`, `input` elements. `<div>` elements are never matched, so the function falls back to `return new List<int> { 0 }` (root element). ALL events get path `[0]` and attach to the same element.

**Fix applied**: Added `data-evt-idx` markers to `CollectHtmlWithMarkers` for EventNode objects. Extended `ComputePathsFromHtml` to also find `data-evt-idx` markers and compute DOM paths. Factory body now uses pre-computed event paths instead of `FindNthInteractiveElementPath`.

**Unit test**: Compile a template with onclick on `<div>`, `<span>`, `<button>` elements, verify each event gets a distinct correct DOM path in the factory body.

---

## BUG-003: Item template event elements not resolved at runtime (HIGH) — FIXED

**Component**: Compiler + Framework
**Files**: `RazorSkinCodeGenerator.cs`, `GraphEngine.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: Even after fixing BUG-001, events in `@foreach` item templates would not attach because the runtime can't find the event target element.

**Root Cause**: `CollectSpanElements` (line 1151) uses `getElementsByTagName("span")` to build `ElemRefs`. Event target elements (like `<div class="folder-item">`) are not `<span>` elements, so they're not in `ElemRefs`. The `EventTargetInfo.ElemIdx` points to an undefined slot.

**Fix applied**: Two-part fix:
1. **Compiler**: `CollectItemTemplateHtmlPublic` inserts `<span data-ns-evt></span>` marker spans inside event target elements. These markers occupy the correct ElemIdx position in the span collection.
2. **Runtime**: `ResolveEventElements` finds `[data-ns-evt]` spans in `ElemRefs` and replaces them with `span.parentNode` (the actual event target element).

**Unit test**: Create an item template with onclick, render a collection, verify the event handler fires on the correct element.

---

## BUG-004: CollectSpanElements collects both outer and inner spans (MEDIUM)

**Component**: Framework — `GraphEngine.CollectSpanElements()`
**Workaround**: Use `<div>` instead of `<span>` for containers inside @foreach item templates

**Symptom**: When `<span class="named">@binding</span>` appears inside `@foreach` items, `getElementsByTagName("span")` collects both the outer `<span class="named">` and the inner binding placeholder `<span></span>`, causing elemIdx misalignment.

**Root Cause**: Flat span index addressing is brittle — any extra `<span>` in the DOM shifts all indices.

**Proper fix**: Either filter `CollectSpanElements` to only collect leaf/placeholder spans, or use DOM path arrays instead of flat span indices.

---

## BUG-005: @if gate for SelectedTodo != null doesn't re-render (MEDIUM)

**Component**: Compiler/Framework — Gate dependency tracking

**Symptom**: Right pane opens when a todo is selected (collapse class removed), but the detail content inside `@if (Model.SelectedTodo != null)` stays empty.

**Root Cause**: The gate condition depends on `SelectedTodo` changing from null to non-null. The property change notification for `SelectedTodo` may not propagate to the gate node, or the gate evaluation may not correctly handle null→non-null transitions.

**Investigation needed**: Check if SelectedTodo property fires FirePropertyChanged, check if the gate subscription is wired to "SelectedTodo" property name, check if gate evaluation handles the null→object transition.

---

## BUG-006: Getter-only computed properties not emitted as functions (LOW)

**Component**: Compiler — JS emission
**Workaround**: Always use full observable pattern (backing field + setter + FirePropertyChanged)

**Symptom**: Properties with only a getter (no backing field, no setter) don't emit a JS getter function. Templates referencing them get "X is not a function" runtime errors.

**Root Cause**: NScript's JS emitter doesn't create getter functions for properties without backing fields.

---

## BUG-008: Template-bound event methods tree-shaken (CRITICAL) — FIXED

**Component**: Compiler — `RazorTemplatingPlugin.GetMethodsToEmitPassN()`
**File**: `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: `dc_a.onSelect_F is not a function` at runtime. Methods like `FolderViewModel.OnSelect()`, `TodoItemViewModel.ToggleComplete()` have resolved identifiers in the graph descriptor but their method bodies are not emitted to JS.

**Root Cause**: `GetMethodsToEmitPassN()` returned an empty list. Template-bound methods are only referenced from JST graph descriptors emitted in `GetPostJavascript()`, which runs AFTER the tree-shaker's `WalkUsedDependencies()` loop. So `Resolve()` calls from `GetPostJavascript` add to the queue but the queue is never processed.

**Fix applied**: `GetMethodsToEmitPassN()` now walks all compiled IR templates, finds all `EventNode` objects (including inside `LoopNode` item templates and `ConditionalNode` branches), resolves handler expressions to `MethodReference` objects via Cecil type lookup (including chained property paths like `SelectedTodo.OnTitleChange`), and returns them. The tree-shaker processes these during the do-while loop and emits the method bodies.

---

## BUG-009: Event handlers don't receive element/event args (MEDIUM) — FIXED

**Component**: Compiler — `GraphDescriptorJSTEmitter.EmitEventGetter()`
**File**: `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: Event handler methods cannot access the DOM element or event object. Methods like `AddTodoOnEnter(Element e, ElementEvent ev)` that need to read `ev.KeyCode` or `((InputElement)e).Value` receive no arguments.

**Root Cause**: The generated event wrapper was `function(e, ev) { dc.method(); }` — args `e` and `ev` were available in scope but NOT passed to the method call.

**Fix applied**: Changed the method call to `function(e, ev) { dc.method(e, ev); }`. JS ignores extra args for methods that don't use them, so existing zero-parameter handlers continue to work.

**Unit test**: Create a handler method with `(Element, ElementEvent)` parameters, fire event, verify element and event are received.

---

**Component**: Compiler — `RazorTemplatingPlugin.GetMethodsToEmitPassN()`
**File**: `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: `dc_a.onSelect_F is not a function` at runtime. Methods like `FolderViewModel.OnSelect()`, `TodoItemViewModel.ToggleComplete()` have resolved identifiers in the graph descriptor but their method bodies are not emitted to JS.

**Root Cause**: `GetMethodsToEmitPassN()` returned an empty list. Template-bound methods are only referenced from JST graph descriptors emitted in `GetPostJavascript()`, which runs AFTER the tree-shaker's `WalkUsedDependencies()` loop. So `Resolve()` calls from `GetPostJavascript` add to the queue but the queue is never processed.

**Fix applied**: `GetMethodsToEmitPassN()` now walks all compiled IR templates, finds all `EventNode` objects (including inside `LoopNode` item templates and `ConditionalNode` branches), resolves handler expressions to `MethodReference` objects via Cecil type lookup, and returns them. The tree-shaker processes these during the do-while loop and emits the method bodies.

**Also needed**: `EmitEventGetter` in `GraphDescriptorJSTEmitter` strips the item variable prefix (e.g., `folder.`) from handler expressions before resolving, and passes the stripped expression to `TryResolveMethodIdentifier`.

---

## BUG-007: DocStorageGetter conflict with dual plugins (LOW)

**Component**: Compiler — `RazorTemplatingPlugin`
**Workaround**: Use only one plugin per project (Razor OR XWML, not both)

**Symptom**: When both RazorTemplatingPlugin and XwmlTemplatingPlugin are loaded, two DocStorageGetter functions are emitted with different minified names.

**Fix applied**: RazorTemplatingPlugin creates its own DocStorageGetter when XWML hasn't provided one.
