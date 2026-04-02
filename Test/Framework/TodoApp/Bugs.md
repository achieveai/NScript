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

## BUG-004: CollectSpanElements collects both outer and inner spans (MEDIUM) — FIXED

**Component**: Compiler + Framework — `RazorSkinCodeGenerator`, `GraphEngine.CollectSpanElements()`
**Files**: `RazorSkinCodeGenerator.cs`, `GraphEngine.cs`
**Status**: FIXED (2026-03-30)

**Symptom**: When `<span class="named">@binding</span>` appears inside `@foreach` items, `getElementsByTagName("span")` collects both the outer `<span class="named">` and the inner binding placeholder `<span></span>`, causing elemIdx misalignment.

**Root Cause**: Flat span index addressing is brittle — any extra `<span>` in the DOM shifts all indices.

**Fix applied**: Two-part fix using `data-ns-ph` (placeholder) marker attributes:
1. **Compiler**: All compiler-generated placeholder spans now get `data-ns-ph` attribute: text binding spans in `CollectHtmlWithMarkers` and `CollectItemTemplateHtmlRecursive`, plus conditional/loop/sub-control placeholder spans.
2. **Runtime**: `CollectSpanElements` now uses `querySelectorAll("[data-ns-ph], [data-ns-evt], [data-ns-bind]")` to only collect compiler-generated markers, ignoring user-authored `<span>` elements. Falls back to legacy `getElementsByTagName("span")` for templates compiled before this change.

---

## BUG-005: @if gate for SelectedTodo != null doesn't re-render (MEDIUM) — CLOSED (WORKAROUND)

**Component**: Compiler/Framework — Gate dependency tracking
**Status**: CLOSED (2026-03-30) — workaround in place, gate mechanism verified working

**Symptom**: Right pane opens when a todo is selected (collapse class removed), but the detail content inside `@if (Model.SelectedTodo != null)` stays empty.

**Root Cause**: `ObservableAnalyzer.IsObservableProperty()` requires the property to have a **setter** (line 49: `return property.SetMethod != null`). If `SelectedTodo` has no setter, `IsObservableProperty()` returns false even though the containing class extends `ObservableObject`. Without observable classification, the gate condition stays `BindingMode.OneTime` instead of `OneWay` — no subscription is added to `GraphTopologyBuilder.ProcessConditional()`, so `PropertyChanged("SelectedTodo")` has no listener, the gate node is never re-evaluated, and the DOM is never updated.

**Resolution**: Replaced `@if (Model.SelectedTodo != null)` with CSS class binding (`class="@Model.DetailPaneContentClass"`) that toggles visibility via `display: none`. This is better UX — no DOM churn when toggling. The gate mechanism works for item templates where properties DO have setters (proven by star toggle test with 33/33 E2E tests passing).

**Proper fix** (future): Modify `ObservableAnalyzer.IsObservableProperty()` to also recognize properties that fire `PropertyChanged` manually in their setter, or special-case null-check conditions to always add subscriptions.

---

## BUG-006: Getter-only computed properties not emitted as functions (LOW) — CLOSED (BY DESIGN)

**Component**: Compiler — JS emission
**Status**: CLOSED (2026-03-30) — by design, use standard observable pattern

**Symptom**: Properties with only a getter (no backing field, no setter) don't emit a JS getter function. Templates referencing them get "X is not a function" runtime errors.

**Root Cause**: Getter-only computed properties don't auto-trigger method emission because the template binding system (`GraphDescriptorJSTEmitter`) is processed AFTER the tree-shaker's method emission pass. The getter method is never enqueued in `RuntimeScopeManager.usedMembersToProcess` because templates are the only consumer, and they run too late. When the binding code calls `_scopeManager.Resolve(property.GetMethod)`, the method body was never emitted to JS.

**Resolution**: Use the standard observable pattern: backing field + getter + setter + `FirePropertyChanged`. This ensures the property getter is referenced through normal code paths and gets emitted. Also ensures reactive updates propagate correctly.

**Proper fix** (future): In `RazorTemplatingPlugin.GetMethodsToEmitPassN()`, also walk property getters referenced by template bindings (similar to how event handler methods are already walked).

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

---

## BUG-010: Detail pane is non-functional — todo text not editable, subtasks not working (HIGH) — FIXED

**Component**: Application — `AppShell.skin.cshtml`, `AppViewModel.cs`
**Status**: FIXED (2026-03-29)

**Repro Steps**:
1. Select a ToDo item from the center pane
2. The detail pane (right pane) opens showing the item
3. The ToDo text appears but is NOT editable (rendered as `<div>` instead of `<input>`)
4. "Add step" button for subtasks does nothing — subtask functionality is not wired

**Expected**: Detail pane should allow editing the todo title (reflected back in the todo list), and adding/managing subtasks.

**Root Cause**: Detail pane was simplified during BUG-005 workaround (removed nested `@if` gates, switched to CSS class binding for visibility). The simplification left the title as a read-only `<div class="detail-title">@Model.DetailTitle</div>` instead of an editable `<input>`. Subtask collection and rendering were removed during this simplification and never restored.

**Fix needed**:
1. Change detail title from `<div>` to `<input type="text">` with `value="@Model.DetailTitle"` binding
2. Add an `onchange`/`onkeyup` handler to sync edited title back to `SelectedTodo.Title` and the todo list
3. Wire `AddSubTaskToSelected()` to create subtask items
4. Add subtask collection rendering (proxy `ObservableCollection` on `AppViewModel` or direct binding)

**Note**: The `value` attribute binding setter uses `setAttribute("value", v)` which doesn't update the displayed input value — needs `element.value = v` instead. This is tracked as BUG-011.

---

## BUG-011: value attribute binding uses setAttribute instead of DOM property (MEDIUM) — FIXED

**Component**: Compiler — `GraphDescriptorJSTEmitter.EmitDomTargetInfo()`
**File**: `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: `<input value="@Model.Property" />` binding doesn't update the displayed input value after user interaction. The initial value appears correct but reactive updates (e.g., selecting a different todo) don't visually change the input.

**Root Cause**: `EmitDomTargetInfo` for `ExpressionTarget.Attribute` emits `e.setAttribute("value", v)` for ALL attributes. For `<input>` elements, `setAttribute("value", x)` only updates the HTML attribute, NOT the `.value` DOM property that browsers actually render. After a user types in the input, the browser disconnects the attribute from the property — `setAttribute` no longer affects what's displayed.

**Fix applied**: Special-cased the `value` attribute in the `ExpressionTarget.Attribute` switch branch to emit `e.value = v || ""` (DOM property assignment) instead of `setAttribute("value", v)`. Similar to how `class` is already special-cased to `e.className`.

**Unit test**: Create a template with `<input value="@Model.Prop" />`, verify the generated setter uses `e.value =` not `setAttribute`.

---

## BUG-012: CollectionManager nodes have no PropertyChanged subscription (CRITICAL) — FIXED

**Component**: Compiler — `GraphTopologyBuilder.ProcessLoop()`
**File**: `Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs`
**Status**: FIXED (2026-03-29)

**Symptom**: When a collection property is reassigned (e.g., `DetailSubTasks = selectedTodo.SubTasks`), the `@foreach` template does not re-render. Items added to the new collection don't appear in the DOM.

**Root Cause**: `ProcessLoop()` creates a CollectionManager node and an edge from Source(0), but never calls `AddSubscription()` for the collection's property name. Compare with `ProcessBinding()` (line 193) and `ProcessConditional()` (line 288/304) which both add subscriptions. Without a subscription, `PropertyChanged("DetailSubTasks")` from the ViewModel has no listener to mark the CollectionManager node dirty. The Flush never evaluates it, so the old collection reference (with its old listener) stays active forever.

**Why Folders/CurrentTodos worked**: They were set BEFORE graph initialization. During `PushInitialValues`, the correct collection was already in place and `RenderCollection` attached the `CollectionChanged` listener. The collection reference never changed after that — only items were added/removed via the existing listener. The bug only manifests when a collection property is reassigned after initialization.

**Fix applied**: Added `AddSubscription(propName, collIdx, 0)` in `ProcessLoop()` after the edge creation. The property name is extracted from `loop.CollectionExpression` by stripping the `"Model."` prefix. Now when the collection property fires `PropertyChanged`, the subscription callback marks the CollectionManager node dirty, and the Flush detects the reference change, re-renders, and re-attaches the listener.

**Unit test**: Create a template with `@foreach (var x in Model.Items)`, reassign `Items` to a new collection after initialization, add an item to the new collection, verify it renders in the DOM.

---

## BUG-013: Selected todo has no visual highlight in the list (LOW) — FIXED

**Component**: Application — `AppShell.skin.cshtml`, `TodoItemViewModel.cs`
**Status**: FIXED (2026-03-30) — required BUG-017 fix first

**Repro Steps**:
1. Click a todo item in the center pane
2. The detail pane opens on the right showing the todo's details
3. But the clicked todo in the list has NO visual distinction — no background highlight, no border, nothing to indicate which item is selected

**Expected**: The selected todo should have a visible highlight (e.g., background color change) so the user knows which item corresponds to the detail pane.

**Root Cause**: `TodoItemViewModel` already has `IsSelected`, `CssClass`, and `CheckboxClass` properties. `AppViewModel.OnSelectTodo` already sets selection state. The problem is binding `class="@todo.CssClass"` on the root `<div>` of a `@foreach` item template — see BUG-017. Adding ANY class/attribute binding on a child element inside an item template shifts the span-based elemIdx values and breaks all subsequent bindings (title, gate, etc.).

**Blocked by**: BUG-017 (class/attribute binding on item template elements misaligns span-based elemIdx)

---

## BUG-014: Subtasks are not editable — no way to set or change subtask text (MEDIUM) — FIXED

**Component**: Application — `AppShell.skin.cshtml`, `SubTaskViewModel.cs`
**Status**: FIXED (2026-03-29)

**Repro Steps**:
1. Select a todo item to open the detail pane
2. Click "Add step" to create a subtask
3. A subtask appears with text "New step" — but it's a static `<div>`, not editable
4. There is no way to change the subtask text after creation
5. There is also no way to set the text BEFORE adding (like the main task input with Enter key)

**Expected**: Subtask title should be rendered as an editable `<input>` so the user can rename it at any time. Ideally, new subtasks should focus the input automatically so the user can immediately type the name.

**Fix needed**:
1. Change `<div class="subtask-title">@sub.Title</div>` to `<input type="text" class="subtask-title-input" value="@sub.Title" onchange="@sub.OnTitleChange" />` in the `@foreach` subtask item template
2. Add `OnTitleChange(Element e, ElementEvent ev)` handler to `SubTaskViewModel` that reads `InputElement.Value` and updates `Title`
3. Add `.subtask-title-input` CSS styling in `TodoApp.htm`
4. Relies on BUG-011 fix (`value` attribute binding uses `e.value` property)

---

## BUG-015: Folder todo count not computed on initial load (LOW) — FIXED

**Component**: Application — `AppViewModel.cs`
**Status**: FIXED (2026-03-29)

**Repro Steps**:
1. Open the app — system folders appear in the left pane with count pills
2. All folder counts show 0 (or stale values) even though the Tasks folder has 3 sample todos
3. Click on a folder — the count updates to the correct number
4. Navigate away and back — the count is correct only for the folder you clicked

**Expected**: All folder count pills should reflect the correct number of todos on initial load, without requiring a click.

**Root Cause**: `FolderViewModel.TodoCount` is only set inside `RefreshCurrentTodos()`, which runs when a folder is selected (`OnSelectFolder`). On initial load, only the Tasks folder is selected via `OnSelectFolder(defaultTasksFolder)`. The other system folders (My Day, Important, Planned) never get their counts computed until the user clicks them.

**Fix needed**:
1. After loading all todos (end of `LoadTodosFromJson` / `AddSampleTodos`), iterate through all folders and compute the count for each based on the filtering logic
2. Extract the per-folder counting logic from `RefreshCurrentTodos` into a reusable method (e.g., `ComputeFolderCount(FolderViewModel folder)`)
3. Call it for every folder after data load completes

---

## BUG-016: No way to change a todo's folder assignment (MEDIUM) — FIXED

**Component**: Application — missing feature in detail pane
**Status**: FIXED (2026-03-29)

**Repro Steps**:
1. Create a todo in the Tasks folder
2. Open its detail pane
3. There is no UI to move the todo to a different folder (e.g., assign it to a user-created list)

**Expected**: The detail pane should provide a way to change which folder a todo belongs to — e.g., a folder dropdown/selector, or a "Move to..." button.

**Fix needed**:
1. Add a folder selector section in the detail pane (could be a simple list of folder names with click handlers)
2. Add `MoveTodoToFolder(FolderViewModel targetFolder)` method on `AppViewModel` that updates `selectedTodo.FolderId`, refreshes the current view, and persists
3. Update folder counts after the move

---

## BUG-017: Class/attribute binding on @foreach item template elements misaligns elemIdx (CRITICAL) — FIXED

**Component**: Compiler + Framework — `CollectItemTemplateHtmlPublic`, `GraphEngine.ResolveEventElements`
**File**: `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs`, `Sources/Framework/Sunlight.Framework.UI/Helpers/BindingGraph/GraphEngine.cs`
**Status**: FIXED (2026-03-30)

**Symptom**: Adding `class="@todo.CssClass"` on a `<div>` inside a `@foreach` item template causes ALL subsequent bindings to write to wrong DOM elements.

**Root Cause**: `CollectSpanElements` only finds `<span>` elements. Class/attribute bindings on `<div>` elements had no span marker in the item template HTML, so their elemIdx pointed to the wrong span.

**Fix applied**: Used the same pattern as event resolution (BUG-003):
1. **Compiler**: `CollectItemTemplateHtmlRecursive` now inserts `<span data-ns-bind></span>` marker spans inside elements with class/style/attribute bindings (new `pendingBindMarkers` counter, parallel to `pendingEvtMarkers`)
2. **Runtime**: `ResolveEventElements` extended to also find `[data-ns-bind]` markers and replace them in ElemRefs with `span.parentNode`

The marker spans get collected by `getElementsByTagName("span")` at the correct elemIdx, then resolved to the actual target element at runtime.

**Unit test**: Create an item template with `class="@item.CssClass"` and `@item.Title` text binding, verify both render correctly.

---

## BUG-018: Add Task and Add Step should share a control pattern (LOW) — CLOSED (DEFERRED)

**Component**: Application — template design
**Status**: CLOSED (2026-03-30) — deferred until framework supports template composition

**Symptom**: "Add a task" (center pane) and "Add step" (detail pane) are duplicated patterns — both are input + Enter + icon. They should be a single reusable control/template to avoid duplication.

**Resolution**: NScript RazorTemplates don't currently support partial templates or reusable sub-components beyond `@foreach` item templates. The duplication is minimal (2 instances) and acceptable for a demo app. This will be revisited when the framework adds template composition / partial template support.

---

## BUG-019: Drag and drop todo to folder (MEDIUM) — FIXED

**Component**: Compiler + Application
**Files**: `TemplateIRBuilder.cs`, `TodoItemViewModel.cs`, `FolderViewModel.cs`, `AppViewModel.cs`, `AppShell.skin.cshtml`
**Status**: FIXED (2026-03-30)

**Symptom**: No drag-and-drop support for moving todos between folders.

**Fix applied**:
1. **Compiler**: Added drag events (`ondragstart`, `ondragend`, `ondragover`, `ondragenter`, `ondragleave`, `ondrop`) to `EventAttributes` HashSet in `TemplateIRBuilder.cs`
2. **TodoItemViewModel**: Added `OnDragStart(Element, ElementEvent)` — sets todo ID in `DataTransfer`
3. **FolderViewModel**: Added `OnDragOver` (PreventDefault), `OnDragEnter`/`OnDragLeave` (drag-over highlight), `OnDrop` (reads DataTransfer, calls DropTodoOnFolder)
4. **AppViewModel**: Added `DropTodoOnFolder(todoId, folder)` — smart behavior per folder type:
   - My Day → sets `IsMyDay = true`
   - Important → sets `IsImportant = true`
   - Completed → sets `IsCompleted = true`
   - Tasks/user folders → changes `FolderId`
5. **Template**: Added `draggable="true"` and `ondragstart` on todo items, `ondragover/ondragenter/ondragleave/ondrop` on folder items
6. **CSS**: Added `.folder-item.drag-over` style with dashed outline

---

## BUG-020: Completed folder shows items in completed section instead of main list (LOW) — FIXED

**Component**: Application — `AppViewModel.RefreshCurrentTodos()`
**Status**: FIXED (2026-03-30)

**Symptom**: When selecting the "Completed" system folder, all matched items (which are all completed) went into the `CompletedTodos` section, leaving the main list empty. The user had to expand the collapsible completed section to see them.

**Fix applied**: When `systemType == "completed"`, items go directly into `CurrentTodos` (main list). The completed section gets class `"completed-section hidden"` (`display: none`) to hide it entirely.

---
---

# Razor Template Compiler — Limitations & Future Work

Compiler-level limitations discovered during TodoApp development. These are gaps in the Razor template system that have workarounds but should be addressed for a production-ready compiler.

---

## LIMIT-001: Gate conditions on setter-less properties stay OneTime

**Component**: `ObservableAnalyzer.IsObservableProperty()` (line ~49)
**File**: `Sources/Compiler/RazorSkinParser/ObservableAnalyzer.cs`
**Discovered via**: BUG-005

**Problem**: `IsObservableProperty()` returns `false` for properties without a setter, even if the containing class extends `ObservableObject` and the property fires `PropertyChanged` manually. This means `@if (Model.Prop != null)` stays `BindingMode.OneTime` — the gate never re-evaluates when the property changes.

**Current workaround**: Use CSS class binding for visibility instead of `@if` gates, or ensure properties have setters.

**Fix**: Two options:
1. **Setter body analysis**: Walk the setter IL/AST to detect `FirePropertyChanged` calls (there's a TODO comment in the code for this)
2. **Conservative approach**: If the containing type is observable, treat ALL properties as potentially observable (may over-subscribe but won't miss updates)

**Files to modify**: `ObservableAnalyzer.cs` — `IsObservableProperty()` method

---

## LIMIT-002: Getter-only computed properties not emitted to JS

**Component**: Tree-shaker timing — `RazorTemplatingPlugin.GetMethodsToEmitPassN()`
**File**: `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`
**Discovered via**: BUG-006

**Problem**: Properties with only a getter (no backing field, no setter) don't get their getter method emitted to JS. Template bindings are processed in `GetPostJavascript()` which runs AFTER the tree-shaker's `WalkUsedDependencies()` loop. The getter method is never enqueued in `RuntimeScopeManager.usedMembersToProcess`.

**Current workaround**: Use backing field + getter + setter + `FirePropertyChanged` (standard observable pattern).

**Fix**: In `GetMethodsToEmitPassN()`, walk all template binding expressions and resolve property getters (not just event handler methods). This mirrors what's already done for `EventNode` handlers.

**Files to modify**: `RazorTemplatingPlugin.cs` — `GetMethodsToEmitPassN()`

---

## LIMIT-003: No `@for`, `@while`, or `@switch` support

**Component**: `TemplateIRBuilder` — directive parsing
**File**: `Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs`

**Problem**: Only `@if`/`@else` and `@foreach` are recognized as control flow directives. `@for`, `@while`, and `@switch` are silently ignored or cause parse errors.

**Current workaround**: Use `@foreach` with a pre-computed collection, or flatten switch logic into nested `@if`/`@else`.

**Fix**: Add IR node types and parsing for:
- `@for` → could emit as a `LoopNode` variant with index variable
- `@switch` → could emit as chained `ConditionalNode` gates
- `@while` → lower priority (rare in templates)

**Files to modify**: `TemplateIRBuilder.cs` (parsing), `IRNode.cs` (new node types), `GraphTopologyBuilder.cs` (graph wiring), `RazorSkinJSTGenerator.cs` (codegen)

---

## LIMIT-004: No two-way binding

**Component**: Binding system — `BindingClassification`
**File**: `Sources/Compiler/RazorSkinParser/TemplateIR/BindingClassification.cs`

**Problem**: Only `OneTime` and `OneWay` binding modes exist. There is no `TwoWay` mode where DOM changes (e.g., user typing in an `<input>`) automatically propagate back to the ViewModel property.

**Current workaround**: Use `onchange` or `onkeyup` event handlers that manually read `element.value` and set the ViewModel property.

**Fix**: Introduce `TwoWay` binding mode:
1. Detect `value="@Model.Prop"` on `<input>`/`<textarea>`/`<select>` elements
2. Auto-generate an `oninput` or `onchange` event handler that sets `Model.Prop = element.value`
3. Wire both the OneWay getter (VM → DOM) and the auto-generated setter (DOM → VM)

**Files to modify**: `BindingClassification.cs` (new mode), `TemplateIRBuilder.cs` (detect two-way contexts), `GraphDescriptorJSTEmitter.cs` (emit setter), `GraphEngine.cs` (wire DOM listener)

---

## LIMIT-005: Chained property paths not tracked for reactivity

**Component**: `GraphTopologyBuilder` + `GraphDescriptorJSTEmitter`
**Files**: `Sources/Compiler/RazorSkinParser/CodeGen/GraphTopologyBuilder.cs`, `GraphDescriptorJSTEmitter.cs`

**Problem**: `@Model.Customer.Address.City` only subscribes to `PropertyChanged("Customer")` on the root Model. If `Address.City` changes on an existing `Customer` object, the binding doesn't update. Multi-level property chains fall back to one-time evaluation for the inner segments.

**Current workaround**: Flatten nested properties to the root ViewModel (e.g., `Model.CustomerCity` with `AddLinkedProperty` wiring).

**Fix**: Implement chained property subscription:
1. For `Model.A.B.C`, subscribe to `Model.PropertyChanged("A")`, `A.PropertyChanged("B")`, and `B.PropertyChanged("C")`
2. When any intermediate object changes, re-subscribe to the new object's properties
3. This is the standard WPF/Avalonia `PropertyPath` pattern

**Files to modify**: `GraphTopologyBuilder.cs` (multi-level dependency extraction), `GraphDescriptorJSTEmitter.cs` (chained getter emission), `GraphEngine.cs` (runtime re-subscription logic)

---

## LIMIT-006: No partial templates / reusable sub-components

**Component**: Template composition
**Files**: `TemplateIRBuilder.cs`, `RazorSkinJSTGenerator.cs`
**Discovered via**: BUG-018

**Problem**: There's no way to define a reusable template fragment and use it in multiple places. PascalCase tags (e.g., `<MyWidget>`) are detected as `SubControlNode` in the IR but are not fully implemented in graph codegen (`GraphTopologyBuilder` has a stub comment: "Sub-controls could be expanded later").

**Current workaround**: Duplicate the HTML pattern in each template, or use `@foreach` with a single-item collection.

**Fix**: Full sub-component support:
1. Allow `@model SubType` on partial templates
2. Resolve `<MyWidget model="@item.SubProp" />` to a nested graph instantiation
3. The `SubControlNode` IR infrastructure is already in place — needs codegen completion

**Files to modify**: `GraphTopologyBuilder.cs` (ProcessSubControl expansion), `RazorSkinJSTGenerator.cs` (nested graph factory), `GraphDescriptorJSTEmitter.cs` (sub-graph descriptor emission)

---

## LIMIT-007: `@else if` requires workaround syntax

**Component**: `TemplateIRBuilder` — conditional parsing
**File**: `Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs`

**Problem**: `@else if (condition)` is not parsed as a single construct. The parser supports `@else` containing a nested `@if`, but a direct `else if` expression on one line may not be handled cleanly. In practice, developers must structure as nested blocks.

**Current workaround**: Use `@else { @if (cond2) { ... } }` or flatten to CSS class binding with computed property.

**Fix**: Parse `else if` as a chained gate — emit as a sequence of `ConditionalNode` gates with mutual exclusion (only one branch active).

**Files to modify**: `TemplateIRBuilder.cs` (else-if detection), `IRNode.cs` (optional: ChainedConditionalNode), `GraphTopologyBuilder.cs` (chained gate wiring)

---

## LIMIT-008: Event attribute whitelist requires manual extension

**Component**: `TemplateIRBuilder.EventAttributes` HashSet
**File**: `Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs`
**Discovered via**: BUG-019

**Problem**: Event attributes are detected via a hardcoded `HashSet<string>`. Any new HTML event (e.g., `ondragstart`, `onpointerdown`, `onanimationend`) must be manually added to the set. Missing events are silently treated as regular attributes.

**Current workaround**: Add new events to the HashSet as needed (done for drag events in BUG-019).

**Fix**: Replace the whitelist with prefix-based detection: any attribute starting with `on` followed by a lowercase letter is an event. This matches the HTML spec behavior.

```csharp
// Instead of: EventAttributes.Contains(attrName)
// Use: attrName.StartsWith("on") && attrName.Length > 2 && char.IsLower(attrName[2])
```

**Files to modify**: `TemplateIRBuilder.cs` — replace HashSet lookup with prefix check

---

## LIMIT-009: No `value` DOM property awareness for other form elements

**Component**: `GraphDescriptorJSTEmitter.EmitDomTargetInfo()`
**File**: `Sources/Compiler/RazorSkinParser/CodeGen/GraphDescriptorJSTEmitter.cs`
**Discovered via**: BUG-011

**Problem**: The `value` attribute was special-cased to use `e.value =` instead of `setAttribute("value", ...)` for `<input>` elements. But other form elements have similar DOM property vs HTML attribute mismatches: `<select>` (`selectedIndex`, `value`), `<textarea>` (`value`), `<input type="checkbox">` (`checked`), `<progress>` (`value`).

**Current workaround**: Only `value` is handled. Other properties require manual event handler workarounds.

**Fix**: Maintain a map of element-tag → attribute → DOM property mappings:
- `input[value]` → `e.value` (already done)
- `input[checked]` → `e.checked` (boolean property)
- `select[value]` → `e.value`
- `textarea[value]` → `e.value`
- `*[disabled]` → `e.disabled` (boolean)
- `*[hidden]` → `e.hidden` (boolean)

**Files to modify**: `GraphDescriptorJSTEmitter.cs` — expand the attribute special-casing in `EmitDomTargetInfo()`

---

## LIMIT-010: No template-level error diagnostics

**Component**: Razor template compilation pipeline
**Files**: `RazorSkinCompiler.cs`, `TemplateIRBuilder.cs`

**Problem**: When a template has errors (unknown property, typo in binding expression, missing event handler method), the compiler either silently drops the binding or emits broken JS that fails at runtime. There are no compile-time diagnostics pointing to the `.skin.cshtml` file and line number.

**Current workaround**: Debug at runtime via browser console errors.

**Fix**: Emit MSBuild-compatible warnings/errors during template compilation:
- Unknown property on `@model` type → error with template file + line
- Event handler method not found on ViewModel → error
- Binding expression parse failure → warning with context
- Use Roslyn `Diagnostic` infrastructure or emit to `ILogger` with structured location info

**Files to modify**: `RazorSkinCompiler.cs` (diagnostic collection), `TemplateIRBuilder.cs` (source location tracking), `GraphDescriptorJSTEmitter.cs` (property/method validation)
